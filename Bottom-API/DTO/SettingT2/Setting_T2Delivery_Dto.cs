using System;
using System.Collections.Generic;

namespace Bottom_API.DTO.SettingT2
{
    public class Setting_T2Delivery_Dto
    {
        public string Factory_ID { get; set; }
        public string T2_Supplier_ID { get; set; }
        public string T2_Supplier_Name { get; set; }
        public string Input_Delivery { get; set; }
        public string Is_Valid { get; set; }
        public List<ReasonCodeInfo> Reasons { get; set; }
    }

    public class ReasonCodeInfo
    {
        public string Reason_Code { get; set; }
        public string Reason_Name { get; set; }
    }
}