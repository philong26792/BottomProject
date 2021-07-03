using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bottom_API.Models
{
    public partial class Materials
    {
        public Materials()
        {
            PoMaterials = new HashSet<PoMaterials>();
        }

        [Key]
        public long Id { get; set; }
        public DateTime CreateAt { get; set; }
        [StringLength(10)]
        public string Cunit { get; set; }
        [StringLength(10)]
        public string Eunit { get; set; }
        [Required]
        [StringLength(10)]
        public string Itnbr { get; set; }
        [Required]
        [StringLength(250)]
        public string NameCN { get; set; }
        [Required]
        [StringLength(250)]
        public string NameLC { get; set; }
        public DateTime UpdateAt { get; set; }
        public long VendorId { get; set; }

        [InverseProperty("Material")]
        public virtual ICollection<PoMaterials> PoMaterials { get; set; }
    }
}