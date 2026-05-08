IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [AgentRouteLog] (
    [Id] nvarchar(64) NOT NULL,
    [UserId] nvarchar(64) NULL,
    [SessionId] nvarchar(64) NULL,
    [QuestionRecordId] nvarchar(64) NULL,
    [InputType] nvarchar(50) NULL,
    [QuestionMode] nvarchar(50) NULL,
    [SelectedAgent] nvarchar(100) NOT NULL,
    [SelectedModelProvider] nvarchar(100) NULL,
    [SelectedModelName] nvarchar(200) NULL,
    [RouteReason] nvarchar(max) NULL,
    [CreatedTime] datetime2 NOT NULL,
    CONSTRAINT [PK_AgentRouteLog] PRIMARY KEY ([Id])
);

CREATE TABLE [AnswerRecord] (
    [Id] nvarchar(64) NOT NULL,
    [QuestionRecordId] nvarchar(64) NOT NULL,
    [UserId] nvarchar(64) NOT NULL,
    [AnswerText] nvarchar(max) NOT NULL,
    [AnswerJson] nvarchar(max) NULL,
    [ModelName] nvarchar(200) NULL,
    [AgentName] nvarchar(200) NULL,
    [PromptVersion] nvarchar(50) NULL,
    [OutputType] nvarchar(50) NULL,
    [AudioPath] nvarchar(500) NULL,
    [VideoPath] nvarchar(500) NULL,
    [AvatarScriptJson] nvarchar(max) NULL,
    [CreatedTime] datetime2 NOT NULL,
    CONSTRAINT [PK_AnswerRecord] PRIMARY KEY ([Id])
);

CREATE TABLE [HomeworkCheckItem] (
    [Id] nvarchar(64) NOT NULL,
    [QuestionRecordId] nvarchar(64) NOT NULL,
    [UserId] nvarchar(64) NOT NULL,
    [Subject] nvarchar(50) NULL,
    [Grade] nvarchar(50) NULL,
    [QuestionNo] nvarchar(50) NULL,
    [QuestionText] nvarchar(max) NULL,
    [StudentAnswer] nvarchar(max) NULL,
    [CorrectAnswer] nvarchar(max) NULL,
    [IsCorrect] bit NULL,
    [ErrorReason] nvarchar(max) NULL,
    [Explanation] nvarchar(max) NULL,
    [KnowledgePointId] nvarchar(64) NULL,
    [ImageCropPath] nvarchar(500) NULL,
    [CreatedTime] datetime2 NOT NULL,
    [UpdatedTime] datetime2 NULL,
    CONSTRAINT [PK_HomeworkCheckItem] PRIMARY KEY ([Id])
);

CREATE TABLE [KnowledgeChunk] (
    [Id] nvarchar(64) NOT NULL,
    [TextbookId] nvarchar(64) NULL,
    [UnitId] nvarchar(64) NULL,
    [LessonId] nvarchar(64) NULL,
    [PageId] nvarchar(64) NULL,
    [KnowledgePointId] nvarchar(64) NULL,
    [ChunkTitle] nvarchar(200) NULL,
    [ChunkText] nvarchar(max) NOT NULL,
    [ChunkType] nvarchar(50) NULL,
    [SourceType] nvarchar(50) NULL,
    [SourcePath] nvarchar(500) NULL,
    [PageNo] int NULL,
    [SortIndex] int NOT NULL,
    [CreatedTime] datetime2 NOT NULL,
    [UpdatedTime] datetime2 NULL,
    CONSTRAINT [PK_KnowledgeChunk] PRIMARY KEY ([Id])
);

CREATE TABLE [KnowledgeEmbedding] (
    [Id] nvarchar(64) NOT NULL,
    [ChunkId] nvarchar(64) NOT NULL,
    [EmbeddingModel] nvarchar(100) NOT NULL,
    [VectorData] nvarchar(max) NULL,
    [VectorHash] nvarchar(128) NULL,
    [CreatedTime] datetime2 NOT NULL,
    CONSTRAINT [PK_KnowledgeEmbedding] PRIMARY KEY ([Id])
);

