using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Bottom_API._Repositories.Interfaces;
using Bottom_API._Services.Interfaces;
using Bottom_API.DTO;
using Microsoft.EntityFrameworkCore;
using Bottom_API.Helpers;
using Bottom_API.Models;
using Bottom_API.DTO.Output;
using Bottom_API.DTO.GenareQrCode;

namespace Bottom_API._Services.Services
{
    public class OutputService : IOutputService
    {
        private readonly IPackingListRepository _repoPackingList;
        private readonly IQRCodeMainRepository _repoQRCodeMain;
        private readonly IQRCodeDetailRepository _repoQRCodeDetail;
        private readonly ITransactionMainRepo _repoTransactionMain;
        private readonly ITransactionDetailRepo _repoTransactionDetail;
        private readonly IMaterialSheetSizeRepository _repoMaterialSheetSize;
        private readonly IRackLocationRepo _repoRackLocation;
        private readonly ICodeIDDetailRepo _repoCode;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _configMapper;
        private readonly IMaterialViewRepository _repoMaterialView;
        private readonly IPackingListDetailRepository _repoPackingListDetail;
        private readonly ITransferFormRepository _repoTransferForm;
        private readonly ICacheRepository _repoCache;

        public OutputService(
            IPackingListRepository repoPackingList,
            IQRCodeMainRepository repoQRCodeMain,
            IQRCodeDetailRepository repoQRCodeDetail,
            ITransactionMainRepo repoTransactionMain,
            ITransactionDetailRepo repoTransactionDetail,
            IMaterialSheetSizeRepository repoMaterialSheetSize,
            IRackLocationRepo repoRackLocation,
            ICodeIDDetailRepo repoCode,
            IMapper mapper, MapperConfiguration configMapper,
            IMaterialViewRepository repoMaterialView,
            IPackingListDetailRepository repoPackingListDetail,
            ITransferFormRepository repoTransferForm,
            ICacheRepository repoCache)
        {
            _configMapper = configMapper;
            _mapper = mapper;
            _repoQRCodeMain = repoQRCodeMain;
            _repoQRCodeDetail = repoQRCodeDetail;
            _repoPackingList = repoPackingList;
            _repoTransactionMain = repoTransactionMain;
            _repoTransactionDetail = repoTransactionDetail;
            _repoMaterialSheetSize = repoMaterialSheetSize;
            _repoRackLocation = repoRackLocation;
            _repoCode = repoCode;
            _repoMaterialView = repoMaterialView;
            _repoPackingListDetail = repoPackingListDetail;
            _repoTransferForm = repoTransferForm;
            _repoCache = repoCache;
        }

