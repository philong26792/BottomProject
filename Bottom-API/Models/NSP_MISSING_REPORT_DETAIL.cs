using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bottom_API.Models
{
    public class NSP_MISSING_REPORT_DETAIL
    {
        public DateTime? Transac_Time { get; set; }
        public DateTime? Plan_Start_STF { get; set; }
        [Column("PO#Batch")]
        public string PO_Batch { get; set; }
        public string Model_Name { get; set; }
        public string Model_No { get; set; }
        public string Article { get; set; }
        public string Part_Name { get; set; }
        public string Material_ID { get; set; }
        public string Material_Name { get; set; }
        public string Unit { get; set; }
        public int? Plan_Qty { get; set; }
        [Column(TypeName = "decimal(9,2)")]
        public decimal? Accumulated { get; set; }
        public string Tool_Size { get; set; }
        [NotMapped]
        public string Tool_Size_Missing {get; set;}
        [Column(TypeName = "decimal(9,2)")]
        public decimal? Missing_Qty { get; set; }
        [Column(TypeName = "decimal(9,2)")]
        public decimal? Accumlated_In_Qty { get; set; }
        public string Missing_Reason { get; set; }
        public string T2_Supplier { get; set; }
        public string T3_Supplier { get; set; }
        public string Missing_No { get; set; }
        public string Purchase_No { get; set; }
        public DateTime? Receive_Date { get; set; }
    }
}