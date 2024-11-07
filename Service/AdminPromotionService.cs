using AutoMapper;
using GoWheels_WebAPI.Models.DTOs;
using GoWheels_WebAPI.Models.Entities;
using GoWheels_WebAPI.Models.ViewModels;
using GoWheels_WebAPI.Repositories;
using GoWheels_WebAPI.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace GoWheels_WebAPI.Service
{
    public class AdminPromotionService
    {
        private readonly PromotionRepository _salepromotionRepository;
        private readonly PostPromotionService _postPromotionService;
        private readonly PostService _postService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _userId;
        private readonly IMapper _mapper;
        public AdminPromotionService(PromotionRepository salepromotionRepository,
                                    PostPromotionService postPromotionService,
                                    PostService postService,
                                    IHttpContextAccessor contextAccessor,
                                    IMapper mapper)
        {
            _salepromotionRepository = salepromotionRepository;
            _postPromotionService = postPromotionService;
            _postService = postService;
            _httpContextAccessor = contextAccessor;
            _mapper = mapper;
            _userId = _httpContextAccessor.HttpContext?.User?
                        .FindFirstValue(ClaimTypes.NameIdentifier) ?? "UnknownUser";
        }
        private bool IsAdminRole()
        {
            var user = _httpContextAccessor.HttpContext?.User!;
            if (user.IsInRole("Admin"))
            {
                return true;
            }
            return false;
        }
        public async Task<List<Promotion>> GetAllAsync()
            => await _salepromotionRepository.GetAllAsync();


        public async Task<List<Promotion>> GetAllAdminPromotionsAsync()
        {
            var promoList = await _salepromotionRepository.GetAllAdminPromotionsAsync();
            return promoList;
        }

        public async Task<Promotion> GetByIdAsync(int id)
            => await _salepromotionRepository.GetByIdAsync(id);

        public async Task AddAsync(Promotion promotion)
        {
            try
            {
                var isAdmin = IsAdminRole(); 
                promotion.IsAdminPromotion = true;
                promotion.CreatedById = _userId;
                promotion.CreatedOn = DateTime.Now;
                promotion.IsDeleted = false;
                await _salepromotionRepository.AddAsync(promotion);
            }
            catch(NullReferenceException nullEx)
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

        public async Task UpdateAsync(int id, Promotion promotion)
        {
            try
            {

                var existingPromotion = await _salepromotionRepository.GetByIdAsync(id);
                promotion.CreatedOn = existingPromotion.CreatedOn;
                promotion.CreatedById = existingPromotion.CreatedById;
                promotion.ModifiedById = existingPromotion.ModifiedById;
                promotion.ModifiedOn = existingPromotion.ModifiedOn;
                promotion.PostPromotions = existingPromotion.PostPromotions;
                promotion.IsDeleted = existingPromotion.IsDeleted;
                promotion.IsAdminPromotion = existingPromotion.IsAdminPromotion;
                var isValueChange = EditHelper<Promotion>.HasChanges(promotion, existingPromotion);
                EditHelper<Promotion>.SetModifiedIfNecessary(promotion, isValueChange, existingPromotion, _userId);
                await _salepromotionRepository.UpdateAsync(promotion);
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

        public async Task DeletedByIdAsync(int id)
        {
            try
            {
                var promotion = await _salepromotionRepository.GetByIdAsync(id);
                promotion.ModifiedById = _userId;
                promotion.ModifiedOn = DateTime.Now;
                promotion.IsDeleted = true;
                await _salepromotionRepository.UpdateAsync(promotion); 
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
