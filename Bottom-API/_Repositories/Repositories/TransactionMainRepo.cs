using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Bottom_API._Repositories.Interfaces;
using Bottom_API.Data;
using Bottom_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Bottom_API._Repositories.Repositories
{
    public class TransactionMainRepo : BottomRepository<WMSB_Transaction_Main>, ITransactionMainRepo
    {
        private readonly DataContext _context;
        public TransactionMainRepo(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task AddRange(List<WMSB_Transaction_Main> param)
        {
            await _context.WMSB_Transaction_Main.AddRangeAsync(param);
        }

        public async Task<bool> CheckRackLocation(object rackLocation)
        {
            var model = await _context.WMSB_Transaction_Main.FirstOrDefaultAsync(x => x.Rack_Location.Trim() == rackLocation.ToString().Trim());
            if(model != null) return true;
            return false;
        }

        public async Task<bool> CheckTransacNo(string transacNo)
        {
            var transactionModel = await _context.WMSB_Transaction_Main
                    .Where(x => x.Transac_No.Trim() == transacNo.Trim()).FirstOrDefaultAsync();
            return transactionModel != null ? true : false;
        }

        public async Task<bool> CheckTranSheetNo(string transheetNo)
        {
            var transModel =  await _context.WMSB_Transaction_Main.FirstOrDefaultAsync(x => x.Transac_Sheet_No.Trim() == transheetNo.Trim());
            return transModel != null ? true : false;
        }

        public async Task<WMSB_Transaction_Main> GetByInputNo(object inputNo)
        {
            return await _context.WMSB_Transaction_Main.FirstOrDefaultAsync(x => x.Transac_No.Trim() == inputNo.ToString().Trim());
        }
    }
}