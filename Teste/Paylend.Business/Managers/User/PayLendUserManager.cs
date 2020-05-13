using Paylend.Business.Core;
using PayLend.Business.Managers.AccountCodeConfirmationsManager;
using PayLend.Business.Managers.Country;
using PayLend.Business.Managers.PayLendCard;
using PayLend.Business.Managers.Log;
using PayLend.Core.DTO.Request.BackOffice;
using PayLend.Core.DTO.Response.BackOffice;
using PayLend.Core.DTO.Response.Commom;
using PayLend.Core.Entities;
using PayLend.Core.Entities.Perfil;
using PayLend.Core.Error;
using PayLend.Core.Types;
using PayLend.Framework.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using PayLend.Business.Managers.Storage;
using System.IO;
using PayLend.Business.Managers.Mail;
using System.Web;
using PayLend.Business.Managers.Bank;
using System.Data.Entity;
using System.Configuration;
using System.Globalization;
using PayLend.Business.Managers.BackOfficeTopUp;
using PayLend.Business.Managers.BankUser;
using PayLend.Business.Managers.BorrowerAgencyData;
using PayLend.Business.Managers.PayLendConfig;
using PayLend.Business.Managers.ResetPasswordManager;
using PayLend.Business.Managers.User_Permission;
using PayLend.Business.Repositories.PayLendUser;

namespace PayLend.Business.Managers.User
{
    public class PayLendUserManager : IPayLendUserManager
    {
        protected readonly IPayLendUserRepository _repository;
        protected readonly ICountryManager _countryManager;
        protected readonly IAccountCodeConfirmationManager _accountCodeConfirmationManager;
        protected readonly IPayLendCardManager _payLendCardManager;
        protected readonly ILogManager _logManager;
        protected readonly IAzureStorageManager _azureStorageManager;
        protected readonly IEmailLogManager _emailLogManager;
        protected readonly IBankManager _bankManager;
        protected readonly IResetPasswordManager _resetPasswordManager;
        protected readonly IBankUserManager _bankUserManager;
        protected readonly IPayLendConfigManager _payLendConfigManager;
        protected readonly IBackOfficeTopUpManager _backOfficeTopUpManager;
        protected readonly IBorrowerAgencyDataManager _borrowerAgencyDataManager;
        protected readonly IUser_PermissionManager _userPermissionManager;

        private string ClassName = "PayLendUserManager";

        public PayLendUserManager(IPayLendUserRepository repository, ICountryManager countryManager, IAccountCodeConfirmationManager accountCodeConfirmationManager, IPayLendCardManager payLendCardManager, ILogManager logManager,
            IAzureStorageManager azureStorageManager, IEmailLogManager emailLogManager, IBankManager bankManager, IResetPasswordManager resetPasswordManager, IBankUserManager bankUserManager,
            IPayLendConfigManager payLendConfigManager, IBackOfficeTopUpManager backOfficeTopUpManager, IBorrowerAgencyDataManager borrowerAgencyDataManager, IUser_PermissionManager userPermissionManager)
        {
            _repository = repository;
            _countryManager = countryManager;
            _accountCodeConfirmationManager = accountCodeConfirmationManager;
            _payLendCardManager = payLendCardManager;
             _logManager = logManager;
            _azureStorageManager = azureStorageManager;
            _emailLogManager = emailLogManager;
            _bankManager = bankManager;
            _resetPasswordManager = resetPasswordManager;
            _bankUserManager = bankUserManager;
            _payLendConfigManager = payLendConfigManager;
            _backOfficeTopUpManager = backOfficeTopUpManager;
            _borrowerAgencyDataManager = borrowerAgencyDataManager;
            _userPermissionManager = userPermissionManager;
        }

