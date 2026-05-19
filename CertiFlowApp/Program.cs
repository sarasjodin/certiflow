using Microsoft.AspNetCore.Identity;
using CertiFlowApp.Components;
using CertiFlowApp.Data;
using Microsoft.EntityFrameworkCore;

// BUILDER
var builder = WebApplication.CreateBuilder(args);

// LOGGING
builder.Logging.ClearProviders();

if (builder.Environment.IsDevelopment())
{
    builder.Logging.SetMinimumLevel(LogLevel.Debug);
}
else
{
    builder.Logging.SetMinimumLevel(LogLevel.Information);
}

builder.Logging.AddSimpleConsole(options =>
{
    options.TimestampFormat = "[yyyy-MM-dd HH:mm:ss] ";
});

// SERVICES

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Authentication & Identity
builder.Services
    .AddDefaultIdentity<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;

        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.AllowedForNewUsers = true;
    })
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddCascadingAuthenticationState();

// UI/services
builder.Services.AddRazorPages();

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
    app.UseStatusCodePages();
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

app.MapRazorPages();

// Seed identity data, create default admin user if not exists
await IdentitySeeder.SeedAsync(app.Services, builder.Configuration);

app.MapPost("/logout", async (SignInManager<ApplicationUser> signInManager) =>
{
    await signInManager.SignOutAsync();
    return Results.Redirect("/");
});

// Run app
app.Run();
