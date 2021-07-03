using AutoMapper;
using Bottom_API.DTO;
using Bottom_API.DTO.Output;
using Bottom_API.DTO.Receiving;
using Bottom_API.DTO.SettingMail;
using Bottom_API.DTO.SettingT2;
using Bottom_API.Models;

namespace Bottom_API.Helpers.AutoMapper
{
    public class DtoToEfMappingProfile : Profile
    {
        public DtoToEfMappingProfile()
        {
            CreateMap<QRCode_Main_Dto, WMSB_QRCode_Main>();
            CreateMap<Packing_List_Dto, WMSB_Packing_List>();
            CreateMap<Packing_List_Detail_Dto, WMSB_PackingList_Detail>();
            CreateMap<RackLocation_Main_Dto, WMSB_RackLocation_Main>();
            CreateMap<QRCode_Detail_Dto, WMSB_QRCode_Detail>();
            CreateMap<Receiving_Dto, WMSB_Material_Purchase>();
            CreateMap<Receiving_Dto, WMSB_Material_Missing>();
            CreateMap<Material_Sheet_Size_Dto, WMSB_Material_Sheet_Size>();
            CreateMap<Transaction_Main_Dto, WMSB_Transaction_Main>();
            CreateMap<TransferLocationDetail_Dto, WMSB_Transaction_Detail>();
            CreateMap<Setting_Mail_Supplier_Dto, WMSB_Setting_Supplier>();
            CreateMap<Setting_Reason_Dto, WMSB_Setting_Reason>();
            CreateMap<Setting_T2Delivery_Dto, WMSB_Setting_T2Delivery>();
            CreateMap<Release_DeliveryNo_Dto, WMSB_Release_DeliveryNo>();
        }
    }
}