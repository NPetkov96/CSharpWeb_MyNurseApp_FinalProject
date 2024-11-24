using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyNurseApp.Data;
using MyNurseApp.Data.Models;
using MyNurseApp.Data.Repository.Interfaces;
using MyNurseApp.Data.Repository;
using MyNurseApp.Services.Data;
using MyNurseApp.Data.Configuration;

namespace MyNurseApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services
                .AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
            builder.Services
                .AddDatabaseDeveloperPageExceptionFilter();


            builder.Services
                .AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.AddRazorPages();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Identity/Account/Login";
            });

            builder.Services.AddControllersWithViews();

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddScoped<PatientService>();
            builder.Services.AddScoped<ManipulationsService>();
            builder.Services.AddScoped(typeof(IRepository<,>), typeof(BaseRepository<,>));

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                DataBaseSeeder.SeedRolesAndAdmin(serviceProvider);
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapRazorPages();
            app.MapDefaultControllerRoute();

            app.Run();
        }
    }
}
