using System.Text;
using AAM.Inventory.Core.Application.Validators;
using AAM.Inventory.Core.Infrastructure;
using AAM.Inventory.Core.Infrastructure.Data;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace Inventory.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                    .AddEnvironmentVariables()
                    .Build())
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", "Inventory.API")
                .WriteTo.Console()
                .WriteTo.File("logs/inventory-.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 30)
                .CreateLogger();

            try
            {
                Log.Information("Starting Inventory API");

                var builder = WebApplication.CreateBuilder(args);

                // Use Serilog for logging
                builder.Host.UseSerilog();

                // Add services to the container
                builder.Services.AddControllers();
                
                // Add FluentValidation
                builder.Services.AddValidatorsFromAssemblyContaining<CreateProductDtoValidator>();
                builder.Services.AddFluentValidationAutoValidation();
                builder.Services.AddFluentValidationClientsideAdapters();


                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                    {
                        Version = "v1",
                        Title = "Inventory Suite API",
                        Description = "API for Inventory Management System",
                        Contact = new Microsoft.OpenApi.Models.OpenApiContact
                        {
                            Name = "Inventory Suite"
                        }
                    });
                
                    // Ensure OpenAPI 3.0 is used
                    options.CustomSchemaIds(type => type.FullName);

                    // Add JWT Authentication to Swagger
                    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                    {
                        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
                        Name = "Authorization",
                        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                        Scheme = "Bearer"
                    });

                    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                    {
                        {
                            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                            {
                                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                                {
                                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            Array.Empty<string>()
                        }
                    });
                });

                // Add Infrastructure services (repositories, services, DbContext)
                builder.Services.AddInfrastructure(builder.Configuration);

                // Add JWT Authentication
                var jwtKey = builder.Configuration["Jwt:Key"];
                if (string.IsNullOrWhiteSpace(jwtKey))
                {
                    throw new InvalidOperationException("JWT Key is required. Please set 'Jwt:Key' in appsettings.json or environment variables.");
                }
                builder.Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ClockSkew = TimeSpan.Zero
                    };
                });

                builder.Services.AddAuthorization();

                // Add CORS for Angular frontend
                builder.Services.AddCors(options =>
                {
                    options.AddPolicy("AllowAngular", policy =>
                    {
                        if (builder.Environment.IsDevelopment())
                        {
                            // In development, allow any localhost origin
                            policy.SetIsOriginAllowed(origin => 
                                origin.StartsWith("http://localhost:") || 
                                origin.StartsWith("http://127.0.0.1:"))
                                  .AllowAnyHeader()
                                  .AllowAnyMethod()
                                  .AllowCredentials();
                        }
                        else
                        {
                            // In production, specify exact origins
                            policy.WithOrigins(
                                    "http://localhost:4200",
                                    "https://yourdomain.com"
                                  )
                                  .AllowAnyHeader()
                                  .AllowAnyMethod()
                                  .AllowCredentials();
                        }
                    });
                });

                var app = builder.Build();

                // Use Serilog request logging
                app.UseSerilogRequestLogging(options =>
                {
                    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
                    options.GetLevel = (httpContext, elapsed, ex) => ex != null || elapsed > 1000
                        ? Serilog.Events.LogEventLevel.Warning
                        : Serilog.Events.LogEventLevel.Information;
                });

                // Apply migrations
                Log.Information("Applying database migrations...");
                await app.Services.MigrateDatabaseAsync();
                
                // Seed database with initial data
                Log.Information("Seeding database...");
                await DatabaseSeeder.SeedAsync(app.Services);

                // Configure the HTTP request pipeline
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI(options =>
                    {
                        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Inventory Suite API v1");
                        options.RoutePrefix = "swagger";
                    });
                }

                // CORS must be before UseAuthorization
                app.UseCors("AllowAngular");
                app.UseHttpsRedirection();
                app.UseAuthentication();
                app.UseAuthorization();
                app.MapControllers();

                Log.Information("Inventory API started successfully");
                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly");
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
