

using PayLend.Repository.Interface;

namespace PayLend.Business.Repositories.ExtractMovements
{
    public interface IExtractMovementsRepository : IEmailRepository<PayLend.Core.Entities.ExtractMovements>
    {
    }
}
