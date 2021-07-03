using Bottom_API._Repositories.Interfaces;
using Bottom_API.Data;
using Bottom_API.Models;

namespace Bottom_API._Repositories.Repositories
{
    public class MaterialOffsetRepository : BottomRepository<WMSB_Material_Offset>, IMaterialOffsetRepository
    {
        public MaterialOffsetRepository(DataContext context) : base(context)
        {
        }
    }
}