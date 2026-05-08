using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AiTutor.Infrastructure.Data;

/// <summary>
/// EF Core 设计时工厂，仅用于生成 Migration 和 SQL 脚本，不会主动连接数据库。
/// </summary>
public class AiTutorDbContextFactory : IDesignTimeDbContextFactory<AiTutorDbContext>
{
    public AiTutorDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AiTutorDbContext>();
        optionsBuilder.UseSqlServer("Server=.;Database=AiTutorDb;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False;");

        return new AiTutorDbContext(optionsBuilder.Options);
    }
}
