using System;

namespace Bottom_API.DTO.GenareQrCode
{
    public class QRCodeMainViewModel
    {
        public string QRCode_ID {get;set;}
        public string MO_No {get;set;}
        public int QRCode_Version {get;set;}
        public string Receive_No {get;set;}
        public DateTime? Receive_Date {get;set;} 
        public string Supplier_ID {get;set;}
        public string Supplier_Name {get;set;}
        public string T3_Supplier {get;set;}
        public string T3_Supplier_Name {get;set;}
        public string Subcon_ID {get;set;}
        public string Subcon_Name {get;set;}
        public string Model_No {get;set;}
        public string Model_Name {get;set;}
        public string Article {get;set;}
        public string MO_Seq {get;set;}
        public string Material_ID {get;set;}
        public string Material_Name {get;set;}
        public DateTime? Stockfiting_Date {get;set;}
        public DateTime? Assembly_Date {get;set;}
        public DateTime? CRD {get;set;}
        public string Line_ASY {get;set;}
        public string Custmoer_Part {get;set;}
        public string Custmoer_Name {get;set;}
        public string QRCode_Type {get;set;}
    }
}