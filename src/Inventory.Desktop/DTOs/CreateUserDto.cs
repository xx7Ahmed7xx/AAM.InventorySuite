using System.Text.Json;
using System.Text.Json.Serialization;

namespace Inventory.Desktop.DTOs;

public class CreateUserDto
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    
    // API expects Role as enum (number), but we work with string internally
    // This converter will serialize the string as the corresponding enum number
    [JsonConverter(typeof(RoleToEnumConverter))]
    public string Role { get; set; } = "Cashier";
    
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Converter that serializes string role ("Cashier", "Moderator", "SuperAdmin") as enum number (1, 2, 3)
/// </summary>
public class RoleToEnumConverter : JsonConverter<string>
{
    public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // When reading, convert enum number back to string
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
        return reader.GetString() ?? "Cashier";
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        // When writing, convert string to enum number
        var roleNumber = value switch
        {
            "Cashier" => 1,
            "Moderator" => 2,
            "SuperAdmin" => 3,
            _ => 1
        };
        writer.WriteNumberValue(roleNumber);
    }
}
