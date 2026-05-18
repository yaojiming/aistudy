using System.Windows.Controls;
using System.Collections.Specialized;

namespace AiTutor.Wpf.Views;

public partial class ChatView : UserControl
{
    public ChatView()
    {
        InitializeComponent();
        Loaded += (_, _) =>
        {
            if (MessagesList.ItemsSource is INotifyCollectionChanged collection)
            {
                collection.CollectionChanged += (_, _) => ScrollToLastMessage();
            }
        };
    }

    private void ScrollToLastMessage()
    {
        if (MessagesList.Items.Count == 0)
        {
            return;
        }

        MessagesList.ScrollIntoView(MessagesList.Items[^1]);
    }
}
