using PayLend.Repository.Interface;


namespace PayLend.Business.Repositories.BackOfficeUser
{
    public interface IBackOfficeUserRepository : IEmailRepository<PayLend.Core.Entities.BackOfficeUser>
    {
    }
}
