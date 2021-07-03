using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bottom_API.DTO.Kanban
{
    public class KanbanByCategoryDetailByToolCode_Dto
    {
        public string Rack { get; set; }
        [Column("PO+Batch")]
        public string PoAndBatch { get; set; }

        [Column("MTRL.Code")]
        public string MaterialId { get; set; }
        [Column("Material Name")]

        public string MaterialName { get; set; }
        public string Unit { get; set; }
        public int? Qty { get; set; }
        [Column("ASL Line")]
        public string LineASY { get; set; }
        [Column("STF Start Date")]
        public string STF_Date { get; set; }
        public DateTime? StockfitingDate
        {
            get
            {
                if (STF_Date != "")
                {
                    return Convert.ToDateTime(STF_Date);
                }
                else
                {
                    return null;
                }
            }
        }

        public string T2_Supplier { get; set; }
        [Column("TTL PRS")]
        public int? TTL_PRS { get; set; }
        [Column("Customer Part")]
        public string CustomerPart { get; set; }
        public string Model { get; set; }
        public string Article { get; set; }
        public string T3_Supplier { get; set; }
    }
}