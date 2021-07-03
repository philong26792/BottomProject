using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Bottom_API._Services.Interfaces;
using Bottom_API.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Bottom_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QRCodeMainController : ControllerBase
    {
        private readonly IQRCodeMainService _service;
        public QRCodeMainController(IQRCodeMainService service) {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> AddListQRCode([FromBody]List<string> listData) {
            var updateBy = User.FindFirst(ClaimTypes.Name).Value;
            var result = await _service.AddListQRCode(listData, updateBy);
            return Ok(result);
        }

        [HttpPost("search")]
        public async Task<IActionResult> Search([FromQuery]PaginationParams param, FilterQrCodeParam filterParam) {
            var lists = await _service.Search(param,filterParam);
            Response.AddPagination(lists.CurrentPage, lists.PageSize, lists.TotalCount, lists.TotalPages);
            return Ok(lists);
        }
    }
}