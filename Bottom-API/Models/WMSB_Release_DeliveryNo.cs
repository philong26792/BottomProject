using System;
using System.ComponentModel.DataAnnotations;

namespace Bottom_API.Models
{
    public class WMSB_Release_DeliveryNo
    {
        [Key]
        public int ID { get; set; }
        public string Delivery_No { get; set; }
        public DateTime? Transac_Time { get; set; }
        public string Supplier_ID { get; set; }
        public string Supplier_Name { get; set; }
        public string Purchase_No { get; set; }
        public string MO_No { get; set; }
        public string Material_ID { get; set; }
        public string Material_Name { get; set; }
        public decimal? Transacted_Qty { get; set; }
        public string Is_Release { get; set; }
        public DateTime? Release_Time { get; set; }
        public string Release_By { get; set; }
        public string Updated_By { get; set; }
        public DateTime? Updated_Time { get; set; }
    }
}