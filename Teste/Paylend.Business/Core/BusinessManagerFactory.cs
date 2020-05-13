using PayLend.Business.Managers.BackOffice;
using PayLend.Business.Managers.Carregamentos;
using PayLend.Business.Managers.Country;
using PayLend.Business.Managers.Dashboard.Borrower;
using PayLend.Business.Managers.Dashboard.Lender;
using PayLend.Business.Managers.FinancingProfiles;
using PayLend.Business.Managers.LoanFractionedAmount;
using PayLend.Business.Managers.Mail;
using PayLend.Business.Managers.OverduePayments;
using PayLend.Business.Managers.PayLendCaptiveBalance;
using PayLend.Business.Managers.PayLendCard;
using PayLend.Business.Managers.ProfitAntecipation;
using PayLend.Business.Managers.RejectionForReasonProfitAntecipation;
using PayLend.Business.Managers.Storage;
using PayLend.Business.Managers.User;
using PayLend.Framework;
using System;

namespace Paylend.Business.Core
{
    public class BusinessManagerFactory : IBusinessManagerFactory
    {
        public IProfitAntecipationManager ProfitAntecipationManager => CompositionRoot.Resolve<IProfitAntecipationManager>();
        public IPayLendUserManager PayLendUserManager => CompositionRoot.Resolve<IPayLendUserManager>();

        public ICarregamentoManager AccountTransactionManager => CompositionRoot.Resolve<ICarregamentoManager>();

        public ILenderDashboardManager LenderDashboardManager => CompositionRoot.Resolve<ILenderDashboardManager>();

        public IFinancingProfileManager FinancingProfileManager => CompositionRoot.Resolve<IFinancingProfileManager>();

        public IBorrowerDashbardManager BorrowerDashboardManager => CompositionRoot.Resolve<IBorrowerDashbardManager>();

        public IPayLendCaptiveBalanceManager PayLendCaptiveBalanceManager => CompositionRoot.Resolve<IPayLendCaptiveBalanceManager>();

        public IBackOfficeManager BackOfficeManager => CompositionRoot.Resolve<IBackOfficeManager>();

        public IAgencyManager AgencyManager => CompositionRoot.Resolve<IAgencyManager>();

        public ILenderPaymentManager LenderPaymentManager => CompositionRoot.Resolve<ILenderPaymentManager>();

        public IExtractMovementsManager ExtractMovementsManager => CompositionRoot.Resolve<IExtractMovementsManager>();

        public IAzureStorageManager AzureStorageManager => CompositionRoot.Resolve<IAzureStorageManager>();

        public IEmailLogManager EmailLogManager => CompositionRoot.Resolve<IEmailLogManager>();

        public ILoanFractionedAmountManager LoanFractionedAmountManager => CompositionRoot.Resolve<ILoanFractionedAmountManager>();

        public IOverduePaymentsManager OverduePaymentsManager => CompositionRoot.Resolve<IOverduePaymentsManager>();

        public ICountryManager CountryManager => CompositionRoot.Resolve<ICountryManager>();

        public IProfitAntecipationRejectionReasonManager ProfitAntecipationRejectionReasonManager => CompositionRoot.Resolve<IProfitAntecipationRejectionReasonManager>();

        public IPayLendCardManager PayLendCardManager => CompositionRoot.Resolve<IPayLendCardManager>();

        //public CarregamentosManager<T> GetManager<T>()
        //{
        //    return CompositionRoot.Resolve<CarregamentosManager<T>>();
        //}
    }

    public interface IBusinessManagerFactory
    {
        IProfitAntecipationManager ProfitAntecipationManager { get; }

        IPayLendUserManager PayLendUserManager { get; }

        IPayLendCaptiveBalanceManager PayLendCaptiveBalanceManager { get; }

        ICarregamentoManager AccountTransactionManager { get; }

        ILenderDashboardManager LenderDashboardManager { get; }

        IFinancingProfileManager FinancingProfileManager { get; }

        IBorrowerDashbardManager BorrowerDashboardManager { get; }

        IBackOfficeManager BackOfficeManager { get; }

        IAgencyManager AgencyManager { get; }

        ILenderPaymentManager LenderPaymentManager { get; }

        IExtractMovementsManager ExtractMovementsManager { get; }

        IAzureStorageManager AzureStorageManager { get; }

        IOverduePaymentsManager OverduePaymentsManager { get; }

        IEmailLogManager EmailLogManager { get; }
        //CarregamentosManager<T> GetManager<T>();

        ILoanFractionedAmountManager LoanFractionedAmountManager { get; }

        ICountryManager CountryManager { get; }

        IProfitAntecipationRejectionReasonManager ProfitAntecipationRejectionReasonManager { get; }

        IPayLendCardManager PayLendCardManager { get; }
    }
}