using PayLend.Business.Managers.Mail;
using PayLend.Core.Entities;
using System.Collections.Generic;
using System.Linq;
using PayLend.Repository.IdentityRpository;

namespace Paylend.Business.Core
{
    public abstract class BusinessManager<TEntity> : BusinessManager
    {

        protected BusinessManager(IIdentityRepository repository):base(repository)
        {
        }
    }

    public class BusinessManager
    {
        private readonly IIdentityRepository Repository;
        public BusinessManager(IIdentityRepository repository)
        {
            Repository = repository;
        }

        public IList<Agency> Agencies { get { return Repository.GetAll<Agency>(x => x.IsActive).ToList(); } }
        public IList<AgencyGroup> Groups { get { return Repository.GetAll<AgencyGroup>().ToList(); } }

        public IList<VATExeptionCode> VATExeptionCode { get { return Repository.GetAll<VATExeptionCode>().ToList(); } }

        public IList<RetentionTax> RetentionTax { get { return Repository.GetAll<RetentionTax>().ToList(); } }

        public ProfitAntecipationParameters ProfitAntecipationParameters { get { return Repository.GetAll<ProfitAntecipationParameters>().FirstOrDefault(); } }

        public IList<LoanDailyFee> LoanDailyFee { get { return Repository.GetAll<LoanDailyFee>().ToList(); } }

        public EmailLogManager EmailLogManager { get { return Repository.GetAll<EmailLogManager>().FirstOrDefault(); } }

    }
}