
using System.Threading.Tasks;
using Bottom_API.DTO;
using Bottom_API.Helpers;
namespace Bottom_API._Services.Interfaces
{
    public interface IRackLocationService
    {
        Task<bool> Add(RackLocation_Main_Dto model);
        Task<bool> Update(RackLocation_Main_Dto model);
        Task<bool> Delete(object id);
        Task<PagedList<RackLocation_Main_Dto>> Filter(PaginationParams param, FilterRackLocationParam filterParam);
        bool CheckExistRackLocation(RackLocation_Main_Dto model);
    }
}