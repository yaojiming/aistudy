using AiTutor.Core.Interfaces;
using AiTutor.Shared.Agent;
using Microsoft.Extensions.Logging;

namespace AiTutor.Infrastructure.Agents;

/// <summary>
/// 图片讲题 Agent，负责把图片题目交给视觉模型识别并生成讲解。
/// </summary>
public class VisionAgent : IStreamingAgent
{
    private readonly IVisionModelProvider _visionProvider;
    private readonly IPromptTemplateService _promptTemplateService;
    private readonly ILogger<VisionAgent> _logger;

    public VisionAgent(
        IVisionModelProvider visionProvider,
        IPromptTemplateService promptTemplateService,
        ILogger<VisionAgent> logger)
    {
        _visionProvider = visionProvider;
        _promptTemplateService = promptTemplateService;
        _logger = logger;
    }

    public string Name => "VisionAgent";

    /// <summary>
    /// 执行非流式图片讲题流程，供普通 Agent 接口使用。
    /// </summary>
    /// <param name="request">包含图片路径的 Agent 请求。</param>
    /// <param name="route">Agent 路由结果。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>完整图片讲题响应。</returns>
    public async Task<AgentResponse> ExecuteAsync(AgentRequest request, AgentRouteResult route, CancellationToken cancellationToken = default)
    {
        var answer = await _visionProvider.AnalyzeImageAsync(request.ImageUrl ?? string.Empty, BuildPrompt(request), request.EnableThinking, request.ModelName, cancellationToken);
        return CreateResponse(route, answer);
    }

    /// <summary>
    /// 执行真实流式图片讲题流程，逐段返回视觉模型输出。
    /// </summary>
    /// <param name="request">包含图片路径与 EnableThinking 的 Agent 请求。</param>
    /// <param name="route">Agent 路由结果。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>delta 文本片段和最终结构化响应。</returns>
    public async IAsyncEnumerable<AgentStreamChunkDto> StreamExecuteAsync(
        AgentRequest request,
        AgentRouteResult route,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var answerText = string.Empty;
        await foreach (var delta in _visionProvider.AnalyzeImageStreamAsync(request.ImageUrl ?? string.Empty, BuildPrompt(request), request.EnableThinking, request.ModelName, cancellationToken))
        {
            answerText += delta;
            yield return new AgentStreamChunkDto { Type = "delta", Text = delta };
        }

        _logger.LogInformation(
            "Vision stream completed. AnswerLength={AnswerLength}, Model={Model}, EnableThinking={EnableThinking}",
            answerText.Length,
            request.ModelName ?? route.ModelName,
            request.EnableThinking);

        yield return new AgentStreamChunkDto { Type = "final", FinalResponse = CreateResponse(route, answerText) };
    }

    /// <summary>
    /// 渲染图片讲题 Prompt。基础要求来自 PromptTemplateService，本次前端指令只作为补充约束。
    /// </summary>
    /// <param name="request">图片讲题请求。</param>
    /// <returns>渲染后的 Prompt。</returns>
    private string BuildPrompt(AgentRequest request)
    {
        var templatePrompt = _promptTemplateService.Render("vision_question_explain", new Dictionary<string, string?>
        {
            ["imageUrl"] = request.ImageUrl,
            ["subject"] = request.Subject,
            ["grade"] = request.Grade
        });

        if (string.IsNullOrWhiteSpace(request.QuestionText))
        {
            return templatePrompt;
        }

        return $"""
            {templatePrompt}

            本次前端指令：
            {request.QuestionText}

            请优先遵守本次前端指令，只讲解用户框选后的这一道题。
            请尽快开始输出，先识别题目，再分步骤讲解。
            数学公式尽量使用普通文本或简单 LaTeX，例如 4 m、16 m²、1/2，不要使用复杂排版命令。
            """;
    }

    /// <summary>
    /// 组装图片讲题响应，并保留前端可展示的结构化示例字段。
    /// </summary>
    /// <param name="route">Agent 路由结果。</param>
    /// <param name="answer">模型输出文本。</param>
    /// <returns>统一 AgentResponse。</returns>
    private static AgentResponse CreateResponse(AgentRouteResult route, string answer)
    {
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
