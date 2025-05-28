using System;
using Microsoft.Maui.Controls;

namespace Todofinal
{
    public partial class EditToDo : ContentPage
    {
        private readonly TaskItem _todoItem;
        private readonly ApiService _apiService;

            public EditToDo(TaskItem todoItem)
        {
            InitializeComponent();

            _apiService = new ApiService();
            _todoItem   = todoItem;

            // populate the existing values using your TaskItem properties
            TaskNameEntry.Text      = _todoItem.TaskName;
            OnDescriptionEntry.Text = _todoItem.Description;
        }

        private async void OnBackTapped(object sender, EventArgs e)  
            => await Navigation.PopAsync();

        private async void OnProfileClicked(object sender, EventArgs e)  
            => await Navigation.PushAsync(new ProfilePage());

        private async void OnToDoClicked(object sender, EventArgs e)  
            => await Navigation.PopAsync();

        private async void OnCompletedClicked(object sender, EventArgs e)  
            => await Navigation.PushAsync(new CompletedTaskPage());

        // ??? DELETE handler ???
        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert(
                "Delete Task",
                "Are you sure you want to delete this task?",
                "Yes",
                "Cancel"
            );
            if (!confirm)
                return;

            // Call your new DELETE API
            var result = await _apiService.DeleteTodoAsync(_todoItem.item_id); 
            if (result.Success)
            {
                // If you’re still using MessagingCenter:
                MessagingCenter.Send(this, "TodoDeleted", _todoItem.item_id);

                // Or if you switched to an event on this page:
                // TaskDeleted?.Invoke(this, _todoItem.item_id);

                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Error", result.message, "OK");
            }
        }
    }
}
