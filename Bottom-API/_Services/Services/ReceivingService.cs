using AutoMapper;
using Bottom_API._Services.Interfaces;
using Bottom_API.DTO;
using System.Threading.Tasks;
using Bottom_API.Helpers;
using Bottom_API._Repositories.Interfaces;
using AutoMapper.QueryableExtensions;
using System.Linq;
using System.Collections.Generic;
using System;
using Microsoft.EntityFrameworkCore;
using Bottom_API.Models;
using LinqKit;
using Bottom_API.Data;
using Dapper;
using Bottom_API.DTO.Receiving;

namespace Bottom_API._Services.Services
{
    public class ReceivingService : IReceivingService
    {
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _configMapper;
        private readonly IMaterialPurchaseRepository _repoPurchase;
        private readonly IMaterialMissingRepository _repoMissing;
        private readonly IMaterialViewRepository _repoMaterialView;
        private readonly IPackingListRepository _repoPackingList;
        private readonly IPackingListDetailRepository _repoPackingListDetail;
        private readonly IMaterialOffsetRepository _repoMaterialOffset;
        private readonly ISettingT2SupplierRepository _repoSettingT2Supplier;
        private readonly DataContext _context;
        private readonly IDatabaseConnectionFactory _database;
        public ReceivingService(IMaterialPurchaseRepository repoPurchase,
                                IMaterialMissingRepository repoMissing,
                                IMaterialViewRepository repoMaterialView,
                                IPackingListRepository repoPackingList,
                                IPackingListDetailRepository repoPackingListDetail,
                                IMaterialOffsetRepository repoMaterialOffset,
                                ISettingT2SupplierRepository repoSettingT2Supplier,
                                IMapper mapper,
                                MapperConfiguration configMapper,
                                DataContext context,
                                IDatabaseConnectionFactory database)
        {
            _repoMissing = repoMissing;
            _repoPurchase = repoPurchase;
            _repoMaterialView = repoMaterialView;
            _repoPackingList = repoPackingList;
            _repoPackingListDetail = repoPackingListDetail;
            _repoMaterialOffset = repoMaterialOffset;
            _repoSettingT2Supplier = repoSettingT2Supplier;
            _configMapper = configMapper;
            _mapper = mapper;
            _context = context;
            _database = database;
        }