        public PaginatedPagesDTO PaginatedSearchUserList(JQueryBootGridRequestDTO userListSearchRequestDto)
        {
            UserListSearchResponseDTO userListSearchResponseDto = new UserListSearchResponseDTO();
            PaginatedPagesDTO paginatedPagesDto = new PaginatedPagesDTO();

            try
            {
                userListSearchResponseDto.simpleReturnDTO = new SimpleReturnDTO();
                paginatedPagesDto.SimpleReturnDTO = new SimpleReturnDTO();
                string name = String.IsNullOrWhiteSpace(userListSearchRequestDto.Name) ? "" : userListSearchRequestDto.Name;
                string email = String.IsNullOrWhiteSpace(userListSearchRequestDto.Email) ? "" : userListSearchRequestDto.Email;
                string nif = String.IsNullOrWhiteSpace(userListSearchRequestDto.NIF) ? "" : userListSearchRequestDto.NIF;

                var users = _repository.GetAll<PayLendUser>(x =>
                            (((x.Name == null || x.Name.Trim() == string.Empty) || x.Name.Contains(name)))
                            && ((x.NIF == null || x.NIF.Trim() == string.Empty) || x.NIF.Contains(nif))
                            && x.Email != null
                            && ((x.Email == null || x.Email.Trim() == string.Empty) || x.Email.Contains(email))
                            && (userListSearchRequestDto.CountryId == 0 || x.CountryId == userListSearchRequestDto.CountryId)
                            && (userListSearchRequestDto.CreatedDate == DateTime.MinValue || (userListSearchRequestDto.CreatedDate.Day == x.CreatedDate.Day && userListSearchRequestDto.CreatedDate.Month == x.CreatedDate.Month && userListSearchRequestDto.CreatedDate.Year == x.CreatedDate.Year))
                        ).ToList();

                users = users.OrderByDescending(x => x.CreatedDate).ToList();

                List<PayLend.Core.Entities.Country> countryList = _repository.GetAll<PayLend.Core.Entities.Country>().ToList();

                if (userListSearchRequestDto.UserType == DiscriminatorEnum.Borrower)
                {
                    users = users.Where(x => x.GetType().BaseType.Name == "Borrower").ToList();
                }
                else if (userListSearchRequestDto.UserType == DiscriminatorEnum.Lender)
                {
                    users = users.Where(x => x.GetType().BaseType.Name == "Lender").ToList();
                }
                else if (userListSearchRequestDto.UserType == DiscriminatorEnum.Admin)
                {
                    users = users.Where(x => x.GetType().BaseType.Name == "Admin").ToList();
                }

                paginatedPagesDto.total = users.Count();

                paginatedPagesDto.current = userListSearchRequestDto.current;
                paginatedPagesDto.rowCount = userListSearchRequestDto.rowCount;

                paginatedPagesDto.rows = _PaylendUserObjectArray(paginatedPagesDto, users, userListSearchRequestDto.current, userListSearchRequestDto.rowCount);

                return paginatedPagesDto;
            }
            catch (Exception ex)
            {
                paginatedPagesDto.SimpleReturnDTO.ErrorMessage = ex.Message;
                paginatedPagesDto.SimpleReturnDTO.ErrorType = ErrorType.Conclusion;
                paginatedPagesDto.SimpleReturnDTO.ErrorKey = "SearchBox";

                var erroTratado = TreatmentError.SerializedErrorDetail(ex);
                erroTratado.User = ClassName;
                _logManager.SaveError(erroTratado);
                return paginatedPagesDto;
            }

        }

        private object[] _PaylendUserObjectArray(PaginatedPagesDTO paginatedPagesDto, List<PayLendUser> users, int page, int rows)
        {
            if (paginatedPagesDto.rowCount > 0)
            {
                return users.OrderBy(x => x.Id).Skip(((page - 1) * (paginatedPagesDto.rowCount))).Take(rows).ToList().Select(x => new
                {
                    id = x.Id,
                    x.Email,
                    x.Name,
                    x.NIF,
                    Country = x.CountryId,
                    CreatedDate = x.CreatedDate.ToShortDateString(),
                    x.Address,
                    x.City,
                    DiscriminatorType = x.GetType().BaseType.Name,
                    securityId = x.SecurityId
                }).ToArray();
            }
            else
            {
                return users.OrderBy(x => x.Id).ToList().Select(x => new
                {
                    id = x.Id,
                    x.Email,
                    x.Name,
                    x.NIF,
                    Country = x.CountryId,
                    CreatedDate = x.CreatedDate.ToShortDateString(),
                    x.Address,
                    x.City,
                    DiscriminatorType = x.GetType().BaseType.Name
                }).ToArray();
            }
        }

