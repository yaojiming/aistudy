# DATABASE_DESIGN.md

## 一、数据库设计目标

AiTutor 使用 SQL Server。数据库从第一阶段开始支持：

1. 用户学习档案
2. 教材知识库
3. 教材上传与解析
4. 问答和作业检查
5. 错题本
6. 错题自动生成同类练习
7. 根据知识点生成练习
8. 分阶段练习
9. 错题复习计划
10. 学习会话
11. 媒体资源
12. 模型调用日志
13. 后续语音讨论
14. 后续 AI 智能人视频讲解

---

## 二、SQL Server 约定

```text
主键：Id NVARCHAR(64) NOT NULL PRIMARY KEY
时间：CreatedTime DATETIME2 NOT NULL，UpdatedTime DATETIME2 NULL
短状态：NVARCHAR(50)
标题名称：NVARCHAR(200)
文件路径：NVARCHAR(500)
大文本：NVARCHAR(MAX)
布尔：BIT
评分：DECIMAL(5,2)
成本：DECIMAL(18,4)
```

常用索引：UserId、TextbookId、KnowledgePointId、WrongQuestionId、PracticeQuestionId、StudyPlanId、StudyStageId、SessionId、QuestionRecordId、Status、Subject、Grade、CreatedTime、NextReviewTime。

---

## 三、表结构总览

### 用户

1. UserProfile
2. LearningProfile

### 教材知识库

3. Textbook
4. TextbookUnit
5. Lesson
6. TextbookPage
7. KnowledgePoint
8. KnowledgeChunk
9. KnowledgeEmbedding

### 教材导入

10. TextbookImportJob
11. TextbookImportFile
12. TextbookParseLog

### 学习会话

13. LearningSession
14. SessionMessage

### 问答与作业

15. QuestionRecord
16. AnswerRecord
17. HomeworkCheckItem

### 错题与复习

18. WrongQuestion
19. WrongQuestionReview
20. ReviewSchedule

### 练习生成

21. PracticeQuestion
22. PracticeQuestionSource
23. PracticeAnswerRecord

### 分阶段练习

24. StudyPlan
25. StudyStage
26. StageTask
27. StageTaskRecord

### 媒体和日志

28. MediaResource
29. ModelCallLog
30. AgentRouteLog

---

## 四、核心表设计

## 4.1 UserProfile

用户基础信息。

```sql
CREATE TABLE UserProfile (
    Id NVARCHAR(64) NOT NULL PRIMARY KEY,
    DisplayName NVARCHAR(100) NOT NULL,
    AvatarPath NVARCHAR(500) NULL,
    UserType NVARCHAR(50) NOT NULL,
    PhoneNumber NVARCHAR(50) NULL,
    CreatedTime DATETIME2 NOT NULL,
    UpdatedTime DATETIME2 NULL
);
```

---

## 4.2 LearningProfile

学生学习档案。

```sql
CREATE TABLE LearningProfile (
    Id NVARCHAR(64) NOT NULL PRIMARY KEY,
    UserId NVARCHAR(64) NOT NULL,
    Grade NVARCHAR(50) NOT NULL,
    Semester NVARCHAR(50) NULL,
    SchoolName NVARCHAR(200) NULL,
    DefaultSubject NVARCHAR(50) NULL,
    TextbookVersion NVARCHAR(100) NULL,
    CreatedTime DATETIME2 NOT NULL,
    UpdatedTime DATETIME2 NULL
);
```

---

## 4.3 Textbook

教材主表。

```sql
CREATE TABLE Textbook (
    Id NVARCHAR(64) NOT NULL PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    Publisher NVARCHAR(200) NULL,
    Subject NVARCHAR(50) NOT NULL,
    Grade NVARCHAR(50) NOT NULL,
    Semester NVARCHAR(50) NULL,
    Version NVARCHAR(100) NULL,
    CoverImagePath NVARCHAR(500) NULL,
    Description NVARCHAR(MAX) NULL,
    CreatedTime DATETIME2 NOT NULL,
    UpdatedTime DATETIME2 NULL
);
CREATE INDEX IX_Textbook_Subject_Grade ON Textbook(Subject, Grade);
```

---

