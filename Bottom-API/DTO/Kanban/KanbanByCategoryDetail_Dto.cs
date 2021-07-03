using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bottom_API.DTO.Kanban
{
    public class KanbanByCategoryDetail_Dto
    {
        public string Rack { get; set; }
        [Column("Tool.Code")]

        public string ToolCode { get; set; }
        [Column("MTRL.Code")]
  
        public string MaterialId { get; set; }
        [Column("Material Name")]
      
        public string MaterialName { get; set; }
        public string Unit { get; set; }
        public int? Qty { get; set; }
        public string T2_Supplier { get; set; }
        [Column("TTL PRS")]
        public int? TTL_PRS { get; set; }
    }
}