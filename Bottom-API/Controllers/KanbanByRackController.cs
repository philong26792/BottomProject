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
    public class KanbanByRackController : ControllerBase
    {
        private readonly IKanbanByRackService _kanbanByRackService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public KanbanByRackController(IKanbanByRackService kanbanByRackService,
                                        IWebHostEnvironment webHostEnvironment)
        {
            _kanbanByRackService = kanbanByRackService;
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpGet("getkanbanbyrack")]
        public async Task<IActionResult> GetKanBanByRack()
        {
            var data = await _kanbanByRackService.GetKanbanByRack();
            return Ok(data);
        }
        [HttpGet("getkanbanbyrackdetail/{build_id}")]
        public async Task<IActionResult> GetKanbanByRackDetail(string build_id)
        {
            var data = await _kanbanByRackService.GetKanbanByRackDetail(build_id);
            return Ok(data);
        }

        [HttpGet("exportExcelKanbanByRackDetail/{build_id}")]
        public async Task<IActionResult> ExportExcelKanbanByRackDetail(string build_id)
        {
            var data = await _kanbanByRackService.GetDataExcelKanbanByRackDetail(build_id);

            var TTL_PRRS = "TTL PRRS: " + (data.Count() > 0 ? data[0].TTL_PRS : 0);

            var path = Path.Combine(_webHostEnvironment.ContentRootPath, "Resources\\Template\\KanbanByRackDetail.xlsx");
            WorkbookDesigner designer = new WorkbookDesigner();
            designer.Workbook = new Workbook(path);
            Worksheet ws = designer.Workbook.Worksheets[0];

            // Gan Gia tri vao
            ws.Cells["A1"].PutValue(TTL_PRRS);

            designer.SetDataSource("result", data);
            designer.Process();

            MemoryStream stream = new MemoryStream();
            designer.Workbook.Save(stream, SaveFormat.Xlsx);

            byte[] result = stream.ToArray();

            return File(result, "application/xlsx", "ExcelKanbanByRackDetail" + DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss") + ".xlsx");
        }

        [HttpGet("getlistpot3")]
        public async Task<IActionResult> GetListPoT3(string rackLoacion, int pageNumber = 1, int pageSize = 10)
        {
            var data = await _kanbanByRackService.GetDetailByRackT2T3(rackLoacion, pageNumber, pageSize);
            var poQty = (await _kanbanByRackService.GetDetailByRackT2T3(rackLoacion)).GroupBy(x => x.MO_No).Count();
            Response.AddPagination(data.CurrentPage, data.PageSize, data.TotalCount, data.TotalPages);
            return Ok(new {
                data,
                poQty
            });
        }

        [HttpGet("exportexcellistpot3")]
        public async Task<IActionResult> ExportExcelListPoT3(string rackLocation)
        {
            var data = await _kanbanByRackService.GetDetailByRackT2T3(rackLocation);
            var poQty = data.GroupBy(x => x.MO_No).Count();

            var TTL_PRRS = "TTL PRRS: " + (data != null ? data[0].TTL_PRS : 0).ToString();
            var PO_Qty = "PO: " + poQty.ToString();

            var path = Path.Combine(_webHostEnvironment.ContentRootPath, "Resources\\Template\\KanbanByRackPoListDetailT3.xlsx");
            WorkbookDesigner designer = new WorkbookDesigner();
            designer.Workbook = new Workbook(path);
            Worksheet ws = designer.Workbook.Worksheets[0];

            // Gan Gia tri tĩnh vào file excel
            ws.Cells["A1"].PutValue(TTL_PRRS);
            ws.Cells["B1"].PutValue(rackLocation);
            ws.Cells["C1"].PutValue(PO_Qty);

            designer.SetDataSource("result", data);
            designer.Process();

            MemoryStream stream = new MemoryStream();
            designer.Workbook.Save(stream, SaveFormat.Xlsx);

            byte[] result = stream.ToArray();

            return File(result, "application/xlsx", "Excel" + DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss") + ".xlsx");
        }

        [HttpGet("getdetailRack")]
        public async Task<IActionResult> GetKanBanRackDetail(string rackLocation, int pageNumber = 1, int pageSize = 10)
        {
            var data = await _kanbanByRackService.GetDetailByRackT2T3(rackLocation, pageNumber, pageSize);
            var poQty = (await _kanbanByRackService.GetDetailByRackT2T3(rackLocation)).GroupBy(x => x.MO_No).Count();
            Response.AddPagination(data.CurrentPage, data.PageSize, data.TotalCount, data.TotalPages);
            return Ok(new {
                data,
                poQty
            });
        }

        [HttpGet("exportExcelRackDetail/{rackLocation}")]
        public async Task<IActionResult> ExportExcelRackDetail(string rackLocation)
        {
            var data = await _kanbanByRackService.GetDetailByRackT2T3(rackLocation);
            var poQty = data.GroupBy(x => x.MO_No).Count();

            var TTL_PRRS = "TTL PRRS: " + (data.Count() > 0 ? data[0].TTL_PRS : 0);
            var PO_Qty = "PO: " + poQty.ToString();

            var path = Path.Combine(_webHostEnvironment.ContentRootPath, "Resources\\Template\\KanbanByRackDetailT2.xlsx");
            WorkbookDesigner designer = new WorkbookDesigner();
            designer.Workbook = new Workbook(path);
            Worksheet ws = designer.Workbook.Worksheets[0];

            // Gan Gia tri vao
            ws.Cells["A1"].PutValue(TTL_PRRS);
            ws.Cells["B1"].PutValue(rackLocation);
            ws.Cells["C1"].PutValue(PO_Qty);

            designer.SetDataSource("result", data);
            designer.Process();

            MemoryStream stream = new MemoryStream();
            designer.Workbook.Save(stream, SaveFormat.Xlsx);

            byte[] result = stream.ToArray();

            return File(result, "application/xlsx", "ExcelKanbanRackDetail" + DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss") + ".xlsx");
        }
    }
}