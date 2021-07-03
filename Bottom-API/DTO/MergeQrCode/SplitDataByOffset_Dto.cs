using System;
using System.Collections.Generic;

namespace Bottom_API.DTO.MergeQrCode
{
    public class SplitDataByOffset_Dto
    {
        public string MO_No { get; set; }
        public string DMO_No { get; set; }
        public string DTransac_No { get; set; }
        public string MO_Seq { get; set; }
        public string Material_ID { get; set; }
        public string Rack_Location { get; set; }
        public DateTime? Plan_Start_STF { get; set; }
        public DateTime? CRD { get; set; }
        public decimal? SumInstockQty { get; set; }
        public decimal? SumMOQty { get; set; }
        public decimal? SumAlreadyOffsetQty { get; set; }
        public decimal? SumOffsetQty { get; set; }
        public List<SizeAndQty> ListSizeAndQty { get; set; }
    }
}