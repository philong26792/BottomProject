namespace Bottom_API.DTO.MergeQrCode
{
    public class MergeQrCodeModel
    {
        public string QRCode_ID {get;set;}
        public string Transac_No {get;set;}
        public string MO_No {get;set;}
        public string Purchase_No {get;set;}
        public int QRCode_Version {get;set;}
        public string Receive_No {get;set;}
        public string Model_No {get;set;}
        public string Model_Name {get;set;}
        public string Article {get;set;}
        public string Supplier_ID {get;set;}
        public string Supplier_Name  {get;set;}
        public string Part_No {get;set;}
        public string Part_Name {get;set;}
        public string Rack_Location {get;set;}
        public string Material_ID {get;set;}
        public string Material_Name {get;set;}
        public decimal? Stock_Qty {get;set;}
        public string Transac_Type {get;set;}
    }
}