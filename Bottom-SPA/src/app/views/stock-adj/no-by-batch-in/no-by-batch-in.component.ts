import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NgxSpinnerService } from 'ngx-spinner';
import { ModifyQrCodeMain } from '../../../_core/_models/modify-qrcode-main';
import { SettingReason } from '../../../_core/_models/setting-reason';
import { SizeInStockPlanQty, SizeInstockQtyByBatch } from '../../../_core/_models/size-instockqty-bybath';
import { AlertifyService } from '../../../_core/_services/alertify.service';
import { ModifyQrcodeService } from '../../../_core/_services/modify-qrcode.service';

@Component({
  selector: 'app-no-by-batch-in',
  templateUrl: './no-by-batch-in.component.html',
  styleUrls: ['./no-by-batch-in.component.scss']
})
export class NoByBatchInComponent implements OnInit {
  modifyQrCodeMain: ModifyQrCodeMain;
  data1: SizeInStockPlanQty[] = [];
  data1Const: SizeInStockPlanQty[] = [];
  data2: SizeInstockQtyByBatch[] = [];
  data2Const: SizeInstockQtyByBatch[] = [];
  actualQtyTotal: number;
  modifyQtyTotal: number;
  modifyQtyList: number[] = [];
  checkMissing = false;
  optionByBatch = '0';
  typeOther = "2";
  reasons: SettingReason[] = [];
  reason: string;
  constructor(private modifyQrCodeService: ModifyQrcodeService,
    private alertify: AlertifyService,
    private spinner: NgxSpinnerService,
    private router: Router) { }

