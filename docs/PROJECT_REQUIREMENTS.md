# PROJECT_REQUIREMENTS.md

## 一、产品定位

AiTutor 是一个运行在 Android 平板上的儿童 AI 学习助手，不是手机管控系统，也不是普通聊天 App。

第一阶段重点是：

1. 文字问答
2. 拍照讲题
3. 作业检查
4. 错题本
5. 教材知识库基础结构
6. Agent 模型路由框架
7. SQL Server 数据库

后续预留：

1. 语音电话式聊天讨论问题
2. AI 智能人视频讲解
3. 教材 RAG 检索
4. 学习报告
5. 家长端

---

## 二、用户角色

### 学生

小学生，使用 Android 平板学习。

主要行为：

- 输入问题
- 拍照提问
- 拍照检查作业
- 查看错题
- 重新练习错题
- 做分阶段练习
- 后续语音讨论问题
- 后续观看 AI 老师视频讲解

### 家长

第一阶段不实现家长端，但数据结构要为后续学习报告预留。

未来关注：

- 学习时长
- 错题数量
- 薄弱知识点
- 练习完成情况
- 阶段计划完成情况

### 管理员 / 教材维护人员

第一阶段可以不做完整后台，但接口和数据库要支持：

- 上传教材
- 维护教材版本
- 维护单元章节
- 维护知识点
- 维护知识片段
- 查看教材解析任务

---

## 三、产品边界

### 第一阶段做

1. 后端 Agent 网关
2. 文本问答 Mock / Provider 接口
3. 视觉问答 Mock / Provider 接口
4. SQL Server 数据库
5. 教材知识库表结构
6. 教材上传和解析任务表结构
7. 错题本
8. 同类练习题生成表结构
9. 分阶段练习表结构
10. MAUI 平板端基础页面

### 第一阶段不做

1. 开机自启动
2. 桌面管控
3. App 白名单
4. 打开微信/系统相机/系统相册
5. 家长端
6. 支付系统
7. 实时语音电话
8. 真实数字人视频生成
9. 复杂权限系统

---

## 四、核心流程

### 4.1 文字问答

```text
学生选择年级和学科
→ 输入问题
→ 平板提交到后端
→ 后端创建 LearningSession
→ 保存 QuestionRecord
→ AgentRouter 选择 ChatAgent
→ ChatAgent 组装 Prompt
→ 调用文本模型 Provider
→ 保存 AnswerRecord、SessionMessage、ModelCallLog
→ 返回 AgentResponse
→ 平板展示讲解
```

要求：

1. 不要只返回答案。
2. 必须分步骤讲解。
3. 必须适合当前年级。
4. 可以返回同类练习建议。
5. 可以加入错题本。

---

### 4.2 拍照讲题

```text
学生拍摄题目
→ 上传图片
→ 后端保存 MediaResource
→ 创建 QuestionRecord
→ AgentRouter 选择 VisionAgent
→ 视觉模型识别题目
→ 生成分步骤讲解
→ 保存 AnswerRecord
→ 返回识别结果和讲解
```

要求：

1. 图片原件必须保存。
2. 识别文本保存到 RecognizedText。
3. 回答包含题目理解、知识点、步骤、答案。
4. 图片不清楚时提示重新拍照。
5. 可加入错题本。

---

### 4.3 作业检查

```text
学生拍一页作业
→ 上传图片
→ 保存 MediaResource
→ HomeworkCheckAgent 调用视觉模型
→ 识别多道题
→ 逐题判断对错
→ 生成 HomeworkCheckItem
→ 错误题自动生成 WrongQuestion
→ 返回逐题检查结果
```

要求：

1. 一张作业图可能包含多道题。
2. 必须拆成 HomeworkCheckItem。
3. 错题自动进入 WrongQuestion。
4. 错题尽量关联 KnowledgePointId。
5. 前端显示每道题的对错和讲解。

---

### 4.4 错题本

