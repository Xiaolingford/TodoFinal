using System.Threading.Tasks;

namespace Todofinal;

public partial class AddTaskPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly TaskCompletionSource<TaskItem> _taskCompletionSource;

    public Task<TaskItem> TaskAdded => _taskCompletionSource.Task;

    public AddTaskPage()
    {
        InitializeComponent();
        _apiService = new ApiService();
        _taskCompletionSource = new TaskCompletionSource<TaskItem>();
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TaskNameEntry.Text))
        {
            await DisplayAlert("Error", "Task name is required", "OK");
            _taskCompletionSource.SetResult(null);
            return;
        }

        string userIdString = Preferences.Get("UserId", "0");
        if (!int.TryParse(userIdString, out int userId))
        {
            await DisplayAlert("Error", "User not logged in", "OK");
            _taskCompletionSource.SetResult(null);
            await Navigation.PopAsync();
            return;
        }

        string taskName = TaskNameEntry.Text;
        string description = string.IsNullOrWhiteSpace(OnDescriptionEntry.Text) ? "" : OnDescriptionEntry.Text;

        var response = await _apiService.AddTaskAsync(userId, taskName, description);
        System.Diagnostics.Debug.WriteLine($"Add Task Result: Status={response.Status}, Message={response.Message}");
        if (response.Success && response.Data != null)
        {
            var newTask = new TaskItem
            {
                Id = response.Data.Id,
                TaskName = response.Data.TaskName,
                Description = response.Data.Description,
                IsCompleted = response.Data.IsCompleted
            };
            _taskCompletionSource.SetResult(newTask);
            await Navigation.PopAsync();
        }
        else
        {
            await DisplayAlert("Error", response.Message ?? "Failed to add task", "OK");
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