using Microsoft.Owin.Security;
using PayLend.Business.Managers.BackOffice;
using PayLend.Business.Managers.Loan;
using PayLend.Business.Managers.User;
using PayLend.Core.Interfaces;
using PayLend.Core.Types;
using PayLend.FrontOffice.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PayLend.Business.Managers.Permissions;
using PayLend.Business.Managers.Bank;
using System.Data;
using System.Configuration;
using System.Globalization;

namespace PayLend.FrontOffice.Areas.Account.Controllers
{
    [Authorize]
    public partial class UserController : Controller
    {
        protected readonly ILoanManager LoanManager;
        protected readonly IPayLendUserManager PayLendUserManager;
        protected readonly IPermissionManager PermissionManager;
        protected readonly IAgencyManager AgencyManager;
        protected readonly IBankManager BankManager;

        public UserController(ILoanManager loanManager, IPayLendUserManager payLendUserManager, IPermissionManager permissionManager, IAgencyManager agency, IBankManager bankManager)
        {
            LoanManager = loanManager;
            PayLendUserManager = payLendUserManager;
            PermissionManager = permissionManager;
            this.AgencyManager = agency;
            this.BankManager = bankManager;
        }
        public virtual ActionResult Login()
        {
            var emails = ((ClaimsIdentity)User.Identity).FindAll(x => x.Type.Equals("nickname"));
            var bointerface = ((ClaimsIdentity)User.Identity).FindAll(x => x.Type.Equals("profile_bo"));
            var jpermissions = (JArray)JsonConvert.DeserializeObject(((ClaimsIdentity)User.Identity).FindAll(x => x.Type.Equals("permissions")).FirstOrDefault()?.Value);

            var permissions = jpermissions.ToObject<int[]>();

            if (!emails.Any())
                throw new System.Exception("Não há um email válido para o utilizador");

            var email = emails.FirstOrDefault()?.Value;
            var isBackOffice = bointerface != null && bointerface.FirstOrDefault().Value.Equals("BackOffice");//UserManager.GetAll().Any(bu => bu.Email.ToLower().Equals(email.ToLower()));
            switch (bointerface.FirstOrDefault().Value.ToLower())
            {
                case ("backoffice"):
                case ("admin"):
                case ("master backoffice"):
                    {
                        SetPermissions(permissions);
                        return RedirectToAction(MVC.BackOffice.LoanApplication.Index());
                    }
                case ("borrower"):
                    {
                        var user = SetApplicationData(email, permissions);

                        if (user == null)
                        {
                            return RedirectToAction(MVC.Management.Improvement.Index());
                        }
                        return RedirectBy(user as Core.Entities.Borrower);
                    }
                case ("lender"):
                    {
                        var user = SetApplicationData(email, permissions);
                        if (user == null)
                        {
                            return RedirectToAction(MVC.Management.Improvement.Index());
                        }
                        // UpdateBalance(user);
                        var id = Cryptography.Encrypt(user.Id.ToString(), true);
                        return RedirectToAction(MVC.Lender.LenderHome.Index(id));
                    }
                default:
                    return Redirect(ConfigurationManager.AppSettings["urlClient"].ToString() + "Home/Registo");
            }
        }

