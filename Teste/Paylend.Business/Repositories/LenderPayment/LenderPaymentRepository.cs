using PayLend.Repository;

namespace PayLend.Business.Repositories.LenderPayment
{
    public class LenderPaymentRepository : Repository<PayLend.Core.Entities.LenderPaymentPending>, ILenderPaymentRepository
    {
    }
}
