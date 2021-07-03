using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bottom_API.Models
{
    public partial class WMSB_Setting_Supplier
    {
        [Key]
        [StringLength(1)]
        public string Factory { get; set; }
        [Key]
        [StringLength(10)]
        public string Supplier_No { get; set; }
        [StringLength(150)]
        public string Supplier_Name { get; set; }
        [StringLength(50)]
        public string Contact_Persion { get; set; }
        [StringLength(15)]
        public string Phone { get; set; }
        [StringLength(2500)]
        public string Email { get; set; }
        [StringLength(250)]
        public string Subject { get; set; }
        [Column(TypeName = "ntext")]
        public string Content { get; set; }
        [StringLength(50)]
        public string Updated_By { get; set; }
        [Key]
        [StringLength(10)]
        public string Subcon_ID { get; set; }
        [StringLength(80)]
        public string Subcon_Name { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? Updated_Time { get; set; }
    }
}