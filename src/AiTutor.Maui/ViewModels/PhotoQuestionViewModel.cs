using AiTutor.Maui.Services;

namespace AiTutor.Maui.ViewModels;

public class PhotoQuestionViewModel : ImageAskViewModelBase
{
    public PhotoQuestionViewModel(IApiClientService apiClientService, ITabletMediaPickerService mediaPickerService)
        : base(apiClientService, mediaPickerService)
    {
    }

    protected override string Mode => "explain";
    protected override string ResourceType => "question_photo";
}
