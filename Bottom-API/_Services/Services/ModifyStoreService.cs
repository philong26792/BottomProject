using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bottom_API._Repositories.Interfaces;
using Bottom_API._Services.Interfaces;
using Bottom_API.DTO.ModifyStore;
using Bottom_API.Helpers;
using Bottom_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Bottom_API._Services.Services
{
    public class ModifyStoreService : IModifyStoreService
    {
        private readonly ITransactionMainRepo _transactionMainRepo;
        private readonly IQRCodeMainRepository _qRCodeMainRepository;
        private readonly IQRCodeDetailRepository _qRCodeDetailRepository;
        private readonly IPackingListRepository _packingListRepository;
        private readonly IMaterialMissingRepository _missingRepository;
        private readonly IMaterialPurchaseRepository _materialPurchaseRepository;
        private readonly IMaterialViewRepository _materialViewRepository;
        private readonly ITransferFormRepository _transferFormRepository;
        private readonly ICacheRepository _cacheRepository;
        private readonly ITransactionDetailRepo _transactionDetailRepo;
        private readonly IReasonDetailRepository _reasonDetailRepo;
        private readonly ISettingT2SupplierRepository _repoSettingT2;
        private readonly IReleaseDeliveryNoRepository _repoReleaseDeliveryNo;

        public ModifyStoreService(ITransactionMainRepo transactionMainRepo,
            IQRCodeMainRepository qRCodeMainRepository,
            IQRCodeDetailRepository qRCodeDetailRepository,
            IPackingListRepository packingListRepository,
            IMaterialMissingRepository missingRepository,
            IMaterialPurchaseRepository materialPurchaseRepository,
            IMaterialViewRepository materialViewRepository,
            ITransferFormRepository transferFormRepository,
            ICacheRepository cacheRepository,
            ITransactionDetailRepo transactionDetailRepo,
            IReasonDetailRepository reasonDetailRepo,
            ISettingT2SupplierRepository repoSettingT2,
            IReleaseDeliveryNoRepository repoReleaseDeliveryNo)
        {
            _qRCodeDetailRepository = qRCodeDetailRepository;
            _packingListRepository = packingListRepository;
            _cacheRepository = cacheRepository;
            _missingRepository = missingRepository;
            _transactionDetailRepo = transactionDetailRepo;
            _qRCodeMainRepository = qRCodeMainRepository;
            _transactionMainRepo = transactionMainRepo;
            _materialPurchaseRepository = materialPurchaseRepository;
            _transferFormRepository = transferFormRepository;
            _materialViewRepository = materialViewRepository;
            _reasonDetailRepo = reasonDetailRepo;
            _repoSettingT2 = repoSettingT2;
            _repoReleaseDeliveryNo = repoReleaseDeliveryNo;
        }
        public async Task<object> GetDetailModifyStore(string moNo, string materialId)
        {
            // Lấy những transaction thỏa mãn điều kiện có gia công,ko gia công tương ứng có và ko có deliveryNo,is_release.....
            var transactionMains = await this.CheckRecordMainSatisfy(moNo, materialId);
            transactionMains = transactionMains.Where(x => !string.IsNullOrEmpty(x.Delivery_No)).OrderByDescending(x => x.Transac_Time).ToList();
            var deliveryNos = transactionMains.Select(x => x.Delivery_No).Distinct().ToList();

            var transactionMainList = await _transactionMainRepo.FindAll(x => x.MO_No.Trim() == moNo.Trim() &&
                                x.Material_ID.Trim() == materialId.Trim() && x.Can_Move == "Y")
                                .ToListAsync();

            var purchaseNos = transactionMainList.GroupBy(x => x.Purchase_No).Select(x => x.Key.Trim()).ToList();
            var batchList = transactionMainList.GroupBy(x => x.MO_Seq).Select(x => x.Key).ToList();
            var transactionNoList = transactionMainList.Select(x => x.Transac_No).ToList();
            var transactionDetailList = await _transactionDetailRepo.FindAll(x => transactionNoList.Contains(x.Transac_No)).OrderBy(x => x.Tool_Size).ThenBy(x => x.Order_Size).ToListAsync();
            var data1 = transactionDetailList.GroupBy(x => new { x.Tool_Size, x.Order_Size }).Select(x => new SizeInStockPlanQty
            {
                Tool_Size = x.Key.Tool_Size,
                Order_Size = x.Key.Order_Size,
                TotalInstockQty = x.Sum(y => y.Instock_Qty)
            }).ToList();

            // PlanQty bằng tổng của MO_Qty
            var materialPurchases = await _materialPurchaseRepository.FindAll(x => x.MO_No.Trim() == moNo.Trim() &&
                                            x.Material_ID.Trim() == materialId.Trim() &&
                                            purchaseNos.Contains(x.Purchase_No.Trim()) &&
                                            batchList.Contains(x.MO_Seq)).ToListAsync();
            foreach (var item in data1)
            {

                item.PlanQty = materialPurchases.Where(x => x.Tool_Size.Trim() == item.Tool_Size.Trim() &&
                                    x.Order_Size.Trim() == item.Order_Size.Trim()).Sum(x => x.MO_Qty);
            }

            var data2 = new List<SizeInstockQtyByBatch>();
            foreach (var item in batchList)
            {
                var dataItem = new SizeInstockQtyByBatch();
                dataItem.MO_Seq = item;
                var transactionNoListByBatch = transactionMainList.Where(x => x.MO_Seq == item).Select(x => x.Transac_No).ToList();
                var dataItemDetail = new List<SizeInstockQty>();
                var transactionDetailListByBatch = transactionDetailList.Where(x => transactionNoListByBatch.Contains(x.Transac_No)).ToList();
                foreach (var item1 in data1)
                {
                    var item2 = new SizeInstockQty();
                    item2.Tool_Size = item1.Tool_Size;
                    item2.Order_Size = item1.Order_Size;
                    var item3 = transactionDetailListByBatch.Where(x => x.Tool_Size == item1.Tool_Size && x.Order_Size == item1.Order_Size);
                    var purchasesByBatchAndBySize = materialPurchases.Where(x => x.Tool_Size.Trim() == item1.Tool_Size.Trim() &&
                                                            x.Order_Size.Trim() == item1.Order_Size.Trim() &&
                                                            x.MO_Seq == item).ToList();
                    // Khi QrCode sau khi Merge thì purchasesByBatchAndBySize =  null vì ko có data trong MaterialPurchase
                    if (item3.Any())
                    {
                        if (purchasesByBatchAndBySize.Any())
                        {
                            item2.PlanQty = purchasesByBatchAndBySize.Sum(x => x.MO_Qty);
                            item2.TotalInstockQty = item3.Sum(x => x.Instock_Qty);
                            item2.JustReceivedQty = item2.PlanQty - item2.TotalInstockQty;
                        }
                        else
                        {
                            item2.PlanQty = null;
                            item2.TotalInstockQty = item3.Sum(x => x.Instock_Qty);
                            item2.JustReceivedQty = null;
                        }
                    }
                    else
                    {
                        item2.TotalInstockQty = null;
                        item2.PlanQty = null;
                    }
                    item2.IsChange = false;
                    dataItemDetail.Add(item2);
                }
                dataItem.dataDetail = dataItemDetail;
                data2.Add(dataItem);
            }
            data2 = data2.OrderByDescending(x => x.MO_Seq).ToList();
            var data3 = data2.OrderBy(x => x.MO_Seq).ToList();
            return new { data1, data2, data3, deliveryNos };
        }

        public async Task<List<WMSB_Transaction_Detail>> GetDetailStoreChange(ModifyQrCodeAfterSave model)
        {
            var data = await _transactionDetailRepo.FindAll(x => x.Transac_No.Trim() == model.Transac_No.Trim()).ToListAsync();
            return data;
        }

        public async Task<List<ModifyQRCodeMain_Dto>> GetModifyStoreMain(string moNo, string supplierId, string qrCodeId)
        {
            var transactionMain = _transactionMainRepo.FindAll(x => x.Can_Move == "Y");
            var qrCodeMain = _qRCodeMainRepository.FindAll();
            var packingList = _packingListRepository.FindAll();
            var cache = _cacheRepository.FindAll();
            var transactionDetail = _transactionDetailRepo.FindAll();

            if (!string.IsNullOrEmpty(moNo))
            {
                transactionMain = transactionMain.Where(x => x.MO_No == moNo);
            }
            if (!string.IsNullOrEmpty(qrCodeId))
            {
                transactionMain = transactionMain.Where(x => x.QRCode_ID == qrCodeId);
            }

            var dataQuery = transactionMain
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
                                (x, y) => new ModifyQRCodeMain_Dto
                                {
                                    Material_ID = x.TransactionMainQrCodeMain.TransactionMain.Material_ID,
                                    Material_Name = x.TransactionMainQrCodeMain.TransactionMain.Material_Name.Trim(),
                                    MO_No = x.TransactionMainQrCodeMain.TransactionMain.MO_No,
                                    Supplier_ID = y.Supplier_ID,
                                    Supplier_Name = y.Supplier_Name.Trim(),
                                    Subcon_ID = y.Subcon_ID,
                                    Subcon_Name = y.Subcon_Name.Trim(),
                                    Model_No = y.Model_No,
                                    Model_Name = y.Model_Name.Trim(),
                                    Status = true,
                                    Article = y.Article,
                                    Custmoer_Part = cache.FirstOrDefault(x => x.MO_No == y.MO_No && x.MO_Seq == y.MO_Seq && x.Material_ID == y.Material_ID) == null ? ""
                                                    : cache.FirstOrDefault(x => x.MO_No == y.MO_No && x.MO_Seq == y.MO_Seq && x.Material_ID == y.Material_ID).Part_No,
                                    Custmoer_Name = cache.FirstOrDefault(x => x.MO_No == y.MO_No && x.MO_Seq == y.MO_Seq && x.Material_ID == y.Material_ID) == null ? ""
                                                    : cache.FirstOrDefault(x => x.MO_No == y.MO_No && x.MO_Seq == y.MO_Seq && x.Material_ID == y.Material_ID).Part_Name.Trim(),
                                    Stock_Qty = transactionDetail.Where(y => y.Transac_No == x.TransactionMainQrCodeMain.TransactionMain.Transac_No).Sum(x => x.Instock_Qty)
                                });
            if (!string.IsNullOrEmpty(supplierId) && supplierId != "All")
            {
                dataQuery = dataQuery.Where(x => x.Supplier_ID == supplierId);
            }

            var data = await dataQuery.ToListAsync();

            var data2 = data.GroupBy(x => new
            {
                x.Material_ID,
                x.Material_Name,
                x.MO_No,
                x.Model_No,
                x.Model_Name,
                x.Supplier_ID,
                x.Supplier_Name,
                x.Subcon_ID,
                x.Article,
                x.Custmoer_Name,
                x.Custmoer_Part,
                x.Subcon_Name,
                x.Status
            }).Select(x => new ModifyQRCodeMain_Dto
            {
                Article = x.Key.Article,
                Custmoer_Name = x.Key.Custmoer_Name,
                Custmoer_Part = x.Key.Custmoer_Part,
                Material_ID = x.Key.Material_ID,
                Material_Name = x.Key.Material_Name,
                MO_No = x.Key.MO_No,
                Model_Name = x.Key.Model_Name,
                Model_No = x.Key.Model_No,
                Subcon_ID = x.Key.Subcon_ID,
                Subcon_Name = x.Key.Subcon_Name,
                Supplier_ID = x.Key.Supplier_ID,
                Supplier_Name = x.Key.Supplier_Name,
                Status = x.Key.Status,
                Stock_Qty = x.Sum(x => x.Stock_Qty)
            }).ToList();
            foreach (var item in data2)
            {
                var checkT2Setting = await _repoSettingT2.FindAll(x => x.T2_Supplier_ID.Trim() == item.Supplier_ID.Trim()).ToListAsync();
                item.Status = checkT2Setting.Any() ? true : false;
            }
            data2 = data2.Where(x => x.Status == true).ToList();
            // -------------------------------Xử lý data theo có gia công và không có gia công-----------------------------------//
            foreach (var item in data2)
            {
                var transactionMainListSatisfy = await this.CheckRecordMainSatisfy(item.MO_No, item.Material_ID);
                item.Status = transactionMainListSatisfy.Any() ? true : false;
            }
            data2 = data2.Where(x => x.Status == true).ToList();
            return data2;
        }

        public async Task<List<ModifyQrCodeAfterSave>> SaveDataNoByBatchOut(ModifyQrCodeSaveParam param, string updateBy)
        {
            var updateTime = DateTime.Now;
            var dataParam = param.data;
            var modelParam = param.model;

            #region Tính toán
            // Mảng Batch có thành phần thay đổi
            var batchList = dataParam.Where(x => (x.dataDetail.Where(y => y.IsChange).Any())).Select(x => x.MO_Seq).ToList();
            dataParam = dataParam.Where(x => batchList.Contains(x.MO_Seq)).ToList();
            var transactions = await this.CheckRecordMainSatisfy(modelParam.MO_No, modelParam.Material_ID);
            var transactionNos = transactions.Select(x => x.Transac_No).ToList();

            // Mảng chứa transac_No và Transac_Type tương ứng (Giá tri cũ)
            var transTypeOfTransNo = transactions.Select(x => new { x.Transac_Type, x.Transac_No }).ToList();

            var dataAddTransactionDetail = new List<WMSB_Transaction_Detail>();
            foreach (var item in dataParam)
            {
                var transactionOfBatch = transactions.Where(x => x.MO_Seq == item.MO_Seq).ToList();
                var transactionNoListOfBatch = transactionOfBatch.Select(x => x.Transac_No).ToList();
                var transactionDetailForTransactionNo = await _transactionDetailRepo.FindAll(x => transactionNoListOfBatch.Contains(x.Transac_No)).ToListAsync();

                // Sắp xếp transaction,qrcode theo thứ tự Kho A => Kho B, và ngày gần nhất trừ trước
                var transactionBottom_A = new List<WMSB_Transaction_Main>();
                var transactionBottom_B = new List<WMSB_Transaction_Main>();
                foreach (var itemOf in transactionOfBatch)
                {
                    if (itemOf.QRCode_ID.StartsWith("A"))
                    {
                        transactionBottom_A.Add(itemOf);
                    }
                    if (itemOf.QRCode_ID.StartsWith("B"))
                    {
                        transactionBottom_B.Add(itemOf);
                    }
                }
                transactionBottom_A = transactionBottom_A.OrderByDescending(x => x.Transac_Time).ToList();
                transactionBottom_B = transactionBottom_B.OrderByDescending(x => x.Transac_Time).ToList();
                transactionBottom_A.AddRange(transactionBottom_B);
                var transactionListOfBatchDesending = transactionBottom_A.Select(x => x.Transac_No).ToList();

                foreach (var item1 in transactionListOfBatchDesending)
                {
                    foreach (var item2 in item.dataDetail)
                    {
                        var transactionDetailModel = new WMSB_Transaction_Detail();
                        var transactionDetailFind = transactionDetailForTransactionNo.Where(x => x.Transac_No == item1 &&
                                                    x.Tool_Size == item2.Tool_Size && x.Order_Size == item2.Order_Size).FirstOrDefault();
                        if (transactionDetailFind != null)
                        {
                            transactionDetailModel.Transac_No = item1;
                            transactionDetailModel.Tool_Size = item2.Tool_Size;
                            transactionDetailModel.Order_Size = item2.Order_Size;
                            transactionDetailModel.Model_Size = transactionDetailFind.Model_Size;
                            transactionDetailModel.Spec_Size = transactionDetailFind.Spec_Size;
                            transactionDetailModel.Qty = transactionDetailFind.Qty;
                            transactionDetailModel.Updated_Time = updateTime;
                            transactionDetailModel.Updated_By = updateBy;
                            if (!item2.IsChange)
                            {
                                transactionDetailModel.Trans_Qty = transactionDetailFind.Trans_Qty;
                                transactionDetailModel.Instock_Qty = transactionDetailFind.Instock_Qty;
                                transactionDetailModel.Untransac_Qty = transactionDetailFind.Untransac_Qty;
                            }
                            else
                            {
                                if (item2.ModifyQty >= transactionDetailFind.Trans_Qty)
                                {
                                    transactionDetailModel.Trans_Qty = 0;
                                    item2.ModifyQty = item2.ModifyQty - transactionDetailFind.Trans_Qty;
                                }
                                else if (item2.ModifyQty < transactionDetailFind.Trans_Qty && item2.ModifyQty > 0)
                                {
                                    transactionDetailModel.Trans_Qty = transactionDetailFind.Trans_Qty - item2.ModifyQty;
                                    item2.ModifyQty = 0;
                                }
                                else if (item2.ModifyQty == 0)
                                {
                                    transactionDetailModel.Trans_Qty = transactionDetailFind.Trans_Qty;
                                }
                                transactionDetailModel.Instock_Qty = transactionDetailModel.Trans_Qty;
                                transactionDetailModel.Untransac_Qty = transactionDetailModel.Qty - transactionDetailModel.Instock_Qty;
                            }
                            dataAddTransactionDetail.Add(transactionDetailModel);
                        }
                    }
                }
            }

            // Trong mảng transaction detail thêm vào,loại ra các transactionNo mà transQty ko thay đổi
            var transactionIsChange = new List<string>();
            var transactionDetailAddGroup = dataAddTransactionDetail.GroupBy(x => x.Transac_No).Select(x => new
            {
                Transac_No = x.Key,
                Trans_Qty_Total = x.Sum(cl => cl.Trans_Qty)
            }).ToList();

            foreach (var item in transactionDetailAddGroup)
            {
                var transQtyTotal = _transactionDetailRepo.FindAll(x => x.Transac_No.Trim() == item.Transac_No.Trim()).Sum(x => x.Trans_Qty);
                if (transQtyTotal == item.Trans_Qty_Total)
                {
                    // Tổng của TransacQty không thay đổi nên qrcode đó cũng ko thay đổi,và sẽ bị loại khỏi mảng dataAddTransactionDetail
                    dataAddTransactionDetail = dataAddTransactionDetail.Where(x => x.Transac_No != item.Transac_No).ToList();
                }
                else
                {
                    transactionIsChange.Add(item.Transac_No.Trim());
                }
            }

            #region Update Transaction Main
            //======================================Update lại trong bảng Transaction Main =======================================//
            var transactionMainIsChange = await _transactionMainRepo.FindAll(x => transactionIsChange.Contains(x.Transac_No)).OrderByDescending(x => x.MO_Seq).ToListAsync();
            foreach (var item in transactionMainIsChange)
            {
                item.Can_Move = "N";
                item.Transac_Type = item.Transac_Type.Trim() + "U";
                item.Updated_Time = updateTime;
                item.Reason_Code = param.reason_code;
            }
            #endregion Tính Toán

            #endregion Update Transaction Main

            #region Add Transaction Main
            //=========================================Thêm mới vào bảng Transaction Main =========================================//
            var transactionListAdd = new List<WMSB_Transaction_Main>();

            // Mảng chứa transactionNo mới và cũ tương ứng
            var transactionNoTime = new List<TransactionNoTime>();
            var transactionNoList = new List<string>();
            var transheetNoList = new List<string>();
            foreach (var item in transactionMainIsChange)
            {
                var transactionModel = new WMSB_Transaction_Main();
                var transTypeOfTransNoItem = transTypeOfTransNo.Where(x => x.Transac_No.Trim() == item.Transac_No.Trim()).FirstOrDefault();
                transactionModel.Transac_Type = transTypeOfTransNoItem.Transac_Type.Trim();
                var transactionNo = "";
                var transac_Sheet_No = "";
                var transactionNoTimeItem = new TransactionNoTime();
                transactionNoTimeItem.TransactionNoOld = item.Transac_No;
                if (transTypeOfTransNoItem.Transac_Type.Trim() == "I")
                {
                    do
                    {
                        transactionNo = "BI" + item.MO_No.Trim() + CodeUtility.RandomNumber(3);
                        transactionModel.Transac_No = transactionNo;
                        transactionNoTimeItem.TransactionNoNew = transactionNo;
                    } while (await _transactionMainRepo.CheckTransacNo(transactionNo) || transactionNoList.Contains(transactionNo));
                    transactionNoList.Add(transactionNo);
                    do
                    {
                        transac_Sheet_No = "IB" + DateTime.Now.ToString("yyyyMMdd") + CodeUtility.RandomNumber(3);
                        transactionModel.Transac_Sheet_No = transac_Sheet_No;
                    } while (await _transactionMainRepo.CheckTranSheetNo(transac_Sheet_No) || transheetNoList.Contains(transac_Sheet_No));
                    transheetNoList.Add(transac_Sheet_No);

                }
                else if (transTypeOfTransNoItem.Transac_Type.Trim() == "M")
                {
                    do
                    {
                        transactionNo = "TB" + item.MO_No.Trim() + CodeUtility.RandomNumber(3);
                        transactionModel.Transac_No = transactionNo;
                        transactionNoTimeItem.TransactionNoNew = transactionNo;
                        transactionModel.Transac_Sheet_No = transactionNo;
                    } while (await _transactionMainRepo.CheckTransacNo(transactionNo) || transactionNoList.Contains(transactionNo));
                    transactionNoList.Add(transactionNo);
                }
                else if (transTypeOfTransNoItem.Transac_Type.Trim() == "R")
                {
                    transactionNo = "R" + transTypeOfTransNoItem.Transac_No;
                    transactionModel.Transac_No = transactionNo;
                    transactionNoTimeItem.TransactionNoNew = transactionNo;
                    transactionModel.Transac_Sheet_No = transactionNo;

                }
                else if (transTypeOfTransNoItem.Transac_No.Trim() == "MG")
                {
                    do
                    {
                        transactionNo = "MB" + item.MO_No.Trim() + CodeUtility.RandomNumber(3);
                        transactionModel.Transac_No = transactionNo;
                        transactionNoTimeItem.TransactionNoNew = transactionNo;
                    } while (await _transactionMainRepo.CheckTransacNo(transactionNo) || transactionNoList.Contains(transactionNo));
                    transactionNoList.Add(transactionNo);
                    do
                    {
                        transac_Sheet_No = "MB" + DateTime.Now.ToString("yyyyMMdd") + CodeUtility.RandomNumber(3);
                        transactionModel.Transac_Sheet_No = transac_Sheet_No;
                    } while (await _transactionMainRepo.CheckTranSheetNo(transac_Sheet_No) || transheetNoList.Contains(transac_Sheet_No));
                    transheetNoList.Add(transac_Sheet_No);
                }
                transactionModel.Can_Move = "Y";
                transactionModel.Transac_Time = updateTime;
                transactionModel.QRCode_ID = item.QRCode_ID;
                transactionModel.QRCode_Version = item.QRCode_Version + 1;
                transactionModel.MO_No = item.MO_No;
                transactionModel.Purchase_No = item.Purchase_No;
                transactionModel.Delivery_No = item.Delivery_No;
                transactionModel.MO_Seq = item.MO_Seq;
                transactionModel.Material_ID = item.Material_ID;
                transactionModel.Material_Name = item.Material_Name;
                transactionModel.Purchase_Qty = item.Purchase_Qty;

                var transactionDetailByTransacNo = dataAddTransactionDetail.Where(x => x.Transac_No.Trim() == item.Transac_No.Trim()).ToList();
                transactionModel.Transacted_Qty = transactionDetailByTransacNo.Sum(x => x.Instock_Qty);

                transactionModel.Rack_Location = item.Rack_Location;
                transactionModel.Missing_No = item.Missing_No;
                transactionModel.Pickup_No = item.Pickup_No;
                transactionModel.Is_Transfer_Form = item.Is_Transfer_Form;

                // reason code tương ứng với batch đó.
                var reasoncodes = param.ReasonDetail.Where(x => x.Batch == item.MO_Seq).OrderBy(x => x.Reason).Select(x => x.Reason).Distinct().ToList();
                var reason = "";
                for (int i = 0; i < reasoncodes.Count; i++)
                {
                    if(i != reasoncodes.Count -1) {
                        reason = reason + reasoncodes[i] + ",";
                    } else {
                        reason = reason + reasoncodes[i];
                    }
                }
                transactionModel.Reason_Code = reason;
                transactionModel.Updated_Time = updateTime;
                transactionModel.Updated_By = updateBy;
                transactionListAdd.Add(transactionModel);
                transactionNoTime.Add(transactionNoTimeItem);
            }

            // Thêm Giá Trị Missing_No vào mảng transactionListAdd
            // 1 batch tương ứng với 1 mã missingNo
            for (int i = 0; i < transactionListAdd.Count; i++)
            {
                if (i == 0)
                {
                    transactionListAdd[i].Missing_No = "BS1" + DateTime.Now.ToString("yyyyMMdd") + CodeUtility.RandomNumber(3);
                }
                else
                {
                    if (transactionListAdd[i].MO_Seq == transactionListAdd[i - 1].MO_Seq)
                    {
                        transactionListAdd[i].Missing_No = transactionListAdd[i - 1].Missing_No;
                    }
                    else
                    {
                        transactionListAdd[i].Missing_No = "BS1" + DateTime.Now.ToString("yyyyMMdd") + CodeUtility.RandomNumber(3);
                    }
                }
            }
            // Mảng chứa missing và Batch tương ứng
            var missingInBatch = transactionListAdd.GroupBy(x => new {x.Missing_No, x.MO_Seq}).Select(x => new MissingInBatch() {
                Missing_No = x.Key.Missing_No,
                MO_Seq = x.Key.MO_Seq
            }).ToList();
            await _transactionMainRepo.AddRange(transactionListAdd);
            #endregion Add Transaction Main

            #region Add Transaction Detail
            //=========================================Thêm vào bảng Transaction Detail============================================//
            foreach (var item in transactionNoTime)
            {
                var transactionDetails = dataAddTransactionDetail.Where(x => x.Transac_No == item.TransactionNoOld).ToList();
                foreach (var item1 in transactionDetails)
                {
                    item1.Transac_No = item.TransactionNoNew;
                }
            }
            await _transactionDetailRepo.AddRange(dataAddTransactionDetail);
            #endregion Add Transaction Detail

            #region Update and Add Transfer Form
            //==================================== Kiểm tra để Update và thêm bảng Transfer Form ==================================//
            ProcessTransferForm(transactionListAdd, transactionNoTime, updateBy, updateTime);
            #endregion Update and Add Transfer Form

            #region Update, Add QrCodeMain, QrCodeDetail
            //=================================Update,thêm mới trong bảng QrCode Main,QrCode Detail ================================//
            ProcessQrCode(transactions, transactionNoTime, transactionIsChange, dataAddTransactionDetail, updateBy, updateTime);
            #endregion Update, Add QrCodeMain, QrCodeDetail

            #region Add Missing
            //============================================Thêm vào bảng Missing=================================================//
            ProcessMissing(transactionListAdd, dataAddTransactionDetail, param.isMissing, updateBy, updateTime);
            #endregion Add Missing

            #region Add Reason Detail
            //==========================================Thêm vào bảng ReasonDetail===============================================//
            ProcessReasonDetail(param.ReasonDetail, missingInBatch, updateBy, updateTime);
            #endregion Add Reason Detail

            //await _transactionMainRepo.SaveAll();

            //========================================= Show data ra ngoài màn hình khi save===================================//
            return DataResult(transactionListAdd, true, transactionNoTime, dataAddTransactionDetail, dataParam, updateBy, updateTime);
        }

        public async Task<List<ModifyQrCodeAfterSave>> SaveDataNoByBatchIn(ModifyQrCodeSaveParam param, string updateBy)
        {
            var updateTime = DateTime.Now;
            var dataParam = param.data;
            var modelParam = param.model;

            // Mảng Batch có thành phần thay đổi
            var batchList = dataParam.Where(x => (x.dataDetail.Where(y => y.IsChange)).Any()).Select(x => x.MO_Seq).ToList();
            dataParam = dataParam.Where(x => batchList.Contains(x.MO_Seq)).ToList();
            var transactions = await _transactionMainRepo.FindAll(x => x.MO_No.Trim() == modelParam.MO_No.Trim() &&
                                                                    x.Can_Move == "Y" &&
                                                                    x.Material_ID.Trim() == modelParam.Material_ID.Trim()
                                                                    && batchList.Contains(x.MO_Seq)).ToListAsync();
            var transactionNos = transactions.Select(x => x.Transac_No).ToList();

            // Mảng chứa transac_No và Transac_Type tương ứng (Giá tri cũ)
            var transTypeOfTransNo = transactions.Select(x => new { x.Transac_Type, x.Transac_No }).ToList();

            var dataAddTransactionDetail = new List<WMSB_Transaction_Detail>();
            foreach (var item in dataParam)
            {
                var transactionOfBatch = transactions.Where(x => x.MO_Seq == item.MO_Seq).ToList();
                var transactionNoOfBatch = transactionOfBatch.OrderBy(x => x.MO_Seq).Select(x => x.Transac_No).FirstOrDefault();
                var transactionDetailForTransactionNo = await _transactionDetailRepo.FindAll(x => x.Transac_No.Trim() == transactionNoOfBatch.Trim()).ToListAsync();

                foreach (var item2 in item.dataDetail)
                {
                    var transactionDetailModel = new WMSB_Transaction_Detail();
                    var transactionDetailFind = transactionDetailForTransactionNo.Where(x => x.Transac_No == transactionNoOfBatch &&
                                                x.Tool_Size == item2.Tool_Size && x.Order_Size == item2.Order_Size).FirstOrDefault();
                    if (transactionDetailFind != null)
                    {
                        transactionDetailModel.Transac_No = transactionNoOfBatch;
                        transactionDetailModel.Tool_Size = item2.Tool_Size;
                        transactionDetailModel.Order_Size = item2.Order_Size;
                        transactionDetailModel.Model_Size = transactionDetailFind.Model_Size;
                        transactionDetailModel.Spec_Size = transactionDetailFind.Spec_Size;
                        transactionDetailModel.Qty = transactionDetailFind.Qty;
                        transactionDetailModel.Updated_Time = updateTime;
                        transactionDetailModel.Updated_By = updateBy;
                        if (!item2.IsChange)
                        {
                            transactionDetailModel.Trans_Qty = transactionDetailFind.Trans_Qty;
                            transactionDetailModel.Instock_Qty = transactionDetailFind.Instock_Qty;
                            transactionDetailModel.Untransac_Qty = transactionDetailFind.Untransac_Qty;
                        }
                        else
                        {
                            transactionDetailModel.Trans_Qty = transactionDetailFind.Trans_Qty + item2.ModifyQty;
                            transactionDetailModel.Instock_Qty = transactionDetailModel.Trans_Qty;
                            transactionDetailModel.Untransac_Qty = transactionDetailModel.Instock_Qty - transactionDetailModel.Qty;
                        }
                        dataAddTransactionDetail.Add(transactionDetailModel);
                    }
                }
            }

            // Trong mảng transaction detail thêm vào,loại ra các transactionNo mà transQty ko thay đổi
            var transactionIsChange = new List<string>();
            var transactionDetailAddGroup = dataAddTransactionDetail.GroupBy(x => x.Transac_No).Select(x => new
            {
                Transac_No = x.Key,
                Trans_Qty_Total = x.Sum(cl => cl.Trans_Qty)
            }).ToList();

            foreach (var item in transactionDetailAddGroup)
            {
                var transQtyTotal = _transactionDetailRepo.FindAll(x => x.Transac_No.Trim() == item.Transac_No.Trim()).Sum(x => x.Trans_Qty);
                if (transQtyTotal == item.Trans_Qty_Total)
                {
                    // Tổng của TransacQty không thay đổi nên qrcode đó cũng ko thay đổi,và sẽ bị loại khỏi mảng dataAddTransactionDetail
                    dataAddTransactionDetail = dataAddTransactionDetail.Where(x => x.Transac_No != item.Transac_No).ToList();
                }
                else
                {
                    transactionIsChange.Add(item.Transac_No.Trim());
                }
            }


            //======================================Update lại trong bảng Transaction Main =======================================//
            var transactionMainIsChange = await _transactionMainRepo.FindAll(x => transactionIsChange.Contains(x.Transac_No)).ToListAsync();
            foreach (var item in transactionMainIsChange)
            {
                item.Can_Move = "N";
                item.Transac_Type = item.Transac_Type.Trim() + "T";
                item.Updated_Time = updateTime;
                item.Reason_Code = param.reason_code;
            }


            //=========================================Thêm mới vào bảng Transaction Main =========================================//
            var transactionListAdd = new List<WMSB_Transaction_Main>();

            // Mảng chứa transactionNo mới và cũ tương ứng
            var transactionNoTime = new List<TransactionNoTime>();
            var transactionNoList = new List<string>();
            var transheetNoList = new List<string>();
            foreach (var item in transactionMainIsChange)
            {
                var transactionModel = new WMSB_Transaction_Main();
                var transTypeOfTransNoItem = transTypeOfTransNo.Where(x => x.Transac_No.Trim() == item.Transac_No.Trim()).FirstOrDefault();
                transactionModel.Transac_Type = transTypeOfTransNoItem.Transac_Type.Trim();
                var transactionNo = "";
                var transac_Sheet_No = "";
                var transactionNoTimeItem = new TransactionNoTime();
                transactionNoTimeItem.TransactionNoOld = item.Transac_No;
                if (transTypeOfTransNoItem.Transac_Type.Trim() == "I")
                {
                    do
                    {
                        transactionNo = "BI" + item.MO_No.Trim() + CodeUtility.RandomNumber(3);
                        transactionModel.Transac_No = transactionNo;
                        transactionNoTimeItem.TransactionNoNew = transactionNo;
                    } while (await _transactionMainRepo.CheckTransacNo(transactionNo) || transactionNoList.Contains(transactionNo));
                    transactionNoList.Add(transactionNo);

                    do
                    {
                        transac_Sheet_No = "IB" + DateTime.Now.ToString("yyyyMMdd") + CodeUtility.RandomNumber(3);
                        transactionModel.Transac_Sheet_No = transac_Sheet_No;
                    } while (await _transactionMainRepo.CheckTranSheetNo(transac_Sheet_No) || transheetNoList.Contains(transac_Sheet_No));
                    transheetNoList.Add(transac_Sheet_No);

                }
                else if (transTypeOfTransNoItem.Transac_Type.Trim() == "M")
                {
                    do
                    {
                        transactionNo = "TB" + item.MO_No.Trim() + CodeUtility.RandomNumber(3);
                        transactionModel.Transac_No = transactionNo;
                        transactionNoTimeItem.TransactionNoNew = transactionNo;
                        transactionModel.Transac_Sheet_No = transactionNo;
                    } while (await _transactionMainRepo.CheckTransacNo(transactionNo) || transactionNoList.Contains(transactionNo));
                    transactionNoList.Add(transactionNo);

                }
                else if (transTypeOfTransNoItem.Transac_Type.Trim() == "R")
                {
                    transactionNo = "R" + transTypeOfTransNoItem.Transac_No;
                    transactionModel.Transac_No = transactionNo;
                    transactionNoTimeItem.TransactionNoNew = transactionNo;
                    transactionModel.Transac_Sheet_No = transactionNo;

                }
                else if (transTypeOfTransNoItem.Transac_No.Trim() == "MG")
                {
                    do
                    {
                        transactionNo = "MB" + item.MO_No.Trim() + CodeUtility.RandomNumber(3);
                        transactionModel.Transac_No = transactionNo;
                        transactionNoTimeItem.TransactionNoNew = transactionNo;
                    } while (await _transactionMainRepo.CheckTransacNo(transactionNo) || transactionNoList.Contains(transactionNo));
                    transactionNoList.Add(transactionNo);

                    do
                    {
                        transac_Sheet_No = "MB" + DateTime.Now.ToString("yyyyMMdd") + CodeUtility.RandomNumber(3);
                        transactionModel.Transac_Sheet_No = transac_Sheet_No;
                    } while (await _transactionMainRepo.CheckTranSheetNo(transac_Sheet_No) || transheetNoList.Contains(transac_Sheet_No));
                    transheetNoList.Add(transac_Sheet_No);
                }
                transactionModel.Can_Move = "Y";
                transactionModel.Transac_Time = updateTime;
                transactionModel.QRCode_ID = item.QRCode_ID;
                transactionModel.QRCode_Version = item.QRCode_Version + 1;
                transactionModel.MO_No = item.MO_No;
                transactionModel.Purchase_No = item.Purchase_No;
                transactionModel.MO_Seq = item.MO_Seq;
                transactionModel.Material_ID = item.Material_ID;
                transactionModel.Material_Name = item.Material_Name;
                transactionModel.Purchase_Qty = item.Purchase_Qty;

                var transactionDetailByTransacNo = dataAddTransactionDetail.Where(x => x.Transac_No.Trim() == item.Transac_No.Trim()).ToList();
                transactionModel.Transacted_Qty = transactionDetailByTransacNo.Sum(x => x.Instock_Qty);

                transactionModel.Rack_Location = item.Rack_Location;
                transactionModel.Missing_No = item.Missing_No;
                transactionModel.Pickup_No = item.Pickup_No;
                transactionModel.Is_Transfer_Form = item.Is_Transfer_Form;
                transactionModel.Reason_Code = param.reason_code;
                transactionModel.Updated_Time = updateTime;
                transactionModel.Updated_By = updateBy;
                transactionListAdd.Add(transactionModel);
                transactionNoTime.Add(transactionNoTimeItem);
            }
            // Thêm Giá Trị Missing_No vào mảng transactionListAdd
            var missingNoNew = "";
            do
            {
                missingNoNew = "BMU" + DateTime.Now.ToString("yyyyMMdd") + CodeUtility.RandomNumber(3);
            } while (await this._missingRepository.CheckMissingNo(missingNoNew));
            for (int i = 0; i < transactionListAdd.Count; i++)
            {
                transactionListAdd[i].Missing_No = missingNoNew;
            }
            await _transactionMainRepo.AddRange(transactionListAdd);


            //=========================================Thêm vào bảng Transaction Detail============================================//
            foreach (var item in transactionNoTime)
            {
                var transactionDetails = dataAddTransactionDetail.Where(x => x.Transac_No == item.TransactionNoOld).ToList();
                foreach (var item1 in transactionDetails)
                {
                    item1.Transac_No = item.TransactionNoNew;
                }
            }
            await _transactionDetailRepo.AddRange(dataAddTransactionDetail);


            //==================================== Kiểm tra để Update và thêm bảng Transfer Form ==================================//
            ProcessTransferForm(transactionListAdd, transactionNoTime, updateBy, updateTime);

            //=================================Update,thêm mới trong bảng QrCode Main,QrCode Detail ================================//
            ProcessQrCode(transactions, transactionNoTime, transactionIsChange, dataAddTransactionDetail, updateBy, updateTime);

            //============================================Thêm vào bảng Missing=================================================//
            ProcessMissing(transactionListAdd, dataAddTransactionDetail, param.isMissing, updateBy, updateTime);

            //==========================================Thêm vào bảng ReasonDetail===============================================//
            //ProcessReasonDetail(param.ReasonDetail, missingNoNew, updateBy, updateTime);

            //await _transactionMainRepo.SaveAll();


            //========================================= Show data ra ngoài màn hình khi save===================================//
            return DataResult(transactionListAdd, false, transactionNoTime, dataAddTransactionDetail, dataParam, updateBy, updateTime);
        }

        public async Task<string> GetPlanNoByQRCodeID(string qrCodeId)
        {
            var planNo = await _transactionMainRepo.FindAll(x => x.QRCode_ID.Trim() == qrCodeId.Trim()).Select(x => x.MO_No).FirstOrDefaultAsync();
            return planNo ?? "";
        }
        public void ProcessQrCode(List<WMSB_Transaction_Main> transactions, List<TransactionNoTime> transactionNoTime,
                                    List<string> transactionIsChange, List<WMSB_Transaction_Detail> dataAddTransactionDetail,
                                    string updateBy, DateTime updateTime)
        {
            transactions = transactions.Where(x => transactionIsChange.Contains(x.Transac_No.Trim())).ToList();
            foreach (var item in transactions)
            {
                // Update bảng QrCode Main
                var qrCodeMainModel = _qRCodeMainRepository.FindAll(x => x.QRCode_ID.Trim() == item.QRCode_ID.Trim())
                        .OrderByDescending(x => x.QRCode_Version).FirstOrDefault();
                if (qrCodeMainModel != null)
                {
                    qrCodeMainModel.Valid_Status = "N";
                    qrCodeMainModel.Invalid_Date = updateTime;
                    qrCodeMainModel.Updated_Time = updateTime;
                }

                // Thêm vào bảng QrCode Main
                var qrCodeMainAdd = new WMSB_QRCode_Main();
                qrCodeMainAdd.QRCode_ID = qrCodeMainModel.QRCode_ID;
                qrCodeMainAdd.QRCode_Version = qrCodeMainModel.QRCode_Version + 1;
                qrCodeMainAdd.QRCode_Type = qrCodeMainModel.QRCode_Type;
                qrCodeMainAdd.Receive_No = qrCodeMainModel.Receive_No;
                qrCodeMainAdd.Valid_Status = "Y";
                qrCodeMainAdd.Is_Scanned = qrCodeMainModel.Is_Scanned;
                qrCodeMainAdd.Updated_Time = updateTime;
                qrCodeMainAdd.Updated_By = updateBy;
                _qRCodeMainRepository.Add(qrCodeMainAdd);

                // Thêm vào bảng qrCode Detail
                var transactionTime = transactionNoTime.Where(x => x.TransactionNoOld.Trim() == item.Transac_No.Trim()).FirstOrDefault();
                var transactionDetailAddFind = dataAddTransactionDetail.Where(x => x.Transac_No.Trim() == transactionTime.TransactionNoNew.Trim()).ToList();
                foreach (var item1 in transactionDetailAddFind)
                {
                    var qrCodeDetailModel = new WMSB_QRCode_Detail();
                    qrCodeDetailModel.QRCode_ID = qrCodeMainModel.QRCode_ID;
                    qrCodeDetailModel.QRCode_Version = qrCodeMainModel.QRCode_Version + 1;
                    qrCodeDetailModel.Tool_Size = item1.Tool_Size;
                    qrCodeDetailModel.Order_Size = item1.Order_Size;
                    qrCodeDetailModel.Model_Size = item1.Model_Size;
                    qrCodeDetailModel.Qty = item1.Trans_Qty;
                    qrCodeDetailModel.Updated_Time = updateTime;
                    qrCodeDetailModel.Updated_By = updateBy;
                    _qRCodeDetailRepository.Add(qrCodeDetailModel);
                }
            }
        }
        public void ProcessTransferForm(List<WMSB_Transaction_Main> transactionListAdd, List<TransactionNoTime> transactionNoTime, string updateBy, DateTime updateTime)
        {
            foreach (var item in transactionListAdd)
            {
                if (item.Is_Transfer_Form == "Y")
                {
                    var transactionNoOfTime = transactionNoTime.Where(x => x.TransactionNoNew.Trim() == item.Transac_No.Trim()).FirstOrDefault();

                    // Update trong bảng Transfer Form
                    var transferFormFind = _transferFormRepository.FindSingle(x => x.Transac_No.Trim() == transactionNoOfTime.TransactionNoOld.Trim());
                    transferFormFind.Valid_Status = false;

                    // Thêm mới trong bảng Transfer Form
                    var transFerFormModel = new WMSB_Transfer_Form();
                    transFerFormModel.Collect_Trans_No = transferFormFind.Collect_Trans_No;
                    transFerFormModel.Transac_No = transactionNoOfTime.TransactionNoNew;
                    transFerFormModel.Generate_Time = transferFormFind.Generate_Time;
                    transFerFormModel.T3_Supplier = transferFormFind.T3_Supplier;
                    transFerFormModel.Is_Release = transferFormFind.Is_Release;
                    transFerFormModel.Release_By = transferFormFind.Release_By;
                    transFerFormModel.Release_Time = transferFormFind.Release_Time;
                    transFerFormModel.Update_Time = updateTime;
                    transFerFormModel.Update_By = updateBy;
                    transFerFormModel.MO_No = transferFormFind.MO_No;
                    transFerFormModel.MO_Seq = transferFormFind.MO_Seq;
                    transFerFormModel.Material_ID = transferFormFind.Material_ID;
                    transFerFormModel.Material_Name = transferFormFind.Material_Name;
                    transFerFormModel.T3_Supplier_Name = transferFormFind.T3_Supplier_Name;
                    transFerFormModel.Collec_Trans_Version = transferFormFind.Collec_Trans_Version + 1;
                    transFerFormModel.Valid_Status = true;
                    transFerFormModel.Is_Closed = "N";
                    _transferFormRepository.Add(transFerFormModel);
                }
            }
        }
        public List<ModifyQrCodeAfterSave> DataResult(List<WMSB_Transaction_Main> transactionListAdd, bool Out,
                                                        List<TransactionNoTime> transactionNoTime,
                                                        List<WMSB_Transaction_Detail> dataAddTransactionDetail,
                                                        List<SizeInstockQtyByBatch> dataParam, string updateBy, DateTime updateTime)
        {
            var resultAfterSave = new List<ModifyQrCodeAfterSave>();
            foreach (var item in transactionListAdd)
            {
                var resultAfterSaveItem = new ModifyQrCodeAfterSave();
                resultAfterSaveItem.Transac_No = item.Transac_No;
                resultAfterSaveItem.Missing_No = item.Missing_No;
                resultAfterSaveItem.MO_No = item.MO_No;
                resultAfterSaveItem.MO_Seq = item.MO_Seq;
                resultAfterSaveItem.Material_ID = item.Material_ID;
                resultAfterSaveItem.Material_Name = item.Material_Name;
                resultAfterSaveItem.Modify_Time = updateTime;
                resultAfterSaveItem.QRCode_ID = item.QRCode_ID;
                resultAfterSaveItem.QRCode_Version = item.QRCode_Version;
                resultAfterSaveItem.Update_By = updateBy;

                var transactionDetailListFind = dataAddTransactionDetail.Where(x => x.Transac_No.Trim() == item.Transac_No.Trim()).ToList();
                resultAfterSaveItem.Actual_Qty = transactionDetailListFind.Sum(x => x.Instock_Qty);
                if (Out)
                {
                    var transacNoOld = transactionNoTime.Where(x => x.TransactionNoNew == item.Transac_No).FirstOrDefault().TransactionNoOld;
                    var instockQtyTotalOld = _transactionDetailRepo.FindAll(x => x.Transac_No.Trim() == transacNoOld.Trim()).Sum(x => x.Instock_Qty);
                    resultAfterSaveItem.Modify_Qty = instockQtyTotalOld - resultAfterSaveItem.Actual_Qty;
                }
                else
                {
                    var totalModify = dataParam.Where(x => x.MO_Seq == item.MO_Seq).Sum(x => x.dataDetail.Sum(y => y.ModifyQty));
                    resultAfterSaveItem.Modify_Qty = totalModify;
                }
                resultAfterSave.Add(resultAfterSaveItem);
            }
            return resultAfterSave;
        }
        public async void ProcessReasonDetail(List<LeftRightInBatchOfReason> data, List<MissingInBatch> missingBatchs, string updateBy, DateTime updateTime)
        {
            var dataAdd = new List<WMSB_Reason_Detail>();
            foreach (var item in data)
            {
                var reasonDetailItem = new WMSB_Reason_Detail();
                var missingBatchItem = missingBatchs.Find(x => x.MO_Seq == item.Batch);
                reasonDetailItem.Missing_No = missingBatchItem.Missing_No;
                reasonDetailItem.Reason_Code = item.Reason;
                reasonDetailItem.Left_Qty = item.Left;
                reasonDetailItem.Right_Qty = item.Right;
                reasonDetailItem.Order_Size = item.Order_Size;
                reasonDetailItem.Model_Size = item.Order_Size;
                reasonDetailItem.Tool_Size = item.Tool_Size;
                reasonDetailItem.Modify_Qty = (item.Left + item.Right)/2;
                reasonDetailItem.Updated_By = updateBy;
                reasonDetailItem.Updated_Time = updateTime;
                dataAdd.Add(reasonDetailItem);
            }
            await _reasonDetailRepo.AddRange(dataAdd);
        }

        public async void ProcessMissing(List<WMSB_Transaction_Main> transactionListAdd,
                                            List<WMSB_Transaction_Detail> dataAddTransactionDetail, bool isMissing,
                                            string updateBy, DateTime updateTime)
        {
            var materialMissingList = new List<WMSB_Material_Missing>();
            foreach (var item in transactionListAdd)
            {
                var viewMaterialFind = _materialViewRepository.FindSingle(x => x.Plan_No.Trim() == item.MO_No.Trim() &&
                                                        x.Purchase_No.Trim() == item.Purchase_No.Trim() &&
                                                        x.Mat_.Trim() == item.Material_ID.Trim() &&
                                                        x.MO_Seq == item.MO_Seq);
                var materialPurchseModel = _materialPurchaseRepository.FindAll(x => x.MO_No.Trim() == item.MO_No.Trim() &&
                                                                x.Purchase_No.Trim() == item.Purchase_No.Trim() &&
                                                                x.MO_Seq == item.MO_Seq &&
                                                                x.Material_ID.Trim() == item.Material_ID.Trim()).ToList();
                var transactionDetailListFind = dataAddTransactionDetail.Where(x => x.Transac_No.Trim() == item.Transac_No.Trim()).ToList();
                foreach (var item1 in transactionDetailListFind)
                {
                    var missingModel = new WMSB_Material_Missing();
                    var materialPurchaseFind = materialPurchseModel.Where(x => x.Tool_Size.Trim() == item1.Tool_Size.Trim() &&
                                        x.Order_Size.Trim() == item1.Order_Size.Trim()).FirstOrDefault();
                    missingModel.Missing_No = item.Missing_No;
                    missingModel.Factory_ID = materialPurchaseFind.Factory_ID;
                    missingModel.Purchase_No = item.Purchase_No;
                    missingModel.Collect_No = materialPurchaseFind.Collect_No;
                    missingModel.MO_No = item.MO_No;
                    missingModel.Reason_Code = item.Reason_Code;
                    missingModel.MO_Seq = item.MO_Seq;
                    missingModel.Order_Size = item1.Order_Size;
                    missingModel.Material_ID = item.Material_ID;
                    missingModel.Material_Name = item.Material_Name;

                    missingModel.Model_No = viewMaterialFind.Model_No;
                    missingModel.Model_Name = viewMaterialFind.Model_Name;
                    missingModel.Article = viewMaterialFind.Article;

                    missingModel.Model_Size = item1.Model_Size;
                    missingModel.Tool_Size = item1.Tool_Size;
                    missingModel.Spec_Size = item1.Spec_Size;
                    missingModel.Purchase_Kind = materialPurchaseFind.Purchase_Kind;
                    missingModel.Purchase_Size = materialPurchaseFind.Purchase_Size;
                    missingModel.MO_Qty = materialPurchaseFind.MO_Qty;
                    missingModel.PreBook_Qty = materialPurchaseFind.PreBook_Qty;
                    missingModel.Stock_Qty = materialPurchaseFind.Stock_Qty;
                    // Purchase_Qty ở trong bảng Material_Missing lấy ở Untransac_Qty trong bảng Transaction Detail
                    missingModel.Purchase_Qty = item1.Untransac_Qty;
                    missingModel.Supplier_ID = materialPurchaseFind.Supplier_ID;
                    missingModel.Supplier_Name = viewMaterialFind.Supplier_Name;
                    missingModel.Require_Delivery = materialPurchaseFind.Require_Delivery;
                    missingModel.Confirm_Delivery = materialPurchaseFind.Confirm_Delivery;
                    missingModel.Tool_Type = materialPurchaseFind.Tool_Type;
                    missingModel.Tool_ID = materialPurchaseFind.Tool_ID;
                    missingModel.Process_Code = materialPurchaseFind.Process_Code;
                    missingModel.Subcon_Name = viewMaterialFind.Subcon_Name;
                    missingModel.Custmoer_Part = materialPurchaseFind.Custmoer_Part;
                    missingModel.T3_Supplier = materialPurchaseFind.T3_Supplier;
                    missingModel.T3_Supplier_Name = viewMaterialFind.T3_Supplier_Name;
                    missingModel.Stage = materialPurchaseFind.Stage;
                    if (isMissing)
                    {
                        missingModel.Is_Missing = "Y";
                    }
                    else
                    {
                        missingModel.Is_Missing = "N";
                    }
                    missingModel.Updated_Time = updateTime;
                    missingModel.Updated_By = updateBy;
                    materialMissingList.Add(missingModel);
                }
            }
            var missingListAdd = materialMissingList.GroupBy(x => new { x.Missing_No, x.MO_Seq, x.Order_Size })
            .Select(x => new WMSB_Material_Missing()
            {
                Missing_No = x.FirstOrDefault().Missing_No,
                Reason_Code = x.FirstOrDefault().Reason_Code,
                Factory_ID = x.FirstOrDefault().Factory_ID,
                Purchase_No = x.FirstOrDefault().Purchase_No,
                Collect_No = x.FirstOrDefault().Collect_No,
                MO_No = x.FirstOrDefault().MO_No,
                MO_Seq = x.Key.MO_Seq,
                Order_Size = x.Key.Order_Size,
                Material_ID = x.FirstOrDefault().Material_ID,
                Material_Name = x.FirstOrDefault().Material_Name,
                Model_No = x.FirstOrDefault().Model_No,
                Model_Name = x.FirstOrDefault().Model_Name,
                Article = x.FirstOrDefault().Article,
                Model_Size = x.FirstOrDefault().Model_Size,
                Tool_Size = x.FirstOrDefault().Tool_Size,
                Spec_Size = x.FirstOrDefault().Spec_Size,
                Purchase_Kind = x.FirstOrDefault().Purchase_Kind,
                Purchase_Size = x.FirstOrDefault().Purchase_Size,
                MO_Qty = x.FirstOrDefault().MO_Qty,
                PreBook_Qty = x.FirstOrDefault().PreBook_Qty,
                Stock_Qty = x.FirstOrDefault().Stock_Qty,
                Purchase_Qty = x.Sum(cl => cl.Purchase_Qty),
                Accumlated_In_Qty = 0,
                Status = "N",
                Missing_Stage = "S1",
                Notice_Kind = "C",
                Supplier_ID = x.FirstOrDefault().Supplier_ID,
                Supplier_Name = x.FirstOrDefault().Supplier_Name,
                Require_Delivery = x.FirstOrDefault().Require_Delivery,
                Confirm_Delivery = x.FirstOrDefault().Confirm_Delivery,
                Tool_Type = x.FirstOrDefault().Tool_Type,
                Tool_ID = x.FirstOrDefault().Tool_ID,
                Process_Code = x.FirstOrDefault().Process_Code,
                Subcon_Name = x.FirstOrDefault().Subcon_Name,
                Custmoer_Part = x.FirstOrDefault().Custmoer_Part,
                T3_Supplier = x.FirstOrDefault().T3_Supplier,
                T3_Supplier_Name = x.FirstOrDefault().T3_Supplier_Name,
                Stage = x.FirstOrDefault().Stage,
                Is_Missing = x.FirstOrDefault().Is_Missing,
                Updated_Time = updateTime,
                Updated_By = updateBy,
            }).ToList();
            await _missingRepository.AddRange(missingListAdd);
        }

        public async Task<List<WMSB_Setting_T2Delivery>> GetReasonOfSupplierID(string supplierId)
        {
            var data = await _repoSettingT2.FindAll(x => x.T2_Supplier_ID.Trim() == supplierId.Trim()).ToListAsync();
            return data;
        }

        public async Task<List<WMSB_Transaction_Main>> CheckRecordMainSatisfy(string moNo, string materialId)
        {
            var transactionTypeGroup = new List<string> { "I", "M", "O", "R", "MG" };
            var checkStatus = new List<bool>();
            var transactionMainListSatisfy = new List<WMSB_Transaction_Main>();
            var transactionMainList_KoGiaCong = await _transactionMainRepo.FindAll(x => x.MO_No.Trim() == moNo.Trim() &&
                                            x.Material_ID.Trim() == materialId.Trim() &&
                                            x.Can_Move == "Y" && x.Is_Transfer_Form == "N").ToListAsync();
            var transactionMainList_CoGiaCong = await _transactionMainRepo.FindAll(x => x.MO_No.Trim() == moNo.Trim() &&
                                            x.Material_ID.Trim() == materialId.Trim() &&
                                            x.Can_Move == "Y" && x.Is_Transfer_Form == "Y").ToListAsync();
            // -------------------------------------------không có gia công---------------------------------------------//
            if (transactionMainList_KoGiaCong.Any())
            {
                foreach (var itemNoGiaCong in transactionMainList_KoGiaCong)
                {
                    // Nếu không có delivery No
                    if (string.IsNullOrEmpty(itemNoGiaCong.Delivery_No))
                    {
                        if (!transactionTypeGroup.Contains(itemNoGiaCong.Transac_Type.Trim()))
                        {
                            checkStatus.Add(false);
                        }
                        else
                        {
                            checkStatus.Add(true);
                            transactionMainListSatisfy.Add(itemNoGiaCong);
                        }
                    }
                    // Nếu có delivery No thì delivery No trong bảng ReleaseDeliveryNo phải= N
                    else
                    {
                        var releaseDeliveryModel = await _repoReleaseDeliveryNo.FindAll(x => x.Delivery_No.Trim() == itemNoGiaCong.Delivery_No.Trim() &&
                                                        x.MO_No.Trim() == moNo.Trim() &&
                                                        x.Material_ID.Trim() == materialId.Trim()).FirstOrDefaultAsync();
                        if (releaseDeliveryModel.Is_Release.Trim() == "N")
                        {
                            checkStatus.Add(true);
                            transactionMainListSatisfy.Add(itemNoGiaCong);
                        }
                        else
                        {
                            checkStatus.Add(false);
                        }
                    }
                }

            }

            // ---------------------------------------------------Có gia công-------------------------------------------------//
            if (transactionMainList_CoGiaCong.Any())
            {
                foreach (var itemCoGiaCong in transactionMainList_CoGiaCong)
                {
                    var tranferForm = await _transferFormRepository.FindAll(x => x.Transac_No.Trim() == itemCoGiaCong.Transac_No.Trim() &&
                                                    x.Valid_Status == true).FirstOrDefaultAsync();
                    if (tranferForm.Is_Release.Trim() == "Y")
                    {
                        checkStatus.Add(false);
                    }
                    else
                    {
                        // Nếu không có delivery No
                        if (string.IsNullOrEmpty(itemCoGiaCong.Delivery_No))
                        {
                            if (!transactionTypeGroup.Contains(itemCoGiaCong.Transac_Type.Trim()))
                            {
                                checkStatus.Add(false);
                            }
                            else
                            {
                                checkStatus.Add(true);
                                transactionMainListSatisfy.Add(itemCoGiaCong);
                            }
                        }
                        else
                        {
                            var releaseDeliveryModel = _repoReleaseDeliveryNo.FindAll(x => x.Delivery_No.Trim() == itemCoGiaCong.Delivery_No.Trim() &&
                                                            x.Purchase_No.Trim() == itemCoGiaCong.Purchase_No.Trim() &&
                                                            x.Material_ID.Trim() == itemCoGiaCong.Material_ID.Trim()).FirstOrDefault();
                            if (releaseDeliveryModel.Is_Release.Trim() == "N")
                            {
                                checkStatus.Add(true);
                                transactionMainListSatisfy.Add(itemCoGiaCong);
                            }
                            else
                            {
                                checkStatus.Add(false);
                            }
                        }
                    }
                }
            }
            return transactionMainListSatisfy;
        }
    }
}