using System;
using System.Collections.Generic;

namespace Bottom_API.DTO.MergeQrCode
{
    public class SplitQrCodeMain_Dto
    {
        public SplitQrCodeDetail SplitPlanNoParent { get; set; }
        public List<SplitQrCodeDetail> SplitPlanNoChild { get; set; }
    }

    public class SplitQrCodeDetail 
    {
        public string Transac_No { get; set; }
        public string Transac_Type { get; set; }
        public string QRCode_ID {get;set;}
        public int QRCode_Version {get;set;}
        public string MO_No { get; set; }
        public string MO_Seq { get; set; }
        public string Material_ID { get; set; }
        public string Material_Name { get; set; }
        public string Rack_Location { get; set; }
        public DateTime? Split_Time { get; set; }
        public string PreBuy_MO_No { get; set; }
        public string Updated_By { get; set; }
        public decimal? Stock_Qty { get; set; }

    }
}