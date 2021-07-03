using Bottom_API.Models;

namespace Bottom_API._Repositories.Interfaces
{
    public interface ICacheRepository : IBottomRepository<WMSB_Cache>
    {
        string GetCustmoerPart(string moNo, string moSeq, string materialId, string purchaseNo);
        string GetCustmoerName(string moNo, string moSeq, string materialId, string purchaseNo);
        string GetLineASY(string moNo, string moSeq, string materialId, string purchaseNo);
    }
}