## 4.4 TextbookUnit

```sql
CREATE TABLE TextbookUnit (
    Id NVARCHAR(64) NOT NULL PRIMARY KEY,
    TextbookId NVARCHAR(64) NOT NULL,
    UnitNo INT NOT NULL,
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    SortIndex INT NOT NULL DEFAULT 0,
    CreatedTime DATETIME2 NOT NULL,
    UpdatedTime DATETIME2 NULL
);
CREATE INDEX IX_TextbookUnit_TextbookId ON TextbookUnit(TextbookId);
```

---

## 4.5 Lesson

```sql
CREATE TABLE Lesson (
    Id NVARCHAR(64) NOT NULL PRIMARY KEY,
    TextbookId NVARCHAR(64) NOT NULL,
    UnitId NVARCHAR(64) NULL,
    LessonNo INT NULL,
    Title NVARCHAR(200) NOT NULL,
    PageStart INT NULL,
    PageEnd INT NULL,
    SortIndex INT NOT NULL DEFAULT 0,
    CreatedTime DATETIME2 NOT NULL,
    UpdatedTime DATETIME2 NULL
);
```

---

## 4.6 TextbookPage

```sql
CREATE TABLE TextbookPage (
    Id NVARCHAR(64) NOT NULL PRIMARY KEY,
    TextbookId NVARCHAR(64) NOT NULL,
    UnitId NVARCHAR(64) NULL,
    LessonId NVARCHAR(64) NULL,
    PageNo INT NOT NULL,
    ImagePath NVARCHAR(500) NULL,
    PdfPath NVARCHAR(500) NULL,
    OcrText NVARCHAR(MAX) NULL,
    CleanText NVARCHAR(MAX) NULL,
    CreatedTime DATETIME2 NOT NULL,
    UpdatedTime DATETIME2 NULL
);
CREATE INDEX IX_TextbookPage_TextbookId_PageNo ON TextbookPage(TextbookId, PageNo);
```

---

## 4.7 KnowledgePoint

```sql
CREATE TABLE KnowledgePoint (
    Id NVARCHAR(64) NOT NULL PRIMARY KEY,
    TextbookId NVARCHAR(64) NULL,
    UnitId NVARCHAR(64) NULL,
    LessonId NVARCHAR(64) NULL,
    PageId NVARCHAR(64) NULL,
    Subject NVARCHAR(50) NOT NULL,
    Grade NVARCHAR(50) NOT NULL,
    Name NVARCHAR(200) NOT NULL,
    PointType NVARCHAR(50) NULL,
    DifficultyLevel INT NOT NULL DEFAULT 1,
    Description NVARCHAR(MAX) NULL,
    ParentId NVARCHAR(64) NULL,
    SortIndex INT NOT NULL DEFAULT 0,
    CreatedTime DATETIME2 NOT NULL,
    UpdatedTime DATETIME2 NULL
);
CREATE INDEX IX_KnowledgePoint_Subject_Grade ON KnowledgePoint(Subject, Grade);
CREATE INDEX IX_KnowledgePoint_ParentId ON KnowledgePoint(ParentId);
```

---

## 4.8 KnowledgeChunk

```sql
CREATE TABLE KnowledgeChunk (
    Id NVARCHAR(64) NOT NULL PRIMARY KEY,
    TextbookId NVARCHAR(64) NULL,
    UnitId NVARCHAR(64) NULL,
    LessonId NVARCHAR(64) NULL,
    PageId NVARCHAR(64) NULL,
    KnowledgePointId NVARCHAR(64) NULL,
    ChunkTitle NVARCHAR(200) NULL,
    ChunkText NVARCHAR(MAX) NOT NULL,
    ChunkType NVARCHAR(50) NULL,
    SourceType NVARCHAR(50) NULL,
    SourcePath NVARCHAR(500) NULL,
    PageNo INT NULL,
    SortIndex INT NOT NULL DEFAULT 0,
    CreatedTime DATETIME2 NOT NULL,
    UpdatedTime DATETIME2 NULL
);
CREATE INDEX IX_KnowledgeChunk_KnowledgePointId ON KnowledgeChunk(KnowledgePointId);
```

---

