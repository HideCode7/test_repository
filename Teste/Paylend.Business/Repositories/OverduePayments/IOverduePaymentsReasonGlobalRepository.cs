using PayLend.Repository.Interface;

namespace PayLend.Business.Repositories.OverduePayments
{
    public interface IOverduePaymentsReasonGlobalRepository : IEmailRepository<PayLend.Core.Entities.OverduePaymentsReasonGlobal>
    {
    }
}
