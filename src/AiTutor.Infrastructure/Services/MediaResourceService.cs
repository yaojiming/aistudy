using AiTutor.Core.Entities;
using AiTutor.Core.Interfaces;
using AiTutor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AiTutor.Infrastructure.Services;

/// <summary>
/// 媒体资源查询服务。
/// </summary>
public class MediaResourceService : IMediaResourceService
{
    private readonly AiTutorDbContext _dbContext;

    public MediaResourceService(AiTutorDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<MediaResource?> FindByUrlAsync(string? url, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return Task.FromResult<MediaResource?>(null);
        }

        return _dbContext.MediaResources.AsNoTracking().FirstOrDefaultAsync(x => x.FilePath == url, cancellationToken);
    }
}