        public class PurchaseConvert
        {
            public string Purchase_No { get; set; }
            public string Status { get; set; }
            public string Missing_No { get; set; }
        }
        public class TotalQty {
            public decimal? TotalPurchaseQty {get;set;}
            public decimal? TotalAccumated_Qty {get;set;}
            public decimal? TotalDeliveryQty {get;set;}
        }
        public class TotalQtyOfBatch {
            public string MO_Seq {get;set;}
            public decimal? Total {get;set;}
        }
        public async Task<object> MaterialMerging(MaterialMainViewModel model)
        {
            var listMaterial = new List<Material_Dto>();
            if (model.Missing_No != "")
            {
                listMaterial = await _repoMissing.FindAll(x => x.Purchase_No.Trim() == model.Purchase_No.Trim() &&
                                        x.Missing_No.Trim() == model.Missing_No.Trim() &&
                                        x.Material_ID.Trim() == model.Material_ID.Trim() &&
                                        x.MO_No.Trim() == model.MO_No.Trim() &&
                                        x.Is_Missing != "N")
                .ProjectTo<Material_Dto>(_configMapper)
                .ToListAsync();
            }
            else
            {
                listMaterial = await _repoPurchase.FindAll(x => x.Purchase_No.Trim() == model.Purchase_No.Trim() &&
                        x.Material_ID.Trim() == model.Material_ID.Trim() &&
                        x.MO_No.Trim() == model.MO_No.Trim() &&
                        x.Biz_Tflag.Trim() != "D" &&
                        (x.MO_Qty != 0 || x.Purchase_Qty != 0)
                        )
                        
                .ProjectTo<Material_Dto>(_configMapper)
                .ToListAsync();
            }
            listMaterial.OrderByDescending(x => x.MO_Seq);
            var OrderSizeGroup = listMaterial.GroupBy(x => x.Order_Size).Select(x => x.Key);
            OrderSizeGroup = OrderSizeGroup.OrderBy(x => decimal.Parse(x)).ToList();
            var listBatch = listMaterial.GroupBy(x => x.MO_Seq).Select(y => new
            {
                MO_Seq = y.First().MO_Seq
            }).OrderBy(x => x.MO_Seq).ToList();
            var list3 = new List<OrderSizeByBatch>();
            foreach (var item in listBatch)
            {
                var item1 = new OrderSizeByBatch();
                item1.MO_Seq = item.MO_Seq;
                item1.Purchase_No = model.Purchase_No;
                item1.Missing_No = model.Missing_No;
                item1.Material_ID = model.Material_ID;
                item1.Material_Name = model.Material_Name;
                item1.Model_No = model.Model_No;
                item1.Model_Name = model.Model_Name;
                item1.MO_No = model.MO_No;
                item1.Article = model.Article;
                item1.Supplier_ID = model.Supplier_ID;
                item1.Supplier_Name = model.Supplier_Name;
                item1.Subcon_No = model.Subcon_No;
                item1.Subcon_Name = model.Subcon_Name;
                item1.T3_Supplier = model.T3_Supplier;
                item1.T3_Supplier_Name = model.T3_Supplier_Name;

                // ---- Start Check xem Batch này đã insert đủ hết chưa.-----
                item1.CheckInsert = "1";
                foreach (var item4 in listMaterial)
                {
                    if (item4.MO_Seq == item.MO_Seq)
                    {
                        if (item4.Accumlated_In_Qty != item4.Purchase_Qty)
                        {
                            item1.CheckInsert = "0";
                            break;
                        }
                    }
                }
                // -----------------------------End------------------------

                var item3 = new List<OrderSizeAccumlate>();
                    foreach (var orderSize in OrderSizeGroup)
                    {
                        var materialByBathOrderSize = listMaterial
                        .Where(x => x.Order_Size.Trim() == orderSize.Trim() &&
                                x.MO_Seq == item.MO_Seq);
                        if(materialByBathOrderSize.Count() > 0) {
                            foreach (var item2 in materialByBathOrderSize)
                            {
                                    var item4 = new OrderSizeAccumlate();
                                    item4.Order_Size = item2.Order_Size;
                                    item4.Model_Size = item2.Model_Size;
                                    item4.Tool_Size = item2.Tool_Size;
                                    item4.Spec_Size = item2.Spec_Size;
                                    item4.Purchase_Qty_Const = item2.Purchase_Qty;
                                    item4.MO_Qty = item2.MO_Qty;
                                    item4.Purchase_Qty = item2.Purchase_Qty - item2.Accumlated_In_Qty;
                                    item4.Accumlated_In_Qty = item2.Accumlated_In_Qty;
                                    item4.Received_Qty = 0;
                                    item3.Add(item4);
                                item1.Purchase_Qty = item3;
                            }
                        } else {
                            var item4 = new OrderSizeAccumlate();
                            item4.Order_Size = orderSize;
                            item3.Add(item4);
                        }
                    }
                    list3.Add(item1);
            }


            // -------------------------------------------------------------------------------------------//
            var list1 = listMaterial.GroupBy(l => l.Order_Size)
                .Select(cl => new
                {
                    Tool_Size = cl.First().Tool_Size,
                    Order_Size = cl.First().Order_Size,
                    Accumlated_In_Qty = cl.Sum(c => c.Accumlated_In_Qty),
                    Delivery_Qty_Batches = cl.Sum(x => x.Accumlated_In_Qty),
                    Purchase_Qty = cl.Sum(c => c.Purchase_Qty),
                    Delivery_Qty = cl.Sum(c => c.Purchase_Qty) - cl.Sum(c => c.Accumlated_In_Qty)
                }).ToList();
            var list2 = listMaterial.GroupBy(x => new { x.Order_Size, x.Purchase_Qty, x.MO_Seq })
            .Select(y => new
            {
                Order_Size = y.First().Order_Size,
                Purchase_Qty = y.First().Purchase_Qty,
                MO_Seq = y.First().MO_Seq,
            });
            var list4 = new List<MaterialMergingViewMode>();
            foreach (var orderSize in OrderSizeGroup)
            {
                var list5 = list1.Where(x => x.Order_Size.Trim() == orderSize.Trim());
                foreach (var item in list5)
                {
                    var arrayItem = new MaterialMergingViewMode();
                    arrayItem.Tool_Size = item.Tool_Size;
                    arrayItem.Order_Size = item.Order_Size;
                    arrayItem.Purchase_Qty = item.Purchase_Qty;
                    arrayItem.Accumlated_In_Qty = item.Accumlated_In_Qty;
                    arrayItem.Delivery_Qty = item.Delivery_Qty;
                    arrayItem.Delivery_Qty_Batches = item.Delivery_Qty_Batches;
                    var array1 = new List<BatchQtyItem>();
                    foreach (var item1 in list2)
                    {
                        if (item1.Order_Size.Trim() == item.Order_Size.Trim())
                        {
                            var item2 = new BatchQtyItem();
                            item2.MO_Seq = item1.MO_Seq;
                            item2.Purchase_Qty = item1.Purchase_Qty;
                            array1.Add(item2);
                            arrayItem.Purchase_Qty_Item = array1;
                        }
                    }
                    list4.Add(arrayItem);
                }
            }
            
            var totalByBatch = new List<TotalQtyOfBatch>();
            foreach (var item in list3)
            {
                var totalQty = item.Purchase_Qty.Sum(x => x.Purchase_Qty);
                var itemTotalQty = new TotalQtyOfBatch();
                itemTotalQty.Total = totalQty;
                itemTotalQty.MO_Seq = item.MO_Seq;
                totalByBatch.Add(itemTotalQty);
            }
            
            var listTotalQty = new TotalQty();
            listTotalQty.TotalPurchaseQty = list4.Sum(x => x.Purchase_Qty);
            listTotalQty.TotalAccumated_Qty = list4.Sum(x => x.Accumlated_In_Qty);
            listTotalQty.TotalDeliveryQty = list4.Sum(x => x.Delivery_Qty);
            // --------------------------------------End----------------------------------------//
            var result = new
            {
                list3,
                list4,
                totalByBatch,
                listTotalQty

            };
            return result;
        }
        public async Task<PagedList<MaterialMainViewModel>> SearchByModel(PaginationParams param, FilterMaterialParam filterParam)
        { 

            var pred_Material_Purchase = PredicateBuilder.New<WMSB_Material_Purchase>(true);
            var pred_Material_Missing = PredicateBuilder.New<WMSB_Material_Missing>(true);
            var pred_VM_Material_Purchase = PredicateBuilder.New<VM_WMSB_Material_Purchase>(true);
            
            // Ở kho có Type = 1
            pred_Material_Purchase.And(x => x.Type.Trim() == "1" && x.Biz_Tflag != "D");
            if (filterParam.From_Date != null && filterParam.To_Date != null)
            {
                pred_Material_Purchase.And(x => x.Confirm_Delivery >= Convert.ToDateTime(filterParam.From_Date + " 00:00:00.000"));
                pred_Material_Purchase.And(x => x.Confirm_Delivery <= Convert.ToDateTime(filterParam.To_Date + " 23:59:59.997"));                
                pred_Material_Missing.And(x => x.Confirm_Delivery >= Convert.ToDateTime(filterParam.From_Date + " 00:00:00.000"));
                pred_Material_Missing.And(x => x.Confirm_Delivery <= Convert.ToDateTime(filterParam.To_Date + " 23:59:59.997"));
            }
            
            if (!String.IsNullOrEmpty(filterParam.MO_No))
            {
                pred_Material_Purchase.And(x => x.MO_No.Trim() == filterParam.MO_No.Trim());                
                pred_Material_Missing.And(x => x.MO_No.Trim() == filterParam.MO_No.Trim());                
                pred_VM_Material_Purchase.And(x => x.Plan_No.Trim() == filterParam.MO_No.Trim());
            }
            if (filterParam.Supplier_ID != null && filterParam.Supplier_ID != "All")
            {
                pred_Material_Purchase.And(x => x.Supplier_ID.Trim() == filterParam.Supplier_ID.Trim());
                pred_Material_Missing.And(x => x.Supplier_ID.Trim() == filterParam.Supplier_ID.Trim());
                pred_VM_Material_Purchase.And(x => x.Supplier_No.Trim() == filterParam.Supplier_ID.Trim());
            }
            if (filterParam.Status != "all")
            {
                pred_Material_Purchase.And(x => x.Status.Trim() == filterParam.Status.Trim());                
                pred_Material_Missing.And(x => x.Status.Trim() == filterParam.Status.Trim());
            }
            pred_Material_Missing.And(x => x.Is_Missing != "N");
            var materialPurchaseList = await _repoPurchase.FindAll(pred_Material_Purchase)
                .Select(x => new PurchaseConvert()
                {
                    Purchase_No = x.Purchase_No,
                    Status = x.Status,
                    Missing_No = "",
                }).Distinct().ToListAsync();
            var materialMissingList = await _repoMissing.FindAll(pred_Material_Missing)
                        .Select(x => new PurchaseConvert()
                        {
                            Purchase_No = x.Purchase_No,
                            Status = x.Status,
                            Missing_No = x.Missing_No,
                        }).Distinct().ToListAsync();
            foreach (var item in materialMissingList)
            {
                materialPurchaseList.Add(item);
            }
            // Nếu purchase đó có 1 batch là N thì status show ra sẽ là N. Còn Y hết thì hiển thị Y
            foreach (var item in materialPurchaseList)
            {
                if (item.Status.Trim() == "N")
                {
                    foreach (var item1 in materialPurchaseList)
                    {
                        if (item1.Purchase_No.Trim() == item.Purchase_No.Trim() && item1.Missing_No == item.Missing_No)
                            item1.Status = "N";
                    }
                }
            }
            // Distinct lại mảng.Do ko xài Distinct ở trong câu lệnh 1 list Object được.
            var listData = materialPurchaseList.GroupBy(x => new { x.Purchase_No, x.Missing_No }).Select(y => y.First());
            // ------------------------------------------------------------------------------------------

            var listData_Purchase_No = listData.Select(x => x.Purchase_No.Trim()).Distinct().ToList();
            pred_VM_Material_Purchase.And(x => listData_Purchase_No.Contains(x.Purchase_No));
            // var conn = await _database.CreateConnectionAsync();
            // var parameter = new {listData_Purchase_No = listData_Purchase_No};
            // var listMaterialView = conn.Query
            // ("Select Distinct Mat# as Mat_,Mat#_Name as Mat__Name,Plan_No,Model_No,Model_No,Model_Name,Article,Custmoer_Name, Purchase_No, Supplier_No,Supplier_Name,Subcon_No,Subcon_Name,T3_Supplier,T3_Supplier_Name from VM_WMSB_Material_Purchase where Purchase_No IN @listData_Purchase_No",parameter, commandTimeout: 1000).ToList();
            var listMaterialView = _repoMaterialView.FindAll(pred_VM_Material_Purchase)
                .Select(x => new {
                x.Mat_, x.Mat__Name,x.Plan_No,x.Model_No, x.Model_Name, x.Article, x.Custmoer_Name,x.Purchase_No,
                x.Supplier_No, x.Supplier_Name, x.Subcon_No, x.Subcon_Name, x.T3_Supplier, x.T3_Supplier_Name,
            }).Distinct();
            var listMaterial =  (from a in listData
                                join b in listMaterialView
            on a.Purchase_No.Trim() equals b.Purchase_No.Trim()
                                select new MaterialMainViewModel
                                {
                                    Status = a.Status,
                                    Material_ID = b.Mat_,
                                    Material_Name = b.Mat__Name,
                                    Missing_No = a.Missing_No == null ? "" : a.Missing_No,
                                    MO_No = b.Plan_No,
                                    Purchase_No = a.Purchase_No,
                                    Model_No = b.Model_No,
                                    Model_Name = b.Model_Name,
                                    Article = b.Article,
                                    Custmoer_Name = b.Custmoer_Name,
                                    Supplier_ID = b.Supplier_No,
                                    Supplier_Name = b.Supplier_Name,
                                    Subcon_No = b.Subcon_No,
                                    Subcon_Name = b.Subcon_Name,
                                    T3_Supplier = b.T3_Supplier,
                                    T3_Supplier_Name = b.T3_Supplier_Name
                                }).ToList();
            listMaterial = listMaterial.OrderBy(x => x.Status).ThenBy(x => x.MO_No).ThenBy(x => x.Purchase_No).ToList();
            return PagedList<MaterialMainViewModel>.Create(listMaterial, param.PageNumber, param.PageSize, false);
        }
        