        public PayLend.Core.Entities.BorrowerAgencyData CreateAgencyData(PayLend.Core.Entities.BorrowerAgencyData borrowerAgencyData)
        {
            
            _borrowerAgencyDataManager.Create(borrowerAgencyData);
            return _borrowerAgencyDataManager.GetAll().FirstOrDefault(x => x.Id == borrowerAgencyData.Id);
        }

        public Borrower GetBorrower(int borrowerId)
        {
            return _repository.GetAll<Borrower>(x => x.Id == borrowerId, includeProperties: "ProfitsAntecipations.AntecipationDataList, " +
                                                                                                 "LoanDailyFee, " +
                                                                                                 "Loans.LoanDailyFee, " +
                                                                                                 "Dashboard, " +
                                                                                                 "Banks, " +
                                                                                                 "AntecipationDataList, " +
                                                                                                 "BorrowerCompanyData, " + "BorrowerAgencyData").FirstOrDefault();
        }


        public List<Borrower> GetAllBorrower()
        {
            return _repository.GetAll<Borrower>().ToList();
        }

        public List<Lender> GetAllLender()
        {
            return _repository.GetAll<Lender>().ToList();
        }

        public Lender GetLenderById(int id)
        {
           return _repository.GetAll<Lender>(x => x.Id == id, includeProperties: "UserBalance, Loans.ProfitAntecipation.AntecipationDataList, LenderSummaries").FirstOrDefault();
        }

        public List<Lender> GetLenderSummaryForAgency(List<int> agencyListId)
        {
            return _repository.GetAll<PayLend.Core.Entities.Lender>(x => x.LenderSummaries.Any(y => agencyListId.Contains(y.Loans.ProfitAntecipation.IdWorkStore)), includeProperties: "LenderSummaries").ToList();
        }

        public IEnumerable<PayLend.Core.Entities.Lender> GetLenderSummaryByLenderID(int lenderId)
        {
            return _repository.GetAll<Lender>(x => x.Id == lenderId, includeProperties: "LenderSummaries");
        }

