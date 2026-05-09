namespace AiTutor.Maui;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute("chat", typeof(Views.ChatPage));
		Routing.RegisterRoute("photo-question", typeof(Views.PhotoQuestionPage));
		Routing.RegisterRoute("homework-check", typeof(Views.HomeworkCheckPage));
		Routing.RegisterRoute("wrong-book", typeof(Views.WrongBookPage));
		Routing.RegisterRoute("wrong-question-detail", typeof(Views.WrongQuestionDetailPage));
	}
}
