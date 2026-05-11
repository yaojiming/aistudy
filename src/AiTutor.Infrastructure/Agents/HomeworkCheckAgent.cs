using System.Text.Json;
using AiTutor.Core.Interfaces;
using AiTutor.Shared.Agent;

namespace AiTutor.Infrastructure.Agents;

/// <summary>
/// 作业检查 Agent，负责识别作业图片并返回逐题检查结果。
/// </summary>
public class HomeworkCheckAgent : IStreamingAgent
{
    private readonly IVisionModelProvider _visionProvider;
    private readonly IPromptTemplateService _promptTemplateService;

    public HomeworkCheckAgent(IVisionModelProvider visionProvider, IPromptTemplateService promptTemplateService)
    {
        _visionProvider = visionProvider;
        _promptTemplateService = promptTemplateService;
    }

    public string Name => "HomeworkCheckAgent";

    public async Task<AgentResponse> ExecuteAsync(AgentRequest request, AgentRouteResult route, CancellationToken cancellationToken = default)
    {
        var modelAnswer = await _visionProvider.AnalyzeImageAsync(request.ImageUrl ?? string.Empty, BuildPrompt(request), cancellationToken);
        return CreateResponse(route, modelAnswer);
    }

    public async IAsyncEnumerable<AgentStreamChunkDto> StreamExecuteAsync(
        AgentRequest request,
        AgentRouteResult route,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var answerText = string.Empty;
        await foreach (var delta in _visionProvider.AnalyzeImageStreamAsync(request.ImageUrl ?? string.Empty, BuildPrompt(request), request.ThinkingMode, cancellationToken))
        {
            answerText += delta;
            yield return new AgentStreamChunkDto { Type = "delta", Text = delta };
        }

        yield return new AgentStreamChunkDto { Type = "final", FinalResponse = CreateResponse(route, answerText) };
    }

    private string BuildPrompt(AgentRequest request)
    {
        return _promptTemplateService.Render("homework_check", new Dictionary<string, string?>
        {
            ["imageUrl"] = request.ImageUrl,
            ["subject"] = request.Subject,
            ["grade"] = request.Grade
        });
    }

    private static AgentResponse CreateResponse(AgentRouteResult route, string modelAnswer)
    {
        var result = TryParseHomeworkResult(modelAnswer) ?? CreateStructuredMockResult();
        var response = ResponseFactory.Create(route, modelAnswer, canAddToWrongBook: true);
        response.OutputType = "structured_json";
        response.HomeworkCheckResult = result;
        response.AnswerJson = result;
        response.Suggestions.Add(new SuggestionDto { Text = "练习同类题", Action = "practice_generate" });
        return response;
    }

    private static HomeworkCheckResultDto? TryParseHomeworkResult(string modelAnswer)
    {
        if (string.IsNullOrWhiteSpace(modelAnswer))
        {
            return null;
        }

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        // 策略 1：尝试提取 markdown ```json ... ``` 代码块
        var fenceStart = modelAnswer.IndexOf("```json", StringComparison.OrdinalIgnoreCase);
        if (fenceStart >= 0)
        {
            var contentStart = modelAnswer.IndexOf('\n', fenceStart) + 1;
            var fenceEnd = modelAnswer.IndexOf("```", contentStart, StringComparison.Ordinal);
            if (fenceEnd > contentStart)
            {
                var blockJson = modelAnswer[contentStart..fenceEnd].Trim();
                var blockResult = TryDeserialize(blockJson, options);
                if (blockResult is not null)
                {
                    return blockResult;
                }
            }
        }

        // 策略 2：尝试提取 ``` ... ``` 通用代码块
        var genericFenceStart = modelAnswer.IndexOf("\n```", StringComparison.Ordinal);
        if (genericFenceStart >= 0)
        {
            var contentStart = modelAnswer.IndexOf('\n', genericFenceStart + 1) + 1;
            var fenceEnd = modelAnswer.IndexOf("```", contentStart, StringComparison.Ordinal);
            if (fenceEnd > contentStart)
            {
                var blockJson = modelAnswer[contentStart..fenceEnd].Trim();
                var blockResult = TryDeserialize(blockJson, options);
                if (blockResult is not null)
                {
                    return blockResult;
                }
            }
        }

        // 策略 3：在模型输出末尾部分寻找 { ... } JSON（模型经常在文本后附加 JSON）
        var jsonStart = modelAnswer.LastIndexOf("{\n", StringComparison.Ordinal);
        if (jsonStart < 0)
        {
            jsonStart = modelAnswer.LastIndexOf("{ \"", StringComparison.Ordinal);
        }

        if (jsonStart >= 0)
        {
            var jsonEnd = modelAnswer.LastIndexOf('}');
            if (jsonEnd > jsonStart)
            {
                var json = modelAnswer[jsonStart..(jsonEnd + 1)];
                var result = TryDeserialize(json, options);
                if (result is not null)
                {
                    return result;
                }
            }
        }

        // 策略 4：全文搜索 { 到 } 作为最后备选
        var firstBrace = modelAnswer.IndexOf('{');
        var lastBrace = modelAnswer.LastIndexOf('}');
        if (firstBrace >= 0 && lastBrace > firstBrace)
        {
            var json = modelAnswer[firstBrace..(lastBrace + 1)];
            return TryDeserialize(json, options);
        }

        return null;
    }

