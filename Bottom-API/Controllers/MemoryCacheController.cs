using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bottom_API._Repositories.Interfaces;
using Bottom_API.Data;
using Bottom_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;
using Dapper;
using Bottom_API.DTO.CompareReport;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace Bottom_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MemoryCacheController : ControllerBase
    {
        private readonly IMemoryCache memoryCache;
        private readonly DataContext context;
        private readonly IDatabaseConnectionFactory _database;
        private readonly IQRCodeMainRepository _repoQrCodeMain;
        private readonly IQRCodeDetailRepository _repoQrCodeDetail;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public MemoryCacheController(IMemoryCache memoryCache, 
                                    DataContext context,
                                    IDatabaseConnectionFactory database,
                                    IQRCodeMainRepository repoQrCodeMain,
                                    IQRCodeDetailRepository repoQrCodeDetail,
                                    IWebHostEnvironment webHostEnvironment ) {
            this.memoryCache = memoryCache;
            this.context = context;
            _database = database;
            _repoQrCodeMain = repoQrCodeMain;
            _repoQrCodeDetail = repoQrCodeDetail;
            _webHostEnvironment = webHostEnvironment;
        }
        public class QrCodeModel {
            public string QRCode_ID {get;set;}
            public string QRCode_Type {get;set;}
            public string Receive_No {get;set;}
            public int? QRCode_Version {get;set;}
            public string Tool_Size {get;set;}
            public string Order_Size {get;set;}
            public string Spec_Size {get;set;}
            public decimal? Qty {get;set;}
        }
        [HttpGet("getData")]
        public async Task<IActionResult> GetData() {
            if(!memoryCache.TryGetValue("purchase", out List<VM_WMSB_Material_Purchase> purchaseList)) {
                purchaseList = await context.VM_WMSB_Material_Purchase.ToListAsync();
                var cacheExpirationOption = new MemoryCacheEntryOptions {
                    AbsoluteExpiration = DateTime.Now.AddHours(6),
                    Priority = CacheItemPriority.Normal,
                    SlidingExpiration = TimeSpan.FromMinutes(2)
                };
                memoryCache.Set("purchase", purchaseList, cacheExpirationOption);
            }
            return Ok(purchaseList);
        }

        [HttpGet("getlinq")]
        public async Task<IActionResult> GetLinq() {
            var qrcodemain = _repoQrCodeMain.FindAll();
            var qrcodetail =  _repoQrCodeDetail.FindAll();
            var query = from a in qrcodemain join b in qrcodetail on 
            new {qrcode = a.QRCode_ID} equals 
            new {qrcode = b.QRCode_ID} 
            select new  QrCodeModel() {
                QRCode_ID = a.QRCode_ID,
                QRCode_Version = a.QRCode_Version,
                QRCode_Type = a.QRCode_Type,
                Receive_No = a.Receive_No,
                Tool_Size = b.Tool_Size,
                Order_Size = b.Order_Size,
                Spec_Size = b.Spec_Size,
                Qty = b.Qty
            };
            var data = await query.ToListAsync();
            return Ok(data);
        }

        [HttpGet("getDapper")]
        public async Task<IActionResult> GetDapper() {
            var conn = await _database.CreateConnectionAsync();
            var Is_Scanned = "Y";
            var parameter = new {Is_Scanned = Is_Scanned};
            var query = "SELECT a.QRCode_ID, a.QRCode_Version, a.QRCode_Type, a.Receive_No, b.Tool_Size, b.Order_Size, b.Spec_Size, b.Qty " +
            "FROM dbo.WMSB_QRCode_Main AS a JOIN SHCWMSB_Test.dbo.WMSB_QRCode_Detail" + 
            " AS b ON a.QRCode_ID = b.QRCode_ID where a.Is_Scanned = @Is_Scanned";
            var data = conn.Query<QrCodeModel>(query, parameter).ToList();
            return Ok(data);
        }

        [HttpGet("getDataStore")]
        public async Task<IActionResult> GetDapperInStore() {
            var conn = await _database.CreateConnectionAsync();
            var procedure = "[NSP_STOCK_COMPARE]";
            var parameter = new {Receive_Date = "2020-12-10"};
            // var query = "EXEC dbo.NSP_STOCK_COMPARE @Receive_Date = @Receive_Date";
            var data = conn.Query<StockCompare>(procedure, parameter, commandType: CommandType.StoredProcedure).ToList();
            return Ok(data);
        }
        
        [HttpGet("getDapperNotResult")]
        public async Task<IActionResult> GetDapperNotResult() {
            var conn = await _database.CreateConnectionAsync();
            var sql = "[NSP_UPDATE_CACHE_BY_SPLIT]";
            var parameter = new {MO_No = "0126401290", MO_Seq = "", Material_ID = "6AB08428"};
            var data = await conn.ExecuteAsync(sql, parameter, commandType: CommandType.StoredProcedure);
            return Ok(data);
        }


        [HttpGet("getsqlStoreNotResult")]
        public async Task<IActionResult> GetsqlStoreNotResult() {
            try
            {
                int i = int.Parse("Mudassar");
            }
            catch (Exception ex)
            {
                string dir = _webHostEnvironment.WebRootPath +  $@"\FileError\ErrorLog.txt";
                string message = string.Format("Time: {0}", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
                message += Environment.NewLine;
                message += "-----------------------------------------------------------";
                message += Environment.NewLine;
                message += string.Format("Message: {0}", ex.Message);
                message += Environment.NewLine;
                message += string.Format("StackTrace: {0}", ex.StackTrace);
                message += Environment.NewLine;
                message += string.Format("Source: {0}", ex.Source);
                message += Environment.NewLine;
                message += string.Format("TargetSite: {0}", ex.TargetSite.ToString());
                message += Environment.NewLine;
                message += "-----------------------------------------------------------";
                message += Environment.NewLine;

                // Kiểm tra nếu tồn tại file thì Clear hết conntent bên trong file
                if(System.IO.File.Exists(dir)) {
                    System.IO.File.WriteAllText(dir, String.Empty);
                }
                // Ghi nội dung error ra file
                using (StreamWriter writer = new StreamWriter(dir, true))
                    {
                        writer.WriteLine(message);
                        writer.Close();
                    }
                throw ex;
            }


            var data = await this.context.Database.ExecuteSqlRawAsync("NSP_UPDATE_CACHE_BY_SPLIT @MO_No, @MO_Seq, @Material_ID",
                    new SqlParameter("MO_No", "0126401290"),
                    new SqlParameter("MO_Seq", ""),
                    new SqlParameter("Material_ID", "6AB08428"));
            return Ok(data);
        }
    }
}