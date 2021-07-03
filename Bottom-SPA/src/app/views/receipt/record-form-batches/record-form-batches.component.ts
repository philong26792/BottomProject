import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MaterialService } from '../../../_core/_services/material.service';
import { MaterialModel } from '../../../_core/_viewmodels/material-model';
import { MaterialMergingViewModel } from '../../../_core/_viewmodels/material-merging';
import { AlertifyService } from '../../../_core/_services/alertify.service';
import { OrderSizeByBatch } from '../../../_core/_viewmodels/order-size-by-batch';
import { NgxSpinnerService } from 'ngx-spinner';
import { FunctionUtility } from '../../../_core/_utility/function-utility';
import { Location } from '@angular/common';
import { OrderSizeAccumlate } from '../../../_core/_viewmodels/ordersize-accumlate';

@Component({
  selector: 'app-record-form-batches',
  templateUrl: './record-form-batches.component.html',
  styleUrls: ['./record-form-batches.component.scss']
})
export class RecordFormBatchesComponent implements OnInit {
  type: string = 'Batches';
  materialModel: MaterialModel;
  orderSizeByBatch: OrderSizeByBatch[];
  materialByBatchList: OrderSizeByBatch[] = [];
  materialMerging: MaterialMergingViewModel[] = [];
  delivery_No: string;
  totalByBatch: any[] = [];
  listTotalQty: any;
  deliveryQtyTotal: number;
  checkSubmit: boolean = false;
  constructor(private router: Router,
              private materialService: MaterialService,
              private locationBack: Location,
              private alertifyService: AlertifyService,
              private functionUtility: FunctionUtility,
              private spinnerService: NgxSpinnerService) { }

  ngOnInit() {
    this.materialService.currentMaterial.subscribe(res => this.materialModel = res);
    this.getDataLoadTable();
    if(this.materialModel === undefined) {
      this.router.navigate(['/receipt/main']);
    }
  }
  getDataLoadTable() {
    this.materialService.searchByPurchase(this.materialModel).subscribe(res => {
      this.orderSizeByBatch = res.list3;
      this.orderSizeByBatch = this.orderSizeByBatch.map(x => {
        x.isEdit = false;
        return x;
      })
      this.materialMerging = res.list4;
      this.listTotalQty = res.listTotalQty;
      this.totalByBatch = res.totalByBatch;
    })
  }
  changeForm() {
    if(this.type === 'No Batch') {
      this.router.navigate(['/receipt/record/add']);
    }
  }
  changeInput(item: OrderSizeAccumlate, index2: number) {
    if(item.purchase_Qty > item.purchase_Qty_Const) {
      item.purchase_Qty = item.purchase_Qty_Const;
    }
    if(item.purchase_Qty < 0) {
      item.purchase_Qty = 0;
    }
    this.materialMerging[index2].delivery_Qty_Batches = this.orderSizeByBatch.filter(x => x.isEdit).map(x => (x.purchase_Qty)[index2].purchase_Qty).reduce((a,c) => {return a + c});
  }
async submitData() {
  let checkInputDelivery = await this.materialService.checkInputDelivery(this.materialModel.supplier_ID).toPromise();
    if(checkInputDelivery) {
      if(this.functionUtility.checkEmpty(this.delivery_No)) {
        return this.alertifyService.error('Please enter “Delivery No” !!');
      }
    }

    let orderSizeByBatchShow = this.orderSizeByBatch.filter(x => x.isEdit);
    if(orderSizeByBatchShow.length === 0) {
      return this.alertifyService.error('Please edit input!');
    }
    this.checkSubmit = true;
    let dataSubmit = orderSizeByBatchShow.map(item => {
      item.purchase_Qty = item.purchase_Qty.map(item1 => {
          item1.accumlated_In_Qty =  item1.purchase_Qty;
          item1.received_Qty = item1.purchase_Qty;
          return item1;
        });
        item.delivery_No = this.delivery_No;
        return item;
    });
    this.materialService.updateMaterial(dataSubmit).subscribe(res => {
      this.spinnerService.hide();
      this.router.navigate(['receipt/record']);
    }, error => {
      this.alertifyService.error(error);
    });
  }
  showInput(item: OrderSizeByBatch, index: number) {
    item.isEdit = true;
    let orderSizeByBatchShow = this.orderSizeByBatch.filter(x => x.isEdit);
    this.materialMerging.forEach((item, index, arr) => {
      item.delivery_Qty_Batches = orderSizeByBatchShow.map(x => (x.purchase_Qty)[index].purchase_Qty).reduce((a,c) => {return a + c});
    });
  }
  backForm() {
    this.locationBack.back();
  }
  ngAfterContentChecked() {
    if(this.materialMerging.length > 0) {
      this.deliveryQtyTotal = this.materialMerging.map(o => o.delivery_Qty_Batches).reduce((a,c) => {return a + c});
    }
  }
}
