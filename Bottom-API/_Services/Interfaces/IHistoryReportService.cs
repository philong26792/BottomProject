using System.Collections.Generic;
using System.Threading.Tasks;
using Bottom_API.DTO.HistoryReport;
using Bottom_API.Helpers;

namespace Bottom_API._Services.Interfaces
{
    public interface IHistoryReportService
    {
        Task<List<HistoryInputReport>> HistoryReportInputExcel(HistoryReportParam param);
        Task<List<HistoryOutputReport>> HistoryReportOutputExcel(HistoryReportParam param);
    }
}