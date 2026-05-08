using AiTutor.Core.Interfaces;
using AiTutor.Shared.Agent;

namespace AiTutor.Infrastructure.Agents;

/// <summary>
/// 数字人讲解占位 Agent。
/// </summary>
public class AvatarAgent : IAgent
{
    public string Name => "AvatarAgent";

    /// <summary>
    /// 执行数字人讲解占位流程。
    /// </summary>
    /// <param name="request">数字人讲解请求。</param>
    /// <param name="route">数字人路由结果。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>可供前端预览的数字人脚本结构。</returns>
    /// <remarks>
    /// 调用链：AgentService -> AvatarAgent。
    /// 当前不生成真实视频，只返回 AvatarScriptDto；
    /// AgentService 会把脚本 JSON 保存到 AnswerRecord.AvatarScriptJson，为后续视频生成服务预留。
    /// </remarks>
    public Task<AgentResponse> ExecuteAsync(AgentRequest request, AgentRouteResult route, CancellationToken cancellationToken = default)
    {
        var response = ResponseFactory.Create(route, "数字人视频讲解功能已经预留。现在先返回一段可展示的脚本结构，后续可以接 TTS 和动画生成。", canAddToWrongBook: false);
        response.OutputType = "avatar_script";
        response.AvatarScript = new AvatarScriptDto
        {
            Subtitle = "我们先看题目，再一步一步写出思路。",
            Segments =
            [
                new AvatarScriptSegmentDto { TimeStart = 0, TimeEnd = 4, SpeechText = "我们先看题目在问什么。", Action = "look_question", BoardText = "读题" },
                new AvatarScriptSegmentDto { TimeStart = 4, TimeEnd = 9, SpeechText = "再把关键条件圈出来。", Action = "point_step", BoardText = "找条件" },
                new AvatarScriptSegmentDto { TimeStart = 9, TimeEnd = 14, SpeechText = "最后按步骤计算，并检查答案。", Action = "show_answer", BoardText = "检查" }
            ]
        };
        response.Suggestions.Add(new SuggestionDto { Text = "生成视频讲解", Action = "avatar_explain" });
        return Task.FromResult(response);
    }
}
