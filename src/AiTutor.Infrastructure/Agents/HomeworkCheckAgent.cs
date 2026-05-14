using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using AiTutor.Core.Interfaces;
using AiTutor.Shared.Agent;
using Microsoft.Extensions.Logging;

namespace AiTutor.Infrastructure.Agents;

/// <summary>
/// 作业检查 Agent，负责调用视觉模型检查整页作业，并把模型 final 文本解析为结构化逐题结果。
/// </summary>
public class HomeworkCheckAgent : IStreamingAgent
{
    private static readonly JsonDocumentOptions JsonOptions = new()
    {
        AllowTrailingCommas = true,
        CommentHandling = JsonCommentHandling.Skip
    };

    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        AllowTrailingCommas = true,
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip
    };

    private readonly IVisionModelProvider _visionProvider;
    private readonly IPromptTemplateService _promptTemplateService;
    private readonly ILogger<HomeworkCheckAgent> _logger;

    public HomeworkCheckAgent(
        IVisionModelProvider visionProvider,
        IPromptTemplateService promptTemplateService,
        ILogger<HomeworkCheckAgent> logger)
    {
        _visionProvider = visionProvider;
        _promptTemplateService = promptTemplateService;
        _logger = logger;
    }

    public string Name => "HomeworkCheckAgent";

    /// <summary>
    /// 执行非流式作业检查，最终响应中会尽量填充 HomeworkCheckResultDto.Items。
    /// </summary>
    public async Task<AgentResponse> ExecuteAsync(
        AgentRequest request,
        AgentRouteResult route,
        CancellationToken cancellationToken = default)
    {
        var modelAnswer = await _visionProvider.AnalyzeImageAsync(
            request.ImageUrl ?? string.Empty,
            BuildPrompt(request),
            cancellationToken);

        return CreateResponse(route, request, modelAnswer);
    }

    /// <summary>
    /// 执行流式作业检查：先把模型 delta 原样返回给前端，final 时解析结构化逐题结果。
    /// </summary>
    public async IAsyncEnumerable<AgentStreamChunkDto> StreamExecuteAsync(
        AgentRequest request,
        AgentRouteResult route,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var answerBuilder = new StringBuilder();
        await foreach (var delta in _visionProvider.AnalyzeImageStreamAsync(
                           request.ImageUrl ?? string.Empty,
                           BuildPrompt(request),
                           request.ThinkingMode,
                           cancellationToken))
        {
            answerBuilder.Append(delta);
            yield return new AgentStreamChunkDto { Type = "delta", Text = delta };
        }

        yield return new AgentStreamChunkDto
        {
            Type = "final",
            FinalResponse = CreateResponse(route, request, answerBuilder.ToString())
        };
    }

    /// <summary>
    /// 渲染作业检查 Prompt，并附带客户端已经识别出的题号和题干摘要，便于 final 结果稳定对齐。
    /// </summary>
    private string BuildPrompt(AgentRequest request)
    {
        var prompt = _promptTemplateService.Render("homework_check", new Dictionary<string, string?>
        {
            ["imageUrl"] = request.ImageUrl,
            ["subject"] = request.Subject,
            ["grade"] = request.Grade
        });

        if (string.IsNullOrWhiteSpace(request.QuestionText))
        {
            return prompt;
        }

        return $"""
            {prompt}

            客户端补充要求如下。请遵守这些要求；不能判断时 isCorrect 返回 null，不要跳过题目：
            {request.QuestionText}
            """;
    }

    /// <summary>
    /// 组装作业检查响应。这里不再返回 mock，结构化结果来自模型 JSON；解析失败时返回不确定结果，避免前端误显示假数据。
    /// </summary>
    private AgentResponse CreateResponse(AgentRouteResult route, AgentRequest request, string modelAnswer)
    {
        var expectedQuestions = ExtractExpectedQuestions(request.QuestionText);
        var result = ParseHomeworkCheckResult(modelAnswer, expectedQuestions);
        _logger.LogInformation(
            "Homework check parsed. ModelAnswerLength={ModelAnswerLength}, ExpectedQuestionCount={ExpectedQuestionCount}, ParsedItemCount={ParsedItemCount}, QuestionNos={QuestionNos}",
            modelAnswer.Length,
            expectedQuestions.Count,
            result.Items.Count,
            string.Join(",", result.Items.Select(item => item.QuestionNo)));

        foreach (var item in result.Items)
        {
            _logger.LogInformation(
                "Homework check parsed item. QuestionNo={QuestionNo}, IsCorrect={IsCorrect}, StudentAnswerLength={StudentAnswerLength}, CorrectAnswerLength={CorrectAnswerLength}, ExplanationLength={ExplanationLength}, HasBBox={HasBBox}",
                item.QuestionNo,
                item.IsCorrect,
                item.StudentAnswer?.Length ?? 0,
                item.CorrectAnswer?.Length ?? 0,
                item.Explanation.Length,
                item.BBox is not null);
        }

        var response = ResponseFactory.Create(route, modelAnswer, canAddToWrongBook: result.Items.Any(item => item.IsCorrect == false));

        response.OutputType = "structured_json";
        response.HomeworkCheckResult = result;
        response.AnswerJson = result;
        response.Suggestions.Add(new SuggestionDto { Text = "生成同类题", Action = "practice_generate" });
        return response;
    }

    /// <summary>
    /// 从模型输出中解析 HomeworkCheckResultDto，并补齐题号、答案、讲解等前端依赖字段。
    /// </summary>
    private static HomeworkCheckResultDto ParseHomeworkCheckResult(
        string modelAnswer,
        IReadOnlyList<ExpectedQuestion> expectedQuestions)
    {
        _ = expectedQuestions;

        var json = ExtractJson(modelAnswer)
            ?? throw new JsonException("模型返回内容中没有找到 JSON。");

        var result = JsonSerializer.Deserialize<HomeworkCheckResultDto>(json, SerializerOptions)
            ?? throw new JsonException("模型返回 JSON 无法反序列化为 HomeworkCheckResultDto。");

        result.Items ??= [];
        return result;

#if false
        var parsedItems = TryParseJsonItems(modelAnswer);
        var items = NormalizeAndMergeItems(parsedItems, expectedQuestions, modelAnswer);
        var declaredTotalCount = ExtractDeclaredTotalCount(modelAnswer);
        EnsureDeclaredItemCount(items, declaredTotalCount);
        var correctCount = items.Count(item => item.IsCorrect == true);
        var wrongCount = items.Count(item => item.IsCorrect == false);
        var uncertainCount = items.Count(item => item.IsCorrect is null);

        return new HomeworkCheckResultDto
        {
            TotalCount = items.Count,
            CorrectCount = correctCount,
            WrongCount = wrongCount,
            Summary = $"共返回 {items.Count} 道题检查结果，正确 {correctCount} 道，错误 {wrongCount} 道，不确定 {uncertainCount} 道。",
            Items = items
        };
#endif
    }

    private static int ExtractDeclaredTotalCount(string modelAnswer)
    {
        var totalMatch = Regex.Match(
            modelAnswer,
            "\"totalCount\"\\s*:\\s*(?<count>\\d+)",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        if (totalMatch.Success && int.TryParse(totalMatch.Groups["count"].Value, out var totalCount))
        {
            return Math.Clamp(totalCount, 0, 100);
        }

        var summaryMatch = Regex.Match(
            modelAnswer,
            "共检查\\s*(?<count>\\d+)\\s*道题",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        return summaryMatch.Success && int.TryParse(summaryMatch.Groups["count"].Value, out totalCount)
            ? Math.Clamp(totalCount, 0, 100)
            : 0;
    }

    private static void EnsureDeclaredItemCount(List<HomeworkCheckItemDto> items, int declaredTotalCount)
    {
        if (declaredTotalCount <= items.Count || items.Count > 1)
        {
            return;
        }

        var existingNumbers = items
            .Select(item => NormalizeQuestionNo(item.QuestionNo))
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        for (var number = 1; items.Count < declaredTotalCount && number <= declaredTotalCount + 20; number++)
        {
            var questionNo = number.ToString();
            if (existingNumbers.Contains(questionNo))
            {
                continue;
            }

            items.Add(new HomeworkCheckItemDto
            {
                QuestionNo = questionNo,
                IsCorrect = null,
                ErrorReason = "AI 返回声明了这道题，但结构化明细解析失败。",
                Explanation = "AI 返回的整页 JSON 中声明包含这道题，但该题字段没有被稳定解析出来。请重新检查图片或查看模型原始返回。"
            });
            existingNumbers.Add(questionNo);
        }
    }

    /// <summary>
    /// 解析模型 JSON，支持纯 JSON、Markdown 代码块包裹 JSON、根对象或根数组。
    /// </summary>
    private static List<HomeworkCheckItemDto> TryParseJsonItems(string modelAnswer)
    {
        var json = ExtractJson(modelAnswer);
        if (string.IsNullOrWhiteSpace(json))
        {
            return [];
        }

        var directItems = TryDeserializeHomeworkItems(json);
        if (directItems.Count > 0)
        {
            return directItems;
        }

        try
        {
            using var document = JsonDocument.Parse(json, JsonOptions);
            var root = FindResultRoot(document.RootElement);
            var itemElements = root.ValueKind == JsonValueKind.Array
                ? root.EnumerateArray().ToArray()
                : TryGetProperty(root, out var itemsElement, "items", "Items", "questions", "Questions", "results", "Results")
                    && itemsElement.ValueKind == JsonValueKind.Array
                    ? itemsElement.EnumerateArray().ToArray()
                    : [];

            var items = new List<HomeworkCheckItemDto>(itemElements.Length);
            for (var index = 0; index < itemElements.Length; index++)
            {
                if (itemElements[index].ValueKind != JsonValueKind.Object)
                {
                    continue;
                }

                items.Add(ParseItem(itemElements[index], index));
            }

            return items.Count > 0 ? items : TryParseLooseJsonItems(json);
        }
        catch (JsonException)
        {
            return TryParseLooseJsonItems(json);
        }
    }

    private static List<HomeworkCheckItemDto> TryDeserializeHomeworkItems(string json)
    {
        try
        {
            var result = JsonSerializer.Deserialize<HomeworkCheckResultDto>(json, SerializerOptions);
            if (result?.Items is { Count: > 0 })
            {
                return result.Items;
            }
        }
        catch (JsonException)
        {
        }

        try
        {
            var items = JsonSerializer.Deserialize<List<HomeworkCheckItemDto>>(json, SerializerOptions);
            return items is { Count: > 0 } ? items : [];
        }
        catch (JsonException)
        {
            return [];
        }
    }

    private static List<HomeworkCheckItemDto> TryParseLooseJsonItems(string json)
    {
        var questionNoMatches = Regex.Matches(
            json,
            "\"(?<key>questionNo|QuestionNo|question_no|index)\"\\s*:",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        if (questionNoMatches.Count == 0)
        {
            return [];
        }

        var items = new List<HomeworkCheckItemDto>(questionNoMatches.Count);
        for (var index = 0; index < questionNoMatches.Count; index++)
        {
            var current = questionNoMatches[index];
            var start = FindItemObjectStart(json, current.Index);

            var end = json.Length;
            if (index + 1 < questionNoMatches.Count)
            {
                var nextStart = FindItemObjectStart(json, questionNoMatches[index + 1].Index);
                end = nextStart > start ? nextStart : questionNoMatches[index + 1].Index;
            }

            var objectEnd = FindItemObjectEnd(json, start);
            if (objectEnd > start && objectEnd < end)
            {
                end = objectEnd + 1;
            }

            var block = json[start..Math.Min(end, json.Length)];
            items.Add(ParseLooseItem(block, index));
        }

        return items;
    }

    private static int FindItemObjectStart(string json, int questionNoIndex)
    {
        var searchIndex = questionNoIndex;
        while (searchIndex >= 0)
        {
            var candidate = json.LastIndexOf('{', searchIndex);
            if (candidate < 0)
            {
                return questionNoIndex;
            }

            var prefix = json[Math.Max(0, candidate - 20)..candidate];
            if (!Regex.IsMatch(prefix, "\"(?:bbox|BBox)\"\\s*:\\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
            {
                return candidate;
            }

            searchIndex = candidate - 1;
        }

        return questionNoIndex;
    }

    private static int FindItemObjectEnd(string json, int objectStart)
    {
        var depth = 0;
        var inString = false;
        var escaped = false;

        for (var index = Math.Max(0, objectStart); index < json.Length; index++)
        {
            var ch = json[index];
            if (escaped)
            {
                escaped = false;
                continue;
            }

            if (ch == '\\' && inString)
            {
                escaped = true;
                continue;
            }

            if (ch == '"')
            {
                inString = !inString;
                continue;
            }

            if (inString)
            {
                continue;
            }

            if (ch == '{')
            {
                depth++;
                continue;
            }

            if (ch == '}')
            {
                depth--;
                if (depth == 0)
                {
                    return index;
                }
            }
        }

        return -1;
    }

    private static HomeworkCheckItemDto ParseLooseItem(string block, int index)
    {
        var isCorrect = ParseBoolText(GetLooseScalar(block, "isCorrect", "IsCorrect", "correct", "status", "result"));
        var explanation = FirstText(
            GetLooseScalar(block, "explanation", "Explanation", "analysis"),
            GetLooseScalar(block, "shortResult", "summary"),
            GetLooseScalar(block, "errorReason", "ErrorReason", "mistakeReason"));

        return new HomeworkCheckItemDto
        {
            QuestionNo = FirstText(GetLooseScalar(block, "questionNo", "QuestionNo", "question_no", "index", "no"), (index + 1).ToString()),
            QuestionText = FirstText(GetLooseScalar(block, "questionText", "QuestionText", "question"), string.Empty),
            StudentAnswer = NullIfEmpty(GetLooseScalar(block, "studentAnswer", "StudentAnswer", "student_answer")),
            CorrectAnswer = NullIfEmpty(GetLooseScalar(block, "correctAnswer", "CorrectAnswer", "correct_answer", "answer")),
            IsCorrect = isCorrect,
            ErrorReason = NullIfEmpty(GetLooseScalar(block, "errorReason", "ErrorReason", "mistakeReason")),
            Explanation = FirstText(explanation, "AI 已返回检查结果，但没有提供详细解析。"),
            KnowledgePointName = NullIfEmpty(GetLooseScalar(block, "knowledgePointName", "KnowledgePointName", "knowledgePoint")),
            BBox = ParseLooseBBox(block)
        };
    }

    private static string? GetLooseScalar(string block, params string[] names)
    {
        var fields = Regex.Matches(
                block,
                "\"(?<key>questionNo|QuestionNo|question_no|index|no|questionText|QuestionText|question|studentAnswer|StudentAnswer|student_answer|correctAnswer|CorrectAnswer|correct_answer|answer|isCorrect|IsCorrect|correct|status|result|errorReason|ErrorReason|mistakeReason|explanation|Explanation|analysis|shortResult|summary|knowledgePointName|KnowledgePointName|knowledgePoint|knowledgePointId|KnowledgePointId|wrongQuestionId|WrongQuestionId|bbox|BBox)\"\\s*:",
                RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)
            .Cast<Match>()
            .OrderBy(match => match.Index)
            .ToList();

        for (var index = 0; index < fields.Count; index++)
        {
            var key = fields[index].Groups["key"].Value;
            if (!names.Any(name => string.Equals(name, key, StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }

            var valueStart = fields[index].Index + fields[index].Length;
            var valueEnd = index + 1 < fields.Count ? fields[index + 1].Index : block.Length;
            if (valueEnd <= valueStart)
            {
                return null;
            }

            return CleanLooseScalar(block[valueStart..valueEnd]);
        }

        return null;
    }

    private static string? CleanLooseScalar(string raw)
    {
        var value = raw.Trim();
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        if (value.StartsWith('{'))
        {
            return value;
        }

        value = value.TrimEnd();
        while (value.EndsWith(',') || value.EndsWith('}') || value.EndsWith(']'))
        {
            value = value[..^1].TrimEnd();
        }

        if (value.StartsWith('"'))
        {
            value = value[1..];
        }

        value = value.Trim();
        while (value.EndsWith('"') || value.EndsWith(','))
        {
            value = value[..^1].TrimEnd();
        }

        var objectEnd = value.IndexOf("\n}", StringComparison.Ordinal);
        if (objectEnd >= 0)
        {
            value = value[..objectEnd].TrimEnd();
        }

        return string.IsNullOrWhiteSpace(value) ? null : value;
    }

    private static HomeworkCheckBBoxDto? ParseLooseBBox(string block)
    {
        var bboxMatch = Regex.Match(
            block,
            "\"(?:bbox|BBox)\"\\s*:\\s*\\{(?<bbox>.*?)\\}",
            RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        if (!bboxMatch.Success)
        {
            return null;
        }

        var bbox = bboxMatch.Groups["bbox"].Value;
        var x1 = GetLooseFloat(bbox, "x1", "X1", "left");
        var y1 = GetLooseFloat(bbox, "y1", "Y1", "top");
        var x2 = GetLooseFloat(bbox, "x2", "X2", "right");
        var y2 = GetLooseFloat(bbox, "y2", "Y2", "bottom");
        if (x1 is null || y1 is null || x2 is null || y2 is null)
        {
            return null;
        }

        return new HomeworkCheckBBoxDto
        {
            CoordinateSystem = "normalized_1000",
            X1 = Math.Clamp(x1.Value, 0, 1000),
            Y1 = Math.Clamp(y1.Value, 0, 1000),
            X2 = Math.Clamp(x2.Value, 0, 1000),
            Y2 = Math.Clamp(y2.Value, 0, 1000)
        };
    }

    private static float? GetLooseFloat(string block, params string[] names)
    {
        foreach (var name in names)
        {
            var match = Regex.Match(
                block,
                $"\"?{Regex.Escape(name)}\"?\\s*:\\s*(?<value>-?\\d+(?:\\.\\d+)?)",
                RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            if (match.Success && float.TryParse(match.Groups["value"].Value, out var value))
            {
                return value;
            }
        }

        return null;
    }

    private static JsonElement FindResultRoot(JsonElement root)
    {
        if (root.ValueKind != JsonValueKind.Object)
        {
            return root;
        }

        if (TryGetProperty(root, out _, "items", "questions", "results"))
        {
            return root;
        }

        foreach (var property in root.EnumerateObject())
        {
            if (property.Value.ValueKind == JsonValueKind.Object
                && TryGetProperty(property.Value, out _, "items", "questions", "results"))
            {
                return property.Value;
            }
        }

        return root;
    }

    private static HomeworkCheckItemDto ParseItem(JsonElement element, int index)
    {
        var isCorrect = GetNullableBool(element, "isCorrect", "IsCorrect", "correct", "status", "result", "判断");
        var explanation = FirstText(
            GetString(element, "explanation", "Explanation", "analysis", "解析", "讲解", "检查过程"),
            GetString(element, "shortResult", "summary", "说明"),
            GetString(element, "errorReason", "ErrorReason", "mistakeReason", "错因"));

        return new HomeworkCheckItemDto
        {
            QuestionNo = FirstText(GetString(element, "questionNo", "QuestionNo", "no", "题号"), (index + 1).ToString()),
            QuestionText = FirstText(GetString(element, "questionText", "QuestionText", "question", "题目"), string.Empty),
            StudentAnswer = NullIfEmpty(GetString(element, "studentAnswer", "StudentAnswer", "student_answer", "学生答案", "作答")),
            CorrectAnswer = NullIfEmpty(GetString(element, "correctAnswer", "CorrectAnswer", "correct_answer", "answer", "正确答案")),
            IsCorrect = isCorrect,
            ErrorReason = NullIfEmpty(GetString(element, "errorReason", "ErrorReason", "mistakeReason", "错因", "错误原因")),
            Explanation = FirstText(explanation, "AI 已返回检查结果，但没有提供详细解析。"),
            KnowledgePointName = NullIfEmpty(GetString(element, "knowledgePointName", "KnowledgePointName", "knowledgePoint", "知识点")),
            BBox = ParseBBox(element)
        };
    }

    private static HomeworkCheckBBoxDto? ParseBBox(JsonElement element)
    {
        if (!TryGetProperty(element, out var bbox, "bbox", "BBox", "box", "region")
            || bbox.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        var x1 = GetFloat(bbox, "x1", "X1", "left");
        var y1 = GetFloat(bbox, "y1", "Y1", "top");
        var x2 = GetFloat(bbox, "x2", "X2", "right");
        var y2 = GetFloat(bbox, "y2", "Y2", "bottom");
        if (x1 is null || y1 is null || x2 is null || y2 is null)
        {
            return null;
        }

        return new HomeworkCheckBBoxDto
        {
            CoordinateSystem = "normalized_1000",
            X1 = Math.Clamp(x1.Value, 0, 1000),
            Y1 = Math.Clamp(y1.Value, 0, 1000),
            X2 = Math.Clamp(x2.Value, 0, 1000),
            Y2 = Math.Clamp(y2.Value, 0, 1000)
        };
    }

    private static List<HomeworkCheckItemDto> NormalizeAndMergeItems(
        List<HomeworkCheckItemDto> parsedItems,
        IReadOnlyList<ExpectedQuestion> expectedQuestions,
        string modelAnswer)
    {
        var normalizedParsed = parsedItems
            .Select((item, index) => NormalizeItem(item, index))
            .Where(item => !string.IsNullOrWhiteSpace(item.QuestionNo))
            .ToList();

        if (expectedQuestions.Count == 0)
        {
            return normalizedParsed.Count > 0
                ? normalizedParsed
                :
                [
                    new HomeworkCheckItemDto
                    {
                        QuestionNo = "1",
                        IsCorrect = null,
                        Explanation = FirstText(modelAnswer, "AI 返回内容不是结构化 JSON，暂时无法判断对错。"),
                        ErrorReason = "AI 返回内容不是结构化 JSON，暂时无法判断对错。"
                    }
                ];
        }

        var result = new List<HomeworkCheckItemDto>(expectedQuestions.Count);
        for (var index = 0; index < expectedQuestions.Count; index++)
        {
            var expected = expectedQuestions[index];
            var matched = normalizedParsed.FirstOrDefault(item =>
                              string.Equals(NormalizeQuestionNo(item.QuestionNo), NormalizeQuestionNo(expected.QuestionNo), StringComparison.OrdinalIgnoreCase))
                          ?? normalizedParsed.ElementAtOrDefault(index);

            if (matched is null)
            {
                result.Add(new HomeworkCheckItemDto
                {
                    QuestionNo = expected.QuestionNo,
                    QuestionText = expected.QuestionText,
                    StudentAnswer = NullIfEmpty(expected.StudentAnswer),
                    IsCorrect = null,
                    ErrorReason = "AI 没有返回这道题的结构化结果。",
                    Explanation = "AI 没有返回这道题的结构化结果，请查看整页检查文本或重新拍摄更清晰的图片。"
                });
                continue;
            }

            matched.QuestionNo = FirstText(matched.QuestionNo, expected.QuestionNo);
            matched.QuestionText = FirstText(matched.QuestionText, expected.QuestionText);
            matched.StudentAnswer = FirstNullableText(matched.StudentAnswer, expected.StudentAnswer);
            matched.Explanation = FirstText(matched.Explanation, "AI 已返回检查结果，但没有提供详细解析。");
            result.Add(matched);
        }

        return result;
    }

    private static HomeworkCheckItemDto NormalizeItem(HomeworkCheckItemDto item, int index)
    {
        item.QuestionNo = FirstText(item.QuestionNo, (index + 1).ToString());
        item.Explanation = FirstText(item.Explanation, item.ErrorReason, "AI 已返回检查结果，但没有提供详细解析。");
        return item;
    }

    private static string? ExtractJson(string modelAnswer)
    {
        if (string.IsNullOrWhiteSpace(modelAnswer))
        {
            return null;
        }

        var cleaned = Regex.Replace(modelAnswer, "```(?:json)?|```", string.Empty, RegexOptions.IgnoreCase).Trim();
        var objectStart = cleaned.IndexOf('{');
        var objectEnd = cleaned.LastIndexOf('}');
        if (objectStart >= 0 && objectEnd > objectStart)
        {
            return cleaned[objectStart..(objectEnd + 1)];
        }

        var arrayStart = cleaned.IndexOf('[');
        var arrayEnd = cleaned.LastIndexOf(']');
        return arrayStart >= 0 && arrayEnd > arrayStart
            ? cleaned[arrayStart..(arrayEnd + 1)]
            : null;
    }

    /// <summary>
    /// 从客户端题目框摘要中提取题号，作为结构化结果的兜底骨架。
    /// </summary>
    private static IReadOnlyList<ExpectedQuestion> ExtractExpectedQuestions(string? questionText)
    {
        if (string.IsNullOrWhiteSpace(questionText))
        {
            return [];
        }

        var result = new List<ExpectedQuestion>();
        var lines = questionText.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        foreach (var line in lines)
        {
            var match = Regex.Match(line, @"第?\s*(?<no>\d+)\s*题?[：:、.)\s]*(?<text>.*)");
            if (!match.Success)
            {
                continue;
            }

            var questionNo = match.Groups["no"].Value;
            if (result.Any(item => NormalizeQuestionNo(item.QuestionNo) == questionNo))
            {
                continue;
            }

            result.Add(new ExpectedQuestion(
                questionNo,
                match.Groups["text"].Value.Trim(),
                null));
        }

        return result;
    }

    private static bool TryGetProperty(JsonElement element, out JsonElement value, params string[] names)
    {
        foreach (var name in names)
        {
            if (element.ValueKind == JsonValueKind.Object && element.TryGetProperty(name, out value))
            {
                return true;
            }
        }

        value = default;
        return false;
    }

    private static string? GetString(JsonElement element, params string[] names)
    {
        if (!TryGetProperty(element, out var value, names))
        {
            return null;
        }

        return value.ValueKind switch
        {
            JsonValueKind.String => value.GetString(),
            JsonValueKind.Number or JsonValueKind.True or JsonValueKind.False => value.ToString(),
            _ => null
        };
    }

    private static bool? GetNullableBool(JsonElement element, params string[] names)
    {
        if (!TryGetProperty(element, out var value, names))
        {
            return null;
        }

        return value.ValueKind switch
        {
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.String => ParseBoolText(value.GetString()),
            JsonValueKind.Number => value.TryGetInt32(out var number) ? number != 0 : null,
            _ => null
        };
    }

    private static float? GetFloat(JsonElement element, params string[] names)
    {
        if (!TryGetProperty(element, out var value, names))
        {
            return null;
        }

        if (value.ValueKind == JsonValueKind.Number && value.TryGetSingle(out var number))
        {
            return number;
        }

        return value.ValueKind == JsonValueKind.String && float.TryParse(value.GetString(), out number)
            ? number
            : null;
    }

    private static bool? ParseBoolText(string? value)
    {
        var text = (value ?? string.Empty).Trim().ToLowerInvariant();
        if (text is "true" or "yes" or "y" or "1" or "correct" or "right" or "对" or "正确")
        {
            return true;
        }

        if (text is "false" or "no" or "n" or "0" or "wrong" or "incorrect" or "错" or "错误")
        {
            return false;
        }

        return null;
    }

    private static string NormalizeQuestionNo(string? value)
    {
        var digits = new string((value ?? string.Empty).Where(char.IsDigit).ToArray());
        return string.IsNullOrWhiteSpace(digits) ? value ?? string.Empty : digits;
    }

    private static string FirstText(params string?[] values)
    {
        return values.FirstOrDefault(value => !string.IsNullOrWhiteSpace(value)) ?? string.Empty;
    }

    private static string? FirstNullableText(params string?[] values)
    {
        return values.FirstOrDefault(value => !string.IsNullOrWhiteSpace(value));
    }

    private static string? NullIfEmpty(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private sealed record ExpectedQuestion(string QuestionNo, string QuestionText, string? StudentAnswer);
}
