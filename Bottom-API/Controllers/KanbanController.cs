using System;
using System.IO;
using System.Linq;
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
    public class KanbanController : ControllerBase
    {
        private readonly ICodeIDDetailService _codeIDDetailService;
        private readonly IKanbanService _kanbanService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public KanbanController(ICodeIDDetailService codeIDDetailService, IKanbanService kanbanService, IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _kanbanService = kanbanService;
            _codeIDDetailService = codeIDDetailService;
        }

        [HttpGet("getkanbancategory")]
        public async Task<IActionResult> GetKanBanByCategory()
        {
            var data = await _kanbanService.GetKanBanByCategory();
            return Ok(data);
        }

        [HttpGet("getkanbancategorydetail")]
        public async Task<IActionResult> GetKanBanByCategoryDetail(string codeId, int pageNumber = 1, int pageSize = 10)
        {
            var data = await _kanbanService.GetKanbanByCategoryDetail(codeId, pageNumber, pageSize);
            Response.AddPagination(data.CurrentPage, data.PageSize, data.TotalCount, data.TotalPages);
            return Ok(data);
        }

        [HttpGet("exportexcelgetkanbancategorydetail")]
        public async Task<IActionResult> ExportExcelKanBanByCategoryDetail(string codeId)
        {
            var data = await _kanbanService.GetKanbanByCategoryDetail(codeId);

            string codeName = _codeIDDetailService.GetCodeName(codeId);
            string codeIdAndName = codeId + " " + codeName;
            decimal? sumQty = data.Select(x => x.Qty).Sum();

            var path = Path.Combine(_webHostEnvironment.ContentRootPath, "Resources\\Template\\KanbanByCategoryDetail.xlsx");
            WorkbookDesigner designer = new WorkbookDesigner();
            designer.Workbook = new Workbook(path);

            Worksheet ws = designer.Workbook.Worksheets[0];
            // Gán giá trị tĩnh
            ws.Cells["A1"].PutValue("TTL PRS :" + sumQty);
            ws.Cells["B1"].PutValue(codeIdAndName);

            designer.SetDataSource("result", data);
            designer.Process();

            MemoryStream stream = new MemoryStream();
            designer.Workbook.Save(stream, SaveFormat.Xlsx);

            byte[] result = stream.ToArray();

            return File(result, "application/xlsx", "Excel" + DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss") + ".xlsx");
        }

        [HttpGet("getkanbancategorydetailbytoolcode")]
        public async Task<IActionResult> GetKanBanByCategoryDetailByToolCode(string codeId, string toolCode, int pageNumber = 1, int pageSize = 10)
        {
            var data = await _kanbanService.GetKanbanByCategoryDetailByToolCode(codeId, toolCode, pageNumber, pageSize);
            Response.AddPagination(data.CurrentPage, data.PageSize, data.TotalCount, data.TotalPages);
            return Ok(data);
        }

        [HttpGet("exportexcelgetkanbancategorydetailbytoolcode")]
        public async Task<IActionResult> ExportExcelKanBanByCategoryDetailByToolCode(string codeId, string toolCode)
        {
            var data = await _kanbanService.GetKanbanByCategoryDetailByToolCode(codeId, toolCode);

            string codeName = _codeIDDetailService.GetCodeName(codeId);
            string codeIdAndName = codeId + " " + codeName;
            decimal? sumQty = data.Select(x => x.Qty).Sum();

            var path = Path.Combine(_webHostEnvironment.ContentRootPath, "Resources\\Template\\KanbanByCategoryDetailByToolCode.xlsx");
            WorkbookDesigner designer = new WorkbookDesigner();
            designer.Workbook = new Workbook(path);

            Worksheet ws = designer.Workbook.Worksheets[0];
            // Gán giá trị tĩnh
            ws.Cells["A1"].PutValue("TTL PRS : " + sumQty);
            ws.Cells["B1"].PutValue(codeIdAndName);
            ws.Cells["C1"].PutValue("Tool ID : " + toolCode);

            designer.SetDataSource("result", data);
            designer.Process();

            MemoryStream stream = new MemoryStream();
            designer.Workbook.Save(stream, SaveFormat.Xlsx);

            byte[] result = stream.ToArray();

            return File(result, "application/xlsx", "Excel" + DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss") + ".xlsx");
        }

        [HttpGet("getkanbancategorydetailbypo")]
        public async Task<IActionResult> GetKanBanByCategoryDetailByPo(string codeId, string toolCode, string po)
        {
            var data = await _kanbanService.GetKanbanByCategoryDetailByPo(codeId, toolCode, po);
            return Ok(data);
        }

        [HttpGet("exportexcelgetkanbancategorydetailbypo")]
        public async Task<IActionResult> ExportExcelKanBanByCategoryDetailByPo(string codeId, string toolCode, string po)
        {
            var data = await _kanbanService.GetKanbanByCategoryDetailByPo(codeId, toolCode, po);

            string codeName = _codeIDDetailService.GetCodeName(codeId);
            string codeIdAndName = codeId + " " + codeName;
            decimal? sumQty = data.Select(x => x.Qty).Sum();

            var path = Path.Combine(_webHostEnvironment.ContentRootPath, "Resources\\Template\\KanbanByCategoryDetailByPo.xlsx");
            WorkbookDesigner designer = new WorkbookDesigner();
            designer.Workbook = new Workbook(path);

            Worksheet ws = designer.Workbook.Worksheets[0];
            // Gán giá trị tĩnh
            ws.Cells["A1"].PutValue("TTL PRS :" + sumQty);
            ws.Cells["B1"].PutValue(codeIdAndName);
            ws.Cells["C1"].PutValue("Tool ID : " + toolCode);
            ws.Cells["D1"].PutValue("PO " + po.Substring(0, 10));

            designer.SetDataSource("result", data);
            designer.Process();

            MemoryStream stream = new MemoryStream();
            designer.Workbook.Save(stream, SaveFormat.Xlsx);

            byte[] result = stream.ToArray();

            return File(result, "application/xlsx", "Excel" + DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss") + ".xlsx");
        }
    }
}