using System.Collections.Generic;
using System.Threading.Tasks;
using Bottom_API.Models;

namespace Bottom_API._Repositories.Interfaces
{
    public interface ITransactionDetailRepo : IBottomRepository<WMSB_Transaction_Detail>
    {
        decimal? GetQtyByTransacNo(string transacNo);
        decimal? GetTransQtyByTransacNo(string transacNo);
        List<WMSB_Transaction_Detail> GetListTransDetailByTransacNo(string transacNo);
        Task AddRange(List<WMSB_Transaction_Detail> data);
        decimal? GetQtyOtherOutByTransacNo(string transacNo);
        decimal? GetQtyOtherInByTransacNo(string transacNo);
    }
}