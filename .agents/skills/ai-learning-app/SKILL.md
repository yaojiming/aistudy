# AI Learning App Skill

## Skill 名称

ai-learning-app

## 使用场景

当任务涉及 AiTutor 儿童平板 AI 学习助手项目时，必须使用本 Skill。

适用任务：

1. 创建项目结构
2. 编写 ASP.NET Core 后端
3. 编写 .NET MAUI 平板端
4. 设计 SQL Server 数据库
5. 实现教材知识库
6. 实现教材上传与解析
7. 实现错题本
8. 实现同类题生成
9. 实现分阶段练习
10. 实现 Agent 路由
11. 接入 DeepSeek
12. 接入 GLM 视觉模型
13. 实现拍照讲题
14. 实现作业检查
15. 预留语音电话聊天
16. 预留 AI 智能人视频讲解

---

## 开始任务前必须阅读

每次开始编码前，必须先阅读：

1. AGENTS.md
2. PLANS.md
3. docs/PROJECT_REQUIREMENTS.md
4. docs/DATABASE_DESIGN.md
5. docs/API_DESIGN.md
6. docs/PROMPT_TEMPLATES.md

如果这些文件不存在，需要先创建这些文件，而不是直接写业务代码。

---

## 工作流程

1. 明确当前任务属于哪个 Phase。
2. 阅读相关设计文档。
3. 不跨 Phase 实现大量无关功能。
4. 先修改 Core 层实体、枚举和接口。
5. 再修改 Infrastructure 实现。
6. 再修改 Api Controller。
7. 最后修改 MAUI 页面。
8. 如果涉及数据库，先生成 Migration 和 SQL 脚本。
9. 运行 build。
10. build 失败时优先修复编译错误。
11. 输出本次修改摘要。

---

## 架构边界

### 必须遵守

1. 所有 AI 调用必须经过后端 Agent 服务。
2. 所有媒体文件必须通过 MediaResource 管理。
3. 所有会话消息必须进入 SessionMessage。
4. 所有问题必须进入 QuestionRecord。
5. 所有回答必须进入 AnswerRecord。
6. 作业检查必须拆成 HomeworkCheckItem。
7. 错题必须进入 WrongQuestion。
8. 同类练习题必须进入 PracticeQuestion。
9. 练习题来源必须进入 PracticeQuestionSource。
10. 学生练习作答必须进入 PracticeAnswerRecord。
11. 分阶段练习必须进入 StudyPlan、StudyStage、StageTask。
12. 阶段任务完成记录必须进入 StageTaskRecord。
13. 复习计划必须进入 ReviewSchedule。
14. 模型调用必须进入 ModelCallLog。
15. Agent 路由必须进入 AgentRouteLog。
16. Prompt 必须支持版本号。

### 禁止事项

1. 禁止在 MAUI 客户端直接调用 AI 模型。
2. 禁止在 MAUI 客户端保存 API Key。
3. 禁止 Controller 直接堆业务逻辑。
4. 禁止把数据库访问写到 MAUI 页面里。
5. 禁止把 Prompt 写死在 Controller。
6. 禁止一次性生成大量无关功能。
7. 禁止删除需求文档。
8. 禁止直接操作生产数据库。
9. 禁止执行 DROP DATABASE。
10. 禁止执行 DROP TABLE，除非用户明确要求。
11. 禁止把数据库密码写入 Git。
12. 禁止把 DeepSeek、GLM API Key 写入 Git。

---

## SQL Server 开发流程

当任务涉及数据库时，必须按以下流程执行：

1. 先修改 Core 实体。
2. 再修改枚举。
3. 再修改 DbContext。
4. 再配置字段长度、索引、默认值。
5. 再生成 EF Core Migration。
6. 再生成 SQL Server 脚本到 scripts/sqlserver。
7. 最后才允许执行 database update。

执行数据库更新前必须确认：

1. 当前连接的是 AiTutorDb 开发库。
2. SQL 脚本不包含 DROP DATABASE。
3. SQL 脚本不包含 DROP TABLE。
4. SQL 脚本不包含 TRUNCATE TABLE。
5. 没有删除已有字段，除非用户明确要求。
6. 没有真实密码写入仓库。
7. dotnet build 成功。

推荐命令：

```bash
dotnet ef migrations add InitialCreate --project src/AiTutor.Infrastructure --startup-project src/AiTutor.Api

dotnet ef migrations script --project src/AiTutor.Infrastructure --startup-project src/AiTutor.Api -o scripts/sqlserver/InitialCreate.sql

dotnet ef database update --project src/AiTutor.Infrastructure --startup-project src/AiTutor.Api
```

---

## 推荐分层实现顺序

### 后端功能

