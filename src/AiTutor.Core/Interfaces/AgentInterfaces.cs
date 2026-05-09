using AiTutor.Core.Entities;
using AiTutor.Shared.Agent;

namespace AiTutor.Core.Interfaces;

/// <summary>
/// Agent 网关服务，负责完整处理一次 AI 学习请求。
/// </summary>
public interface IAgentService
{
    Task<AgentResponse> AskAsync(AgentRequest request, CancellationToken cancellationToken = default);

    IAsyncEnumerable<AgentStreamChunkDto> StreamAskAsync(AgentRequest request, CancellationToken cancellationToken = default);
}

/// <summary>
/// Agent 路由器，按规则选择具体 Agent。
/// </summary>
public interface IAgentRouter
{
    AgentRouteResult Route(AgentRequest request);
}

/// <summary>
/// Agent 执行接口。
/// </summary>
public interface IAgent
{
    string Name { get; }

    Task<AgentResponse> ExecuteAsync(AgentRequest request, AgentRouteResult route, CancellationToken cancellationToken = default);
}

/// <summary>
/// 支持真实流式输出的 Agent 接口。
/// </summary>
public interface IStreamingAgent : IAgent
{
    IAsyncEnumerable<AgentStreamChunkDto> StreamExecuteAsync(AgentRequest request, AgentRouteResult route, CancellationToken cancellationToken = default);
}

/// <summary>
/// 文本模型提供方接口。
/// </summary>
public interface ITextModelProvider
{
    string ProviderName { get; }

    string ModelName { get; }

    Task<string> GenerateAsync(string prompt, CancellationToken cancellationToken = default);

    IAsyncEnumerable<string> GenerateStreamAsync(string prompt, string? thinkingMode = null, CancellationToken cancellationToken = default);
}

/// <summary>
/// 视觉模型提供方接口。
/// </summary>
public interface IVisionModelProvider
{
    string ProviderName { get; }

    string ModelName { get; }

    Task<string> AnalyzeImageAsync(string imageUrl, string prompt, CancellationToken cancellationToken = default);

    IAsyncEnumerable<string> AnalyzeImageStreamAsync(string imageUrl, string prompt, string? thinkingMode = null, CancellationToken cancellationToken = default);
}

/// <summary>
/// Prompt 模板服务接口。
/// </summary>
public interface IPromptTemplateService
{
    string GetTemplate(string templateCode);

    string GetVersion(string templateCode);

    string Render(string templateCode, IReadOnlyDictionary<string, string?> variables);
}

/// <summary>
/// 教材知识服务接口。
/// </summary>
public interface ITextbookKnowledgeService
{
    Task<IReadOnlyList<TextbookReferenceDto>> SearchAsync(string? subject, string? grade, string? question, string? knowledgePointId, CancellationToken cancellationToken = default);
}

/// <summary>
/// 错题服务接口。
/// </summary>
public interface IWrongQuestionService
{
    Task<WrongQuestionDto?> GetAsync(string wrongQuestionId, CancellationToken cancellationToken = default);
}

/// <summary>
/// 作业检查服务接口。
/// </summary>
public interface IHomeworkCheckService
{
    Task SaveItemsAsync(string questionRecordId, string userId, string? subject, string? grade, HomeworkCheckResultDto result, CancellationToken cancellationToken = default);
}

/// <summary>
/// 模型调用日志服务接口。
/// </summary>
public interface IModelCallLogService
{
    Task SaveAsync(ModelCallLog log, CancellationToken cancellationToken = default);
}

/// <summary>
/// Agent 路由日志服务接口。
/// </summary>
public interface IAgentRouteLogService
{
    Task SaveAsync(AgentRouteLog log, CancellationToken cancellationToken = default);
}

/// <summary>
/// 媒体资源服务接口。
/// </summary>
public interface IMediaResourceService
{
    Task<MediaResource?> FindByUrlAsync(string? url, CancellationToken cancellationToken = default);

    Task<MediaUploadResultDto> SaveImageAsync(
        Stream content,
        string fileName,
        string? mimeType,
        string? resourceType,
        string? userId,
        string? sourceType,
        CancellationToken cancellationToken = default);
}
