using PayLend.Repository;

namespace PayLend.Business.Repositories.LogRepository
{
    public class LogRepository : Repository<PayLend.Core.Entities.ErrorLog>, ILogRepository
    {
    }
}
