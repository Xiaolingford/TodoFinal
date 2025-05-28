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

    // *** Added: Add ToDo Task API method ***
    /// <summary>
    /// Calls POST /addItem_action.php to create a new ToDo.
    /// </summary>
    public async Task<AddTodoResponse> AddTodoAsync(string itemName, string itemDescription, int userId)
    {
        try
        {
            var todoData = new
            {
                item_name = itemName,
                item_description = itemDescription,
                user_id = userId
            };
            var response = await _httpClient.PostAsJsonAsync("addItem_action.php", todoData);
            var result = await response.Content.ReadFromJsonAsync<AddTodoResponse>();

            // in case the server returns no body
            return result
                 ?? new AddTodoResponse
                 {
                     status = (int)response.StatusCode,
                     message = "Empty response from server."
                 };
        }
        catch (Exception ex)
        {
            return new AddTodoResponse
            {
                status = 500,
                message = $"Error: {ex.Message}"
            };
        }
    }


    // Delete Todo:
    public async Task<DeleteResponse> DeleteTodoAsync(int itemId)
    {
        try
        {
            // Call DELETE endpoint
            var response = await _httpClient.DeleteAsync($"deleteItem_action.php?item_id={itemId}");
            var result = await response.Content.ReadFromJsonAsync<DeleteResponse>();

            // If server returned no body, synthesize one from status code
            return result ?? new DeleteResponse
            {
                status = (int)response.StatusCode,
                message = response.ReasonPhrase
            };
        }
        catch (Exception ex)
        {
            return new DeleteResponse
            {
                status = 500,
                message = $"Error: {ex.Message}"
            };
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

// *** Added: Models for Add ToDo API response ***
public class AddTodoResponse
{
    public int status { get; set; }
    public TodoItem data { get; set; }
    public string message { get; set; }

    /// <summary>
    /// Convenience flag for status == 200
    /// </summary>
    public bool Success => status == 200;
}

public class DeleteResponse
{
    public int status { get; set; }
    public string message { get; set; }
    public bool Success => status == 200;
}

public class TodoItem
{
    public int item_id { get; set; }
    public string item_name { get; set; }
    public string item_description { get; set; }
    public string status { get; set; }
    public int user_id { get; set; }
    public string timemodified { get; set; }
}