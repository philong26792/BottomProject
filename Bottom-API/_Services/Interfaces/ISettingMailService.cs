
using System.Threading.Tasks;
using Bottom_API.DTO.SettingMail;
using Bottom_API.Helpers;

namespace Bottom_API._Services.Interfaces
{
    public interface ISettingMailService
    {
        Task<object> GetAllSupplierNo();
        Task<bool> CreatSettingSupplier(Setting_Mail_Supplier_Dto model);


        Task<bool> EditSettingSupplier(Setting_Mail_Supplier_Dto model);

        Task<bool> DeleteSettingSupplier(Setting_Mail_Supplier_Dto model);

        Task<PagedList<Setting_Mail_Supplier_Dto>> GetAllSettingMail(string supplierNo, string factory, PaginationParams paginationParams);

        Task<object> GetAllSubcon();
    }
}