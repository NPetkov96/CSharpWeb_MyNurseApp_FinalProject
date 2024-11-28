using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyNurseApp.Data.Models;

namespace MyNurseApp.Data.Configuration
{
    public static class DataBaseSeeder
    {
        public static void SeedAndAdmin(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();

            var adminSection = configuration.GetSection("Administrator");
            var adminEmail = adminSection["Email"] ?? throw new InvalidOperationException("Admin Email not configured.");
            var adminUserName = adminSection["UserName"] ?? throw new InvalidOperationException("Admin UserName not configured.");
            var adminPassword = adminSection["Password"] ?? throw new InvalidOperationException("Admin Password not configured.");

            var adminRole = "Admin";

            if (!roleManager.RoleExistsAsync(adminRole).GetAwaiter().GetResult())
            {
                var roleResult = roleManager.CreateAsync(new IdentityRole<Guid>(adminRole)).GetAwaiter().GetResult();
                if (!roleResult.Succeeded)
                {
                    throw new InvalidOperationException($"Failed to create role '{adminRole}'.");
                }
            }

            var existingAdmin = userManager.FindByEmailAsync(adminEmail).GetAwaiter().GetResult();
            if (existingAdmin == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = adminUserName,
                    Email = adminEmail
                };

                var userResult = userManager.CreateAsync(adminUser, adminPassword).GetAwaiter().GetResult();
                if (!userResult.Succeeded)
                {
                    throw new InvalidOperationException("Failed to create the admin user.");
                }

                var roleAssignResult = userManager.AddToRoleAsync(adminUser, adminRole).GetAwaiter().GetResult();
                if (!roleAssignResult.Succeeded)
                {
                    throw new InvalidOperationException($"Failed to assign role '{adminRole}' to the admin user.");
                }
            }
        }
        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

            // Уверете се, че ролята Nurse съществува
            if (!await roleManager.RoleExistsAsync("Nurse"))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>("Nurse"));
            }

            // Уверете се, че ролята User съществува
            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>("User"));
            }
        }
    }

}