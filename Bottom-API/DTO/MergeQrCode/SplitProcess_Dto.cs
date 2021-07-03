using System.Collections.Generic;

namespace Bottom_API.DTO.MergeQrCode
{
    public class SplitProcess_Dto
    {
        public MergeQrCodeModel TransacMainMergeQrCode { get; set; }
        public List<SizeAndQty> ListSizeAndQty { get; set; }
        public List<string> ListOffsetNo { get; set; }
    }

    public class SizeAndQty
    {
        public string Tool_Size { get; set; }
        public string Order_Size { get; set; }
        public string Model_Size { get; set; }
        public decimal? Instock_Qty { get; set; }
        public decimal? Trans_Qty { get; set; }
        public decimal? Act_Out_Qty { get; set; }
        public decimal? MO_Qty { get; set; }
        public decimal? Purchase_Qty { get; set; }
        public decimal? Offset_Qty { get; set; }
        public decimal? Already_Offset_Qty { get; set; }
    }
}