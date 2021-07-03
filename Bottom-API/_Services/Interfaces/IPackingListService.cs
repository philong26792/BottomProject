using System.Collections.Generic;
using System.Threading.Tasks;
using Bottom_API.DTO;
using Bottom_API.DTO.Input;
using Bottom_API.Helpers;
using Bottom_API.Models;
namespace Bottom_API._Services.Interfaces
{
    public interface IPackingListService
    {
        Task<PagedList<WMSB_Packing_List>> Search(PaginationParams param,FilterPackingListParam filterParam);
        Task<Packing_List_Dto> FindBySupplier(string supplier_ID);
        Task<List<SupplierModel>> SupplierList();
        
    }
}