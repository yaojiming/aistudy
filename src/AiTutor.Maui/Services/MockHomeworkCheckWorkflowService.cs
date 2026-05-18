using AiTutor.Maui.Models;
using Microsoft.Extensions.Logging;

namespace AiTutor.Maui.Services;

/// <summary>
/// 作业检查 Mock 服务：一次性返回模拟结果，保持和真实后端的非流式调用形态一致。
/// </summary>
public sealed class MockHomeworkCheckWorkflowService : IHomeworkCheckWorkflowService
{
    private readonly ILogger<MockHomeworkCheckWorkflowService> _logger;

    public MockHomeworkCheckWorkflowService(ILogger<MockHomeworkCheckWorkflowService> logger)
    {
        _logger = logger;
    }

    public Task<IReadOnlyList<HomeworkQuestionCheckResult>> CheckAsync(
        string imagePath,
        IReadOnlyList<HomeworkQuestionRegion> questions,
        string? modelName = null,
        bool enableThinking = false,
        byte[]? imageBytes = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock homework check started. ImagePath={ImagePath}, Count={Count}", imagePath, questions.Count);

        var sourceQuestions = questions.Count > 0
            ? questions
            : CreateFallbackQuestions();

        var results = sourceQuestions.Select((question, index) =>
        {
            var isCorrect = index % 3 != 0;
            return new HomeworkQuestionCheckResult
            {
                QuestionId = question.QuestionId,
                QuestionNo = question.QuestionNo,
                QuestionText = string.IsNullOrWhiteSpace(question.QuestionText)
                    ? $"第 {question.QuestionNo} 题题干"
                    : question.QuestionText,
                StudentAnswer = question.StudentAnswer ?? (isCorrect ? "B" : "D"),
                CorrectAnswer = isCorrect ? question.StudentAnswer ?? "B" : "B",
                IsCorrect = isCorrect,
                BBox = question.BBox,
                ShortResult = isCorrect ? "答案正确。" : "答案不一致，需要订正。",
                Explanation = isCorrect
                    ? "这道题的作答和参考答案一致，步骤基本正确。"
                    : "这道题的学生答案和参考答案不一致，建议先回到题干找关键条件，再重新判断。",
                MistakeReason = isCorrect ? null : "可能是读题不完整，或者把关键条件看反了。",
                KnowledgePoint = "基础计算与审题",
                StreamingCheckText = BuildDetailText(question, isCorrect)
            };
        }).ToList();

        return Task.FromResult<IReadOnlyList<HomeworkQuestionCheckResult>>(results);
    }

    private static IReadOnlyList<HomeworkQuestionRegion> CreateFallbackQuestions()
    {
        return
        [
            new HomeworkQuestionRegion("mock-q1", "1", new Microsoft.Maui.Graphics.RectF(80, 120, 800, 120), "第 1 题", null, 0.9),
            new HomeworkQuestionRegion("mock-q2", "2", new Microsoft.Maui.Graphics.RectF(80, 270, 800, 120), "第 2 题", null, 0.9),
            new HomeworkQuestionRegion("mock-q3", "3", new Microsoft.Maui.Graphics.RectF(80, 420, 800, 120), "第 3 题", null, 0.9)
        ];
    }

    private static string BuildDetailText(HomeworkQuestionRegion question, bool isCorrect)
    {
        var status = isCorrect ? "正确" : "错误";
        var suggestion = isCorrect
            ? "保持这个解题习惯，继续检查下一题。"
            : "先圈出题目中的关键条件，再把自己的答案和正确答案对照一遍。";

        return $"""
            ## 第 {question.QuestionNo} 题
            **检查结果：** {status}

            **题目：** {(string.IsNullOrWhiteSpace(question.QuestionText) ? "题干暂未识别完整" : question.QuestionText)}

            **简短说明：** {(isCorrect ? "答案和参考答案一致。" : "学生答案和参考答案不一致。")}

            **订正建议：** {suggestion}
            """;
    }
}
