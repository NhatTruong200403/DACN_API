using Microsoft.AspNetCore.Identity;

namespace GoWheels_WebAPI.Models.Entities
{
    public static class ApplicationRole
    {
        public const string Admin = "Admin";
        public const string User = "User";
        public const string Employee = "Employee";

        public static async Task InitialRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            await CreateRoleIfExistAsync(roleManager, Admin);
            await CreateRoleIfExistAsync(roleManager, User);
            await CreateRoleIfExistAsync(roleManager, Employee);
        }
        public static async Task CreateRoleIfExistAsync(RoleManager<IdentityRole> roleManager, string roleName)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }
}
