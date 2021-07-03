using Bottom_API._Repositories.Interfaces;
using Bottom_API.Data;
using Bottom_API.Models;

namespace Bottom_API._Repositories.Repositories
{
    public class ViewPoRepository : BottomRepository<VM_WMSB_PO>, IViewPoRepository
    {
        private readonly DataContext _context;
        public ViewPoRepository(DataContext context) : base(context) {
            _context = context;
        }
    }
}