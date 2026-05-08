BEGIN TRANSACTION;
DECLARE @defaultSchema AS sysname;
SET @defaultSchema = SCHEMA_NAME();
DECLARE @description AS sql_variant;
SET @description = N'错题复习记录表，保存学生复习错题时的作答、反馈和耗时。';
EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'WrongQuestionReview';

DECLARE @defaultSchema1 AS sysname;
SET @defaultSchema1 = SCHEMA_NAME();
DECLARE @description1 AS sql_variant;
SET @description1 = N'错题表，保存错题原题、答案、错因、讲解、掌握状态和复习时间。';
EXEC sp_addextendedproperty 'MS_Description', @description1, 'SCHEMA', @defaultSchema1, 'TABLE', N'WrongQuestion';

DECLARE @defaultSchema2 AS sysname;
SET @defaultSchema2 = SCHEMA_NAME();
DECLARE @description2 AS sql_variant;
SET @description2 = N'用户基础信息表，保存学生、家长或维护人员的基础身份信息。';
EXEC sp_addextendedproperty 'MS_Description', @description2, 'SCHEMA', @defaultSchema2, 'TABLE', N'UserProfile';

DECLARE @defaultSchema3 AS sysname;
SET @defaultSchema3 = SCHEMA_NAME();
DECLARE @description3 AS sql_variant;
SET @description3 = N'教材单元表，按教材组织单元目录和排序信息。';
EXEC sp_addextendedproperty 'MS_Description', @description3, 'SCHEMA', @defaultSchema3, 'TABLE', N'TextbookUnit';

DECLARE @defaultSchema4 AS sysname;
SET @defaultSchema4 = SCHEMA_NAME();
DECLARE @description4 AS sql_variant;
SET @description4 = N'教材解析日志表，记录教材导入过程中的步骤、状态、消息和错误详情。';
EXEC sp_addextendedproperty 'MS_Description', @description4, 'SCHEMA', @defaultSchema4, 'TABLE', N'TextbookParseLog';

DECLARE @defaultSchema5 AS sysname;
SET @defaultSchema5 = SCHEMA_NAME();
DECLARE @description5 AS sql_variant;
SET @description5 = N'教材页表，保存教材页面图片、PDF 路径、OCR 原文和清洗文本。';
EXEC sp_addextendedproperty 'MS_Description', @description5, 'SCHEMA', @defaultSchema5, 'TABLE', N'TextbookPage';

DECLARE @defaultSchema6 AS sysname;
SET @defaultSchema6 = SCHEMA_NAME();
DECLARE @description6 AS sql_variant;
SET @description6 = N'教材导入任务表，记录教材上传、解析、OCR、切片和向量化流程状态。';
EXEC sp_addextendedproperty 'MS_Description', @description6, 'SCHEMA', @defaultSchema6, 'TABLE', N'TextbookImportJob';

DECLARE @defaultSchema7 AS sysname;
SET @defaultSchema7 = SCHEMA_NAME();
DECLARE @description7 AS sql_variant;
SET @description7 = N'教材导入文件表，记录导入任务关联的 PDF、图片等源文件信息。';
EXEC sp_addextendedproperty 'MS_Description', @description7, 'SCHEMA', @defaultSchema7, 'TABLE', N'TextbookImportFile';

DECLARE @defaultSchema8 AS sysname;
SET @defaultSchema8 = SCHEMA_NAME();
DECLARE @description8 AS sql_variant;
SET @description8 = N'教材主表，保存教材名称、出版社、学科、年级、册别、版本和封面信息。';
EXEC sp_addextendedproperty 'MS_Description', @description8, 'SCHEMA', @defaultSchema8, 'TABLE', N'Textbook';

DECLARE @defaultSchema9 AS sysname;
SET @defaultSchema9 = SCHEMA_NAME();
DECLARE @description9 AS sql_variant;
SET @description9 = N'学习阶段表，保存学习计划拆分出的阶段目标、状态和时间。';
EXEC sp_addextendedproperty 'MS_Description', @description9, 'SCHEMA', @defaultSchema9, 'TABLE', N'StudyStage';

DECLARE @defaultSchema10 AS sysname;
SET @defaultSchema10 = SCHEMA_NAME();
DECLARE @description10 AS sql_variant;
SET @description10 = N'学习计划表，保存面向知识点、错题或考试准备的阶段化学习计划。';
EXEC sp_addextendedproperty 'MS_Description', @description10, 'SCHEMA', @defaultSchema10, 'TABLE', N'StudyPlan';

DECLARE @defaultSchema11 AS sysname;
SET @defaultSchema11 = SCHEMA_NAME();
DECLARE @description11 AS sql_variant;
SET @description11 = N'阶段任务记录表，保存学生完成阶段任务时的动作、结果、得分和备注。';
EXEC sp_addextendedproperty 'MS_Description', @description11, 'SCHEMA', @defaultSchema11, 'TABLE', N'StageTaskRecord';

DECLARE @defaultSchema12 AS sysname;
SET @defaultSchema12 = SCHEMA_NAME();
DECLARE @description12 AS sql_variant;
SET @description12 = N'阶段任务表，保存每个学习阶段中的讲解、练习、复习、语音讨论或视频任务。';
EXEC sp_addextendedproperty 'MS_Description', @description12, 'SCHEMA', @defaultSchema12, 'TABLE', N'StageTask';

DECLARE @defaultSchema13 AS sysname;
SET @defaultSchema13 = SCHEMA_NAME();
DECLARE @description13 AS sql_variant;
SET @description13 = N'会话消息表，记录学习会话中的文本、图片、音频、视频和数字人脚本消息。';
EXEC sp_addextendedproperty 'MS_Description', @description13, 'SCHEMA', @defaultSchema13, 'TABLE', N'SessionMessage';

DECLARE @defaultSchema14 AS sysname;
SET @defaultSchema14 = SCHEMA_NAME();
DECLARE @description14 AS sql_variant;
SET @description14 = N'复习计划表，保存错题、知识点或练习题的下次复习安排。';
EXEC sp_addextendedproperty 'MS_Description', @description14, 'SCHEMA', @defaultSchema14, 'TABLE', N'ReviewSchedule';

DECLARE @defaultSchema15 AS sysname;
SET @defaultSchema15 = SCHEMA_NAME();
DECLARE @description15 AS sql_variant;
SET @description15 = N'提问记录表，保存学生输入的问题文本、图片、音频、识别文本和提问模式。';
EXEC sp_addextendedproperty 'MS_Description', @description15, 'SCHEMA', @defaultSchema15, 'TABLE', N'QuestionRecord';

DECLARE @defaultSchema16 AS sysname;
SET @defaultSchema16 = SCHEMA_NAME();
DECLARE @description16 AS sql_variant;
SET @description16 = N'练习题来源表，记录练习题与错题、知识点、教材切片等来源的关系。';
EXEC sp_addextendedproperty 'MS_Description', @description16, 'SCHEMA', @defaultSchema16, 'TABLE', N'PracticeQuestionSource';

DECLARE @defaultSchema17 AS sysname;
SET @defaultSchema17 = SCHEMA_NAME();
DECLARE @description17 AS sql_variant;
SET @description17 = N'练习题表，保存由错题、知识点、阶段任务或人工生成的练习题。';
EXEC sp_addextendedproperty 'MS_Description', @description17, 'SCHEMA', @defaultSchema17, 'TABLE', N'PracticeQuestion';

DECLARE @defaultSchema18 AS sysname;
SET @defaultSchema18 = SCHEMA_NAME();
DECLARE @description18 AS sql_variant;
SET @description18 = N'练习作答记录表，保存学生练习题作答结果、反馈和耗时。';
EXEC sp_addextendedproperty 'MS_Description', @description18, 'SCHEMA', @defaultSchema18, 'TABLE', N'PracticeAnswerRecord';

DECLARE @defaultSchema19 AS sysname;
SET @defaultSchema19 = SCHEMA_NAME();
DECLARE @description19 AS sql_variant;
SET @description19 = N'模型调用日志表，记录每次模型调用的输入输出、Token、耗时、成本和错误。';
EXEC sp_addextendedproperty 'MS_Description', @description19, 'SCHEMA', @defaultSchema19, 'TABLE', N'ModelCallLog';

DECLARE @defaultSchema20 AS sysname;
SET @defaultSchema20 = SCHEMA_NAME();
DECLARE @description20 AS sql_variant;
SET @description20 = N'媒体资源表，统一管理图片、音频、视频、PDF、裁剪图和数字人脚本资源。';
EXEC sp_addextendedproperty 'MS_Description', @description20, 'SCHEMA', @defaultSchema20, 'TABLE', N'MediaResource';

DECLARE @defaultSchema21 AS sysname;
SET @defaultSchema21 = SCHEMA_NAME();
DECLARE @description21 AS sql_variant;
SET @description21 = N'教材课时或章节表，保存课题、页码范围和所属单元。';
EXEC sp_addextendedproperty 'MS_Description', @description21, 'SCHEMA', @defaultSchema21, 'TABLE', N'Lesson';

DECLARE @defaultSchema22 AS sysname;
SET @defaultSchema22 = SCHEMA_NAME();
DECLARE @description22 AS sql_variant;
SET @description22 = N'学习会话表，记录一次问答、拍照讲题、作业检查或复习过程。';
EXEC sp_addextendedproperty 'MS_Description', @description22, 'SCHEMA', @defaultSchema22, 'TABLE', N'LearningSession';

DECLARE @defaultSchema23 AS sysname;
SET @defaultSchema23 = SCHEMA_NAME();
DECLARE @description23 AS sql_variant;
SET @description23 = N'学生学习档案表，保存年级、学期、学校、默认学科和教材版本等学习画像。';
EXEC sp_addextendedproperty 'MS_Description', @description23, 'SCHEMA', @defaultSchema23, 'TABLE', N'LearningProfile';

DECLARE @defaultSchema24 AS sysname;
SET @defaultSchema24 = SCHEMA_NAME();
DECLARE @description24 AS sql_variant;
SET @description24 = N'知识点表，保存学科年级下的知识点、层级关系和难度信息。';
EXEC sp_addextendedproperty 'MS_Description', @description24, 'SCHEMA', @defaultSchema24, 'TABLE', N'KnowledgePoint';

DECLARE @defaultSchema25 AS sysname;
SET @defaultSchema25 = SCHEMA_NAME();
DECLARE @description25 AS sql_variant;
SET @description25 = N'知识向量表，预留向量检索模型、向量数据和哈希信息。';
EXEC sp_addextendedproperty 'MS_Description', @description25, 'SCHEMA', @defaultSchema25, 'TABLE', N'KnowledgeEmbedding';

DECLARE @defaultSchema26 AS sysname;
SET @defaultSchema26 = SCHEMA_NAME();
DECLARE @description26 AS sql_variant;
SET @description26 = N'知识片段表，保存教材切片文本及其来源、页码和知识点关联。';
EXEC sp_addextendedproperty 'MS_Description', @description26, 'SCHEMA', @defaultSchema26, 'TABLE', N'KnowledgeChunk';

DECLARE @defaultSchema27 AS sysname;
SET @defaultSchema27 = SCHEMA_NAME();
DECLARE @description27 AS sql_variant;
SET @description27 = N'作业检查明细表，保存一张作业图片拆出的单题检查结果。';
EXEC sp_addextendedproperty 'MS_Description', @description27, 'SCHEMA', @defaultSchema27, 'TABLE', N'HomeworkCheckItem';

DECLARE @defaultSchema28 AS sysname;
SET @defaultSchema28 = SCHEMA_NAME();
DECLARE @description28 AS sql_variant;
SET @description28 = N'回答记录表，保存 AI 回答文本、结构化结果、模型、Agent 和多媒体输出。';
EXEC sp_addextendedproperty 'MS_Description', @description28, 'SCHEMA', @defaultSchema28, 'TABLE', N'AnswerRecord';

DECLARE @defaultSchema29 AS sysname;
SET @defaultSchema29 = SCHEMA_NAME();
DECLARE @description29 AS sql_variant;
SET @description29 = N'Agent 路由日志表，记录每次请求选择的 Agent、模型和路由原因。';
EXEC sp_addextendedproperty 'MS_Description', @description29, 'SCHEMA', @defaultSchema29, 'TABLE', N'AgentRouteLog';

DECLARE @defaultSchema30 AS sysname;
SET @defaultSchema30 = SCHEMA_NAME();
DECLARE @description30 AS sql_variant;
SET @description30 = N'错题 Id，关联 WrongQuestion.Id。';
EXEC sp_addextendedproperty 'MS_Description', @description30, 'SCHEMA', @defaultSchema30, 'TABLE', N'WrongQuestionReview', 'COLUMN', N'WrongQuestionId';

DECLARE @defaultSchema31 AS sysname;
SET @defaultSchema31 = SCHEMA_NAME();
DECLARE @description31 AS sql_variant;
SET @description31 = N'用户 Id，关联 UserProfile.Id。';
EXEC sp_addextendedproperty 'MS_Description', @description31, 'SCHEMA', @defaultSchema31, 'TABLE', N'WrongQuestionReview', 'COLUMN', N'UserId';

DECLARE @defaultSchema32 AS sysname;
SET @defaultSchema32 = SCHEMA_NAME();
DECLARE @description32 AS sql_variant;
SET @description32 = N'用户作答内容。';
EXEC sp_addextendedproperty 'MS_Description', @description32, 'SCHEMA', @defaultSchema32, 'TABLE', N'WrongQuestionReview', 'COLUMN', N'UserAnswer';

DECLARE @defaultSchema33 AS sysname;
SET @defaultSchema33 = SCHEMA_NAME();
DECLARE @description33 AS sql_variant;
SET @description33 = N'复习类型。';
EXEC sp_addextendedproperty 'MS_Description', @description33, 'SCHEMA', @defaultSchema33, 'TABLE', N'WrongQuestionReview', 'COLUMN', N'ReviewType';

DECLARE @defaultSchema34 AS sysname;
SET @defaultSchema34 = SCHEMA_NAME();
DECLARE @description34 AS sql_variant;
SET @description34 = N'复习耗时，单位秒。';
EXEC sp_addextendedproperty 'MS_Description', @description34, 'SCHEMA', @defaultSchema34, 'TABLE', N'WrongQuestionReview', 'COLUMN', N'ReviewDurationSec';

DECLARE @defaultSchema35 AS sysname;
SET @defaultSchema35 = SCHEMA_NAME();
DECLARE @description35 AS sql_variant;
SET @description35 = N'是否正确。';
EXEC sp_addextendedproperty 'MS_Description', @description35, 'SCHEMA', @defaultSchema35, 'TABLE', N'WrongQuestionReview', 'COLUMN', N'IsCorrect';

DECLARE @defaultSchema36 AS sysname;
SET @defaultSchema36 = SCHEMA_NAME();
DECLARE @description36 AS sql_variant;
SET @description36 = N'创建时间，使用 UTC 时间记录。';
EXEC sp_addextendedproperty 'MS_Description', @description36, 'SCHEMA', @defaultSchema36, 'TABLE', N'WrongQuestionReview', 'COLUMN', N'CreatedTime';

DECLARE @defaultSchema37 AS sysname;
SET @defaultSchema37 = SCHEMA_NAME();
DECLARE @description37 AS sql_variant;
SET @description37 = N'AI 反馈。';
EXEC sp_addextendedproperty 'MS_Description', @description37, 'SCHEMA', @defaultSchema37, 'TABLE', N'WrongQuestionReview', 'COLUMN', N'AiFeedback';

DECLARE @defaultSchema38 AS sysname;
SET @defaultSchema38 = SCHEMA_NAME();
DECLARE @description38 AS sql_variant;
SET @description38 = N'主键，使用 32 位无分隔符 Guid 字符串。';
EXEC sp_addextendedproperty 'MS_Description', @description38, 'SCHEMA', @defaultSchema38, 'TABLE', N'WrongQuestionReview', 'COLUMN', N'Id';

DECLARE @defaultSchema39 AS sysname;
SET @defaultSchema39 = SCHEMA_NAME();
DECLARE @description39 AS sql_variant;
SET @description39 = N'用户 Id，关联 UserProfile.Id。';
EXEC sp_addextendedproperty 'MS_Description', @description39, 'SCHEMA', @defaultSchema39, 'TABLE', N'WrongQuestion', 'COLUMN', N'UserId';

DECLARE @defaultSchema40 AS sysname;
SET @defaultSchema40 = SCHEMA_NAME();
DECLARE @description40 AS sql_variant;
SET @description40 = N'最后更新时间，未更新时为空。';
EXEC sp_addextendedproperty 'MS_Description', @description40, 'SCHEMA', @defaultSchema40, 'TABLE', N'WrongQuestion', 'COLUMN', N'UpdatedTime';

DECLARE @defaultSchema41 AS sysname;
SET @defaultSchema41 = SCHEMA_NAME();
DECLARE @description41 AS sql_variant;
SET @description41 = N'学科。';
EXEC sp_addextendedproperty 'MS_Description', @description41, 'SCHEMA', @defaultSchema41, 'TABLE', N'WrongQuestion', 'COLUMN', N'Subject';

DECLARE @defaultSchema42 AS sysname;
SET @defaultSchema42 = SCHEMA_NAME();
DECLARE @description42 AS sql_variant;
SET @description42 = N'学生答案。';
EXEC sp_addextendedproperty 'MS_Description', @description42, 'SCHEMA', @defaultSchema42, 'TABLE', N'WrongQuestion', 'COLUMN', N'StudentAnswer';

DECLARE @defaultSchema43 AS sysname;
SET @defaultSchema43 = SCHEMA_NAME();
DECLARE @description43 AS sql_variant;
SET @description43 = N'复习次数。';
EXEC sp_addextendedproperty 'MS_Description', @description43, 'SCHEMA', @defaultSchema43, 'TABLE', N'WrongQuestion', 'COLUMN', N'ReviewCount';

DECLARE @defaultSchema44 AS sysname;
SET @defaultSchema44 = SCHEMA_NAME();
DECLARE @description44 AS sql_variant;
SET @description44 = N'题目或问题文本。';
EXEC sp_addextendedproperty 'MS_Description', @description44, 'SCHEMA', @defaultSchema44, 'TABLE', N'WrongQuestion', 'COLUMN', N'QuestionText';

DECLARE @defaultSchema45 AS sysname;
SET @defaultSchema45 = SCHEMA_NAME();
DECLARE @description45 AS sql_variant;
SET @description45 = N'提问记录 Id，关联 QuestionRecord.Id。';
EXEC sp_addextendedproperty 'MS_Description', @description45, 'SCHEMA', @defaultSchema45, 'TABLE', N'WrongQuestion', 'COLUMN', N'QuestionRecordId';

DECLARE @defaultSchema46 AS sysname;
SET @defaultSchema46 = SCHEMA_NAME();
DECLARE @description46 AS sql_variant;
SET @description46 = N'题目图片路径。';
EXEC sp_addextendedproperty 'MS_Description', @description46, 'SCHEMA', @defaultSchema46, 'TABLE', N'WrongQuestion', 'COLUMN', N'QuestionImagePath';

