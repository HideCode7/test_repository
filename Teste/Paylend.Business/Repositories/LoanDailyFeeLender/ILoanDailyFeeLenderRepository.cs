using PayLend.Repository.Interface;

namespace PayLend.Business.Repositories.LoanDailyFeeLender
{
    public interface ILoanDailyFeeLenderRepository : IEmailRepository<PayLend.Core.Entities.LoanDailyFeeLender>
    {
    }
}
