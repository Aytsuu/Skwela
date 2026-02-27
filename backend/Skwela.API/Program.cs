using Microsoft.EntityFrameworkCore;
using Skwela.Infrastructure;
using Skwela.Application;
using Skwela.Infrastructure.Data;
using System.Text.Json.Serialization;

/// <summary>
/// Skwela API Application Entry Point
/// Sets up all required services, middleware, and database configuration for the learning management system
/// </summary>
var builder = WebApplication.CreateBuilder(args);


// Retrieve JWT secret key from configuration and validate it exists
var jwtKey = builder.Configuration["Jwt:Key"];

if (string.IsNullOrEmpty(jwtKey))
{
    throw new InvalidOperationException("JWT Key is not configured.");
}

// Register infrastructure services (database, repositories, authentication)
builder.Services.AddInfrastructure(builder.Configuration);

// Register application services (use cases, business logic)
builder.Services.AddApplication();

// Retrieve database connection string and validate it exists
var conn = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(conn))
{
    throw new InvalidOperationException("Database connection string is not configured.");
}

// Configure controllers and JSON serialization to handle enums as strings
builder.Services.AddControllers()
    .AddJsonOptions(options => 
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
    
// Add Swagger/OpenAPI documentation generation
builder.Services.AddSwaggerGen();

// Add authorization policies
builder.Services.AddAuthorization();


// Configure CORS (Cross-Origin Resource Sharing) to allow frontend to access API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        // Allow requests from specified frontend origins
        policy.WithOrigins(
            "https://skwela.paoloaraneta.dev",
            "http://skwela.local:3000",
            "http://127.0.0.1:3000",
            "http://localhost:3000"
        )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Build the web application
var app = builder.Build();

// Enable Swagger documentation UI in development
app.UseSwagger();
app.UseSwaggerUI();

// Configure middleware pipeline
// app.UseHttpsRedirection(); // Disabled for development
app.UseRouting();
app.UseCors("AllowFrontend"); // Apply CORS policy

// Apply pending database migrations on application startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    var context = services.GetRequiredService<AppDbContext>();

    for (int retries = 0; retries < 10; retries++)
    {
        try
        {
            // This checks for any pending migrations and applies them to whatever 
            // database is defined in your connection string.
            context.Database.Migrate();
            logger.LogInformation("Database migration applied successfully.");
            break;
        }
        catch (Exception)
        {
            if (retries == 9)
            {
                logger.LogError("Could not connect to the database.");
            }   
            else
            {
                logger.LogError("Connecting...");
                await Task.Delay(3000);
            }
        }
    }
}

// Configure additional middleware
app.UseForwardedHeaders(); // Handle forwarded headers for proxy scenarios
app.UseAuthentication(); // Enable JWT/Cookie authentication
app.UseAuthorization(); // Enable authorization checks

// Map controller endpoints and run the application
app.MapControllers();
app.Run();