using GoWheels_WebAPI.Models.Entities;
using GoWheels_WebAPI.Repositories.Interface;
using GoWheels_WebAPI.Service.Interface;
using GoWheels_WebAPI.Utilities;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
namespace GoWheels_WebAPI.Service
{
    public class AmenityService : IAmenityService
    {
        public readonly IGenericRepository<Amenity> _amenityRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _userId;
        public AmenityService(IGenericRepository<Amenity> amenityRepository, IHttpContextAccessor httpContextAccessor)
        {
            _amenityRepository = amenityRepository;
            _httpContextAccessor = httpContextAccessor;
            _userId = _httpContextAccessor.HttpContext?.User?
                        .FindFirstValue(ClaimTypes.NameIdentifier) ?? "UnknownUser";
        }

        public List<Amenity> GetAll()
            => _amenityRepository.GetAll();


        public Amenity GetById(int id)
            => _amenityRepository.GetById(id);

        public void Add(Amenity amenity, IFormFile formFile)
        {
            try
            {
                if(amenity.Name == null)
                {
                    throw new InvalidDataException("Name is required");
                }    
                amenity.CreatedById = _userId;
                amenity.CreatedOn = DateTime.Now;
                amenity.IsDeleted = false;
                if (formFile != null && formFile.Length > 0)
                {
                    amenity.IconImage = SaveImage(formFile);
                }
                _amenityRepository.Add(amenity);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private string SaveImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File cannot be null or empty");
            }

            // Đường dẫn tới thư mục lưu trữ ảnh
            var savePath = "./wwwroot/images/amenities/";
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
                    file.CopyTo(fileStream);
                }

                // Trả về URL để lưu vào database
                return "images/amenities/" + fileName;
            }
            catch (Exception ex)
            {
                // Xử lý lỗi
                throw new Exception("Could not save file", ex);
            }
        }

        public void DeletedById(int id)
        {
            try
            {
                var amenity = _amenityRepository.GetById(id);
                amenity.ModifiedById = _userId;
                amenity.ModifiedOn = DateTime.Now;
                amenity.IsDeleted = true;
                _amenityRepository.Update(amenity);
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

        public void Update(int id, Amenity amenity, IFormFile formFile)
        {

            try
            {
                string imageUrl = "";
                if (amenity.IconImage != null)
                {
                    imageUrl = "./wwwroot/images/amenities/" + Path.GetFileName(formFile.FileName);
                }
                var existingAmenity = _amenityRepository.GetById(id);

                if (formFile != null && formFile.Length > 0 && imageUrl != existingAmenity.IconImage)
                {
                    amenity.IconImage = SaveImage(formFile);
                }
                else
                {
                    amenity.IconImage = existingAmenity.IconImage;
                }
                amenity.CreatedOn = existingAmenity.CreatedOn;
                amenity.CreatedById = existingAmenity.CreatedById;
                amenity.ModifiedById = existingAmenity.ModifiedById;
                amenity.ModifiedOn = existingAmenity.ModifiedOn;
                var isValueChange = EditHelper<Amenity>.HasChanges(amenity, existingAmenity);
                EditHelper<Amenity>.SetModifiedIfNecessary(amenity, isValueChange, existingAmenity, _userId);
                _amenityRepository.Update(amenity);
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
