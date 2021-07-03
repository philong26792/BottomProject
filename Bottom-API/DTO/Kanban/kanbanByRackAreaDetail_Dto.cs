using System.Collections.Generic;

namespace Bottom_API.DTO.Kanban
{
    public class KanbanByRackAreaDetail_Dto
    {
        public string  Build_ID { get; set; }
        public string Area_ID  { get; set; }
        public string Area_Name { get; set; }
        public List<RackDetail> RackDetail { get; set; }
    }
    public class RackDetail
    {
        public string Rack { get; set; }
        public decimal? Count { get; set; }
        public string T3 { get; set; }
    }
}