DECLARE @defaultSchema47 AS sysname;
SET @defaultSchema47 = SCHEMA_NAME();
DECLARE @description47 AS sql_variant;
SET @description47 = N'下次复习时间。';
EXEC sp_addextendedproperty 'MS_Description', @description47, 'SCHEMA', @defaultSchema47, 'TABLE', N'WrongQuestion', 'COLUMN', N'NextReviewTime';

DECLARE @defaultSchema48 AS sysname;
SET @defaultSchema48 = SCHEMA_NAME();
DECLARE @description48 AS sql_variant;
SET @description48 = N'掌握状态。';
EXEC sp_addextendedproperty 'MS_Description', @description48, 'SCHEMA', @defaultSchema48, 'TABLE', N'WrongQuestion', 'COLUMN', N'MasteryStatus';

DECLARE @defaultSchema49 AS sysname;
SET @defaultSchema49 = SCHEMA_NAME();
DECLARE @description49 AS sql_variant;
SET @description49 = N'上次复习时间。';
EXEC sp_addextendedproperty 'MS_Description', @description49, 'SCHEMA', @defaultSchema49, 'TABLE', N'WrongQuestion', 'COLUMN', N'LastReviewTime';

DECLARE @defaultSchema50 AS sysname;
SET @defaultSchema50 = SCHEMA_NAME();
DECLARE @description50 AS sql_variant;
SET @description50 = N'知识点 Id，关联 KnowledgePoint.Id。';
EXEC sp_addextendedproperty 'MS_Description', @description50, 'SCHEMA', @defaultSchema50, 'TABLE', N'WrongQuestion', 'COLUMN', N'KnowledgePointId';

DECLARE @defaultSchema51 AS sysname;
SET @defaultSchema51 = SCHEMA_NAME();
DECLARE @description51 AS sql_variant;
SET @description51 = N'作业检查明细 Id，关联 HomeworkCheckItem.Id。';
EXEC sp_addextendedproperty 'MS_Description', @description51, 'SCHEMA', @defaultSchema51, 'TABLE', N'WrongQuestion', 'COLUMN', N'HomeworkCheckItemId';

DECLARE @defaultSchema52 AS sysname;
SET @defaultSchema52 = SCHEMA_NAME();
DECLARE @description52 AS sql_variant;
SET @description52 = N'年级。';
EXEC sp_addextendedproperty 'MS_Description', @description52, 'SCHEMA', @defaultSchema52, 'TABLE', N'WrongQuestion', 'COLUMN', N'Grade';

DECLARE @defaultSchema53 AS sysname;
SET @defaultSchema53 = SCHEMA_NAME();
DECLARE @description53 AS sql_variant;
SET @description53 = N'讲解内容。';
EXEC sp_addextendedproperty 'MS_Description', @description53, 'SCHEMA', @defaultSchema53, 'TABLE', N'WrongQuestion', 'COLUMN', N'Explanation';

DECLARE @defaultSchema54 AS sysname;
SET @defaultSchema54 = SCHEMA_NAME();
DECLARE @description54 AS sql_variant;
SET @description54 = N'错误原因。';
EXEC sp_addextendedproperty 'MS_Description', @description54, 'SCHEMA', @defaultSchema54, 'TABLE', N'WrongQuestion', 'COLUMN', N'ErrorReason';

DECLARE @defaultSchema55 AS sysname;
SET @defaultSchema55 = SCHEMA_NAME();
DECLARE @description55 AS sql_variant;
SET @description55 = N'难度等级，默认 1。';
EXEC sp_addextendedproperty 'MS_Description', @description55, 'SCHEMA', @defaultSchema55, 'TABLE', N'WrongQuestion', 'COLUMN', N'DifficultyLevel';

DECLARE @defaultSchema56 AS sysname;
SET @defaultSchema56 = SCHEMA_NAME();
DECLARE @description56 AS sql_variant;
SET @description56 = N'创建时间，使用 UTC 时间记录。';
EXEC sp_addextendedproperty 'MS_Description', @description56, 'SCHEMA', @defaultSchema56, 'TABLE', N'WrongQuestion', 'COLUMN', N'CreatedTime';

DECLARE @defaultSchema57 AS sysname;
SET @defaultSchema57 = SCHEMA_NAME();
DECLARE @description57 AS sql_variant;
SET @description57 = N'正确答案。';
EXEC sp_addextendedproperty 'MS_Description', @description57, 'SCHEMA', @defaultSchema57, 'TABLE', N'WrongQuestion', 'COLUMN', N'CorrectAnswer';

DECLARE @defaultSchema58 AS sysname;
SET @defaultSchema58 = SCHEMA_NAME();
DECLARE @description58 AS sql_variant;
SET @description58 = N'主键，使用 32 位无分隔符 Guid 字符串。';
EXEC sp_addextendedproperty 'MS_Description', @description58, 'SCHEMA', @defaultSchema58, 'TABLE', N'WrongQuestion', 'COLUMN', N'Id';

DECLARE @defaultSchema59 AS sysname;
SET @defaultSchema59 = SCHEMA_NAME();
DECLARE @description59 AS sql_variant;
SET @description59 = N'用户类型，例如 student、parent 或 admin。';
EXEC sp_addextendedproperty 'MS_Description', @description59, 'SCHEMA', @defaultSchema59, 'TABLE', N'UserProfile', 'COLUMN', N'UserType';

DECLARE @defaultSchema60 AS sysname;
SET @defaultSchema60 = SCHEMA_NAME();
DECLARE @description60 AS sql_variant;
SET @description60 = N'最后更新时间，未更新时为空。';
EXEC sp_addextendedproperty 'MS_Description', @description60, 'SCHEMA', @defaultSchema60, 'TABLE', N'UserProfile', 'COLUMN', N'UpdatedTime';

DECLARE @defaultSchema61 AS sysname;
SET @defaultSchema61 = SCHEMA_NAME();
DECLARE @description61 AS sql_variant;
SET @description61 = N'手机号，第一阶段可为空。';
EXEC sp_addextendedproperty 'MS_Description', @description61, 'SCHEMA', @defaultSchema61, 'TABLE', N'UserProfile', 'COLUMN', N'PhoneNumber';

DECLARE @defaultSchema62 AS sysname;
SET @defaultSchema62 = SCHEMA_NAME();
DECLARE @description62 AS sql_variant;
SET @description62 = N'用户显示名称。';
EXEC sp_addextendedproperty 'MS_Description', @description62, 'SCHEMA', @defaultSchema62, 'TABLE', N'UserProfile', 'COLUMN', N'DisplayName';

DECLARE @defaultSchema63 AS sysname;
SET @defaultSchema63 = SCHEMA_NAME();
DECLARE @description63 AS sql_variant;
SET @description63 = N'创建时间，使用 UTC 时间记录。';
EXEC sp_addextendedproperty 'MS_Description', @description63, 'SCHEMA', @defaultSchema63, 'TABLE', N'UserProfile', 'COLUMN', N'CreatedTime';

DECLARE @defaultSchema64 AS sysname;
SET @defaultSchema64 = SCHEMA_NAME();
DECLARE @description64 AS sql_variant;
SET @description64 = N'用户头像文件路径。';
EXEC sp_addextendedproperty 'MS_Description', @description64, 'SCHEMA', @defaultSchema64, 'TABLE', N'UserProfile', 'COLUMN', N'AvatarPath';

DECLARE @defaultSchema65 AS sysname;
SET @defaultSchema65 = SCHEMA_NAME();
DECLARE @description65 AS sql_variant;
SET @description65 = N'主键，使用 32 位无分隔符 Guid 字符串。';
EXEC sp_addextendedproperty 'MS_Description', @description65, 'SCHEMA', @defaultSchema65, 'TABLE', N'UserProfile', 'COLUMN', N'Id';

DECLARE @defaultSchema66 AS sysname;
SET @defaultSchema66 = SCHEMA_NAME();
DECLARE @description66 AS sql_variant;
SET @description66 = N'最后更新时间，未更新时为空。';
EXEC sp_addextendedproperty 'MS_Description', @description66, 'SCHEMA', @defaultSchema66, 'TABLE', N'TextbookUnit', 'COLUMN', N'UpdatedTime';

DECLARE @defaultSchema67 AS sysname;
SET @defaultSchema67 = SCHEMA_NAME();
DECLARE @description67 AS sql_variant;
SET @description67 = N'单元序号。';
EXEC sp_addextendedproperty 'MS_Description', @description67, 'SCHEMA', @defaultSchema67, 'TABLE', N'TextbookUnit', 'COLUMN', N'UnitNo';

DECLARE @defaultSchema68 AS sysname;
SET @defaultSchema68 = SCHEMA_NAME();
DECLARE @description68 AS sql_variant;
SET @description68 = N'标题。';
EXEC sp_addextendedproperty 'MS_Description', @description68, 'SCHEMA', @defaultSchema68, 'TABLE', N'TextbookUnit', 'COLUMN', N'Title';

DECLARE @defaultSchema69 AS sysname;
SET @defaultSchema69 = SCHEMA_NAME();
DECLARE @description69 AS sql_variant;
SET @description69 = N'教材 Id，关联 Textbook.Id。';
EXEC sp_addextendedproperty 'MS_Description', @description69, 'SCHEMA', @defaultSchema69, 'TABLE', N'TextbookUnit', 'COLUMN', N'TextbookId';

DECLARE @defaultSchema70 AS sysname;
SET @defaultSchema70 = SCHEMA_NAME();
DECLARE @description70 AS sql_variant;
SET @description70 = N'排序值，数值越小越靠前。';
EXEC sp_addextendedproperty 'MS_Description', @description70, 'SCHEMA', @defaultSchema70, 'TABLE', N'TextbookUnit', 'COLUMN', N'SortIndex';

DECLARE @defaultSchema71 AS sysname;
SET @defaultSchema71 = SCHEMA_NAME();
DECLARE @description71 AS sql_variant;
SET @description71 = N'详细说明。';
EXEC sp_addextendedproperty 'MS_Description', @description71, 'SCHEMA', @defaultSchema71, 'TABLE', N'TextbookUnit', 'COLUMN', N'Description';

DECLARE @defaultSchema72 AS sysname;
SET @defaultSchema72 = SCHEMA_NAME();
DECLARE @description72 AS sql_variant;
SET @description72 = N'创建时间，使用 UTC 时间记录。';
EXEC sp_addextendedproperty 'MS_Description', @description72, 'SCHEMA', @defaultSchema72, 'TABLE', N'TextbookUnit', 'COLUMN', N'CreatedTime';

DECLARE @defaultSchema73 AS sysname;
SET @defaultSchema73 = SCHEMA_NAME();
DECLARE @description73 AS sql_variant;
SET @description73 = N'主键，使用 32 位无分隔符 Guid 字符串。';
EXEC sp_addextendedproperty 'MS_Description', @description73, 'SCHEMA', @defaultSchema73, 'TABLE', N'TextbookUnit', 'COLUMN', N'Id';

DECLARE @defaultSchema74 AS sysname;
SET @defaultSchema74 = SCHEMA_NAME();
DECLARE @description74 AS sql_variant;
SET @description74 = N'教材 Id，关联 Textbook.Id。';
EXEC sp_addextendedproperty 'MS_Description', @description74, 'SCHEMA', @defaultSchema74, 'TABLE', N'TextbookParseLog', 'COLUMN', N'TextbookId';

DECLARE @defaultSchema75 AS sysname;
SET @defaultSchema75 = SCHEMA_NAME();
DECLARE @description75 AS sql_variant;
SET @description75 = N'处理步骤名称。';
EXEC sp_addextendedproperty 'MS_Description', @description75, 'SCHEMA', @defaultSchema75, 'TABLE', N'TextbookParseLog', 'COLUMN', N'StepName';

DECLARE @defaultSchema76 AS sysname;
SET @defaultSchema76 = SCHEMA_NAME();
DECLARE @description76 AS sql_variant;
SET @description76 = N'状态。';
EXEC sp_addextendedproperty 'MS_Description', @description76, 'SCHEMA', @defaultSchema76, 'TABLE', N'TextbookParseLog', 'COLUMN', N'Status';

DECLARE @defaultSchema77 AS sysname;
SET @defaultSchema77 = SCHEMA_NAME();
DECLARE @description77 AS sql_variant;
SET @description77 = N'页码。';
EXEC sp_addextendedproperty 'MS_Description', @description77, 'SCHEMA', @defaultSchema77, 'TABLE', N'TextbookParseLog', 'COLUMN', N'PageNo';

DECLARE @defaultSchema78 AS sysname;
SET @defaultSchema78 = SCHEMA_NAME();
DECLARE @description78 AS sql_variant;
SET @description78 = N'日志消息。';
EXEC sp_addextendedproperty 'MS_Description', @description78, 'SCHEMA', @defaultSchema78, 'TABLE', N'TextbookParseLog', 'COLUMN', N'Message';

DECLARE @defaultSchema79 AS sysname;
SET @defaultSchema79 = SCHEMA_NAME();
DECLARE @description79 AS sql_variant;
SET @description79 = N'教材导入任务 Id，关联 TextbookImportJob.Id。';
EXEC sp_addextendedproperty 'MS_Description', @description79, 'SCHEMA', @defaultSchema79, 'TABLE', N'TextbookParseLog', 'COLUMN', N'ImportJobId';

DECLARE @defaultSchema80 AS sysname;
SET @defaultSchema80 = SCHEMA_NAME();
DECLARE @description80 AS sql_variant;
SET @description80 = N'错误详细信息。';
EXEC sp_addextendedproperty 'MS_Description', @description80, 'SCHEMA', @defaultSchema80, 'TABLE', N'TextbookParseLog', 'COLUMN', N'ErrorDetail';

DECLARE @defaultSchema81 AS sysname;
SET @defaultSchema81 = SCHEMA_NAME();
DECLARE @description81 AS sql_variant;
SET @description81 = N'创建时间，使用 UTC 时间记录。';
EXEC sp_addextendedproperty 'MS_Description', @description81, 'SCHEMA', @defaultSchema81, 'TABLE', N'TextbookParseLog', 'COLUMN', N'CreatedTime';

DECLARE @defaultSchema82 AS sysname;
SET @defaultSchema82 = SCHEMA_NAME();
DECLARE @description82 AS sql_variant;
SET @description82 = N'主键，使用 32 位无分隔符 Guid 字符串。';
EXEC sp_addextendedproperty 'MS_Description', @description82, 'SCHEMA', @defaultSchema82, 'TABLE', N'TextbookParseLog', 'COLUMN', N'Id';

DECLARE @defaultSchema83 AS sysname;
SET @defaultSchema83 = SCHEMA_NAME();
DECLARE @description83 AS sql_variant;
SET @description83 = N'最后更新时间，未更新时为空。';
EXEC sp_addextendedproperty 'MS_Description', @description83, 'SCHEMA', @defaultSchema83, 'TABLE', N'TextbookPage', 'COLUMN', N'UpdatedTime';

DECLARE @defaultSchema84 AS sysname;
SET @defaultSchema84 = SCHEMA_NAME();
DECLARE @description84 AS sql_variant;
SET @description84 = N'教材单元 Id，关联 TextbookUnit.Id。';
EXEC sp_addextendedproperty 'MS_Description', @description84, 'SCHEMA', @defaultSchema84, 'TABLE', N'TextbookPage', 'COLUMN', N'UnitId';

DECLARE @defaultSchema85 AS sysname;
SET @defaultSchema85 = SCHEMA_NAME();
DECLARE @description85 AS sql_variant;
SET @description85 = N'教材 Id，关联 Textbook.Id。';
EXEC sp_addextendedproperty 'MS_Description', @description85, 'SCHEMA', @defaultSchema85, 'TABLE', N'TextbookPage', 'COLUMN', N'TextbookId';

DECLARE @defaultSchema86 AS sysname;
SET @defaultSchema86 = SCHEMA_NAME();
DECLARE @description86 AS sql_variant;
SET @description86 = N'PDF 文件路径。';
EXEC sp_addextendedproperty 'MS_Description', @description86, 'SCHEMA', @defaultSchema86, 'TABLE', N'TextbookPage', 'COLUMN', N'PdfPath';

DECLARE @defaultSchema87 AS sysname;
SET @defaultSchema87 = SCHEMA_NAME();
DECLARE @description87 AS sql_variant;
SET @description87 = N'页码。';
EXEC sp_addextendedproperty 'MS_Description', @description87, 'SCHEMA', @defaultSchema87, 'TABLE', N'TextbookPage', 'COLUMN', N'PageNo';

DECLARE @defaultSchema88 AS sysname;
SET @defaultSchema88 = SCHEMA_NAME();
DECLARE @description88 AS sql_variant;
SET @description88 = N'OCR 识别原文。';
EXEC sp_addextendedproperty 'MS_Description', @description88, 'SCHEMA', @defaultSchema88, 'TABLE', N'TextbookPage', 'COLUMN', N'OcrText';

DECLARE @defaultSchema89 AS sysname;
SET @defaultSchema89 = SCHEMA_NAME();
DECLARE @description89 AS sql_variant;
SET @description89 = N'课时或章节 Id，关联 Lesson.Id。';
EXEC sp_addextendedproperty 'MS_Description', @description89, 'SCHEMA', @defaultSchema89, 'TABLE', N'TextbookPage', 'COLUMN', N'LessonId';

DECLARE @defaultSchema90 AS sysname;
SET @defaultSchema90 = SCHEMA_NAME();
DECLARE @description90 AS sql_variant;
SET @description90 = N'图片文件路径。';
EXEC sp_addextendedproperty 'MS_Description', @description90, 'SCHEMA', @defaultSchema90, 'TABLE', N'TextbookPage', 'COLUMN', N'ImagePath';

DECLARE @defaultSchema91 AS sysname;
SET @defaultSchema91 = SCHEMA_NAME();
DECLARE @description91 AS sql_variant;
SET @description91 = N'创建时间，使用 UTC 时间记录。';
EXEC sp_addextendedproperty 'MS_Description', @description91, 'SCHEMA', @defaultSchema91, 'TABLE', N'TextbookPage', 'COLUMN', N'CreatedTime';

DECLARE @defaultSchema92 AS sysname;
SET @defaultSchema92 = SCHEMA_NAME();
DECLARE @description92 AS sql_variant;
SET @description92 = N'清洗后的文本。';
EXEC sp_addextendedproperty 'MS_Description', @description92, 'SCHEMA', @defaultSchema92, 'TABLE', N'TextbookPage', 'COLUMN', N'CleanText';

DECLARE @defaultSchema93 AS sysname;
SET @defaultSchema93 = SCHEMA_NAME();
DECLARE @description93 AS sql_variant;
SET @description93 = N'主键，使用 32 位无分隔符 Guid 字符串。';
EXEC sp_addextendedproperty 'MS_Description', @description93, 'SCHEMA', @defaultSchema93, 'TABLE', N'TextbookPage', 'COLUMN', N'Id';

DECLARE @defaultSchema94 AS sysname;
SET @defaultSchema94 = SCHEMA_NAME();
DECLARE @description94 AS sql_variant;
SET @description94 = N'用户 Id，关联 UserProfile.Id。';
EXEC sp_addextendedproperty 'MS_Description', @description94, 'SCHEMA', @defaultSchema94, 'TABLE', N'TextbookImportJob', 'COLUMN', N'UserId';

