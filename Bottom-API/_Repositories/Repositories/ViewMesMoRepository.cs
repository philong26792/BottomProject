using Bottom_API._Repositories.Interfaces;
using Bottom_API.Data;
using Bottom_API.Models;

namespace Bottom_API._Repositories.Repositories
{
    public class ViewMesMoRepository : BottomRepository<VM_WMSB_MES_MO>, IViewMesMoRepository
    {
        private readonly DataContext _context;
        public ViewMesMoRepository(DataContext context) : base(context) {
            _context = context;
        }
    }
}