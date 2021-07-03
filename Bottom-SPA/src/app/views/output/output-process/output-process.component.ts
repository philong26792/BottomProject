import { Component, OnInit } from '@angular/core';
import { Router} from '@angular/router';
import { OutputService } from '../../../_core/_services/output.service';
import { TransferService } from '../../../_core/_services/transfer.service';
import { TransferDetail } from '../../../_core/_models/transfer-detail';
import { OutputM } from '../../../_core/_models/outputM';
import { FunctionUtility } from '../../../_core/_utility/function-utility';
import { NgxSpinnerService } from 'ngx-spinner';

@Component({
  selector: 'app-output-process',
  templateUrl: './output-process.component.html',
  styleUrls: ['./output-process.component.scss'],
})
export class OutputProcessComponent implements OnInit {
  transactionDetails: TransferDetail[] = [];
  result1 = []; // là listmaterialsheetsize sau khi group by theo toolsize, vì lúc hiện theo toolsize
  result2 = []; // là transactiondetail sau khi group by theo toolsize, vì lúc hiện theo toolsize
  result3 = []; // mảng chứa số lượng cần output ra theo từng size: là mảng để so sánh result1 và result2 xem ai nhỏ hơn thì lấy, và result 3 có thể thay đổi được nên tách ra thêm mảng nữa
  output: any = [];
  sumResult1: number = 0;
  sumResult3: number = 0;
  pickupNo: string = '';
  listOutputSave: any[] = [];
  // ------------------------------------------Total Qty------------------------------------//
  totalInStockQtyA: number;
  totalPickupQty: number;
  totalTransOutQtyB: number;
  totalRemainingQty: number;
  constructor(
    private router: Router,
    private outputService: OutputService,
    private transferService: TransferService,
    private functionUtility: FunctionUtility,
    private spinnerService: NgxSpinnerService
  ) { }

  ngOnInit() {
    // lấy ra currenoutput lưu trong oututservice, khi từ output main qua là có gán currentoutput
    this.outputService.currentOutputM.subscribe((res) => {
      this.output = res;
    }).unsubscribe();

    // lấy ra materialsheetsize: số lượng xuất ra theo đơn: lưu trong output service lúc load main lên là có lưu
    this.outputService.currentListMaterialSheetSize.subscribe((res) => {
      this.result1 = res;
      this.sumResult1 = this.result1.reduce((value, i) => {
        return value += i.value;
      }, 0);
    }).unsubscribe();

    // lấy ra những output đã được process
    this.outputService.currentListOutputSave.subscribe(res => {
      this.listOutputSave = res;
    }).unsubscribe();

    // load dữ liệu lên để proccess
    this.getData();
  }

