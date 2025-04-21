namespace Todofinal;

public partial class ProfilePage : ContentPage
{
	public ProfilePage()
	{
		InitializeComponent();
	}

	private async void OnToDoClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new TodoPage());
    }

	private async void OnCompletedClicked(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new CompletedTaskPage());
	}

	private async void OnLogOut(object sender, EventArgs e)
    {
       
        await Navigation.PopToRootAsync();
    }
}