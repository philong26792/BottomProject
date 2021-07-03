import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MaterialService } from '../../../_core/_services/material.service';
import { AlertifyService } from '../../../_core/_services/alertify.service';
import { MaterialEditModel } from '../../../_core/_viewmodels/material-edit-model';
import { ReceiveNoMain } from '../../../_core/_viewmodels/receive_no_main';
import { NgxSpinnerService } from 'ngx-spinner';

@Component({
  selector: 'app-record-edit',
  templateUrl: './record-edit.component.html',
  styleUrls: ['./record-edit.component.scss']
})
export class RecordEditComponent implements OnInit {
  delivery_No: string;
  materialEditModels: MaterialEditModel[] = [];
  materialEditModelsConst: MaterialEditModel[] = [];
  receiveNoMain: ReceiveNoMain;
  checkSubmit = false;
  totalPurchaseQty: number;
  totalAccumatedQty: number;
  totalDeleveryQty: number;
  totalQtyByBatch: number;
  constructor(private router: Router,
              private altertifyService: AlertifyService,
              private materialService: MaterialService,
              private spinnerService: NgxSpinnerService) { }

  ngOnInit() {
    this.materialService.currentReceiveNoMainItem.subscribe(res => this.receiveNoMain = res);
    this.getLoadTable();

  }
  submitEdit() {
    let checkOnEdit = 0;
    this.materialEditModelsConst.forEach(element1 => {
      this.materialEditModels.forEach(element2 => {
        if (element2.order_Size.trim() === element1.order_Size.trim()) {
          if (element1.purchase_Qty < (element2.received_Qty)) {
            checkOnEdit = checkOnEdit + 1;
            let errorMessage = 'Input of Order_Size ' + element1.order_Size.toString() + ' has errors';
            this.altertifyService.error(errorMessage);
          }
        }
      });
    });
    if (checkOnEdit < 1) {
      this.checkSubmit = true;
      this.spinnerService.show();
      this.materialService.editDetail(this.materialEditModels).subscribe(res => {
        this.spinnerService.hide();
        this.altertifyService.success('Edit data successed!');
        this.router.navigate(['receipt/record/']);
      }, error => {
        this.altertifyService.error(error);
      });
    }
  }
  getLoadTable() {
    if(this.receiveNoMain !== undefined) {
      this.delivery_No = this.receiveNoMain.delivery_No;
    }
    this.materialService.editMaterial(this.receiveNoMain).subscribe(res => {
        this.materialEditModels = res;
        // Mảng materialEditModelsConst sẽ không thay đổi
        this.materialEditModelsConst = JSON.parse(JSON.stringify(res));
        this.totalPurchaseQty = this.materialEditModelsConst.map(o => o.purchase_Qty)
        .reduce((a,c) => {return a + c});
        this.totalAccumatedQty = this.materialEditModelsConst.map(o => o.accumated_Qty)
          .reduce((a,c) => {return a + c});
        this.totalDeleveryQty = this.materialEditModelsConst.map(o => o.delivery_Qty)
          .reduce((a,c) => {return a + c});
          this.totalQtyByBatch = this.materialEditModelsConst.map(o => o.received_Qty)
          .reduce((a,c) => {return a + c});
        console.log(this.materialEditModels);
    });
  }
  cancel() {
    this.router.navigate(['receipt/record/']);
  }
  changeInput(e) {
    let columnInput = 0;
    let thisInput = e.toString();
    for(let i = 0; i < this.materialEditModels.length; i++) {
      if (thisInput === this.materialEditModels[i].order_Size.toString()) {
        columnInput = i;
      }
    }
    // Giá trị lấy được khi nhập input.
    let valueInput = (<HTMLInputElement>document.getElementById(thisInput.toString())).value;
    if(valueInput === '') {
      this.altertifyService.error('Please enter number');
    } else {
      this.materialEditModels.forEach(element => {
        if (element.order_Size.trim() === thisInput.trim()) {
          element.received_Qty_Edit = parseFloat(valueInput.toString());
          element.received_Qty = element.accumated_Qty + parseFloat(valueInput.toString());
          if (element.received_Qty > element.purchase_Qty) {
            element.received_Qty_Edit = element.delivery_Qty_Const;
            element.delivery_Qty = element.delivery_Qty_Const;
            (<HTMLInputElement>document.getElementById(thisInput.toString())).value = element.delivery_Qty_Const.toString();
          }
        }
      });
      
      this.totalAccumatedQty = this.materialEditModels.map(o => o.accumated_Qty)
      .reduce((a,c) => {return a + c});
      this.totalDeleveryQty = this.materialEditModels.map(o => o.delivery_Qty)
        .reduce((a,c) => {return a + c});
      this.totalQtyByBatch = this.materialEditModels.map(o => o.received_Qty)
        .reduce((a,c) => {return a + c});
      }
  }
  backForm() {
    this.router.navigate(['receipt/record']);
  }
}
