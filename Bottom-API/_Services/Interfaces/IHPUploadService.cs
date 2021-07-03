using System.Threading.Tasks;
using Bottom_API.Models;

namespace Bottom_API._Services.Interfaces
{
    public interface IHPUploadService
    {
        Task<object> HPUpload();
    }
}