using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using Aspose.Cells;
using Bottom_API._Services.Interfaces;
using Bottom_API.DTO.Input;
using Bottom_API.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace Bottom_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InputController : ControllerBase
    {

        private readonly IInputService _service;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public InputController(IInputService service, IWebHostEnvironment webHostEnvironment)
        {
            _service = service;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet("{qrCodeID}", Name = "GetByQrCodeID")]
        public async Task<IActionResult> GetByQrCodeID(string qrCodeID)
        {
            var model = await _service.GetByQRCodeID(qrCodeID);
            if (model.QrCode_Id != null)
                return Ok(model);
            else return NoContent();
        }

        [HttpGet("detail/{qrCode}", Name = "GetDetail")]
        public async Task<IActionResult> GetDetailByQrCodeID(string qrCode)
        {
            var model = await _service.GetDetailByQRCodeID(qrCode);
            if (model != null)
                return Ok(model);
            else return NoContent();
        }

        [HttpPost("create", Name = "CreateInput")]
        public async Task<IActionResult> CreateInput(Transaction_Detail_Dto model)
        {
            var updateBy = User.FindFirst(ClaimTypes.Name).Value;
            if (await _service.CreateInput(model, updateBy))
            {
                return Ok();
            }

            throw new Exception("Creating the rack location failed on save");
        }

        [HttpPost("submit", Name = "SubmitInput")]
        public async Task<IActionResult> SubmitInput([FromBody] InputSubmitModel data)
        {
            var updateBy = User.FindFirst(ClaimTypes.Name).Value;
            if (await _service.SubmitInput(data, updateBy))
            {
                return Ok();
            }

            throw new Exception("Submit failed on save");
        }

        [HttpPost("printmissing", Name = "PrintMissing")]
        public async Task<IActionResult> PrintMissing(MissingOfBatch param)
        {
            var model = await _service.GetMaterialPrint(param.Missing_No, param.MO_Seq);
            if (model != null)
                return Ok(model);
            else return NoContent();
        }

        [HttpPost("printlistmissing", Name = "PrintListMissing")]
        public async Task<IActionResult> PrintListMissing(List<string> listMissingNo)
        {
            var model = await _service.GetListMaterialPrint(listMissingNo);
            if (model != null)
                return Ok(model);
            else return NoContent();
        }


        [HttpPost("filterQrCodeAgain")]
        public async Task<IActionResult> FilterQrCodeAgain([FromQuery] PaginationParams param, FilterQrCodeAgainParam filterParam)
        {
            var data = await _service.FilterQrCodeAgain(param, filterParam);
            Response.AddPagination(data.CurrentPage, data.PageSize, data.TotalCount, data.TotalPages);
            return Ok(data);
        }

        [HttpPost("filterMissingPrint")]
        public async Task<IActionResult> FilterMissingPrint([FromQuery] PaginationParams param, FilterMissingParam filterParam)
        {
            var data = await _service.FilterMissingPrint(param, filterParam);
            Response.AddPagination(data.CurrentPage, data.PageSize, data.TotalCount, data.TotalPages);
            return Ok(data);
        }

        [HttpGet("findMaterialName/{materialID}")]
        public async Task<IActionResult> FindMaterialName(string materialID)
        {
            var materialName = await _service.FindMaterialName(materialID);
            return Ok(new { materialName = materialName });
        }

        [HttpGet("findMiss/{qrCodeId}")]
        public async Task<IActionResult> FindMissingByQrCode(string qrCodeId)
        {
            var missingNo = await _service.FindMissingByQrCode(qrCodeId);
            return Ok(new { missingNo = missingNo });
        }


        [HttpGet("checkQrCodeInV696/{qrCodeId}")]
        public async Task<IActionResult> CheckQrCodeInV696(string qrCodeId)
        {
            var data = await _service.CheckQrCodeInV696(qrCodeId);
            return Ok(new { result = data });
        }
        [HttpGet("checkRacLocation/{rackLocation}")]
        public async Task<IActionResult> CheckRacklocation(string rackLocation)
        {
            var data = await _service.CheckRackLocation(rackLocation);
            return Ok(new { result = data });
        }

        [HttpPost("integrations")]
        public async Task<IActionResult> SearchIntegrationInput([FromQuery] PaginationParams param, FilterPackingListParam filterparam)
        {
            var result = await _service.SearchIntegrationInput(param, filterparam);
            Response.AddPagination(result.CurrentPage, result.PageSize, result.TotalCount, result.TotalPages);
            return Ok(result);
        }

        [HttpPost("intergationSubmit")]
        public async Task<IActionResult> IntergationSubmit([FromBody] List<IntegrationInputModel> data)
        {
            var updateBy = User.FindFirst(ClaimTypes.Name).Value;
            var result = await _service.IntegrationInputSubmit(data, updateBy);
            return Ok(new { result = result });
        }

        [HttpGet("findQrCodeInput/{qrCodeId}")]
        public async Task<IActionResult> FindQrCodeInput(string qrCodeId)
        {
            var data = await _service.FindQrCodeInput(qrCodeId);
            return Ok(data);
        }

        [HttpGet("checkEnterRackInputIntergration")]
        public async Task<IActionResult> CheckEnterRackInputIntergration(string racklocation, string receiveNo)
        {
            var data = await _service.CheckEnterRackInputIntergration(racklocation, receiveNo);
            return Ok(new { result = data });
        }

        [HttpPost("ExportExcelMissingReportDetail")]
        public async Task<IActionResult> ExportExcelMissingReportDetail(FilterMissingParam filterParam)
        {
            var data = await _service.ExportExcelMissingReportDetail(filterParam);

            var path = Path.Combine(_webHostEnvironment.ContentRootPath, "Resources\\Template\\MissingReportDetail.xlsx");
            WorkbookDesigner designer = new WorkbookDesigner();
            designer.Workbook = new Workbook(path);
            Cell cell = designer.Workbook.Worksheets[0].Cells["A1"];
            Worksheet ws = designer.Workbook.Worksheets[0];

            designer.SetDataSource("result", data);
            designer.Process();

            // Style Excel
            Style styleSumTotal = designer.Workbook.CreateStyle();
            styleSumTotal.ForegroundColor = Color.Yellow;
            styleSumTotal.Pattern = BackgroundType.Solid;
            styleSumTotal.IsTextWrapped = true;
            styleSumTotal.VerticalAlignment = TextAlignmentType.Center;

            Style styleItem = designer.Workbook.CreateStyle();
            styleItem.HorizontalAlignment = TextAlignmentType.Center;
            styleItem.VerticalAlignment = TextAlignmentType.Center;
            styleItem.ForegroundColor = Color.Yellow;
            styleItem.Pattern = BackgroundType.Solid;

            StyleFlag flg = new StyleFlag();
            flg.Font = true;
            flg.CellShading = true;
            flg.Alignments = true;

            for (int i = 3; i < data.Count + 3; i++)
            {
                string partName = ws.Cells["I" + i].Value.ToString();
                if (partName == "SubTotal")
                {
                    Aspose.Cells.Range range = ws.Cells.CreateRange(i - 1, 0, 1, 20);
                    range.ApplyStyle(styleSumTotal, flg);
                    ws.Cells["I" + i].SetStyle(styleItem, flg);
                    ws.Cells["J" + i].SetStyle(styleItem, flg);
                    ws.Cells["M" + i].SetStyle(styleItem, flg);
                    ws.Cells["N" + i].SetStyle(styleItem, flg);
                    ws.Cells["O" + i].SetStyle(styleItem, flg);
                    ws.Cells["P" + i].SetStyle(styleItem, flg);
                }
                // Set height row auto
                ws.AutoFitRow(i - 1);
            }

            MemoryStream stream = new MemoryStream();
            designer.Workbook.Save(stream, SaveFormat.Xlsx);

            byte[] result = stream.ToArray();

            return File(result, "application/xlsx", "Excel" + DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss") + ".xlsx");
        }
    }
}