using AiTutor.Core.Interfaces;
using AiTutor.Infrastructure.Data;
using AiTutor.Shared.Agent;
using Microsoft.EntityFrameworkCore;

namespace AiTutor.Infrastructure.Services;

/// <summary>
/// 错题查询服务。
/// </summary>
public class WrongQuestionService : IWrongQuestionService
{
    private readonly AiTutorDbContext _dbContext;

    public WrongQuestionService(AiTutorDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<WrongQuestionDto?> GetAsync(string wrongQuestionId, CancellationToken cancellationToken = default)
    {
        var item = await _dbContext.WrongQuestions.AsNoTracking().FirstOrDefaultAsync(x => x.Id == wrongQuestionId, cancellationToken);
        if (item is null)
        {
            return null;
        }

        return new WrongQuestionDto
        {
            Id = item.Id,
            Subject = item.Subject,
            Grade = item.Grade,
            QuestionText = item.QuestionText,
            StudentAnswer = item.StudentAnswer,
            CorrectAnswer = item.CorrectAnswer,
            ErrorReason = item.ErrorReason,
            Explanation = item.Explanation,
            MasteryStatus = item.MasteryStatus.ToString()
        };
    }
}
