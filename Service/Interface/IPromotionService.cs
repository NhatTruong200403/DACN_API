using GoWheels_WebAPI.Models.Entities;

namespace GoWheels_WebAPI.Service.Interface
{
    public interface IPromotionService
    {
        List<Promotion> GetAllByAdmin();
        List<Promotion> GetAllByUser();
        Promotion GetById(int id);
        void Add(Promotion promotion);
        void Update(int id, Promotion promotion);
        void DeletedById(int id);

    }
}
