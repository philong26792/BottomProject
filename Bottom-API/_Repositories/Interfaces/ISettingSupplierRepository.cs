using Bottom_API.Models;

namespace Bottom_API._Repositories.Interfaces
{
    public interface ISettingSupplierRepository: IBottomRepository<WMSB_Setting_Supplier>
    {
        string GetEmailByT3Supplier(string t3Supplier, string subconId);
    }
}