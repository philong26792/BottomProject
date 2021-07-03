using System.Collections.Generic;
using System.Threading.Tasks;
using Bottom_API._Repositories.Interfaces;
using Bottom_API.Data;
using Bottom_API.Models;

namespace Bottom_API._Repositories.Repositories
{
    public class ReasonDetailRepository : BottomRepository<WMSB_Reason_Detail>, IReasonDetailRepository
    {
        private readonly DataContext _context;
        public ReasonDetailRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task AddRange(List<WMSB_Reason_Detail> data)
        {
            await _context.WMSB_Reason_Detail.AddRangeAsync(data);
        }
    }
}