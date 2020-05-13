namespace PayLend.Database
{
    using PayLend.Core.Entities;
    using PayLend.Core.Entities.Admin;
    using PayLend.Core.Entities.Fee;
    using PayLend.Core.Entities.Perfil;
    using PayLend.Core.Entities.Promotions;
    using PayLend.Core.Entities.Register;
    using System.Configuration;
    using System.Data.Entity;
    using System.Data.Entity.ModelConfiguration.Conventions;
    using System.Linq;

    public class PayLendContext : DbContext
    {
        //PROD
        public PayLendContext()
            : base(ConfigurationManager.ConnectionStrings["payLendContext"].ToString())   //ConfigurationManager.ConnectionStrings["payLendContext"].ToString()
        {
            Database.SetInitializer<PayLendContext>(null);
            this.Database.CommandTimeout = 180;
        }

        #region "PayLend"
        public DbSet<Agency> Agency { get; set; }
        public DbSet<PayLendUser> PayLendUser { get; set; }
        public DbSet<ProfitWithoutStart> ProfitWithoutStart { get; set; }
        public DbSet<AgencyGroup> AgencyGroup { get; set; }
        public DbSet<EmailLog> EmailLog { get; set; }
        public DbSet<BackOfficeUser> BackOfficeUser { get; set; }
        public DbSet<BackOfficeUserAgency> BackOfficeUserAgencies { get; set; }
        public DbSet<LoanFractionedAmount> LoanFractionedAmount { get; set; }
        public DbSet<LoanDailyFeeLender> LoanDailyFeeLender { get; set; }
        public DbSet<ProfitAntecipation> ProfitAntecipation { get; set; }



        public DbSet<LenderBalance> LenderBalance { get; set; }
        public DbSet<PayLendCaptiveBalance> CaptiveBalance { get; set; }
        public DbSet<Loans> PayLendLoans { get; set; }
        public DbSet<ErrorLog> Log { get; set; }
        public DbSet<AgenciesFinancingProfile> AgenciesFinancingProfiles { get; set; }
        public DbSet<PayLendProduct> PayLendProduct { get; set; }
        public DbSet<Invoices> Invoices { get; set; }
        public DbSet<PayLendConfig> PayLendConfig { get; set; }
       
        public DbSet<AntecipationData> AntecipationData { get; set; }
        public DbSet<LenderSummary> LenderSummary { get; set; }
        public DbSet<BorrowerDashboard> BorrowerDashboard { get; set; }
        public DbSet<LoanDailyFee> LoanDailyFee { get; set; }
        
        public DbSet<OperationLog> OperationLog { get; set; }
        public DbSet<ExtractMovements> ExtractMovements { get; set; }
        public DbSet<LenderPaymentPending> LenderPaymentPending { get; set; }
        
        public DbSet<MovementType> MovementType { get; set; }
        public DbSet<OverduePayments> OverduePayments { get; set; }
        public DbSet<Service> Service { get; set; }
        public DbSet<Country> Country { get; set; }

        public DbSet<PayLendCard> PayLendCard { get; set; }
        public DbSet<Currency> Currency { get; set; }
        public DbSet<ProfitAntecipationRejectionReason> ProfitAntecipationRejectionReason { get; set; }
        public DbSet<LenderPaymentPendingGroup> LenderPaymentPendingGroup { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<ActionType> ActionType { get; set; }
        public DbSet<Permission> Permission { get; set; }
        public DbSet<User_Permission> User_Permission { get; set; }
        public DbSet<FreeFee> FreeFee { get; set; }
        public DbSet<PasswordResetDetails> PasswordResetDetails { get; set; }
        public DbSet<AccountCodeConfirmation> AccountCodeConfirmation { get; set; }
        public DbSet<BorrowerAgencyData> BorrowerAgencyData { get; set; }
        public DbSet<Bank> Bank { get; set; }
        public DbSet<MBReference> MBReference { get; set; }
        public DbSet<TopUpProcessed> TopUpProcessed { get; set; }
        public DbSet<BankUser> BankUser { get; set; }
        public DbSet<BankTransfer> BankTransfer { get; set; }
        public DbSet<BorrowerCompanyData> BorrowerCompanyData { get; set; }
        public DbSet<PostalCodeAddress> PostalCodeAddress { get; set; }
        public DbSet<Concelhos> Concelhos { get; set; }
        public DbSet<Distritos> Distritos { get; set; }
        
        public DbSet<BackOfficeTopUp> BackOfficeTopUp { get; set; }
        public DbSet<PhonePrefix> PhonePrefix { get; set; }
        public DbSet<Comission> Comission { get; set; }
        public DbSet<Promotion> Promotion { get; set; }
        public DbSet<FilePath> FilePath { get; set; }
        public DbSet<Lender> Lender { get; set; }
        public DbSet<FinancingProfile> InvestmentLenderConfiguration { get; set; }
        public DbSet<Borrower> Borrower { get; set; }
    
        public DbSet<VATExeptionCode> VATExeptionCode { get; set; }
        public DbSet<RetentionTax> RetentionTax { get; set; }
        public DbSet<ProfitAntecipationParameters> ProfitAntecipationParameter { get; set; }
        public DbSet<PayLendContract> PayLendContract { get; set; }
        public DbSet<FinancingContract> FinancingContract { get; set; }
        public DbSet<UserBillingAddresses> UserBillingAddresses { get; set; }
        public DbSet<RealStateFinancing> RealStateFinancingProduct { get; set; }

        //public DbSet<Charge> Carregamento { get; set; }
        //public DbSet<CarregamentoMbWay> CarregamentoMbWay { get; set; }
        //public DbSet<CarregamentoMultibanco> CarregamentoMultibanco { get; set; }
        //public DbSet<ActionLog> ActionLog { get; set; }
        public DbSet<HistoriCOverduePayments> HistoriCollections { get; set; }
        public DbSet<OverduePaymentsReason> OverduePaymentsReason { get; set; }
        public DbSet<OverduePaymentsReasonGlobal> OverduePaymentsReasonGlobal { get; set; }
        public DbSet<ServiceReason> ServiceReason { get; set; }
        #endregion

        //IQueryable<T> IPayLendContext.Set<T>()
        //{
        //    return base.Set<T>();
        //}

        public void Rollback()
        {
            ChangeTracker.Entries().ToList().ForEach(m =>
            {
                m.State = EntityState.Detached;
            });
        }

        public void SaveChanges(string User = "WebJob")
        {
            LogOperations(User);
            base.SaveChanges();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FinancingProfile>()
                        .Property(x => x.FundingPercent)
                        .HasPrecision(18, 4);

            modelBuilder.Entity<LoanFractionedAmount>()
                        .Property(x => x.FundingPercent)
                        .HasPrecision(18, 4);

            modelBuilder.Entity<LoanFractionedAmount>()
                       .Property(x => x.DailyFee)
                       .HasPrecision(18, 4);

            modelBuilder.Entity<LoanDailyFee>()
                        .Property(x => x.Amount)
                        .HasPrecision(18, 4);

            modelBuilder.Entity<LoanDailyFeeLender>()
                       .Property(x => x.Amount)
                       .HasPrecision(18, 4);

            modelBuilder.Entity<AntecipationData>()
                      .Property(x => x.DailyFee)
                      .HasPrecision(18, 4);

            modelBuilder.Entity<OverduePayments>()
                .Property(x => x.NewDailyFee)
                .HasPrecision(18, 4);

            modelBuilder.Entity<HistoriCOverduePayments>()
                .Property(x => x.DailyFee)
                .HasPrecision(18, 4);

            modelBuilder.Entity<OverduePayments>()
                .Property(x => x.NewDailyFeePer1000)
                .HasPrecision(18, 4);

            modelBuilder.Entity<PayLendCard>()
                .Property(X => X.Balance)
                .HasPrecision(10, 2);

            modelBuilder.Entity<PayLendCard>()
                .Property(X => X.PendingBalance)
                .HasPrecision(10, 2);

            modelBuilder.Entity<PayLendCard>()
                .Property(X => X.AvaiableBalance)
                .HasPrecision(10, 2);

            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            base.OnModelCreating(modelBuilder);
        }

        public void LogOperations(string user)
        {
            var emailUsuarioLogado = user;

            var stateAdded = new[] { EntityState.Added };
            var statesModifiedAndDeleted = new[] { EntityState.Modified, EntityState.Deleted };

            var changesToLogAdded = base.ChangeTracker.Entries().Where(x => stateAdded.Contains(x.State)).ToList();
            var changesToLogModifiedAndDeleted = base.ChangeTracker.Entries().Where(x => statesModifiedAndDeleted.Contains(x.State)).ToList();

            if (changesToLogModifiedAndDeleted.Any())
            {
                foreach (var change in changesToLogModifiedAndDeleted)
                {
                    LogContext.LogOperation(this, change, change.State, emailUsuarioLogado);
                }
            }

            if (changesToLogAdded.Any())
            {
                base.SaveChanges();
                foreach (var change in changesToLogAdded)
                {
                    LogContext.LogOperation(this, change, EntityState.Added, emailUsuarioLogado);
                }
            }
        }
    }
}
