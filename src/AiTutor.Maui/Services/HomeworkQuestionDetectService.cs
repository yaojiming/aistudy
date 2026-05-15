using AiTutor.Maui.Models;
using Microsoft.Extensions.Logging;

namespace AiTutor.Maui.Services;

/// <summary>
/// MVP 阶段使用端侧 OCR + 题号规则生成作业题目区域。后续可在同一接口下替换为视觉模型返回 bbox 的实现。
/// </summary>
public sealed class HomeworkQuestionDetectService : IHomeworkQuestionDetectService
{
    private readonly IOcrService _ocrService;
    private readonly IQuestionRegionBuilder _regionBuilder;
    private readonly IImageCropService _imageCropService;
    private readonly IAiQuestionRegionDetector _aiQuestionRegionDetector;
    private readonly ILogger<HomeworkQuestionDetectService> _logger;

    public HomeworkQuestionDetectService(
        IOcrService ocrService,
        IQuestionRegionBuilder regionBuilder,
        IImageCropService imageCropService,
        IAiQuestionRegionDetector aiQuestionRegionDetector,
        ILogger<HomeworkQuestionDetectService> logger)
    {
        _ocrService = ocrService;
        _regionBuilder = regionBuilder;
        _imageCropService = imageCropService;
        _aiQuestionRegionDetector = aiQuestionRegionDetector;
        _logger = logger;
    }

    public async Task<IReadOnlyList<HomeworkQuestionRegion>> DetectQuestionsAsync(
        string correctedImagePath,
        QuestionDetectionMode detectionMode = QuestionDetectionMode.Local,
        CancellationToken cancellationToken = default)
    {
        if (!File.Exists(correctedImagePath))
        {
            throw new FileNotFoundException("纠正后的图片文件不存在，请重新选择图片。", correctedImagePath);
        }

        _logger.LogInformation("开始识别作业题目区域。ImagePath={ImagePath}", correctedImagePath);
        var imageSize = await _imageCropService.GetImageSizeAsync(correctedImagePath);
        var result = detectionMode switch
        {
            QuestionDetectionMode.Ai => await DetectWithAiAsync(correctedImagePath, imageSize, cancellationToken),
            QuestionDetectionMode.LocalThenAi => await DetectWithLocalThenAiAsync(correctedImagePath, imageSize, cancellationToken),
            _ => await DetectWithLocalAsync(correctedImagePath)
        };

        _logger.LogInformation("作业题目区域识别完成。Mode={Mode}, Count={Count}", detectionMode, result.Count);
        foreach (var item in result)
        {
            _logger.LogInformation("题目区域。QuestionNo={QuestionNo}, BBox={BBox}, Text={Text}", item.QuestionNo, item.BBox, item.QuestionText);
        }

        return result;
    }

    private async Task<IReadOnlyList<HomeworkQuestionRegion>> DetectWithLocalThenAiAsync(
        string imagePath,
        SizeF imageSize,
        CancellationToken cancellationToken)
    {
        var local = await DetectWithLocalAsync(imagePath);
        var ai = await DetectWithAiAsync(imagePath, imageSize, cancellationToken);
        return ai.Count > 0 ? ai : local;
    }

    private async Task<IReadOnlyList<HomeworkQuestionRegion>> DetectWithAiAsync(
        string imagePath,
        SizeF imageSize,
        CancellationToken cancellationToken)
    {
        var regions = await _aiQuestionRegionDetector.DetectAsync(imagePath, imageSize.Width, imageSize.Height, cancellationToken);
        return regions
            .Select((region, index) => new HomeworkQuestionRegion(
                region.Id,
                region.Index > 0 ? region.Index.ToString() : (index + 1).ToString(),
                region.Bounds,
                string.IsNullOrWhiteSpace(region.Note) ? $"AI识别第{index + 1}题" : region.Note,
                null,
                region.Confidence))
            .ToList();
    }

    private async Task<IReadOnlyList<HomeworkQuestionRegion>> DetectWithLocalAsync(string correctedImagePath)
    {
        var imageSize = await _imageCropService.GetImageSizeAsync(correctedImagePath);
        var lines = await _ocrService.RecognizeLinesAsync(correctedImagePath);
        var regions = _regionBuilder.Build(lines, imageSize);

        return regions
            .Select((region, index) => new HomeworkQuestionRegion(
                region.Id,
                (index + 1).ToString(),
                region.Bounds,
                region.TextPreview,
                null,
                0.85))
            .ToList();
    }
}
