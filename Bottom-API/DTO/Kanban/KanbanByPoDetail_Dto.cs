using System;

namespace Bottom_API.DTO.Kanban
{
    public class KanbanByPoDetail_Dto
    {
        public string MO_No { get; set; }
        public string MO_Seq { get; set; }
        public DateTime? Confirm_Date { get; set; }
        public string T2_Supplier { get; set; }
        public string T2_Supplier_Name { get; set; }
        public string Material_NO { get; set; }
        public string Material_Name { get; set; }
        public string CustomerPart { get; set; }
        public int? Prs { get; set; }
        public int? RevQty { get; set; }
        public int? StkQty { get; set; }
        public string Rack_Location { get; set; }
        public int Source_Count { get; set; }
        
    }
}