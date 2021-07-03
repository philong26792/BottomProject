using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bottom_API.Models
{
    public class VM_WMSB_Material_Purchase
    {
        public string Factory_ID { get; set; }

        [Key]
        [Column(Order = 0)]
        public string Plan_No { get; set; }

        [Key]
        [Column(Order = 1)]
        public string Purchase_No { get; set; }

        [Key]
        [Column("Mat#", Order = 2)]
        public string Mat_ { get; set; }

        [Column("Mat#_Name")]
        public string Mat__Name { get; set; }

        public string Unit { get; set; }

        public string Model_No { get; set; }

        public string Model_Name { get; set; }

        public string Article { get; set; }

        public DateTime? Cutting_Date { get; set; }

        public DateTime? STC_Date { get; set; }

        public DateTime? Stockfiting_Date { get; set; }

        public DateTime? Assembly_Date { get; set; }

        [Column(TypeName = "date")]
        public DateTime? CRD { get; set; }

        public string Line_ASY { get; set; }

        public string Line_STF { get; set; }

        public string Tool_Type { get; set; }

        public string Supplier_No { get; set; }

        public string Supplier_Name { get; set; }

        public string Subcon_No { get; set; }

        public string Custmoer_Part { get; set; }

        public string Custmoer_Name { get; set; }

        public string Subcon_Name { get; set; }

        public string T3_Supplier { get; set; }

        public string T3_Supplier_Name { get; set; }

        public string Tool_ID { get; set; }

        [Key]
        [Column(Order = 3)]
        public string MO_Seq { get; set; }

        public string Process_Code { get; set; }
    }
}