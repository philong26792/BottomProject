namespace Bottom_API.DTO.ModifyStore
{
    public class SizeInStockPlanQty
    {
        public string Tool_Size {get;set;}
        public string Order_Size {get;set;}
        public decimal? TotalInstockQty {get;set;}
        public decimal? PlanQty {get;set;}
    }
}