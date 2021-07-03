using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Bottom_API._Repositories.Interfaces;
using Bottom_API._Services.Interfaces;
using Bottom_API.DTO;
using Bottom_API.DTO.TransferLocation;
using Bottom_API.Helpers;
using Bottom_API.Models;
using LinqKit;
using Microsoft.EntityFrameworkCore;

namespace Bottom_API._Services.Services
{
    public class TransferLocationService : ITransferLocationService
    {
        private readonly ITransactionMainRepo _repoTransactionMain;
        private readonly ITransactionDetailRepo _repoTransactionDetail;
        private readonly IQRCodeMainRepository _repoQRCodeMain;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _configMapper;
        private readonly IPackingListRepository _repoPackingList;
        private readonly IMaterialViewRepository _repoMaterialView;
        private readonly IRackLocationRepo _repoRackLocation;
        private readonly ITransferFormRepository _repoTransferForm;

        public TransferLocationService(
            ITransactionMainRepo repoTransactionMain,
            ITransactionDetailRepo repoTransactionDetail,
            IQRCodeMainRepository repoQRCodeMain,
            IMapper mapper,
            MapperConfiguration configMapper,
            IPackingListRepository repoPackingList,
            IMaterialViewRepository repoMaterialView,
            IRackLocationRepo repoRackLocation,
            ITransferFormRepository repoTransferForm)
        {
            _configMapper = configMapper;
            _repoPackingList = repoPackingList;
            _mapper = mapper;
            _repoTransactionMain = repoTransactionMain;
            _repoTransactionDetail = repoTransactionDetail;
            _repoQRCodeMain = repoQRCodeMain;
            _repoMaterialView = repoMaterialView;
            _repoRackLocation = repoRackLocation;
            _repoTransferForm = repoTransferForm;
        }
        public async Task<TransferLocation_Dto> GetByQrCodeId(object qrCodeId, string updateBy)
        {
            TransferLocation_Dto model = new TransferLocation_Dto();
            // Lấy ra TransactionMain cùng QRCode_ID và Can_Move == "Y" và QRCode_Version mới nhất
            var transactionModel = await _repoTransactionMain.FindAll(x => x.QRCode_ID.Trim() == qrCodeId.ToString().Trim() && x.Can_Move == "Y").OrderByDescending(x => x.QRCode_Version).FirstOrDefaultAsync();
            var qrCodeModel = await _repoQRCodeMain.GetByQRCodeID(qrCodeId);
            if (transactionModel != null)
            {
                var packingListModel = await _repoPackingList.FindAll(x => x.MO_No.Trim() == transactionModel.MO_No.Trim()).FirstOrDefaultAsync();
                var materialPurchaseModel = await _repoMaterialView.FindAll(x => x.Plan_No.Trim() == transactionModel.MO_No.Trim() && x.Purchase_No.Trim() == transactionModel.Purchase_No.Trim() && x.Mat_.Trim() == transactionModel.Material_ID.Trim()).FirstOrDefaultAsync();
                model.Id = transactionModel.ID;
                model.QrCodeId = transactionModel.QRCode_ID.Trim();
                model.TransferNo = "TB" + DateTime.Now.ToString("yyyyMMdd") + "001";
                model.PlanNo = transactionModel.MO_No.Trim();
                model.ReceiveNo = qrCodeModel.Receive_No.Trim();
                model.Batch = transactionModel.MO_Seq;
                model.MatId = transactionModel.Material_ID.Trim();
                model.MatName = transactionModel.Material_Name.Trim();
                model.FromLocation = transactionModel.Rack_Location.Trim();
                model.Qty = _repoTransactionDetail.GetQtyByTransacNo(transactionModel.Transac_No);
                model.UpdateBy = updateBy;
                model.TransacTime = DateTime.Now;
                model.ModelName = packingListModel.Model_Name;
                model.ModelNo = packingListModel.Model_No;
                model.Article = packingListModel.Article;
                model.CustmoerPart = materialPurchaseModel.Custmoer_Part;
                model.CustmoerName = materialPurchaseModel.Custmoer_Name;
            }

            return model;
        }

