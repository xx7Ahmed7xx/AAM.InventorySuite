using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Inventory.Desktop.DTOs;

namespace Inventory.Desktop.Services;

/// <summary>
/// HTTP client service for communicating with the Inventory API
/// </summary>
public class ApiClient
{
    private readonly HttpClient _httpClient;
    private readonly string _apiUrl;

    public ApiClient(string apiUrl, string? authToken = null)
    {
        _apiUrl = apiUrl.TrimEnd('/');
        _httpClient = new HttpClient();
        
        if (!string.IsNullOrEmpty(authToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);
        }
    }

    public void SetAuthToken(string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }

    // Products
    public async Task<List<ProductDto>> GetProductsAsync()
    {
        var response = await _httpClient.GetAsync($"{_apiUrl}/products");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<ProductDto>>() ?? new List<ProductDto>();
    }

    public async Task<PagedResultDto<ProductDto>> GetProductsPagedAsync(int pageNumber, int pageSize)
    {
        var response = await _httpClient.GetAsync($"{_apiUrl}/products?pageNumber={pageNumber}&pageSize={pageSize}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PagedResultDto<ProductDto>>() 
            ?? throw new InvalidOperationException("Failed to deserialize paged products");
    }

    public async Task<ProductDto?> GetProductAsync(int id)
    {
        var response = await _httpClient.GetAsync($"{_apiUrl}/products/{id}");
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ProductDto>();
    }

    public async Task<List<ProductDto>> SearchProductsAsync(string searchTerm)
    {
        var response = await _httpClient.GetAsync($"{_apiUrl}/products/search?term={Uri.EscapeDataString(searchTerm)}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<ProductDto>>() ?? new List<ProductDto>();
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync($"{_apiUrl}/products", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ProductDto>() 
            ?? throw new InvalidOperationException("Failed to deserialize product");
    }

    public async Task<ProductDto> UpdateProductAsync(UpdateProductDto dto)
    {
        var response = await _httpClient.PutAsJsonAsync($"{_apiUrl}/products/{dto.Id}", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ProductDto>() 
            ?? throw new InvalidOperationException("Failed to deserialize product");
    }

    public async Task DeleteProductAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"{_apiUrl}/products/{id}");
        response.EnsureSuccessStatusCode();
    }

    // Categories
    public async Task<List<CategoryDto>> GetCategoriesAsync()
    {
        var response = await _httpClient.GetAsync($"{_apiUrl}/categories");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<CategoryDto>>() ?? new List<CategoryDto>();
    }

    public async Task<CategoryDto?> GetCategoryAsync(int id)
    {
        var response = await _httpClient.GetAsync($"{_apiUrl}/categories/{id}");
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CategoryDto>();
    }

    public async Task<CategoryDto> CreateCategoryAsync(CategoryDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync($"{_apiUrl}/categories", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CategoryDto>() 
            ?? throw new InvalidOperationException("Failed to deserialize category");
    }

    public async Task<CategoryDto> UpdateCategoryAsync(CategoryDto dto)
    {
        var response = await _httpClient.PutAsJsonAsync($"{_apiUrl}/categories/{dto.Id}", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CategoryDto>() 
            ?? throw new InvalidOperationException("Failed to deserialize category");
    }

    public async Task DeleteCategoryAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"{_apiUrl}/categories/{id}");
        response.EnsureSuccessStatusCode();
    }

    // Stock Movements
    public async Task<List<StockMovementDto>> GetStockMovementsAsync()
    {
        var response = await _httpClient.GetAsync($"{_apiUrl}/stockmovements");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<StockMovementDto>>() ?? new List<StockMovementDto>();
    }

    public async Task<PagedResultDto<StockMovementDto>> GetStockMovementsPagedAsync(int pageNumber, int pageSize)
    {
        var response = await _httpClient.GetAsync($"{_apiUrl}/stockmovements?pageNumber={pageNumber}&pageSize={pageSize}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PagedResultDto<StockMovementDto>>() 
            ?? throw new InvalidOperationException("Failed to deserialize paged stock movements");
    }

    public async Task<StockMovementDto> AddStockAsync(StockMovementRequestDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync($"{_apiUrl}/stockmovements/add", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<StockMovementDto>() 
            ?? throw new InvalidOperationException("Failed to deserialize stock movement");
    }

    public async Task<StockMovementDto> RemoveStockAsync(StockMovementRequestDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync($"{_apiUrl}/stockmovements/remove", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<StockMovementDto>() 
            ?? throw new InvalidOperationException("Failed to deserialize stock movement");
    }

    public async Task<StockMovementDto> AdjustStockAsync(StockMovementRequestDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync($"{_apiUrl}/stockmovements/adjust", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<StockMovementDto>() 
            ?? throw new InvalidOperationException("Failed to deserialize stock movement");
    }

    // Reports
    public async Task<List<ProductDto>> GetStockReportAsync()
    {
        var response = await _httpClient.GetAsync($"{_apiUrl}/reports/stock");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<ProductDto>>() ?? new List<ProductDto>();
    }

    public async Task<PagedResultDto<ProductDto>> GetStockReportPagedAsync(int pageNumber, int pageSize)
    {
        var response = await _httpClient.GetAsync($"{_apiUrl}/reports/stock?pageNumber={pageNumber}&pageSize={pageSize}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PagedResultDto<ProductDto>>() 
            ?? throw new InvalidOperationException("Failed to deserialize paged stock report");
    }

    public async Task<List<ProductDto>> GetLowStockReportAsync()
    {
        var response = await _httpClient.GetAsync($"{_apiUrl}/reports/low-stock");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<ProductDto>>() ?? new List<ProductDto>();
    }

    public async Task<PagedResultDto<ProductDto>> GetLowStockReportPagedAsync(int pageNumber, int pageSize)
    {
        var response = await _httpClient.GetAsync($"{_apiUrl}/reports/low-stock?pageNumber={pageNumber}&pageSize={pageSize}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PagedResultDto<ProductDto>>() 
            ?? throw new InvalidOperationException("Failed to deserialize paged low stock report");
    }

    public async Task<List<StockMovementDto>> GetMovementReportAsync(string? startDate = null, string? endDate = null)
    {
        var url = $"{_apiUrl}/reports/movements";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(startDate))
            queryParams.Add($"startDate={Uri.EscapeDataString(startDate)}");
        if (!string.IsNullOrEmpty(endDate))
            queryParams.Add($"endDate={Uri.EscapeDataString(endDate)}");
        if (queryParams.Count > 0)
            url += "?" + string.Join("&", queryParams);

        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<StockMovementDto>>() ?? new List<StockMovementDto>();
    }

    // Users
    public async Task<List<UserDto>> GetUsersAsync()
    {
        var response = await _httpClient.GetAsync($"{_apiUrl}/users");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<UserDto>>() ?? new List<UserDto>();
    }

    public async Task<PagedResultDto<UserDto>> GetUsersPagedAsync(int pageNumber, int pageSize)
    {
        var response = await _httpClient.GetAsync($"{_apiUrl}/users?pageNumber={pageNumber}&pageSize={pageSize}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PagedResultDto<UserDto>>() 
            ?? throw new InvalidOperationException("Failed to deserialize paged users");
    }

    public async Task<UserDto?> GetUserAsync(int id)
    {
        var response = await _httpClient.GetAsync($"{_apiUrl}/users/{id}");
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<UserDto>();
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto dto)
    {
        // Use JsonSerializer to ensure the [JsonConverter] attribute on Role property is respected
        // The RoleToEnumConverter attribute will automatically convert string to enum number
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            // The [JsonConverter] attribute on Role property will be automatically used
        };
        
        var json = JsonSerializer.Serialize(dto, jsonOptions);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync($"{_apiUrl}/users", content);
        
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Request failed with status {response.StatusCode}: {errorContent}");
        }
        
        // Use JsonOptions.Default which includes RoleConverter for deserializing UserDto
        var responseJson = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<UserDto>(responseJson, JsonOptions.Default) 
            ?? throw new InvalidOperationException("Failed to deserialize user");
    }

    public async Task<UserDto> UpdateUserAsync(UpdateUserDto dto)
    {
        // Use JsonSerializer to ensure the [JsonConverter] attribute on Role property is respected
        // The RoleToEnumConverter attribute will automatically convert string to enum number
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            // The [JsonConverter] attribute on Role property will be automatically used
        };
        
        var json = JsonSerializer.Serialize(dto, jsonOptions);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync($"{_apiUrl}/users/{dto.Id}", content);
        
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Request failed with status {response.StatusCode}: {errorContent}");
        }
        
        // Use JsonOptions.Default which includes RoleConverter for deserializing UserDto
        var responseJson = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<UserDto>(responseJson, JsonOptions.Default) 
            ?? throw new InvalidOperationException("Failed to deserialize user");
    }

    public async Task DeleteUserAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"{_apiUrl}/users/{id}");
        response.EnsureSuccessStatusCode();
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}

