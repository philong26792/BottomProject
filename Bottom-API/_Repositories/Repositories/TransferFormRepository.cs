using System.Linq;
using System.Threading.Tasks;
using Bottom_API._Repositories.Interfaces;
using Bottom_API.Data;
using Bottom_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Bottom_API._Repositories.Repositories
{
    public class TransferFormRepository : BottomRepository<WMSB_Transfer_Form>, ITransferFormRepository
    {
        private readonly DataContext _context;
        public TransferFormRepository(DataContext context) : base(context)
        {
            _context = context;
        }
        
        public async Task<bool> CheckCollectTransNo(string collectTransNo)
        {
            var transferFormModel = await _context.WMSB_Transfer_Form
                    .Where(x => x.Collect_Trans_No.Trim() == collectTransNo.Trim()).FirstOrDefaultAsync();
            if(transferFormModel != null) {
                return true;
            } else {
                return false;
            }
        }
    }
}