using PayLend.Repository.Interface;

namespace PayLend.Business.Repositories.LenderSummary
{
    public interface ILenderSummaryRepository : IEmailRepository<PayLend.Core.Entities.LenderSummary>
    {
    }
}
