# Security Policy

## Supported Versions

| Version | Supported          |
| ------- | ------------------ |
| 1.0.x   | :white_check_mark: |

## Security Features

The Inventory Suite includes the following security features:

- ✅ **JWT Authentication**: Secure token-based authentication
- ✅ **Role-Based Access Control**: Granular permissions per role
- ✅ **Input Validation**: FluentValidation for all API inputs
- ✅ **Structured Logging**: Serilog for security event tracking
- ✅ **Password Hashing**: SHA256 hashing (upgrade to BCrypt recommended for production)
- ✅ **HTTPS Support**: Enforced in production environments
- ✅ **CORS Configuration**: Restricted to allowed origins

## Reporting a Vulnerability

If you discover a security vulnerability, please **do not** open a public issue. Instead, please email the repository maintainers or create a private security advisory on GitHub.

When reporting, please include:
- Description of the vulnerability
- Steps to reproduce
- Potential impact
- Suggested fix (if any)

We will respond within 48 hours and work with you to address the issue before making it public.

## Security Best Practices

When deploying this software in production, follow these essential security practices:

### 1. Authentication & Authorization

- **Change Default Credentials**: Immediately change the default admin password (`admin`/`admin123`) after first login
- **Use Strong JWT Keys**: Generate a secure random key for JWT tokens:
  ```bash
  # Generate a secure 64-character key
  openssl rand -base64 48
  ```
  Update `Jwt:Key` in `appsettings.json` or set via environment variable
- **Token Expiration**: Configure appropriate JWT token expiration times (default: 24 hours)

### 2. Configuration Security

- **Environment Variables**: Store sensitive configuration in environment variables:
  ```bash
  # Set JWT key via environment variable
  export JWT__Key="your-secure-key-here"
  ```
- **Never Commit Secrets**: Ensure `appsettings.json` with real secrets is in `.gitignore`
- **Use `appsettings.Example.json`**: Provide example configuration files without real secrets

### 3. Network Security

- **Use HTTPS**: Always use HTTPS in production environments
- **CORS Configuration**: Restrict CORS to specific origins in production:
  ```json
  "CORS": {
    "AllowedOrigins": ["https://yourdomain.com"]
  }
  ```
- **Firewall Rules**: Restrict database access to application servers only

### 4. Database Security

- **Strong Passwords**: Use strong database passwords (if using SQL Server)
- **Connection Encryption**: Enable encrypted connections to the database
- **Backup Security**: Secure database backups with encryption
- **SQLite Files**: Ensure SQLite database files have proper file system permissions

### 5. Application Security

- **Keep Dependencies Updated**: Regularly update NuGet packages and npm packages:
  ```bash
  dotnet list package --outdated
  npm outdated
  ```
- **Input Validation**: All API endpoints use FluentValidation - ensure custom validators are comprehensive
- **Logging**: Monitor Serilog logs for security events (failed logins, unauthorized access attempts)
- **Error Handling**: Avoid exposing sensitive information in error messages

### 6. Password Security

- **Current Implementation**: Uses SHA256 hashing
- **Production Recommendation**: Upgrade to BCrypt or Argon2 for better security:
  ```csharp
  // Recommended: Use BCrypt.Net-Next
  string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
  bool isValid = BCrypt.Net.BCrypt.Verify(password, hashedPassword);
  ```

## Known Security Considerations

### Current Limitations

1. **Password Hashing**: Currently uses SHA256. For production use, upgrade to BCrypt or Argon2
2. **JWT Secret**: Must be changed from default value in production (application will fail to start if not set)
3. **Default Credentials**: Default admin credentials (`admin`/`admin123`) - change immediately
4. **SQLite**: Default database uses SQLite (file-based). For production, consider SQL Server with proper security

### Security Checklist

Before deploying to production:

- [ ] Change default admin password
- [ ] Set secure JWT key (at least 32 characters, cryptographically random)
- [ ] Configure HTTPS
- [ ] Restrict CORS to specific origins
- [ ] Update all dependencies to latest secure versions
- [ ] Review and configure Serilog log retention policies
- [ ] Set up database backups with encryption
- [ ] Configure firewall rules
- [ ] Review and test role-based access control
- [ ] Enable application monitoring and alerting
- [ ] Document security procedures for your team

## Security Updates

Security updates will be released as patch versions (e.g., 1.0.1, 1.0.2). Please keep your installation updated.

### How to Update

```bash
# Update .NET packages
dotnet restore
dotnet list package --outdated

# Update Angular packages
cd src/inventory-web
npm update
npm audit fix
```

## Security Contact

For security concerns or to report vulnerabilities, please:
1. **Do NOT** open a public GitHub issue
2. Email the repository maintainers or create a private security advisory
3. Include detailed information about the vulnerability
4. Allow 48 hours for initial response

