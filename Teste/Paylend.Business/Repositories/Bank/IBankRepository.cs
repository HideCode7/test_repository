using PayLend.Repository.Interface;

namespace PayLend.Business.Repositories.Bank
{
    public interface IBankRepository : IEmailRepository<PayLend.Core.Entities.Bank>
    {
    }
}
