using GoWheels_WebAPI.Models.Entities;
using GoWheels_WebAPI.Models.ViewModels;

namespace GoWheels_WebAPI.Service.Interface
{
    public interface IPostService
    {

        List<Post> GetAll();
        List<Post> ApplyFilters(List<Post> posts,SearchFilterModel filterModel);
        Post GetById(int id);
        List<Post> GetAllByUserId();
        List<Post> GetAllByUserId(string userId);
        void Add(Post post, IFormFile formFile, List<IFormFile> formFiles);
        void Update(int id, Post post, IFormFile image);
        void UpdatePostImages(List<IFormFile> imageFiles, int postId);
        void UpdateRideNumber(int postId, int rideNumber);
        void DeleteById(int id);
    }
}
