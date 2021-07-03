using System.Drawing;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bottom_API.DTO.Kanban
{
    public class Rack_Detail_T3T2_Dto
    {
        public string Rack { get; set; }
        public string Tooling { get; set; }
        public string Process_Name { get; set; }
        public string MO_No { get; set; }
        public string MO_Seq { get; set; }
        public string PoBatch
        {
            get
            {
                return MO_No + " " + MO_Seq;
            }
        }

        [Column("MTRL.Code")]
        public string MTRLCode { get; set; }

        [Column("Material Name")]
        public string MaterialName { get; set; }

        public string Unit { get; set; }

        public int? Qty { get; set; }

        public string T2_Supplier { get; set; }

        public string T3_Supplier { get; set; }

        [Column("ASL LINE")]
        public string ASLLine { get; set; }


        [Column("STF Start Date")]
        public string STFStartDate { get; set; }

        public int? TTL_PRS { get; set; }
        [Column("Customer Part")]
        public string CustomerPart { get; set; }
        public string Model { get; set; }
        public string Article { get; set; }
    }
}