DECLARE @defaultSchema95 AS sysname;
SET @defaultSchema95 = SCHEMA_NAME();
DECLARE @description95 AS sql_variant;
SET @description95 = N'最后更新时间，未更新时为空。';
EXEC sp_addextendedproperty 'MS_Description', @description95, 'SCHEMA', @defaultSchema95, 'TABLE', N'TextbookImportJob', 'COLUMN', N'UpdatedTime';

DECLARE @defaultSchema96 AS sysname;
SET @defaultSchema96 = SCHEMA_NAME();
DECLARE @description96 AS sql_variant;
SET @description96 = N'总页数。';
EXEC sp_addextendedproperty 'MS_Description', @description96, 'SCHEMA', @defaultSchema96, 'TABLE', N'TextbookImportJob', 'COLUMN', N'TotalPages';

DECLARE @defaultSchema97 AS sysname;
SET @defaultSchema97 = SCHEMA_NAME();
DECLARE @description97 AS sql_variant;
SET @description97 = N'生成的知识片段总数。';
EXEC sp_addextendedproperty 'MS_Description', @description97, 'SCHEMA', @defaultSchema97, 'TABLE', N'TextbookImportJob', 'COLUMN', N'TotalChunks';

DECLARE @defaultSchema98 AS sysname;
SET @defaultSchema98 = SCHEMA_NAME();
DECLARE @description98 AS sql_variant;
SET @description98 = N'教材 Id，关联 Textbook.Id。';
EXEC sp_addextendedproperty 'MS_Description', @description98, 'SCHEMA', @defaultSchema98, 'TABLE', N'TextbookImportJob', 'COLUMN', N'TextbookId';

DECLARE @defaultSchema99 AS sysname;
SET @defaultSchema99 = SCHEMA_NAME();
DECLARE @description99 AS sql_variant;
SET @description99 = N'状态。';
EXEC sp_addextendedproperty 'MS_Description', @description99, 'SCHEMA', @defaultSchema99, 'TABLE', N'TextbookImportJob', 'COLUMN', N'Status';

DECLARE @defaultSchema100 AS sysname;
SET @defaultSchema100 = SCHEMA_NAME();
DECLARE @description100 AS sql_variant;
SET @description100 = N'源文件路径。';
EXEC sp_addextendedproperty 'MS_Description', @description100, 'SCHEMA', @defaultSchema100, 'TABLE', N'TextbookImportJob', 'COLUMN', N'SourceFilePath';

DECLARE @defaultSchema101 AS sysname;
SET @defaultSchema101 = SCHEMA_NAME();
DECLARE @description101 AS sql_variant;
SET @description101 = N'已解析页数。';
EXEC sp_addextendedproperty 'MS_Description', @description101, 'SCHEMA', @defaultSchema101, 'TABLE', N'TextbookImportJob', 'COLUMN', N'ParsedPages';

DECLARE @defaultSchema102 AS sysname;
SET @defaultSchema102 = SCHEMA_NAME();
DECLARE @description102 AS sql_variant;
SET @description102 = N'导入任务名称。';
EXEC sp_addextendedproperty 'MS_Description', @description102, 'SCHEMA', @defaultSchema102, 'TABLE', N'TextbookImportJob', 'COLUMN', N'JobName';

DECLARE @defaultSchema103 AS sysname;
SET @defaultSchema103 = SCHEMA_NAME();
DECLARE @description103 AS sql_variant;
SET @description103 = N'教材导入类型。';
EXEC sp_addextendedproperty 'MS_Description', @description103, 'SCHEMA', @defaultSchema103, 'TABLE', N'TextbookImportJob', 'COLUMN', N'ImportType';

DECLARE @defaultSchema104 AS sysname;
SET @defaultSchema104 = SCHEMA_NAME();
DECLARE @description104 AS sql_variant;
SET @description104 = N'任务完成时间。';
EXEC sp_addextendedproperty 'MS_Description', @description104, 'SCHEMA', @defaultSchema104, 'TABLE', N'TextbookImportJob', 'COLUMN', N'FinishedTime';

DECLARE @defaultSchema105 AS sysname;
SET @defaultSchema105 = SCHEMA_NAME();
DECLARE @description105 AS sql_variant;
SET @description105 = N'错误消息。';
EXEC sp_addextendedproperty 'MS_Description', @description105, 'SCHEMA', @defaultSchema105, 'TABLE', N'TextbookImportJob', 'COLUMN', N'ErrorMessage';

DECLARE @defaultSchema106 AS sysname;
SET @defaultSchema106 = SCHEMA_NAME();
DECLARE @description106 AS sql_variant;
SET @description106 = N'创建时间，使用 UTC 时间记录。';
EXEC sp_addextendedproperty 'MS_Description', @description106, 'SCHEMA', @defaultSchema106, 'TABLE', N'TextbookImportJob', 'COLUMN', N'CreatedTime';

DECLARE @defaultSchema107 AS sysname;
SET @defaultSchema107 = SCHEMA_NAME();
DECLARE @description107 AS sql_variant;
SET @description107 = N'主键，使用 32 位无分隔符 Guid 字符串。';
EXEC sp_addextendedproperty 'MS_Description', @description107, 'SCHEMA', @defaultSchema107, 'TABLE', N'TextbookImportJob', 'COLUMN', N'Id';

DECLARE @defaultSchema108 AS sysname;
SET @defaultSchema108 = SCHEMA_NAME();
DECLARE @description108 AS sql_variant;
SET @description108 = N'教材 Id，关联 Textbook.Id。';
EXEC sp_addextendedproperty 'MS_Description', @description108, 'SCHEMA', @defaultSchema108, 'TABLE', N'TextbookImportFile', 'COLUMN', N'TextbookId';

DECLARE @defaultSchema109 AS sysname;
SET @defaultSchema109 = SCHEMA_NAME();
DECLARE @description109 AS sql_variant;
SET @description109 = N'文件页数。';
EXEC sp_addextendedproperty 'MS_Description', @description109, 'SCHEMA', @defaultSchema109, 'TABLE', N'TextbookImportFile', 'COLUMN', N'PageCount';

DECLARE @defaultSchema110 AS sysname;
SET @defaultSchema110 = SCHEMA_NAME();
DECLARE @description110 AS sql_variant;
SET @description110 = N'MIME 类型。';
EXEC sp_addextendedproperty 'MS_Description', @description110, 'SCHEMA', @defaultSchema110, 'TABLE', N'TextbookImportFile', 'COLUMN', N'MimeType';

DECLARE @defaultSchema111 AS sysname;
SET @defaultSchema111 = SCHEMA_NAME();
DECLARE @description111 AS sql_variant;
SET @description111 = N'教材导入任务 Id，关联 TextbookImportJob.Id。';
EXEC sp_addextendedproperty 'MS_Description', @description111, 'SCHEMA', @defaultSchema111, 'TABLE', N'TextbookImportFile', 'COLUMN', N'ImportJobId';

DECLARE @defaultSchema112 AS sysname;
SET @defaultSchema112 = SCHEMA_NAME();
DECLARE @description112 AS sql_variant;
SET @description112 = N'文件或内容哈希。';
EXEC sp_addextendedproperty 'MS_Description', @description112, 'SCHEMA', @defaultSchema112, 'TABLE', N'TextbookImportFile', 'COLUMN', N'Hash';

DECLARE @defaultSchema113 AS sysname;
SET @defaultSchema113 = SCHEMA_NAME();
DECLARE @description113 AS sql_variant;
SET @description113 = N'文件类型。';
EXEC sp_addextendedproperty 'MS_Description', @description113, 'SCHEMA', @defaultSchema113, 'TABLE', N'TextbookImportFile', 'COLUMN', N'FileType';

DECLARE @defaultSchema114 AS sysname;
SET @defaultSchema114 = SCHEMA_NAME();
DECLARE @description114 AS sql_variant;
SET @description114 = N'文件大小，单位字节。';
EXEC sp_addextendedproperty 'MS_Description', @description114, 'SCHEMA', @defaultSchema114, 'TABLE', N'TextbookImportFile', 'COLUMN', N'FileSize';

DECLARE @defaultSchema115 AS sysname;
SET @defaultSchema115 = SCHEMA_NAME();
DECLARE @description115 AS sql_variant;
SET @description115 = N'文件路径。';
EXEC sp_addextendedproperty 'MS_Description', @description115, 'SCHEMA', @defaultSchema115, 'TABLE', N'TextbookImportFile', 'COLUMN', N'FilePath';

DECLARE @defaultSchema116 AS sysname;
SET @defaultSchema116 = SCHEMA_NAME();
DECLARE @description116 AS sql_variant;
SET @description116 = N'文件名。';
EXEC sp_addextendedproperty 'MS_Description', @description116, 'SCHEMA', @defaultSchema116, 'TABLE', N'TextbookImportFile', 'COLUMN', N'FileName';

DECLARE @defaultSchema117 AS sysname;
SET @defaultSchema117 = SCHEMA_NAME();
DECLARE @description117 AS sql_variant;
SET @description117 = N'创建时间，使用 UTC 时间记录。';
EXEC sp_addextendedproperty 'MS_Description', @description117, 'SCHEMA', @defaultSchema117, 'TABLE', N'TextbookImportFile', 'COLUMN', N'CreatedTime';

DECLARE @defaultSchema118 AS sysname;
SET @defaultSchema118 = SCHEMA_NAME();
DECLARE @description118 AS sql_variant;
SET @description118 = N'主键，使用 32 位无分隔符 Guid 字符串。';
EXEC sp_addextendedproperty 'MS_Description', @description118, 'SCHEMA', @defaultSchema118, 'TABLE', N'TextbookImportFile', 'COLUMN', N'Id';

DECLARE @defaultSchema119 AS sysname;
SET @defaultSchema119 = SCHEMA_NAME();
DECLARE @description119 AS sql_variant;
SET @description119 = N'版本号或版本描述。';
EXEC sp_addextendedproperty 'MS_Description', @description119, 'SCHEMA', @defaultSchema119, 'TABLE', N'Textbook', 'COLUMN', N'Version';

DECLARE @defaultSchema120 AS sysname;
SET @defaultSchema120 = SCHEMA_NAME();
DECLARE @description120 AS sql_variant;
SET @description120 = N'最后更新时间，未更新时为空。';
EXEC sp_addextendedproperty 'MS_Description', @description120, 'SCHEMA', @defaultSchema120, 'TABLE', N'Textbook', 'COLUMN', N'UpdatedTime';

DECLARE @defaultSchema121 AS sysname;
SET @defaultSchema121 = SCHEMA_NAME();
DECLARE @description121 AS sql_variant;
SET @description121 = N'学科。';
EXEC sp_addextendedproperty 'MS_Description', @description121, 'SCHEMA', @defaultSchema121, 'TABLE', N'Textbook', 'COLUMN', N'Subject';

DECLARE @defaultSchema122 AS sysname;
SET @defaultSchema122 = SCHEMA_NAME();
DECLARE @description122 AS sql_variant;
SET @description122 = N'学期或册别，例如上册、下册。';
EXEC sp_addextendedproperty 'MS_Description', @description122, 'SCHEMA', @defaultSchema122, 'TABLE', N'Textbook', 'COLUMN', N'Semester';

DECLARE @defaultSchema123 AS sysname;
SET @defaultSchema123 = SCHEMA_NAME();
DECLARE @description123 AS sql_variant;
SET @description123 = N'出版社。';
EXEC sp_addextendedproperty 'MS_Description', @description123, 'SCHEMA', @defaultSchema123, 'TABLE', N'Textbook', 'COLUMN', N'Publisher';

DECLARE @defaultSchema124 AS sysname;
SET @defaultSchema124 = SCHEMA_NAME();
DECLARE @description124 AS sql_variant;
SET @description124 = N'名称。';
EXEC sp_addextendedproperty 'MS_Description', @description124, 'SCHEMA', @defaultSchema124, 'TABLE', N'Textbook', 'COLUMN', N'Name';

DECLARE @defaultSchema125 AS sysname;
SET @defaultSchema125 = SCHEMA_NAME();
DECLARE @description125 AS sql_variant;
SET @description125 = N'年级。';
EXEC sp_addextendedproperty 'MS_Description', @description125, 'SCHEMA', @defaultSchema125, 'TABLE', N'Textbook', 'COLUMN', N'Grade';

DECLARE @defaultSchema126 AS sysname;
SET @defaultSchema126 = SCHEMA_NAME();
DECLARE @description126 AS sql_variant;
SET @description126 = N'详细说明。';
EXEC sp_addextendedproperty 'MS_Description', @description126, 'SCHEMA', @defaultSchema126, 'TABLE', N'Textbook', 'COLUMN', N'Description';

DECLARE @defaultSchema127 AS sysname;
SET @defaultSchema127 = SCHEMA_NAME();
DECLARE @description127 AS sql_variant;
SET @description127 = N'创建时间，使用 UTC 时间记录。';
EXEC sp_addextendedproperty 'MS_Description', @description127, 'SCHEMA', @defaultSchema127, 'TABLE', N'Textbook', 'COLUMN', N'CreatedTime';

DECLARE @defaultSchema128 AS sysname;
SET @defaultSchema128 = SCHEMA_NAME();
DECLARE @description128 AS sql_variant;
SET @description128 = N'封面图片路径。';
EXEC sp_addextendedproperty 'MS_Description', @description128, 'SCHEMA', @defaultSchema128, 'TABLE', N'Textbook', 'COLUMN', N'CoverImagePath';

DECLARE @defaultSchema129 AS sysname;
SET @defaultSchema129 = SCHEMA_NAME();
DECLARE @description129 AS sql_variant;
SET @description129 = N'主键，使用 32 位无分隔符 Guid 字符串。';
EXEC sp_addextendedproperty 'MS_Description', @description129, 'SCHEMA', @defaultSchema129, 'TABLE', N'Textbook', 'COLUMN', N'Id';

DECLARE @defaultSchema130 AS sysname;
SET @defaultSchema130 = SCHEMA_NAME();
DECLARE @description130 AS sql_variant;
SET @description130 = N'最后更新时间，未更新时为空。';
EXEC sp_addextendedproperty 'MS_Description', @description130, 'SCHEMA', @defaultSchema130, 'TABLE', N'StudyStage', 'COLUMN', N'UpdatedTime';

DECLARE @defaultSchema131 AS sysname;
SET @defaultSchema131 = SCHEMA_NAME();
DECLARE @description131 AS sql_variant;
SET @description131 = N'标题。';
EXEC sp_addextendedproperty 'MS_Description', @description131, 'SCHEMA', @defaultSchema131, 'TABLE', N'StudyStage', 'COLUMN', N'Title';

DECLARE @defaultSchema132 AS sysname;
SET @defaultSchema132 = SCHEMA_NAME();
DECLARE @description132 AS sql_variant;
SET @description132 = N'学习计划 Id，关联 StudyPlan.Id。';
EXEC sp_addextendedproperty 'MS_Description', @description132, 'SCHEMA', @defaultSchema132, 'TABLE', N'StudyStage', 'COLUMN', N'StudyPlanId';

DECLARE @defaultSchema133 AS sysname;
SET @defaultSchema133 = SCHEMA_NAME();
DECLARE @description133 AS sql_variant;
SET @description133 = N'状态。';
EXEC sp_addextendedproperty 'MS_Description', @description133, 'SCHEMA', @defaultSchema133, 'TABLE', N'StudyStage', 'COLUMN', N'Status';

DECLARE @defaultSchema134 AS sysname;
SET @defaultSchema134 = SCHEMA_NAME();
DECLARE @description134 AS sql_variant;
SET @description134 = N'开始日期。';
EXEC sp_addextendedproperty 'MS_Description', @description134, 'SCHEMA', @defaultSchema134, 'TABLE', N'StudyStage', 'COLUMN', N'StartDate';

DECLARE @defaultSchema135 AS sysname;
SET @defaultSchema135 = SCHEMA_NAME();
DECLARE @description135 AS sql_variant;
SET @description135 = N'阶段序号。';
EXEC sp_addextendedproperty 'MS_Description', @description135, 'SCHEMA', @defaultSchema135, 'TABLE', N'StudyStage', 'COLUMN', N'StageNo';

DECLARE @defaultSchema136 AS sysname;
SET @defaultSchema136 = SCHEMA_NAME();
DECLARE @description136 AS sql_variant;
SET @description136 = N'阶段目标。';
EXEC sp_addextendedproperty 'MS_Description', @description136, 'SCHEMA', @defaultSchema136, 'TABLE', N'StudyStage', 'COLUMN', N'Goal';

DECLARE @defaultSchema137 AS sysname;
SET @defaultSchema137 = SCHEMA_NAME();
DECLARE @description137 AS sql_variant;
SET @description137 = N'结束日期。';
EXEC sp_addextendedproperty 'MS_Description', @description137, 'SCHEMA', @defaultSchema137, 'TABLE', N'StudyStage', 'COLUMN', N'EndDate';

DECLARE @defaultSchema138 AS sysname;
SET @defaultSchema138 = SCHEMA_NAME();
DECLARE @description138 AS sql_variant;
SET @description138 = N'创建时间，使用 UTC 时间记录。';
EXEC sp_addextendedproperty 'MS_Description', @description138, 'SCHEMA', @defaultSchema138, 'TABLE', N'StudyStage', 'COLUMN', N'CreatedTime';

DECLARE @defaultSchema139 AS sysname;
SET @defaultSchema139 = SCHEMA_NAME();
DECLARE @description139 AS sql_variant;
SET @description139 = N'主键，使用 32 位无分隔符 Guid 字符串。';
EXEC sp_addextendedproperty 'MS_Description', @description139, 'SCHEMA', @defaultSchema139, 'TABLE', N'StudyStage', 'COLUMN', N'Id';

DECLARE @defaultSchema140 AS sysname;
SET @defaultSchema140 = SCHEMA_NAME();
DECLARE @description140 AS sql_variant;
SET @description140 = N'用户 Id，关联 UserProfile.Id。';
EXEC sp_addextendedproperty 'MS_Description', @description140, 'SCHEMA', @defaultSchema140, 'TABLE', N'StudyPlan', 'COLUMN', N'UserId';

DECLARE @defaultSchema141 AS sysname;
SET @defaultSchema141 = SCHEMA_NAME();
DECLARE @description141 AS sql_variant;
SET @description141 = N'最后更新时间，未更新时为空。';
EXEC sp_addextendedproperty 'MS_Description', @description141, 'SCHEMA', @defaultSchema141, 'TABLE', N'StudyPlan', 'COLUMN', N'UpdatedTime';

DECLARE @defaultSchema142 AS sysname;
SET @defaultSchema142 = SCHEMA_NAME();
DECLARE @description142 AS sql_variant;
SET @description142 = N'标题。';
EXEC sp_addextendedproperty 'MS_Description', @description142, 'SCHEMA', @defaultSchema142, 'TABLE', N'StudyPlan', 'COLUMN', N'Title';