        public UserDetailResponseDTO EditSave(EditUserRequestDTO model, HttpPostedFileBase filedata)
        {
            UserDetailResponseDTO result = new UserDetailResponseDTO();
            result.simpleReturnDTO = new SimpleReturnDTO();
            try
            {
                var users = _repository.GetAll<PayLendUser>();
                var user = users.FirstOrDefault(x => x.Id == model.Id);
                var bouser = users.FirstOrDefault(x => x.Email == model.BoUserEmail);
                var bank = _bankManager.GetAll().FirstOrDefault(x => x.SwiftCode.Equals(model.SWIFT));

                if (user == null)
                {
                    result.simpleReturnDTO.ErrorKey = "User";
                    result.simpleReturnDTO.ErrorMessage = "User Not Registered";
                    result.simpleReturnDTO.ErrorType = ErrorType.Conclusion;
                    return result;
                }

                user.Name = model.Name;
                user.MobilePhone = model.MobilePhone;
                user.Email = model.Email;
                user.NIF = model.NIF;
                user.Address = model.Address;
                user.City = model.City;
                user.PostalCode = model.PostalCode;
                user.MobilePhone = model.MobilePhone;
                user.BirthdayDate = model.BirthdayDate;
                user.PhonePrefixID = model.PhonePrefixID;
                user.DocumentValidDate = model.DocumentValidDate == DateTime.MinValue ? null : model.DocumentValidDate;
                user.Nationality = model.Nationality;
                user.IdentificationDocNumber = model.IdentificationDocNumber;

                var userBank = _repository.GetAll<PayLend.Core.Entities.BankUser>(x => x.UserId == user.Id).ToList();
                if(userBank.Any())
                {
                    user.Banks.FirstOrDefault().IBAN = model.IBAN;
                    user.Banks.FirstOrDefault().NIB = model.IBAN;
                    user.Banks.FirstOrDefault().BankId = bank.Id;
                }
                else
                {
                    var bankUser = new PayLend.Core.Entities.BankUser
                    {
                        UserId = user.Id,
                        BankId = bank.Id,
                        IBAN = model.IBAN,
                        NIB = model.IBAN,
                        Default = true,
                    };
                    _bankUserManager.Create(bankUser);
                }

                if (user.BorrowerCompanyData != null)
                { user.BorrowerCompanyData = model.borrowerCompanyData;}

                //Caso exista alteração de saldo
                if (filedata != null)
                {
                    var url = "";                                                                       
                    var containerUrlCpcv = _payLendConfigManager.GetConfigValue("BlobContainerTopUpReceipt");
                    string extension = Path.GetExtension(filedata.FileName);
                    if (filedata.FileName != null)
                    {
                        String filenameCpcv = Guid.NewGuid().ToString() + "_" + filedata.FileName.Replace(" ", "");
                        _azureStorageManager.uploadFile(filedata.InputStream, filenameCpcv, filedata.ContentLength, FileType.PaymentReceipt);
                        url = containerUrlCpcv + filenameCpcv;
                    }

                    //Criar o pedido de carregamento de saldo que tem que ter aprovação
                    var topUp = new PayLend.Core.Entities.BackOfficeTopUp
                    {
                        UserId = user.Id,
                        Ammount = model.TopUpValue,
                        BoUserIdRequest = bouser.Id, 
                        PaymentReceiptUrl = url,
                        Status = TopUpStatusEnum.Pendente,
                    };

                    _backOfficeTopUpManager.Create(topUp);
                    //Enviar email a informar que foi feito um pedido de aumento de saldo
                    _emailLogManager.SendRequestToTopUp(bouser.Id, user);
                }

                _repository.Update(user);
               // _repository.SaveChanges(true);

                return result;
            }
            catch (Exception ex)
            {
                result.simpleReturnDTO.ErrorKey = "Error";
                result.simpleReturnDTO.ErrorMessage = ex.Message;
                result.simpleReturnDTO.ErrorType = ErrorType.Form;

                var erroTratado = TreatmentError.SerializedErrorDetail(ex);
                erroTratado.User = ClassName;
                _logManager.SaveError(erroTratado);
                return result;
            }

        }

        SimpleReturnDTO IActionManager<PayLendUser>.Create(PayLendUser entity)
        {
            SimpleReturnDTO simpleReturnDto = new SimpleReturnDTO();
            try
            {
                _repository.Add(entity);
                simpleReturnDto.ID = entity.Id;
                return simpleReturnDto;

            }
            catch (Exception e)
            {
                simpleReturnDto.ErrorMessage = e.Message;
                return simpleReturnDto;
            }
        }

        public SimpleReturnDTO CreateNewBo(PayLendUser entity)
        {
            SimpleReturnDTO simpleReturnDto = new SimpleReturnDTO();
            try
            {
                entity.DiscriminatorType = DiscriminatorEnum.Admin;
                Create(entity);


                var permissions = _repository.GetAll<Permission>(x => x.CategoryID == 3 && x.isVisible == true).ToList();

                foreach (var item in permissions)
                {
                    _userPermissionManager.Create(new PayLend.Core.Entities.Perfil.User_Permission { PayLendUser = entity, Permission = item, Active = true, CreatedDate = DateTime.Now });
                }

                var currency = _repository.Get<Currency>(11);
                _payLendCardManager.Create(new PayLend.Core.Entities.PayLendCard
                {
                    CardNumber = (entity.CountryId.ToString() + (entity.Id.ToString().PadLeft(000000, '0'))),
                    PayLendUser = entity,
                    Active = true,
                    CardFriendlyName = "Conta Paylend",
                    Currency = currency,
                    Default = true
                });
                SaveChanges();

                return simpleReturnDto;
            }
            catch(Exception e)
            {
                simpleReturnDto.ErrorMessage = e.Message;
                return simpleReturnDto;
            }
        }

