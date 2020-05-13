using System.Collections.Generic;
using Paylend.Business.Core;
using PayLend.Core.DTO.Request.BackOffice;
using PayLend.Core.DTO.Response.BackOffice;
using PayLend.Core.DTO.Response.Commom;
using PayLend.Core.Entities;
using System.Web;

namespace PayLend.Business.Managers.User
{
    public interface IPayLendUserManager : IActionManager<PayLendUser>
    {
        Borrower GetBorrower(int borrowerId);
        UserDetailResponseDTO GetUserById(int id);
        UserDetailResponseDTO EditSave(EditUserRequestDTO model, HttpPostedFileBase fileData);
        PaginatedPagesDTO PaginatedSearchUserList(JQueryBootGridRequestDTO userListSearchRequestDTO);
        bool IsTokenValid(string token, string email);
        PasswordResetDetails CreateToken(string email, string IP);
        SimpleReturnDTO CreateNewBorrower(PayLendUser entity);

        SimpleReturnDTO CreateNewBo(PayLendUser entity);

        SimpleReturnDTO CreateNewLender(Lender entity);

        PayLend.Core.Entities.BorrowerAgencyData CreateAgencyData(PayLend.Core.Entities.BorrowerAgencyData borrowerAgencyData);

        List<Borrower> GetAllBorrower();

        List<Lender> GetAllLender();

        Lender GetLenderById(int id);

        List<Lender> GetLenderSummaryForAgency(List<int> agencyListId);


        IEnumerable<PayLend.Core.Entities.Lender>  GetLenderSummaryByLenderID(int lenderId);

        void SaveLogChanges();

        void Rollback();
    }
}
