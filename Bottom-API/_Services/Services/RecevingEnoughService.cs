using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Bottom_API._Repositories.Interfaces;
using Bottom_API._Services.Interfaces;
using Bottom_API.DTO.Receiving;
using Bottom_API.Helpers;
using Bottom_API.Models;
using LinqKit;
using Microsoft.EntityFrameworkCore;

namespace Bottom_API._Services.Services
{
    public class RecevingEnoughService : IRecevingEnoughService
    {
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _configMapper;
        private readonly IMaterialPurchaseRepository _repoPurchase;
        private readonly IMaterialViewRepository _repoMaterialView;
        private readonly IPackingListRepository _repoPackingList;
        private readonly IPackingListDetailRepository _repoPackingListDetail;
        public RecevingEnoughService(IMaterialPurchaseRepository repoPurchase,
                                IMaterialViewRepository repoMaterialView,
                                IPackingListRepository repoPackingList,
                                IPackingListDetailRepository repoPackingListDetail,
                                IMapper mapper,
                                MapperConfiguration configMapper) {
            _repoPurchase = repoPurchase;
            _repoMaterialView = repoMaterialView;
            _repoPackingList = repoPackingList;
            _repoPackingListDetail = repoPackingListDetail;
            _configMapper = configMapper;
            _mapper = mapper;

        }

        public class PurchaseConvert
        {
            public string Purchase_No { get; set; }
            public string MO_No { get; set; }
            public string Material_ID {get;set;}
            public string Status { get; set; }
        }
        public async Task<PagedList<ReceivingMaterialMainModel>> SearchByModel(PaginationParams param, FilterReceivingMateParam filterParam)
        {
            var pred_Material_Purchase = PredicateBuilder.New<WMSB_Material_Purchase>(true);
            var pred_VM_Material_Purchase = PredicateBuilder.New<VM_WMSB_Material_Purchase>(true);
            // Ở kho có Type = 2
            pred_Material_Purchase.And(x => x.Type.Trim() == "2" && x.Biz_Tflag != "D");
            if (filterParam.From_Date != null && filterParam.To_Date != null) {
                pred_Material_Purchase.And(x => x.Confirm_Delivery >= Convert.ToDateTime(filterParam.From_Date + " 00:00:00.000"));
                pred_Material_Purchase.And(x => x.Confirm_Delivery <= Convert.ToDateTime(filterParam.To_Date + " 23:59:59.997"));
            }
            if(!String.IsNullOrEmpty(filterParam.MO_No)) {
                pred_Material_Purchase.And(x => x.MO_No.Trim() == filterParam.MO_No.Trim());
                pred_VM_Material_Purchase.And(x => x.Plan_No.Trim() == filterParam.MO_No.Trim());
            }
            if (!String.IsNullOrEmpty(filterParam.Purchase_No)) {
                pred_Material_Purchase.And(x => x.Purchase_No.Trim() == filterParam.Purchase_No.Trim());
            }
            if (filterParam.Status != "all") {
                pred_Material_Purchase.And(x => x.Status.Trim() == filterParam.Status.Trim());
            }

            var materialPurchases = await _repoPurchase.FindAll(pred_Material_Purchase).ToListAsync();
            var materialPurchaseList = materialPurchases
                .Select(x => new PurchaseConvert() {
                    Purchase_No = x.Purchase_No.Trim(),Status = x.Status,MO_No = x.MO_No,Material_ID = x.Material_ID
                }).GroupBy(x => new {x.Purchase_No, x.Status, x.MO_No, x.Material_ID}).Select(x => x.FirstOrDefault()).ToList();

            var purchaseContainQty = materialPurchases.Select(x => new {
                Purchase_No = x.Purchase_No.Trim(),MO_No = x.MO_No.Trim(),Purchase_Qty = x.Purchase_Qty, Material_ID = x.Material_ID.Trim()
            }).GroupBy(x => new { x.Purchase_No, x.MO_No, x.Material_ID }).Select(x => new {
                Purchase_No = x.FirstOrDefault().Purchase_No,
                MO_No = x.FirstOrDefault().MO_No,
                Material_ID = x.FirstOrDefault().Material_ID,
                Qty = x.Sum(cl => cl.Purchase_Qty)
            }).ToList();

            // Distinct lại mảng.Do ko xài Distinct ở trong câu lệnh 1 list Object được.
            var listData = materialPurchaseList.GroupBy(x => new { x.Purchase_No, x.MO_No, x.Material_ID }).Select(y => y.First());
            // ---------------------------------------------------------------------------------------------------------

            var listData_Purchase_No = listData.Select(x => x.Purchase_No.Trim()).Distinct().ToList();

            pred_VM_Material_Purchase.And(x => listData_Purchase_No.Contains(x.Purchase_No));

            var listMaterialView = await _repoMaterialView.FindAll(pred_VM_Material_Purchase)
                .Select(x => new { x.Mat_,x.Mat__Name,x.Plan_No,x.Model_No,x.Model_Name,x.Article,x.Purchase_No,
                    x.Supplier_No,x.Supplier_Name,x.Subcon_No, x.Subcon_Name, x.T3_Supplier, x.T3_Supplier_Name
                }).Distinct().ToListAsync();
            listMaterialView = listMaterialView.Distinct().ToList();
            var result = (from a in listData
                                join c in purchaseContainQty
                                on new {Purchase_No = a.Purchase_No.Trim(), MO_No = a.MO_No.Trim(), MaterialID = a.Material_ID.Trim()}
                                equals new {Purchase_No = c.Purchase_No.Trim(), MO_No = c.MO_No.Trim(), MaterialID = c.Material_ID.Trim()}
                                join b in listMaterialView
                                on new {Purchase_No = a.Purchase_No.Trim(), MO_No = a.MO_No.Trim(), MaterialID = a.Material_ID.Trim()}
                                equals new {Purchase_No = b.Purchase_No, MO_No = b.Plan_No.Trim(),MaterialID=b.Mat_.Trim() }
                                select new ReceivingMaterialMainModel {
                                    Status = a.Status,
                                    Material_ID = b.Mat_,
                                    Material_Name = b.Mat__Name,
                                    MO_No = b.Plan_No,
                                    Purchase_No = a.Purchase_No,
                                    Model_No = b.Model_No,
                                    Model_Name = b.Model_Name,
                                    Article = b.Article,
                                    Supplier_ID = b.Supplier_No,
                                    Supplier_Name = b.Supplier_Name,
                                    Subcon_No = b.Subcon_No,
                                    Subcon_Name = b.Subcon_Name,
                                    T3_Supplier = b.T3_Supplier,
                                    T3_Supplier_Name = b.T3_Supplier_Name,
                                    Qty = c.Qty
                                }).ToList();
            result = result.OrderBy(x => x.Status).ThenBy(x => x.MO_No).ThenBy(x => x.Purchase_No).ToList();
            return PagedList<ReceivingMaterialMainModel>.Create(result, param.PageNumber, param.PageSize, false);
        }

