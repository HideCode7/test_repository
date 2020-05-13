

using PayLend.Repository.Interface;

namespace PayLend.Business.Repositories.Loans
{
    public interface ILoansRepository : IEmailRepository<PayLend.Core.Entities.Loans>
    {
    }
}
