using System;

namespace Bottom_API.DTO.TransferForm
{
    public class Transfer_Form_Generate_Dto
    {
        public string Factory_ID { get; set; }
        public string Collect_Trans_No { get; set; }
        public string Is_Release { get; set; }
        public string Transac_No { get; set; }
        public string T3_Supplier { get; set; }
        public string T3_Supplier_Name { get; set; }
        public string Supplier_ID { get; set; }
        public string Supplier_Name { get; set; }
        public string Subcon_ID { get; set; }
        public string Article { get; set; }
        public string QRCode_ID { get; set; }
        public string MO_No { get; set; }
        public string Material_ID { get; set; }
        public string Material_Name { get; set; }
        public string MO_Seq { get; set; }
        public string Purchase_No {get;set;}
        public string Custmoer_Part { get; set; }
        public string Custmoer_Name { get; set; }
        public string Subcon_Name { get; set; }
        public decimal? Purchase_Qty { get; set; }
        public decimal? Transacted_Qty { get; set; }
        public string Email { get; set; }
        public DateTime? Release_Time { get; set; }
        public DateTime? Update_Time { get; set; }
        public string Model_Name { get; set; }
        public string Model_No { get; set; }
        public DateTime? Transac_Time { get; set; }
        public int? Logmail_Info { get; set; }
        public int? Logmail_Release { get; set; }
        public int? CountLogEmail { get; set; }
    }
}