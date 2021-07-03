using System.Collections.Generic;
using System.Threading.Tasks;
using Bottom_API.DTO;
using Bottom_API.DTO.TransferLocation;
using Bottom_API.Helpers;

namespace Bottom_API._Services.Interfaces
{
    public interface ITransferLocationService
    {
        Task<TransferLocation_Dto> GetByQrCodeId(object qrCodeId, string updateBy);
        Task<bool> SubmitTransfer(List<TransferLocation_Dto> lists, string updateBy);
        Task<PagedList<TransferLocation_Dto>> Search(TransferLocationParam transferLocationParam, PaginationParams paginationParams);
        Task<HistoryDetail_Dto> GetDetailTransaction(string transacNo);
        Task<HistoryDetail_Dto> GetDetailTransactionForOutput(string transacNo);
        Task<bool> CheckExistRackLocation(string rackLocation);
        bool CheckRackLocationHaveTheSameArea(string fromLoaction, string toLocation);
        Task<bool> CheckTransacNoDuplicate(string transacNo);
    }
}