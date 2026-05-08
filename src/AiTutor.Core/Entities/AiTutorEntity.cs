namespace AiTutor.Core.Entities;

/// <summary>
/// 领域实体基础类，统一使用字符串主键和创建时间。
/// </summary>
public abstract class AiTutorEntity
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// 支持更新时间的领域实体基础类。
/// </summary>
public abstract class AuditableEntity : AiTutorEntity
{
    public DateTime? UpdatedTime { get; set; }
}
