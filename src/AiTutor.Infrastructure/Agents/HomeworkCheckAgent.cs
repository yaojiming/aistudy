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

    /// <summary>
    /// 执行非流式作业检查，供普通 Agent 接口使用。
    /// </summary>
    /// <param name="request">包含作业图片路径的请求。</param>
    /// <param name="route">Agent 路由结果。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>包含作业检查结构化结果的完整响应。</returns>
    public async Task<AgentResponse> ExecuteAsync(AgentRequest request, AgentRouteResult route, CancellationToken cancellationToken = default)
    {
        var modelAnswer = await _visionProvider.AnalyzeImageAsync(request.ImageUrl ?? string.Empty, BuildPrompt(request), cancellationToken);
        return CreateResponse(route, modelAnswer);
    }

    /// <summary>
    /// 执行真实流式作业检查，先流式展示视觉模型判断，再在 final 中返回结构化结果。
    /// </summary>
    /// <param name="request">包含作业图片路径与 ThinkingMode 的请求。</param>
    /// <param name="route">Agent 路由结果。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>delta 文本片段和最终作业检查响应。</returns>
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

    /// <summary>
    /// 渲染作业检查 Prompt。
    /// </summary>
    /// <param name="request">作业检查请求。</param>
    /// <returns>渲染后的 Prompt。</returns>
    private string BuildPrompt(AgentRequest request)
    {
        return _promptTemplateService.Render("homework_check", new Dictionary<string, string?>
        {
            ["imageUrl"] = request.ImageUrl,
            ["subject"] = request.Subject,
            ["grade"] = request.Grade
        });
    }

    /// <summary>
    /// 组装作业检查响应，当前结构化 DTO 保持 Mock，文本部分使用真实视觉模型输出。
    /// </summary>
    /// <param name="route">Agent 路由结果。</param>
    /// <param name="modelAnswer">视觉模型输出文本。</param>
    /// <returns>统一 AgentResponse。</returns>
    private static AgentResponse CreateResponse(AgentRouteResult route, string modelAnswer)
    {
        var result = CreateStructuredMockResult();
        var response = ResponseFactory.Create(route, modelAnswer, canAddToWrongBook: true);
        response.OutputType = "structured_json";
        response.HomeworkCheckResult = result;
        response.AnswerJson = result;
        response.Suggestions.Add(new SuggestionDto { Text = "练习乘法口诀", Action = "practice_generate" });
        return response;
    }

    /// <summary>
    /// 创建 MVP 阶段稳定的作业检查结构化结果，保证保存 HomeworkCheckItem 和 WrongQuestion 链路可验证。
    /// </summary>
    /// <returns>作业检查 DTO。</returns>
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
