namespace AiTutor.Maui.Models;

/// <summary>
/// 作业检查流程的前端状态更新。它只描述客户端可确定的阶段和已返回结果，不假装知道模型内部进度。
/// </summary>
public sealed class HomeworkCheckWorkflowUpdate
{
    public HomeworkCheckWorkflowUpdateKind Kind { get; init; }

    public string Message { get; init; } = string.Empty;

    public HomeworkQuestionCheckResult? Result { get; init; }

    public static HomeworkCheckWorkflowUpdate Status(string message)
    {
        return new HomeworkCheckWorkflowUpdate
        {
            Kind = HomeworkCheckWorkflowUpdateKind.Status,
            Message = message
        };
    }

    public static HomeworkCheckWorkflowUpdate ResultReturned(HomeworkQuestionCheckResult result, string message)
    {
        return new HomeworkCheckWorkflowUpdate
        {
            Kind = HomeworkCheckWorkflowUpdateKind.ResultReturned,
            Message = message,
            Result = result
        };
    }

    public static HomeworkCheckWorkflowUpdate Completed(string message)
    {
        return new HomeworkCheckWorkflowUpdate
        {
            Kind = HomeworkCheckWorkflowUpdateKind.Completed,
            Message = message
        };
    }
}

public enum HomeworkCheckWorkflowUpdateKind
{
    Status,
    ResultReturned,
    Completed
}
