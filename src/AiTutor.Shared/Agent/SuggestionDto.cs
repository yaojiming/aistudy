namespace AiTutor.Shared.Agent;

/// <summary>
/// 前端快捷建议 DTO。
/// </summary>
public class SuggestionDto
{
    public string Text { get; set; } = string.Empty;

    public string Action { get; set; } = string.Empty;
}
