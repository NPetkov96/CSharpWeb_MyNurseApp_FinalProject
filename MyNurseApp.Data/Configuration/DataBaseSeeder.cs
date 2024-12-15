using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyNurseApp.Data.Models;
using MyNurseApp.Data.Repository.Interfaces;

namespace MyNurseApp.Data.Configuration
{
    public static class DataBaseSeeder
    {
        public static void SeedAdmin(IServiceProvider serviceProvider)
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

            if (!await roleManager.RoleExistsAsync("Nurse"))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>("Nurse"));
            }

            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>("User"));
            }
        }
        public static async Task SeedManipulationsAsync(IServiceProvider serviceProvider)
        {
            var manipulationRepository = serviceProvider.GetRequiredService<IRepository<MedicalManipulation, Guid>>();

            if (!manipulationRepository.GetAllAttached().Any())
            {
                var manipulations = new List<MedicalManipulation>
                {
                    new MedicalManipulation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Blood Test",
                        Duration = 30,
                        Description = "A simple blood test to analyze general health markers.",
                        Price = 50m
                    },
                    new MedicalManipulation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Vaccination",
                        Duration = 15,
                        Description = "Basic vaccination procedure for immunization.",
                        Price = 20m
                    },
                    new MedicalManipulation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Electrocardiogram (ECG)",
                        Duration = 40,
                        Description = "A test to measure the electrical activity of the heart.",
                        Price = 70m
                    },
                    new MedicalManipulation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Wound Dressing",
                        Duration = 25,
                        Description = "Changing and cleaning wound dressings to prevent infection.",
                        Price = 40m
                    },
                    new MedicalManipulation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Ultrasound Scan",
                        Duration = 60,
                        Description = "An imaging procedure to evaluate internal organs and structures.",
                        Price = 120m
                    },
                    new MedicalManipulation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Blood Pressure Monitoring",
                        Duration = 10,
                        Description = "A quick check of blood pressure levels.",
                        Price = 15m
                    },
                    new MedicalManipulation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Intravenous Therapy",
                        Duration = 45,
                        Description = "Administration of fluids or medication through an IV.",
                        Price = 100m
                    }
                };

                foreach (var manipulation in manipulations)
                {
                    await manipulationRepository.AddAsync(manipulation);
                }
            }
        }

    }

}