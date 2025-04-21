namespace Todofinal;

public partial class EditComplete : ContentPage
{
	public EditComplete()
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
        await Navigation.PushAsync(new TodoPage());
    }
    private async void OnCompletedClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}