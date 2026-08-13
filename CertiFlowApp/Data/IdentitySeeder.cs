using CertiFlow.Web.Infrastructure.Identity;
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

        var domain =
            configuration["SEED_USER_DOMAIN"]
            ?? "certiflow.local";

        var password = GetSeedPassword(configuration);

        var users = new[]
        {
            new SeedUser(
                $"admin@{domain}",
                [ApplicationRoles.SystemAdmin, ApplicationRoles.Approver]),

            new SeedUser(
                $"haddad.operator@{domain}",
                [ApplicationRoles.Operator]),

            new SeedUser(
                $"tanaka.qc@{domain}",
                [ApplicationRoles.Operator, ApplicationRoles.Verifier]),

            new SeedUser(
                $"rossi.approver@{domain}",
                [ApplicationRoles.Approver]),

            new SeedUser(
                $"pereira.itadmin@{domain}",
                [ApplicationRoles.SystemAdmin]),

            new SeedUser(
                $"client.demo@{domain}",
                [ApplicationRoles.Client])
        };

        foreach (var user in users)
        {
            await CreateUserIfMissingAsync(
                userManager,
                user,
                password);
        }
    }

    private static string GetSeedPassword(
        IConfiguration configuration)
    {
        var password =
            configuration["SeedUserPassword"]
            ?? configuration["SEED_USER_PASSWORD"];

        if (string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException(
                "Seed user password is missing.");
        }

        return password;
    }

    private static async Task CreateUserIfMissingAsync(
        UserManager<ApplicationUser> userManager,
        SeedUser seedUser,
        string password)
    {
        var existingUser =
            await userManager.FindByEmailAsync(seedUser.Email);

        if (existingUser is null)
        {
            existingUser = await CreateUserAsync(
                userManager,
                seedUser.Email,
                password);
        }

        await EnsureRolesAsync(
            userManager,
            existingUser,
            seedUser.Roles);
    }

    private static async Task<ApplicationUser> CreateUserAsync(
        UserManager<ApplicationUser> userManager,
        string email,
        string password)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true
        };

        var result =
            await userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            var errors = string.Join(
                ", ",
                result.Errors.Select(error => error.Description));

            throw new InvalidOperationException(
                $"Failed to create seed user '{email}': {errors}");
        }

        return user;
    }

    private static async Task EnsureRolesAsync(
        UserManager<ApplicationUser> userManager,
        ApplicationUser user,
        string[] roles)
    {
        foreach (var role in roles)
        {
            var isInRole =
                await userManager.IsInRoleAsync(user, role);

            if (isInRole)
            {
                continue;
            }

            var result =
                await userManager.AddToRoleAsync(user, role);

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    result.Errors.Select(error => error.Description));

                throw new InvalidOperationException(
                    $"Failed to add role '{role}' to '{user.Email}': {errors}");
            }
        }
    }

    private sealed record SeedUser(
        string Email,
        string[] Roles);
}
