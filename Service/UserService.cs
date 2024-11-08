using GoWheels_WebAPI.Models.Entities;
using GoWheels_WebAPI.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GoWheels_WebAPI.Service
{
    public class UserService
    {
        private readonly IUserRepository _autheticationRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _userId;
        private readonly IConfiguration _config;

        public UserService(IUserRepository autheticationRepository, 
                            IHttpContextAccessor httpContextAccessor, 
                            IConfiguration config)
        {
            _autheticationRepository = autheticationRepository;
            _httpContextAccessor = httpContextAccessor;
            _userId = _httpContextAccessor.HttpContext?.User?
                        .FindFirstValue(ClaimTypes.NameIdentifier) ?? "UnknownUser";
            _config = config;
        }

        public async Task UpdateUserInfoAsync(ApplicationUser user)
        {
            try
            {
                var existingUser = await _autheticationRepository.FindByUserIdAsync(_userId);
                existingUser.Name = user.Name == null ? existingUser.Name : user.Name;
                existingUser.PhoneNumber = user.PhoneNumber == null ? existingUser.PhoneNumber : user.PhoneNumber;
                existingUser.Image = user.Image == null ? existingUser.Image : user.Image;
                existingUser.Birthday = user.Birthday == null ? existingUser.Birthday : user.Birthday;
                existingUser.License = user.License == null ? existingUser.License : user.License;
                await _autheticationRepository.UpdateAsync(existingUser);
            }
            catch(NullReferenceException nullEx)
            {
                throw new NullReferenceException(nullEx.InnerException!.Message);
            }
            catch (DbUpdateException dbEx)
            {
                throw new DbUpdateException(dbEx.InnerException!.Message);
            }
            catch (InvalidOperationException operationEx)
            {
                throw new InvalidOperationException(operationEx.InnerException!.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task UpdateUserReportPointAsync(string userId, int reportPoint)
        {
            try
            {
                var user = await _autheticationRepository.FindByUserIdAsync(userId);
                user.ReportPoint += reportPoint;
                await _autheticationRepository.UpdateAsync(user);
            }
            catch (NullReferenceException nullEx)
            {
                throw new NullReferenceException(nullEx.InnerException!.Message);
            }
            catch (DbUpdateException dbEx)
            {
                throw new DbUpdateException(dbEx.InnerException!.Message);
            }
            catch (InvalidOperationException operationEx)
            {
                throw new InvalidOperationException(operationEx.InnerException!.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
