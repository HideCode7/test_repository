

using System;
using System.Linq;
using System.Linq.Expressions;
using PayLend.Repository;

namespace PayLend.Business.Repositories.LoanDailyFeeLender
{
    public class LoanDailyFeeLenderRepository : Repository<PayLend.Core.Entities.LoanDailyFeeLender>, ILoanDailyFeeLenderRepository
    {
    }
}
