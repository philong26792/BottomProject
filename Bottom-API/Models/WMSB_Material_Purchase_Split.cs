using System;
using System.ComponentModel.DataAnnotations;

namespace Bottom_API.Models
{
    public class WMSB_Material_Purchase_Split
    {
        [Key]
        public int ID {get;set;}
        public string MO_No {get;set;}
        public string MO_Seq {get;set;}
        public string Purchase_No {get;set;}
        public DateTime? Purchase_Date {get;set;}
        public DateTime? Confirm_Delivery {get;set;}
        public string Supplier_ID {get;set;}
        public string Custmoer_Part {get;set;}
        public string Updated_By {get;set;}
        public DateTime? Updated_Time {get;set;}
    }
}