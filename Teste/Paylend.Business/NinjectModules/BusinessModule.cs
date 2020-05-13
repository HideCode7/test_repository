using Ninject.Modules;
using Paylend.Business.Core;
using PayLend.Business.Managers.Admin;
using PayLend.Business.Managers.BackOffice;
using PayLend.Business.Managers.OverduePayments;
using PayLend.Business.Managers.Country;
using PayLend.Business.Managers.Dashboard.Borrower;
using PayLend.Business.Managers.Dashboard.Lender;
using PayLend.Business.Managers.FinancingProfiles;
using PayLend.Business.Managers.Loan;
using PayLend.Business.Managers.LoanFractionedAmount;
using PayLend.Business.Managers.Mail;
using PayLend.Business.Managers.PayLendCaptiveBalance;
using PayLend.Business.Managers.PayLendConfig;
using PayLend.Business.Managers.LoanDailyFee;
using PayLend.Business.Managers.LenderPaymentPendingGroupBusiness;
using PayLend.Business.Managers.ProfitAntecipation;
using PayLend.Business.Managers.RejectionForReasonProfitAntecipation;
using PayLend.Business.Managers.User;
using PayLend.Business.Managers.BackOfficeUserManager;
using PayLend.Business.Managers.InvoiceManager;
using PayLend.Business.Managers.PayLendCard;
using PayLend.Business.Managers.Log;
using PayLend.Business.Managers.Permissions;
using PayLend.Business.Managers.ResetPasswordManager;
using PayLend.Business.Managers.AccountCodeConfirmationsManager;
using PayLend.Business.Managers.Agency;
using PayLend.Business.Managers.AgencyGroup;
using PayLend.Business.Managers.AntecipationData;
using PayLend.Business.Managers.BackOfficeTopUp;
using PayLend.Business.Managers.ExternalAPIManager.EuPagoAPI;
using PayLend.Business.Managers.BankTransfer;
using PayLend.Business.Managers.PostalCodeManagers;
using PayLend.Business.Managers.Bank;
using PayLend.Business.Managers.BankUser;
using PayLend.Business.Managers.BorrowerAgencyData;
using PayLend.Business.Managers.Fees;
using PayLend.Business.Managers.Storage;
using PayLend.Business.Managers.ExternalAPIManager.KeyInvoice;
using PayLend.Business.Managers.BorrowerCompanyData;
using PayLend.Business.Managers.Comission;
using PayLend.Business.Managers.LenderBalance;
using PayLend.Business.Managers.LenderSummary;
using PayLend.Business.Managers.LoanDailyFeeLenderManager;
using PayLend.Business.Managers.MBReference;
using PayLend.Business.Managers.OperationLog;
using PayLend.Business.Managers.PayLendProductManager;
using PayLend.Business.Managers.ProfitAntecipationParameters;
using PayLend.Business.Managers.ProfitWithoutStartManager;
using PayLend.Business.Managers.Promotion;
using PayLend.Business.Managers.RetentionTax;
using PayLend.Business.Managers.TopUpProcessed;
using PayLend.Business.Managers.User_Permission;
using PayLend.Business.Managers.VATExeptionCode;
using PayLend.Core.Entities;

