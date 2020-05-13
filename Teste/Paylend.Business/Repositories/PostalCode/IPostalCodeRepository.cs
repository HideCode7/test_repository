using PayLend.Repository.Interface;

namespace PayLend.Business.Repositories.PostalCode
{
    public interface IPostalCodeRepository : IEmailRepository<PayLend.Core.Entities.PostalCodeAddress>
    {
    }
}
