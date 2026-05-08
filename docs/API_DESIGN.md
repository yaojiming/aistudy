# API_DESIGN.md

## 一、接口设计原则

1. MAUI 客户端只调用后端 API。
2. MAUI 客户端不直接调用 DeepSeek 或 GLM。
3. MAUI 客户端不保存 API Key。
4. 所有 AI 请求统一经过 Agent 网关。
5. 所有上传文件统一经过媒体资源接口。
6. 所有错误返回统一格式。
7. 所有关键请求必须记录日志。
8. 接口返回尽量结构化，便于平板端展示。

---

## 二、统一响应格式

```json
{
  "success": true,
  "message": "ok",
  "data": {},
  "errorCode": null,
  "traceId": "string"
}
```

失败：

```json
{
  "success": false,
  "message": "图片识别失败，请重新拍照",
  "data": null,
  "errorCode": "VISION_RECOGNIZE_FAILED",
  "traceId": "string"
}
```

---

## 三、核心接口清单

### Agent

| 方法 | 路径 | 说明 |
|---|---|---|
| POST | /api/agent/ask | 统一 AI 提问接口 |
| POST | /api/agent/route-test | 路由测试接口 |
| GET | /api/agent/sessions/{sessionId} | 获取会话详情 |

### 媒体

| 方法 | 路径 | 说明 |
|---|---|---|
| POST | /api/media/upload | 上传图片、音频、PDF |
| GET | /api/media/{id} | 获取媒体信息 |
| GET | /api/media/file/{id} | 访问媒体文件 |

### 教材

| 方法 | 路径 | 说明 |
|---|---|---|
| GET | /api/textbooks | 教材列表 |
| POST | /api/textbooks | 创建教材 |
| GET | /api/textbooks/{id} | 教材详情 |
| POST | /api/textbooks/{id}/import | 创建教材导入任务 |
| GET | /api/textbooks/import-jobs/{id} | 查看导入任务 |
| POST | /api/textbooks/{id}/knowledge-points | 创建知识点 |
| GET | /api/textbooks/{id}/knowledge-points | 知识点列表 |
| POST | /api/textbooks/knowledge-chunks/search | 检索知识片段 |

### 错题

| 方法 | 路径 | 说明 |
|---|---|---|
| GET | /api/wrong-questions | 错题列表 |
| POST | /api/wrong-questions | 手动添加错题 |
| GET | /api/wrong-questions/{id} | 错题详情 |
| PUT | /api/wrong-questions/{id} | 更新错题 |
| POST | /api/wrong-questions/{id}/review | 记录复习 |
| POST | /api/wrong-questions/{id}/generate-practice | 生成同类题 |

### 练习

| 方法 | 路径 | 说明 |
|---|---|---|
| GET | /api/practice-questions | 练习题列表 |
| POST | /api/practice-questions/generate | 生成练习题 |
| POST | /api/practice-questions/{id}/answer | 提交练习答案 |

### 学习计划

| 方法 | 路径 | 说明 |
|---|---|---|
| GET | /api/study-plans | 学习计划列表 |
| POST | /api/study-plans | 创建学习计划 |
| GET | /api/study-plans/{id} | 学习计划详情 |
| POST | /api/study-plans/{id}/stages | 创建阶段 |
| POST | /api/stage-tasks/{id}/complete | 完成阶段任务 |

---

## 四、POST /api/agent/ask

统一 AI 提问接口，用于文字问答、拍照讲题、作业检查、错题复习、教材问答、练习题生成、分阶段练习。

### 请求 AgentRequest

```json
{
  "userId": "test-user",
  "sessionId": null,
  "grade": "三年级",
  "subject": "数学",
  "inputType": "text",
  "mode": "ask",
  "questionText": "鸡兔同笼怎么做？",
  "imageMediaId": null,
  "audioMediaId": null,
  "relatedWrongQuestionId": null,
  "relatedKnowledgePointId": null,
  "relatedTextbookId": null,
  "needSaveHistory": true,
  "needGeneratePractice": false,
  "needTextbookReference": false,
  "clientInfo": {
    "deviceType": "android_tablet",
    "appVersion": "1.0.0"
  }
}
```

### 返回 AgentResponse

```json
{
  "sessionId": "session001",
  "questionRecordId": "question001",
  "answerRecordId": "answer001",
  "agentName": "ChatAgent",
  "modelProvider": "Mock",
  "modelName": "mock-text-model",
  "outputType": "text",
  "answerText": "我们先不要急着算，先看题目里的条件……",
  "answerJson": {
    "summary": "这道题考查假设法。",
    "steps": [
      "先假设全是鸡。",
      "再计算脚的数量差。",
      "用差值除以每只兔多出来的脚。"
    ],
    "finalAnswer": "……",
    "encouragement": "你已经理解关键思路了，再练一道就更熟了。"
  },
  "homeworkCheckResult": null,
  "textbookReferences": [],
  "practiceQuestions": [],
  "suggestions": [
    "再讲简单一点",
    "给我出一道类似题",
    "加入错题本"
  ],
  "audioUrl": null,
  "videoUrl": null,
  "avatarScript": null,
  "canAddToWrongBook": true,
  "createdWrongQuestionIds": [],
  "durationMs": 1234
}
```

---

## 五、作业检查返回结构

