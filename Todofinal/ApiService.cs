using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;

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
    public async Task<SignUpResponse> SignUpAsync(string firstName, string lastName, string email, string password,
        string confirmPassword)
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
            var rawContent = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"Signup API URL: {BaseUrl}signup_action.php");
            System.Diagnostics.Debug.WriteLine($"Signup HTTP Status: {response.StatusCode}");
            System.Diagnostics.Debug.WriteLine($"Signup Raw Response: {rawContent}");

            if (!response.IsSuccessStatusCode)
            {
                return new SignUpResponse { Status = (int)response.StatusCode, Message = $"HTTP Error: {rawContent}" };
            }

            SignUpResponse result;
            try
            {
                result = JsonSerializer.Deserialize<SignUpResponse>(rawContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (JsonException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Signup JSON Error: {ex.Message}");
                return new SignUpResponse { Status = 500, Message = $"JSON Error: {ex.Message}" };
            }

            return result ?? new SignUpResponse { Status = (int)response.StatusCode, Message = "No response data" };
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Signup Exception: {ex.Message}");
            return new SignUpResponse { Status = 500, Message = $"Error: {ex.Message}" };
        }
    }

    // Sign-In: GET /signin_action.php?email={email}&password={password}
    public async Task<SignInResponse> SignInAsync(string email, string password)
    {
        try
        {
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

    // Get Tasks: GET /getItems_action.php?status={status}&user_id={user_id}
    public async Task<GetTasksResponse> GetTasksAsync(int userId, string status = "active")
    {
        try
        {
            var query = $"?status={Uri.EscapeDataString(status)}&user_id={Uri.EscapeDataString(userId.ToString())}";
            var response = await _httpClient.GetAsync($"getItems_action.php{query}");
            var rawContent = await response.Content.ReadAsStringAsync();
            
            System.Diagnostics.Debug.WriteLine($"API URL: {BaseUrl}getItems_action.php{query}");
            System.Diagnostics.Debug.WriteLine($"HTTP Status: {response.StatusCode}");
            System.Diagnostics.Debug.WriteLine($"Raw Response: {rawContent}");

            if (!response.IsSuccessStatusCode)
            {
                System.Diagnostics.Debug.WriteLine($"HTTP Error Details: {rawContent}");
                return new GetTasksResponse { Status = (int)response.StatusCode, Message = $"HTTP Error: {response.StatusCode}" };
            }

            if (string.IsNullOrWhiteSpace(rawContent))
            {
                return new GetTasksResponse { Status = 400, Message = "Empty API response" };
            }

            // Extract JSON by removing HTML warnings
            string jsonContent = rawContent;
            int jsonStartIndex = rawContent.IndexOf("{");
            if (jsonStartIndex >= 0)
            {
                jsonContent = rawContent.Substring(jsonStartIndex);
                System.Diagnostics.Debug.WriteLine($"Cleaned JSON: {jsonContent}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("No valid JSON found in response");
                return new GetTasksResponse { Status = 400, Message = "No valid JSON in response" };
            }

            // Parse JSON with JsonDocument to handle data object
            JsonDocument doc;
            try
            {
                doc = JsonDocument.Parse(jsonContent);
            }
            catch (JsonException ex)
            {
                System.Diagnostics.Debug.WriteLine($"JSON Parse Error: {ex.Message}");
                return new GetTasksResponse { Status = 500, Message = $"JSON Parse Error: {ex.Message}" };
            }

            var result = new GetTasksResponse { Data = new List<TaskResponse>() };
            using (doc)
            {
                var root = doc.RootElement;
                if (root.TryGetProperty("status", out var statusProp))
                    result.Status = statusProp.GetInt32();
                if (root.TryGetProperty("message", out var message))
                    result.Message = message.GetString();
                if (root.TryGetProperty("count", out var count))
                {
                    // Handle count as string or int
                    if (count.ValueKind == JsonValueKind.String && int.TryParse(count.GetString(), out int countValue))
                        result.Count = countValue;
                    else if (count.ValueKind == JsonValueKind.Number)
                        result.Count = count.GetInt32();
                }
                if (root.TryGetProperty("data", out var data))
                {
                    if (data.ValueKind == JsonValueKind.Object)
                    {
                        foreach (var prop in data.EnumerateObject())
                        {
                            var task = JsonSerializer.Deserialize<TaskResponse>(prop.Value.GetRawText(), new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            });
                            if (task != null)
                                result.Data.Add(task);
                        }
                    }
                }
            }

            System.Diagnostics.Debug.WriteLine($"Parsed Response: Status={result.Status}, Count={result.Count}, DataCount={result.Data.Count}");
            return result;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
            return new GetTasksResponse { Status = 500, Message = $"Error: {ex.Message}" };
        }
    }
    
    // Add Task: POST /addItem_action.php
    public async Task<AddTaskResponse> AddTaskAsync(int userId, string taskName, string description)
    {
        try
        {
            var taskData = new
            {
                user_id = userId,
                item_name = taskName,
                item_description = description
            };
            var response = await _httpClient.PostAsJsonAsync("addItem_action.php", taskData);
            var rawContent = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"Add Task API URL: {BaseUrl}addItem_action.php");
            System.Diagnostics.Debug.WriteLine($"Add Task HTTP Status: {response.StatusCode}");
            System.Diagnostics.Debug.WriteLine($"Add Task Response: {rawContent}");

            if (!response.IsSuccessStatusCode)
            {
                return new AddTaskResponse { Status = (int)response.StatusCode, Message = $"HTTP Error: {rawContent}" };
            }

            AddTaskResponse result;
            try
            {
                result = JsonSerializer.Deserialize<AddTaskResponse>(rawContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (JsonException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Add Task JSON Error: {ex.Message}");
                return new AddTaskResponse { Status = 500, Message = $"JSON Error: {ex.Message}" };
            }

            return result ?? new AddTaskResponse { Status = (int)response.StatusCode, Message = "No response data" };
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Add Task Exception: {ex.Message}");
            return new AddTaskResponse { Status = 500, Message = $"Error: {ex.Message}" };
        }
    }
    
    public async Task<UpdateTaskStatusResponse> UpdateTaskStatusAsync(int itemId, string status)
    {
        try
        {
            var requestData = new
            {
                item_id = itemId,
                status = status // "active" or "inactive"
            };

            var response = await _httpClient.PutAsJsonAsync("statusItem_action.php", requestData);
            var rawContent = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"Update Task Status API URL: {BaseUrl}statusItem_action.php");
            System.Diagnostics.Debug.WriteLine($"Update Task Status HTTP Status: {response.StatusCode}");
            System.Diagnostics.Debug.WriteLine($"Update Task Status Response: {rawContent}");

            if (!response.IsSuccessStatusCode)
            {
                return new UpdateTaskStatusResponse { Status = (int)response.StatusCode, Message = $"HTTP Error: {rawContent}" };
            }

            UpdateTaskStatusResponse result;
            try
            {
                result = JsonSerializer.Deserialize<UpdateTaskStatusResponse>(rawContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (JsonException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Update Task Status JSON Error: {ex.Message}");
                return new UpdateTaskStatusResponse { Status = 500, Message = $"JSON Error: {ex.Message}" };
            }

            return result ?? new UpdateTaskStatusResponse { Status = (int)response.StatusCode, Message = "No response data" };
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Update Task Status Exception: {ex.Message}");
            return new UpdateTaskStatusResponse { Status = 500, Message = $"Error: {ex.Message}" };
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

public class GetTasksResponse
{
    [JsonPropertyName("status")]
    public int Status { get; set; }

    [JsonPropertyName("data")]
    public List<TaskResponse> Data { get; set; } = new List<TaskResponse>();

    [JsonPropertyName("message")]
    public string Message { get; set; }

    [JsonPropertyName("count")]
    public int Count { get; set; }

    public bool Success => Status == 200;
}

public class TaskResponse
{
    [JsonPropertyName("item_id")]
    public int Id { get; set; }

    [JsonPropertyName("item_name")]
    public string TaskName { get; set; }

    [JsonPropertyName("item_description")]
    public string Description { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } // "active" or "inactive"

    [JsonPropertyName("user_id")]
    public int UserId { get; set; }

    [JsonPropertyName("timemodified")]
    public string Timemodified { get; set; }

    // Map status to isCompleted for UI
    public bool IsCompleted => Status == "inactive";
}
public class AddTaskResponse
{
    [JsonPropertyName("status")]
    public int Status { get; set; }

    [JsonPropertyName("data")]
    public TaskResponse Data { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; }

    public bool Success => Status == 200;
}

public class UpdateTaskStatusResponse
{
    [JsonPropertyName("status")]
    public int Status { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; }

    public bool Success => Status == 200;
}