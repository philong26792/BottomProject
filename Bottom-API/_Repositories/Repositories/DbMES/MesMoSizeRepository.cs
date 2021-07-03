using Bottom_API._Repositories.Interfaces.DbMES;
using Bottom_API.Data;
using Bottom_API.Models;

namespace Bottom_API._Repositories.Repositories.DbMES
{
    public class MesMoSizeRepository : MesRepository<MES_MO_Size>, IMesMoSizeRepository
    {
        public MesMoSizeRepository(MesDataContext context) : base(context)
        {
        }
    }
}