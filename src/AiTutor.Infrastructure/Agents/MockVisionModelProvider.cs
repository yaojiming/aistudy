using AiTutor.Core.Interfaces;

namespace AiTutor.Infrastructure.Agents;

/// <summary>
/// Mock 视觉模型，第一阶段用于模拟图片识别和作业检查。
/// </summary>
public class MockVisionModelProvider : IVisionModelProvider
{
    public string ProviderName => "Mock";

    public string ModelName => "mock-vision-model";

    /// <summary>
    /// 模拟图片识别和讲题。
    /// </summary>
    /// <param name="imageUrl">图片地址或媒体路径。</param>
    /// <param name="prompt">视觉 Prompt。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>固定的图片题目识别和分步讲解。</returns>
    /// <remarks>
    /// 调用链：VisionAgent 调用本方法。
    /// Phase 2 不接 GLM 视觉模型，因此这里不上传图片、不请求外部接口；
    /// 只模拟“识别题目 -> 讲解步骤 -> 给出答案”的返回格式。
    /// </remarks>
    public Task<string> AnalyzeImageAsync(string imageUrl, string prompt, CancellationToken cancellationToken = default)
    {
        var answer = """
            我从图片里模拟识别到一道数学题：24 ÷ 3 = ?

            题目在问：把 24 平均分成 3 份，每份是多少。
            解题步骤：
            1. 先想乘法口诀：三八二十四。
            2. 所以 24 ÷ 3 = 8。
            3. 检查一下：8 × 3 = 24，说明答案正确。

            答案是 8。下次遇到除法题，可以先想对应的乘法口诀。
            """;

        return Task.FromResult(answer);
    }
}
