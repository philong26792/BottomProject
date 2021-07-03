using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Bottom_API._Repositories.Interfaces;
using Bottom_API._Services.Interfaces;
using Bottom_API.DTO;
using Bottom_API.Helpers;
using Bottom_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Bottom_API._Services.Services
{
    public class SettingReasonService : ISettingReasonService
    {
        private readonly ISettingReasonRepository _reasonRepo;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _configMapper;
        DateTime timeNow = DateTime.Now;

        public SettingReasonService(ISettingReasonRepository reasonRepo, IMapper mapper, MapperConfiguration configMapper)
        {
            _reasonRepo = reasonRepo;
            _mapper = mapper;
            _configMapper = configMapper;
        }

        public async Task<bool> AddReason(Setting_Reason_Dto model)
        {
            var item = _reasonRepo.FindSingle(x => x.Reason_Code == model.Reason_Code);
            if (item == null)
            {
                model.Updated_Time = timeNow;
                _reasonRepo.Add(_mapper.Map<WMSB_Setting_Reason>(model));
                try
                {
                    return await _reasonRepo.SaveAll();
                }
                catch (System.Exception)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }

        public async Task<bool> DeleteReason(Setting_Reason_Dto model)
        {
            var item = _mapper.Map<WMSB_Setting_Reason>(model);
            _reasonRepo.Remove(item);
            try
            {
                return await _reasonRepo.SaveAll();
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<PagedList<Setting_Reason_Dto>> GetAll(PaginationParams paginationParams,
        string reasonCode,string reasonName, string  trans_toHP)
        {

            var data = _reasonRepo.FindAll().AsNoTracking().ProjectTo<Setting_Reason_Dto>(_configMapper);
        if (!String.IsNullOrEmpty(reasonCode))
        {
              data = data.Where(x=>x.Reason_Code.Trim() ==reasonCode.Trim());
        }
         if (!String.IsNullOrEmpty(reasonName))
        {
              data = data.Where(x=>x.Reason_Ename.ToLower().Contains(reasonName.ToLower()) 
              || x.Reason_Cname.ToLower().Contains(reasonName.ToLower()) ||
              x.Reason_Lname.ToLower().Contains(reasonName.ToLower()));
        }
         if (!String.IsNullOrEmpty(trans_toHP))
        {
              data = data.Where(x=>x.Trans_toHP == trans_toHP.Trim() || (trans_toHP =="N" && x.Trans_toHP ==null) );
        }
            data = data.OrderByDescending(x => x.Updated_Time);
            return await PagedList<Setting_Reason_Dto>.CreateAsync(data, paginationParams.PageNumber, paginationParams.PageSize);
        }

        public async Task<List<WMSB_Setting_Reason>> GetAll()
        {
            return await _reasonRepo.FindAll().ToListAsync();
        }

        public async Task<bool> UpdateReason(Setting_Reason_Dto model)
        {
            model.Updated_Time = timeNow;
            var item = _mapper.Map<WMSB_Setting_Reason>(model);
            try
            {
                _reasonRepo.Update(item);
                return await _reasonRepo.SaveAll();
            }
            catch (System.Exception)
            {
                return false;
            }
        }
        public async Task<object> GetReasonCode(){
            var data = await _reasonRepo.FindAll().GroupBy(x=>x.Reason_Code).Select(x=>x.Key).ToListAsync();
            return data;
        }
    }
}