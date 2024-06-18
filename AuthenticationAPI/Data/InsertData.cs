using AccessIdentityAPI.Models;
using Microsoft.AspNetCore.Identity;

namespace AccessIdentityAPI.Data
{
    public static class InsertData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            SeedRoles(roleManager);
            SeedUsers(userManager);
        }
        /// <summary>
        /// Seeds default roles ("Admin" and "Member") into the role manager if they don't already exist.
        /// </summary>
        /// <param name="roleManager">The role manager to use for role operations.</param>

        private static void SeedRoles(RoleManager<ApplicationRole> roleManager)
        {
            if (!roleManager.RoleExistsAsync("Admin").Result)
            {
                var role = new ApplicationRole
                {
                    Name = "Admin"
                };
                
                roleManager.CreateAsync(role).Wait();
            }
            if (!roleManager.RoleExistsAsync("Member").Result)
            {
                var role = new ApplicationRole
                {
                    Name = "Member"
                };

                roleManager.CreateAsync(role).Wait();
            }
        }

        private static void SeedUsers(UserManager<ApplicationUser> userManager)
        {
            if (userManager.FindByEmailAsync("priyagoli@gmail.com").Result == null)
            {
                var user = new ApplicationUser
                {
                    FirstName = "Janaki Priya ",
                    LastName = "Goli",
                    Email = "priyagoli@gmail.com",
                    UserName = "janakipriya",
                    PhoneNumber = "7382008888",
                };

                var result = userManager.CreateAsync(user, "P@ssw0rd").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Admin").Wait();
                    userManager.AddToRoleAsync(user, "Member").Wait();
                }
            }
        }
    }
}
