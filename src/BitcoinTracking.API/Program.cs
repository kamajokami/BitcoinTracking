using Serilog;
using BitcoinTracking.DAL.Extensions;
using BitcoinTracking.BAL.Extensions;
using BitcoinTracking.API.Extensions;
using BitcoinTracking.Shared.Logging;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container

// Configure Serilog
Log.Logger = SerilogConfiguration.CreateLogger(builder.Configuration);

// Default ASP.NET Core logging replaced with Serilog
builder.Host.UseSerilog();

// Register MVC controllers for REST API endpoints
builder.Services.AddControllers();

// Register Data Access Layer (DAL): DbContext, Repositories, External API clients
builder.Services.AddInfrastructure(builder.Configuration);

// Register Business Logic Layer (BAL): Services, Domain logic, DTO mappings
builder.Services.AddBusinessLogic(builder.Configuration);

// Configure endpoint for minimal APIs and Swagger
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger / OpenAPI documentation
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Bitcoin Tracking API",
        Version = "v1",
        Description = "REST API for Bitcoin price tracking with CoinDesk and ÈNB integration",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Bitcoin Tracking",
            Email = "info@bitcointracking.cz"
        }
    });

    // Include XML comments (generated from code Swagger documentation)
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";

    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

// Configure Cross-Origin Resource Sharing only from frontend app
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
            "https://localhost:5001",
            "http://localhost:5000") // Web MVC app
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline

// Register custom exception handling Middleware
app.UseExceptionHandling();

// Swagger UI (only in Development)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Bitcoin Tracking API v1");

        // Swagger UI available at application root  https://localhost:xxx
        options.RoutePrefix = string.Empty;
    });
}

// HTTPS redirection
app.UseHttpsRedirection();

// Enable CORS middleware
app.UseCors();

// Enable authorization middleware
app.UseAuthorization();

// Map controller endpoint
app.MapControllers();

// Log successful application startup
Log.Information("Bitcoin Tracking API started");

try
{
    // Start web application
    app.Run();
}
catch (Exception ex)
{
    // Log fatal startup or runtime errors
    Log.Fatal(ex, "API terminated unexpectedly");
}
finally
{
    // Flush and close Serilog sinks
    Log.CloseAndFlush();
}