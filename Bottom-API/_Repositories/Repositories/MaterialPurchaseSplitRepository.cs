using Bottom_API._Repositories.Interfaces;
using Bottom_API.Data;
using Bottom_API.Models;

namespace Bottom_API._Repositories.Repositories
{
    public class MaterialPurchaseSplitRepository : BottomRepository<WMSB_Material_Purchase_Split>, IMaterialPurchaseSplitRepository
    {
         private readonly DataContext _context;
        public MaterialPurchaseSplitRepository(DataContext context) : base(context) {
            _context = context;
        }
    }
}