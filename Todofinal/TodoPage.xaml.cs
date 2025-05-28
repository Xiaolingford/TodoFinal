using System.Collections.ObjectModel;
using System.ComponentModel;


namespace Todofinal;

public partial class TodoPage : ContentPage
{
    //private readonly TaskCompletionSource<TodoItem> _taskAddedTcs = new();
    //public Task<TodoItem> TaskAdded => _taskAddedTcs.Task;

    private readonly ApiService _apiService;

    public ObservableCollection<TaskItem> Tasks { get; set; }

    public TodoPage()
    {
        InitializeComponent();
        // Sample task data (replace with your data source, e.g., database)
      
        Tasks = new ObservableCollection<TaskItem>
        {
            new TaskItem { TaskName = "My first task",                                                    IsCompleted = false },
            new TaskItem { TaskName = "Reviews daily goals before sleeping. Add some new if time permits", IsCompleted = false },
            new TaskItem { TaskName = "Stretch for 15 minutes",                                           IsCompleted = false },
            new TaskItem { TaskName = "Read a chapter of a book",                                        IsCompleted = false },
            new TaskItem { TaskName = "Water indoor plants",                                             IsCompleted = false },
            new TaskItem { TaskName = "Create icons for a dashboard",                                    IsCompleted = false },
        };

        TasksCollectionView.ItemsSource = Tasks;
        BindingContext = this;

        // ─── NEW: Listen for deletions and remove the TaskItem instance when EditToDo signals it’s gone
        MessagingCenter.Subscribe<EditToDo, TaskItem>(this, "TodoDeleted", (sender, deletedTask) =>
        {
            Tasks.Remove(deletedTask);
        });
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
        Tasks.Add(new TaskItem
        {
            TaskName = newTask.item_name,
            Description = newTask.item_description
        });
    }

    private async void OnEditTaskClicked(object sender, EventArgs e)
    {
        var button = (ImageButton)sender;
        var task = (TaskItem)button.CommandParameter;
        await Navigation.PushAsync(new EditToDo());
    }

    private async void OnDeleteTaskClicked(object sender, EventArgs e)
    {
        var button = (ImageButton)sender;
        var task = (TaskItem)button.CommandParameter;
        await Navigation.PushAsync(new EditToDo());
    }
}

public class TaskItem : INotifyPropertyChanged
{
    public int ItemId { get; set; }
    private string _taskName;
    private bool _isCompleted;
    private string _description;
    public string TaskName
    {
        get => _taskName;
        set
        {
            _taskName = value;
            OnPropertyChanged(nameof(TaskName));
        }
    }

    public bool IsCompleted
    {
        get => _isCompleted;
        set
        {
            _isCompleted = value;
            OnPropertyChanged(nameof(IsCompleted));
        }
    }

    public string Description
    {
        get => _description;
        set
        {
            _description = value;
            OnPropertyChanged(nameof(Description));
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

   