CREATE TABLE [KnowledgePoint] (
    [Id] nvarchar(64) NOT NULL,
    [TextbookId] nvarchar(64) NULL,
    [UnitId] nvarchar(64) NULL,
    [LessonId] nvarchar(64) NULL,
    [PageId] nvarchar(64) NULL,
    [Subject] nvarchar(50) NOT NULL,
    [Grade] nvarchar(50) NOT NULL,
    [Name] nvarchar(200) NOT NULL,
    [PointType] nvarchar(50) NULL,
    [DifficultyLevel] int NOT NULL,
    [Description] nvarchar(max) NULL,
    [ParentId] nvarchar(64) NULL,
    [SortIndex] int NOT NULL,
    [CreatedTime] datetime2 NOT NULL,
    [UpdatedTime] datetime2 NULL,
    CONSTRAINT [PK_KnowledgePoint] PRIMARY KEY ([Id])
);

CREATE TABLE [LearningProfile] (
    [Id] nvarchar(64) NOT NULL,
    [UserId] nvarchar(64) NOT NULL,
    [Grade] nvarchar(50) NOT NULL,
    [Semester] nvarchar(50) NULL,
    [SchoolName] nvarchar(200) NULL,
    [DefaultSubject] nvarchar(max) NULL,
    [TextbookVersion] nvarchar(100) NULL,
    [CreatedTime] datetime2 NOT NULL,
    [UpdatedTime] datetime2 NULL,
    CONSTRAINT [PK_LearningProfile] PRIMARY KEY ([Id])
);

CREATE TABLE [LearningSession] (
    [Id] nvarchar(64) NOT NULL,
    [UserId] nvarchar(64) NOT NULL,
    [SessionType] nvarchar(50) NOT NULL,
    [Subject] nvarchar(50) NULL,
    [Grade] nvarchar(50) NULL,
    [Title] nvarchar(200) NULL,
    [RelatedQuestionId] nvarchar(64) NULL,
    [RelatedWrongId] nvarchar(64) NULL,
    [StartTime] datetime2 NOT NULL,
    [EndTime] datetime2 NULL,
    [Summary] nvarchar(max) NULL,
    [CreatedTime] datetime2 NOT NULL,
    [UpdatedTime] datetime2 NULL,
    CONSTRAINT [PK_LearningSession] PRIMARY KEY ([Id])
);

CREATE TABLE [Lesson] (
    [Id] nvarchar(64) NOT NULL,
    [TextbookId] nvarchar(64) NOT NULL,
    [UnitId] nvarchar(64) NULL,
    [LessonNo] int NULL,
    [Title] nvarchar(200) NOT NULL,
    [PageStart] int NULL,
    [PageEnd] int NULL,
    [SortIndex] int NOT NULL,
    [CreatedTime] datetime2 NOT NULL,
    [UpdatedTime] datetime2 NULL,
    CONSTRAINT [PK_Lesson] PRIMARY KEY ([Id])
);

CREATE TABLE [MediaResource] (
    [Id] nvarchar(64) NOT NULL,
    [UserId] nvarchar(64) NULL,
    [ResourceType] nvarchar(50) NOT NULL,
    [FileName] nvarchar(200) NULL,
    [FilePath] nvarchar(500) NOT NULL,
    [MimeType] nvarchar(100) NULL,
    [FileSize] bigint NULL,
    [DurationMs] int NULL,
    [Width] int NULL,
    [Height] int NULL,
    [Hash] nvarchar(128) NULL,
    [SourceType] nvarchar(50) NULL,
    [CreatedTime] datetime2 NOT NULL,
    [UpdatedTime] datetime2 NULL,
    CONSTRAINT [PK_MediaResource] PRIMARY KEY ([Id])
);

