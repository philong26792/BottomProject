import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NgxSpinnerService } from 'ngx-spinner';
import { ModifyQrCodeMain } from '../../../_core/_models/modify-qrcode-main';
import { SettingReason } from '../../../_core/_models/setting-reason';
import { SizeInStockPlanQty, SizeInstockQty, SizeInstockQtyByBatch } from '../../../_core/_models/size-instockqty-bybath';
import { AlertifyService } from '../../../_core/_services/alertify.service';
import { ModifyQrcodeService } from '../../../_core/_services/modify-qrcode.service';

@Component({
  selector: 'app-by-batch-in',
  templateUrl: './by-batch-in.component.html',
  styleUrls: ['./by-batch-in.component.scss']
})
export class ByBatchInComponent implements OnInit {
  data1DB: SizeInStockPlanQty[] = [];// giá trị bên API trả lên
  data2DB: SizeInstockQtyByBatch[] = [];// giá trị bên API trả lên
  data2DB_L1: SizeInstockQtyByBatch[] = [];
  data2DB_L2: SizeInstockQtyByBatch[] = [];
  data2DB_L3: SizeInstockQtyByBatch[] = [];
  modifyQrCodeMain: ModifyQrCodeMain;
  data1: SizeInStockPlanQty[] = [];// để lấy toolsize, othersize, planqty của từng phần tử
  data1Const: SizeInstockQty[] = [];// giá trị ban đầu của list đó
  data2: SizeInstockQtyByBatch[] = [];// giá trị sau khi thay đổi
  planQtyTotal: number;
  stockQtyATotal: number;
  actualQtyTotal: number;
  modifyQtyTotal: number;
  modifyQtyList: number[] = [];
  checkMissing = false;
  modifyType: number = 1;
  listBatch: string[] = [];
  batch: string = '';
  reasons: SettingReason[] = [];
  reason: string;
  typeOther = 2;

  constructor(private modifyQrCodeService: ModifyQrcodeService,
    private spinner: NgxSpinnerService,
    private alertify: AlertifyService,
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
    this.getDetail();
    this.getReason();
  }
  getDetail() {
    this.modifyQrCodeService.getDetailModifyQrCode(this.modifyQrCodeMain.mO_No, this.modifyQrCodeMain.material_ID).subscribe(res => {
      this.data1DB = res.data1;
      this.data2DB = res.data2;

      // gán thành 3 biến mới để tạo ra vùng nhớ mới, khi thay đổi giá trị không ảnh hưởng đến biến khác
      // vì có nhiều biến trỏ đến res.data2
      this.data2DB_L1 = JSON.parse(JSON.stringify(res.data2));
      this.data2DB_L2 = JSON.parse(JSON.stringify(res.data2));
      this.data2DB_L3 = JSON.parse(JSON.stringify(res.data2));

      /////////////tách ra batch
      this.listBatch = this.data2DB.map(item => {
        return item.mO_Seq
      });
      this.batch = this.listBatch[0];

      this.data2 = this.data2DB_L1.filter(item => {
        return item.mO_Seq == this.batch;
      });
      this.data1 = this.data2DB_L2.filter(item => {
        return item.mO_Seq == this.batch;
      })[0].dataDetail;
      this.data1.map(obj => {
        if (obj.totalInstockQty >= obj.planQty) {
          obj.disableInput = true;
        } else if (obj.totalInstockQty < obj.planQty) {
          obj.disableInput = false;
        }
        return obj;
      })

      this.data1Const = this.data2DB_L3.filter(item => {
        return item.mO_Seq == this.batch;
      })[0].dataDetail;
    })
  }

  ngAfterContentChecked() {
    if (this.data1.length > 0) {
      this.modifyQtyList.length = 0;

      this.planQtyTotal = this.data1Const.map(o => o.planQty).reduce((a, c) => { return a + c });
      this.stockQtyATotal = this.data1Const.map(o => o.totalInstockQty).reduce((a, c) => { return a + c });
      this.actualQtyTotal = this.data1.map(o => o.totalInstockQty).reduce((a, c) => { return a + c });
      // this.modifyQtyTotal = this.stockQtyATotal - this.actualQtyTotal;
      this.modifyQtyTotal = this.actualQtyTotal - this.stockQtyATotal;
      
      this.data2.forEach(item => {
        item.total = item.dataDetail.map(o => o.totalInstockQty).reduce((a, c) => { return a + c });
      })
      // Tính số lượng được cộng thêm của mỗi size theo batch(Mỗi ô)
      let countSize = this.data1Const.length;
      for (let n = 0; n < countSize; n++) {
        (this.data2[0].dataDetail)[n].modifyQty = (this.data2[0].dataDetail)[n].totalInstockQty - (this.data1Const)[n].totalInstockQty;
      }

      let countData = this.data1Const.length;
      for (let y = 0; y < countData; y++) {
        let modifyQtyOfSize = this.data1Const[y].totalInstockQty - this.data1[y].totalInstockQty;
        this.modifyQtyList.push(modifyQtyOfSize);
      }

      // Set màu cho ô nào có thay đổi giá trị
      this.data2[0].dataDetail.forEach(item1 => {
        if (item1.modifyQty !== null && item1.modifyQty !== 0) {
          item1.isChange = true;
        } else {
          item1.isChange = false;
        }
      });
    }
  }

  changeBatch() {
    this.data2 = this.data2DB_L1.filter(item => {
      return item.mO_Seq == this.batch;
    });
    this.data1 = this.data2DB_L2.filter(item => {
      return item.mO_Seq == this.batch;
    })[0].dataDetail;
    this.data1.map(obj => {
      if (obj.totalInstockQty >= obj.planQty) {
        obj.disableInput = true;
      } else if (obj.totalInstockQty < obj.planQty) {
        obj.disableInput = false;
      }
      return obj;
    })
    this.data1Const = this.data2DB_L3.filter(item => {
      return item.mO_Seq == this.batch;
    })[0].dataDetail;
  }

  changeInput(e, i) {
    if(e <= this.data1Const[i].totalInstockQty) {
      this.data1[i].totalInstockQty = this.data1Const[i].totalInstockQty;
      (this.data2[0].dataDetail)[i].totalInstockQty = this.data1Const[i].totalInstockQty;
      (<HTMLInputElement>document.getElementById(this.data1Const[i].order_Size)).value = this.data1Const[i].totalInstockQty.toString();
    } else if (e > this.data1Const[i].planQty) {
      this.data1[i].totalInstockQty = this.data1Const[i].planQty;
      (<HTMLInputElement>document.getElementById(this.data1Const[i].order_Size)).value = this.data1Const[i].planQty.toString();
    } else {
      let valueInput = e;
      (this.data2[0].dataDetail)[i].totalInstockQty = valueInput;
    }
  }

  changePageByBath() {
    if (this.modifyType == 0) {
      this.router.navigate(['/stock-adj/no-by-batch-out']);
    }
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
        this.spinner.hide();
        this.alertify.error('Error Save!')
      });
    } else {
      this.alertify.error('Please change value input!');
    }
  }

  getReason() {
    this.modifyQrCodeService.getAllReason().subscribe(res => {
      this.reasons = res.filter(x => x.kind === 1);
      if(this.reasons.length > 0) {
        this.reason = this.reasons[0].hP_Reason_Code;
      }
    });
  }

  changeOtherType() {
    this.router.navigate(['/stock-adj/by-batch-out']);
  }
}
