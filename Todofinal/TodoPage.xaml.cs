using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Todofinal;

public partial class TodoPage : ContentPage
{
    public ObservableCollection<TaskItem> Tasks { get; set; }
    private readonly ApiService _apiService;

    public TodoPage()
    {
        InitializeComponent();
        _apiService = new ApiService();
        Tasks = new ObservableCollection<TaskItem>();
        TasksCollectionView.ItemsSource = Tasks;
        BindingContext = this;
        
        LoadTasksAsync();
    }
    
    private async void LoadTasksAsync()
    {
        string userIdString = Preferences.Get("UserId", "0");
        System.Diagnostics.Debug.WriteLine($"User ID: {userIdString}");
        if (!int.TryParse(userIdString, out int userId) || userId == 0)
        {
            await DisplayAlert("Error", "User not logged in. Please sign in again.", "OK");
            await Navigation.PushAsync(new MainPage());
            return;
        }

        var response = await _apiService.GetTasksAsync(userId, status: "active");
        System.Diagnostics.Debug.WriteLine($"Response Status: {response.Status}, Message: {response.Message}, Count: {response.Count}");
        if (response.Success && response.Data != null)
        {
            Tasks.Clear();
            foreach (var task in response.Data)
            {
                System.Diagnostics.Debug.WriteLine($"Task: ID={task.Id}, TaskName={task.TaskName}, Status={task.Status}, Description={task.Description}");
                Tasks.Add(new TaskItem
                {
                    Id = task.Id,
                    TaskName = task.TaskName,
                    IsCompleted = task.IsCompleted,
                    Description = task.Description
                });
            }
            System.Diagnostics.Debug.WriteLine($"Total Tasks Loaded: {Tasks.Count}");
        }
        else
        {
            System.Diagnostics.Debug.WriteLine($"Failed to load tasks: {response.Message}");
            await DisplayAlert("Error", response.Message ?? "Failed to load tasks.", "OK");
        }
    }

    private async void OnCompletedClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CompletedTaskPage());
    }
    
    private async void OnProfileClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ProfilePage());
    }

    private async void OnAddTaskClicked(object sender, EventArgs e)
    {
        var addTaskPage = new AddTaskPage();
        await Navigation.PushAsync(addTaskPage);
        var newTask = await addTaskPage.TaskAdded;
        if (newTask != null)
        {
            Tasks.Add(newTask);
        }
    }

    private async void OnEditTaskClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new EditToDo());
    }

    private async void OnDeleteTaskClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new EditToDo());
    }
}