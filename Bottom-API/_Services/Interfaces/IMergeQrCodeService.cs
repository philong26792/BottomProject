using System.Collections.Generic;
using System.Threading.Tasks;
using Bottom_API.DTO.MergeQrCode;
using Bottom_API.Models;

namespace Bottom_API._Services.Interfaces
{
    public interface IMergeQrCodeService
    {
        Task<List<MergeQrCodeModel>> SearchOfMerge(FilterMergeQrCodeParam param);
        Task<List<MergeQrCodeModel>> SearchTransactionForSplit(string moNo, string qrCodeID, string timeFrom, string timeEnd, bool searchByPrebook = true);
        Task<List<MergeQrCodeModel>> SearchTransactionForOtherSplit(string moNo, string qrCodeID);
        Task<SplitQrCodeMain_Dto> SplitInfoDetail(string moNo, string transacNo);
        Task<SplitProcess_Dto> SplitProcess(string transacNo);
        Task<List<QrCodeAfterMerge>> MergeQRCode(List<MergeQrCodeModel> data, string updateBy);
        Task<List<WMSB_Transaction_Detail>> GetDetailQrCode(QrCodeAfterMerge model);
        Task<List<SplitDataByOffset_Dto>> GetDataSplitByOffsetNo(string offsetNo, string materialId, string moNo, string transacNoParent);
        Task<SplitDataByOffset_Dto> GetDataOtherSplitByMONo(string materialId, string moNo, string moSeq, string transacNoParent, string dMoNo);
        Task<bool> SaveSplitData(List<SplitDataByOffset_Dto> dataSplit, string updateBy, bool otherSplit = false);
        Task<List<MaterialInformation>> GetMaterialInformation();
        Task<List<MaterialInformation>> GetMaterialInformationInPO(string moNo);
        Task<QrCodeSplitDetail_Dto> QrCodeSplitDetail(string transacNo);

    }
}