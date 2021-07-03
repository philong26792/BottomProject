using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bottom_API.DTO.CompareReport
{
    public class StockCompare
    {
        public string MO_No {get;set;}
        public string Model_No {get;set;}
        public string Model_Name {get;set;}
        public string Article {get;set;}
        public string Material_ID {get;set;}
        public string Material_Name {get;set;}
        public string HP_WH {get;set;}
        public DateTime? A_WMS_Rec_Date {get;set;}
        public DateTime? B_HP_Rec_Date {get;set;}
        public DateTime? Freeze_Date {get;set;}
        public int Coverage {get;set;}
        [Column(TypeName = "decimal(9,2)")]
        public decimal? C_WMS_Accu_Rec_Qty {get;set;}
        [Column(TypeName = "decimal(9,2)")]
        public decimal? D_HP_Accu_Rec_Qty {get;set;}
        [Column(TypeName = "decimal(9,2)")]
        public decimal? E_Balance {get;set;}
        public int Accuracy {get;set;}
        public string Supplier_ID {get;set;}
        public string T2_Supplier_Name {get;set;}
        [NotMapped]
        public string Supplier {
            get {
                return Supplier_ID.Trim() + " - " + T2_Supplier_Name.Trim();
            }
        }
    }
}