        public async Task<Output_Dto> GetByQrCodeId(string qrCodeId)
        {
            string message = "No data!";
            // bi???n qrcodeid l?? sheet no c???a b???ng materialsheetsize, d???a theo m?? ???? l???y ra listmaterialsheetsize l?? danh s??nh ????n output ra
            var listMaterialSheetSize = await _repoMaterialSheetSize.FindAll(x => x.Sheet_No.Trim() == qrCodeId.Trim()).ProjectTo<Material_Sheet_Size_Dto>(_configMapper).OrderBy(x => x.Tool_Size).ToListAsync();

            // ki???m tra xem Sheet_No ???? ???? output bao gi??? ch??a, n???u c?? r???i l?? ki???m tra ????n ???? ???? output ra h???t ch??a
            var listTransactionMainTypeOByPickupNo = await _repoTransactionMain.FindAll(x => x.Pickup_No.Trim() == qrCodeId && x.Transac_Type.Trim() == "O").ToListAsync();
            List<WMSB_Transaction_Detail> listTransactionDetailByBistTransactionMainTypeOByPickupNo = new List<WMSB_Transaction_Detail>();
            if (listTransactionMainTypeOByPickupNo.Count > 0)
            {
                foreach (var item in listTransactionMainTypeOByPickupNo)
                {
                    var tmp = await _repoTransactionDetail.FindAll(x => x.Transac_No == item.Transac_No).ToListAsync();
                    listTransactionDetailByBistTransactionMainTypeOByPickupNo.AddRange(tmp);
                }

                var totalTransQtyOutput = listTransactionDetailByBistTransactionMainTypeOByPickupNo.GroupBy(x => x.Order_Size )
                    .Select(y => new
                    {
                        OrderSize = y.Key,
                        TotalTransQty = (decimal)y.Sum(z => z.Trans_Qty)
                    }).ToList();

                foreach (var itemMaterialSheetSize in listMaterialSheetSize)
                {
                    var transQtyHadOutput = totalTransQtyOutput.Where(x => x.OrderSize == itemMaterialSheetSize.Order_Size).FirstOrDefault();
                    var qty = transQtyHadOutput != null ? transQtyHadOutput.TotalTransQty : 0;
                    itemMaterialSheetSize.Qty = (decimal)itemMaterialSheetSize.Qty - qty;
                }
            }
            // h???t ki???m tra xem Sheet_No ???? ???? output ra bao gi??? ch??a

            List<OutputMain_Dto> listOuput = new List<OutputMain_Dto>();
            var materialSheetSize = await _repoMaterialSheetSize.FindAll(x => x.Sheet_No.Trim() == qrCodeId.Trim()).FirstOrDefaultAsync();
            if (materialSheetSize != null)
            {
                List<WMSB_Transaction_Main> transactionModel = new List<WMSB_Transaction_Main>();
                // trong transaction main l???y theeo mo_no, material_id v?? batch
                transactionModel = await _repoTransactionMain.FindAll(x => x.MO_No.Trim() == materialSheetSize.Manno.Trim() && x.MO_Seq.Trim() == materialSheetSize.Batch.Trim() && x.Material_ID == materialSheetSize.Material_ID && x.Can_Move == "Y").ToListAsync();

                foreach (var item in transactionModel)
                {
                    var rackLocation = await _repoRackLocation.FindAll(x => x.Rack_Location.Trim() == item.Rack_Location.Trim()).FirstOrDefaultAsync();
                    // d??? li???u output c???n hi???n th???: trong b???ng tranasaction main, transaction detail ...
                    OutputMain_Dto output = new OutputMain_Dto();
                    output.Id = item.ID;
                    output.TransacNo = item.Transac_No;
                    output.QrCodeId = item.QRCode_ID.Trim();
                    output.QrCodeVersion = item.QRCode_Version;
                    output.PlanNo = item.MO_No.Trim();
                    output.Batch = item.MO_Seq;
                    output.MatId = item.Material_ID.Trim();
                    output.MatName = item.Material_Name.Trim();
                    output.PickupNo = qrCodeId;
                    output.RackLocation = item.Rack_Location;
                    output.InStockQty = _repoTransactionDetail.GetQtyByTransacNo(item.Transac_No);
                    output.TransOutQty = 0;
                    output.RemainingQty = _repoTransactionDetail.GetQtyByTransacNo(item.Transac_No);

                    var qrCodeModel = await _repoQRCodeMain.GetByQRCodeID(item.QRCode_ID);
                    var packingListModel = await _repoPackingList.GetByReceiveNo(qrCodeModel.Receive_No);
                    if (packingListModel != null)
                    {
                        output.SupplierName = packingListModel.Supplier_Name.Trim();
                        output.SupplierNo = packingListModel.Supplier_ID.Trim();
                        output.T3Supplier = packingListModel.T3_Supplier.Trim();
                        output.T3SupplierName = packingListModel.T3_Supplier_Name;
                        output.SubconId = packingListModel.Subcon_ID.Trim();
                        output.SubconName = packingListModel.Subcon_Name;
                    }

                    listOuput.Add(output);
                }
                if (transactionModel.Count() == 0)
                {
                    message = "Had been finishing outputted!";
                }
            }

            // ki???m tra ????n y??u c???u c?? b??? d?? toolsize n??o ko th?? lo???i ra
            // do h??? th???ng c??n li??n quan ?????n hp n??n c?? th??? toolsize b??? d?? ko output ???????c
            // l???y theo toolsize c???a b???ng transaction detail, c???a nh???ng ????n c???n output
            List<OutputTotalNeedQty_Dto> listNeedQtyByOutput = new List<OutputTotalNeedQty_Dto>();
            if (listOuput.Count() > 0)
            {
                var tmpTransacno = listOuput.FirstOrDefault().TransacNo;
                var tmpListDetail = await _repoTransactionDetail.FindAll(x => x.Transac_No.Trim() == tmpTransacno.Trim()).OrderBy(x => x.Tool_Size).ToListAsync();
                foreach (var itemTmpListDetail in tmpListDetail)
                {
                    var tmpMaterialSheetSize = listMaterialSheetSize.Where(x => x.Tool_Size.Trim() == itemTmpListDetail.Tool_Size.Trim() && x.Order_Size.Trim() == itemTmpListDetail.Order_Size.Trim() && x.Model_Size.Trim() == itemTmpListDetail.Model_Size.Trim())
                            .Select(x => new OutputTotalNeedQty_Dto
                            {
                                Model_Size = x.Model_Size,
                                Order_Size = x.Order_Size,
                                Qty = x.Qty,
                                Tool_Size = x.Tool_Size
                            }).FirstOrDefault();
                    if (tmpMaterialSheetSize != null)
                    {
                        listNeedQtyByOutput.Add(tmpMaterialSheetSize);
                    }
                }
                // ki???m tra ph???n t??? c?? trong transaction detail c?? m?? trong material sheet size ko c??, th?? th??m v??o material sheet size qty b???ng 0
                var toolSizeHaveInDetailNotHaveInSheet = tmpListDetail.Where(p => !listMaterialSheetSize.Any(x => x.Tool_Size.Trim() == p.Tool_Size.Trim() && x.Order_Size.Trim() == p.Order_Size.Trim() && x.Model_Size.Trim() == p.Model_Size.Trim())).ToList();
                if (toolSizeHaveInDetailNotHaveInSheet.Any())
                {
                    var tmpMaterialSheetSize = listMaterialSheetSize.First();
                    foreach (var item in toolSizeHaveInDetailNotHaveInSheet)
                    {
                        OutputTotalNeedQty_Dto tmp = new OutputTotalNeedQty_Dto();
                        tmp.Model_Size = item.Model_Size;
                        tmp.Tool_Size = item.Tool_Size;
                        tmp.Order_Size = item.Order_Size;
                        tmp.Qty = 0;
                        listNeedQtyByOutput.Add(tmp);
                    }
                }
            }
            //// h???t ??o???n ki???m tra toolsize

            // d??? li???u c???n l???y ra ????? hi???n th???: listoutputmain l?? trong b???ng transaction main v???i type b???ng I, R, M v?? listmaterialsheetsize l?? s??? l?????ng c???n output ra c???a ????n
            Output_Dto result = new Output_Dto();
            result.Outputs = listOuput.OrderBy(x => x.InStockQty).ToList();
            result.OutputTotalNeedQty = listNeedQtyByOutput.OrderBy(x => x.Tool_Size).ToList();
            result.Message = message;
            return result;
        }