        public SimpleReturnDTO CreateNewLender(Lender entity)
        {
            SimpleReturnDTO simpleReturnDto = new SimpleReturnDTO();
            try
            {
                var isDuplicated = FindBy(x => x.Email.Equals(entity.Email)).Any();

                if (isDuplicated)
                {
                    simpleReturnDto.ErrorKey = "DuplicateEmail";
                    simpleReturnDto.ErrorMessage = "DuplicateEmail";
                    simpleReturnDto.ErrorType = ErrorType.Register;
                }
                else
                {
                    entity.DiscriminatorType = DiscriminatorEnum.Lender;

                    if (entity.SWIFT == null)
                    {
                        string zip = entity.IBAN.Substring(4, 4);
                        var result = _bankManager.FindBy(x => x.IbanCode.Equals(zip)).ToList();
                        entity.SWIFT = result.FirstOrDefault()?.SwiftCode;
                    }

                    if (!entity.Banks.Any() || !entity.Banks.Any(x => x.Bank.SwiftCode.Equals(entity.SWIFT)))
                    {
                        var bank = _bankManager.GetAll().FirstOrDefault(x => x.SwiftCode.Equals(entity.SWIFT));

                        entity.Banks.Add(new PayLend.Core.Entities.BankUser
                        {
                            IBAN = entity.IBAN,
                            Bank = bank,
                            NIB = entity.IBAN,
                            Default = true,
                        });
                    }

                    
                    _repository.Create(entity);
                    _bankUserManager.Create(entity.Banks.FirstOrDefault());
                    //this._repository.Create(entity);
                    //this._repository.SaveChanges();

                    var permissions = _repository.GetAll<Permission>(x => x.CategoryID == 5 && x.isVisible == true).ToList();

                    foreach (var item in permissions)
                    {
                        _userPermissionManager.Create(new PayLend.Core.Entities.Perfil.User_Permission { PayLendUser = entity, Permission = item, Active = true, CreatedDate = DateTime.Now });
                    }

                    var currency = _repository.Get<Currency>(11);
                    _payLendCardManager.Create(new PayLend.Core.Entities.PayLendCard
                    {
                        CardNumber = (entity.CountryId.ToString() + (entity.Id.ToString().PadLeft(000000, '0'))),
                        PayLendUser = entity,
                        Active = true,
                        CardFriendlyName = "Conta Paylend",
                        Currency = currency,
                        Default = true
                    });

                    //this._repository.SaveChanges();
                }

                return simpleReturnDto;
            }
            catch (Exception e)
            {
                simpleReturnDto.ErrorMessage = e.Message;
                return simpleReturnDto;
            }
        }

        public SimpleReturnDTO CreateNewBorrower(PayLendUser entity)
        {
            SimpleReturnDTO simpleReturnDto = new SimpleReturnDTO();
            try
            {
                var isDuplicated = FindBy(x => x.Email.Equals(entity.Email)).Any();

                if (isDuplicated)
                {
                    simpleReturnDto.ErrorKey = "DuplicateEmail";
                    simpleReturnDto.ErrorMessage = "DuplicateEmail";
                    simpleReturnDto.ErrorType = ErrorType.Register;
                }
                else
                {
                    Random generator = new Random();
                    String r = generator.Next(1000, 99999).ToString("D6");
                    Create(entity);
                    var accountcode = new AccountCodeConfirmation
                    {
                        Email = entity.Email,
                        CreatedDate = DateTime.Now,
                        Code = r
                    };

                    var permissions = _repository.GetAll<Permission>(x => x.CategoryID == 4 && x.isVisible == true).ToList();

                    foreach (var item in permissions)
                    {
                        _userPermissionManager.Create(new PayLend.Core.Entities.Perfil.User_Permission { PayLendUser = entity, Permission = item, Active = true, CreatedDate = DateTime.Now });
                    }

                    _accountCodeConfirmationManager?.Create(accountcode);
                    var currency = _repository.Get<Currency>(11);
                _payLendCardManager.Create(new PayLend.Core.Entities.PayLendCard
                {
                    CardNumber = (entity.CountryId.ToString()+(entity.Id.ToString().PadLeft(000000,'0'))),
                    PayLendUser = entity,
                    Active = true,
                    CardFriendlyName = "Conta Paylend",
                    Currency = currency,
                    Default = true
                });
                SaveChanges();

                string subject = PayLend.Core.Languages.Site.TyRegister + " " + entity.Name;
                string messageBody = PayLend.Core.Languages.Email.MailTop;
                var text = PayLend.Core.Languages.Email.EmailConfirmacao;
                
                messageBody += text.Replace("[AccountCode]", accountcode.Code)
                    .Replace("[User]", PayLend.Core.Languages.Site.TyRegister + " " + entity.Name)
                    .Replace("[Email]", entity.Email)
                    .Replace("[BaseUrl]", ConfigurationManager.AppSettings["urlClient"].ToString())         
                    .Replace("[Password]", PayLend.Core.EncriptPassword.Decrypt(entity.Password));


                    messageBody += PayLend.Core.Languages.Email.MailBottom;
                    MailSender.SendHtmlMenssage(new List<string> { entity.Email }, new List<string>(), subject, messageBody, "admin@paylend.pt", null);

                }

                return simpleReturnDto;

            }
            catch (Exception e)
            {
                simpleReturnDto.ErrorMessage = e.Message;
                return simpleReturnDto;
            }
        }

