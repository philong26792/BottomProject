using System.ComponentModel.DataAnnotations.Schema;

namespace Bottom_API.Models
{
    public class NSP_RACKS_AREA_RACK_LIST
    {
        public string  Build_ID { get; set; }

        public string  Area_ID { get; set; }
        public string  Area_Name { get; set; }
        public string  Rack { get; set; }
        [Column(TypeName = "decimal(9,2)")]
        public decimal?  Stk_Qty { get; set; }

        public string  T3 { get; set; }  
    }   
}