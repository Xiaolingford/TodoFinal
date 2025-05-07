using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;      // *** Added: Preferences API to fetch saved user ID

namespace Todofinal
{
    public partial class AddTaskPage : ContentPage
    {
        // *** Added: ApiService instance for calling AddTodoAsync ***
        private readonly ApiService _apiService;

        public AddTaskPage()
        {
            InitializeComponent();

            _apiService = new ApiService();
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            // *** Gather & trim inputs ***
            var name = TaskNameEntry.Text?.Trim();
            var desc = OnDescriptionEntry.Text?.Trim();

            // *** Validation: ensure name is provided ***
            if (string.IsNullOrEmpty(name))
            {
                await DisplayAlert("Validation Error", "Please enter a task name.", "OK");
                return;
            }

            // *** Retrieve the signed-in user’s ID from Preferences ***
            //    (You must save this at sign-in via: Preferences.Set("UserId", <id>))
            int userId = Preferences.Get("UserId", 0);
            if (userId == 0)
            {
                await DisplayAlert("Error", "User not signed in. Please sign in first.", "OK");
                return;
            }

            // *** Call the new AddTodo API ***
            var result = await _apiService.AddTodoAsync(name, desc, userId);

            if (result.Success)
            {
                await DisplayAlert("Success", result.message, "OK");

                // *** Optional: inform your list page of the new item ***
                MessagingCenter.Send(this, "TodoAdded", result.data);

                // Navigate back
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Error", result.message, "OK");
            }
        }

        private async void OnBackTapped(object sender, EventArgs e) =>
            await Navigation.PopAsync();

        private async void OnCompletedClicked(object sender, EventArgs e) =>
            await Navigation.PushAsync(new CompletedTaskPage());

        private async void OnProfileClicked(object sender, EventArgs e) =>
            await Navigation.PushAsync(new ProfilePage());
    }
}
