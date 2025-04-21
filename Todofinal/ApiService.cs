using System.Net.Http.Json;
using System.Text.Json;

namespace Todofinal;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://todo-list.dcism.org/";

    public ApiService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(BaseUrl)
        };
    }

    // Sign-Up: POST /signup_action.php
    public async Task<SignUpResponse> SignUpAsync(string firstName, string lastName, string email, string password, string confirmPassword)
    {
        try
        {
            var userData = new
            {
                first_name = firstName,
                last_name = lastName,
                email,
                password,
                confirm_password = confirmPassword
            };
            var response = await _httpClient.PostAsJsonAsync("signup_action.php", userData);
            var result = await response.Content.ReadFromJsonAsync<SignUpResponse>();
            return result ?? new SignUpResponse { Status = (int)response.StatusCode };
        }
        catch (Exception ex)
        {
            return new SignUpResponse { Status = 500, Message = $"Error: {ex.Message}" };
        }
    }

    // Sign-In: GET /signin_action.php?email={email}&password={password}
    public async Task<SignInResponse> SignInAsync(string email, string password)
    {
        try
        {
            // Encode query parameters to handle special characters
            var query = $"?email={Uri.EscapeDataString(email)}&password={Uri.EscapeDataString(password)}";
            var response = await _httpClient.GetAsync($"signin_action.php{query}");
            var result = await response.Content.ReadFromJsonAsync<SignInResponse>();
            return result ?? new SignInResponse { Status = (int)response.StatusCode };
        }
        catch (Exception ex)
        {
            return new SignInResponse { Status = 500, Message = $"Error: {ex.Message}" };
        }
    }
}

public class SignUpResponse
{
    public int Status { get; set; }
    public string Message { get; set; }
    public bool Success => Status == 200;
}

public class SignInResponse
{
    public int Status { get; set; }
    public SignInData Data { get; set; }
    public string Message { get; set; }
    public bool Success => Status == 200;
}

public class SignInData
{
    public int Id { get; set; }
    public string Fname { get; set; }
    public string Lname { get; set; }
    public string Email { get; set; }
    public string Timemodified { get; set; }
}