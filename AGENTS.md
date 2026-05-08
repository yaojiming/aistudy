# AGENTS.md

## 项目名称

AiTutor - 儿童平板 AI 学习助手

## 项目定位

AiTutor 是一个运行在 Android 平板上的儿童 AI 学习助手，面向小学阶段学生。第一阶段不做手机管控、不做开机自启动、不做桌面管控、不做打开微信/相机/相册，只专注学习场景。

第一阶段核心能力：

1. 文字问答
2. 拍照讲题
3. 作业检查
4. 错题本
5. 教材知识库基础结构
6. Agent 模型路由框架
7. SQL Server 数据库
8. 后续语音电话聊天和 AI 智能人视频讲解的数据预留

---

## 总体架构

```text
.NET MAUI Android 平板端
        |
        | HTTP API
        v
ASP.NET Core WebAPI 后端
        |
        | AgentRouter
        v
ChatAgent / VisionAgent / HomeworkCheckAgent / WrongBookAgent / TextbookRagAgent
        |
        v
DeepSeek 文本模型 / GLM 视觉模型 / Mock Provider
        |
        v
SQL Server：教材库、错题库、练习、学习计划、日志、媒体资源
```

---

## 解决方案结构

推荐结构：

```text
AiTutor/
├── AGENTS.md
├── PLANS.md
├── docs/
│   ├── PROJECT_REQUIREMENTS.md
│   ├── DATABASE_DESIGN.md
│   ├── API_DESIGN.md
│   └── PROMPT_TEMPLATES.md
├── .agents/
│   └── skills/
│       └── ai-learning-app/
│           └── SKILL.md
├── scripts/
│   └── sqlserver/
├── src/
│   ├── AiTutor.Core
│   ├── AiTutor.Infrastructure
│   ├── AiTutor.Api
│   ├── AiTutor.Shared
│   └── AiTutor.Maui
└── tests/
    └── AiTutor.Tests
```

---

## 项目职责边界

### AiTutor.Core

只放领域模型、枚举、接口，不引用 EF Core、HTTP Client、第三方模型 SDK。

包含：

- Entities
- Enums
- Interfaces
- DomainServices

### AiTutor.Infrastructure

放基础设施实现：

- AiTutorDbContext
- EF Core SQL Server 配置
- Repository
- Agent 实现
- ModelProvider 实现
- PromptTemplateService
- FileStorageService
- ModelCallLogService

### AiTutor.Api

放 WebAPI：

- Controllers
- Middlewares
- DependencyInjection
- Swagger
- appsettings

Controller 只负责请求和响应，不能堆业务逻辑。

### AiTutor.Shared

放前后端共享 DTO：

- AgentRequest
- AgentResponse
- HomeworkCheckResultDto
- WrongQuestionDto
- PracticeQuestionDto
- StudyPlanDto

### AiTutor.Maui

放 Android 平板端页面和 ViewModel。

要求：

- 不直接调用 AI 模型
- 不保存 API Key
- 只调用 AiTutor.Api
- MVVM
- 平板大屏布局
- 大按钮、大字体、儿童友好

---

## SQL Server 数据库规则

1. 本项目使用 SQL Server，不使用 SQLite。
2. 开发库名称建议：AiTutorDb。
3. 禁止连接生产库。
4. 禁止执行 DROP DATABASE。
5. 禁止执行 DROP TABLE，除非用户明确要求。
6. 禁止把数据库密码写入 Git。
7. 连接字符串放在 appsettings.Development.json、用户机密或环境变量中。
8. 所有表结构变更通过 EF Core Migration 管理。
9. 每次 Migration 必须同步生成 SQL 脚本到 scripts/sqlserver。
10. 执行 database update 前必须先 build 成功，并说明将执行的命令。

字段约定：

```text
主键：NVARCHAR(64)
短状态字段：NVARCHAR(50)
名称标题：NVARCHAR(200)
文件路径：NVARCHAR(500)
大文本：NVARCHAR(MAX)
时间：DATETIME2
布尔：BIT
评分：DECIMAL(5,2)
金额/成本：DECIMAL(18,4)
```

常用索引字段：