```json
{
  "summary": "共识别 5 道题，其中 4 道正确，1 道错误。",
  "totalCount": 5,
  "correctCount": 4,
  "wrongCount": 1,
  "items": [
    {
      "questionNo": "第1题",
      "questionText": "12 ÷ 3 = ?",
      "studentAnswer": "4",
      "correctAnswer": "4",
      "isCorrect": true,
      "errorReason": null,
      "explanation": "这道题做对了。",
      "knowledgePointId": "kp001",
      "knowledgePointName": "表内除法",
      "wrongQuestionId": null
    }
  ]
}
```

---

## 六、POST /api/media/upload

Content-Type：multipart/form-data

字段：

| 字段 | 说明 |
|---|---|
| file | 文件 |
| resourceType | image / audio / pdf / homework_photo / question_photo |
| userId | 用户 Id |
| sourceType | 来源 |

返回：

```json
{
  "mediaId": "media001",
  "resourceType": "homework_photo",
  "fileName": "homework.jpg",
  "filePath": "/uploads/homework/2026/05/homework.jpg",
  "mimeType": "image/jpeg",
  "fileSize": 123456,
  "url": "/api/media/file/media001"
}
```

要求：

1. 限制文件大小。
2. 限制文件类型。
3. 计算 Hash。
4. 保存 MediaResource。
5. 不允许客户端指定最终物理路径。

---

## 七、教材接口示例

### GET /api/textbooks

请求参数：

```text
subject=数学
grade=三年级
semester=下册
```

### POST /api/textbooks

```json
{
  "name": "人教版小学数学三年级下册",
  "publisher": "人民教育出版社",
  "subject": "数学",
  "grade": "三年级",
  "semester": "下册",
  "version": "2024"
}
```

### POST /api/textbooks/{id}/import

```json
{
  "mediaId": "media001",
  "importType": "pdf",
  "jobName": "导入三年级数学下册PDF"
}
```

### POST /api/textbooks/knowledge-chunks/search

```json
{
  "subject": "数学",
  "grade": "三年级",
  "keyword": "有余数除法",
  "topK": 5
}
```

---

## 八、错题接口示例

### GET /api/wrong-questions

请求参数：

```text
userId=test-user
subject=数学
masteryStatus=learning
pageIndex=1
pageSize=20
```

### POST /api/wrong-questions/{id}/generate-practice

```json
{
  "count": 3,
  "difficultyLevel": 1,
  "needExplanation": true
}
```

---

## 九、练习接口示例

### POST /api/practice-questions/generate

```json
{
  "userId": "test-user",
  "subject": "数学",
  "grade": "三年级",
  "knowledgePointId": "kp001",
  "generateType": "knowledge_point_practice",
  "count": 5,
  "difficultyLevel": 1
}
```

### POST /api/practice-questions/{id}/answer

```json
{
  "userId": "test-user",
  "userAnswer": "5余2",
  "durationSeconds": 30
}
```

---

## 十、学习计划接口示例

### POST /api/study-plans

```json
{
  "userId": "test-user",
  "title": "有余数除法专项提升",
  "subject": "数学",
  "grade": "三年级",
  "planType": "knowledge_point_practice",
  "knowledgePointId": "kp001"
}
```

### POST /api/stage-tasks/{id}/complete

```json
{
  "userId": "test-user",
  "actionType": "complete",
  "result": "correct",
  "score": 95,
  "durationSeconds": 180,
  "remark": "完成基础练习"
}
```

---

## 十一、语音和数字人接口预留

### POST /api/voice/ask

第一阶段不实现，只预留。

用途：语音提问、语音讨论、电话式连续对话。

### POST /api/avatar/explain

第一阶段不实现，只预留。

请求：

```json
{
  "userId": "test-user",
  "questionRecordId": "q001",
  "wrongQuestionId": "wq001",
  "teacherRole": "math_teacher",
  "style": "cartoon"
}
```

返回：

```json
{
  "mode": "avatar_animation",
  "subtitle": "我们先看题目条件……",
  "audioUrl": "/media/audio/explain001.mp3",
  "videoUrl": null,
  "avatarScript": [
    {
      "time": 0,
      "action": "look_question"
    }
  ]
}
```

---

## 十二、错误码建议

| 错误码 | 说明 |
|---|---|
| INVALID_REQUEST | 请求参数错误 |
| MEDIA_UPLOAD_FAILED | 媒体上传失败 |
| MEDIA_NOT_FOUND | 媒体不存在 |
| AGENT_ROUTE_FAILED | Agent 路由失败 |
| MODEL_CALL_FAILED | 模型调用失败 |
| VISION_RECOGNIZE_FAILED | 图片识别失败 |
| HOMEWORK_CHECK_FAILED | 作业检查失败 |
| TEXTBOOK_NOT_FOUND | 教材不存在 |
| WRONG_QUESTION_NOT_FOUND | 错题不存在 |
| PRACTICE_GENERATE_FAILED | 练习题生成失败 |
| STUDY_PLAN_NOT_FOUND | 学习计划不存在 |
| DATABASE_ERROR | 数据库错误 |
```

---

## 十三、Swagger 测试样例

普通文字问答：

```json
{
  "userId": "test-user",
  "grade": "三年级",
  "subject": "数学",
  "inputType": "text",
  "mode": "ask",
  "questionText": "什么是有余数除法？",
  "needSaveHistory": true
}
```

作业检查：

```json
{
  "userId": "test-user",
  "grade": "三年级",
  "subject": "数学",
  "inputType": "image",
  "mode": "check_homework",
  "imageMediaId": "media001",
  "needSaveHistory": true
}
```
