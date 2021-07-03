using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Bottom_API._Repositories.Interfaces;
using Bottom_API._Services.Interfaces;
using Bottom_API.DTO;
using Bottom_API.DTO.GenareQrCode;
using Bottom_API.Helpers;
using Bottom_API.Models;
using LinqKit;
using Microsoft.EntityFrameworkCore;

namespace Bottom_API._Services.Services
{
    public class QRCodeMainService : IQRCodeMainService
    {
        private readonly IQRCodeMainRepository _repoQrcode;
        private readonly IPackingListRepository _repoPacking;
        private readonly IPackingListDetailRepository _repoPackingDetail;
        private readonly IQRCodeDetailRepository _repoQrCodeDetail;
        private readonly ITransactionDetailRepo _repoTransactionDetail;
        private readonly ITransactionMainRepo _repoTransactionMain;
        private readonly IMapper _mapper;
        public QRCodeMainService(IQRCodeMainRepository repoQrcode,
                                    IPackingListRepository repoPacking,
                                    IPackingListDetailRepository repoPackingDetail,
                                    IQRCodeDetailRepository repoQrCodeDetail,
                                    IMapper mapper,
                                    ITransactionDetailRepo repoTransactionDetail,
                                    ITransactionMainRepo repoTransactionMain)
        {
            _repoQrcode = repoQrcode;
            _repoPacking = repoPacking;
            _repoPackingDetail = repoPackingDetail;
            _repoQrCodeDetail = repoQrCodeDetail;
            _mapper = mapper;
            _repoTransactionDetail = repoTransactionDetail;
            _repoTransactionMain = repoTransactionMain;
        }

