namespace AiTutor.Maui.Models;

/// <summary>
/// 作业单题从识别到检查完成的前端状态。
/// </summary>
public enum HomeworkQuestionStatus
{
    Pending,
    Detecting,
    Checking,
    Correct,
    Wrong,
    Uncertain
}

