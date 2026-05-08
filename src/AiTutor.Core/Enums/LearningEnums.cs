namespace AiTutor.Core.Enums;

public enum MasteryStatus
{
    New,
    Learning,
    Reviewing,
    Mastered,
    Ignored
}

public enum MediaResourceType
{
    Image,
    Audio,
    Video,
    Pdf,
    TextbookPage,
    HomeworkPhoto,
    QuestionPhoto,
    ImageCrop,
    StudentVoice,
    AiAnswerAudio,
    AvatarVideo,
    AvatarScript
}

public enum TextbookImportStatus
{
    Pending,
    Uploading,
    Uploaded,
    Parsing,
    OcrProcessing,
    Chunking,
    Embedding,
    Completed,
    Failed,
    Cancelled
}

public enum TextbookImportType
{
    Pdf,
    Image,
    Mixed
}

public enum PracticeGenerateType
{
    SimilarWrongQuestion,
    KnowledgePointPractice,
    StagePractice,
    ReviewPractice,
    ExamLike,
    Manual
}

public enum PracticeSourceType
{
    WrongQuestion,
    KnowledgePoint,
    TextbookChunk,
    StudyStage,
    Manual
}

public enum StudyPlanType
{
    KnowledgePointPractice,
    WrongQuestionReview,
    StagePractice,
    ExamPreparation,
    Manual
}

public enum StudyPlanStatus
{
    Draft,
    Active,
    Paused,
    Completed,
    Cancelled
}

public enum StageTaskType
{
    ReadExplanation,
    WatchAvatarVideo,
    VoiceDiscussion,
    DoPractice,
    RedoWrongQuestion,
    ReviewKnowledgePoint,
    TakeQuiz
}

public enum StageTaskStatus
{
    Pending,
    InProgress,
    Completed,
    Skipped,
    Cancelled
}

public enum ReviewTargetType
{
    WrongQuestion,
    KnowledgePoint,
    PracticeQuestion
}

public enum ReviewScheduleStatus
{
    Pending,
    Completed,
    Skipped,
    Cancelled
}
