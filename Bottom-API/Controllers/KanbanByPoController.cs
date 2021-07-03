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
    [ApiController]
    [Route("api/[controller]")]
    public class KanbanByPoController : ControllerBase
    {
        private readonly IKanBanByPoService _kanbanByPoService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public KanbanByPoController(IKanBanByPoService kanbanByPoService, IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _kanbanByPoService = kanbanByPoService;
        }
        [HttpPost]
        public async Task<IActionResult> GetKanbanByPo([FromQuery] PaginationParams paginationParams, KanbanByPoParam kanbanByPoParam)
        {
            var result = await _kanbanByPoService.GetKanbanByPo(kanbanByPoParam, paginationParams);
            Response.AddPagination(result.CurrentPage, result.PageSize, result.TotalCount, result.TotalPages);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetKanbanByPoDetail(string moNo, string moSeq)
        {
            var result = await _kanbanByPoService.GetKanbanByPoDetail(moNo, moSeq);
            return Ok(result);
        }

        [HttpGet("GetKanbanByPoDetailReceivingType")]
        public async Task<IActionResult> GetKanbanByPoDetailByReceivingType(string moNo, string moSeq, string materialId)
        {
            var result = await _kanbanByPoService.GetKanbanByPoDetailByReceivingType(moNo, moSeq, materialId);
            return Ok(result);
        }

        [HttpGet("getline")]
        public async Task<IActionResult> GetAllLine()
        {
            var result = await _kanbanByPoService.GetLine();
            return Ok(result);
        }

        [HttpGet("getsupplier")]
        public async Task<IActionResult> GetAllSupplier()
        {
            var result = await _kanbanByPoService.GetSupplier();
            return Ok(result);
        }


        [HttpPost("Excel-main-detail")]
        public async Task<IActionResult> ExportExcelMainDetail(KanbanByPoParam kanbanByPoParam)
        {
            var data = await _kanbanByPoService.GetKanbanByPoDetailMainExcel(kanbanByPoParam);
            var path = Path.Combine(_webHostEnvironment.ContentRootPath, "Resources\\Template\\KanbanByPoMainDetail.xlsx");
            WorkbookDesigner designer = new WorkbookDesigner();
            designer.Workbook = new Workbook(path);
            Worksheet ws = designer.Workbook.Worksheets[0];

            designer.SetDataSource("result", data);
            designer.Process();

            // custom style 
            for (int i = 2; i < data.Count + 2; i++)
            {
                //Custom cell Kind nếu kind bằng 6 hoặc 7 thì cho background màu vàng
                Cell cellKind = ws.Cells["Q" + i];
                CustomStyle(ref cellKind);
            }

            MemoryStream stream = new MemoryStream();
            designer.Workbook.Save(stream, SaveFormat.Xlsx);

            byte[] result = stream.ToArray();

            return File(result, "application/xlsx", "Excel" + DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss") + ".xlsx");
        }

        [HttpPost("Excel-main-summary")]
        public async Task<IActionResult> ExportExcelMainSummary([FromQuery] PaginationParams paginationParams, KanbanByPoParam kanbanByPoParam)
        {
            var data = await _kanbanByPoService.GetKanbanByPo(kanbanByPoParam, paginationParams, false);
            var path = Path.Combine(_webHostEnvironment.ContentRootPath, "Resources\\Template\\KanbanByPoMainSummary.xlsx");
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

        [HttpGet("Excel-Detail")]
        public async Task<IActionResult> ExportExcelDetail(string moNo, string moSeq)
        {
            var data = await _kanbanByPoService.GetKanbanByPoDetailExcel(moNo, moSeq);
            var path = Path.Combine(_webHostEnvironment.ContentRootPath, "Resources\\Template\\KanbanByPoDetail2.xlsx");
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

        [NonAction]
        public void CustomStyle(ref Cell cellCustom)
        {
            if (cellCustom.Value != null)
            {
                string value = cellCustom.Value.ToString();
                if (value == "6.Accept. Setting Qty" || value == "7.Current setting In. Qty")
                {
                    Style styleCustom = cellCustom.GetStyle();
                    styleCustom.Pattern = BackgroundType.Solid;
                    styleCustom.ForegroundColor = Color.Yellow;
                    cellCustom.SetStyle(styleCustom);
                }
            }
        }
    }
}