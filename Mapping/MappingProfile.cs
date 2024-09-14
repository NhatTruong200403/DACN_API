using AutoMapper;
using GoWheels_WebAPI.Models.DTOs;
using GoWheels_WebAPI.Models.Entities;

namespace GoWheels_WebAPI.Mapping
{
    public class MappingProfile : Profile 
    {
        public MappingProfile() 
        {
            CreateMap<AmenityDTO, Amentity>();
            CreateMap<Amentity, AmenityDTO>();
            CreateMap<CarType, CarTypeDTO>();
            CreateMap<CompanyDTO, Company>();
            CreateMap<Company, CompanyDTO>();
            CreateMap<CarTypeDetailDTO, CarTypeDetail>();
            CreateMap<CarTypeDetail, CarTypeDetailDTO>();
            CreateMap<Promotion, PromotionType>();
            CreateMap<PromotionType, Promotion>();
        }
    }
}
