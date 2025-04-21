using System.Collections.ObjectModel;

namespace Todofinal;

public partial class CompletedTaskPage : ContentPage
{
	public CompletedTaskPage()
	{
        InitializeComponent();
        
    }

    private async void OnProfileClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ProfilePage());
    }
    private async void OnToDoClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void OnEditClickedC(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new EditComplete());
    }

    private async void OnDeleteClickedC(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new EditComplete());
    }
}

