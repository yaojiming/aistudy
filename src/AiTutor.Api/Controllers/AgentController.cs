using AiTutor.Core.Interfaces;
using AiTutor.Shared.Agent;
using Microsoft.AspNetCore.Mvc;

namespace AiTutor.Api.Controllers;

/// <summary>
/// Agent 网关 API 控制器。
/// </summary>
/// <remarks>
/// 调用链位置：MAUI 客户端或 Swagger 调用本控制器，本控制器只做 HTTP 入参和出参处理，
/// 真实业务链路全部交给 IAgentService。这样可以避免 Controller 堆业务逻辑，
/// 也方便后续把同一套 AgentService 复用于其他入口，例如语音 WebSocket 或后台任务。
/// </remarks>
[ApiController]
[Route("api/[controller]")]
public class AgentController : ControllerBase
{
    private readonly IAgentService _agentService;

    public AgentController(IAgentService agentService)
    {
        _agentService = agentService;
    }

    /// <summary>
    /// 统一 AI 提问入口。
    /// </summary>
    /// <param name="request">学生提问、拍照讲题、作业检查、错题复习或预留语音/数字人请求。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>AgentResponse，包含回答文本、结构化结果、路由信息和前端建议。</returns>
    /// <remarks>
    /// 调用链：
    /// Controller -> AgentService -> AgentRouter -> 具体 Agent -> Mock Provider -> 数据库存储。
    /// Controller 不直接调用 DbContext，也不直接拼 Prompt 或调用模型。
    /// </remarks>
    [HttpPost("ask")]
    [ProducesResponseType(typeof(AgentResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<AgentResponse>> Ask([FromBody] AgentRequest request, CancellationToken cancellationToken)
    {
        var response = await _agentService.AskAsync(request, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Agent 服务健康检查。
    /// </summary>
    /// <returns>返回 Mock Agent 网关的基本状态。</returns>
    /// <remarks>
    /// 该接口用于 Swagger、MAUI 或部署检查快速确认 API 已启动。
    /// 它不触发模型调用，也不写数据库。
    /// </remarks>
    [HttpGet("health")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<object> Health()
    {
        return Ok(new
        {
            status = "ok",
            provider = "Mock",
            message = "AiTutor Agent API is running."
        });
    }
}