        public async Task<Output_Dto> GetByQrCodeIdByCollectionTransferForm(string qrCodeId)
        {
            string message = "No data!";
            // bi???n qrcodeid l?? sheet no c???a b???ng materialsheetsize, d???a theo m?? ???? l???y ra listmaterialsheetsize l?? danh s??nh ????n output ra
            var listTransacNo = await _repoTransferForm.FindAll(x => x.Collect_Trans_No.Trim() == qrCodeId.Trim()).Select(x => x.Transac_No).ToListAsync();

            // d??? li???u output c???n hi???n th???: trong b???ng tranasaction main, transaction detail ...
            var listOutput = await GetListOutputInfo(listTransacNo, qrCodeId);
            var outputTotalNeedQty = await GetOutputNeedQty(listTransacNo);
            // d??? li???u c???n l???y ra ????? hi???n th???: listoutputmain l?? trong b???ng transaction main v???i type b???ng I, R, M v?? listmaterialsheetsize l?? s??? l?????ng c???n output ra c???a ????n
            Output_Dto result = new Output_Dto();
            result.Outputs = listOutput;
            result.OutputTotalNeedQty = outputTotalNeedQty;
            result.Message = message;
            return result;
        }

        public async Task<Output_Dto> GetByQrCodeIdBySortingForm(string qrCodeId)
        {
            string message = "No data!";
            // bi???n qrcodeid l?? sheet no c???a b???ng materialsheetsize, d???a theo m?? ???? l???y ra listmaterialsheetsize l?? danh s??nh ????n output ra
            var listTransacNo = await _repoTransactionMain.FindAll(x => x.QRCode_ID.Trim() == qrCodeId.Trim() && x.Can_Move == "Y").Select(x => x.Transac_No).ToListAsync();

            // d??? li???u output c???n hi???n th???: trong b???ng tranasaction main, transaction detail ...
            var listOutput = await GetListOutputInfo(listTransacNo, qrCodeId);
            var outputTotalNeedQty = await GetOutputNeedQty(listTransacNo);
            // d??? li???u c???n l???y ra ????? hi???n th???: listoutputmain l?? trong b???ng transaction main v???i type b???ng I, R, M v?? listmaterialsheetsize l?? s??? l?????ng c???n output ra c???a ????n
            Output_Dto result = new Output_Dto();
            result.Outputs = listOutput;
            result.OutputTotalNeedQty = outputTotalNeedQty;
            result.Message = message;
            return result;
        }
        public async Task<OutputDetail_Dto> GetDetailOutput(string transacNo)
        {
            var transactionMain = _repoTransactionMain.FindSingle(x => x.Transac_No.Trim() == transacNo.Trim());
            var transactionDetail = await _repoTransactionDetail.FindAll(x => x.Transac_No.Trim() == transactionMain.Transac_No.Trim()).ProjectTo<TransferLocationDetail_Dto>(_configMapper).OrderBy(x => x.Tool_Size).ToListAsync();

            // L???y ra nh???ng thu???c t??nh c???n
            OutputDetail_Dto result = new OutputDetail_Dto();
            result.Id = transactionMain.ID;
            result.QrCodeId = transactionMain.QRCode_ID;
            result.PlanNo = transactionMain.MO_No;
            result.MatId = transactionMain.Material_ID;
            result.MatName = transactionMain.Material_Name;
            result.Batch = transactionMain.MO_Seq;
            result.TransactionDetail = transactionDetail;

            return result;
        }

