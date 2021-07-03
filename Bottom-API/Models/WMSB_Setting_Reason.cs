using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bottom_API.Models
{
    public class WMSB_Setting_Reason
    {
        [Key]
        public int Kind { get; set; }
        [Key]
        [StringLength(10)]
        public string Reason_Code { get; set; }
        [StringLength(50)]
        public string Kind_Name { get; set; }
        [StringLength(10)]
        public string HP_Reason_Code { get; set; }
        [StringLength(50)]
        public string Reason_Cname { get; set; }
        [StringLength(50)]
        public string Reason_Ename { get; set; }
        [StringLength(50)]
        public string Reason_Lname { get; set; }
        public string Trans_toHP { get; set; }
        public string Is_Shortage { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime Updated_Time { get; set; }

        [StringLength(50)]
        public string Updated_By { get; set; }
    }
}