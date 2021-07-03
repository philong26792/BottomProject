using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bottom_API.Models
{
    public class WMSB_Reason_Detail
    {
        [Key]
        public int ID {get;set;}
        public string Missing_No {get;set;}
        public string Reason_Code {get;set;}
        public string Order_Size {get;set;}
        public string Model_Size {get;set;}
        public string Tool_Size {get;set;}
        [Column(TypeName = "decimal(9,2)")]
        public decimal? Left_Qty {get;set;}
        [Column(TypeName = "decimal(9,2)")]
        public decimal? Right_Qty {get;set;}
        [Column(TypeName = "decimal(9,2)")]
        public decimal? Modify_Qty {get;set;}
        public string Updated_By {get;set;}
        public DateTime? Updated_Time {get;set;}
    }
}