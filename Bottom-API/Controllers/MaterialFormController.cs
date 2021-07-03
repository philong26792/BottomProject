using System.Collections.Generic;
using System.Threading.Tasks;
using Bottom_API._Services.Interfaces;
using Bottom_API.DTO.Output;
using Microsoft.AspNetCore.Mvc;

namespace Bottom_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MaterialFormController : ControllerBase
    {
        private readonly IMaterialFormService _service;
        public MaterialFormController(IMaterialFormService service) {
            _service = service;
        }

        [HttpPost("findPrint")]
        public async Task<IActionResult> FindByQRCodeIDList([FromBody]List<QrCodeIDVersion> data) {
            var result = await _service.PrintByQRCodeIDList(data);
            return Ok(result);
        }
        
    }
}