using System.Collections.Generic;
using Bottom_API.DTO.GenareQrCode;

namespace Bottom_API.DTO.Output
{
    public class OutputPrintQrCode_Dto
    {
        public QRCodeMainViewModel QrCodeModel { get; set; }
        public List<PackingListDetailViewModel> PackingListDetail { get; set; }
        public string RackLocation { get; set; }
    }
}