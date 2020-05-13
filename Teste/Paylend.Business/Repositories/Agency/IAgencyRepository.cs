using System;
using System.Collections.Generic;
using PayLend.Repository.Interface;

namespace PayLend.Business.Repositories.Agency
{
    public interface IAgencyRepository : IEmailRepository<PayLend.Core.Entities.Agency>
    {
        //Add any custom method
        IList<PayLend.Core.Entities.Agency> GetAllAgencyActive(bool isActive);
    }
}
