using AutoMapper;
using GoWheels_WebAPI.Models.Entities;
using GoWheels_WebAPI.Models.ViewModels;
using GoWheels_WebAPI.Repositories.Interface;
using GoWheels_WebAPI.Service.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text.Json;

namespace GoWheels_WebAPI.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _autheticationRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly RedisCacheService _redisCacheService;
        private readonly IMapper _mapper;
        private readonly string _userId;

        public UserService(IUserRepository autheticationRepository,
                            IHttpContextAccessor httpContextAccessor,
                            RedisCacheService redisCacheService,
                            IMapper mapper)
        {
            _autheticationRepository = autheticationRepository;
            _httpContextAccessor = httpContextAccessor;
            _redisCacheService = redisCacheService;
            _mapper = mapper;
            _userId = _httpContextAccessor.HttpContext?.User?
                        .FindFirstValue(ClaimTypes.NameIdentifier) ?? "UnknownUser";
        }

        public List<ApplicationUser> GetAllUser()
        {
            var userRequest = _httpContextAccessor.HttpContext?.User;
            var users = _autheticationRepository.GetAllUser();
            if (userRequest != null && userRequest.IsInRole("User"))
            {
                foreach (var user in users)
                {
                    user.CIC = string.Empty;
                    user.License = string.Empty;
                }
            }
            return users;
        }

        public async Task<ApplicationUser> GetByUserIdAsync()
            => await _autheticationRepository.FindByUserId(_userId);
        public async Task<ApplicationUser> GetByUserIdAsync(string userId)
        {
            var user = await _autheticationRepository.FindByUserId(userId);
            var userRequest = _httpContextAccessor.HttpContext?.User;
            if (userRequest != null && userRequest.IsInRole("User"))
            {
                user.CIC = string.Empty;
                user.License = string.Empty;
            }    
            return user;
        }


        public async Task<IList<string>> GetUserRolesAsync(ApplicationUser user)
            => await _autheticationRepository.GetUserRolesAsync(user);

        private async Task<string> SaveImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File cannot be null or empty");
            }

            // Đường dẫn tới thư mục lưu trữ ảnh
            var savePath = "./wwwroot/images/ImageUser/";
            var fileName = Path.GetFileName(file.FileName); // Đặt tên ngẫu nhiên để tránh trùng lặp
            var filePath = Path.Combine(savePath, fileName);

            try
            {
                // Tạo thư mục nếu chưa tồn tại
                if (!Directory.Exists(savePath))
                {
                    Directory.CreateDirectory(savePath);
                }

                // Lưu ảnh vào thư mục
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                // Trả về URL để lưu vào database

                return "images/ImageUser/" + fileName;
            }
            catch (Exception ex)
            {
                // Xử lý lỗi
                throw new Exception("Could not save file", ex);
            }
        }

        public async Task UpdateUserInfoAsync(ApplicationUser user, IFormFile License, IFormFile CIC, IFormFile Image)
        {
            try
            {
                var existingUser = await _autheticationRepository.FindByUserId(_userId);
                existingUser.Name = user.Name == null ? existingUser.Name : user.Name;
                existingUser.PhoneNumber = user.PhoneNumber == null ? existingUser.PhoneNumber : user.PhoneNumber;
                existingUser.Birthday = user.Birthday == null ? existingUser.Birthday : user.Birthday;
                existingUser.License = user.License == null ? existingUser.License : await SaveImage(License);
                existingUser.CIC = user.CIC == null ? existingUser.CIC : await SaveImage(CIC);
                existingUser.Image = user.Image == null ? existingUser.Image : await SaveImage(Image);
                await _autheticationRepository.UpdateAsync(existingUser);
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

        public async Task UpdateUserReportPointAsync(string userId, int reportPoint)
        {
            try
            {
                var user = await _autheticationRepository.FindByUserId(userId);
                user.ReportPoint += reportPoint;
                if (user.ReportPoint > 10)
                {
                    user.LockoutEnabled = true;
                    user.LockoutEnd = DateTime.Now.AddDays(7);
                }
                if (user.ReportPoint > 15)
                {
                    user.LockoutEnd = DateTime.Now.AddYears(1000);
                }
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

        public async Task UpdateUserLockAccountAsync(string userId)
        {
            try
            {
                var user = await _autheticationRepository.FindByUserId(userId);
                user.LockoutEnabled = !user.LockoutEnabled;
                if(user.LockoutEnabled)
                {
                    user.LockoutEnd = DateTime.Now.AddYears(1000);
                }
                else
                {
                    user.LockoutEnd = null;
                }
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
