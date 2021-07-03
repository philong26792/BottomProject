namespace Bottom_API.DTO.ModifyStore
{
    public class ModifyQRCodeMain_Dto
    {
        public string Supplier_ID { get; set; }
        public string Supplier_Name { get; set; }
        public string Subcon_ID { get; set; }
        public string Subcon_Name { get; set; }
        public string Article { get; set; }
        public string MO_No { get; set; }
        public string Material_ID { get; set; }
        public string Material_Name { get; set; }
        public string Custmoer_Part { get; set; }
        public string Custmoer_Name { get; set; }
        public decimal? Stock_Qty { get; set; }
        public string Model_Name { get; set; }
        public string Model_No { get; set; }
        public string QRCode_ID {get;set;}
        public bool Status {get;set;}
    }
}