        public async Task<List<ReceiveAfterSubmit>> SubmitData(List<ReceivingMaterialMainModel> data, string updateBy)
        {
            var receiveAfterSubmit = new List<ReceiveAfterSubmit>();
            foreach (var itemData in data)
            {
                // Update ở bảng WMSB_Material_Purchase
                var materialPurchaseByPurchase = await _repoPurchase.FindAll(x => x.Biz_Tflag.Trim() != "D" &&
                                (x.MO_Qty != 0 || x.Purchase_Qty != 0) &&
                                x.Purchase_No.Trim() == itemData.Purchase_No.Trim() &&
                                x.MO_No.Trim() == itemData.MO_No.Trim() &&
                                x.Material_ID.Trim() == itemData.Material_ID.Trim()).ToListAsync();
                                
                var sumPurchaseQtyOfBatch = materialPurchaseByPurchase.GroupBy(x => x.MO_Seq).Select(x => new {
                    MO_Seq = x.Key,
                    SumPurchaseQty = x.Sum(cl => cl.Purchase_Qty)
                });
                // Batch nào có tổng PurchaseQty > 0 thì mới xử lý
                sumPurchaseQtyOfBatch = sumPurchaseQtyOfBatch.Where(x => x.SumPurchaseQty > 0).ToList();
                var batchInSumPurchaseQtyMoreZero = sumPurchaseQtyOfBatch.Select(x => x.MO_Seq).ToList();

                materialPurchaseByPurchase = materialPurchaseByPurchase.Where(x => batchInSumPurchaseQtyMoreZero.Contains(x.MO_Seq)).ToList();

                materialPurchaseByPurchase.ForEach(item => {
                    item.Status = "Y";
                    item.Accumlated_In_Qty =  item.Purchase_Qty;
                });
                if(batchInSumPurchaseQtyMoreZero.Count > 0) {
                    foreach (var itemBatch in batchInSumPurchaseQtyMoreZero)
                    {
                        //Thêm ở bảng PackingList
                        var packingListModel = new WMSB_Packing_List();
                        packingListModel.Sheet_Type = "B";
                        packingListModel.Missing_No = "";
                        packingListModel.Delivery_No = "";
                        packingListModel.Supplier_ID = itemData.Supplier_ID;
                        packingListModel.Supplier_Name = itemData.Supplier_Name;
                        packingListModel.Receive_Date = DateTime.Now;
                        packingListModel.MO_No = itemData.MO_No;
                        packingListModel.Purchase_No = itemData.Purchase_No;
                        packingListModel.MO_Seq = itemBatch;
                        packingListModel.Receive_No = CodeUtility.RandomReceiveNo("RW", 2);
                        packingListModel.Material_ID = itemData.Material_ID;
                        packingListModel.Material_Name = itemData.Material_Name;
                        packingListModel.Article = itemData.Article;
                        packingListModel.Model_No = itemData.Model_No;
                        packingListModel.Model_Name = itemData.Model_Name;
                        packingListModel.Subcon_ID = itemData.Subcon_No;
                        packingListModel.Subcon_Name = itemData.Subcon_Name;
                        packingListModel.T3_Supplier = itemData.T3_Supplier;
                        packingListModel.T3_Supplier_Name = itemData.T3_Supplier_Name;
                        packingListModel.Generated_QRCode = "N";
                        packingListModel.Updated_Time = DateTime.Now;
                        packingListModel.Updated_By = updateBy;
                        _repoPackingList.Add(packingListModel);

                        // Tạo ra record để show ngoài màn hình sau khi submit
                        var receiveAfterSubmitItem = new ReceiveAfterSubmit();
                        var sumAccumlatedQty = materialPurchaseByPurchase
                            .Where(x => x.MO_Seq == itemBatch).Sum(x => x.Purchase_Qty);
                        receiveAfterSubmitItem.MO_No = packingListModel.MO_No;
                        receiveAfterSubmitItem.Purchase_No = packingListModel.Purchase_No;
                        receiveAfterSubmitItem.Receive_No = packingListModel.Receive_No;
                        receiveAfterSubmitItem.MO_Seq = itemBatch;
                        receiveAfterSubmitItem.Material_ID = packingListModel.Material_ID;
                        receiveAfterSubmitItem.Material_Name = packingListModel.Material_Name;
                        receiveAfterSubmitItem.Receive_Time = packingListModel.Receive_Date;
                        receiveAfterSubmitItem.Purchase_Qty = sumAccumlatedQty;
                        receiveAfterSubmitItem.Type = "R";
                        receiveAfterSubmitItem.Update_By = packingListModel.Updated_By;
                        receiveAfterSubmit.Add(receiveAfterSubmitItem);

                        // Thêm ở bảng PackingListDetail
                        var materialPurchaseForBatchs = materialPurchaseByPurchase.Where(x =>x.MO_Seq == itemBatch).ToList();
                        foreach (var materialForBatchItem in materialPurchaseForBatchs)
                        {
                            var packingListDetailModel = new WMSB_PackingList_Detail();
                            packingListDetailModel.Receive_No = packingListModel.Receive_No;
                            packingListDetailModel.Order_Size = materialForBatchItem.Order_Size;
                            packingListDetailModel.Model_Size = materialForBatchItem.Model_Size;
                            packingListDetailModel.Tool_Size = materialForBatchItem.Tool_Size;
                            packingListDetailModel.Spec_Size = materialForBatchItem.Spec_Size;
                            packingListDetailModel.MO_Qty = materialForBatchItem.MO_Qty;
                            packingListDetailModel.Purchase_Qty = materialForBatchItem.Purchase_Qty;
                            packingListDetailModel.Received_Qty = materialForBatchItem.Purchase_Qty;
                            packingListDetailModel.Updated_Time = DateTime.Now;
                            packingListDetailModel.Updated_By = updateBy;
                            _repoPackingListDetail.Add(packingListDetailModel);
                        }
                    }
                }
            }
            await _repoPackingListDetail.SaveAll();
            return receiveAfterSubmit;
        }

    }
}