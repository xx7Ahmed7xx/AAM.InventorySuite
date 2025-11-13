using System.Text.Json;
using System.Text.Json.Serialization;

namespace Inventory.Desktop.DTOs;

public class AuthResponseDto
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    
    // API returns role as number (enum), but we want it as string
    [JsonConverter(typeof(RoleConverter))]
    public string Role { get; set; } = string.Empty; // "Cashier", "Moderator", "SuperAdmin"
    
    public string Token { get; set; } = string.Empty;
}

/// <summary>
/// Custom JSON converter to handle role as either number (enum) or string
/// </summary>
public class RoleConverter : JsonConverter<string>
{
    public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            var roleValue = reader.GetInt32();
            return roleValue switch
            {
                1 => "Cashier",
                2 => "Moderator",
                3 => "SuperAdmin",
                _ => "Cashier"
            };
        }
        else if (reader.TokenType == JsonTokenType.String)
        {
            return reader.GetString() ?? "Cashier";
        }
        else
        {
            throw new JsonException($"Unexpected token type: {reader.TokenType}");
        }
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value);
    }
}
