using System.Collections.Generic;

namespace Bottom_API.DTO.MergeQrCode
{
    public class QrCodeSplitDetail_Dto
    {
        public string MO_No {get;set;}
        public string Model_No {get;set;}
        public string Model_Name {get;set;}
        public string Article {get;set;}
        public string Material_ID {get;set;}
        public string Material_Name {get;set;}
        public List<SizeAndQty> ListSizeAndQty { get; set; }
    }
}