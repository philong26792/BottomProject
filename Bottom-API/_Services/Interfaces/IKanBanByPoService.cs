using System.Collections.Generic;
using System.Threading.Tasks;
using Bottom_API.DTO.Kanban;
using Bottom_API.Helpers;
using Bottom_API.Models;

namespace Bottom_API._Services.Interfaces
{
    public interface IKanBanByPoService
    {
        Task<PagedList<KanbanByPo_Dto>> GetKanbanByPo (KanbanByPoParam kanbanByPoParam, PaginationParams paginationParams,bool isPaging=true);
        Task<List<KanbanByPoDetail_Dto>> GetKanbanByPoDetail (string moNo, string moSeq);
        Task<List<NSP_REPORT_DETAIL_NO_SIZE_2ND>> GetKanbanByPoDetailByReceivingType (string moNo, string moSeq, string materialId);

          Task<List<NSP_REPORT_DETAIL_NO_SIZE>> GetKanbanByPoDetailExcel (string moNo, string moSeq);
        Task<List<string>> GetLine();
        Task<object> GetSupplier();

        Task<List<KanbanByPoDetailExcel_Dto>> GetKanbanByPoDetailMainExcel(KanbanByPoParam kanbanByPoParam);
    }
}