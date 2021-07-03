using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bottom_API.Models
{
    public class WMSB_Material_Missing
    {
        [Key]
        public int ID {get;set;}
        public string Missing_Stage {get;set;}
        public string Delivery_No {get;set;}
        public string Missing_No {get;set;}
        public string Factory_ID {get;set;}
        public string Purchase_No {get;set;}
        public string Collect_No {get;set;}
        public string MO_No {get;set;}
        public string MO_Seq {get;set;}
        public string Order_Size {get;set;}
        public string Material_ID {get;set;}
        public string Material_Name {get;set;}
        public string Model_No {get;set;}
        public string Model_Name {get;set;}
        public string Article {get;set;}
        public string Model_Size {get;set;}
        public string Tool_Size {get;set;}
        public string Spec_Size {get;set;}
        public string Purchase_Kind {get;set;}
        public string Purchase_Size {get;set;}
        [Column(TypeName = "decimal(9,2)")]
        public decimal? MO_Qty {get;set;}
        [Column(TypeName = "decimal(9,2)")]
        public decimal? PreBook_Qty {get;set;}
        [Column(TypeName = "decimal(9,2)")]
        public decimal? Stock_Qty {get;set;}
        [Column(TypeName = "decimal(9,2)")]
        public decimal? Purchase_Qty {get;set;}
        [Column(TypeName = "decimal(9,2)")]
        public decimal? Accumlated_In_Qty {get;set;}
        public string Status {get;set;}
        public string Supplier_ID {get;set;}
        public string Supplier_Name {get;set;}
        public DateTime? Require_Delivery {get;set;}
        public DateTime? Confirm_Delivery {get;set;}
        public string Tool_Type {get;set;}
        public string Tool_ID {get;set;}
        public string Process_Code {get;set;}
        public string Subcon_Name {get;set;}
        public string Custmoer_Part {get;set;}
        public string T3_Supplier {get;set;}
        public string T3_Supplier_Name {get;set;}
        public string Stage {get;set;}
        public string Notice_Kind {get;set;}
        public string T3_Purchase_No {get;set;}
        public DateTime? Updated_Time {get;set;}
        public string Updated_By {get;set;}
        public string Reason_Code {get;set;}
        public string Is_Missing {get;set;}
        public int?  Download_count { get; set; }
        public WMSB_Material_Missing() {
            this.Download_count = 0;
        }
        
    }
}