## 4.9 KnowledgeEmbedding

向量检索预留。

```sql
CREATE TABLE KnowledgeEmbedding (
    Id NVARCHAR(64) NOT NULL PRIMARY KEY,
    ChunkId NVARCHAR(64) NOT NULL,
    EmbeddingModel NVARCHAR(100) NOT NULL,
    VectorData NVARCHAR(MAX) NULL,
    VectorHash NVARCHAR(128) NULL,
    CreatedTime DATETIME2 NOT NULL
);
```

---

## 4.10 TextbookImportJob

教材导入任务。

```sql
CREATE TABLE TextbookImportJob (
    Id NVARCHAR(64) NOT NULL PRIMARY KEY,
    TextbookId NVARCHAR(64) NULL,
    UserId NVARCHAR(64) NULL,
    JobName NVARCHAR(200) NOT NULL,
    ImportType NVARCHAR(50) NOT NULL,
    Status NVARCHAR(50) NOT NULL,
    SourceFilePath NVARCHAR(500) NULL,
    TotalPages INT NULL,
    ParsedPages INT NULL,
    TotalChunks INT NULL,
    ErrorMessage NVARCHAR(MAX) NULL,
    CreatedTime DATETIME2 NOT NULL,
    UpdatedTime DATETIME2 NULL,
    FinishedTime DATETIME2 NULL
);
CREATE INDEX IX_TextbookImportJob_Status ON TextbookImportJob(Status);
```

状态：pending、uploading、uploaded、parsing、ocr_processing、chunking、embedding、completed、failed、cancelled。

---

## 4.11 TextbookImportFile

```sql
CREATE TABLE TextbookImportFile (
    Id NVARCHAR(64) NOT NULL PRIMARY KEY,
    ImportJobId NVARCHAR(64) NOT NULL,
    TextbookId NVARCHAR(64) NULL,
    FileName NVARCHAR(255) NOT NULL,
    FilePath NVARCHAR(500) NOT NULL,
    FileType NVARCHAR(50) NOT NULL,
    MimeType NVARCHAR(100) NULL,
    FileSize BIGINT NULL,
    PageCount INT NULL,
    Hash NVARCHAR(128) NULL,
    CreatedTime DATETIME2 NOT NULL
);
```

---

## 4.12 TextbookParseLog

```sql
CREATE TABLE TextbookParseLog (
    Id NVARCHAR(64) NOT NULL PRIMARY KEY,
    ImportJobId NVARCHAR(64) NOT NULL,
    TextbookId NVARCHAR(64) NULL,
    PageNo INT NULL,
    StepName NVARCHAR(100) NOT NULL,
    Status NVARCHAR(50) NOT NULL,
    Message NVARCHAR(MAX) NULL,
    ErrorDetail NVARCHAR(MAX) NULL,
    CreatedTime DATETIME2 NOT NULL
);
```

---

## 4.13 LearningSession

```sql
CREATE TABLE LearningSession (
    Id NVARCHAR(64) NOT NULL PRIMARY KEY,
    UserId NVARCHAR(64) NOT NULL,
    SessionType NVARCHAR(50) NOT NULL,
    Subject NVARCHAR(50) NULL,
    Grade NVARCHAR(50) NULL,
    Title NVARCHAR(200) NULL,
    RelatedQuestionId NVARCHAR(64) NULL,
    RelatedWrongId NVARCHAR(64) NULL,
    StartTime DATETIME2 NOT NULL,
    EndTime DATETIME2 NULL,
    Summary NVARCHAR(MAX) NULL,
    CreatedTime DATETIME2 NOT NULL,
    UpdatedTime DATETIME2 NULL
);
CREATE INDEX IX_LearningSession_UserId_CreatedTime ON LearningSession(UserId, CreatedTime);
```

---

## 4.14 SessionMessage

