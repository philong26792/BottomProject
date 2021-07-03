using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Bottom_API._Repositories.Interfaces;
using Bottom_API._Services.Interfaces;
using Bottom_API.DTO;
using Bottom_API.Models;
using Bottom_API.Helpers;
using System;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using System.Linq;
using LinqKit;
using Bottom_API.Data;
using Dapper;
using Bottom_API.DTO.Input;
using Microsoft.Data.SqlClient;

namespace Bottom_API._Services.Services
{
    public class InputService : IInputService
    {
        private readonly IPackingListRepository _repoPackingList;
        private readonly IPackingListDetailRepository _repoPacKingListDetail;
        private readonly IQRCodeMainRepository _repoQRCodeMain;
        private readonly IQRCodeDetailRepository _repoQRCodeDetail;
        private readonly ITransactionMainRepo _repoTransactionMain;
        private readonly ITransactionDetailRepo _repoTransactionDetail;
        private readonly IMaterialMissingRepository _repoMaterialMissing;
        private readonly IMaterialPurchaseRepository _repoMatPurchase;
        private readonly IMaterialMissingRepository _repoMatMissing;
        private readonly IMaterialViewRepository _repoMaterialView;
        private readonly IRackLocationRepo _repoRacklocationRepo;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _configMapper;
        private readonly ICacheRepository _cacheRepository;
        private readonly IDatabaseConnectionFactory _database;
        private readonly DataContext _context;
        public InputService(
            IPackingListRepository repoPackingList,
            IQRCodeMainRepository repoQRCodeMain,
            IQRCodeDetailRepository repoQRCodeDetail,
            ITransactionMainRepo repoTransactionMain,
            ITransactionDetailRepo repoTransactionDetail,
            IMaterialMissingRepository repoMaterialMissing,
            IMaterialPurchaseRepository repoMatPurchase,
            IMaterialMissingRepository repoMatMissing,
            IMaterialViewRepository repoMaterialView,
            IRackLocationRepo repoRacklocationRepo,
            IPackingListDetailRepository repoPacKingListDetail,
            IDatabaseConnectionFactory database,
            IMapper mapper,
            MapperConfiguration configMapper,
            ICacheRepository cacheRepository, DataContext context)
        {
            _configMapper = configMapper;
            _cacheRepository = cacheRepository;
            _mapper = mapper;
            _repoQRCodeMain = repoQRCodeMain;
            _repoQRCodeDetail = repoQRCodeDetail;
            _repoPackingList = repoPackingList;
            _repoTransactionMain = repoTransactionMain;
            _repoTransactionDetail = repoTransactionDetail;
            _repoMaterialMissing = repoMaterialMissing;
            _repoMatMissing = repoMatMissing;
            _repoMatPurchase = repoMatPurchase;
            _repoMaterialView = repoMaterialView;
            _repoRacklocationRepo = repoRacklocationRepo;
            _repoPacKingListDetail = repoPacKingListDetail;
            _database = database;
            _context = context;
        }
        public async Task<Transaction_Dto> GetByQRCodeID(object qrCodeID)
        {
            Transaction_Dto model = new Transaction_Dto();
            var qrCodeModel = await _repoQRCodeMain.GetByQRCodeID(qrCodeID);
            if (qrCodeModel != null)
            {
                var packingListModel = await _repoPackingList.GetByReceiveNo(qrCodeModel.Receive_No);
                var listQrCodeDetails = await _repoQRCodeDetail.GetByQRCodeIDAndVersion(qrCodeID, qrCodeModel.QRCode_Version);
                decimal? num = 0;
                foreach (var item in listQrCodeDetails)
                {
                    num += item.Qty;
                }

                model.QrCode_Id = qrCodeModel.QRCode_ID.Trim();
                model.Plan_No = packingListModel.MO_No.Trim();
                model.Suplier_No = packingListModel.Supplier_ID.Trim();
                model.Suplier_Name = packingListModel.Supplier_Name.Trim();
                model.Batch = packingListModel.MO_Seq;
                model.Mat_Id = packingListModel.Material_ID.Trim();
                model.Mat_Name = packingListModel.Material_Name.Trim();
                model.Accumated_Qty = num;
                model.Trans_In_Qty = 0;
                model.InStock_Qty = 0;
            }

            return model;
        }

        public async Task<Transaction_Detail_Dto> GetDetailByQRCodeID(object qrCodeID)
        {
            Transaction_Detail_Dto model = new Transaction_Detail_Dto();
            var qrCodeMainList = await _repoQRCodeMain.CheckQrCodeID(qrCodeID);
            if (qrCodeMainList.Count == 0)
            {
                return null;
            }
            else
            {
                var qrCodeModel = qrCodeMainList.Where(x => x.Is_Scanned.Trim() == "N").FirstOrDefault();
                if (qrCodeModel != null)
                {
                    var packingListModel = await _repoPackingList.GetByReceiveNo(qrCodeModel.Receive_No);
                    var listQrCodeDetails = await _repoQRCodeDetail.GetByQRCodeIDAndVersion(qrCodeID, qrCodeModel.QRCode_Version);
                    decimal? num = 0;
                    List<DetailSize> listDetail = new List<DetailSize>();
                    foreach (var item in listQrCodeDetails)
                    {
                        DetailSize detail = new DetailSize();
                        num += item.Qty;
                        detail.Tool_Size = item.Tool_Size;
                        detail.Size = item.Order_Size;
                        detail.Qty = item.Qty;
                        listDetail.Add(detail);
                    }
                    model.Suplier_No = packingListModel.Supplier_ID;
                    model.Suplier_Name = packingListModel.Supplier_Name;
                    model.Detail_Size = listDetail;
                    model.QrCode_Id = qrCodeModel.QRCode_ID.Trim();
                    model.Plan_No = packingListModel.MO_No.Trim();
                    model.Batch = packingListModel.MO_Seq;
                    model.Mat_Id = packingListModel.Material_ID.Trim();
                    model.Mat_Name = packingListModel.Material_Name.Trim();
                    model.Accumated_Qty = num;
                    model.Trans_In_Qty = 0;
                    model.InStock_Qty = 0;
                    model.Is_Scanned = qrCodeModel.Is_Scanned;
                    return model;
                }
                else
                {
                    model.Is_Scanned = "Y";
                    return model;
                }
            }
        }

