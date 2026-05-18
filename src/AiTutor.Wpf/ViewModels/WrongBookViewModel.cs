using System.Collections.ObjectModel;
using AiTutor.Wpf.Infrastructure;
using AiTutor.Wpf.Models;

namespace AiTutor.Wpf.ViewModels;

public sealed class WrongBookViewModel : ViewModelBase
{
    private WrongQuestionItem? _selectedItem;

    public WrongBookViewModel()
    {
        Items =
        [
            new WrongQuestionItem
            {
                Title = "分数单位与数量",
                Subject = "数学",
                KnowledgePoint = "分数的意义",
                Reason = "把分数单位和分子数量混在一起了。",
                NextReviewTime = "明天"
            },
            new WrongQuestionItem
            {
                Title = "体积单位换算",
                Subject = "数学",
                KnowledgePoint = "体积与容积单位",
                Reason = "立方米到立方分米的进率记错。",
                NextReviewTime = "3 天后"
            }
        ];

        SelectedItem = Items.FirstOrDefault();
    }

    public ObservableCollection<WrongQuestionItem> Items { get; }

    public WrongQuestionItem? SelectedItem
    {
        get => _selectedItem;
        set => SetProperty(ref _selectedItem, value);
    }
}
