using System.Collections.Generic;
using System.Threading.Tasks;
using Bottom_API.Models;

namespace Bottom_API._Services.Interfaces
{
    public interface ICodeIDDetailService
    {
        Task<List<WMS_Code>> GetFactory();
        Task<List<WMS_Code>> GetWH();
        Task<List<WMS_Code>> GetBuilding();
        Task<List<WMS_Code>> GetFloor();
        Task<List<WMS_Code>> GetArea();
        Task<List<WMS_Code>> GetKanBanByCategory();
        string GetCodeName(string codeId);
    }
}