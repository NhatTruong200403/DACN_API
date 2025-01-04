using GoWheels_WebAPI.Data;
using GoWheels_WebAPI.Models.Entities;
using GoWheels_WebAPI.Models.ViewModels;
using GoWheels_WebAPI.Repositories.Interface;
using GoWheels_WebAPI.Service.Interface;
using GoWheels_WebAPI.Utilities;
using System.Security.Claims;

namespace GoWheels_WebAPI.Service
{
    public class PostService : IPostService
    {

        private readonly IPostRepository _postRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _context;
        private  readonly string _userId;

        public PostService(IPostRepository postRepository,
                            IHttpContextAccessor httpContextAccessor,
                            ApplicationDbContext context)
        {
            _postRepository = postRepository;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _userId = _httpContextAccessor.HttpContext?.User?
                        .FindFirstValue(ClaimTypes.NameIdentifier) ?? "UnknownUser";
        }

        public List<Post> GetAll()
            => _postRepository.GetAll();

        public Post GetById(int id)
            => _postRepository.GetById(id);

        public List<Post> GetAllByUserId()
            => _postRepository.GetPostsByUserId(_userId);

        public List<Post> GetAllByUserId(string userId)
            => _postRepository.GetPostsByUserId(userId);

        public List<Post> ApplyFilters(List<Post> query, SearchFilterModel filterModel)
        {
            if (!string.IsNullOrWhiteSpace(filterModel.Company))
            {
                query = query.Where(post => post.Company.Name == filterModel.Company).ToList();
            }

            if (!string.IsNullOrEmpty(filterModel.CarType))
            {
                query = query.Where(post => post.CarType.Name == filterModel.Company).ToList();
            }

            if (filterModel.Seat > 0)
            {
                query = query.Where(post => post.Seat == filterModel.Seat).ToList();
            }
            return query;
        }

        public void Add(Post post, IFormFile formFile, List<IFormFile> formFiles)
        {
            try
            {

                List<string> imgUrls = new List<string>();
                if (formFile != null && formFile.Length > 0)
                {
                    post.Image = SaveImage(formFile);
                }
                post.CreatedById = _userId;
                post.CreatedOn = DateTime.Now;
                post.IsDeleted = false;
                _postRepository.Add(post);
                if (formFiles.Count != 0)
                {
                    foreach (var file in formFiles)
                    {
                        var img = SaveImage(file);
                        imgUrls.Add(img);
                    }

                    _postRepository.AddPostImages(imgUrls, post.Id);
                }

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
            var savePath = "./wwwroot/images/posts/";
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

                return "images/posts/" + fileName;
            }
            catch (Exception ex)
            {
                // Xử lý lỗi
                throw new Exception("Could not save file", ex);
            }
        }

        public void Update(int id, Post post, IFormFile image)
        {
            try
            {
                var existingPost = _postRepository.GetById(id);
                var imageUrl = "./wwwroot/images/posts/" + Path.GetFileName(image.FileName);
                if (post.Image == null)
                {
                    post.Image = existingPost.Image;
                }
                else
                {
                    post.Image = SaveImage(image);
                }
                post.CreatedOn = existingPost.CreatedOn;
                post.CreatedById = existingPost.CreatedById;
                post.ModifiedById = existingPost.ModifiedById;
                post.ModifiedOn = existingPost.ModifiedOn;
                post.IsDeleted = existingPost.IsDeleted;
                post.Favorites = existingPost.Favorites;
                if (image != null && imageUrl != existingPost.Image)
                {
                    post.Image = SaveImage(image);
                }
                else
                {
                    post.Image = existingPost.Image;
                }
                post.Images = existingPost.Images;
                post.CarType = existingPost.CarType;
                post.Company = existingPost.Company;
               
                var isPostDataChange = EditHelper<Post>.HasChanges(post, existingPost);
                EditHelper<Post>.SetModifiedIfNecessary(post, isPostDataChange, existingPost, _userId);
                _postRepository.Update(post);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private bool IsImagesListChange(List<IFormFile> imageList, Post post)
        {
            foreach (var file in imageList)
            {
                var url = "images/posts/" + Path.GetFileName(file.FileName);
                var isChange = !post.Images.Any(i => i.Url.Equals(url));
                if (isChange)
                {
                    return true;
                }
            }
            return false;
        }
        public void UpdatePostImages(List<IFormFile> imageFiles, int postId)
        {
            try
            {
                var post = _postRepository.GetById(postId);
                var imageUrls = new List<string>();
                if (imageFiles.Count != 0)
                {
                    var isImagesChange = IsImagesListChange(imageFiles, post);
                    if (!isImagesChange)
                    {
                        return;
                    }
                    post.ModifiedById = _userId;
                    post.ModifiedOn = DateTime.Now;
                    _postRepository.Update(post);
                    _postRepository.DeletePostImages(post.Id);
                    foreach (var file in imageFiles)
                    {
                        var img = SaveImage(file);
                        imageUrls.Add(img);
                    }
                    _postRepository.AddPostImages(imageUrls, postId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void UpdateRideNumber(int postId, int rideNumber)
        {
            try
            {
                var post = _postRepository.GetById(postId);
                post.RideNumber += rideNumber;
                _postRepository.Update(post);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public void DeleteById(int id)
        {
            try
            {
                var post = _postRepository.GetById(id);
                post.IsDeleted = true;
                post.ModifiedById = _userId;
                post.ModifiedOn = DateTime.Now;
                _postRepository.Update(post);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
