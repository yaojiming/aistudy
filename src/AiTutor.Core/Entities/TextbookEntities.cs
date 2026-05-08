namespace AiTutor.Core.Entities;

/// <summary>
/// 教材主实体。
/// </summary>
/// <remarks>
/// 保存教材的基础元数据，包括名称、出版社、学科、年级、册别、版本和封面。
/// 教材知识库的数据主线从该实体开始，后续关联单元、课时、页面、知识点和知识片段。
/// </remarks>
public class Textbook : AuditableEntity
{
    public string Name { get; set; } = string.Empty;

    public string? Publisher { get; set; }

    public string Subject { get; set; } = string.Empty;

    public string Grade { get; set; } = string.Empty;

    public string? Semester { get; set; }

    public string? Version { get; set; }

    public string? CoverImagePath { get; set; }

    public string? Description { get; set; }
}

/// <summary>
/// 教材单元实体。
/// </summary>
/// <remarks>
/// 表示一本教材中的单元目录，用于组织课时、页面和知识点。
/// 单元只保存结构信息，不负责解析或 AI 调用。
/// </remarks>
public class TextbookUnit : AuditableEntity
{
    public string TextbookId { get; set; } = string.Empty;

    public int UnitNo { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int SortIndex { get; set; }
}

/// <summary>
/// 教材课时或章节实体。
/// </summary>
/// <remarks>
/// 表示教材中的一课、一节或一个章节，可归属到教材单元并记录页码范围。
/// 后续教材问答和知识点检索可以用它定位内容来源。
/// </remarks>
public class Lesson : AuditableEntity
{
    public string TextbookId { get; set; } = string.Empty;

    public string? UnitId { get; set; }

    public int? LessonNo { get; set; }

    public string Title { get; set; } = string.Empty;

    public int? PageStart { get; set; }

    public int? PageEnd { get; set; }

    public int SortIndex { get; set; }
}

/// <summary>
/// 教材页实体，保存页面图片、PDF、OCR 和清洗文本。
/// </summary>
/// <remarks>
/// 用于承接教材 PDF 或图片解析后的页级结果。
/// OCR 原文与清洗文本会进一步切分为知识片段，并可关联知识点。
/// </remarks>
public class TextbookPage : AuditableEntity
{
    public string TextbookId { get; set; } = string.Empty;

    public string? UnitId { get; set; }

    public string? LessonId { get; set; }

    public int PageNo { get; set; }

    public string? ImagePath { get; set; }

    public string? PdfPath { get; set; }

    public string? OcrText { get; set; }

    public string? CleanText { get; set; }
}

/// <summary>
/// 知识点实体。
/// </summary>
/// <remarks>
/// 表示某个学科、年级下的概念、方法、题型或能力点。
/// 支持父子层级，方便形成教材知识树、错题归因和练习生成依据。
/// </remarks>
public class KnowledgePoint : AuditableEntity
{
    public string? TextbookId { get; set; }

    public string? UnitId { get; set; }

    public string? LessonId { get; set; }

    public string? PageId { get; set; }

    public string Subject { get; set; } = string.Empty;

    public string Grade { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? PointType { get; set; }

    public int DifficultyLevel { get; set; } = 1;

    public string? Description { get; set; }

    public string? ParentId { get; set; }

    public int SortIndex { get; set; }
}

/// <summary>
/// 教材知识片段实体。
/// </summary>
/// <remarks>
/// 保存教材解析后的文本切片，是教材 RAG 和知识检索的基础单元。
/// 片段可以记录来源页码、文件路径和关联知识点，第一阶段先做结构预留。
/// </remarks>
public class KnowledgeChunk : AuditableEntity
{
    public string? TextbookId { get; set; }

    public string? UnitId { get; set; }

    public string? LessonId { get; set; }

    public string? PageId { get; set; }

    public string? KnowledgePointId { get; set; }

    public string? ChunkTitle { get; set; }

    public string ChunkText { get; set; } = string.Empty;

    public string? ChunkType { get; set; }

    public string? SourceType { get; set; }

    public string? SourcePath { get; set; }

    public int? PageNo { get; set; }

    public int SortIndex { get; set; }
}

/// <summary>
/// 知识向量实体。
/// </summary>
/// <remarks>
/// 为后续向量检索预留，记录知识片段对应的向量模型、向量数据和哈希。
/// 第一阶段不实现真实向量数据库，仅保留 SQL Server 中的结构字段。
/// </remarks>
public class KnowledgeEmbedding : AiTutorEntity
{
    public string ChunkId { get; set; } = string.Empty;

    public string EmbeddingModel { get; set; } = string.Empty;

    public string? VectorData { get; set; }

    public string? VectorHash { get; set; }
}
