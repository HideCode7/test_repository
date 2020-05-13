using PayLend.Repository.Interface;

namespace PayLend.Business.Repositories.Promotion
{
    public interface IPromotionRepository : IEmailRepository<PayLend.Core.Entities.Promotions.Promotion>
    {
    }
}