        public async Task<bool> CreateInput(Transaction_Detail_Dto model, string updateBy)
        {
            var qrCodeModel = await _repoQRCodeMain.GetByQRCodeID(model.QrCode_Id);
            if (qrCodeModel != null && qrCodeModel.Valid_Status == "Y")
            {
                var listQrCodeDetails = await _repoQRCodeDetail.GetByQRCodeIDAndVersion(qrCodeModel.QRCode_ID, qrCodeModel.QRCode_Version);
                var packingListModel = await _repoPackingList.GetByReceiveNo(qrCodeModel.Receive_No);
                WMSB_Transaction_Main inputModel = new WMSB_Transaction_Main();
                inputModel.Transac_Type = "I";
                inputModel.Transac_No = model.Input_No;
                inputModel.Transac_Time = DateTime.Now;
                inputModel.QRCode_ID = qrCodeModel.QRCode_ID;
                inputModel.QRCode_Version = qrCodeModel.QRCode_Version;
                inputModel.MO_No = packingListModel.MO_No;
                inputModel.MO_Seq = packingListModel.MO_Seq;
                inputModel.Purchase_No = packingListModel.Purchase_No;
                inputModel.Material_ID = packingListModel.Material_ID;
                inputModel.Material_Name = packingListModel.Material_Name;
                inputModel.Purchase_Qty = model.Accumated_Qty;
                inputModel.Transacted_Qty = model.Trans_In_Qty;
                inputModel.Rack_Location = model.Rack_Location;
                inputModel.Can_Move = "Y";
                inputModel.Is_Transfer_Form = "N";
                inputModel.Updated_By = updateBy;
                inputModel.Updated_Time = DateTime.Now;
                _repoTransactionMain.Add(inputModel);

                var i = 0;
                foreach (var item in model.Detail_Size)
                {
                    WMSB_Transaction_Detail inputDetailModel = new WMSB_Transaction_Detail();
                    inputDetailModel.Transac_No = inputModel.Transac_No;
                    inputDetailModel.Tool_Size = listQrCodeDetails[i].Tool_Size;
                    inputDetailModel.Model_Size = listQrCodeDetails[i].Model_Size;
                    inputDetailModel.Order_Size = listQrCodeDetails[i].Order_Size;
                    inputDetailModel.Spec_Size = listQrCodeDetails[i].Spec_Size;
                    inputDetailModel.Qty = listQrCodeDetails[i].Qty;
                    inputDetailModel.Trans_Qty = item.Qty;
                    inputDetailModel.Instock_Qty = item.Qty;
                    inputDetailModel.Untransac_Qty = inputDetailModel.Qty - inputDetailModel.Trans_Qty;
                    inputDetailModel.Updated_By = updateBy;
                    inputDetailModel.Updated_Time = DateTime.Now;
                    _repoTransactionDetail.Add(inputDetailModel);
                    i += 1;
                }
                return await _repoTransactionMain.SaveAll();
            }
            return false;
        }

        public async Task<bool> SubmitInput(InputSubmitModel data, string updateBy)
        {
            foreach (var item in data.TransactionList)
            {
                await CreateInput(item, updateBy);
            }

            var lists = data.InputNoList;
            if (lists.Count > 0)
            {
                string Transac_Sheet_No;
                do
                {
                    string num = CodeUtility.RandomNumber(3);
                    Transac_Sheet_No = "IB" + DateTime.Now.ToString("yyyyMMdd") + num;
                } while (await _repoTransactionMain.CheckTranSheetNo(Transac_Sheet_No));
                var missingNoList = new List<string>();
                foreach (var item in lists)
                {
                    string Missing_No;
                    do
                    {
                        // Để Mising No khác nhau 
                        string num2 = CodeUtility.RandomNumber(3);
                        Missing_No = "BTM" + DateTime.Now.ToString("yyyyMMdd") + num2;
                    } while (await this._repoMaterialMissing.CheckMissingNo(Missing_No) || missingNoList.Contains(Missing_No));
                    missingNoList.Add(Missing_No);

                    WMSB_Transaction_Main model = await _repoTransactionMain.GetByInputNo(item);
                    model.Can_Move = "Y";
                    model.Transac_Sheet_No = Transac_Sheet_No;
                    model.Is_Transfer_Form = "N";
                    model.Updated_By = updateBy;
                    model.Updated_Time = DateTime.Now;
                    if (model.Purchase_Qty > model.Transacted_Qty)
                    {
                        model.Missing_No = Missing_No;

                        //Tạo mới record và update status record cũ trong bảng QRCode_Main và QRCode_Detail
                        GenerateNewQrCode(model.QRCode_ID, model.QRCode_Version, item, updateBy);

                        //Update QrCode Version cho bảng Transaction_Main
                        model.QRCode_Version += 1;

                        // Tạo mới record trong bảng Missing
                        CreateMissing(model.Purchase_No, model.MO_No, model.MO_Seq, model.Material_ID, model.Transac_No, model.Missing_No, updateBy);
                    }
                    else
                    {
                        WMSB_QRCode_Main qrModel = await _repoQRCodeMain.GetByQRCodeID(model.QRCode_ID);
                        qrModel.Is_Scanned = "Y";
                        _repoQRCodeMain.Update(qrModel);
                    }

                    _repoTransactionMain.Update(model);
                }
                return await _repoTransactionMain.SaveAll();
            }
            return false;
        }

