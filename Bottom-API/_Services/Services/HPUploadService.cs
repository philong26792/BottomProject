
using System.Linq;
using System.Threading.Tasks;
using Bottom_API._Repositories.Interfaces;
using Bottom_API._Repositories.Interfaces.DbHpBasic;
using Bottom_API._Services.Interfaces;
using Bottom_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Bottom_API._Services.Services
{
    public class HPUploadService : IHPUploadService
    {
        private readonly IHPUploadTimeLogRepository _repo;
        private readonly ICacheRepository _repoCache;

        public HPUploadService(IHPUploadTimeLogRepository repo,
                              ICacheRepository repoCache) {
                _repo = repo;
                _repoCache = repoCache;
        }

        public async Task<object> HPUpload()
        {
            var data_HP = await _repo.FindAll().OrderByDescending(x => x.Upload_Time).FirstOrDefaultAsync();
            var data_Cache = await _repoCache.FindAll().OrderByDescending(x => x.Upload_Time).FirstOrDefaultAsync();
            var color = "";
            if(data_Cache.Version != data_HP.Version ||data_Cache.Upload_Time !=data_HP.Upload_Time)
            {
                color = "red";
            }
            return new{
                data =data_Cache,
                color = color
            };
        }
    }
}