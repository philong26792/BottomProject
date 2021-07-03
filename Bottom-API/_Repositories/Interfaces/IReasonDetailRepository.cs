using System.Collections.Generic;
using System.Threading.Tasks;
using Bottom_API.Models;

namespace Bottom_API._Repositories.Interfaces
{
    public interface IReasonDetailRepository : IBottomRepository<WMSB_Reason_Detail> {
        Task AddRange (List<WMSB_Reason_Detail> data);
    }
}