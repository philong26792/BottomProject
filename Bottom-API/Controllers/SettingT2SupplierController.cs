using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Bottom_API._Services.Interfaces;
using Bottom_API.DTO;
using Bottom_API.DTO.SettingT2;
using Bottom_API.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Bottom_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SettingT2SupplierController : ControllerBase
    {
        private readonly ISettingT2SupplierService _settingT2SupplierService;
        public SettingT2SupplierController(ISettingT2SupplierService settingT2SupplierService)
        {
            _settingT2SupplierService = settingT2SupplierService;

        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] PaginationParams paginationParams, [FromQuery] SettingT2SupplierParam settingT2SupplierParam)
        {
            var result = await _settingT2SupplierService.GetAll(paginationParams, settingT2SupplierParam);
            Response.AddPagination(result.CurrentPage, result.PageSize, result.TotalCount, result.TotalPages);
            return Ok(result);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateT2Supplier(Setting_T2Delivery_Dto model)
        {
            var updateBy = User.FindFirst(ClaimTypes.Name).Value;

            if (await _settingT2SupplierService.AddT2(model, updateBy))
                return NoContent();
            else
                throw new Exception("Add T2 Supplier error ");
        }

        [HttpPut("edit")]
        public async Task<IActionResult> EditT2Supplier(Setting_T2Delivery_Dto model)
        {
            var updateBy = User.FindFirst(ClaimTypes.Name).Value;
            if (await _settingT2SupplierService.UpdateT2(model, updateBy))
                return NoContent();
            else
                throw new Exception("Edit T2 Supplier error");
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteT2Supplier(Setting_T2Delivery_Dto model)
        {
            if (await _settingT2SupplierService.DeleteT2(model))
                return NoContent();
            else
                throw new Exception("Delete T2 Supplier Error");
        }

    }
}