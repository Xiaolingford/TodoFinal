namespace Todofinal;

public partial class AddTaskPage : ContentPage
{
    private TaskCompletionSource<TaskItem> _taskCompletionSource;

    public Task<TaskItem> TaskAdded => _taskCompletionSource.Task;

    public AddTaskPage()
    {
        InitializeComponent();
        _taskCompletionSource = new TaskCompletionSource<TaskItem>();
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(TaskNameEntry.Text))
        {
            var newTask = new TaskItem
            {
                TaskName = TaskNameEntry.Text,
                IsCompleted = false,
                Description = string.IsNullOrWhiteSpace(OnDescriptionEntry.Text) ? null : OnDescriptionEntry.Text
            };
            _taskCompletionSource.SetResult(newTask);
            await Navigation.PopAsync();
        }
        else
        {
            _taskCompletionSource.SetResult(null);
            await Navigation.PopAsync();
        }


    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        if (!_taskCompletionSource.Task.IsCompleted)
        {
            _taskCompletionSource.SetResult(null);
        }
    }
    private async void OnBackTapped(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

   private async void OnCompletedClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CompletedTaskPage());
    }

    private async void OnProfileClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ProfilePage());
    }
}