using Bottom_API.Models;

namespace Bottom_API._Repositories.Interfaces
{
    public interface IMaterialViewRepository : IBottomRepository<VM_WMSB_Material_Purchase>
    {
        string GetCustmoerPart(string moNo, string moSeq, string materialId, string purchaseNo);
        string GetCustmoerName(string moNo, string moSeq, string materialId, string purchaseNo);
        string GetLineSTF(string moNo, string moSeq, string materialId, string purchaseNo);
        string GetLineASY(string moNo, string moSeq, string materialId, string purchaseNo);
    }
}