using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AAM.Inventory.Core.Application.DTOs;
using AAM.Inventory.Core.Application.Interfaces;
using AAM.Inventory.Core.Domain.Entities;
using AAM.Inventory.Core.Domain.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace AAM.Inventory.Core.Application.Services;

/// <summary>
/// Service for authentication operations
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(IUserRepository userRepository, IConfiguration configuration, ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Login attempt for username: {Username}", loginDto.Username);
        
        var user = await _userRepository.GetByUsernameAsync(loginDto.Username, cancellationToken);
        
        if (user == null || !user.IsActive)
        {
            _logger.LogWarning("Failed login attempt - user not found or inactive: {Username}", loginDto.Username);
            throw new UnauthorizedAccessException("Invalid username or password");
        }

        if (!VerifyPassword(loginDto.Password, user.PasswordHash))
        {
            _logger.LogWarning("Failed login attempt - invalid password for user: {Username}", loginDto.Username);
            throw new UnauthorizedAccessException("Invalid username or password");
        }

        // Update last login
        user.LastLoginDate = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user, cancellationToken);

        var token = GenerateJwtToken(user);

        _logger.LogInformation("User logged in successfully. Username: {Username}, Role: {Role}", user.Username, user.Role);

        return new AuthResponseDto
        {
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role,
            Token = token
        };
    }

    public Task<bool> ValidateTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtKey = _configuration["Jwt:Key"];
            if (string.IsNullOrWhiteSpace(jwtKey))
            {
                return Task.FromResult(false);
            }
            var key = Encoding.UTF8.GetBytes(jwtKey);
            
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    private string GenerateJwtToken(User user)
    {
        var jwtKey = _configuration["Jwt:Key"];
        if (string.IsNullOrWhiteSpace(jwtKey))
        {
            throw new InvalidOperationException("JWT Key is required. Please configure 'Jwt:Key' in appsettings.json or environment variables.");
        }
        var key = Encoding.UTF8.GetBytes(jwtKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            }),
            Expires = DateTime.UtcNow.AddHours(8),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private static bool VerifyPassword(string password, string hash)
    {
        // Simple password verification - in production, use proper hashing (BCrypt, Argon2, etc.)
        // For now, using SHA256 as a placeholder
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        var hashedPassword = Convert.ToBase64String(hashedBytes);
        return hashedPassword == hash;
    }

    public static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
}

