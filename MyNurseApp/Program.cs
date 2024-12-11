using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyNurseApp.Data;
using MyNurseApp.Data.Configuration;
using MyNurseApp.Data.Models;
using MyNurseApp.Data.Repository;
using MyNurseApp.Data.Repository.Interfaces;
using MyNurseApp.Services.Data;
using MyNurseApp.Services.Data.Interfaces;

namespace MyNurseApp
{
    public class Program
    {
        public static async Task Main(string[] args)
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

            builder.Services.AddDistributedMemoryCache();

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddScoped<IPatientService,PatientService>();
            builder.Services.AddScoped<IManipulationsService,ManipulationsService>();
            builder.Services.AddScoped<IScheduleService,ScheduleService>();
            builder.Services.AddScoped<INurseService,NurseService>();
            builder.Services.AddScoped<IReviewService,ReviewService>();
            builder.Services.AddScoped<IAdminInformationService, AdminInformationService>();
            builder.Services.AddScoped(typeof(IRepository<,>), typeof(BaseRepository<,>));


            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<RedirectIfPendingMiddleware>();

            using (var scope = app.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;

                //"Email": "admin@abv.bg",
                //"UserName": "Admin",
                //"Password": "Admin7!"
                await DataBaseSeeder.SeedRolesAsync(serviceProvider);
                DataBaseSeeder.SeedAndAdmin(serviceProvider);
            }


            app.UseSession();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapRazorPages();
            app.MapDefaultControllerRoute();

            app.Run();
        }
    }
}
