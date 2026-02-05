using BitcoinTracking.BAL.Extensions;
using BitcoinTracking.DAL.Extensions;
using BitcoinTracking.Shared.Logging;
using BitcoinTracking.Web.Configuration;
using BitcoinTracking.Web.Services;
using Microsoft.Extensions.Options;
using Serilog;


var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = SerilogConfiguration.CreateLogger(builder.Configuration);
builder.Host.UseSerilog();

// Add services to the container (MVC)
builder.Services.AddControllersWithViews();

// Register Data Access Layer (DAL)
builder.Services.AddInfrastructure(builder.Configuration);

// TODO: Register Business Logic Layer (BAL)
builder.Services.AddBusinessLogic(builder.Configuration);


// Register HttpClient for API calls
builder.Services.Configure<ApiSettings>(
    builder.Configuration.GetSection("ApiSettings"));

builder.Services.AddHttpClient<BitcoinApiClient>((sp, client) =>
{
    var apiSettings = sp.GetRequiredService<IOptions<ApiSettings>>().Value;

    client.BaseAddress = new Uri(apiSettings.BaseUrl);
    client.Timeout = TimeSpan.FromSeconds(apiSettings.TimeoutSeconds);
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Auth pipeline
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Log application startup
Log.Information("BitcoinTracking Web Application started");

try
{
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
