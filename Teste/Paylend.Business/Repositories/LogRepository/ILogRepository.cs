using PayLend.Repository.Interface;

namespace PayLend.Business.Repositories.LogRepository
{
    public interface ILogRepository : IEmailRepository<PayLend.Core.Entities.ErrorLog>
    {
    }
}
