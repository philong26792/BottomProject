using System;
using System.IO;
using System.Threading.Tasks;
using Aspose.Cells;
using AutoMapper;
using Bottom_API._Services.Interfaces;
using Bottom_API.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace Bottom_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HistoryReportController : ControllerBase
    {
        private readonly IHistoryReportService _serviceHistoryReport;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public HistoryReportController( IHistoryReportService serviceHistoryReport, 
                                        IWebHostEnvironment webHostEnvironment) {
            _serviceHistoryReport = serviceHistoryReport;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpPost("excelInputReport")]
        public async Task<IActionResult> InputReport(HistoryReportParam param) {
            var data = await _serviceHistoryReport.HistoryReportInputExcel(param);
            var path = Path.Combine(_webHostEnvironment.ContentRootPath, "Resources\\Template\\HistoryReportInput.xlsx");
            WorkbookDesigner designer = new WorkbookDesigner();
            designer.Workbook = new Workbook(path);
            Worksheet ws = designer.Workbook.Worksheets[0];
            designer.SetDataSource("result", data);
            designer.Process();

            MemoryStream stream = new MemoryStream();
            designer.Workbook.Save(stream, SaveFormat.Xlsx);

            byte[] result = stream.ToArray();

            return File(result, "application/xlsx", "Excel" + DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss") + ".xlsx");
        }

        [HttpPost("excelOutputReport")]
        public async Task<IActionResult> OutputReport(HistoryReportParam param) {
            var data = await _serviceHistoryReport.HistoryReportOutputExcel(param);
            var path = Path.Combine(_webHostEnvironment.ContentRootPath, "Resources\\Template\\HistoryReportOutput.xlsx");
            WorkbookDesigner designer = new WorkbookDesigner();
            designer.Workbook = new Workbook(path);
            Worksheet ws = designer.Workbook.Worksheets[0];
            designer.SetDataSource("result", data);
            designer.Process();

            MemoryStream stream = new MemoryStream();
            designer.Workbook.Save(stream, SaveFormat.Xlsx);

            byte[] result = stream.ToArray();

            return File(result, "application/xlsx", "Excel" + DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss") + ".xlsx");
        }
    }
}