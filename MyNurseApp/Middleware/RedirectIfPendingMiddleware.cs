﻿using Microsoft.AspNetCore.Identity;
using MyNurseApp.Data.Models;

public class RedirectIfPendingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IServiceProvider _serviceProvider;

    private readonly string[] _excludedPaths = [
    "/Home/PendingApproval",
    "/Identity/Account/Logout",
    "/Review",
    "/Nurse/CreateNurseProfile",
    "/Nurse/Profile",
    "/Nurse/EditNurseProfile",
];
    public RedirectIfPendingMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
    {
        _next = next;
        _serviceProvider = serviceProvider;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Изключване на определени пътища от проверката
        var path = context.Request.Path.Value;
        var excludedPaths = _excludedPaths;
        if (path != null && excludedPaths.Any(p => path.StartsWith(p, StringComparison.OrdinalIgnoreCase)))
        {
            Console.WriteLine($"Path {path} is excluded. Continuing pipeline...");
            await _next(context);
            Console.WriteLine($"Pipeline continued successfully for {path}.");
            return;
        }

        // Проверка дали потребителят е логнат
        if (context.User.Identity!.IsAuthenticated)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var user = await userManager.GetUserAsync(context.User);

                if (user != null && user.IsPending)
                {
                    // Пренасочване към екшън, ако е в статус Pending
                    context.Response.Redirect("/Home/PendingApproval");
                    return;
                }
            }
        }

        // Продължаваме с останалия pipeline
        await _next(context);
    }
}
