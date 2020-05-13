using PayLend.Repository.Interface;

namespace PayLend.Business.Repositories.PasswordResetDetails
{
    public interface IPasswordResetDetailsRepository : IEmailRepository<PayLend.Core.Entities.PasswordResetDetails>
    {
    }
}