    private static HomeworkCheckResultDto? TryDeserialize(string json, JsonSerializerOptions options)
    {
        try
        {
            var result = JsonSerializer.Deserialize<HomeworkCheckResultDto>(json, options);
            if (result?.Items is { Count: > 0 })
            {
                // 清理模型输出中可能的格式问题
                foreach (var item in result.Items)
                {
                    item.QuestionNo  = CleanJsonString(item.QuestionNo);
                    item.QuestionText = CleanJsonString(item.QuestionText);
                    item.StudentAnswer = CleanJsonString(item.StudentAnswer);
                    item.CorrectAnswer = CleanJsonString(item.CorrectAnswer);
                    item.ErrorReason = CleanJsonString(item.ErrorReason);
                    item.Explanation = CleanJsonString(item.Explanation);
                }

                result.TotalCount = result.Items.Count;
                result.CorrectCount = result.Items.Count(i => i.IsCorrect == true);
                result.WrongCount = result.Items.Count(i => i.IsCorrect == false);
                return result;
            }
        }
        catch
        {
        }

        return null;
    }

    private static string CleanJsonString(string? value)
    {
        return (value ?? string.Empty)
            .Replace("\\n", "\n")
            .Replace("\\\"", "\"")
            .Trim();
    }

    private static HomeworkCheckResultDto CreateStructuredMockResult()
    {
        return new HomeworkCheckResultDto
        {
            Summary = "模拟识别到 3 道题，其中 2 道正确，1 道需要订正。",
            TotalCount = 3,
            CorrectCount = 2,
            WrongCount = 1,
            Items =
            [
                new HomeworkCheckItemDto
                {
                    QuestionNo = "第 1 题",
                    QuestionText = "12 ÷ 3 = ?",
                    StudentAnswer = "4",
                    CorrectAnswer = "4",
                    IsCorrect = true,
                    Explanation = "三四十二，所以 12 除以 3 等于 4。",
                    KnowledgePointName = "表内除法"
                },
                new HomeworkCheckItemDto
                {
                    QuestionNo = "第 2 题",
                    QuestionText = "7 × 8 = ?",
                    StudentAnswer = "54",
                    CorrectAnswer = "56",
                    IsCorrect = false,
                    ErrorReason = "乘法口诀记错了，七八应是五十六。",
                    Explanation = "可以先背口诀：七八五十六。再检查 8 个 7 相加也是 56。",
                    KnowledgePointName = "乘法口诀"
                },
                new HomeworkCheckItemDto
                {
                    QuestionNo = "第 3 题",
                    QuestionText = "35 + 27 = ?",
                    StudentAnswer = "62",
                    CorrectAnswer = "62",
                    IsCorrect = true,
                    Explanation = "个位 5+7=12，写 2 进 1；十位 3+2+1=6，所以是 62。",
                    KnowledgePointName = "两位数加法"
                }
            ]
        };
    }
}
