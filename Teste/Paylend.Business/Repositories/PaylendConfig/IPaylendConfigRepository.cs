using PayLend.Repository.Interface;

namespace PayLend.Business.Repositories.PaylendConfig
{
    public interface IPaylendConfigRepository : IEmailRepository<PayLend.Core.Entities.PayLendConfig>
    {
    }
}
