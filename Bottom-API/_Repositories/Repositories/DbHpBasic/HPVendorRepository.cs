using Bottom_API._Repositories.Interfaces.DbHpBasic;
using Bottom_API.Data;
using Bottom_API.Models;

namespace Bottom_API._Repositories.Repositories.DbHpBasic
{
    public class HPVendorRepository : HPRepository<HP_Vendor_u01>, IHPVendorRepository
    {
        private readonly HPDataContext _context;
        public HPVendorRepository(HPDataContext context) : base(context) {
            _context = context;
        }
    }
}