using GoWheels_WebAPI.Data;
using GoWheels_WebAPI.Models.Entities;
using GoWheels_WebAPI.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace GoWheels_WebAPI.Repositories
{
    public class FavoriteRepository : IFavoriteRepository
    {
        private readonly ApplicationDbContext _context;

        public FavoriteRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add(Favorite post)
        {
            _context.Favorites.AddAsync(post);
            _context.SaveChangesAsync();
        }

        public void Update(Favorite favorite)
        {
            _context.Entry(favorite).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public List<Favorite> GetAllByUserId(string userId)
            => _context.Favorites.AsNoTracking()
                                        .Where(p => p.UserId == userId && !p.IsDeleted)
                                        .Include(p => p.Post)
                                        .ThenInclude(post => post.Images)
                                        .Include(p => p.Post)
                                        .ThenInclude(post => post.CarType)
                                        .Include(p => p.Post)
                                        .ThenInclude(post => post.Company)
                                        .ToList();

        public Favorite? GetByPostId(int postId, string userId)
            => _context.Favorites.AsNoTracking()
                                        .Where(p => p.UserId == userId)
                                        .Include(p => p.Post)
                                        .Include(p => p.Post)
                                        .ThenInclude(post => post.Images)
                                        .Include(p => p.Post)
                                        .ThenInclude(post => post.CarType)
                                        .Include(p => p.Post)
                                        .ThenInclude(post => post.Company)
                                        .FirstOrDefault(p => p.PostId == postId);

        public Favorite GetById(int id, string userId)
            => _context.Favorites.AsNoTracking()
                                        .Where(p => p.UserId == userId)
                                        .FirstOrDefault(p => p.Id == id)
                                        ?? throw new NullReferenceException("Favorite not found");
    }
}
