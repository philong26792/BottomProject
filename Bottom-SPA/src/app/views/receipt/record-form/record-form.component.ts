import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MaterialService } from '../../../_core/_services/material.service';
import { MaterialModel } from '../../../_core/_viewmodels/material-model';
import { MaterialMergingViewModel } from '../../../_core/_viewmodels/material-merging';
import { AlertifyService } from '../../../_core/_services/alertify.service';
import { ReceiveNoMain } from '../../../_core/_viewmodels/receive_no_main';
import { OrderSizeByBatch } from '../../../_core/_viewmodels/order-size-by-batch';
import { NgxSpinnerService } from 'ngx-spinner';
import { FunctionUtility } from '../../../_core/_utility/function-utility';
declare var $: any;
@Component({
  selector: 'app-record-form',
  templateUrl: './record-form.component.html',
  styleUrls: ['./record-form.component.scss']
})
export class RecordFormComponent implements OnInit {
  fromDate = new Date();
  isDisabled = true;
  delivery_No: string;
  type: string = 'No Batch';
  materialModel: MaterialModel;
  orderSizeByBatch: OrderSizeByBatch[] = [];
  orderSizeByBatchConst: OrderSizeByBatch[];
  materialMerging: MaterialMergingViewModel[];
  receiveNoMain: ReceiveNoMain[] = [];
  // Mảng order_size khi input ko đc nhập gì cả.
  listOrderSizeInputChange: any[] = [];
  totalByBatch: any[] = [];
  checkSubmit = false;
  listTotalQty: any;
  constructor(private router: Router,
              private alertifyService: AlertifyService,
              private materialService: MaterialService,
              private spinnerService: NgxSpinnerService,
              private functionUtility: FunctionUtility) { }

