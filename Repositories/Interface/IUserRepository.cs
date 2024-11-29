using GoWheels_WebAPI.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace GoWheels_WebAPI.Repositories.Interface
{
    public interface IUserRepository
    {
        Task<List<ApplicationUser>> GetAllSubmitDriversAsync();
        Task<List<ApplicationUser>> GetAllUserAsync();
        Task<ApplicationUser> FindByUserNameAsync(string userName);
        Task<ApplicationUser> FindByUserIdAsync(string userId);
        Task<bool> ValidatePasswordAsync(ApplicationUser user, string password);
        Task<bool> IsInRoleAsync(ApplicationUser user, string roleName);
        Task<IList<string>> GetUserRolesAsync(ApplicationUser user);
        Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password);
        Task AddUserToRoleAsync(ApplicationUser user, string roleName);
        Task UpdateAsync(ApplicationUser user);
        Task EnsureRoleExistsAsync(string roleName);
    }
}
