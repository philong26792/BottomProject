import { Component, OnInit } from '@angular/core';
import { InputService } from '../../../_core/_services/input.service';
import { AlertifyService } from '../../../_core/_services/alertify.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-input-print',
  templateUrl: './input-print.component.html',
  styleUrls: ['./input-print.component.scss']
})
export class InputPrintComponent implements OnInit {
  listInputMain: any = [];
  inputDetailItem: any = [];
  transInModel: any = [];
  rackLocation = "";
  checkSubmitMain: boolean;
  listAccumatedQty: any = [];
  totalAccumatedQty: number;
  totalTransInQty: number;
  totalInStocQty: number;
  constructor(
    private inputService: InputService,
    private alertify: AlertifyService,
    private router: Router) { }

  ngOnInit() {
      this.inputService.currentInputDetail.subscribe(inputDetailItem => this.inputDetailItem = inputDetailItem);
      this.inputDetailItem.detail_Size.forEach(e => {
          this.transInModel.push(e.qty)
      });
      this.inputService.currentCheckSubmit.subscribe(res => this.checkSubmitMain = res);
      this.inputService.currentListInputMain.subscribe(listInputMain => this.listInputMain = listInputMain);
      this.inputService.currentListAccumatedQty.subscribe(res => this.listAccumatedQty = res);
  }

  changeInput(e, i) {
    if (e > this.listAccumatedQty[i]) {
      let ele = document.getElementById("id-" + i) as HTMLInputElement;
      ele.value = this.listAccumatedQty[i];
      this.transInModel[i] = this.listAccumatedQty[i];
    }
    else if (e < 0) {
      let ele = document.getElementById("id-" + i) as HTMLInputElement;
      ele.value = '0';
      this.transInModel[i] = 0;
    }
  }
  cancel() {
    this.router.navigate(["/input/main"]);
  }
  saveInput() {
    let params = this.inputDetailItem;
    params.trans_In_Qty = 0;
    params.detail_Size.forEach((element, index) => {
      element.qty = this.transInModel[index];
      params.trans_In_Qty += element.qty;
    });
    params.inStock_Qty = params.trans_In_Qty;

    this.listInputMain.forEach((e, i) => {
      if (e.qrCode_Id === params.qrCode_Id)
        this.listInputMain[i] = params;
      });
      this.alertify.success("Save succeed");
      this.inputService.changeListInputMain(this.listInputMain);
      this.router.navigate(["/input/main"]);
  }

  ngAfterContentChecked() {
    if(this.listAccumatedQty.length > 0) {
      this.totalAccumatedQty = this.listAccumatedQty.reduce((a,c) => {return a + c});
    }
    if(this.transInModel.length > 0) {
      this.totalTransInQty = this.transInModel.reduce((a,c) => {return a + c});
      this.totalInStocQty = this.totalTransInQty;
    }
  }
}
