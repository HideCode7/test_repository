
using PayLend.Repository.Interface;

namespace PayLend.Business.Repositories.FreeFee
{
    public interface IFreeFeeRepository : IEmailRepository<PayLend.Core.Entities.Fee.FreeFee>
    {
    }
}
