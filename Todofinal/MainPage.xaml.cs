using System.Windows.Input;

namespace Todofinal;

public partial class MainPage : ContentPage
{
    private readonly ApiService _apiService;

    public ICommand ForgotPasswordCommand { get; }

    public MainPage()
    {
        InitializeComponent();
        _apiService = new ApiService();
        ForgotPasswordCommand = new Command(OnForgotPassword);
        BindingContext = this;

        // Load saved credentials if "Remember Me" was checked
        if (Preferences.Get("RememberMe", false))
        {
            EmailEntry.Text = Preferences.Get("SavedEmail", "");
            PasswordEntry.Text = Preferences.Get("SavedPassword", "");
            RememberMeCheckBox.IsChecked = true;
        }
    }

    private async void OnSignIn(object sender, EventArgs e)
    {
        // Validate inputs
        if (string.IsNullOrWhiteSpace(EmailEntry.Text) || string.IsNullOrWhiteSpace(PasswordEntry.Text))
        {
            await DisplayAlert("Error", "Please enter both email and password.", "OK");
            return;
        }

        // Basic email validation
        if (!EmailEntry.Text.Contains("@") || !EmailEntry.Text.Contains("."))
        {
            await DisplayAlert("Error", "Please enter a valid email address.", "OK");
            return;
        }

        var response = await _apiService.SignInAsync(EmailEntry.Text, PasswordEntry.Text);
        if (response.Success)
        {
            // Store user data
            Preferences.Set("UserId", response.Data.Id.ToString());
            Preferences.Set("UserFirstName", response.Data.Fname);
            Preferences.Set("UserLastName", response.Data.Lname);
            Preferences.Set("UserEmail", response.Data.Email);
            Preferences.Set("UserTimeModified", response.Data.Timemodified);

            // Handle "Remember Me"
            if (RememberMeCheckBox.IsChecked)
            {
                Preferences.Set("RememberMe", true);
                Preferences.Set("SavedEmail", EmailEntry.Text);
                Preferences.Set("SavedPassword", PasswordEntry.Text);
            }
            else
            {
                Preferences.Set("RememberMe", false);
                Preferences.Remove("SavedEmail");
                Preferences.Remove("SavedPassword");
            }

            await Navigation.PushAsync(new TodoPage());
        }
        else
        {
            await DisplayAlert("Error", response.Message ?? "Invalid email or password.", "OK");
        }
    }

    private async void OnSignUp(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SignUpPage());
    }

    private void OnTogglePasswordVisibility(object sender, EventArgs e)
    {
        PasswordEntry.IsPassword = !PasswordEntry.IsPassword;
        TogglePasswordButton.Text = PasswordEntry.IsPassword ? "👁" : "👁‍🗨";
    }

    private async void OnForgotPassword()
    {
        // Placeholder for forgot password functionality
        await DisplayAlert("Info", "Forgot Password functionality is not yet implemented.", "OK");
    }
}