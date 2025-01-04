using GoWheels_WebAPI.Models.Entities;

namespace GoWheels_WebAPI.Repositories.Interface
{
    public interface IPromotionRepository : IGenericRepository<Promotion>
    {
        List<Promotion> GetAllByUser();

    }
}
