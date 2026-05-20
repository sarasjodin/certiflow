using Microsoft.AspNetCore.Identity;

namespace CertiFlow.Web.Infrastructure.Identity;

public static class RoleSeeder
{
    public static async Task SeedAsync(RoleManager<IdentityRole> roleManager)
    {
        foreach (var role in ApplicationRoles.All)
        {
            var roleExists = await roleManager.RoleExistsAsync(role);

            if (roleExists)
            {
                continue;
            }

            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}