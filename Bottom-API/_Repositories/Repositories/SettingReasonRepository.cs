using Bottom_API._Repositories.Interfaces;
using Bottom_API.Data;
using Bottom_API.Models;

namespace Bottom_API._Repositories.Repositories
{
    public class SettingReasonRepository : BottomRepository<WMSB_Setting_Reason>, ISettingReasonRepository
    {
        public SettingReasonRepository(DataContext context) : base(context)
        {
        }
    }
}