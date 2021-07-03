using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Bottom_API._Repositories.Interfaces.DbHpBasic;
using Bottom_API._Services.Interfaces;
using Bottom_API.Data;
using Bottom_API.DTO.CompareReport;
using Bottom_API.Helpers;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Bottom_API._Services.Services
{
    public class CompareReportService : ICompareReportService
    {
        private readonly DataContext _context;
        private readonly IHPHolidaysi46Repository _hPHolidaysi46Repository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public CompareReportService(DataContext context,
            IHPHolidaysi46Repository hPHolidaysi46Repository,
            IMapper mapper,
            IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _hPHolidaysi46Repository = hPHolidaysi46Repository;
            _configuration = configuration;
        }
        public async Task<List<StockCompare>> GetCompare(string Receive_Date)
        {
            var data = await (_context.StockCompare.FromSqlRaw("EXEC [dbo].[NSP_STOCK_COMPARE] @Receive_Date",
            new SqlParameter("Receive_Date", Receive_Date))).ToListAsync();
            var timeSpanOneDay = TimeSpan.FromTicks(TimeSpan.TicksPerDay);
            var factoryId = _configuration.GetSection("FactoryId").Value;

            var dataTimeHpUpdateLaterBottomUpdate = data.Where(x => (x.B_HP_Rec_Date - x.A_WMS_Rec_Date) > timeSpanOneDay)
                            .OrderBy(x => x.A_WMS_Rec_Date).ToList();

            if (dataTimeHpUpdateLaterBottomUpdate.Any())
            {
                var timeBottomSmallest = dataTimeHpUpdateLaterBottomUpdate.OrderBy(x => x.A_WMS_Rec_Date).FirstOrDefault().A_WMS_Rec_Date;
                var timeHpLargest = dataTimeHpUpdateLaterBottomUpdate.OrderByDescending(x => x.B_HP_Rec_Date).FirstOrDefault().B_HP_Rec_Date;

                var hpHoliday = await _hPHolidaysi46Repository.FindAll(x => x.Holiday >= timeBottomSmallest && x.Holiday <= timeHpLargest && x.Factory_ID == factoryId)
                                    .Select(x => x.Holiday).Distinct().ToListAsync();

                foreach (var item in dataTimeHpUpdateLaterBottomUpdate)
                {
                    var countHolidayPerItem = hpHoliday.Where(x => x > item.A_WMS_Rec_Date && x < item.B_HP_Rec_Date).Count();
                    var countDaySubtraction = (item.B_HP_Rec_Date - item.A_WMS_Rec_Date) / timeSpanOneDay - countHolidayPerItem;
                    if (countDaySubtraction > 1) {
                        item.Coverage = 0;
                    }
                }
            }

            return data;
        }

        public async Task<PagedList<StockCompare>> Search(string receive_Date, int page = 1, int pageSize = 10)
        {
            var data = await this.GetCompare(receive_Date);
            return PagedList<StockCompare>.Create(data, page, pageSize);
        }
    }
}