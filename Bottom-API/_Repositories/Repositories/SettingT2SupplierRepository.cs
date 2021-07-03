using Bottom_API._Repositories.Interfaces;
using Bottom_API.Data;
using Bottom_API.Models;

namespace Bottom_API._Repositories.Repositories
{
    public class SettingT2SupplierRepository : BottomRepository<WMSB_Setting_T2Delivery>, ISettingT2SupplierRepository
    {
        private readonly DataContext _context;
        public SettingT2SupplierRepository(DataContext context) : base(context)
        {
            _context = context;
        }
    }
}