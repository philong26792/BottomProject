using AutoMapper;
using Bottom_API._Repositories.Interfaces;
using Bottom_API._Services.Interfaces;
using System.Threading.Tasks;
using Bottom_API.DTO;
using Bottom_API.Helpers;
using AutoMapper.QueryableExtensions;
using System.Linq;
using Bottom_API.Models;
using System;
using LinqKit;

namespace Bottom_API._Services.Services
{
    public class RackLocationService : IRackLocationService
    {
        private readonly IRackLocationRepo _repoRackLocation;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _configMapper;
        private readonly ITransactionMainRepo _repoTransaction;
        private readonly ICodeIDDetailRepo _repoCode;
        public RackLocationService(IRackLocationRepo repoRackLocation, ITransactionMainRepo repoTransaction, IMapper mapper, MapperConfiguration configMapper, ICodeIDDetailRepo repoCode)
        {
            _repoTransaction = repoTransaction;
            _configMapper = configMapper;
            _mapper = mapper;
            _repoRackLocation = repoRackLocation;
            _repoCode = repoCode;
        }

        public async Task<bool> Add(RackLocation_Main_Dto model)
        {
            var item = _mapper.Map<WMSB_RackLocation_Main>(model);
            _repoRackLocation.Add(item);
            return await _repoRackLocation.SaveAll();
        }

        public async Task<bool> Update(RackLocation_Main_Dto model)
        {
            var check = await _repoTransaction.CheckRackLocation(model.Rack_Location);
            if (check) return false;
            else
            {
                var item = _mapper.Map<WMSB_RackLocation_Main>(model);
                item.Updated_Time = DateTime.Now;
                _repoRackLocation.Update(item);
                return await _repoRackLocation.SaveAll();
            }
        }

        public bool CheckExistRackLocation(RackLocation_Main_Dto model)
        {
            var item = _repoRackLocation.FindSingle(x => x.Factory_ID.Trim() == model.Factory_ID.Trim()
                                                    && x.WH_ID.Trim() == model.WH_ID.Trim()
                                                    && x.Build_ID.Trim() == model.Build_ID.Trim()
                                                    && x.Floor_ID.Trim() == model.Floor_ID.Trim() 
                                                    && x.Area_ID.Trim() == model.Area_ID.Trim()
                                                    && x.Rack_Location.Trim() == model.Rack_Location.Trim());
            if (item != null) {
                return true;
            }
            else {
                return false;
            }
        }

        public async Task<bool> Delete(object id)
        {
            var item = _repoRackLocation.FindById(id);
            var check = await _repoTransaction.CheckRackLocation(item.Rack_Location);
            if (check) return false;
            else {
                _repoRackLocation.Remove(item);
                return await _repoRackLocation.SaveAll();
            }
        }

        public async Task<PagedList<RackLocation_Main_Dto>> Filter(PaginationParams param, FilterRackLocationParam filterParam)
        {
            var pred_Rack_Location = PredicateBuilder.New<WMSB_RackLocation_Main>(true);
            if (!String.IsNullOrEmpty(filterParam.Factory)) {
                pred_Rack_Location.And(x => x.Factory_ID == filterParam.Factory);
            }

            if (!String.IsNullOrEmpty(filterParam.Wh)) {
                pred_Rack_Location.And(x => x.WH_ID == filterParam.Wh);
            }

            if (!String.IsNullOrEmpty(filterParam.Building)) {
                pred_Rack_Location.And(x => x.Build_ID == filterParam.Building);
            }

            if (!String.IsNullOrEmpty(filterParam.Floor)) {
                pred_Rack_Location.And(x => x.Floor_ID == filterParam.Floor);
            }

            if (!String.IsNullOrEmpty(filterParam.Area)) {
                pred_Rack_Location.And(x => x.Area_ID == filterParam.Area);
            }
            var resultAll =  _repoRackLocation.FindAll(pred_Rack_Location).ProjectTo<RackLocation_Main_Dto>(_configMapper);
            resultAll = resultAll.OrderByDescending(x => x.Updated_Time).Select(x => new RackLocation_Main_Dto
            {
                BuildingName = _repoCode.GetBuildingName(x.Build_ID),
                AreaName = _repoCode.GetAreaName(x.Area_ID),
                FloorName = _repoCode.GetFloorName(x.Floor_ID),
                ID = x.ID,
                Rack_Location = x.Rack_Location,
                Rack_Code = x.Rack_Code,
                Rack_Level = x.Rack_Level,
                Rack_Bin = x.Rack_Bin,
                Factory_ID = x.Factory_ID,
                WH_ID = x.WH_ID,
                Build_ID = x.Build_ID,
                Floor_ID = x.Floor_ID,
                Area_ID = x.Area_ID,
                CBM = x.CBM,
                Max_per = x.Max_per,
                Memo = x.Memo,
                Rack_Invalid_date = x.Rack_Invalid_date,
                Updated_Time = x.Updated_Time,
                Updated_By = x.Updated_By,
            });

            return await PagedList<RackLocation_Main_Dto>.CreateAsync(resultAll, param.PageNumber, param.PageSize);
        }
    }
}