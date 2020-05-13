
using PayLend.Repository.Interface;

namespace PayLend.Business.Repositories.LenderPayment
{
    public interface ILenderPaymentRepository : IEmailRepository<PayLend.Core.Entities.LenderPaymentPending>
    {
    }
}
