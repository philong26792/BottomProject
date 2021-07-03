using System.Collections.Generic;
using System.Threading.Tasks;
using Bottom_API.DTO.Kanban;
using Bottom_API.Helpers;

namespace Bottom_API._Services.Interfaces
{
    public interface IKanbanService
    {
        Task<List<KanbanByCategoryDetail_Dto>> GetKanbanByCategoryDetail(string codeId);
        Task<PagedList<KanbanByCategoryDetail_Dto>> GetKanbanByCategoryDetail(string codeId, int page = 1, int pageSize = 10);

        Task<List<KanbanByCategoryDetailByToolCode_Dto>> GetKanbanByCategoryDetailByToolCode(string codeId, string toolCode);
        Task<PagedList<KanbanByCategoryDetailByToolCode_Dto>> GetKanbanByCategoryDetailByToolCode(string codeId, string toolCode, int page = 1, int pageSize = 10);
        Task<List<KanbanByCategoryDetailByPo_Dto>> GetKanbanByCategoryDetailByPo(string codeId, string toolCode, string po);
        Task<List<KanbanByCategories_Dto>> GetKanBanByCategory();
    }
}