        public UserDetailResponseDTO GetUserById(int id)
        {
            return FillUserDetail(_repository.Get<PayLendUser>(id));
        }

        public PasswordResetDetails CreateToken(string email, string IP)
        {
            try
            {
                var users = _repository.GetAll<PayLendUser>(x => x.Email.ToLower().Equals(email.ToLower()));
                if (users.Any())
                {
                    var paylendUser = users.First();

                    var token = ComputeSha256Hash(DateTime.Now.ToUniversalTime().ToString(CultureInfo.InvariantCulture) + email);
                    var passwordreset = new PasswordResetDetails
                    {
                        Email = paylendUser.Email,
                        PayLendUserId = paylendUser.Id,
                        KeyText = token,
                        IP = IP,
                        CreatedDate = DateTime.Now
                    };
                    _resetPasswordManager.Create(passwordreset);
                    //_repository.SaveChanges(true);

                    return passwordreset;
                }
                else
                {
                    return new PasswordResetDetails();
                }
            }
            catch (Exception er)
            {
                _emailLogManager.ErrorMail(er.Message.ToString() + " - " + er.GetBaseException().Message);
                return new PasswordResetDetails();
            }

        }

        public bool IsTokenValid(string token, string email)
        {
            if (String.IsNullOrWhiteSpace(token) || String.IsNullOrWhiteSpace(email))
                return false;

            return _repository.GetAll<PasswordResetDetails>(x => x.KeyText.Equals(token) && x.PayLendUser.Email.Equals(email) && ((System.Data.Entity.DbFunctions.AddMinutes(x.CreatedDate, 60) >= DateTime.Now))).Any();
        }


        private string ComputeSha256Hash(string rawData)
        {
            StringBuilder builder = new StringBuilder();
            try
            {
                // Create a SHA256   
                using (SHA256 sha256Hash = SHA256.Create())
                {
                    // ComputeHash - returns byte array  
                    byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                    // Convert byte array to a string   
                    //StringBuilder builder = new StringBuilder();
                    for(int i = 0; i < bytes.Length; i++)
                    {
                        builder.Append(bytes[i].ToString("x2"));
                    }
                    return builder.ToString();
                }
            }
            catch (Exception er)
            {
                _emailLogManager.ErrorMail(er.Message.ToString());
            }
            return builder.ToString();
        }

        public void SaveLogChanges()
        {
            _repository.SaveLogChanges();
        }

        public void Rollback()
        {
            _repository.Rollback();
        }

        //Script para gerar contas para os users que ainda não têm PaylendCard
        #region Manager
        public void SaveChanges()
        {
            throw new NotImplementedException();
        }

        public void Update(PayLendUser entity)
        {
            _repository.Update(entity);
        }