CREATE TABLE [ModelCallLog] (
    [Id] nvarchar(64) NOT NULL,
    [UserId] nvarchar(64) NULL,
    [SessionId] nvarchar(64) NULL,
    [QuestionRecordId] nvarchar(64) NULL,
    [AgentName] nvarchar(200) NULL,
    [ModelProvider] nvarchar(100) NULL,
    [ModelName] nvarchar(200) NULL,
    [RequestType] nvarchar(50) NULL,
    [PromptText] nvarchar(max) NULL,
    [ResponseText] nvarchar(max) NULL,
    [InputTokens] int NULL,
    [OutputTokens] int NULL,
    [Cost] decimal(18,4) NULL,
    [DurationMs] int NULL,
    [IsSuccess] bit NOT NULL,
    [ErrorMessage] nvarchar(max) NULL,
    [CreatedTime] datetime2 NOT NULL,
    CONSTRAINT [PK_ModelCallLog] PRIMARY KEY ([Id])
);

CREATE TABLE [PracticeAnswerRecord] (
    [Id] nvarchar(64) NOT NULL,
    [UserId] nvarchar(64) NOT NULL,
    [PracticeQuestionId] nvarchar(64) NOT NULL,
    [UserAnswer] nvarchar(max) NULL,
    [IsCorrect] bit NULL,
    [AiFeedback] nvarchar(max) NULL,
    [DurationSeconds] int NULL,
    [CreatedTime] datetime2 NOT NULL,
    CONSTRAINT [PK_PracticeAnswerRecord] PRIMARY KEY ([Id])
);

CREATE TABLE [PracticeQuestion] (
    [Id] nvarchar(64) NOT NULL,
    [UserId] nvarchar(64) NULL,
    [Subject] nvarchar(50) NOT NULL,
    [Grade] nvarchar(50) NULL,
    [KnowledgePointId] nvarchar(64) NULL,
    [WrongQuestionId] nvarchar(64) NULL,
    [QuestionText] nvarchar(max) NOT NULL,
    [QuestionImagePath] nvarchar(500) NULL,
    [CorrectAnswer] nvarchar(max) NULL,
    [Explanation] nvarchar(max) NULL,
    [DifficultyLevel] int NOT NULL,
    [GenerateType] nvarchar(50) NOT NULL,
    [SourceType] nvarchar(50) NULL,
    [ModelName] nvarchar(200) NULL,
    [PromptVersion] nvarchar(50) NULL,
    [CreatedTime] datetime2 NOT NULL,
    [UpdatedTime] datetime2 NULL,
    CONSTRAINT [PK_PracticeQuestion] PRIMARY KEY ([Id])
);

CREATE TABLE [PracticeQuestionSource] (
    [Id] nvarchar(64) NOT NULL,
    [PracticeQuestionId] nvarchar(64) NOT NULL,
    [SourceType] nvarchar(50) NOT NULL,
    [SourceId] nvarchar(64) NOT NULL,
    [CreatedTime] datetime2 NOT NULL,
    CONSTRAINT [PK_PracticeQuestionSource] PRIMARY KEY ([Id])
);

CREATE TABLE [QuestionRecord] (
    [Id] nvarchar(64) NOT NULL,
    [UserId] nvarchar(64) NOT NULL,
    [SessionId] nvarchar(64) NULL,
    [Subject] nvarchar(50) NULL,
    [Grade] nvarchar(50) NULL,
    [InputType] nvarchar(50) NOT NULL,
    [QuestionText] nvarchar(max) NULL,
    [ImagePath] nvarchar(500) NULL,
    [AudioPath] nvarchar(500) NULL,
    [RecognizedText] nvarchar(max) NULL,
    [Mode] nvarchar(50) NULL,
    [SourceType] nvarchar(50) NULL,
    [CreatedTime] datetime2 NOT NULL,
    [UpdatedTime] datetime2 NULL,
    CONSTRAINT [PK_QuestionRecord] PRIMARY KEY ([Id])
);

