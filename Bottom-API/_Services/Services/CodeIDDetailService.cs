using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Bottom_API._Repositories.Interfaces;
using Bottom_API._Services.Interfaces;
using Bottom_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Bottom_API._Services.Services
{
    public class CodeIDDetailService : ICodeIDDetailService
    {
        private readonly ICodeIDDetailRepo _repoCodeIDDetail;
        public CodeIDDetailService(ICodeIDDetailRepo repoCodeIDDetail, IMapper mapper)
        {
            _repoCodeIDDetail = repoCodeIDDetail;
        }

        public async Task<List<WMS_Code>> GetArea()
        {
            return await _repoCodeIDDetail.FindAll(x => x.Code_Type == 5).ToListAsync();
        }

        public async Task<List<WMS_Code>> GetBuilding()
        {
            return await _repoCodeIDDetail.FindAll(x => x.Code_Type == 3).ToListAsync();
        }

        public async Task<List<WMS_Code>> GetFactory()
        {
            return await _repoCodeIDDetail.FindAll(x => x.Code_Type == 1).ToListAsync();
        }

        public async Task<List<WMS_Code>> GetFloor()
        {
            return await _repoCodeIDDetail.FindAll(x => x.Code_Type == 4).ToListAsync();
        }

        public async Task<List<WMS_Code>> GetWH()
        {
            return await _repoCodeIDDetail.FindAll(x => x.Code_Type == 2).ToListAsync();
        }

        public async Task<List<WMS_Code>> GetKanBanByCategory()
        {
            return await _repoCodeIDDetail.FindAll(x => x.Code_Type == 6).OrderBy(x => x.Code_ID).ToListAsync();
        }

        public string GetCodeName(string codeId)
        {
            var code = _repoCodeIDDetail.FindSingle(x => x.Code_ID.Trim() == codeId.Trim());
            if (code != null)
            {
                return code.Code_Ename;
            }
            else
            {
                return "";
            }
        }
    }

}