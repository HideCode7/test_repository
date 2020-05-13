using PayLend.Core.Entities;
using PayLend.Repository.Interface;

namespace PayLend.Business.Repositories.PayLendUser
{
    public interface IPayLendUserRepository : IEmailRepository<PayLend.Core.Entities.PayLendUser>
    {
    }
}