        public async Task<bool> SaveListOutput(List<OutputParam> outputParam, string updateBy)
        {
            DateTime timeNow = DateTime.Now;
            string outputSheetNo;
            do
            {
                string num = CodeUtility.RandomNumber(3);
                outputSheetNo = "OB" + DateTime.Now.ToString("yyyyMMdd") + num;// OB + 20200421 + 001
            } while (await _repoTransactionMain.CheckTranSheetNo(outputSheetNo));

            foreach (var itemt in outputParam)
            {
                // T??m ra TransactionMain theo id
                var transactionMain = _repoTransactionMain.FindSingle(x => x.ID == itemt.output.Id);
                transactionMain.Can_Move = "N"; // update transaction main c??: Can_Move th??nh N
                _repoTransactionMain.Update(transactionMain);

                // th??m transaction main type O
                WMSB_Transaction_Main modelTypeO = new WMSB_Transaction_Main();
                modelTypeO.Transac_Type = "O";
                modelTypeO.Can_Move = "N";
                modelTypeO.Transac_No = itemt.output.TransacNo;
                modelTypeO.Transac_Sheet_No = outputSheetNo;
                modelTypeO.Transacted_Qty = itemt.output.TransOutQty;
                modelTypeO.Pickup_No = itemt.output.PickupNo;
                modelTypeO.Transac_Time = timeNow;
                modelTypeO.Updated_Time = timeNow;
                modelTypeO.Updated_By = updateBy;
                modelTypeO.Missing_No = transactionMain.Missing_No;
                modelTypeO.Material_ID = transactionMain.Material_ID;
                modelTypeO.Material_Name = transactionMain.Material_Name;
                modelTypeO.Purchase_No = transactionMain.Purchase_No;
                modelTypeO.Rack_Location = null;// type O: racklocation r???ng
                modelTypeO.Purchase_Qty = transactionMain.Purchase_Qty;
                modelTypeO.QRCode_Version = transactionMain.QRCode_Version;
                modelTypeO.QRCode_ID = transactionMain.QRCode_ID;
                modelTypeO.MO_No = transactionMain.MO_No;
                modelTypeO.MO_Seq = transactionMain.MO_Seq;
                modelTypeO.Is_Transfer_Form = transactionMain.Is_Transfer_Form;
                modelTypeO.Reason_Code = transactionMain.Reason_Code;
                _repoTransactionMain.Add(modelTypeO);

                if(transactionMain.Is_Transfer_Form == "Y") {
                    // Update l???i valid_status c???a TransferForm c?? = 0;
                    var tranferFormOld = await _repoTransferForm.FindAll(x => x.Transac_No.Trim() == transactionMain.Transac_No.Trim()).FirstOrDefaultAsync();
                    if(tranferFormOld != null) {
                        tranferFormOld.Valid_Status = false;
                        // Th??m 1 record ??? b???ng TransferForm
                        var transferFormNew = new WMSB_Transfer_Form();
                        transferFormNew.Collec_Trans_Version = tranferFormOld.Collec_Trans_Version + 1;
                        transferFormNew.Transac_No = itemt.output.RemainingQty > 0 ? "R" + transactionMain.Transac_No : itemt.output.TransacNo;
                        transferFormNew.Collect_Trans_No = tranferFormOld.Collect_Trans_No;
                        transferFormNew.Valid_Status =  true;
                        transferFormNew.Generate_Time =  tranferFormOld.Generate_Time;
                        transferFormNew.Logmail_Info = tranferFormOld.Logmail_Info;
                        transferFormNew.Logmail_Release = tranferFormOld.Logmail_Release;
                        transferFormNew.T3_Supplier = tranferFormOld.T3_Supplier;
                        transferFormNew.T3_Supplier_Name = tranferFormOld.T3_Supplier_Name;
                        transferFormNew.Is_Release = tranferFormOld.Is_Release;
                        transferFormNew.Release_By = tranferFormOld.Release_By;
                        transferFormNew.Update_Time = DateTime.Now;
                        transferFormNew.Release_Time = tranferFormOld.Release_Time;
                        transferFormNew.Update_By = updateBy;
                        transferFormNew.MO_No = tranferFormOld.MO_No;
                        transferFormNew.MO_Seq = tranferFormOld.MO_Seq;
                        transferFormNew.Material_ID = tranferFormOld.Material_ID;
                        transferFormNew.Material_Name = tranferFormOld.Material_Name;
                        _repoTransferForm.Add(transferFormNew);
                    }
                }

                // Th??m transaction detail m???i theo type = o, d???a v??o transaction detail c???a transaction main c??
                foreach (var item in itemt.transactionDetail)
                {
                    item.ID = 0;// ID trong db l?? t??? t??ng: d???a v??o transaction detail c?? n??n th??m m???i g??n id b???ng 0, kh??ng c???n ph???i new h???t thu???c t??nh c???a ?????i t?????ng ra
                    item.Transac_No = itemt.output.TransacNo;
                    item.Updated_By = updateBy;
                    item.Updated_Time = timeNow;
                    var itemModel = _mapper.Map<WMSB_Transaction_Detail>(item);
                    _repoTransactionDetail.Add(itemModel);
                }

                // N???u output ra ch??a h???t th?? th??m transaction main type R, v?? transaction detail, th??m qrcode m???i v?? update version l??n
                if (itemt.output.RemainingQty > 0)
                {
                    //  th??m type R
                    var tmpQrcodeVersion = transactionMain.QRCode_Version + 1;
                    WMSB_Transaction_Main modelTypeR = new WMSB_Transaction_Main();
                    modelTypeR.Transac_Type = "R";
                    modelTypeR.Transac_No = "R" + transactionMain.Transac_No;
                    modelTypeR.Transac_Sheet_No = "R" + transactionMain.Transac_Sheet_No;
                    modelTypeR.Transacted_Qty = itemt.output.TransOutQty;
                    modelTypeR.Updated_By = updateBy;
                    modelTypeR.Updated_Time = timeNow;
                    modelTypeR.Missing_No = transactionMain.Missing_No;
                    modelTypeR.Material_ID = transactionMain.Material_ID;
                    modelTypeR.Material_Name = transactionMain.Material_Name;
                    modelTypeR.Purchase_No = transactionMain.Purchase_No;
                    modelTypeR.Rack_Location = transactionMain.Rack_Location;
                    modelTypeR.Purchase_Qty = transactionMain.Purchase_Qty;
                    modelTypeR.QRCode_Version = tmpQrcodeVersion;
                    modelTypeR.QRCode_ID = transactionMain.QRCode_ID;
                    modelTypeR.MO_No = transactionMain.MO_No;
                    modelTypeR.MO_Seq = transactionMain.MO_Seq;
                    modelTypeR.Can_Move = "Y";
                    modelTypeR.Is_Transfer_Form = transactionMain.Is_Transfer_Form;
                    modelTypeR.Reason_Code = transactionMain.Reason_Code;
                    modelTypeR.Transac_Time = timeNow;
                    _repoTransactionMain.Add(modelTypeR);

                    // th??m transaction main c??ng ph???i th??m transaction detail
                    foreach (var itemTypeR in itemt.transactionDetail)
                    {
                        itemTypeR.ID = 0;// ID trong db l?? t??? t??ng: d???a v??o transaction detail c?? n??n th??m m???i g??n id b???ng 0, kh??ng c???n ph???i new h???t thu???c t??nh c???a ?????i t?????ng ra
                        itemTypeR.Transac_No = modelTypeR.Transac_No;
                        itemTypeR.Updated_By = updateBy;
                        itemTypeR.Updated_Time = timeNow;
                        itemTypeR.Qty = itemTypeR.Instock_Qty;
                        itemTypeR.Trans_Qty = itemTypeR.Instock_Qty;
                        var itemModel = _mapper.Map<WMSB_Transaction_Detail>(itemTypeR);
                        _repoTransactionDetail.Add(itemModel);
                    }

                    // th??m qrcode m???i, n???u output ra ch??a h???t th?? th??m qrcode main m???i d???a v??o c??i c?? v?? update version l??n
                    var qrCodeMain = await _repoQRCodeMain.FindAll(x => x.QRCode_ID.Trim() == itemt.output.QrCodeId.Trim()).OrderByDescending(x => x.QRCode_Version).FirstOrDefaultAsync();
                    WMSB_QRCode_Main modelQrCodeMain = new WMSB_QRCode_Main();
                    modelQrCodeMain.QRCode_ID = qrCodeMain.QRCode_ID;
                    modelQrCodeMain.QRCode_Type = qrCodeMain.QRCode_Type;
                    modelQrCodeMain.Receive_No = qrCodeMain.Receive_No;
                    modelQrCodeMain.Valid_Status = "Y";
                    modelQrCodeMain.Is_Scanned = "Y";
                    modelQrCodeMain.Invalid_Date = qrCodeMain.Invalid_Date;
                    modelQrCodeMain.QRCode_Version = qrCodeMain.QRCode_Version + 1;
                    modelQrCodeMain.Updated_Time = timeNow;
                    modelQrCodeMain.Updated_By = updateBy;
                    _repoQRCodeMain.Add(modelQrCodeMain);

                    // Update cho QRCode c??, Valid_Status =N, Invalid_Date = Ng??y m?? t???o ra version m???i
                    qrCodeMain.Valid_Status = "N";
                    qrCodeMain.Invalid_Date = timeNow;
                    _repoQRCodeMain.Update(qrCodeMain);

                    // th??m qrcodedetail c???a qrcode m???i: th??m qrcode main c??ng ph???i th??m qrcode detail
                    var qrCodeDetails = await _repoQRCodeDetail.FindAll(x => x.QRCode_ID.Trim() == qrCodeMain.QRCode_ID.Trim() && x.QRCode_Version == qrCodeMain.QRCode_Version).ToListAsync();
                    foreach (var itemQrCodeDetail in qrCodeDetails)
                    {
                        itemQrCodeDetail.QID = 0;
                        itemQrCodeDetail.Updated_By = updateBy;
                        itemQrCodeDetail.Updated_Time = timeNow;
                        itemQrCodeDetail.QRCode_Version = modelQrCodeMain.QRCode_Version;
                        itemQrCodeDetail.Qty = itemt.transactionDetail.Where(x => x.Tool_Size == itemQrCodeDetail.Tool_Size && x.Order_Size == itemQrCodeDetail.Order_Size && x.Model_Size == itemQrCodeDetail.Model_Size).FirstOrDefault().Instock_Qty;
                        _repoQRCodeDetail.Add(itemQrCodeDetail);
                    }
                }
                // N???u QRCode ???? out h???t s??? l?????ng, c???n update cho n?? kh??ng c??n hi???u l???c 
                // ( ??? b???ng WMSB_QRCode_Main: UPDATE tr?????ng Valid_Status =???N???, Invalid_Date l?? ng??y Output h???t s??? l?????ng, 
                // ?????ng th???i c??ng update tr?????ng Update_Time & Update_By)
                else
                {
                    var qrCodeMain = await _repoQRCodeMain.FindAll(x => x.QRCode_ID.Trim() == itemt.output.QrCodeId.Trim()).OrderByDescending(x => x.QRCode_Version).FirstOrDefaultAsync();
                    qrCodeMain.Valid_Status = "N";
                    qrCodeMain.Invalid_Date = timeNow;
                    qrCodeMain.Updated_Time = timeNow;
                    qrCodeMain.Updated_By = updateBy;
                    _repoQRCodeMain.Update(qrCodeMain);
                }
            }

            // l??u Db
            return await _repoTransactionMain.SaveAll();
        }

