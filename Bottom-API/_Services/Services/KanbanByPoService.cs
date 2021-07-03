using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Bottom_API._Repositories.Interfaces;
using Bottom_API._Repositories.Interfaces.DbHpBasic;
using Bottom_API._Services.Interfaces;
using Bottom_API.Data;
using Bottom_API.DTO.Kanban;
using Bottom_API.Helpers;
using Bottom_API.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace Bottom_API._Services.Services
{
    public class KanbanByPoService : IKanBanByPoService
    {
        private readonly IMaterialPurchaseRepository _materialPurchaseRepository;
        private readonly DataContext _context;
        private readonly IHPVendorRepository _repoHPVendor;
        private readonly IMesMoRepository _mesMoRepository;
        private readonly ICacheRepository _cacheRepository;

        public KanbanByPoService(IViewMesMoRepository viewMesMoRepository,
            IViewPoRepository viewPoRepository,
            ITransactionMainRepo transactionMainRepo,
            IMaterialPurchaseRepository materialPurchaseRepository,
            IPackingListRepository packingListRepository, DataContext context,
            IMaterialViewRepository materialViewRepository,
            IHPVendorRepository repoHPVendor,
            IMesMoRepository mesMoRepository,
            ICacheRepository cacheRepository)
        {
            _materialPurchaseRepository = materialPurchaseRepository;
            _context = context;
            _repoHPVendor = repoHPVendor;
            _mesMoRepository = mesMoRepository;
            _cacheRepository = cacheRepository;
        }

        public async Task<PagedList<KanbanByPo_Dto>> GetKanbanByPo(KanbanByPoParam kanbanByPoParam, PaginationParams paginationParams, bool isPaging = true)
        {
            // gọi store procedure
            var data = await (_context.KanbanByPo_Dto.FromSqlRaw("EXEC [dbo].[NSP_REPORT_MAIN] @DateType, @Date_S, @Date_E, @Line, @MO_No, @MO_Seq, @Material_No, @Supplier, @Model_Name, @Article ",
            new SqlParameter("DateType", kanbanByPoParam.DateType != 0 ? kanbanByPoParam.DateType : (object)DBNull.Value),
            new SqlParameter("Date_S", kanbanByPoParam.DateStart != "" ? kanbanByPoParam.DateStart : (object)DBNull.Value),
            new SqlParameter("Date_E", kanbanByPoParam.DateEnd != "" ? kanbanByPoParam.DateEnd : (object)DBNull.Value),
            new SqlParameter("Line", kanbanByPoParam.Line != "" ? kanbanByPoParam.Line : (object)DBNull.Value),
            new SqlParameter("MO_No", kanbanByPoParam.MoNo != "" ? kanbanByPoParam.MoNo : (object)DBNull.Value),
            new SqlParameter("MO_Seq", kanbanByPoParam.MoSeq != "" ? kanbanByPoParam.MoSeq : (object)DBNull.Value),
            new SqlParameter("Material_No", (object)DBNull.Value),
            new SqlParameter("Supplier", (kanbanByPoParam.Supplier != "" && kanbanByPoParam.Supplier != "All") ? kanbanByPoParam.Supplier : (object)DBNull.Value),
            new SqlParameter("Model_Name", kanbanByPoParam.ModelName != "" ? kanbanByPoParam.ModelName : (object)DBNull.Value),
            new SqlParameter("Article", kanbanByPoParam.Article != "" ? kanbanByPoParam.Article : (object)DBNull.Value)

            )).ToListAsync();
            data.ForEach(item => {
                if(item.Prod_Status == "Y") {
                    item.Prod_Status = "Y.Prod. Closed";
                } 
            });
            data = data.OrderBy(x => x.Date).ToList();

            return PagedList<KanbanByPo_Dto>.Create(data, paginationParams.PageNumber, paginationParams.PageSize, isPaging);
        }


        public async Task<List<KanbanByPoDetail_Dto>> GetKanbanByPoDetail(string moNo, string moSeq)
        {
            var data = await (_context.NSP_REPORT_DETAIL_NO_SIZE.FromSqlRaw("EXEC [dbo].[NSP_REPORT_DETAIL_NO_SIZE] @MO_No , @MO_Seq",
            new SqlParameter("MO_No", (moNo != "" && moNo != null) ? moNo : (object)DBNull.Value),
            new SqlParameter("MO_Seq", (moSeq != "" && moSeq != null) ? moSeq : (object)DBNull.Value))).ToListAsync();

            List<KanbanByPoDetail_Dto> listData = new List<KanbanByPoDetail_Dto>();

            var dataGroup = data.GroupBy(x => new { x.Material_Name, x.MO_No, x.MO_Seq, x.Material_NO, x.Confirm_Date, x.T2_Supplier, x.T2_Supplier_Name, x.CustomerPart, x.Rack_Location, x.Source_Count });
            foreach (var item in dataGroup)
            {
                KanbanByPoDetail_Dto kanban = new KanbanByPoDetail_Dto();
                kanban.MO_No = item.Key.MO_No;
                kanban.MO_Seq = item.Key.MO_Seq;
                kanban.Material_NO = item.Key.Material_NO;
                kanban.Confirm_Date = item.Key.Confirm_Date;
                kanban.T2_Supplier_Name = item.Key.T2_Supplier_Name;
                kanban.T2_Supplier = item.Key.T2_Supplier;
                kanban.CustomerPart = item.Key.CustomerPart;
                kanban.Material_Name = item.Key.Material_Name;
                kanban.Rack_Location = item.Key.Rack_Location;
                kanban.Source_Count = item.Key.Source_Count;
                kanban.Prs = data.Where(x => x.Material_NO.Trim() == item.Key.Material_NO.Trim() && x.Title_ID == "Prs" && x.T2_Supplier == item.Key.T2_Supplier).FirstOrDefault().Total_Pairs;
                kanban.RevQty = data.Where(x => x.Material_NO.Trim() == item.Key.Material_NO.Trim() && x.Title_ID == "Rev_Qty" && x.T2_Supplier == item.Key.T2_Supplier).FirstOrDefault().Total_Pairs;
                kanban.StkQty = data.Where(x => x.Material_NO.Trim() == item.Key.Material_NO.Trim() && x.Title_ID == "Stk_Qty" && x.T2_Supplier == item.Key.T2_Supplier).FirstOrDefault().Total_Pairs;
                listData.Add(kanban);
            }
            return listData.OrderBy(x => x.Confirm_Date).ToList();
        }

        public async Task<List<NSP_REPORT_DETAIL_NO_SIZE_2ND>> GetKanbanByPoDetailByReceivingType(string moNo, string moSeq, string materialId)
        {
            var data = await (_context.NSP_REPORT_DETAIL_NO_SIZE_2ND.FromSqlRaw("EXEC [dbo].[NSP_REPORT_DETAIL_NO_SIZE_2ND] @MO_No , @MO_Seq, @Material_No",
            new SqlParameter("MO_No", (moNo != "" && moNo != null) ? moNo : (object)DBNull.Value),
            new SqlParameter("MO_Seq", (moSeq != "" && moSeq != null) ? moSeq : string.Empty),
            new SqlParameter("Material_No", (materialId != "" && materialId != null) ? materialId : (object)DBNull.Value))).ToListAsync();

            return data;
        }

        public async Task<List<NSP_REPORT_DETAIL_NO_SIZE>> GetKanbanByPoDetailExcel(string moNo, string moSeq)
        {
            var data = await (_context.NSP_REPORT_DETAIL_NO_SIZE.FromSqlRaw("EXEC [dbo].[NSP_REPORT_DETAIL_NO_SIZE] @MO_No , @MO_Seq",
            new SqlParameter("MO_No", (moNo != "" && moNo != null) ? moNo : (object)DBNull.Value),
            new SqlParameter("MO_Seq", (moSeq != "" && moSeq != null) ? moSeq : (object)DBNull.Value))).ToListAsync();

            return data;

        }

        public async Task<List<KanbanByPoDetailExcel_Dto>> GetKanbanByPoDetailMainExcel(KanbanByPoParam kanbanByPoParam)
        {
            var data = await (_context.KanbanByPoDetailExcel_Dto.FromSqlRaw("EXEC [dbo].[NSP_REPORT_DETAIL] @DateType, @Date_S, @Date_E, @Line, @MO_No, @MO_Seq, @Material_No, @Supplier, @Model_Name, @Article ",
            new SqlParameter("DateType", kanbanByPoParam.DateType != 0 ? kanbanByPoParam.DateType : (object)DBNull.Value),
            new SqlParameter("Date_S", kanbanByPoParam.DateStart != "" ? kanbanByPoParam.DateStart : (object)DBNull.Value),
            new SqlParameter("Date_E", kanbanByPoParam.DateEnd != "" ? kanbanByPoParam.DateEnd : (object)DBNull.Value),
            new SqlParameter("Line", kanbanByPoParam.Line != "" ? kanbanByPoParam.Line : (object)DBNull.Value),
            new SqlParameter("MO_No", kanbanByPoParam.MoNo != "" ? kanbanByPoParam.MoNo : (object)DBNull.Value),
            new SqlParameter("MO_Seq", kanbanByPoParam.MoSeq != "" ? kanbanByPoParam.MoSeq : (object)DBNull.Value),
            new SqlParameter("Material_No", (object)DBNull.Value),
            new SqlParameter("Supplier", kanbanByPoParam.Supplier != "" ? kanbanByPoParam.Supplier : (object)DBNull.Value),
            new SqlParameter("Model_Name", kanbanByPoParam.ModelName != "" ? kanbanByPoParam.ModelName : (object)DBNull.Value),
            new SqlParameter("Article", kanbanByPoParam.Article != "" ? kanbanByPoParam.Article : (object)DBNull.Value)

        )).ToListAsync();
            var result = data.Select(x =>
                {
                    x.Material_No = x.Material_No.Trim() == "ZZZZZZZZZZ" ? "" : x.Material_No;
                    return x;
                }).OrderBy(x => x.Plan_Start_STF).ToList();
            return result;
        }


        public async Task<List<string>> GetLine()
        {
            var listMaterialPurchase = await _materialPurchaseRepository.FindAll()
                                .Select(x => new {x.Factory_ID, x.MO_No}).Distinct().ToListAsync();

            var listMesMo = await _mesMoRepository.FindAll()
                            .Select(x => new {x.Factory_ID, x.MO_No, x.Line_ID_ASY}).Distinct().ToListAsync();

            var data = listMaterialPurchase.Join(listMesMo, x => new { x.Factory_ID, x.MO_No  }, y => new { y.Factory_ID, y.MO_No }, 
                                        (x, y) => y.Line_ID_ASY)
                                        .Distinct().OrderBy(z => z).ToList();
            return data;
        }

        public async Task<object> GetSupplier()
        {
            var listMaterialPurchase = await _materialPurchaseRepository.FindAll(x => !String.IsNullOrEmpty(x.Supplier_ID))
                                        .Select(x => x.Supplier_ID).Distinct().ToListAsync();

            var listHPVendor = await _repoHPVendor.FindAll(x => !String.IsNullOrEmpty(x.Vendor_No))
                                    .Select(x => new { x.Vendor_No, x.Vendor_Name }).Distinct().ToListAsync();

            var data = (from T1 in listMaterialPurchase
                        join T2 in listHPVendor
            on T1.Trim() equals T2.Vendor_No.Trim()
                        select new
                        {
                            SupplierNo = T1.Trim(),
                            SupplierName = T2.Vendor_Name.Trim()
                        }).Distinct()
                        .OrderBy(x => x.SupplierNo)
                        .ToList();
            return data;
        }
    }
}