using System.Collections.Generic;

namespace Bottom_API.DTO.TransferLocation
{
    public class HistoryDetail_Dto
    {
        public Transaction_Main_Dto transactionMain { get; set; }
        public List<TransferLocationDetail_Dto> listTransactionDetail { get; set; }
    }
}