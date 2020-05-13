using PayLend.Repository.Interface;

namespace PayLend.Business.Repositories.TopUpProcessed
{
    public interface ITopUpProcessedRepository : IEmailRepository<PayLend.Core.Entities.TopUpProcessed>
    {
    }
}
