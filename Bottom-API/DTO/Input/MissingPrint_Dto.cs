using System.Collections.Generic;

namespace Bottom_API.DTO.Input
{
    public class MissingPrint_Dto
    {
        public Material_Dto MaterialMissing { get; set; }
        public List<TransferLocationDetail_Dto> TransactionDetailByMissingNo {get; set;}
    }
}