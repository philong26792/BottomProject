import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgxSpinnerService } from 'ngx-spinner';
import { fromEvent } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { SplitDataByOffset } from '../../../_core/_models/merge-qrcode/split-data-by-Offset';
import { SplitProcess } from '../../../_core/_models/merge-qrcode/split-process';
import { AlertifyService } from '../../../_core/_services/alertify.service';
import { MergeQrcodeService } from '../../../_core/_services/merge-qrcode.service';
import { TransferService } from '../../../_core/_services/transfer.service';
import { FunctionUtility } from '../../../_core/_utility/function-utility';

@Component({
  selector: 'app-other-split-process',
  templateUrl: './other-split-process.component.html',
  styleUrls: ['./other-split-process.component.scss']
})
export class OtherSplitProcessComponent implements OnInit {

  transacNo: string = '';
  listOffsetNo: string[] = [];
  offsetNo: string = '0';
  splitProccess: SplitProcess = {
    transacMainMergeQrCode: {
      qrCode_ID: '',
      mO_No: '',
      purchase_No: '',
      transac_No: '',
      model_No: '',
      model_Name: '',
      receive_No: '',
      article: '',
      qRCode_Version: 0,
      supplier_ID: '',
      supplier_Name: '',
      part_No: '',
      part_Name: '',
      rack_Location: '',
      material_ID: '',
      material_Name: '',
      stock_Qty: 0
    },
    listSizeAndQty: [],
    listOffsetNo: [],
    sumInstockQty: 0,
    sumAccQty: 0,
    sumRemainingInstockQty: 0,
  };
  listSplitDataByOffset: SplitDataByOffset[] = [];
  listSplitDataByOffsetConst: SplitDataByOffset[] = [];
  newMoNo: string = '';
  moSeq: string = '';
  totalAccumlatedOutQty: number;
  totalRemainingQty: number;
  constructor(private route: ActivatedRoute,
    private router: Router,
    private functionUtility: FunctionUtility,
    private spinner: NgxSpinnerService,
    private alertify: AlertifyService,
    private mergeQrcodeService: MergeQrcodeService,
    private transferService: TransferService) {
  }

  ngOnInit(): void {
    this.transacNo = this.route.snapshot.params['transacNo'];
    this.getData();
  }

  getData() {
    this.spinner.show();
    this.mergeQrcodeService.searchSplitProcess(this.transacNo).subscribe(res => {
      this.splitProccess = res;
      this.spinner.hide();
    }, error => {
      this.spinner.hide();
    });
  }

  deleteItemDateSplitByOffset(index: number) {
    this.alertify.confirm('Delete', 'Do you want remove it', () => {
      this.listSplitDataByOffset.splice(index, 1);
      this.listSplitDataByOffsetConst.splice(index, 1);
      let countSize = this.splitProccess.listSizeAndQty.length;
      for (let i = 0; i < countSize; i++) {
        let totalAccumQty = 0;
        this.listSplitDataByOffset.forEach(item => {
          totalAccumQty = totalAccumQty + item.listSizeAndQty[i].instock_Qty;
        });
        this.splitProccess.listSizeAndQty[i].act_Out_Qty = totalAccumQty;
      }
    });
  }

