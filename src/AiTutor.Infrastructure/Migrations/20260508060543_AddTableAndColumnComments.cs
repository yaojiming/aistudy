using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AiTutor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTableAndColumnComments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterTable(
                name: "WrongQuestionReview",
                comment: "错题复习记录表，保存学生复习错题时的作答、反馈和耗时。");

            migrationBuilder.AlterTable(
                name: "WrongQuestion",
                comment: "错题表，保存错题原题、答案、错因、讲解、掌握状态和复习时间。");

            migrationBuilder.AlterTable(
                name: "UserProfile",
                comment: "用户基础信息表，保存学生、家长或维护人员的基础身份信息。");

            migrationBuilder.AlterTable(
                name: "TextbookUnit",
                comment: "教材单元表，按教材组织单元目录和排序信息。");

            migrationBuilder.AlterTable(
                name: "TextbookParseLog",
                comment: "教材解析日志表，记录教材导入过程中的步骤、状态、消息和错误详情。");

            migrationBuilder.AlterTable(
                name: "TextbookPage",
                comment: "教材页表，保存教材页面图片、PDF 路径、OCR 原文和清洗文本。");

            migrationBuilder.AlterTable(
                name: "TextbookImportJob",
                comment: "教材导入任务表，记录教材上传、解析、OCR、切片和向量化流程状态。");

            migrationBuilder.AlterTable(
                name: "TextbookImportFile",
                comment: "教材导入文件表，记录导入任务关联的 PDF、图片等源文件信息。");

            migrationBuilder.AlterTable(
                name: "Textbook",
                comment: "教材主表，保存教材名称、出版社、学科、年级、册别、版本和封面信息。");

            migrationBuilder.AlterTable(
                name: "StudyStage",
                comment: "学习阶段表，保存学习计划拆分出的阶段目标、状态和时间。");

            migrationBuilder.AlterTable(
                name: "StudyPlan",
                comment: "学习计划表，保存面向知识点、错题或考试准备的阶段化学习计划。");

            migrationBuilder.AlterTable(
                name: "StageTaskRecord",
                comment: "阶段任务记录表，保存学生完成阶段任务时的动作、结果、得分和备注。");

            migrationBuilder.AlterTable(
                name: "StageTask",
                comment: "阶段任务表，保存每个学习阶段中的讲解、练习、复习、语音讨论或视频任务。");

            migrationBuilder.AlterTable(
                name: "SessionMessage",
                comment: "会话消息表，记录学习会话中的文本、图片、音频、视频和数字人脚本消息。");

            migrationBuilder.AlterTable(
                name: "ReviewSchedule",
                comment: "复习计划表，保存错题、知识点或练习题的下次复习安排。");

            migrationBuilder.AlterTable(
                name: "QuestionRecord",
                comment: "提问记录表，保存学生输入的问题文本、图片、音频、识别文本和提问模式。");

            migrationBuilder.AlterTable(
                name: "PracticeQuestionSource",
                comment: "练习题来源表，记录练习题与错题、知识点、教材切片等来源的关系。");

            migrationBuilder.AlterTable(
                name: "PracticeQuestion",
                comment: "练习题表，保存由错题、知识点、阶段任务或人工生成的练习题。");

            migrationBuilder.AlterTable(
                name: "PracticeAnswerRecord",
                comment: "练习作答记录表，保存学生练习题作答结果、反馈和耗时。");

            migrationBuilder.AlterTable(
                name: "ModelCallLog",
                comment: "模型调用日志表，记录每次模型调用的输入输出、Token、耗时、成本和错误。");

            migrationBuilder.AlterTable(
                name: "MediaResource",
                comment: "媒体资源表，统一管理图片、音频、视频、PDF、裁剪图和数字人脚本资源。");

            migrationBuilder.AlterTable(
                name: "Lesson",
                comment: "教材课时或章节表，保存课题、页码范围和所属单元。");

            migrationBuilder.AlterTable(
                name: "LearningSession",
                comment: "学习会话表，记录一次问答、拍照讲题、作业检查或复习过程。");

            migrationBuilder.AlterTable(
                name: "LearningProfile",
                comment: "学生学习档案表，保存年级、学期、学校、默认学科和教材版本等学习画像。");

            migrationBuilder.AlterTable(
                name: "KnowledgePoint",
                comment: "知识点表，保存学科年级下的知识点、层级关系和难度信息。");

            migrationBuilder.AlterTable(
                name: "KnowledgeEmbedding",
                comment: "知识向量表，预留向量检索模型、向量数据和哈希信息。");

            migrationBuilder.AlterTable(
                name: "KnowledgeChunk",
                comment: "知识片段表，保存教材切片文本及其来源、页码和知识点关联。");

            migrationBuilder.AlterTable(
                name: "HomeworkCheckItem",
                comment: "作业检查明细表，保存一张作业图片拆出的单题检查结果。");

            migrationBuilder.AlterTable(
                name: "AnswerRecord",
                comment: "回答记录表，保存 AI 回答文本、结构化结果、模型、Agent 和多媒体输出。");

            migrationBuilder.AlterTable(
                name: "AgentRouteLog",
                comment: "Agent 路由日志表，记录每次请求选择的 Agent、模型和路由原因。");

            migrationBuilder.AlterColumn<string>(
                name: "WrongQuestionId",
                table: "WrongQuestionReview",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                comment: "错题 Id，关联 WrongQuestion.Id。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "WrongQuestionReview",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                comment: "用户 Id，关联 UserProfile.Id。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "UserAnswer",
                table: "WrongQuestionReview",
                type: "nvarchar(max)",
                nullable: true,
                comment: "用户作答内容。",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ReviewType",
                table: "WrongQuestionReview",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "复习类型。",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "ReviewDurationSec",
                table: "WrongQuestionReview",
                type: "int",
                nullable: true,
                comment: "复习耗时，单位秒。",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsCorrect",
                table: "WrongQuestionReview",
                type: "bit",
                nullable: true,
                comment: "是否正确。",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedTime",
                table: "WrongQuestionReview",
                type: "datetime2",
                nullable: false,
                comment: "创建时间，使用 UTC 时间记录。",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "AiFeedback",
                table: "WrongQuestionReview",
                type: "nvarchar(max)",
                nullable: true,
                comment: "AI 反馈。",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "WrongQuestionReview",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                comment: "主键，使用 32 位无分隔符 Guid 字符串。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "WrongQuestion",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                comment: "用户 Id，关联 UserProfile.Id。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedTime",
                table: "WrongQuestion",
                type: "datetime2",
                nullable: true,
                comment: "最后更新时间，未更新时为空。",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Subject",
                table: "WrongQuestion",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "学科。",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "StudentAnswer",
                table: "WrongQuestion",
                type: "nvarchar(max)",
                nullable: true,
                comment: "学生答案。",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ReviewCount",
                table: "WrongQuestion",
                type: "int",
                nullable: false,
                comment: "复习次数。",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "QuestionText",
                table: "WrongQuestion",
                type: "nvarchar(max)",
                nullable: false,
                comment: "题目或问题文本。",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "QuestionRecordId",
                table: "WrongQuestion",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                comment: "提问记录 Id，关联 QuestionRecord.Id。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "QuestionImagePath",
                table: "WrongQuestion",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                comment: "题目图片路径。",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "NextReviewTime",
                table: "WrongQuestion",
                type: "datetime2",
                nullable: true,
                comment: "下次复习时间。",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MasteryStatus",
                table: "WrongQuestion",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "掌握状态。",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastReviewTime",
                table: "WrongQuestion",
                type: "datetime2",
                nullable: true,
                comment: "上次复习时间。",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "KnowledgePointId",
                table: "WrongQuestion",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                comment: "知识点 Id，关联 KnowledgePoint.Id。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "HomeworkCheckItemId",
                table: "WrongQuestion",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                comment: "作业检查明细 Id，关联 HomeworkCheckItem.Id。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Grade",
                table: "WrongQuestion",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "年级。",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Explanation",
                table: "WrongQuestion",
                type: "nvarchar(max)",
                nullable: true,
                comment: "讲解内容。",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ErrorReason",
                table: "WrongQuestion",
                type: "nvarchar(max)",
                nullable: true,
                comment: "错误原因。",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DifficultyLevel",
                table: "WrongQuestion",
                type: "int",
                nullable: false,
                comment: "难度等级，默认 1。",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedTime",
                table: "WrongQuestion",
                type: "datetime2",
                nullable: false,
                comment: "创建时间，使用 UTC 时间记录。",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "CorrectAnswer",
                table: "WrongQuestion",
                type: "nvarchar(max)",
                nullable: true,
                comment: "正确答案。",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "WrongQuestion",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                comment: "主键，使用 32 位无分隔符 Guid 字符串。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "UserType",
                table: "UserProfile",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "用户类型，例如 student、parent 或 admin。",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedTime",
                table: "UserProfile",
                type: "datetime2",
                nullable: true,
                comment: "最后更新时间，未更新时为空。",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "UserProfile",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "手机号，第一阶段可为空。",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DisplayName",
                table: "UserProfile",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                comment: "用户显示名称。",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedTime",
                table: "UserProfile",
                type: "datetime2",
                nullable: false,
                comment: "创建时间，使用 UTC 时间记录。",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "AvatarPath",
                table: "UserProfile",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                comment: "用户头像文件路径。",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "UserProfile",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                comment: "主键，使用 32 位无分隔符 Guid 字符串。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedTime",
                table: "TextbookUnit",
                type: "datetime2",
                nullable: true,
                comment: "最后更新时间，未更新时为空。",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UnitNo",
                table: "TextbookUnit",
                type: "int",
                nullable: false,
                comment: "单元序号。",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "TextbookUnit",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                comment: "标题。",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "TextbookId",
                table: "TextbookUnit",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                comment: "教材 Id，关联 Textbook.Id。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<int>(
                name: "SortIndex",
                table: "TextbookUnit",
                type: "int",
                nullable: false,
                comment: "排序值，数值越小越靠前。",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "TextbookUnit",
                type: "nvarchar(max)",
                nullable: true,
                comment: "详细说明。",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedTime",
                table: "TextbookUnit",
                type: "datetime2",
                nullable: false,
                comment: "创建时间，使用 UTC 时间记录。",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "TextbookUnit",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                comment: "主键，使用 32 位无分隔符 Guid 字符串。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "TextbookId",
                table: "TextbookParseLog",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                comment: "教材 Id，关联 Textbook.Id。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "StepName",
                table: "TextbookParseLog",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                comment: "处理步骤名称。",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "TextbookParseLog",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "状态。",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "PageNo",
                table: "TextbookParseLog",
                type: "int",
                nullable: true,
                comment: "页码。",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "TextbookParseLog",
                type: "nvarchar(max)",
                nullable: true,
                comment: "日志消息。",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ImportJobId",
                table: "TextbookParseLog",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                comment: "教材导入任务 Id，关联 TextbookImportJob.Id。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "ErrorDetail",
                table: "TextbookParseLog",
                type: "nvarchar(max)",
                nullable: true,
                comment: "错误详细信息。",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedTime",
                table: "TextbookParseLog",
                type: "datetime2",
                nullable: false,
                comment: "创建时间，使用 UTC 时间记录。",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "TextbookParseLog",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                comment: "主键，使用 32 位无分隔符 Guid 字符串。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedTime",
                table: "TextbookPage",
                type: "datetime2",
                nullable: true,
                comment: "最后更新时间，未更新时为空。",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UnitId",
                table: "TextbookPage",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                comment: "教材单元 Id，关联 TextbookUnit.Id。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TextbookId",
                table: "TextbookPage",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                comment: "教材 Id，关联 Textbook.Id。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "PdfPath",
                table: "TextbookPage",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                comment: "PDF 文件路径。",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PageNo",
                table: "TextbookPage",
                type: "int",
                nullable: false,
                comment: "页码。",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "OcrText",
                table: "TextbookPage",
                type: "nvarchar(max)",
                nullable: true,
                comment: "OCR 识别原文。",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LessonId",
                table: "TextbookPage",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                comment: "课时或章节 Id，关联 Lesson.Id。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ImagePath",
                table: "TextbookPage",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                comment: "图片文件路径。",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedTime",
                table: "TextbookPage",
                type: "datetime2",
                nullable: false,
                comment: "创建时间，使用 UTC 时间记录。",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "CleanText",
                table: "TextbookPage",
                type: "nvarchar(max)",
                nullable: true,
                comment: "清洗后的文本。",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "TextbookPage",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                comment: "主键，使用 32 位无分隔符 Guid 字符串。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "TextbookImportJob",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                comment: "用户 Id，关联 UserProfile.Id。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedTime",
                table: "TextbookImportJob",
                type: "datetime2",
                nullable: true,
                comment: "最后更新时间，未更新时为空。",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TotalPages",
                table: "TextbookImportJob",
                type: "int",
                nullable: true,
                comment: "总页数。",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TotalChunks",
                table: "TextbookImportJob",
                type: "int",
                nullable: true,
                comment: "生成的知识片段总数。",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TextbookId",
                table: "TextbookImportJob",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                comment: "教材 Id，关联 Textbook.Id。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "TextbookImportJob",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "状态。",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "SourceFilePath",
                table: "TextbookImportJob",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                comment: "源文件路径。",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ParsedPages",
                table: "TextbookImportJob",
                type: "int",
                nullable: true,
                comment: "已解析页数。",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "JobName",
                table: "TextbookImportJob",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                comment: "导入任务名称。",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "ImportType",
                table: "TextbookImportJob",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "教材导入类型。",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FinishedTime",
                table: "TextbookImportJob",
                type: "datetime2",
                nullable: true,
                comment: "任务完成时间。",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ErrorMessage",
                table: "TextbookImportJob",
                type: "nvarchar(max)",
                nullable: true,
                comment: "错误消息。",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedTime",
                table: "TextbookImportJob",
                type: "datetime2",
                nullable: false,
                comment: "创建时间，使用 UTC 时间记录。",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "TextbookImportJob",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                comment: "主键，使用 32 位无分隔符 Guid 字符串。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "TextbookId",
                table: "TextbookImportFile",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                comment: "教材 Id，关联 Textbook.Id。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PageCount",
                table: "TextbookImportFile",
                type: "int",
                nullable: true,
                comment: "文件页数。",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MimeType",
                table: "TextbookImportFile",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "MIME 类型。",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ImportJobId",
                table: "TextbookImportFile",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                comment: "教材导入任务 Id，关联 TextbookImportJob.Id。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "Hash",
                table: "TextbookImportFile",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                comment: "文件或内容哈希。",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FileType",
                table: "TextbookImportFile",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "文件类型。",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<long>(
                name: "FileSize",
                table: "TextbookImportFile",
                type: "bigint",
                nullable: true,
                comment: "文件大小，单位字节。",
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FilePath",
                table: "TextbookImportFile",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                comment: "文件路径。",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "TextbookImportFile",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                comment: "文件名。",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedTime",
                table: "TextbookImportFile",
                type: "datetime2",
                nullable: false,
                comment: "创建时间，使用 UTC 时间记录。",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "TextbookImportFile",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                comment: "主键，使用 32 位无分隔符 Guid 字符串。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "Version",
                table: "Textbook",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "版本号或版本描述。",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedTime",
                table: "Textbook",
                type: "datetime2",
                nullable: true,
                comment: "最后更新时间，未更新时为空。",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Subject",
                table: "Textbook",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "学科。",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Semester",
                table: "Textbook",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "学期或册别，例如上册、下册。",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Publisher",
                table: "Textbook",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "出版社。",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Textbook",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                comment: "名称。",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Grade",
                table: "Textbook",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "年级。",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Textbook",
                type: "nvarchar(max)",
                nullable: true,
                comment: "详细说明。",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedTime",
                table: "Textbook",
                type: "datetime2",
                nullable: false,
                comment: "创建时间，使用 UTC 时间记录。",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "CoverImagePath",
                table: "Textbook",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                comment: "封面图片路径。",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Textbook",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                comment: "主键，使用 32 位无分隔符 Guid 字符串。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedTime",
                table: "StudyStage",
                type: "datetime2",
                nullable: true,
                comment: "最后更新时间，未更新时为空。",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "StudyStage",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                comment: "标题。",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "StudyPlanId",
                table: "StudyStage",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                comment: "学习计划 Id，关联 StudyPlan.Id。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "StudyStage",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "状态。",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "StudyStage",
                type: "datetime2",
                nullable: true,
                comment: "开始日期。",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "StageNo",
                table: "StudyStage",
                type: "int",
                nullable: false,
                comment: "阶段序号。",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Goal",
                table: "StudyStage",
                type: "nvarchar(max)",
                nullable: true,
                comment: "阶段目标。",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "StudyStage",
                type: "datetime2",
                nullable: true,
                comment: "结束日期。",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedTime",
                table: "StudyStage",
                type: "datetime2",
                nullable: false,
                comment: "创建时间，使用 UTC 时间记录。",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "StudyStage",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                comment: "主键，使用 32 位无分隔符 Guid 字符串。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "StudyPlan",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                comment: "用户 Id，关联 UserProfile.Id。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedTime",
                table: "StudyPlan",
                type: "datetime2",
                nullable: true,
                comment: "最后更新时间，未更新时为空。",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "StudyPlan",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                comment: "标题。",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Summary",
                table: "StudyPlan",
                type: "nvarchar(max)",
                nullable: true,
                comment: "摘要。",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Subject",
                table: "StudyPlan",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "学科。",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "StudyPlan",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "状态。",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "StudyPlan",
                type: "datetime2",
                nullable: true,
                comment: "开始日期。",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PlanType",
                table: "StudyPlan",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "学习计划类型。",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Grade",
                table: "StudyPlan",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "年级。",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "StudyPlan",
                type: "datetime2",
                nullable: true,
                comment: "结束日期。",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedTime",
                table: "StudyPlan",
                type: "datetime2",
                nullable: false,
                comment: "创建时间，使用 UTC 时间记录。",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "StudyPlan",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                comment: "主键，使用 32 位无分隔符 Guid 字符串。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "StageTaskRecord",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                comment: "用户 Id，关联 UserProfile.Id。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "StageTaskId",
                table: "StageTaskRecord",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                comment: "阶段任务 Id，关联 StageTask.Id。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<decimal>(
                name: "Score",
                table: "StageTaskRecord",
                type: "decimal(5,2)",
                nullable: true,
                comment: "得分。",
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Result",
                table: "StageTaskRecord",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "执行结果。",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Remark",
                table: "StageTaskRecord",
                type: "nvarchar(max)",
                nullable: true,
                comment: "备注。",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DurationSeconds",
                table: "StageTaskRecord",
                type: "int",
                nullable: true,
                comment: "耗时，单位秒。",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedTime",
                table: "StageTaskRecord",
                type: "datetime2",
                nullable: false,
                comment: "创建时间，使用 UTC 时间记录。",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "ActionType",
                table: "StageTaskRecord",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "动作类型。",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "StageTaskRecord",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                comment: "主键，使用 32 位无分隔符 Guid 字符串。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "StageTask",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                comment: "用户 Id，关联 UserProfile.Id。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedTime",
                table: "StageTask",
                type: "datetime2",
                nullable: true,
                comment: "最后更新时间，未更新时为空。",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "StageTask",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                comment: "标题。",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "TaskType",
                table: "StageTask",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "阶段任务类型。",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "StudyStageId",
                table: "StageTask",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                comment: "学习阶段 Id，关联 StudyStage.Id。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "StageTask",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "状态。",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "SortIndex",
                table: "StageTask",
                type: "int",
                nullable: false,
                comment: "排序值，数值越小越靠前。",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "RelatedWrongQuestionId",
                table: "StageTask",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                comment: "关联错题 Id。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RelatedPracticeQuestionId",
                table: "StageTask",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                comment: "关联练习题 Id。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RelatedKnowledgePointId",
                table: "StageTask",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                comment: "关联知识点 Id。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "StageTask",
                type: "nvarchar(max)",
                nullable: true,
                comment: "详细说明。",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedTime",
                table: "StageTask",
                type: "datetime2",
                nullable: false,
                comment: "创建时间，使用 UTC 时间记录。",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "StageTask",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                comment: "主键，使用 32 位无分隔符 Guid 字符串。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "VideoPath",
                table: "SessionMessage",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                comment: "视频文件路径。",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "SessionMessage",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                comment: "用户 Id，关联 UserProfile.Id。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "TextContent",
                table: "SessionMessage",
                type: "nvarchar(max)",
                nullable: true,
                comment: "文本内容。",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SessionId",
                table: "SessionMessage",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                comment: "学习会话 Id，关联 LearningSession.Id。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "SessionMessage",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "消息角色。",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "MessageJson",
                table: "SessionMessage",
                type: "nvarchar(max)",
                nullable: true,
                comment: "消息结构化 JSON。",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ImagePath",
                table: "SessionMessage",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                comment: "图片文件路径。",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedTime",
                table: "SessionMessage",
                type: "datetime2",
                nullable: false,
                comment: "创建时间，使用 UTC 时间记录。",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "ContentType",
                table: "SessionMessage",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "消息内容类型。",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "AudioPath",
                table: "SessionMessage",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                comment: "音频文件路径。",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "SessionMessage",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                comment: "主键，使用 32 位无分隔符 Guid 字符串。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ReviewSchedule",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                comment: "用户 Id，关联 UserProfile.Id。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedTime",
                table: "ReviewSchedule",
                type: "datetime2",
                nullable: true,
                comment: "最后更新时间，未更新时为空。",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Subject",
                table: "ReviewSchedule",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "学科。",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "ReviewSchedule",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "状态。",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "ReviewTargetType",
                table: "ReviewSchedule",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "复习目标类型。",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "ReviewTargetId",
                table: "ReviewSchedule",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                comment: "复习目标 Id。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<int>(
                name: "ReviewLevel",
                table: "ReviewSchedule",
                type: "int",
                nullable: false,
                comment: "复习等级。",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ReviewCount",
                table: "ReviewSchedule",
                type: "int",
                nullable: false,
                comment: "复习次数。",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NextReviewTime",
                table: "ReviewSchedule",
                type: "datetime2",
                nullable: false,
                comment: "下次复习时间。",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastReviewTime",
                table: "ReviewSchedule",
                type: "datetime2",
                nullable: true,
                comment: "上次复习时间。",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "KnowledgePointId",
                table: "ReviewSchedule",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                comment: "知识点 Id，关联 KnowledgePoint.Id。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Grade",
                table: "ReviewSchedule",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "年级。",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedTime",
                table: "ReviewSchedule",
                type: "datetime2",
                nullable: false,
                comment: "创建时间，使用 UTC 时间记录。",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "ReviewSchedule",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                comment: "主键，使用 32 位无分隔符 Guid 字符串。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "QuestionRecord",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                comment: "用户 Id，关联 UserProfile.Id。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedTime",
                table: "QuestionRecord",
                type: "datetime2",
                nullable: true,
                comment: "最后更新时间，未更新时为空。",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Subject",
                table: "QuestionRecord",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "学科。",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SourceType",
                table: "QuestionRecord",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "来源类型。",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SessionId",
                table: "QuestionRecord",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                comment: "学习会话 Id，关联 LearningSession.Id。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RecognizedText",
                table: "QuestionRecord",
                type: "nvarchar(max)",
                nullable: true,
                comment: "从图片或音频识别出的文本。",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "QuestionText",
                table: "QuestionRecord",
                type: "nvarchar(max)",
                nullable: true,
                comment: "题目或问题文本。",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Mode",
                table: "QuestionRecord",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "提问模式。",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InputType",
                table: "QuestionRecord",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "Agent 输入类型。",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "ImagePath",
                table: "QuestionRecord",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                comment: "图片文件路径。",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Grade",
                table: "QuestionRecord",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "年级。",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedTime",
                table: "QuestionRecord",
                type: "datetime2",
                nullable: false,
                comment: "创建时间，使用 UTC 时间记录。",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "AudioPath",
                table: "QuestionRecord",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                comment: "音频文件路径。",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "QuestionRecord",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                comment: "主键，使用 32 位无分隔符 Guid 字符串。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "SourceType",
                table: "PracticeQuestionSource",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "来源类型。",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "SourceId",
                table: "PracticeQuestionSource",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                comment: "练习题来源表，记录练习题与错题、知识点、教材切片等来源的关系。字段 SourceId。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "PracticeQuestionId",
                table: "PracticeQuestionSource",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                comment: "练习题 Id，关联 PracticeQuestion.Id。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedTime",
                table: "PracticeQuestionSource",
                type: "datetime2",
                nullable: false,
                comment: "创建时间，使用 UTC 时间记录。",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "PracticeQuestionSource",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                comment: "主键，使用 32 位无分隔符 Guid 字符串。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "WrongQuestionId",
                table: "PracticeQuestion",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                comment: "错题 Id，关联 WrongQuestion.Id。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "PracticeQuestion",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                comment: "用户 Id，关联 UserProfile.Id。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedTime",
                table: "PracticeQuestion",
                type: "datetime2",
                nullable: true,
                comment: "最后更新时间，未更新时为空。",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Subject",
                table: "PracticeQuestion",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "学科。",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "SourceType",
                table: "PracticeQuestion",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "来源类型。",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "QuestionText",
                table: "PracticeQuestion",
                type: "nvarchar(max)",
                nullable: false,
                comment: "题目或问题文本。",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "QuestionImagePath",
                table: "PracticeQuestion",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                comment: "题目图片路径。",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PromptVersion",
                table: "PracticeQuestion",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "Prompt 版本。",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ModelName",
                table: "PracticeQuestion",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "模型名称。",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "KnowledgePointId",
                table: "PracticeQuestion",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                comment: "知识点 Id，关联 KnowledgePoint.Id。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Grade",
                table: "PracticeQuestion",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "年级。",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "GenerateType",
                table: "PracticeQuestion",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "练习题生成类型。",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Explanation",
                table: "PracticeQuestion",
                type: "nvarchar(max)",
                nullable: true,
                comment: "讲解内容。",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DifficultyLevel",
                table: "PracticeQuestion",
                type: "int",
                nullable: false,
                comment: "难度等级，默认 1。",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedTime",
                table: "PracticeQuestion",
                type: "datetime2",
                nullable: false,
                comment: "创建时间，使用 UTC 时间记录。",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "CorrectAnswer",
                table: "PracticeQuestion",
                type: "nvarchar(max)",
                nullable: true,
                comment: "正确答案。",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "PracticeQuestion",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                comment: "主键，使用 32 位无分隔符 Guid 字符串。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "PracticeAnswerRecord",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                comment: "用户 Id，关联 UserProfile.Id。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "UserAnswer",
                table: "PracticeAnswerRecord",
                type: "nvarchar(max)",
                nullable: true,
                comment: "用户作答内容。",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PracticeQuestionId",
                table: "PracticeAnswerRecord",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                comment: "练习题 Id，关联 PracticeQuestion.Id。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<bool>(
                name: "IsCorrect",
                table: "PracticeAnswerRecord",
                type: "bit",
                nullable: true,
                comment: "是否正确。",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DurationSeconds",
                table: "PracticeAnswerRecord",
                type: "int",
                nullable: true,
                comment: "耗时，单位秒。",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedTime",
                table: "PracticeAnswerRecord",
                type: "datetime2",
                nullable: false,
                comment: "创建时间，使用 UTC 时间记录。",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "AiFeedback",
                table: "PracticeAnswerRecord",
                type: "nvarchar(max)",
                nullable: true,
                comment: "AI 反馈。",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "PracticeAnswerRecord",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                comment: "主键，使用 32 位无分隔符 Guid 字符串。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ModelCallLog",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                comment: "用户 Id，关联 UserProfile.Id。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SessionId",
                table: "ModelCallLog",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                comment: "学习会话 Id，关联 LearningSession.Id。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ResponseText",
                table: "ModelCallLog",
                type: "nvarchar(max)",
                nullable: true,
                comment: "模型返回文本。",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RequestType",
                table: "ModelCallLog",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "请求类型。",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "QuestionRecordId",
                table: "ModelCallLog",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                comment: "提问记录 Id，关联 QuestionRecord.Id。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PromptText",
                table: "ModelCallLog",
                type: "nvarchar(max)",
                nullable: true,
                comment: "发送给模型的 Prompt 文本。",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "OutputTokens",
                table: "ModelCallLog",
                type: "int",
                nullable: true,
                comment: "输出 Token 数。",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ModelProvider",
                table: "ModelCallLog",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "模型提供方。",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ModelName",
                table: "ModelCallLog",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "模型名称。",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsSuccess",
                table: "ModelCallLog",
                type: "bit",
                nullable: false,
                comment: "模型调用是否成功。",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<int>(
                name: "InputTokens",
                table: "ModelCallLog",
                type: "int",
                nullable: true,
                comment: "输入 Token 数。",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ErrorMessage",
                table: "ModelCallLog",
                type: "nvarchar(max)",
                nullable: true,
                comment: "错误消息。",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DurationMs",
                table: "ModelCallLog",
                type: "int",
                nullable: true,
                comment: "媒体时长，单位毫秒。",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedTime",
                table: "ModelCallLog",
                type: "datetime2",
                nullable: false,
                comment: "创建时间，使用 UTC 时间记录。",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<decimal>(
                name: "Cost",
                table: "ModelCallLog",
                type: "decimal(18,4)",
                nullable: true,
                comment: "模型调用成本。",
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AgentName",
                table: "ModelCallLog",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "Agent 名称。",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "ModelCallLog",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                comment: "主键，使用 32 位无分隔符 Guid 字符串。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<int>(
                name: "Width",
                table: "MediaResource",
                type: "int",
                nullable: true,
                comment: "图片或视频宽度。",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "MediaResource",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                comment: "用户 Id，关联 UserProfile.Id。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedTime",
                table: "MediaResource",
                type: "datetime2",
                nullable: true,
                comment: "最后更新时间，未更新时为空。",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SourceType",
                table: "MediaResource",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "来源类型。",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ResourceType",
                table: "MediaResource",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "媒体资源类型。",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "MimeType",
                table: "MediaResource",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "MIME 类型。",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Height",
                table: "MediaResource",
                type: "int",
                nullable: true,
                comment: "图片或视频高度。",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Hash",
                table: "MediaResource",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                comment: "文件或内容哈希。",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "FileSize",
                table: "MediaResource",
                type: "bigint",
                nullable: true,
                comment: "文件大小，单位字节。",
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FilePath",
                table: "MediaResource",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                comment: "文件路径。",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "MediaResource",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "文件名。",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DurationMs",
                table: "MediaResource",
                type: "int",
                nullable: true,
                comment: "媒体时长，单位毫秒。",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedTime",
                table: "MediaResource",
                type: "datetime2",
                nullable: false,
                comment: "创建时间，使用 UTC 时间记录。",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "MediaResource",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                comment: "主键，使用 32 位无分隔符 Guid 字符串。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedTime",
                table: "Lesson",
                type: "datetime2",
                nullable: true,
                comment: "最后更新时间，未更新时为空。",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UnitId",
                table: "Lesson",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                comment: "教材单元 Id，关联 TextbookUnit.Id。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Lesson",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                comment: "标题。",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "TextbookId",
                table: "Lesson",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                comment: "教材 Id，关联 Textbook.Id。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<int>(
                name: "SortIndex",
                table: "Lesson",
                type: "int",
                nullable: false,
                comment: "排序值，数值越小越靠前。",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "PageStart",
                table: "Lesson",
                type: "int",
                nullable: true,
                comment: "起始页码。",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PageEnd",
                table: "Lesson",
                type: "int",
                nullable: true,
                comment: "结束页码。",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "LessonNo",
                table: "Lesson",
                type: "int",
                nullable: true,
                comment: "课时或章节序号。",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedTime",
                table: "Lesson",
                type: "datetime2",
                nullable: false,
                comment: "创建时间，使用 UTC 时间记录。",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Lesson",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                comment: "主键，使用 32 位无分隔符 Guid 字符串。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "LearningSession",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                comment: "用户 Id，关联 UserProfile.Id。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedTime",
                table: "LearningSession",
                type: "datetime2",
                nullable: true,
                comment: "最后更新时间，未更新时为空。",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "LearningSession",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "标题。",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Summary",
                table: "LearningSession",
                type: "nvarchar(max)",
                nullable: true,
                comment: "摘要。",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Subject",
                table: "LearningSession",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "学科。",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartTime",
                table: "LearningSession",
                type: "datetime2",
                nullable: false,
                comment: "会话开始时间。",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "SessionType",
                table: "LearningSession",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "学习会话类型。",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "RelatedWrongId",
                table: "LearningSession",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                comment: "关联错题 Id。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RelatedQuestionId",
                table: "LearningSession",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                comment: "关联问题 Id。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Grade",
                table: "LearningSession",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "年级。",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndTime",
                table: "LearningSession",
                type: "datetime2",
                nullable: true,
                comment: "会话结束时间。",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedTime",
                table: "LearningSession",
                type: "datetime2",
                nullable: false,
                comment: "创建时间，使用 UTC 时间记录。",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "LearningSession",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                comment: "主键，使用 32 位无分隔符 Guid 字符串。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "LearningProfile",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                comment: "用户 Id，关联 UserProfile.Id。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedTime",
                table: "LearningProfile",
                type: "datetime2",
                nullable: true,
                comment: "最后更新时间，未更新时为空。",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TextbookVersion",
                table: "LearningProfile",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "默认教材版本。",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Semester",
                table: "LearningProfile",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "学期或册别，例如上册、下册。",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SchoolName",
                table: "LearningProfile",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "学校名称。",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Grade",
                table: "LearningProfile",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "年级。",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "DefaultSubject",
                table: "LearningProfile",
                type: "nvarchar(max)",
                nullable: true,
                comment: "默认学习学科。",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedTime",
                table: "LearningProfile",
                type: "datetime2",
                nullable: false,
                comment: "创建时间，使用 UTC 时间记录。",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "LearningProfile",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                comment: "主键，使用 32 位无分隔符 Guid 字符串。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedTime",
                table: "KnowledgePoint",
                type: "datetime2",
                nullable: true,
                comment: "最后更新时间，未更新时为空。",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UnitId",
                table: "KnowledgePoint",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                comment: "教材单元 Id，关联 TextbookUnit.Id。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TextbookId",
                table: "KnowledgePoint",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                comment: "教材 Id，关联 Textbook.Id。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Subject",
                table: "KnowledgePoint",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "学科。",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "SortIndex",
                table: "KnowledgePoint",
                type: "int",
                nullable: false,
                comment: "排序值，数值越小越靠前。",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "PointType",
                table: "KnowledgePoint",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "知识点类型。",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ParentId",
                table: "KnowledgePoint",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                comment: "父级 Id，用于树形层级。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PageId",
                table: "KnowledgePoint",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                comment: "教材页 Id，关联 TextbookPage.Id。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "KnowledgePoint",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                comment: "名称。",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "LessonId",
                table: "KnowledgePoint",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                comment: "课时或章节 Id，关联 Lesson.Id。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Grade",
                table: "KnowledgePoint",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "年级。",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "DifficultyLevel",
                table: "KnowledgePoint",
                type: "int",
                nullable: false,
                comment: "难度等级，默认 1。",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "KnowledgePoint",
                type: "nvarchar(max)",
                nullable: true,
                comment: "详细说明。",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedTime",
                table: "KnowledgePoint",
                type: "datetime2",
                nullable: false,
                comment: "创建时间，使用 UTC 时间记录。",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "KnowledgePoint",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                comment: "主键，使用 32 位无分隔符 Guid 字符串。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "VectorHash",
                table: "KnowledgeEmbedding",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                comment: "向量数据哈希。",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "VectorData",
                table: "KnowledgeEmbedding",
                type: "nvarchar(max)",
                nullable: true,
                comment: "向量数据预留字段，第一阶段以文本形式保存。",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EmbeddingModel",
                table: "KnowledgeEmbedding",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "生成向量的模型名称。",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedTime",
                table: "KnowledgeEmbedding",
                type: "datetime2",
                nullable: false,
                comment: "创建时间，使用 UTC 时间记录。",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "ChunkId",
                table: "KnowledgeEmbedding",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                comment: "知识片段 Id，关联 KnowledgeChunk.Id。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "KnowledgeEmbedding",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                comment: "主键，使用 32 位无分隔符 Guid 字符串。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedTime",
                table: "KnowledgeChunk",
                type: "datetime2",
                nullable: true,
                comment: "最后更新时间，未更新时为空。",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UnitId",
                table: "KnowledgeChunk",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                comment: "教材单元 Id，关联 TextbookUnit.Id。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TextbookId",
                table: "KnowledgeChunk",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                comment: "教材 Id，关联 Textbook.Id。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SourceType",
                table: "KnowledgeChunk",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "来源类型。",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SourcePath",
                table: "KnowledgeChunk",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                comment: "来源文件或资源路径。",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SortIndex",
                table: "KnowledgeChunk",
                type: "int",
                nullable: false,
                comment: "排序值，数值越小越靠前。",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "PageNo",
                table: "KnowledgeChunk",
                type: "int",
                nullable: true,
                comment: "页码。",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PageId",
                table: "KnowledgeChunk",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                comment: "教材页 Id，关联 TextbookPage.Id。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LessonId",
                table: "KnowledgeChunk",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                comment: "课时或章节 Id，关联 Lesson.Id。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "KnowledgePointId",
                table: "KnowledgeChunk",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                comment: "知识点 Id，关联 KnowledgePoint.Id。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedTime",
                table: "KnowledgeChunk",
                type: "datetime2",
                nullable: false,
                comment: "创建时间，使用 UTC 时间记录。",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "ChunkType",
                table: "KnowledgeChunk",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "知识片段类型。",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ChunkTitle",
                table: "KnowledgeChunk",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "知识片段标题。",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ChunkText",
                table: "KnowledgeChunk",
                type: "nvarchar(max)",
                nullable: false,
                comment: "知识片段正文。",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "KnowledgeChunk",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                comment: "主键，使用 32 位无分隔符 Guid 字符串。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "HomeworkCheckItem",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                comment: "用户 Id，关联 UserProfile.Id。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedTime",
                table: "HomeworkCheckItem",
                type: "datetime2",
                nullable: true,
                comment: "最后更新时间，未更新时为空。",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Subject",
                table: "HomeworkCheckItem",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "学科。",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "StudentAnswer",
                table: "HomeworkCheckItem",
                type: "nvarchar(max)",
                nullable: true,
                comment: "学生答案。",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "QuestionText",
                table: "HomeworkCheckItem",
                type: "nvarchar(max)",
                nullable: true,
                comment: "题目或问题文本。",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "QuestionRecordId",
                table: "HomeworkCheckItem",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                comment: "提问记录 Id，关联 QuestionRecord.Id。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "QuestionNo",
                table: "HomeworkCheckItem",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "题号。",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "KnowledgePointId",
                table: "HomeworkCheckItem",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                comment: "知识点 Id，关联 KnowledgePoint.Id。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsCorrect",
                table: "HomeworkCheckItem",
                type: "bit",
                nullable: true,
                comment: "是否正确。",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ImageCropPath",
                table: "HomeworkCheckItem",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                comment: "题目截图裁剪路径。",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Grade",
                table: "HomeworkCheckItem",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "年级。",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Explanation",
                table: "HomeworkCheckItem",
                type: "nvarchar(max)",
                nullable: true,
                comment: "讲解内容。",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ErrorReason",
                table: "HomeworkCheckItem",
                type: "nvarchar(max)",
                nullable: true,
                comment: "错误原因。",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedTime",
                table: "HomeworkCheckItem",
                type: "datetime2",
                nullable: false,
                comment: "创建时间，使用 UTC 时间记录。",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "CorrectAnswer",
                table: "HomeworkCheckItem",
                type: "nvarchar(max)",
                nullable: true,
                comment: "正确答案。",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "HomeworkCheckItem",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                comment: "主键，使用 32 位无分隔符 Guid 字符串。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "VideoPath",
                table: "AnswerRecord",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                comment: "视频文件路径。",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "AnswerRecord",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                comment: "用户 Id，关联 UserProfile.Id。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "QuestionRecordId",
                table: "AnswerRecord",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                comment: "提问记录 Id，关联 QuestionRecord.Id。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "PromptVersion",
                table: "AnswerRecord",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "Prompt 版本。",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OutputType",
                table: "AnswerRecord",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "输出类型。",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ModelName",
                table: "AnswerRecord",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "模型名称。",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedTime",
                table: "AnswerRecord",
                type: "datetime2",
                nullable: false,
                comment: "创建时间，使用 UTC 时间记录。",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "AvatarScriptJson",
                table: "AnswerRecord",
                type: "nvarchar(max)",
                nullable: true,
                comment: "数字人脚本 JSON。",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AudioPath",
                table: "AnswerRecord",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                comment: "音频文件路径。",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AnswerText",
                table: "AnswerRecord",
                type: "nvarchar(max)",
                nullable: false,
                comment: "AI 回答文本。",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "AnswerJson",
                table: "AnswerRecord",
                type: "nvarchar(max)",
                nullable: true,
                comment: "AI 回答结构化 JSON。",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AgentName",
                table: "AnswerRecord",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "Agent 名称。",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "AnswerRecord",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                comment: "主键，使用 32 位无分隔符 Guid 字符串。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "AgentRouteLog",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                comment: "用户 Id，关联 UserProfile.Id。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SessionId",
                table: "AgentRouteLog",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                comment: "学习会话 Id，关联 LearningSession.Id。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SelectedModelProvider",
                table: "AgentRouteLog",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "路由选中的模型提供方。",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SelectedModelName",
                table: "AgentRouteLog",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "路由选中的模型名称。",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SelectedAgent",
                table: "AgentRouteLog",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "路由选中的 Agent。",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "RouteReason",
                table: "AgentRouteLog",
                type: "nvarchar(max)",
                nullable: true,
                comment: "路由原因。",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "QuestionRecordId",
                table: "AgentRouteLog",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                comment: "提问记录 Id，关联 QuestionRecord.Id。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "QuestionMode",
                table: "AgentRouteLog",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "路由识别的提问模式。",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InputType",
                table: "AgentRouteLog",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "Agent 输入类型。",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedTime",
                table: "AgentRouteLog",
                type: "datetime2",
                nullable: false,
                comment: "创建时间，使用 UTC 时间记录。",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "AgentRouteLog",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                comment: "主键，使用 32 位无分隔符 Guid 字符串。",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterTable(
                name: "WrongQuestionReview",
                oldComment: "错题复习记录表，保存学生复习错题时的作答、反馈和耗时。");

            migrationBuilder.AlterTable(
                name: "WrongQuestion",
                oldComment: "错题表，保存错题原题、答案、错因、讲解、掌握状态和复习时间。");

            migrationBuilder.AlterTable(
                name: "UserProfile",
                oldComment: "用户基础信息表，保存学生、家长或维护人员的基础身份信息。");

            migrationBuilder.AlterTable(
                name: "TextbookUnit",
                oldComment: "教材单元表，按教材组织单元目录和排序信息。");

            migrationBuilder.AlterTable(
                name: "TextbookParseLog",
                oldComment: "教材解析日志表，记录教材导入过程中的步骤、状态、消息和错误详情。");

            migrationBuilder.AlterTable(
                name: "TextbookPage",
                oldComment: "教材页表，保存教材页面图片、PDF 路径、OCR 原文和清洗文本。");

            migrationBuilder.AlterTable(
                name: "TextbookImportJob",
                oldComment: "教材导入任务表，记录教材上传、解析、OCR、切片和向量化流程状态。");

            migrationBuilder.AlterTable(
                name: "TextbookImportFile",
                oldComment: "教材导入文件表，记录导入任务关联的 PDF、图片等源文件信息。");

            migrationBuilder.AlterTable(
                name: "Textbook",
                oldComment: "教材主表，保存教材名称、出版社、学科、年级、册别、版本和封面信息。");

            migrationBuilder.AlterTable(
                name: "StudyStage",
                oldComment: "学习阶段表，保存学习计划拆分出的阶段目标、状态和时间。");

            migrationBuilder.AlterTable(
                name: "StudyPlan",
                oldComment: "学习计划表，保存面向知识点、错题或考试准备的阶段化学习计划。");

            migrationBuilder.AlterTable(
                name: "StageTaskRecord",
                oldComment: "阶段任务记录表，保存学生完成阶段任务时的动作、结果、得分和备注。");

            migrationBuilder.AlterTable(
                name: "StageTask",
                oldComment: "阶段任务表，保存每个学习阶段中的讲解、练习、复习、语音讨论或视频任务。");

            migrationBuilder.AlterTable(
                name: "SessionMessage",
                oldComment: "会话消息表，记录学习会话中的文本、图片、音频、视频和数字人脚本消息。");

            migrationBuilder.AlterTable(
                name: "ReviewSchedule",
                oldComment: "复习计划表，保存错题、知识点或练习题的下次复习安排。");

            migrationBuilder.AlterTable(
                name: "QuestionRecord",
                oldComment: "提问记录表，保存学生输入的问题文本、图片、音频、识别文本和提问模式。");

            migrationBuilder.AlterTable(
                name: "PracticeQuestionSource",
                oldComment: "练习题来源表，记录练习题与错题、知识点、教材切片等来源的关系。");

            migrationBuilder.AlterTable(
                name: "PracticeQuestion",
                oldComment: "练习题表，保存由错题、知识点、阶段任务或人工生成的练习题。");

            migrationBuilder.AlterTable(
                name: "PracticeAnswerRecord",
                oldComment: "练习作答记录表，保存学生练习题作答结果、反馈和耗时。");

            migrationBuilder.AlterTable(
                name: "ModelCallLog",
                oldComment: "模型调用日志表，记录每次模型调用的输入输出、Token、耗时、成本和错误。");

            migrationBuilder.AlterTable(
                name: "MediaResource",
                oldComment: "媒体资源表，统一管理图片、音频、视频、PDF、裁剪图和数字人脚本资源。");

            migrationBuilder.AlterTable(
                name: "Lesson",
                oldComment: "教材课时或章节表，保存课题、页码范围和所属单元。");

            migrationBuilder.AlterTable(
                name: "LearningSession",
                oldComment: "学习会话表，记录一次问答、拍照讲题、作业检查或复习过程。");

            migrationBuilder.AlterTable(
                name: "LearningProfile",
                oldComment: "学生学习档案表，保存年级、学期、学校、默认学科和教材版本等学习画像。");

            migrationBuilder.AlterTable(
                name: "KnowledgePoint",
                oldComment: "知识点表，保存学科年级下的知识点、层级关系和难度信息。");

            migrationBuilder.AlterTable(
                name: "KnowledgeEmbedding",
                oldComment: "知识向量表，预留向量检索模型、向量数据和哈希信息。");

            migrationBuilder.AlterTable(
                name: "KnowledgeChunk",
                oldComment: "知识片段表，保存教材切片文本及其来源、页码和知识点关联。");

            migrationBuilder.AlterTable(
                name: "HomeworkCheckItem",
                oldComment: "作业检查明细表，保存一张作业图片拆出的单题检查结果。");

            migrationBuilder.AlterTable(
                name: "AnswerRecord",
                oldComment: "回答记录表，保存 AI 回答文本、结构化结果、模型、Agent 和多媒体输出。");

            migrationBuilder.AlterTable(
                name: "AgentRouteLog",
                oldComment: "Agent 路由日志表，记录每次请求选择的 Agent、模型和路由原因。");

            migrationBuilder.AlterColumn<string>(
                name: "WrongQuestionId",
                table: "WrongQuestionReview",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldComment: "错题 Id，关联 WrongQuestion.Id。");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "WrongQuestionReview",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldComment: "用户 Id，关联 UserProfile.Id。");

            migrationBuilder.AlterColumn<string>(
                name: "UserAnswer",
                table: "WrongQuestionReview",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "用户作答内容。");

            migrationBuilder.AlterColumn<string>(
                name: "ReviewType",
                table: "WrongQuestionReview",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "复习类型。");

            migrationBuilder.AlterColumn<int>(
                name: "ReviewDurationSec",
                table: "WrongQuestionReview",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "复习耗时，单位秒。");

            migrationBuilder.AlterColumn<bool>(
                name: "IsCorrect",
                table: "WrongQuestionReview",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldComment: "是否正确。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedTime",
                table: "WrongQuestionReview",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "创建时间，使用 UTC 时间记录。");

            migrationBuilder.AlterColumn<string>(
                name: "AiFeedback",
                table: "WrongQuestionReview",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "AI 反馈。");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "WrongQuestionReview",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldComment: "主键，使用 32 位无分隔符 Guid 字符串。");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "WrongQuestion",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldComment: "用户 Id，关联 UserProfile.Id。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedTime",
                table: "WrongQuestion",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "最后更新时间，未更新时为空。");

            migrationBuilder.AlterColumn<string>(
                name: "Subject",
                table: "WrongQuestion",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "学科。");

            migrationBuilder.AlterColumn<string>(
                name: "StudentAnswer",
                table: "WrongQuestion",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "学生答案。");

            migrationBuilder.AlterColumn<int>(
                name: "ReviewCount",
                table: "WrongQuestion",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "复习次数。");

            migrationBuilder.AlterColumn<string>(
                name: "QuestionText",
                table: "WrongQuestion",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldComment: "题目或问题文本。");

            migrationBuilder.AlterColumn<string>(
                name: "QuestionRecordId",
                table: "WrongQuestion",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true,
                oldComment: "提问记录 Id，关联 QuestionRecord.Id。");

            migrationBuilder.AlterColumn<string>(
                name: "QuestionImagePath",
                table: "WrongQuestion",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true,
                oldComment: "题目图片路径。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NextReviewTime",
                table: "WrongQuestion",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "下次复习时间。");

            migrationBuilder.AlterColumn<string>(
                name: "MasteryStatus",
                table: "WrongQuestion",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "掌握状态。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastReviewTime",
                table: "WrongQuestion",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "上次复习时间。");

            migrationBuilder.AlterColumn<string>(
                name: "KnowledgePointId",
                table: "WrongQuestion",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true,
                oldComment: "知识点 Id，关联 KnowledgePoint.Id。");

            migrationBuilder.AlterColumn<string>(
                name: "HomeworkCheckItemId",
                table: "WrongQuestion",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true,
                oldComment: "作业检查明细 Id，关联 HomeworkCheckItem.Id。");

            migrationBuilder.AlterColumn<string>(
                name: "Grade",
                table: "WrongQuestion",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "年级。");

            migrationBuilder.AlterColumn<string>(
                name: "Explanation",
                table: "WrongQuestion",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "讲解内容。");

            migrationBuilder.AlterColumn<string>(
                name: "ErrorReason",
                table: "WrongQuestion",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "错误原因。");

            migrationBuilder.AlterColumn<int>(
                name: "DifficultyLevel",
                table: "WrongQuestion",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "难度等级，默认 1。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedTime",
                table: "WrongQuestion",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "创建时间，使用 UTC 时间记录。");

            migrationBuilder.AlterColumn<string>(
                name: "CorrectAnswer",
                table: "WrongQuestion",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "正确答案。");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "WrongQuestion",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldComment: "主键，使用 32 位无分隔符 Guid 字符串。");

            migrationBuilder.AlterColumn<string>(
                name: "UserType",
                table: "UserProfile",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "用户类型，例如 student、parent 或 admin。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedTime",
                table: "UserProfile",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "最后更新时间，未更新时为空。");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "UserProfile",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "手机号，第一阶段可为空。");

            migrationBuilder.AlterColumn<string>(
                name: "DisplayName",
                table: "UserProfile",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldComment: "用户显示名称。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedTime",
                table: "UserProfile",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "创建时间，使用 UTC 时间记录。");

            migrationBuilder.AlterColumn<string>(
                name: "AvatarPath",
                table: "UserProfile",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true,
                oldComment: "用户头像文件路径。");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "UserProfile",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldComment: "主键，使用 32 位无分隔符 Guid 字符串。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedTime",
                table: "TextbookUnit",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "最后更新时间，未更新时为空。");

            migrationBuilder.AlterColumn<int>(
                name: "UnitNo",
                table: "TextbookUnit",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "单元序号。");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "TextbookUnit",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldComment: "标题。");

            migrationBuilder.AlterColumn<string>(
                name: "TextbookId",
                table: "TextbookUnit",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldComment: "教材 Id，关联 Textbook.Id。");

            migrationBuilder.AlterColumn<int>(
                name: "SortIndex",
                table: "TextbookUnit",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "排序值，数值越小越靠前。");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "TextbookUnit",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "详细说明。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedTime",
                table: "TextbookUnit",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "创建时间，使用 UTC 时间记录。");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "TextbookUnit",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldComment: "主键，使用 32 位无分隔符 Guid 字符串。");

            migrationBuilder.AlterColumn<string>(
                name: "TextbookId",
                table: "TextbookParseLog",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true,
                oldComment: "教材 Id，关联 Textbook.Id。");

            migrationBuilder.AlterColumn<string>(
                name: "StepName",
                table: "TextbookParseLog",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldComment: "处理步骤名称。");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "TextbookParseLog",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "状态。");

            migrationBuilder.AlterColumn<int>(
                name: "PageNo",
                table: "TextbookParseLog",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "页码。");

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "TextbookParseLog",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "日志消息。");

            migrationBuilder.AlterColumn<string>(
                name: "ImportJobId",
                table: "TextbookParseLog",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldComment: "教材导入任务 Id，关联 TextbookImportJob.Id。");

            migrationBuilder.AlterColumn<string>(
                name: "ErrorDetail",
                table: "TextbookParseLog",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "错误详细信息。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedTime",
                table: "TextbookParseLog",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "创建时间，使用 UTC 时间记录。");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "TextbookParseLog",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldComment: "主键，使用 32 位无分隔符 Guid 字符串。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedTime",
                table: "TextbookPage",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "最后更新时间，未更新时为空。");

            migrationBuilder.AlterColumn<string>(
                name: "UnitId",
                table: "TextbookPage",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true,
                oldComment: "教材单元 Id，关联 TextbookUnit.Id。");

            migrationBuilder.AlterColumn<string>(
                name: "TextbookId",
                table: "TextbookPage",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldComment: "教材 Id，关联 Textbook.Id。");

            migrationBuilder.AlterColumn<string>(
                name: "PdfPath",
                table: "TextbookPage",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true,
                oldComment: "PDF 文件路径。");

            migrationBuilder.AlterColumn<int>(
                name: "PageNo",
                table: "TextbookPage",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "页码。");

            migrationBuilder.AlterColumn<string>(
                name: "OcrText",
                table: "TextbookPage",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "OCR 识别原文。");

            migrationBuilder.AlterColumn<string>(
                name: "LessonId",
                table: "TextbookPage",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true,
                oldComment: "课时或章节 Id，关联 Lesson.Id。");

            migrationBuilder.AlterColumn<string>(
                name: "ImagePath",
                table: "TextbookPage",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true,
                oldComment: "图片文件路径。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedTime",
                table: "TextbookPage",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "创建时间，使用 UTC 时间记录。");

            migrationBuilder.AlterColumn<string>(
                name: "CleanText",
                table: "TextbookPage",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "清洗后的文本。");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "TextbookPage",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldComment: "主键，使用 32 位无分隔符 Guid 字符串。");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "TextbookImportJob",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true,
                oldComment: "用户 Id，关联 UserProfile.Id。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedTime",
                table: "TextbookImportJob",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "最后更新时间，未更新时为空。");

            migrationBuilder.AlterColumn<int>(
                name: "TotalPages",
                table: "TextbookImportJob",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "总页数。");

            migrationBuilder.AlterColumn<int>(
                name: "TotalChunks",
                table: "TextbookImportJob",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "生成的知识片段总数。");

            migrationBuilder.AlterColumn<string>(
                name: "TextbookId",
                table: "TextbookImportJob",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true,
                oldComment: "教材 Id，关联 Textbook.Id。");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "TextbookImportJob",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "状态。");

            migrationBuilder.AlterColumn<string>(
                name: "SourceFilePath",
                table: "TextbookImportJob",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true,
                oldComment: "源文件路径。");

            migrationBuilder.AlterColumn<int>(
                name: "ParsedPages",
                table: "TextbookImportJob",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "已解析页数。");

            migrationBuilder.AlterColumn<string>(
                name: "JobName",
                table: "TextbookImportJob",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldComment: "导入任务名称。");

            migrationBuilder.AlterColumn<string>(
                name: "ImportType",
                table: "TextbookImportJob",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "教材导入类型。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FinishedTime",
                table: "TextbookImportJob",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "任务完成时间。");

            migrationBuilder.AlterColumn<string>(
                name: "ErrorMessage",
                table: "TextbookImportJob",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "错误消息。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedTime",
                table: "TextbookImportJob",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "创建时间，使用 UTC 时间记录。");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "TextbookImportJob",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldComment: "主键，使用 32 位无分隔符 Guid 字符串。");

            migrationBuilder.AlterColumn<string>(
                name: "TextbookId",
                table: "TextbookImportFile",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true,
                oldComment: "教材 Id，关联 Textbook.Id。");

            migrationBuilder.AlterColumn<int>(
                name: "PageCount",
                table: "TextbookImportFile",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "文件页数。");

            migrationBuilder.AlterColumn<string>(
                name: "MimeType",
                table: "TextbookImportFile",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "MIME 类型。");

            migrationBuilder.AlterColumn<string>(
                name: "ImportJobId",
                table: "TextbookImportFile",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldComment: "教材导入任务 Id，关联 TextbookImportJob.Id。");

            migrationBuilder.AlterColumn<string>(
                name: "Hash",
                table: "TextbookImportFile",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true,
                oldComment: "文件或内容哈希。");

            migrationBuilder.AlterColumn<string>(
                name: "FileType",
                table: "TextbookImportFile",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "文件类型。");

            migrationBuilder.AlterColumn<long>(
                name: "FileSize",
                table: "TextbookImportFile",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true,
                oldComment: "文件大小，单位字节。");

            migrationBuilder.AlterColumn<string>(
                name: "FilePath",
                table: "TextbookImportFile",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldComment: "文件路径。");

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "TextbookImportFile",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldComment: "文件名。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedTime",
                table: "TextbookImportFile",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "创建时间，使用 UTC 时间记录。");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "TextbookImportFile",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldComment: "主键，使用 32 位无分隔符 Guid 字符串。");

            migrationBuilder.AlterColumn<string>(
                name: "Version",
                table: "Textbook",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "版本号或版本描述。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedTime",
                table: "Textbook",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "最后更新时间，未更新时为空。");

            migrationBuilder.AlterColumn<string>(
                name: "Subject",
                table: "Textbook",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "学科。");

            migrationBuilder.AlterColumn<string>(
                name: "Semester",
                table: "Textbook",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "学期或册别，例如上册、下册。");

            migrationBuilder.AlterColumn<string>(
                name: "Publisher",
                table: "Textbook",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "出版社。");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Textbook",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldComment: "名称。");

            migrationBuilder.AlterColumn<string>(
                name: "Grade",
                table: "Textbook",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "年级。");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Textbook",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "详细说明。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedTime",
                table: "Textbook",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "创建时间，使用 UTC 时间记录。");

            migrationBuilder.AlterColumn<string>(
                name: "CoverImagePath",
                table: "Textbook",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true,
                oldComment: "封面图片路径。");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Textbook",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldComment: "主键，使用 32 位无分隔符 Guid 字符串。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedTime",
                table: "StudyStage",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "最后更新时间，未更新时为空。");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "StudyStage",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldComment: "标题。");

            migrationBuilder.AlterColumn<string>(
                name: "StudyPlanId",
                table: "StudyStage",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldComment: "学习计划 Id，关联 StudyPlan.Id。");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "StudyStage",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "状态。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "StudyStage",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "开始日期。");

            migrationBuilder.AlterColumn<int>(
                name: "StageNo",
                table: "StudyStage",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "阶段序号。");

            migrationBuilder.AlterColumn<string>(
                name: "Goal",
                table: "StudyStage",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "阶段目标。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "StudyStage",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "结束日期。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedTime",
                table: "StudyStage",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "创建时间，使用 UTC 时间记录。");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "StudyStage",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldComment: "主键，使用 32 位无分隔符 Guid 字符串。");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "StudyPlan",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldComment: "用户 Id，关联 UserProfile.Id。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedTime",
                table: "StudyPlan",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "最后更新时间，未更新时为空。");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "StudyPlan",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldComment: "标题。");

            migrationBuilder.AlterColumn<string>(
                name: "Summary",
                table: "StudyPlan",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "摘要。");

            migrationBuilder.AlterColumn<string>(
                name: "Subject",
                table: "StudyPlan",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "学科。");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "StudyPlan",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "状态。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "StudyPlan",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "开始日期。");

            migrationBuilder.AlterColumn<string>(
                name: "PlanType",
                table: "StudyPlan",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "学习计划类型。");

            migrationBuilder.AlterColumn<string>(
                name: "Grade",
                table: "StudyPlan",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "年级。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "StudyPlan",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "结束日期。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedTime",
                table: "StudyPlan",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "创建时间，使用 UTC 时间记录。");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "StudyPlan",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldComment: "主键，使用 32 位无分隔符 Guid 字符串。");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "StageTaskRecord",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldComment: "用户 Id，关联 UserProfile.Id。");

            migrationBuilder.AlterColumn<string>(
                name: "StageTaskId",
                table: "StageTaskRecord",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldComment: "阶段任务 Id，关联 StageTask.Id。");

            migrationBuilder.AlterColumn<decimal>(
                name: "Score",
                table: "StageTaskRecord",
                type: "decimal(5,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)",
                oldNullable: true,
                oldComment: "得分。");

            migrationBuilder.AlterColumn<string>(
                name: "Result",
                table: "StageTaskRecord",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "执行结果。");

            migrationBuilder.AlterColumn<string>(
                name: "Remark",
                table: "StageTaskRecord",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "备注。");

            migrationBuilder.AlterColumn<int>(
                name: "DurationSeconds",
                table: "StageTaskRecord",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "耗时，单位秒。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedTime",
                table: "StageTaskRecord",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "创建时间，使用 UTC 时间记录。");

            migrationBuilder.AlterColumn<string>(
                name: "ActionType",
                table: "StageTaskRecord",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "动作类型。");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "StageTaskRecord",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldComment: "主键，使用 32 位无分隔符 Guid 字符串。");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "StageTask",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldComment: "用户 Id，关联 UserProfile.Id。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedTime",
                table: "StageTask",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "最后更新时间，未更新时为空。");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "StageTask",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldComment: "标题。");

            migrationBuilder.AlterColumn<string>(
                name: "TaskType",
                table: "StageTask",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "阶段任务类型。");

            migrationBuilder.AlterColumn<string>(
                name: "StudyStageId",
                table: "StageTask",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldComment: "学习阶段 Id，关联 StudyStage.Id。");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "StageTask",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "状态。");

            migrationBuilder.AlterColumn<int>(
                name: "SortIndex",
                table: "StageTask",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "排序值，数值越小越靠前。");

            migrationBuilder.AlterColumn<string>(
                name: "RelatedWrongQuestionId",
                table: "StageTask",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true,
                oldComment: "关联错题 Id。");

            migrationBuilder.AlterColumn<string>(
                name: "RelatedPracticeQuestionId",
                table: "StageTask",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true,
                oldComment: "关联练习题 Id。");

            migrationBuilder.AlterColumn<string>(
                name: "RelatedKnowledgePointId",
                table: "StageTask",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true,
                oldComment: "关联知识点 Id。");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "StageTask",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "详细说明。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedTime",
                table: "StageTask",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "创建时间，使用 UTC 时间记录。");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "StageTask",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldComment: "主键，使用 32 位无分隔符 Guid 字符串。");

            migrationBuilder.AlterColumn<string>(
                name: "VideoPath",
                table: "SessionMessage",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true,
                oldComment: "视频文件路径。");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "SessionMessage",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldComment: "用户 Id，关联 UserProfile.Id。");

            migrationBuilder.AlterColumn<string>(
                name: "TextContent",
                table: "SessionMessage",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "文本内容。");

            migrationBuilder.AlterColumn<string>(
                name: "SessionId",
                table: "SessionMessage",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldComment: "学习会话 Id，关联 LearningSession.Id。");

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "SessionMessage",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "消息角色。");

            migrationBuilder.AlterColumn<string>(
                name: "MessageJson",
                table: "SessionMessage",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "消息结构化 JSON。");

            migrationBuilder.AlterColumn<string>(
                name: "ImagePath",
                table: "SessionMessage",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true,
                oldComment: "图片文件路径。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedTime",
                table: "SessionMessage",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "创建时间，使用 UTC 时间记录。");

            migrationBuilder.AlterColumn<string>(
                name: "ContentType",
                table: "SessionMessage",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "消息内容类型。");

            migrationBuilder.AlterColumn<string>(
                name: "AudioPath",
                table: "SessionMessage",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true,
                oldComment: "音频文件路径。");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "SessionMessage",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldComment: "主键，使用 32 位无分隔符 Guid 字符串。");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ReviewSchedule",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldComment: "用户 Id，关联 UserProfile.Id。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedTime",
                table: "ReviewSchedule",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "最后更新时间，未更新时为空。");

            migrationBuilder.AlterColumn<string>(
                name: "Subject",
                table: "ReviewSchedule",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "学科。");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "ReviewSchedule",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "状态。");

            migrationBuilder.AlterColumn<string>(
                name: "ReviewTargetType",
                table: "ReviewSchedule",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "复习目标类型。");

            migrationBuilder.AlterColumn<string>(
                name: "ReviewTargetId",
                table: "ReviewSchedule",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldComment: "复习目标 Id。");

            migrationBuilder.AlterColumn<int>(
                name: "ReviewLevel",
                table: "ReviewSchedule",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "复习等级。");

            migrationBuilder.AlterColumn<int>(
                name: "ReviewCount",
                table: "ReviewSchedule",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "复习次数。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NextReviewTime",
                table: "ReviewSchedule",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "下次复习时间。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastReviewTime",
                table: "ReviewSchedule",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "上次复习时间。");

            migrationBuilder.AlterColumn<string>(
                name: "KnowledgePointId",
                table: "ReviewSchedule",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true,
                oldComment: "知识点 Id，关联 KnowledgePoint.Id。");

            migrationBuilder.AlterColumn<string>(
                name: "Grade",
                table: "ReviewSchedule",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "年级。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedTime",
                table: "ReviewSchedule",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "创建时间，使用 UTC 时间记录。");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "ReviewSchedule",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldComment: "主键，使用 32 位无分隔符 Guid 字符串。");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "QuestionRecord",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldComment: "用户 Id，关联 UserProfile.Id。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedTime",
                table: "QuestionRecord",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "最后更新时间，未更新时为空。");

            migrationBuilder.AlterColumn<string>(
                name: "Subject",
                table: "QuestionRecord",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "学科。");

            migrationBuilder.AlterColumn<string>(
                name: "SourceType",
                table: "QuestionRecord",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "来源类型。");

            migrationBuilder.AlterColumn<string>(
                name: "SessionId",
                table: "QuestionRecord",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true,
                oldComment: "学习会话 Id，关联 LearningSession.Id。");

            migrationBuilder.AlterColumn<string>(
                name: "RecognizedText",
                table: "QuestionRecord",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "从图片或音频识别出的文本。");

            migrationBuilder.AlterColumn<string>(
                name: "QuestionText",
                table: "QuestionRecord",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "题目或问题文本。");

            migrationBuilder.AlterColumn<string>(
                name: "Mode",
                table: "QuestionRecord",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "提问模式。");

            migrationBuilder.AlterColumn<string>(
                name: "InputType",
                table: "QuestionRecord",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "Agent 输入类型。");

            migrationBuilder.AlterColumn<string>(
                name: "ImagePath",
                table: "QuestionRecord",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true,
                oldComment: "图片文件路径。");

            migrationBuilder.AlterColumn<string>(
                name: "Grade",
                table: "QuestionRecord",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "年级。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedTime",
                table: "QuestionRecord",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "创建时间，使用 UTC 时间记录。");

            migrationBuilder.AlterColumn<string>(
                name: "AudioPath",
                table: "QuestionRecord",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true,
                oldComment: "音频文件路径。");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "QuestionRecord",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldComment: "主键，使用 32 位无分隔符 Guid 字符串。");

            migrationBuilder.AlterColumn<string>(
                name: "SourceType",
                table: "PracticeQuestionSource",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "来源类型。");

            migrationBuilder.AlterColumn<string>(
                name: "SourceId",
                table: "PracticeQuestionSource",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldComment: "练习题来源表，记录练习题与错题、知识点、教材切片等来源的关系。字段 SourceId。");

            migrationBuilder.AlterColumn<string>(
                name: "PracticeQuestionId",
                table: "PracticeQuestionSource",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldComment: "练习题 Id，关联 PracticeQuestion.Id。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedTime",
                table: "PracticeQuestionSource",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "创建时间，使用 UTC 时间记录。");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "PracticeQuestionSource",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldComment: "主键，使用 32 位无分隔符 Guid 字符串。");

            migrationBuilder.AlterColumn<string>(
                name: "WrongQuestionId",
                table: "PracticeQuestion",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true,
                oldComment: "错题 Id，关联 WrongQuestion.Id。");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "PracticeQuestion",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true,
                oldComment: "用户 Id，关联 UserProfile.Id。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedTime",
                table: "PracticeQuestion",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "最后更新时间，未更新时为空。");

            migrationBuilder.AlterColumn<string>(
                name: "Subject",
                table: "PracticeQuestion",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "学科。");

            migrationBuilder.AlterColumn<string>(
                name: "SourceType",
                table: "PracticeQuestion",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "来源类型。");

            migrationBuilder.AlterColumn<string>(
                name: "QuestionText",
                table: "PracticeQuestion",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldComment: "题目或问题文本。");

            migrationBuilder.AlterColumn<string>(
                name: "QuestionImagePath",
                table: "PracticeQuestion",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true,
                oldComment: "题目图片路径。");

            migrationBuilder.AlterColumn<string>(
                name: "PromptVersion",
                table: "PracticeQuestion",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "Prompt 版本。");

            migrationBuilder.AlterColumn<string>(
                name: "ModelName",
                table: "PracticeQuestion",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "模型名称。");

            migrationBuilder.AlterColumn<string>(
                name: "KnowledgePointId",
                table: "PracticeQuestion",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true,
                oldComment: "知识点 Id，关联 KnowledgePoint.Id。");

            migrationBuilder.AlterColumn<string>(
                name: "Grade",
                table: "PracticeQuestion",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "年级。");

            migrationBuilder.AlterColumn<string>(
                name: "GenerateType",
                table: "PracticeQuestion",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "练习题生成类型。");

            migrationBuilder.AlterColumn<string>(
                name: "Explanation",
                table: "PracticeQuestion",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "讲解内容。");

            migrationBuilder.AlterColumn<int>(
                name: "DifficultyLevel",
                table: "PracticeQuestion",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "难度等级，默认 1。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedTime",
                table: "PracticeQuestion",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "创建时间，使用 UTC 时间记录。");

            migrationBuilder.AlterColumn<string>(
                name: "CorrectAnswer",
                table: "PracticeQuestion",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "正确答案。");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "PracticeQuestion",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldComment: "主键，使用 32 位无分隔符 Guid 字符串。");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "PracticeAnswerRecord",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldComment: "用户 Id，关联 UserProfile.Id。");

            migrationBuilder.AlterColumn<string>(
                name: "UserAnswer",
                table: "PracticeAnswerRecord",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "用户作答内容。");

            migrationBuilder.AlterColumn<string>(
                name: "PracticeQuestionId",
                table: "PracticeAnswerRecord",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldComment: "练习题 Id，关联 PracticeQuestion.Id。");

            migrationBuilder.AlterColumn<bool>(
                name: "IsCorrect",
                table: "PracticeAnswerRecord",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldComment: "是否正确。");

            migrationBuilder.AlterColumn<int>(
                name: "DurationSeconds",
                table: "PracticeAnswerRecord",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "耗时，单位秒。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedTime",
                table: "PracticeAnswerRecord",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "创建时间，使用 UTC 时间记录。");

            migrationBuilder.AlterColumn<string>(
                name: "AiFeedback",
                table: "PracticeAnswerRecord",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "AI 反馈。");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "PracticeAnswerRecord",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldComment: "主键，使用 32 位无分隔符 Guid 字符串。");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ModelCallLog",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true,
                oldComment: "用户 Id，关联 UserProfile.Id。");

            migrationBuilder.AlterColumn<string>(
                name: "SessionId",
                table: "ModelCallLog",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true,
                oldComment: "学习会话 Id，关联 LearningSession.Id。");

            migrationBuilder.AlterColumn<string>(
                name: "ResponseText",
                table: "ModelCallLog",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "模型返回文本。");

            migrationBuilder.AlterColumn<string>(
                name: "RequestType",
                table: "ModelCallLog",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "请求类型。");

            migrationBuilder.AlterColumn<string>(
                name: "QuestionRecordId",
                table: "ModelCallLog",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true,
                oldComment: "提问记录 Id，关联 QuestionRecord.Id。");

            migrationBuilder.AlterColumn<string>(
                name: "PromptText",
                table: "ModelCallLog",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "发送给模型的 Prompt 文本。");

            migrationBuilder.AlterColumn<int>(
                name: "OutputTokens",
                table: "ModelCallLog",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "输出 Token 数。");

            migrationBuilder.AlterColumn<string>(
                name: "ModelProvider",
                table: "ModelCallLog",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "模型提供方。");

            migrationBuilder.AlterColumn<string>(
                name: "ModelName",
                table: "ModelCallLog",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "模型名称。");

            migrationBuilder.AlterColumn<bool>(
                name: "IsSuccess",
                table: "ModelCallLog",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "模型调用是否成功。");

            migrationBuilder.AlterColumn<int>(
                name: "InputTokens",
                table: "ModelCallLog",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "输入 Token 数。");

            migrationBuilder.AlterColumn<string>(
                name: "ErrorMessage",
                table: "ModelCallLog",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "错误消息。");

            migrationBuilder.AlterColumn<int>(
                name: "DurationMs",
                table: "ModelCallLog",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "媒体时长，单位毫秒。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedTime",
                table: "ModelCallLog",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "创建时间，使用 UTC 时间记录。");

            migrationBuilder.AlterColumn<decimal>(
                name: "Cost",
                table: "ModelCallLog",
                type: "decimal(18,4)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldNullable: true,
                oldComment: "模型调用成本。");

            migrationBuilder.AlterColumn<string>(
                name: "AgentName",
                table: "ModelCallLog",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "Agent 名称。");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "ModelCallLog",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldComment: "主键，使用 32 位无分隔符 Guid 字符串。");

            migrationBuilder.AlterColumn<int>(
                name: "Width",
                table: "MediaResource",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "图片或视频宽度。");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "MediaResource",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true,
                oldComment: "用户 Id，关联 UserProfile.Id。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedTime",
                table: "MediaResource",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "最后更新时间，未更新时为空。");

            migrationBuilder.AlterColumn<string>(
                name: "SourceType",
                table: "MediaResource",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "来源类型。");

            migrationBuilder.AlterColumn<string>(
                name: "ResourceType",
                table: "MediaResource",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "媒体资源类型。");

            migrationBuilder.AlterColumn<string>(
                name: "MimeType",
                table: "MediaResource",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "MIME 类型。");

            migrationBuilder.AlterColumn<int>(
                name: "Height",
                table: "MediaResource",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "图片或视频高度。");

            migrationBuilder.AlterColumn<string>(
                name: "Hash",
                table: "MediaResource",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true,
                oldComment: "文件或内容哈希。");

            migrationBuilder.AlterColumn<long>(
                name: "FileSize",
                table: "MediaResource",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true,
                oldComment: "文件大小，单位字节。");

            migrationBuilder.AlterColumn<string>(
                name: "FilePath",
                table: "MediaResource",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldComment: "文件路径。");

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "MediaResource",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "文件名。");

            migrationBuilder.AlterColumn<int>(
                name: "DurationMs",
                table: "MediaResource",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "媒体时长，单位毫秒。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedTime",
                table: "MediaResource",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "创建时间，使用 UTC 时间记录。");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "MediaResource",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldComment: "主键，使用 32 位无分隔符 Guid 字符串。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedTime",
                table: "Lesson",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "最后更新时间，未更新时为空。");

            migrationBuilder.AlterColumn<string>(
                name: "UnitId",
                table: "Lesson",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true,
                oldComment: "教材单元 Id，关联 TextbookUnit.Id。");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Lesson",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldComment: "标题。");

            migrationBuilder.AlterColumn<string>(
                name: "TextbookId",
                table: "Lesson",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldComment: "教材 Id，关联 Textbook.Id。");

            migrationBuilder.AlterColumn<int>(
                name: "SortIndex",
                table: "Lesson",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "排序值，数值越小越靠前。");

            migrationBuilder.AlterColumn<int>(
                name: "PageStart",
                table: "Lesson",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "起始页码。");

            migrationBuilder.AlterColumn<int>(
                name: "PageEnd",
                table: "Lesson",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "结束页码。");

            migrationBuilder.AlterColumn<int>(
                name: "LessonNo",
                table: "Lesson",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "课时或章节序号。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedTime",
                table: "Lesson",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "创建时间，使用 UTC 时间记录。");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Lesson",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldComment: "主键，使用 32 位无分隔符 Guid 字符串。");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "LearningSession",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldComment: "用户 Id，关联 UserProfile.Id。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedTime",
                table: "LearningSession",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "最后更新时间，未更新时为空。");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "LearningSession",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "标题。");

            migrationBuilder.AlterColumn<string>(
                name: "Summary",
                table: "LearningSession",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "摘要。");

            migrationBuilder.AlterColumn<string>(
                name: "Subject",
                table: "LearningSession",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "学科。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartTime",
                table: "LearningSession",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "会话开始时间。");

            migrationBuilder.AlterColumn<string>(
                name: "SessionType",
                table: "LearningSession",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "学习会话类型。");

            migrationBuilder.AlterColumn<string>(
                name: "RelatedWrongId",
                table: "LearningSession",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true,
                oldComment: "关联错题 Id。");

            migrationBuilder.AlterColumn<string>(
                name: "RelatedQuestionId",
                table: "LearningSession",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true,
                oldComment: "关联问题 Id。");

            migrationBuilder.AlterColumn<string>(
                name: "Grade",
                table: "LearningSession",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "年级。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndTime",
                table: "LearningSession",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "会话结束时间。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedTime",
                table: "LearningSession",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "创建时间，使用 UTC 时间记录。");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "LearningSession",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldComment: "主键，使用 32 位无分隔符 Guid 字符串。");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "LearningProfile",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldComment: "用户 Id，关联 UserProfile.Id。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedTime",
                table: "LearningProfile",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "最后更新时间，未更新时为空。");

            migrationBuilder.AlterColumn<string>(
                name: "TextbookVersion",
                table: "LearningProfile",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "默认教材版本。");

            migrationBuilder.AlterColumn<string>(
                name: "Semester",
                table: "LearningProfile",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "学期或册别，例如上册、下册。");

            migrationBuilder.AlterColumn<string>(
                name: "SchoolName",
                table: "LearningProfile",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "学校名称。");

            migrationBuilder.AlterColumn<string>(
                name: "Grade",
                table: "LearningProfile",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "年级。");

            migrationBuilder.AlterColumn<string>(
                name: "DefaultSubject",
                table: "LearningProfile",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "默认学习学科。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedTime",
                table: "LearningProfile",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "创建时间，使用 UTC 时间记录。");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "LearningProfile",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldComment: "主键，使用 32 位无分隔符 Guid 字符串。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedTime",
                table: "KnowledgePoint",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "最后更新时间，未更新时为空。");

            migrationBuilder.AlterColumn<string>(
                name: "UnitId",
                table: "KnowledgePoint",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true,
                oldComment: "教材单元 Id，关联 TextbookUnit.Id。");

            migrationBuilder.AlterColumn<string>(
                name: "TextbookId",
                table: "KnowledgePoint",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true,
                oldComment: "教材 Id，关联 Textbook.Id。");

            migrationBuilder.AlterColumn<string>(
                name: "Subject",
                table: "KnowledgePoint",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "学科。");

            migrationBuilder.AlterColumn<int>(
                name: "SortIndex",
                table: "KnowledgePoint",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "排序值，数值越小越靠前。");

            migrationBuilder.AlterColumn<string>(
                name: "PointType",
                table: "KnowledgePoint",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "知识点类型。");

            migrationBuilder.AlterColumn<string>(
                name: "ParentId",
                table: "KnowledgePoint",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true,
                oldComment: "父级 Id，用于树形层级。");

            migrationBuilder.AlterColumn<string>(
                name: "PageId",
                table: "KnowledgePoint",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true,
                oldComment: "教材页 Id，关联 TextbookPage.Id。");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "KnowledgePoint",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldComment: "名称。");

            migrationBuilder.AlterColumn<string>(
                name: "LessonId",
                table: "KnowledgePoint",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true,
                oldComment: "课时或章节 Id，关联 Lesson.Id。");

            migrationBuilder.AlterColumn<string>(
                name: "Grade",
                table: "KnowledgePoint",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "年级。");

            migrationBuilder.AlterColumn<int>(
                name: "DifficultyLevel",
                table: "KnowledgePoint",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "难度等级，默认 1。");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "KnowledgePoint",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "详细说明。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedTime",
                table: "KnowledgePoint",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "创建时间，使用 UTC 时间记录。");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "KnowledgePoint",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldComment: "主键，使用 32 位无分隔符 Guid 字符串。");

            migrationBuilder.AlterColumn<string>(
                name: "VectorHash",
                table: "KnowledgeEmbedding",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true,
                oldComment: "向量数据哈希。");

            migrationBuilder.AlterColumn<string>(
                name: "VectorData",
                table: "KnowledgeEmbedding",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "向量数据预留字段，第一阶段以文本形式保存。");

            migrationBuilder.AlterColumn<string>(
                name: "EmbeddingModel",
                table: "KnowledgeEmbedding",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldComment: "生成向量的模型名称。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedTime",
                table: "KnowledgeEmbedding",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "创建时间，使用 UTC 时间记录。");

            migrationBuilder.AlterColumn<string>(
                name: "ChunkId",
                table: "KnowledgeEmbedding",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldComment: "知识片段 Id，关联 KnowledgeChunk.Id。");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "KnowledgeEmbedding",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldComment: "主键，使用 32 位无分隔符 Guid 字符串。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedTime",
                table: "KnowledgeChunk",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "最后更新时间，未更新时为空。");

            migrationBuilder.AlterColumn<string>(
                name: "UnitId",
                table: "KnowledgeChunk",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true,
                oldComment: "教材单元 Id，关联 TextbookUnit.Id。");

            migrationBuilder.AlterColumn<string>(
                name: "TextbookId",
                table: "KnowledgeChunk",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true,
                oldComment: "教材 Id，关联 Textbook.Id。");

            migrationBuilder.AlterColumn<string>(
                name: "SourceType",
                table: "KnowledgeChunk",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "来源类型。");

            migrationBuilder.AlterColumn<string>(
                name: "SourcePath",
                table: "KnowledgeChunk",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true,
                oldComment: "来源文件或资源路径。");

            migrationBuilder.AlterColumn<int>(
                name: "SortIndex",
                table: "KnowledgeChunk",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "排序值，数值越小越靠前。");

            migrationBuilder.AlterColumn<int>(
                name: "PageNo",
                table: "KnowledgeChunk",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "页码。");

            migrationBuilder.AlterColumn<string>(
                name: "PageId",
                table: "KnowledgeChunk",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true,
                oldComment: "教材页 Id，关联 TextbookPage.Id。");

            migrationBuilder.AlterColumn<string>(
                name: "LessonId",
                table: "KnowledgeChunk",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true,
                oldComment: "课时或章节 Id，关联 Lesson.Id。");

            migrationBuilder.AlterColumn<string>(
                name: "KnowledgePointId",
                table: "KnowledgeChunk",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true,
                oldComment: "知识点 Id，关联 KnowledgePoint.Id。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedTime",
                table: "KnowledgeChunk",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "创建时间，使用 UTC 时间记录。");

            migrationBuilder.AlterColumn<string>(
                name: "ChunkType",
                table: "KnowledgeChunk",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "知识片段类型。");

            migrationBuilder.AlterColumn<string>(
                name: "ChunkTitle",
                table: "KnowledgeChunk",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "知识片段标题。");

            migrationBuilder.AlterColumn<string>(
                name: "ChunkText",
                table: "KnowledgeChunk",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldComment: "知识片段正文。");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "KnowledgeChunk",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldComment: "主键，使用 32 位无分隔符 Guid 字符串。");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "HomeworkCheckItem",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldComment: "用户 Id，关联 UserProfile.Id。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedTime",
                table: "HomeworkCheckItem",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "最后更新时间，未更新时为空。");

            migrationBuilder.AlterColumn<string>(
                name: "Subject",
                table: "HomeworkCheckItem",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "学科。");

            migrationBuilder.AlterColumn<string>(
                name: "StudentAnswer",
                table: "HomeworkCheckItem",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "学生答案。");

            migrationBuilder.AlterColumn<string>(
                name: "QuestionText",
                table: "HomeworkCheckItem",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "题目或问题文本。");

            migrationBuilder.AlterColumn<string>(
                name: "QuestionRecordId",
                table: "HomeworkCheckItem",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldComment: "提问记录 Id，关联 QuestionRecord.Id。");

            migrationBuilder.AlterColumn<string>(
                name: "QuestionNo",
                table: "HomeworkCheckItem",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "题号。");

            migrationBuilder.AlterColumn<string>(
                name: "KnowledgePointId",
                table: "HomeworkCheckItem",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true,
                oldComment: "知识点 Id，关联 KnowledgePoint.Id。");

            migrationBuilder.AlterColumn<bool>(
                name: "IsCorrect",
                table: "HomeworkCheckItem",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldComment: "是否正确。");

            migrationBuilder.AlterColumn<string>(
                name: "ImageCropPath",
                table: "HomeworkCheckItem",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true,
                oldComment: "题目截图裁剪路径。");

            migrationBuilder.AlterColumn<string>(
                name: "Grade",
                table: "HomeworkCheckItem",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "年级。");

            migrationBuilder.AlterColumn<string>(
                name: "Explanation",
                table: "HomeworkCheckItem",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "讲解内容。");

            migrationBuilder.AlterColumn<string>(
                name: "ErrorReason",
                table: "HomeworkCheckItem",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "错误原因。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedTime",
                table: "HomeworkCheckItem",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "创建时间，使用 UTC 时间记录。");

            migrationBuilder.AlterColumn<string>(
                name: "CorrectAnswer",
                table: "HomeworkCheckItem",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "正确答案。");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "HomeworkCheckItem",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldComment: "主键，使用 32 位无分隔符 Guid 字符串。");

            migrationBuilder.AlterColumn<string>(
                name: "VideoPath",
                table: "AnswerRecord",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true,
                oldComment: "视频文件路径。");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "AnswerRecord",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldComment: "用户 Id，关联 UserProfile.Id。");

            migrationBuilder.AlterColumn<string>(
                name: "QuestionRecordId",
                table: "AnswerRecord",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldComment: "提问记录 Id，关联 QuestionRecord.Id。");

            migrationBuilder.AlterColumn<string>(
                name: "PromptVersion",
                table: "AnswerRecord",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "Prompt 版本。");

            migrationBuilder.AlterColumn<string>(
                name: "OutputType",
                table: "AnswerRecord",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "输出类型。");

            migrationBuilder.AlterColumn<string>(
                name: "ModelName",
                table: "AnswerRecord",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "模型名称。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedTime",
                table: "AnswerRecord",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "创建时间，使用 UTC 时间记录。");

            migrationBuilder.AlterColumn<string>(
                name: "AvatarScriptJson",
                table: "AnswerRecord",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "数字人脚本 JSON。");

            migrationBuilder.AlterColumn<string>(
                name: "AudioPath",
                table: "AnswerRecord",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true,
                oldComment: "音频文件路径。");

            migrationBuilder.AlterColumn<string>(
                name: "AnswerText",
                table: "AnswerRecord",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldComment: "AI 回答文本。");

            migrationBuilder.AlterColumn<string>(
                name: "AnswerJson",
                table: "AnswerRecord",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "AI 回答结构化 JSON。");

            migrationBuilder.AlterColumn<string>(
                name: "AgentName",
                table: "AnswerRecord",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "Agent 名称。");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "AnswerRecord",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldComment: "主键，使用 32 位无分隔符 Guid 字符串。");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "AgentRouteLog",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true,
                oldComment: "用户 Id，关联 UserProfile.Id。");

            migrationBuilder.AlterColumn<string>(
                name: "SessionId",
                table: "AgentRouteLog",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true,
                oldComment: "学习会话 Id，关联 LearningSession.Id。");

            migrationBuilder.AlterColumn<string>(
                name: "SelectedModelProvider",
                table: "AgentRouteLog",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "路由选中的模型提供方。");

            migrationBuilder.AlterColumn<string>(
                name: "SelectedModelName",
                table: "AgentRouteLog",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "路由选中的模型名称。");

            migrationBuilder.AlterColumn<string>(
                name: "SelectedAgent",
                table: "AgentRouteLog",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldComment: "路由选中的 Agent。");

            migrationBuilder.AlterColumn<string>(
                name: "RouteReason",
                table: "AgentRouteLog",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "路由原因。");

            migrationBuilder.AlterColumn<string>(
                name: "QuestionRecordId",
                table: "AgentRouteLog",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true,
                oldComment: "提问记录 Id，关联 QuestionRecord.Id。");

            migrationBuilder.AlterColumn<string>(
                name: "QuestionMode",
                table: "AgentRouteLog",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "路由识别的提问模式。");

            migrationBuilder.AlterColumn<string>(
                name: "InputType",
                table: "AgentRouteLog",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "Agent 输入类型。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedTime",
                table: "AgentRouteLog",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "创建时间，使用 UTC 时间记录。");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "AgentRouteLog",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldComment: "主键，使用 32 位无分隔符 Guid 字符串。");
        }
    }
}
