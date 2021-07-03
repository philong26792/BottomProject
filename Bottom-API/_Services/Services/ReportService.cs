using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bottom_API._Repositories.Interfaces;
using Bottom_API._Services.Interfaces;
using Bottom_API.Data;
using Bottom_API.DTO.ReportMaterial;
using Bottom_API.Helpers;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Bottom_API._Services.Services
{
    public class ReportService : IReportService
    {
        private readonly DataContext _context;

        public ReportService(DataContext context)
        {
            _context = context;
        }

        public async Task<List<ReportMatRecExcel_Dto>> GetMaterialReceiveExcel(MaterialReceiveParam MaterialReceiveParam)
        {
            var data = await (_context.ReportMatRecExcel_Dto.FromSqlRaw("EXEC [dbo].[NSP_Receive_Material_Report] @DateType, @Date_S, @Date_E, @MO_No, @MO_Seq, @Supplier, @Status ,@Article,@Tool_ID",
            new SqlParameter("DateType", MaterialReceiveParam.DateType != 0 ? MaterialReceiveParam.DateType : (object)DBNull.Value),
            new SqlParameter("Date_S", MaterialReceiveParam.DateStart != "" ? MaterialReceiveParam.DateStart : (object)DBNull.Value),
            new SqlParameter("Date_E", MaterialReceiveParam.DateEnd != "" ? MaterialReceiveParam.DateEnd : (object)DBNull.Value),
            new SqlParameter("MO_No", MaterialReceiveParam.MoNo != "" ? MaterialReceiveParam.MoNo : (object)DBNull.Value),
            new SqlParameter("MO_Seq", MaterialReceiveParam.MoSeq != "" ? MaterialReceiveParam.MoSeq : (object)DBNull.Value),
            new SqlParameter("Supplier", (MaterialReceiveParam.Supplier != "" && MaterialReceiveParam.Supplier != "All") ? MaterialReceiveParam.Supplier : (object)DBNull.Value),
            new SqlParameter("Status", MaterialReceiveParam.Status),
            new SqlParameter("Article", MaterialReceiveParam.Article != "" ? MaterialReceiveParam.Article : (object)DBNull.Value),
            new SqlParameter("Tool_ID", MaterialReceiveParam.Tooling != "" ? MaterialReceiveParam.Tooling : (object)DBNull.Value)
                )).ToListAsync();
            data.ForEach(item => {
                if(item.I125_Status == "Y") {
                    item.I125_Status = "Y.Prod. Closed";
                }
                if(item.Order_Status == "Y") {
                    item.Order_Status = "Y.Close";
                } else if(item.Order_Status == "D") {
                    item.Order_Status = "D.Split";
                } else if(item.Order_Status == "C") {
                    item.Order_Status = "C.Cancel";
                } else if(item.Order_Status == "P") {
                    item.Order_Status = "P.Partial";
                } else {
                    item.Order_Status = "Unship";
                }
            });
            return data;
        }
    }
}