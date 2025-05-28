using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Todofinal;

public partial class TodoPage : ContentPage
{
    public ObservableCollection<TaskItem> Tasks { get; set; }
    private readonly ApiService _apiService;

    public TodoPage()
    {
        Debug.WriteLine("TodoPage constructor started");
        InitializeComponent();
        _apiService = new ApiService();
        Tasks = new ObservableCollection<TaskItem>();
        TasksCollectionView.ItemsSource = Tasks; // Assuming a CollectionView named TasksCollectionView
        BindingContext = this;
        Debug.WriteLine("TodoPage constructor completed");

        Device.BeginInvokeOnMainThread(async () => await LoadTasksAsync());
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        Debug.WriteLine("TodoPage OnAppearing called");
        Device.BeginInvokeOnMainThread(async () => await LoadTasksAsync());
    }

    private async Task LoadTasksAsync()
    {
        Debug.WriteLine("LoadTasksAsync started");
        try
        {
            string userIdString = Preferences.Get("UserId", "0");
            Debug.WriteLine($"User ID: {userIdString}");
            if (!int.TryParse(userIdString, out int userId) || userId == 0)
            {
                Debug.WriteLine("User not logged in");
                await DisplayAlert("Error", "User not logged in. Please sign in again.", "OK");
                await Navigation.PushAsync(new MainPage());
                return;
            }

            Debug.WriteLine("Calling GetTasksAsync for active tasks");
            var response = await _apiService.GetTasksAsync(userId, status: "active");
            Debug.WriteLine($"GetTasksAsync response: Status={response.Status}, Message={response.Message}, Count={response.Count}");

            if (response.Success && response.Data != null)
            {
                Debug.WriteLine("Clearing Tasks collection");
                Tasks.Clear();
                Debug.WriteLine($"Processing {response.Data.Count} tasks");
                foreach (var task in response.Data)
                {
                    Debug.WriteLine($"Adding task: ID={task.Id}, TaskName={task.TaskName}, Status={task.Status}, Description={task.Description}");
                    Tasks.Add(new TaskItem
                    {
                        Id = task.Id,
                        TaskName = task.TaskName,
                        IsCompleted = task.IsCompleted,
                        Description = task.Description
                    });
                }
                Debug.WriteLine($"Total Tasks Loaded: {Tasks.Count}");
            }
            else
            {
                Debug.WriteLine($"Failed to load active tasks: {response.Message}");
                await DisplayAlert("Error", response.Message ?? "Failed to load active tasks.", "OK");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"LoadTasksAsync exception: {ex.Message}, StackTrace: {ex.StackTrace}");
            await DisplayAlert("Error", $"Failed to load tasks: {ex.Message}", "OK");
        }
        Debug.WriteLine("LoadTasksAsync completed");
    }

    private async void OnTaskCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        Debug.WriteLine("OnTaskCheckedChanged triggered");
        if (sender is CheckBox checkBox && checkBox.BindingContext is TaskItem task)
        {
            Debug.WriteLine($"Task ID: {task.Id}, New IsCompleted: {e.Value}");
            string userIdString = Preferences.Get("UserId", "0");
            if (!int.TryParse(userIdString, out int userId) || userId == 0)
            {
                Debug.WriteLine("User not logged in");
                await DisplayAlert("Error", "User not logged in. Please sign in again.", "OK");
                await Navigation.PushAsync(new MainPage());
                return;
            }

            string newStatus = e.Value ? "inactive" : "active";
            Debug.WriteLine($"Updating task ID {task.Id} to status {newStatus}");

            var response = await _apiService.UpdateTaskStatusAsync(task.Id, newStatus);
            Debug.WriteLine($"Update Task Status Response: Status={response.Status}, Message={response.Message}");

            if (response.Success)
            {
                Debug.WriteLine($"Task update successful, removing task ID {task.Id} if inactive");
                if (newStatus == "inactive")
                {
                    Debug.WriteLine($"Tasks count before removal: {Tasks.Count}");
                    bool removed = Tasks.Remove(task);
                    Debug.WriteLine($"Task ID {task.Id} {(removed ? "removed" : "not removed")} from Tasks collection");
                    Debug.WriteLine($"Tasks count after removal: {Tasks.Count}");
                    TasksCollectionView.ItemsSource = null; // Force UI refresh
                    TasksCollectionView.ItemsSource = Tasks;
                }
            }
            else
            {
                Debug.WriteLine($"Task update failed: {response.Message}");
                checkBox.IsChecked = !e.Value;
                task.IsCompleted = !e.Value;
                await DisplayAlert("Error", response.Message ?? "Failed to update task status.", "OK");
            }
        }
        else
        {
            Debug.WriteLine("CheckBox or TaskItem binding context is null");
        }
    }

    private async void OnCompletedClicked(object sender, EventArgs e)
    {
        Debug.WriteLine("Navigating to CompletedTaskPage");
        await Navigation.PushAsync(new CompletedTaskPage());
    }

    private async void OnProfileClicked(object sender, EventArgs e)
    {
        Debug.WriteLine("Navigating to ProfilePage");
        await Navigation.PushAsync(new ProfilePage());
    }

    private async void OnAddTaskClicked(object sender, EventArgs e)
    {
        Debug.WriteLine("Navigating to AddTaskPage");
        await Navigation.PushAsync(new AddTaskPage());
    }

    private async void OnEditTaskClicked(object sender, EventArgs e)
    {
        Debug.WriteLine("Edit button clicked");
        if (sender is ImageButton button && button.CommandParameter is TaskItem task)
        {
            Debug.WriteLine($"Editing task ID: {task.Id}");
            await Navigation.PushAsync(new EditToDo(task));
        }
    }

    private async void OnDeleteTaskClicked(object sender, EventArgs e)
    {
        Debug.WriteLine("Delete button clicked");
        if (sender is ImageButton button && button.CommandParameter is TaskItem task)
        {
            Debug.WriteLine($"Deleting task ID: {task.Id}");
            bool confirm = await DisplayAlert("Confirm Delete", $"Are you sure you want to delete '{task.TaskName}'?", "Yes", "No");
            if (confirm)
            {
                await Navigation.PushAsync(new EditToDo(task));
            }
        }
    }
}