        public async Task<MissingPrint_Dto> GetMaterialPrint(string missingNo, string mO_Seq)
        {
            var materialMissingModel = await _repoMaterialMissing.FindAll(x => x.Missing_No.Trim() == missingNo.Trim() && x.MO_Seq == mO_Seq).ProjectTo<Material_Dto>(_configMapper).FirstOrDefaultAsync();
            var transactionMainModel = await _repoTransactionMain.FindAll(x => x.Missing_No.Trim() == missingNo.Trim() && x.MO_Seq == mO_Seq).Select(x => x.Transac_No).ToListAsync();
            var transactionDetailByMissingNo = await _repoTransactionDetail.FindAll(x => transactionMainModel.Contains(x.Transac_No.Trim())).ProjectTo<TransferLocationDetail_Dto>(_configMapper)
                                                .GroupBy(x => new { x.Tool_Size, x.Order_Size, x.Model_Size })
                                                .Select(x => new TransferLocationDetail_Dto
                                                {
                                                    Tool_Size = x.Key.Tool_Size,
                                                    Model_Size = x.Key.Model_Size,
                                                    Order_Size = x.Key.Order_Size,
                                                    Trans_Qty = x.Sum(z => z.Trans_Qty),
                                                    Qty = x.Sum(z => z.Qty),
                                                    Untransac_Qty = x.Sum(x => x.Untransac_Qty)
                                                }).OrderBy(x => x.Tool_Size).ToListAsync();
            var materialPurchaseModel = await _repoMaterialView
                            .FindAll(x => x.Plan_No.Trim() == materialMissingModel.MO_No.Trim() &&
                                  x.Purchase_No.Trim() == materialMissingModel.Purchase_No.Trim() &&
                                  x.Mat_.Trim() == materialMissingModel.Material_ID.Trim())
                            .FirstOrDefaultAsync();

            // nếu materialPurchaseModel rỗng thì  Custmoer_Name gán bằng rỗng
            materialMissingModel.Custmoer_Name = materialPurchaseModel == null ? "" : materialPurchaseModel.Custmoer_Name;
            // Lấy ra những thuộc tính cần in
            MissingPrint_Dto result = new MissingPrint_Dto();
            result.MaterialMissing = materialMissingModel;
            result.TransactionDetailByMissingNo = transactionDetailByMissingNo;

            return result;
        }

        public async Task<List<MissingPrint_Dto>> GetListMaterialPrint(List<string> listMissingNo)
        {
            var listResult = new List<MissingPrint_Dto>();

            foreach (var missingNo in listMissingNo.Distinct())
            {
                var materialMissingModel = await _repoMaterialMissing.FindAll(x => x.Missing_No.Trim() == missingNo.Trim()).ProjectTo<Material_Dto>(_configMapper).FirstOrDefaultAsync();
                var transactionMainModel = await _repoTransactionMain.FindAll(x => x.Missing_No.Trim() == missingNo.Trim()).Select(x => x.Transac_No).ToListAsync();
                var transactionDetailByMissingNo = await _repoTransactionDetail.FindAll(x => transactionMainModel.Contains(x.Transac_No.Trim())).ProjectTo<TransferLocationDetail_Dto>(_configMapper)
                                                    .GroupBy(x => new { x.Tool_Size, x.Order_Size, x.Model_Size })
                                                    .Select(x => new TransferLocationDetail_Dto
                                                    {
                                                        Tool_Size = x.Key.Tool_Size,
                                                        Model_Size = x.Key.Model_Size,
                                                        Order_Size = x.Key.Order_Size,
                                                        Trans_Qty = x.Sum(z => z.Trans_Qty),
                                                        Qty = x.Sum(z => z.Qty),
                                                        Untransac_Qty = x.Sum(x => x.Untransac_Qty)
                                                    }).OrderBy(x => x.Tool_Size).ToListAsync();
                var materialPurchaseModel = await _repoMaterialView
                                .FindAll(x => x.Plan_No.Trim() == materialMissingModel.MO_No.Trim() &&
                                      x.Purchase_No.Trim() == materialMissingModel.Purchase_No.Trim() &&
                                      x.Mat_.Trim() == materialMissingModel.Material_ID.Trim())
                                .FirstOrDefaultAsync();

                // nếu materialPurchaseModel rỗng thì  Custmoer_Name gán bằng rỗng
                materialMissingModel.Custmoer_Name = materialPurchaseModel == null ? "" : materialPurchaseModel.Custmoer_Name;
                // Lấy ra những thuộc tính cần in
                MissingPrint_Dto result = new MissingPrint_Dto();
                result.MaterialMissing = materialMissingModel;
                result.TransactionDetailByMissingNo = transactionDetailByMissingNo;
                listResult.Add(result);
            }

            return listResult;
        }
        private void CreateMissing(string Purchase_No, string MO_No, string MO_Seq, string Material_ID, string transacNo, string Missing_No, string updateBy)
        {
            //Lấy list Material Purchase
            var matPurchase = _repoMatPurchase.GetFactory(Purchase_No, MO_No, MO_Seq, Material_ID);

            //Lấy PackingList
            var packingList = _repoPackingList.GetPackingList(Purchase_No, MO_No, MO_Seq, Material_ID);
            //Lấy danh sách transaction detail
            List<WMSB_Transaction_Detail> listDetails = _repoTransactionDetail.GetListTransDetailByTransacNo(transacNo);
            foreach (var detail in listDetails)
            {
                WMSB_Material_Missing model = new WMSB_Material_Missing();
                model.Missing_No = Missing_No;
                model.Purchase_No = Purchase_No;
                model.MO_No = MO_No;
                model.MO_Seq = MO_Seq;
                model.Material_ID = packingList.Material_ID;
                model.Material_Name = packingList.Material_Name;
                model.Model_No = packingList.Model_No;
                model.Model_Name = packingList.Model_Name;
                model.Article = packingList.Article;
                model.Supplier_ID = packingList.Supplier_ID;
                model.Supplier_Name = packingList.Supplier_Name;
                model.Process_Code = packingList.Subcon_ID;
                model.Subcon_Name = packingList.Subcon_Name;
                model.T3_Supplier = packingList.T3_Supplier;
                model.T3_Supplier_Name = packingList.T3_Supplier_Name;
                model.Order_Size = detail.Order_Size;
                model.Model_Size = detail.Model_Size;
                model.Tool_Size = detail.Tool_Size;
                model.Spec_Size = detail.Spec_Size;
                model.Purchase_Qty = detail.Untransac_Qty;
                model.Accumlated_In_Qty = 0;
                model.Status = "N";
                model.Is_Missing = "Y";
                model.Updated_Time = DateTime.Now;
                model.Updated_By = updateBy;
                model.Missing_Stage = "R1";
                foreach (var purchase in matPurchase)
                {
                    if (detail.Order_Size == purchase.Order_Size)
                    {
                        model.Factory_ID = purchase.Factory_ID;
                        model.MO_Qty = purchase.MO_Qty;
                        model.PreBook_Qty = purchase.PreBook_Qty;
                        model.Stock_Qty = purchase.Stock_Qty;
                        model.Require_Delivery = purchase.Require_Delivery;
                        model.Confirm_Delivery = purchase.Confirm_Delivery;
                        model.Custmoer_Part = purchase.Custmoer_Part;
                        model.T3_Purchase_No = purchase.T3_Purchase_No;
                        model.Stage = purchase.Stage;
                        model.Tool_ID = purchase.Tool_ID;
                        model.Tool_Type = purchase.Tool_Type;
                        model.Purchase_Kind = purchase.Purchase_Kind;
                        model.Collect_No = purchase.Collect_No;
                        model.Purchase_Size = purchase.Purchase_Size;
                    }
                }
                _repoMatMissing.Add(model);
            }
        }
        private void GenerateNewQrCode(string qrCodeID, int qrCodeVersion, string transacNo, string updateBy)
        {
            //Update dòng QRCodeMain cũ
            var qrCodeMain = _repoQRCodeMain.GetByQRCodeIDAndVersion(qrCodeID, qrCodeVersion);
            qrCodeMain.Valid_Status = "N";
            qrCodeMain.Is_Scanned = "Y";
            qrCodeMain.Invalid_Date = DateTime.Now;
            _repoQRCodeMain.Update(qrCodeMain);

            //Thêm QRCode mới và update version lên 
            WMSB_QRCode_Main model = new WMSB_QRCode_Main();
            model.QRCode_ID = qrCodeID;
            model.QRCode_Version = qrCodeMain.QRCode_Version + 1;
            model.QRCode_Type = qrCodeMain.QRCode_Type;
            model.Receive_No = qrCodeMain.Receive_No;
            model.Valid_Status = "Y";
            model.Is_Scanned = "Y";
            model.Updated_By = updateBy;
            model.Updated_Time = DateTime.Now;
            _repoQRCodeMain.Add(model);

            //Lấy danh sách transaction detail
            List<WMSB_Transaction_Detail> listDetails = _repoTransactionDetail.GetListTransDetailByTransacNo(transacNo);

            //Tạo mới các dòng QRCode Detail dựa trên QRcode version mới với Qty tương ứng với Trans_Qty của Transaction_Detail
            foreach (var item in listDetails)
            {
                WMSB_QRCode_Detail detailQRCode = new WMSB_QRCode_Detail();
                detailQRCode.QRCode_ID = qrCodeID;
                detailQRCode.QRCode_Version = model.QRCode_Version;
                detailQRCode.Tool_Size = item.Tool_Size;
                detailQRCode.Model_Size = item.Model_Size;
                detailQRCode.Order_Size = item.Order_Size;
                detailQRCode.Spec_Size = item.Spec_Size;
                detailQRCode.Qty = item.Trans_Qty;
                detailQRCode.Updated_By = updateBy;
                detailQRCode.Updated_Time = DateTime.Now;
                _repoQRCodeDetail.Add(detailQRCode);
            }
        }

