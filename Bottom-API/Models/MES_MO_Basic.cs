using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bottom_API.Models
{
    /// <summary>
    /// 半底倉日生產計畫檔(HP轉入)
    /// </summary>
    public partial class MES_MO_Basic
    {
        /// <summary>
        /// 廠別
        /// </summary>
        [Key]
        [StringLength(1)]
        public string Factory_ID { get; set; }
        /// <summary>
        /// 線別
        /// </summary>
        [StringLength(3)]
        public string Line_ID { get; set; }
        /// <summary>
        /// 成型線別
        /// </summary>
        [StringLength(3)]
        public string Line_ID_ASY { get; set; }
        /// <summary>
        /// 組底線別
        /// </summary>
        [StringLength(3)]
        public string Line_ID_STF { get; set; }
        /// <summary>
        /// 成型部門
        /// </summary>
        [StringLength(3)]
        public string Dept_ID { get; set; }
        /// <summary>
        /// 針車部門
        /// </summary>
        [StringLength(3)]
        public string Dept_ID_STC { get; set; }
        /// <summary>
        /// 組底部門
        /// </summary>
        [StringLength(3)]
        public string Dept_ID_STF { get; set; }
        /// <summary>
        /// 企劃單號
        /// </summary>
        [Key]
        [StringLength(15)]
        public string MO_No { get; set; }
        /// <summary>
        /// 企劃單批次
        /// </summary>
        [Key]
        [StringLength(3)]
        public string MO_Seq { get; set; }
        /// <summary>
        /// 型體編號
        /// </summary>
        [StringLength(15)]
        public string Style_No { get; set; }
        /// <summary>
        /// 型體名稱
        /// </summary>
        [StringLength(40)]
        public string Style_Name { get; set; }
        /// <summary>
        /// 色號
        /// </summary>
        [StringLength(10)]
        public string Color_No { get; set; }
        /// <summary>
        /// 企劃單目標數
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
        /// 組底產量
        /// </summary>
        public int? UTN_Yield_Qty_STF { get; set; }
        /// <summary>
        /// 組底轉出數量
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
        /// 成型應開始日
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime? Plan_Start_ASY { get; set; }
        /// <summary>
        /// 成型應結束日
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime? Plan_End_ASY { get; set; }
        /// <summary>
        /// 組底應開始日
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime? Plan_Start_STF { get; set; }
        /// <summary>
        /// 組底應結束日
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime? Plan_End_STF { get; set; }
        /// <summary>
        /// 針車應開始日
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime? Plan_Start_STC { get; set; }
        /// <summary>
        /// 針車應結束日
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime? Plan_End_STC { get; set; }
        /// <summary>
        /// 裁斷應開始日
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime? Plan_Start_CUT { get; set; }
        /// <summary>
        /// 裁斷應結束日
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime? Plan_End_CUT { get; set; }
        /// <summary>
        /// 成型實際開始日
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime? Act_Start_ASY { get; set; }
        /// <summary>
        /// 成型實際結束日
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime? Act_End_ASY { get; set; }
        /// <summary>
        /// 組底實際開始日
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime? Act_Start_STF { get; set; }
        /// <summary>
        /// 組底實際結束日
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime? Act_End_STF { get; set; }
        /// <summary>
        /// 針車實際開始日
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime? Act_Start_STC { get; set; }
        /// <summary>
        /// 針車實際結束日
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime? Act_End_STC { get; set; }
        /// <summary>
        /// 裁斷實際開始日
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime? Act_Start_CUT { get; set; }
        /// <summary>
        /// 裁斷實際結束日
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime? Act_End_CUT { get; set; }
        /// <summary>
        /// 目的地
        /// </summary>
        [StringLength(50)]
        public string Destination { get; set; }
        /// <summary>
        /// 最後裝箱日
        /// </summary>
        [Column(TypeName = "date")]
        public DateTime? Comfirm_Date { get; set; }
        /// <summary>
        /// 生產季節
        /// </summary>
        [StringLength(4)]
        public string Prod_Season { get; set; }
        /// <summary>
        /// 重點型體
        /// </summary>
        [StringLength(1)]
        public string Top_Model { get; set; }
        /// <summary>
        /// 狀態
        /// </summary>
        [StringLength(15)]
        public string Status { get; set; }
        public int? Received_Qty { get; set; }
        /// <summary>
        /// 預計出貨日,final裝船日
        /// </summary>
        [Column(TypeName = "date")]
        public DateTime? CRD { get; set; }
        /// <summary>
        /// 入庫數
        /// </summary>
        public int? In_QTY { get; set; }
        /// <summary>
        /// 最後入庫日
        /// </summary>
        [Column(TypeName = "date")]
        public DateTime? Last_In_Date { get; set; }
        /// <summary>
        /// 異動者
        /// </summary>
        [StringLength(16)]
        public string Updated_By { get; set; }
        /// <summary>
        /// 異動時間
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime? Update_Time { get; set; }
        /// <summary>
        /// 現場回報成型預計完成日
        /// </summary>
        [Column(TypeName = "date")]
        public DateTime? Prd_Plan_End_ASY { get; set; }
        /// <summary>
        /// 生產別
        /// </summary>
        [StringLength(1)]
        public string Product_Type { get; set; }
        /// <summary>
        /// 訂單狀況
        /// </summary>
        [StringLength(1)]
        public string Order_Status { get; set; }
        /// <summary>
        /// 現場回報成型預計完成日
        /// </summary>
        [Column(TypeName = "date")]
        public DateTime? Prd_Plan_End_STC { get; set; }
    }
}