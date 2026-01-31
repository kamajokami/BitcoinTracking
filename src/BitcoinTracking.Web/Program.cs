using Serilog;
using BitcoinTracking.DAL.Extensions;
using BitcoinTracking.BAL.Extensions;
using BitcoinTracking.Shared.Logging;


var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = SerilogConfiguration.CreateLogger(builder.Configuration);
builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register Data Access Layer (DAL)
builder.Services.AddInfrastructure(builder.Configuration);

// TODO: Register Business Logic Layer (BAL) - FÁZE D
builder.Services.AddBusinessLogic(builder.Configuration);


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