        public async Task<PagedList<QrCodeAgain_Dto>> FilterQrCodeAgain(PaginationParams param, FilterQrCodeAgainParam filterParam)
        {
            var pred_List_Transaction_Main = PredicateBuilder.New<WMSB_Transaction_Main>(true);
            pred_List_Transaction_Main.And(x => x.Can_Move == "Y");

            if (!String.IsNullOrEmpty(filterParam.To_Date) && !String.IsNullOrEmpty(filterParam.From_Date))
            {
                pred_List_Transaction_Main.And(x => x.Transac_Time >= Convert.ToDateTime(filterParam.From_Date + " 00:00:00.000") &&
                    x.Transac_Time <= Convert.ToDateTime(filterParam.To_Date + " 23:59:59.997"));
            }
            if (!String.IsNullOrEmpty(filterParam.MO_No))
            {
                pred_List_Transaction_Main.And(x => x.MO_No.Trim() == filterParam.MO_No.Trim());
            }
            if (!String.IsNullOrEmpty(filterParam.Rack_Location))
            {
                pred_List_Transaction_Main.And(x => x.Rack_Location.Trim() == filterParam.Rack_Location.Trim());
            }
            if (!String.IsNullOrEmpty(filterParam.Material_ID))
            {
                pred_List_Transaction_Main.And(x => x.Material_ID.Trim() == filterParam.Material_ID.Trim());
            }
            var listTransactionMain = _repoTransactionMain.FindAll(pred_List_Transaction_Main);
            var packingLists = _repoPackingList.FindAll();
            var data = (from a in listTransactionMain
                        join b in packingLists
on new { MO_No = a.MO_No.Trim(), Purchase_No = a.Purchase_No.Trim(), Mo_Seq = a.MO_Seq, Material_View_Dto = a.Material_ID.Trim() }
equals new { MO_No = b.MO_No.Trim(), Purchase_No = b.Purchase_No.Trim(), Mo_Seq = b.MO_Seq, Material_View_Dto = b.Material_ID.Trim() }
                        select new QrCodeAgain_Dto()
                        {
                            ID = a.ID,
                            Transac_Type = a.Transac_Type,
                            Transac_No = a.Transac_No,
                            Transac_Sheet_No = a.Transac_Sheet_No,
                            Can_Move = a.Can_Move,
                            Transac_Time = a.Transac_Time,
                            QRCode_ID = a.QRCode_ID,
                            QRCode_Version = a.QRCode_Version,
                            MO_No = a.MO_No,
                            Purchase_No = a.Purchase_No,
                            MO_Seq = a.MO_Seq,
                            Material_ID = a.Material_ID,
                            Material_Name = a.Material_Name,
                            Purchase_Qty = a.Purchase_Qty,
                            Transacted_Qty = a.Transacted_Qty,
                            Rack_Location = a.Rack_Location,
                            Missing_No = a.Missing_No,
                            Pickup_No = a.Pickup_No,
                            Supplier_ID = b.Supplier_ID,
                            Supplier_Name = b.Supplier_Name,
                            Updated_By = a.Updated_By,
                            Updated_Time = a.Updated_Time
                        });
            if (!String.IsNullOrEmpty(filterParam.Supplier_ID) && filterParam.Supplier_ID.Trim() != "All")
            {
                data = data.Where(x => x.Supplier_ID.Trim() == filterParam.Supplier_ID.Trim());
            }
            data = data.Distinct().OrderByDescending(x => x.Updated_Time);
            return await PagedList<QrCodeAgain_Dto>.CreateAsync(data, param.PageNumber, param.PageSize, false);
        }

