namespace Bottom_API.DTO.Receiving
{
    public class ReceivingMaterialMainModel
    {
        public string Status {get;set;}
        public string Material_ID {get;set;}
        public string Material_Name {get;set;}
        public string MO_No {get;set;}
        public string Purchase_No {get;set;}
        public string Model_No {get;set;}
        public string Model_Name {get;set;}
        public string Article {get;set;}
        public string Supplier_ID {get;set;}
        public string Supplier_Name {get;set;}
        public string Subcon_No {get;set;}
        public string Subcon_Name {get;set;}
        public string T3_Supplier {get;set;}
        public string T3_Supplier_Name {get;set;}
        public decimal? Qty {get;set;}
    }
}