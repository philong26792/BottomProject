
using System.Linq;
using Bottom_API._Repositories.Interfaces;
using Bottom_API.Data;
using Bottom_API.Models;

namespace Bottom_API._Repositories.Repositories
{
    public class PackingListDetailRepository : BottomRepository<WMSB_PackingList_Detail>, IPackingListDetailRepository
    {
        private readonly DataContext _context;
        public PackingListDetailRepository(DataContext context) : base(context) {
            _context = context;
        }

        public decimal? SumPurchaseQtyByReceiveNo(string receiveNo)
        {
            return _context.WMSB_PackingList_Detail.Where(x => x.Receive_No.Trim() == receiveNo.Trim()).Sum(x => x.Purchase_Qty);
        }

        public decimal? SumMOQtyByReceiveNo(string receiveNo)
        {
            return _context.WMSB_PackingList_Detail.Where(x => x.Receive_No.Trim() == receiveNo.Trim()).Sum(x => x.MO_Qty);
        }
    }
}