        public async Task<List<OutputPrintQrCode_Dto>> PrintByQRCodeIDAgain(List<QrCodeIDVersion> ListParamPrintQrCodeAgain)
        {
            List<OutputPrintQrCode_Dto> listResult = new List<OutputPrintQrCode_Dto>();
            foreach (var paramPrintQrCodeAgain in ListParamPrintQrCodeAgain)
            {
                // h??m l???y ra th??ng tin qrcode main, m???y th??ng tin ch??nh
                var qrCodeModel = await GetQrCodeInfor(paramPrintQrCodeAgain.QRCode_ID, paramPrintQrCodeAgain.MO_Seq);

                // ??? d?????i l?? l???y ra th??ng tin chi ti???t c???a qrcode ????
                var qrCodeMan = _repoQRCodeMain.FindSingle(x => x.QRCode_ID.Trim() == paramPrintQrCodeAgain.QRCode_ID.Trim()
                                                && x.QRCode_Version == paramPrintQrCodeAgain.QRCode_Version);
                var transactionMainTypeR = await _repoTransactionMain.FindAll(x => x.QRCode_ID.Trim() == paramPrintQrCodeAgain.QRCode_ID.Trim()
                                                                && x.QRCode_Version == paramPrintQrCodeAgain.QRCode_Version && x.Can_Move == "Y").FirstOrDefaultAsync();
                var transactionDetailsTypeR = await _repoTransactionDetail.FindAll(x => x.Transac_No.Trim() == transactionMainTypeR.Transac_No.Trim()).ToListAsync();
                var transactionMain = await _repoTransactionMain.FindAll(x => x.QRCode_ID.Trim() == paramPrintQrCodeAgain.QRCode_ID.Trim()
                                                                && (x.Transac_Type == "I" || x.Transac_Type == "MG")).FirstOrDefaultAsync();
                var transactionDetails = await _repoTransactionDetail
                            .FindAll(x => x.Transac_No.Trim() == transactionMain.Transac_No.Trim()).ToListAsync();
                var listsPackingListDetail = await _repoPackingListDetail.FindAll(x => x.Receive_No.Trim() == qrCodeMan.Receive_No.Trim()).ToListAsync();

                //--------- ph???n xu???t th??m gi?? tr??? ???? output
                var transactionMainOutputs = await _repoTransactionMain.FindAll(x => x.Purchase_No.Trim() == transactionMain.Purchase_No.Trim()
                                                   && x.MO_No.Trim() == transactionMain.MO_No.Trim() && x.MO_Seq.Trim() == transactionMain.MO_Seq.Trim()
                                                   && x.Material_ID == transactionMain.Material_ID && x.Transac_Type.Trim() == "O").ToListAsync();
                List<WMSB_Transaction_Detail> transactionDetailOutputs = new List<WMSB_Transaction_Detail>();
                if (transactionMainOutputs.Count() > 0)
                {
                    foreach (var item in transactionMainOutputs)
                    {
                        var transactionDetailOutput = await _repoTransactionDetail.FindAll(x => x.Transac_No.Trim() == item.Transac_No.Trim()).ToListAsync();
                        transactionDetailOutputs.AddRange(transactionDetailOutput);
                    }
                }

                var transactionDetailOutputGroupBy = transactionDetailOutputs.GroupBy(x => new { x.Tool_Size, x.Order_Size }).Select(x => new
                {
                    OutQty = x.Sum(y => y.Trans_Qty),
                    Tool_Size = x.Key.Tool_Size,
                    Order_Size = x.Key.Order_Size
                }).ToList();
                //------------

                var packingListDetailModel = new List<PackingListDetailViewModel>();
                foreach (var item in listsPackingListDetail)
                {
                    var packingItem = new PackingListDetailViewModel();
                    packingItem.Receive_No = item.Receive_No;
                    packingItem.Order_Size = item.Order_Size;
                    packingItem.Model_Size = item.Model_Size;
                    packingItem.Tool_Size = item.Tool_Size;
                    packingItem.Spec_Size = item.Spec_Size;
                    packingItem.MO_Qty = item.MO_Qty;
                    packingItem.Purchase_Qty = item.Purchase_Qty;
                    foreach (var item1 in transactionDetails)
                    {
                        if (item1.Tool_Size.Trim() == item.Tool_Size.Trim() && item1.Order_Size.Trim() == item.Order_Size.Trim())
                        {
                            packingItem.Received_Qty = item1.Qty;
                            packingItem.Bal = item1.Untransac_Qty;
                        }
                    }
                    foreach (var item3 in transactionDetailsTypeR)
                    {
                        if (item3.Tool_Size.Trim() == item.Tool_Size.Trim() && item3.Order_Size.Trim() == item.Order_Size.Trim())
                        {
                            packingItem.Act = item3.Instock_Qty;
                        }
                    }
                    if (transactionDetailOutputGroupBy.Count() > 0)
                    {
                        foreach (var item2 in transactionDetailOutputGroupBy)
                        {
                            if (item2.Tool_Size.Trim() == item.Tool_Size.Trim() && item2.Order_Size.Trim() == item.Order_Size.Trim())
                            {
                                packingItem.OutQty = item2.OutQty;
                            }
                        }
                    }
                    else
                    {
                        packingItem.OutQty = 0;
                    }
                    packingListDetailModel.Add(packingItem);
                }
                packingListDetailModel = packingListDetailModel.OrderBy(x => x.Tool_Size).ThenBy(x => x.Order_Size).ToList();
                // List c??c Tool Size m?? c?? nhi???u Order Size trong b???ng Packing List Detail
                var toolSizeMoreOrderSize = packingListDetailModel.Where(x => x.Tool_Size.Trim() != x.Order_Size.Trim()).Select(x => x.Tool_Size).Distinct().ToList();
                if (toolSizeMoreOrderSize.Count() > 0)
                {
                    foreach (var itemToolSize in toolSizeMoreOrderSize)
                    {
                        var model1 = packingListDetailModel.Where(x => x.Tool_Size.Trim() == itemToolSize.Trim())
                            .GroupBy(x => x.Tool_Size).Select(x => new
                            {
                                Purchase_Qty = x.Sum(cl => cl.Purchase_Qty),
                                Received_Qty = x.Sum(cl => cl.Received_Qty),
                                Act = x.Sum(cl => cl.Act),
                                Bal = x.Sum(cl => cl.Bal),
                                OutQty = x.Sum(cl => cl.OutQty)
                            }).FirstOrDefault();
                        var packingListByToolSize = packingListDetailModel
                            .Where(x => x.Tool_Size.Trim() == itemToolSize.Trim()).ToList();
                        for (var i = 0; i < packingListByToolSize.Count; i++)
                        {
                            if (i != 0)
                            {
                                packingListByToolSize[i].Purchase_Qty = null;
                                packingListByToolSize[i].Received_Qty = null;
                                packingListByToolSize[i].Act = null;
                                packingListByToolSize[i].Bal = null;
                                packingListByToolSize[i].OutQty = null;
                            }
                            else
                            {
                                packingListByToolSize[i].Purchase_Qty = model1.Purchase_Qty;
                                packingListByToolSize[i].Received_Qty = model1.Received_Qty;
                                packingListByToolSize[i].Act = model1.Act;
                                packingListByToolSize[i].Bal = model1.Bal;
                                packingListByToolSize[i].OutQty = model1.OutQty;
                            }
                        }
                    }
                }

                OutputPrintQrCode_Dto result = new OutputPrintQrCode_Dto
                {
                    QrCodeModel = qrCodeModel,
                    PackingListDetail = packingListDetailModel,
                    RackLocation = transactionMainTypeR.Rack_Location
                };
                listResult.Add(result);
            }
            return listResult;
        }

