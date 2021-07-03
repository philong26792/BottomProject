using Bottom_API._Repositories.Interfaces;
using Bottom_API.Data;
using Bottom_API.Models;

namespace Bottom_API._Repositories.Repositories
{
    public class ReleaseDeliveryNoRepository : BottomRepository<WMSB_Release_DeliveryNo>, IReleaseDeliveryNoRepository
    {
        private readonly DataContext _context;
        public ReleaseDeliveryNoRepository(DataContext context) : base(context)
        {
            _context = context;
        }
    }
}