using System.Collections.Generic;
using System.Threading.Tasks;
using Bottom_API.DTO.CompareReport;
using Bottom_API.Helpers;

namespace Bottom_API._Services.Interfaces
{
    public interface ICompareReportService
    {
        Task<List<StockCompare>> GetCompare(string Receive_Date);
        Task<PagedList<StockCompare>> Search(string receive_Date, int pageNumber = 1, int pageSize = 10);
    }
}