        public async Task<string> FindMaterialName(string materialID)
        {
            if (!String.IsNullOrEmpty(materialID))
            {
                var materialModel = await _repoMaterialView.FindAll(x => x.Mat_.Trim() == materialID.Trim()).FirstOrDefaultAsync();
                return materialModel != null ? materialModel.Mat__Name : "";
            }
            else
            {
                return "";
            }
        }

        public async Task<PagedList<MissingAgain_Dto>> FilterMissingPrint(PaginationParams param, FilterMissingParam filterParam)
        {
            var transacTypeList = new List<string>{"I", "M", "R"};
            var transactionMain = _repoTransactionMain.FindAll(x => transacTypeList.Contains(x.Transac_Type) && x.Missing_No != null);
            var qrCodeMain = _repoQRCodeMain.FindAll();
            var packingList = _repoPackingList.FindAll();
            var cache = _cacheRepository.FindAll();
            var materialMissing = _repoMaterialMissing.FindAll();
            if (!String.IsNullOrEmpty(filterParam.MO_No))
            {
                transactionMain = transactionMain.Where(x => x.MO_No == filterParam.MO_No);
            }
            if (filterParam.Downloaded != "All")
            {
                var listMissing = new List<string>();
                if(filterParam.Downloaded =="N")
                {
                    listMissing =materialMissing.Where(x=>x.Download_count==0 || x.Download_count ==null)
                    .Select(x=>x.Missing_No).Distinct().ToList();   
                }
                else
                {
                    listMissing =materialMissing.Where(x=>x.Download_count>0)
                    .Select(x=>x.Missing_No).Distinct().ToList();
                }
                
                transactionMain = transactionMain.Where(x =>listMissing.Contains(x.Missing_No));
            }
            if (filterParam.Material_ID != null && filterParam.Material_ID != "")
            {
                transactionMain = transactionMain.Where(x => x.Material_ID == filterParam.Material_ID);
            }
            if (!string.IsNullOrEmpty(filterParam.FromTime) && !string.IsNullOrEmpty(filterParam.ToTime))
            {
                DateTime t1 = Convert.ToDateTime(filterParam.FromTime + " 00:00:00");
                DateTime t2 = Convert.ToDateTime(filterParam.ToTime + " 23:59:59");
                materialMissing = materialMissing.Where(x => x.Updated_Time >= t1 && x.Updated_Time <= t2);
            }

            var lists = transactionMain
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
                                (x, y) => new MissingAgain_Dto
                                {
                                    Material_ID = x.TransactionMainQrCodeMain.TransactionMain.Material_ID,
                                    Material_Name = x.TransactionMainQrCodeMain.TransactionMain.Material_Name,
                                    Missing_No = x.TransactionMainQrCodeMain.TransactionMain.Missing_No,
                                    MO_No = x.TransactionMainQrCodeMain.TransactionMain.MO_No,
                                    MO_Seq = x.TransactionMainQrCodeMain.TransactionMain.MO_Seq,
                                    Updated_Time = x.TransactionMainQrCodeMain.TransactionMain.Updated_Time,
                                    Model_No = y.Model_No,
                                    Model_Name = y.Model_Name,
                                    Article = y.Article,
                                    Supplier_ID = y.Supplier_ID,
                                    Type = _repoMaterialMissing.GetReasonKind(x.TransactionMainQrCodeMain.TransactionMain.Missing_No),
                                    Custmoer_Part = cache.FirstOrDefault(x => x.MO_No == y.MO_No && x.MO_Seq == y.MO_Seq && x.Material_ID == y.Material_ID) == null ? ""
                                                    : cache.FirstOrDefault(x => x.MO_No == y.MO_No && x.MO_Seq == y.MO_Seq && x.Material_ID == y.Material_ID).Part_No,
                                    Custmoer_Name = cache.FirstOrDefault(x => x.MO_No == y.MO_No && x.MO_Seq == y.MO_Seq && x.Material_ID == y.Material_ID) == null ? ""
                                                    : cache.FirstOrDefault(x => x.MO_No == y.MO_No && x.MO_Seq == y.MO_Seq && x.Material_ID == y.Material_ID).Part_Name,
                                    Missing_Qty = materialMissing.Where(z => z.Missing_No == x.TransactionMainQrCodeMain.TransactionMain.Missing_No && z.MO_Seq == y.MO_Seq).Sum(z => z.Purchase_Qty),
                                    Download_count = materialMissing.Where(z=>z.Missing_No ==x.TransactionMainQrCodeMain.TransactionMain.Missing_No && z.MO_Seq == y.MO_Seq).FirstOrDefault().Download_count
                                });
            if (!string.IsNullOrEmpty(filterParam.Supplier_ID))
            {
                lists = lists.Where(x => x.Supplier_ID == filterParam.Supplier_ID);
            }

            var result = await lists.Distinct().ToListAsync();
            return PagedList<MissingAgain_Dto>.Create(result.OrderByDescending(x => x.Updated_Time).ToList(), param.PageNumber, param.PageSize, false);
        }

        public async Task<string> FindMissingByQrCode(string qrCodeID)
        {
            var trans = await _repoTransactionMain
                            .FindAll(x => x.QRCode_ID.Trim() == qrCodeID.Trim() &&
                                     (!String.IsNullOrEmpty(x.Missing_No)))
                    .OrderByDescending(x => x.QRCode_Version).FirstOrDefaultAsync();
            return trans.Missing_No;
        }