DECLARE @defaultSchema143 AS sysname;
SET @defaultSchema143 = SCHEMA_NAME();
DECLARE @description143 AS sql_variant;
SET @description143 = N'摘要。';
EXEC sp_addextendedproperty 'MS_Description', @description143, 'SCHEMA', @defaultSchema143, 'TABLE', N'StudyPlan', 'COLUMN', N'Summary';

DECLARE @defaultSchema144 AS sysname;
SET @defaultSchema144 = SCHEMA_NAME();
DECLARE @description144 AS sql_variant;
SET @description144 = N'学科。';
EXEC sp_addextendedproperty 'MS_Description', @description144, 'SCHEMA', @defaultSchema144, 'TABLE', N'StudyPlan', 'COLUMN', N'Subject';

DECLARE @defaultSchema145 AS sysname;
SET @defaultSchema145 = SCHEMA_NAME();
DECLARE @description145 AS sql_variant;
SET @description145 = N'状态。';
EXEC sp_addextendedproperty 'MS_Description', @description145, 'SCHEMA', @defaultSchema145, 'TABLE', N'StudyPlan', 'COLUMN', N'Status';

DECLARE @defaultSchema146 AS sysname;
SET @defaultSchema146 = SCHEMA_NAME();
DECLARE @description146 AS sql_variant;
SET @description146 = N'开始日期。';
EXEC sp_addextendedproperty 'MS_Description', @description146, 'SCHEMA', @defaultSchema146, 'TABLE', N'StudyPlan', 'COLUMN', N'StartDate';

DECLARE @defaultSchema147 AS sysname;
SET @defaultSchema147 = SCHEMA_NAME();
DECLARE @description147 AS sql_variant;
SET @description147 = N'学习计划类型。';
EXEC sp_addextendedproperty 'MS_Description', @description147, 'SCHEMA', @defaultSchema147, 'TABLE', N'StudyPlan', 'COLUMN', N'PlanType';

DECLARE @defaultSchema148 AS sysname;
SET @defaultSchema148 = SCHEMA_NAME();
DECLARE @description148 AS sql_variant;
SET @description148 = N'年级。';
EXEC sp_addextendedproperty 'MS_Description', @description148, 'SCHEMA', @defaultSchema148, 'TABLE', N'StudyPlan', 'COLUMN', N'Grade';

DECLARE @defaultSchema149 AS sysname;
SET @defaultSchema149 = SCHEMA_NAME();
DECLARE @description149 AS sql_variant;
SET @description149 = N'结束日期。';
EXEC sp_addextendedproperty 'MS_Description', @description149, 'SCHEMA', @defaultSchema149, 'TABLE', N'StudyPlan', 'COLUMN', N'EndDate';

DECLARE @defaultSchema150 AS sysname;
SET @defaultSchema150 = SCHEMA_NAME();
DECLARE @description150 AS sql_variant;
SET @description150 = N'创建时间，使用 UTC 时间记录。';
EXEC sp_addextendedproperty 'MS_Description', @description150, 'SCHEMA', @defaultSchema150, 'TABLE', N'StudyPlan', 'COLUMN', N'CreatedTime';

DECLARE @defaultSchema151 AS sysname;
SET @defaultSchema151 = SCHEMA_NAME();
DECLARE @description151 AS sql_variant;
SET @description151 = N'主键，使用 32 位无分隔符 Guid 字符串。';
EXEC sp_addextendedproperty 'MS_Description', @description151, 'SCHEMA', @defaultSchema151, 'TABLE', N'StudyPlan', 'COLUMN', N'Id';

DECLARE @defaultSchema152 AS sysname;
SET @defaultSchema152 = SCHEMA_NAME();
DECLARE @description152 AS sql_variant;
SET @description152 = N'用户 Id，关联 UserProfile.Id。';
EXEC sp_addextendedproperty 'MS_Description', @description152, 'SCHEMA', @defaultSchema152, 'TABLE', N'StageTaskRecord', 'COLUMN', N'UserId';

DECLARE @defaultSchema153 AS sysname;
SET @defaultSchema153 = SCHEMA_NAME();
DECLARE @description153 AS sql_variant;
SET @description153 = N'阶段任务 Id，关联 StageTask.Id。';
EXEC sp_addextendedproperty 'MS_Description', @description153, 'SCHEMA', @defaultSchema153, 'TABLE', N'StageTaskRecord', 'COLUMN', N'StageTaskId';

DECLARE @defaultSchema154 AS sysname;
SET @defaultSchema154 = SCHEMA_NAME();
DECLARE @description154 AS sql_variant;
SET @description154 = N'得分。';
EXEC sp_addextendedproperty 'MS_Description', @description154, 'SCHEMA', @defaultSchema154, 'TABLE', N'StageTaskRecord', 'COLUMN', N'Score';

DECLARE @defaultSchema155 AS sysname;
SET @defaultSchema155 = SCHEMA_NAME();
DECLARE @description155 AS sql_variant;
SET @description155 = N'执行结果。';
EXEC sp_addextendedproperty 'MS_Description', @description155, 'SCHEMA', @defaultSchema155, 'TABLE', N'StageTaskRecord', 'COLUMN', N'Result';

DECLARE @defaultSchema156 AS sysname;
SET @defaultSchema156 = SCHEMA_NAME();
DECLARE @description156 AS sql_variant;
SET @description156 = N'备注。';
EXEC sp_addextendedproperty 'MS_Description', @description156, 'SCHEMA', @defaultSchema156, 'TABLE', N'StageTaskRecord', 'COLUMN', N'Remark';

DECLARE @defaultSchema157 AS sysname;
SET @defaultSchema157 = SCHEMA_NAME();
DECLARE @description157 AS sql_variant;
SET @description157 = N'耗时，单位秒。';
EXEC sp_addextendedproperty 'MS_Description', @description157, 'SCHEMA', @defaultSchema157, 'TABLE', N'StageTaskRecord', 'COLUMN', N'DurationSeconds';

DECLARE @defaultSchema158 AS sysname;
SET @defaultSchema158 = SCHEMA_NAME();
DECLARE @description158 AS sql_variant;
SET @description158 = N'创建时间，使用 UTC 时间记录。';
EXEC sp_addextendedproperty 'MS_Description', @description158, 'SCHEMA', @defaultSchema158, 'TABLE', N'StageTaskRecord', 'COLUMN', N'CreatedTime';

DECLARE @defaultSchema159 AS sysname;
SET @defaultSchema159 = SCHEMA_NAME();
DECLARE @description159 AS sql_variant;
SET @description159 = N'动作类型。';
EXEC sp_addextendedproperty 'MS_Description', @description159, 'SCHEMA', @defaultSchema159, 'TABLE', N'StageTaskRecord', 'COLUMN', N'ActionType';

DECLARE @defaultSchema160 AS sysname;
SET @defaultSchema160 = SCHEMA_NAME();
DECLARE @description160 AS sql_variant;
SET @description160 = N'主键，使用 32 位无分隔符 Guid 字符串。';
EXEC sp_addextendedproperty 'MS_Description', @description160, 'SCHEMA', @defaultSchema160, 'TABLE', N'StageTaskRecord', 'COLUMN', N'Id';

DECLARE @defaultSchema161 AS sysname;
SET @defaultSchema161 = SCHEMA_NAME();
DECLARE @description161 AS sql_variant;
SET @description161 = N'用户 Id，关联 UserProfile.Id。';
EXEC sp_addextendedproperty 'MS_Description', @description161, 'SCHEMA', @defaultSchema161, 'TABLE', N'StageTask', 'COLUMN', N'UserId';

DECLARE @defaultSchema162 AS sysname;
SET @defaultSchema162 = SCHEMA_NAME();
DECLARE @description162 AS sql_variant;
SET @description162 = N'最后更新时间，未更新时为空。';
EXEC sp_addextendedproperty 'MS_Description', @description162, 'SCHEMA', @defaultSchema162, 'TABLE', N'StageTask', 'COLUMN', N'UpdatedTime';

DECLARE @defaultSchema163 AS sysname;
SET @defaultSchema163 = SCHEMA_NAME();
DECLARE @description163 AS sql_variant;
SET @description163 = N'标题。';
EXEC sp_addextendedproperty 'MS_Description', @description163, 'SCHEMA', @defaultSchema163, 'TABLE', N'StageTask', 'COLUMN', N'Title';

DECLARE @defaultSchema164 AS sysname;
SET @defaultSchema164 = SCHEMA_NAME();
DECLARE @description164 AS sql_variant;
SET @description164 = N'阶段任务类型。';
EXEC sp_addextendedproperty 'MS_Description', @description164, 'SCHEMA', @defaultSchema164, 'TABLE', N'StageTask', 'COLUMN', N'TaskType';

DECLARE @defaultSchema165 AS sysname;
SET @defaultSchema165 = SCHEMA_NAME();
DECLARE @description165 AS sql_variant;
SET @description165 = N'学习阶段 Id，关联 StudyStage.Id。';
EXEC sp_addextendedproperty 'MS_Description', @description165, 'SCHEMA', @defaultSchema165, 'TABLE', N'StageTask', 'COLUMN', N'StudyStageId';

DECLARE @defaultSchema166 AS sysname;
SET @defaultSchema166 = SCHEMA_NAME();
DECLARE @description166 AS sql_variant;
SET @description166 = N'状态。';
EXEC sp_addextendedproperty 'MS_Description', @description166, 'SCHEMA', @defaultSchema166, 'TABLE', N'StageTask', 'COLUMN', N'Status';

DECLARE @defaultSchema167 AS sysname;
SET @defaultSchema167 = SCHEMA_NAME();
DECLARE @description167 AS sql_variant;
SET @description167 = N'排序值，数值越小越靠前。';
EXEC sp_addextendedproperty 'MS_Description', @description167, 'SCHEMA', @defaultSchema167, 'TABLE', N'StageTask', 'COLUMN', N'SortIndex';

DECLARE @defaultSchema168 AS sysname;
SET @defaultSchema168 = SCHEMA_NAME();
DECLARE @description168 AS sql_variant;
SET @description168 = N'关联错题 Id。';
EXEC sp_addextendedproperty 'MS_Description', @description168, 'SCHEMA', @defaultSchema168, 'TABLE', N'StageTask', 'COLUMN', N'RelatedWrongQuestionId';

DECLARE @defaultSchema169 AS sysname;
SET @defaultSchema169 = SCHEMA_NAME();
DECLARE @description169 AS sql_variant;
SET @description169 = N'关联练习题 Id。';
EXEC sp_addextendedproperty 'MS_Description', @description169, 'SCHEMA', @defaultSchema169, 'TABLE', N'StageTask', 'COLUMN', N'RelatedPracticeQuestionId';

DECLARE @defaultSchema170 AS sysname;
SET @defaultSchema170 = SCHEMA_NAME();
DECLARE @description170 AS sql_variant;
SET @description170 = N'关联知识点 Id。';
EXEC sp_addextendedproperty 'MS_Description', @description170, 'SCHEMA', @defaultSchema170, 'TABLE', N'StageTask', 'COLUMN', N'RelatedKnowledgePointId';

DECLARE @defaultSchema171 AS sysname;
SET @defaultSchema171 = SCHEMA_NAME();
DECLARE @description171 AS sql_variant;
SET @description171 = N'详细说明。';
EXEC sp_addextendedproperty 'MS_Description', @description171, 'SCHEMA', @defaultSchema171, 'TABLE', N'StageTask', 'COLUMN', N'Description';

DECLARE @defaultSchema172 AS sysname;
SET @defaultSchema172 = SCHEMA_NAME();
DECLARE @description172 AS sql_variant;
SET @description172 = N'创建时间，使用 UTC 时间记录。';
EXEC sp_addextendedproperty 'MS_Description', @description172, 'SCHEMA', @defaultSchema172, 'TABLE', N'StageTask', 'COLUMN', N'CreatedTime';

DECLARE @defaultSchema173 AS sysname;
SET @defaultSchema173 = SCHEMA_NAME();
DECLARE @description173 AS sql_variant;
SET @description173 = N'主键，使用 32 位无分隔符 Guid 字符串。';
EXEC sp_addextendedproperty 'MS_Description', @description173, 'SCHEMA', @defaultSchema173, 'TABLE', N'StageTask', 'COLUMN', N'Id';

DECLARE @defaultSchema174 AS sysname;
SET @defaultSchema174 = SCHEMA_NAME();
DECLARE @description174 AS sql_variant;
SET @description174 = N'视频文件路径。';
EXEC sp_addextendedproperty 'MS_Description', @description174, 'SCHEMA', @defaultSchema174, 'TABLE', N'SessionMessage', 'COLUMN', N'VideoPath';

DECLARE @defaultSchema175 AS sysname;
SET @defaultSchema175 = SCHEMA_NAME();
DECLARE @description175 AS sql_variant;
SET @description175 = N'用户 Id，关联 UserProfile.Id。';
EXEC sp_addextendedproperty 'MS_Description', @description175, 'SCHEMA', @defaultSchema175, 'TABLE', N'SessionMessage', 'COLUMN', N'UserId';

DECLARE @defaultSchema176 AS sysname;
SET @defaultSchema176 = SCHEMA_NAME();
DECLARE @description176 AS sql_variant;
SET @description176 = N'文本内容。';
EXEC sp_addextendedproperty 'MS_Description', @description176, 'SCHEMA', @defaultSchema176, 'TABLE', N'SessionMessage', 'COLUMN', N'TextContent';

DECLARE @defaultSchema177 AS sysname;
SET @defaultSchema177 = SCHEMA_NAME();
DECLARE @description177 AS sql_variant;
SET @description177 = N'学习会话 Id，关联 LearningSession.Id。';
EXEC sp_addextendedproperty 'MS_Description', @description177, 'SCHEMA', @defaultSchema177, 'TABLE', N'SessionMessage', 'COLUMN', N'SessionId';

DECLARE @defaultSchema178 AS sysname;
SET @defaultSchema178 = SCHEMA_NAME();
DECLARE @description178 AS sql_variant;
SET @description178 = N'消息角色。';
EXEC sp_addextendedproperty 'MS_Description', @description178, 'SCHEMA', @defaultSchema178, 'TABLE', N'SessionMessage', 'COLUMN', N'Role';

DECLARE @defaultSchema179 AS sysname;
SET @defaultSchema179 = SCHEMA_NAME();
DECLARE @description179 AS sql_variant;
SET @description179 = N'消息结构化 JSON。';
EXEC sp_addextendedproperty 'MS_Description', @description179, 'SCHEMA', @defaultSchema179, 'TABLE', N'SessionMessage', 'COLUMN', N'MessageJson';

DECLARE @defaultSchema180 AS sysname;
SET @defaultSchema180 = SCHEMA_NAME();
DECLARE @description180 AS sql_variant;
SET @description180 = N'图片文件路径。';
EXEC sp_addextendedproperty 'MS_Description', @description180, 'SCHEMA', @defaultSchema180, 'TABLE', N'SessionMessage', 'COLUMN', N'ImagePath';

DECLARE @defaultSchema181 AS sysname;
SET @defaultSchema181 = SCHEMA_NAME();
DECLARE @description181 AS sql_variant;
SET @description181 = N'创建时间，使用 UTC 时间记录。';
EXEC sp_addextendedproperty 'MS_Description', @description181, 'SCHEMA', @defaultSchema181, 'TABLE', N'SessionMessage', 'COLUMN', N'CreatedTime';

DECLARE @defaultSchema182 AS sysname;
SET @defaultSchema182 = SCHEMA_NAME();
DECLARE @description182 AS sql_variant;
SET @description182 = N'消息内容类型。';
EXEC sp_addextendedproperty 'MS_Description', @description182, 'SCHEMA', @defaultSchema182, 'TABLE', N'SessionMessage', 'COLUMN', N'ContentType';

DECLARE @defaultSchema183 AS sysname;
SET @defaultSchema183 = SCHEMA_NAME();
DECLARE @description183 AS sql_variant;
SET @description183 = N'音频文件路径。';
EXEC sp_addextendedproperty 'MS_Description', @description183, 'SCHEMA', @defaultSchema183, 'TABLE', N'SessionMessage', 'COLUMN', N'AudioPath';

DECLARE @defaultSchema184 AS sysname;
SET @defaultSchema184 = SCHEMA_NAME();
DECLARE @description184 AS sql_variant;
SET @description184 = N'主键，使用 32 位无分隔符 Guid 字符串。';
EXEC sp_addextendedproperty 'MS_Description', @description184, 'SCHEMA', @defaultSchema184, 'TABLE', N'SessionMessage', 'COLUMN', N'Id';

DECLARE @defaultSchema185 AS sysname;
SET @defaultSchema185 = SCHEMA_NAME();
DECLARE @description185 AS sql_variant;
SET @description185 = N'用户 Id，关联 UserProfile.Id。';
EXEC sp_addextendedproperty 'MS_Description', @description185, 'SCHEMA', @defaultSchema185, 'TABLE', N'ReviewSchedule', 'COLUMN', N'UserId';

DECLARE @defaultSchema186 AS sysname;
SET @defaultSchema186 = SCHEMA_NAME();
DECLARE @description186 AS sql_variant;
SET @description186 = N'最后更新时间，未更新时为空。';
EXEC sp_addextendedproperty 'MS_Description', @description186, 'SCHEMA', @defaultSchema186, 'TABLE', N'ReviewSchedule', 'COLUMN', N'UpdatedTime';

DECLARE @defaultSchema187 AS sysname;
SET @defaultSchema187 = SCHEMA_NAME();
DECLARE @description187 AS sql_variant;
SET @description187 = N'学科。';
EXEC sp_addextendedproperty 'MS_Description', @description187, 'SCHEMA', @defaultSchema187, 'TABLE', N'ReviewSchedule', 'COLUMN', N'Subject';

DECLARE @defaultSchema188 AS sysname;
SET @defaultSchema188 = SCHEMA_NAME();
DECLARE @description188 AS sql_variant;
SET @description188 = N'状态。';
EXEC sp_addextendedproperty 'MS_Description', @description188, 'SCHEMA', @defaultSchema188, 'TABLE', N'ReviewSchedule', 'COLUMN', N'Status';

DECLARE @defaultSchema189 AS sysname;
SET @defaultSchema189 = SCHEMA_NAME();
DECLARE @description189 AS sql_variant;
SET @description189 = N'复习目标类型。';
EXEC sp_addextendedproperty 'MS_Description', @description189, 'SCHEMA', @defaultSchema189, 'TABLE', N'ReviewSchedule', 'COLUMN', N'ReviewTargetType';

DECLARE @defaultSchema190 AS sysname;
SET @defaultSchema190 = SCHEMA_NAME();
DECLARE @description190 AS sql_variant;
SET @description190 = N'复习目标 Id。';
EXEC sp_addextendedproperty 'MS_Description', @description190, 'SCHEMA', @defaultSchema190, 'TABLE', N'ReviewSchedule', 'COLUMN', N'ReviewTargetId';

DECLARE @defaultSchema191 AS sysname;
SET @defaultSchema191 = SCHEMA_NAME();
DECLARE @description191 AS sql_variant;
SET @description191 = N'复习等级。';
EXEC sp_addextendedproperty 'MS_Description', @description191, 'SCHEMA', @defaultSchema191, 'TABLE', N'ReviewSchedule', 'COLUMN', N'ReviewLevel';

