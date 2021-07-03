using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Bottom_API._Repositories.Interfaces;
using Bottom_API._Services.Interfaces;
using Bottom_API.DTO.SettingT2;
using Bottom_API.Helpers;
using Bottom_API.Models;
using LinqKit;
using Microsoft.EntityFrameworkCore;

namespace Bottom_API._Services.Services
{
    public class SettingT2SupplierService : ISettingT2SupplierService
    {
        private readonly ISettingT2SupplierRepository _settingT2SupplierRepository;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _configMapper;
        private readonly IMaterialPurchaseRepository _materialPurchaseRepository;
        private readonly ICacheRepository _cacheRepository;
        private readonly ISettingReasonRepository _settingReasonRepository;
        DateTime timeNow = DateTime.Now;
        public SettingT2SupplierService(
                ISettingT2SupplierRepository settingT2SupplierRepository,
                IMapper mapper,
                MapperConfiguration configMapper,
                IMaterialPurchaseRepository materialPurchaseRepository,
                ICacheRepository cacheRepository,
                ISettingReasonRepository settingReasonRepository)
        {
            _configMapper = configMapper;
            _materialPurchaseRepository = materialPurchaseRepository;
            _cacheRepository = cacheRepository;
            _settingReasonRepository = settingReasonRepository;
            _mapper = mapper;
            _settingT2SupplierRepository = settingT2SupplierRepository;

        }
        public async Task<bool> AddT2(Setting_T2Delivery_Dto model, string updateBy)
        {
            foreach (var i in model.Reasons)
            {
                if (_settingT2SupplierRepository.FindSingle(x => x.T2_Supplier_ID == model.T2_Supplier_ID && x.Reason_Code == i.Reason_Code) != null)
                    return false;
            }
            try
            {
                foreach (var item in model.Reasons)
                {
                    var data = new WMSB_Setting_T2Delivery()
                    {
                        Factory_ID = model.Factory_ID,
                        T2_Supplier_ID = model.T2_Supplier_ID,
                        T2_Supplier_Name = model.T2_Supplier_Name,
                        Input_Delivery = model.Input_Delivery,
                        Reason_Code = item.Reason_Code,
                        Reason_Name = item.Reason_Name,
                        Is_Valid = model.Is_Valid,
                        Invalid_Date = null,
                        Updated_By = updateBy,
                        Updated_Time = timeNow
                    };
                    _settingT2SupplierRepository.Add(data);
                }
                return await _settingT2SupplierRepository.SaveAll();
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteT2(Setting_T2Delivery_Dto model)
        {
            var dataToDel = _settingT2SupplierRepository.FindAll(x => x.T2_Supplier_ID == model.T2_Supplier_ID).ToList();
            _settingT2SupplierRepository.RemoveMultiple(dataToDel);
            try
            {
                return await _settingT2SupplierRepository.SaveAll();
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<PagedList<Setting_T2Delivery_Dto>> GetAll(PaginationParams paginationParams, SettingT2SupplierParam settingT2SupplierParam)
        {
            var pred = PredicateBuilder.New<WMSB_Setting_T2Delivery>(true);
            if (!string.IsNullOrEmpty(settingT2SupplierParam.factory_id))
            {
                pred.And(x => x.Factory_ID.Trim() == settingT2SupplierParam.factory_id.Trim());
            }
            if (!string.IsNullOrEmpty(settingT2SupplierParam.reason_code))
            {
                pred.And(x => x.Reason_Code.Trim() == settingT2SupplierParam.reason_code.Trim());
            }
            if (!string.IsNullOrEmpty(settingT2SupplierParam.supplier_id))
            {
                pred.And(x => x.T2_Supplier_ID.Trim() == settingT2SupplierParam.supplier_id.Trim());
            }
            if (!string.IsNullOrEmpty(settingT2SupplierParam.input_delivery))
            {
                pred.And(x => x.Input_Delivery.Trim() == settingT2SupplierParam.input_delivery.Trim());
            }
            var dataAll = await _settingT2SupplierRepository.FindAll(pred).OrderByDescending(x => x.Updated_Time).ToListAsync();
            var data = dataAll.GroupBy(x => x.T2_Supplier_ID).Select(x => new Setting_T2Delivery_Dto
            {
                T2_Supplier_ID = x.FirstOrDefault().T2_Supplier_ID,
                T2_Supplier_Name = x.FirstOrDefault().T2_Supplier_Name,
                Input_Delivery = x.FirstOrDefault().Input_Delivery,
                Is_Valid = x.FirstOrDefault().Is_Valid
            }).ToList();
            data.ForEach(item =>
            {
                item.Reasons = dataAll.Where(x => x.T2_Supplier_ID.Trim() == item.T2_Supplier_ID.Trim())
                                .Select(x => new ReasonCodeInfo()
                                {
                                    Reason_Code = x.Reason_Code,
                                    Reason_Name = x.Reason_Name
                                }).ToList();
            });
            return PagedList<Setting_T2Delivery_Dto>.Create(data, paginationParams.PageNumber, paginationParams.PageSize);
        }
        public async Task<bool> UpdateT2(Setting_T2Delivery_Dto model, string updateBy)
        {
            try
            {
                var dataToDel = _settingT2SupplierRepository.FindAll(x => x.T2_Supplier_ID == model.T2_Supplier_ID).ToList();
                _settingT2SupplierRepository.RemoveMultiple(dataToDel);

                List<WMSB_Setting_T2Delivery> list = new List<WMSB_Setting_T2Delivery>();
                foreach (var item in model.Reasons)
                {
                    var data = new WMSB_Setting_T2Delivery()
                    {
                        Factory_ID = model.Factory_ID,
                        T2_Supplier_ID = model.T2_Supplier_ID,
                        T2_Supplier_Name = model.T2_Supplier_Name,
                        Input_Delivery = model.Input_Delivery,
                        Reason_Code = item.Reason_Code,
                        Reason_Name = item.Reason_Name,
                        Is_Valid = model.Is_Valid,
                        Invalid_Date = null,
                        Updated_By = updateBy,
                        Updated_Time = timeNow
                    };
                    list.Add(data);
                }
                _settingT2SupplierRepository.UpdateRange(list);
                return await _settingT2SupplierRepository.SaveAll();
            }
            catch (System.Exception)
            {
                return false;
            }
        }


    }
}