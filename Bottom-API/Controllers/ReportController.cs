using System;
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
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ReportController(IReportService reportService, IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _reportService = reportService;
        }

        [HttpPost("exportmatrec")]
        public async Task<IActionResult> GetMaterialReceiveExcel(MaterialReceiveParam MaterialReceiveParam)
        {
            if (MaterialReceiveParam.Article != "")
                MaterialReceiveParam.Article = MaterialReceiveParam.Article.Replace("*", "%");
            if (MaterialReceiveParam.Tooling != "")
                MaterialReceiveParam.Tooling = MaterialReceiveParam.Tooling.Replace("*", "%");
            var data = await _reportService.GetMaterialReceiveExcel(MaterialReceiveParam);
            var path = Path.Combine(_webHostEnvironment.ContentRootPath, "Resources\\Template\\MaterialReceive.xlsx");
            WorkbookDesigner designer = new WorkbookDesigner();
            designer.Workbook = new Workbook(path);
            Worksheet ws = designer.Workbook.Worksheets[0];
            if(data.Count > 0) {
                ws.Cells["A1"].PutValue("Version: " + data[0].Version.Trim());
                ws.Cells["A2"].PutValue("Version Update Time: " + data[0].Upload_Time);
                ws.Cells["A3"].PutValue("Report Download Time: " + DateTime.Now);
            }
            foreach (var item in data)
            {
                item.Article = item.Article.Trim();
                item.Model_Name = item.Model_Name.Trim();
                item.Model_No = item.Model_No.Trim();
                item.Material_Name = item.Material_Name.Trim();
                item.MO_No = item.MO_No.Trim();
            }

            designer.SetDataSource("result", data);
            designer.Process();
            for (int i = 2; i < data.Count; i++)
            {
                ws.Cells.SetRowHeight(i, 15);
            }

            MemoryStream stream = new MemoryStream();
            if (data.Count > 0)
            {
                designer.Workbook.Save(stream, SaveFormat.Xlsx);
            }

            byte[] result = stream.ToArray();

            return File(result, "application/xlsx", "Excel" + DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss") + ".xlsx");
        }
    }
}