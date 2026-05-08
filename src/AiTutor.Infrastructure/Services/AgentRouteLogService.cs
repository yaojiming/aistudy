using AiTutor.Core.Entities;
using AiTutor.Core.Interfaces;
using AiTutor.Infrastructure.Data;

namespace AiTutor.Infrastructure.Services;

/// <summary>
/// Agent 路由日志服务。
/// </summary>
public class AgentRouteLogService : IAgentRouteLogService
{
    private readonly AiTutorDbContext _dbContext;

    public AgentRouteLogService(AiTutorDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SaveAsync(AgentRouteLog log, CancellationToken cancellationToken = default)
    {
        _dbContext.AgentRouteLogs.Add(log);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
