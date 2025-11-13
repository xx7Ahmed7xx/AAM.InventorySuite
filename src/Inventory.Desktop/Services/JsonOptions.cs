using System.Text.Json;
using Inventory.Desktop.DTOs;

namespace Inventory.Desktop.Services;

/// <summary>
/// Shared JSON serializer options for API communication
/// </summary>
public static class JsonOptions
{
    public static JsonSerializerOptions Default { get; } = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
}

