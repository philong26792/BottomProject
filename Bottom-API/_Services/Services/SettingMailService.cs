
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Bottom_API._Repositories.Interfaces;
using Bottom_API._Services.Interfaces;
using Bottom_API.Helpers;
using Bottom_API.Models;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using LinqKit;
using Bottom_API.DTO.SettingMail;

namespace Bottom_API._Services.Services
{
    public class SettingMailService : ISettingMailService
    {
        private readonly IMaterialPurchaseRepository _iMaterialPurchaseRepo;
        private readonly ISettingSupplierRepository _iSettingMailSupplierReopo;
        private readonly IPackingListRepository _iPackingListRepository;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _configMapper;
        DateTime timeNow = DateTime.Now;
        public SettingMailService(IMaterialPurchaseRepository iMaterialPurchaseRepo, ISettingSupplierRepository iSettingMailSupplierReopo, IMapper mapper, MapperConfiguration configMapper, IPackingListRepository iPackingListRepository)
        {
            _iMaterialPurchaseRepo = iMaterialPurchaseRepo;
            _iSettingMailSupplierReopo = iSettingMailSupplierReopo;
            _mapper = mapper;
            _configMapper = configMapper;
            _iPackingListRepository = iPackingListRepository;
        }

        public async Task<bool> CreatSettingSupplier(Setting_Mail_Supplier_Dto model)
        {
            var item = await _iSettingMailSupplierReopo.FindAll(x => x.Factory == model.Factory && x.Supplier_No == model.Supplier_No && x.Subcon_ID == model.Subcon_ID).FirstOrDefaultAsync();
            if (item == null)
            {
                model.Updated_Time = timeNow;
                _iSettingMailSupplierReopo.Add(_mapper.Map<WMSB_Setting_Supplier>(model));
                try
                {
                    return await _iSettingMailSupplierReopo.SaveAll();
                }
                catch (System.Exception)
                {
                    return false;
                }
            }
            else
                return false;
        }

        public async Task<bool> DeleteSettingSupplier(Setting_Mail_Supplier_Dto model)
        {
            var item = _mapper.Map<WMSB_Setting_Supplier>(model);
            _iSettingMailSupplierReopo.Remove(item);
            try
            {
                return await _iSettingMailSupplierReopo.SaveAll();
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<bool> EditSettingSupplier(Setting_Mail_Supplier_Dto model)
        {
            model.Updated_Time = timeNow;
            var item = _mapper.Map<WMSB_Setting_Supplier>(model);
            try
            {
                _iSettingMailSupplierReopo.Update(item);
                return await _iSettingMailSupplierReopo.SaveAll();
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<PagedList<Setting_Mail_Supplier_Dto>> GetAllSettingMail(string supplierNo, string factory, PaginationParams paginationParams)
        {
            var pred_Setting_Mail = PredicateBuilder.New<WMSB_Setting_Supplier>(true);
            // pred_Setting_Mail.And(x => x.Supplier_No != "ZZZZ");
            if (!String.IsNullOrEmpty(supplierNo)) {
                pred_Setting_Mail.And(x => x.Supplier_No.Trim() == supplierNo.Trim());
            }
            if (!String.IsNullOrEmpty(factory)) {
                pred_Setting_Mail.And(x => x.Factory.Trim() == factory.Trim());
            }
            var data = _iSettingMailSupplierReopo.FindAll(pred_Setting_Mail).ProjectTo<Setting_Mail_Supplier_Dto>(_configMapper).OrderByDescending(x => x.Updated_Time);
            return await PagedList<Setting_Mail_Supplier_Dto>.CreateAsync(data, paginationParams.PageNumber, paginationParams.PageSize);
        }

        public async Task<object> GetAllSubcon() => await _iPackingListRepository.FindAll(x => x.Subcon_ID != "")
        .Select(x => new { x.Subcon_ID, x.Subcon_Name }).Distinct().ToListAsync();


        public async Task<object> GetAllSupplierNo() => await _iPackingListRepository.FindAll(x => x.T3_Supplier != "")
        .Select(x => new { x.T3_Supplier, x.T3_Supplier_Name }).Distinct().ToListAsync();

    }
}