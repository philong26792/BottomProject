using System.Linq;
using Bottom_API._Repositories.Interfaces;
using Bottom_API.Data;
using Bottom_API.Models;

namespace Bottom_API._Repositories.Repositories
{
    public class SettingSupplierRepository : BottomRepository<WMSB_Setting_Supplier>, ISettingSupplierRepository
    {
        private readonly DataContext _context;
        public SettingSupplierRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public string GetEmailByT3Supplier(string t3Supplier, string subconId)
        {
            t3Supplier = t3Supplier == "" ? "ZZZZ" : t3Supplier;
            if (t3Supplier != "0000")
            {
                var data = _context.WMSB_Setting_Supplier.Where(x => x.Supplier_No.Trim() == t3Supplier.Trim()).FirstOrDefault();
                if (data != null)
                {
                    return data.Email;
                }
                else
                {
                    return null;
                }
            }
            else 
            {
                var data = _context.WMSB_Setting_Supplier.Where(x => x.Supplier_No.Trim() == t3Supplier.Trim() && x.Subcon_ID.Trim() == subconId.Trim()).FirstOrDefault();
                if (data != null)
                {
                    return data.Email;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}