CREATE TABLE [ReviewSchedule] (
    [Id] nvarchar(64) NOT NULL,
    [UserId] nvarchar(64) NOT NULL,
    [ReviewTargetType] nvarchar(50) NOT NULL,
    [ReviewTargetId] nvarchar(64) NOT NULL,
    [Subject] nvarchar(50) NULL,
    [Grade] nvarchar(50) NULL,
    [KnowledgePointId] nvarchar(64) NULL,
    [ReviewLevel] int NOT NULL,
    [NextReviewTime] datetime2 NOT NULL,
    [LastReviewTime] datetime2 NULL,
    [ReviewCount] int NOT NULL,
    [Status] nvarchar(50) NOT NULL,
    [CreatedTime] datetime2 NOT NULL,
    [UpdatedTime] datetime2 NULL,
    CONSTRAINT [PK_ReviewSchedule] PRIMARY KEY ([Id])
);

CREATE TABLE [SessionMessage] (
    [Id] nvarchar(64) NOT NULL,
    [SessionId] nvarchar(64) NOT NULL,
    [UserId] nvarchar(64) NOT NULL,
    [Role] nvarchar(50) NOT NULL,
    [ContentType] nvarchar(50) NOT NULL,
    [TextContent] nvarchar(max) NULL,
    [ImagePath] nvarchar(500) NULL,
    [AudioPath] nvarchar(500) NULL,
    [VideoPath] nvarchar(500) NULL,
    [MessageJson] nvarchar(max) NULL,
    [CreatedTime] datetime2 NOT NULL,
    CONSTRAINT [PK_SessionMessage] PRIMARY KEY ([Id])
);

CREATE TABLE [StageTask] (
    [Id] nvarchar(64) NOT NULL,
    [StudyStageId] nvarchar(64) NOT NULL,
    [UserId] nvarchar(64) NOT NULL,
    [TaskType] nvarchar(50) NOT NULL,
    [Title] nvarchar(200) NOT NULL,
    [Description] nvarchar(max) NULL,
    [RelatedKnowledgePointId] nvarchar(64) NULL,
    [RelatedWrongQuestionId] nvarchar(64) NULL,
    [RelatedPracticeQuestionId] nvarchar(64) NULL,
    [SortIndex] int NOT NULL,
    [Status] nvarchar(50) NOT NULL,
    [CreatedTime] datetime2 NOT NULL,
    [UpdatedTime] datetime2 NULL,
    CONSTRAINT [PK_StageTask] PRIMARY KEY ([Id])
);

CREATE TABLE [StageTaskRecord] (
    [Id] nvarchar(64) NOT NULL,
    [StageTaskId] nvarchar(64) NOT NULL,
    [UserId] nvarchar(64) NOT NULL,
    [ActionType] nvarchar(50) NOT NULL,
    [Result] nvarchar(50) NULL,
    [Score] decimal(5,2) NULL,
    [DurationSeconds] int NULL,
    [Remark] nvarchar(max) NULL,
    [CreatedTime] datetime2 NOT NULL,
    CONSTRAINT [PK_StageTaskRecord] PRIMARY KEY ([Id])
);

CREATE TABLE [StudyPlan] (
    [Id] nvarchar(64) NOT NULL,
    [UserId] nvarchar(64) NOT NULL,
    [Title] nvarchar(200) NOT NULL,
    [Subject] nvarchar(50) NOT NULL,
    [Grade] nvarchar(50) NULL,
    [PlanType] nvarchar(50) NOT NULL,
    [Status] nvarchar(50) NOT NULL,
    [StartDate] datetime2 NULL,
    [EndDate] datetime2 NULL,
    [Summary] nvarchar(max) NULL,
    [CreatedTime] datetime2 NOT NULL,
    [UpdatedTime] datetime2 NULL,
    CONSTRAINT [PK_StudyPlan] PRIMARY KEY ([Id])
);

