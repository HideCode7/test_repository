using System.Collections.Generic;
using System.Linq;
using PayLend.Business.Repositories.Agency;
using PayLend.Core.Entities;
using PayLend.Repository;

namespace PayLend.Database.Repositories
{
    public class AgencyRepository : Repository<Agency>, IAgencyRepository
    {
        //Add any custom method
        public IList<Agency> GetAllAgencyActive(bool isActive)
        {
            return Db.Agency.Where(x => x.IsActive == isActive).ToList();
        }
    }
}
