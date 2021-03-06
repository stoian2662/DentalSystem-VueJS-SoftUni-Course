using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using DentalSystem.Interfaces.Infrastructure.Identity.Dto.Output;

namespace DentalSystem.Interfaces.Infrastructure.Identity
{
    public interface IUserService<TUser>
    {
        TUser CurrentUser { get; }

        Task<TUser> FindByNameAsync(string name);

        Task<bool> CheckPasswordAsync(TUser user, string password);

        Task<IAuthResult> CreateAsync(TUser user, string password);

        Task<IAuthResult> AddToRoleAsync(TUser user, string roleName);

        Task<IList<string>> GetRolesAsync(TUser user);

        Task<IAuthResult> UpdateAsync(TUser user);
    }
}