using GoWheels_WebAPI.Data;
using GoWheels_WebAPI.Models.Entities;
using GoWheels_WebAPI.Models.ViewModels;
using GoWheels_WebAPI.Repositories.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace GoWheels_WebAPI.Repositories
{
    public class AuthenticationRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public AuthenticationRepository(UserManager<ApplicationUser> userManager,
                                        RoleManager<IdentityRole> roleManager, 
                                        ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public List<ApplicationUser> GetAllUser()
            => _userManager.Users.ToList();

        public async Task AddUserToRoleAsync(ApplicationUser user, string roleName)
        {
            var trackedUser = _context.ChangeTracker.Entries<IdentityUser>().FirstOrDefault(b => b.Entity.Id == user.Id);
            if (trackedUser != null)
            {
                _context.Entry(trackedUser.Entity).State = EntityState.Detached;
            }
            _context.Entry(user).State = EntityState.Modified;
            await _userManager.AddToRoleAsync(user, roleName);
            _context.Entry(user).State = EntityState.Detached;
        }    


        public async Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password)
            => await _userManager.CreateAsync(user, password);

        public async Task EnsureRoleExistsAsync(string roleName)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        public async Task<bool> CheckValidPasswordAsync(ApplicationUser user, string password)
            => await _userManager.CheckPasswordAsync(user, password);
        
        public async Task ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword)
            => await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);


        public async Task<ApplicationUser> FindByUserId(string userId)
            => await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId) ?? throw new NullReferenceException("User not found");

        public async Task<ApplicationUser> FindByUserNameAsync(string userName) 
            => await _userManager.FindByNameAsync(userName) ?? throw new NullReferenceException("User not found");

        public async Task<IList<string>> GetUserRolesAsync(ApplicationUser user) 
            => await _userManager.GetRolesAsync(user);
        public async Task UpdateAsync(ApplicationUser user)
        {
            var existingUser = _context.ChangeTracker.Entries<ApplicationUser>().FirstOrDefault(e => e.Entity.Id == user.Id);
            if (existingUser != null)
            {
                _context.Entry(existingUser.Entity).State = EntityState.Detached;
            }
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            //detached tracking obj after modified
            _context.Entry(user).State = EntityState.Detached;
        }

        public async Task<bool> IsInRole(ApplicationUser user, string roleName)
            => await _userManager.IsInRoleAsync(user, roleName);
    }
}
