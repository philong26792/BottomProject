using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bottom_API.Models
{
    public class WMSB_Setting_T2Delivery
    {
        [Key]
        public int ID { get; set; }
        public string Factory_ID { get; set; }
        public string T2_Supplier_ID { get; set; }
        public string T2_Supplier_Name { get; set; }
        public string Input_Delivery { get; set; }
        public string Reason_Code { get; set; }
        public string Reason_Name { get; set; }
        public string Is_Valid { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Invalid_Date { get; set; }
        [Required]
        public string Updated_By { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Updated_Time { get; set; }
    }
}