using System.Collections.Generic;
using System.Threading.Tasks;
using Bottom_API.DTO.GenareQrCode;
using Bottom_API.Helpers;
namespace Bottom_API._Services.Interfaces
{
    public interface IQRCodeMainService
    {
        Task<bool> AddListQRCode(List<string> listReceiveNo, string updateBy);
        Task<PagedList<QRCodeMainViewModel>> Search(PaginationParams param, FilterQrCodeParam filterParam);
        
    }
}