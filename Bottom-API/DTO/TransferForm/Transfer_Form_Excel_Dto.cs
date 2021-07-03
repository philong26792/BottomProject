using System;

namespace Bottom_API.DTO.TransferForm
{
    public class Transfer_Form_Excel_Dto
    {
        public string Collect_Trans_No { get; set; }
        public string MO_No { get; set; }
        public string MO_Seq { get; set; }
        public string Material_ID { get; set; }
        public string Material_Name { get; set; }
        public string Article { get; set; }
        public string Model_Name { get; set; }
        public string Model_No { get; set; }
        public string Subcon_ID { get; set; }
        public string Subcon_Name { get; set; }
        public string T3_Supplier { get; set; }
        public string T3_Supplier_Name { get; set; }
        public string Custmoer_Part { get; set; }
        public string Custmoer_Name { get; set; }
        public decimal? Plan_Qty { get; set; }
        public decimal? Trans_Qty { get; set; }
        public decimal? Accumulated_Trans_Qty { get; set; }
        public string Release_Time { get; set; }
        public string Subject { get; set; }
        public string Trans_Qty_Of_All_Tool_Size { get; set; }
        public string PoBatch
        {
            get
            {
                return MO_No + MO_Seq;
            }
        }
        public string Process
        {
            get
            {
                return Subcon_ID + Subcon_Name;
            }
        }

        public string T3Supplier
        {
            get
            {
                return T3_Supplier + T3_Supplier_Name;
            }
        }
    }
}