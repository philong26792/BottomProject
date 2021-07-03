using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bottom_API.Models
{
    public class NSP_REPORT_DETAIL_NO_SIZE
    {
         public string MO_No { get; set; }
        public string MO_Seq { get; set; }
        public DateTime? Confirm_Date { get; set; }
        public string T2_Supplier { get; set; }
        public string T2_Supplier_Name { get; set; }
        public string Material_NO { get; set; }
        public string Material_Name { get; set; }
        public string  Title_ID {get;set;}
        public int? Total_Pairs { get; set; }
        [Column("Part")]
        public string CustomerPart { get; set; }
        public string Rack_Location { get; set; }
        public int Source_Count { get; set; }
    }
}