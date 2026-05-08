namespace AiTutor.Shared.Agent;

/// <summary>
/// AI 数字人讲解脚本 DTO。
/// </summary>
public class AvatarScriptDto
{
    public string Subtitle { get; set; } = string.Empty;

    public List<AvatarScriptSegmentDto> Segments { get; set; } = [];
}

/// <summary>
/// 数字人脚本片段。
/// </summary>
public class AvatarScriptSegmentDto
{
    public int TimeStart { get; set; }

    public int TimeEnd { get; set; }

    public string SpeechText { get; set; } = string.Empty;

    public string Action { get; set; } = "smile";

    public string? BoardText { get; set; }
}
