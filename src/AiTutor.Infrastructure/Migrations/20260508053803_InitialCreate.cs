using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AiTutor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AgentRouteLog",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    SessionId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    QuestionRecordId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    InputType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    QuestionMode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SelectedAgent = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SelectedModelProvider = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SelectedModelName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RouteReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgentRouteLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AnswerRecord",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    QuestionRecordId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    AnswerText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AnswerJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModelName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AgentName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PromptVersion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OutputType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AudioPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    VideoPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AvatarScriptJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnswerRecord", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HomeworkCheckItem",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    QuestionRecordId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Grade = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    QuestionNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    QuestionText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StudentAnswer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CorrectAnswer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: true),
                    ErrorReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Explanation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KnowledgePointId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    ImageCropPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeworkCheckItem", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KnowledgeChunk",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    TextbookId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UnitId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    LessonId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    PageId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    KnowledgePointId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    ChunkTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ChunkText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChunkType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SourceType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SourcePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PageNo = table.Column<int>(type: "int", nullable: true),
                    SortIndex = table.Column<int>(type: "int", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KnowledgeChunk", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KnowledgeEmbedding",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    ChunkId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    EmbeddingModel = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    VectorData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VectorHash = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KnowledgeEmbedding", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KnowledgePoint",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    TextbookId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UnitId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    LessonId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    PageId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Subject = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Grade = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PointType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DifficultyLevel = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    SortIndex = table.Column<int>(type: "int", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KnowledgePoint", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LearningProfile",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Grade = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Semester = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SchoolName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DefaultSubject = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TextbookVersion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LearningProfile", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LearningSession",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    SessionType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Grade = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RelatedQuestionId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    RelatedWrongId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Summary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LearningSession", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Lesson",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    TextbookId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    UnitId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    LessonNo = table.Column<int>(type: "int", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PageStart = table.Column<int>(type: "int", nullable: true),
                    PageEnd = table.Column<int>(type: "int", nullable: true),
                    SortIndex = table.Column<int>(type: "int", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lesson", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MediaResource",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    ResourceType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    MimeType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FileSize = table.Column<long>(type: "bigint", nullable: true),
                    DurationMs = table.Column<int>(type: "int", nullable: true),
                    Width = table.Column<int>(type: "int", nullable: true),
                    Height = table.Column<int>(type: "int", nullable: true),
                    Hash = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    SourceType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaResource", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ModelCallLog",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    SessionId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    QuestionRecordId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    AgentName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ModelProvider = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ModelName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RequestType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PromptText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResponseText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InputTokens = table.Column<int>(type: "int", nullable: true),
                    OutputTokens = table.Column<int>(type: "int", nullable: true),
                    Cost = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    DurationMs = table.Column<int>(type: "int", nullable: true),
                    IsSuccess = table.Column<bool>(type: "bit", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModelCallLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PracticeAnswerRecord",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    PracticeQuestionId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    UserAnswer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: true),
                    AiFeedback = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DurationSeconds = table.Column<int>(type: "int", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PracticeAnswerRecord", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PracticeQuestion",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Subject = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Grade = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    KnowledgePointId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    WrongQuestionId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    QuestionText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuestionImagePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CorrectAnswer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Explanation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DifficultyLevel = table.Column<int>(type: "int", nullable: false),
                    GenerateType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SourceType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ModelName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PromptVersion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PracticeQuestion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PracticeQuestionSource",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    PracticeQuestionId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    SourceType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SourceId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PracticeQuestionSource", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QuestionRecord",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    SessionId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Subject = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Grade = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    InputType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    QuestionText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImagePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AudioPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RecognizedText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Mode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SourceType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionRecord", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReviewSchedule",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    ReviewTargetType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ReviewTargetId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Grade = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    KnowledgePointId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    ReviewLevel = table.Column<int>(type: "int", nullable: false),
                    NextReviewTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastReviewTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewCount = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewSchedule", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SessionMessage",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    SessionId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TextContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImagePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AudioPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    VideoPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MessageJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionMessage", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StageTask",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    StudyStageId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    TaskType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RelatedKnowledgePointId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    RelatedWrongQuestionId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    RelatedPracticeQuestionId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    SortIndex = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StageTask", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StageTaskRecord",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    StageTaskId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    ActionType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Result = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Score = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    DurationSeconds = table.Column<int>(type: "int", nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StageTaskRecord", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudyPlan",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Grade = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PlanType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Summary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyPlan", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudyStage",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    StudyPlanId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    StageNo = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Goal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyStage", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Textbook",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Publisher = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Subject = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Grade = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Semester = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Version = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CoverImagePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Textbook", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TextbookImportFile",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    ImportJobId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    TextbookId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FileType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MimeType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FileSize = table.Column<long>(type: "bigint", nullable: true),
                    PageCount = table.Column<int>(type: "int", nullable: true),
                    Hash = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TextbookImportFile", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TextbookImportJob",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    TextbookId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    JobName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ImportType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SourceFilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TotalPages = table.Column<int>(type: "int", nullable: true),
                    ParsedPages = table.Column<int>(type: "int", nullable: true),
                    TotalChunks = table.Column<int>(type: "int", nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FinishedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TextbookImportJob", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TextbookPage",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    TextbookId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    UnitId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    LessonId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    PageNo = table.Column<int>(type: "int", nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PdfPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    OcrText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CleanText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TextbookPage", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TextbookParseLog",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    ImportJobId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    TextbookId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    PageNo = table.Column<int>(type: "int", nullable: true),
                    StepName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ErrorDetail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TextbookParseLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TextbookUnit",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    TextbookId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    UnitNo = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SortIndex = table.Column<int>(type: "int", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TextbookUnit", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserProfile",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AvatarPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UserType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfile", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WrongQuestion",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Grade = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    QuestionRecordId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    HomeworkCheckItemId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    KnowledgePointId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    QuestionText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuestionImagePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    StudentAnswer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CorrectAnswer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ErrorReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Explanation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DifficultyLevel = table.Column<int>(type: "int", nullable: false),
                    MasteryStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ReviewCount = table.Column<int>(type: "int", nullable: false),
                    LastReviewTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NextReviewTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WrongQuestion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WrongQuestionReview",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    WrongQuestionId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    ReviewType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UserAnswer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: true),
                    AiFeedback = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReviewDurationSec = table.Column<int>(type: "int", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WrongQuestionReview", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AgentRouteLog_CreatedTime",
                table: "AgentRouteLog",
                column: "CreatedTime");

            migrationBuilder.CreateIndex(
                name: "IX_AgentRouteLog_QuestionRecordId",
                table: "AgentRouteLog",
                column: "QuestionRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_AgentRouteLog_SessionId",
                table: "AgentRouteLog",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_AgentRouteLog_UserId",
                table: "AgentRouteLog",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AnswerRecord_QuestionRecordId",
                table: "AnswerRecord",
                column: "QuestionRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_AnswerRecord_UserId",
                table: "AnswerRecord",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_HomeworkCheckItem_KnowledgePointId",
                table: "HomeworkCheckItem",
                column: "KnowledgePointId");

            migrationBuilder.CreateIndex(
                name: "IX_HomeworkCheckItem_QuestionRecordId",
                table: "HomeworkCheckItem",
                column: "QuestionRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_HomeworkCheckItem_UserId",
                table: "HomeworkCheckItem",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeChunk_KnowledgePointId",
                table: "KnowledgeChunk",
                column: "KnowledgePointId");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeChunk_TextbookId",
                table: "KnowledgeChunk",
                column: "TextbookId");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeEmbedding_ChunkId",
                table: "KnowledgeEmbedding",
                column: "ChunkId");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgePoint_ParentId",
                table: "KnowledgePoint",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgePoint_Subject_Grade",
                table: "KnowledgePoint",
                columns: new[] { "Subject", "Grade" });

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgePoint_TextbookId",
                table: "KnowledgePoint",
                column: "TextbookId");

            migrationBuilder.CreateIndex(
                name: "IX_LearningProfile_UserId",
                table: "LearningProfile",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_LearningSession_UserId_CreatedTime",
                table: "LearningSession",
                columns: new[] { "UserId", "CreatedTime" });

            migrationBuilder.CreateIndex(
                name: "IX_Lesson_TextbookId",
                table: "Lesson",
                column: "TextbookId");

            migrationBuilder.CreateIndex(
                name: "IX_Lesson_UnitId",
                table: "Lesson",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaResource_UserId_ResourceType",
                table: "MediaResource",
                columns: new[] { "UserId", "ResourceType" });

            migrationBuilder.CreateIndex(
                name: "IX_ModelCallLog_CreatedTime",
                table: "ModelCallLog",
                column: "CreatedTime");

            migrationBuilder.CreateIndex(
                name: "IX_ModelCallLog_QuestionRecordId",
                table: "ModelCallLog",
                column: "QuestionRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_ModelCallLog_SessionId",
                table: "ModelCallLog",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_ModelCallLog_UserId",
                table: "ModelCallLog",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PracticeAnswerRecord_PracticeQuestionId",
                table: "PracticeAnswerRecord",
                column: "PracticeQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_PracticeAnswerRecord_UserId",
                table: "PracticeAnswerRecord",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PracticeQuestion_KnowledgePointId",
                table: "PracticeQuestion",
                column: "KnowledgePointId");

            migrationBuilder.CreateIndex(
                name: "IX_PracticeQuestion_UserId",
                table: "PracticeQuestion",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PracticeQuestion_WrongQuestionId",
                table: "PracticeQuestion",
                column: "WrongQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_PracticeQuestionSource_PracticeQuestionId",
                table: "PracticeQuestionSource",
                column: "PracticeQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionRecord_SessionId",
                table: "QuestionRecord",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionRecord_UserId_CreatedTime",
                table: "QuestionRecord",
                columns: new[] { "UserId", "CreatedTime" });

            migrationBuilder.CreateIndex(
                name: "IX_ReviewSchedule_KnowledgePointId",
                table: "ReviewSchedule",
                column: "KnowledgePointId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewSchedule_Status",
                table: "ReviewSchedule",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewSchedule_UserId_NextReviewTime",
                table: "ReviewSchedule",
                columns: new[] { "UserId", "NextReviewTime" });

            migrationBuilder.CreateIndex(
                name: "IX_SessionMessage_SessionId_CreatedTime",
                table: "SessionMessage",
                columns: new[] { "SessionId", "CreatedTime" });

            migrationBuilder.CreateIndex(
                name: "IX_SessionMessage_UserId",
                table: "SessionMessage",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_StageTask_RelatedKnowledgePointId",
                table: "StageTask",
                column: "RelatedKnowledgePointId");

            migrationBuilder.CreateIndex(
                name: "IX_StageTask_RelatedPracticeQuestionId",
                table: "StageTask",
                column: "RelatedPracticeQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_StageTask_RelatedWrongQuestionId",
                table: "StageTask",
                column: "RelatedWrongQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_StageTask_StudyStageId_Status",
                table: "StageTask",
                columns: new[] { "StudyStageId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_StageTask_UserId",
                table: "StageTask",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_StageTaskRecord_StageTaskId",
                table: "StageTaskRecord",
                column: "StageTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_StageTaskRecord_UserId",
                table: "StageTaskRecord",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyPlan_UserId_Status",
                table: "StudyPlan",
                columns: new[] { "UserId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_StudyStage_Status",
                table: "StudyStage",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_StudyStage_StudyPlanId",
                table: "StudyStage",
                column: "StudyPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_Textbook_Subject_Grade",
                table: "Textbook",
                columns: new[] { "Subject", "Grade" });

            migrationBuilder.CreateIndex(
                name: "IX_TextbookImportFile_ImportJobId",
                table: "TextbookImportFile",
                column: "ImportJobId");

            migrationBuilder.CreateIndex(
                name: "IX_TextbookImportFile_TextbookId",
                table: "TextbookImportFile",
                column: "TextbookId");

            migrationBuilder.CreateIndex(
                name: "IX_TextbookImportJob_Status",
                table: "TextbookImportJob",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_TextbookImportJob_TextbookId",
                table: "TextbookImportJob",
                column: "TextbookId");

            migrationBuilder.CreateIndex(
                name: "IX_TextbookImportJob_UserId",
                table: "TextbookImportJob",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TextbookPage_LessonId",
                table: "TextbookPage",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "IX_TextbookPage_TextbookId_PageNo",
                table: "TextbookPage",
                columns: new[] { "TextbookId", "PageNo" });

            migrationBuilder.CreateIndex(
                name: "IX_TextbookPage_UnitId",
                table: "TextbookPage",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_TextbookParseLog_ImportJobId",
                table: "TextbookParseLog",
                column: "ImportJobId");

            migrationBuilder.CreateIndex(
                name: "IX_TextbookParseLog_Status",
                table: "TextbookParseLog",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_TextbookParseLog_TextbookId",
                table: "TextbookParseLog",
                column: "TextbookId");

            migrationBuilder.CreateIndex(
                name: "IX_TextbookUnit_TextbookId",
                table: "TextbookUnit",
                column: "TextbookId");

            migrationBuilder.CreateIndex(
                name: "IX_WrongQuestion_KnowledgePointId",
                table: "WrongQuestion",
                column: "KnowledgePointId");

            migrationBuilder.CreateIndex(
                name: "IX_WrongQuestion_MasteryStatus",
                table: "WrongQuestion",
                column: "MasteryStatus");

            migrationBuilder.CreateIndex(
                name: "IX_WrongQuestion_NextReviewTime",
                table: "WrongQuestion",
                column: "NextReviewTime");

            migrationBuilder.CreateIndex(
                name: "IX_WrongQuestion_QuestionRecordId",
                table: "WrongQuestion",
                column: "QuestionRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_WrongQuestion_UserId_Subject",
                table: "WrongQuestion",
                columns: new[] { "UserId", "Subject" });

            migrationBuilder.CreateIndex(
                name: "IX_WrongQuestionReview_UserId",
                table: "WrongQuestionReview",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WrongQuestionReview_WrongQuestionId",
                table: "WrongQuestionReview",
                column: "WrongQuestionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AgentRouteLog");

            migrationBuilder.DropTable(
                name: "AnswerRecord");

            migrationBuilder.DropTable(
                name: "HomeworkCheckItem");

            migrationBuilder.DropTable(
                name: "KnowledgeChunk");

            migrationBuilder.DropTable(
                name: "KnowledgeEmbedding");

            migrationBuilder.DropTable(
                name: "KnowledgePoint");

            migrationBuilder.DropTable(
                name: "LearningProfile");

            migrationBuilder.DropTable(
                name: "LearningSession");

            migrationBuilder.DropTable(
                name: "Lesson");

            migrationBuilder.DropTable(
                name: "MediaResource");

            migrationBuilder.DropTable(
                name: "ModelCallLog");

            migrationBuilder.DropTable(
                name: "PracticeAnswerRecord");

            migrationBuilder.DropTable(
                name: "PracticeQuestion");

            migrationBuilder.DropTable(
                name: "PracticeQuestionSource");

            migrationBuilder.DropTable(
                name: "QuestionRecord");

            migrationBuilder.DropTable(
                name: "ReviewSchedule");

            migrationBuilder.DropTable(
                name: "SessionMessage");

            migrationBuilder.DropTable(
                name: "StageTask");

            migrationBuilder.DropTable(
                name: "StageTaskRecord");

            migrationBuilder.DropTable(
                name: "StudyPlan");

            migrationBuilder.DropTable(
                name: "StudyStage");

            migrationBuilder.DropTable(
                name: "Textbook");

            migrationBuilder.DropTable(
                name: "TextbookImportFile");

            migrationBuilder.DropTable(
                name: "TextbookImportJob");

            migrationBuilder.DropTable(
                name: "TextbookPage");

            migrationBuilder.DropTable(
                name: "TextbookParseLog");

            migrationBuilder.DropTable(
                name: "TextbookUnit");

            migrationBuilder.DropTable(
                name: "UserProfile");

            migrationBuilder.DropTable(
                name: "WrongQuestion");

            migrationBuilder.DropTable(
                name: "WrongQuestionReview");
        }
    }
}
