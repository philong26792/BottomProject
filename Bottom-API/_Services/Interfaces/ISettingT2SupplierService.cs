using System.Threading.Tasks;
using Bottom_API.DTO;
using Bottom_API.DTO.SettingT2;
using Bottom_API.Helpers;

namespace Bottom_API._Services.Interfaces
{
    public interface ISettingT2SupplierService
    {
        Task<bool> AddT2(Setting_T2Delivery_Dto model, string updateBy);
        Task<bool> UpdateT2(Setting_T2Delivery_Dto model, string updateBy);
        Task<bool> DeleteT2(Setting_T2Delivery_Dto model);
        Task<PagedList<Setting_T2Delivery_Dto>> GetAll(PaginationParams paginationParams, SettingT2SupplierParam settingT2SupplierParam);
    }
}