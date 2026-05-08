# PLANS.md

## 开发总原则

AiTutor 必须分阶段开发。每个阶段只完成当前目标，不要一次性实现所有功能。每个阶段结束后必须保持可编译、可运行、可验收。

---

# Phase 0：项目初始化

## 目标

创建解决方案结构，不实现业务功能。

## 任务

1. 创建解决方案 AiTutor。
2. 创建项目：
   - src/AiTutor.Core
   - src/AiTutor.Infrastructure
   - src/AiTutor.Api
   - src/AiTutor.Shared
   - src/AiTutor.Maui
   - tests/AiTutor.Tests
3. 配置项目引用：
   - Infrastructure 引用 Core
   - Api 引用 Core、Infrastructure、Shared
   - Maui 引用 Shared
   - Tests 引用 Core、Infrastructure
4. Api 启用 Swagger。
5. 创建 scripts/sqlserver 目录。
6. 创建 README.md 和 .gitignore。

## 验收标准

1. dotnet build 成功。
2. Api 可以启动 Swagger。
3. 不包含真实 API Key。
4. 不实现业务功能。

---

# Phase 1：领域模型和 SQL Server 数据库

## 目标

建立完整的数据模型，为教材知识库、错题本、练习生成、分阶段练习、语音和视频预留打基础。

## 必须创建的实体

### 用户

- UserProfile
- LearningProfile

### 教材知识库

- Textbook
- TextbookUnit
- Lesson
- TextbookPage
- KnowledgePoint
- KnowledgeChunk
- KnowledgeEmbedding

### 教材导入

- TextbookImportJob
- TextbookImportFile
- TextbookParseLog

### 会话和问答

- LearningSession
- SessionMessage
- QuestionRecord
- AnswerRecord
- HomeworkCheckItem

### 错题和复习

- WrongQuestion
- WrongQuestionReview
- ReviewSchedule

### 练习生成

- PracticeQuestion
- PracticeQuestionSource
- PracticeAnswerRecord

### 分阶段练习

- StudyPlan
- StudyStage
- StageTask
- StageTaskRecord

### 媒体与日志

- MediaResource
- ModelCallLog
- AgentRouteLog

## 必须创建的枚举

- AgentInputType
- AgentOutputType
- SessionType
- MessageRole
- MessageContentType
- QuestionMode
- ModelProviderType
- MasteryStatus
- MediaResourceType
- TextbookImportStatus
- TextbookImportType
- PracticeGenerateType
- PracticeSourceType
- StudyPlanType
- StudyPlanStatus
- StageTaskType
- StageTaskStatus
- ReviewTargetType
- ReviewScheduleStatus

## 数据库任务

1. 创建 AiTutorDbContext。
2. 使用 Microsoft.EntityFrameworkCore.SqlServer。
3. 配置所有 DbSet。
4. 配置字段长度、必填、默认值。
5. 配置常用索引。
6. 生成 InitialCreate Migration。
7. 生成 SQL 脚本：scripts/sqlserver/InitialCreate.sql。

## 验收标准

1. dotnet build 成功。
2. 能生成 Migration。
3. SQL 脚本不包含 DROP DATABASE。
4. SQL 脚本不包含 DROP TABLE。
5. 不直接执行 database update，除非用户明确允许。

---

# Phase 2：Agent 后端框架

## 目标

实现可运行的 Agent 网关，先使用 Mock Provider，不接真实模型。

## 任务

1. 创建共享 DTO：
   - AgentRequest
   - AgentResponse
   - AgentRouteResult
   - HomeworkCheckResultDto
   - HomeworkCheckItemDto
   - WrongQuestionDto
   - TextbookReferenceDto
   - PracticeQuestionDto
   - StudyPlanDto
   - MediaUploadResultDto
2. 创建核心接口：
   - IAgentService
   - IAgentRouter
   - IAgent
   - ITextModelProvider
   - IVisionModelProvider
   - IPromptTemplateService
   - ITextbookKnowledgeService
   - IWrongQuestionService
   - IHomeworkCheckService
   - IPracticeQuestionService
   - IStudyPlanService
   - IReviewScheduleService
   - IModelCallLogService
   - IAgentRouteLogService
   - IMediaResourceService
3. 创建 Agent：
   - ChatAgent
   - VisionAgent
   - HomeworkCheckAgent
   - WrongBookAgent
   - TextbookRagAgent
   - PracticeGenerateAgent
   - StudyPlanAgent
4. 创建 MockTextModelProvider 和 MockVisionModelProvider。
5. 创建 AgentController：POST /api/agent/ask。
6. 每次请求保存：
   - LearningSession
   - SessionMessage
   - QuestionRecord
   - AnswerRecord
   - AgentRouteLog
   - ModelCallLog

## 验收标准

1. Swagger 可调用 /api/agent/ask。
2. 普通文字问题能返回模拟讲解。
3. 图片问题能进入 VisionAgent。
4. 作业检查能返回结构化结果。
5. 错题和练习生成接口有 Mock 返回。
6. dotnet build 成功。

