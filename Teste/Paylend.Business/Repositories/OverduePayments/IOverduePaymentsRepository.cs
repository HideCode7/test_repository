using PayLend.Repository.Interface;

namespace PayLend.Business.Repositories.OverduePayments
{
    public interface IOverduePaymentsRepository : IEmailRepository<PayLend.Core.Entities.OverduePayments>
    {
    }
}