```text
作业检查或拍照讲题
→ 发现错误或用户手动加入
→ 创建 WrongQuestion
→ 关联知识点
→ 保存学生答案、正确答案、错误原因、AI 讲解
→ 生成 ReviewSchedule
→ 后续生成同类练习题
```

要求：

1. 保存原题。
2. 保存学生答案。
3. 保存正确答案。
4. 保存错误原因。
5. 保存 AI 讲解。
6. 保存掌握状态。
7. 保存复习次数和下次复习时间。

---

### 4.5 教材知识库

```text
上传教材 PDF / 图片
→ 创建 TextbookImportJob
→ 保存 TextbookImportFile
→ 解析页面
→ 保存 TextbookPage
→ OCR 识别
→ 保存 OcrText
→ 清洗文本
→ 保存 CleanText
→ 切分 KnowledgeChunk
→ 关联 KnowledgePoint
→ 后续生成 Embedding
```

要求：

1. 支持教材版本、年级、学科、上下册。
2. 支持单元、章节、页码。
3. 支持知识点树。
4. 支持知识片段 Chunk。
5. 支持导入任务状态。
6. 支持解析日志。
7. 后续支持向量检索。

---

### 4.6 练习题生成

根据错题生成同类题：

```text
选择 WrongQuestion
→ 查找 KnowledgePoint
→ 查找相关 KnowledgeChunk
→ PracticeGenerateAgent 组装 Prompt
→ 模型生成同类题
→ 保存 PracticeQuestion
→ 保存 PracticeQuestionSource
→ 学生作答
→ 保存 PracticeAnswerRecord
→ 更新掌握状态
```

要求：

1. 练习题不能混在 WrongQuestion 表。
2. 生成题要记录来源。
3. 生成题要记录模型和 PromptVersion。
4. 学生作答必须保存。
5. 作答结果影响复习计划。

---

### 4.7 分阶段练习

```text
系统根据错题和知识点生成 StudyPlan
→ 计划拆成 StudyStage
→ 每个阶段拆成 StageTask
→ 学生完成任务
→ 保存 StageTaskRecord
→ 更新阶段状态
→ 生成下一步复习计划
```

推荐阶段：

```text
阶段1：概念理解
阶段2：基础练习
阶段3：错题强化
阶段4：综合应用
阶段5：复习检测
```

---

### 4.8 语音电话式讨论预留

未来流程：

```text
学生点击语音讨论
→ 建立语音会话
→ ASR 识别学生语音
→ Agent 理解上下文
→ 文本模型生成回答
→ TTS 生成语音
→ 播放给学生
→ 支持连续追问
```

第一阶段只预留数据结构和接口，不实现真实通话。

---

### 4.9 AI 智能人视频讲解预留

推荐先做伪数字人：

```text
AI 老师头像
+ TTS 语音
+ 字幕
+ 嘴型动画
+ 黑板步骤高亮
```

第一阶段只预留字段：AudioUrl、VideoUrl、AvatarScript、Subtitle。

---

## 五、AI 回答要求

### 普通问答

返回：

1. 问题理解
2. 分步骤讲解
3. 关键知识点
4. 简短结论
5. 同类练习题
6. 鼓励语

### 拍照讲题

返回：

1. 识别出的题目
2. 考查知识点
3. 解题思路
4. 分步骤讲解
5. 最终答案
6. 易错提醒

### 作业检查

返回：

1. 每道题题号
2. 学生答案
3. 正确答案
4. 是否正确
5. 错误原因
6. 讲解
7. 是否加入错题本

---

## 六、第一版 MVP 验收

1. 后端可以启动。
2. SQL Server 表可以创建。
3. Swagger 可以调用 /api/agent/ask。
4. 文字问答返回模拟讲解。
5. 图片问题进入 VisionAgent。
6. 作业检查返回多题结构化结果。
7. 错题可以保存和查询。
8. 可以创建教材和知识点。
9. 可以根据错题生成模拟练习题。
10. 可以创建学习计划。
11. MAUI 平板端可以调用后端。
12. 全项目 build 成功。
