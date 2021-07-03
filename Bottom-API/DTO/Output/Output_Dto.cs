using System.Collections.Generic;

namespace Bottom_API.DTO.Output
{
    public class Output_Dto
    {
        public List<OutputMain_Dto> Outputs { get; set; }
        public List<OutputTotalNeedQty_Dto> OutputTotalNeedQty { get; set; }
        public string Message { get; set; }
    }

    public class OutputTotalNeedQty_Dto 
    {
        public string Order_Size { get; set; }
        public string Model_Size { get; set; }
        public string Tool_Size { get; set; }
        public decimal? Qty { get; set; }
    }
}