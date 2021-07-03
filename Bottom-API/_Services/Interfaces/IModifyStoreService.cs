using System.Collections.Generic;
using System.Threading.Tasks;
using Bottom_API.Models;
using Bottom_API.DTO.ModifyStore;

namespace Bottom_API._Services.Interfaces
{
    public interface IModifyStoreService
    {
        Task<List<ModifyQRCodeMain_Dto>> GetModifyStoreMain(string moNo, string supplierId, string qrCodeId);
        Task<object> GetDetailModifyStore(string moNo, string materialId);
        Task<List<ModifyQrCodeAfterSave>> SaveDataNoByBatchOut(ModifyQrCodeSaveParam param, string updateBy);
        Task<List<ModifyQrCodeAfterSave>> SaveDataNoByBatchIn(ModifyQrCodeSaveParam param, string updateBy);
        Task<List<WMSB_Transaction_Detail>> GetDetailStoreChange(ModifyQrCodeAfterSave model);
        Task<string> GetPlanNoByQRCodeID(string qrCodeId);
        Task<List<WMSB_Setting_T2Delivery>> GetReasonOfSupplierID(string supplierId);

    }
}