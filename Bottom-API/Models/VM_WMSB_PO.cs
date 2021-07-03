using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bottom_API.Models
{
    public class VM_WMSB_PO
    {
        [Required]
        [StringLength(5)]
        public string Factory_ID { get; set; }
        [Required]
        [StringLength(20)]
        public string PO { get; set; }
        [StringLength(50)]
        public string Article { get; set; }
        [StringLength(20)]
        public string Model_ID { get; set; }
        [StringLength(50)]
        public string Model_Name { get; set; }
        [Column(TypeName = "numeric(11, 0)")]
        public decimal? Order_Qty { get; set; }
        [Column(TypeName = "numeric(11, 0)")]
        public decimal? Actual_Qty { get; set; }
        [StringLength(50)]
        public string Customer { get; set; }
        [StringLength(50)]
        public string Customer_Id { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Complete_Date { get; set; }
        [StringLength(1)]
        public string Complete { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Biz_Time { get; set; }
        [StringLength(16)]
        public string Updated_By { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Update_Time { get; set; }
        [StringLength(1)]
        public string Ship_Complete { get; set; }
        [StringLength(50)]
        public string Nation { get; set; }
        [StringLength(20)]
        public string Shipping_Way { get; set; }
        [StringLength(40)]
        public string Category { get; set; }
        [StringLength(20)]
        public string Pono { get; set; }
        [StringLength(4)]
        public string Service_Seq { get; set; }
        [StringLength(4)]
        public string Customer_No { get; set; }
        [StringLength(1)]
        public string Flag { get; set; }
    }
}