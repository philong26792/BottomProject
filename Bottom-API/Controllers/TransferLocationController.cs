using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Bottom_API._Services.Interfaces;
using Bottom_API.DTO;
using Bottom_API.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Bottom_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransferLocationController : ControllerBase
    {
        private readonly ITransferLocationService _service;

        public TransferLocationController(ITransferLocationService service)
        {
            _service = service;
        }

        [HttpGet("{qrCodeId}", Name = "GetByQrCode")]
        public async Task<IActionResult> GetByQrCodeId(string qrCodeId)
        {
            var updateBy = User.FindFirst(ClaimTypes.Name).Value;
            var model = await _service.GetByQrCodeId(qrCodeId, updateBy);
            if (model.QrCodeId != null)
                return Ok(model);
            else return NoContent();
        }

        [HttpPost("submit", Name = "SubmitTransfer")]
        public async Task<IActionResult> Submit(List<TransferLocation_Dto> lists)
        {
            var updateBy = User.FindFirst(ClaimTypes.Name).Value;
            if (await _service.SubmitTransfer(lists, updateBy))
            {
                return Ok();
            }

            throw new Exception("Submit failed on save");
        }

        [HttpPost("search")]
        public async Task<IActionResult> Search([FromQuery]PaginationParams paginationParams, TransferLocationParam transferLocationParam) {
            var result = await _service.Search(transferLocationParam, paginationParams);
            Response.AddPagination(result.CurrentPage, result.PageSize, result.TotalCount, result.TotalPages);
            return Ok(result);
        }

        [HttpGet("GetDetailTransaction")]
        public async Task<IActionResult> GetDetailTransaction(string transferNo) {
            var result = await _service.GetDetailTransaction(transferNo);
            return Ok(result);
        }

        [HttpGet("GetDetailTransactionForOutput")]
        public async Task<IActionResult> GetDetailTransactionForOutput(string transferNo) {
            var result = await _service.GetDetailTransactionForOutput(transferNo);
            return Ok(result);
        }

        [HttpGet("CheckExistRackLocation")]
        public async Task<bool> CheckExistRackLocation(string rackLocation) {
            var result = await _service.CheckExistRackLocation(rackLocation);
            return result;
        }
        [HttpGet("CheckRackLocationHaveTheSameArea")]
        public bool CheckRackLocationHaveTheSameArea(string fromLocation, string toLocation) {
            return _service.CheckRackLocationHaveTheSameArea(fromLocation, toLocation);
        }
        [HttpGet("CheckTransacNoDuplicate")]
        public async Task<bool> CheckTransacNoDuplicate(string transacNo) {
            return await _service.CheckTransacNoDuplicate(transacNo);
        }
    }
}