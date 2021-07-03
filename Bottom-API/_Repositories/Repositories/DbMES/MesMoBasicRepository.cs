using Bottom_API._Repositories.Interfaces.DbMES;
using Bottom_API.Data;
using Bottom_API.Models;

namespace Bottom_API._Repositories.Repositories.DbMES
{
    public class MesMoBasicRepository : MesRepository<MES_MO_Basic>, IMesMoBasicRepository
    {
        public MesMoBasicRepository(MesDataContext context) : base(context)
        {
        }
    }
}