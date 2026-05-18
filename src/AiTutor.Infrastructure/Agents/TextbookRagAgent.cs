using AiTutor.Core.Interfaces;
using AiTutor.Shared.Agent;

namespace AiTutor.Infrastructure.Agents;

/// <summary>
/// 教材知识库问答 Agent，负责检索教材引用并调用文本模型生成回答。
/// </summary>
/// <remarks>
/// 调用链：AgentService -> AgentRouter 选择 TextbookRagAgent -> ITextbookKnowledgeService -> PromptTemplateService -> ITextModelProvider。
/// Phase 3 接入 DeepSeek 后，教材问答可在 UseMock=false 时走真实文本模型；教材检索本身仍保持 Phase 2 的占位实现。
/// </remarks>
public class TextbookRagAgent : IAgent
{
    private readonly ITextbookKnowledgeService _knowledgeService;
    private readonly ITextModelProvider _modelProvider;
    private readonly IPromptTemplateService _promptTemplateService;

    public TextbookRagAgent(
        ITextbookKnowledgeService knowledgeService,
        ITextModelProvider modelProvider,
        IPromptTemplateService promptTemplateService)
    {
        _knowledgeService = knowledgeService;
        _modelProvider = modelProvider;
        _promptTemplateService = promptTemplateService;
    }

    public string Name => "TextbookRagAgent";

    /// <summary>
    /// 执行教材知识库问答流程。
    /// </summary>
    /// <param name="request">教材问答请求。</param>
    /// <param name="route">教材问答路由结果。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>包含教材引用列表和模型讲解文本的响应。</returns>
    /// <remarks>
    /// 代码逻辑：
    /// 1. 调用 ITextbookKnowledgeService 检索教材引用；
    /// 2. 把引用摘要合并成 textbook_context；
    /// 3. 使用 textbook_rag_qa 模板渲染 Prompt；
    /// 4. 调用当前文本 Provider，Mock 模式返回模拟讲解，Real 模式调用 DeepSeek；
    /// 5. 把教材引用附加到 AgentResponse.TextbookReferences。
    /// </remarks>
    public async Task<AgentResponse> ExecuteAsync(AgentRequest request, AgentRouteResult route, CancellationToken cancellationToken = default)
    {
        var references = await _knowledgeService.SearchAsync(request.Subject, request.Grade, request.QuestionText, request.RelatedKnowledgePointId, cancellationToken);
        var textbookContext = string.Join(Environment.NewLine, references.Select(x => $"- {x.Title}: {x.Content}"));
        var prompt = _promptTemplateService.Render("textbook_rag_qa", new Dictionary<string, string?>
        {
            ["subject"] = request.Subject ?? "学习",
            ["grade"] = request.Grade ?? "小学",
            ["question"] = request.QuestionText,
            ["textbook_context"] = textbookContext
        });

        var answer = await _modelProvider.GenerateAsync(prompt, request.EnableThinking, cancellationToken);
        var response = ResponseFactory.Create(route, answer, canAddToWrongBook: true);
        response.TextbookReferences.AddRange(references);
        response.Suggestions.Add(new SuggestionDto { Text = "查看教材例题", Action = "open_textbook_reference" });
        return response;
    }
}
