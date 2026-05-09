using System.Net.Http.Json;
using System.Text.Json;
using AiTutor.Shared.Agent;

namespace AiTutor.Maui.Services;

public interface IApiClientService
{
    /// <summary>
    /// 调用普通非流式 Agent 接口，主要用于兼容和调试。
    /// </summary>
    Task<AgentResponse> AskAsync(AgentRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 调用后端 SSE 流式 Agent 接口，收到 delta 时立即回调给 ViewModel。
    /// </summary>
    Task<AgentResponse?> AskStreamAsync(
        AgentRequest request,
        Func<string, Task> onDelta,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 上传题目或作业图片到 AiTutor.Api，并返回媒体资源信息。
    /// </summary>
    Task<MediaUploadResultDto> UploadImageAsync(FileResult file, string resourceType, string userId, CancellationToken cancellationToken = default);
}

/// <summary>
/// MAUI 端统一 API 客户端。客户端只访问 AiTutor.Api，不保存模型 API Key。
/// </summary>
public class ApiClientService : IApiClientService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;
    private readonly IAppSettingsService _settingsService;

    public ApiClientService(HttpClient httpClient, IAppSettingsService settingsService)
    {
        _httpClient = httpClient;
        _settingsService = settingsService;
    }

    /// <summary>
    /// 调用 POST /api/agent/ask，等待完整 AgentResponse。
    /// </summary>
    /// <param name="request">统一 Agent 请求。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>完整 Agent 响应。</returns>
    public async Task<AgentResponse> AskAsync(AgentRequest request, CancellationToken cancellationToken = default)
    {
        await ConfigureHttpClientAsync(cancellationToken);

        try
        {
            using var response = await _httpClient.PostAsJsonAsync("/api/agent/ask", request, JsonOptions, cancellationToken);
            return await ReadResponseAsync<AgentResponse>(response, cancellationToken);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            throw new TimeoutException("请求超时了，请确认后端服务已启动。", ex);
        }
        catch (HttpRequestException ex)
        {
            throw new InvalidOperationException("无法连接到 AiTutor.Api，请检查后端地址和网络。", ex);
        }
    }

    /// <summary>
    /// 调用 POST /api/agent/ask-stream，并按 SSE data 行逐段读取模型输出。
    /// </summary>
    /// <param name="request">统一 Agent 请求。</param>
    /// <param name="onDelta">收到增量文本时执行的 UI 更新回调。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>流结束时的完整响应；异常时由统一错误处理抛出。</returns>
    public async Task<AgentResponse?> AskStreamAsync(
        AgentRequest request,
        Func<string, Task> onDelta,
        CancellationToken cancellationToken = default)
    {
        await ConfigureHttpClientAsync(cancellationToken);

        try
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/agent/ask-stream")
            {
                Content = JsonContent.Create(request, options: JsonOptions)
            };

            // ResponseHeadersRead 让客户端在响应头到达后立刻开始读流，不等待完整内容下载。
            using var response = await _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                var errorText = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new InvalidOperationException($"流式请求失败：{(int)response.StatusCode} {errorText}");
            }

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var reader = new StreamReader(stream);
            AgentResponse? finalResponse = null;

            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync(cancellationToken);
                if (string.IsNullOrWhiteSpace(line) || !line.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var json = line["data:".Length..].Trim();
                var chunk = JsonSerializer.Deserialize<AgentStreamChunkDto>(json, JsonOptions);
                if (chunk is null)
                {
                    continue;
                }

                if (chunk.Type == "delta" && !string.IsNullOrEmpty(chunk.Text))
                {
                    // 每个 delta 都立即交给 ViewModel，形成真正的逐段显示。
                    await onDelta(chunk.Text);
                }
                else if (chunk.Type == "final")
                {
                    finalResponse = chunk.FinalResponse;
                }
                else if (chunk.Type == "error")
                {
                    throw new InvalidOperationException(chunk.ErrorMessage ?? "流式输出失败。");
                }
            }

            return finalResponse;
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            throw new TimeoutException("流式请求超时了，请确认后端服务已启动。", ex);
        }
        catch (HttpRequestException ex)
        {
            throw new InvalidOperationException("无法连接到 AiTutor.Api 流式接口，请检查后端地址和网络。", ex);
        }
    }

    /// <summary>
    /// 上传图片到 POST /api/media/upload-image。
    /// </summary>
    /// <param name="file">MAUI MediaPicker 返回的图片文件。</param>
    /// <param name="resourceType">业务资源类型，例如 question_photo 或 homework_photo。</param>
    /// <param name="userId">当前学生用户 Id。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>后端媒体资源信息。</returns>
    public async Task<MediaUploadResultDto> UploadImageAsync(FileResult file, string resourceType, string userId, CancellationToken cancellationToken = default)
    {
        await ConfigureHttpClientAsync(cancellationToken);

        try
        {
            await using var stream = await file.OpenReadAsync();
            using var content = new MultipartFormDataContent();
            using var fileContent = new StreamContent(stream);
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType ?? "image/jpeg");
            content.Add(fileContent, "file", file.FileName);
            content.Add(new StringContent(resourceType), "resourceType");
            content.Add(new StringContent(userId), "userId");
            content.Add(new StringContent("maui_android_tablet"), "sourceType");

            using var response = await _httpClient.PostAsync("/api/media/upload-image", content, cancellationToken);
            return await ReadResponseAsync<MediaUploadResultDto>(response, cancellationToken);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            throw new TimeoutException("图片上传超时了，请稍后再试。", ex);
        }
        catch (HttpRequestException ex)
        {
            throw new InvalidOperationException("图片上传失败，请检查 AiTutor.Api 是否正在运行。", ex);
        }
    }

    /// <summary>
    /// 根据配置初始化 HttpClient 的 BaseAddress 和 Timeout。
    /// </summary>
    /// <param name="cancellationToken">取消令牌。</param>
    private async Task ConfigureHttpClientAsync(CancellationToken cancellationToken)
    {
        var options = await _settingsService.GetApiOptionsAsync(cancellationToken);
        _httpClient.BaseAddress ??= new Uri(options.BaseUrl);
        _httpClient.Timeout = TimeSpan.FromSeconds(Math.Clamp(options.TimeoutSeconds, 5, 120));
    }

    /// <summary>
    /// 统一解析普通 JSON 响应，并将 HTTP 错误转换为可展示异常。
    /// </summary>
    /// <typeparam name="T">响应 DTO 类型。</typeparam>
    /// <param name="response">HTTP 响应。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>反序列化后的 DTO。</returns>
    private static async Task<T> ReadResponseAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (!response.IsSuccessStatusCode)
        {
            var errorText = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException($"服务返回错误：{(int)response.StatusCode} {errorText}");
        }

        var result = await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken);
        return result ?? throw new InvalidOperationException("服务返回内容为空。");
    }
}
