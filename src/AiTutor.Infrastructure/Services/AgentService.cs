using System.Diagnostics;
using System.Text.Json;
using AiTutor.Core.Entities;
using AiTutor.Core.Enums;
using AiTutor.Core.Interfaces;
using AiTutor.Infrastructure.Data;
using AiTutor.Shared.Agent;
using Microsoft.EntityFrameworkCore;

namespace AiTutor.Infrastructure.Services;

/// <summary>
/// Agent 网关服务，负责路由、调用 Mock Agent 和保存关键学习记录。
/// </summary>
public class AgentService : IAgentService
{
    private readonly AiTutorDbContext _dbContext;
    private readonly IAgentRouter _agentRouter;
    private readonly IEnumerable<IAgent> _agents;
    private readonly IAgentRouteLogService _routeLogService;
    private readonly IModelCallLogService _modelCallLogService;
    private readonly IHomeworkCheckService _homeworkCheckService;

    public AgentService(
        AiTutorDbContext dbContext,
        IAgentRouter agentRouter,
        IEnumerable<IAgent> agents,
        IAgentRouteLogService routeLogService,
        IModelCallLogService modelCallLogService,
        IHomeworkCheckService homeworkCheckService)
    {
        _dbContext = dbContext;
        _agentRouter = agentRouter;
        _agents = agents;
        _routeLogService = routeLogService;
        _modelCallLogService = modelCallLogService;
        _homeworkCheckService = homeworkCheckService;
    }