CREATE TABLE [StudyStage] (
    [Id] nvarchar(64) NOT NULL,
    [StudyPlanId] nvarchar(64) NOT NULL,
    [StageNo] int NOT NULL,
    [Title] nvarchar(200) NOT NULL,
    [Goal] nvarchar(max) NULL,
    [Status] nvarchar(50) NOT NULL,
    [StartDate] datetime2 NULL,
    [EndDate] datetime2 NULL,
    [CreatedTime] datetime2 NOT NULL,
    [UpdatedTime] datetime2 NULL,
    CONSTRAINT [PK_StudyStage] PRIMARY KEY ([Id])
);

CREATE TABLE [Textbook] (
    [Id] nvarchar(64) NOT NULL,
    [Name] nvarchar(200) NOT NULL,
    [Publisher] nvarchar(200) NULL,
    [Subject] nvarchar(50) NOT NULL,
    [Grade] nvarchar(50) NOT NULL,
    [Semester] nvarchar(50) NULL,
    [Version] nvarchar(100) NULL,
    [CoverImagePath] nvarchar(500) NULL,
    [Description] nvarchar(max) NULL,
    [CreatedTime] datetime2 NOT NULL,
    [UpdatedTime] datetime2 NULL,
    CONSTRAINT [PK_Textbook] PRIMARY KEY ([Id])
);

CREATE TABLE [TextbookImportFile] (
    [Id] nvarchar(64) NOT NULL,
    [ImportJobId] nvarchar(64) NOT NULL,
    [TextbookId] nvarchar(64) NULL,
    [FileName] nvarchar(200) NOT NULL,
    [FilePath] nvarchar(500) NOT NULL,
    [FileType] nvarchar(50) NOT NULL,
    [MimeType] nvarchar(100) NULL,
    [FileSize] bigint NULL,
    [PageCount] int NULL,
    [Hash] nvarchar(128) NULL,
    [CreatedTime] datetime2 NOT NULL,
    CONSTRAINT [PK_TextbookImportFile] PRIMARY KEY ([Id])
);

CREATE TABLE [TextbookImportJob] (
    [Id] nvarchar(64) NOT NULL,
    [TextbookId] nvarchar(64) NULL,
    [UserId] nvarchar(64) NULL,
    [JobName] nvarchar(200) NOT NULL,
    [ImportType] nvarchar(50) NOT NULL,
    [Status] nvarchar(50) NOT NULL,
    [SourceFilePath] nvarchar(500) NULL,
    [TotalPages] int NULL,
    [ParsedPages] int NULL,
    [TotalChunks] int NULL,
    [ErrorMessage] nvarchar(max) NULL,
    [FinishedTime] datetime2 NULL,
    [CreatedTime] datetime2 NOT NULL,
    [UpdatedTime] datetime2 NULL,
    CONSTRAINT [PK_TextbookImportJob] PRIMARY KEY ([Id])
);

CREATE TABLE [TextbookPage] (
    [Id] nvarchar(64) NOT NULL,
    [TextbookId] nvarchar(64) NOT NULL,
    [UnitId] nvarchar(64) NULL,
    [LessonId] nvarchar(64) NULL,
    [PageNo] int NOT NULL,
    [ImagePath] nvarchar(500) NULL,
    [PdfPath] nvarchar(500) NULL,
    [OcrText] nvarchar(max) NULL,
    [CleanText] nvarchar(max) NULL,
    [CreatedTime] datetime2 NOT NULL,
    [UpdatedTime] datetime2 NULL,
    CONSTRAINT [PK_TextbookPage] PRIMARY KEY ([Id])
);

CREATE TABLE [TextbookParseLog] (
    [Id] nvarchar(64) NOT NULL,
    [ImportJobId] nvarchar(64) NOT NULL,
    [TextbookId] nvarchar(64) NULL,
    [PageNo] int NULL,
    [StepName] nvarchar(200) NOT NULL,
    [Status] nvarchar(50) NOT NULL,
    [Message] nvarchar(max) NULL,
    [ErrorDetail] nvarchar(max) NULL,
    [CreatedTime] datetime2 NOT NULL,
    CONSTRAINT [PK_TextbookParseLog] PRIMARY KEY ([Id])
);

