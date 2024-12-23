using Microsoft.AspNetCore.Identity;

namespace EntityLayer
{
    public static class SeedData
    {
        public static async Task Initialize(RoleManager<IdentityRole<int>> roleManager, UserManager<AppUser> userManager)
        {
            string[] roleNames = { "Admin", "Barber", "Customer" };
            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    // Rolü oluştur
                    roleResult = await roleManager.CreateAsync(new IdentityRole<int>(roleName));
                    if (!roleResult.Succeeded)
                    {
                        // Hata durumunda loglama yapabilirsiniz
                        foreach (var error in roleResult.Errors)
                        {
                            Console.WriteLine($"Error creating role {roleName}: {error.Description}");
                        }
                    }
                }
            }

            // Örnek bir admin kullanıcısı oluşturma
            var adminUser = new AppUser
            {
                UserName = "admin@example.com",
                Email = "admin@example.com",
                EmailConfirmed = true
            };

            var user = await userManager.FindByEmailAsync(adminUser.Email);
            if (user == null)
            {
                var createAdminUser = await userManager.CreateAsync(adminUser, "AdminPassword123!");
                if (createAdminUser.Succeeded)
                {
                    // Admin rolünü kullanıcıya ata
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}