        public async Task<bool> AddListQRCode(List<string> listReceiveNo, string updateBy)
        {
            listReceiveNo = listReceiveNo.Select(x => x.Trim()).ToList();
            var packingLists = await _repoPacking.FindAll(x => listReceiveNo.Contains(x.Receive_No.Trim())).ToListAsync();
            var checkCreate = true;
            var listQrCode = new List<string>();
            foreach (var item in listReceiveNo)
            {
                // Tạo QrCodeMain để thêm vào database
                var qrCodeDto = new QRCode_Main_Dto();
                var packing = packingLists.Where(x => x.Receive_No.Trim() == item).FirstOrDefault();
                // Nếu Generated_QrCode khác Y thì mới Tạo QrCode
                if (packing.Generated_QRCode != "Y")
                {
                    packing.Generated_QRCode = "Y";
                    string qrCodeId = "";
                    if (packing.Sheet_Type.Trim() == "R" || packing.Sheet_Type.Trim() == "M")
                    {
                        do
                        {
                            var po = packing.MO_No.Trim().Length == 9 ? packing.MO_No.Trim() + "Z" : packing.MO_No.Trim();
                            string so = CodeUtility.RandomNumber(3);
                            qrCodeId = "A" + po + so + CodeUtility.RandomStringUpper(1);
                        } while (await this.CheckQrCodeID(qrCodeId) || listQrCode.Contains(qrCodeId));
                        listQrCode.Add(qrCodeId);
                    }
                    else if (packing.Sheet_Type.Trim() == "B")
                    {
                        do
                        {
                            var po = packing.MO_No.Trim().Length == 9 ? packing.MO_No.Trim() + "Z" : packing.MO_No.Trim();
                            string so = CodeUtility.RandomNumber(3);
                            qrCodeId = "B" + po + so + CodeUtility.RandomStringUpper(1);
                        } while (await this.CheckQrCodeID(qrCodeId) || listQrCode.Contains(qrCodeId));
                        listQrCode.Add(qrCodeId);
                    }
                    qrCodeDto.QRCode_ID = qrCodeId;
                    qrCodeDto.Receive_No = packing.Receive_No.Trim();
                    qrCodeDto.QRCode_Version = 1;
                    qrCodeDto.Valid_Status = "Y";
                    qrCodeDto.Is_Scanned = "N";
                    qrCodeDto.QRCode_Type = packing.Sheet_Type.Trim();
                    qrCodeDto.Updated_By = updateBy;
                    await _repoPacking.SaveAll();
                    var qrCodeMain = _mapper.Map<WMSB_QRCode_Main>(qrCodeDto);
                    _repoQrcode.Add(qrCodeMain);

                    // Tạo QrCodeDetail để thêm vào database
                    var listPackingDetail = await _repoPackingDetail.FindAll(x => x.Receive_No.Trim() == item).ToListAsync();
                    foreach (var packingItem in listPackingDetail)
                    {
                        var qrCodeDetailDto = new QRCode_Detail_Dto();
                        qrCodeDetailDto.QRCode_ID = qrCodeId;
                        qrCodeDetailDto.QRCode_Version = 1;
                        qrCodeDetailDto.Order_Size = packingItem.Order_Size;
                        qrCodeDetailDto.Model_Size = packingItem.Model_Size;
                        qrCodeDetailDto.Tool_Size = packingItem.Tool_Size;
                        qrCodeDetailDto.Spec_Size = packingItem.Spec_Size;
                        qrCodeDetailDto.Qty = packingItem.Received_Qty;
                        qrCodeDetailDto.Updated_By = updateBy;
                        var qrCodeDetail = _mapper.Map<WMSB_QRCode_Detail>(qrCodeDetailDto);
                        _repoQrCodeDetail.Add(qrCodeDetail);
                        if (!await _repoQrCodeDetail.SaveAll())
                        {
                            checkCreate = false;
                            break;
                        }
                    }
                }
            }
            await _repoQrcode.SaveAll();
            return checkCreate;
        }
        public async Task<PagedList<QRCodeMainViewModel>> Search(PaginationParams param, FilterQrCodeParam filterParam)
        {
            var pred_Packing_List = PredicateBuilder.New<WMSB_Packing_List>(true);
            if (filterParam.From_Date != null && filterParam.To_Date != null)
            {
                pred_Packing_List.And(x => x.Receive_Date >= Convert.ToDateTime(filterParam.From_Date) &&
                    x.Receive_Date <= Convert.ToDateTime(filterParam.To_Date + " 23:59:59.997"));
            }
            if (!String.IsNullOrEmpty(filterParam.MO_No))
            {
                pred_Packing_List.And(x => x.MO_No.Trim() == filterParam.MO_No.Trim());
            }
            var listPackingList = _repoPacking.FindAll(pred_Packing_List);
            var listQrCodeMain = _repoQrcode.FindAll(x => x.Is_Scanned.Trim() == "N");
            var listQrCodeModel = ( from x in listQrCodeMain
                                    join y in listPackingList
                                    on x.Receive_No.Trim() equals y.Receive_No.Trim()
                                select new QRCodeMainViewModel()
                                    {
                                        QRCode_ID = x.QRCode_ID,
                                        MO_No = y.MO_No,
                                        QRCode_Version = x.QRCode_Version,
                                        Receive_No = x.Receive_No,
                                        Receive_Date = y.Receive_Date,
                                        Supplier_ID = y.Supplier_ID,
                                        Supplier_Name = y.Supplier_Name,
                                        T3_Supplier = y.T3_Supplier,
                                        T3_Supplier_Name = y.T3_Supplier_Name,
                                        Subcon_ID = y.Subcon_ID,
                                        Subcon_Name = y.Subcon_Name,
                                        Model_Name = y.Model_Name,
                                        Model_No = y.Model_No,
                                        Article = y.Article,
                                        MO_Seq = y.MO_Seq,
                                        Material_ID = y.Material_ID,
                                        Material_Name = y.Material_Name
                                    }).Distinct().OrderByDescending(x => x.Receive_Date);
            return await PagedList<QRCodeMainViewModel>.CreateAsync(listQrCodeModel, param.PageNumber, param.PageSize, false);
        }
        public async Task<bool> CheckQrCodeID(string qrCodeID)
        {
            var qrCodeMainModel = await _repoQrcode.FindAll(x => x.QRCode_ID.Trim() == qrCodeID.Trim()).FirstOrDefaultAsync();
            if (qrCodeMainModel != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}