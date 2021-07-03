using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bottom_API._Repositories.Interfaces;
using Bottom_API.Data;
using Bottom_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Bottom_API._Repositories.Repositories
{
    public class MaterialMissingRepository : BottomRepository<WMSB_Material_Missing>, IMaterialMissingRepository
    {
        private readonly DataContext _context;
        public MaterialMissingRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task AddRange(List<WMSB_Material_Missing> data)
        {
            await _context.WMSB_Material_Missing.AddRangeAsync(data);
        }

        public async Task<bool> CheckMissingNo(string missingNo)
        {
            var data = await _context.WMSB_Material_Missing.FirstOrDefaultAsync(x => x.Missing_No.Trim() == missingNo.Trim());
            return data != null ? true : false;
        }

        public string GetReasonKind(string missingNo)
        {
            var data = _context.WMSB_Material_Missing.FirstOrDefault(x => x.Missing_No == missingNo);
            if (data != null)
            {
                if(string.IsNullOrEmpty(data.Reason_Code)) {
                    return "";
                } else {
                    var result = "";
                    var reasonString = data.Reason_Code.Trim();
                    string[] reasonList = reasonString.Split(",");
                    foreach (var item in reasonList)
                    {
                        var reasonModel = _context.WMSB_Setting_Reason.FirstOrDefault(x => x.Reason_Code.Trim() == item.Trim());
                        if(reasonModel != null) {
                            result = result + reasonModel.Kind_Name.Trim() + "-" + reasonModel.HP_Reason_Code.Trim() + "-" + reasonModel.Reason_Ename.Trim() + "<br>";
                        }
                    }
                    return result;
                } 
            }
            else
            {
                return "";
            }
        }

        public decimal? SumMOQtyByMissingNo(string missingNo)
        {
            var data = _context.WMSB_Material_Missing.Where(x => x.Missing_No == missingNo).Sum(x => x.MO_Qty);
            return data;
        }
    }
}