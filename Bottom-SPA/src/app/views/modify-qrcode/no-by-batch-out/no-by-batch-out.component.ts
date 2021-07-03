import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NgxSpinnerService } from 'ngx-spinner';
import { ModifyQrCodeMain } from '../../../_core/_models/modify-qrcode-main';
import { SizeInStockPlanQty, SizeInstockQtyByBatch } from '../../../_core/_models/size-instockqty-bybath';
import { AlertifyService } from '../../../_core/_services/alertify.service';
import { ModifyQrcodeService } from '../../../_core/_services/modify-qrcode.service';
import { FunctionUtility } from '../../../_core/_utility/function-utility';
import { ModifyReasonQtyModel, QtyOfSize } from '../../../_core/_viewmodels/modify-reason-qty-model';
import { BatchDetail, LeftRightInBatchOfReason, ReasonDetail } from '../../../_core/_viewmodels/modify-store/reason-detail';

@Component({
  selector: 'app-no-by-batch-out',
  templateUrl: './no-by-batch-out.component.html',
  styleUrls: ['./no-by-batch-out.component.css']
})
export class NoByBatchOutComponent implements OnInit {
  modifyQrCodeMain: ModifyQrCodeMain;
  data1: SizeInStockPlanQty[] = [];
  data1Const: SizeInStockPlanQty[] = [];
  data2: SizeInstockQtyByBatch[] = [];
  data2Const: SizeInstockQtyByBatch[] = [];
  deliveryNos: string[] = [];
  stockQtyATotal: number;
  actualQtyTotal: number;
  modifyQtyTotal: number;
  modifyQtyList: number[] = [];
  checkMissing = true;
  optionByBatch = '0';
  typeOther = "1";
  reasonDataList: ModifyReasonQtyModel[] = [];
  leftRightInBatch: LeftRightInBatchOfReason[] = [];
  constructor(private modifyQrCodeService: ModifyQrcodeService,
              private funtionUtility: FunctionUtility,
              private spinner: NgxSpinnerService,
              private router: Router,
              private alertify: AlertifyService) { }

