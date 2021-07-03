using System.Collections.Generic;
using System.Threading.Tasks;
using Bottom_API.DTO;
using Bottom_API.DTO.Kanban;
using Bottom_API.Helpers;

namespace Bottom_API._Services.Interfaces
{
    public interface IKanbanByRackService
    {
        Task<List<RackArea_Dto>> GetKanbanByRack();
        Task<List<KanbanByRackAreaDetail_Dto>> GetKanbanByRackDetail(string build_id);
        Task<List<Rack_Detail_T3T2_Dto>> GetDataExcelKanbanByRackDetail(string build_id);
        Task<List<Rack_Detail_T3T2_Dto>> GetDetailByRackT2T3(string rackLocation);
        Task<PagedList<Rack_Detail_T3T2_Dto>> GetDetailByRackT2T3(string rackLocation, int page = 1, int pageSize = 10);
    }
}