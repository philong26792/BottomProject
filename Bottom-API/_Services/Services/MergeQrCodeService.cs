using System;
using System.Data; 
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Bottom_API._Repositories.Interfaces;
using Bottom_API._Repositories.Interfaces.DbMES;
using Bottom_API._Repositories.Interfaces.DbUser;
using Bottom_API._Services.Interfaces;
using Bottom_API.DTO;
using Bottom_API.DTO.MergeQrCode;
using Bottom_API.Helpers;
using Bottom_API.Models;
using Microsoft.EntityFrameworkCore;
using LinqKit;

namespace Bottom_API._Services.Services
{
    public class MergeQrCodeService : IMergeQrCodeService
    {
        private readonly ITransactionMainRepo _repoTransactionMain;
        private readonly ITransactionDetailRepo _repoTransactionDetail;
        private readonly IQRCodeMainRepository _repoQRCodeMain;
        private readonly IQRCodeDetailRepository _repoQRCodeDetail;
        private readonly IPackingListRepository _repoPackingList;
        private readonly IPackingListDetailRepository _repoPackingListDetail;
        private readonly IMaterialMissingRepository _repoMaterialMissing;
        private readonly IMaterialPurchaseRepository _repoMaterialPurchase;
        private readonly IMaterialViewRepository _repoMaterialView;
        private readonly ITransferFormRepository _repoTransferForm;
        private readonly ICacheRepository _repoCache;
        private readonly IMapper _mapper;
        private readonly IMaterialOffsetRepository _repoMaterialOffset;
        private readonly IMesMoSizeRepository _repoMesMoSize;
        private readonly IMaterialsRepository _repoMaterials;
        private readonly IPoMaterialsRepository _repoPoMaterials;
        private readonly IPoRootsRepository _repoPoRoots;
        private readonly IMesMoBasicRepository _repoMesMoBasic;
        private readonly IMaterialPurchaseSplitRepository _repoMaterialPurchaseSplit;

        public MergeQrCodeService(ITransactionMainRepo repoTransactionMain,
                                    ITransactionDetailRepo repoTransactionDetail,
                                    IQRCodeMainRepository repoQRCodeMain,
                                    IQRCodeDetailRepository repoQRCodeDetail,
                                    IPackingListRepository repoPackingList,
                                    IPackingListDetailRepository repoPackingListDetail,
                                    IMaterialMissingRepository repoMaterialMissing,
                                    IMaterialPurchaseRepository repoMaterialPurchase,
                                    IMaterialViewRepository repoMaterialView,
                                    ITransferFormRepository repoTransferForm,
                                    ICacheRepository repoCache,
                                    IMapper mapper,
                                    IMaterialOffsetRepository repoMaterialOffset,
                                    IMesMoSizeRepository repoMesMoSize,
                                    IMaterialsRepository repoMaterials,
                                    IPoMaterialsRepository repoPoMaterials,
                                    IPoRootsRepository repoPoRoots,
                                    IMesMoBasicRepository repoMesMoBasic,
                                    IMaterialPurchaseSplitRepository repoMaterialPurchaseSplit)
        {
            _repoTransactionMain = repoTransactionMain;
            _repoTransactionDetail = repoTransactionDetail;
            _repoQRCodeMain = repoQRCodeMain;
            _repoQRCodeDetail = repoQRCodeDetail;
            _repoPackingList = repoPackingList;
            _repoMaterialMissing = repoMaterialMissing;
            _repoMaterialPurchase = repoMaterialPurchase;
            _repoMaterialView = repoMaterialView;
            _repoTransferForm = repoTransferForm;
            _repoCache = repoCache;
            _mapper = mapper;
            _repoPackingListDetail = repoPackingListDetail;
            _repoMaterialOffset = repoMaterialOffset;
            _repoMesMoSize = repoMesMoSize;
            _repoMaterials = repoMaterials;
            _repoPoMaterials = repoPoMaterials;
            _repoPoRoots = repoPoRoots;
            _repoMesMoBasic = repoMesMoBasic;
            _repoMaterialPurchaseSplit = repoMaterialPurchaseSplit;
        }

        public async Task<List<QrCodeAfterMerge>> MergeQRCode(List<MergeQrCodeModel> data, string updateBy)
        {
            var dataResult = new List<QrCodeAfterMerge>();
            var timeNow = DateTime.Now;
            var purchaseNos = data.GroupBy(x => x.Purchase_No).Select(x => x.Key).ToList();
            var transac_Nos = data.Select(x => x.Transac_No).ToList();
            var transactionMainList = await _repoTransactionMain.FindAll(x => transac_Nos.Contains(x.Transac_No.Trim())).ToListAsync();
            var transactionDetailList = await _repoTransactionDetail.FindAll(x => transac_Nos.Contains(x.Transac_No.Trim())).ToListAsync();
            #region Add Transaction Main
            // ========================================Add vào bảng TransactionMain========================================//
            var transactionMain = new WMSB_Transaction_Main();
            transactionMain.Transac_Type = "MG";
            var transacNoNew = "";
            do
            {
                transacNoNew = "MB" + data[0].MO_No.Trim() + CodeUtility.RandomNumber(3);
                transactionMain.Transac_No = transacNoNew;
                
            }
            while (await _repoTransactionMain.CheckTransacNo(transacNoNew));

            var qrCodeIdNew = "";
            do
            {
                var po = data[0].MO_No.Trim().Length == 9 ? data[0].MO_No.Trim() + "Z" : data[0].MO_No.Trim();
                string so = CodeUtility.RandomNumber(3);
                qrCodeIdNew = "M" + po + so + CodeUtility.RandomStringUpper(1);
                transactionMain.QRCode_ID = qrCodeIdNew;
                
            } while (await this.CheckQrCodeID(qrCodeIdNew));

            var transacSheetNoNew = "";
            do
            {
                transacSheetNoNew = "MB" + timeNow.ToString("yyyyMMdd") + CodeUtility.RandomNumber(3);
                transactionMain.Transac_Sheet_No = transacSheetNoNew;
               
            } while (await _repoTransactionMain.CheckTranSheetNo(transacSheetNoNew));

            transactionMain.MO_No = data[0].MO_No;
            transactionMain.Can_Move = "Y";
            transactionMain.Purchase_No = data[0].Purchase_No;
            transactionMain.Transac_Time = timeNow;
            transactionMain.QRCode_Version = 1;
            transactionMain.MO_Seq = "";
            transactionMain.Material_ID = data[0].Material_ID;
            transactionMain.Material_Name = data[0].Material_Name;
            transactionMain.Purchase_Qty = transactionMainList.Sum(x => x.Purchase_Qty);
            transactionMain.Transacted_Qty = transactionDetailList.Sum(x => x.Instock_Qty);
            transactionMain.Rack_Location = data[0].Rack_Location;
            transactionMain.Is_Transfer_Form = "N";
            transactionMain.Updated_Time = timeNow;
            transactionMain.Updated_By = updateBy;
            _repoTransactionMain.Add(transactionMain);
            #endregion Add Transaction Main

            #region Update Transaction Main
            // ==========================================Update vào bảng TransactionMain==========================================//
            foreach (var item in data)
            {
                var transactionModel = await _repoTransactionMain.FindAll(x => x.Transac_No.Trim() == item.Transac_No.Trim()).FirstOrDefaultAsync();
                transactionModel.Can_Move = "N";
                transactionModel.Merge_Transac_No = transacNoNew;
                transactionModel.Updated_Time = timeNow;
            }
            #endregion Update Transaction Main

            #region Add Transaction Detail
            // ========================================Add Table Transaction Detail========================================//
            var groupSize = transactionDetailList.GroupBy(x => new { x.Tool_Size, x.Order_Size, x.Model_Size, x.Spec_Size })
                .Select(x => new
                {
                    Tool_Size = x.Key.Tool_Size,
                    Order_Size = x.Key.Order_Size,
                    Model_Size = x.Key.Model_Size,
                    Spec_Size = x.Key.Spec_Size
                }).ToList();
            var transactionDetailListAdd = new List<WMSB_Transaction_Detail>();
            foreach (var sizeItem in groupSize)
            {
                var transactionDetailItem = new WMSB_Transaction_Detail();
                var transactionDetailOfSize = transactionDetailList.Where(x => x.Tool_Size == sizeItem.Tool_Size &&
                                                x.Order_Size == sizeItem.Order_Size && x.Model_Size == sizeItem.Model_Size &&
                                                x.Spec_Size == sizeItem.Spec_Size).ToList();
                transactionDetailItem.Transac_No = transacNoNew;
                transactionDetailItem.Tool_Size = sizeItem.Tool_Size;
                transactionDetailItem.Order_Size = sizeItem.Order_Size;
                transactionDetailItem.Model_Size = sizeItem.Model_Size;
                transactionDetailItem.Spec_Size = sizeItem.Spec_Size;
                transactionDetailItem.Qty = transactionDetailOfSize.Sum(x => x.Qty);
                transactionDetailItem.Trans_Qty = transactionDetailOfSize.Sum(x => x.Trans_Qty);
                transactionDetailItem.Instock_Qty = transactionDetailOfSize.Sum(x => x.Instock_Qty);
                transactionDetailItem.Untransac_Qty = transactionDetailOfSize.Sum(x => x.Untransac_Qty);
                transactionDetailItem.Updated_By = updateBy;
                transactionDetailItem.Updated_Time = timeNow;
                transactionDetailListAdd.Add(transactionDetailItem);
            }
            await _repoTransactionDetail.AddRange(transactionDetailListAdd);
            #endregion Add Transaction Detail

            #region Update QrCode Main
            //===================================================Update QrCode Main===================================================//
            foreach (var item in data)
            {
                var qrcodeMain = await _repoQRCodeMain.FindAll(x => x.QRCode_ID == item.QRCode_ID
                                && x.QRCode_Version == item.QRCode_Version && x.Is_Scanned == "Y").FirstOrDefaultAsync();
                if (qrcodeMain != null)
                {
                    qrcodeMain.Valid_Status = "N";
                    qrcodeMain.Invalid_Date = timeNow;
                    qrcodeMain.Merge_QRCodeID = qrCodeIdNew;
                    qrcodeMain.Updated_Time = timeNow;
                }
            }
            #endregion Update QrCode Main

            #region Add QrCode Main
            //===================================================Add QrCode Main===================================================//
            var qrCodeMainAdd = new WMSB_QRCode_Main();
            var receiveNoNew = CodeUtility.RandomReceiveNo("MW", 2);
            qrCodeMainAdd.QRCode_ID = qrCodeIdNew;
            qrCodeMainAdd.Receive_No = receiveNoNew;
            qrCodeMainAdd.QRCode_Version = 1;
            qrCodeMainAdd.QRCode_Type = "M";
            qrCodeMainAdd.Valid_Status = "Y";
            qrCodeMainAdd.Is_Scanned = "Y";
            qrCodeMainAdd.Updated_By = updateBy;
            qrCodeMainAdd.Updated_Time = timeNow;
            _repoQRCodeMain.Add(qrCodeMainAdd);
            #endregion Add QrCode Main

            #region Add QrCode Detail
            //===================================================Add QrCode Detail===================================================//
            foreach (var item in transactionDetailListAdd)
            {
                var qrCodeDetailAdd = new WMSB_QRCode_Detail();
                qrCodeDetailAdd.QRCode_ID = qrCodeIdNew;
                qrCodeDetailAdd.QRCode_Version = 1;
                qrCodeDetailAdd.Tool_Size = item.Tool_Size;
                qrCodeDetailAdd.Order_Size = item.Order_Size;
                qrCodeDetailAdd.Model_Size = item.Model_Size;
                qrCodeDetailAdd.Spec_Size = item.Spec_Size;
                qrCodeDetailAdd.Qty = item.Instock_Qty;
                qrCodeDetailAdd.Updated_By = updateBy;
                qrCodeDetailAdd.Updated_Time = timeNow;
                _repoQRCodeDetail.Add(qrCodeDetailAdd);
            }
            #endregion Add QrCode Detail

            #region Add PackingList
            //===========================================================Add PackingList=================================================//
            var receiveNos = data.Select(x => x.Receive_No).ToList();
            var packingLists = await _repoPackingList.FindAll(x => receiveNos.Contains(x.Receive_No)).ToListAsync();
            var packingListModelAdd = new WMSB_Packing_List();
            packingListModelAdd.Sheet_Type = "M";
            packingListModelAdd.Receive_No = receiveNoNew;
            packingListModelAdd.Supplier_ID = data[0].Supplier_ID;
            packingListModelAdd.Supplier_Name = data[0].Supplier_Name;
            packingListModelAdd.Receive_Date = timeNow;
            packingListModelAdd.MO_No = data[0].MO_No;
            packingListModelAdd.Purchase_No = data[0].Purchase_No;
            packingListModelAdd.MO_Seq = "";
            packingListModelAdd.Missing_No = "";
            packingListModelAdd.Material_ID = data[0].Material_ID;
            packingListModelAdd.Material_Name = data[0].Material_Name;
            packingListModelAdd.Model_No = data[0].Model_No;
            packingListModelAdd.Model_Name = data[0].Model_Name;
            packingListModelAdd.Article = data[0].Article;
            packingListModelAdd.Subcon_ID = packingLists[0].Subcon_ID;
            packingListModelAdd.Subcon_Name = packingLists[0].Subcon_Name;
            packingListModelAdd.T3_Supplier = packingLists[0].T3_Supplier;
            packingListModelAdd.T3_Supplier_Name = packingLists[0].T3_Supplier_Name;
            packingListModelAdd.Generated_QRCode = "Y";
            packingListModelAdd.Updated_Time = timeNow;
            packingListModelAdd.Updated_By = updateBy;
            _repoPackingList.Add(packingListModelAdd);

            #endregion Add PackingList

            #region Add Packing List Detail
            //=================================================== Add Packing List Detail=========================================//
            var packingListDetails = await _repoPackingListDetail.FindAll(x => receiveNos.Contains(x.Receive_No)).ToListAsync();
            var groupSizePacking = packingListDetails.GroupBy(x => new { x.Tool_Size, x.Order_Size, x.Model_Size, x.Spec_Size })
                .Select(x => new
                {
                    Tool_Size = x.Key.Tool_Size,
                    Order_Size = x.Key.Order_Size,
                    Model_Size = x.Key.Model_Size,
                    Spec_Size = x.Key.Spec_Size
                }).ToList();
            var packingListDetailAdd = new List<WMSB_PackingList_Detail>();
            foreach (var sizeItem in groupSizePacking)
            {
                var packingListDetailItem = new WMSB_PackingList_Detail();
                var packingListDetailOfSize = packingListDetails.Where(x => x.Order_Size == sizeItem.Order_Size &&
                                                            x.Model_Size == sizeItem.Model_Size &&
                                                            x.Tool_Size == sizeItem.Tool_Size &&
                                                            x.Spec_Size == sizeItem.Spec_Size).ToList();
                packingListDetailItem.Receive_No = receiveNoNew;
                packingListDetailItem.Order_Size = sizeItem.Order_Size;
                packingListDetailItem.Model_Size = sizeItem.Model_Size;
                packingListDetailItem.Tool_Size = sizeItem.Tool_Size;
                packingListDetailItem.Spec_Size = sizeItem.Spec_Size;
                packingListDetailItem.MO_Qty = packingListDetailOfSize.Sum(x => x.MO_Qty);
                packingListDetailItem.Purchase_Qty = packingListDetailOfSize.Sum(x => x.Purchase_Qty);
                packingListDetailItem.Received_Qty = packingListDetailOfSize.Sum(x => x.Received_Qty);
                packingListDetailItem.Updated_Time = timeNow;
                packingListDetailItem.Updated_By = updateBy;
                _repoPackingListDetail.Add(packingListDetailItem);
            }
            #endregion Add Packing List Detail

            #region Show Data
            var dataResultItem = new QrCodeAfterMerge();
            dataResultItem.Transac_No = transacNoNew;
            dataResultItem.QRCode_ID = qrCodeIdNew;
            dataResultItem.MO_No = data[0].MO_No;
            dataResultItem.Model_No = data[0].Model_No;
            dataResultItem.Model_Name = data[0].Model_Name;
            dataResultItem.Article = data[0].Article;
            dataResultItem.Qty = transactionDetailListAdd.Sum(x => x.Trans_Qty);
            dataResultItem.Material_ID = data[0].Material_ID;
            dataResultItem.Material_Name = data[0].Material_Name;
            dataResultItem.Merge_Time = timeNow;
            dataResultItem.Update_By = updateBy;
            dataResult.Add(dataResultItem);
            #endregion Show Data
            await _repoTransactionMain.SaveAll();
            return dataResult;
        }