  ngOnInit(): void {
    this.modifyQrCodeService.currentModifyQrCodeMain.subscribe(res => {
      if (res == null) {
        this.router.navigate(['/modify-store/main']);
      } else {
        this.modifyQrCodeMain = res;
      }
    });
    this.getDetail();
  }
  getDetail() {
    this.modifyQrCodeService.getDetailModifyQrCode(this.modifyQrCodeMain.mO_No, this.modifyQrCodeMain.material_ID).subscribe(res => {
      this.data1 = res.data1;
      this.data1Const = JSON.parse(JSON.stringify(res.data1));
      this.data2 = res.data2;
      this.data2Const = JSON.parse(JSON.stringify(res.data2));
      this.deliveryNos = res.deliveryNos;
      
      this.getReasonOfSupplier(this.modifyQrCodeMain.supplier_ID);
      console.log(this.data2Const);
      

      // this.modifyQtyList = new Array(this.data1.length).fill(0);
    })
  }
  getReasonOfSupplier(supplier: string) {
    this.modifyQrCodeService.getReasonOfSupplier(supplier).subscribe(res => {
        res.forEach(x => {
          let reasonModel: ModifyReasonQtyModel = {
            reason_Code: x.reason_Code.trim(),
            reason_Name: x.reason_Name.trim(),
            qtyOfSizes: this.data1.map(item => {
              return {
                tool_Size: item.tool_Size,
                order_Size: item.order_Size,
                qty: 0
              }
            }),
            qtyOfSizesLeft: this.data1.map(item => {
              return {
                tool_Size: item.tool_Size,
                order_Size: item.order_Size,
                qty: 0
              }
            }),
            qtyOfSizesRight: this.data1.map(item => {
              return {
                tool_Size: item.tool_Size,
                order_Size: item.order_Size,
                qty: 0
              }
            }),
            totalModifyQty:0,
            totalLeft: 0,
            totalRight: 0
          };
          this.reasonDataList.push(reasonModel);
        }); 
    });
  }
  changePageByBath() {
    this.router.navigate(['/modify-store/by-batch-out']);
  }
  changeOtherType() {
    this.router.navigate(['/modify-store/no-by-batch-in']);
  }
  changeInputReason(e: number,item1: ModifyReasonQtyModel, item2: QtyOfSize, i1: number, i2: number) {
    item1.qtyOfSizesLeft[i2].qty = e;
    item1.qtyOfSizesRight[i2].qty = e;
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
  changeInputLeftOrRight(i1: number, i2: number) {
    // Đang change Input ở Right
    let qtyRight = this.reasonDataList[i1].qtyOfSizesRight[i2].qty;
    let qtyLeft = this.reasonDataList[i1].qtyOfSizesLeft[i2].qty;
    this.reasonDataList[i1].qtyOfSizes[i2].qty = (qtyRight+ qtyLeft)/2;
  }
  cancel() {

  }
  shareLeftRight() {
    this.leftRightInBatch.length = 0;
    this.data1Const.forEach((element1,i) => {
      // Nếu size đó có thay đổi số lượng thì thực hiện tính toán
      if(this.modifyQtyList[i] > 0) {
        let reasons: ReasonDetail[] = [];
        let batchs: BatchDetail[] = [];
        this.reasonDataList.forEach(element2 => {
          let left = element2.qtyOfSizesLeft[i].qty;
          let right =  element2.qtyOfSizesRight[i].qty;
          if(left !== 0 && right !== 0) {
            let itemReasonOfSize: ReasonDetail = {
              reason_code: element2.reason_Code, 
              tool_size: element1.tool_Size,
              order_size: element1.order_Size,
              left: left, 
              right: right };
            reasons.push(itemReasonOfSize);
          }
        });

        // Gửi vào hàm getLeftRight để tính toán
        this.data2Const.forEach(item => {
          let qty = item.dataDetail[i].totalInstockQty;
          // qty hiển thị bên ngoài phải khác 0 thì mới add vào để tính toán
          if(qty != 0) {
            let batchItemOfSize: BatchDetail = {batch: item.mO_Seq, left: qty, right: qty};
            batchs.push(batchItemOfSize);
          }
        });
        if(batchs.length > 0) {
          let shareLeftAndRightOfBatch = this.funtionUtility.getLeftRight(reasons, batchs);
          this.leftRightInBatch.push(...shareLeftAndRightOfBatch);
  
          // Kiểm tra batch nào có thay đổi giá trị
          let batchInChange = [...new Set(shareLeftAndRightOfBatch.map(x => x.batch))];
          if(batchInChange.length>0) {
            batchInChange.forEach(itemBatch => {
              let leftAndRight = shareLeftAndRightOfBatch.filter(x => x.batch == itemBatch).map(x => x.left + x.right).reduce((a,c) => {return a+ c});
              let data2Find = this.data2.find(x => x.mO_Seq == itemBatch);
              data2Find.dataDetail[i].modifyQty = leftAndRight/2;
            });
          }
          // Kiểm tra batch nào ko có thay đổi giá trị thì trả về số lượng như ban đầu
          let batchAll = this.data2.map(x => x.mO_Seq);
          let batchNoChange = batchAll.filter(x => !batchInChange.includes(x));
          if(batchNoChange.length> 0) {
            batchNoChange.forEach(itemBatch => {
              let data2Find = this.data2.find(x => x.mO_Seq == itemBatch);
              data2Find.dataDetail[i].modifyQty = 0;
            })
          }
        }
      } else {
        // Nếu size đó ko có thay đổi số lượng thì qty ở bên ngoài phải hiển thị như ban đầu,khi chưa giảm
        this.data2.forEach((item,y) => {
          item.dataDetail[i].totalInstockQty = this.data2Const[y].dataDetail[i].totalInstockQty;
          item.dataDetail[i].modifyQty = 0;
        });
      }
    });
    
  }
  submitTable() {
    let checkError = this.modifyQtyList.some((x,i) => x > this.data1Const[i].totalInstockQty);
    if(checkError) {
      return this.alertify.error('The number of pairs out has exceeded the allowed limit!');
    }
    if(this.modifyQtyTotal === 0) {
      return this.alertify.error('Please change value input!');
    }

    this.spinner.show();
    let param = {
      model: this.modifyQrCodeMain,
      data: this.data2,
      reason_code: '',
      isMissing: this.checkMissing,
      reasonDetail: this.leftRightInBatch
    };
    //console.log(this.leftRightInBatch);
    
    this.modifyQrCodeService.saveNoByBatch(param).subscribe(res => {
      this.modifyQrCodeService.changeModifyQrCodeAfterSave(res);
      this.modifyQrCodeService.changeOtherType('Other Out');
      this.router.navigate(['/modify-store/list-qrcode-change']);
      this.spinner.hide();
    }, error => {
      this.alertify.error(error);
      this.spinner.hide();
    });
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

      this.data2.forEach(item => {
        item.total = item.dataDetail.map(o => o.totalInstockQty).reduce((a, c) => { return a + c });
      })

      // ------------------------------------------------------------------------------------------------------------------//
      // Hiển thị số đôi lỗi theo từng size của all các reason
      if(this.reasonDataList.length>0) {
        for (let y = 0; y < countData; y++) {
          let modifyQtyOfSize = this.reasonDataList.map(x => x.qtyOfSizes).map(x => x[y].qty).reduce((a,c) => {return a + c});
          this.modifyQtyList.push(modifyQtyOfSize);
        }
      }

     // Hiển thị tổng số đôi lỗi của all Size
     if(this.modifyQtyList.length > 0) {
      this.modifyQtyTotal = this.modifyQtyList.reduce((a,c) => {return a + c});
     }

     // ------------------------------------------------------------------------------------------------------------------//
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
      // ------------------------------------------------------------------------------------------------------------------//
      // Hiển thị tổng số chiếc Right và Left của mỗi reason
      if(this.reasonDataList.length > 0) {
        this.reasonDataList.forEach(item => {
          item.totalRight = item.qtyOfSizesRight.map(x => x.qty).reduce((a,c) => {return a + c});
          item.totalLeft = item.qtyOfSizesLeft.map(x => x.qty).reduce((a,c) => {return a + c});
          item.totalModifyQty = (item.totalRight + item.totalLeft)/2;
        });
      }

     // ------------------------------------------------------------------------------------------------------------------//
      // this.data1.forEach((item,i) => {
      //   item.totalInstockQty = this.data1Const[i].totalInstockQty - this.modifyQtyList[i];
      // })
      this.data1.map((x,i) => x.totalInstockQty = this.data1Const[i].totalInstockQty - this.modifyQtyList[i]);
      // this.data1.forEach((x,i) => {
      //   this.changeInput(x.totalInstockQty,i);
      // });
      
      this.shareLeftRight();

      // ------------------------------------------------------------------------------------------------------------------//
      // Tính số lượng bị trừ của mỗi size theo batch(Mỗi ô)
      for (let m = 0; m < this.data2.length; m++) {
        for (let n = 0; n < countSize; n++) {
          (this.data2[m].dataDetail)[n].totalInstockQty = (this.data2Const[m].dataDetail)[n].totalInstockQty - (this.data2[m].dataDetail)[n].modifyQty;
        }
      }
      // ------------------------------------------------------------------------------------------------------------------//
    }
  }
}