        public async Task<QRCodeMainViewModel> GetQrCodeInfor(string qrCodeId, string moSeq)
        {
            var packingListQuery = _repoPackingList.FindAll(x => x.MO_Seq == moSeq);
            var qrCodeMainQuery = _repoQRCodeMain.FindAll(x => x.QRCode_ID.Trim() == qrCodeId.Trim());
            var qrCodeModel = await (from x in qrCodeMainQuery
                                        join y in packingListQuery
                                        on x.Receive_No.Trim() equals y.Receive_No.Trim()
                                        select new QRCodeMainViewModel()
                                        {
                                            QRCode_Type = x.QRCode_Type,
                                            QRCode_ID = x.QRCode_ID,
                                            T3_Supplier = y.T3_Supplier,
                                            T3_Supplier_Name = y.T3_Supplier_Name,
                                            Subcon_ID = y.Subcon_ID,
                                            Subcon_Name = y.Subcon_Name,
                                            MO_No = y.MO_No,
                                            Receive_No = x.Receive_No,
                                            Receive_Date = y.Receive_Date,
                                            MO_Seq = y.MO_Seq,
                                            Material_ID = y.Material_ID,
                                            Material_Name = y.Material_Name,
                                        }).Where(x => x.QRCode_ID.Trim() == qrCodeId.Trim() && x.MO_Seq == moSeq).FirstOrDefaultAsync();
            var cacheModel = await _repoCache.FindAll(x => x.MO_No == qrCodeModel.MO_No && x.MO_Seq == qrCodeModel.MO_Seq &&
                                                        x.Material_ID.Trim() == qrCodeModel.Material_ID.Trim()).FirstOrDefaultAsync();
            qrCodeModel.Line_ASY = cacheModel == null ? "" : cacheModel.Line_ID;
            qrCodeModel.Article =  cacheModel == null ? "" : cacheModel.Article;     
            qrCodeModel.Model_No =  cacheModel == null ? "" : cacheModel.Model_No;
            qrCodeModel.Model_Name =  cacheModel == null ? "" : cacheModel.Model_Name;
            qrCodeModel.Custmoer_Part =  cacheModel == null ? "" : cacheModel.Part_No;
            qrCodeModel.Custmoer_Name =  cacheModel == null ? "" : cacheModel.Part_Name;
            qrCodeModel.Stockfiting_Date =  cacheModel == null ? null : cacheModel.Plan_Start_STF;
            qrCodeModel.Assembly_Date =  cacheModel == null ? null : cacheModel.Plan_Start_ASY;
            qrCodeModel.CRD =  cacheModel == null ? null : cacheModel.CRD; 
            return qrCodeModel;
        }

