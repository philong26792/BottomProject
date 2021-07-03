using Bottom_API.Models;

namespace Bottom_API._Repositories.Interfaces
{
    public interface IPackingListDetailRepository : IBottomRepository<WMSB_PackingList_Detail>
    {

        decimal? SumPurchaseQtyByReceiveNo(string receiveNo);
        decimal? SumMOQtyByReceiveNo(string receiveNo);
    }
}