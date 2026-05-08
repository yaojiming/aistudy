using AiTutor.Core.Interfaces;
using AiTutor.Shared.Agent;

namespace AiTutor.Infrastructure.Agents;

/// <summary>
/// 错题复习 Agent，负责根据错题上下文生成复习讲解和同类练习建议。
/// </summary>
/// <remarks>
/// 调用链：AgentService -> AgentRouter 选择 WrongBookAgent -> IWrongQuestionService -> PromptTemplateService -> ITextModelProvider。
/// 当前错题读取服务仍是基础实现；Phase 3 的重点是让复习讲解可以在 UseMock=false 时走 DeepSeek。
/// </remarks>
public class WrongBookAgent : IAgent
{
    private readonly IWrongQuestionService _wrongQuestionService;
    private readonly ITextModelProvider _modelProvider;
    private readonly IPromptTemplateService _promptTemplateService;

    public WrongBookAgent(
        IWrongQuestionService wrongQuestionService,
        ITextModelProvider modelProvider,
        IPromptTemplateService promptTemplateService)
    {
        _wrongQuestionService = wrongQuestionService;
        _modelProvider = modelProvider;
        _promptTemplateService = promptTemplateService;
    }

    public string Name => "WrongBookAgent";

    /// <summary>
    /// 执行错题复习流程。
    /// </summary>
    /// <param name="request">错题复习请求，可携带 RelatedWrongQuestionId。</param>
    /// <param name="route">错题复习路由结果。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>包含错因回顾、正确方法和复习建议的响应。</returns>
    /// <remarks>
    /// 代码逻辑：
    /// 1. 如果传入 RelatedWrongQuestionId，则尝试读取真实错题 DTO；
    /// 2. 没有错题上下文时使用当前问题文本作为复习题；
    /// 3. 使用 wrong_question_review 模板渲染 Prompt；
    /// 4. 调用当前文本 Provider；
    /// 5. 添加生成同类练习和稍后复习的建议动作。
    /// </remarks>
    public async Task<AgentResponse> ExecuteAsync(AgentRequest request, AgentRouteResult route, CancellationToken cancellationToken = default)
    {
        WrongQuestionDto? wrongQuestion = null;
        if (!string.IsNullOrWhiteSpace(request.RelatedWrongQuestionId))
        {
            wrongQuestion = await _wrongQuestionService.GetAsync(request.RelatedWrongQuestionId, cancellationToken);
        }

        var prompt = _promptTemplateService.Render("wrong_question_review", new Dictionary<string, string?>
        {
            ["subject"] = request.Subject ?? "学习",
            ["grade"] = request.Grade ?? "小学",
            ["question"] = wrongQuestion?.QuestionText ?? request.QuestionText,
            ["student_answer"] = wrongQuestion?.StudentAnswer ?? string.Empty,
            ["correct_answer"] = wrongQuestion?.CorrectAnswer ?? string.Empty,
            ["error_reason"] = wrongQuestion?.ErrorReason ?? "暂时没有历史错因，请根据题目帮助学生找出容易错的地方。",
            ["explanation"] = wrongQuestion?.Explanation ?? string.Empty
        });

        var answer = await _modelProvider.GenerateAsync(prompt, cancellationToken);
        var response = ResponseFactory.Create(route, answer, canAddToWrongBook: false);
        response.Suggestions.Add(new SuggestionDto { Text = "生成同类练习", Action = "practice_generate" });
        response.Suggestions.Add(new SuggestionDto { Text = "稍后再复习", Action = "schedule_review" });
        return response;
    }
}
