namespace TaskTronic.Identity.Data
{
    using Microsoft.AspNetCore.Identity;
    using System.Threading.Tasks;
    using Models;
    using TaskTronic.Services;
    using System.Linq;
    using System.Globalization;
    using System;

    public class IdentityDbSeeder : IDbSeeder
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public IdentityDbSeeder(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public void SeedData()
        {
            if (this.roleManager.Roles.Any())
            {
                return;
            }

            Task.Run(async () =>
            {
                const string adminEmail = "admin@mymail.com";
                const string adminUserName = "admin";

                var adminRole = new IdentityRole(GlobalConstants.AdministratorRoleName);

                await this.roleManager.CreateAsync(adminRole);

                var adminUser = new ApplicationUser
                {
                    UserName = adminUserName,
                    Email = adminEmail,
                    SecurityStamp = Guid.NewGuid().ToString()
                };

                await userManager.CreateAsync(adminUser, "admin12");

                await userManager.AddToRoleAsync(adminUser, GlobalConstants.AdministratorRoleName);
            }).GetAwaiter().GetResult();
        }
    }
}