DECLARE @defaultSchema192 AS sysname;
SET @defaultSchema192 = SCHEMA_NAME();
DECLARE @description192 AS sql_variant;
SET @description192 = N'复习次数。';
EXEC sp_addextendedproperty 'MS_Description', @description192, 'SCHEMA', @defaultSchema192, 'TABLE', N'ReviewSchedule', 'COLUMN', N'ReviewCount';

DECLARE @defaultSchema193 AS sysname;
SET @defaultSchema193 = SCHEMA_NAME();
DECLARE @description193 AS sql_variant;
SET @description193 = N'下次复习时间。';
EXEC sp_addextendedproperty 'MS_Description', @description193, 'SCHEMA', @defaultSchema193, 'TABLE', N'ReviewSchedule', 'COLUMN', N'NextReviewTime';

DECLARE @defaultSchema194 AS sysname;
SET @defaultSchema194 = SCHEMA_NAME();
DECLARE @description194 AS sql_variant;
SET @description194 = N'上次复习时间。';
EXEC sp_addextendedproperty 'MS_Description', @description194, 'SCHEMA', @defaultSchema194, 'TABLE', N'ReviewSchedule', 'COLUMN', N'LastReviewTime';

DECLARE @defaultSchema195 AS sysname;
SET @defaultSchema195 = SCHEMA_NAME();
DECLARE @description195 AS sql_variant;
SET @description195 = N'知识点 Id，关联 KnowledgePoint.Id。';
EXEC sp_addextendedproperty 'MS_Description', @description195, 'SCHEMA', @defaultSchema195, 'TABLE', N'ReviewSchedule', 'COLUMN', N'KnowledgePointId';

DECLARE @defaultSchema196 AS sysname;
SET @defaultSchema196 = SCHEMA_NAME();
DECLARE @description196 AS sql_variant;
SET @description196 = N'年级。';
EXEC sp_addextendedproperty 'MS_Description', @description196, 'SCHEMA', @defaultSchema196, 'TABLE', N'ReviewSchedule', 'COLUMN', N'Grade';

DECLARE @defaultSchema197 AS sysname;
SET @defaultSchema197 = SCHEMA_NAME();
DECLARE @description197 AS sql_variant;
SET @description197 = N'创建时间，使用 UTC 时间记录。';
EXEC sp_addextendedproperty 'MS_Description', @description197, 'SCHEMA', @defaultSchema197, 'TABLE', N'ReviewSchedule', 'COLUMN', N'CreatedTime';

DECLARE @defaultSchema198 AS sysname;
SET @defaultSchema198 = SCHEMA_NAME();
DECLARE @description198 AS sql_variant;
SET @description198 = N'主键，使用 32 位无分隔符 Guid 字符串。';
EXEC sp_addextendedproperty 'MS_Description', @description198, 'SCHEMA', @defaultSchema198, 'TABLE', N'ReviewSchedule', 'COLUMN', N'Id';

DECLARE @defaultSchema199 AS sysname;
SET @defaultSchema199 = SCHEMA_NAME();
DECLARE @description199 AS sql_variant;
SET @description199 = N'用户 Id，关联 UserProfile.Id。';
EXEC sp_addextendedproperty 'MS_Description', @description199, 'SCHEMA', @defaultSchema199, 'TABLE', N'QuestionRecord', 'COLUMN', N'UserId';

DECLARE @defaultSchema200 AS sysname;
SET @defaultSchema200 = SCHEMA_NAME();
DECLARE @description200 AS sql_variant;
SET @description200 = N'最后更新时间，未更新时为空。';
EXEC sp_addextendedproperty 'MS_Description', @description200, 'SCHEMA', @defaultSchema200, 'TABLE', N'QuestionRecord', 'COLUMN', N'UpdatedTime';

DECLARE @defaultSchema201 AS sysname;
SET @defaultSchema201 = SCHEMA_NAME();
DECLARE @description201 AS sql_variant;
SET @description201 = N'学科。';
EXEC sp_addextendedproperty 'MS_Description', @description201, 'SCHEMA', @defaultSchema201, 'TABLE', N'QuestionRecord', 'COLUMN', N'Subject';

DECLARE @defaultSchema202 AS sysname;
SET @defaultSchema202 = SCHEMA_NAME();
DECLARE @description202 AS sql_variant;
SET @description202 = N'来源类型。';
EXEC sp_addextendedproperty 'MS_Description', @description202, 'SCHEMA', @defaultSchema202, 'TABLE', N'QuestionRecord', 'COLUMN', N'SourceType';

DECLARE @defaultSchema203 AS sysname;
SET @defaultSchema203 = SCHEMA_NAME();
DECLARE @description203 AS sql_variant;
SET @description203 = N'学习会话 Id，关联 LearningSession.Id。';
EXEC sp_addextendedproperty 'MS_Description', @description203, 'SCHEMA', @defaultSchema203, 'TABLE', N'QuestionRecord', 'COLUMN', N'SessionId';

DECLARE @defaultSchema204 AS sysname;
SET @defaultSchema204 = SCHEMA_NAME();
DECLARE @description204 AS sql_variant;
SET @description204 = N'从图片或音频识别出的文本。';
EXEC sp_addextendedproperty 'MS_Description', @description204, 'SCHEMA', @defaultSchema204, 'TABLE', N'QuestionRecord', 'COLUMN', N'RecognizedText';

DECLARE @defaultSchema205 AS sysname;
SET @defaultSchema205 = SCHEMA_NAME();
DECLARE @description205 AS sql_variant;
SET @description205 = N'题目或问题文本。';
EXEC sp_addextendedproperty 'MS_Description', @description205, 'SCHEMA', @defaultSchema205, 'TABLE', N'QuestionRecord', 'COLUMN', N'QuestionText';

DECLARE @defaultSchema206 AS sysname;
SET @defaultSchema206 = SCHEMA_NAME();
DECLARE @description206 AS sql_variant;
SET @description206 = N'提问模式。';
EXEC sp_addextendedproperty 'MS_Description', @description206, 'SCHEMA', @defaultSchema206, 'TABLE', N'QuestionRecord', 'COLUMN', N'Mode';

DECLARE @defaultSchema207 AS sysname;
SET @defaultSchema207 = SCHEMA_NAME();
DECLARE @description207 AS sql_variant;
SET @description207 = N'Agent 输入类型。';
EXEC sp_addextendedproperty 'MS_Description', @description207, 'SCHEMA', @defaultSchema207, 'TABLE', N'QuestionRecord', 'COLUMN', N'InputType';

DECLARE @defaultSchema208 AS sysname;
SET @defaultSchema208 = SCHEMA_NAME();
DECLARE @description208 AS sql_variant;
SET @description208 = N'图片文件路径。';
EXEC sp_addextendedproperty 'MS_Description', @description208, 'SCHEMA', @defaultSchema208, 'TABLE', N'QuestionRecord', 'COLUMN', N'ImagePath';

DECLARE @defaultSchema209 AS sysname;
SET @defaultSchema209 = SCHEMA_NAME();
DECLARE @description209 AS sql_variant;
SET @description209 = N'年级。';
EXEC sp_addextendedproperty 'MS_Description', @description209, 'SCHEMA', @defaultSchema209, 'TABLE', N'QuestionRecord', 'COLUMN', N'Grade';

DECLARE @defaultSchema210 AS sysname;
SET @defaultSchema210 = SCHEMA_NAME();
DECLARE @description210 AS sql_variant;
SET @description210 = N'创建时间，使用 UTC 时间记录。';
EXEC sp_addextendedproperty 'MS_Description', @description210, 'SCHEMA', @defaultSchema210, 'TABLE', N'QuestionRecord', 'COLUMN', N'CreatedTime';

DECLARE @defaultSchema211 AS sysname;
SET @defaultSchema211 = SCHEMA_NAME();
DECLARE @description211 AS sql_variant;
SET @description211 = N'音频文件路径。';
EXEC sp_addextendedproperty 'MS_Description', @description211, 'SCHEMA', @defaultSchema211, 'TABLE', N'QuestionRecord', 'COLUMN', N'AudioPath';

DECLARE @defaultSchema212 AS sysname;
SET @defaultSchema212 = SCHEMA_NAME();
DECLARE @description212 AS sql_variant;
SET @description212 = N'主键，使用 32 位无分隔符 Guid 字符串。';
EXEC sp_addextendedproperty 'MS_Description', @description212, 'SCHEMA', @defaultSchema212, 'TABLE', N'QuestionRecord', 'COLUMN', N'Id';

DECLARE @defaultSchema213 AS sysname;
SET @defaultSchema213 = SCHEMA_NAME();
DECLARE @description213 AS sql_variant;
SET @description213 = N'来源类型。';
EXEC sp_addextendedproperty 'MS_Description', @description213, 'SCHEMA', @defaultSchema213, 'TABLE', N'PracticeQuestionSource', 'COLUMN', N'SourceType';

DECLARE @defaultSchema214 AS sysname;
SET @defaultSchema214 = SCHEMA_NAME();
DECLARE @description214 AS sql_variant;
SET @description214 = N'练习题来源表，记录练习题与错题、知识点、教材切片等来源的关系。字段 SourceId。';
EXEC sp_addextendedproperty 'MS_Description', @description214, 'SCHEMA', @defaultSchema214, 'TABLE', N'PracticeQuestionSource', 'COLUMN', N'SourceId';

DECLARE @defaultSchema215 AS sysname;
SET @defaultSchema215 = SCHEMA_NAME();
DECLARE @description215 AS sql_variant;
SET @description215 = N'练习题 Id，关联 PracticeQuestion.Id。';
EXEC sp_addextendedproperty 'MS_Description', @description215, 'SCHEMA', @defaultSchema215, 'TABLE', N'PracticeQuestionSource', 'COLUMN', N'PracticeQuestionId';

DECLARE @defaultSchema216 AS sysname;
SET @defaultSchema216 = SCHEMA_NAME();
DECLARE @description216 AS sql_variant;
SET @description216 = N'创建时间，使用 UTC 时间记录。';
EXEC sp_addextendedproperty 'MS_Description', @description216, 'SCHEMA', @defaultSchema216, 'TABLE', N'PracticeQuestionSource', 'COLUMN', N'CreatedTime';

DECLARE @defaultSchema217 AS sysname;
SET @defaultSchema217 = SCHEMA_NAME();
DECLARE @description217 AS sql_variant;
SET @description217 = N'主键，使用 32 位无分隔符 Guid 字符串。';
EXEC sp_addextendedproperty 'MS_Description', @description217, 'SCHEMA', @defaultSchema217, 'TABLE', N'PracticeQuestionSource', 'COLUMN', N'Id';

DECLARE @defaultSchema218 AS sysname;
SET @defaultSchema218 = SCHEMA_NAME();
DECLARE @description218 AS sql_variant;
SET @description218 = N'错题 Id，关联 WrongQuestion.Id。';
EXEC sp_addextendedproperty 'MS_Description', @description218, 'SCHEMA', @defaultSchema218, 'TABLE', N'PracticeQuestion', 'COLUMN', N'WrongQuestionId';

DECLARE @defaultSchema219 AS sysname;
SET @defaultSchema219 = SCHEMA_NAME();
DECLARE @description219 AS sql_variant;
SET @description219 = N'用户 Id，关联 UserProfile.Id。';
EXEC sp_addextendedproperty 'MS_Description', @description219, 'SCHEMA', @defaultSchema219, 'TABLE', N'PracticeQuestion', 'COLUMN', N'UserId';

DECLARE @defaultSchema220 AS sysname;
SET @defaultSchema220 = SCHEMA_NAME();
DECLARE @description220 AS sql_variant;
SET @description220 = N'最后更新时间，未更新时为空。';
EXEC sp_addextendedproperty 'MS_Description', @description220, 'SCHEMA', @defaultSchema220, 'TABLE', N'PracticeQuestion', 'COLUMN', N'UpdatedTime';

DECLARE @defaultSchema221 AS sysname;
SET @defaultSchema221 = SCHEMA_NAME();
DECLARE @description221 AS sql_variant;
SET @description221 = N'学科。';
EXEC sp_addextendedproperty 'MS_Description', @description221, 'SCHEMA', @defaultSchema221, 'TABLE', N'PracticeQuestion', 'COLUMN', N'Subject';

DECLARE @defaultSchema222 AS sysname;
SET @defaultSchema222 = SCHEMA_NAME();
DECLARE @description222 AS sql_variant;
SET @description222 = N'来源类型。';
EXEC sp_addextendedproperty 'MS_Description', @description222, 'SCHEMA', @defaultSchema222, 'TABLE', N'PracticeQuestion', 'COLUMN', N'SourceType';

DECLARE @defaultSchema223 AS sysname;
SET @defaultSchema223 = SCHEMA_NAME();
DECLARE @description223 AS sql_variant;
SET @description223 = N'题目或问题文本。';
EXEC sp_addextendedproperty 'MS_Description', @description223, 'SCHEMA', @defaultSchema223, 'TABLE', N'PracticeQuestion', 'COLUMN', N'QuestionText';

DECLARE @defaultSchema224 AS sysname;
SET @defaultSchema224 = SCHEMA_NAME();
DECLARE @description224 AS sql_variant;
SET @description224 = N'题目图片路径。';
EXEC sp_addextendedproperty 'MS_Description', @description224, 'SCHEMA', @defaultSchema224, 'TABLE', N'PracticeQuestion', 'COLUMN', N'QuestionImagePath';

DECLARE @defaultSchema225 AS sysname;
SET @defaultSchema225 = SCHEMA_NAME();
DECLARE @description225 AS sql_variant;
SET @description225 = N'Prompt 版本。';
EXEC sp_addextendedproperty 'MS_Description', @description225, 'SCHEMA', @defaultSchema225, 'TABLE', N'PracticeQuestion', 'COLUMN', N'PromptVersion';

DECLARE @defaultSchema226 AS sysname;
SET @defaultSchema226 = SCHEMA_NAME();
DECLARE @description226 AS sql_variant;
SET @description226 = N'模型名称。';
EXEC sp_addextendedproperty 'MS_Description', @description226, 'SCHEMA', @defaultSchema226, 'TABLE', N'PracticeQuestion', 'COLUMN', N'ModelName';

DECLARE @defaultSchema227 AS sysname;
SET @defaultSchema227 = SCHEMA_NAME();
DECLARE @description227 AS sql_variant;
SET @description227 = N'知识点 Id，关联 KnowledgePoint.Id。';
EXEC sp_addextendedproperty 'MS_Description', @description227, 'SCHEMA', @defaultSchema227, 'TABLE', N'PracticeQuestion', 'COLUMN', N'KnowledgePointId';

DECLARE @defaultSchema228 AS sysname;
SET @defaultSchema228 = SCHEMA_NAME();
DECLARE @description228 AS sql_variant;
SET @description228 = N'年级。';
EXEC sp_addextendedproperty 'MS_Description', @description228, 'SCHEMA', @defaultSchema228, 'TABLE', N'PracticeQuestion', 'COLUMN', N'Grade';

DECLARE @defaultSchema229 AS sysname;
SET @defaultSchema229 = SCHEMA_NAME();
DECLARE @description229 AS sql_variant;
SET @description229 = N'练习题生成类型。';
EXEC sp_addextendedproperty 'MS_Description', @description229, 'SCHEMA', @defaultSchema229, 'TABLE', N'PracticeQuestion', 'COLUMN', N'GenerateType';

DECLARE @defaultSchema230 AS sysname;
SET @defaultSchema230 = SCHEMA_NAME();
DECLARE @description230 AS sql_variant;
SET @description230 = N'讲解内容。';
EXEC sp_addextendedproperty 'MS_Description', @description230, 'SCHEMA', @defaultSchema230, 'TABLE', N'PracticeQuestion', 'COLUMN', N'Explanation';

DECLARE @defaultSchema231 AS sysname;
SET @defaultSchema231 = SCHEMA_NAME();
DECLARE @description231 AS sql_variant;
SET @description231 = N'难度等级，默认 1。';
EXEC sp_addextendedproperty 'MS_Description', @description231, 'SCHEMA', @defaultSchema231, 'TABLE', N'PracticeQuestion', 'COLUMN', N'DifficultyLevel';

DECLARE @defaultSchema232 AS sysname;
SET @defaultSchema232 = SCHEMA_NAME();
DECLARE @description232 AS sql_variant;
SET @description232 = N'创建时间，使用 UTC 时间记录。';
EXEC sp_addextendedproperty 'MS_Description', @description232, 'SCHEMA', @defaultSchema232, 'TABLE', N'PracticeQuestion', 'COLUMN', N'CreatedTime';

DECLARE @defaultSchema233 AS sysname;
SET @defaultSchema233 = SCHEMA_NAME();
DECLARE @description233 AS sql_variant;
SET @description233 = N'正确答案。';
EXEC sp_addextendedproperty 'MS_Description', @description233, 'SCHEMA', @defaultSchema233, 'TABLE', N'PracticeQuestion', 'COLUMN', N'CorrectAnswer';

DECLARE @defaultSchema234 AS sysname;
SET @defaultSchema234 = SCHEMA_NAME();
DECLARE @description234 AS sql_variant;
SET @description234 = N'主键，使用 32 位无分隔符 Guid 字符串。';
EXEC sp_addextendedproperty 'MS_Description', @description234, 'SCHEMA', @defaultSchema234, 'TABLE', N'PracticeQuestion', 'COLUMN', N'Id';

DECLARE @defaultSchema235 AS sysname;
SET @defaultSchema235 = SCHEMA_NAME();
DECLARE @description235 AS sql_variant;
SET @description235 = N'用户 Id，关联 UserProfile.Id。';
EXEC sp_addextendedproperty 'MS_Description', @description235, 'SCHEMA', @defaultSchema235, 'TABLE', N'PracticeAnswerRecord', 'COLUMN', N'UserId';

DECLARE @defaultSchema236 AS sysname;
SET @defaultSchema236 = SCHEMA_NAME();
DECLARE @description236 AS sql_variant;
SET @description236 = N'用户作答内容。';
EXEC sp_addextendedproperty 'MS_Description', @description236, 'SCHEMA', @defaultSchema236, 'TABLE', N'PracticeAnswerRecord', 'COLUMN', N'UserAnswer';

DECLARE @defaultSchema237 AS sysname;
SET @defaultSchema237 = SCHEMA_NAME();
DECLARE @description237 AS sql_variant;
SET @description237 = N'练习题 Id，关联 PracticeQuestion.Id。';
EXEC sp_addextendedproperty 'MS_Description', @description237, 'SCHEMA', @defaultSchema237, 'TABLE', N'PracticeAnswerRecord', 'COLUMN', N'PracticeQuestionId';

DECLARE @defaultSchema238 AS sysname;
SET @defaultSchema238 = SCHEMA_NAME();
DECLARE @description238 AS sql_variant;
SET @description238 = N'是否正确。';
EXEC sp_addextendedproperty 'MS_Description', @description238, 'SCHEMA', @defaultSchema238, 'TABLE', N'PracticeAnswerRecord', 'COLUMN', N'IsCorrect';

