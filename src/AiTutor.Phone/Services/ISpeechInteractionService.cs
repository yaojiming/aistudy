namespace AiTutor.Maui.Services;

/// <summary>
/// 提供问答页语音输入和语音播报能力。客户端只处理本地语音交互，不直接调用任何 AI 模型。
/// </summary>
public interface ISpeechInteractionService
{
    /// <summary>
    /// 调用当前平台的语音识别能力，将学生说的话转换成文字。
    /// </summary>
    Task<string?> ListenOnceAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 使用系统 TTS 播报 AI 老师返回的文字。
    /// </summary>
    Task SpeakAsync(string text, CancellationToken cancellationToken = default);

    /// <summary>
    /// 停止当前正在进行的语音识别或语音播报。
    /// </summary>
    void Stop();
}
