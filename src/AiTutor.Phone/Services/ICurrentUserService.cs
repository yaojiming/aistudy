namespace AiTutor.Maui.Services;

public interface ICurrentUserService
{
    string UserId { get; }
}

public sealed class CurrentUserService : ICurrentUserService
{
    public string UserId => "student-001";
}
