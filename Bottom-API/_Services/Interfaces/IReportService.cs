using System.Collections.Generic;
using System.Threading.Tasks;
using Bottom_API.DTO.ReportMaterial;
using Bottom_API.Helpers;

namespace Bottom_API._Services.Interfaces
{
    public interface IReportService
    {
        Task<List<ReportMatRecExcel_Dto>> GetMaterialReceiveExcel (MaterialReceiveParam MaterialReceiveParam);
    }
}