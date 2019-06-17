using AccessControl.Architects.Entities;
using AccessControl.Architects.Entities.Contexts;
using AccessControl.Architects.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AccessControl.Architects.Services
{
    public class UserService : IUserService
    {
        private readonly DatabaseContext _context;

        public UserService(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<UserEntity> FindByAccountPasswordAsync(string account, string password)
        {
            return await _context.UserEntities.SingleOrDefaultAsync(v => v.Account == account && v.Password == password);
        }

        public Claim[] AssembleClaims(UserEntity model)
        {
            return new Claim[]
            {
                new Claim("account", model.Account)
            };
        }
    }
}
