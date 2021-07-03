using System.Threading.Tasks;
using Bottom_API.DTO.Auth;

namespace Bottom_API._Services.Interfaces
{
    public interface IAuthService
    {
        Task<UserForLogged_Dto> GetUser(string username);
    }
}