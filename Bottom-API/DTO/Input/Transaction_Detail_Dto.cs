using System.Collections.Generic;

namespace Bottom_API.DTO.Input
{
    public class Transaction_Detail_Dto
    {
        public string Input_No { get; set; }
        public string QrCode_Id { get; set; }
        public string Plan_No { get; set; }
        public string Suplier_No { get; set; }
        public string Suplier_Name { get; set; }
        public string Batch { get; set; }
        public decimal? Accumated_Qty { get; set; }
        public decimal? Trans_In_Qty { get; set; }
        public decimal? InStock_Qty { get; set; }
        public string Mat_Id { get; set; }
        public string Mat_Name { get; set; }
        public string Rack_Location { get; set; }
        public string Is_Scanned {get;set;}
        public List<DetailSize> Detail_Size { get; set; }
    }
}