        public void Delete(PayLendUser entity)
        {
            _repository.Remove(entity);
        }

        public void Create(PayLendUser entity)
        {
            _repository.Create(entity);
        }

        public IEnumerable<PayLendUser> FindBy(Expression<Func<PayLendUser, bool>> filter)
        {
            List<PayLendUser> paylendUsers = new List<PayLendUser>();
            var listUsers = _repository.GetAll<PayLendUser>(filter, includeProperties: "Banks, " + "BorrowerCompanyData, " + "BorrowerAgencyData");
            paylendUsers.AddRange(listUsers);
            return paylendUsers;
        }

        public IEnumerable<PayLendUser> GetAll()
        {
            List<PayLendUser> paylendUsers = new List<PayLendUser>();
            var listUsers = _repository.GetAll<PayLendUser>().Include(x => x.Banks);
            paylendUsers.AddRange(listUsers);
            return paylendUsers;
        }
        #endregion

        #region Fill DTO
        private UserDetailResponseDTO FillUserDetail(PayLendUser resultDto)
        {
            UserDetailResponseDTO userDetail = new UserDetailResponseDTO();
            try
            {
                userDetail.simpleReturnDTO = new SimpleReturnDTO();
                if (resultDto != null)
                {
                    userDetail.payLendUser = resultDto;
                    userDetail.payLendUser.Country = _countryManager.GetAll().FirstOrDefault(x => x.ID == resultDto.CountryId);
                    userDetail.payLendUser.Banks = _repository.GetAll<PayLend.Core.Entities.BankUser>(includeProperties: "Bank").Where(x => x.PayLendUser.Id == resultDto.Id).ToList();

                    var memberInfo = resultDto.GetType().BaseType;
                    if (memberInfo != null && memberInfo.Name == DiscriminatorEnum.Borrower.ToString())
                    {
                        userDetail.payLendUser.DiscriminatorType = DiscriminatorEnum.Borrower;
                        //Legal Representative tem que passar a ser os dados da empresa
                        if (userDetail.payLendUser.RegisterType == RegisterTypeEnum.EmpresaCompleto)
                        {
                            //userDetail.legalRepresentative = Repository.GetAll<LegalRepresentative>(x => x.PayLendUserId == resultDTO.Id).FirstOrDefault();
                        }
                    }
                    else if (resultDto.GetType().BaseType.Name == DiscriminatorEnum.Lender.ToString())
                    {
                        userDetail.payLendUser.DiscriminatorType = DiscriminatorEnum.Lender;
                    }
                    else if (resultDto.GetType().BaseType.Name == DiscriminatorEnum.Admin.ToString())
                    {
                        userDetail.payLendUser.DiscriminatorType = DiscriminatorEnum.Admin;
                    }
                    else
                    {
                        userDetail.payLendUser.DiscriminatorType = DiscriminatorEnum.Todos;
                    }

                    userDetail.payLendUser.PayLendCard.FirstOrDefault().Balance = userDetail.payLendUser.PayLendCard.FirstOrDefault().Balance;
                    userDetail.payLendUser.PayLendCard.FirstOrDefault().PendingBalance = userDetail.payLendUser.PayLendCard.FirstOrDefault().PendingBalance;
                    userDetail.payLendUser.PayLendCard.FirstOrDefault().AvaiableBalance = userDetail.payLendUser.PayLendCard.FirstOrDefault().AvaiableBalance;
                }
                else
                {
                    userDetail.simpleReturnDTO.ErrorMessage = "HaveUserID_DontHaveData";
                    userDetail.simpleReturnDTO.ErrorType = ErrorType.Form;
                    userDetail.simpleReturnDTO.ErrorKey = "UserDetails";
                }
            }
            catch (Exception e)
            {
                userDetail.simpleReturnDTO.ErrorMessage = e.Message;
                userDetail.simpleReturnDTO.ErrorType = ErrorType.Form;
                userDetail.simpleReturnDTO.ErrorKey = "UserDetails";
                return userDetail;
            }
            return userDetail;
        }
        #endregion
    }
}
