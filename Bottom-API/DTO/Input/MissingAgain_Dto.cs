using System;

namespace Bottom_API.DTO.Input
{
    public class MissingAgain_Dto
    {
        public string Type {get;set;}
        public string Missing_No {get;set;}
        public string MO_No {get;set;}
        public string MO_Seq {get;set;}
        public string Material_ID {get;set;}
        public string Material_Name {get;set;}
        public string Model_No {get;set;}
        public string Model_Name {get;set;}
        public string Article {get;set;}
        public string Custmoer_Part {get;set;}
        public string Custmoer_Name { get; set; }
        public string Rack_Location { get; set; }
        public string Supplier_ID { get; set; }
        public decimal? Missing_Qty {get;set;}
        public DateTime? Updated_Time {get;set;}
        
        public int?  Download_count { get; set; }
    }
}