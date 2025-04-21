namespace Todofinal;

public partial class EditToDo : ContentPage
{
	public EditToDo()
	{
		InitializeComponent();
	}

    private async void OnBackTapped(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
    private async void OnProfileClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ProfilePage());
    }
    private async void OnToDoClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
    private async void OnCompletedClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CompletedTaskPage());
    }
}