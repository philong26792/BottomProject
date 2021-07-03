using System.Threading.Tasks;
using Bottom_API._Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Bottom_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HPUploadTimeLogController : ControllerBase
    {
        private readonly IHPUploadService _service;
        public HPUploadTimeLogController(IHPUploadService service) {
            _service = service;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetHPUpload() {
            var data = await _service.HPUpload();
            return Ok(data);
        }
    }
}