        public async Task<List<ReceiveNoMain>> UpdateMaterial(List<OrderSizeByBatch> data, string updateBy)
        {
            foreach (var item in data)
            {
                item.Purchase_Qty = item.Purchase_Qty.Where(x => x.Tool_Size != null).ToList();
            }
            var Purchase_No = data[0].Purchase_No.Trim();
            var MO_No = data[0].MO_No.Trim();
            // ------------------------------------------------------------------------------------------------------//
            if (data[0].Missing_No == "")
            {
                // --------Update lại Accumlated_In_Qty theo Purchase_No,Order_Size và Mo_Seq ở bảng Material_Purchase----//
                var purchaseMaterialAll = await _repoPurchase.FindAll(x => x.Biz_Tflag.Trim() != "D" && 
                                                                    x.Material_ID.Trim() == data[0].Material_ID.Trim() &&
                                                                                (x.MO_Qty != 0 || x.Purchase_Qty != 0) &&
                                                                x.Purchase_No.Trim() == Purchase_No.Trim()).ToListAsync();
                foreach (var item in data)
                {
                    foreach (var item1 in item.Purchase_Qty)
                    {
                        var materialItem = purchaseMaterialAll
                            .Where(x => x.MO_No.Trim() == MO_No &&
                                    x.Order_Size == item1.Order_Size &&
                                    x.MO_Seq == item.MO_Seq).FirstOrDefault();
                        // materialItem.Accumlated_In_Qty = item1.Accumlated_In_Qty;
                        // Số lượng bằng số lượng nhận hiện tại + số lượng vừa mới nhận vào.
                        materialItem.Accumlated_In_Qty = materialItem.Accumlated_In_Qty + item1.Received_Qty;
                    }
                }

                //------------------------- Update giá trị Status--------------------------------------------//
                foreach (var item in data)
                {
                    await this.UpdateStatusMaterialSubmit(item.Purchase_No, item.Material_ID, item.MO_No, item.MO_Seq, item.Missing_No);
                }
            }
            else
            {
                // Update lại Accumlated_In_Qty theo Purchase_No,Order_Size và Mo_Seq ở bảng Material_Missing
                var MaterialMissingAll = await _repoMissing.FindAll(x => x.Purchase_No.Trim() == Purchase_No.Trim() &&
                                                                    x.Material_ID.Trim() == data[0].Material_ID.Trim() &&
                                                                    x.Is_Missing != "N").ToListAsync();
                foreach (var item in data)
                {
                    foreach (var item1 in item.Purchase_Qty)
                    {
                        var materialItem = MaterialMissingAll
                            .Where(x => x.MO_No.Trim() == MO_No &&
                                    x.Order_Size == item1.Order_Size &&
                                    x.MO_Seq == item.MO_Seq).FirstOrDefault();
                        materialItem.Accumlated_In_Qty = item1.Accumlated_In_Qty;
                    }
                }

                // Update lại Status
                foreach (var item in data)
                {
                    await this.UpdateStatusMaterialSubmit(item.Purchase_No, item.Material_ID, item.MO_No ,item.MO_Seq, item.Missing_No);
                }
            }

            //------------------------Thêm vào 2 bảng Packing_List và Packing_List_Detail------------------//
            var ReceiveNoMain = new List<ReceiveNoMain>();
            var receiveNoList = new List<string>();
            foreach (var item in data)
            {
                // Check xem có tiến hành thêm hay ko
                var checkAdd = false;
                foreach (var item1 in item.Purchase_Qty)
                {
                    // Kiểm tra nếu tồn tại Received_Qty lớn hơn 0
                    //,có nghĩa là tồn tại 1 Order_Size trong batch đó có nhận hàng
                    if (item1.Received_Qty > 0)
                    {
                        checkAdd = true;
                        break;
                    }
                }
                // Tiến hành thêm vào bảng Packing_List và Packing_List_Detail
                if (checkAdd == true)
                {
                    var packing_List = new Packing_List_Dto();
                    if (item.Missing_No == string.Empty)
                    {
                        packing_List.Sheet_Type = "R";
                    }
                    else
                    {
                        packing_List.Sheet_Type = "M";
                    }
                    packing_List.Missing_No = item.Missing_No;
                    packing_List.Supplier_ID = item.Supplier_ID;
                    packing_List.Supplier_Name = item.Supplier_Name;
                    packing_List.MO_No = item.MO_No;
                    packing_List.Purchase_No = item.Purchase_No;
                    packing_List.MO_Seq = item.MO_Seq;
                    packing_List.Delivery_No = item.Delivery_No;
                    packing_List.Material_ID = item.Material_ID;
                    packing_List.Material_Name = item.Material_Name;
                    packing_List.Model_No = item.Model_No;
                    packing_List.Model_Name = item.Model_Name;
                    packing_List.Article = item.Article;
                    packing_List.Subcon_ID = item.Subcon_No;
                    packing_List.Subcon_Name = item.Subcon_Name;
                    packing_List.T3_Supplier = item.T3_Supplier;
                    packing_List.T3_Supplier_Name = item.T3_Supplier_Name;
                    packing_List.Generated_QRCode = "N";
                    packing_List.Receive_Date = DateTime.Now;
                    packing_List.Updated_By = updateBy;
                    var receiveNo = "";
                    do
                    {
                        receiveNo =  CodeUtility.RandomReceiveNo("RW", 2);
                    } while (await CheckReceiveNo(receiveNo) || receiveNoList.Contains(receiveNo));
                    receiveNoList.Add(receiveNo);
                    packing_List.Receive_No = receiveNo;

                    var packing_ListAdd = _mapper.Map<WMSB_Packing_List>(packing_List);
                    _repoPackingList.Add(packing_ListAdd);

                    foreach (var item2 in item.Purchase_Qty)
                    {
                        var packing_List_detail = new Packing_List_Detail_Dto();
                        packing_List_detail.Receive_No = packing_List.Receive_No;
                        packing_List_detail.Order_Size = item2.Order_Size;
                        packing_List_detail.Model_Size = item2.Model_Size;
                        packing_List_detail.Tool_Size = item2.Tool_Size;
                        packing_List_detail.Spec_Size = item2.Spec_Size;
                        packing_List_detail.MO_Qty = item2.MO_Qty;
                        packing_List_detail.Purchase_Qty = item2.Purchase_Qty_Const;
                        packing_List_detail.Received_Qty = item2.Received_Qty;
                        packing_List_detail.Updated_Time = DateTime.Now;
                        packing_List_detail.Updated_By = updateBy;
                        var packingDetailAdd = _mapper.Map<WMSB_PackingList_Detail>(packing_List_detail);
                        _repoPackingListDetail.Add(packingDetailAdd);
                    }
                }
            }
            await _repoPackingList.SaveAll();
            return ReceiveNoMain;
        }

