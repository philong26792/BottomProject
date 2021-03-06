using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Bottom_API._Services.Interfaces;
using Bottom_API.DTO.Receiving;
using Bottom_API.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Bottom_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReceivingController : ControllerBase
    {
        private readonly IReceivingService _service;
        public ReceivingController(IReceivingService service)
        {
            _service = service;
        }
        
        [HttpPost("search")]
        public async Task<IActionResult> SearchByModel([FromQuery]PaginationParams param,FilterMaterialParam filterParam) {
            var result = await _service.SearchByModel(param, filterParam);
            Response.AddPagination(result.CurrentPage, result.PageSize, result.TotalCount, result.TotalPages);
            return Ok(result);
        }

        [HttpPost("searchTable")]
        public async Task<IActionResult> SearchByPurchase([FromBody]MaterialMainViewModel model) {
            var data = await _service.MaterialMerging(model);
            return Ok(data);   
        }

        [HttpPost("updateMaterial")]
        public async Task<IActionResult> UpdateMaterial([FromBody] List<OrderSizeByBatch> model) {
            var updateBy = User.FindFirst(ClaimTypes.Name).Value;
            var data  = await _service.UpdateMaterial(model, updateBy);
            return Ok(data);
        }

        [HttpGet("receiveNoDetails/{receive_No}")]
        public async Task<IActionResult> ReceiveNoDetails(string receive_No) {
            var data = await _service.ReceiveNoDetails(receive_No);
            return Ok(data);
        }
        
        [HttpPost("receiveNoMain")]
        public async Task<IActionResult> PurchaseNoDetail([FromBody]MaterialMainViewModel model) {
            var data = await _service.ReceiveNoMain(model);
            return Ok(data);
        }

        [HttpPost("closePurchase")]
        public async Task<IActionResult> ClosePurchase([FromBody]MaterialMainViewModel model) {
            var data = await _service.ClosePurchase(model);
            return Ok(data);
        }

        [HttpPost("statusPurchase")]
        public async Task<IActionResult> StatusPurchase([FromBody]MaterialMainViewModel model) {
            var status = await _service.StatusPurchase(model);
            return Ok(new {status = status});
        }


        // Save khi thay ?????i d??? li???u nh???n h??ng.
        [HttpPost("editMaterial")]
        public async Task<IActionResult> EditMaterial ([FromBody] ReceiveNoMain model) {
            var data = await _service.EditMaterial(model);
            return Ok(data);
        }

        [HttpPost("editDetail")]
        public async Task<IActionResult> EditDetail([FromBody] List<MaterialEditModel> data) {
            var result = await _service.EditDetail(data);
            return Ok(result);
        }

        [HttpGet("getDMONo")]
        public async Task<IActionResult> GetDMO_No(string moNo) {
            var data = await _service.GetDMO_No(moNo);
            return Ok(data);
        }

        [HttpGet("checkInputDelivery")]
        public async Task<IActionResult> CheckInputDelivery(string supplier_ID) {
            var data = await _service.CheckInputDelivery(supplier_ID);
            return Ok(data);
        }
    }
}