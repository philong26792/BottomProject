using System.Collections.Generic;

namespace Bottom_API.DTO.TransferForm
{
    public class Transfer_Form_Print_Dto
    {
        public string Collect_Trans_No { get; set; }
        public string MO_No { get; set; }
        public string MO_Seq { get; set; }
        public string Material_ID { get; set; }
        public string Material_Name { get; set; }
        public string Article { get; set; }
        public decimal? Plan_Qty { get; set; }
        public string Model_Name { get; set; }
        public string Model_No { get; set; }
        public string Subcon_ID { get; set; }
        public string Subcon_Name { get; set; }
        public string T3_Supplier { get; set; }
        public string T3_Supplier_Name { get; set; }
        public string Supplier_ID { get; set; }
        public string Supplier_Name { get; set; }
        public string Custmoer_Part { get; set; }
        public string Custmoer_Name { get; set; }
        public string Line_ASY { get; set; }
        public string Line_STF { get; set; }
        public string Rack_Location { get; set; }
        public List<Transfer_Form_Print_Qty_Dto> TransferFormQty { get; set; }
    }

    public class Transfer_Form_Print_Qty_Dto
    {
        public string Tool_Size { get; set; }
        public string Order_Size { get; set; }
        public decimal? MO_Qty { get; set; }
        public decimal? Trans_Qty { get; set; }
        public decimal? Act_Qty { get; set; } = 0;
        public decimal? Act_Trans_Qty { get; set; }
        public decimal? SumMOQty { get; set; }
        public decimal? SumTransQty { get; set; }
    }
}