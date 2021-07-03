using Bottom_API._Repositories.Interfaces;
using Bottom_API.Data;
using Bottom_API.Models;

namespace Bottom_API._Repositories.Repositories
{
    public class MesMoRepository : MesRepository<MES_MO>, IMesMoRepository
    {
        private readonly MesDataContext _context;
        public MesMoRepository(MesDataContext context) : base(context) {
            _context = context;
        }
    }
}