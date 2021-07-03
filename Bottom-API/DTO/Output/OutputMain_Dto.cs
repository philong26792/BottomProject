
namespace Bottom_API.DTO.Output
{
    public class OutputMain_Dto
    {
        public long Id { get; set; }
        public string TransacNo { get; set; }
        public string QrCodeId { get; set; }
        public int QrCodeVersion { get; set; }
        public string PlanNo { get; set; }
        public string SupplierNo { get; set; }
        public string SupplierName { get; set; }
        public string Batch { get; set; }
        public string MatId { get; set; }
        public string MatName { get; set; }
        public string RackLocation { get; set; }
        public decimal? InStockQty { get; set; }
        public decimal? TransOutQty { get; set; }
        public decimal? RemainingQty { get; set; }
        public string PickupNo { get; set; }
        public string SubconId { get; set; }
        public string SubconName { get; set; }
        public string T3Supplier { get; set; }
        public string T3SupplierName { get; set; }
    }
}