        public async Task<HistoryDetail_Dto> GetDetailTransaction(string transacNo)
        {
            var transactionMain = _repoTransactionMain.FindSingle(x => x.Transac_No.Trim() == transacNo.Trim());

            // lấy ra tất cả transaction detail dựa vào transacno
            List<TransferLocationDetail_Dto> transactionDetail = new List<TransferLocationDetail_Dto>();

            var model = _repoTransactionDetail.FindAll();
            if (transactionMain.Transac_Type.Trim() == "I" || transactionMain.Transac_Type.Trim() == "M")
            {
                transactionDetail = await model.Where(x => x.Transac_No.Trim() == transacNo.Trim()).Select(x => new TransferLocationDetail_Dto
                {
                    Order_Size = x.Order_Size,
                    Model_Size = x.Model_Size,
                    Tool_Size = x.Tool_Size,
                    Qty = x.Instock_Qty
                }).OrderBy(x => x.Tool_Size).ToListAsync();
            }
            else if (transactionMain.Transac_Type.Trim() == "O")
            {
                transactionDetail = await model.Where(x => x.Transac_No.Trim() == transacNo.Trim()).Select(x => new TransferLocationDetail_Dto
                {
                    Order_Size = x.Order_Size,
                    Model_Size = x.Model_Size,
                    Tool_Size = x.Tool_Size,
                    Qty = x.Trans_Qty
                }).OrderBy(x => x.Tool_Size).ToListAsync();
            }
            else if (transactionMain.Transac_Type.Trim().Contains("U"))
            {
                var transaactionMainNew = await _repoTransactionMain.FindAll(x => x.QRCode_ID.Trim() == transactionMain.QRCode_ID.Trim()
                                        && x.QRCode_Version == transactionMain.QRCode_Version + 1).Select(x => x.Transac_No).FirstOrDefaultAsync();
                transactionDetail = await model.Where(x => x.Transac_No.Trim() == transacNo.Trim())
                        .Join(model.Where(x => x.Transac_No.Trim() == transaactionMainNew.Trim()),
                        x => new { Tool_Size = x.Tool_Size.Trim(), Model_Size = x.Model_Size.Trim(), Order_Size = x.Order_Size.Trim() },
                        y => new { Tool_Size = y.Tool_Size.Trim(), Model_Size = y.Model_Size.Trim(), Order_Size = y.Order_Size.Trim() },
                        (x, y) => new TransferLocationDetail_Dto
                        {
                            Order_Size = x.Order_Size,
                            Model_Size = x.Model_Size,
                            Tool_Size = x.Tool_Size,
                            Qty = x.Instock_Qty - y.Instock_Qty
                        }).OrderBy(x => x.Tool_Size).ToListAsync();
            }
            else if (transactionMain.Transac_Type.Trim().Contains("T"))
            {
                var transaactionMainNew = await _repoTransactionMain.FindAll(x => x.QRCode_ID.Trim() == transactionMain.QRCode_ID.Trim()
                                        && x.QRCode_Version == transactionMain.QRCode_Version + 1).Select(x => x.Transac_No).FirstOrDefaultAsync();
                transactionDetail = await model.Where(x => x.Transac_No.Trim() == transacNo.Trim())
                        .Join(model.Where(x => x.Transac_No.Trim() == transaactionMainNew.Trim()),
                        x => new { Tool_Size = x.Tool_Size.Trim(), Model_Size = x.Model_Size.Trim(), Order_Size = x.Order_Size.Trim() },
                        y => new { Tool_Size = y.Tool_Size.Trim(), Model_Size = y.Model_Size.Trim(), Order_Size = y.Order_Size.Trim() },
                        (x, y) => new TransferLocationDetail_Dto
                        {
                            Order_Size = x.Order_Size,
                            Model_Size = x.Model_Size,
                            Tool_Size = x.Tool_Size,
                            Qty = y.Instock_Qty - x.Instock_Qty
                        }).OrderBy(x => x.Tool_Size).ToListAsync();
            }
            HistoryDetail_Dto data = new HistoryDetail_Dto();
            data.listTransactionDetail = transactionDetail;
            data.transactionMain = _mapper.Map<Transaction_Main_Dto>(transactionMain);

            return data;
        }

        public async Task<HistoryDetail_Dto> GetDetailTransactionForOutput(string transacNo)
        {
            var transactionMain = _repoTransactionMain.FindSingle(x => x.Transac_No.Trim() == transacNo.Trim());

            // lấy ra tất cả transaction detail dựa vào transacno
            var model = _repoTransactionDetail.FindAll(x => x.Transac_No.Trim() == transacNo.Trim());
            var transactionDetail = await model.ProjectTo<TransferLocationDetail_Dto>(_configMapper).OrderBy(x => x.Tool_Size).ToListAsync();
            HistoryDetail_Dto data = new HistoryDetail_Dto();
            data.listTransactionDetail = transactionDetail;
            data.transactionMain = _mapper.Map<Transaction_Main_Dto>(transactionMain);

            return data;
        }

