using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bottom_API._Services.Interfaces;
using Bottom_API.Data;
using Bottom_API.DTO;
using Bottom_API.DTO.Kanban;
using Bottom_API.Helpers;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Bottom_API._Services.Services
{
    public class KanbanByRackService : IKanbanByRackService
    {
        private readonly DataContext _context;
        public KanbanByRackService( DataContext context){
            _context = context;
        }

        //Building
        public async Task<List<RackArea_Dto>> GetKanbanByRack()
        {
            return await (_context.RackArea_Dto.FromSqlRaw("EXEC  [dbo].[NSP_RACKS_AREA_BUILDING]")).ToListAsync();

        }

        //Detail
        public async Task<List<KanbanByRackAreaDetail_Dto>> GetKanbanByRackDetail(string build_id)
        {

            var dataQuery = await (_context.NSP_RACKS_AREA_RACK_LIST.FromSqlRaw("EXEC [dbo].[NSP_RACKS_AREA_RACK_LIST] @Build_ID",
            new SqlParameter("Build_ID", build_id != "" ? build_id : (object)DBNull.Value))).ToListAsync();
            var ListGroup = dataQuery.GroupBy(x => x.Area_ID);


            List<KanbanByRackAreaDetail_Dto> data = new List<KanbanByRackAreaDetail_Dto>();
            foreach (var item in ListGroup)
            {
                KanbanByRackAreaDetail_Dto byRackDetail = new KanbanByRackAreaDetail_Dto();
                var dataByrackArea = dataQuery.Where(x => x.Area_ID == item.Key).ToList();
                byRackDetail.Build_ID = dataByrackArea.FirstOrDefault().Build_ID;
                byRackDetail.Area_ID = dataByrackArea.FirstOrDefault().Area_ID;
                byRackDetail.Area_Name = dataByrackArea.FirstOrDefault().Area_Name;
                byRackDetail.RackDetail = dataByrackArea.Distinct().Select(x => new RackDetail
                {
                    Rack = x.Rack,
                    Count = x.Stk_Qty,
                    T3 = x.T3

                }).ToList();

                data.Add(byRackDetail);
            }

            return data;
        }

        //Excel Detail
        public async Task<List<Rack_Detail_T3T2_Dto>> GetDataExcelKanbanByRackDetail(string build_id)
        {

            var data = await (_context.Rack_Detail_T3T2_Dto.FromSqlRaw("EXEC [dbo].[NSP_RACKS_AREA_PO_LIST] @Rack_Location, @Build_ID",
             new SqlParameter("Rack_Location", (object)DBNull.Value),
             new SqlParameter("Build_ID",  build_id != "" ? build_id : (object)DBNull.Value))).ToListAsync();
             return data.OrderBy(x => x.Rack).ThenBy(x => x.STFStartDate).ToList();
        }

        public async Task<List<Rack_Detail_T3T2_Dto>> GetDetailByRackT2T3(string rackLocation)
        {
            var data = await (_context.Rack_Detail_T3T2_Dto.FromSqlRaw("EXEC [dbo].[NSP_RACKS_AREA_PO_LIST] @Rack_Location, @Build_ID",
             new SqlParameter("Rack_Location", rackLocation != "" ? rackLocation : (object)DBNull.Value),
             new SqlParameter("Build_ID", (object)DBNull.Value))).ToListAsync();
             return data.OrderBy(x => x.STFStartDate).ToList();
        }

        public async Task<PagedList<Rack_Detail_T3T2_Dto>> GetDetailByRackT2T3(string rackLocation, int page = 1, int pageSize = 10)
        {
            var data = await (_context.Rack_Detail_T3T2_Dto.FromSqlRaw("EXEC [dbo].[NSP_RACKS_AREA_PO_LIST] @Rack_Location, @Build_ID",
             new SqlParameter("Rack_Location", rackLocation != "" ? rackLocation : (object)DBNull.Value),
             new SqlParameter("Build_ID", (object)DBNull.Value))).ToListAsync();
            data = data.OrderBy(x => x.STFStartDate).ToList();
             return PagedList<Rack_Detail_T3T2_Dto>.Create(data, page, pageSize);
        }
    }

}