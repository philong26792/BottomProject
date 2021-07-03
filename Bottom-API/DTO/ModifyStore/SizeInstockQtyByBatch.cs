using System.Collections.Generic;

namespace Bottom_API.DTO.ModifyStore
{
    public class SizeInstockQtyByBatch
    {
        public string MO_Seq {get;set;}
        public List<SizeInstockQty> dataDetail {get;set;}
    }
}