using System.Collections.Generic;
using System.Threading.Tasks;
using Bottom_API.DTO;
using Bottom_API.DTO.Output;
using Bottom_API.Helpers;
namespace Bottom_API._Services.Interfaces
{
    public interface IOutputService
    {
        Task<Output_Dto> GetByQrCodeId(string qrCodeId);
        Task<Output_Dto> GetByQrCodeIdByCollectionTransferForm(string qrCodeId);
        Task<Output_Dto> GetByQrCodeIdBySortingForm(string qrCodeId);
        Task<bool> SaveListOutput(List<OutputParam> outputParam, string updateBy);
        Task<OutputDetail_Dto> GetDetailOutput(string transacNo);
        Task<List<OutputPrintQrCode_Dto>> PrintByQRCodeIDAgain(List<QrCodeIDVersion> ListParamPrintQrCodeAgain);
    }
}