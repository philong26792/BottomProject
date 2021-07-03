using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bottom_API._Repositories.Interfaces;
using Bottom_API._Services.Interfaces;
using Bottom_API.DTO;
using Bottom_API.DTO.TransferForm;
using Bottom_API.Helpers;
using Bottom_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Bottom_API._Services.Services
{
    public class TransferFormService : ITransferFormService
    {
        private readonly ITransactionMainRepo _transactionMainRepo;
        private readonly IPackingListRepository _packingListRepository;
        private readonly IPackingListDetailRepository _packingListDetailRepository;
        private readonly ITransferFormRepository _transferFormRepository;
        private readonly IMaterialViewRepository _materialViewRepository;
        private readonly ITransactionDetailRepo _transactionDetailRepo;
        private readonly IMaterialPurchaseRepository _materialPurchaseRepository;
        private readonly ISettingSupplierRepository _settingSupplierRepository;
        private readonly ICacheRepository _cacheRepository;
        private readonly IQRCodeMainRepository _qRCodeMainRepository;
        private readonly IMailUtility _iMailUtility;

        public TransferFormService(ITransactionMainRepo transactionMainRepo,
            IPackingListRepository packingListRepository,
            IQRCodeMainRepository qRCodeMainRepository,
            IPackingListDetailRepository packingListDetailRepository,
            ITransferFormRepository transferFormRepository,
            IMaterialViewRepository materialViewRepository,
            ITransactionDetailRepo transactionDetailRepo,
            IMaterialPurchaseRepository materialPurchaseRepository,
            ISettingSupplierRepository settingSupplierRepository,
            ICacheRepository cacheRepository,
            IMailUtility iMailUtility)
        {
            _transactionMainRepo = transactionMainRepo;
            _packingListRepository = packingListRepository;
            _packingListDetailRepository = packingListDetailRepository;
            _transferFormRepository = transferFormRepository;
            _materialViewRepository = materialViewRepository;
            _transactionDetailRepo = transactionDetailRepo;
            _materialPurchaseRepository = materialPurchaseRepository;
            _settingSupplierRepository = settingSupplierRepository;
            _qRCodeMainRepository = qRCodeMainRepository;
            _iMailUtility = iMailUtility;
            _cacheRepository = cacheRepository;
        }

        public async Task<bool> GenerateTransferForm(List<Transfer_Form_Generate_Dto> generateTransferForm, string updateBy)
        {
            DateTime timeNow = DateTime.Now;
            var generateTransferFormGroupBy = generateTransferForm.GroupBy(x => new { x.MO_No, x.MO_Seq, x.Material_ID });
            var collectTransNoList = new List<string>();
            foreach (var itemGroup in generateTransferFormGroupBy)
            {
                string collectTransNo;
                do
                {
                    string num = CodeUtility.RandomNumber(3);
                    // Mã xưởng+T+mã nhà cc T3+yymmdd(ngày mà sản sinh ra mã chuyển giao)+3 số random
                    // VD: CTV057200922001
                    // Nếu T3 là = null: Mã xưởng+T+ZZZZ+yymmdd(ngày mà sản sinh ra mã chuyển giao)+3 số random
                    var t3_Supplier = itemGroup.FirstOrDefault().T3_Supplier.Trim() == "" ? "ZZZZ" : itemGroup.FirstOrDefault().T3_Supplier.Trim();
                    collectTransNo = itemGroup.FirstOrDefault().Factory_ID.Trim() + "T" + t3_Supplier + DateTime.Now.ToString("yyyyMMdd") + num;
                } while (await _transferFormRepository.CheckCollectTransNo(collectTransNo) || collectTransNoList.Contains(collectTransNo));
                collectTransNoList.Add(collectTransNo);
                foreach (var item in itemGroup.ToList())
                {
                    WMSB_Transfer_Form transferForm = new WMSB_Transfer_Form();
                    transferForm.Collect_Trans_No = collectTransNo;
                    transferForm.Transac_No = item.Transac_No;
                    transferForm.Generate_Time = timeNow;
                    transferForm.T3_Supplier = item.T3_Supplier;
                    transferForm.T3_Supplier_Name = item.T3_Supplier_Name;
                    transferForm.Is_Release = "N";
                    transferForm.Release_By = null;
                    transferForm.Release_Time = null;
                    transferForm.Update_By = updateBy;
                    transferForm.Update_Time = timeNow;
                    transferForm.MO_No = item.MO_No;
                    transferForm.MO_Seq = item.MO_Seq;
                    transferForm.Material_ID = item.Material_ID;
                    transferForm.Material_Name = item.Material_Name;
                    transferForm.Valid_Status = true;
                    transferForm.Collec_Trans_Version = 1;


                    _transferFormRepository.Add(transferForm);

                    var transactionMain = _transactionMainRepo.FindSingle(x => x.Transac_No.Trim() == item.Transac_No.Trim());
                    transactionMain.Is_Transfer_Form = "Y";

                    _transactionMainRepo.Update(transactionMain);
                }
                await _transferFormRepository.SaveAll();
            }

            return true;
        }

        public async Task<List<Transfer_Form_Print_Dto>> GetInfoTransferFormPrintDetail(List<Transfer_Form_Generate_Dto> generateTransferForm)
        {
            var transationMain = _transactionMainRepo.FindAll();
            var packingList = _packingListRepository.FindAll();
            var qrCodeMain = _qRCodeMainRepository.FindAll();

            var data = generateTransferForm.GroupJoin(
                            transationMain,
                            x => x.Transac_No,
                            y => y.Transac_No,
                            (x, y) => new { GenerateTransferForm = x, TransationMain = y })
                                .SelectMany(
                                    x => x.TransationMain.DefaultIfEmpty(),
                                    (x, y) => new { GenerateTransferForm = x.GenerateTransferForm, TransationMain = y })
                                    .GroupJoin(
                                        qrCodeMain,
                                        x => new { QRCode_ID = x.TransationMain.QRCode_ID, QRCode_Version = x.TransationMain.QRCode_Version },
                                        y => new { QRCode_ID = y.QRCode_ID, QRCode_Version = y.QRCode_Version },
                                        (x, y) => new { GenerateTransferFormTransactionMain = x, QrCodeMain = y })
                                        .SelectMany(
                                            x => x.QrCodeMain.DefaultIfEmpty(),
                                            (x, y) => new { GenerateTransferFormTransactionMain = x.GenerateTransferFormTransactionMain, QrCodeMain = y })
                                            .GroupJoin(
                                                packingList,
                                                x => x.QrCodeMain.Receive_No,
                                                y => y.Receive_No,
                                                (x, y) => new { GenerateTransferFormTransactionMainQrCodeMain = x, PackingList = y })
                                                .SelectMany(
                                                    x => x.PackingList.DefaultIfEmpty(),
                                                    (x, y) => new Transfer_Form_Print_Dto
                                                    {
                                                        Rack_Location = x.GenerateTransferFormTransactionMainQrCodeMain.GenerateTransferFormTransactionMain.TransationMain.Rack_Location,
                                                        Collect_Trans_No = x.GenerateTransferFormTransactionMainQrCodeMain.GenerateTransferFormTransactionMain.GenerateTransferForm.Collect_Trans_No,
                                                        MO_No = x.GenerateTransferFormTransactionMainQrCodeMain.GenerateTransferFormTransactionMain.TransationMain.MO_No,
                                                        MO_Seq = x.GenerateTransferFormTransactionMainQrCodeMain.GenerateTransferFormTransactionMain.TransationMain.MO_Seq,
                                                        Material_ID = x.GenerateTransferFormTransactionMainQrCodeMain.GenerateTransferFormTransactionMain.TransationMain.Material_ID,
                                                        Material_Name = x.GenerateTransferFormTransactionMainQrCodeMain.GenerateTransferFormTransactionMain.TransationMain.Material_Name,
                                                        T3_Supplier = y.T3_Supplier,
                                                        T3_Supplier_Name = y.T3_Supplier_Name,
                                                        Supplier_ID = y.Supplier_ID,
                                                        Supplier_Name = y.Supplier_Name,
                                                        Article = y.Article,
                                                        Subcon_ID = y.Subcon_ID,
                                                        Subcon_Name = y.Subcon_Name,
                                                        Model_Name = y.Model_Name,
                                                        Model_No = y.Model_No,
                                                        Custmoer_Part = x.GenerateTransferFormTransactionMainQrCodeMain.GenerateTransferFormTransactionMain.GenerateTransferForm.Custmoer_Part,
                                                        Custmoer_Name = x.GenerateTransferFormTransactionMainQrCodeMain.GenerateTransferFormTransactionMain.GenerateTransferForm.Custmoer_Name,
                                                        Line_ASY = _cacheRepository.GetLineASY(y.MO_No, y.MO_Seq, y.Material_ID, y.Purchase_No),
                                                        Line_STF = _materialViewRepository.GetLineSTF(y.MO_No, y.MO_Seq, y.Material_ID, y.Purchase_No),
                                                        TransferFormQty = GetTransferFormPrintQty(x.GenerateTransferFormTransactionMainQrCodeMain.GenerateTransferFormTransactionMain.GenerateTransferForm.Collect_Trans_No)
                                                    }).ToList();

            return await Task.FromResult(data);
        }

        public List<Transfer_Form_Print_Qty_Dto> GetTransferFormPrintQty(string collectTransNo)
        {
            // Trans_Qty
            var transactionDetailResult = GetTransQtyTransferForm(collectTransNo);
            // MO_Qty
            var packingListDetailResult = GetMOQtyTransferForm(collectTransNo);
            // Act_Qty
            var transactionDetailResultAct = GetAccumulatedTransQtyReleaseTransferForm(collectTransNo);
            // Act_Trans_Qty
            // var transactionDetailResultActTranQty = GetAccumulatedTransQtyTransferForm(collectTransNo);

            List<Transfer_Form_Print_Qty_Dto> data = new List<Transfer_Form_Print_Qty_Dto>();
            if (transactionDetailResultAct.Any())
            {
                data = transactionDetailResult.Join(
                        packingListDetailResult,
                        x => new { Tool_Size = x.Tool_Size.Trim(), Order_Size = x.Order_Size.Trim() },
                        y => new { Tool_Size = y.Tool_Size.Trim(), Order_Size = y.Order_Size.Trim() },
                        (x, y) => new { x, y })
                                    .Join(
                                        transactionDetailResultAct,
                                        x => new { Tool_Size = x.x.Tool_Size.Trim(), Order_Size = x.x.Order_Size.Trim() },
                                        y => new { Tool_Size = y.Tool_Size.Trim(), Order_Size = y.Order_Size.Trim() },
                                        (x, y) => new Transfer_Form_Print_Qty_Dto
                                        {
                                            Tool_Size = x.x.Tool_Size,
                                            Order_Size = x.x.Order_Size,
                                            Trans_Qty = x.x.Trans_Qty,
                                            MO_Qty = x.y.MO_Qty,
                                            Act_Qty = y.Trans_Qty
                                        }).OrderBy(x => x.Order_Size).ThenBy(x => x.Tool_Size).ToList();
            }
            else
            {
                data = transactionDetailResult.Join(
                        packingListDetailResult,
                        x => new { Tool_Size = x.Tool_Size.Trim(), Order_Size = x.Order_Size.Trim() },
                        y => new { Tool_Size = y.Tool_Size.Trim(), Order_Size = y.Order_Size.Trim() },
                        (x, y) => new Transfer_Form_Print_Qty_Dto
                                {
                                    Tool_Size = x.Tool_Size,
                                    Order_Size = x.Order_Size,
                                    Trans_Qty = x.Trans_Qty,
                                    MO_Qty = y.MO_Qty,
                                }).OrderBy(x => x.Order_Size).ThenBy(x => x.Tool_Size).ToList();
            }
            return data;
        }

        public async Task<PagedList<Transfer_Form_Generate_Dto>> GetTransferFormGerenate(string fromTime, string toTime, string moNo, string isSubcont, string t2Supplier, string t3Supplier, int pageNumber = 1, int pageSize = 10)
        {
            var transactionMain = _transactionMainRepo.FindAll(x => x.Is_Transfer_Form == "N" && x.Transac_Type != "R" && x.Can_Move == "Y");
            var qrCodeMain = _qRCodeMainRepository.FindAll();
            var packingList = _packingListRepository.FindAll();
            var pakingListDetail = _packingListDetailRepository.FindAll();
            var materialPurcharse = _materialPurchaseRepository.FindAll();

            if (!string.IsNullOrEmpty(moNo))
            {
                transactionMain = transactionMain.Where(x => x.MO_No.Trim() == moNo.Trim());
            }

            if (!string.IsNullOrEmpty(fromTime) && !string.IsNullOrEmpty(toTime))
            {
                DateTime t1 = Convert.ToDateTime(fromTime);
                DateTime t2 = Convert.ToDateTime(toTime + " 23:59:59");
                transactionMain = transactionMain.Where(x => x.Transac_Time >= t1 && x.Transac_Time <= t2);
            }

            var data = transactionMain.OrderByDescending(x => x.Transac_Time).ThenBy(x => x.MO_No).ThenBy(x => x.MO_Seq).ThenBy(x => x.Material_ID)
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
                                (x, y) => new Transfer_Form_Generate_Dto
                                {
                                    Material_ID = x.TransactionMainQrCodeMain.TransactionMain.Material_ID,
                                    Material_Name = x.TransactionMainQrCodeMain.TransactionMain.Material_Name,
                                    MO_No = x.TransactionMainQrCodeMain.TransactionMain.MO_No,
                                    MO_Seq = x.TransactionMainQrCodeMain.TransactionMain.MO_Seq,
                                    QRCode_ID = x.TransactionMainQrCodeMain.TransactionMain.QRCode_ID,
                                    Transac_No = x.TransactionMainQrCodeMain.TransactionMain.Transac_No,
                                    T3_Supplier = y.T3_Supplier,
                                    T3_Supplier_Name = y.T3_Supplier_Name,
                                    Supplier_ID = y.Supplier_ID,
                                    Supplier_Name = y.Supplier_Name,
                                    Subcon_ID = y.Subcon_ID,
                                    Subcon_Name = y.Subcon_Name,
                                    Purchase_Qty = pakingListDetail.Where(x => x.Receive_No == y.Receive_No).Sum(x => x.Purchase_Qty),
                                    Transacted_Qty = x.TransactionMainQrCodeMain.TransactionMain.Transacted_Qty,
                                    Factory_ID = materialPurcharse.FirstOrDefault(x => x.MO_No == y.MO_No && x.MO_Seq == y.MO_Seq && x.Material_ID == y.Material_ID && x.Purchase_No == y.Purchase_No) == null ? ""
                                                : materialPurcharse.FirstOrDefault(x => x.MO_No == y.MO_No && x.MO_Seq == y.MO_Seq && x.Material_ID == y.Material_ID && x.Purchase_No == y.Purchase_No).Factory_ID
                                });
            if (isSubcont == "N")
            {
                data = data.Where(x => x.Subcon_ID == "");
            }
            else // isSubcont == "Y"
            {
                data = data.Where(x => x.Subcon_ID != "");
            }
            if (!string.IsNullOrEmpty(t3Supplier))
            {
                data = data.Where(x => x.T3_Supplier.Trim() == t3Supplier.Trim());
            }
            if (!string.IsNullOrEmpty(t2Supplier))
            {
                data = data.Where(x => x.Supplier_ID.Trim() == t2Supplier.Trim());
            }

            return await PagedList<Transfer_Form_Generate_Dto>.CreateAsync(data, pageNumber, pageSize, false);
        }

        public async Task<PagedList<Transfer_Form_Generate_Dto>> GetTransferFormPrint(string fromTime, string toTime, string moNo, string isRelease, string t2Supplier, string t3Supplier, int pageNumber = 1, int pageSize = 10)
        {
            var transferForm = _transferFormRepository.FindAll(x => x.Is_Release == isRelease && x.Valid_Status == true);
            var transactionMain = _transactionMainRepo.FindAll();
            var qrCodeMain = _qRCodeMainRepository.FindAll();
            var packingList = _packingListRepository.FindAll();

            var pakingListDetail = _packingListDetailRepository.FindAll();
            var cache = _cacheRepository.FindAll();
            var settingSupplier = _settingSupplierRepository.FindAll();

            if (!string.IsNullOrEmpty(fromTime) && !string.IsNullOrEmpty(toTime) && isRelease == "Y")
            {
                DateTime t1 = Convert.ToDateTime(fromTime);
                DateTime t2 = Convert.ToDateTime(toTime + " 23:59:59");
                transferForm = transferForm.Where(x => x.Release_Time >= t1 && x.Release_Time <= t2);
            }
            if (!string.IsNullOrEmpty(t3Supplier))
            {
                packingList = packingList.Where(x => x.T3_Supplier.Trim() == t3Supplier.Trim());
            }

            var data = transferForm.OrderByDescending(x => x.Generate_Time).GroupJoin(
                                transactionMain,
                                e => e.Transac_No,
                                f => f.Transac_No,
                                (e, f) => new { e, f })
                            .SelectMany(
                                g => g.f.DefaultIfEmpty(),
                                (g, h) => new { g.e,  h })
                                .GroupJoin(
                                    qrCodeMain,
                                    x => new { QRCode_ID = x.h.QRCode_ID, QRCode_Version = x.h.QRCode_Version },
                                    y => new { QRCode_ID = y.QRCode_ID, QRCode_Version = y.QRCode_Version },
                                    (x, y) => new { x, y })
                                    .SelectMany(
                                        z => z.y.DefaultIfEmpty(),
                                        (z, t) => new { z.x, t })
                                        .Join(
                                            packingList,
                                            a => a.t.Receive_No,
                                            b => b.Receive_No,
                                            (a, b) => new Transfer_Form_Generate_Dto
                                                {
                                                    Transac_Time = a.x.h.Transac_Time,
                                                    Update_Time = a.x.e.Update_Time,
                                                    Release_Time = a.x.e.Release_Time,
                                                    Collect_Trans_No = a.x.e.Collect_Trans_No,
                                                    Is_Release = a.x.e.Is_Release,
                                                    Logmail_Info =  a.x.e.Logmail_Info,
                                                    Logmail_Release =  a.x.e.Logmail_Release,
                                                    CountLogEmail =  a.x.e.Is_Release == "Y" ? a.x.e.Logmail_Release : a.x.e.Logmail_Info,
                                                    Material_ID = a.x.h.Material_ID,
                                                    Material_Name = a.x.h.Material_Name,
                                                    MO_No = a.x.h.MO_No,
                                                    MO_Seq = a.x.h.MO_Seq,
                                                    QRCode_ID = a.x.h.QRCode_ID,
                                                    Transac_No = a.x.h.Transac_No,
                                                    T3_Supplier = b.T3_Supplier,
                                                    T3_Supplier_Name = b.T3_Supplier_Name,
                                                    Subcon_ID = b.Subcon_ID,
                                                    Subcon_Name = b.Subcon_Name,
                                                    Supplier_ID = b.Supplier_ID,
                                                    Supplier_Name = b.Supplier_Name,
                                                    Model_Name = b.Model_Name,
                                                    Model_No = b.Model_No,
                                                    Article = b.Article,
                                                    Custmoer_Part = cache.FirstOrDefault(x => x.MO_No == b.MO_No && x.MO_Seq == b.MO_Seq && x.Material_ID == b.Material_ID) == null ? ""
                                                                    : cache.FirstOrDefault(x => x.MO_No == b.MO_No && x.MO_Seq == b.MO_Seq && x.Material_ID == b.Material_ID).Part_No,
                                                    Custmoer_Name = cache.FirstOrDefault(x => x.MO_No == b.MO_No && x.MO_Seq == b.MO_Seq && x.Material_ID == b.Material_ID) == null ? ""
                                                                    : cache.FirstOrDefault(x => x.MO_No == b.MO_No && x.MO_Seq == b.MO_Seq && x.Material_ID == b.Material_ID).Part_Name,
                                                    Purchase_Qty = pakingListDetail.Where(x => x.Receive_No == b.Receive_No).Sum(x => x.Purchase_Qty),
                                                    Transacted_Qty = a.x.h.Transacted_Qty,
                                                    Email = _settingSupplierRepository.GetEmailByT3Supplier(b.T3_Supplier, b.Subcon_ID),
                                                });


            if (!string.IsNullOrEmpty(moNo))
            {
                data = data.Where(x => x.MO_No == moNo);
            }
            if (!string.IsNullOrEmpty(t2Supplier))
            {
                data = data.Where(x => x.Supplier_ID == t2Supplier);
            }
            if (!string.IsNullOrEmpty(fromTime) && !string.IsNullOrEmpty(toTime) && isRelease == "N")
            {
                DateTime t1 = Convert.ToDateTime(fromTime);
                DateTime t2 = Convert.ToDateTime(toTime + " 23:59:59");
                data = data.Where(x => x.Transac_Time >= t1 && x.Transac_Time <= t2);
            }

            var dataList = data.ToList();

            List<Transfer_Form_Generate_Dto> resultList = new List<Transfer_Form_Generate_Dto>();

            var dataGroup = dataList.GroupBy(x => x.Collect_Trans_No);
            foreach (var item in dataGroup)
            {
                var itemDataGroup = dataList.Where(x => x.Collect_Trans_No == item.Key).Select(x => new Transfer_Form_Generate_Dto
                {
                    Update_Time = x.Update_Time,
                    Collect_Trans_No = x.Collect_Trans_No,
                    Is_Release = x.Is_Release,
                    Material_ID = x.Material_ID,
                    Material_Name = x.Material_Name,
                    MO_No = x.MO_No,
                    MO_Seq = x.MO_Seq,
                    Transac_No = x.Transac_No,
                    T3_Supplier = x.T3_Supplier,
                    T3_Supplier_Name = x.T3_Supplier_Name,
                    Supplier_ID = x.Supplier_ID,
                    Supplier_Name = x.Supplier_Name,
                    Email = x.Email,
                    Release_Time = x.Release_Time,
                    Subcon_ID = x.Subcon_ID,
                    Subcon_Name = x.Subcon_Name,
                    Custmoer_Name = x.Custmoer_Name,
                    Custmoer_Part = x.Custmoer_Part,
                    Model_Name = x.Model_Name,
                    Model_No = x.Model_No,
                    Article = x.Article,
                    CountLogEmail = x.CountLogEmail,
                    Purchase_Qty = dataList.Where(x => x.Collect_Trans_No == item.Key).Sum(x => x.Purchase_Qty)/ (dataList.Where(x => x.Collect_Trans_No == item.Key).Count()),
                    Transacted_Qty = dataList.Where(x => x.Collect_Trans_No == item.Key).Sum(x => x.Transacted_Qty),
                }).FirstOrDefault();
                resultList.Add(itemDataGroup);
            }

            resultList = resultList.OrderByDescending(x => x.Update_Time).ThenBy(x => x.MO_No).ThenBy(x => x.MO_Seq).ThenBy(x => x.Material_ID).ToList();

            return await Task.FromResult(PagedList<Transfer_Form_Generate_Dto>.Create(resultList, pageNumber, pageSize, false));
        }

        public async Task<bool> ReleaseTransferForm(List<Transfer_Form_Generate_Dto> generateTransferForm, string updateBy)
        {
            DateTime timeNow = DateTime.Now;
            foreach (var item in generateTransferForm)
            {
                var transferFormList = _transferFormRepository.FindAll(x => x.Collect_Trans_No.Trim() == item.Collect_Trans_No.Trim() && x.Valid_Status == true).ToList();
                foreach (var transferForm in transferFormList)
                {
                    transferForm.Is_Release = "Y";
                    transferForm.Release_By = updateBy;
                    transferForm.Release_Time = timeNow;
                    _transferFormRepository.Update(transferForm);
                }
            }

            return await _transferFormRepository.SaveAll();
        }

        public async Task SendEmail(Transfer_Form_Generate_Dto dataItem, string pathFileExcel)
        {
            var dataPar = _settingSupplierRepository.FindAll();

            var dataMail = dataPar.Where(x => x.Supplier_No == dataItem.T3_Supplier).FirstOrDefault();

            if (dataItem.T3_Supplier == "")
                dataMail = dataPar.Where(x => x.Supplier_No == "ZZZZ").FirstOrDefault();

            if (dataItem.T3_Supplier == "0000")
                dataMail = dataPar.Where(x => x.Supplier_No == dataItem.T3_Supplier && x.Subcon_ID == dataItem.Subcon_ID).FirstOrDefault();
            var dateNow = DateTime.Now.ToString("yyyy/MM/dd");
            var timeNow = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            string subject = dataMail.Subject + "-" + dateNow;
            if (dataItem.Is_Release == "Y")
            {
                subject = "Released " + subject;
            }
            var t3SupplierName = dataItem.T3_Supplier == "" ? "ZZZZ" : dataItem.T3_Supplier + "-" + dataItem.T3_Supplier_Name;
            string content = "通知時間：" + timeNow + "\n"
                            + "Notification time: " + timeNow + "\n"
                            + "T3 廠商：" + t3SupplierName + "\n"
                            + "T3 Supplier: " + t3SupplierName + "\n"
                            + dataMail.Content;
            List<string> listMail = dataMail.Email.Split(";").ToList();
            await _iMailUtility.SendListMailAsync(listMail, subject, content, pathFileExcel);
        }

        public async Task<List<Transfer_Form_Excel_Dto>> GetDataExcelTransferForm(List<Transfer_Form_Generate_Dto> generateTransferForm)
        {
            List<Transfer_Form_Excel_Dto> result = new List<Transfer_Form_Excel_Dto>();
            foreach (var item in generateTransferForm)
            {
                var dataPar = _settingSupplierRepository.FindAll();

                var dataMail = dataPar.Where(x => x.Supplier_No == item.T3_Supplier).FirstOrDefault();

                if (item.T3_Supplier == "")
                    dataMail = dataPar.Where(x => x.Supplier_No == "ZZZZ").FirstOrDefault();

                if (item.T3_Supplier == "0000")
                    dataMail = dataPar.Where(x => x.Supplier_No == item.T3_Supplier && x.Subcon_ID == item.Subcon_ID).FirstOrDefault();

                var planQty = GetMOQtyTransferForm(item.Collect_Trans_No).Sum(x => x.MO_Qty);
                var transQty = GetTransQtyTransferForm(item.Collect_Trans_No).Sum(x => x.Trans_Qty);
                var accumulatedTransQty = GetAccumulatedTransQtyReleaseTransferForm(item.Collect_Trans_No).Sum(x => x.Trans_Qty);
                var transQtyByToolSizeExportExcelTransferForm = await GetTransQtyByToolSizeExportExcelTransferForm(item.Collect_Trans_No);

                Transfer_Form_Excel_Dto data = new Transfer_Form_Excel_Dto();
                data.Release_Time = item.Release_Time == null ? "" : Convert.ToDateTime(item.Release_Time).ToString("yyyy/MM/dd HH:mm:ss");
                data.Collect_Trans_No = item.Collect_Trans_No;
                data.MO_No = item.MO_No;
                data.MO_Seq = item.MO_Seq;
                data.Model_Name = item.Model_Name;
                data.Model_No = item.Model_No;
                data.Article = item.Article;
                data.Custmoer_Part = item.Custmoer_Part;
                data.Material_ID = item.Material_ID;
                data.Material_Name = item.Material_Name;
                data.T3_Supplier = item.T3_Supplier;
                data.T3_Supplier_Name = item.T3_Supplier_Name;
                data.Subcon_ID = item.Subcon_ID;
                data.Subcon_Name = item.Subcon_Name;
                data.Custmoer_Name = item.Custmoer_Name;
                data.Plan_Qty = planQty;
                data.Trans_Qty = transQty;
                data.Accumulated_Trans_Qty = accumulatedTransQty;
                data.Subject = dataMail.Subject;
                data.Trans_Qty_Of_All_Tool_Size = item.Is_Release == "Y" ? transQtyByToolSizeExportExcelTransferForm : "";
                result.Add(data);

                // Update count Log Send mail
                var transferForms = await _transferFormRepository.FindAll(x => x.Collect_Trans_No == item.Collect_Trans_No && x.Transac_No == item.Transac_No && x.Valid_Status == true).ToListAsync();
                foreach (var transferForm in transferForms)
                {
                    if (item.Is_Release == "N")
                {
                    if (transferForm.Logmail_Info == null)
                    {
                        transferForm.Logmail_Info = 1;
                    }
                    else
                    {
                        transferForm.Logmail_Info = transferForm.Logmail_Info + 1;
                    }
                }
                else
                {
                    if (transferForm.Logmail_Release == null)
                    {
                        transferForm.Logmail_Release = 1;
                    }
                    else 
                    {
                        transferForm.Logmail_Release = transferForm.Logmail_Release + 1;
                    }
                }
                }
                
                await _transferFormRepository.SaveAll();
            }

            return result;
        }

        #region Function
        public List<dynamic> GetAccumulatedTransQtyTransferForm(string collectTransNo)
        {
            List<dynamic> transactionDetailTotalActTranQty = new List<dynamic>();
            var transferFormActTranQty = _transferFormRepository.FindAll(x => x.Collect_Trans_No == collectTransNo && x.Valid_Status == true).FirstOrDefault();
            var transacNoActTranQty = _transferFormRepository.FindAll(x => x.MO_No == transferFormActTranQty.MO_No && 
                                    x.MO_Seq == transferFormActTranQty.MO_Seq && 
                                    x.Material_ID == transferFormActTranQty.Material_ID && 
                                    x.Valid_Status == true).Select(x => x.Transac_No).ToList();
            foreach (var itemTransacNoActTranQty in transacNoActTranQty)
            {
                var transactionDetail = _transactionDetailRepo.FindAll(x => x.Transac_No == itemTransacNoActTranQty).Select(x => new { x.Tool_Size, x.Order_Size, x.Trans_Qty }).OrderBy(x => x.Tool_Size).ToList();
                transactionDetailTotalActTranQty.AddRange(transactionDetail);
            }
            var transactionDetailResultActTranQty = transactionDetailTotalActTranQty.GroupBy(x => new { x.Tool_Size, x.Order_Size })
                                            .Select(x => new
                                            {
                                                Tool_Size = x.Key.Tool_Size,
                                                Order_Size = x.Key.Order_Size,
                                                Trans_Qty = x.Sum(x => Convert.ToInt32(x.Trans_Qty))
                                            }).ToList<dynamic>();
            return transactionDetailResultActTranQty;
        }

        public List<dynamic> GetAccumulatedTransQtyReleaseTransferForm(string collectTransNo)
        {
            List<dynamic> transactionDetailTotalAct = new List<dynamic>();
            var transferFormAct = _transferFormRepository.FindAll(x => x.Collect_Trans_No == collectTransNo && x.Valid_Status == true).FirstOrDefault();
            var transacNoAct = _transferFormRepository.FindAll(x => x.Transac_No.Trim() == transferFormAct.Transac_No.Trim() &&
                                                                x.Is_Release == "Y" && x.Valid_Status == true).Select(x => x.Transac_No).ToList();
            foreach (var itemTransacNoAct in transacNoAct)
            {
                var transactionDetail = _transactionDetailRepo.FindAll(x => x.Transac_No == itemTransacNoAct).Select(x => new { x.Tool_Size, x.Order_Size, x.Trans_Qty }).OrderBy(x => x.Tool_Size).ToList();
                transactionDetailTotalAct.AddRange(transactionDetail);
            }
            var transactionDetailResultAct = transactionDetailTotalAct.GroupBy(x => new { x.Tool_Size, x.Order_Size })
                                            .Select(x => new
                                            {
                                                Tool_Size = x.Key.Tool_Size,
                                                Order_Size = x.Key.Order_Size,
                                                Trans_Qty = x.Sum(x => Convert.ToInt32(x.Trans_Qty))
                                            }).ToList<dynamic>();
            return transactionDetailResultAct;
        }

        public List<dynamic> GetTransQtyTransferForm(string collectTransNo)
        {
            List<dynamic> transactionDetailTotal = new List<dynamic>();
            var transacNo = _transferFormRepository.FindAll(x => x.Collect_Trans_No == collectTransNo && x.Valid_Status == true).Select(x => x.Transac_No).ToList();
            foreach (var itemTransacNo in transacNo)
            {
                var transactionDetail = _transactionDetailRepo.FindAll(x => x.Transac_No == itemTransacNo).Select(x => new { x.Tool_Size, x.Order_Size, x.Trans_Qty }).OrderBy(x => x.Tool_Size).ToList();
                transactionDetailTotal.AddRange(transactionDetail);
            }
            var transactionDetailResult = transactionDetailTotal.GroupBy(x => new { x.Tool_Size, x.Order_Size })
                                            .Select(x => new
                                            {
                                                Tool_Size = x.Key.Tool_Size,
                                                Order_Size = x.Key.Order_Size,
                                                Trans_Qty = x.Sum(x => Convert.ToInt32(x.Trans_Qty))
                                            }).ToList<dynamic>();
            return transactionDetailResult;
        }

        public List<dynamic> GetMOQtyTransferForm(string collectTransNo)
        {
            var transacNo = _transferFormRepository.FindAll(x => x.Collect_Trans_No == collectTransNo).Select(x => x.Transac_No).ToList().FirstOrDefault();
            var transactionMainModel = _transactionMainRepo.FindAll(x => x.Transac_No.Trim() == transacNo.Trim()).FirstOrDefault();
            var qrcodeMainModel = _qRCodeMainRepository.FindAll(x => x.QRCode_ID.Trim() == transactionMainModel.QRCode_ID.Trim() &&
                                                        x.QRCode_Version == transactionMainModel.QRCode_Version).FirstOrDefault();
            var packingListDetailTotal = _packingListDetailRepository.FindAll(x => x.Receive_No.Trim() == qrcodeMainModel.Receive_No.Trim()).ToList(); 
            var packingListDetailResult = packingListDetailTotal.GroupBy(x => new { x.Tool_Size, x.Order_Size })
                                            .Select(x => new
                                            {
                                                Tool_Size = x.Key.Tool_Size,
                                                Order_Size = x.Key.Order_Size,
                                                MO_Qty = x.Sum(x => Convert.ToInt32(x.MO_Qty))
                                            }).ToList<dynamic>();
            return packingListDetailResult;
        }

        public async Task<string> GetTransQtyByToolSizeExportExcelTransferForm(string collectTransNo)
        {
            List<dynamic> transactionDetailTotal = new List<dynamic>();
            var transacNo = await _transferFormRepository.FindAll(x => x.Collect_Trans_No == collectTransNo && x.Valid_Status == true).Select(x => x.Transac_No).ToListAsync();
            foreach (var itemTransacNo in transacNo)
            {
                var transactionDetail = await _transactionDetailRepo.FindAll(x => x.Transac_No == itemTransacNo).Select(x => new { x.Tool_Size, x.Order_Size, x.Trans_Qty }).OrderBy(x => x.Tool_Size).ToListAsync();
                transactionDetailTotal.AddRange(transactionDetail);
            }
            var transactionDetailResult = transactionDetailTotal.GroupBy(x => new { x.Tool_Size })
                                            .Select(x => new
                                            {
                                                Tool_Size = x.Key.Tool_Size,
                                                Trans_Qty = x.Sum(x => Convert.ToInt32(x.Trans_Qty))
                                            }).ToList<dynamic>();
            string result = "";
            foreach (var item in transactionDetailResult)
            {
                result += item.Tool_Size + "(" + item.Trans_Qty + "), ";
            }
            return result;
        }

        #endregion
    }
}