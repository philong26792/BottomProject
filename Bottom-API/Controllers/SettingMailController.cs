using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Bottom_API._Services.Interfaces;
using Bottom_API.DTO;
using Bottom_API.DTO.SettingMail;
using Bottom_API.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Bottom_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SettingMailController : ControllerBase
    {
        private readonly ISettingMailService _settingMailService;

        public SettingMailController(ISettingMailService settingMailService)
        {
            _settingMailService = settingMailService;

        }

        [HttpGet]
        public async Task<IActionResult> GetAllSupplierNo() => Ok(await _settingMailService.GetAllSupplierNo());

        [HttpGet("getallsubcon")]
        public async Task<IActionResult> GetAllSubcon() => Ok(await _settingMailService.GetAllSubcon());


        [HttpPost("create")]
        public async Task<IActionResult> CreatAndUpdateSettingMail([FromBody] Setting_Mail_Supplier_Dto model)
        {
            var updateBy = User.FindFirst(ClaimTypes.Name).Value;
            model.Updated_By = updateBy;
            if (await _settingMailService.CreatSettingSupplier(model))
            {
                return NoContent();
            }
            throw new Exception("Add setting mail error ");
        }

        [HttpPost("edit")]
        public async Task<IActionResult> UpdateSettingMail([FromBody] Setting_Mail_Supplier_Dto model)
        {
            var updateBy = User.FindFirst(ClaimTypes.Name).Value;
            model.Updated_By = updateBy;
            if (await _settingMailService.EditSettingSupplier(model))
            {
                return NoContent();
            }
            throw new Exception("Edit setting mail error ");
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteSettingMail([FromBody] Setting_Mail_Supplier_Dto model)
        {
            if (await _settingMailService.DeleteSettingSupplier(model))
            {
                return NoContent();
            }
            throw new Exception("Delete setting mail error ");
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] PaginationParams paginationParams, string factory, string supplierNo)
        {
            var result = await _settingMailService.GetAllSettingMail(supplierNo, factory, paginationParams);
            Response.AddPagination(result.CurrentPage, result.PageSize, result.TotalCount, result.TotalPages);
            return Ok(result);
        }
    }
}