CREATE TABLE [TextbookUnit] (
    [Id] nvarchar(64) NOT NULL,
    [TextbookId] nvarchar(64) NOT NULL,
    [UnitNo] int NOT NULL,
    [Title] nvarchar(200) NOT NULL,
    [Description] nvarchar(max) NULL,
    [SortIndex] int NOT NULL,
    [CreatedTime] datetime2 NOT NULL,
    [UpdatedTime] datetime2 NULL,
    CONSTRAINT [PK_TextbookUnit] PRIMARY KEY ([Id])
);

CREATE TABLE [UserProfile] (
    [Id] nvarchar(64) NOT NULL,
    [DisplayName] nvarchar(200) NOT NULL,
    [AvatarPath] nvarchar(500) NULL,
    [UserType] nvarchar(50) NOT NULL,
    [PhoneNumber] nvarchar(50) NULL,
    [CreatedTime] datetime2 NOT NULL,
    [UpdatedTime] datetime2 NULL,
    CONSTRAINT [PK_UserProfile] PRIMARY KEY ([Id])
);

CREATE TABLE [WrongQuestion] (
    [Id] nvarchar(64) NOT NULL,
    [UserId] nvarchar(64) NOT NULL,
    [Subject] nvarchar(50) NOT NULL,
    [Grade] nvarchar(50) NULL,
    [QuestionRecordId] nvarchar(64) NULL,
    [HomeworkCheckItemId] nvarchar(64) NULL,
    [KnowledgePointId] nvarchar(64) NULL,
    [QuestionText] nvarchar(max) NOT NULL,
    [QuestionImagePath] nvarchar(500) NULL,
    [StudentAnswer] nvarchar(max) NULL,
    [CorrectAnswer] nvarchar(max) NULL,
    [ErrorReason] nvarchar(max) NULL,
    [Explanation] nvarchar(max) NULL,
    [DifficultyLevel] int NOT NULL,
    [MasteryStatus] nvarchar(50) NOT NULL,
    [ReviewCount] int NOT NULL,
    [LastReviewTime] datetime2 NULL,
    [NextReviewTime] datetime2 NULL,
    [CreatedTime] datetime2 NOT NULL,
    [UpdatedTime] datetime2 NULL,
    CONSTRAINT [PK_WrongQuestion] PRIMARY KEY ([Id])
);

CREATE TABLE [WrongQuestionReview] (
    [Id] nvarchar(64) NOT NULL,
    [WrongQuestionId] nvarchar(64) NOT NULL,
    [UserId] nvarchar(64) NOT NULL,
    [ReviewType] nvarchar(50) NOT NULL,
    [UserAnswer] nvarchar(max) NULL,
    [IsCorrect] bit NULL,
    [AiFeedback] nvarchar(max) NULL,
    [ReviewDurationSec] int NULL,
    [CreatedTime] datetime2 NOT NULL,
    CONSTRAINT [PK_WrongQuestionReview] PRIMARY KEY ([Id])
);

CREATE INDEX [IX_AgentRouteLog_CreatedTime] ON [AgentRouteLog] ([CreatedTime]);

CREATE INDEX [IX_AgentRouteLog_QuestionRecordId] ON [AgentRouteLog] ([QuestionRecordId]);

CREATE INDEX [IX_AgentRouteLog_SessionId] ON [AgentRouteLog] ([SessionId]);

CREATE INDEX [IX_AgentRouteLog_UserId] ON [AgentRouteLog] ([UserId]);

CREATE INDEX [IX_AnswerRecord_QuestionRecordId] ON [AnswerRecord] ([QuestionRecordId]);

