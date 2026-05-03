namespace TaskNest.Infrastructure.Persistence
{
    public static class IdentitySeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();

            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var roles = config.GetSection("Roles").Get<string[]>() ?? Array.Empty<string>();

            foreach (var role in roles)
            {
                if (string.IsNullOrWhiteSpace(role))
                    continue;

                if (!await roleManager.RoleExistsAsync(role))
                {
                    var r = await roleManager.CreateAsync(new IdentityRole(role));

                    if (!r.Succeeded)
                    {
                        throw new Exception($"Failed to create role {role}: {string.Join(", ", r.Errors.Select(e => e.Description))}");
                    }
                }
            }

            var adminSection = config.GetSection("AdminUsers");
            var adminEmails = adminSection.GetSection("Emails").Get<string[]>() ?? Array.Empty<string>();
            var adminPassword = adminSection["Password"] ?? "Admin123!";

            foreach (var email in adminEmails.Where(e => !string.IsNullOrWhiteSpace(e)))
            {
                var existing = await userManager.FindByEmailAsync(email);

                if (existing != null) 
                    continue;

                var newUser = new ApplicationUser
                {
                    Email = email,
                    UserName = email,
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(newUser, adminPassword);
                if (!createResult.Succeeded)
                {
                    var errs = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    throw new Exception($"Failed to create admin user '{email}': {errs}");
                }

                if (roles.Contains("Admin"))
                {
                    var addRoleResult = await userManager.AddToRoleAsync(newUser, "Admin");
                    if (!addRoleResult.Succeeded)
                    {
                        var errs = string.Join(", ", addRoleResult.Errors.Select(e => e.Description));
                        throw new Exception($"Failed to add user '{email}' to role Admin: {errs}");
                    }
                }
            }
        }
    }
}