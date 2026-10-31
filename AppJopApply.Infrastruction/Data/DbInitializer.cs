using AppJopApply.Domain.Constants;
using AppJopApply.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace AppJopApply.Infrastruction.Data
{
    public static class DbInitializer
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

            string[] roles = [AppRoles.Admin, AppRoles.HR, AppRoles.Candidate];
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Seed default administrator if not present
            var adminEmail = "admin@jopapply.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser is null)
            {
                adminUser = new AppUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "System",
                    LastName = "Admin",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin@123456");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, AppRoles.Admin);
                }
            }
        }
    }
}