```sql
CREATE TABLE SessionMessage (
    Id NVARCHAR(64) NOT NULL PRIMARY KEY,
    SessionId NVARCHAR(64) NOT NULL,
    UserId NVARCHAR(64) NOT NULL,
    Role NVARCHAR(50) NOT NULL,
    ContentType NVARCHAR(50) NOT NULL,
    TextContent NVARCHAR(MAX) NULL,
    ImagePath NVARCHAR(500) NULL,
    AudioPath NVARCHAR(500) NULL,
    VideoPath NVARCHAR(500) NULL,
    MessageJson NVARCHAR(MAX) NULL,
    CreatedTime DATETIME2 NOT NULL
);
CREATE INDEX IX_SessionMessage_SessionId_CreatedTime ON SessionMessage(SessionId, CreatedTime);
```

---

## 4.15 QuestionRecord

```sql
CREATE TABLE QuestionRecord (
    Id NVARCHAR(64) NOT NULL PRIMARY KEY,
    UserId NVARCHAR(64) NOT NULL,
    SessionId NVARCHAR(64) NULL,
    Subject NVARCHAR(50) NULL,
    Grade NVARCHAR(50) NULL,
    InputType NVARCHAR(50) NOT NULL,
    QuestionText NVARCHAR(MAX) NULL,
    ImagePath NVARCHAR(500) NULL,
    AudioPath NVARCHAR(500) NULL,
    RecognizedText NVARCHAR(MAX) NULL,
    Mode NVARCHAR(50) NULL,
    SourceType NVARCHAR(50) NULL,
    CreatedTime DATETIME2 NOT NULL,
    UpdatedTime DATETIME2 NULL
);
CREATE INDEX IX_QuestionRecord_UserId_CreatedTime ON QuestionRecord(UserId, CreatedTime);
```

---

## 4.16 AnswerRecord

```sql
CREATE TABLE AnswerRecord (
    Id NVARCHAR(64) NOT NULL PRIMARY KEY,
    QuestionRecordId NVARCHAR(64) NOT NULL,
    UserId NVARCHAR(64) NOT NULL,
    AnswerText NVARCHAR(MAX) NOT NULL,
    AnswerJson NVARCHAR(MAX) NULL,
    ModelName NVARCHAR(100) NULL,
    AgentName NVARCHAR(100) NULL,
    PromptVersion NVARCHAR(50) NULL,
    OutputType NVARCHAR(50) NULL,
    AudioPath NVARCHAR(500) NULL,
    VideoPath NVARCHAR(500) NULL,
    AvatarScriptJson NVARCHAR(MAX) NULL,
    CreatedTime DATETIME2 NOT NULL
);
CREATE INDEX IX_AnswerRecord_QuestionRecordId ON AnswerRecord(QuestionRecordId);
```

---

## 4.17 HomeworkCheckItem

```sql
CREATE TABLE HomeworkCheckItem (
    Id NVARCHAR(64) NOT NULL PRIMARY KEY,
    QuestionRecordId NVARCHAR(64) NOT NULL,
    UserId NVARCHAR(64) NOT NULL,
    Subject NVARCHAR(50) NULL,
    Grade NVARCHAR(50) NULL,
    QuestionNo NVARCHAR(50) NULL,
    QuestionText NVARCHAR(MAX) NULL,
    StudentAnswer NVARCHAR(MAX) NULL,
    CorrectAnswer NVARCHAR(MAX) NULL,
    IsCorrect BIT NULL,
    ErrorReason NVARCHAR(MAX) NULL,
    Explanation NVARCHAR(MAX) NULL,
    KnowledgePointId NVARCHAR(64) NULL,
    ImageCropPath NVARCHAR(500) NULL,
    CreatedTime DATETIME2 NOT NULL,
    UpdatedTime DATETIME2 NULL
);
CREATE INDEX IX_HomeworkCheckItem_QuestionRecordId ON HomeworkCheckItem(QuestionRecordId);
```

---

## 4.18 WrongQuestion