CREATE INDEX [IX_AnswerRecord_UserId] ON [AnswerRecord] ([UserId]);

CREATE INDEX [IX_HomeworkCheckItem_KnowledgePointId] ON [HomeworkCheckItem] ([KnowledgePointId]);

CREATE INDEX [IX_HomeworkCheckItem_QuestionRecordId] ON [HomeworkCheckItem] ([QuestionRecordId]);

CREATE INDEX [IX_HomeworkCheckItem_UserId] ON [HomeworkCheckItem] ([UserId]);

CREATE INDEX [IX_KnowledgeChunk_KnowledgePointId] ON [KnowledgeChunk] ([KnowledgePointId]);

CREATE INDEX [IX_KnowledgeChunk_TextbookId] ON [KnowledgeChunk] ([TextbookId]);

CREATE INDEX [IX_KnowledgeEmbedding_ChunkId] ON [KnowledgeEmbedding] ([ChunkId]);

CREATE INDEX [IX_KnowledgePoint_ParentId] ON [KnowledgePoint] ([ParentId]);

CREATE INDEX [IX_KnowledgePoint_Subject_Grade] ON [KnowledgePoint] ([Subject], [Grade]);

CREATE INDEX [IX_KnowledgePoint_TextbookId] ON [KnowledgePoint] ([TextbookId]);

CREATE INDEX [IX_LearningProfile_UserId] ON [LearningProfile] ([UserId]);

CREATE INDEX [IX_LearningSession_UserId_CreatedTime] ON [LearningSession] ([UserId], [CreatedTime]);

CREATE INDEX [IX_Lesson_TextbookId] ON [Lesson] ([TextbookId]);

CREATE INDEX [IX_Lesson_UnitId] ON [Lesson] ([UnitId]);

CREATE INDEX [IX_MediaResource_UserId_ResourceType] ON [MediaResource] ([UserId], [ResourceType]);

CREATE INDEX [IX_ModelCallLog_CreatedTime] ON [ModelCallLog] ([CreatedTime]);

CREATE INDEX [IX_ModelCallLog_QuestionRecordId] ON [ModelCallLog] ([QuestionRecordId]);

CREATE INDEX [IX_ModelCallLog_SessionId] ON [ModelCallLog] ([SessionId]);

CREATE INDEX [IX_ModelCallLog_UserId] ON [ModelCallLog] ([UserId]);

CREATE INDEX [IX_PracticeAnswerRecord_PracticeQuestionId] ON [PracticeAnswerRecord] ([PracticeQuestionId]);

CREATE INDEX [IX_PracticeAnswerRecord_UserId] ON [PracticeAnswerRecord] ([UserId]);

CREATE INDEX [IX_PracticeQuestion_KnowledgePointId] ON [PracticeQuestion] ([KnowledgePointId]);

CREATE INDEX [IX_PracticeQuestion_UserId] ON [PracticeQuestion] ([UserId]);

CREATE INDEX [IX_PracticeQuestion_WrongQuestionId] ON [PracticeQuestion] ([WrongQuestionId]);

CREATE INDEX [IX_PracticeQuestionSource_PracticeQuestionId] ON [PracticeQuestionSource] ([PracticeQuestionId]);

CREATE INDEX [IX_QuestionRecord_SessionId] ON [QuestionRecord] ([SessionId]);

CREATE INDEX [IX_QuestionRecord_UserId_CreatedTime] ON [QuestionRecord] ([UserId], [CreatedTime]);

CREATE INDEX [IX_ReviewSchedule_KnowledgePointId] ON [ReviewSchedule] ([KnowledgePointId]);

CREATE INDEX [IX_ReviewSchedule_Status] ON [ReviewSchedule] ([Status]);

CREATE INDEX [IX_ReviewSchedule_UserId_NextReviewTime] ON [ReviewSchedule] ([UserId], [NextReviewTime]);

CREATE INDEX [IX_SessionMessage_SessionId_CreatedTime] ON [SessionMessage] ([SessionId], [CreatedTime]);

