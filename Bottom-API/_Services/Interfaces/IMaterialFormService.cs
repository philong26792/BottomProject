using System.Collections.Generic;
using System.Threading.Tasks;
using Bottom_API.DTO.Output;

namespace Bottom_API._Services.Interfaces
{
    public interface IMaterialFormService
    {
        Task<object> FindByQrCodeID(QrCodeIDVersion data);
        Task<List<object>> PrintByQRCodeIDList(List<QrCodeIDVersion> data);
    }
}