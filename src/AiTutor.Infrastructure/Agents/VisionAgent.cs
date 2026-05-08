using AiTutor.Core.Interfaces;
using AiTutor.Shared.Agent;

namespace AiTutor.Infrastructure.Agents;

/// <summary>
/// 图片讲题 Agent，负责把图片题目交给视觉模型识别并生成讲解。
/// </summary>
/// <remarks>
/// 调用链：AgentService -> AgentRouter 选择 VisionAgent -> VisionAgent -> PromptTemplateService -> IVisionModelProvider。
/// Phase 3 后 IVisionModelProvider 可由 DI 切换为 MockVisionModelProvider 或 GlmVisionModelProvider。
/// 本类只负责模型调用和响应组装，图片资源保存与 QuestionRecord 保存由 AgentService 或后续 MediaResourceService 完成。
/// </remarks>
public class VisionAgent : IAgent
{
    private readonly IVisionModelProvider _visionProvider;
    private readonly IPromptTemplateService _promptTemplateService;

    public VisionAgent(IVisionModelProvider visionProvider, IPromptTemplateService promptTemplateService)
    {
        _visionProvider = visionProvider;
        _promptTemplateService = promptTemplateService;
    }

    public string Name => "VisionAgent";

    /// <summary>
    /// 执行图片讲题流程。
    /// </summary>
    /// <param name="request">包含 ImageUrl 的图片提问请求。</param>
    /// <param name="route">图片讲题路由结果。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>包含识别说明、解题步骤和建议动作的 AgentResponse。</returns>
    /// <remarks>
    /// 代码逻辑：
    /// 1. 使用 vision_question_explain 模板生成视觉模型 Prompt；
    /// 2. 调用当前注入的视觉模型 Provider；
    /// 3. 把模型文本转换为统一 AgentResponse；
    /// 4. 保留 AnswerJson 示例结构，方便 Swagger 和 MAUI 后续按结构展示。
    /// </remarks>
    public async Task<AgentResponse> ExecuteAsync(AgentRequest request, AgentRouteResult route, CancellationToken cancellationToken = default)
    {
        var prompt = _promptTemplateService.Render("vision_question_explain", new Dictionary<string, string?>
        {
            ["imageUrl"] = request.ImageUrl,
            ["subject"] = request.Subject,
            ["grade"] = request.Grade
        });

        var answer = await _visionProvider.AnalyzeImageAsync(request.ImageUrl ?? string.Empty, prompt, cancellationToken);
        var response = ResponseFactory.Create(route, answer, canAddToWrongBook: true);
        response.AnswerJson = new
        {
            recognizedQuestion = "由视觉模型识别的题目内容",
            knowledgePoint = "由视觉模型判断的知识点",
            steps = new[] { "读题", "分析条件", "分步骤计算或推理", "检查答案" },
            finalAnswer = "以模型讲解为准"
        };
        response.Suggestions.Add(new SuggestionDto { Text = "加入错题本", Action = "add_wrong_question" });
        return response;
    }
}
