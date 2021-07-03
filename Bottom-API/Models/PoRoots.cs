using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bottom_API.Models
{
    public partial class PoRoots
    {
        public PoRoots()
        {
            PoMaterials = new HashSet<PoMaterials>();
        }

        [Key]
        public long Id { get; set; }
        public DateTime CreateAt { get; set; }
        [Required]
        [StringLength(15)]
        public string Manno { get; set; }
        [Column(TypeName = "date")]
        public DateTime ProdAt { get; set; }
        [Required]
        [StringLength(5)]
        public string Teamno { get; set; }
        public DateTime UpdateAt { get; set; }
        [Column(TypeName = "date")]
        public DateTime? CutAt { get; set; }
        [StringLength(15)]
        public string Model { get; set; }
        [StringLength(1)]
        public string Final { get; set; }
        [StringLength(20)]
        public string Article { get; set; }
        [StringLength(40)]
        public string ModelNa { get; set; }
        [StringLength(20)]
        public string Cell { get; set; }
        [StringLength(3)]
        public string Batch { get; set; }
        [Column(TypeName = "decimal(7, 1)")]
        public decimal? TotQty { get; set; }
        [Column(TypeName = "date")]
        public DateTime? StfiAt { get; set; }
        [Column(TypeName = "date")]
        public DateTime? AssyAt { get; set; }
        [Column(TypeName = "date")]
        public DateTime? StiAt { get; set; }
        [Column(TypeName = "date")]
        public DateTime? Receive_Date { get; set; }

        [InverseProperty("PoRoot")]
        public virtual ICollection<PoMaterials> PoMaterials { get; set; }
    }
}