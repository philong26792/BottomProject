using System.Security.Claims;
using System.Threading.Tasks;
using Bottom_API._Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Bottom_API.DTO.ModifyStore;
namespace Bottom_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ModifyStoreController : ControllerBase
    {
        private readonly IModifyStoreService _modifyQRCodeService;

        public ModifyStoreController(IModifyStoreService modifyQRCodeService)
        {
            _modifyQRCodeService = modifyQRCodeService;
        }

        [HttpGet("search")]
        public async Task<IActionResult> ModifyQRCodeMain(string moNo, string supplierId, string qrCodeId)
        {
            var data = await _modifyQRCodeService.GetModifyStoreMain(moNo, supplierId, qrCodeId);
            return Ok(data);
        }

        [HttpGet("modifyDetail")]
        public async Task<IActionResult> GetDetailModifyQrCode(string moNo, string materialId) {
            var data = await _modifyQRCodeService.GetDetailModifyStore(moNo, materialId);
            return Ok(data);
        }

        [HttpPost("saveNoByBatchOut")]
        public async Task<IActionResult> SaveDataNoByBatch(ModifyQrCodeSaveParam param) {
            var updateBy = User.FindFirst(ClaimTypes.Name).Value;
            var data = await _modifyQRCodeService.SaveDataNoByBatchOut(param, updateBy);
            return Ok(data);
        }

        [HttpPost("saveNoByBatchIn")]
        public async Task<IActionResult> SaveDataNoByIn(ModifyQrCodeSaveParam param) {
            var updateBy = User.FindFirst(ClaimTypes.Name).Value;
            var data = await _modifyQRCodeService.SaveDataNoByBatchIn(param, updateBy);
            return Ok(data);
        }

        [HttpPost("getDetailQrCode")]
        public async Task<IActionResult> GetDetailQrCode(ModifyQrCodeAfterSave model) {
            var data = await _modifyQRCodeService.GetDetailStoreChange(model);
            return Ok(data);
        }

        [HttpGet("PlanNoByQRCodeID/{qrCodeId}")]
        public async Task<IActionResult> PlanNoByQRCodeID(string qrCodeId) 
        {
            var data = await _modifyQRCodeService.GetPlanNoByQRCodeID(qrCodeId);
            return Ok(new { planNo = data });
        }

        [HttpGet("getReasonOfSupplierID")]
        public async Task<IActionResult> GetReasonOfSupplierID(string supplierId) {
            var data = await _modifyQRCodeService.GetReasonOfSupplierID(supplierId);
            return Ok(data);
        }
    }
}