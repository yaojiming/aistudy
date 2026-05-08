using AiTutor.Core.Interfaces;
using AiTutor.Shared.Agent;

namespace AiTutor.Infrastructure.Services;

/// <summary>
/// 教材知识 Mock 服务，后续替换为真实检索。
/// </summary>
public class TextbookKnowledgeService : ITextbookKnowledgeService
{
    public Task<IReadOnlyList<TextbookReferenceDto>> SearchAsync(string? subject, string? grade, string? question, string? knowledgePointId, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<TextbookReferenceDto> references =
        [
            new TextbookReferenceDto
            {
                KnowledgePointId = knowledgePointId,
                Title = "Mock 教材知识片段",
                Content = $"这里模拟返回 {grade ?? "小学"}{subject ?? "学科"} 的教材知识片段。真实教材检索会在 Phase 5 接入。",
                PageNo = 12,
                Score = 0.86
            }
        ];

        return Task.FromResult(references);
    }
}