```sql
CREATE TABLE WrongQuestion (
    Id NVARCHAR(64) NOT NULL PRIMARY KEY,
    UserId NVARCHAR(64) NOT NULL,
    Subject NVARCHAR(50) NOT NULL,
    Grade NVARCHAR(50) NULL,
    QuestionRecordId NVARCHAR(64) NULL,
    HomeworkCheckItemId NVARCHAR(64) NULL,
    KnowledgePointId NVARCHAR(64) NULL,
    QuestionText NVARCHAR(MAX) NOT NULL,
    QuestionImagePath NVARCHAR(500) NULL,
    StudentAnswer NVARCHAR(MAX) NULL,
    CorrectAnswer NVARCHAR(MAX) NULL,
    ErrorReason NVARCHAR(MAX) NULL,
    Explanation NVARCHAR(MAX) NULL,
    DifficultyLevel INT NOT NULL DEFAULT 1,
    MasteryStatus NVARCHAR(50) NOT NULL,
    ReviewCount INT NOT NULL DEFAULT 0,
    LastReviewTime DATETIME2 NULL,
    NextReviewTime DATETIME2 NULL,
    CreatedTime DATETIME2 NOT NULL,
    UpdatedTime DATETIME2 NULL
);
CREATE INDEX IX_WrongQuestion_UserId_Subject ON WrongQuestion(UserId, Subject);
CREATE INDEX IX_WrongQuestion_KnowledgePointId ON WrongQuestion(KnowledgePointId);
```

MasteryStatus：new、learning、reviewing、mastered、ignored。

---

## 4.19 WrongQuestionReview

```sql
CREATE TABLE WrongQuestionReview (
    Id NVARCHAR(64) NOT NULL PRIMARY KEY,
    WrongQuestionId NVARCHAR(64) NOT NULL,
    UserId NVARCHAR(64) NOT NULL,
    ReviewType NVARCHAR(50) NOT NULL,
    UserAnswer NVARCHAR(MAX) NULL,
    IsCorrect BIT NULL,
    AiFeedback NVARCHAR(MAX) NULL,
    ReviewDurationSec INT NULL,
    CreatedTime DATETIME2 NOT NULL
);
```

---

## 4.20 ReviewSchedule

```sql
CREATE TABLE ReviewSchedule (
    Id NVARCHAR(64) NOT NULL PRIMARY KEY,
    UserId NVARCHAR(64) NOT NULL,
    ReviewTargetType NVARCHAR(50) NOT NULL,
    ReviewTargetId NVARCHAR(64) NOT NULL,
    Subject NVARCHAR(50) NULL,
    Grade NVARCHAR(50) NULL,
    KnowledgePointId NVARCHAR(64) NULL,
    ReviewLevel INT NOT NULL DEFAULT 1,
    NextReviewTime DATETIME2 NOT NULL,
    LastReviewTime DATETIME2 NULL,
    ReviewCount INT NOT NULL DEFAULT 0,
    Status NVARCHAR(50) NOT NULL,
    CreatedTime DATETIME2 NOT NULL,
    UpdatedTime DATETIME2 NULL
);
CREATE INDEX IX_ReviewSchedule_UserId_NextReviewTime ON ReviewSchedule(UserId, NextReviewTime);
```

---

## 4.21 PracticeQuestion

```sql
CREATE TABLE PracticeQuestion (
    Id NVARCHAR(64) NOT NULL PRIMARY KEY,
    UserId NVARCHAR(64) NULL,
    Subject NVARCHAR(50) NOT NULL,
    Grade NVARCHAR(50) NULL,
    KnowledgePointId NVARCHAR(64) NULL,
    WrongQuestionId NVARCHAR(64) NULL,
    QuestionText NVARCHAR(MAX) NOT NULL,
    QuestionImagePath NVARCHAR(500) NULL,
    CorrectAnswer NVARCHAR(MAX) NULL,
    Explanation NVARCHAR(MAX) NULL,
    DifficultyLevel INT NOT NULL DEFAULT 1,
    GenerateType NVARCHAR(50) NOT NULL,
    SourceType NVARCHAR(50) NULL,
    ModelName NVARCHAR(100) NULL,
    PromptVersion NVARCHAR(50) NULL,
    CreatedTime DATETIME2 NOT NULL,
    UpdatedTime DATETIME2 NULL
);
CREATE INDEX IX_PracticeQuestion_WrongQuestionId ON PracticeQuestion(WrongQuestionId);
CREATE INDEX IX_PracticeQuestion_KnowledgePointId ON PracticeQuestion(KnowledgePointId);
```

GenerateType：similar_wrong_question、knowledge_point_practice、stage_practice、review_practice、exam_like、manual。

---

