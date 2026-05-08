using AiTutor.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace AiTutor.Infrastructure.Data;

/// <summary>
/// AiTutor SQL Server 数据库上下文。
/// </summary>
public class AiTutorDbContext : DbContext
{
    public AiTutorDbContext(DbContextOptions<AiTutorDbContext> options)
        : base(options)
    {
    }

    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();

    public DbSet<LearningProfile> LearningProfiles => Set<LearningProfile>();

    public DbSet<Textbook> Textbooks => Set<Textbook>();

    public DbSet<TextbookUnit> TextbookUnits => Set<TextbookUnit>();

    public DbSet<Lesson> Lessons => Set<Lesson>();

    public DbSet<TextbookPage> TextbookPages => Set<TextbookPage>();

    public DbSet<KnowledgePoint> KnowledgePoints => Set<KnowledgePoint>();

    public DbSet<KnowledgeChunk> KnowledgeChunks => Set<KnowledgeChunk>();

    public DbSet<KnowledgeEmbedding> KnowledgeEmbeddings => Set<KnowledgeEmbedding>();

    public DbSet<TextbookImportJob> TextbookImportJobs => Set<TextbookImportJob>();

    public DbSet<TextbookImportFile> TextbookImportFiles => Set<TextbookImportFile>();

    public DbSet<TextbookParseLog> TextbookParseLogs => Set<TextbookParseLog>();

    public DbSet<LearningSession> LearningSessions => Set<LearningSession>();

    public DbSet<SessionMessage> SessionMessages => Set<SessionMessage>();

    public DbSet<QuestionRecord> QuestionRecords => Set<QuestionRecord>();

    public DbSet<AnswerRecord> AnswerRecords => Set<AnswerRecord>();

    public DbSet<HomeworkCheckItem> HomeworkCheckItems => Set<HomeworkCheckItem>();

    public DbSet<WrongQuestion> WrongQuestions => Set<WrongQuestion>();

    public DbSet<WrongQuestionReview> WrongQuestionReviews => Set<WrongQuestionReview>();

    public DbSet<ReviewSchedule> ReviewSchedules => Set<ReviewSchedule>();

    public DbSet<PracticeQuestion> PracticeQuestions => Set<PracticeQuestion>();

    public DbSet<PracticeQuestionSource> PracticeQuestionSources => Set<PracticeQuestionSource>();

    public DbSet<PracticeAnswerRecord> PracticeAnswerRecords => Set<PracticeAnswerRecord>();

    public DbSet<StudyPlan> StudyPlans => Set<StudyPlan>();

    public DbSet<StudyStage> StudyStages => Set<StudyStage>();

    public DbSet<StageTask> StageTasks => Set<StageTask>();

    public DbSet<StageTaskRecord> StageTaskRecords => Set<StageTaskRecord>();

    public DbSet<MediaResource> MediaResources => Set<MediaResource>();

    public DbSet<ModelCallLog> ModelCallLogs => Set<ModelCallLog>();