DECLARE @defaultSchema239 AS sysname;
SET @defaultSchema239 = SCHEMA_NAME();
DECLARE @description239 AS sql_variant;
SET @description239 = N'耗时，单位秒。';
EXEC sp_addextendedproperty 'MS_Description', @description239, 'SCHEMA', @defaultSchema239, 'TABLE', N'PracticeAnswerRecord', 'COLUMN', N'DurationSeconds';

DECLARE @defaultSchema240 AS sysname;
SET @defaultSchema240 = SCHEMA_NAME();
DECLARE @description240 AS sql_variant;
SET @description240 = N'创建时间，使用 UTC 时间记录。';
EXEC sp_addextendedproperty 'MS_Description', @description240, 'SCHEMA', @defaultSchema240, 'TABLE', N'PracticeAnswerRecord', 'COLUMN', N'CreatedTime';

DECLARE @defaultSchema241 AS sysname;
SET @defaultSchema241 = SCHEMA_NAME();
DECLARE @description241 AS sql_variant;
SET @description241 = N'AI 反馈。';
EXEC sp_addextendedproperty 'MS_Description', @description241, 'SCHEMA', @defaultSchema241, 'TABLE', N'PracticeAnswerRecord', 'COLUMN', N'AiFeedback';

DECLARE @defaultSchema242 AS sysname;
SET @defaultSchema242 = SCHEMA_NAME();
DECLARE @description242 AS sql_variant;
SET @description242 = N'主键，使用 32 位无分隔符 Guid 字符串。';
EXEC sp_addextendedproperty 'MS_Description', @description242, 'SCHEMA', @defaultSchema242, 'TABLE', N'PracticeAnswerRecord', 'COLUMN', N'Id';

DECLARE @defaultSchema243 AS sysname;
SET @defaultSchema243 = SCHEMA_NAME();
DECLARE @description243 AS sql_variant;
SET @description243 = N'用户 Id，关联 UserProfile.Id。';
EXEC sp_addextendedproperty 'MS_Description', @description243, 'SCHEMA', @defaultSchema243, 'TABLE', N'ModelCallLog', 'COLUMN', N'UserId';

DECLARE @defaultSchema244 AS sysname;
SET @defaultSchema244 = SCHEMA_NAME();
DECLARE @description244 AS sql_variant;
SET @description244 = N'学习会话 Id，关联 LearningSession.Id。';
EXEC sp_addextendedproperty 'MS_Description', @description244, 'SCHEMA', @defaultSchema244, 'TABLE', N'ModelCallLog', 'COLUMN', N'SessionId';

DECLARE @defaultSchema245 AS sysname;
SET @defaultSchema245 = SCHEMA_NAME();
DECLARE @description245 AS sql_variant;
SET @description245 = N'模型返回文本。';
EXEC sp_addextendedproperty 'MS_Description', @description245, 'SCHEMA', @defaultSchema245, 'TABLE', N'ModelCallLog', 'COLUMN', N'ResponseText';

DECLARE @defaultSchema246 AS sysname;
SET @defaultSchema246 = SCHEMA_NAME();
DECLARE @description246 AS sql_variant;
SET @description246 = N'请求类型。';
EXEC sp_addextendedproperty 'MS_Description', @description246, 'SCHEMA', @defaultSchema246, 'TABLE', N'ModelCallLog', 'COLUMN', N'RequestType';

DECLARE @defaultSchema247 AS sysname;
SET @defaultSchema247 = SCHEMA_NAME();
DECLARE @description247 AS sql_variant;
SET @description247 = N'提问记录 Id，关联 QuestionRecord.Id。';
EXEC sp_addextendedproperty 'MS_Description', @description247, 'SCHEMA', @defaultSchema247, 'TABLE', N'ModelCallLog', 'COLUMN', N'QuestionRecordId';

DECLARE @defaultSchema248 AS sysname;
SET @defaultSchema248 = SCHEMA_NAME();
DECLARE @description248 AS sql_variant;
SET @description248 = N'发送给模型的 Prompt 文本。';
EXEC sp_addextendedproperty 'MS_Description', @description248, 'SCHEMA', @defaultSchema248, 'TABLE', N'ModelCallLog', 'COLUMN', N'PromptText';

DECLARE @defaultSchema249 AS sysname;
SET @defaultSchema249 = SCHEMA_NAME();
DECLARE @description249 AS sql_variant;
SET @description249 = N'输出 Token 数。';
EXEC sp_addextendedproperty 'MS_Description', @description249, 'SCHEMA', @defaultSchema249, 'TABLE', N'ModelCallLog', 'COLUMN', N'OutputTokens';

DECLARE @defaultSchema250 AS sysname;
SET @defaultSchema250 = SCHEMA_NAME();
DECLARE @description250 AS sql_variant;
SET @description250 = N'模型提供方。';
EXEC sp_addextendedproperty 'MS_Description', @description250, 'SCHEMA', @defaultSchema250, 'TABLE', N'ModelCallLog', 'COLUMN', N'ModelProvider';

DECLARE @defaultSchema251 AS sysname;
SET @defaultSchema251 = SCHEMA_NAME();
DECLARE @description251 AS sql_variant;
SET @description251 = N'模型名称。';
EXEC sp_addextendedproperty 'MS_Description', @description251, 'SCHEMA', @defaultSchema251, 'TABLE', N'ModelCallLog', 'COLUMN', N'ModelName';

DECLARE @defaultSchema252 AS sysname;
SET @defaultSchema252 = SCHEMA_NAME();
DECLARE @description252 AS sql_variant;
SET @description252 = N'模型调用是否成功。';
EXEC sp_addextendedproperty 'MS_Description', @description252, 'SCHEMA', @defaultSchema252, 'TABLE', N'ModelCallLog', 'COLUMN', N'IsSuccess';

DECLARE @defaultSchema253 AS sysname;
SET @defaultSchema253 = SCHEMA_NAME();
DECLARE @description253 AS sql_variant;
SET @description253 = N'输入 Token 数。';
EXEC sp_addextendedproperty 'MS_Description', @description253, 'SCHEMA', @defaultSchema253, 'TABLE', N'ModelCallLog', 'COLUMN', N'InputTokens';

DECLARE @defaultSchema254 AS sysname;
SET @defaultSchema254 = SCHEMA_NAME();
DECLARE @description254 AS sql_variant;
SET @description254 = N'错误消息。';
EXEC sp_addextendedproperty 'MS_Description', @description254, 'SCHEMA', @defaultSchema254, 'TABLE', N'ModelCallLog', 'COLUMN', N'ErrorMessage';

DECLARE @defaultSchema255 AS sysname;
SET @defaultSchema255 = SCHEMA_NAME();
DECLARE @description255 AS sql_variant;
SET @description255 = N'媒体时长，单位毫秒。';
EXEC sp_addextendedproperty 'MS_Description', @description255, 'SCHEMA', @defaultSchema255, 'TABLE', N'ModelCallLog', 'COLUMN', N'DurationMs';

DECLARE @defaultSchema256 AS sysname;
SET @defaultSchema256 = SCHEMA_NAME();
DECLARE @description256 AS sql_variant;
SET @description256 = N'创建时间，使用 UTC 时间记录。';
EXEC sp_addextendedproperty 'MS_Description', @description256, 'SCHEMA', @defaultSchema256, 'TABLE', N'ModelCallLog', 'COLUMN', N'CreatedTime';

DECLARE @defaultSchema257 AS sysname;
SET @defaultSchema257 = SCHEMA_NAME();
DECLARE @description257 AS sql_variant;
SET @description257 = N'模型调用成本。';
EXEC sp_addextendedproperty 'MS_Description', @description257, 'SCHEMA', @defaultSchema257, 'TABLE', N'ModelCallLog', 'COLUMN', N'Cost';

DECLARE @defaultSchema258 AS sysname;
SET @defaultSchema258 = SCHEMA_NAME();
DECLARE @description258 AS sql_variant;
SET @description258 = N'Agent 名称。';
EXEC sp_addextendedproperty 'MS_Description', @description258, 'SCHEMA', @defaultSchema258, 'TABLE', N'ModelCallLog', 'COLUMN', N'AgentName';

DECLARE @defaultSchema259 AS sysname;
SET @defaultSchema259 = SCHEMA_NAME();
DECLARE @description259 AS sql_variant;
SET @description259 = N'主键，使用 32 位无分隔符 Guid 字符串。';
EXEC sp_addextendedproperty 'MS_Description', @description259, 'SCHEMA', @defaultSchema259, 'TABLE', N'ModelCallLog', 'COLUMN', N'Id';

DECLARE @defaultSchema260 AS sysname;
SET @defaultSchema260 = SCHEMA_NAME();
DECLARE @description260 AS sql_variant;
SET @description260 = N'图片或视频宽度。';
EXEC sp_addextendedproperty 'MS_Description', @description260, 'SCHEMA', @defaultSchema260, 'TABLE', N'MediaResource', 'COLUMN', N'Width';

DECLARE @defaultSchema261 AS sysname;
SET @defaultSchema261 = SCHEMA_NAME();
DECLARE @description261 AS sql_variant;
SET @description261 = N'用户 Id，关联 UserProfile.Id。';
EXEC sp_addextendedproperty 'MS_Description', @description261, 'SCHEMA', @defaultSchema261, 'TABLE', N'MediaResource', 'COLUMN', N'UserId';

DECLARE @defaultSchema262 AS sysname;
SET @defaultSchema262 = SCHEMA_NAME();
DECLARE @description262 AS sql_variant;
SET @description262 = N'最后更新时间，未更新时为空。';
EXEC sp_addextendedproperty 'MS_Description', @description262, 'SCHEMA', @defaultSchema262, 'TABLE', N'MediaResource', 'COLUMN', N'UpdatedTime';

DECLARE @defaultSchema263 AS sysname;
SET @defaultSchema263 = SCHEMA_NAME();
DECLARE @description263 AS sql_variant;
SET @description263 = N'来源类型。';
EXEC sp_addextendedproperty 'MS_Description', @description263, 'SCHEMA', @defaultSchema263, 'TABLE', N'MediaResource', 'COLUMN', N'SourceType';

DECLARE @defaultSchema264 AS sysname;
SET @defaultSchema264 = SCHEMA_NAME();
DECLARE @description264 AS sql_variant;
SET @description264 = N'媒体资源类型。';
EXEC sp_addextendedproperty 'MS_Description', @description264, 'SCHEMA', @defaultSchema264, 'TABLE', N'MediaResource', 'COLUMN', N'ResourceType';

DECLARE @defaultSchema265 AS sysname;
SET @defaultSchema265 = SCHEMA_NAME();
DECLARE @description265 AS sql_variant;
SET @description265 = N'MIME 类型。';
EXEC sp_addextendedproperty 'MS_Description', @description265, 'SCHEMA', @defaultSchema265, 'TABLE', N'MediaResource', 'COLUMN', N'MimeType';

DECLARE @defaultSchema266 AS sysname;
SET @defaultSchema266 = SCHEMA_NAME();
DECLARE @description266 AS sql_variant;
SET @description266 = N'图片或视频高度。';
EXEC sp_addextendedproperty 'MS_Description', @description266, 'SCHEMA', @defaultSchema266, 'TABLE', N'MediaResource', 'COLUMN', N'Height';

DECLARE @defaultSchema267 AS sysname;
SET @defaultSchema267 = SCHEMA_NAME();
DECLARE @description267 AS sql_variant;
SET @description267 = N'文件或内容哈希。';
EXEC sp_addextendedproperty 'MS_Description', @description267, 'SCHEMA', @defaultSchema267, 'TABLE', N'MediaResource', 'COLUMN', N'Hash';

DECLARE @defaultSchema268 AS sysname;
SET @defaultSchema268 = SCHEMA_NAME();
DECLARE @description268 AS sql_variant;
SET @description268 = N'文件大小，单位字节。';
EXEC sp_addextendedproperty 'MS_Description', @description268, 'SCHEMA', @defaultSchema268, 'TABLE', N'MediaResource', 'COLUMN', N'FileSize';

DECLARE @defaultSchema269 AS sysname;
SET @defaultSchema269 = SCHEMA_NAME();
DECLARE @description269 AS sql_variant;
SET @description269 = N'文件路径。';
EXEC sp_addextendedproperty 'MS_Description', @description269, 'SCHEMA', @defaultSchema269, 'TABLE', N'MediaResource', 'COLUMN', N'FilePath';

DECLARE @defaultSchema270 AS sysname;
SET @defaultSchema270 = SCHEMA_NAME();
DECLARE @description270 AS sql_variant;
SET @description270 = N'文件名。';
EXEC sp_addextendedproperty 'MS_Description', @description270, 'SCHEMA', @defaultSchema270, 'TABLE', N'MediaResource', 'COLUMN', N'FileName';

DECLARE @defaultSchema271 AS sysname;
SET @defaultSchema271 = SCHEMA_NAME();
DECLARE @description271 AS sql_variant;
SET @description271 = N'媒体时长，单位毫秒。';
EXEC sp_addextendedproperty 'MS_Description', @description271, 'SCHEMA', @defaultSchema271, 'TABLE', N'MediaResource', 'COLUMN', N'DurationMs';

DECLARE @defaultSchema272 AS sysname;
SET @defaultSchema272 = SCHEMA_NAME();
DECLARE @description272 AS sql_variant;
SET @description272 = N'创建时间，使用 UTC 时间记录。';
EXEC sp_addextendedproperty 'MS_Description', @description272, 'SCHEMA', @defaultSchema272, 'TABLE', N'MediaResource', 'COLUMN', N'CreatedTime';

DECLARE @defaultSchema273 AS sysname;
SET @defaultSchema273 = SCHEMA_NAME();
DECLARE @description273 AS sql_variant;
SET @description273 = N'主键，使用 32 位无分隔符 Guid 字符串。';
EXEC sp_addextendedproperty 'MS_Description', @description273, 'SCHEMA', @defaultSchema273, 'TABLE', N'MediaResource', 'COLUMN', N'Id';

DECLARE @defaultSchema274 AS sysname;
SET @defaultSchema274 = SCHEMA_NAME();
DECLARE @description274 AS sql_variant;
SET @description274 = N'最后更新时间，未更新时为空。';
EXEC sp_addextendedproperty 'MS_Description', @description274, 'SCHEMA', @defaultSchema274, 'TABLE', N'Lesson', 'COLUMN', N'UpdatedTime';

DECLARE @defaultSchema275 AS sysname;
SET @defaultSchema275 = SCHEMA_NAME();
DECLARE @description275 AS sql_variant;
SET @description275 = N'教材单元 Id，关联 TextbookUnit.Id。';
EXEC sp_addextendedproperty 'MS_Description', @description275, 'SCHEMA', @defaultSchema275, 'TABLE', N'Lesson', 'COLUMN', N'UnitId';

DECLARE @defaultSchema276 AS sysname;
SET @defaultSchema276 = SCHEMA_NAME();
DECLARE @description276 AS sql_variant;
SET @description276 = N'标题。';
EXEC sp_addextendedproperty 'MS_Description', @description276, 'SCHEMA', @defaultSchema276, 'TABLE', N'Lesson', 'COLUMN', N'Title';

DECLARE @defaultSchema277 AS sysname;
SET @defaultSchema277 = SCHEMA_NAME();
DECLARE @description277 AS sql_variant;
SET @description277 = N'教材 Id，关联 Textbook.Id。';
EXEC sp_addextendedproperty 'MS_Description', @description277, 'SCHEMA', @defaultSchema277, 'TABLE', N'Lesson', 'COLUMN', N'TextbookId';

DECLARE @defaultSchema278 AS sysname;
SET @defaultSchema278 = SCHEMA_NAME();
DECLARE @description278 AS sql_variant;
SET @description278 = N'排序值，数值越小越靠前。';
EXEC sp_addextendedproperty 'MS_Description', @description278, 'SCHEMA', @defaultSchema278, 'TABLE', N'Lesson', 'COLUMN', N'SortIndex';

DECLARE @defaultSchema279 AS sysname;
SET @defaultSchema279 = SCHEMA_NAME();
DECLARE @description279 AS sql_variant;
SET @description279 = N'起始页码。';
EXEC sp_addextendedproperty 'MS_Description', @description279, 'SCHEMA', @defaultSchema279, 'TABLE', N'Lesson', 'COLUMN', N'PageStart';

DECLARE @defaultSchema280 AS sysname;
SET @defaultSchema280 = SCHEMA_NAME();
DECLARE @description280 AS sql_variant;
SET @description280 = N'结束页码。';
EXEC sp_addextendedproperty 'MS_Description', @description280, 'SCHEMA', @defaultSchema280, 'TABLE', N'Lesson', 'COLUMN', N'PageEnd';

DECLARE @defaultSchema281 AS sysname;
SET @defaultSchema281 = SCHEMA_NAME();
DECLARE @description281 AS sql_variant;
SET @description281 = N'课时或章节序号。';
EXEC sp_addextendedproperty 'MS_Description', @description281, 'SCHEMA', @defaultSchema281, 'TABLE', N'Lesson', 'COLUMN', N'LessonNo';

DECLARE @defaultSchema282 AS sysname;
SET @defaultSchema282 = SCHEMA_NAME();
DECLARE @description282 AS sql_variant;
SET @description282 = N'创建时间，使用 UTC 时间记录。';
EXEC sp_addextendedproperty 'MS_Description', @description282, 'SCHEMA', @defaultSchema282, 'TABLE', N'Lesson', 'COLUMN', N'CreatedTime';

DECLARE @defaultSchema283 AS sysname;
SET @defaultSchema283 = SCHEMA_NAME();
DECLARE @description283 AS sql_variant;
SET @description283 = N'主键，使用 32 位无分隔符 Guid 字符串。';
EXEC sp_addextendedproperty 'MS_Description', @description283, 'SCHEMA', @defaultSchema283, 'TABLE', N'Lesson', 'COLUMN', N'Id';

DECLARE @defaultSchema284 AS sysname;
SET @defaultSchema284 = SCHEMA_NAME();
DECLARE @description284 AS sql_variant;
SET @description284 = N'用户 Id，关联 UserProfile.Id。';
EXEC sp_addextendedproperty 'MS_Description', @description284, 'SCHEMA', @defaultSchema284, 'TABLE', N'LearningSession', 'COLUMN', N'UserId';

DECLARE @defaultSchema285 AS sysname;
SET @defaultSchema285 = SCHEMA_NAME();
DECLARE @description285 AS sql_variant;
SET @description285 = N'最后更新时间，未更新时为空。';
EXEC sp_addextendedproperty 'MS_Description', @description285, 'SCHEMA', @defaultSchema285, 'TABLE', N'LearningSession', 'COLUMN', N'UpdatedTime';

DECLARE @defaultSchema286 AS sysname;
SET @defaultSchema286 = SCHEMA_NAME();
DECLARE @description286 AS sql_variant;
SET @description286 = N'标题。';
EXEC sp_addextendedproperty 'MS_Description', @description286, 'SCHEMA', @defaultSchema286, 'TABLE', N'LearningSession', 'COLUMN', N'Title';

DECLARE @defaultSchema287 AS sysname;
SET @defaultSchema287 = SCHEMA_NAME();
DECLARE @description287 AS sql_variant;
SET @description287 = N'摘要。';
EXEC sp_addextendedproperty 'MS_Description', @description287, 'SCHEMA', @defaultSchema287, 'TABLE', N'LearningSession', 'COLUMN', N'Summary';

