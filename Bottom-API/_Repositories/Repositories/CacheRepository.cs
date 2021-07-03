using System.Linq;
using Bottom_API._Repositories.Interfaces;
using Bottom_API.Data;
using Bottom_API.Models;

namespace Bottom_API._Repositories.Repositories
{
    public class CacheRepository : BottomRepository<WMSB_Cache>, ICacheRepository
    {
        private readonly DataContext _context;
        public CacheRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public string GetCustmoerName(string moNo, string moSeq, string materialId, string purchaseNo)
        {
            var data = _context.WMSB_Cache.Where(x => x.MO_No.Trim() == moNo.Trim()
                            && x.MO_Seq.Trim() == moSeq.Trim()
                            && x.Material_ID.Trim() == materialId.Trim()).FirstOrDefault();
            if (data != null)
            {
                return data.Part_Name;
            }
            else
            {
                return "";
            }
        }

        public string GetCustmoerPart(string moNo, string moSeq, string materialId, string purchaseNo)
        {
            var data = _context.WMSB_Cache.Where(x => x.MO_No.Trim() == moNo.Trim()
                            && x.MO_Seq.Trim() == moSeq.Trim()
                            && x.Material_ID.Trim() == materialId.Trim()).FirstOrDefault();
            if (data != null)
            {
                return data.Part_No;
            }
            else
            {
                return "";
            }
        }

        public string GetLineASY(string moNo, string moSeq, string materialId, string purchaseNo)
        {
            var data = _context.WMSB_Cache.Where(x => x.MO_No.Trim() == moNo.Trim()
                            && x.MO_Seq.Trim() == moSeq.Trim()
                            && x.Material_ID.Trim() == materialId.Trim()).FirstOrDefault();
            if (data != null)
            {
                return data.Line_ID;
            }
            else
            {
                return "";
            }
        }
    }
}