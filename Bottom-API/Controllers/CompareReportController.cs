
using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Aspose.Cells;
using Bottom_API._Services.Interfaces;
using Bottom_API.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace Bottom_API.Controllers
{
    [Route("api/[controller]")]
    public class CompareReportController : ControllerBase
    {
        private readonly ICompareReportService _serviceCompare;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public CompareReportController( ICompareReportService serviceCompare,
                                        IWebHostEnvironment webHostEnvironment) {
            _serviceCompare = serviceCompare;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(string receive_Date, int pageNumber = 1, int pageSize = 10) {
            var data = await _serviceCompare.Search(receive_Date, pageNumber, pageSize);
            Response.AddPagination(data.CurrentPage, data.PageSize, data.TotalCount, data.TotalPages);
            return Ok(data);
        }

        [HttpGet("exportExcel")]
        public async Task<IActionResult> ExportExcel(string receive_Date) {
            var data = await _serviceCompare.GetCompare(receive_Date);
            var count = data.Count;
            var path = Path.Combine(_webHostEnvironment.ContentRootPath, "Resources\\Template\\CompareReport.xlsx");
            WorkbookDesigner designer = new WorkbookDesigner();
            designer.Workbook = new Workbook(path);
            Worksheet worksheet = designer.Workbook.Worksheets[0];
            designer.SetDataSource("result", data);
            designer.Process();

            // ----------------Add style excel------------------------//
            Style stl = designer.Workbook.CreateStyle();
            stl.ForegroundColor = Color.FromArgb(210,105,30);
            stl.Pattern = BackgroundType.Solid;
            StyleFlag flg = new StyleFlag();
            flg.Font = true;
            flg.CellShading = true;
            flg.Alignments = true;


            for(int i = 17; i < count + 17; i ++) {
                var checkCell1 = worksheet.Cells["K" + i].Value.ToString();
                var checkCell2 = worksheet.Cells["O" + i].Value.ToString();
                Aspose.Cells.Range range = worksheet.Cells.CreateRange(i-1,0,1,16);
                if(checkCell1 == "0" || checkCell2 == "0") {
                    range.ApplyStyle(stl, flg);
                }
            }
            //--------------------End add Style-------------------------//

            // for (int i = 16; i < count + 16; i++)
            // {
            //     worksheet.Cells["H"+ i].PutValue(receive_Date);
            // }
            worksheet.Cells["E2"].PutValue(data[0].Freeze_Date);

            MemoryStream stream = new MemoryStream();
            designer.Workbook.Save(stream, SaveFormat.Xlsx);

            byte[] result = stream.ToArray();

            return File(result, "application/xlsx", "Excel" + DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss") + ".xlsx");
        }
    }
}