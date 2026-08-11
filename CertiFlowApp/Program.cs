using CertiFlow.Web.Infrastructure.Identity;
using CertiFlowApp.Components;
using CertiFlowApp.Data;
using CertiFlowApp.Features.Customers;
using CertiFlowApp.Features.Jobs;
using CertiFlowApp.Features.Measurements;
using CertiFlowApp.Features.Public;
using CertiFlowApp.Features.Tools;
using CertiFlowApp.Services.CurrentUser;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

// BUILDER
// Creates the application builder
// Used to configure logging, services, configuration and environment settings
var builder = WebApplication.CreateBuilder(args);

// SERVICES
// Managed by ASP.NET Core dependency injection

// Database
// Builds the PostgreSQL connection string from application configuration
var connectionString =
    $"Host={builder.Configuration["POSTGRES_HOST"]};" +
    $"Port={builder.Configuration["POSTGRES_PORT"]};" +
    $"Database={builder.Configuration["POSTGRES_DB"]};" +
    $"Username={builder.Configuration["POSTGRES_USER"]};" +
    $"Password={builder.Configuration["POSTGRES_PASSWORD"]}";


// Registers a DbContext factory for AppDbContext
// A new AppDbContext can be created for each database operation
// This is preferable for Blazor Interactive Server because a Blazor circuit
// can live longer than a normal HTTP request
builder.Services.AddDbContextFactory<AppDbContext>(
    options =>
        options.UseNpgsql(connectionString),
    ServiceLifetime.Scoped);

// Authentication & Identity
// Configures ASP.NET Core Identity.
// Identity handles users, passwords, login, lockout and roles
builder.Services
    .AddDefaultIdentity<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount =
            builder.Environment.IsProduction();
        // Password requirements.
        options.Password.RequiredLength = 6;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        // Locks the account temporarily after repeated failed login attempts
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.AllowedForNewUsers = true;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();

// Makes the authentication state available to Blazor components
builder.Services.AddCascadingAuthenticationState();

// APPLICATION SERVICES

// Registers the system clock as a singleton
// Same instance of TimeProvider is used throughout the application, ensuring consistent time handling
// Used for UTC audit timestamps and application date calculations.
builder.Services.AddSingleton(TimeProvider.System);

// HttpContextAccessor is used to access the current HTTP context, which is necessary for retrieving the current user
builder.Services.AddHttpContextAccessor();

// Scoped services are created once per dependency-injection scope.
// Unlike a traditional web request, Blazor Interactive Server
// keeps the same scoped service while the user interacts with the app.

// Services depending on ICurrentUser receive CurrentUser.
builder.Services.AddScoped<ICurrentUser, CurrentUser>();

// Application services with feature-specific application logic
builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<ToolService>();
builder.Services.AddScoped<JobService>();
builder.Services.AddScoped<MeasurementService>();
builder.Services.AddScoped<PublicDashboardService>();

// UI
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

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

// APP CONFIGURATIONS
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapRazorPages();

// DATA INITIALIZATION
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    await RoleSeeder.SeedAsync(roleManager);
}

// Seed identity data, create default admin user if not exists
if (app.Environment.IsDevelopment())
{
    // Seed development admin user only in Development
    await IdentitySeeder.SeedAsync(app.Services, builder.Configuration);
}

// ENDPOINTS
app.MapPost("/logout", async (SignInManager<ApplicationUser> signInManager) =>
{
    await signInManager.SignOutAsync();
    return Results.Redirect("/");
});

// Run DB MIGRATIONS on Dev environment
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();

    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

// RUN
app.Run();
