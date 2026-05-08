using AiTutor.Core.Interfaces;
using AiTutor.Shared.Agent;

namespace AiTutor.Infrastructure.Agents;

/// <summary>
/// 语音讨论占位 Agent。
/// </summary>
public class VoiceAgent : IAgent
{
    public string Name => "VoiceAgent";

    /// <summary>
    /// 执行语音讨论占位流程。
    /// </summary>
    /// <param name="request">语音讨论请求。</param>
    /// <param name="route">语音路由结果。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>文本模拟的语音讨论响应。</returns>
    /// <remarks>
    /// 调用链：AgentService -> VoiceAgent。
    /// 目前不做 ASR、TTS 或实时通话，只返回 text_and_audio 类型占位结果，
    /// 同时通过 SessionMessage 和 AnswerRecord 预留音频字段。
    /// </remarks>
    public Task<AgentResponse> ExecuteAsync(AgentRequest request, AgentRouteResult route, CancellationToken cancellationToken = default)
    {
        var response = ResponseFactory.Create(route, "语音讨论功能已经预留接口。现在先用文字模拟：你可以先用一句话说说自己卡在哪里，我会接着追问一个小问题。", canAddToWrongBook: false);
        response.OutputType = "text_and_audio";
        response.Suggestions.Add(new SuggestionDto { Text = "继续语音讨论", Action = "voice_chat" });
        return Task.FromResult(response);
    }
}