        public async Task<PagedList<TransferLocation_Dto>> Search(TransferLocationParam transferLocationParam, PaginationParams paginationParams)
        {
            var pred_Transaction_Main = PredicateBuilder.New<WMSB_Transaction_Main>(true);
            var packingList = _repoPackingList.FindAll();
            pred_Transaction_Main.And(x => x.Transac_Type.Trim() != "R" && x.Transac_Type.Trim() != "MG");
            if (transferLocationParam.FromDate != "" && transferLocationParam.ToDate != "") {
                DateTime t1 = Convert.ToDateTime(transferLocationParam.FromDate + " 00:00:00.000");
                DateTime t2 = DateTime.Parse(transferLocationParam.ToDate + " 23:59:59");// ép về kiểu ngày truyền vào và giờ là 23h59'
                pred_Transaction_Main.And(x => x.Transac_Time >= t1 && x.Transac_Time <= t2);
            }
            if ((transferLocationParam.Status.Trim() == "I" || transferLocationParam.Status.Trim() == "O")) {
                pred_Transaction_Main.And(x => x.Transac_Type.Trim() == transferLocationParam.Status.Trim() && x.Reason_Code != "ZZZ");
            }
            if(transferLocationParam.Status.Trim() == "M") {
                pred_Transaction_Main.And(x => x.Transac_Type == "M");
            }
            if (transferLocationParam.Status.Trim() == "U") {
                pred_Transaction_Main.And(x => x.Transac_Type.Trim().Contains(transferLocationParam.Status.Trim()) || 
                                                (x.Transac_Type == "O" && x.Reason_Code == "ZZZ"));
            }
            if (transferLocationParam.Status.Trim() == "T") {
                pred_Transaction_Main.And(x => x.Transac_Type.Trim().Contains(transferLocationParam.Status.Trim()) || 
                                        (x.Transac_Type == "I" && x.Reason_Code == "ZZZ"));
            }
            if (!String.IsNullOrEmpty(transferLocationParam.MoNo)) {
                pred_Transaction_Main.And(x => x.MO_No.Trim() == transferLocationParam.MoNo.Trim());
            }
            if (!String.IsNullOrEmpty(transferLocationParam.MaterialId)) {
                pred_Transaction_Main.And(x => x.Material_ID.Trim() == transferLocationParam.MaterialId.Trim());
            }
            var model = _repoTransactionMain.FindAll(pred_Transaction_Main).ProjectTo<Transaction_Main_Dto>(_configMapper);
            if (!String.IsNullOrEmpty(transferLocationParam.Supplier_ID) && transferLocationParam.Supplier_ID.Trim() != "All") {
                packingList = packingList.Where(x => x.Supplier_ID.Trim() == transferLocationParam.Supplier_ID.Trim());

            }
            var packingListQuery = packingList.Select(x => new { x.MO_No, x.Purchase_No, x.MO_Seq, x.Material_ID }).Distinct();
            var data = (from a in model
                        join b in packingListQuery on
                        new { Mono = a.MO_No.Trim(), PurchaseNo = a.Purchase_No.Trim(), Batch = a.MO_Seq, MaterialID = a.Material_ID.Trim() }
                        equals new { Mono = b.MO_No.Trim(), PurchaseNo = b.Purchase_No.Trim(), Batch = b.MO_Seq, MaterialID = b.Material_ID.Trim() }
                        select new TransferLocation_Dto()
                        {
                            Batch = a.MO_Seq,
                            FromLocation = "",
                            Qty = a.Transac_Type.Trim() == "O" ? _repoTransactionDetail.GetTransQtyByTransacNo(a.Transac_No)
                                : a.Transac_Type.Trim() == "I" ? _repoTransactionDetail.GetQtyByTransacNo(a.Transac_No)
                                : a.Transac_Type.Trim() == "M" ? _repoTransactionDetail.GetQtyByTransacNo(a.Transac_No)
                                : a.Transac_Type.Trim().Contains("U") ? _repoTransactionDetail.GetQtyOtherOutByTransacNo(a.Transac_No)
                                : a.Transac_Type.Trim().Contains("T") ? _repoTransactionDetail.GetQtyOtherInByTransacNo(a.Transac_No)
                                : 0,
                            UpdateBy = a.Updated_By,
                            TransferNo = a.Transac_No.Trim(),
                            TransacTime = a.Transac_Time,
                            PlanNo = a.MO_No.Trim(),
                            MatId = a.Material_ID.Trim(),
                            MatName = a.Material_Name,
                            ToLocation = a.Rack_Location,
                            Id = a.ID,
                            TransacType = a.Transac_Type.Trim()
                        }).OrderBy(x => x.TransacType).ThenBy(x => x.MatId).ThenByDescending(x => x.TransacTime);
            return await PagedList<TransferLocation_Dto>.CreateAsync(data, paginationParams.PageNumber, paginationParams.PageSize);
        }