    public DbSet<AgentRouteLog> AgentRouteLogs => Set<AgentRouteLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureTables(modelBuilder);
        ConfigureCommonColumns(modelBuilder);
        ConfigureStringColumns(modelBuilder);
        ConfigureDecimalColumns(modelBuilder);
        ConfigureEnumColumns(modelBuilder);
        ConfigureIndexes(modelBuilder);
        ConfigureComments(modelBuilder);
    }

    private static void ConfigureTables(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserProfile>().ToTable("UserProfile");
        modelBuilder.Entity<LearningProfile>().ToTable("LearningProfile");
        modelBuilder.Entity<Textbook>().ToTable("Textbook");
        modelBuilder.Entity<TextbookUnit>().ToTable("TextbookUnit");
        modelBuilder.Entity<Lesson>().ToTable("Lesson");
        modelBuilder.Entity<TextbookPage>().ToTable("TextbookPage");
        modelBuilder.Entity<KnowledgePoint>().ToTable("KnowledgePoint");
        modelBuilder.Entity<KnowledgeChunk>().ToTable("KnowledgeChunk");
        modelBuilder.Entity<KnowledgeEmbedding>().ToTable("KnowledgeEmbedding");
        modelBuilder.Entity<TextbookImportJob>().ToTable("TextbookImportJob");
        modelBuilder.Entity<TextbookImportFile>().ToTable("TextbookImportFile");
        modelBuilder.Entity<TextbookParseLog>().ToTable("TextbookParseLog");
        modelBuilder.Entity<LearningSession>().ToTable("LearningSession");
        modelBuilder.Entity<SessionMessage>().ToTable("SessionMessage");
        modelBuilder.Entity<QuestionRecord>().ToTable("QuestionRecord");
        modelBuilder.Entity<AnswerRecord>().ToTable("AnswerRecord");
        modelBuilder.Entity<HomeworkCheckItem>().ToTable("HomeworkCheckItem");
        modelBuilder.Entity<WrongQuestion>().ToTable("WrongQuestion");
        modelBuilder.Entity<WrongQuestionReview>().ToTable("WrongQuestionReview");
        modelBuilder.Entity<ReviewSchedule>().ToTable("ReviewSchedule");
        modelBuilder.Entity<PracticeQuestion>().ToTable("PracticeQuestion");
        modelBuilder.Entity<PracticeQuestionSource>().ToTable("PracticeQuestionSource");
        modelBuilder.Entity<PracticeAnswerRecord>().ToTable("PracticeAnswerRecord");
        modelBuilder.Entity<StudyPlan>().ToTable("StudyPlan");
        modelBuilder.Entity<StudyStage>().ToTable("StudyStage");
        modelBuilder.Entity<StageTask>().ToTable("StageTask");
        modelBuilder.Entity<StageTaskRecord>().ToTable("StageTaskRecord");
        modelBuilder.Entity<MediaResource>().ToTable("MediaResource");
        modelBuilder.Entity<ModelCallLog>().ToTable("ModelCallLog");
        modelBuilder.Entity<AgentRouteLog>().ToTable("AgentRouteLog");
    }

    private static void ConfigureCommonColumns(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var builder = modelBuilder.Entity(entityType.ClrType);

            builder.HasKey(nameof(AiTutorEntity.Id));
            builder.Property(nameof(AiTutorEntity.Id)).HasColumnType("nvarchar(64)").HasMaxLength(64).IsRequired();
            builder.Property(nameof(AiTutorEntity.CreatedTime)).HasColumnType("datetime2").IsRequired();

            if (typeof(AuditableEntity).IsAssignableFrom(entityType.ClrType))
            {
                builder.Property(nameof(AuditableEntity.UpdatedTime)).HasColumnType("datetime2");
            }

            foreach (var property in entityType.ClrType.GetProperties().Where(p => p.PropertyType == typeof(bool) || p.PropertyType == typeof(bool?)))
            {
                builder.Property(property.Name).HasColumnType("bit");
            }

            foreach (var property in entityType.ClrType.GetProperties().Where(p => p.PropertyType == typeof(DateTime) || p.PropertyType == typeof(DateTime?)))
            {
                builder.Property(property.Name).HasColumnType("datetime2");
            }
        }
    }

    private static void ConfigureStringColumns(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var builder = modelBuilder.Entity(entityType.ClrType);

            foreach (var property in entityType.ClrType.GetProperties().Where(p => p.PropertyType == typeof(string)))
            {
                var name = property.Name;

                if (name == "Id" || name.EndsWith("Id", StringComparison.Ordinal) || name is "RelatedWrongId")
                {
                    builder.Property(name).HasColumnType("nvarchar(64)").HasMaxLength(64);
                }
                else if (name.Contains("Path", StringComparison.Ordinal) || name.Contains("Url", StringComparison.Ordinal))
                {
                    builder.Property(name).HasColumnType("nvarchar(500)").HasMaxLength(500);
                }
                else if (name.Contains("Status", StringComparison.Ordinal) || name is "Subject" or "Grade" or "Semester" or "UserType" or "PhoneNumber" or "FileType" or "SourceType" or "QuestionNo" or "ReviewType" or "ActionType" or "Result" or "RequestType" or "PointType" or "ChunkType")
                {
                    builder.Property(name).HasColumnType("nvarchar(50)").HasMaxLength(50);
                }
                else if (name.Contains("Name", StringComparison.Ordinal) || name.Contains("Title", StringComparison.Ordinal) || name is "Publisher" or "SchoolName")
                {
                    builder.Property(name).HasColumnType("nvarchar(200)").HasMaxLength(200);
                }
                else if (name is "Version" or "TextbookVersion" or "EmbeddingModel" or "MimeType" or "ModelName" or "AgentName" or "StepName" or "ModelProvider" or "SelectedAgent" or "SelectedModelProvider" or "SelectedModelName")
                {
                    builder.Property(name).HasColumnType("nvarchar(100)").HasMaxLength(100);
                }
                else if (name is "Hash" or "VectorHash")
                {
                    builder.Property(name).HasColumnType("nvarchar(128)").HasMaxLength(128);
                }
                else if (name is "FileName")
                {
                    builder.Property(name).HasColumnType("nvarchar(255)").HasMaxLength(255);
                }
                else if (name is "PromptVersion" or "OutputType")
                {
                    builder.Property(name).HasColumnType("nvarchar(50)").HasMaxLength(50);
                }
                else
                {
                    builder.Property(name).HasColumnType("nvarchar(max)");
                }
            }
        }
    }

    private static void ConfigureDecimalColumns(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StageTaskRecord>().Property(x => x.Score).HasColumnType("decimal(5,2)");
        modelBuilder.Entity<ModelCallLog>().Property(x => x.Cost).HasColumnType("decimal(18,4)");
    }

    private static void ConfigureEnumColumns(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TextbookImportJob>().Property(x => x.ImportType).HasConversion<string>().HasColumnType("nvarchar(50)").HasMaxLength(50);
        modelBuilder.Entity<TextbookImportJob>().Property(x => x.Status).HasConversion<string>().HasColumnType("nvarchar(50)").HasMaxLength(50);
        modelBuilder.Entity<LearningSession>().Property(x => x.SessionType).HasConversion<string>().HasColumnType("nvarchar(50)").HasMaxLength(50);
        modelBuilder.Entity<SessionMessage>().Property(x => x.Role).HasConversion<string>().HasColumnType("nvarchar(50)").HasMaxLength(50);
        modelBuilder.Entity<SessionMessage>().Property(x => x.ContentType).HasConversion<string>().HasColumnType("nvarchar(50)").HasMaxLength(50);
        modelBuilder.Entity<QuestionRecord>().Property(x => x.InputType).HasConversion<string>().HasColumnType("nvarchar(50)").HasMaxLength(50);
        modelBuilder.Entity<QuestionRecord>().Property(x => x.Mode).HasConversion<string>().HasColumnType("nvarchar(50)").HasMaxLength(50);
        modelBuilder.Entity<AnswerRecord>().Property(x => x.OutputType).HasConversion<string>().HasColumnType("nvarchar(50)").HasMaxLength(50);
        modelBuilder.Entity<WrongQuestion>().Property(x => x.MasteryStatus).HasConversion<string>().HasColumnType("nvarchar(50)").HasMaxLength(50);
        modelBuilder.Entity<ReviewSchedule>().Property(x => x.ReviewTargetType).HasConversion<string>().HasColumnType("nvarchar(50)").HasMaxLength(50);
        modelBuilder.Entity<ReviewSchedule>().Property(x => x.Status).HasConversion<string>().HasColumnType("nvarchar(50)").HasMaxLength(50);
        modelBuilder.Entity<PracticeQuestion>().Property(x => x.GenerateType).HasConversion<string>().HasColumnType("nvarchar(50)").HasMaxLength(50);
        modelBuilder.Entity<PracticeQuestion>().Property(x => x.SourceType).HasConversion<string>().HasColumnType("nvarchar(50)").HasMaxLength(50);
        modelBuilder.Entity<PracticeQuestionSource>().Property(x => x.SourceType).HasConversion<string>().HasColumnType("nvarchar(50)").HasMaxLength(50);
        modelBuilder.Entity<StudyPlan>().Property(x => x.PlanType).HasConversion<string>().HasColumnType("nvarchar(50)").HasMaxLength(50);
        modelBuilder.Entity<StudyPlan>().Property(x => x.Status).HasConversion<string>().HasColumnType("nvarchar(50)").HasMaxLength(50);
        modelBuilder.Entity<StudyStage>().Property(x => x.Status).HasConversion<string>().HasColumnType("nvarchar(50)").HasMaxLength(50);
        modelBuilder.Entity<StageTask>().Property(x => x.TaskType).HasConversion<string>().HasColumnType("nvarchar(50)").HasMaxLength(50);
        modelBuilder.Entity<StageTask>().Property(x => x.Status).HasConversion<string>().HasColumnType("nvarchar(50)").HasMaxLength(50);
        modelBuilder.Entity<MediaResource>().Property(x => x.ResourceType).HasConversion<string>().HasColumnType("nvarchar(50)").HasMaxLength(50);
        modelBuilder.Entity<ModelCallLog>().Property(x => x.ModelProvider).HasConversion<string>().HasColumnType("nvarchar(100)").HasMaxLength(100);
        modelBuilder.Entity<AgentRouteLog>().Property(x => x.InputType).HasConversion<string>().HasColumnType("nvarchar(50)").HasMaxLength(50);
        modelBuilder.Entity<AgentRouteLog>().Property(x => x.QuestionMode).HasConversion<string>().HasColumnType("nvarchar(50)").HasMaxLength(50);
        modelBuilder.Entity<AgentRouteLog>().Property(x => x.SelectedModelProvider).HasConversion<string>().HasColumnType("nvarchar(100)").HasMaxLength(100);
    }

    private static void ConfigureIndexes(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LearningProfile>().HasIndex(x => x.UserId);
        modelBuilder.Entity<Textbook>().HasIndex(x => new { x.Subject, x.Grade });
        modelBuilder.Entity<TextbookUnit>().HasIndex(x => x.TextbookId);
        modelBuilder.Entity<Lesson>().HasIndex(x => x.TextbookId);
        modelBuilder.Entity<Lesson>().HasIndex(x => x.UnitId);
        modelBuilder.Entity<TextbookPage>().HasIndex(x => new { x.TextbookId, x.PageNo });
        modelBuilder.Entity<TextbookPage>().HasIndex(x => x.UnitId);
        modelBuilder.Entity<TextbookPage>().HasIndex(x => x.LessonId);
        modelBuilder.Entity<KnowledgePoint>().HasIndex(x => x.TextbookId);
        modelBuilder.Entity<KnowledgePoint>().HasIndex(x => new { x.Subject, x.Grade });
        modelBuilder.Entity<KnowledgePoint>().HasIndex(x => x.ParentId);
        modelBuilder.Entity<KnowledgeChunk>().HasIndex(x => x.TextbookId);
        modelBuilder.Entity<KnowledgeChunk>().HasIndex(x => x.KnowledgePointId);
        modelBuilder.Entity<KnowledgeEmbedding>().HasIndex(x => x.ChunkId);
        modelBuilder.Entity<TextbookImportJob>().HasIndex(x => x.UserId);
        modelBuilder.Entity<TextbookImportJob>().HasIndex(x => x.TextbookId);
        modelBuilder.Entity<TextbookImportJob>().HasIndex(x => x.Status);
        modelBuilder.Entity<TextbookImportFile>().HasIndex(x => x.ImportJobId);
        modelBuilder.Entity<TextbookImportFile>().HasIndex(x => x.TextbookId);
        modelBuilder.Entity<TextbookParseLog>().HasIndex(x => x.ImportJobId);
        modelBuilder.Entity<TextbookParseLog>().HasIndex(x => x.TextbookId);
        modelBuilder.Entity<TextbookParseLog>().HasIndex(x => x.Status);
        modelBuilder.Entity<LearningSession>().HasIndex(x => new { x.UserId, x.CreatedTime });
        modelBuilder.Entity<SessionMessage>().HasIndex(x => new { x.SessionId, x.CreatedTime });
        modelBuilder.Entity<SessionMessage>().HasIndex(x => x.UserId);
        modelBuilder.Entity<QuestionRecord>().HasIndex(x => new { x.UserId, x.CreatedTime });
        modelBuilder.Entity<QuestionRecord>().HasIndex(x => x.SessionId);
        modelBuilder.Entity<AnswerRecord>().HasIndex(x => x.QuestionRecordId);
        modelBuilder.Entity<AnswerRecord>().HasIndex(x => x.UserId);
        modelBuilder.Entity<HomeworkCheckItem>().HasIndex(x => x.QuestionRecordId);
        modelBuilder.Entity<HomeworkCheckItem>().HasIndex(x => x.UserId);
        modelBuilder.Entity<HomeworkCheckItem>().HasIndex(x => x.KnowledgePointId);
        modelBuilder.Entity<WrongQuestion>().HasIndex(x => new { x.UserId, x.Subject });
        modelBuilder.Entity<WrongQuestion>().HasIndex(x => x.KnowledgePointId);
        modelBuilder.Entity<WrongQuestion>().HasIndex(x => x.QuestionRecordId);
        modelBuilder.Entity<WrongQuestion>().HasIndex(x => x.NextReviewTime);
        modelBuilder.Entity<WrongQuestion>().HasIndex(x => x.MasteryStatus);
        modelBuilder.Entity<WrongQuestionReview>().HasIndex(x => x.WrongQuestionId);
        modelBuilder.Entity<WrongQuestionReview>().HasIndex(x => x.UserId);
        modelBuilder.Entity<ReviewSchedule>().HasIndex(x => new { x.UserId, x.NextReviewTime });
        modelBuilder.Entity<ReviewSchedule>().HasIndex(x => x.KnowledgePointId);
        modelBuilder.Entity<ReviewSchedule>().HasIndex(x => x.Status);
        modelBuilder.Entity<PracticeQuestion>().HasIndex(x => x.WrongQuestionId);
        modelBuilder.Entity<PracticeQuestion>().HasIndex(x => x.KnowledgePointId);
        modelBuilder.Entity<PracticeQuestion>().HasIndex(x => x.UserId);
        modelBuilder.Entity<PracticeQuestionSource>().HasIndex(x => x.PracticeQuestionId);
        modelBuilder.Entity<PracticeAnswerRecord>().HasIndex(x => x.UserId);
        modelBuilder.Entity<PracticeAnswerRecord>().HasIndex(x => x.PracticeQuestionId);
        modelBuilder.Entity<StudyPlan>().HasIndex(x => new { x.UserId, x.Status });
        modelBuilder.Entity<StudyStage>().HasIndex(x => x.StudyPlanId);
        modelBuilder.Entity<StudyStage>().HasIndex(x => x.Status);
        modelBuilder.Entity<StageTask>().HasIndex(x => new { x.StudyStageId, x.Status });
        modelBuilder.Entity<StageTask>().HasIndex(x => x.UserId);
        modelBuilder.Entity<StageTask>().HasIndex(x => x.RelatedKnowledgePointId);
        modelBuilder.Entity<StageTask>().HasIndex(x => x.RelatedWrongQuestionId);
        modelBuilder.Entity<StageTask>().HasIndex(x => x.RelatedPracticeQuestionId);
        modelBuilder.Entity<StageTaskRecord>().HasIndex(x => x.StageTaskId);
        modelBuilder.Entity<StageTaskRecord>().HasIndex(x => x.UserId);
        modelBuilder.Entity<MediaResource>().HasIndex(x => new { x.UserId, x.ResourceType });
        modelBuilder.Entity<ModelCallLog>().HasIndex(x => x.CreatedTime);
        modelBuilder.Entity<ModelCallLog>().HasIndex(x => x.UserId);
        modelBuilder.Entity<ModelCallLog>().HasIndex(x => x.SessionId);
        modelBuilder.Entity<ModelCallLog>().HasIndex(x => x.QuestionRecordId);
        modelBuilder.Entity<AgentRouteLog>().HasIndex(x => x.UserId);
        modelBuilder.Entity<AgentRouteLog>().HasIndex(x => x.SessionId);
        modelBuilder.Entity<AgentRouteLog>().HasIndex(x => x.QuestionRecordId);
        modelBuilder.Entity<AgentRouteLog>().HasIndex(x => x.CreatedTime);
    }

    private static void ConfigureComments(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var builder = modelBuilder.Entity(entityType.ClrType);
            var entityName = entityType.ClrType.Name;

            builder.ToTable(table => table.HasComment(GetTableComment(entityName)));

            foreach (var property in entityType.ClrType.GetProperties().Where(p => p.GetMethod is not null && p.SetMethod is not null))
            {
                builder.Property(property.Name).HasComment(GetColumnComment(entityName, property.Name));
            }
        }
    }

    private static string GetTableComment(string entityName)
    {
        var comments = new Dictionary<string, string>
        {
            ["UserProfile"] = "用户基础信息表，保存学生、家长或维护人员的基础身份信息。",
            ["LearningProfile"] = "学生学习档案表，保存年级、学期、学校、默认学科和教材版本等学习画像。",
            ["Textbook"] = "教材主表，保存教材名称、出版社、学科、年级、册别、版本和封面信息。",
            ["TextbookUnit"] = "教材单元表，按教材组织单元目录和排序信息。",
            ["Lesson"] = "教材课时或章节表，保存课题、页码范围和所属单元。",
            ["TextbookPage"] = "教材页表，保存教材页面图片、PDF 路径、OCR 原文和清洗文本。",
            ["KnowledgePoint"] = "知识点表，保存学科年级下的知识点、层级关系和难度信息。",
            ["KnowledgeChunk"] = "知识片段表，保存教材切片文本及其来源、页码和知识点关联。",
            ["KnowledgeEmbedding"] = "知识向量表，预留向量检索模型、向量数据和哈希信息。",
            ["TextbookImportJob"] = "教材导入任务表，记录教材上传、解析、OCR、切片和向量化流程状态。",
            ["TextbookImportFile"] = "教材导入文件表，记录导入任务关联的 PDF、图片等源文件信息。",
            ["TextbookParseLog"] = "教材解析日志表，记录教材导入过程中的步骤、状态、消息和错误详情。",
            ["LearningSession"] = "学习会话表，记录一次问答、拍照讲题、作业检查或复习过程。",
            ["SessionMessage"] = "会话消息表，记录学习会话中的文本、图片、音频、视频和数字人脚本消息。",
            ["QuestionRecord"] = "提问记录表，保存学生输入的问题文本、图片、音频、识别文本和提问模式。",
            ["AnswerRecord"] = "回答记录表，保存 AI 回答文本、结构化结果、模型、Agent 和多媒体输出。",
            ["HomeworkCheckItem"] = "作业检查明细表，保存一张作业图片拆出的单题检查结果。",
            ["WrongQuestion"] = "错题表，保存错题原题、答案、错因、讲解、掌握状态和复习时间。",
            ["WrongQuestionReview"] = "错题复习记录表，保存学生复习错题时的作答、反馈和耗时。",
            ["ReviewSchedule"] = "复习计划表，保存错题、知识点或练习题的下次复习安排。",
            ["PracticeQuestion"] = "练习题表，保存由错题、知识点、阶段任务或人工生成的练习题。",
            ["PracticeQuestionSource"] = "练习题来源表，记录练习题与错题、知识点、教材切片等来源的关系。",
            ["PracticeAnswerRecord"] = "练习作答记录表，保存学生练习题作答结果、反馈和耗时。",
            ["StudyPlan"] = "学习计划表，保存面向知识点、错题或考试准备的阶段化学习计划。",
            ["StudyStage"] = "学习阶段表，保存学习计划拆分出的阶段目标、状态和时间。",
            ["StageTask"] = "阶段任务表，保存每个学习阶段中的讲解、练习、复习、语音讨论或视频任务。",
            ["StageTaskRecord"] = "阶段任务记录表，保存学生完成阶段任务时的动作、结果、得分和备注。",
            ["MediaResource"] = "媒体资源表，统一管理图片、音频、视频、PDF、裁剪图和数字人脚本资源。",
            ["ModelCallLog"] = "模型调用日志表，记录每次模型调用的输入输出、Token、耗时、成本和错误。",
            ["AgentRouteLog"] = "Agent 路由日志表，记录每次请求选择的 Agent、模型和路由原因。"
        };

        return comments.TryGetValue(entityName, out var comment)
            ? comment
            : $"{entityName} 表。";
    }

    private static string GetColumnComment(string entityName, string propertyName)
    {
        var comments = new Dictionary<string, string>
        {
            ["Id"] = "主键，使用 32 位无分隔符 Guid 字符串。",
            ["CreatedTime"] = "创建时间，使用 UTC 时间记录。",
            ["UpdatedTime"] = "最后更新时间，未更新时为空。",
            ["UserId"] = "用户 Id，关联 UserProfile.Id。",
            ["DisplayName"] = "用户显示名称。",
            ["AvatarPath"] = "用户头像文件路径。",
            ["UserType"] = "用户类型，例如 student、parent 或 admin。",
            ["PhoneNumber"] = "手机号，第一阶段可为空。",
            ["Grade"] = "年级。",
            ["Semester"] = "学期或册别，例如上册、下册。",
            ["SchoolName"] = "学校名称。",
            ["DefaultSubject"] = "默认学习学科。",
            ["TextbookVersion"] = "默认教材版本。",
            ["Name"] = "名称。",
            ["Publisher"] = "出版社。",
            ["Subject"] = "学科。",
            ["Version"] = "版本号或版本描述。",
            ["CoverImagePath"] = "封面图片路径。",
            ["Description"] = "详细说明。",
            ["TextbookId"] = "教材 Id，关联 Textbook.Id。",
            ["UnitId"] = "教材单元 Id，关联 TextbookUnit.Id。",
            ["LessonId"] = "课时或章节 Id，关联 Lesson.Id。",
            ["PageId"] = "教材页 Id，关联 TextbookPage.Id。",
            ["UnitNo"] = "单元序号。",
            ["LessonNo"] = "课时或章节序号。",
            ["PageNo"] = "页码。",
            ["Title"] = "标题。",
            ["PageStart"] = "起始页码。",
            ["PageEnd"] = "结束页码。",
            ["SortIndex"] = "排序值，数值越小越靠前。",
            ["ImagePath"] = "图片文件路径。",
            ["PdfPath"] = "PDF 文件路径。",
            ["OcrText"] = "OCR 识别原文。",
            ["CleanText"] = "清洗后的文本。",
            ["PointType"] = "知识点类型。",
            ["DifficultyLevel"] = "难度等级，默认 1。",
            ["ParentId"] = "父级 Id，用于树形层级。",
            ["KnowledgePointId"] = "知识点 Id，关联 KnowledgePoint.Id。",
            ["ChunkTitle"] = "知识片段标题。",
            ["ChunkText"] = "知识片段正文。",
            ["ChunkType"] = "知识片段类型。",
            ["SourceType"] = "来源类型。",
            ["SourcePath"] = "来源文件或资源路径。",
            ["ChunkId"] = "知识片段 Id，关联 KnowledgeChunk.Id。",
            ["EmbeddingModel"] = "生成向量的模型名称。",
            ["VectorData"] = "向量数据预留字段，第一阶段以文本形式保存。",
            ["VectorHash"] = "向量数据哈希。",
            ["JobName"] = "导入任务名称。",
            ["ImportType"] = "教材导入类型。",
            ["Status"] = "状态。",
            ["SourceFilePath"] = "源文件路径。",
            ["TotalPages"] = "总页数。",
            ["ParsedPages"] = "已解析页数。",
            ["TotalChunks"] = "生成的知识片段总数。",
            ["ErrorMessage"] = "错误消息。",
            ["FinishedTime"] = "任务完成时间。",
            ["ImportJobId"] = "教材导入任务 Id，关联 TextbookImportJob.Id。",
            ["FileName"] = "文件名。",
            ["FilePath"] = "文件路径。",
            ["FileType"] = "文件类型。",
            ["MimeType"] = "MIME 类型。",
            ["FileSize"] = "文件大小，单位字节。",
            ["PageCount"] = "文件页数。",
            ["Hash"] = "文件或内容哈希。",
            ["StepName"] = "处理步骤名称。",
            ["Message"] = "日志消息。",
            ["ErrorDetail"] = "错误详细信息。",
            ["SessionType"] = "学习会话类型。",
            ["RelatedQuestionId"] = "关联问题 Id。",
            ["RelatedWrongId"] = "关联错题 Id。",
            ["StartTime"] = "会话开始时间。",
            ["EndTime"] = "会话结束时间。",
            ["Summary"] = "摘要。",
            ["SessionId"] = "学习会话 Id，关联 LearningSession.Id。",
            ["Role"] = "消息角色。",
            ["ContentType"] = "消息内容类型。",
            ["TextContent"] = "文本内容。",
            ["AudioPath"] = "音频文件路径。",
            ["VideoPath"] = "视频文件路径。",
            ["MessageJson"] = "消息结构化 JSON。",
            ["InputType"] = "Agent 输入类型。",
            ["QuestionText"] = "题目或问题文本。",
            ["RecognizedText"] = "从图片或音频识别出的文本。",
            ["Mode"] = "提问模式。",
            ["QuestionRecordId"] = "提问记录 Id，关联 QuestionRecord.Id。",
            ["AnswerText"] = "AI 回答文本。",
            ["AnswerJson"] = "AI 回答结构化 JSON。",
            ["ModelName"] = "模型名称。",
            ["AgentName"] = "Agent 名称。",
            ["PromptVersion"] = "Prompt 版本。",
            ["OutputType"] = "输出类型。",
            ["AvatarScriptJson"] = "数字人脚本 JSON。",
            ["QuestionNo"] = "题号。",
            ["StudentAnswer"] = "学生答案。",
            ["CorrectAnswer"] = "正确答案。",
            ["IsCorrect"] = "是否正确。",
            ["ErrorReason"] = "错误原因。",
            ["Explanation"] = "讲解内容。",
            ["ImageCropPath"] = "题目截图裁剪路径。",
            ["HomeworkCheckItemId"] = "作业检查明细 Id，关联 HomeworkCheckItem.Id。",
            ["QuestionImagePath"] = "题目图片路径。",
            ["MasteryStatus"] = "掌握状态。",
            ["ReviewCount"] = "复习次数。",
            ["LastReviewTime"] = "上次复习时间。",
            ["NextReviewTime"] = "下次复习时间。",
            ["WrongQuestionId"] = "错题 Id，关联 WrongQuestion.Id。",
            ["ReviewType"] = "复习类型。",
            ["UserAnswer"] = "用户作答内容。",
            ["AiFeedback"] = "AI 反馈。",
            ["ReviewDurationSec"] = "复习耗时，单位秒。",
            ["ReviewTargetType"] = "复习目标类型。",
            ["ReviewTargetId"] = "复习目标 Id。",
            ["ReviewLevel"] = "复习等级。",
            ["PracticeQuestionId"] = "练习题 Id，关联 PracticeQuestion.Id。",
            ["GenerateType"] = "练习题生成类型。",
            ["DurationSeconds"] = "耗时，单位秒。",
            ["PlanType"] = "学习计划类型。",
            ["StartDate"] = "开始日期。",
            ["EndDate"] = "结束日期。",
            ["StudyPlanId"] = "学习计划 Id，关联 StudyPlan.Id。",
            ["StageNo"] = "阶段序号。",
            ["Goal"] = "阶段目标。",
            ["StudyStageId"] = "学习阶段 Id，关联 StudyStage.Id。",
            ["TaskType"] = "阶段任务类型。",
            ["RelatedKnowledgePointId"] = "关联知识点 Id。",
            ["RelatedWrongQuestionId"] = "关联错题 Id。",
            ["RelatedPracticeQuestionId"] = "关联练习题 Id。",
            ["StageTaskId"] = "阶段任务 Id，关联 StageTask.Id。",
            ["ActionType"] = "动作类型。",
            ["Result"] = "执行结果。",
            ["Score"] = "得分。",
            ["Remark"] = "备注。",
            ["ResourceType"] = "媒体资源类型。",
            ["DurationMs"] = "媒体时长，单位毫秒。",
            ["Width"] = "图片或视频宽度。",
            ["Height"] = "图片或视频高度。",
            ["ModelProvider"] = "模型提供方。",
            ["RequestType"] = "请求类型。",
            ["PromptText"] = "发送给模型的 Prompt 文本。",
            ["ResponseText"] = "模型返回文本。",
            ["InputTokens"] = "输入 Token 数。",
            ["OutputTokens"] = "输出 Token 数。",
            ["Cost"] = "模型调用成本。",
            ["IsSuccess"] = "模型调用是否成功。",
            ["SelectedAgent"] = "路由选中的 Agent。",
            ["SelectedModelProvider"] = "路由选中的模型提供方。",
            ["SelectedModelName"] = "路由选中的模型名称。",
            ["QuestionMode"] = "路由识别的提问模式。",
            ["RouteReason"] = "路由原因。"
        };

        return comments.TryGetValue(propertyName, out var comment)
            ? comment
            : $"{GetTableComment(entityName)}字段 {propertyName}。";
    }
}
