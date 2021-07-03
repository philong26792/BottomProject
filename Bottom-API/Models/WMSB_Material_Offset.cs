using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bottom_API.Models
{
    public class WMSB_Material_Offset
    {
        [StringLength(10)]
        public string Factory_ID { get; set; }
        [Key]
        [StringLength(20)]
        public string Offset_No { get; set; }
        [StringLength(50)]
        public string DCollect_No { get; set; }
        [Key]
        [StringLength(20)]
        public string DMO_No { get; set; }
        [StringLength(50)]
        public string Collect_No { get; set; }
        [Key]
        [StringLength(20)]
        public string MO_No { get; set; }
        [Key]
        [StringLength(10)]
        public string MO_Seq { get; set; }
        [Key]
        [StringLength(10)]
        public string Material_ID { get; set; }
        [Key]
        [StringLength(10)]
        public string Order_Size { get; set; }
        [StringLength(10)]
        public string Model_Size { get; set; }
        [StringLength(10)]
        public string Tool_Size { get; set; }
        [StringLength(10)]
        public string Spec_Size { get; set; }
        [StringLength(10)]
        public string Purchase_Size { get; set; }
        [Column(TypeName = "decimal(9, 2)")]
        public decimal? MO_Qty { get; set; }
        [Column(TypeName = "decimal(9, 2)")]
        public decimal? PreBook_Qty { get; set; }
        [Column(TypeName = "decimal(9, 2)")]
        public decimal? Stock_Qty { get; set; }
        [StringLength(1)]
        public string Biz_Tflag { get; set; }
        [StringLength(16)]
        public string HP_User { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? HP_Time { get; set; }
        [StringLength(50)]
        public string Update_By { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Update_Time { get; set; }
    }
}