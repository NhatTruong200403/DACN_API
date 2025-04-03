using GoWheels_WebAPI.Models.Entities;
using GoWheels_WebAPI.Repositories.Interface;
using GoWheels_WebAPI.Service.Interface;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GoWheels_WebAPI.Service
{
    public class FavoriteService : IFavoriteService
    {
        private readonly IFavoriteRepository _favoriteRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _userId;

        public FavoriteService(IFavoriteRepository favoriteRepository,
                                IHttpContextAccessor httpContextAccessor)
        {
            _favoriteRepository = favoriteRepository;
            _httpContextAccessor = httpContextAccessor;
            _userId = _httpContextAccessor.HttpContext?.User?
                     .FindFirstValue(ClaimTypes.NameIdentifier) ?? "UnknownUser";
        }

        public List<Favorite> GetAll()
            => _favoriteRepository.GetAllByUserId(_userId);


        public void Add(Favorite favorite)
        {
            try
            {
                var existingFavorite = _favoriteRepository.GetByPostId(favorite.PostId, _userId);
                if (existingFavorite == null)
                {
                    favorite.UserId = _userId;
                    favorite.IsDeleted = false;
                    _favoriteRepository.Add(favorite);
                }
                else
                {
                    existingFavorite.IsDeleted = false;
                    _favoriteRepository.Update(existingFavorite);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Deleted(int id)
        {
            try
            {
                var favorite = _favoriteRepository.GetById(id, _userId);
                favorite.IsDeleted = true;
                _favoriteRepository.Update(favorite);
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