1. Core Entity
2. Core Enum
3. Core Interface
4. Shared DTO
5. Infrastructure DbContext
6. Infrastructure Repository
7. Infrastructure Service
8. Infrastructure Agent
9. Infrastructure ModelProvider
10. Api Controller
11. Api Swagger 测试

### MAUI 功能

1. Shared DTO
2. ApiClientService
3. ViewModel
4. Page
5. 样式
6. 错误提示
7. 加载状态
8. 测试调用

---

## Agent 开发要求

推荐 Agent：

1. ChatAgent：普通文字问答
2. VisionAgent：图片题讲解
3. HomeworkCheckAgent：作业检查
4. WrongBookAgent：错题复习
5. TextbookRagAgent：教材知识库问答
6. PracticeGenerateAgent：练习题生成
7. StudyPlanAgent：分阶段学习计划
8. VoiceAgent：语音讨论预留
9. AvatarAgent：数字人讲解预留

AgentRouter 第一阶段规则：

1. 有图片，走 VisionAgent。
2. Mode = check_homework，走 HomeworkCheckAgent。
3. Mode = wrong_review，走 WrongBookAgent。
4. Mode = textbook_qa，走 TextbookRagAgent。
5. Mode = practice_generate，走 PracticeGenerateAgent。
6. Mode = study_plan，走 StudyPlanAgent。
7. 其他走 ChatAgent。

---

## 教材知识库要求

教材相关功能必须围绕以下表：

1. Textbook
2. TextbookUnit
3. Lesson
4. TextbookPage
5. KnowledgePoint
6. KnowledgeChunk
7. KnowledgeEmbedding
8. TextbookImportJob
9. TextbookImportFile
10. TextbookParseLog

教材上传不能只保存 PDF 路径，必须创建导入任务。

---

## 错题和练习要求

相关功能必须围绕：

1. WrongQuestion
2. WrongQuestionReview
3. ReviewSchedule
4. PracticeQuestion
5. PracticeQuestionSource
6. PracticeAnswerRecord

要求：

1. 错题必须关联知识点。
2. 练习题必须记录来源。
3. 学生作答必须记录。
4. 错题复习必须更新 ReviewSchedule。
5. 同类题生成必须记录模型和 PromptVersion。

---

## 分阶段练习要求

相关功能必须围绕：

1. StudyPlan
2. StudyStage
3. StageTask
4. StageTaskRecord

阶段建议：

1. 概念理解
2. 基础练习
3. 错题强化
4. 综合应用
5. 复习检测

StageTask 需要支持未来任务类型：

1. read_explanation
2. do_practice
3. redo_wrong_question
4. review_knowledge_point
5. voice_discussion
6. watch_avatar_video
7. take_quiz

---

## 语音与数字人预留要求

第一阶段不实现真实功能，但 DTO、数据库、枚举必须预留。

需要支持：

1. AgentInputType.Voice
2. AgentOutputType.TextAndAudio
3. AgentOutputType.AvatarScript
4. AgentOutputType.Video
5. SessionType.VoiceCall
6. SessionType.AvatarLesson
7. MessageContentType.Audio
8. MessageContentType.Video
9. MessageContentType.AvatarScript
10. MediaResourceType.StudentVoice
11. MediaResourceType.AiAnswerAudio
12. MediaResourceType.AvatarVideo

---

## 返回格式

每次完成任务后，回复：

```text
本次完成内容：
- ...

修改文件：
- ...

新增文件：
- ...

数据库变更：
- 是否有
- Migration 名称
- SQL 脚本路径

如何运行：
- ...

如何测试：
- ...

build 结果：
- 成功/失败
- 如果失败，原因是什么

风险说明：
- ...

当前未完成：
- ...

下一步建议：
- ...
```

---

## 常用任务提示词

### 初始化项目

```text
请读取 AGENTS.md、PLANS.md 和 docs 下的设计文档。
现在只执行 Phase 0：项目初始化。
不要实现业务功能。
完成后运行 dotnet build。
```

### 数据库阶段

```text
请执行 Phase 1：领域模型和 SQL Server 数据库。
先生成实体、枚举、DbContext、Migration 和 SQL 脚本。
不要执行 database update，除非我明确允许。
```

### Agent 阶段

```text
请执行 Phase 2：Agent 后端框架。
先使用 Mock Provider，不接真实 DeepSeek 和 GLM。
所有请求必须记录 LearningSession、QuestionRecord、AnswerRecord、AgentRouteLog、ModelCallLog。
```

### MAUI 阶段

```text
请执行 Phase 4：MAUI 平板端 MVP。
不要在客户端保存 API Key。
不要直接调用 AI 模型。
只调用后端 API。
```
