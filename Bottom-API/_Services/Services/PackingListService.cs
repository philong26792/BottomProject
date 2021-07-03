using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Bottom_API._Repositories.Interfaces;
using Bottom_API._Repositories.Interfaces.DbHpBasic;
using Bottom_API._Services.Interfaces;
using Bottom_API.DTO;
using Bottom_API.DTO.Input;
using Bottom_API.Helpers;
using Bottom_API.Models;
using LinqKit;
using Microsoft.EntityFrameworkCore;

namespace Bottom_API._Services.Services
{
    public class PackingListService : IPackingListService
    {
        private readonly IPackingListRepository _repoPackingList;
        private readonly IHPVendorRepository _repoHPVendor;
        private readonly IMaterialPurchaseRepository _repoMaterialPurchase;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _configMapper;
        public PackingListService(  IPackingListRepository repoPackingList,
                                    IHPVendorRepository repoHPVendor,
                                    IMaterialPurchaseRepository repoMaterialPurchase,
                                    IMapper mapper,
                                    MapperConfiguration configMapper) {
            _repoPackingList = repoPackingList;
            _repoHPVendor = repoHPVendor;
            _repoMaterialPurchase = repoMaterialPurchase;
            _mapper = mapper;
            _configMapper = configMapper;
        }
        public async Task<List<SupplierModel>> SupplierList()
        {
            var listMaterialPurchase = await _repoMaterialPurchase.FindAll(x => x.Supplier_ID.Trim() != "")
                                                .Select(x => new
                                                {
                                                    Supplier_ID = x.Supplier_ID
                                                }).Distinct()
                                                .ToListAsync();

            var listHPVendor = await _repoHPVendor.FindAll(x => x.Vendor_No.Trim() != "")
                                        .Select(x => new 
                                        {
                                            Vendor_No = x.Vendor_No,
                                            Vendor_Name = x.Vendor_Name
                                        }).Distinct()
                                        .ToListAsync();

            var data = (from T1 in listMaterialPurchase
                        join T2 in listHPVendor
            on T1.Supplier_ID.Trim() equals T2.Vendor_No.Trim()
                        select new SupplierModel
                        {
                            Supplier_No = T1.Supplier_ID.Trim(),
                            Supplier_Name = T2.Vendor_Name.Trim()
                        }).Distinct()
                        .OrderBy(x => x.Supplier_No)
                        .ToList();

            return data;
        }
        public async Task<PagedList<WMSB_Packing_List>> Search(PaginationParams param,FilterPackingListParam filterParam)
        {
            var pred_Packing_List = PredicateBuilder.New<WMSB_Packing_List>(true);
            pred_Packing_List.And(x => x.Generated_QRCode.Trim() == "N");
            if(filterParam.From_Date != null && filterParam.To_Date != null) {
                pred_Packing_List.And(x => x.Receive_Date >= DateTime.Parse(filterParam.From_Date + " 00:00:00.000") &&
                                    x.Receive_Date <= DateTime.Parse(filterParam.To_Date + " 23:59:59.000"));
            }
            if (!String.IsNullOrEmpty(filterParam.MO_No)) {
                pred_Packing_List.And(x => x.MO_No.Trim() == filterParam.MO_No.Trim());
            }
            if( filterParam.Supplier_ID != "All" && !String.IsNullOrEmpty(filterParam.Supplier_ID)) {
                pred_Packing_List.And(x => x.Supplier_ID.Trim() == filterParam.Supplier_ID.Trim());
            }
            var data = _repoPackingList.FindAll(pred_Packing_List);
            return await PagedList<WMSB_Packing_List>.CreateAsync(data, param.PageNumber, param.PageSize, false);
        }

        public async Task<Packing_List_Dto> FindBySupplier(string supplier_ID)
        {
            var data = await _repoPackingList.FindAll(x => x.Supplier_ID.Trim() == supplier_ID.Trim()).FirstOrDefaultAsync();
            var model = _mapper.Map<Packing_List_Dto>(data);
            return model;
        }
    }
}