using GoWheels_WebAPI.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace GoWheels_WebAPI.Repositories.Interface
{
    public interface IUserRepository
    {
        Task<ApplicationUser> FindByIdAsync (string id);
        Task<ApplicationUser?> FindByUserNameAsync(string userName);
        Task<ApplicationUser?> FindByEmailAsync(string email);
        Task<bool> ValidatePasswordAsync(ApplicationUser user, string password);
        Task<IList<string>> GetUserRolesAsync(ApplicationUser user);
        Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password);
        Task AddUserToRoleAsync(ApplicationUser user, string roleName);
        Task EnsureRoleExistsAsync(string roleName);
    }
}
