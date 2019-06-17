using AccessControl.Architects.Entities;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AccessControl.Architects.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserEntity> FindByAccountPasswordAsync(string account, string password); // model

        Claim[] AssembleClaims(UserEntity model); // model
    }
}