```text
UserId
TextbookId
UnitId
LessonId
PageId
KnowledgePointId
WrongQuestionId
PracticeQuestionId
StudyPlanId
StudyStageId
SessionId
QuestionRecordId
Status
Subject
Grade
CreatedTime
NextReviewTime
```

---

## 必须预留的未来能力

数据库和 DTO 从第一阶段开始必须支持：

1. 教材上传
2. 教材 PDF / 图片解析
3. OCR 结果保存
4. 教材 Chunk 切片
5. 向量检索预留
6. 错题自动生成同类练习
7. 根据知识点生成练习题
8. 分阶段练习计划
9. 错题复习计划
10. 语音讨论记录
11. AI 智能人视频讲解记录

---

## 核心数据主线

本项目不能把数据做散。核心主线是：

```text
Textbook / TextbookUnit / Lesson / TextbookPage
→ KnowledgePoint / KnowledgeChunk / KnowledgeEmbedding
→ QuestionRecord / HomeworkCheckItem
→ WrongQuestion
→ PracticeQuestion
→ StudyPlan / StudyStage / StageTask
→ ReviewSchedule
→ LearningReport
```

---

## Agent 设计要求

必须包含接口：

```text
IAgentService
IAgentRouter
IAgent
ITextModelProvider
IVisionModelProvider
IPromptTemplateService
ITextbookKnowledgeService
IWrongQuestionService
IHomeworkCheckService
IPracticeQuestionService
IStudyPlanService
IReviewScheduleService
IModelCallLogService
IAgentRouteLogService
IMediaResourceService
```

必须包含 Agent：

```text
ChatAgent
VisionAgent
HomeworkCheckAgent
WrongBookAgent
TextbookRagAgent
PracticeGenerateAgent
StudyPlanAgent
```

后续预留：

```text
VoiceAgent
AvatarAgent
```

---

## 模型路由规则

第一阶段使用规则路由，不要一开始做复杂 LLM 路由：

1. InputType = Image 或 Mixed，并且有图片时，走 VisionAgent。
2. Mode = check_homework，走 HomeworkCheckAgent。
3. Mode = wrong_review，走 WrongBookAgent。
4. Mode = textbook_qa，走 TextbookRagAgent。
5. Mode = practice_generate，走 PracticeGenerateAgent。
6. Mode = study_plan，走 StudyPlanAgent。
7. 其他走 ChatAgent。

每次路由必须保存 AgentRouteLog。

---

## AI 调用规则

1. MAUI 客户端禁止直接调用 DeepSeek。
2. MAUI 客户端禁止直接调用 GLM。
3. API Key 只能在后端。
4. 默认使用 Mock Provider。
5. 真实 DeepSeekProvider 和 GlmVisionProvider 通过配置开启。
6. 所有调用必须写 ModelCallLog。
7. Prompt 必须集中管理，不要散落在 Controller。

---

## Prompt 规则

1. Prompt 由 PromptTemplateService 管理。
2. 每个 Prompt 必须有 TemplateCode。
3. 每个 Prompt 必须有 Version。
4. ModelCallLog 必须记录 PromptVersion。
5. Controller 不能直接写 Prompt。
6. 回答必须适合小学生。
7. 不要只给答案，要讲解。
8. 作业检查必须返回结构化结果。
9. 不确定时要说明不确定，不能编造教材内容。

---

## 编码规范

1. C# 使用 async/await。
2. 服务接口以 I 开头。
3. DTO 与 Entity 分开。
4. Controller 不直接操作 DbContext。
5. 主键使用 string，默认 Guid.NewGuid().ToString("N")。
6. 所有实体有 CreatedTime。
7. 需要更新的实体有 UpdatedTime。
8. 重要类和方法添加中文注释。
9. 异常要记录日志，不要吞掉。
10. 不要无关重构。
11. 不要删除 docs 下设计文档。
12. 每次修改后必须尝试 build。

---

## 每次任务完成后的输出要求

Codex 每次完成任务后必须输出：

1. 本次完成内容
2. 修改文件列表
3. 新增文件列表
4. 数据库是否变更
5. Migration 名称
6. SQL 脚本路径
7. 如何运行
8. 如何测试
9. build 是否成功
10. 当前未完成事项
11. 下一步建议
