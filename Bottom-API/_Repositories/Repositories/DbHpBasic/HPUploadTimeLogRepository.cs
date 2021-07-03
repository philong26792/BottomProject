using Bottom_API._Repositories.Interfaces.DbHpBasic;
using Bottom_API.Data;
using Bottom_API.Models;

namespace Bottom_API._Repositories.Repositories.DbHpBasic
{
    public class HPUploadTimeLogRepository : HPRepository<HP_Upload_Time_ie27_1_log>, IHPUploadTimeLogRepository
    {
        private readonly HPDataContext _context;
        public HPUploadTimeLogRepository(HPDataContext context) : base(context) {
            _context = context;
        }
    }
}