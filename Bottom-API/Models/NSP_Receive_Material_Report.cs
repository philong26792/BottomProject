using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bottom_API.Models
{
    public class NSP_Receive_Material_Report
    {
        public string MO_No { get; set; }
        public string MO_Seq { get; set; }
        public string Material_ID { get; set; }
        public string Material_Name { get; set; }
        public string Purchase_No { get; set; }
        public DateTime? Confirm_Date { get; set; }
        [Column(TypeName = "decimal(9,2)")]
        public decimal? Purchase_Qty { get; set; }
        [Column(TypeName = "decimal(9,2)")]
        public decimal? Plan_Qty { get; set; }
        public DateTime? Plan_Start_ASY { get; set; }
        public DateTime? Plan_Start_STF { get; set; }
        public string Line_ID { get; set; }
        public string Apart { get; set; }
        public string Model_No { get; set; }
        public string Model_Name { get; set; }
        public string Article { get; set; }
        public string Supplier_ID { get; set; }
        public string Supplier_Name { get; set; }
        [Column(TypeName = "decimal(9,2)")]
        public decimal? Instock_Qty { get; set; }
        public string Process_No { get; set; }
        public string Process_Supplier { get; set; }
        public DateTime? Receive_Date { get; set; }
        [Column(TypeName = "decimal(9,2)")]
        public decimal? Received_Qty { get; set; }
        [Column(TypeName = "decimal(9,2)")]
        public decimal? Balance { get; set; }
    }
}