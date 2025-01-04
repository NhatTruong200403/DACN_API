using GoWheels_WebAPI.Models.Entities;
using GoWheels_WebAPI.Repositories.Interface;
using GoWheels_WebAPI.Service.Interface;
using GoWheels_WebAPI.Utilities;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GoWheels_WebAPI.Service
{
    public class PromotionService : IPromotionService
    {
        private readonly IPromotionRepository _salePromotionRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _userId;
        public PromotionService(IPromotionRepository salepromotionRepository,
                                    IHttpContextAccessor contextAccessor)
        {
            _salePromotionRepository = salepromotionRepository;
            _httpContextAccessor = contextAccessor;
            _userId = _httpContextAccessor.HttpContext?.User?
                        .FindFirstValue(ClaimTypes.NameIdentifier) ?? "UnknownUser";
        }

        public List<Promotion> GetAllByAdmin()
            => _salePromotionRepository.GetAll();

        public List<Promotion> GetAllByUser()
            => _salePromotionRepository.GetAllByUser();

        public Promotion GetById(int id)
            => _salePromotionRepository.GetById(id);

        public void Add(Promotion promotion)
        {
            try
            {
                promotion.IsDeleted = false;
                _salePromotionRepository.Add(promotion);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Update(int id, Promotion promotion)
        {
            try
            {

                var existingPromotion = _salePromotionRepository.GetById(id);
                promotion.IsDeleted = existingPromotion.IsDeleted;
                var isValueChange = EditHelper<Promotion>.HasChanges(promotion, existingPromotion);
                EditHelper<Promotion>.SetModifiedIfNecessary(promotion, isValueChange, existingPromotion, _userId);
                _salePromotionRepository.Update(promotion);
            }
            catch (NullReferenceException nullEx)
            {
                throw new NullReferenceException(nullEx.Message);
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

        public void DeletedById(int id)
        {
            try
            {
                var promotion = _salePromotionRepository.GetById(id);
                promotion.IsDeleted = true;
                _salePromotionRepository.Update(promotion);
            }
            catch (NullReferenceException nullEx)
            {
                throw new NullReferenceException(nullEx.Message);
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