---

# Phase 3：真实模型 Provider 预留

## 目标

建立可切换模型 Provider 机制。

## 任务

1. 增加 AI 配置节点。
2. 支持 ProviderMode = Mock / Real。
3. 创建 DeepSeekProvider。
4. 创建 GlmVisionProvider。
5. API Key 从配置、用户机密或环境变量读取。
6. 增加超时和重试配置。
7. 模型调用失败时返回友好错误。

## 验收标准

1. 默认 Mock 模式可用。
2. Real 模式可配置。
3. 不在代码中硬编码 API Key。
4. MAUI 客户端没有 API Key。
5. dotnet build 成功。

---

# Phase 4：MAUI 平板端 MVP

## 目标

实现 Android 平板端基础页面，调用后端 Agent API。

## 页面

1. 首页
2. 文字问答页
3. 拍照讲题页
4. 作业检查页
5. 错题本页
6. 教材学习页
7. 分阶段练习页

## 要求

1. MVVM 架构。
2. 平板大屏风格。
3. 大按钮、大字体。
4. 不在客户端保存 API Key。
5. 不直接调用 AI 模型。
6. 后端地址可配置。
7. 网络失败有提示。

## 验收标准

1. 能调用 /api/agent/ask。
2. 能显示模拟 AI 回答。
3. 能上传图片。
4. 能显示作业检查结果。
5. 能查看错题列表。
6. dotnet build 成功。

---

# Phase 5：教材知识库基础功能

## 目标

实现教材上传、解析任务记录、知识点管理、知识片段管理和关键词检索。

## 任务

1. 教材列表管理。
2. 教材 PDF / 图片上传。
3. 创建 TextbookImportJob。
4. 保存 TextbookImportFile。
5. 保存 TextbookParseLog。
6. 保存 TextbookPage。
7. 保存 OcrText 和 CleanText。
8. 维护 KnowledgePoint。
9. 维护 KnowledgeChunk。
10. 支持关键词检索。
11. Agent 可引用教材知识片段。

## 暂不实现

1. 自动 OCR。
2. 自动章节识别。
3. 自动知识点抽取。
4. 真正向量数据库。

## 验收标准

1. 可以创建教材。
2. 可以上传教材文件。
3. 可以记录导入任务。
4. 可以录入知识点和知识片段。
5. Agent 可以检索知识片段。
6. dotnet build 成功。

---

# Phase 6：错题练习生成与分阶段练习

## 目标

根据错题和知识点生成练习题，并形成分阶段练习计划。

## 任务

### 练习题生成

1. 根据 WrongQuestion 生成 PracticeQuestion。
2. 根据 KnowledgePoint 生成 PracticeQuestion。
3. 保存 PracticeQuestionSource。
4. 学生作答后保存 PracticeAnswerRecord。
5. 根据作答结果更新 MasteryStatus。
6. 生成 ReviewSchedule。

### 分阶段练习

1. 创建 StudyPlan。
2. 创建 StudyStage。
3. 创建 StageTask。
4. 学生完成后保存 StageTaskRecord。
5. 根据完成情况更新阶段状态。

## 推荐阶段

```text
阶段1：概念理解
阶段2：基础练习
阶段3：错题强化
阶段4：综合应用
阶段5：复习检测
```

## 验收标准

1. 能根据错题生成同类题。
2. 能根据知识点生成练习题。
3. 能创建学习计划。
4. 能查看阶段任务。
5. 能记录任务完成。
6. 能生成下一次复习时间。
7. dotnet build 成功。

---

# Phase 7：语音和 AI 智能人视频预留

## 目标

预留语音讨论和 AI 智能人视频讲解能力。

## 任务

1. 增加 VoiceAgent 接口。
2. 增加 AvatarAgent 接口。
3. AgentResponse 支持 AudioUrl、VideoUrl、AvatarScript、Subtitle。
4. SessionMessage 支持 audio、video、avatar_script。
5. MediaResource 支持 student_voice、ai_answer_audio、avatar_video。
6. StageTask 支持 voice_discussion、watch_avatar_video。

## 暂不实现

1. 实时语音电话。
2. 语音识别。
3. 语音合成。
4. 数字人视频生成。
5. WebRTC。

## 验收标准

1. DTO 支持音频和视频字段。
2. 数据库支持音频和视频资源。
3. 不影响现有文字和图片问答。
4. dotnet build 成功。

---

# Phase 8：真实语音电话与数字人讲解

## 目标

实现电话式语音讨论和 AI 智能人视频讲解。

## 语音电话链路

```text
麦克风录音
→ ASR
→ Agent
→ 文本模型
→ TTS
→ 播放语音
```

高级形态：

```text
WebSocket / WebRTC
→ 流式语音识别
→ 流式模型输出
→ 流式 TTS
→ 支持打断
```

## 数字人讲解

优先实现伪数字人：

```text
AI 老师头像
+ TTS
+ 字幕
+ 嘴型动画
+ 黑板步骤高亮
```

后续再接真实数字人视频服务。
