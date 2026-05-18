using AiTutor.Maui.Services;
using Microsoft.Extensions.Logging;

namespace AiTutor.Maui.ViewModels;

public class PhotoQuestionViewModel : ImageAskViewModelBase
{
    public PhotoQuestionViewModel(
        IApiClientService apiClientService,
        ITabletMediaPickerService mediaPickerService,
        ICurrentUserService currentUserService,
        IAppSettingsService settingsService,
        IOcrService ocrService,
        IQuestionRegionBuilder questionRegionBuilder,
        IImageCropService imageCropService,
        ILogger<ImageAskViewModelBase> logger)
        : base(apiClientService, mediaPickerService, currentUserService, settingsService, ocrService, questionRegionBuilder, imageCropService, logger)
    {
    }

    protected override string Mode => "explain";
    protected override string ResourceType => "question_photo";
    protected override bool EnableQuestionRegionSelection => true;
    protected override string QuestionText => "请只讲解图片中的这道题，面向小学生，按步骤讲解，不要只给答案。";
}
