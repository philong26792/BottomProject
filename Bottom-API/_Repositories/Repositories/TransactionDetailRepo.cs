using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bottom_API._Repositories.Interfaces;
using Bottom_API.Data;
using Bottom_API.Models;

namespace Bottom_API._Repositories.Repositories
{
    public class TransactionDetailRepo : BottomRepository<WMSB_Transaction_Detail>, ITransactionDetailRepo
    {
        private readonly DataContext _context;
        public TransactionDetailRepo(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task AddRange(List<WMSB_Transaction_Detail> data)
        {
            await _context.WMSB_Transaction_Detail.AddRangeAsync(data);
        }

        public List<WMSB_Transaction_Detail> GetListTransDetailByTransacNo(string transacNo)
        {
            var lists = _context.WMSB_Transaction_Detail.Where(x => x.Transac_No.Trim() == transacNo).ToList();
            return lists;
        }

        public decimal? GetQtyByTransacNo(string transacNo)
        {
            var data = _context.WMSB_Transaction_Detail.Where(x => x.Transac_No.Trim() == transacNo).Sum(x => x.Instock_Qty);
            return data;
        }

        public decimal? GetTransQtyByTransacNo(string transacNo)
        {
            var data = _context.WMSB_Transaction_Detail.Where(x => x.Transac_No.Trim() == transacNo).Sum(x => x.Trans_Qty);
            return data;
        }

        public decimal? GetUntransacQtyByTransacNo(string transacNo)
        {
            var data = _context.WMSB_Transaction_Detail.Where(x => x.Transac_No.Trim() == transacNo).Sum(x => x.Untransac_Qty);
            return data;
        }

        public decimal? GetQtyOtherOutByTransacNo(string transacNo)
        {
            var data = _context.WMSB_Transaction_Detail.Where(x => x.Transac_No.Trim() == transacNo).Sum(x => x.Instock_Qty);

            var transaactionMainOld = _context.WMSB_Transaction_Main.Where(x => x.Transac_No.Trim() == transacNo).FirstOrDefault();
            var transaactionMainNew = _context.WMSB_Transaction_Main.Where(x => x.QRCode_ID.Trim() == transaactionMainOld.QRCode_ID.Trim()
                                        && x.QRCode_Version == transaactionMainOld.QRCode_Version + 1).Select(x => x.Transac_No).FirstOrDefault();
            var data2 =  _context.WMSB_Transaction_Detail.Where(x => x.Transac_No.Trim() == transaactionMainNew).Sum(x => x.Instock_Qty);

            return data - data2;
        }
        public decimal? GetQtyOtherInByTransacNo(string transacNo)
        {
            var data = _context.WMSB_Transaction_Detail.Where(x => x.Transac_No.Trim() == transacNo).Sum(x => x.Instock_Qty);

            var transaactionMainOld = _context.WMSB_Transaction_Main.Where(x => x.Transac_No.Trim() == transacNo).FirstOrDefault();
            var transaactionMainNew = _context.WMSB_Transaction_Main.Where(x => x.QRCode_ID.Trim() == transaactionMainOld.QRCode_ID.Trim()
                                        && x.QRCode_Version == transaactionMainOld.QRCode_Version + 1).Select(x => x.Transac_No).FirstOrDefault();
            var data2 =  _context.WMSB_Transaction_Detail.Where(x => x.Transac_No.Trim() == transaactionMainNew).Sum(x => x.Instock_Qty);

            return data2 - data;
        }
    }
}