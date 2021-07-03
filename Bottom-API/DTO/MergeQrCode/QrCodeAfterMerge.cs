using System;

namespace Bottom_API.DTO.MergeQrCode
{
    public class QrCodeAfterMerge
    {
        public string Transac_No {get;set;}
        public string QRCode_ID {get;set;}
        public string MO_No {get;set;}
        public string Model_No {get;set;}
        public string Model_Name {get;set;}
        public string Article {get;set;}
        public string Material_ID {get;set;}
        public string Material_Name {get;set;}
        public DateTime? Merge_Time {get;set;}
        public decimal? Qty {get;set;}
        public string Update_By {get;set;}
    }
}