namespace AiTutor.Shared.Agent;

/// <summary>
/// Agent 路由结果。
/// </summary>
public class AgentRouteResult
{
    public string AgentName { get; set; } = string.Empty;

    public string ModelProvider { get; set; } = "Mock";

    public string ModelName { get; set; } = "mock-text-model";

    public string RouteReason { get; set; } = string.Empty;
}