CREATE INDEX [IX_SessionMessage_UserId] ON [SessionMessage] ([UserId]);

CREATE INDEX [IX_StageTask_RelatedKnowledgePointId] ON [StageTask] ([RelatedKnowledgePointId]);

CREATE INDEX [IX_StageTask_RelatedPracticeQuestionId] ON [StageTask] ([RelatedPracticeQuestionId]);

CREATE INDEX [IX_StageTask_RelatedWrongQuestionId] ON [StageTask] ([RelatedWrongQuestionId]);

CREATE INDEX [IX_StageTask_StudyStageId_Status] ON [StageTask] ([StudyStageId], [Status]);

CREATE INDEX [IX_StageTask_UserId] ON [StageTask] ([UserId]);

CREATE INDEX [IX_StageTaskRecord_StageTaskId] ON [StageTaskRecord] ([StageTaskId]);

CREATE INDEX [IX_StageTaskRecord_UserId] ON [StageTaskRecord] ([UserId]);

CREATE INDEX [IX_StudyPlan_UserId_Status] ON [StudyPlan] ([UserId], [Status]);

CREATE INDEX [IX_StudyStage_Status] ON [StudyStage] ([Status]);

CREATE INDEX [IX_StudyStage_StudyPlanId] ON [StudyStage] ([StudyPlanId]);

CREATE INDEX [IX_Textbook_Subject_Grade] ON [Textbook] ([Subject], [Grade]);

CREATE INDEX [IX_TextbookImportFile_ImportJobId] ON [TextbookImportFile] ([ImportJobId]);

CREATE INDEX [IX_TextbookImportFile_TextbookId] ON [TextbookImportFile] ([TextbookId]);

CREATE INDEX [IX_TextbookImportJob_Status] ON [TextbookImportJob] ([Status]);

CREATE INDEX [IX_TextbookImportJob_TextbookId] ON [TextbookImportJob] ([TextbookId]);

CREATE INDEX [IX_TextbookImportJob_UserId] ON [TextbookImportJob] ([UserId]);

CREATE INDEX [IX_TextbookPage_LessonId] ON [TextbookPage] ([LessonId]);

CREATE INDEX [IX_TextbookPage_TextbookId_PageNo] ON [TextbookPage] ([TextbookId], [PageNo]);

CREATE INDEX [IX_TextbookPage_UnitId] ON [TextbookPage] ([UnitId]);

CREATE INDEX [IX_TextbookParseLog_ImportJobId] ON [TextbookParseLog] ([ImportJobId]);

CREATE INDEX [IX_TextbookParseLog_Status] ON [TextbookParseLog] ([Status]);

CREATE INDEX [IX_TextbookParseLog_TextbookId] ON [TextbookParseLog] ([TextbookId]);

CREATE INDEX [IX_TextbookUnit_TextbookId] ON [TextbookUnit] ([TextbookId]);

CREATE INDEX [IX_WrongQuestion_KnowledgePointId] ON [WrongQuestion] ([KnowledgePointId]);

CREATE INDEX [IX_WrongQuestion_MasteryStatus] ON [WrongQuestion] ([MasteryStatus]);

CREATE INDEX [IX_WrongQuestion_NextReviewTime] ON [WrongQuestion] ([NextReviewTime]);

CREATE INDEX [IX_WrongQuestion_QuestionRecordId] ON [WrongQuestion] ([QuestionRecordId]);

CREATE INDEX [IX_WrongQuestion_UserId_Subject] ON [WrongQuestion] ([UserId], [Subject]);

CREATE INDEX [IX_WrongQuestionReview_UserId] ON [WrongQuestionReview] ([UserId]);

CREATE INDEX [IX_WrongQuestionReview_WrongQuestionId] ON [WrongQuestionReview] ([WrongQuestionId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260508053803_InitialCreate', N'10.0.7');

COMMIT;
GO

