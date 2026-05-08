using AiTutor.Core.Interfaces;
using AiTutor.Shared.Agent;

namespace AiTutor.Infrastructure.Agents;

/// <summary>
/// 作业检查 Agent，负责识别作业图片并返回逐题检查结果。
/// </summary>
/// <remarks>
/// 调用链：AgentService -> AgentRouter 选择 HomeworkCheckAgent -> HomeworkCheckAgent -> IVisionModelProvider。
/// Phase 3 后视觉 Provider 可按配置切换 Mock 或 GLM；HomeworkCheckItem 和 WrongQuestion 的保存仍由 AgentService 调用 HomeworkCheckService 完成。
/// 当前阶段保留结构化 Mock 检查项，保证数据库拆题保存链路稳定，同时把真实视觉模型返回文本放入 AnswerText。
/// </remarks>
public class HomeworkCheckAgent : IAgent
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
    /// 执行作业检查流程。
    /// </summary>
    /// <param name="request">作业图片检查请求。</param>
    /// <param name="route">作业检查路由结果。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>包含 AnswerText 和 HomeworkCheckResult 的统一响应。</returns>
    /// <remarks>
    /// 代码逻辑：
    /// 1. 使用 homework_check 模板生成视觉 Prompt；
    /// 2. 调用当前注入的视觉 Provider，Mock 模式不需要 API Key，Real 模式会访问 GLM；
    /// 3. 构造结构化 HomeworkCheckResult，供 HomeworkCheckService 保存到 HomeworkCheckItem；
    /// 4. 错题项会在保存服务中自动生成 WrongQuestion。
    /// </remarks>
    public async Task<AgentResponse> ExecuteAsync(AgentRequest request, AgentRouteResult route, CancellationToken cancellationToken = default)
    {
        var prompt = _promptTemplateService.Render("homework_check", new Dictionary<string, string?>
        {
            ["imageUrl"] = request.ImageUrl,
            ["subject"] = request.Subject,
            ["grade"] = request.Grade
        });

        var modelAnswer = await _visionProvider.AnalyzeImageAsync(request.ImageUrl ?? string.Empty, prompt, cancellationToken);
        var result = CreateStructuredMockResult();

        var response = ResponseFactory.Create(route, modelAnswer, canAddToWrongBook: true);
        response.OutputType = "structured_json";
        response.HomeworkCheckResult = result;
        response.AnswerJson = result;
        response.Suggestions.Add(new SuggestionDto { Text = "练习乘法口诀", Action = "practice_generate" });
        return response;
    }

    /// <summary>
    /// 创建稳定的结构化作业检查结果。
    /// </summary>
    /// <returns>包含 3 道题、1 道错题的 HomeworkCheckResultDto。</returns>
    /// <remarks>
    /// 调用链：ExecuteAsync -> CreateStructuredMockResult -> AgentService -> HomeworkCheckService.SaveItemsAsync。
    /// Phase 3 的重点是真实 Provider 接入，不扩展 JSON 解析功能；因此暂时保留结构化 Mock 结果，
    /// 后续可以把 GLM 返回的 JSON 解析为同一 DTO，而不改变保存链路。
    /// </remarks>
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
                    QuestionNo = "第1题",
                    QuestionText = "12 ÷ 3 = ?",
                    StudentAnswer = "4",
                    CorrectAnswer = "4",
                    IsCorrect = true,
                    Explanation = "三四十二，所以 12 除以 3 等于 4。",
                    KnowledgePointName = "表内除法"
                },
                new HomeworkCheckItemDto
                {
                    QuestionNo = "第2题",
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
                    QuestionNo = "第3题",
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