  ngOnInit(): void {
    this.modifyQrCodeService.currentModifyQrCodeMain.subscribe(res => {
      if (res == null) {
        this.modifyQrCodeMain = {
          supplier_ID: '',
          supplier_Name: '',
          subcon_ID: '',
          subcon_Name: '',
          article: '',
          mO_No: '',
          material_ID: '',
          material_Name: '',
          custmoer_Part: '',
          custmoer_Name: '',
          stock_Qty: 0,
          model_Name: '',
          model_No: '',
        };
      } else {
        this.modifyQrCodeMain = res;
      }
    });
    this.getReason();
    this.getDetail();
  }
  getDetail() {
    this.modifyQrCodeService.getDetailModifyQrCode(this.modifyQrCodeMain.mO_No, this.modifyQrCodeMain.material_ID).subscribe(res => {
      this.data1 = res.data1;
      this.data1Const = JSON.parse(JSON.stringify(res.data1));
      this.data1.map(obj => {
        if (obj.totalInstockQty >= obj.planQty) {
          obj.disableInput = true;
        } else if (obj.totalInstockQty < obj.planQty) {
          obj.disableInput = false;
        }
        return obj;
      })
      this.data2 = res.data3;
      this.data2Const = JSON.parse(JSON.stringify(res.data3));

    })
  }
  getReason() {
    this.modifyQrCodeService.getAllReason().subscribe(res => {
      this.reasons = res.filter(x => x.kind === 1);
      if(this.reasons.length > 0) {
        this.reason = this.reasons[0].hP_Reason_Code;
      }
    });
  }
  changeInput(e, i) {
     // load l???i gi?? tr??? c???a c???t ???? t????ng ???ng order-size.
     this.data2Const.forEach(item1 => {
      this.data2.forEach(item2 => {
        if (item2.mO_Seq === item1.mO_Seq) {
          item2.dataDetail[i].totalInstockQty = item1.dataDetail[i].totalInstockQty;
        }
      });
    });
    if (e <= this.data1Const[i].totalInstockQty) {
      this.data1[i].totalInstockQty = this.data1Const[i].totalInstockQty;
      // G??n l???i justReceiving nh?? ban ?????u
      for (let u = 0; u < this.data2Const.length; u++) {
        (this.data2[u].dataDetail)[i].justReceivedQty = (this.data2Const[u].dataDetail)[i].justReceivedQty;
      }
      (<HTMLInputElement>document.getElementById(this.data1Const[i].order_Size)).value = this.data1Const[i].totalInstockQty.toString();
    } else if (e > this.data1Const[i].planQty) {
      this.data1[i].totalInstockQty = this.data1Const[i].planQty;
      (<HTMLInputElement>document.getElementById(this.data1Const[i].order_Size)).value = this.data1Const[i].planQty.toString();
    } else {
      let qtyReceived = e - this.data1Const[i].totalInstockQty;
      for (let y = 0; y < this.data2Const.length; y++) {
        // N???u s??? l?????ng t???i ??a c?? th??? nh???n v??o kh??c 0 v?? kh??c null
        if ((this.data2Const[y].dataDetail)[i].justReceivedQty !== 0 && (this.data2Const[y].dataDetail)[i].justReceivedQty !== null) {
          let totalJustReceivedBeforeSize = 0;
          if(y > 0) {
            for (let z = y - 1; z >=0 ; z--) {
              if((this.data2Const[z].dataDetail)[i].justReceivedQty !== 0 && (this.data2Const[z].dataDetail)[i].justReceivedQty !== null) {
                totalJustReceivedBeforeSize += (this.data2Const[z].dataDetail)[i].justReceivedQty;
                break;
              }
            }
            qtyReceived = qtyReceived - totalJustReceivedBeforeSize;
          }
             // N???u s??? l?????ng nh???p v??o <= s??? l?????ng t???i ??a c?? th??? nh???n v??o
            if (qtyReceived > 0 && qtyReceived <= (this.data2Const[y].dataDetail)[i].justReceivedQty) {
              (this.data2[y].dataDetail)[i].totalInstockQty = (this.data2[y].dataDetail)[i].totalInstockQty + qtyReceived;

              // L?????ng c???n nh???n ????? ????? c??n l???i
              (this.data2[y].dataDetail)[i].justReceivedQty = (this.data2Const[y].dataDetail)[i].justReceivedQty - qtyReceived;
              qtyReceived = 0;

              break;
            } // N???u s??? l?????ng nh???p v??o > s??? l?????ng t???i ??a c?? th??? nh???n v??o
            else {
              // Gi?? tr??? hi???n th??? gi??? s??? b???ng gi?? tr??? c?? + gi?? tr??? t???i ??a c?? th??? nh???n v??o.
              (this.data2[y].dataDetail)[i].totalInstockQty = (this.data2[y].dataDetail)[i].totalInstockQty + (this.data2Const[y].dataDetail)[i].justReceivedQty;
              qtyReceived = qtyReceived - (this.data2[y].dataDetail)[i].justReceivedQty;
              // L?????ng c???n nh???n ????? ????? c??n l???i s??? = 0
              (this.data2[y].dataDetail)[i].justReceivedQty = 0;
            }
        }
      }
    }
  }
  cancel() {

  }
  submitTable() {
    if (this.modifyQtyTotal > 0) {
      this.spinner.show();
      let reasons = [{
        reason: this.reason,
        qty: this.modifyQtyTotal
      }];
      let param = {
        model: this.modifyQrCodeMain,
        data: this.data2,
        isMissing: this.checkMissing,
        reason_code: this.reason,
        reasons: reasons
      }
      this.modifyQrCodeService.saveNoByBatchIn(param).subscribe(res => {
        this.modifyQrCodeService.changeModifyQrCodeAfterSave(res);
        this.modifyQrCodeService.changeOtherType('Other In');
        this.router.navigate(['/stock-adj/list-qrcode-change']);
        this.spinner.hide();
      }, error => {
        this.alertify.error(error);
        this.spinner.hide();
      });
    } else {
      this.alertify.error('Please change value input!');
    }
  }
  changePageByBath() {
    this.router.navigate(['/stock-adj/by-batch-in']);
  }
  changeOtherType() {
    this.router.navigate(['/stock-adj/no-by-batch-out']);
  }
  ngAfterContentChecked() {
    if (this.data1.length > 0) {
      let countSize = this.data1Const.length;
      for (let t = 0; t < countSize; t++) {
        if (this.data1[t].totalInstockQty <= this.data1Const[t].totalInstockQty) {
          this.data1[t].totalInstockQty = this.data1Const[t].totalInstockQty
        }
      }
      let countData = this.data1Const.length;
      this.modifyQtyList.length = 0;

      let stockQtyATotal = this.data1Const.map(o => o.totalInstockQty).reduce((a, c) => { return a + c });
      this.actualQtyTotal = this.data1.map(o => o.totalInstockQty).reduce((a, c) => { return a + c });
      // this.modifyQtyTotal = stockQtyATotal - this.actualQtyTotal;
      this.modifyQtyTotal = this.actualQtyTotal - stockQtyATotal;

      for (let y = 0; y < countData; y++) {
        let modifyQtyOfSize;
        if (this.data1Const[y].totalInstockQty <= this.data1[y].totalInstockQty) {
          // modifyQtyOfSize = this.data1Const[y].totalInstockQty - this.data1[y].totalInstockQty;
          modifyQtyOfSize = this.data1[y].totalInstockQty - this.data1Const[y].totalInstockQty;
        } else {
          modifyQtyOfSize = 0;
        }
        this.modifyQtyList.push(modifyQtyOfSize);
      }

      this.data2.forEach(item => {
        item.total = item.dataDetail.map(o => o.totalInstockQty).reduce((a, c) => { return a + c });
      });

      // T??nh s??? l?????ng ???????c c???ng c???a m???i size theo batch(M???i ??)
      for (let m = 0; m < this.data2.length; m++) {
        for (let n = 0; n < countSize; n++) {
          (this.data2[m].dataDetail)[n].modifyQty = (this.data2[m].dataDetail)[n].totalInstockQty - (this.data2Const[m].dataDetail)[n].totalInstockQty;
        }
      }

      // Set m??u cho ?? n??o c?? thay ?????i gi?? tr???
      this.data2.forEach(item => {
        item.dataDetail.forEach(item1 => {
          if (item1.modifyQty !== null && item1.modifyQty !== 0) {
            item1.isChange = true;
          } else {
            item1.isChange = false;
          }
        });
      })
    }
  }
}

