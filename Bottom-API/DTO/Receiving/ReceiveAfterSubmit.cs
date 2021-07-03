using System;

namespace Bottom_API.DTO.Receiving
{
    public class ReceiveAfterSubmit
    {
        public string MO_No {get;set;}
        public string Purchase_No {get;set;}
        public string Receive_No {get;set;}
        public string MO_Seq {get;set;}
        public string Material_ID {get;set;}
        public string Material_Name {get;set;}
        public DateTime? Receive_Time {get;set;}
        public decimal? Purchase_Qty {get;set;}
        public string Type {get;set;}
        public string Update_By {get;set;}
    }
}