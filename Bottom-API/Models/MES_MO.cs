using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bottom_API.Models
{
    public class MES_MO
    {
        [Key][Column(Order = 0)]
        public string Factory_ID {get;set;}
        public string Line_ID {get;set;}
        public string Line_ID_ASY {get;set;}
        public string Line_ID_STF {get;set;}
        public string Dept_ID {get;set;}
        public string Dept_ID_STC {get;set;}
        public string Dept_ID_STF {get;set;}
        public string MO_No {get;set;}
        public string MO_Seq {get;set;}
        [Key][Column(Order = 1)]
        public string Cycle_No {get;set;}
        public string Style_No {get;set;}
        public string Style_Name {get;set;}
        public string Color_No {get;set;}
        public int? Plan_Qty {get;set;}
        public int? UTN_Yield_Qty {get;set;}
        public int? UTN_Yield_Qty_STC {get;set;}
        public int? UTN_Yield_Qty_STF {get;set;}
        public int? UTN_Forward_Qty_STF {get;set;}
        public int? UTN_Forward_Qty_STC {get;set;}
        public int? UTN_Yield_Qty_STC_In {get;set;}
        public DateTime? Plan_Start_ASY {get;set;}
        public DateTime? Plan_End_ASY {get;set;}
        public DateTime? Plan_Start_STF {get;set;}
        public DateTime? Plan_End_STF {get;set;}
        public DateTime? Plan_Start_STC {get;set;}
        public DateTime? Plan_End_STC {get;set;}
        public DateTime? Plan_Start_CUT {get;set;}
        public DateTime? Plan_End_CUT {get;set;}
        public DateTime? Act_Start_ASY {get;set;}
        public DateTime? Act_End_ASY {get;set;}
        public DateTime? Act_Start_STF {get;set;}
        public DateTime? Act_End_STF {get;set;}
        public DateTime? Act_Start_STC {get;set;}
        public DateTime? Act_End_STC {get;set;}
        public DateTime? Act_Start_CUT {get;set;}
        public DateTime? Act_End_CUT {get;set;}
        public string Destination {get;set;}
        public DateTime? Comfirm_Date {get;set;}
        public string Prod_Season {get;set;}
        public string Top_Model {get;set;}
        public string Status {get;set;}
        public int? Received_Qty {get;set;}
        public DateTime? Update_Time {get;set;}
        public string Updated_By {get;set;}
    }
}