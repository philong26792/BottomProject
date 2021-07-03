using System.Collections.Generic;
using System.Threading.Tasks;
using Bottom_API.Models;

namespace Bottom_API._Repositories.Interfaces
{
    public interface ITransactionMainRepo : IBottomRepository<WMSB_Transaction_Main>
    {
        
        Task<WMSB_Transaction_Main> GetByInputNo(object inputNo);

        Task<bool> CheckRackLocation(object rackLocation);
        Task<bool> CheckTransacNo(string transacNo);
        Task<bool> CheckTranSheetNo(string transheetNo);
        Task AddRange(List<WMSB_Transaction_Main> param); 
    }
}