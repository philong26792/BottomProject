using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Bottom_API._Services.Interfaces;
using Bottom_API.DTO.Receiving;
using Bottom_API.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Bottom_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReceivingMaterialController : ControllerBase
    {
        private readonly IRecevingEnoughService _service;
        public ReceivingMaterialController(IRecevingEnoughService service)
        {
            _service = service;
        }

        [HttpPost("search")]
        public async Task<IActionResult> Search([FromQuery] PaginationParams param, FilterReceivingMateParam filterParam)
        {
            var result = await _service.SearchByModel(param, filterParam);
            Response.AddPagination(result.CurrentPage, result.PageSize, result.TotalCount, result.TotalPages);
            return Ok(result);
        }

        [HttpPost("receiving")]
        public async Task<IActionResult> ReceivingMaterial([FromBody] List<ReceivingMaterialMainModel> data)
        {
            var updateBy = User.FindFirst(ClaimTypes.Name).Value;
            var result = await _service.SubmitData(data, updateBy);
            return Ok(result);
        }
    }
}