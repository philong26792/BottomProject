using System.Collections.Generic;
using System.Data;
using System.Security.Claims;
using System.Threading.Tasks;
using Bottom_API._Services.Interfaces;
using Bottom_API.Data;
using Bottom_API.DTO.MergeQrCode;
using Dapper;
using Microsoft.AspNetCore.Mvc;

namespace Bottom_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MergeQrCodeController : ControllerBase
    {
        private readonly IMergeQrCodeService _serviceMergeQrCode;
        private readonly IDatabaseConnectionFactory _database;
        public MergeQrCodeController(IMergeQrCodeService serviceMergeQrCode, IDatabaseConnectionFactory database) {
            _serviceMergeQrCode = serviceMergeQrCode;
            _database = database;
        }
        
        [HttpPost("searchOfMerge")]
        public async Task<IActionResult> SearchOfMerge(FilterMergeQrCodeParam param) {
            var data = await _serviceMergeQrCode.SearchOfMerge(param);
            return Ok(data);
        }

        [HttpPost("mergeQrCode")]
        public async Task<IActionResult> MergeQrCode(List<MergeQrCodeModel> param) {
            var updateBy = User.FindFirst(ClaimTypes.Name).Value;
            var data = await _serviceMergeQrCode.MergeQRCode(param, updateBy);
            return Ok(data);
        }

        [HttpGet("TransactionForSplit")]
        public async Task<IActionResult> SearchTransactionForSplit(string moNo, string qrCodeID, string timeFrom, string timeEnd, bool searchByPrebook) 
        {
            var data = await _serviceMergeQrCode.SearchTransactionForSplit(moNo, qrCodeID, timeFrom, timeEnd, searchByPrebook);
            return Ok(data);
        }

        [HttpGet("TransactionForOtherSplit")]
        public async Task<IActionResult> SearchTransactionForOtherSplit(string moNo, string qrCodeID) 
        {
            var data = await _serviceMergeQrCode.SearchTransactionForOtherSplit(moNo, qrCodeID);
            return Ok(data);
        }

        [HttpGet("SplitInfoDetail")]
        public async Task<IActionResult> SplitInfoDetail(string moNo, string transacNo) 
        {
            var data = await _serviceMergeQrCode.SplitInfoDetail(moNo, transacNo);
            return Ok(data);
        }

        [HttpGet("SplitProcess/{transacNo}")]
        public async Task<IActionResult> SplitProcess(string transacNo) 
        {
            var data = await _serviceMergeQrCode.SplitProcess(transacNo);
            return Ok(data);
        }

        [HttpGet("QrCodeSplitDetail/{transacNo}")]
        public async Task<IActionResult> QrCodeSplitDetail(string transacNo) 
        {
            var data = await _serviceMergeQrCode.QrCodeSplitDetail(transacNo);
            return Ok(data);
        }

        [HttpPost("qrCodeDetail")]
        public async Task<IActionResult> GetQrCodeDetail(QrCodeAfterMerge model) {
            var data = await _serviceMergeQrCode.GetDetailQrCode(model);
            return Ok(data);
        }

        [HttpGet("DataSplitByOffsetNo")]
        public async Task<IActionResult> GetDataSplitByOffsetNo(string offsetNo, string materialId, string moNo, string transacNoParent) 
        {
            var data = await _serviceMergeQrCode.GetDataSplitByOffsetNo(offsetNo, materialId, moNo, transacNoParent);
            return Ok(data);
        }

        // Other Split => Add MoNo con
        [HttpGet("DataOtherSplitByMONo")]
        public async Task<IActionResult> GetDataOtherSplitByMONo(string materialId, string moNo, string moSeq, string transacNoParent, string dMoNo) 
        {
            var data = await _serviceMergeQrCode.GetDataOtherSplitByMONo(materialId, moNo, moSeq, transacNoParent, dMoNo);
            return Ok(data);
        }

        [HttpPost("SaveSplitData")]
        public async Task<IActionResult> SaveSplitData(List<SplitDataByOffset_Dto> dataSplit) 
        {
            var updateBy = User.FindFirst(ClaimTypes.Name).Value;
            var data = await _serviceMergeQrCode.SaveSplitData(dataSplit, updateBy);

             // Chạy lệnh store để cập nhập data bảng Cache
            var conn = await _database.CreateConnectionAsync();
            var sql = "[NSP_UPDATE_CACHE_BY_SPLIT]";

            // Chạy Store Mẹ
            var parameter = new {MO_No = dataSplit[0].DMO_No, MO_Seq = dataSplit[0].MO_Seq, Material_ID = dataSplit[0].Material_ID};
            await conn.ExecuteAsync(sql, parameter, commandType: CommandType.StoredProcedure);
            // Chạy All Store con
            foreach (var item in dataSplit) {

                var parameterItem = new {MO_No = item.MO_No, MO_Seq = item.MO_Seq, Material_ID = item.Material_ID};
                await conn.ExecuteAsync(sql, parameterItem, commandType: CommandType.StoredProcedure);
            }
            return Ok(data);
        }

        [HttpPost("SaveOtherSplitData")]
        public async Task<IActionResult> SaveOtherSplitData(List<SplitDataByOffset_Dto> dataSplit) 
        {
            var updateBy = User.FindFirst(ClaimTypes.Name).Value;
            var data = await _serviceMergeQrCode.SaveSplitData(dataSplit, updateBy, true);

             // Chạy lệnh store để cập nhập data bảng Cache
            var conn = await _database.CreateConnectionAsync();
            var sql = "[NSP_UPDATE_CACHE_BY_SPLIT]";

            // Chạy Store Mẹ
            var parameter = new {MO_No = dataSplit[0].DMO_No, MO_Seq = dataSplit[0].MO_Seq, Material_ID = dataSplit[0].Material_ID};
            await conn.ExecuteAsync(sql, parameter, commandType: CommandType.StoredProcedure);
            // // Chạy All Store con
            foreach (var item in dataSplit) {

                var parameterItem = new {MO_No = item.MO_No, MO_Seq = item.MO_Seq, Material_ID = item.Material_ID};
                await conn.ExecuteAsync(sql, parameterItem, commandType: CommandType.StoredProcedure);
            }
            
            return Ok(data);
        }

        [HttpGet("materialInformation")]
        public async Task<IActionResult> GetMaterialInformation() {
            var data = await _serviceMergeQrCode.GetMaterialInformation();
            return Ok(data);
        }

        [HttpGet("getMaterialInformationByPO")]
        public async Task<IActionResult> GetMaterialInformationByPO(string moNo) {
            var data = await _serviceMergeQrCode.GetMaterialInformationInPO(moNo);
            return Ok(data);
        }
    }
}