    /// <summary>
    /// 处理一次完整 Agent 请求。
    /// </summary>
    /// <param name="request">统一 Agent 请求。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>统一 Agent 响应。</returns>
    /// <remarks>
    /// 完整调用链：
    /// 1. 校验 UserId；
    /// 2. 创建或复用 LearningSession；
    /// 3. 保存用户 SessionMessage 和 QuestionRecord；
    /// 4. 调用 AgentRouter 得到 AgentRouteResult；
    /// 5. 保存 AgentRouteLog；
    /// 6. 根据路由选择具体 IAgent 并执行 Mock 模型链路；
    /// 7. 保存 AnswerRecord 和助手 SessionMessage；
    /// 8. 如为作业检查，保存 HomeworkCheckItem，并为错题预留 WrongQuestion；
    /// 9. 保存 ModelCallLog；
    /// 10. 返回 AgentResponse 给 Controller。
    /// </remarks>
    public async Task<AgentResponse> AskAsync(AgentRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.UserId))
        {
            throw new ArgumentException("UserId 不能为空。", nameof(request));
        }

        var stopwatch = Stopwatch.StartNew();
        var session = await GetOrCreateSessionAsync(request, cancellationToken);
        var questionRecord = CreateQuestionRecord(request, session.Id);

        _dbContext.QuestionRecords.Add(questionRecord);
        _dbContext.SessionMessages.Add(CreateUserMessage(request, session.Id));
        await _dbContext.SaveChangesAsync(cancellationToken);

        var route = _agentRouter.Route(request);
        await _routeLogService.SaveAsync(new AgentRouteLog
        {
            UserId = request.UserId,
            SessionId = session.Id,
            QuestionRecordId = questionRecord.Id,
            InputType = ParseInputType(request.InputType),
            QuestionMode = ParseQuestionMode(request.Mode),
            SelectedAgent = route.AgentName,
            SelectedModelProvider = ParseModelProvider(route.ModelProvider),
            SelectedModelName = route.ModelName,
            RouteReason = route.RouteReason
        }, cancellationToken);

        var agent = _agents.FirstOrDefault(x => string.Equals(x.Name, route.AgentName, StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException($"未找到 Agent：{route.AgentName}");

        AgentResponse response;
        Exception? modelException = null;
        try
        {
            response = await agent.ExecuteAsync(request, route, cancellationToken);
        }
        catch (Exception ex)
        {
            modelException = ex;
            response = new AgentResponse
            {
                SessionId = session.Id,
                QuestionRecordId = questionRecord.Id,
                OutputType = "text",
                AnswerText = "这次模拟 Agent 调用失败了，请稍后再试。",
                AgentName = route.AgentName,
                ModelUsed = route.ModelName,
                RouteReason = route.RouteReason
            };
        }

        stopwatch.Stop();
        response.SessionId = session.Id;
        response.QuestionRecordId = questionRecord.Id;
        response.DurationMs = (int)stopwatch.ElapsedMilliseconds;

        var answerRecord = CreateAnswerRecord(request, questionRecord.Id, response);
        _dbContext.AnswerRecords.Add(answerRecord);
        _dbContext.SessionMessages.Add(CreateAssistantMessage(request.UserId, session.Id, response));
        session.UpdatedTime = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken);

        response.AnswerRecordId = answerRecord.Id;

        if (response.HomeworkCheckResult is not null)
        {
            await _homeworkCheckService.SaveItemsAsync(questionRecord.Id, request.UserId, request.Subject, request.Grade, response.HomeworkCheckResult, cancellationToken);
        }

        await _modelCallLogService.SaveAsync(new ModelCallLog
        {
            UserId = request.UserId,
            SessionId = session.Id,
            QuestionRecordId = questionRecord.Id,
            AgentName = route.AgentName,
            ModelProvider = ParseModelProvider(route.ModelProvider),
            ModelName = route.ModelName,
            RequestType = request.Mode,
            PromptText = request.QuestionText ?? request.ImageUrl ?? request.AudioUrl,
            ResponseText = response.AnswerText,
            InputTokens = EstimateTokens(request.QuestionText),
            OutputTokens = EstimateTokens(response.AnswerText),
            DurationMs = response.DurationMs,
            IsSuccess = modelException is null,
            ErrorMessage = modelException?.Message
        }, cancellationToken);

        return response;
    }

    /// <summary>
    /// 创建或复用学习会话。
    /// </summary>
    /// <remarks>
    /// 调用链位置：AskAsync 的第一段持久化流程。
    /// 如果前端传入 SessionId 且数据库中存在，则继续沿用原会话；
    /// 否则创建新的 LearningSession，确保每次 Agent 调用都有会话主线。
    /// </remarks>
    private async Task<LearningSession> GetOrCreateSessionAsync(AgentRequest request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(request.SessionId))
        {
            var existing = await _dbContext.LearningSessions.FirstOrDefaultAsync(x => x.Id == request.SessionId, cancellationToken);
            if (existing is not null)
            {
                return existing;
            }
        }

        var session = new LearningSession
        {
            UserId = request.UserId,
            SessionType = ParseSessionType(request.Mode, request.InputType),
            Subject = request.Subject,
            Grade = request.Grade,
            Title = CreateSessionTitle(request),
            RelatedWrongId = request.RelatedWrongQuestionId,
            StartTime = DateTime.UtcNow
        };

        _dbContext.LearningSessions.Add(session);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return session;
    }

    /// <summary>
    /// 根据请求创建问题记录实体。
    /// </summary>
    /// <remarks>
    /// 调用链位置：AskAsync 保存用户问题前。
    /// 该实体承接前端输入的文本、图片、音频、模式和识别文本，
    /// 是 AnswerRecord、HomeworkCheckItem、AgentRouteLog 和 ModelCallLog 的共同关联点。
    /// </remarks>
    private static QuestionRecord CreateQuestionRecord(AgentRequest request, string sessionId)
    {
        return new QuestionRecord
        {
            UserId = request.UserId,
            SessionId = sessionId,
            Subject = request.Subject,
            Grade = request.Grade,
            InputType = ParseInputType(request.InputType),
            QuestionText = request.QuestionText,
            ImagePath = request.ImageUrl,
            AudioPath = request.AudioUrl,
            RecognizedText = string.Equals(request.InputType, "image", StringComparison.OrdinalIgnoreCase) ? "Mock 识别文本：24 ÷ 3 = ?" : null,
            Mode = ParseQuestionMode(request.Mode),
            SourceType = "api"
        };
    }

    /// <summary>
    /// 创建用户侧会话消息。
    /// </summary>
    /// <remarks>
    /// 调用链位置：AskAsync 保存 QuestionRecord 的同时保存。
    /// SessionMessage 用于还原完整对话流，QuestionRecord 用于问题业务主线，
    /// 两者都保存是为了兼顾聊天历史和结构化学习数据。
    /// </remarks>
    private static SessionMessage CreateUserMessage(AgentRequest request, string sessionId)
    {
        return new SessionMessage
        {
            SessionId = sessionId,
            UserId = request.UserId,
            Role = MessageRole.User,
            ContentType = ParseMessageContentType(request.InputType),
            TextContent = request.QuestionText,
            ImagePath = request.ImageUrl,
            AudioPath = request.AudioUrl,
            MessageJson = JsonSerializer.Serialize(request)
        };
    }

    /// <summary>
    /// 创建 AI 助手侧会话消息。
    /// </summary>
    /// <remarks>
    /// 调用链位置：具体 Agent 返回响应后。
    /// 该消息保存 AnswerText 和完整响应 JSON，供后续会话详情页直接展示历史对话。
    /// </remarks>
    private static SessionMessage CreateAssistantMessage(string userId, string sessionId, AgentResponse response)
    {
        return new SessionMessage
        {
            SessionId = sessionId,
            UserId = userId,
            Role = MessageRole.Assistant,
            ContentType = MessageContentType.Text,
            TextContent = response.AnswerText,
            AudioPath = response.AudioUrl,
            VideoPath = response.VideoUrl,
            MessageJson = JsonSerializer.Serialize(response)
        };
    }

    /// <summary>
    /// 创建回答记录实体。
    /// </summary>
    /// <remarks>
    /// 调用链位置：具体 Agent 执行结束后。
    /// AnswerRecord 保存模型名称、Agent 名称、Prompt 版本、结构化 JSON 和多媒体预留字段，
    /// 是后续学习报告、错题生成和数字人讲解的回答来源。
    /// </remarks>
    private static AnswerRecord CreateAnswerRecord(AgentRequest request, string questionRecordId, AgentResponse response)
    {
        return new AnswerRecord
        {
            QuestionRecordId = questionRecordId,
            UserId = request.UserId,
            AnswerText = response.AnswerText,
            AnswerJson = response.AnswerJson is null ? null : JsonSerializer.Serialize(response.AnswerJson),
            ModelName = response.ModelUsed,
            AgentName = response.AgentName,
            PromptVersion = "v1.0",
            OutputType = ParseOutputType(response.OutputType),
            AudioPath = response.AudioUrl,
            VideoPath = response.VideoUrl,
            AvatarScriptJson = response.AvatarScript is null ? null : JsonSerializer.Serialize(response.AvatarScript)
        };
    }

    private static string CreateSessionTitle(AgentRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.QuestionText))
        {
            return request.QuestionText.Length > 30 ? request.QuestionText[..30] : request.QuestionText;
        }

        return request.Mode switch
        {
            "check_homework" => "作业检查",
            "voice_chat" => "语音讨论",
            "avatar_explain" => "数字人讲解",
            _ => "AI 学习会话"
        };
    }

    /// <summary>
    /// 将前端字符串输入类型转换为领域枚举。
    /// </summary>
    /// <remarks>
    /// 调用链位置：创建 QuestionRecord 和 AgentRouteLog 时。
    /// 前端使用 text/image/voice 等轻量字符串，后端入库使用枚举字符串，保持数据库值稳定。
    /// </remarks>
    private static AgentInputType ParseInputType(string? inputType)
    {
        return Normalize(inputType) switch
        {
            "image" => AgentInputType.Image,
            "voice" or "audio" => AgentInputType.Voice,
            "mixed" => AgentInputType.Mixed,
            _ => AgentInputType.Text
        };
    }

    /// <summary>
    /// 将前端 mode 转换为问题模式枚举。
    /// </summary>
    /// <remarks>
    /// 调用链位置：QuestionRecord 和 AgentRouteLog 保存。
    /// 该转换与 AgentRouter 的规则保持一致，方便后续按模式统计和检索。
    /// </remarks>
    private static QuestionMode? ParseQuestionMode(string? mode)
    {
        return Normalize(mode) switch
        {
            "check_homework" => QuestionMode.CheckHomework,
            "wrong_review" => QuestionMode.WrongReview,
            "textbook_qa" => QuestionMode.TextbookQa,
            "practice_generate" => QuestionMode.PracticeGenerate,
            "study_plan" => QuestionMode.StudyPlan,
            "voice_chat" => QuestionMode.VoiceChat,
            "avatar_explain" => QuestionMode.AvatarExplain,
            "ask" => QuestionMode.Ask,
            _ => QuestionMode.Ask
        };
    }

    /// <summary>
    /// 根据 mode 和 inputType 推断会话类型。
    /// </summary>
    /// <remarks>
    /// 调用链位置：创建 LearningSession 时。
    /// 会话类型用于把普通问答、图片讲题、作业检查、错题复习、语音和数字人入口区分开。
    /// </remarks>
    private static SessionType ParseSessionType(string? mode, string? inputType)
    {
        return Normalize(mode) switch
        {
            "check_homework" => SessionType.HomeworkCheck,
            "wrong_review" => SessionType.WrongReview,
            "textbook_qa" => SessionType.TextbookQa,
            "practice_generate" => SessionType.Practice,
            "study_plan" => SessionType.StudyPlan,
            "voice_chat" => SessionType.VoiceCall,
            "avatar_explain" => SessionType.AvatarLesson,
            _ when Normalize(inputType) == "image" => SessionType.PhotoQuestion,
            _ => SessionType.TextChat
        };
    }

    /// <summary>
    /// 将前端输入类型转换为会话消息内容类型。
    /// </summary>
    /// <param name="inputType">前端传入的 text、image、voice、mixed 等输入类型。</param>
    /// <returns>SessionMessage.ContentType 使用的枚举值。</returns>
    /// <remarks>
    /// 调用链：AskAsync -> CreateUserMessage -> ParseMessageContentType。
    /// 该转换用于聊天历史展示；QuestionRecord 使用 ParseInputType 保存问题业务类型，二者职责不同。
    /// </remarks>
    private static MessageContentType ParseMessageContentType(string? inputType)
    {
        return Normalize(inputType) switch
        {
            "image" => MessageContentType.Image,
            "voice" or "audio" => MessageContentType.Audio,
            "mixed" => MessageContentType.Mixed,
            _ => MessageContentType.Text
        };
    }

    /// <summary>
    /// 将 AgentResponse.OutputType 字符串转换为领域枚举。
    /// </summary>
    /// <param name="outputType">Agent 返回的 text、structured_json、avatar_script 等输出类型。</param>
    /// <returns>AnswerRecord.OutputType 使用的枚举值。</returns>
    /// <remarks>
    /// 调用链：AskAsync -> CreateAnswerRecord -> ParseOutputType。
    /// 该转换保证数据库保存的是稳定枚举，前端仍可以使用轻量字符串与 API 交互。
    /// </remarks>
    private static AgentOutputType? ParseOutputType(string? outputType)
    {
        return Normalize(outputType) switch
        {
            "text_and_audio" => AgentOutputType.TextAndAudio,
            "avatar_script" => AgentOutputType.AvatarScript,
            "video" => AgentOutputType.Video,
            "structured_json" => AgentOutputType.StructuredJson,
            "text" => AgentOutputType.Text,
            _ => AgentOutputType.Text
        };
    }

    /// <summary>
    /// 将路由结果中的模型提供方字符串转换为领域枚举。
    /// </summary>
    /// <param name="provider">AgentRouter 写入 AgentRouteResult.ModelProvider 的提供方名称。</param>
    /// <returns>可入库的 ModelProviderType。</returns>
    /// <remarks>
    /// 调用链：AskAsync 保存 AgentRouteLog 和 ModelCallLog 时调用。
    /// Phase 3 支持 Mock、DeepSeek 和 GlmVision 三类 Provider，枚举化保存可以让后续后台统计按模型提供方聚合。
    /// </remarks>
    private static ModelProviderType ParseModelProvider(string? provider)
    {
        return Normalize(provider) switch
        {
            "deepseek" => ModelProviderType.DeepSeek,
            "glmvision" or "glm_vision" or "zhipu" => ModelProviderType.GlmVision,
            _ => ModelProviderType.Mock
        };
    }

    /// <summary>
    /// 粗略估算文本 token 数。
    /// </summary>
    /// <param name="text">待估算的输入或输出文本。</param>
    /// <returns>估算 token 数；空文本返回 0。</returns>
    /// <remarks>
    /// 调用链：AskAsync 创建 ModelCallLog 时调用。
    /// 当前没有接入真实 tokenizer，先用字符数近似，保证日志字段有基础值；后续可以替换为模型对应 tokenizer。
    /// </remarks>
    private static int? EstimateTokens(string? text)
    {
        return string.IsNullOrWhiteSpace(text) ? 0 : Math.Max(1, text.Length / 2);
    }

    /// <summary>
    /// 标准化轻量字符串。
    /// </summary>
    /// <param name="value">前端 mode、inputType、outputType 或 provider 字符串。</param>
    /// <returns>去空格并转小写后的字符串。</returns>
    /// <remarks>
    /// 调用链：本服务所有 Parse* 方法。
    /// 集中标准化可以减少 Swagger 手工输入大小写差异带来的判断错误。
    /// </remarks>
    private static string Normalize(string? value)
    {
        return (value ?? string.Empty).Trim().ToLowerInvariant();
    }
}
