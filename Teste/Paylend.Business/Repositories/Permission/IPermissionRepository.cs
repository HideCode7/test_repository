using PayLend.Repository.Interface;

namespace PayLend.Business.Repositories.Permission
{
    public interface IPermissionRepository : IEmailRepository<PayLend.Core.Entities.Perfil.Permission>
    {
    }
}