        #region Fuction
        public async Task<List<OutputMain_Dto>> GetListOutputInfo(List<string> listTransacNo, string qrCodeId)
        {
            var qrCodeMain = _repoQRCodeMain.FindAll();
            var packingList = _repoPackingList.FindAll();
            var listTransactionMainByTransferForm = _repoTransactionMain.FindAll(x => listTransacNo.Contains(x.Transac_No) && x.Can_Move == "Y");
            // d??? li???u output c???n hi???n th???: trong b???ng tranasaction main, transaction detail ...
            var listOutput = await listTransactionMainByTransferForm
            .GroupJoin(
                    qrCodeMain,
                    x => new { QRCode_ID = x.QRCode_ID, QRCode_Version = x.QRCode_Version },
                    y => new { QRCode_ID = y.QRCode_ID, QRCode_Version = y.QRCode_Version },
                    (x, y) => new { TransactionMain = x, QrCodeMain = y })
                .SelectMany(
                    x => x.QrCodeMain.DefaultIfEmpty(),
                    (x, y) => new { TransactionMain = x.TransactionMain, QrCodeMain = y })
                    .GroupJoin(
                        packingList,
                        x => x.QrCodeMain.Receive_No,
                        y => y.Receive_No,
                        (x, y) => new { TransactionMainQrCodeMain = x, PackingList = y })
                        .SelectMany(
                            x => x.PackingList.DefaultIfEmpty(),
                            (x, y) => new OutputMain_Dto
                            {
                                Id = x.TransactionMainQrCodeMain.TransactionMain.ID,
                                TransacNo = x.TransactionMainQrCodeMain.TransactionMain.Transac_No,
                                QrCodeId = x.TransactionMainQrCodeMain.TransactionMain.QRCode_ID.Trim(),
                                QrCodeVersion = x.TransactionMainQrCodeMain.TransactionMain.QRCode_Version,
                                PlanNo = x.TransactionMainQrCodeMain.TransactionMain.MO_No.Trim(),
                                Batch = x.TransactionMainQrCodeMain.TransactionMain.MO_Seq,
                                MatId = x.TransactionMainQrCodeMain.TransactionMain.Material_ID.Trim(),
                                MatName = x.TransactionMainQrCodeMain.TransactionMain.Material_Name.Trim(),
                                PickupNo = qrCodeId,
                                SubconId = y.Subcon_ID,
                                SubconName = y.Subcon_Name,
                                T3Supplier = y.T3_Supplier,
                                T3SupplierName = y.T3_Supplier_Name,
                                SupplierName = y.Supplier_Name,
                                SupplierNo = y.Supplier_ID,
                                RackLocation = x.TransactionMainQrCodeMain.TransactionMain.Rack_Location,
                                InStockQty = 0,
                                // InStockQty = _repoTransactionDetail.GetQtyByTransacNo(x.TransactionMainQrCodeMain.TransactionMain.Transac_No),
                                TransOutQty = 0
                            }).OrderBy(x => x.InStockQty).ToListAsync();

            foreach (var item in listOutput)
            {
                item.InStockQty = _repoTransactionDetail.GetQtyByTransacNo(item.TransacNo);
                item.RemainingQty = item.InStockQty;
            }

            return listOutput;
        }

        public async Task<List<OutputTotalNeedQty_Dto>> GetOutputNeedQty(List<string> listTransacNo)
        {
            var listTransactionDetailByTransacNo = await _repoTransactionDetail.FindAll(x => listTransacNo.Contains(x.Transac_No))
                    .Select(x => new { x.Tool_Size, x.Order_Size, x.Model_Size, x.Instock_Qty }).ToListAsync();
            var outputTotalNeedQty = listTransactionDetailByTransacNo.GroupBy(x => new { x.Tool_Size, x.Order_Size, x.Model_Size })
                        .Select(x => new OutputTotalNeedQty_Dto
                        {
                            Model_Size = x.Key.Model_Size,
                            Order_Size = x.Key.Order_Size,
                            Tool_Size = x.Key.Tool_Size,
                            Qty = x.Sum(x => x.Instock_Qty)
                        }).OrderBy(x => x.Tool_Size).ToList();
            return outputTotalNeedQty;
        }

        #endregion
    }
}