using System;

namespace Bottom_API.DTO.ModifyStore
{
    public class ModifyQrCodeAfterSave
    {
        public string Transac_No {get;set;}
        public string Missing_No {get;set;}
        public string MO_No {get;set;}
        public string MO_Seq {get;set;}
        public string Material_ID {get;set;}
        public string Material_Name {get;set;}
        public decimal? Actual_Qty {get;set;}
        public decimal? Modify_Qty {get;set;}
        public DateTime? Modify_Time {get;set;}
        public string QRCode_ID {get;set;}
        public int QRCode_Version {get;set;}
        public string Update_By {get;set;}
    }
}