## 4.22 PracticeQuestionSource

```sql
CREATE TABLE PracticeQuestionSource (
    Id NVARCHAR(64) NOT NULL PRIMARY KEY,
    PracticeQuestionId NVARCHAR(64) NOT NULL,
    SourceType NVARCHAR(50) NOT NULL,
    SourceId NVARCHAR(64) NOT NULL,
    CreatedTime DATETIME2 NOT NULL
);
```

---

## 4.23 PracticeAnswerRecord

```sql
CREATE TABLE PracticeAnswerRecord (
    Id NVARCHAR(64) NOT NULL PRIMARY KEY,
    UserId NVARCHAR(64) NOT NULL,
    PracticeQuestionId NVARCHAR(64) NOT NULL,
    UserAnswer NVARCHAR(MAX) NULL,
    IsCorrect BIT NULL,
    AiFeedback NVARCHAR(MAX) NULL,
    DurationSeconds INT NULL,
    CreatedTime DATETIME2 NOT NULL
);
```

---

## 4.24 StudyPlan

```sql
CREATE TABLE StudyPlan (
    Id NVARCHAR(64) NOT NULL PRIMARY KEY,
    UserId NVARCHAR(64) NOT NULL,
    Title NVARCHAR(200) NOT NULL,
    Subject NVARCHAR(50) NOT NULL,
    Grade NVARCHAR(50) NULL,
    PlanType NVARCHAR(50) NOT NULL,
    Status NVARCHAR(50) NOT NULL,
    StartDate DATETIME2 NULL,
    EndDate DATETIME2 NULL,
    Summary NVARCHAR(MAX) NULL,
    CreatedTime DATETIME2 NOT NULL,
    UpdatedTime DATETIME2 NULL
);
CREATE INDEX IX_StudyPlan_UserId_Status ON StudyPlan(UserId, Status);
```

---

## 4.25 StudyStage

```sql
CREATE TABLE StudyStage (
    Id NVARCHAR(64) NOT NULL PRIMARY KEY,
    StudyPlanId NVARCHAR(64) NOT NULL,
    StageNo INT NOT NULL,
    Title NVARCHAR(200) NOT NULL,
    Goal NVARCHAR(MAX) NULL,
    Status NVARCHAR(50) NOT NULL,
    StartDate DATETIME2 NULL,
    EndDate DATETIME2 NULL,
    CreatedTime DATETIME2 NOT NULL,
    UpdatedTime DATETIME2 NULL
);
CREATE INDEX IX_StudyStage_StudyPlanId ON StudyStage(StudyPlanId);
```

---

## 4.26 StageTask

```sql
CREATE TABLE StageTask (
    Id NVARCHAR(64) NOT NULL PRIMARY KEY,
    StudyStageId NVARCHAR(64) NOT NULL,
    UserId NVARCHAR(64) NOT NULL,
    TaskType NVARCHAR(50) NOT NULL,
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    RelatedKnowledgePointId NVARCHAR(64) NULL,
    RelatedWrongQuestionId NVARCHAR(64) NULL,
    RelatedPracticeQuestionId NVARCHAR(64) NULL,
    SortIndex INT NOT NULL DEFAULT 0,
    Status NVARCHAR(50) NOT NULL,
    CreatedTime DATETIME2 NOT NULL,
    UpdatedTime DATETIME2 NULL
);
CREATE INDEX IX_StageTask_StudyStageId_Status ON StageTask(StudyStageId, Status);
```

TaskType：read_explanation、watch_avatar_video、voice_discussion、do_practice、redo_wrong_question、review_knowledge_point、take_quiz。

---

## 4.27 StageTaskRecord

```sql
CREATE TABLE StageTaskRecord (
    Id NVARCHAR(64) NOT NULL PRIMARY KEY,
    StageTaskId NVARCHAR(64) NOT NULL,
    UserId NVARCHAR(64) NOT NULL,
    ActionType NVARCHAR(50) NOT NULL,
    Result NVARCHAR(50) NULL,
    Score DECIMAL(5,2) NULL,
    DurationSeconds INT NULL,
    Remark NVARCHAR(MAX) NULL,
    CreatedTime DATETIME2 NOT NULL
);
```

