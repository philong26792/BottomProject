using System.ComponentModel.DataAnnotations.Schema;

namespace Bottom_API.DTO.HistoryReport
{
    public class HistoryInputReport
    {
        public string Transac_No {get;set;}
        public string Transac_Date {get;set;}
        public string Line_ID {get;set;}
        public string PO {get;set;}
        public string Model_Name {get;set;}
        public string Article {get;set;}
        public double? MO_Qty {get;set;}
        public double? Status {get;set;}
        public string StatusPercent {
            get {
                return Status + "%";
            }
        }
        public string Material_ID {get;set;}
        public string Material_Name {get;set;}
        [Column(TypeName = "decimal(9,2)")]
        public decimal? Trans_Qty {get;set;}
        [Column(TypeName = "decimal(9,2)")]
        public decimal? Sum_Instock_Qty {get;set;}
        public double? Last_Qty {get;set;}
        public string Rack_Location {get;set;}
        public string Supplier {get;set;}
        public string Updated_By {get;set;}
    }
}