  submit() {
    for (let index = 0; index < this.listSplitDataByOffset.length; index++) {
      if (this.functionUtility.checkEmpty(this.listSplitDataByOffset[index].rack_Location)) {
        this.alertify.error('Please enter Rack Location');
        return;
      }
    }
    // Đếm số ô có thay đổi Input => Nếu = 0 thì thông báo cần nhập Input 
    let countChangeInput = 0;
    this.listSplitDataByOffset.forEach(item => {
      let count = item.listSizeAndQty.filter(x => x.instock_Qty !== 0 && x.instock_Qty !== null).length;
      countChangeInput = countChangeInput + count;
    });
    if(countChangeInput === 0) {
      this.alertify.error('Please Enter Qty');
    } else {
      // Tính số ô bị disable ban đầu,có nghĩa là các ô có instock_qty = null => a. 
      // Tính số ô có instock_qty = null trước khi bấm save => b
      // Nếu b > a có nghĩa là có 1 vài ô người dùng xóa nhưng không nhập vào.Thì bấm save sẽ hiển thị lỗi cần nhập vào

      let countIsNullDB = 0, countIsNullSave = 0;
      this.listSplitDataByOffsetConst.forEach(item => {
        let countIsNullItem = item.listSizeAndQty.filter(x => x.instock_Qty === null).length;
        countIsNullDB = countIsNullDB + countIsNullItem;
      });
      this.listSplitDataByOffset.forEach(item => {
        let countIsNullItem = item.listSizeAndQty.filter(x => x.instock_Qty === null).length;
        countIsNullSave = countIsNullSave + countIsNullItem;
      });
      if(countIsNullSave > countIsNullDB) {
        this.alertify.error('Input not blank!');
      } else {
        this.spinner.show();
        this.mergeQrcodeService.saveDataOtherSplit(this.listSplitDataByOffset).subscribe(res => {
          if (res) {
            this.alertify.success('Save success!');
          } else {
            this.alertify.error('Error save server!');
          }
          this.spinner.hide();
          this.router.navigate(['/merge-qrcode/other-split/detail', this.transacNo]);
        }, error => {
          this.spinner.hide();
        });
      }
    }
  }

