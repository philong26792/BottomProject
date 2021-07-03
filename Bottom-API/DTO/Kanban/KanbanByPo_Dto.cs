using System;
namespace Bottom_API.DTO.Kanban
{
    public class KanbanByPo_Dto
    {
        public DateTime? Date { get; set; }
        public string Prod_Status {get;set;}
        public string Cell { get; set; }
        public string MO_No { get; set; }
        public string MO_Seq { get; set; }
        public string Model_Name { get; set; }
        public string Article { get; set; }
        public string Order_Status { get; set; }
        public int? Prs { get; set; }
        public int? status { get; set; }
        public int? Rev_Qty { get; set; }
        public int? Stk_Qty { get; set; }
        public string PoAndBatch
        {
            get
            {
                return MO_No + " " + MO_Seq;
            }
        }

        public string StatusPercent
        {
            get
            {
                return Math.Round(((decimal)Rev_Qty/(decimal)((Prs == null || Prs == 0) ? 1 : Prs)) * 100)  + " %";
            }
        }
    }
}