  ngOnInit() {
    this.materialService.currentMaterial.subscribe(res => this.materialModel = res);
    this.getDataLoadTable();
    if (this.materialModel === undefined) {
      this.router.navigate(['/receipt/main']);
    }
  }
  changeForm() {
    if (this.type === 'Batches') {
      this.router.navigate(['/receipt/record/add-batches']);
    }
  }
  getDataLoadTable() {
    this.materialService.searchByPurchase(this.materialModel).subscribe(res => {
      this.orderSizeByBatch = res.list3;
      // Để tránh orderSizeByBatchConst thay đổi theo orderSizeByBatch
      this.orderSizeByBatchConst = JSON.parse(JSON.stringify(res.list3));
      console.log(this.orderSizeByBatchConst);
      this.materialMerging = res.list4;
      this.listTotalQty = res.listTotalQty;
      this.totalByBatch = res.totalByBatch;
      console.log(this.listTotalQty);
      this.materialMerging.forEach(item => {
          this.listOrderSizeInputChange.push(item.order_Size.toString());
      });
    })
  }
  changeInput(e) {
    console.log(this.orderSizeByBatch);
    // console.log(this.materialMerging);
    let indexOf = this.listOrderSizeInputChange.indexOf(e.toString());
    if (indexOf!== -1) {
      this.listOrderSizeInputChange.splice(indexOf,1);
    }

    //---------- Phải load lại các giá trị của Order_Size đó lại ban đầu lúc gettable.---------
    this.orderSizeByBatchConst.forEach(element => {
      element.purchase_Qty.forEach(element1 => {
        if (element1.order_Size.toString() === e.toString()) {
          this.orderSizeByBatch.forEach(element2 => {
            if(element2.mO_Seq.toString() === element.mO_Seq.toString()) {
              element2.purchase_Qty.forEach(element3 => {
                if (element3.order_Size.toString() === e.toString()) {
                  element3.purchase_Qty = element1.purchase_Qty;
                  element3.accumlated_In_Qty = element1.accumlated_In_Qty;
                  element3.received_Qty = 0;
                  // console.log(element1.purchase_Qty);
                }
              })
            }
          });
        }
      });
    });
    // ------------------------------------------------------------------------------------------
    let columnInput = 0;
    // Order_Size tuong ung
    let thisInput = e;
    for (let i = 0; i < this.materialMerging.length; i++) {
      if (e.toString() === this.materialMerging[i].order_Size.toString()) {
        columnInput = i;
        break;
      }
    }
    // Mảng giá trị Purchase_Qty tương ứng với Order_Size đó. 
    let listInput = [];
    this.orderSizeByBatch.forEach(element => {
      listInput.push(element.purchase_Qty[columnInput]);
    });
    // Giá trị lấy được khi nhập input.
    let valueInput = (<HTMLInputElement>document.getElementById(thisInput.toString())).value;
    let materialMergingItem = this.materialMerging[columnInput];
    if (parseFloat(valueInput) > materialMergingItem.delivery_Qty) {
      (<HTMLInputElement>document.getElementById(thisInput.toString())).value = materialMergingItem.delivery_Qty.toString();
      valueInput = materialMergingItem.delivery_Qty.toString();
    }
    let n;
    for (let x = 0; x < listInput.length; x++) {
      if (x === 0) {
        // Khi accumlated_In_Qty ở dòng đầu tiên  = 0 có nghĩa là chưa nhận lô hàng nào.
        if (parseFloat(listInput[x].accumlated_In_Qty) === 0) {
          if (parseFloat(valueInput) <= parseFloat(listInput[x].purchase_Qty)) {
            listInput[0].purchase_Qty = parseFloat(valueInput);
            listInput[0].accumlated_In_Qty = parseFloat(valueInput);
            listInput[0].received_Qty = parseFloat(valueInput);
            for (let y = 1; y < listInput.length; y++) {
              listInput[y].purchase_Qty = 0;
            }
            break;
          } else {
            n = parseFloat(valueInput) - parseFloat(listInput[0].purchase_Qty);
            listInput[0].accumlated_In_Qty = listInput[0].purchase_Qty;
            listInput[0].received_Qty = listInput[0].purchase_Qty;
          }
        // Khi lô hàng đã nhận đủ.
      } else if (parseFloat(listInput[x].purchase_Qty) === 0) {
        n = parseFloat(valueInput);
      } 
      // Khi lô hàng nhận chưa đủ.
      else if (parseFloat(listInput[x].purchase_Qty) > 0) {
        // Lượng nhận vào + lượng đã nhận nhỏ thua hoặc bằng lượng cần nhận để đủ.
        if (parseFloat(valueInput) <= parseFloat(listInput[x].purchase_Qty)) {
          // Cần nhận từng này nữa mới đủ số lượng cần nhận vào.
          // listInput[0].purchase_Qty = parseFloat(listInput[x].purchase_Qty) + parseFloat(valueInput) ;
          listInput[0].purchase_Qty = parseFloat(valueInput);
          listInput[0].received_Qty = parseFloat(valueInput);
          listInput[0].accumlated_In_Qty = parseFloat(valueInput) + parseFloat(listInput[x].accumlated_In_Qty);
          for (let t = x + 1; t < listInput.length; t++) {
            listInput[t].purchase_Qty = 0;
          }
          break;
        } 
        // Lượng nhận vào + lượng đã nhận lớn hơn lượng cần nhận để đủ.
        else {
          // Lượng nhận còn lại sau khi nhập hàng đủ ở trên.
          n = parseFloat(valueInput) - parseFloat(listInput[0].purchase_Qty);
          listInput[0].accumlated_In_Qty = listInput[0].accumlated_In_Qty + listInput[0].purchase_Qty;
          listInput[0].received_Qty = parseFloat(listInput[0].purchase_Qty);
        }
      } else if(listInput[x].accumlated_In_Qty === null) {
        n = parseFloat(valueInput);
      }
    } else {
      if (parseFloat(listInput[x].accumlated_In_Qty) === 0) {
        if (n <= parseFloat(listInput[x].purchase_Qty)) {
            listInput[x].purchase_Qty = n;
            listInput[x].accumlated_In_Qty = n;
            listInput[x].received_Qty = n;
            for (let y = x + 1; y < listInput.length; y++) {
              listInput[y].purchase_Qty = 0;
            }
            break;
        } else {
          n = n - parseFloat(listInput[x].purchase_Qty);
          listInput[x].accumlated_In_Qty = listInput[x].purchase_Qty;
          listInput[x].received_Qty = listInput[x].purchase_Qty;
        }
      }
      // Khi lô hàng đã nhận đủ.
        else if ( parseFloat(listInput[x].purchase_Qty) === 0){
      }
      // Khi lô hàng nhận chưa đủ.
      else if(parseFloat(listInput[x].purchase_Qty) > 0) {
         // Lượng nhận vào + lượng đã nhận nhỏ thua hoặc bằng lượng cần nhận để đủ.
        if (n <= parseFloat(listInput[x].purchase_Qty)) {
          listInput[x].purchase_Qty = n;
          listInput[x].received_Qty = n;
          listInput[x].accumlated_In_Qty = n + parseFloat(listInput[x].accumlated_In_Qty);
          for (let z = x + 1; z < listInput.length; z++) {
            listInput[z].purchase_Qty = 0;
          }
          break;
        } 
         // Lượng nhận vào + lượng đã nhận lớn hơn lượng cần nhận để đủ.
        else {
           // Lượng nhận còn lại sau khi nhập hàng đủ ở trên.
          n = n - parseFloat(listInput[x].purchase_Qty);
          listInput[x].accumlated_In_Qty = listInput[x].purchase_Qty;
          listInput[x].received_Qty = parseFloat(listInput[x].purchase_Qty);
        }
      }
    };
  }
}
  async submitTable() {
    let checkInputDelivery = await this.materialService.checkInputDelivery(this.materialModel.supplier_ID).toPromise();
    if(checkInputDelivery) {
      if(this.functionUtility.checkEmpty(this.delivery_No)) {
        return this.alertifyService.error('Please enter “Delivery No” !!');
      }
    }
        this.spinnerService.show();
        this.checkSubmit = true;
          this.orderSizeByBatch.map(item => {
            item.delivery_No = this.delivery_No;
            return item;
          });
          this.listOrderSizeInputChange.forEach(element => {
            this.orderSizeByBatch.forEach(item => {
              item.purchase_Qty.forEach(item1 => {
                if (item1.order_Size.toString() === element.toString()) {
                  item1.received_Qty = item1.purchase_Qty;
                  if (item1.accumlated_In_Qty === 0) {
                    item1.accumlated_In_Qty = item1.purchase_Qty;            
                  } else {
                    item1.accumlated_In_Qty = item1.accumlated_In_Qty + item1.purchase_Qty;
                  }
                }
              });
            });
          });
          this.materialService.updateMaterial(this.orderSizeByBatch).subscribe(res => {
              this.spinnerService.hide();
              this.router.navigate(['receipt/record']);
            this.alertifyService.success('Submit success');
          }, error => {
            this.alertifyService.error(error);
          });

  }
  cancel() {
    this.router.navigate(['/receipt/record']);
  }
  backForm() {
    this.router.navigate(['/receipt/record/']);
  }
  ngAfterContentChecked() {
      this.orderSizeByBatch.forEach(obj1 => {
        let totalPurchaseQty = obj1.purchase_Qty.map(o => o.purchase_Qty).reduce((a,c) => {return a + c});
        this.totalByBatch.forEach(obj2 => {
          if(obj2.mO_Seq === obj1.mO_Seq) {
            obj2.total = totalPurchaseQty;
          }
        });
      });

      // ----------------------------------------------------------------------------------------------------- //
      let totalDeliveryQty = 0;
      debugger
      if(this.orderSizeByBatch !== undefined) {
      this.orderSizeByBatch.forEach(item => {
        let totalItem = item.purchase_Qty.map(o => o.purchase_Qty).reduce((a,c) => {return a + c});
        totalDeliveryQty = totalDeliveryQty + totalItem;
      });
      if(this.listTotalQty !== undefined) {
        this.listTotalQty.totalDeliveryQty = totalDeliveryQty;
      }
    }
  }
}
