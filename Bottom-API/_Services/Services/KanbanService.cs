using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bottom_API._Services.Interfaces;
using Bottom_API.Data;
using Bottom_API.DTO.Kanban;
using Bottom_API.Helpers;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Bottom_API._Services.Services
{
    public class KanbanService : IKanbanService
    {
        private readonly DataContext _context;

        public KanbanService(DataContext context){
            _context = context;
        }
        public async Task<List<KanbanByCategories_Dto>> GetKanBanByCategory()
        {
            var data = await _context.KanbanByCategories_Dto.
            FromSqlRaw($"EXEC [dbo].[NSP_CATEGORIES_ALL_TOOL]").ToListAsync();
            return data.OrderBy(x => x.Code_ID).ToList();
        }
        public async Task<List<KanbanByCategoryDetail_Dto>> GetKanbanByCategoryDetail(string codeId)
        {
            var result = await _context.KanbanByCategoryDetail_Dto.FromSqlRaw($"EXEC [dbo].[NSP_CATEGORIES_TOOL_TYPE ] @Tool_Type",
            new SqlParameter("Tool_Type", codeId)).ToListAsync();
            result = result.OrderBy(x => x.ToolCode).ThenBy(x => x.MaterialId).ToList();
            return result;
        }

        public async Task<List<KanbanByCategoryDetailByToolCode_Dto>> GetKanbanByCategoryDetailByToolCode(string codeId, string toolCode)
        {
            var result = await _context.KanbanByCategoryDetailByToolCode_Dto.
            FromSqlRaw($"EXEC [dbo].[NSP_CATEGORIES_TOOL_CODE] @Tool_Type,@Tool_Code",
            new SqlParameter("Tool_Type", codeId),
            new SqlParameter("Tool_Code", toolCode)).ToListAsync();
            result = result.OrderBy(x => x.MaterialId).ThenBy(x => x.STF_Date).ToList();
            return result;
        }

        public async Task<List<KanbanByCategoryDetailByPo_Dto>> GetKanbanByCategoryDetailByPo(string codeId, string toolCode, string po)
        {
            var result = await _context.KanbanByCategoryDetailByPo_Dtos.
            FromSqlRaw($"EXEC [dbo].[NSP_CATEGORIES_PO] @Tool_Type,@Tool_Code,@PO",
            new SqlParameter("Tool_Type", codeId),
            new SqlParameter("Tool_Code", toolCode),
            new SqlParameter("PO", po) ).ToListAsync();
            result = result.OrderBy(x => x.STF_Date).ToList();
            return result;
        }

        public async Task<PagedList<KanbanByCategoryDetail_Dto>> GetKanbanByCategoryDetail(string codeId, int page = 1, int pageSize = 10)
        {
            var result = await _context.KanbanByCategoryDetail_Dto.FromSqlRaw($"EXEC [dbo].[NSP_CATEGORIES_TOOL_TYPE ] @Tool_Type",
            new SqlParameter("Tool_Type", codeId)).ToListAsync();
            result = result.OrderBy(x => x.ToolCode).ThenBy(x => x.MaterialId).ToList();
            return PagedList<KanbanByCategoryDetail_Dto>.Create(result, page, pageSize);
        }

        public async Task<PagedList<KanbanByCategoryDetailByToolCode_Dto>> GetKanbanByCategoryDetailByToolCode(string codeId, string toolCode, int page = 1, int pageSize = 10)
        {
            var result = await _context.KanbanByCategoryDetailByToolCode_Dto.
            FromSqlRaw($"EXEC [dbo].[NSP_CATEGORIES_TOOL_CODE] @Tool_Type,@Tool_Code",
            new SqlParameter("Tool_Type", codeId),
            new SqlParameter("Tool_Code", toolCode)).ToListAsync();
            result = result.OrderBy(x => x.MaterialId).ThenBy(x => x.STF_Date).ToList();
            return PagedList<KanbanByCategoryDetailByToolCode_Dto>.Create(result, page, pageSize);
        }
    }
}
