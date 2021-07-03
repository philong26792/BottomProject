namespace Bottom_API.DTO.ModifyStore
{
    public class SizeInstockQty
    {
        public string Tool_Size {get;set;}
        public string Order_Size {get;set;}
        //public string Model_Size {get;set;}
        public decimal? TotalInstockQty {get;set;}
        public decimal? PlanQty {get;set;}
        public decimal? JustReceivedQty {get;set;}
        public decimal? ModifyQty {get;set;}
        public bool IsChange {get;set;}
    }
}