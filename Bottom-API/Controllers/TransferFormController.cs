using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Aspose.Cells;
using Bottom_API._Services.Interfaces;
using Bottom_API.DTO;
using Bottom_API.DTO.TransferForm;
using Bottom_API.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace Bottom_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransferFormController : ControllerBase
    {
        private readonly ITransferFormService _transferFormService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public TransferFormController(ITransferFormService transferFormService, IWebHostEnvironment webHostEnvironment)
        {
            _transferFormService = transferFormService;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet("GetTransferFormGenerate")]
        public async Task<IActionResult> GetTransferFormGenerate(string fromTime, string toTime, string moNo, string isSubcont, string t2Supplier, string t3Supplier, int pageNumber = 1, int pageSize = 10)
        {
            var result = await _transferFormService.GetTransferFormGerenate(fromTime, toTime, moNo, isSubcont, t2Supplier, t3Supplier, pageNumber, pageSize);
            Response.AddPagination(result.CurrentPage, result.PageSize, result.TotalCount, result.TotalPages);
            return Ok(result);
        }

        [HttpPost("GenerateTransferForm")]
        public async Task<IActionResult> GenerateTransferForm(List<Transfer_Form_Generate_Dto> generateTransferForm)
        {
            string updateBy = User.FindFirst(ClaimTypes.Name).Value;
            var result = await _transferFormService.GenerateTransferForm(generateTransferForm, updateBy);
            if (result)
            {
                return Ok();
            }

            throw new Exception("Submit failed on save");
        }

        [HttpGet("GetTransferFormPrint")]
        public async Task<IActionResult> GetTransferFormPrint(string fromTime, string toTime, string moNo, string isRelease, string t2Supplier, string t3Supplier, int pageNumber = 1, int pageSize = 10)
        {
            var result = await _transferFormService.GetTransferFormPrint(fromTime, toTime, moNo, isRelease, t2Supplier, t3Supplier, pageNumber, pageSize);
            Response.AddPagination(result.CurrentPage, result.PageSize, result.TotalCount, result.TotalPages);
            return Ok(result);
        }

        [HttpPost("ReleaseTransferForm")]
        public async Task<IActionResult> ReleaseTransferForm(List<Transfer_Form_Generate_Dto> generateTransferForm)
        {
            string updateBy = User.FindFirst(ClaimTypes.Name).Value;
            var result = await _transferFormService.ReleaseTransferForm(generateTransferForm, updateBy);
            if (result)
            {
                return Ok();
            }

            throw new Exception("Submit failed on save");
        }

        [HttpPost("GetInfoTransferFormPrintDetail")]
        public async Task<IActionResult> GetInfoTransferFormPrintDetail(List<Transfer_Form_Generate_Dto> generateTransferForm)
        {
            var result = await _transferFormService.GetInfoTransferFormPrintDetail(generateTransferForm);
            return Ok(result);
        }

        [HttpPost("SendEmail")]
        public async Task<IActionResult> SendEmail(List<Transfer_Form_Generate_Dto> generateTransferForm)
        {
            var timeNow = DateTime.Now.ToString("yyyy/MM/dd");
            string path = Path.Combine(_webHostEnvironment.ContentRootPath, "Resources\\Template\\Send_Mail_Suppllier_T3_Template.xlsx");
            string forderSaveFile = _webHostEnvironment.WebRootPath + $@"\FileSendEmailPrintTranferForm\";
            foreach (var item in generateTransferForm.GroupBy(x => x.T3_Supplier))
            {
                var dataForExcel = await _transferFormService.GetDataExcelTransferForm(generateTransferForm.Where(x => x.T3_Supplier == item.Key).ToList());
                WorkbookDesigner designer = new WorkbookDesigner();
                designer.Workbook = new Workbook(path);
                Worksheet ws = designer.Workbook.Worksheets[0];
                var fileExcelName =  dataForExcel[0].Subject + "-" +  DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss") + ".xlsx";
                if (generateTransferForm.Where(x => x.T3_Supplier == item.Key).FirstOrDefault().Is_Release == "Y")
                {
                    fileExcelName = "Released " + fileExcelName;
                }
                string pathFileExcel = forderSaveFile + fileExcelName;
                designer.SetDataSource("result", dataForExcel);
                designer.Process();
                designer.Workbook.Save(pathFileExcel, SaveFormat.Xlsx);

                //End Excel

                //Send Mail
                await _transferFormService.SendEmail(generateTransferForm.Where(x => x.T3_Supplier == item.Key).FirstOrDefault(), pathFileExcel);
            }
            return NoContent();
            throw new Exception("Send mail failed ");
        }
    }
}