using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bottom_API.Models
{
    public partial class PoMaterials
    {
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        [Key]
        public long Id { get; set; }
        public long PoRootId { get; set; }
        public long MaterialId { get; set; }
        public long VendorId { get; set; }
        [StringLength(1)]
        public string Status { get; set; }
        [StringLength(15)]
        public string Conno { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Qty { get; set; }
        [StringLength(4)]
        public string Size { get; set; }
        [Column(TypeName = "date")]
        public DateTime? DeliveryAt { get; set; }
        [Column(TypeName = "date")]
        public DateTime? ShippingAt { get; set; }
        [Column(TypeName = "date")]
        public DateTime? IssueAt { get; set; }
        [Column(TypeName = "date")]
        public DateTime? ReturnAt { get; set; }
        [Column(TypeName = "date")]
        public DateTime? ETAC { get; set; }
        [Column(TypeName = "decimal(7, 1)")]
        public decimal? PlanNSQty { get; set; }
        [Column(TypeName = "decimal(7, 1)")]
        public decimal? PlanSQty { get; set; }
        [StringLength(2)]
        public string Kind { get; set; }
        [Column(TypeName = "date")]
        public DateTime? ETAC_Import { get; set; }
        public string Purchasing_Order { get; set; }
        [StringLength(999)]
        public string Merchandiser { get; set; }
        [Column(TypeName = "decimal(9, 2)")]
        public decimal? Dispatch_Qty { get; set; }
        [Column(TypeName = "decimal(9, 2)")]
        public decimal? CollectPlan_Qty { get; set; }
        [Column(TypeName = "decimal(9, 2)")]
        public decimal? Ready_Qty { get; set; }

        [ForeignKey(nameof(MaterialId))]
        [InverseProperty(nameof(Materials.PoMaterials))]
        public virtual Materials Material { get; set; }
        [ForeignKey(nameof(PoRootId))]
        [InverseProperty(nameof(PoRoots.PoMaterials))]
        public virtual PoRoots PoRoot { get; set; }
    }
}