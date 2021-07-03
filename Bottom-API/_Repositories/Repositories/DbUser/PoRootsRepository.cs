using Bottom_API._Repositories.Interfaces.DbUser;
using Bottom_API.Data;
using Bottom_API.Models;

namespace Bottom_API._Repositories.Repositories.DbUser
{
    public class PoRootsRepository : DbUserRepository<PoRoots>, IPoRootsRepository
    {
        public PoRootsRepository(UserContext context) : base(context)
        {
        }
    }
}