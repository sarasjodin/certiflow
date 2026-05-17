using CertiFlowApp.Components;
using CertiFlowApp.Data;
using Microsoft.EntityFrameworkCore;

// BUILDER
var builder = WebApplication.CreateBuilder(args);

// LOGGING
builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(options =>
{
    options.TimestampFormat = "[yyyy-MM-dd HH:mm:ss] ";
});

// SERVICES

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// UI/services
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// BUILD APP
var app = builder.Build();
var logger = app.Logger;
logger.LogInformation("Application starting");
logger.LogInformation(
    "Current environment: {Environment}",
    app.Environment.EnvironmentName);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// MIDDLEWARE
// Security
app.UseHttpsRedirection();
// app.UseAuthentication();
// app.UseAuthorization();
app.UseAntiforgery();

// Static files
app.UseStaticFiles();

// APP CONFIGURATIONS / ENDPOINTS
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Run app
app.Run();