DECLARE @defaultSchema288 AS sysname;
SET @defaultSchema288 = SCHEMA_NAME();
DECLARE @description288 AS sql_variant;
SET @description288 = N'学科。';
EXEC sp_addextendedproperty 'MS_Description', @description288, 'SCHEMA', @defaultSchema288, 'TABLE', N'LearningSession', 'COLUMN', N'Subject';

DECLARE @defaultSchema289 AS sysname;
SET @defaultSchema289 = SCHEMA_NAME();
DECLARE @description289 AS sql_variant;
SET @description289 = N'会话开始时间。';
EXEC sp_addextendedproperty 'MS_Description', @description289, 'SCHEMA', @defaultSchema289, 'TABLE', N'LearningSession', 'COLUMN', N'StartTime';

DECLARE @defaultSchema290 AS sysname;
SET @defaultSchema290 = SCHEMA_NAME();
DECLARE @description290 AS sql_variant;
SET @description290 = N'学习会话类型。';
EXEC sp_addextendedproperty 'MS_Description', @description290, 'SCHEMA', @defaultSchema290, 'TABLE', N'LearningSession', 'COLUMN', N'SessionType';

DECLARE @defaultSchema291 AS sysname;
SET @defaultSchema291 = SCHEMA_NAME();
DECLARE @description291 AS sql_variant;
SET @description291 = N'关联错题 Id。';
EXEC sp_addextendedproperty 'MS_Description', @description291, 'SCHEMA', @defaultSchema291, 'TABLE', N'LearningSession', 'COLUMN', N'RelatedWrongId';

DECLARE @defaultSchema292 AS sysname;
SET @defaultSchema292 = SCHEMA_NAME();
DECLARE @description292 AS sql_variant;
SET @description292 = N'关联问题 Id。';
EXEC sp_addextendedproperty 'MS_Description', @description292, 'SCHEMA', @defaultSchema292, 'TABLE', N'LearningSession', 'COLUMN', N'RelatedQuestionId';

DECLARE @defaultSchema293 AS sysname;
SET @defaultSchema293 = SCHEMA_NAME();
DECLARE @description293 AS sql_variant;
SET @description293 = N'年级。';
EXEC sp_addextendedproperty 'MS_Description', @description293, 'SCHEMA', @defaultSchema293, 'TABLE', N'LearningSession', 'COLUMN', N'Grade';

DECLARE @defaultSchema294 AS sysname;
SET @defaultSchema294 = SCHEMA_NAME();
DECLARE @description294 AS sql_variant;
SET @description294 = N'会话结束时间。';
EXEC sp_addextendedproperty 'MS_Description', @description294, 'SCHEMA', @defaultSchema294, 'TABLE', N'LearningSession', 'COLUMN', N'EndTime';

DECLARE @defaultSchema295 AS sysname;
SET @defaultSchema295 = SCHEMA_NAME();
DECLARE @description295 AS sql_variant;
SET @description295 = N'创建时间，使用 UTC 时间记录。';
EXEC sp_addextendedproperty 'MS_Description', @description295, 'SCHEMA', @defaultSchema295, 'TABLE', N'LearningSession', 'COLUMN', N'CreatedTime';

DECLARE @defaultSchema296 AS sysname;
SET @defaultSchema296 = SCHEMA_NAME();
DECLARE @description296 AS sql_variant;
SET @description296 = N'主键，使用 32 位无分隔符 Guid 字符串。';
EXEC sp_addextendedproperty 'MS_Description', @description296, 'SCHEMA', @defaultSchema296, 'TABLE', N'LearningSession', 'COLUMN', N'Id';

DECLARE @defaultSchema297 AS sysname;
SET @defaultSchema297 = SCHEMA_NAME();
DECLARE @description297 AS sql_variant;
SET @description297 = N'用户 Id，关联 UserProfile.Id。';
EXEC sp_addextendedproperty 'MS_Description', @description297, 'SCHEMA', @defaultSchema297, 'TABLE', N'LearningProfile', 'COLUMN', N'UserId';

DECLARE @defaultSchema298 AS sysname;
SET @defaultSchema298 = SCHEMA_NAME();
DECLARE @description298 AS sql_variant;
SET @description298 = N'最后更新时间，未更新时为空。';
EXEC sp_addextendedproperty 'MS_Description', @description298, 'SCHEMA', @defaultSchema298, 'TABLE', N'LearningProfile', 'COLUMN', N'UpdatedTime';

DECLARE @defaultSchema299 AS sysname;
SET @defaultSchema299 = SCHEMA_NAME();
DECLARE @description299 AS sql_variant;
SET @description299 = N'默认教材版本。';
EXEC sp_addextendedproperty 'MS_Description', @description299, 'SCHEMA', @defaultSchema299, 'TABLE', N'LearningProfile', 'COLUMN', N'TextbookVersion';

DECLARE @defaultSchema300 AS sysname;
SET @defaultSchema300 = SCHEMA_NAME();
DECLARE @description300 AS sql_variant;
SET @description300 = N'学期或册别，例如上册、下册。';
EXEC sp_addextendedproperty 'MS_Description', @description300, 'SCHEMA', @defaultSchema300, 'TABLE', N'LearningProfile', 'COLUMN', N'Semester';

DECLARE @defaultSchema301 AS sysname;
SET @defaultSchema301 = SCHEMA_NAME();
DECLARE @description301 AS sql_variant;
SET @description301 = N'学校名称。';
EXEC sp_addextendedproperty 'MS_Description', @description301, 'SCHEMA', @defaultSchema301, 'TABLE', N'LearningProfile', 'COLUMN', N'SchoolName';

DECLARE @defaultSchema302 AS sysname;
SET @defaultSchema302 = SCHEMA_NAME();
DECLARE @description302 AS sql_variant;
SET @description302 = N'年级。';
EXEC sp_addextendedproperty 'MS_Description', @description302, 'SCHEMA', @defaultSchema302, 'TABLE', N'LearningProfile', 'COLUMN', N'Grade';

DECLARE @defaultSchema303 AS sysname;
SET @defaultSchema303 = SCHEMA_NAME();
DECLARE @description303 AS sql_variant;
SET @description303 = N'默认学习学科。';
EXEC sp_addextendedproperty 'MS_Description', @description303, 'SCHEMA', @defaultSchema303, 'TABLE', N'LearningProfile', 'COLUMN', N'DefaultSubject';

DECLARE @defaultSchema304 AS sysname;
SET @defaultSchema304 = SCHEMA_NAME();
DECLARE @description304 AS sql_variant;
SET @description304 = N'创建时间，使用 UTC 时间记录。';
EXEC sp_addextendedproperty 'MS_Description', @description304, 'SCHEMA', @defaultSchema304, 'TABLE', N'LearningProfile', 'COLUMN', N'CreatedTime';

DECLARE @defaultSchema305 AS sysname;
SET @defaultSchema305 = SCHEMA_NAME();
DECLARE @description305 AS sql_variant;
SET @description305 = N'主键，使用 32 位无分隔符 Guid 字符串。';
EXEC sp_addextendedproperty 'MS_Description', @description305, 'SCHEMA', @defaultSchema305, 'TABLE', N'LearningProfile', 'COLUMN', N'Id';

DECLARE @defaultSchema306 AS sysname;
SET @defaultSchema306 = SCHEMA_NAME();
DECLARE @description306 AS sql_variant;
SET @description306 = N'最后更新时间，未更新时为空。';
EXEC sp_addextendedproperty 'MS_Description', @description306, 'SCHEMA', @defaultSchema306, 'TABLE', N'KnowledgePoint', 'COLUMN', N'UpdatedTime';

DECLARE @defaultSchema307 AS sysname;
SET @defaultSchema307 = SCHEMA_NAME();
DECLARE @description307 AS sql_variant;
SET @description307 = N'教材单元 Id，关联 TextbookUnit.Id。';
EXEC sp_addextendedproperty 'MS_Description', @description307, 'SCHEMA', @defaultSchema307, 'TABLE', N'KnowledgePoint', 'COLUMN', N'UnitId';

DECLARE @defaultSchema308 AS sysname;
SET @defaultSchema308 = SCHEMA_NAME();
DECLARE @description308 AS sql_variant;
SET @description308 = N'教材 Id，关联 Textbook.Id。';
EXEC sp_addextendedproperty 'MS_Description', @description308, 'SCHEMA', @defaultSchema308, 'TABLE', N'KnowledgePoint', 'COLUMN', N'TextbookId';

DECLARE @defaultSchema309 AS sysname;
SET @defaultSchema309 = SCHEMA_NAME();
DECLARE @description309 AS sql_variant;
SET @description309 = N'学科。';
EXEC sp_addextendedproperty 'MS_Description', @description309, 'SCHEMA', @defaultSchema309, 'TABLE', N'KnowledgePoint', 'COLUMN', N'Subject';

DECLARE @defaultSchema310 AS sysname;
SET @defaultSchema310 = SCHEMA_NAME();
DECLARE @description310 AS sql_variant;
SET @description310 = N'排序值，数值越小越靠前。';
EXEC sp_addextendedproperty 'MS_Description', @description310, 'SCHEMA', @defaultSchema310, 'TABLE', N'KnowledgePoint', 'COLUMN', N'SortIndex';

DECLARE @defaultSchema311 AS sysname;
SET @defaultSchema311 = SCHEMA_NAME();
DECLARE @description311 AS sql_variant;
SET @description311 = N'知识点类型。';
EXEC sp_addextendedproperty 'MS_Description', @description311, 'SCHEMA', @defaultSchema311, 'TABLE', N'KnowledgePoint', 'COLUMN', N'PointType';

DECLARE @defaultSchema312 AS sysname;
SET @defaultSchema312 = SCHEMA_NAME();
DECLARE @description312 AS sql_variant;
SET @description312 = N'父级 Id，用于树形层级。';
EXEC sp_addextendedproperty 'MS_Description', @description312, 'SCHEMA', @defaultSchema312, 'TABLE', N'KnowledgePoint', 'COLUMN', N'ParentId';

DECLARE @defaultSchema313 AS sysname;
SET @defaultSchema313 = SCHEMA_NAME();
DECLARE @description313 AS sql_variant;
SET @description313 = N'教材页 Id，关联 TextbookPage.Id。';
EXEC sp_addextendedproperty 'MS_Description', @description313, 'SCHEMA', @defaultSchema313, 'TABLE', N'KnowledgePoint', 'COLUMN', N'PageId';

DECLARE @defaultSchema314 AS sysname;
SET @defaultSchema314 = SCHEMA_NAME();
DECLARE @description314 AS sql_variant;
SET @description314 = N'名称。';
EXEC sp_addextendedproperty 'MS_Description', @description314, 'SCHEMA', @defaultSchema314, 'TABLE', N'KnowledgePoint', 'COLUMN', N'Name';

DECLARE @defaultSchema315 AS sysname;
SET @defaultSchema315 = SCHEMA_NAME();
DECLARE @description315 AS sql_variant;
SET @description315 = N'课时或章节 Id，关联 Lesson.Id。';
EXEC sp_addextendedproperty 'MS_Description', @description315, 'SCHEMA', @defaultSchema315, 'TABLE', N'KnowledgePoint', 'COLUMN', N'LessonId';

DECLARE @defaultSchema316 AS sysname;
SET @defaultSchema316 = SCHEMA_NAME();
DECLARE @description316 AS sql_variant;
SET @description316 = N'年级。';
EXEC sp_addextendedproperty 'MS_Description', @description316, 'SCHEMA', @defaultSchema316, 'TABLE', N'KnowledgePoint', 'COLUMN', N'Grade';

DECLARE @defaultSchema317 AS sysname;
SET @defaultSchema317 = SCHEMA_NAME();
DECLARE @description317 AS sql_variant;
SET @description317 = N'难度等级，默认 1。';
EXEC sp_addextendedproperty 'MS_Description', @description317, 'SCHEMA', @defaultSchema317, 'TABLE', N'KnowledgePoint', 'COLUMN', N'DifficultyLevel';

DECLARE @defaultSchema318 AS sysname;
SET @defaultSchema318 = SCHEMA_NAME();
DECLARE @description318 AS sql_variant;
SET @description318 = N'详细说明。';
EXEC sp_addextendedproperty 'MS_Description', @description318, 'SCHEMA', @defaultSchema318, 'TABLE', N'KnowledgePoint', 'COLUMN', N'Description';

DECLARE @defaultSchema319 AS sysname;
SET @defaultSchema319 = SCHEMA_NAME();
DECLARE @description319 AS sql_variant;
SET @description319 = N'创建时间，使用 UTC 时间记录。';
EXEC sp_addextendedproperty 'MS_Description', @description319, 'SCHEMA', @defaultSchema319, 'TABLE', N'KnowledgePoint', 'COLUMN', N'CreatedTime';

DECLARE @defaultSchema320 AS sysname;
SET @defaultSchema320 = SCHEMA_NAME();
DECLARE @description320 AS sql_variant;
SET @description320 = N'主键，使用 32 位无分隔符 Guid 字符串。';
EXEC sp_addextendedproperty 'MS_Description', @description320, 'SCHEMA', @defaultSchema320, 'TABLE', N'KnowledgePoint', 'COLUMN', N'Id';

DECLARE @defaultSchema321 AS sysname;
SET @defaultSchema321 = SCHEMA_NAME();
DECLARE @description321 AS sql_variant;
SET @description321 = N'向量数据哈希。';
EXEC sp_addextendedproperty 'MS_Description', @description321, 'SCHEMA', @defaultSchema321, 'TABLE', N'KnowledgeEmbedding', 'COLUMN', N'VectorHash';

DECLARE @defaultSchema322 AS sysname;
SET @defaultSchema322 = SCHEMA_NAME();
DECLARE @description322 AS sql_variant;
SET @description322 = N'向量数据预留字段，第一阶段以文本形式保存。';
EXEC sp_addextendedproperty 'MS_Description', @description322, 'SCHEMA', @defaultSchema322, 'TABLE', N'KnowledgeEmbedding', 'COLUMN', N'VectorData';

DECLARE @defaultSchema323 AS sysname;
SET @defaultSchema323 = SCHEMA_NAME();
DECLARE @description323 AS sql_variant;
SET @description323 = N'生成向量的模型名称。';
EXEC sp_addextendedproperty 'MS_Description', @description323, 'SCHEMA', @defaultSchema323, 'TABLE', N'KnowledgeEmbedding', 'COLUMN', N'EmbeddingModel';

DECLARE @defaultSchema324 AS sysname;
SET @defaultSchema324 = SCHEMA_NAME();
DECLARE @description324 AS sql_variant;
SET @description324 = N'创建时间，使用 UTC 时间记录。';
EXEC sp_addextendedproperty 'MS_Description', @description324, 'SCHEMA', @defaultSchema324, 'TABLE', N'KnowledgeEmbedding', 'COLUMN', N'CreatedTime';

DECLARE @defaultSchema325 AS sysname;
SET @defaultSchema325 = SCHEMA_NAME();
DECLARE @description325 AS sql_variant;
SET @description325 = N'知识片段 Id，关联 KnowledgeChunk.Id。';
EXEC sp_addextendedproperty 'MS_Description', @description325, 'SCHEMA', @defaultSchema325, 'TABLE', N'KnowledgeEmbedding', 'COLUMN', N'ChunkId';

DECLARE @defaultSchema326 AS sysname;
SET @defaultSchema326 = SCHEMA_NAME();
DECLARE @description326 AS sql_variant;
SET @description326 = N'主键，使用 32 位无分隔符 Guid 字符串。';
EXEC sp_addextendedproperty 'MS_Description', @description326, 'SCHEMA', @defaultSchema326, 'TABLE', N'KnowledgeEmbedding', 'COLUMN', N'Id';

DECLARE @defaultSchema327 AS sysname;
SET @defaultSchema327 = SCHEMA_NAME();
DECLARE @description327 AS sql_variant;
SET @description327 = N'最后更新时间，未更新时为空。';
EXEC sp_addextendedproperty 'MS_Description', @description327, 'SCHEMA', @defaultSchema327, 'TABLE', N'KnowledgeChunk', 'COLUMN', N'UpdatedTime';

DECLARE @defaultSchema328 AS sysname;
SET @defaultSchema328 = SCHEMA_NAME();
DECLARE @description328 AS sql_variant;
SET @description328 = N'教材单元 Id，关联 TextbookUnit.Id。';
EXEC sp_addextendedproperty 'MS_Description', @description328, 'SCHEMA', @defaultSchema328, 'TABLE', N'KnowledgeChunk', 'COLUMN', N'UnitId';

DECLARE @defaultSchema329 AS sysname;
SET @defaultSchema329 = SCHEMA_NAME();
DECLARE @description329 AS sql_variant;
SET @description329 = N'教材 Id，关联 Textbook.Id。';
EXEC sp_addextendedproperty 'MS_Description', @description329, 'SCHEMA', @defaultSchema329, 'TABLE', N'KnowledgeChunk', 'COLUMN', N'TextbookId';

DECLARE @defaultSchema330 AS sysname;
SET @defaultSchema330 = SCHEMA_NAME();
DECLARE @description330 AS sql_variant;
SET @description330 = N'来源类型。';
EXEC sp_addextendedproperty 'MS_Description', @description330, 'SCHEMA', @defaultSchema330, 'TABLE', N'KnowledgeChunk', 'COLUMN', N'SourceType';

DECLARE @defaultSchema331 AS sysname;
SET @defaultSchema331 = SCHEMA_NAME();
DECLARE @description331 AS sql_variant;
SET @description331 = N'来源文件或资源路径。';
EXEC sp_addextendedproperty 'MS_Description', @description331, 'SCHEMA', @defaultSchema331, 'TABLE', N'KnowledgeChunk', 'COLUMN', N'SourcePath';

DECLARE @defaultSchema332 AS sysname;
SET @defaultSchema332 = SCHEMA_NAME();
DECLARE @description332 AS sql_variant;
SET @description332 = N'排序值，数值越小越靠前。';
EXEC sp_addextendedproperty 'MS_Description', @description332, 'SCHEMA', @defaultSchema332, 'TABLE', N'KnowledgeChunk', 'COLUMN', N'SortIndex';

DECLARE @defaultSchema333 AS sysname;
SET @defaultSchema333 = SCHEMA_NAME();
DECLARE @description333 AS sql_variant;
SET @description333 = N'页码。';
EXEC sp_addextendedproperty 'MS_Description', @description333, 'SCHEMA', @defaultSchema333, 'TABLE', N'KnowledgeChunk', 'COLUMN', N'PageNo';

DECLARE @defaultSchema334 AS sysname;
SET @defaultSchema334 = SCHEMA_NAME();
DECLARE @description334 AS sql_variant;
SET @description334 = N'教材页 Id，关联 TextbookPage.Id。';
EXEC sp_addextendedproperty 'MS_Description', @description334, 'SCHEMA', @defaultSchema334, 'TABLE', N'KnowledgeChunk', 'COLUMN', N'PageId';

