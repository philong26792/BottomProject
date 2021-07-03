using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bottom_API.Models
{
    public class HP_Holidays_i46
    {
        /// <summary>
        /// 公司別
        /// </summary>
        [Key]
        [StringLength(1)]
        public string Company_No { get; set; }
        /// <summary>
        /// 事業部
        /// </summary>
        [Key]
        [StringLength(1)]
        public string Division_No { get; set; }
        /// <summary>
        /// 廠別
        /// </summary>
        [Key]
        [StringLength(10)]
        public string Factory_ID { get; set; }
        /// <summary>
        /// 休假日
        /// </summary>
        [Key]
        [Column(TypeName = "date")]
        public DateTime Holiday { get; set; }
        /// <summary>
        /// HP異動者
        /// </summary>
        [StringLength(6)]
        public string HP_User { get; set; }
        /// <summary>
        /// HP異動日
        /// </summary>
        [Column(TypeName = "date")]
        public DateTime? HP_Date { get; set; }
        /// <summary>
        /// 異動者
        /// </summary>
        [StringLength(50)]
        public string Update_By { get; set; }
        /// <summary>
        /// 異動時間
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime? Update_Time { get; set; }
    }
}