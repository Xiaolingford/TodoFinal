namespace Todofinal;

public partial class EditComplete : ContentPage
{
    private TaskItem _task;
	public EditComplete(TaskItem task = null)
	{
		InitializeComponent();
        _task = task;
        
        if (_task != null)
        {
           
        }
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