using System;

namespace Bottom_API.Models
{
    public class NSP_REPORT_MAIN
    {
        public DateTime? STF_Date { get; set; }
        public string Cell { get; set; }
        public string MO_No { get; set; }
        public string MO_Seq { get; set; }
        public string Model_Name { get; set; }
        public string Article { get; set; }
        public decimal? Prs { get; set; }
        public int status { get; set; }
        public decimal? Rev_Qty { get; set; }
        public decimal? Stk_Qty { get; set; }

    }
}