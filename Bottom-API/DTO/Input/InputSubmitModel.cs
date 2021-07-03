using System.Collections.Generic;

namespace Bottom_API.DTO.Input
{
    public class InputSubmitModel
    {
        public List<Transaction_Detail_Dto> TransactionList {get;set;}
        public List<string> InputNoList{get;set;}
    }
}