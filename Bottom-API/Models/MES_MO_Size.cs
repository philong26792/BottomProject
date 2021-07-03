using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bottom_API.Models
{
    [Table("MES_MO_Size")]
    public partial class MES_MO_Size
    {
        /// <summary>
        /// Chinese_Taiwan_Stroke_CS_AS
        /// </summary>
        [Key]
        [StringLength(1)]
        public string Factory_ID { get; set; }
        /// <summary>
        /// Chinese_Taiwan_Stroke_CS_AS
        /// </summary>
        [Key]
        [StringLength(21)]
        public string Cycle_No { get; set; }
        /// <summary>
        /// Chinese_Taiwan_Stroke_CS_AS
        /// </summary>
        [Key]
        [StringLength(4)]
        public string Size_Code { get; set; }
        /// <summary>
        /// 工具尺寸
        /// </summary>
        [StringLength(4)]
        public string Size_TCode { get; set; }
        /// <summary>
        /// 目標產量
        /// </summary>
        public int? Plan_Qty { get; set; }
        /// <summary>
        /// 實際產量
        /// </summary>
        public int? UTN_Yield_Qty { get; set; }
        /// <summary>
        /// 針車產量
        /// </summary>
        public int? UTN_Yield_Qty_STC { get; set; }
        /// <summary>
        /// 組底實際累計產量
        /// </summary>
        public int? UTN_Yield_Qty_STF { get; set; }
        /// <summary>
        /// 組底轉出數
        /// </summary>
        public int? UTN_Forward_Qty_STF { get; set; }
        /// <summary>
        /// 針車轉出數量
        /// </summary>
        public int? UTN_Forward_Qty_STC { get; set; }
        /// <summary>
        /// 針車入庫數量
        /// </summary>
        public int? UTN_Yield_Qty_STC_In { get; set; }
        /// <summary>
        /// 異動時間
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime? Update_Time { get; set; }
        /// <summary>
        /// 異動者
        /// </summary>
        [StringLength(16)]
        public string Updated_By { get; set; }
        /// <summary>
        /// Chinese_Taiwan_Stroke_CS_AS
        /// </summary>
        [Required]
        [StringLength(26)]
        public string Biz_Key { get; set; }
        /// <summary>
        /// Chinese_Taiwan_Stroke_CS_AS
        /// </summary>
        [StringLength(21)]
        public string MO_No { get; set; }
        /// <summary>
        /// Chinese_Taiwan_Stroke_CS_AS
        /// </summary>
        [StringLength(21)]
        public string MO_Seq { get; set; }
    }
}