        // ------------------Hàm lấy chi tiết của 1 Receive_No.------------------------------------------------------------
        public async Task<List<ReceiveNoDetail>> ReceiveNoDetails(string receive_No)
        {

            var packingListDetail = await _repoPackingListDetail.FindAll(x => x.Receive_No.Trim() == receive_No.Trim()).ToListAsync();
            var listData = packingListDetail.Select(x => new ReceiveNoDetail()
            {
                Tool_Size = x.Tool_Size,
                Order_Size = x.Order_Size,
                Purchase_Qty = x.Purchase_Qty,
                Received_Qty = x.Received_Qty,
            }).OrderBy(x => decimal.Parse(x.Order_Size)).ToList();
            return listData;
        }
        public async Task<List<ReceiveNoMain>> ReceiveNoMain(MaterialMainViewModel model)
        {
            var packingList =  _repoPackingList.FindAll(x => x.MO_No.Trim() == model.MO_No.Trim());
            var packingListDetail =  _repoPackingListDetail.FindAll();
            var materialList = new List<Material_Dto>();
            if (model.Missing_No == "")
            {
                materialList = await _repoPurchase.FindAll( x => x.Purchase_No.Trim() == model.Purchase_No.Trim() &&
                                x.Material_ID.Trim() == model.Material_ID.Trim() &&
                                x.MO_No.Trim() == model.MO_No.Trim() &&
                                x.Biz_Tflag.Trim() != "D" &&
                                (x.MO_Qty != 0 || x.Purchase_Qty != 0))
                .ProjectTo<Material_Dto>(_configMapper)
                        .ToListAsync();
            }
            else
            {
                materialList = await _repoMissing.FindAll(x => x.Is_Missing != "N").ProjectTo<Material_Dto>(_configMapper)
                        .Where( x => x.Purchase_No.Trim() == model.Purchase_No.Trim() && 
                                x.Material_ID.Trim() == model.Material_ID.Trim() &&
                                x.MO_No.Trim() == model.MO_No.Trim()).ToListAsync();
            }
            var dataModel = await (from a in packingList
                        join b in packingListDetail on a.Receive_No.Trim() equals b.Receive_No.Trim()
                        where a.Purchase_No.Trim() == model.Purchase_No.Trim()
                        select new
                        {
                            Status = model.Status,
                            Purchase_No = model.Purchase_No,
                            Delivery_No = a.Delivery_No,
                            Missing_No = model.Missing_No,
                            MO_No = model.MO_No,
                            Receive_No = a.Receive_No,
                            MO_Seq = a.MO_Seq,
                            Material_ID = a.Material_ID,
                            Material_Name = a.Material_Name,
                            Receive_Date = a.Receive_Date,
                            Purchase_Qty = b.Purchase_Qty,
                            Received_Qty = b.Received_Qty,
                            Generated_QRCode = a.Generated_QRCode,
                            Sheet_Type = a.Sheet_Type,
                            Updated_By = a.Updated_By
                        }).ToListAsync();
            var data = dataModel.GroupBy(x => x.Receive_No).Select(cl => new ReceiveNoMain()
                        {
                            MO_No = cl.FirstOrDefault().MO_No,
                            Missing_No = cl.FirstOrDefault().Missing_No,
                            Purchase_No = cl.FirstOrDefault().Purchase_No,
                            Delivery_No = cl.FirstOrDefault().Delivery_No,
                            Receive_No = cl.FirstOrDefault().Receive_No,
                            MO_Seq = cl.FirstOrDefault().MO_Seq,
                            Material_ID = cl.FirstOrDefault().Material_ID,
                            Material_Name = cl.FirstOrDefault().Material_Name,
                            Receive_Date = cl.FirstOrDefault().Receive_Date,
                            Purchase_Qty = cl.Sum(c => c.Purchase_Qty),
                            Accumated_Qty = cl.Sum(c => c.Received_Qty),
                            Generated_QRCode = cl.FirstOrDefault().Generated_QRCode,
                            Sheet_Type = cl.FirstOrDefault().Sheet_Type,
                            Updated_By = cl.FirstOrDefault().Updated_By
                        }).OrderByDescending(x => x.Receive_Date).ToList();
            foreach (var item1 in data)
            {
                var materialByPurchaseBath = materialList.Where(x => x.MO_Seq == item1.MO_Seq);
                var list1 = materialByPurchaseBath.GroupBy(x => x.Purchase_No).Select(cl => new
                {
                    Accumated_Qty = cl.Sum(x => x.Accumlated_In_Qty)
                }).ToList();
                item1.Accumated_Qty_All = list1[0].Accumated_Qty;
            }
            data = data.OrderByDescending(x => x.Receive_Date).ThenBy(x => x.MO_Seq).ToList();
            return data;
        }

