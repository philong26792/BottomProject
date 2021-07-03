using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Bottom_API._Services.Interfaces;
using Bottom_API.DTO.Output;
using Bottom_API.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Bottom_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OutputController : ControllerBase
    {
        private readonly IOutputService _service;
        public OutputController(IOutputService service)
        {
            _service = service;
        }

        [HttpGet("GetByQrCodeId")]
        public async Task<IActionResult> GetByQrCodeId(string qrCodeId)
        {
            var model = await _service.GetByQrCodeId(qrCodeId);
            if (model != null)
                return Ok(model);
            else return NoContent();
        }

        [HttpGet("GetByQrCodeIdByCollectionTransferForm")]
        public async Task<IActionResult> GetByQrCodeIdByCollectionTransferForm(string qrCodeId)
        {
            var model = await _service.GetByQrCodeIdByCollectionTransferForm(qrCodeId);
            if (model != null)
                return Ok(model);
            else return NoContent();
        }

        [HttpGet("GetByQrCodeIdBySortingForm")]
        public async Task<IActionResult> GetByQrCodeIdBySortingForm(string qrCodeId)
        {
            var model = await _service.GetByQrCodeIdBySortingForm(qrCodeId);
            if (model != null)
                return Ok(model);
            else return NoContent();
        }
        
        [HttpPost("savelistoutput")]
        public async Task<IActionResult> SaveListOutput(List<OutputParam> outputParam)
        {
            var updateBy = User.FindFirst(ClaimTypes.Name).Value;
            if (await _service.SaveListOutput(outputParam, updateBy))
            {
                return Ok();
            }

            throw new Exception("Submit failed on save");
        }

        [HttpGet("detail/{transacNo}")]
        public async Task<IActionResult> OutputDetail(string transacNo)
        {
            var model = await _service.GetDetailOutput(transacNo);
            if (model != null)
                return Ok(model);
            else return NoContent();
        }

        [HttpPost("printqrcodeagain")]
        public async Task<IActionResult> PrintQrCode(List<QrCodeIDVersion> ListParamPrintQrCodeAgain)
        {
            var model = await _service.PrintByQRCodeIDAgain(ListParamPrintQrCodeAgain);
            if (model != null)
                return Ok(model);
            else return NoContent();
        }
    }
}