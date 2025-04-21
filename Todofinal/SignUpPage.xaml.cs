namespace Todofinal;

public partial class SignUpPage : ContentPage
{
    private readonly ApiService _apiService;

    public SignUpPage()
    {
        InitializeComponent();
        _apiService = new ApiService();
    }

    private async void OnSignUpClicked(object sender, EventArgs e)
    {
        // Validate inputs
        if (string.IsNullOrWhiteSpace(FirstNameEntry.Text) ||
            string.IsNullOrWhiteSpace(LastNameEntry.Text) ||
            string.IsNullOrWhiteSpace(EmailEntry.Text) ||
            string.IsNullOrWhiteSpace(PasswordEntry.Text) ||
            string.IsNullOrWhiteSpace(ConfirmPasswordEntry.Text))
        {
            await DisplayAlert("Error", "Please fill in all fields.", "OK");
            return;
        }

        if (PasswordEntry.Text != ConfirmPasswordEntry.Text)
        {
            await DisplayAlert("Error", "Passwords do not match.", "OK");
            return;
        }

        // Basic email validation
        if (!EmailEntry.Text.Contains("@") || !EmailEntry.Text.Contains("."))
        {
            await DisplayAlert("Error", "Please enter a valid email address.", "OK");
            return;
        }

        var response = await _apiService.SignUpAsync(
            FirstNameEntry.Text,
            LastNameEntry.Text,
            EmailEntry.Text,
            PasswordEntry.Text,
            ConfirmPasswordEntry.Text
        );

        if (response.Success)
        {
            await DisplayAlert("Success", response.Message ?? "Account created successfully!", "OK");
            await Navigation.PushAsync(new MainPage());
        }
        else
        {
            await DisplayAlert("Error", response.Message ?? "Sign-up failed. Please try again.", "OK");
        }
    }

    private async void OnSignInClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MainPage());
    }

    private void OnTogglePasswordVisibility(object sender, EventArgs e)
    {
        PasswordEntry.IsPassword = !PasswordEntry.IsPassword;
        TogglePasswordButton.Text = PasswordEntry.IsPassword ? "??" : "?????";
    }

    private void OnToggleConfirmPasswordVisibility(object sender, EventArgs e)
    {
        ConfirmPasswordEntry.IsPassword = !ConfirmPasswordEntry.IsPassword;
        ToggleConfirmPasswordButton.Text = ConfirmPasswordEntry.IsPassword ? "??" : "?????";
    }
}