using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PayLend.Repository.Interface;

namespace PayLend.Business.Repositories.BackOfficeUserAgencies
{
    public interface IBackOfficeUserAgenciesRepository : IEmailRepository<PayLend.Core.Entities.BackOfficeUserAgency>
    {
    }

}