        public async Task<bool> SubmitTransfer(List<TransferLocation_Dto> lists, string updateBy)
        {
            DateTime timeNow = DateTime.Now;
            if (lists.Count > 0)
            {
                foreach (var item in lists)
                {
                    // Tìm ra TransactionMain theo id
                    var transactionMain = _repoTransactionMain.FindSingle(x => x.ID == item.Id);
                    // tạo biến lấy ra Transac_No của TransactionMain
                    var transacNo = transactionMain.Transac_No;

                    // Update TransactionMain cũ:  Can_Move = "N"
                    transactionMain.Can_Move = "N";
                    _repoTransactionMain.Update(transactionMain);
                    await _repoTransactionMain.SaveAll();

                    // Thêm TransactionMain mới dựa vào TransactionMain cũ: thêm mới chỉ thay đổi mấy trường dưới còn lại giữ nguyên
                    transactionMain.ID = 0; // Trong DB có identity tự tăng
                    transactionMain.Transac_Type = "M";
                    transactionMain.Can_Move = "Y";
                    // transactionMain.Is_Transfer_Form = "N";
                    transactionMain.Rack_Location = item.ToLocation;
                    transactionMain.Updated_By = updateBy;
                    transactionMain.Updated_Time = timeNow;
                    transactionMain.Transac_Time = item.TransacTime;
                    transactionMain.Transac_No = item.TransferNo;
                    transactionMain.Transac_Sheet_No = item.TransferNo;
                    transactionMain.Is_Transfer_Form = transactionMain.Is_Transfer_Form == "Y" ? "Y" : "N";
                    _repoTransactionMain.Add(transactionMain);

                    if(transactionMain.Is_Transfer_Form == "Y") {
                        // Update lại valid_status của TransferForm cũ = 0;
                        var tranferFormOld = _repoTransferForm.FindSingle(x => x.Transac_No.Trim() == transacNo);
                        tranferFormOld.Valid_Status = false;

                        // Thêm 1 record ở bảng TransferForm
                        var transferFormNew = new WMSB_Transfer_Form();
                        transferFormNew.Collec_Trans_Version = tranferFormOld.Collec_Trans_Version + 1;
                        transferFormNew.Transac_No = item.TransferNo;
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
                    // Thêm TransactionDetail mới dựa vào TransactionDetail của TransactionMain cũ(có bao nhiêu TransactionDetail cũ là thêm bấy nhiêu TransactionDetail mới): chỉ thay đổi Transac_No thành của TransactionMain mới
                    var transactionDetails = await _repoTransactionDetail.FindAll(x => x.Transac_No.Trim() == transacNo.Trim()).ToListAsync();
                    foreach (var transactionDetail in transactionDetails)
                    {
                        // thêm mới chỉ thay đổi mấy trường dưới, còn lại giữ nguyên
                        transactionDetail.ID = 0; // Trong DB có identity tự tăng
                        transactionDetail.Transac_No = item.TransferNo;
                        transactionDetail.Updated_Time = timeNow;
                        transactionDetail.Updated_By = updateBy;
                        _repoTransactionDetail.Add(transactionDetail);
                    }

                }
                return await _repoTransactionMain.SaveAll();
            }

            return false;
        }

        public async Task<bool> CheckExistRackLocation(string rackLocation)
        {
            var rack = await _repoRackLocation.FindAll(x => x.Rack_Location.Trim() == rackLocation.Trim()).FirstOrDefaultAsync();
            return rack != null ? true : false;

        }

        public bool CheckRackLocationHaveTheSameArea(string fromLocation, string toLocation)
        {
            var rackLocation = _repoRackLocation.FindSingle(x => x.Rack_Location.Trim() == fromLocation.Trim());

            if (fromLocation == toLocation)
            {
                return false;
            }

            if (rackLocation != null)
            {
                string areaId = rackLocation.Area_ID;
                var tmp = _repoRackLocation.FindAll(x => x.Rack_Location.Contains(toLocation.Trim()) && x.Area_ID.Trim() == areaId.Trim()).ToList();
                return tmp.Count() > 0 ? true : false;
            }
            else
            {
                return false;
            }

        }

        public async Task<bool> CheckTransacNoDuplicate(string transacNo)
        {
            return await _repoTransactionMain.CheckTransacNo(transacNo);
        }
    }
}