        public async Task<List<MergeQrCodeModel>> SearchOfMerge(FilterMergeQrCodeParam param)
        {
            var transactionMain = _repoTransactionMain.FindAll(x => x.Can_Move == "Y");
            var qrcodeMain = _repoQRCodeMain.FindAll(x => x.Valid_Status == "Y");
            var packingList = _repoPackingList.FindAll();
            var cache = _repoCache.FindAll();
            var transactionDetail = _repoTransactionDetail.FindAll();

            if (!string.IsNullOrEmpty(param.MO_No))
            {
                transactionMain = transactionMain.Where(x => x.MO_No.Trim() == param.MO_No.Trim());
            }
            if (!string.IsNullOrEmpty(param.Material_ID) && param.Material_ID != "All")
            {
                transactionMain = transactionMain.Where(x => x.Material_ID.Trim() == param.Material_ID.Trim());
            }
            if (!string.IsNullOrEmpty(param.Supplier_ID) && param.Supplier_ID != "All")
            {
                packingList = packingList.Where(x => x.Supplier_ID.Trim() == param.Supplier_ID.Trim());
            }
            var dataQuery = from a in transactionMain
                            join b in qrcodeMain
    on new { qrCodeID = a.QRCode_ID, version = a.QRCode_Version } equals
        new { qrCodeID = b.QRCode_ID, version = b.QRCode_Version }
                            join c in packingList on b.Receive_No equals c.Receive_No
                            select new MergeQrCodeModel
                            {
                                QRCode_ID = b.QRCode_ID,
                                MO_No = a.MO_No,
                                QRCode_Version = b.QRCode_Version,
                                Transac_No = a.Transac_No.Trim(),
                                Receive_No = b.Receive_No,
                                Purchase_No = a.Purchase_No.Trim(),
                                Material_ID = a.Material_ID.Trim(),
                                Material_Name = a.Material_Name.Trim(),
                                Rack_Location = a.Rack_Location,
                                Model_No = c.Model_No.Trim(),
                                Model_Name = c.Model_Name.Trim(),
                                Article = c.Article.Trim(),
                                Supplier_ID = c.Supplier_ID.Trim(),
                                Supplier_Name = c.Supplier_Name.Trim(),
                                Part_No = cache.FirstOrDefault(x => x.MO_No == a.MO_No && x.MO_Seq == a.MO_Seq && x.Material_ID == a.Material_ID) == null ? ""
                                                    : cache.FirstOrDefault(x => x.MO_No == a.MO_No && x.MO_Seq == a.MO_Seq && x.Material_ID == a.Material_ID).Part_No,
                                Part_Name = cache.FirstOrDefault(x => x.MO_No == a.MO_No && x.MO_Seq == a.MO_Seq && x.Material_ID == a.Material_ID) == null ? ""
                                                    : cache.FirstOrDefault(x => x.MO_No == a.MO_No && x.MO_Seq == a.MO_Seq && x.Material_ID == a.Material_ID).Part_Name.Trim(),
                                Stock_Qty = transactionDetail.Where(x => x.Transac_No.Trim() == a.Transac_No.Trim()).Sum(x => x.Instock_Qty)
                            };
            var data = await dataQuery.ToListAsync();
            data = data.OrderBy(x => x.Purchase_No).ThenBy(x => x.Material_ID).ToList();
            return data;
        }

