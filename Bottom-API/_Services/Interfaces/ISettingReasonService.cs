using System.Collections.Generic;
using System.Threading.Tasks;
using Bottom_API.DTO;
using Bottom_API.Helpers;
using Bottom_API.Models;

namespace Bottom_API._Services.Interfaces
{
    public interface ISettingReasonService
    {
        Task<bool> AddReason(Setting_Reason_Dto model);
        Task<bool> UpdateReason(Setting_Reason_Dto model);
        Task<bool> DeleteReason(Setting_Reason_Dto model);

        Task<PagedList<Setting_Reason_Dto>> GetAll(PaginationParams paginationParams,string reasonCode,string reasonName, string  trans_toHP);
        Task<List<WMSB_Setting_Reason>> GetAll();
        Task<object> GetReasonCode();
    }
}