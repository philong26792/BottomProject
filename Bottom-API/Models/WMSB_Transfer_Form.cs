using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bottom_API.Models
{
    public partial class WMSB_Transfer_Form
    {
        [Required]
        [StringLength(100)]
        public string Collect_Trans_No { get; set; }
        [Required]
        [StringLength(100)]
        public string Transac_No { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Generate_Time { get; set; }
        [StringLength(10)]
        public string T3_Supplier { get; set; }
        [StringLength(50)]
        public string T3_Supplier_Name { get; set; }
        [StringLength(1)]
        public string Is_Release { get; set; }
        public string Is_Closed {get;set;}
        [StringLength(50)]
        public string Release_By { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Release_Time { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Update_Time { get; set; }
        [StringLength(50)]
        public string Update_By { get; set; }
        public string MO_No { get; set; }
        public string MO_Seq { get; set; }
        public string Material_ID { get; set; }
        public string Material_Name { get; set; }
        public int? Collec_Trans_Version { get; set; }
        public bool? Valid_Status { get; set; }
        public int? Logmail_Info { get; set; }
        public int? Logmail_Release { get; set; }
    }
}