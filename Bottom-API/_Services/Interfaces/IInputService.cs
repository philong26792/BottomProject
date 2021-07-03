using System.Collections.Generic;
using System.Threading.Tasks;
using Bottom_API.DTO.Input;
using Bottom_API.Helpers;
using Bottom_API.Models;

namespace Bottom_API._Services.Interfaces
{
    public interface IInputService 
    {
        Task<Transaction_Dto> GetByQRCodeID(object qrCodeID);
        Task<Transaction_Detail_Dto> GetDetailByQRCodeID(object qrCodeID);
        Task<bool> CreateInput(Transaction_Detail_Dto model, string updateBy);
        Task<bool> SubmitInput(InputSubmitModel data, string updateBy);
        Task<MissingPrint_Dto> GetMaterialPrint(string listMissingNo, string mO_Seq);
        Task<List<MissingPrint_Dto>> GetListMaterialPrint(List<string> missingNo);
        Task<PagedList<QrCodeAgain_Dto>> FilterQrCodeAgain(PaginationParams param, FilterQrCodeAgainParam filterParam);
        Task<PagedList<MissingAgain_Dto>> FilterMissingPrint(PaginationParams param, FilterMissingParam filterParam);
        Task<string> FindMaterialName(string materialID);
        Task<string> FindMissingByQrCode(string qrCodeID);
        Task<bool> CheckQrCodeInV696(string qrCodeID);
        Task<bool> CheckRackLocation(string rackLocation);
        Task<PagedList<IntegrationInputModel>> SearchIntegrationInput(PaginationParams param,FilterPackingListParam filterparam);
        Task<bool> IntegrationInputSubmit(List<IntegrationInputModel> data, string user);
        Task<WMSB_Transaction_Main> FindQrCodeInput(string qrCodeId);
        Task<string> CheckEnterRackInputIntergration(string racklocation, string receiveNo);
        Task<List<NSP_MISSING_REPORT_DETAIL>> ExportExcelMissingReportDetail (FilterMissingParam filterParam);
    }
}