        private ActionResult RedirectBy(Core.Entities.Borrower borrower)
        {
            var loans = LoanManager.GetAllByBorrower(borrower);
            Session[SessionHelper.AvaiableBalance] = borrower.PayLendCard.FirstOrDefault()?.AvaiableBalance.ToString(CultureInfo.InvariantCulture) + " €";
            decimal montantePorAmortizar = 0;
            var loanList = loans.Where(x => x.Status == LoanStatusEnum.OnGoingPayment || x.Status == LoanStatusEnum.OnLatePayment).ToList();
            foreach (var loan in loanList)
            {
                decimal feesDiariosPorAmortizar = 0;

                foreach (var payment in loan.LoanDailyFee)
                {
                    if (payment.Status == LoanDailyFeetatusEnum.Unpaid)
                    {
                        feesDiariosPorAmortizar += payment.Amount;
                    }
                    else if (payment.Status == LoanDailyFeetatusEnum.PartialPayment)
                    {
                        feesDiariosPorAmortizar += (payment.Amount - payment.PartialPayment);
                    }
                }

                var antecipationData = loan.ProfitAntecipation.AntecipationDataList.FirstOrDefault();

                if (antecipationData != null)
                    montantePorAmortizar += (antecipationData.Amount + feesDiariosPorAmortizar);
            }

            Session[SessionHelper.Divida] = montantePorAmortizar.ToString("C");

            //AQUI
            var id = Cryptography.Encrypt(borrower.Id.ToString(), true);

            if (borrower.BorrowerAgencyData == null || string.IsNullOrWhiteSpace(borrower.BorrowerAgencyData.AgencyIdentification?.Substring(0,5)))
            {
                return RedirectToAction(MVC.Borrower.BorrowerHome.GetAgencyInformation());
            }
            else {
                //verificar se agencia existe.
                var agency = AgencyManager.FindBy(x => x.AgencyExternalId.Equals(borrower.BorrowerAgencyData.AgencyIdentification.Substring(0, 5))).Any();
                if (!agency)
                {
                    return RedirectToAction(MVC.Borrower.BorrowerHome.GetAgencyInformation());
                }
            }

            if (borrower.ProfitsAntecipations.Count == 0)
                return RedirectToAction(MVC.Borrower.Antecipation.Create(id));


            if (borrower.Banks == null)
            {
                return RedirectToAction(MVC.Borrower.BorrowerHome.FinalizeRegister(id));
            }
            else
            {
                var swift = borrower.Banks.Any(y => y.Bank != null && (y.Bank.SwiftCode != null || y.Bank.SwiftCode != ""));
                if (!swift)
                {
                    return RedirectToAction(MVC.Borrower.BorrowerHome.FinalizeRegister(id));
                }
            }
            

            if (borrower.ProfitsAntecipations.Any(x => x.Status == Core.Types.LoanApplicationStatus.Pending || x.Status == Core.Types.LoanApplicationStatus.Approved))
                return RedirectToAction(MVC.Borrower.Antecipation.CreateResponse(true));
            else if (borrower.ProfitsAntecipations.ToList().TrueForAll(x => x.Status == Core.Types.LoanApplicationStatus.Captivated))
            {
                if(!string.IsNullOrWhiteSpace(borrower.NIF))
                {
                    return RedirectToAction(MVC.Borrower.BorrowerHome.ResumoAnteciparGanhos(id));
                }
                else if ((string.IsNullOrWhiteSpace(borrower.NIF)) && ((PayLend.Core.Entities.Borrower)borrower)
                    .ProfitsAntecipations.Any(x => x.Status == LoanApplicationStatus.Approved || x.Status == LoanApplicationStatus.Captivated))
                {
                    //formulario terceira fase
                    return RedirectToAction(MVC.Borrower.BorrowerHome.FinalizeRegister(id));    
                }
                else
                {
                    return RedirectToAction(MVC.Borrower.BorrowerHome.FinalizeRegister(id));
                }
            }
            //return RedirectToAction(MVC.Borrower.BorrowerHome.ConfirmationLoan());                
            else
            {
                return RedirectToAction(MVC.Borrower.Antecipation.Create(id));
            }
        }

        public virtual ActionResult ResetPassword()
        {
            //TO-DO
            return View();
        }

        /// <summary>
        /// Signs the user out and clears the cache of access tokens.
        /// </summary>
        public virtual ActionResult SignOut()
        {
            // Remove all cache entries for this user and send an OpenID Connect sign-out request.
            IEnumerable<AuthenticationDescription> authTypes = HttpContext.GetOwinContext().Authentication.GetAuthenticationTypes();
            HttpContext.GetOwinContext().Authentication.SignOut(authTypes.Select(t => t.AuthenticationType).ToArray());
            Request.GetOwinContext().Authentication.GetAuthenticationTypes();

            return Redirect(ConfigHelper.PostLogoutRedirectUri);
        }

        protected virtual ActionResult CreateAccount() => View();

        private IPayLendUser SetApplicationData(string email, int[] permissions)
        {
            if (PayLendUserManager != null)
            {
                var user = PayLendUserManager.GetAll().FirstOrDefault(x => x.Email.ToLower().Equals(email.ToLower()));
                if (user == null)
                    return null;

                Session[SessionHelper.SessionUser] = user;
                Session[SessionHelper.SessionUserEmail] = email;
                Session[SessionHelper.Password] = user.Password;
                Session[SessionHelper.HashID] = Cryptography.Encrypt(user.Id.ToString(), true);
                Session[SessionHelper.AvaiableBalance] = user.PayLendCard.FirstOrDefault().AvaiableBalance.ToString() + " €";
                SetPermissions(permissions);
                return user;
            }
            else
            {
                //TO-DO
                return null;
            }
        }

        private void SetPermissions(int[] permissions)
        {
            if (permissions.Any())
            {
                Session[SessionHelper.Permissions] = PermissionManager.GetPermissions(permissions);
            }
        }
    }
}