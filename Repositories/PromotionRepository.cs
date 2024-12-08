﻿using AutoMapper;
using GoWheels_WebAPI.Data;
using GoWheels_WebAPI.Models.Entities;
using GoWheels_WebAPI.Models.ViewModels;
using GoWheels_WebAPI.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace GoWheels_WebAPI.Repositories
{
    public class PromotionRepository : IPromotionRepository
    {
        private readonly ApplicationDbContext _context;
        public PromotionRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<List<Promotion>> GetAllAsync()
            => await _context.Promotions.AsNoTracking()
                                        .Include(p => p.PostPromotions)
                                        .ThenInclude(p => p.Post)
                                        .ToListAsync();



        public async Task<Promotion> GetByIdAsync(int id) 
            => await _context.Promotions.AsNoTracking()
                                        .Include(p => p.PostPromotions)
                                        .ThenInclude(p => p.Post)
                                        .FirstOrDefaultAsync(p => p.Id == id)
                                        ?? throw new NullReferenceException("Promotion not found");

        public async Task<Promotion> GetUserPromotionByIdAsync(int id, string userId)
            => await _context.Promotions.AsNoTracking()
                                        .Include(p => p.PostPromotions)
                                        .ThenInclude(p => p.Post)
                                        .FirstOrDefaultAsync(p => p.Id == id && p.CreatedById == userId)
                                        ?? throw new NullReferenceException("Promotion not found");

        public async Task<List<Promotion>> GetPromotionsByUserIdAsync(string userId)
                    => await _context.Promotions.AsNoTracking()
                                                .Include(p => p.PostPromotions)
                                                .ThenInclude(p => p.Post)
                                                .Where(p => !p.IsDeleted && p.CreatedById == userId && !p.IsAdminPromotion)
                                                .ToListAsync();
        public async Task<List<Promotion>> GetAllAdminPromotionsAsync()
                    => await _context.Promotions.AsNoTracking()
                                                .Include(p => p.PostPromotions)
                                                .ThenInclude(p => p.Post)
                                                .Where(p => !p.IsDeleted && p.IsAdminPromotion)
                                                .ToListAsync();
        public async Task<List<Promotion>> GetAllAdminPromotionsByUserIdAsync(string userId)
            => await _context.Promotions.AsNoTracking()
                                        .Include(p => p.PostPromotions)
                                        .ThenInclude(p => p.Post)
                                        .Where(p => !p.IsDeleted 
                                                    && p.IsAdminPromotion 
                                                    && !p.Bookings.Any(b => b.UserId == userId))
                                        .ToListAsync();


        public async Task<List<Promotion>> GetAllUserPromotionsAsync()
            => await _context.Promotions.AsNoTracking()
                                        .Include(p => p.PostPromotions)
                                        .ThenInclude(p => p.Post)
                                        .Where(p => !p.IsAdminPromotion)
                                        .ToListAsync();

        public async Task AddAsync(Promotion promotion)
        {
            await _context.Promotions.AddAsync(promotion);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Promotion promotion)
        {
            _context.Entry(promotion).State = EntityState.Modified;
            promotion.IsDeleted = true;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Promotion promotion)
        {
            var existingPromotion = await _context.Promotions.AsNoTracking()
                                      .FirstOrDefaultAsync(p => p.Id == promotion.Id);

            if (existingPromotion != null)
            {
                _context.Entry(existingPromotion).State = EntityState.Detached;
            }

            // Gán lại trạng thái cho đối tượng là modified và lưu các thay đổi
            _context.Entry(promotion).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
