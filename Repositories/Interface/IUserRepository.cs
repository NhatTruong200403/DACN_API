using GoWheels_WebAPI.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace GoWheels_WebAPI.Repositories.Interface
{
    public interface IUserRepository
    {
        List<ApplicationUser> GetAllUser();
        Task<ApplicationUser> FindByUserNameAsync(string userName);
        Task<ApplicationUser> FindByUserId(string userId);
        Task<bool> CheckValidPasswordAsync(ApplicationUser user, string password);
        Task ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword);
        Task<bool> IsInRole(ApplicationUser user, string roleName);
        Task<IList<string>> GetUserRolesAsync(ApplicationUser user);
        Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password);
        Task AddUserToRoleAsync(ApplicationUser user, string roleName);
        Task UpdateAsync(ApplicationUser user);
        Task EnsureRoleExistsAsync(string roleName);
    }
}
