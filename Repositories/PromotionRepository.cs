using AutoMapper;
using GoWheels_WebAPI.Data;
using GoWheels_WebAPI.Models.Entities;
using GoWheels_WebAPI.Models.ViewModels;
using GoWheels_WebAPI.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace GoWheels_WebAPI.Repositories
{
    public class PromotionRepository : IPromotionRepository
    {
        private readonly ApplicationDbContext _context;
        public PromotionRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        public List<Promotion> GetAll()
            => _context.Promotions.AsNoTracking()
                                        .ToList();



        public Promotion GetById(int id)
            => _context.Promotions.AsNoTracking()
                                        .FirstOrDefault(p => p.Id == id)
                                        ?? throw new NullReferenceException("Promotion not found");

        public List<Promotion> GetAllByUser()
            => _context.Promotions.AsNoTracking()
                                        .Where(p => !p.IsDeleted
                                                    && p.ExpiredDate > DateTime.Now)
                                        .ToList();

        public void Add(Promotion promotion)
        {
            _context.Promotions.Add(promotion);
            _context.SaveChanges();
        }

        public void Delete(Promotion promotion)
        {
            _context.Entry(promotion).State = EntityState.Modified;
            promotion.IsDeleted = true;
            _context.SaveChanges();
        }

        public void Update(Promotion promotion)
        {
            var existingPromotion = _context.ChangeTracker.Entries<Promotion>()
                                                            .FirstOrDefault(e => e.Entity.Id == promotion.Id);

            if (existingPromotion != null)
            {
                _context.Entry(existingPromotion).State = EntityState.Detached;
            }

            // Gán lại trạng thái cho đối tượng là modified và lưu các thay đổi
            _context.Entry(promotion).State = EntityState.Modified;
            _context.SaveChanges();
        }
    }
}