        public async Task<bool> ClosePurchase(MaterialMainViewModel model)
        {
            if (model.Missing_No == "")
            {
                var purchaseList = await _repoPurchase.FindAll(x => x.Purchase_No.Trim() == model.Purchase_No.Trim() &&
                            x.MO_No.Trim() == model.MO_No.Trim() &&
                            x.Material_ID.Trim() == model.Material_ID.Trim() &&
                            x.Biz_Tflag.Trim() != "D" &&
                            (x.MO_Qty != 0 || x.Purchase_Qty != 0)).ToListAsync();
                foreach (var item in purchaseList)
                {
                    item.Status = "Y";
                }
                return await _repoPurchase.SaveAll();
            }
            else
            {
                var purchaseList = await _repoMissing.FindAll(x => x.Purchase_No.Trim() == model.Purchase_No.Trim() &&
                            x.Material_ID.Trim() == model.Material_ID.Trim() &&
                            x.MO_No.Trim() == model.MO_No.Trim() &&
                            x.Is_Missing != "N").ToListAsync();
                foreach (var item in purchaseList)
                {
                    item.Status = "Y";
                }
                return await _repoMissing.SaveAll();
            }
        }

        public async Task<string> StatusPurchase(MaterialMainViewModel model)
        {
            var packingList = _repoPackingList.FindAll(x => x.Purchase_No.Trim() == model.Purchase_No.Trim() &&
                                x.MO_No.Trim() == model.MO_No.Trim());
            var material = new List<Material_Dto>();
            var status = "ok";
            if (model.Missing_No == "")
            {
                material = await _repoPurchase.FindAll(x => x.Purchase_No.Trim() == model.Purchase_No.Trim() &&
                                x.Material_ID.Trim() == model.Material_ID.Trim() &&
                                x.MO_No.Trim() == model.MO_No.Trim() &&
                                x.Biz_Tflag.Trim() != "D" &&
                                (x.MO_Qty != 0 || x.Purchase_Qty != 0))
                    .ProjectTo<Material_Dto>(_configMapper).ToListAsync();
            }
            else
            {
                material = await _repoMissing.FindAll(x => x.Purchase_No.Trim() == model.Purchase_No.Trim() &&
                                x.Material_ID.Trim() == model.Material_ID.Trim() &&
                                x.MO_No.Trim() == model.MO_No.Trim() && x.Is_Missing != "N")
                    .ProjectTo<Material_Dto>(_configMapper).ToListAsync();
            }

            // Nếu purchase đó chưa có hàng nhận vào.
            if (packingList.Count() > 0)
            {
                foreach (var item in packingList)
                {
                    // Nếu trong bảng packingList có 1 receiveNo của purchase đó chưa đc sản sinh qrcode thì 
                    // chưa cho thêm hàng tiếp
                    if (item.Generated_QRCode.Trim() == "N")
                    {
                        status = "no";
                        break;
                    }
                }
            }
            else
            {
                status = "ok";
            }

            // Nếu tồn tại 1 Status trong bảng Purchase hoặc bảng Missing có status = N thì thêm đc.Còn Y là
            // đã đủ hàng rồi.Ko đc thêm tiếp.
            if (status == "ok")
            {
                var checkmaterial = "enough";
                foreach (var item in material)
                {
                    if (item.Status == "N")
                    {
                        checkmaterial = "not enough";
                        break;
                    }
                }
                if (checkmaterial == "enough")
                {
                    status = "no";
                }
            }
            return status;
        }

