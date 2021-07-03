using System.Collections.Generic;

namespace Bottom_API.DTO.ModifyStore
{
    public class ModifyQrCodeSaveParam
    {
        public ModifyQRCodeMain_Dto model { get; set; }
        public List<SizeInstockQtyByBatch> data { get; set; }
        public string reason_code { get; set; }
        public bool isMissing { get; set; }
        public List<LeftRightInBatchOfReason> ReasonDetail { get; set; }
    }
    public class QtyOfSize
    {
        public string Tool_Size { get; set; }
        public string Order_Size { get; set; }
        public decimal? Qty { get; set; }
    }

    public class LeftRightInBatchOfReason
    {
        public string Batch { get; set; }
        public string Reason { get; set; }
        public string Tool_Size { get; set; }
        public string Order_Size { get; set; }
        public decimal? Left {get;set;}
        public decimal? Right {get;set;}
    }
}