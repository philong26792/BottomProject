using System.Collections.Generic;
using System.Threading.Tasks;
using Bottom_API.Models;

namespace Bottom_API._Repositories.Interfaces
{
    public interface IMaterialMissingRepository : IBottomRepository<WMSB_Material_Missing>
    {
        decimal? SumMOQtyByMissingNo(string missingNo);
        string GetReasonKind(string missingNo);
        Task AddRange (List<WMSB_Material_Missing> data);
        Task<bool> CheckMissingNo(string missingNo);
    }
}