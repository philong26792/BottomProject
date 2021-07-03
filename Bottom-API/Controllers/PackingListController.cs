
using System.Threading.Tasks;
using Bottom_API._Services.Interfaces;
using Bottom_API.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Bottom_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PackingListController : ControllerBase
    {
        private readonly IPackingListService _service;
        public PackingListController(IPackingListService service)
        {
            _service = service;
        }


        [HttpPost("search")]
        public async Task<IActionResult> Search([FromQuery] PaginationParams param, FilterPackingListParam filterParam)
        {
            var lists = await _service.Search(param, filterParam);
            Response.AddPagination(lists.CurrentPage, lists.PageSize, lists.TotalCount, lists.TotalPages);
            return Ok(lists);
        }
        
        [HttpGet("findBySupplier/{supplier_ID}")]
        public async Task<IActionResult> FindBySupplier(string supplier_ID)
        {
            var data = await _service.FindBySupplier(supplier_ID);
            return Ok(data);
        }


        [HttpGet("supplierList")]
        public async Task<IActionResult> SupplierList()
        {
            var data = await _service.SupplierList();
            return Ok(data);
        }

    }
}