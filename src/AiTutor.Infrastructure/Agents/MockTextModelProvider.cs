using AiTutor.Core.Interfaces;

namespace AiTutor.Infrastructure.Agents;

/// <summary>
/// Mock 文本模型，第一阶段用于跑通 Agent 链路。
/// </summary>
public class MockTextModelProvider : ITextModelProvider
{
    public string ProviderName => "Mock";

    public string ModelName => "mock-text-model";

    /// <summary>
    /// 模拟文本模型生成。
    /// </summary>
    /// <param name="prompt">由 PromptTemplateService 渲染后的提示词。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>固定的小学老师风格讲解。</returns>
    /// <remarks>
    /// 调用链：ChatAgent 或未来文本类 Agent 调用本方法。
    /// 这里不访问外网、不读取 API Key，只返回稳定文本，便于验证路由、保存和 Swagger 调用。
    /// </remarks>
    public Task<string> GenerateAsync(string prompt, CancellationToken cancellationToken = default)
    {
        var answer = """
            我们先不急着找答案，先看清楚题目在问什么。

            第一步：把题目里的已知条件圈出来。
            第二步：想一想这些条件之间有什么关系。
            第三步：按关系一步一步计算或推理，最后再检查答案是否符合题意。

            这道题主要练的是“先理解，再动手”的方法。你已经抓到关键了，下一题可以试着自己先说一遍思路。
            """;

        return Task.FromResult(answer);
    }

    /// <summary>
    /// Mock 文本模型的流式输出，用于开发环境验证端到端流式链路。
    /// </summary>
    /// <param name="prompt">渲染后的 Prompt。</param>
    /// <param name="enableThinking">是否启用思考模式，占位保留。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>模拟增量文本片段。</returns>
    public async IAsyncEnumerable<string> GenerateStreamAsync(
        string prompt,
        bool enableThinking = false,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var answer = await GenerateAsync(prompt, cancellationToken);
        foreach (var chunk in SplitForStreaming(answer))
        {
            cancellationToken.ThrowIfCancellationRequested();
            await Task.Delay(60, cancellationToken);
            yield return chunk;
        }
    }

    /// <summary>
    /// 将完整 Mock 文本切成小片段，模拟模型逐段输出。
    /// </summary>
    /// <param name="text">完整文本。</param>
    /// <returns>文本片段序列。</returns>
    private static IEnumerable<string> SplitForStreaming(string text)
    {
        for (var index = 0; index < text.Length; index += 12)
        {
            yield return text.Substring(index, Math.Min(12, text.Length - index));
        }
    }
}
