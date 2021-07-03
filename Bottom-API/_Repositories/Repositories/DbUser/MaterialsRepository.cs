using Bottom_API._Repositories.Interfaces.DbUser;
using Bottom_API.Data;
using Bottom_API.Models;

namespace Bottom_API._Repositories.Repositories.DbUser
{
    public class MaterialsRepository : DbUserRepository<Materials>, IMaterialsRepository
    {
        public MaterialsRepository(UserContext context) : base(context)
        {
        }
    }
}