  add() {
    if (this.newMoNo == '') {
      this.alertify.error('Please enter Plan No');
      return;
    }
    if (this.listSplitDataByOffset.map(item => item.mO_No.trim() + item.mO_Seq.trim()).includes(this.newMoNo.trim() + this.moSeq.trim())) {
      this.alertify.error('Plan No Existed');
      return;
    }
    
    this.spinner.show();
    this.mergeQrcodeService.getDataOtherSplitByMONo(this.splitProccess.transacMainMergeQrCode.material_ID, this.newMoNo, this.moSeq, this.splitProccess.transacMainMergeQrCode.transac_No, this.splitProccess.transacMainMergeQrCode.mO_No)
      .subscribe(res => {
        if (res != null) {
          res.listSizeAndQty.map(obj => {
            obj.instock_QtyDb = obj.instock_Qty;
            return obj;
          });
          res.sumInstockQty = 0;
          this.listSplitDataByOffset.push(res);
          this.listSplitDataByOffsetConst.push(JSON.parse(JSON.stringify(res)));
          // this.listSplitDataByOffsetConst = JSON.parse(JSON.stringify(this.listSplitDataByOffset));
        } else {
          this.alertify.error('No data');
        }
        this.spinner.hide();
      }, error => {
        this.alertify.error('Error Server');
        this.spinner.hide();
      });
  } 
  ngAfterContentChecked() {
    if (this.splitProccess.listSizeAndQty.length > 0) {
      this.totalAccumlatedOutQty = this.splitProccess.listSizeAndQty.map(x => x.act_Out_Qty).reduce((a, c) => { return a + c });
      let totalStockQty = this.splitProccess.listSizeAndQty.map(x => x.instock_Qty).reduce((a, c) => { return a + c });
      this.totalRemainingQty = totalStockQty - this.totalAccumlatedOutQty;
    }
  }
  changeInput(e, i, y, item) {
    //let idChange = (this.listSplitDataByOffsetConst[i].listSizeAndQty)[y].order_Size + '' + i;
    // let changeInput = document.getElementById(idChange);
    // let changeInput$ = fromEvent(changeInput, 'input');
    // changeInput$.pipe(
    //   debounceTime(500),
    //   distinctUntilChanged()
    // ).subscribe(() => {
    //   let purchase_qtyOfSize = (this.listSplitDataByOffset[i].listSizeAndQty)[y].purchase_Qty;
    //   if (e > purchase_qtyOfSize) {
    //     this.alertify.error('Quantity must not be more than' + purchase_qtyOfSize);
    //     (this.listSplitDataByOffset[i].listSizeAndQty)[y].instock_Qty = (this.listSplitDataByOffset[i].listSizeAndQty)[y].purchase_Qty;
    //     (<HTMLInputElement>document.getElementById((this.listSplitDataByOffsetConst[i].listSizeAndQty)[y].order_Size + '' + i)).value = (this.listSplitDataByOffset[i].listSizeAndQty)[y].purchase_Qty.toString();
    //   } else {
    //     let totalQty = 0;
    //     this.listSplitDataByOffset.forEach(item => {
    //       totalQty = totalQty + item.listSizeAndQty[y].instock_Qty;
    //     });
    //     if (totalQty > (this.splitProccess.listSizeAndQty)[y].instock_Qty) {
    //       this.alertify.error('Total All Size Not be more than' + (this.splitProccess.listSizeAndQty)[y].instock_Qty + '');
    //       (this.listSplitDataByOffset[i].listSizeAndQty)[y].instock_Qty = (this.listSplitDataByOffsetConst[i].listSizeAndQty)[y].instock_Qty;
    //       (<HTMLInputElement>document.getElementById((this.listSplitDataByOffsetConst[i].listSizeAndQty)[y].order_Size + '' + i)).value = (this.listSplitDataByOffsetConst[i].listSizeAndQty)[y].instock_Qty.toString();
    //     } else {
    //       // Xử lý bình thường
    //       this.splitProccess.listSizeAndQty[y].act_Out_Qty = totalQty;
    //       this.listSplitDataByOffsetConst = JSON.parse(JSON.stringify(this.listSplitDataByOffset));
    //       item.sumInstockQty = item.listSizeAndQty.reduce((sum, item) => sum + item['instock_Qty'], 0);
    //     }
    //   }
    // })
    const purchase_qtyOfSize = (this.listSplitDataByOffset[i].listSizeAndQty)[y].purchase_Qty;
      if (e > purchase_qtyOfSize) {
        this.alertify.error('Quantity must not be more than' + purchase_qtyOfSize);
        (this.listSplitDataByOffset[i].listSizeAndQty)[y].instock_Qty = (this.listSplitDataByOffset[i].listSizeAndQty)[y].purchase_Qty;
        (<HTMLInputElement>document.getElementById((this.listSplitDataByOffsetConst[i].listSizeAndQty)[y].order_Size + '' + i)).value = (this.listSplitDataByOffset[i].listSizeAndQty)[y].purchase_Qty.toString();
      } else {
        let totalQty = 0;
        this.listSplitDataByOffset.forEach(item => {
          totalQty = totalQty + item.listSizeAndQty[y].instock_Qty;
        });
        if (totalQty > (this.splitProccess.listSizeAndQty)[y].instock_Qty) {
          this.alertify.error('Total All Size Not be more than' + (this.splitProccess.listSizeAndQty)[y].instock_Qty + '');
          (this.listSplitDataByOffset[i].listSizeAndQty)[y].instock_Qty = (this.listSplitDataByOffsetConst[i].listSizeAndQty)[y].instock_Qty;
          (<HTMLInputElement>document.getElementById((this.listSplitDataByOffsetConst[i].listSizeAndQty)[y].order_Size + '' + i)).value = (this.listSplitDataByOffsetConst[i].listSizeAndQty)[y].instock_Qty.toString();
        } else {
          // Xử lý bình thường
          this.splitProccess.listSizeAndQty[y].act_Out_Qty = totalQty;
          // this.listSplitDataByOffsetConst = JSON.parse(JSON.stringify(this.listSplitDataByOffset));
          item.sumInstockQty = item.listSizeAndQty.reduce((sum, item) => sum + item['instock_Qty'], 0);
        }
      }
  }

  checkRackExist(item: SplitDataByOffset) {
    if (!this.functionUtility.checkEmpty(item.rack_Location)) {
      this.transferService
      .checkExistLocation(item.rack_Location.toUpperCase())
      .subscribe((res) => {
        if (res === false) {
          item.rack_Location = '';
          this.alertify.error('Rack Location does not exist please scan again!');
          return;
        }
      });
    }
  }
}
