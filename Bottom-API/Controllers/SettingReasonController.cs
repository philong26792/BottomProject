using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Bottom_API._Services.Interfaces;
using Bottom_API.DTO;
using Bottom_API.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Bottom_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SettingReasonController : ControllerBase
    {
        private readonly ISettingReasonService _reasonService;

        public SettingReasonController(ISettingReasonService reasonService)
        {
            _reasonService = reasonService;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationParams paginationParams
        , string reasonCode, string reasonName, string trans_toHP)
        {
            var result = await _reasonService.GetAll(paginationParams, reasonCode, reasonName, trans_toHP);
            Response.AddPagination(result.CurrentPage, result.PageSize, result.TotalCount, result.TotalPages);
            return Ok(result);
        }


        [HttpPost]
        public async Task<IActionResult> CreatReason(Setting_Reason_Dto model)
        {
            var updateBy = User.FindFirst(ClaimTypes.Name).Value;
            model.Updated_By = updateBy;
            if (await _reasonService.AddReason(model))
                return NoContent();
            else
                throw new Exception("Add setting mail error ");
        }

        [HttpPost("edit")]
        public async Task<IActionResult> EditReason(Setting_Reason_Dto model)
        {
            var updateBy = User.FindFirst(ClaimTypes.Name).Value;
            model.Updated_By = updateBy;
            if (await _reasonService.UpdateReason(model))
                return NoContent();
            else
                throw new Exception("Add setting mail error ");
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteReason(Setting_Reason_Dto model)
        {
            if (await _reasonService.DeleteReason(model))
                return NoContent();
            else
                throw new Exception("Add setting mail error ");
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetAll()
        {
            var data = await _reasonService.GetAll();
            return Ok(data);
        }
        [HttpGet("GetReasonCode")]
        public async Task<IActionResult> GetReasonCode()
        {
            var data = await _reasonService.GetReasonCode();
            return Ok(data);
        }
    }
}