        public async Task<List<MaterialEditModel>> EditMaterial(ReceiveNoMain model)
        {
            var materialList = new List<Material_Dto>();
            if (model.Missing_No == "")
            {
                materialList = await _repoPurchase.FindAll(x => x.Purchase_No.Trim() == model.Purchase_No.Trim() &&
                        x.Material_ID.Trim() == model.Material_ID.Trim() &&
                        x.Biz_Tflag.Trim() != "D" &&
                        (x.MO_Qty != 0 || x.Purchase_Qty != 0) &&
                        x.MO_No.Trim() == model.MO_No.Trim() &&
                        x.MO_Seq.Trim() == model.MO_Seq.Trim())
                    .ProjectTo<Material_Dto>(_configMapper).ToListAsync();
            }
            else
            {
                materialList = await _repoMissing.FindAll(x => x.Purchase_No.Trim() == model.Purchase_No.Trim() &&
                        x.Material_ID.Trim() == model.Material_ID.Trim() &&
                        x.MO_No.Trim() == model.MO_No.Trim() &&
                        x.MO_Seq.Trim() == model.MO_Seq.Trim() && x.Is_Missing != "N")
                    .ProjectTo<Material_Dto>(_configMapper).ToListAsync();
            }
            var packingListDetail = await _repoPackingListDetail.FindAll(x => x.Receive_No.Trim() == model.Receive_No.Trim()).ToListAsync();
            var dataList = materialList.GroupBy(x => x.Order_Size).Select(y => new MaterialEditModel()
            {
                Purchase_No = y.FirstOrDefault().Purchase_No,
                Material_ID = model.Material_ID,
                MO_No = model.MO_No,
                Missing_No = model.Missing_No,
                Receive_No = model.Receive_No,
                Tool_Size = y.FirstOrDefault().Tool_Size,
                Order_Size = y.FirstOrDefault().Order_Size,
                Purchase_Qty = y.Sum(cl => cl.Purchase_Qty),
                Accumated_Qty = y.Sum(cl => cl.Accumlated_In_Qty),
                Delivery_Qty = y.Sum(cl => (cl.Purchase_Qty - cl.Accumlated_In_Qty)),
                Delivery_Qty_Const = y.Sum(cl => (cl.Purchase_Qty - cl.Accumlated_In_Qty)),
                Received_Qty = y.Sum(cl => cl.Purchase_Qty),
                Received_Qty_Edit = y.Sum(cl => (cl.Purchase_Qty - cl.Accumlated_In_Qty)),
                MO_Seq_Edit = model.MO_Seq,
            }).ToList();
            dataList = dataList.OrderBy(x => decimal.Parse(x.Order_Size)).ToList();
            return dataList;
        }