        public async Task<bool> CheckQrCodeID(string qrCodeID)
        {
            var qrCodeMainModel = await _repoQRCodeMain.FindAll(x => x.QRCode_ID.Trim() == qrCodeID.Trim()).FirstOrDefaultAsync();
            if (qrCodeMainModel != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<List<WMSB_Transaction_Detail>> GetDetailQrCode(QrCodeAfterMerge model)
        {
            return await _repoTransactionDetail.FindAll(x => x.Transac_No.Trim() == model.Transac_No.Trim()).OrderBy(x => x.Tool_Size).ThenBy(x => x.Order_Size).ToListAsync();
        }

        public async Task<List<MaterialInformation>> GetMaterialInformation()
        {
            var data = await _repoTransactionMain.FindAll().ToListAsync();
            var dataResult = data.GroupBy(x => x.Material_ID)
            .Select(x => new MaterialInformation
            {
                Material_ID = x.Key.Trim(),
                Material_Name = x.FirstOrDefault().Material_Name.Trim()
            }).ToList();
            return dataResult;
        }

        #region Split
        public async Task<List<MergeQrCodeModel>> SearchTransactionForSplit(string moNo, string qrCodeID, string timeFrom, string timeEnd, bool searchByPrebook = true)
        {
            // tìm những đơn còn hiệu lực: Can_Move == "Y" và không phải là những được được nhận từ tách đơn Reason_Code != "ZZZ"
            var transactionMain = _repoTransactionMain.FindAll(x => x.Can_Move == "Y" && x.Reason_Code.Trim() != "ZZZ");
            var qrCodeMain = _repoQRCodeMain.FindAll(x => x.Valid_Status == "Y");
            var packingList = _repoPackingList.FindAll();
            var cache = _repoCache.FindAll();
            var transactionDetail = _repoTransactionDetail.FindAll();

            if (searchByPrebook == true)
            {
                if (!string.IsNullOrEmpty(moNo))
                {
                    transactionMain = transactionMain.Where(x => x.MO_No.Trim() == moNo.Trim());
                }
                if (!string.IsNullOrEmpty(qrCodeID))
                {
                    transactionMain = transactionMain.Where(x => x.QRCode_ID.Trim() == qrCodeID.Trim());
                }
                if (!string.IsNullOrEmpty(timeFrom) && !string.IsNullOrEmpty(timeEnd))
                {
                    DateTime t1 = Convert.ToDateTime(timeFrom + " 00:00:00");
                    DateTime t2 = Convert.ToDateTime(timeEnd + " 23:59:59");
                    transactionMain = transactionMain.Where(x => x.Transac_Time >= t1 && x.Transac_Time <= t2);
                }
                var dMoNoByChildMoNoOffset = await _repoMaterialOffset.FindAll().Select(x => x.DMO_No.Trim()).Distinct().ToListAsync();
                transactionMain = transactionMain.Where(x => dMoNoByChildMoNoOffset.Contains(x.MO_No));
            }
            else
            {
                var dMoNoByChildMoNoOffset = await _repoMaterialOffset.FindAll(x => x.MO_No.Trim() == moNo.Trim()).Select(x => x.DMO_No.Trim()).Distinct().ToListAsync();
                    transactionMain = transactionMain.Where(x => dMoNoByChildMoNoOffset.Contains(x.MO_No.Trim()));
                if (!string.IsNullOrEmpty(qrCodeID))
                {
                    transactionMain = transactionMain.Where(x => x.QRCode_ID.Trim() == qrCodeID.Trim());
                }

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
                                (x, y) => new MergeQrCodeModel
                                {
                                    Transac_No = x.TransactionMainQrCodeMain.TransactionMain.Transac_No,
                                    QRCode_ID = x.TransactionMainQrCodeMain.TransactionMain.QRCode_ID,
                                    MO_No = x.TransactionMainQrCodeMain.TransactionMain.MO_No,
                                    Material_ID = x.TransactionMainQrCodeMain.TransactionMain.Material_ID,
                                    Material_Name = x.TransactionMainQrCodeMain.TransactionMain.Material_Name,
                                    Rack_Location = x.TransactionMainQrCodeMain.TransactionMain.Rack_Location,
                                    Model_No = y.Model_No,
                                    Model_Name = y.Model_Name,
                                    Article = y.Article,
                                    Supplier_ID = y.Supplier_ID,
                                    Supplier_Name = y.Supplier_Name,
                                    Part_No = cache.FirstOrDefault(z => z.MO_No == y.MO_No && z.MO_Seq == y.MO_Seq && z.Material_ID == y.Material_ID) == null ? ""
                                                    : cache.FirstOrDefault(z => z.MO_No == y.MO_No && z.MO_Seq == y.MO_Seq && z.Material_ID == y.Material_ID).Part_No,
                                    Part_Name = cache.FirstOrDefault(z => z.MO_No == y.MO_No && z.MO_Seq == y.MO_Seq && z.Material_ID == y.Material_ID) == null ? ""
                                                    : cache.FirstOrDefault(z => z.MO_No == y.MO_No && z.MO_Seq == y.MO_Seq && z.Material_ID == y.Material_ID).Part_Name.Trim(),
                                    Stock_Qty = transactionDetail.Where(z => z.Transac_No == x.TransactionMainQrCodeMain.TransactionMain.Transac_No).Sum(z => z.Instock_Qty)
                                });
            var data = await dataQuery.ToListAsync();
            return data;
        }

        public async Task<List<MergeQrCodeModel>> SearchTransactionForOtherSplit(string moNo, string qrCodeID)
        {
            // tìm những đơn còn hiệu lực: Can_Move == "Y" và không phải là những được được nhận từ tách đơn Reason_Code != "ZZZ"
            var transactionMain = _repoTransactionMain.FindAll(x => x.Can_Move == "Y" && x.Reason_Code.Trim() != "ZZZ");
            var qrCodeMain = _repoQRCodeMain.FindAll(x => x.Valid_Status == "Y");
            var packingList = _repoPackingList.FindAll();
            var cache = _repoCache.FindAll();
            var transactionDetail = _repoTransactionDetail.FindAll();

            if (!string.IsNullOrEmpty(moNo))
            {
                transactionMain = transactionMain.Where(x => x.MO_No.Trim() == moNo.Trim());
            }
            if (!string.IsNullOrEmpty(qrCodeID))
            {
                transactionMain = transactionMain.Where(x => x.QRCode_ID.Trim() == qrCodeID.Trim());
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
                                (x, y) => new MergeQrCodeModel
                                {
                                    Transac_No = x.TransactionMainQrCodeMain.TransactionMain.Transac_No,
                                    QRCode_ID = x.TransactionMainQrCodeMain.TransactionMain.QRCode_ID,
                                    MO_No = x.TransactionMainQrCodeMain.TransactionMain.MO_No,
                                    Material_ID = x.TransactionMainQrCodeMain.TransactionMain.Material_ID,
                                    Material_Name = x.TransactionMainQrCodeMain.TransactionMain.Material_Name,
                                    Rack_Location = x.TransactionMainQrCodeMain.TransactionMain.Rack_Location,
                                    Model_No = y.Model_No,
                                    Model_Name = y.Model_Name,
                                    Article = y.Article,
                                    Supplier_ID = y.Supplier_ID,
                                    Supplier_Name = y.Supplier_Name,
                                    Part_No = cache.FirstOrDefault(z => z.MO_No == y.MO_No && z.MO_Seq == y.MO_Seq && z.Material_ID == y.Material_ID) == null ? ""
                                                    : cache.FirstOrDefault(z => z.MO_No == y.MO_No && z.MO_Seq == y.MO_Seq && z.Material_ID == y.Material_ID).Part_No,
                                    Part_Name = cache.FirstOrDefault(z => z.MO_No == y.MO_No && z.MO_Seq == y.MO_Seq && z.Material_ID == y.Material_ID) == null ? ""
                                                    : cache.FirstOrDefault(z => z.MO_No == y.MO_No && z.MO_Seq == y.MO_Seq && z.Material_ID == y.Material_ID).Part_Name.Trim(),
                                    Stock_Qty = transactionDetail.Where(z => z.Transac_No == x.TransactionMainQrCodeMain.TransactionMain.Transac_No).Sum(z => z.Instock_Qty)
                                });
            var data = await dataQuery.ToListAsync();
            return data;
        }

        public async Task<SplitQrCodeMain_Dto> SplitInfoDetail(string moNo, string transacNo)
        {
            var transactionMain = _repoTransactionMain.FindAll();
            var transactionMainParent = _repoTransactionMain.FindAll(x => x.Transac_No == transacNo);
            // tìm những đơn con của đơn đó đã được tách cho Merge_Transac_No == transacNo && Reason_Code == "ZZZ" && Transac_Type == "I"
            var transactionMainChild = _repoTransactionMain.FindAll(x => x.Merge_Transac_No == transacNo && x.Reason_Code == "ZZZ" && x.Transac_Type == "I");
            var transactionDetail = _repoTransactionDetail.FindAll();

            var splitParent = await transactionMainParent.Select(x => new SplitQrCodeDetail
            {
                Material_ID = x.Material_ID,
                Material_Name = x.Material_Name,
                MO_No = x.MO_No,
                MO_Seq = x.MO_Seq,
                Rack_Location = x.Rack_Location,
                Transac_No = x.Transac_No,
                Transac_Type = x.Transac_Type.Trim(),
                QRCode_ID = x.QRCode_ID,
                QRCode_Version = x.QRCode_Version,
                Updated_By = x.Updated_By,
                // số lượng còn lại của đơn cha sau khi đã trừ phần được tách
                Stock_Qty = transactionMain.Where(z => z.Transac_No == x.Transac_No && x.Transac_Type == "MG").FirstOrDefault() != null
                        ? transactionDetail.Where(z => z.Transac_No == x.Transac_No).Sum(z => z.Instock_Qty)
                        : transactionMain.Where(z => z.QRCode_ID == x.QRCode_ID && z.QRCode_Version == x.QRCode_Version + 1 && z.Can_Move == "Y").FirstOrDefault() == null
                        ? 0
                        : transactionDetail.Where(z => z.Transac_No == transactionMain.Where(z => z.QRCode_ID == x.QRCode_ID && z.QRCode_Version == x.QRCode_Version + 1 && z.Can_Move == "Y").FirstOrDefault().Transac_No).Sum(z => z.Instock_Qty)
            }).FirstOrDefaultAsync();

            var splitChild = await transactionMainChild.Select(x => new SplitQrCodeDetail
            {
                Material_ID = x.Material_ID,
                Material_Name = x.Material_Name,
                MO_No = x.MO_No,
                MO_Seq = x.MO_Seq,
                Rack_Location = x.Rack_Location,
                Transac_No = x.Transac_No,
                QRCode_ID = x.QRCode_ID,
                QRCode_Version = x.QRCode_Version,
                Updated_By = x.Updated_By,
                PreBuy_MO_No = splitParent.MO_No,
                Split_Time = x.Transac_Time,
                Stock_Qty = transactionDetail.Where(z => z.Transac_No == x.Transac_No).Sum(z => z.Instock_Qty)
            }).ToListAsync();

            SplitQrCodeMain_Dto result = new SplitQrCodeMain_Dto();
            result.SplitPlanNoParent = splitParent;
            result.SplitPlanNoChild = splitChild;
            return result;
        }

        // Process của đơn mẹ hiển thị ra
        public async Task<SplitProcess_Dto> SplitProcess(string transacNo)
        {
            var transactionMain = _repoTransactionMain.FindAll(x => x.Transac_No == transacNo && x.Can_Move == "Y");
            var qrCodeMain = _repoQRCodeMain.FindAll(x => x.Valid_Status == "Y");
            var packingList = _repoPackingList.FindAll();
            var cache = _repoCache.FindAll();
            var transactionDetail = _repoTransactionDetail.FindAll();

            var transacMainMergeQrCode = await transactionMain
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
                                (x, y) => new MergeQrCodeModel
                                {
                                    Transac_No = x.TransactionMainQrCodeMain.TransactionMain.Transac_No,
                                    QRCode_ID = x.TransactionMainQrCodeMain.TransactionMain.QRCode_ID,
                                    MO_No = x.TransactionMainQrCodeMain.TransactionMain.MO_No,
                                    Material_ID = x.TransactionMainQrCodeMain.TransactionMain.Material_ID,
                                    Material_Name = x.TransactionMainQrCodeMain.TransactionMain.Material_Name,
                                    Model_No = y.Model_No,
                                    Model_Name = y.Model_Name,
                                    Article = y.Article,
                                    Transac_Type = x.TransactionMainQrCodeMain.TransactionMain.Transac_Type.Trim()
                                }).FirstOrDefaultAsync();

            var listSizeAndQty = await transactionDetail.Where(x => x.Transac_No == transacNo).Select(x => new SizeAndQty
            {
                Order_Size = x.Order_Size,
                Model_Size = x.Model_Size,
                Tool_Size = x.Tool_Size,
                Act_Out_Qty = 0,
                Trans_Qty = x.Trans_Qty,
                Instock_Qty = transacMainMergeQrCode.Transac_Type == "MG" ? x.Qty : x.Instock_Qty
            }).OrderBy(x => x.Tool_Size).ToListAsync();

            // Kiểm tra xem mã đó đã tách bao giờ chưa, tính số lượng đã tách
            var listTransacmainSplited = await _repoTransactionMain.FindAll(x => x.Merge_Transac_No.Trim() == transacNo.Trim() && x.Transac_Type == "I" && x.Reason_Code == "ZZZ")
                    .Join(transactionDetail, x => x.Transac_No, y => y.Transac_No, (x, y) => new
                    {
                        y.Tool_Size,
                        y.Order_Size,
                        y.Model_Size,
                        y.Instock_Qty
                    }).GroupBy(x => new { Tool_Size = x.Tool_Size.Trim(), Model_Size = x.Model_Size.Trim(), Order_Size = x.Order_Size.Trim() })
                        .Select(x => new
                        {
                            x.Key.Tool_Size,
                            x.Key.Order_Size,
                            x.Key.Model_Size,
                            Instock_Qty = x.Sum(x => x.Instock_Qty)
                        }).ToListAsync();
            // nếu đơn đó được tách rồi tính số lượng đã tách của đơn đó Act_Out_Qty
            if (listTransacmainSplited.Any())
            {
                listSizeAndQty = listSizeAndQty.GroupJoin(listTransacmainSplited, x => new { Tool_Size = x.Tool_Size.Trim(), Order_Size = x.Order_Size.Trim(), Model_Size = x.Model_Size.Trim() },
                                    y => new { Tool_Size = y.Tool_Size.Trim(), Order_Size = y.Order_Size.Trim(), Model_Size = y.Model_Size.Trim() },
                                    (x, y) => new
                                    {
                                        x,
                                        y
                                    }).SelectMany(x => x.y.DefaultIfEmpty(),
                                        (x, y) =>
                                        {
                                            x.x.Act_Out_Qty = y == null ? 0 : y.Instock_Qty;
                                            return x.x;
                                        }).ToList();
            }
            //////

            var listOffsetNo = await _repoMaterialOffset.FindAll(x => x.DMO_No == transacMainMergeQrCode.MO_No && x.Material_ID == transacMainMergeQrCode.Material_ID)
                                    .Select(x => x.Offset_No).Distinct().ToListAsync();

            SplitProcess_Dto result = new SplitProcess_Dto();
            result.TransacMainMergeQrCode = transacMainMergeQrCode;
            result.ListSizeAndQty = listSizeAndQty.OrderBy(x => x.Tool_Size).ThenBy(x => x.Order_Size).ToList();
            result.ListOffsetNo = listOffsetNo;
            return result;
        }

        public async Task<List<SplitDataByOffset_Dto>> GetDataSplitByOffsetNo(string offsetNo, string materialId, string moNo, string transacNoParent)
        {
            var mesMoBasicCRDAndSTF = await _repoMesMoBasic.FindAll().Select(x => new { x.CRD, x.Plan_Start_STF, x.MO_No, x.MO_Seq }).ToListAsync();

            var transactionMainRackLocation = _repoTransactionMain.FindAll(x => x.Transac_No == transacNoParent).Select(x => x.Rack_Location).FirstOrDefault();
            var transactionDetail = _repoTransactionDetail.FindAll();

            var toolSizeOfTransacnoParent = await _repoTransactionDetail.FindAll(x => x.Transac_No == transacNoParent).Select(x => new
            {
                x.Tool_Size,
                x.Model_Size,
                x.Order_Size
            }).ToListAsync();

            var pred_Material_Offset = PredicateBuilder.New<WMSB_Material_Offset>(true);
            pred_Material_Offset.And(x => x.DMO_No.Trim() == moNo.Trim() && x.Material_ID.Trim() == materialId.Trim());
            if(offsetNo != "all") {
                pred_Material_Offset.And(x => x.Offset_No.Trim() == offsetNo.Trim());
            }
            var dataNeedSplitByOffsetAndMaterialId = await _repoMaterialOffset.FindAll(pred_Material_Offset).ToListAsync();

            var dataNeedSplitByOffsetAndMaterialIdGroup = dataNeedSplitByOffsetAndMaterialId.GroupBy(x => new { MO_No = x.MO_No.Trim(), DMO_No = x.DMO_No.Trim(), Material_ID = x.Material_ID.Trim(), MO_Seq = x.MO_Seq.Trim() });

            var dataNeedSplitByOffsetAndMaterialIdForDisplay = dataNeedSplitByOffsetAndMaterialIdGroup.Select(x => new SplitDataByOffset_Dto
            {
                DTransac_No = transacNoParent,
                DMO_No = x.Key.DMO_No,
                MO_No = x.Key.MO_No,
                MO_Seq = x.Key.MO_Seq,
                Material_ID = x.Key.Material_ID,
                Rack_Location = transactionMainRackLocation,
                Plan_Start_STF = mesMoBasicCRDAndSTF.Where(z => z.MO_Seq.Trim() == x.Key.MO_Seq.Trim() && z.MO_No.Trim() == x.Key.MO_No.Trim()).Select(x => x.Plan_Start_STF).FirstOrDefault(),
                CRD = mesMoBasicCRDAndSTF.Where(z => z.MO_Seq.Trim() == x.Key.MO_Seq.Trim() && z.MO_No.Trim() == x.Key.MO_No.Trim()).Select(x => x.CRD).FirstOrDefault(),
                ListSizeAndQty = dataNeedSplitByOffsetAndMaterialId.Where(z => z.MO_No.Trim() == x.Key.MO_No.Trim() && z.MO_Seq.Trim() == x.Key.MO_Seq.Trim())
                                .Select(t => new SizeAndQty
                                {
                                    Tool_Size = t.Tool_Size,
                                    Order_Size = t.Order_Size,
                                    Model_Size = t.Model_Size,
                                    MO_Qty = t.MO_Qty,
                                    Purchase_Qty = t.PreBook_Qty,
                                    Instock_Qty = t.PreBook_Qty,
                                    Already_Offset_Qty = 0,
                                    Offset_Qty = t.PreBook_Qty
                                }).OrderBy(x => x.Tool_Size).ToList()
            }).ToList();

            // kiểm tra đơn cần nhận xem trong kho còn bao nhiêu và đã out bao nhiêu, nếu có thì trừ số lượng được nhận này
            // theo MO_No, MO_Seq, Material_ID
            foreach (var item in dataNeedSplitByOffsetAndMaterialIdForDisplay)
            {
                // tìm những đơn còn trong kho canmove = Y rồi trừ đi Instock_Qty
                var itemTransactionInput = await _repoTransactionMain.FindAll(x => x.MO_No.Trim() == item.MO_No.Trim() && x.MO_Seq.Trim() == item.MO_Seq.Trim()
                                                        && x.Material_ID.Trim() == item.Material_ID.Trim() && x.Can_Move == "Y")
                    .Join(transactionDetail, x => x.Transac_No, y => y.Transac_No, (x, y) => new
                    {
                        y.Tool_Size,
                        y.Order_Size,
                        y.Model_Size,
                        y.Instock_Qty
                    }).GroupBy(x => new { Tool_Size = x.Tool_Size.Trim(), Model_Size = x.Model_Size.Trim(), Order_Size = x.Order_Size.Trim() })
                        .Select(x => new
                        {
                            x.Key.Tool_Size,
                            x.Key.Order_Size,
                            x.Key.Model_Size,
                            Instock_Qty = x.Sum(x => x.Instock_Qty)
                        }).ToListAsync();
                if (itemTransactionInput.Any())
                {
                    item.ListSizeAndQty = item.ListSizeAndQty.Join(itemTransactionInput, x => new { Tool_Size = x.Tool_Size.Trim(), Order_Size = x.Order_Size.Trim(), Model_Size = x.Model_Size.Trim() },
                                        y => new { Tool_Size = y.Tool_Size.Trim(), Order_Size = y.Order_Size.Trim(), Model_Size = y.Model_Size.Trim() },
                                        (x, y) =>
                                        {
                                            x.Instock_Qty = (x.Instock_Qty - y.Instock_Qty) > 0 ? (x.Instock_Qty - y.Instock_Qty) : 0;
                                            x.Already_Offset_Qty = x.Already_Offset_Qty + y.Instock_Qty;
                                            return x;
                                        }).ToList();
                }
                // tìm những đơn đã xuất kệ rồi type = O rồi trừ đi Trans_Qty
                var itemTransactionOutput = await _repoTransactionMain.FindAll(x => x.MO_No.Trim() == item.MO_No.Trim() && x.MO_Seq.Trim() == item.MO_Seq.Trim()
                                                        && x.Material_ID.Trim() == item.Material_ID.Trim() && x.Transac_Type == "O")
                    .Join(transactionDetail, x => x.Transac_No, y => y.Transac_No, (x, y) => new
                    {
                        y.Tool_Size,
                        y.Order_Size,
                        y.Model_Size,
                        y.Trans_Qty
                    }).GroupBy(x => new { Tool_Size = x.Tool_Size.Trim(), Model_Size = x.Model_Size.Trim(), Order_Size = x.Order_Size.Trim() })
                        .Select(x => new
                        {
                            x.Key.Tool_Size,
                            x.Key.Order_Size,
                            x.Key.Model_Size,
                            Trans_Qty = x.Sum(x => x.Trans_Qty)
                        }).ToListAsync();
                if (itemTransactionOutput.Any())
                {
                    item.ListSizeAndQty = item.ListSizeAndQty.Join(itemTransactionOutput, x => new { Tool_Size = x.Tool_Size.Trim(), Order_Size = x.Order_Size.Trim(), Model_Size = x.Model_Size.Trim() },
                                        y => new { Tool_Size = y.Tool_Size.Trim(), Order_Size = y.Order_Size.Trim(), Model_Size = y.Model_Size.Trim() },
                                        (x, y) =>
                                        {
                                            x.Instock_Qty = (x.Instock_Qty - y.Trans_Qty) > 0 ? (x.Instock_Qty - y.Trans_Qty) : 0;
                                            x.Already_Offset_Qty = x.Already_Offset_Qty + y.Trans_Qty;
                                            return x;
                                        }).ToList();
                }
            }
            ///////

            // kiểm tra đơn mẹ có nhưng size nào mà đơn con không có thì tự thêm vào vs Instock_Qty = null
            foreach (var item in toolSizeOfTransacnoParent)
            {
                foreach (var item1 in dataNeedSplitByOffsetAndMaterialIdForDisplay)
                {
                    if (!(item1.ListSizeAndQty.Where(x => x.Tool_Size.Trim() == item.Tool_Size.Trim() && x.Order_Size.Trim() == item.Order_Size.Trim() && x.Model_Size.Trim() == item.Model_Size.Trim()).Any()))
                    {
                        item1.ListSizeAndQty.Add(new SizeAndQty { Tool_Size = item.Tool_Size, Model_Size = item.Model_Size, Order_Size = item.Order_Size, Instock_Qty = null });
                    }
                }
            }
            ////


            // sắp xếp kết quả toolsize tăng dần
            foreach (var item in dataNeedSplitByOffsetAndMaterialIdForDisplay)
            {
                item.ListSizeAndQty = item.ListSizeAndQty.OrderBy(x => x.Tool_Size).ToList();
                item.SumInstockQty = item.ListSizeAndQty.Sum(x => x.Instock_Qty);
                item.SumMOQty = item.ListSizeAndQty.Sum(x => x.MO_Qty);
                item.SumAlreadyOffsetQty = item.ListSizeAndQty.Sum(x => x.Already_Offset_Qty);
                item.SumOffsetQty = item.ListSizeAndQty.Sum(x => x.Offset_Qty);
            }

            return dataNeedSplitByOffsetAndMaterialIdForDisplay.Where(x => x.SumInstockQty > 0).OrderBy(x => x.Plan_Start_STF).ToList();
        }

        // Other Split => Add MoNo con
        public async Task<SplitDataByOffset_Dto> GetDataOtherSplitByMONo(string materialId, string moNo, string moSeq, string transacNoParent, string dMoNo)
        {
            moSeq = moSeq ?? "";
            var transactionDetail = _repoTransactionDetail.FindAll();

            // kiểm tra trong bảng materialpurchase có đơn này ko, theo MO_No, Material_ID, MO_Seq, nếu có lấy trong bảng này ra luôn
            // trừ rồi trừ số lượng ra: trừ số lượng trong kho canmove="Y" và số lượng đã out ra transac_type="O"
            var checkMoNoInMaterialPurchase = await _repoMaterialPurchase.FindAll(x => x.MO_No.Trim() == moNo.Trim() 
                                                                                && x.Material_ID.Trim() == materialId.Trim() 
                                                                                && x.MO_Seq.Trim() == moSeq.Trim()).AnyAsync();

            // kiểm tra đơn cần nhận xem trong kho còn bao nhiêu và đã out bao nhiêu, nếu có thì trừ số lượng được nhận này
            // theo MO_No, MO_Seq, Material_ID
            var itemTransactionInput = await _repoTransactionMain.FindAll(x => x.MO_No.Trim() == moNo.Trim() 
                                                        && x.MO_Seq.Trim() == moSeq.Trim()
                                                        && x.Material_ID.Trim() == materialId.Trim())
                    .Join(transactionDetail, x => x.Transac_No, y => y.Transac_No, (x, y) => new
                    {
                        y.Tool_Size,
                        y.Order_Size,
                        y.Model_Size,
                        y.Instock_Qty
                    }).GroupBy(x => new { Tool_Size = x.Tool_Size.Trim(), Model_Size = x.Model_Size.Trim(), Order_Size = x.Order_Size.Trim() })
                        .Select(x => new
                        {
                            x.Key.Tool_Size,
                            x.Key.Order_Size,
                            x.Key.Model_Size,
                            Instock_Qty = x.Sum(x => x.Instock_Qty)
                        }).ToListAsync();

            if (checkMoNoInMaterialPurchase)
            {
                var rackLocation = (await _repoTransactionMain.FindAll(x => x.MO_No.Trim() == moNo.Trim() && x.Material_ID.Trim() == materialId.Trim()).Select(x => x.Rack_Location).FirstOrDefaultAsync())
                                    ?? await _repoTransactionMain.FindAll(x => x.Transac_No == transacNoParent).Select(x => x.Rack_Location).FirstOrDefaultAsync();
                var toolSizeOfTransacnoParent = await _repoTransactionDetail.FindAll(x => x.Transac_No == transacNoParent).Select(x => new
                {
                    x.Tool_Size,
                    x.Model_Size,
                    x.Order_Size
                }).ToListAsync();

                var dataNeedOtherSplit = await _repoMaterialPurchase.FindAll(x => x.MO_No.Trim() == moNo.Trim() && x.Material_ID.Trim() == materialId.Trim() && x.MO_Seq.Trim() == moSeq.Trim()).ToListAsync();
                var dataNeedOtherSplitForDisplay = new SplitDataByOffset_Dto
                {
                    DTransac_No = transacNoParent,
                    DMO_No = dMoNo,
                    MO_No = moNo,
                    MO_Seq = moSeq,
                    Material_ID = materialId,
                    Rack_Location = rackLocation,
                    ListSizeAndQty = dataNeedOtherSplit
                                    .Select(t => new SizeAndQty
                                    {
                                        Tool_Size = t.Tool_Size,
                                        Order_Size = t.Order_Size,
                                        Model_Size = t.Model_Size,
                                        MO_Qty = t.MO_Qty,
                                        Purchase_Qty = t.MO_Qty,
                                        Instock_Qty = 0
                                    }).OrderBy(x => x.Tool_Size).ToList()
                };

                // Trừ đi số lượng đã nhận những lần ban đầu để hiển thị số lượng tối đa có thể nhận được
                if (itemTransactionInput.Any())
                {
                    dataNeedOtherSplitForDisplay.ListSizeAndQty = dataNeedOtherSplitForDisplay.ListSizeAndQty.Join(itemTransactionInput, x => new { Tool_Size = x.Tool_Size.Trim(), Order_Size = x.Order_Size.Trim(), Model_Size = x.Model_Size.Trim() },
                                        y => new { Tool_Size = y.Tool_Size.Trim(), Order_Size = y.Order_Size.Trim(), Model_Size = y.Model_Size.Trim() },
                                        (x, y) =>
                                        {
                                            x.Purchase_Qty = x.Purchase_Qty - y.Instock_Qty;
                                            return x;
                                        }).ToList();
                }
                // tìm những đơn đã xuất kệ rồi type = O rồi trừ đi Trans_Qty
                var itemTransactionOutput = await _repoTransactionMain.FindAll(x => x.MO_No.Trim() == dataNeedOtherSplitForDisplay.MO_No.Trim() && x.MO_Seq.Trim() == dataNeedOtherSplitForDisplay.MO_Seq.Trim()
                                                        && x.Material_ID.Trim() == dataNeedOtherSplitForDisplay.Material_ID.Trim() && x.Transac_Type == "O")
                    .Join(transactionDetail, x => x.Transac_No, y => y.Transac_No, (x, y) => new
                    {
                        y.Tool_Size,
                        y.Order_Size,
                        y.Model_Size,
                        y.Trans_Qty
                    }).GroupBy(x => new { Tool_Size = x.Tool_Size.Trim(), Model_Size = x.Model_Size.Trim(), Order_Size = x.Order_Size.Trim() })
                        .Select(x => new
                        {
                            x.Key.Tool_Size,
                            x.Key.Order_Size,
                            x.Key.Model_Size,
                            Trans_Qty = x.Sum(x => x.Trans_Qty)
                        }).ToListAsync();
                if (itemTransactionOutput.Any())
                {
                    dataNeedOtherSplitForDisplay.ListSizeAndQty = dataNeedOtherSplitForDisplay.ListSizeAndQty.Join(itemTransactionOutput, x => new { Tool_Size = x.Tool_Size.Trim(), Order_Size = x.Order_Size.Trim(), Model_Size = x.Model_Size.Trim() },
                                        y => new { Tool_Size = y.Tool_Size.Trim(), Order_Size = y.Order_Size.Trim(), Model_Size = y.Model_Size.Trim() },
                                        (x, y) =>
                                        {
                                            x.Purchase_Qty = x.Purchase_Qty - y.Trans_Qty;
                                            return x;
                                        }).ToList();
                }
                ///////

                // kiểm tra đơn mẹ có nhưng size nào mà đơn con không có thì tự thêm vào vs Instock_Qty = null
                foreach (var item in toolSizeOfTransacnoParent)
                {
                    if (!(dataNeedOtherSplitForDisplay.ListSizeAndQty.Where(x => x.Tool_Size.Trim() == item.Tool_Size.Trim() && x.Order_Size.Trim() == item.Order_Size.Trim() && x.Model_Size.Trim() == item.Model_Size.Trim()).Any()))
                    {
                        dataNeedOtherSplitForDisplay.ListSizeAndQty.Add(new SizeAndQty { Tool_Size = item.Tool_Size, Model_Size = item.Model_Size, Order_Size = item.Order_Size, Instock_Qty = null });
                    }
                }
                ////

                // sắp xếp kết quả toolsize tăng dần
                dataNeedOtherSplitForDisplay.ListSizeAndQty = dataNeedOtherSplitForDisplay.ListSizeAndQty.OrderBy(x => x.Tool_Size).ToList();

                return dataNeedOtherSplitForDisplay.ListSizeAndQty.Sum(x => x.Purchase_Qty) == 0 ? null : dataNeedOtherSplitForDisplay;
            }
            // ngược lại nếu không có vào dbMES bảng MES_MO_Size lấy thông tin ra
            else
            {
                // kiểm tra trong materialpurchase có materialid này chưa, chưa có thì vào racknet kiểm tra ngược lại có rồi vào mesmosize kiểm tra
                var checkMaterialIdInMaterialPurchase = await _repoMaterialPurchase.FindAll(x => x.MO_No.Trim() == moNo.Trim() && x.Material_ID.Trim() == materialId.Trim()).AnyAsync();
                bool checkMonoInMesOrRackNet = false;
                if (!checkMaterialIdInMaterialPurchase)
                {

                    var repoMaterials = _repoMaterials.FindAll();
                    var repoPoMaterials = _repoPoMaterials.FindAll();
                    var repoPoRoots = _repoPoRoots.FindAll(x => x.Manno.Trim() == moNo.Trim() && x.Batch.Trim() == moSeq.Trim());
                    checkMonoInMesOrRackNet = await repoPoRoots.Join(repoPoMaterials, x => x.Id, y => y.PoRootId, (x, y) => y.MaterialId)
                                                .Join(repoMaterials, x => x, y => y.Id, (x, y) => new { x, y }).Distinct().Where(x => x.y.Itnbr.Trim() == materialId.Trim()).AnyAsync();
                }
                else
                {
                    checkMonoInMesOrRackNet = await _repoMesMoSize.FindAll(x => x.MO_No.Trim() == moNo.Trim() && x.MO_Seq.Trim() == moSeq.Trim())
                                    .AnyAsync();
                }

                // if (checkMonoInMesOrRackNet)
                // {
                    var toolSizeOfTransacnoParent = await _repoTransactionDetail.FindAll(x => x.Transac_No == transacNoParent).Select(x => new
                    {
                        x.Tool_Size,
                        x.Model_Size,
                        x.Order_Size
                    }).ToListAsync();

                    var dataNeedOtherSplit = await _repoMesMoSize.FindAll(x => x.MO_No.Trim() == moNo.Trim() && x.MO_Seq.Trim() == moSeq.Trim())
                                    .GroupBy(x => new { MO_No = x.MO_No.Trim(), MO_Seq = x.MO_Seq.Trim(), Size_Code = x.Size_Code.Trim(), Size_TCode = x.Size_TCode.Trim() })
                                    .Select(t => new SizeAndQty
                                    {
                                        Tool_Size = t.Key.Size_TCode,
                                        Order_Size = t.Key.Size_Code,
                                        Model_Size = t.Key.Size_Code,
                                        MO_Qty = t.Sum(x => x.Plan_Qty),
                                        Purchase_Qty = t.Sum(x => x.Plan_Qty),
                                        Instock_Qty = 0
                                    }).ToListAsync();

                    // kiểm tra đơn mẹ có nhưng size nào mà đơn con không có thì tự thêm vào vs Instock_Qty = null
                    var listSizeAndQty = toolSizeOfTransacnoParent.GroupJoin(
                                                dataNeedOtherSplit, 
                                                x => new { Order_Size = x.Order_Size.Trim() }, 
                                                y => new { Order_Size = y.Order_Size.Trim() },
                                    (x, y) => new { x, y })
                                    .SelectMany(x => x.y.DefaultIfEmpty(),
                                        (x, y) => new SizeAndQty
                                        {
                                            Tool_Size = x.x.Tool_Size,
                                            Order_Size = x.x.Order_Size,
                                            Model_Size = x.x.Model_Size,
                                            MO_Qty = y == null ? null : y.MO_Qty,
                                            Purchase_Qty = y == null ? null : y.Purchase_Qty,
                                            Instock_Qty = y == null ? null : y.Instock_Qty
                                        }).ToList();

                    // Trừ đi số lượng đã nhận những lần ban đầu để hiển thị số lượng tối đa có thể nhận được
                    if(itemTransactionInput.Any()) {
                        listSizeAndQty.Join(
                            itemTransactionInput,
                            x => new {Order_Size = x.Order_Size, Model_Size =  x.Model_Size,Tool_Size = x.Tool_Size},
                            y => new {Order_Size = y.Order_Size, Model_Size =  y.Model_Size,Tool_Size = y.Tool_Size},
                            (x,y) => {
                                x.Instock_Qty = x.Instock_Qty - y.Instock_Qty;
                                return x;
                            }
                        );
                    }

                    var rackLocation = (await _repoTransactionMain.FindAll(x => x.MO_No.Trim() == moNo.Trim() 
                                                                            && x.Material_ID.Trim() == materialId.Trim())
                                                                            .Select(x => x.Rack_Location).FirstOrDefaultAsync())
                                    ?? await _repoTransactionMain.FindAll(x => x.Transac_No == transacNoParent).Select(x => x.Rack_Location).FirstOrDefaultAsync();

                    var dataNeedOtherSplitForDisplay = new SplitDataByOffset_Dto
                    {
                        DTransac_No = transacNoParent,
                        DMO_No = dMoNo,
                        MO_No = moNo,
                        MO_Seq = moSeq,
                        Material_ID = materialId,
                        Rack_Location = rackLocation,
                        ListSizeAndQty = listSizeAndQty
                    };
                    dataNeedOtherSplitForDisplay.ListSizeAndQty = dataNeedOtherSplitForDisplay.ListSizeAndQty.OrderBy(x => x.Tool_Size).ThenBy(x => x.Order_Size).ToList();
                    return dataNeedOtherSplitForDisplay.ListSizeAndQty.Sum(x => x.Purchase_Qty) == 0 ? null : dataNeedOtherSplitForDisplay;
                //}
                // else
                // {
                //     return null;
                // }
            }
        }

        public async Task<bool> SaveSplitData(List<SplitDataByOffset_Dto> dataSplit, string updateBy, bool otherSplit = false)
        {
            var firstDataSplit = dataSplit.FirstOrDefault();
            var packingParent = await _repoPackingList.FindAll(x => x.Material_ID.Trim() == firstDataSplit.Material_ID.Trim()
                                    // && x.MO_Seq.Trim() == firstDataSplit.MO_Seq.Trim()
                                    && x.MO_No.Trim() == firstDataSplit.DMO_No).FirstOrDefaultAsync();
            var transactionMainParent = await _repoTransactionMain.FindAll(x => x.Transac_No.Trim() == firstDataSplit.DTransac_No).FirstOrDefaultAsync();
            var timeNow = DateTime.Now;
            var transacNoNewList = new List<string>();
            var transacSheetNoNewList = new List<string>();
            var qrCodeIDNewList = new List<string>();

            var materialPurchaseParent = await _repoMaterialPurchase.FindAll(x => x.MO_No.Trim() == firstDataSplit.DMO_No.Trim() &&
                                                                        x.MO_Seq == firstDataSplit.MO_Seq &&
                                                                        x.Material_ID.Trim() == firstDataSplit.Material_ID.Trim() &&
                                                                        x.Purchase_No.Trim() == transactionMainParent.Purchase_No.Trim()).FirstOrDefaultAsync();

            foreach (var item in dataSplit)
            {
                // Khi thêm thì lấy toàn bộ size của PO + batch + Material_ID đó thuộc MaterialPurchase.Nếu bảng MaterialPurchase ko có 
                // thì vào Mes lấy toàn bộ Size của PO + batch 
                var materialPurchaseOfSizeInPo = new List<SizeOfPo>();
                materialPurchaseOfSizeInPo = await _repoMaterialPurchase.FindAll(x => x.MO_No.Trim() == item.MO_No.Trim() &&
                                                                                    x.MO_Seq.Trim() == item.MO_Seq.Trim() && 
                                                                                    x.Material_ID.Trim() == item.Material_ID.Trim()).Select(x => new SizeOfPo {
                                                                                        Tool_Size = x.Tool_Size.Trim(),
                                                                                        Order_Size = x.Order_Size.Trim(), 
                                                                                        Model_Size = x.Model_Size.Trim()
                                                                                    }).Distinct().ToListAsync();
                if(!materialPurchaseOfSizeInPo.Any()) {
                    materialPurchaseOfSizeInPo = await _repoMesMoSize.FindAll(x => x.MO_No.Trim() == item.MO_No.Trim() && 
                                                                                x.MO_Seq.Trim() == item.MO_Seq.Trim()).Select(x => new SizeOfPo {
                                                                                    Tool_Size = x.Size_TCode.Trim(),
                                                                                    Order_Size = x.Size_Code.Trim(),
                                                                                    Model_Size = x.Size_Code.Trim()
                                                                                }).Distinct().ToListAsync();
                }
                
                var itemListSizeAndQtyHaveInstockNotNull = item.ListSizeAndQty.Where(x => x.Instock_Qty != null).ToList();
                foreach (var itemSize in materialPurchaseOfSizeInPo)
                {
                    var sizeCheckIn = itemListSizeAndQtyHaveInstockNotNull.Where(x => x.Tool_Size.Trim() == itemSize.Tool_Size.Trim() && 
                                                                                x.Order_Size.Trim() == itemSize.Order_Size.Trim() &&
                                                                                x.Model_Size.Trim() == itemSize.Model_Size.Trim());
                    if(!sizeCheckIn.Any()) {
                        var addSizeEmpty = new SizeAndQty();
                        addSizeEmpty.Tool_Size = itemSize.Tool_Size;
                        addSizeEmpty.Order_Size = itemSize.Order_Size;
                        addSizeEmpty.Model_Size = itemSize.Model_Size;
                        addSizeEmpty.Instock_Qty = 0;
                        addSizeEmpty.Act_Out_Qty = 0;
                        addSizeEmpty.MO_Qty = 0;
                        addSizeEmpty.Purchase_Qty = 0;
                        addSizeEmpty.Offset_Qty = 0;
                        itemListSizeAndQtyHaveInstockNotNull.Add(addSizeEmpty);
                    }
                }

                // Thêm vào bảng Material Purchase Split
                var materialPurchaseSplitModel = new WMSB_Material_Purchase_Split();
                materialPurchaseSplitModel.MO_No = item.MO_No;
                materialPurchaseSplitModel.Purchase_No = materialPurchaseParent == null ? null : materialPurchaseParent.Purchase_No;
                materialPurchaseSplitModel.MO_Seq = item.MO_Seq;
                materialPurchaseSplitModel.Purchase_Date = materialPurchaseParent == null ? null : materialPurchaseParent.Purchase_Date;
                materialPurchaseSplitModel.Confirm_Delivery = materialPurchaseParent == null ? null : materialPurchaseParent.Confirm_Delivery;
                materialPurchaseSplitModel.Custmoer_Part = materialPurchaseParent == null ? null : materialPurchaseParent.Custmoer_Part;
                materialPurchaseSplitModel.Supplier_ID = materialPurchaseParent == null ? null : materialPurchaseParent.Supplier_ID;
                materialPurchaseSplitModel.Updated_By = updateBy;
                materialPurchaseSplitModel.Updated_Time = timeNow;
                _repoMaterialPurchaseSplit.Add(materialPurchaseSplitModel);


                //------------------------Thêm vào 2 bảng Packing_List và Packing_List_Detail------------------//
                var packingListAddRecieveNo = AddPackingList(firstDataSplit.DMO_No ,item, packingParent, itemListSizeAndQtyHaveInstockNotNull, timeNow, updateBy);
                //////////////
                //===================================================Add QrCode Main - Detail===================================================//
                var qrCodeIdNew = "";
                do
                {
                    var po = item.MO_No.Trim().Length == 9 ? item.MO_No.Trim() + "Z" : item.MO_No.Trim();
                    string so = CodeUtility.RandomNumber(3);
                    qrCodeIdNew = "S" + po + so + CodeUtility.RandomStringUpper(1);
                } while (await this.CheckQrCodeID(qrCodeIdNew) || (qrCodeIDNewList.Contains(qrCodeIdNew)));
                qrCodeIDNewList.Add(qrCodeIdNew);

                AddQrCode(transactionMainParent.QRCode_ID ,qrCodeIdNew, packingListAddRecieveNo, itemListSizeAndQtyHaveInstockNotNull, timeNow, updateBy);
                // ========================================Add vào bảng TransactionMain - Detail========================================//

                var transacNoNew = "";
                do
                {
                    transacNoNew = "IB" + item.MO_No.Trim() + CodeUtility.RandomNumber(3);
                    
                } while (await _repoTransactionMain.CheckTransacNo(transacNoNew) || (transacNoNewList.Contains(transacNoNew)));
                transacNoNewList.Add(transacNoNew);

                var transacSheetNoNew = "";
                do
                {
                    transacSheetNoNew = "IB" + DateTime.Now.ToString("yyyyMMdd") + CodeUtility.RandomNumber(3);
                    
                } while (await _repoTransactionMain.CheckTranSheetNo(transacSheetNoNew) || (transacSheetNoNewList.Contains(transacSheetNoNew)));
                transacSheetNoNewList.Add(transacSheetNoNew);

                AddTransaction(transacNoNew, transacSheetNoNew, qrCodeIdNew, item.MO_No, item.MO_Seq, item.Rack_Location, packingParent, transactionMainParent, itemListSizeAndQtyHaveInstockNotNull, timeNow, updateBy);
            }

            // nếu không phải là otherSplit thì ouput khác
            if (!otherSplit)
            {
                //Update transaction main parent(update lại transactionmain cũ) nếu transactype="MG"
                ////////////
                // ngược lại là type I M R out put ra như bình thường: tạo O -> update lại transac cũ
                await Output(transactionMainParent, dataSplit, timeNow, updateBy);
                ///////////
            }
            // ngược lại là otherSplit ouput khác: otherSplit là không update lại transac_type=="MG" còn ở trên là update lại transac_type="MG"
            else
            {
                await OutputOtherSplit(transactionMainParent, dataSplit, timeNow, updateBy);
            }

            return await _repoTransactionMain.SaveAll();
            //return true;
        }

        public string AddPackingList(string parentMono ,SplitDataByOffset_Dto item, WMSB_Packing_List packingParent, IEnumerable<SizeAndQty> itemListSizeAndQtyHaveInstockNotNull, DateTime timeNow, string updateBy)
        {
            var packingListAdd = new Packing_List_Dto();
            packingListAdd.Sheet_Type = "S";
            packingListAdd.Missing_No = "";
            packingListAdd.Supplier_ID = packingParent.Supplier_ID;
            packingListAdd.Supplier_Name = packingParent.Supplier_Name;
            packingListAdd.MO_No = item.MO_No;
            packingListAdd.Purchase_No = packingParent.Purchase_No;
            packingListAdd.MO_Seq = item.MO_Seq;
            packingListAdd.DMO_No = parentMono;
            // packingListAdd.Delivery_No = "";
            packingListAdd.Material_ID = packingParent.Material_ID;
            packingListAdd.Material_Name = packingParent.Material_Name;
            packingListAdd.Model_No = packingParent.Model_No;
            packingListAdd.Model_Name = packingParent.Model_Name;
            packingListAdd.Article = packingParent.Article;
            packingListAdd.Subcon_ID = packingParent.Subcon_ID;
            packingListAdd.Subcon_Name = packingParent.Subcon_Name;
            packingListAdd.T3_Supplier = packingParent.T3_Supplier;
            packingListAdd.T3_Supplier_Name = packingParent.T3_Supplier_Name;
            packingListAdd.Generated_QRCode = "Y";
            packingListAdd.Receive_Date = timeNow;
            packingListAdd.Updated_By = updateBy;
            packingListAdd.Receive_No = CodeUtility.RandomReceiveNo("SW", 2);
            packingListAdd.Updated_Time = timeNow;

            var packingListAddModel = _mapper.Map<WMSB_Packing_List>(packingListAdd);
            _repoPackingList.Add(packingListAddModel);

            foreach (var item1 in itemListSizeAndQtyHaveInstockNotNull)
            {
                var packingListDetailAdd = new Packing_List_Detail_Dto();
                packingListDetailAdd.Receive_No = packingListAdd.Receive_No;
                packingListDetailAdd.Order_Size = item1.Order_Size;
                packingListDetailAdd.Model_Size = item1.Model_Size;
                packingListDetailAdd.Tool_Size = item1.Tool_Size;
                packingListDetailAdd.Spec_Size = "";
                packingListDetailAdd.MO_Qty = item1.MO_Qty;
                packingListDetailAdd.Purchase_Qty = item1.Purchase_Qty;
                packingListDetailAdd.Received_Qty = item1.Instock_Qty;
                packingListDetailAdd.Updated_Time = timeNow;
                packingListDetailAdd.Updated_By = updateBy;
                var packingListDetailAddModel = _mapper.Map<WMSB_PackingList_Detail>(packingListDetailAdd);
                _repoPackingListDetail.Add(packingListDetailAddModel);
            }

            return packingListAdd.Receive_No;
        }

        public void AddQrCode(string parentQrCodeId ,string qrCodeIdNew, string packingListAddRecieveNo, IEnumerable<SizeAndQty> itemListSizeAndQtyHaveInstockNotNull, DateTime timeNow, string updateBy)
        {
            var qrCodeMainAdd = new WMSB_QRCode_Main();
            qrCodeMainAdd.QRCode_ID = qrCodeIdNew;
            qrCodeMainAdd.Receive_No = packingListAddRecieveNo;
            qrCodeMainAdd.QRCode_Version = 1;
            qrCodeMainAdd.QRCode_Type = "S";
            qrCodeMainAdd.Valid_Status = "Y";
            qrCodeMainAdd.Is_Scanned = "Y";
            qrCodeMainAdd.Merge_QRCodeID = parentQrCodeId;
            qrCodeMainAdd.Updated_By = updateBy;
            qrCodeMainAdd.Updated_Time = timeNow;
            _repoQRCodeMain.Add(qrCodeMainAdd);

            foreach (var item1 in itemListSizeAndQtyHaveInstockNotNull)
            {
                var qrCodeDetailAdd = new WMSB_QRCode_Detail();
                qrCodeDetailAdd.QRCode_ID = qrCodeIdNew;
                qrCodeDetailAdd.QRCode_Version = 1;
                qrCodeDetailAdd.Tool_Size = item1.Tool_Size;
                qrCodeDetailAdd.Order_Size = item1.Order_Size;
                qrCodeDetailAdd.Model_Size = item1.Model_Size;
                qrCodeDetailAdd.Spec_Size = "";
                qrCodeDetailAdd.Qty = item1.Instock_Qty;
                qrCodeDetailAdd.Updated_By = updateBy;
                qrCodeDetailAdd.Updated_Time = timeNow;
                _repoQRCodeDetail.Add(qrCodeDetailAdd);
            }
        }

        public void AddTransaction(string transacNoNew, string transacSheetNoNew, string qrCodeIdNew, string moNo, string moSeq, string rackLocation, WMSB_Packing_List packingParent, WMSB_Transaction_Main transactionMainParent, IEnumerable<SizeAndQty> itemListSizeAndQtyHaveInstockNotNull, DateTime timeNow, string updateBy)
        {
            var transactionMainAdd = new WMSB_Transaction_Main();
            transactionMainAdd.Transac_No = transacNoNew;
            transactionMainAdd.Transac_Type = "I";
            transactionMainAdd.Transac_Sheet_No = transacSheetNoNew;
            transactionMainAdd.MO_No = moNo;
            transactionMainAdd.Can_Move = "Y";
            transactionMainAdd.Purchase_No = packingParent.Purchase_No;
            transactionMainAdd.Transac_Time = timeNow;
            transactionMainAdd.QRCode_ID = qrCodeIdNew;
            transactionMainAdd.QRCode_Version = 1;
            transactionMainAdd.MO_Seq = moSeq;
            transactionMainAdd.Material_ID = transactionMainParent.Material_ID;
            transactionMainAdd.Material_Name = transactionMainParent.Material_Name;
            transactionMainAdd.Purchase_Qty = itemListSizeAndQtyHaveInstockNotNull.Sum(x => x.Purchase_Qty);
            transactionMainAdd.Transacted_Qty = itemListSizeAndQtyHaveInstockNotNull.Sum(x => x.Instock_Qty);
            transactionMainAdd.Rack_Location = rackLocation;
            transactionMainAdd.Is_Transfer_Form = "N";
            transactionMainAdd.Reason_Code = "ZZZ";// những đơn được tách mặc đinh Reason_Code = "ZZZ" để sau này dễ báo cáo
            transactionMainAdd.Merge_Transac_No = transactionMainParent.Transac_No;
            transactionMainAdd.Updated_Time = timeNow;
            transactionMainAdd.Updated_By = updateBy;
            _repoTransactionMain.Add(transactionMainAdd);
            // ========================================Add Table Transaction Detail========================================//
            foreach (var item1 in itemListSizeAndQtyHaveInstockNotNull)
            {
                var transactionDetailAdd = new WMSB_Transaction_Detail();
                transactionDetailAdd.Transac_No = transacNoNew;
                transactionDetailAdd.Tool_Size = item1.Tool_Size;
                transactionDetailAdd.Order_Size = item1.Order_Size;
                transactionDetailAdd.Model_Size = item1.Model_Size;
                transactionDetailAdd.Spec_Size = "";
                transactionDetailAdd.Qty = item1.Instock_Qty;
                transactionDetailAdd.Trans_Qty = item1.Instock_Qty;
                transactionDetailAdd.Instock_Qty = item1.Instock_Qty;
                transactionDetailAdd.Untransac_Qty = 0;
                transactionDetailAdd.Updated_By = updateBy;
                transactionDetailAdd.Updated_Time = timeNow;
                _repoTransactionDetail.Add(transactionDetailAdd);
            }
        }

        public void AddMaterialPurchaseSplit(SplitDataByOffset_Dto dataSplit, WMSB_Material_Purchase materialPurchaseParent, DateTime timeNow, string updateBy) {
            var materialPurchaseSplitModel = new WMSB_Material_Purchase_Split();
            materialPurchaseSplitModel.MO_No = dataSplit.MO_No;
            materialPurchaseSplitModel.Purchase_No = materialPurchaseParent == null ? null : materialPurchaseParent.Purchase_No;
            materialPurchaseSplitModel.MO_Seq = dataSplit.MO_Seq;
            materialPurchaseSplitModel.Purchase_Date = materialPurchaseParent == null ? null : materialPurchaseParent.Purchase_Date;
            materialPurchaseSplitModel.Confirm_Delivery = materialPurchaseParent == null ? null : materialPurchaseParent.Confirm_Delivery;
            materialPurchaseSplitModel.Custmoer_Part = materialPurchaseParent == null ? null : materialPurchaseParent.Custmoer_Part;
            materialPurchaseSplitModel.Supplier_ID = materialPurchaseParent == null ? null : materialPurchaseParent.Supplier_ID;
            materialPurchaseSplitModel.Updated_By = updateBy;
            materialPurchaseSplitModel.Updated_Time = timeNow;
            _repoMaterialPurchaseSplit.Add(materialPurchaseSplitModel);
        }
        public async Task Output(WMSB_Transaction_Main transactionMainParent, List<SplitDataByOffset_Dto> dataSplit, DateTime timeNow, string updateBy)
        {
            var itemListSizeAndQtyHaveInstockNotNull = new List<SizeAndQty>();
            foreach (var item in dataSplit)
            {
                itemListSizeAndQtyHaveInstockNotNull.AddRange(item.ListSizeAndQty.Where(x => x.Instock_Qty != null));
            }
            itemListSizeAndQtyHaveInstockNotNull = itemListSizeAndQtyHaveInstockNotNull.GroupBy(x => new { Tool_Size = x.Tool_Size.Trim(), Order_Size = x.Order_Size.Trim(), Model_Size = x.Model_Size.Trim() })
                                                    .Select(x => new SizeAndQty
                                                    {
                                                        Tool_Size = x.Key.Tool_Size,
                                                        Order_Size = x.Key.Order_Size,
                                                        Model_Size = x.Key.Model_Size,
                                                        Instock_Qty = x.Sum(z => z.Instock_Qty)
                                                    }).ToList();


                // -----------------------thêm transaction main type O-----------------------------------------------//
                string outputSheetNo;
                do
                {
                    string num = CodeUtility.RandomNumber(3);
                    outputSheetNo = "OB" + DateTime.Now.ToString("yyyyMMdd") + num;// OB + 20200421 + 001
                } while (await _repoTransactionMain.CheckTranSheetNo(outputSheetNo));
                string transacNo;
                do
                {
                    transacNo = "BO" + transactionMainParent.MO_No.Trim() + CodeUtility.RandomNumber(3);
                } while (await _repoTransactionMain.CheckTransacNo(transacNo));

                WMSB_Transaction_Main modelTypeO = new WMSB_Transaction_Main();
                modelTypeO.Transac_Type = "O";
                modelTypeO.Can_Move = "N";
                // modelTypeO.Is_Transfer_Form = "N";
                modelTypeO.Transac_No = transacNo;
                modelTypeO.Transac_Sheet_No = outputSheetNo;
                modelTypeO.Transacted_Qty = itemListSizeAndQtyHaveInstockNotNull.Sum(x => x.Instock_Qty);
                modelTypeO.Pickup_No = "";
                modelTypeO.Transac_Time = timeNow;
                modelTypeO.Updated_Time = timeNow;
                modelTypeO.Updated_By = updateBy;
                modelTypeO.Missing_No = transactionMainParent.Missing_No;
                modelTypeO.Material_ID = transactionMainParent.Material_ID;
                modelTypeO.Material_Name = transactionMainParent.Material_Name;
                modelTypeO.Purchase_No = transactionMainParent.Purchase_No;
                modelTypeO.Rack_Location = null;// type O: racklocation rỗng
                modelTypeO.Purchase_Qty = transactionMainParent.Purchase_Qty;
                modelTypeO.QRCode_Version = transactionMainParent.QRCode_Version;
                modelTypeO.QRCode_ID = transactionMainParent.QRCode_ID;
                modelTypeO.MO_No = transactionMainParent.MO_No;
                modelTypeO.MO_Seq = transactionMainParent.MO_Seq;
                _repoTransactionMain.Add(modelTypeO);

                // ---------------------------------------------------------------------------------------------------//


            //Update transaction main parent(update lại transactionmain cũ) nếu transactype="MG"
            if (transactionMainParent.Transac_Type.Trim() == "MG")
            {
                var transactionDetailParent = await _repoTransactionDetail.FindAll(x => x.Transac_No.Trim() == transactionMainParent.Transac_No.Trim()).ToListAsync();

                if (transactionDetailParent.Sum(x => x.Instock_Qty) - itemListSizeAndQtyHaveInstockNotNull.Sum(x => x.Instock_Qty) == 0)
                {
                    transactionMainParent.Can_Move = "N";

                    var qrCodeMain = await _repoQRCodeMain.FindAll(x => x.QRCode_ID.Trim() == transactionMainParent.QRCode_ID.Trim()).OrderByDescending(x => x.QRCode_Version).FirstOrDefaultAsync();
                    qrCodeMain.Valid_Status = "N";
                    qrCodeMain.Invalid_Date = timeNow;
                    qrCodeMain.Updated_Time = timeNow;
                    qrCodeMain.Updated_By = updateBy;
                    _repoQRCodeMain.Update(qrCodeMain);
                }
                else
                {
                    var transactionDetailUpdate = transactionDetailParent.GroupJoin(itemListSizeAndQtyHaveInstockNotNull,
                                                    x => new { Tool_Size = x.Tool_Size.Trim(), Model_Size = x.Model_Size.Trim(), Order_Size = x.Order_Size.Trim() },
                                                    y => new { Tool_Size = y.Tool_Size.Trim(), Model_Size = y.Model_Size.Trim(), Order_Size = y.Order_Size.Trim() },
                                                    (x, y) => new { x, y })
                                                    .SelectMany(x => x.y.DefaultIfEmpty(),
                                                    (x, y) =>
                                                    {
                                                        x.x.Instock_Qty = x.x.Instock_Qty - (y == null ? 0 : y.Instock_Qty);
                                                        x.x.Trans_Qty = x.x.Qty - x.x.Instock_Qty;
                                                        x.x.Updated_Time = timeNow;
                                                        return x.x;
                                                    }).ToList();
                    _repoTransactionDetail.UpdateRange(transactionDetailUpdate);
                }
                // Update lại Transacted_Qty sau khi tách
                var transacted_Qty = transactionDetailParent.Sum(x => x.Trans_Qty);
                transactionMainParent.Transacted_Qty = transacted_Qty;
                transactionMainParent.Updated_Time = timeNow;
                _repoTransactionMain.Update(transactionMainParent);

            }
            ////////////
            // ngược lại là type I M R out put ra như bình thường: tạo O -> update lại transac cũ
            else
            {
                var transactionDetailByParent = await _repoTransactionDetail.FindAll(x => x.Transac_No == transactionMainParent.Transac_No).ToListAsync();

                transactionMainParent.Can_Move = "N"; // update transaction main cũ: Can_Move thành N
                _repoTransactionMain.Update(transactionMainParent);

                // Thêm transaction detail mới theo type = o, dựa vào transaction detail của transaction main cũ
                foreach (var item1 in itemListSizeAndQtyHaveInstockNotNull)
                {
                    var instockQtyByParent = transactionDetailByParent.Where(x => x.Tool_Size.Trim() == item1.Tool_Size.Trim() && x.Order_Size.Trim() == item1.Order_Size.Trim() && x.Model_Size.Trim() == item1.Model_Size.Trim()).FirstOrDefault() == null ?
                                    0 : transactionDetailByParent.Where(x => x.Tool_Size.Trim() == item1.Tool_Size.Trim() && x.Order_Size.Trim() == item1.Order_Size.Trim() && x.Model_Size.Trim() == item1.Model_Size.Trim()).FirstOrDefault().Instock_Qty;

                    var transactionDetailAdd = new WMSB_Transaction_Detail();
                    transactionDetailAdd.Transac_No = modelTypeO.Transac_No;
                    transactionDetailAdd.Tool_Size = item1.Tool_Size;
                    transactionDetailAdd.Order_Size = item1.Order_Size;
                    transactionDetailAdd.Model_Size = item1.Model_Size;
                    transactionDetailAdd.Spec_Size = "";
                    transactionDetailAdd.Qty = instockQtyByParent;
                    transactionDetailAdd.Trans_Qty = item1.Instock_Qty;
                    transactionDetailAdd.Instock_Qty = instockQtyByParent - item1.Instock_Qty;
                    transactionDetailAdd.Untransac_Qty = 0;
                    transactionDetailAdd.Updated_By = updateBy;
                    transactionDetailAdd.Updated_Time = timeNow;
                    _repoTransactionDetail.Add(transactionDetailAdd);
                }

                // Nếu output ra chưa hết thì thêm transaction main type R, và transaction detail, thêm qrcode mới và update version lên
                if (transactionDetailByParent.Sum(x => x.Instock_Qty) - itemListSizeAndQtyHaveInstockNotNull.Sum(x => x.Instock_Qty) > 0)
                {
                    //  thêm type R
                    var tmpQrcodeVersion = transactionMainParent.QRCode_Version + 1;
                    WMSB_Transaction_Main modelTypeR = new WMSB_Transaction_Main();
                    modelTypeR.Transac_Type = "R";
                    modelTypeR.Transac_No = "R" + transactionMainParent.Transac_No;
                    modelTypeR.Transac_Sheet_No = "R" + transactionMainParent.Transac_Sheet_No;
                    modelTypeR.Transacted_Qty = itemListSizeAndQtyHaveInstockNotNull.Sum(x => x.Instock_Qty);
                    modelTypeR.Updated_By = updateBy;
                    modelTypeR.Updated_Time = timeNow;
                    modelTypeR.Missing_No = transactionMainParent.Missing_No;
                    modelTypeR.Material_ID = transactionMainParent.Material_ID;
                    modelTypeR.Material_Name = transactionMainParent.Material_Name;
                    modelTypeR.Purchase_No = transactionMainParent.Purchase_No;
                    modelTypeR.Rack_Location = transactionMainParent.Rack_Location;
                    modelTypeR.Purchase_Qty = transactionMainParent.Purchase_Qty;
                    modelTypeR.QRCode_Version = tmpQrcodeVersion;
                    modelTypeR.QRCode_ID = transactionMainParent.QRCode_ID;
                    modelTypeR.MO_No = transactionMainParent.MO_No;
                    modelTypeR.MO_Seq = transactionMainParent.MO_Seq;
                    modelTypeR.Can_Move = "Y";
                    // modelTypeR.Is_Transfer_Form = "N";
                    modelTypeR.Transac_Time = timeNow;
                    _repoTransactionMain.Add(modelTypeR);

                    // thêm transaction main cũng phải thêm transaction detail
                    foreach (var item1 in transactionDetailByParent)
                    {
                        var qtyTransOut = itemListSizeAndQtyHaveInstockNotNull.Where(x => x.Tool_Size.Trim() == item1.Tool_Size.Trim() && x.Order_Size.Trim() == item1.Order_Size.Trim() && x.Model_Size.Trim() == item1.Model_Size.Trim()).FirstOrDefault() == null ?
                                        0 : itemListSizeAndQtyHaveInstockNotNull.Where(x => x.Tool_Size.Trim() == item1.Tool_Size.Trim() && x.Order_Size.Trim() == item1.Order_Size.Trim() && x.Model_Size.Trim() == item1.Model_Size.Trim()).FirstOrDefault().Instock_Qty;

                        var transactionDetailAdd = new WMSB_Transaction_Detail();
                        transactionDetailAdd.Transac_No = modelTypeR.Transac_No;
                        transactionDetailAdd.Tool_Size = item1.Tool_Size;
                        transactionDetailAdd.Order_Size = item1.Order_Size;
                        transactionDetailAdd.Model_Size = item1.Model_Size;
                        transactionDetailAdd.Spec_Size = "";
                        transactionDetailAdd.Qty = item1.Instock_Qty - qtyTransOut;
                        transactionDetailAdd.Trans_Qty = item1.Instock_Qty - qtyTransOut;
                        transactionDetailAdd.Instock_Qty = item1.Instock_Qty - qtyTransOut;
                        transactionDetailAdd.Untransac_Qty = 0;
                        transactionDetailAdd.Updated_By = updateBy;
                        transactionDetailAdd.Updated_Time = timeNow;
                        _repoTransactionDetail.Add(transactionDetailAdd);
                    }

                    // thêm qrcode mới, nếu output ra chưa hết thì thêm qrcode main mới dựa vào cái cũ và update version lên
                    var qrCodeMain = await _repoQRCodeMain.FindAll(x => x.QRCode_ID.Trim() == transactionMainParent.QRCode_ID.Trim()).OrderByDescending(x => x.QRCode_Version).FirstOrDefaultAsync();
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

                    // Update cho QRCode cũ, Valid_Status =N, Invalid_Date = Ngày mà tạo ra version mới
                    qrCodeMain.Valid_Status = "N";
                    qrCodeMain.Invalid_Date = timeNow;
                    _repoQRCodeMain.Update(qrCodeMain);

                    // thêm qrcodedetail của qrcode mới: thêm qrcode main cũng phải thêm qrcode detail
                    var qrCodeDetails = await _repoQRCodeDetail.FindAll(x => x.QRCode_ID.Trim() == qrCodeMain.QRCode_ID.Trim() && x.QRCode_Version == qrCodeMain.QRCode_Version).ToListAsync();
                    foreach (var itemQrCodeDetail in qrCodeDetails)
                    {
                        var qtyTransOut = itemListSizeAndQtyHaveInstockNotNull.Where(x => x.Tool_Size.Trim() == itemQrCodeDetail.Tool_Size.Trim() && x.Order_Size.Trim() == itemQrCodeDetail.Order_Size.Trim() && x.Model_Size.Trim() == itemQrCodeDetail.Model_Size.Trim()).FirstOrDefault() == null ?
                                        0 : itemListSizeAndQtyHaveInstockNotNull.Where(x => x.Tool_Size.Trim() == itemQrCodeDetail.Tool_Size.Trim() && x.Order_Size.Trim() == itemQrCodeDetail.Order_Size.Trim() && x.Model_Size.Trim() == itemQrCodeDetail.Model_Size.Trim()).FirstOrDefault().Instock_Qty;

                        itemQrCodeDetail.QID = 0;
                        itemQrCodeDetail.Updated_By = updateBy;
                        itemQrCodeDetail.Updated_Time = timeNow;
                        itemQrCodeDetail.QRCode_Version = modelQrCodeMain.QRCode_Version;
                        itemQrCodeDetail.Qty = itemQrCodeDetail.Qty - qtyTransOut;
                        _repoQRCodeDetail.Add(itemQrCodeDetail);
                    }
                }
                // Nếu QRCode đã out hết số lượng, cần update cho nó không còn hiệu lực 
                // ( Ở bảng WMSB_QRCode_Main: UPDATE trường Valid_Status =’N”, Invalid_Date là ngày Output hết số lượng, 
                // đồng thời cũng update trường Update_Time & Update_By)
                else
                {
                    var qrCodeMain = await _repoQRCodeMain.FindAll(x => x.QRCode_ID.Trim() == transactionMainParent.QRCode_ID.Trim()).OrderByDescending(x => x.QRCode_Version).FirstOrDefaultAsync();
                    qrCodeMain.Valid_Status = "N";
                    qrCodeMain.Invalid_Date = timeNow;
                    qrCodeMain.Updated_Time = timeNow;
                    qrCodeMain.Updated_By = updateBy;
                    _repoQRCodeMain.Update(qrCodeMain);
                }

            }
        }

        public async Task OutputOtherSplit(WMSB_Transaction_Main transactionMainParent, List<SplitDataByOffset_Dto> dataSplit, DateTime timeNow, string updateBy)
        {
            var itemListSizeAndQtyHaveInstockNotNull = new List<SizeAndQty>();
            foreach (var item in dataSplit)
            {
                itemListSizeAndQtyHaveInstockNotNull.AddRange(item.ListSizeAndQty.Where(x => x.Instock_Qty != null));
            }
            itemListSizeAndQtyHaveInstockNotNull = itemListSizeAndQtyHaveInstockNotNull.GroupBy(x => new { Tool_Size = x.Tool_Size.Trim(), Order_Size = x.Order_Size.Trim(), Model_Size = x.Model_Size.Trim() })
                                                    .Select(x => new SizeAndQty
                                                    {
                                                        Tool_Size = x.Key.Tool_Size,
                                                        Order_Size = x.Key.Order_Size,
                                                        Model_Size = x.Key.Model_Size,
                                                        Instock_Qty = x.Sum(z => z.Instock_Qty)
                                                    }).ToList();

            var transactionDetailByParent = await _repoTransactionDetail.FindAll(x => x.Transac_No == transactionMainParent.Transac_No).ToListAsync();

            transactionMainParent.Can_Move = "N"; // update transaction main cũ: Can_Move thành N
            _repoTransactionMain.Update(transactionMainParent);

            // thêm transaction main type O
            string outputSheetNo;
            do
            {
                string num = CodeUtility.RandomNumber(3);
                outputSheetNo = "OB" + DateTime.Now.ToString("yyyyMMdd") + num;// OB + 20200421 + 001
            } while (await _repoTransactionMain.CheckTranSheetNo(outputSheetNo));
            string transacNo;
            do
            {
                transacNo = "BO" + transactionMainParent.MO_No.Trim() + CodeUtility.RandomNumber(3);
            } while (await _repoTransactionMain.CheckTransacNo(transacNo));

            WMSB_Transaction_Main modelTypeO = new WMSB_Transaction_Main();
            modelTypeO.Transac_Type = "O";
            modelTypeO.Can_Move = "N";
            // modelTypeO.Is_Transfer_Form = "N";
            modelTypeO.Transac_No = transacNo;
            modelTypeO.Transac_Sheet_No = outputSheetNo;
            modelTypeO.Transacted_Qty = itemListSizeAndQtyHaveInstockNotNull.Sum(x => x.Instock_Qty);
            modelTypeO.Pickup_No = "";
            modelTypeO.Transac_Time = timeNow;
            modelTypeO.Updated_Time = timeNow;
            modelTypeO.Updated_By = updateBy;
            modelTypeO.Missing_No = transactionMainParent.Missing_No;
            modelTypeO.Material_ID = transactionMainParent.Material_ID;
            modelTypeO.Material_Name = transactionMainParent.Material_Name;
            modelTypeO.Purchase_No = transactionMainParent.Purchase_No;
            modelTypeO.Rack_Location = null;// type O: racklocation rỗng
            modelTypeO.Purchase_Qty = transactionMainParent.Purchase_Qty;
            modelTypeO.QRCode_Version = transactionMainParent.QRCode_Version;
            modelTypeO.Reason_Code = "ZZZ";
            modelTypeO.QRCode_ID = transactionMainParent.QRCode_ID;
            modelTypeO.MO_No = transactionMainParent.MO_No;
            modelTypeO.MO_Seq = transactionMainParent.MO_Seq;
            _repoTransactionMain.Add(modelTypeO);

            // Thêm transaction detail mới theo type = o, dựa vào transaction detail của transaction main cũ
            foreach (var item1 in itemListSizeAndQtyHaveInstockNotNull)
            {
                var instockQtyByParent = transactionDetailByParent.Where(x => x.Tool_Size.Trim() == item1.Tool_Size.Trim() && x.Order_Size.Trim() == item1.Order_Size.Trim() && x.Model_Size.Trim() == item1.Model_Size.Trim()).FirstOrDefault() == null ?
                                    0 : transactionDetailByParent.Where(x => x.Tool_Size.Trim() == item1.Tool_Size.Trim() && x.Order_Size.Trim() == item1.Order_Size.Trim() && x.Model_Size.Trim() == item1.Model_Size.Trim()).FirstOrDefault().Instock_Qty;
                var transactionDetailAdd = new WMSB_Transaction_Detail();
                transactionDetailAdd.Transac_No = modelTypeO.Transac_No;
                transactionDetailAdd.Tool_Size = item1.Tool_Size;
                transactionDetailAdd.Order_Size = item1.Order_Size;
                transactionDetailAdd.Model_Size = item1.Model_Size;
                transactionDetailAdd.Spec_Size = "";
                transactionDetailAdd.Qty = instockQtyByParent;
                transactionDetailAdd.Trans_Qty = item1.Instock_Qty;
                transactionDetailAdd.Instock_Qty = instockQtyByParent - item1.Instock_Qty;
                transactionDetailAdd.Untransac_Qty = 0;
                transactionDetailAdd.Updated_By = updateBy;
                transactionDetailAdd.Updated_Time = timeNow;
                _repoTransactionDetail.Add(transactionDetailAdd);
            }

            // Nếu output ra chưa hết thì thêm transaction main type R, và transaction detail, thêm qrcode mới và update version lên
            if (transactionDetailByParent.Sum(x => x.Instock_Qty) - itemListSizeAndQtyHaveInstockNotNull.Sum(x => x.Instock_Qty) > 0)
            {
                //  thêm type R
                var tmpQrcodeVersion = transactionMainParent.QRCode_Version + 1;
                WMSB_Transaction_Main modelTypeR = new WMSB_Transaction_Main();
                modelTypeR.Transac_Type = "R";
                modelTypeR.Transac_No = "R" + transactionMainParent.Transac_No;
                modelTypeR.Transac_Sheet_No = "R" + transactionMainParent.Transac_Sheet_No;
                modelTypeR.Transacted_Qty = itemListSizeAndQtyHaveInstockNotNull.Sum(x => x.Instock_Qty);
                modelTypeR.Updated_By = updateBy;
                modelTypeR.Updated_Time = timeNow;
                modelTypeR.Missing_No = transactionMainParent.Missing_No;
                modelTypeR.Material_ID = transactionMainParent.Material_ID;
                modelTypeR.Material_Name = transactionMainParent.Material_Name;
                modelTypeR.Purchase_No = transactionMainParent.Purchase_No;
                modelTypeR.Rack_Location = transactionMainParent.Rack_Location;
                modelTypeR.Purchase_Qty = transactionMainParent.Purchase_Qty;
                modelTypeR.QRCode_Version = tmpQrcodeVersion;
                modelTypeR.QRCode_ID = transactionMainParent.QRCode_ID;
                modelTypeR.MO_No = transactionMainParent.MO_No;
                modelTypeR.MO_Seq = transactionMainParent.MO_Seq;
                modelTypeO.Reason_Code = "ZZZ";
                modelTypeR.Can_Move = "Y";
                // modelTypeR.Is_Transfer_Form = "N";
                modelTypeR.Transac_Time = timeNow;
                _repoTransactionMain.Add(modelTypeR);

                // thêm transaction main cũng phải thêm transaction detail
                foreach (var item1 in transactionDetailByParent)
                {
                    var qtyTransOut = itemListSizeAndQtyHaveInstockNotNull.Where(x => x.Tool_Size.Trim() == item1.Tool_Size.Trim() && x.Order_Size.Trim() == item1.Order_Size.Trim() && x.Model_Size.Trim() == item1.Model_Size.Trim()).FirstOrDefault() == null ?
                                    0 : itemListSizeAndQtyHaveInstockNotNull.Where(x => x.Tool_Size.Trim() == item1.Tool_Size.Trim() && x.Order_Size.Trim() == item1.Order_Size.Trim() && x.Model_Size.Trim() == item1.Model_Size.Trim()).FirstOrDefault().Instock_Qty;

                    var transactionDetailAdd = new WMSB_Transaction_Detail();
                    transactionDetailAdd.Transac_No = modelTypeR.Transac_No;
                    transactionDetailAdd.Tool_Size = item1.Tool_Size;
                    transactionDetailAdd.Order_Size = item1.Order_Size;
                    transactionDetailAdd.Model_Size = item1.Model_Size;
                    transactionDetailAdd.Spec_Size = "";
                    transactionDetailAdd.Qty = item1.Instock_Qty - qtyTransOut;
                    transactionDetailAdd.Trans_Qty = item1.Instock_Qty - qtyTransOut;
                    transactionDetailAdd.Instock_Qty = item1.Instock_Qty - qtyTransOut;
                    transactionDetailAdd.Untransac_Qty = 0;
                    transactionDetailAdd.Updated_By = updateBy;
                    transactionDetailAdd.Updated_Time = timeNow;
                    _repoTransactionDetail.Add(transactionDetailAdd);
                }

                // thêm qrcode mới, nếu output ra chưa hết thì thêm qrcode main mới dựa vào cái cũ và update version lên
                var qrCodeMain = await _repoQRCodeMain.FindAll(x => x.QRCode_ID.Trim() == transactionMainParent.QRCode_ID.Trim()).OrderByDescending(x => x.QRCode_Version).FirstOrDefaultAsync();
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

                // Update cho QRCode cũ, Valid_Status =N, Invalid_Date = Ngày mà tạo ra version mới
                qrCodeMain.Valid_Status = "N";
                qrCodeMain.Invalid_Date = timeNow;
                _repoQRCodeMain.Update(qrCodeMain);

                // thêm qrcodedetail của qrcode mới: thêm qrcode main cũng phải thêm qrcode detail
                var qrCodeDetails = await _repoQRCodeDetail.FindAll(x => x.QRCode_ID.Trim() == qrCodeMain.QRCode_ID.Trim() && x.QRCode_Version == qrCodeMain.QRCode_Version).ToListAsync();
                foreach (var itemQrCodeDetail in qrCodeDetails)
                {
                    var qtyTransOut = itemListSizeAndQtyHaveInstockNotNull.Where(x => x.Tool_Size.Trim() == itemQrCodeDetail.Tool_Size.Trim() && x.Order_Size.Trim() == itemQrCodeDetail.Order_Size.Trim() && x.Model_Size.Trim() == itemQrCodeDetail.Model_Size.Trim()).FirstOrDefault() == null ?
                                    0 : itemListSizeAndQtyHaveInstockNotNull.Where(x => x.Tool_Size.Trim() == itemQrCodeDetail.Tool_Size.Trim() && x.Order_Size.Trim() == itemQrCodeDetail.Order_Size.Trim() && x.Model_Size.Trim() == itemQrCodeDetail.Model_Size.Trim()).FirstOrDefault().Instock_Qty;

                    itemQrCodeDetail.QID = 0;
                    itemQrCodeDetail.Updated_By = updateBy;
                    itemQrCodeDetail.Updated_Time = timeNow;
                    itemQrCodeDetail.QRCode_Version = modelQrCodeMain.QRCode_Version;
                    itemQrCodeDetail.Qty = itemQrCodeDetail.Qty - qtyTransOut;
                    _repoQRCodeDetail.Add(itemQrCodeDetail);
                }
            }
            // Nếu QRCode đã out hết số lượng, cần update cho nó không còn hiệu lực 
            // ( Ở bảng WMSB_QRCode_Main: UPDATE trường Valid_Status =’N”, Invalid_Date là ngày Output hết số lượng, 
            // đồng thời cũng update trường Update_Time & Update_By)
            else
            {
                var qrCodeMain = await _repoQRCodeMain.FindAll(x => x.QRCode_ID.Trim() == transactionMainParent.QRCode_ID.Trim()).OrderByDescending(x => x.QRCode_Version).FirstOrDefaultAsync();
                qrCodeMain.Valid_Status = "N";
                qrCodeMain.Invalid_Date = timeNow;
                qrCodeMain.Updated_Time = timeNow;
                qrCodeMain.Updated_By = updateBy;
                _repoQRCodeMain.Update(qrCodeMain);
            }

        }

        // merge-qrcode/split/qrcode-detail/transactionNo
        public async Task<QrCodeSplitDetail_Dto> QrCodeSplitDetail(string transacNo)
        {
            var transactionMain = _repoTransactionMain.FindAll(x => x.Transac_No == transacNo);
            var qrCodeMain = _repoQRCodeMain.FindAll();
            var packingList = _repoPackingList.FindAll();
            var transactionDetail = _repoTransactionDetail.FindAll();
            var packingListDetail = _repoPackingListDetail.FindAll();

            var transactionModel = await transactionMain.FirstOrDefaultAsync();
            var qrCodeMainModel = await _repoQRCodeMain.FindAll(x => x.QRCode_ID.Trim() == transactionModel.QRCode_ID.Trim() &&
                                            x.QRCode_Version == transactionModel.QRCode_Version).FirstOrDefaultAsync();
            var packingListDetailOfTransacNo = await _repoPackingListDetail.FindAll(x => x.Receive_No.Trim() == qrCodeMainModel.Receive_No.Trim()).ToListAsync();
            var transacMainMergeQrCode = await transactionMain
                .GroupJoin(
                        qrCodeMain,
                        x => new { QRCode_ID = x.QRCode_ID.Trim(), QRCode_Version = x.QRCode_Version },
                        y => new { QRCode_ID = y.QRCode_ID.Trim(), QRCode_Version = y.QRCode_Version },
                        (x, y) => new { TransactionMain = x, QrCodeMain = y })
                    .SelectMany(
                        x => x.QrCodeMain.DefaultIfEmpty(),
                        (x, y) => new { TransactionMain = x.TransactionMain, QrCodeMain = y })
                        .GroupJoin(
                            packingList,
                            x => x.QrCodeMain.Receive_No.Trim(),
                            y => y.Receive_No.Trim(),
                            (x, y) => new { TransactionMainQrCodeMain = x, PackingList = y })
                            .SelectMany(
                                x => x.PackingList.DefaultIfEmpty(),
                                (x, y) => new QrCodeSplitDetail_Dto
                                {
                                    MO_No = x.TransactionMainQrCodeMain.TransactionMain.MO_No,
                                    Material_ID = x.TransactionMainQrCodeMain.TransactionMain.Material_ID,
                                    Material_Name = x.TransactionMainQrCodeMain.TransactionMain.Material_Name,
                                    Model_No = y.Model_No,
                                    Model_Name = y.Model_Name,
                                    Article = y.Article,
                                    ListSizeAndQty = transactionDetail.Where(x => x.Transac_No == transacNo).Select(z => new SizeAndQty
                                    {
                                        Order_Size = z.Order_Size,
                                        Model_Size = z.Model_Size,
                                        Tool_Size = z.Tool_Size,
                                        // MO_Qty = z.Qty,
                                        Act_Out_Qty = 0,
                                        Trans_Qty = z.Trans_Qty,
                                        Instock_Qty = x.TransactionMainQrCodeMain.TransactionMain.Transac_Type.Trim() == "MG" ? z.Qty : z.Instock_Qty
                                    }).OrderBy(x => x.Tool_Size).ToList()
                                }).FirstOrDefaultAsync();
            transacMainMergeQrCode.ListSizeAndQty.GroupJoin(
                packingListDetailOfTransacNo,
                x => new {Order_Size = x.Order_Size, Model_Size = x.Model_Size, Tool_Size = x.Tool_Size},
                y => new {Order_Size = y.Order_Size, Model_Size = y.Model_Size, Tool_Size = y.Tool_Size},
                (x,y) => new {x , y}).SelectMany(x => x.y.DefaultIfEmpty(),
                    (x,y) => {
                        x.x.MO_Qty = y.MO_Qty;
                        return x.x;
                    }
                ).ToList();
            // Kiểm tra xem mã đó đã tách bao giờ chưa, tính số lượng đã tách
            var listTransacmainSplited = await _repoTransactionMain.FindAll(x => x.Merge_Transac_No.Trim() == transacNo.Trim() && x.Transac_Type == "I" && x.Reason_Code == "ZZZ")
                    .Join(transactionDetail, x => x.Transac_No, y => y.Transac_No, (x, y) => new
                    {
                        y.Tool_Size,
                        y.Order_Size,
                        y.Model_Size,
                        y.Instock_Qty
                    }).GroupBy(x => new { Tool_Size = x.Tool_Size.Trim(), Model_Size = x.Model_Size.Trim(), Order_Size = x.Order_Size.Trim() })
                        .Select(x => new
                        {
                            x.Key.Tool_Size,
                            x.Key.Order_Size,
                            x.Key.Model_Size,
                            Instock_Qty = x.Sum(x => x.Instock_Qty)
                        }).ToListAsync();
            if (listTransacmainSplited.Any())
            {
                transacMainMergeQrCode.ListSizeAndQty = transacMainMergeQrCode.ListSizeAndQty.GroupJoin(listTransacmainSplited, x => new
                {
                    Tool_Size = x.Tool_Size.Trim(),
                    Order_Size = x.Order_Size.Trim(),
                    Model_Size = x.Model_Size.Trim()
                },
                y => new
                {
                    Tool_Size = y.Tool_Size.Trim(),
                    Order_Size = y.Order_Size.Trim(),
                    Model_Size = y.Model_Size.Trim()
                },
                (x, y) => new
                {
                    x,
                    y
                }).SelectMany(x => x.y.DefaultIfEmpty(),
                    (x, y) =>
                    {
                        x.x.Instock_Qty = y == null ? x.x.Instock_Qty : (x.x.Instock_Qty - y.Instock_Qty);
                        x.x.Act_Out_Qty = y == null ? 0 : y.Instock_Qty;
                        return x.x;
                    }).ToList();
            }
            //////
            return transacMainMergeQrCode;
        }

        public async Task<List<MaterialInformation>> GetMaterialInformationInPO(string moNo)
        {
            var dataResult = await _repoTransactionMain.FindAll(x => x.MO_No.Trim() == moNo.Trim())
                .Select(x => new MaterialInformation()
                {
                    Material_ID = x.Material_ID.Trim(),
                    Material_Name = x.Material_Name.Trim()
                }).Distinct().ToListAsync();
            return dataResult;
        }

        #endregion
    }
}