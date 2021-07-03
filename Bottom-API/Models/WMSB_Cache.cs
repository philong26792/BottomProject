using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bottom_API.Models
{
    public class WMSB_Cache
    {
        [Key]
        [StringLength(15)]
        public string MO_No { get; set; }
        [Key]
        [StringLength(3)]
        public string MO_Seq { get; set; }
        // public int? Plan_Qty { get; set; }
        [StringLength(20)]
        public string Model_No { get; set; }
        [StringLength(50)]
        public string Model_Name { get; set; }
        [StringLength(50)]
        public string Article { get; set; }
        [StringLength(3)]
        public string Line_ID { get; set; }
        [Column(TypeName = "date")]
        public DateTime? Plan_Start_STF { get; set; }
        [Column(TypeName = "date")]
        public DateTime? Plan_Start_CUT { get; set; }
        [StringLength(10)]
        public string Tool_Type { get; set; }
        [StringLength(20)]
        public string Tool_ID { get; set; }
        [Key]
        [StringLength(10)]
        public string Material_ID { get; set; }
        [StringLength(240)]
        public string Material_Name { get; set; }
        public double? MO_Qty { get; set; }
        [StringLength(4)]
        public string Unit { get; set; }
        [StringLength(20)]
        public string Part_No { get; set; }
        [StringLength(100)]
        public string Part_Name { get; set; }
        [StringLength(5)]
        public string Process_Code { get; set; }
        [StringLength(80)]
        public string Process_Name { get; set; }
        [StringLength(4)]
        public string T2_Supplier_ID { get; set; }
        [StringLength(10)]
        public string T2_Supplier_Name { get; set; }
        [StringLength(4)]
        public string T3_Supplier_ID { get; set; }
        [StringLength(10)]
        public string T3_Supplier_Name { get; set; }
        [StringLength(1000)]
        public string Order_Size { get; set; }
        [StringLength(1000)]
        public string Size_Run { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Update_Time { get; set; }
        [Column(TypeName = "date")]
        public DateTime? Plan_Start_ASY { get; set; }
        public string Version {get;set;}
        [Column(TypeName = "datetime")]
        public DateTime? Upload_Time { get; set; }
                
        public DateTime? CRD {get;set;}
    }
}