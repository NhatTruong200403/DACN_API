using GoWheels_WebAPI.Data;
using GoWheels_WebAPI.Models.Entities;
using GoWheels_WebAPI.Models.ViewModels;
using GoWheels_WebAPI.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace GoWheels_WebAPI.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly ApplicationDbContext _context;

        public PostRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void AddPostImages(List<string> postImageUrls, int postId)
        {
            foreach (var url in postImageUrls)
            {
                var postImage = new PostImage()
                {
                    PostId = postId,
                    Url = url
                };
                _context.PostImages.Add(postImage);
            }
            _context.SaveChanges();
        }

        public void DeletePostImages(int postId)
        {
            var imagesList = _context.PostImages.Where(p => p.PostId == postId).ToList();
            _context.PostImages.RemoveRange(imagesList);
        }

        public void Add(Post post)
        {
            _context.Posts.Add(post);
            _context.SaveChanges();

        }

        public void Update(Post post)
        {
            var existingPost = _context.ChangeTracker.Entries<Post>().FirstOrDefault(e => e.Entity.Id == post.Id);
            if (existingPost != null)
            {
                _context.Entry(existingPost.Entity).State = EntityState.Detached;
            }
            _context.Entry(post).State = EntityState.Modified;
            _context.SaveChanges();

            //detached tracking obj after modified
            _context.Entry(post).State = EntityState.Detached;
        }

        public void Delete(Post post)
        {
            _context.Entry(post).State = EntityState.Modified;
            post.IsDeleted = true;
            _context.SaveChanges();
        }

        public List<Post> GetAll()
            => _context.Posts.AsNoTracking()
                                    .Include(p => p.CarType)
                                    .Include(p => p.Company)
                                    .Include(p => p.Images)
                                    .Where(p => !p.IsDeleted)
                                    .ToList();


        public Post GetById(int id)
            => _context.Posts.AsNoTracking()
                                    .Include(p => p.CarType)
                                    .Include(p => p.Company)
                                    .Include(p => p.Images)
                                    .Where(p => !p.IsDeleted)
                                    .FirstOrDefault(p => p.Id == id)
                                    ?? throw new NullReferenceException("Post not found");



        public List<Post> GetPostsByUserId(string userId)
            => _context.Posts.Include(p => p.CarType)
                            .Include(p => p.Company)
                            .Include(p => p.Images)
                            .Where(p => !p.IsDeleted)
                            .ToList();
    }
}
