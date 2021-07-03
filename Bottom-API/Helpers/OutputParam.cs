using System.Collections.Generic;
using Bottom_API.DTO;
using Bottom_API.DTO.Output;

namespace Bottom_API.Helpers
{
    public class OutputParam
    {
        public OutputMain_Dto output { get; set; }
        public List<TransferLocationDetail_Dto> transactionDetail { get; set; }
    }
}