
using PayLend.Repository.Interface;

namespace PayLend.Business.Repositories.Invoice
{
    public interface IInvoiceRepository : IEmailRepository<PayLend.Core.Entities.Invoices>
    {
    }
}
