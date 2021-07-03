using System.Collections.Generic;
using Bottom_API.Models;

namespace Bottom_API.DTO.Input
{
    public class IntegrationInputModel
    {
        public string Rack_Location {get;set;}
        public int QRCode_Version {get;set;}
        public string Purchase_No {get;set;}
        public string Receive_No {get;set;}
        public string QRCode_ID {get;set;}
        public string MO_No {get;set;}
        public string MO_Seq {get;set;}
        public string Supplier_ID {get;set;}
        public string Supplier_Name {get;set;}
        public string Material_ID {get;set;}
        public string Material_Name {get;set;}
        public decimal? Receive_Qty {get;set;}
        public List<WMSB_PackingList_Detail> PackingListDetailItem {get;set;}
    }
}