        public async Task<bool> EditDetail(List<MaterialEditModel> data)
        {

            // Update Receive_Date trong bảng PackingList.
            var packingListFind = await _repoPackingList.FindAll(x => x.Receive_No.Trim() == data[0].Receive_No.Trim()).FirstOrDefaultAsync();
            packingListFind.Receive_Date = DateTime.Now;
            packingListFind.Updated_Time = DateTime.Now;
            await _repoPackingList.SaveAll();

            var editResult = false;
            var receive_No = data[0].Receive_No;
            var missing_No = data[0].Missing_No;
            var packinglistDetail = _repoPackingListDetail.FindAll(x => x.Receive_No.Trim() == receive_No.Trim());
            foreach (var item in packinglistDetail)
            {
                foreach (var item1 in data)
                {
                    if (item1.Order_Size.Trim() == item.Order_Size.Trim())
                    {
                        item.Received_Qty = item.Received_Qty + item1.Received_Qty_Edit;
                    }
                }
            }
            var SavePackingListDetail = await _repoPackingListDetail.SaveAll();
            var saveMaterial = false;
            // Áp dụng cho bảng Material_Purchase
            if (missing_No == "")
            {
                var materialPurchaseList = _repoPurchase.FindAll(x => x.Purchase_No.Trim() == data[0].Purchase_No.Trim() &&
                            x.Biz_Tflag.Trim() != "D" &&
                            (x.MO_Qty != 0 || x.Purchase_Qty != 0) &&
                            x.MO_No.Trim() == data[0].MO_No.Trim() &&
                            x.MO_Seq.Trim() == data[0].MO_Seq_Edit.Trim());
                foreach (var item2 in data)
                {
                    foreach (var item4 in materialPurchaseList)
                    {
                        if (item4.Order_Size.Trim() == item2.Order_Size.Trim())
                        {
                            // Số lượng mới = số lượng hiện tại trừ đi số lượng đã nhận trước và + cho số lượng nhận mới.
                            item4.Accumlated_In_Qty = item4.Accumlated_In_Qty + item2.Received_Qty_Edit;
                        }
                    }
                }
                await _repoPurchase.SaveAll();
                //------------------------- Update giá trị Status--------------------------------------------//
                await this.UpdateStatusMaterial(data[0].Purchase_No, data[0].Material_ID, data[0].MO_No, data[0].MO_Seq_Edit, missing_No);
                saveMaterial = await _repoPurchase.SaveAll();
            }
            // Áp dụng cho bảng Material_Missing
            else
            {
                var materialPurchaseList = _repoMissing.FindAll(x => x.Purchase_No.Trim() == data[0].Purchase_No.Trim() &&
                                                                x.MO_No.Trim() == data[0].MO_No.Trim() &&
                                                                x.MO_Seq.Trim() == data[0].MO_Seq_Edit.Trim() &&
                                                                x.Is_Missing != "N");
                foreach (var item2 in data)
                {
                    foreach (var item4 in materialPurchaseList)
                    {
                        if (item4.Order_Size.Trim() == item2.Order_Size.Trim())
                        {
                            // Số lượng mới = số lượng hiện tại trừ đi số lượng đã nhận trước và + cho số lượng nhận mới.
                            item4.Accumlated_In_Qty = item4.Accumlated_In_Qty + item2.Received_Qty_Edit;
                        }
                    }
                }
                await _repoMissing.SaveAll();
                await this.UpdateStatusMaterial(data[0].Purchase_No, data[0].Material_ID, data[0].MO_No, data[0].MO_Seq_Edit, missing_No);
                saveMaterial = await _repoMissing.SaveAll();
            }
            if (SavePackingListDetail == true && saveMaterial == true)
            {
                editResult = true;
            }
            return editResult;
        }

