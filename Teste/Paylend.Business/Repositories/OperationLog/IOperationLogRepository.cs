

using PayLend.Repository.Interface;

namespace PayLend.Business.Repositories.OperationLog
{
    public interface IOperationLogRepository : IEmailRepository<PayLend.Core.Entities.OperationLog>
    {
    }
}
