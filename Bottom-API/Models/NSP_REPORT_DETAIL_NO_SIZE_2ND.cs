using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bottom_API.Models
{
    public class NSP_REPORT_DETAIL_NO_SIZE_2ND
    {
        public DateTime? Plan_Start_STF { get; set; }
        public string Line_ID { get; set; }
        public string MO_No { get; set; }
        public string MO_Seq { get; set; }
        public string Model_Name { get; set; }
        public string Article { get; set; }
        public int? Plan_Qty { get; set; }
        public DateTime? Transaction_Date {get;set;}
        public string T2_Supplier { get; set; }
        public string Rack_Location { get; set; }
        public string Material_No { get; set; }
        public string Material_Name { get; set; }
        public string QRCode_ID { get; set; }
        public string Receiving_Type { get; set; }
        [Column(TypeName = "decimal(9,2)")]
        public decimal? Total_Pairs { get; set; }
    }
}