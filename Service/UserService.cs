using GoWheels_WebAPI.Models.Entities;
using GoWheels_WebAPI.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace GoWheels_WebAPI.Service
{
    public class UserService
    {
        private readonly IUserRepository _autheticationRepository;
        private readonly DriverService _driverService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _userId;
        private readonly IConfiguration _config;

        public UserService(IUserRepository autheticationRepository,
                            DriverService driverService,
                            IHttpContextAccessor httpContextAccessor,
                            IConfiguration config)
        {
            _autheticationRepository = autheticationRepository;
            _driverService = driverService;
            _httpContextAccessor = httpContextAccessor;
            _userId = _httpContextAccessor.HttpContext?.User?
                        .FindFirstValue(ClaimTypes.NameIdentifier) ?? "UnknownUser";
            _config = config;
        }

        public async Task<List<ApplicationUser>> GetAllDriverSubmitAsync()
            => await _autheticationRepository.GetAllSubmitDriversAsync();

        public async Task<string> SaveImage(IFormFile file)
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
                var existingUser = await _autheticationRepository.FindByUserIdAsync(_userId);
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
                var user = await _autheticationRepository.FindByUserIdAsync(userId);
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

        public async Task SendDriverSubmitAsync()
        {

            try
            {
                var user = await _autheticationRepository.FindByUserIdAsync(_userId);
                var userRole = _httpContextAccessor.HttpContext!.User.IsInRole("Driver");
                if (userRole)
                {
                    throw new InvalidOperationException("Account is already a driver");
                }
                if (user.IsSubmitDriver)
                {
                    throw new InvalidOperationException("Submit already sent");
                }
                if (user.License.IsNullOrEmpty() || user.CIC.IsNullOrEmpty())
                {
                    throw new InvalidOperationException("License & CIC required");
                }
                user.IsSubmitDriver = true;
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
                throw new InvalidOperationException(operationEx.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task ConfirmDriverSubmit(string userId, bool isAccept)
        {
            try
            {
                var user = await _autheticationRepository.FindByUserIdAsync(userId);
                user.IsSubmitDriver = false;  
                user.isDriver = isAccept;
                
                await _autheticationRepository.UpdateAsync(user);
                if (isAccept)
                {
                    await _driverService.AddAsync(user);
                }
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
                throw new InvalidOperationException(operationEx.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
    }
}
