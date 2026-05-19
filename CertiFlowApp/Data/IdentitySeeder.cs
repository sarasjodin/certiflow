using Microsoft.AspNetCore.Identity;

namespace CertiFlowApp.Data;

public static class IdentitySeeder
{
    public static async Task SeedAsync(
        IServiceProvider services,
        IConfiguration configuration)
    {
        using var scope = services.CreateScope();

        var userManager =
            scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        const string email = "admin@certiflow.local";

        var password =
            configuration["SeedUserPassword"]
            ?? configuration["SEED_USER_PASSWORD"];

        if (string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException(
                "Seed user password is missing.");
        }

        var existingUser =
            await userManager.FindByEmailAsync(email);

        if (existingUser is not null)
        {
            return;
        }

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            var errors = string.Join(
                ", ",
                result.Errors.Select(error => error.Description));

            throw new InvalidOperationException(
                $"Failed to create seed user: {errors}");
        }
    }
}