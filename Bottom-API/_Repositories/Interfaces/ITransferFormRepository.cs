using System.Threading.Tasks;
using Bottom_API.Models;

namespace Bottom_API._Repositories.Interfaces
{
    public interface ITransferFormRepository : IBottomRepository<WMSB_Transfer_Form>
    {
        Task<bool> CheckCollectTransNo(string collectTransNo);
    }
}