        public async Task<bool> CheckQrCodeInV696(string qrCodeID)
        {
            var qrCodeMain = await _repoQRCodeMain.FindAll(x => x.Is_Scanned == "N" && x.QRCode_ID.Trim() == qrCodeID.Trim())
                .FirstOrDefaultAsync();
            if (qrCodeMain == null)
            {
                return false;
            }
            else
            {
                var packingModel = await _repoPackingList.FindAll(x => x.Receive_No.Trim() == qrCodeMain.Receive_No.Trim())
                    .FirstOrDefaultAsync();
                return packingModel == null ? false : ( packingModel.Subcon_ID.Trim() == "V696" ? true : false);
            }
        }
        public async Task<bool> CheckRackLocation(string rackLocation)
        {
            var rackMain = await _repoRacklocationRepo.FindAll(x => x.Rack_Location.Trim() == rackLocation.Trim())
                .FirstOrDefaultAsync();
            return rackMain == null ? false : (rackMain.Area_ID.Trim() == "A012" ? true : false);
        }

        // ---Phần in QrCodeId sau khi Input (Sorting Form) ----------------------------//
        public async Task<WMSB_Transaction_Main> FindQrCodeInput(string qrCodeId)
        {
            var trans = await _repoTransactionMain.FindAll(x => x.QRCode_ID.Trim() == qrCodeId.Trim())
                .OrderByDescending(x => x.QRCode_Version).FirstOrDefaultAsync();
            return trans;
        }
        public async Task<PagedList<IntegrationInputModel>> SearchIntegrationInput(PaginationParams param, FilterPackingListParam filterparam)
        {
            var pred_Packing_Lists = PredicateBuilder.New<WMSB_Packing_List>(true);
            pred_Packing_Lists.And(x => x.Generated_QRCode.Trim() == "Y");
            if (!String.IsNullOrEmpty(filterparam.MO_No))
            {
                pred_Packing_Lists.And(x => x.MO_No.Trim() == filterparam.MO_No.Trim());
            }
            if (filterparam.Supplier_ID != null && filterparam.Supplier_ID != "All")
            {
                pred_Packing_Lists.And(x => x.Supplier_ID.Trim() == filterparam.Supplier_ID.Trim());
            }
            if (filterparam.From_Date != null && filterparam.To_Date != null)
            {
                pred_Packing_Lists.And(x => x.Receive_Date >= Convert.ToDateTime(filterparam.From_Date + " 00:00:00.000") &&
                    x.Receive_Date <= Convert.ToDateTime(filterparam.To_Date + " 23:59:59.997"));
            }
            var packingLists = await _repoPackingList.FindAll(pred_Packing_Lists).ToListAsync();
            var conn = await _database.CreateConnectionAsync();
            var qrCodeMains = conn.Query<WMSB_QRCode_Main>("Select * from WMSB_QRCode_Main where Is_Scanned = 'N'").ToList();
            var qrCodeMainDetail = conn.Query<WMSB_QRCode_Detail>("Select * from WMSB_QRCode_Detail").ToList();
            var data = (from a in packingLists
                        join b in qrCodeMains
                        on a.Receive_No.Trim() equals b.Receive_No.Trim()
                        join c in qrCodeMainDetail on b.QRCode_ID.Trim() equals c.QRCode_ID.Trim()
                        select new IntegrationInputModel()
                        {
                            Receive_No = a.Receive_No,
                            QRCode_Version = b.QRCode_Version,
                            QRCode_ID = b.QRCode_ID,
                            MO_No = a.MO_No,
                            Purchase_No = a.Purchase_No,
                            MO_Seq = a.MO_Seq,
                            Material_ID = a.Material_ID,
                            Material_Name = a.Material_Name,
                            Supplier_ID = a.Supplier_ID,
                            Supplier_Name = a.Supplier_Name,
                            Receive_Qty = c.Qty
                        }).GroupBy(x => x.QRCode_ID).Select(z => new IntegrationInputModel()
                        {
                            Receive_No = z.FirstOrDefault().Receive_No,
                            QRCode_Version = z.FirstOrDefault().QRCode_Version,
                            Purchase_No = z.FirstOrDefault().Purchase_No,
                            QRCode_ID = z.FirstOrDefault().QRCode_ID,
                            MO_No = z.FirstOrDefault().MO_No,
                            MO_Seq = z.FirstOrDefault().MO_Seq,
                            Material_ID = z.FirstOrDefault().Material_ID,
                            Material_Name = z.FirstOrDefault().Material_Name,
                            Supplier_ID = z.FirstOrDefault().Supplier_ID,
                            Supplier_Name = z.FirstOrDefault().Supplier_Name,
                            Receive_Qty = z.Sum(k => k.Receive_Qty)
                        }).ToList();

            var receiveNos = data.Select(x => x.Receive_No.Trim()).ToList();
            var packingListDetails = conn
                                .Query<WMSB_PackingList_Detail>("Select * from WMSB_PackingList_Detail where Receive_No IN @receiveNos", new { receiveNos = receiveNos })
                                .ToList();
            foreach (var item in data)
            {
                item.PackingListDetailItem = packingListDetails.Where(x => x.Receive_No.Trim() == item.Receive_No.Trim()).ToList();
            }

            return PagedList<IntegrationInputModel>.Create(data, param.PageNumber, param.PageSize, false);
        }