---

## 4.28 MediaResource

```sql
CREATE TABLE MediaResource (
    Id NVARCHAR(64) NOT NULL PRIMARY KEY,
    UserId NVARCHAR(64) NULL,
    ResourceType NVARCHAR(50) NOT NULL,
    FileName NVARCHAR(255) NULL,
    FilePath NVARCHAR(500) NOT NULL,
    MimeType NVARCHAR(100) NULL,
    FileSize BIGINT NULL,
    DurationMs INT NULL,
    Width INT NULL,
    Height INT NULL,
    Hash NVARCHAR(128) NULL,
    SourceType NVARCHAR(50) NULL,
    CreatedTime DATETIME2 NOT NULL,
    UpdatedTime DATETIME2 NULL
);
CREATE INDEX IX_MediaResource_UserId_ResourceType ON MediaResource(UserId, ResourceType);
```

ResourceType：image、audio、video、pdf、textbook_page、homework_photo、question_photo、image_crop、student_voice、ai_answer_audio、avatar_video、avatar_script。

---

## 4.29 ModelCallLog

```sql
CREATE TABLE ModelCallLog (
    Id NVARCHAR(64) NOT NULL PRIMARY KEY,
    UserId NVARCHAR(64) NULL,
    SessionId NVARCHAR(64) NULL,
    QuestionRecordId NVARCHAR(64) NULL,
    AgentName NVARCHAR(100) NULL,
    ModelProvider NVARCHAR(100) NULL,
    ModelName NVARCHAR(100) NULL,
    RequestType NVARCHAR(50) NULL,
    PromptText NVARCHAR(MAX) NULL,
    ResponseText NVARCHAR(MAX) NULL,
    InputTokens INT NULL,
    OutputTokens INT NULL,
    Cost DECIMAL(18,4) NULL,
    DurationMs INT NULL,
    IsSuccess BIT NOT NULL,
    ErrorMessage NVARCHAR(MAX) NULL,
    CreatedTime DATETIME2 NOT NULL
);
CREATE INDEX IX_ModelCallLog_CreatedTime ON ModelCallLog(CreatedTime);
```

---

## 4.30 AgentRouteLog

```sql
CREATE TABLE AgentRouteLog (
    Id NVARCHAR(64) NOT NULL PRIMARY KEY,
    UserId NVARCHAR(64) NULL,
    SessionId NVARCHAR(64) NULL,
    QuestionRecordId NVARCHAR(64) NULL,
    InputType NVARCHAR(50) NULL,
    QuestionMode NVARCHAR(50) NULL,
    SelectedAgent NVARCHAR(100) NOT NULL,
    SelectedModelProvider NVARCHAR(100) NULL,
    SelectedModelName NVARCHAR(100) NULL,
    RouteReason NVARCHAR(MAX) NULL,
    CreatedTime DATETIME2 NOT NULL
);
```

---

## 五、EF Core 实体配置要求

1. 所有 Id：HasMaxLength(64)。
2. 所有 Status：HasMaxLength(50)。
3. Subject / Grade：HasMaxLength(50)。
4. 文件路径：HasMaxLength(500)。
5. 大文本不设置最大长度。
6. CreatedTime 必填。
7. UpdatedTime 可空。
8. 不建议第一阶段添加过多外键约束，避免早期调整困难。
9. 稳定后再逐步增加外键和级联规则。

---

## 六、数据库安全要求

执行数据库更新前必须确认：

1. 当前连接的是 AiTutorDb 开发库。
2. SQL 脚本不包含 DROP DATABASE。
3. SQL 脚本不包含 DROP TABLE。
4. SQL 脚本不包含 TRUNCATE TABLE。
5. 没有真实密码。
6. build 成功。
7. 已备份已有数据。

推荐命令：

```bash
dotnet ef migrations add InitialCreate --project src/AiTutor.Infrastructure --startup-project src/AiTutor.Api

dotnet ef migrations script --project src/AiTutor.Infrastructure --startup-project src/AiTutor.Api -o scripts/sqlserver/InitialCreate.sql

dotnet ef database update --project src/AiTutor.Infrastructure --startup-project src/AiTutor.Api
```
