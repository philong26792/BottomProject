import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NgxSpinnerService } from 'ngx-spinner';
import { ModifyQrCodeMain } from '../../../_core/_models/modify-qrcode-main';
import { SettingReason } from '../../../_core/_models/setting-reason';
import { SizeInStockPlanQty, SizeInstockQtyByBatch } from '../../../_core/_models/size-instockqty-bybath';
import { AlertifyService } from '../../../_core/_services/alertify.service';
import { ModifyQrcodeService } from '../../../_core/_services/modify-qrcode.service';

@Component({
  selector: 'app-no-by-batch-out',
  templateUrl: './no-by-batch-out.component.html',
  styleUrls: ['./no-by-batch-out.component.scss']
})
export class NoByBatchOutComponent implements OnInit {
  modifyQrCodeMain: ModifyQrCodeMain;
  data1: SizeInStockPlanQty[] = [];
  data1Const: SizeInStockPlanQty[] = [];
  data2: SizeInstockQtyByBatch[] = [];
  data2Const: SizeInstockQtyByBatch[] = [];
  stockQtyATotal: number;
  actualQtyTotal: number;
  modifyQtyTotal: number;
  modifyQtyList: number[] = [];
  checkMissing = true;
  optionByBatch = '0';
  typeOther = "1";
  reasons: SettingReason[] = [];
  reason: string;
  constructor(private modifyQrCodeService: ModifyQrcodeService,
    private spinner: NgxSpinnerService,
    private router: Router,
    private alertify: AlertifyService) { }

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
      this.data2 = res.data2;
      this.data2Const = JSON.parse(JSON.stringify(res.data2));

    })
  }
  getReason() {
    this.modifyQrCodeService.getAllReason().subscribe(res => {
      this.reasons = res.filter(x => x.kind === 2);
      if(this.reasons.length > 0) {
        this.reason = this.reasons[0].hP_Reason_Code;
      }
    });
  }
  changePageByBath() {
    this.router.navigate(['/stock-adj/by-batch-out']);
  }
  changeOtherType() {
    this.router.navigate(['/stock-adj/no-by-batch-in']);
  }
  changeInput(e, i) {
    // load lại giá trị của cột đó tương ứng order-size.
    this.data2Const.forEach(item1 => {
      this.data2.forEach(item2 => {
        if (item2.mO_Seq === item1.mO_Seq) {
          item2.dataDetail[i].totalInstockQty = item1.dataDetail[i].totalInstockQty;
        }
      });
    });

    if (e > this.data1Const[i].totalInstockQty) {
      this.data1[i].totalInstockQty = this.data1Const[i].totalInstockQty;
      (<HTMLInputElement>document.getElementById(this.data1Const[i].order_Size)).value = this.data1Const[i].totalInstockQty.toString();
    } else {
      let valueInput = e;
      for (let y = this.data2Const.length - 1; y >= 0; y--) {
        let instockQtyValue = (this.data2[y].dataDetail)[i].totalInstockQty;
        if (instockQtyValue !== null) {
          if (valueInput <= instockQtyValue) {
            (this.data2[y].dataDetail)[i].totalInstockQty = valueInput;
            let t = y - 1;
            // instock những cột bên trên = 0 và thoát khỏi vòng lặp
            for (t; t >= 0; t--) {
              if ((this.data2[t].dataDetail)[i].totalInstockQty !== null) {
                (this.data2[t].dataDetail)[i].totalInstockQty = 0;
              }
            }
            break;
          } else {
            valueInput = valueInput - instockQtyValue;
          }
        }
      }
    }
  }
  cancel() {

  }
  submitTable() {
    // Check tổng số lượng thay đổi khi nào > 0 thì mới cho phép save data
    let totalModify = 0;
    this.data2.forEach(item => {
      let totalModifyByBatch = item.dataDetail.map(x => x.modifyQty).reduce((a, c) => { return a + c });
      totalModify = totalModify + totalModifyByBatch;
    });
    if (totalModify > 0) {
      this.spinner.show();
      let reasons = [{
        reason: this.reason,
        qty: -this.modifyQtyTotal
      }];
      let param = {
        model: this.modifyQrCodeMain,
        data: this.data2,
        isMissing: this.checkMissing,
        reason_code: this.reason,
        reasons: reasons
      }
      this.modifyQrCodeService.saveNoByBatch(param).subscribe(res => {
        this.modifyQrCodeService.changeModifyQrCodeAfterSave(res);
        this.modifyQrCodeService.changeOtherType('Other Out');
        this.router.navigate(['/stock-adj/list-qrcode-change']);
        this.spinner.hide();
      }, error => {
        this.alertify.error(error);
      });
    } else {
      this.alertify.error('Please change value input!');
    }
  }
  ngAfterContentChecked() {
    if (this.data1.length > 0) {
      this.modifyQtyList.length = 0;
      let countData = this.data1Const.length;
      for (let i = 0; i < countData; i++) {
        if (this.data1[i].totalInstockQty > this.data1Const[i].totalInstockQty) {
          this.data1[i].totalInstockQty = this.data1Const[i].totalInstockQty
        }
      }
      this.stockQtyATotal = this.data1Const.map(o => o.totalInstockQty).reduce((a, c) => { return a + c });
      this.actualQtyTotal = this.data1.map(o => o.totalInstockQty).reduce((a, c) => { return a + c });
      // this.modifyQtyTotal = this.stockQtyATotal - this.actualQtyTotal;
      this.modifyQtyTotal = this.actualQtyTotal - this.stockQtyATotal;

      this.data2.forEach(item => {
        item.total = item.dataDetail.map(o => o.totalInstockQty).reduce((a, c) => { return a + c });
      })

      for (let y = 0; y < countData; y++) {
        // let modifyQtyOfSize = this.data1Const[y].totalInstockQty - this.data1[y].totalInstockQty;
        let modifyQtyOfSize = this.data1[y].totalInstockQty - this.data1Const[y].totalInstockQty;
        this.modifyQtyList.push(modifyQtyOfSize);
      }

      // Set màu cho ô nào có thay đổi giá trị
      let countSize = this.data2Const[0].dataDetail.length
      for (let z = 0; z < this.data2Const.length; z++) {
        for (let t = 0; t < countSize; t++) {
          if ((this.data2[z].dataDetail)[t].totalInstockQty !== (this.data2Const[z].dataDetail)[t].totalInstockQty) {
            (this.data2[z].dataDetail)[t].isChange = true;
          } else {
            (this.data2[z].dataDetail)[t].isChange = false;
          }
        }
      }

      // Tính số lượng bị trừ của mỗi size theo batch(Mỗi ô)
      for (let m = 0; m < this.data2.length; m++) {
        for (let n = 0; n < countSize; n++) {
          (this.data2[m].dataDetail)[n].modifyQty = (this.data2Const[m].dataDetail)[n].totalInstockQty - (this.data2[m].dataDetail)[n].totalInstockQty;
        }
      }

    }
  }
}
