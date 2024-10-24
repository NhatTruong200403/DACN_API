using AutoMapper;
using GoWheels_WebAPI.Models.Entities;
using GoWheels_WebAPI.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GoWheels_WebAPI.Service
{
    public class PostPromotionService
    {
        private readonly PostPromotionReposity _postPromotionRepository;

        public PostPromotionService(PostPromotionReposity postPromotionRepository)
        {
            _postPromotionRepository = postPromotionRepository;
        }

        public async Task AddAsync(int promotionId, int postId)
        {
            try
            {
                var postPromotion = new PostPromotion()
                {
                    PostId = postId,
                    PromotionId = promotionId
                };
                await _postPromotionRepository.AddAsync(postPromotion);
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
