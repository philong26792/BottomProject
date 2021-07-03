using System.Linq;
using Bottom_API._Repositories.Interfaces;
using Bottom_API.Data;
using Bottom_API.Models;

namespace Bottom_API._Repositories.Repositories
{
    public class MaterialViewRepository : BottomRepository<VM_WMSB_Material_Purchase>, IMaterialViewRepository
    {
        private readonly DataContext _context;
        public MaterialViewRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public string GetCustmoerName(string moNo, string moSeq, string materialId, string purchaseNo)
        {
            var data = _context.VM_WMSB_Material_Purchase.Where(x => x.Plan_No.Trim() == moNo.Trim()
                            && x.MO_Seq.Trim() == moSeq.Trim()
                            && x.Mat_.Trim() == materialId.Trim()
                            && x.Purchase_No.Trim() == purchaseNo.Trim()).FirstOrDefault();
            if (data != null)
            {
                return data.Custmoer_Name;
            }
            else
            {
                return "";
            }
        }

        public string GetCustmoerPart(string moNo, string moSeq, string materialId, string purchaseNo)
        {
            var data = _context.VM_WMSB_Material_Purchase.Where(x => x.Plan_No.Trim() == moNo.Trim()
                            && x.MO_Seq.Trim() == moSeq.Trim()
                            && x.Mat_.Trim() == materialId.Trim()
                            && x.Purchase_No.Trim() == purchaseNo.Trim()).FirstOrDefault();
            if (data != null)
            {
                return data.Custmoer_Part;
            }
            else
            {
                return "";
            }
        }

        public string GetLineASY(string moNo, string moSeq, string materialId, string purchaseNo)
        {
            var data = _context.VM_WMSB_Material_Purchase.Where(x => x.Plan_No.Trim() == moNo.Trim()
                            && x.MO_Seq.Trim() == moSeq.Trim()
                            && x.Mat_.Trim() == materialId.Trim()
                            && x.Purchase_No.Trim() == purchaseNo.Trim()).FirstOrDefault();
            if (data != null)
            {
                return data.Line_ASY;
            }
            else
            {
                return "";
            }
        }

        public string GetLineSTF(string moNo, string moSeq, string materialId, string purchaseNo)
        {
            var data = _context.VM_WMSB_Material_Purchase.Where(x => x.Plan_No.Trim() == moNo.Trim()
                            && x.MO_Seq.Trim() == moSeq.Trim()
                            && x.Mat_.Trim() == materialId.Trim()
                            && x.Purchase_No.Trim() == purchaseNo.Trim()).FirstOrDefault();
            if (data != null)
            {
                return data.Line_STF;
            }
            else
            {
                return "";
            }
        }
    }
}