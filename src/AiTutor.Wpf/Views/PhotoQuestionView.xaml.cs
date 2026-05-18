using System.Windows.Controls;
using System.ComponentModel;

namespace AiTutor.Wpf.Views;

public partial class PhotoQuestionView : UserControl
{
    public PhotoQuestionView()
    {
        InitializeComponent();
        DataContextChanged += (_, _) =>
        {
            if (DataContext is INotifyPropertyChanged notify)
            {
                notify.PropertyChanged += (_, args) =>
                {
                    if (args.PropertyName == "AnswerText")
                    {
                        AnswerTextBox.ScrollToEnd();
                    }
                };
            }
        };
    }
}