DECLARE @defaultSchema335 AS sysname;
SET @defaultSchema335 = SCHEMA_NAME();
DECLARE @description335 AS sql_variant;
SET @description335 = N'课时或章节 Id，关联 Lesson.Id。';
EXEC sp_addextendedproperty 'MS_Description', @description335, 'SCHEMA', @defaultSchema335, 'TABLE', N'KnowledgeChunk', 'COLUMN', N'LessonId';

DECLARE @defaultSchema336 AS sysname;
SET @defaultSchema336 = SCHEMA_NAME();
DECLARE @description336 AS sql_variant;
SET @description336 = N'知识点 Id，关联 KnowledgePoint.Id。';
EXEC sp_addextendedproperty 'MS_Description', @description336, 'SCHEMA', @defaultSchema336, 'TABLE', N'KnowledgeChunk', 'COLUMN', N'KnowledgePointId';

DECLARE @defaultSchema337 AS sysname;
SET @defaultSchema337 = SCHEMA_NAME();
DECLARE @description337 AS sql_variant;
SET @description337 = N'创建时间，使用 UTC 时间记录。';
EXEC sp_addextendedproperty 'MS_Description', @description337, 'SCHEMA', @defaultSchema337, 'TABLE', N'KnowledgeChunk', 'COLUMN', N'CreatedTime';

DECLARE @defaultSchema338 AS sysname;
SET @defaultSchema338 = SCHEMA_NAME();
DECLARE @description338 AS sql_variant;
SET @description338 = N'知识片段类型。';
EXEC sp_addextendedproperty 'MS_Description', @description338, 'SCHEMA', @defaultSchema338, 'TABLE', N'KnowledgeChunk', 'COLUMN', N'ChunkType';

DECLARE @defaultSchema339 AS sysname;
SET @defaultSchema339 = SCHEMA_NAME();
DECLARE @description339 AS sql_variant;
SET @description339 = N'知识片段标题。';
EXEC sp_addextendedproperty 'MS_Description', @description339, 'SCHEMA', @defaultSchema339, 'TABLE', N'KnowledgeChunk', 'COLUMN', N'ChunkTitle';

DECLARE @defaultSchema340 AS sysname;
SET @defaultSchema340 = SCHEMA_NAME();
DECLARE @description340 AS sql_variant;
SET @description340 = N'知识片段正文。';
EXEC sp_addextendedproperty 'MS_Description', @description340, 'SCHEMA', @defaultSchema340, 'TABLE', N'KnowledgeChunk', 'COLUMN', N'ChunkText';

DECLARE @defaultSchema341 AS sysname;
SET @defaultSchema341 = SCHEMA_NAME();
DECLARE @description341 AS sql_variant;
SET @description341 = N'主键，使用 32 位无分隔符 Guid 字符串。';
EXEC sp_addextendedproperty 'MS_Description', @description341, 'SCHEMA', @defaultSchema341, 'TABLE', N'KnowledgeChunk', 'COLUMN', N'Id';

DECLARE @defaultSchema342 AS sysname;
SET @defaultSchema342 = SCHEMA_NAME();
DECLARE @description342 AS sql_variant;
SET @description342 = N'用户 Id，关联 UserProfile.Id。';
EXEC sp_addextendedproperty 'MS_Description', @description342, 'SCHEMA', @defaultSchema342, 'TABLE', N'HomeworkCheckItem', 'COLUMN', N'UserId';

DECLARE @defaultSchema343 AS sysname;
SET @defaultSchema343 = SCHEMA_NAME();
DECLARE @description343 AS sql_variant;
SET @description343 = N'最后更新时间，未更新时为空。';
EXEC sp_addextendedproperty 'MS_Description', @description343, 'SCHEMA', @defaultSchema343, 'TABLE', N'HomeworkCheckItem', 'COLUMN', N'UpdatedTime';

DECLARE @defaultSchema344 AS sysname;
SET @defaultSchema344 = SCHEMA_NAME();
DECLARE @description344 AS sql_variant;
SET @description344 = N'学科。';
EXEC sp_addextendedproperty 'MS_Description', @description344, 'SCHEMA', @defaultSchema344, 'TABLE', N'HomeworkCheckItem', 'COLUMN', N'Subject';

DECLARE @defaultSchema345 AS sysname;
SET @defaultSchema345 = SCHEMA_NAME();
DECLARE @description345 AS sql_variant;
SET @description345 = N'学生答案。';
EXEC sp_addextendedproperty 'MS_Description', @description345, 'SCHEMA', @defaultSchema345, 'TABLE', N'HomeworkCheckItem', 'COLUMN', N'StudentAnswer';

DECLARE @defaultSchema346 AS sysname;
SET @defaultSchema346 = SCHEMA_NAME();
DECLARE @description346 AS sql_variant;
SET @description346 = N'题目或问题文本。';
EXEC sp_addextendedproperty 'MS_Description', @description346, 'SCHEMA', @defaultSchema346, 'TABLE', N'HomeworkCheckItem', 'COLUMN', N'QuestionText';

DECLARE @defaultSchema347 AS sysname;
SET @defaultSchema347 = SCHEMA_NAME();
DECLARE @description347 AS sql_variant;
SET @description347 = N'提问记录 Id，关联 QuestionRecord.Id。';
EXEC sp_addextendedproperty 'MS_Description', @description347, 'SCHEMA', @defaultSchema347, 'TABLE', N'HomeworkCheckItem', 'COLUMN', N'QuestionRecordId';

DECLARE @defaultSchema348 AS sysname;
SET @defaultSchema348 = SCHEMA_NAME();
DECLARE @description348 AS sql_variant;
SET @description348 = N'题号。';
EXEC sp_addextendedproperty 'MS_Description', @description348, 'SCHEMA', @defaultSchema348, 'TABLE', N'HomeworkCheckItem', 'COLUMN', N'QuestionNo';

DECLARE @defaultSchema349 AS sysname;
SET @defaultSchema349 = SCHEMA_NAME();
DECLARE @description349 AS sql_variant;
SET @description349 = N'知识点 Id，关联 KnowledgePoint.Id。';
EXEC sp_addextendedproperty 'MS_Description', @description349, 'SCHEMA', @defaultSchema349, 'TABLE', N'HomeworkCheckItem', 'COLUMN', N'KnowledgePointId';

DECLARE @defaultSchema350 AS sysname;
SET @defaultSchema350 = SCHEMA_NAME();
DECLARE @description350 AS sql_variant;
SET @description350 = N'是否正确。';
EXEC sp_addextendedproperty 'MS_Description', @description350, 'SCHEMA', @defaultSchema350, 'TABLE', N'HomeworkCheckItem', 'COLUMN', N'IsCorrect';

DECLARE @defaultSchema351 AS sysname;
SET @defaultSchema351 = SCHEMA_NAME();
DECLARE @description351 AS sql_variant;
SET @description351 = N'题目截图裁剪路径。';
EXEC sp_addextendedproperty 'MS_Description', @description351, 'SCHEMA', @defaultSchema351, 'TABLE', N'HomeworkCheckItem', 'COLUMN', N'ImageCropPath';

DECLARE @defaultSchema352 AS sysname;
SET @defaultSchema352 = SCHEMA_NAME();
DECLARE @description352 AS sql_variant;
SET @description352 = N'年级。';
EXEC sp_addextendedproperty 'MS_Description', @description352, 'SCHEMA', @defaultSchema352, 'TABLE', N'HomeworkCheckItem', 'COLUMN', N'Grade';

DECLARE @defaultSchema353 AS sysname;
SET @defaultSchema353 = SCHEMA_NAME();
DECLARE @description353 AS sql_variant;
SET @description353 = N'讲解内容。';
EXEC sp_addextendedproperty 'MS_Description', @description353, 'SCHEMA', @defaultSchema353, 'TABLE', N'HomeworkCheckItem', 'COLUMN', N'Explanation';

DECLARE @defaultSchema354 AS sysname;
SET @defaultSchema354 = SCHEMA_NAME();
DECLARE @description354 AS sql_variant;
SET @description354 = N'错误原因。';
EXEC sp_addextendedproperty 'MS_Description', @description354, 'SCHEMA', @defaultSchema354, 'TABLE', N'HomeworkCheckItem', 'COLUMN', N'ErrorReason';

DECLARE @defaultSchema355 AS sysname;
SET @defaultSchema355 = SCHEMA_NAME();
DECLARE @description355 AS sql_variant;
SET @description355 = N'创建时间，使用 UTC 时间记录。';
EXEC sp_addextendedproperty 'MS_Description', @description355, 'SCHEMA', @defaultSchema355, 'TABLE', N'HomeworkCheckItem', 'COLUMN', N'CreatedTime';

DECLARE @defaultSchema356 AS sysname;
SET @defaultSchema356 = SCHEMA_NAME();
DECLARE @description356 AS sql_variant;
SET @description356 = N'正确答案。';
EXEC sp_addextendedproperty 'MS_Description', @description356, 'SCHEMA', @defaultSchema356, 'TABLE', N'HomeworkCheckItem', 'COLUMN', N'CorrectAnswer';

DECLARE @defaultSchema357 AS sysname;
SET @defaultSchema357 = SCHEMA_NAME();
DECLARE @description357 AS sql_variant;
SET @description357 = N'主键，使用 32 位无分隔符 Guid 字符串。';
EXEC sp_addextendedproperty 'MS_Description', @description357, 'SCHEMA', @defaultSchema357, 'TABLE', N'HomeworkCheckItem', 'COLUMN', N'Id';

DECLARE @defaultSchema358 AS sysname;
SET @defaultSchema358 = SCHEMA_NAME();
DECLARE @description358 AS sql_variant;
SET @description358 = N'视频文件路径。';
EXEC sp_addextendedproperty 'MS_Description', @description358, 'SCHEMA', @defaultSchema358, 'TABLE', N'AnswerRecord', 'COLUMN', N'VideoPath';

DECLARE @defaultSchema359 AS sysname;
SET @defaultSchema359 = SCHEMA_NAME();
DECLARE @description359 AS sql_variant;
SET @description359 = N'用户 Id，关联 UserProfile.Id。';
EXEC sp_addextendedproperty 'MS_Description', @description359, 'SCHEMA', @defaultSchema359, 'TABLE', N'AnswerRecord', 'COLUMN', N'UserId';

DECLARE @defaultSchema360 AS sysname;
SET @defaultSchema360 = SCHEMA_NAME();
DECLARE @description360 AS sql_variant;
SET @description360 = N'提问记录 Id，关联 QuestionRecord.Id。';
EXEC sp_addextendedproperty 'MS_Description', @description360, 'SCHEMA', @defaultSchema360, 'TABLE', N'AnswerRecord', 'COLUMN', N'QuestionRecordId';

DECLARE @defaultSchema361 AS sysname;
SET @defaultSchema361 = SCHEMA_NAME();
DECLARE @description361 AS sql_variant;
SET @description361 = N'Prompt 版本。';
EXEC sp_addextendedproperty 'MS_Description', @description361, 'SCHEMA', @defaultSchema361, 'TABLE', N'AnswerRecord', 'COLUMN', N'PromptVersion';

DECLARE @defaultSchema362 AS sysname;
SET @defaultSchema362 = SCHEMA_NAME();
DECLARE @description362 AS sql_variant;
SET @description362 = N'输出类型。';
EXEC sp_addextendedproperty 'MS_Description', @description362, 'SCHEMA', @defaultSchema362, 'TABLE', N'AnswerRecord', 'COLUMN', N'OutputType';

DECLARE @defaultSchema363 AS sysname;
SET @defaultSchema363 = SCHEMA_NAME();
DECLARE @description363 AS sql_variant;
SET @description363 = N'模型名称。';
EXEC sp_addextendedproperty 'MS_Description', @description363, 'SCHEMA', @defaultSchema363, 'TABLE', N'AnswerRecord', 'COLUMN', N'ModelName';

DECLARE @defaultSchema364 AS sysname;
SET @defaultSchema364 = SCHEMA_NAME();
DECLARE @description364 AS sql_variant;
SET @description364 = N'创建时间，使用 UTC 时间记录。';
EXEC sp_addextendedproperty 'MS_Description', @description364, 'SCHEMA', @defaultSchema364, 'TABLE', N'AnswerRecord', 'COLUMN', N'CreatedTime';

DECLARE @defaultSchema365 AS sysname;
SET @defaultSchema365 = SCHEMA_NAME();
DECLARE @description365 AS sql_variant;
SET @description365 = N'数字人脚本 JSON。';
EXEC sp_addextendedproperty 'MS_Description', @description365, 'SCHEMA', @defaultSchema365, 'TABLE', N'AnswerRecord', 'COLUMN', N'AvatarScriptJson';

DECLARE @defaultSchema366 AS sysname;
SET @defaultSchema366 = SCHEMA_NAME();
DECLARE @description366 AS sql_variant;
SET @description366 = N'音频文件路径。';
EXEC sp_addextendedproperty 'MS_Description', @description366, 'SCHEMA', @defaultSchema366, 'TABLE', N'AnswerRecord', 'COLUMN', N'AudioPath';

DECLARE @defaultSchema367 AS sysname;
SET @defaultSchema367 = SCHEMA_NAME();
DECLARE @description367 AS sql_variant;
SET @description367 = N'AI 回答文本。';
EXEC sp_addextendedproperty 'MS_Description', @description367, 'SCHEMA', @defaultSchema367, 'TABLE', N'AnswerRecord', 'COLUMN', N'AnswerText';

DECLARE @defaultSchema368 AS sysname;
SET @defaultSchema368 = SCHEMA_NAME();
DECLARE @description368 AS sql_variant;
SET @description368 = N'AI 回答结构化 JSON。';
EXEC sp_addextendedproperty 'MS_Description', @description368, 'SCHEMA', @defaultSchema368, 'TABLE', N'AnswerRecord', 'COLUMN', N'AnswerJson';

DECLARE @defaultSchema369 AS sysname;
SET @defaultSchema369 = SCHEMA_NAME();
DECLARE @description369 AS sql_variant;
SET @description369 = N'Agent 名称。';
EXEC sp_addextendedproperty 'MS_Description', @description369, 'SCHEMA', @defaultSchema369, 'TABLE', N'AnswerRecord', 'COLUMN', N'AgentName';

DECLARE @defaultSchema370 AS sysname;
SET @defaultSchema370 = SCHEMA_NAME();
DECLARE @description370 AS sql_variant;
SET @description370 = N'主键，使用 32 位无分隔符 Guid 字符串。';
EXEC sp_addextendedproperty 'MS_Description', @description370, 'SCHEMA', @defaultSchema370, 'TABLE', N'AnswerRecord', 'COLUMN', N'Id';

DECLARE @defaultSchema371 AS sysname;
SET @defaultSchema371 = SCHEMA_NAME();
DECLARE @description371 AS sql_variant;
SET @description371 = N'用户 Id，关联 UserProfile.Id。';
EXEC sp_addextendedproperty 'MS_Description', @description371, 'SCHEMA', @defaultSchema371, 'TABLE', N'AgentRouteLog', 'COLUMN', N'UserId';

DECLARE @defaultSchema372 AS sysname;
SET @defaultSchema372 = SCHEMA_NAME();
DECLARE @description372 AS sql_variant;
SET @description372 = N'学习会话 Id，关联 LearningSession.Id。';
EXEC sp_addextendedproperty 'MS_Description', @description372, 'SCHEMA', @defaultSchema372, 'TABLE', N'AgentRouteLog', 'COLUMN', N'SessionId';

DECLARE @defaultSchema373 AS sysname;
SET @defaultSchema373 = SCHEMA_NAME();
DECLARE @description373 AS sql_variant;
SET @description373 = N'路由选中的模型提供方。';
EXEC sp_addextendedproperty 'MS_Description', @description373, 'SCHEMA', @defaultSchema373, 'TABLE', N'AgentRouteLog', 'COLUMN', N'SelectedModelProvider';

DECLARE @defaultSchema374 AS sysname;
SET @defaultSchema374 = SCHEMA_NAME();
DECLARE @description374 AS sql_variant;
SET @description374 = N'路由选中的模型名称。';
EXEC sp_addextendedproperty 'MS_Description', @description374, 'SCHEMA', @defaultSchema374, 'TABLE', N'AgentRouteLog', 'COLUMN', N'SelectedModelName';

DECLARE @defaultSchema375 AS sysname;
SET @defaultSchema375 = SCHEMA_NAME();
DECLARE @description375 AS sql_variant;
SET @description375 = N'路由选中的 Agent。';
EXEC sp_addextendedproperty 'MS_Description', @description375, 'SCHEMA', @defaultSchema375, 'TABLE', N'AgentRouteLog', 'COLUMN', N'SelectedAgent';

DECLARE @defaultSchema376 AS sysname;
SET @defaultSchema376 = SCHEMA_NAME();
DECLARE @description376 AS sql_variant;
SET @description376 = N'路由原因。';
EXEC sp_addextendedproperty 'MS_Description', @description376, 'SCHEMA', @defaultSchema376, 'TABLE', N'AgentRouteLog', 'COLUMN', N'RouteReason';

DECLARE @defaultSchema377 AS sysname;
SET @defaultSchema377 = SCHEMA_NAME();
DECLARE @description377 AS sql_variant;
SET @description377 = N'提问记录 Id，关联 QuestionRecord.Id。';
EXEC sp_addextendedproperty 'MS_Description', @description377, 'SCHEMA', @defaultSchema377, 'TABLE', N'AgentRouteLog', 'COLUMN', N'QuestionRecordId';

DECLARE @defaultSchema378 AS sysname;
SET @defaultSchema378 = SCHEMA_NAME();
DECLARE @description378 AS sql_variant;
SET @description378 = N'路由识别的提问模式。';
EXEC sp_addextendedproperty 'MS_Description', @description378, 'SCHEMA', @defaultSchema378, 'TABLE', N'AgentRouteLog', 'COLUMN', N'QuestionMode';

DECLARE @defaultSchema379 AS sysname;
SET @defaultSchema379 = SCHEMA_NAME();
DECLARE @description379 AS sql_variant;
SET @description379 = N'Agent 输入类型。';
EXEC sp_addextendedproperty 'MS_Description', @description379, 'SCHEMA', @defaultSchema379, 'TABLE', N'AgentRouteLog', 'COLUMN', N'InputType';

DECLARE @defaultSchema380 AS sysname;
SET @defaultSchema380 = SCHEMA_NAME();
DECLARE @description380 AS sql_variant;
SET @description380 = N'创建时间，使用 UTC 时间记录。';
EXEC sp_addextendedproperty 'MS_Description', @description380, 'SCHEMA', @defaultSchema380, 'TABLE', N'AgentRouteLog', 'COLUMN', N'CreatedTime';

DECLARE @defaultSchema381 AS sysname;
SET @defaultSchema381 = SCHEMA_NAME();
DECLARE @description381 AS sql_variant;
SET @description381 = N'主键，使用 32 位无分隔符 Guid 字符串。';
EXEC sp_addextendedproperty 'MS_Description', @description381, 'SCHEMA', @defaultSchema381, 'TABLE', N'AgentRouteLog', 'COLUMN', N'Id';

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260508060543_AddTableAndColumnComments', N'10.0.7');

COMMIT;
GO

