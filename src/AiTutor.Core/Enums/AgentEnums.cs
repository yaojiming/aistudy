namespace AiTutor.Core.Enums;

public enum AgentInputType
{
    Text,
    Image,
    Voice,
    Mixed
}

public enum AgentOutputType
{
    Text,
    TextAndAudio,
    AvatarScript,
    Video,
    StructuredJson
}

public enum SessionType
{
    TextChat,
    PhotoQuestion,
    HomeworkCheck,
    WrongReview,
    TextbookQa,
    Practice,
    StudyPlan,
    VoiceCall,
    AvatarLesson
}

public enum MessageRole
{
    User,
    Assistant,
    System
}

public enum MessageContentType
{
    Text,
    Image,
    Audio,
    Video,
    AvatarScript,
    Mixed
}

public enum QuestionMode
{
    Ask,
    CheckHomework,
    WrongReview,
    TextbookQa,
    PracticeGenerate,
    StudyPlan,
    VoiceChat,
    AvatarExplain
}

public enum ModelProviderType
{
    Mock,
    DeepSeek,
    GlmVision
}
