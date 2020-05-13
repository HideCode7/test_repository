
using PayLend.Repository.Interface;

namespace PayLend.Business.Repositories.BankUser
{
    public interface IBankUserRepository : IEmailRepository<PayLend.Core.Entities.BankUser>
    {
    }
}