namespace PayLend.Business.NinjectModules
{
    public class BusinessModule : NinjectModule
    {
        public override void Load()
        {
            if (Kernel != null)
            {
                Kernel.Bind<IBusinessManagerFactory>().To<BusinessManagerFactory>();
                Kernel.Bind<IVATExeptionCodeManager>().To<VATExeptionCodeManager>();
                Kernel.Bind<IUser_PermissionManager>().To<User_PermissionManager>();
                Kernel.Bind<IPayLendUserManager>().To<PayLendUserManager>();
                Kernel.Bind<ITopUpProcessedManager>().To<TopUpProcessedManager>();
                Kernel.Bind<IRetentionTaxManager>().To<RetentionTaxManager>();
                Kernel.Bind<IResetPasswordManager>().To<ResetPasswordManager>();
                Kernel.Bind<IPromotionManager>().To<PromotionManager>();
                Kernel.Bind<IProfitWithoutStartManager>().To<ProfitWithoutStartManager>();
                Kernel.Bind<IProfitAntecipationRejectionReasonManager>().To<ProfitAntecipationRejectionReasonManager>();
                Kernel.Bind<IProfitAntecipationParametersManager>().To<ProfitAntecipationParametersManager>();
                Kernel.Bind<IProfitAntecipationManager>().To<ProfitAntecipationManager>();
                Kernel.Bind<IPostalCodeManager>().To<PostalCodeManager>();
                Kernel.Bind<IPermissionManager>().To<PermissionManager>();
                Kernel.Bind<IPayLendProductManager>().To<PayLendProductManager>();
                Kernel.Bind<IPayLendConfigManager>().To<PayLendConfigManager>();
                Kernel.Bind<IPayLendCardManager>().To<PayLendCardManager>();
                Kernel.Bind<IPayLendCaptiveBalanceManager>().To<PayLendCaptiveBalanceManager>();
                Kernel.Bind<IOverduePaymentsManager>().To<OverduePaymentsManager>();
                Kernel.Bind<IOverduePaymentsReasonGlobalManager>().To<OverduePaymentsReasonGlobalManager>();
                Kernel.Bind<IOperationLogManager>().To<OperationLogManager>();
                Kernel.Bind<IMBReferenceManager>().To<MBReferenceManager>();
                Kernel.Bind<IEmailLogManager>().To<EmailLogManager>();
                Kernel.Bind<ILogManager>().To<LogManager>();
                Kernel.Bind<ILoanFractionedAmountManager>().To<LoanFractionedAmountManager>();
                Kernel.Bind<ILoanDailyFeeLenderManager>().To<LoanDailyFeeLenderManager>();
                Kernel.Bind<ILoanDailyFeeManager>().To<LoanDailyFeeManager>();
                Kernel.Bind<ILoanManager>().To<LoanManager>();
                Kernel.Bind<ILenderSummaryMananager>().To<LenderSummaryMananager>();
                Kernel.Bind<ILenderPaymentPendingGroupManger>().To<LenderPaymentPendingGroupManager>();
                Kernel.Bind<ILenderPaymentManager>().To<LenderPaymentManager>();
                Kernel.Bind<ILenderBalanceManager>().To<LenderBalanceManager>();
                Kernel.Bind<IInvoicesManager>().To<InvoicesManager>();
                Kernel.Bind<IFinancingProfileManager>().To<FinancingProfileManager>();
                Kernel.Bind<IFeesManager>().To<FeesManager>();
                Kernel.Bind<IExtractMovementsManager>().To<ExtractMovementsManager>();
                Kernel.Bind<IKeyInvoiceManager>().To<KeyInvoiceManager>();
                Kernel.Bind<IEuPagoManager>().To<EuPagoManager>();
                Kernel.Bind<ILenderDashboardManager>().To<LenderDashboardManager>();
                Kernel.Bind<IBorrowerDashbardManager>().To<BorrowerDashboardManager>();
                Kernel.Bind<ICountryManager>().To<CountryManager>();
                Kernel.Bind<IComissionManager>().To<ComissionManager>();
                Kernel.Bind<IBorrowerCompanyManager>().To<BorrowerCompanyManager>();
                Kernel.Bind<IBorrowerAgencyDataManager>().To<BorrowerAgencyDataManager>();
                Kernel.Bind<IBankUserManager>().To<BankUserManager>();
                Kernel.Bind<IBankTransferManager>().To<BankTransferManager>();
                Kernel.Bind<IBankManager>().To<BankManager>();
                Kernel.Bind<IBackOfficeUserAgencyManager>().To<BackOfficeUserAgencyManager>();
                Kernel.Bind<IBackOfficeTopUpManager>().To<BackOfficeTopUpManager>();
                Kernel.Bind<IBackOfficeManager>().To<BackOfficeManager>();
                Kernel.Bind<IAntecipationDataManager>().To<AntecipationDataManager>();
                Kernel.Bind<IAgencyGroupManager>().To<AgencyGroupManager>();
                Kernel.Bind<IAgencyManager>().To<AgencyManager>();
                Kernel.Bind<IActionLogManager>().To<ActionLogManager>();
                Kernel.Bind<IAccountCodeConfirmationManager>().To<AccountCodeConfirmationManager>();
                Kernel.Bind<IAzureStorageManager>().To<AzureStorageManager>();
            }
        }
    }
}