        public async Task<bool> UpdateStatusMaterial(string purchaseNo, string materialId, string mo_No, string mOSeq, string missingNo)
        {
            if (missingNo == "")
            {
                var checkStatus = "Y";
                var purchaseForBatch = await _repoPurchase.FindAll(x => x.Purchase_No.Trim() == purchaseNo.Trim() &&
                                x.Material_ID.Trim() == materialId.Trim() &&
                                x.Biz_Tflag.Trim() != "D" &&
                                (x.MO_Qty != 0 || x.Purchase_Qty != 0) &&
                                x.MO_No.Trim() == mo_No.Trim() &&
                                x.MO_Seq == mOSeq).ToListAsync();

                var purchaseForBatchSum = purchaseForBatch.GroupBy(x => x.Purchase_No)
                    .Select(x => new {
                        Purchase_Qty = x.Sum(cl => cl.Purchase_Qty),
                        Accumlated_In_Qty = x.Sum(cl => cl.Accumlated_In_Qty),
                    }).FirstOrDefault();
                if(purchaseForBatchSum.Purchase_Qty != purchaseForBatchSum.Accumlated_In_Qty) {
                    checkStatus = "N";
                }
                // Nếu mà Accumlated_In_Qty đều bằng Purchase_Qty có nghĩa là batch đó đã nhận đủ hàng.
                // Cập nhập lại Status trong table của batch đó là Y.
                if (checkStatus == "Y")
                {
                    foreach (var item3 in purchaseForBatch)
                    {
                        item3.Status = "Y";
                    }
                }
                else
                {
                    foreach (var item3 in purchaseForBatch)
                    {
                        item3.Status = "N";
                    }
                }
                return true;
            }
            else
            {
                var checkStatus = "Y";
                var purchaseForBatch = await _repoMissing
                                    .FindAll(x => x.Purchase_No.Trim() == purchaseNo.Trim() 
                                            && x.Material_ID.Trim() == materialId.Trim()
                                            && x.MO_No.Trim() == mo_No.Trim() &&
                                            x.Is_Missing != "N" &&
                                            x.MO_Seq == mOSeq).ToListAsync();

                var purchaseForBatchSum = purchaseForBatch.GroupBy(x => x.Purchase_No)
                    .Select(x => new {
                        Purchase_Qty = x.Sum(cl => cl.Purchase_Qty),
                        Accumlated_In_Qty = x.Sum(cl => cl.Accumlated_In_Qty),
                    }).FirstOrDefault();
                if(purchaseForBatchSum.Purchase_Qty != purchaseForBatchSum.Accumlated_In_Qty) {
                    checkStatus = "N";
                }
                if (checkStatus == "Y")
                {
                    foreach (var item3 in purchaseForBatch)
                    {
                        item3.Status = "Y";
                    }
                }
                else
                {
                    foreach (var item3 in purchaseForBatch)
                    {
                        item3.Status = "N";
                    }
                }
                return true;
            }
        }

        public async Task<bool> UpdateStatusMaterialSubmit(string purchaseNo, string materialId, string mo_No, string mOSeq, string missingNo)
        {
            if (missingNo == "")
            {
                var checkStatus = "Y";
                var purchaseForBatch = await _repoPurchase.FindAll(x => x.Purchase_No.Trim() == purchaseNo.Trim() &&
                                x.Material_ID.Trim() == materialId.Trim() &&
                                x.Biz_Tflag.Trim() != "D" &&
                                (x.MO_Qty != 0 || x.Purchase_Qty != 0) &&
                                x.MO_No.Trim() == mo_No.Trim() &&
                                x.MO_Seq == mOSeq).ToListAsync();

                var purchaseForBatchSum = purchaseForBatch.GroupBy(x => x.Purchase_No)
                    .Select(x => new {
                        Purchase_Qty = x.Sum(cl => cl.Purchase_Qty),
                        Accumlated_In_Qty = x.Sum(cl => cl.Accumlated_In_Qty),
                    }).FirstOrDefault();
                if(purchaseForBatchSum.Purchase_Qty != purchaseForBatchSum.Accumlated_In_Qty) {
                    checkStatus = "N";
                }
                // Nếu mà Accumlated_In_Qty đều bằng Purchase_Qty có nghĩa là batch đó đã nhận đủ hàng.
                // Cập nhập lại Status trong table của batch đó là Y.
                if (checkStatus == "Y")
                {
                    foreach (var item3 in purchaseForBatch)
                    {
                        item3.Status = "Y";
                    }
                }
                return true;
            }
            else
            {
                var checkStatus = "Y";
                var purchaseForBatch = await _repoMissing
                            .FindAll(x => x.Purchase_No.Trim() == purchaseNo.Trim() &&
                                    x.Material_ID.Trim() == materialId.Trim() &&
                                    x.MO_No.Trim() == mo_No.Trim() &&
                                    x.MO_Seq == mOSeq &&
                                    x.Is_Missing != "N").ToListAsync();

                var purchaseForBatchSum = purchaseForBatch.GroupBy(x => x.Purchase_No)
                    .Select(x => new {
                        Purchase_Qty = x.Sum(cl => cl.Purchase_Qty),
                        Accumlated_In_Qty = x.Sum(cl => cl.Accumlated_In_Qty),
                    }).FirstOrDefault();
                if(purchaseForBatchSum.Purchase_Qty != purchaseForBatchSum.Accumlated_In_Qty) {
                    checkStatus = "N";
                }
                if (checkStatus == "Y")
                {
                    foreach (var item3 in purchaseForBatch)
                    {
                        item3.Status = "Y";
                    }
                }
                return true;
            }
        }

        public async Task<WMSB_Material_Offset> GetDMO_No(string moNo)
        {
            var data = await _repoMaterialOffset.FindAll(x => x.MO_No.Trim() == moNo.Trim()).FirstOrDefaultAsync();
            return data;
        }

        public async Task<bool> CheckReceiveNo(string receiveNo) {
            var data = await _repoPackingList.FindAll(x => x.Receive_No.Trim() == receiveNo.Trim()).FirstOrDefaultAsync();
            return data == null ? false : true;
        }

        public async Task<bool> CheckInputDelivery(string supplier_ID)
        {
            var data = await _repoSettingT2Supplier.FindAll(x => x.T2_Supplier_ID.Trim() == supplier_ID.Trim()).FirstOrDefaultAsync();
            return data == null ? false : data.Input_Delivery == "Y" ? true : false;
        }
    }
}