using Bottom_API._Repositories.Interfaces.DbHpBasic;
using Bottom_API.Data;
using Bottom_API.Models;

namespace Bottom_API._Repositories.Repositories.DbHpBasic
{
    public class HPHolidaysi46Repository : HPRepository<HP_Holidays_i46>, IHPHolidaysi46Repository
    {
        public HPHolidaysi46Repository(HPDataContext context) : base(context)
        {
        }
    }
}