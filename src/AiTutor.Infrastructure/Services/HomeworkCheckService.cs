using AiTutor.Core.Entities;
using AiTutor.Core.Enums;
using AiTutor.Core.Interfaces;
using AiTutor.Infrastructure.Data;
using AiTutor.Shared.Agent;
using Microsoft.Extensions.Logging;

namespace AiTutor.Infrastructure.Services;

/// <summary>
/// 作业检查保存服务。
/// </summary>
public class HomeworkCheckService : IHomeworkCheckService
{
    private readonly AiTutorDbContext _dbContext;
    private readonly ILogger<HomeworkCheckService> _logger;

    public HomeworkCheckService(AiTutorDbContext dbContext, ILogger<HomeworkCheckService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task SaveItemsAsync(string questionRecordId, string userId, string? subject, string? grade, HomeworkCheckResultDto result, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Saving homework check items. QuestionRecordId={QuestionRecordId}, ItemCount={ItemCount}, TotalCount={TotalCount}, QuestionNos={QuestionNos}",
            questionRecordId,
            result.Items.Count,
            result.TotalCount,
            string.Join(",", result.Items.Select(item => item.QuestionNo)));

        foreach (var item in result.Items)
        {
            _logger.LogInformation(
                "Saving homework check item. QuestionRecordId={QuestionRecordId}, QuestionNo={QuestionNo}, IsCorrect={IsCorrect}",
                questionRecordId,
                item.QuestionNo,
                item.IsCorrect);

            var entity = new HomeworkCheckItem
            {
                QuestionRecordId = questionRecordId,
                UserId = userId,
                Subject = subject,
                Grade = grade,
                QuestionNo = item.QuestionNo,
                QuestionText = item.QuestionText,
                StudentAnswer = item.StudentAnswer,
                CorrectAnswer = item.CorrectAnswer,
                IsCorrect = item.IsCorrect,
                ErrorReason = item.ErrorReason,
                Explanation = item.Explanation,
                KnowledgePointId = item.KnowledgePointId
            };

            _dbContext.HomeworkCheckItems.Add(entity);

            if (item.IsCorrect == false)
            {
                var wrongQuestion = new WrongQuestion
                {
                    UserId = userId,
                    Subject = subject ?? "未指定",
                    Grade = grade,
                    QuestionRecordId = questionRecordId,
                    HomeworkCheckItemId = entity.Id,
                    KnowledgePointId = item.KnowledgePointId,
                    QuestionText = item.QuestionText,
                    StudentAnswer = item.StudentAnswer,
                    CorrectAnswer = item.CorrectAnswer,
                    ErrorReason = item.ErrorReason,
                    Explanation = item.Explanation,
                    MasteryStatus = MasteryStatus.New,
                    NextReviewTime = DateTime.UtcNow.AddDays(1)
                };

                _dbContext.WrongQuestions.Add(wrongQuestion);
                item.WrongQuestionId = wrongQuestion.Id;
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