        public async Task<bool> IntegrationInputSubmit(List<IntegrationInputModel> data, string user)
        {
            data = data.Where(x => !string.IsNullOrEmpty(x.Rack_Location)).ToList();
            var listTranSheetNoNew = new List<string>();
            var listTransacNoList = new List<string>();
            foreach (var item in data)
            {
                // Update Table WMSB_QRCode_Main
                var qrCodeModels = await _repoQRCodeMain.FindAll(x => x.QRCode_ID.Trim() == item.QRCode_ID.Trim() &&
                                                    x.Is_Scanned.Trim() == "N" &&
                                                    x.Receive_No.Trim() == item.Receive_No.Trim()).FirstOrDefaultAsync();
                if (qrCodeModels != null)
                {
                    qrCodeModels.Is_Scanned = "Y";
                }


                //---------------------------- Check TransactionMain-------------------------------//
                var transationMainCheck = await _repoTransactionMain.FindAll(x => x.QRCode_ID.Trim() == item.QRCode_ID.Trim() &&
                                                x.MO_No.Trim() == item.MO_No.Trim() &&
                                                x.Purchase_No.Trim() == item.Purchase_No.Trim() &&
                                                x.Material_ID.Trim() == item.Material_ID.Trim()).FirstOrDefaultAsync();
                if (transationMainCheck == null)
                {
                    string Transac_Sheet_No, Transac_No;
                    do
                    {
                        string num = CodeUtility.RandomNumber(3);
                        Transac_Sheet_No = "IB" + DateTime.Now.ToString("yyyyMMdd") + num;
                        
                    } while (await _repoTransactionMain.CheckTranSheetNo(Transac_Sheet_No) || (listTranSheetNoNew.Contains(Transac_Sheet_No)));
                    listTranSheetNoNew.Add(Transac_Sheet_No);
                    do
                    {
                        string num1 = CodeUtility.RandomNumber(3);
                        Transac_No = "IB" + item.MO_No.Trim() + num1;
                        
                    } while (await _repoTransactionMain.CheckTransacNo(Transac_No) || (listTransacNoList.Contains(Transac_No)));
                    listTransacNoList.Add(Transac_No);
                    // Tạo New Model để Add vào Table WMSB_Transaction_Main
                    var transactionMainModel = new WMSB_Transaction_Main();
                    transactionMainModel.Transac_Type = "I";
                    transactionMainModel.Transac_No = Transac_No;
                    transactionMainModel.Transac_Sheet_No = Transac_Sheet_No;
                    transactionMainModel.Can_Move = "Y";
                    transactionMainModel.Transac_Time = DateTime.Now;
                    transactionMainModel.QRCode_ID = item.QRCode_ID;
                    transactionMainModel.QRCode_Version = item.QRCode_Version;
                    transactionMainModel.MO_No = item.MO_No.Trim();
                    transactionMainModel.Purchase_No = item.Purchase_No.Trim();
                    transactionMainModel.MO_Seq = item.MO_Seq;
                    transactionMainModel.Material_ID = item.Material_ID.Trim();
                    transactionMainModel.Material_Name = item.Material_Name.Trim();
                    transactionMainModel.Purchase_Qty = item.Receive_Qty;
                    transactionMainModel.Transacted_Qty = item.Receive_Qty;
                    transactionMainModel.Rack_Location = item.Rack_Location;
                    transactionMainModel.Is_Transfer_Form = "N";
                    transactionMainModel.Updated_Time = DateTime.Now;
                    transactionMainModel.Updated_By = user;
                    _repoTransactionMain.Add(transactionMainModel);

                    // Add vào table WMSB_Transaction_Detail             
                    foreach (var item1 in item.PackingListDetailItem)
                    {
                        var transactionDetailModel = new WMSB_Transaction_Detail();
                        transactionDetailModel.Transac_No = Transac_No.Trim();
                        transactionDetailModel.Tool_Size = item1.Tool_Size;
                        transactionDetailModel.Order_Size = item1.Order_Size;
                        transactionDetailModel.Model_Size = item1.Model_Size;
                        transactionDetailModel.Spec_Size = item1.Spec_Size;
                        transactionDetailModel.Qty = item1.Received_Qty;
                        transactionDetailModel.Trans_Qty = item1.Received_Qty;
                        transactionDetailModel.Instock_Qty = item1.Received_Qty;
                        transactionDetailModel.Untransac_Qty = 0;
                        transactionDetailModel.Updated_Time = DateTime.Now;
                        transactionDetailModel.Updated_By = user;
                        _repoTransactionDetail.Add(transactionDetailModel);
                    }
                }
            }
            return await _repoTransactionDetail.SaveAll();
        }

        public async Task<string> CheckEnterRackInputIntergration(string racklocation, string receiveNo)
        {
            var packingListModel = await _repoPackingList.FindAll(x => x.Receive_No.Trim() == receiveNo.Trim()).FirstOrDefaultAsync();
            var supplier_ID = packingListModel.Supplier_ID.Trim();
            var rackModel = await _repoRacklocationRepo.FindAll(x => x.Rack_Location.Trim() == racklocation.Trim()).FirstOrDefaultAsync();
            var areaId = rackModel.Area_ID.Trim();
            if (supplier_ID == "V696") {
                return areaId != "A012" ? "input-rack-A012" : "ok";
            }
            else {
                return areaId == "A012" ? "input-rack-not-A012" : "ok";
            }
        }

