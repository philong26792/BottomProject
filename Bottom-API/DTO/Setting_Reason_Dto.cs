using System;

namespace Bottom_API.DTO
{
    public class Setting_Reason_Dto
    {
        public int Kind { get; set; }
        public string Reason_Code { get; set; }
        public string Kind_Name { get; set; }
        public string HP_Reason_Code { get; set; }
        public string Reason_Cname { get; set; }
        public string Reason_Ename { get; set; }
        public string Reason_Lname { get; set; }
        public string Trans_toHP { get; set; }
        public string Is_Shortage { get; set; }
        public DateTime Updated_Time { get; set; }
        public string Updated_By { get; set; }
    }
}