  back() {
    this.router.navigate(['output/main']);
  }
  changeInput(e, i) {
    // khi thay đổi giá trị input thì bắt ràng buộc
    const tmp = this.result1[i].value > this.result2[i].value ? this.result2[i].value : this.result1[i].value;
    if (e > tmp) {
      const ele = document.getElementById('id-' + i) as HTMLInputElement;
      ele.value = tmp;
      this.result3[i].value = tmp;
    }
    if (e <= 0) {
      const ele = document.getElementById('id-' + i) as HTMLInputElement;
      ele.value = '0';
      this.result3[i].value = 0;
    }
    this.sumResult3 = this.result3.reduce((value, i) => {
      return value += i.value;
    }, 0);
  }
  getData() {
    // lấy ra transaction detail dựa vào transaction main
    this.transferService
      .getTransferDetailForOutput(this.output.transacNo)
      .subscribe((res) => {
        this.transactionDetails = res.listTransactionDetail;

        // Group by transactiondetail theo tool_Size rồi gán vào result2
        const groups = new Set(
          this.transactionDetails.map((item) => item.tool_Size)
        ),
          results = [];
        groups.forEach((g) =>
          results.push({
            name: g,
            value: this.transactionDetails
              .filter((i) => i.tool_Size === g)
              .reduce((instock_Qty, j) => {
                return (instock_Qty += j.instock_Qty);
              }, 0),
            array: this.transactionDetails.filter((i) => i.tool_Size === g),
          })
        );
        this.result2 = results;

        // chạy từng phần tử trong result1 và result2 để so sánh phần tử nào nhỏ hơn thì lấy phần tử đó gán vào result3:
        // result1 và result2 có cùng độ dài và result3 cũng bằng độ dài
        for (let i = 0; i < this.result1.length; i++) {
          this.result3.push({
            value:
              this.result1[i].value > this.result2[i].value
                ? this.result2[i].value
                : this.result1[i].value,
            name: this.result1[i].name,
          });
        }
        this.sumResult3 = this.result3.reduce((value, i) => {
          return value += i.value;
        }, 0);
      });
  }
  cancel() {
    this.router.navigate(['/output/main']);
  }
  async save() {
    this.spinnerService.show();
    //// -------- lúc lưu thì lấy biến listoutputmain lưu trên outputservice rồi gán giá trị mới của outputmain mới lưu
    let listOutputM: OutputM[];
    this.outputService.currentListOutputM.subscribe(
      (res) => (listOutputM = res)
    ).unsubscribe();
    // lấy ra vị trí của outputmain vừa lưu
    const indexOutput = listOutputM.indexOf(this.output);

    // sinh ra transacno mới theo yêu cầu viết trong hàm tiện ích
    this.output.transacNo = await this.functionUtility.getOutSheetNo(this.output.planNo);
    
    // gán giá trị transoutqty mới bằng tổng số lượng đã output ra trong result3
    this.output.transOutQty = this.result3.reduce((value, i) => {
      return (value += i.value);
    }, 0);
    this.output.remainingQty = this.output.inStockQty - this.output.transOutQty;

    // thay thế outputmain cũ thành giá trị mới để lúc save quay về trang main hiện giá trị sau lúc save
    if (indexOutput !== -1) {
      listOutputM[indexOutput] = this.output;
    }
    //// --------

    // thay đổi giá trị result1 sau khi output lần đầu nếu output chưa hết bằng giá trị cũ trừ cho giá trị đã output ra
    this.result1.forEach((element, i) => {
      element.value = element.value - this.result3[i].value;
    });
    this.outputService.changeListMaterialSheetSize(this.result1);

    //// -------- tạo biến lưu danh sách transactiondetail có giá trị thay đổi mới sau khi output để gửi lên server lưu db
    const tmpTransactionDetails = [];
    // chạy từng phần tử rồi output từng phần tử ra theo sheetno(đơn hàng) sao cho không có cái nào nhỏ hơn 0
    this.result3.forEach((i) => {
      this.result2.forEach((j) => {
        if (i.name === j.name) {
          j.array.forEach((k) => {
            if (i.value > k.instock_Qty) {
              const tmpInstock_Qty = k.instock_Qty;
              k.instock_Qty = 0;
              i.value = i.value - tmpInstock_Qty;
              k.qty = k.trans_Qty;
              k.trans_Qty = tmpInstock_Qty;
            } else if (i.value !== 0) {
              k.instock_Qty = k.instock_Qty - i.value;
              k.qty = k.trans_Qty;
              k.trans_Qty = i.value;
              i.value = 0;
            }
            else {
              k.trans_Qty = 0;
            }
            tmpTransactionDetails.push(k);
          });
        }
      });
    });

    //// lưu output vừa được proccess lưu vào giá trị chung qua trang output main lấy 1 list để gửi server lưu
    const tmpOutputSave = {output: this.output, transactionDetail: tmpTransactionDetails};
    this.listOutputSave.push(tmpOutputSave);
    this.outputService.changeListOutputSave(this.listOutputSave);
    //// -- ----------

    // lưu lại những biến dùng chung ở outputservice rồi chuyển lại trang main
    this.outputService.changeListOutputM(listOutputM);
    this.outputService.changeFlagFinish(true);
    this.spinnerService.hide();
    this.router.navigate(['/output/main']);
  }

  ngAfterContentChecked() {
    if(this.result1.length >0) {
      this.totalPickupQty = this.result1.map(o => o.value).reduce((a,c) => {return a + c});
    }
    if(this.result2.length > 0) {
      this.totalInStockQtyA = this.result2.map(o => o.value).reduce((a,c) => {return a + c});
    }
    if(this.result3.length > 0) {
      this.totalTransOutQtyB = this.result3.map(o => o.value).reduce((a,c) => {return a + c});
    }
    if(this.result2.length > 0 && this.result3.length > 0) {
      this.totalRemainingQty = this.totalInStockQtyA - this.totalTransOutQtyB;
    }
  }
}