        public async Task<List<NSP_MISSING_REPORT_DETAIL>> ExportExcelMissingReportDetail(FilterMissingParam filterParam)
        {
            // Lấy data từ Store
            var data = await (_context.NSP_MISSING_REPORT_DETAIL.FromSqlRaw("EXEC [dbo].[NSP_MISSING_REPORT_DETAIL] @Date_S, @Date_E, @MO_No, @Material_ID, @T2_Supplier_ID",
            new SqlParameter("Date_S", filterParam.FromTime != "" ? filterParam.FromTime : (object)DBNull.Value),
            new SqlParameter("Date_E", filterParam.ToTime != "" ? filterParam.ToTime : (object)DBNull.Value),
            new SqlParameter("MO_No", filterParam.MO_No != "" ? filterParam.MO_No : (object)DBNull.Value),
            new SqlParameter("Material_ID", filterParam.Material_ID != "" ? filterParam.Material_ID : (object)DBNull.Value),
            new SqlParameter("T2_Supplier_ID", filterParam.Supplier_ID != "" && filterParam.Supplier_ID != "All" ? filterParam.Supplier_ID : (object)DBNull.Value)
            )).ToListAsync();
            
            var listMissingNo = filterParam.ListMissingNo.Select(x => x.missingNo).ToList();
            data = data.Where(x => listMissingNo.Contains(x.Missing_No)).OrderByDescending(x => x.Plan_Start_STF).ThenBy(x => x.Material_ID).ToList();
            foreach (var missingbyBatchItem in filterParam.ListMissingNo)
            {
                var materialMissing = await _repoMaterialMissing.FindAll(x => x.Missing_No == missingbyBatchItem.missingNo && x.MO_Seq == missingbyBatchItem.batch).ToListAsync();
                foreach (var item in materialMissing)
                {
                    item.Download_count =item.Download_count==null?1:item.Download_count+1; 
                }
            }
            await  _repoMaterialMissing.SaveAll();

            
            List<NSP_MISSING_REPORT_DETAIL> result = new List<NSP_MISSING_REPORT_DETAIL>();
            var listDataGroup = data.GroupBy(x => x.Material_ID);
            foreach (var item in listDataGroup)
            {
                NSP_MISSING_REPORT_DETAIL sumTotal = new NSP_MISSING_REPORT_DETAIL();
                sumTotal.Transac_Time = null;
                sumTotal.Plan_Start_STF = null;
                sumTotal.PO_Batch = "";
                sumTotal.Model_Name = "";
                sumTotal.Model_No = "";
                sumTotal.Article = "";
                sumTotal.Part_Name = "SubTotal";
                sumTotal.Material_ID = item.Key.Trim();
                sumTotal.Material_Name = "";
                sumTotal.Unit = "";
                sumTotal.Missing_Qty = item.Sum(x => x.Missing_Qty);
                sumTotal.Missing_Reason = "";
                sumTotal.T2_Supplier = "";
                sumTotal.T3_Supplier = "";
                sumTotal.Receive_Date = null;
                sumTotal.Purchase_No = "";
                var toolSizeMissing = data.Where(x => x.Material_ID.Trim() == item.Key.Trim()).Select(t =>
                {
                    t.Tool_Size_Missing = t.Tool_Size + "(" + t.Missing_Qty.ToString().Substring(0, t.Missing_Qty.ToString().IndexOf(".00")) + ")";
                    return t;
                }).ToList();

                var toolSize = item.GroupBy(x => x.Tool_Size);
                foreach (var size in toolSize)
                {
                    sumTotal.Tool_Size_Missing += size.Key + "(" + size.Sum(x => x.Missing_Qty).ToString().Substring(0, size.Sum(x => x.Missing_Qty).ToString().IndexOf(".00")) + "), ";
                }

                // Gộp những item có cùng PO_Batch trong cùng Material_ID
                foreach (var itemResult in toolSizeMissing.GroupBy(x => x.PO_Batch))
                {
                    var itemMissing = toolSizeMissing.Where(x => x.PO_Batch.Trim() == itemResult.Key.Trim()).FirstOrDefault();
                    NSP_MISSING_REPORT_DETAIL dataResult = new NSP_MISSING_REPORT_DETAIL();
                    dataResult.Accumulated = itemMissing.Accumulated;
                    dataResult.Article = itemMissing.Article;
                    dataResult.Material_ID = itemMissing.Material_ID;
                    dataResult.Material_Name = itemMissing.Material_Name;
                    dataResult.Missing_No = itemMissing.Missing_No;
                    dataResult.Missing_Qty = toolSizeMissing.Where(x => x.PO_Batch.Trim() == itemResult.Key.Trim()).Sum(x => x.Missing_Qty);
                    dataResult.Missing_Reason = itemMissing.Missing_Reason;
                    dataResult.Model_Name = itemMissing.Model_Name.Trim();
                    dataResult.Model_No = itemMissing.Model_No;
                    dataResult.Part_Name = itemMissing.Part_Name;
                    dataResult.Plan_Qty = itemMissing.Plan_Qty;
                    dataResult.Plan_Start_STF = itemMissing.Plan_Start_STF;
                    dataResult.PO_Batch = itemResult.Key;
                    dataResult.Purchase_No = itemMissing.Purchase_No;
                    dataResult.Receive_Date = itemMissing.Receive_Date;
                    dataResult.Transac_Time = itemMissing.Transac_Time;
                    dataResult.T2_Supplier = itemMissing.T2_Supplier;
                    dataResult.T3_Supplier = itemMissing.T3_Supplier;
                    dataResult.Unit = itemMissing.Unit;
                    dataResult.Accumlated_In_Qty = toolSizeMissing.Where(x => x.PO_Batch.Trim() == itemResult.Key.Trim()).Sum(x => x.Accumlated_In_Qty);
                    foreach (var size2 in toolSizeMissing.Where(x => x.PO_Batch.Trim() == itemResult.Key.Trim()).GroupBy(x => x.Tool_Size))
                    {
                        dataResult.Tool_Size_Missing += size2.Key + "(" + size2.Sum(x => x.Missing_Qty).ToString().Substring(0, size2.Sum(x => x.Missing_Qty).ToString().IndexOf(".00")) + "), ";
                    }
                    // Xóa ký tự " ,_ " cuối dòng
                    dataResult.Tool_Size_Missing = dataResult.Tool_Size_Missing.Substring(0, dataResult.Tool_Size_Missing.LastIndexOf(", "));
                    result.Add(dataResult);
                }
                // Xóa ký tự " ,_ " cuối dòng 
                sumTotal.Tool_Size_Missing = sumTotal.Tool_Size_Missing.Substring(0, sumTotal.Tool_Size_Missing.LastIndexOf(", "));
                sumTotal.Plan_Qty = result.Where(x => x.Part_Name != "Subtotal" && x.Material_ID == item.Key).ToList().Sum(x => x.Plan_Qty);
                sumTotal.Accumulated = result.Where(x => x.Part_Name != "Subtotal" && x.Material_ID == item.Key).ToList().Sum(x => x.Accumulated);
                sumTotal.Accumlated_In_Qty = result.Where(x => x.Part_Name != "Subtotal" && x.Material_ID == item.Key).ToList().Sum(x => x.Accumlated_In_Qty);

                result.Add(sumTotal);
            }

            return result;
        }
    }
}