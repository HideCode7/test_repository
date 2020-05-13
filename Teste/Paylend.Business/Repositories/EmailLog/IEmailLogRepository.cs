using System;
using PayLend.Repository.Interface;

namespace PayLend.Business.Repositories.EmailLog
{

    public interface IEmailLogRepository : IEmailRepository<PayLend.Core.Entities.EmailLog>
    {
    }
}
