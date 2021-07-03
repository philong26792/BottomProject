using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bottom_API.Models
{
    public class VM_WMSB_MES_MO
    {
        [Required]
        [StringLength(1)]
        public string Factory_ID { get; set; }
        [StringLength(3)]
        public string Line_ID { get; set; }
        [StringLength(3)]
        public string Line_ID_ASY { get; set; }
        [StringLength(3)]
        public string Line_ID_STF { get; set; }
        [StringLength(3)]
        public string Dept_ID { get; set; }
        [StringLength(3)]
        public string Dept_ID_STC { get; set; }
        [StringLength(3)]
        public string Dept_ID_STF { get; set; }
        [StringLength(15)]
        public string MO_No { get; set; }
        [StringLength(3)]
        public string MO_Seq { get; set; }
        [Required]
        [StringLength(21)]
        public string Cycle_No { get; set; }
        [StringLength(15)]
        public string Style_No { get; set; }
        [StringLength(40)]
        public string Style_Name { get; set; }
        [StringLength(10)]
        public string Color_No { get; set; }
        public int? Plan_Qty { get; set; }
        public int? UTN_Yield_Qty { get; set; }
        public int? UTN_Yield_Qty_STC { get; set; }
        public int? UTN_Yield_Qty_STF { get; set; }
        public int? UTN_Forward_Qty_STF { get; set; }
        public int? UTN_Forward_Qty_STC { get; set; }
        public int? UTN_Yield_Qty_STC_In { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Plan_Start_ASY { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Plan_End_ASY { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Plan_Start_STF { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Plan_End_STF { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Plan_Start_STC { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Plan_End_STC { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Plan_Start_CUT { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Plan_End_CUT { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Act_Start_ASY { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Act_End_ASY { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Act_Start_STF { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Act_End_STF { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Act_Start_STC { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Act_End_STC { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Act_Start_CUT { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Act_End_CUT { get; set; }
        [StringLength(50)]
        public string Destination { get; set; }
        [Column(TypeName = "date")]
        public DateTime? Comfirm_Date { get; set; }
        [StringLength(4)]
        public string Prod_Season { get; set; }
        [StringLength(1)]
        public string Top_Model { get; set; }
        [StringLength(15)]
        public string Status { get; set; }
        public int? Received_Qty { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Update_Time { get; set; }
        [StringLength(16)]
        public string Updated_By { get; set; }
    }
}