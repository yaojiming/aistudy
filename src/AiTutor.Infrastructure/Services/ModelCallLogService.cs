using AiTutor.Core.Entities;
using AiTutor.Core.Interfaces;
using AiTutor.Infrastructure.Data;

namespace AiTutor.Infrastructure.Services;

/// <summary>
/// 模型调用日志服务。
/// </summary>
public class ModelCallLogService : IModelCallLogService
{
    private readonly AiTutorDbContext _dbContext;

    public ModelCallLogService(AiTutorDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SaveAsync(ModelCallLog log, CancellationToken cancellationToken = default)
    {
        _dbContext.ModelCallLogs.Add(log);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
