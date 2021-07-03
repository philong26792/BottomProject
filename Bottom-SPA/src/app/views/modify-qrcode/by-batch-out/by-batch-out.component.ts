import { Component, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { NgxSpinnerService } from "ngx-spinner";
import { ModifyQrCodeMain } from "../../../_core/_models/modify-qrcode-main";
import {
  SizeInStockPlanQty,
  SizeInstockQty,
  SizeInstockQtyByBatch,
} from "../../../_core/_models/size-instockqty-bybath";
import { AlertifyService } from "../../../_core/_services/alertify.service";
import { ModifyQrcodeService } from "../../../_core/_services/modify-qrcode.service";
import { FunctionUtility } from "../../../_core/_utility/function-utility";
import {
  ModifyReasonQtyModel,
  QtyOfSize,
} from "../../../_core/_viewmodels/modify-reason-qty-model";
import {
  BatchDetail,
  LeftRightInBatchOfReason,
  ReasonDetail,
} from "../../../_core/_viewmodels/modify-store/reason-detail";

@Component({
  selector: "app-by-batch-out",
  templateUrl: "./by-batch-out.component.html",
  styleUrls: ["./by-batch-out.component.css"],
})
export class ByBatchOutComponent implements OnInit {
  data2DB: SizeInstockQtyByBatch[] = []; // giá trị bên API trả lên
  data2DB_L1: SizeInstockQtyByBatch[] = [];
  data2DB_L2: SizeInstockQtyByBatch[] = [];
  data2DB_L3: SizeInstockQtyByBatch[] = [];
  modifyQrCodeMain: ModifyQrCodeMain;
  deliveryNos: string[] = [];
  data1: SizeInstockQty[] = []; // để lấy toolsize, othersize, planqty của từng phần tử
  data1Const: SizeInstockQty[] = []; // giá trị ban đầu của list đó
  data2: SizeInstockQtyByBatch[] = []; // giá trị sau khi thay đổi
  stockQtyATotal: number;
  actualQtyTotal: number;
  modifyQtyTotal: number;
  modifyQtyList: number[] = [];
  checkMissing = true;
  modifyType: number = 1;
  listBatch: string[] = [];
  batch: string = "";
  typeOther = 1;
  reasonDataList: ModifyReasonQtyModel[] = [];
  leftRightInBatch: LeftRightInBatchOfReason[] = [];
  constructor(
    private modifyQrCodeService: ModifyQrcodeService,
    private funtionUtility: FunctionUtility,
    private spinner: NgxSpinnerService,
    private alertify: AlertifyService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.modifyQrCodeService.currentModifyQrCodeMain.subscribe((res) => {
      if (res == null) {
        this.router.navigate(["/modify-store/main"]);
      } else {
        this.modifyQrCodeMain = res;
      }
    });
    this.getDetail();
  }
  getDetail() {
    this.modifyQrCodeService.getDetailModifyQrCode(this.modifyQrCodeMain.mO_No,this.modifyQrCodeMain.material_ID).subscribe((res) => {
        this.data2DB = res.data2;
        // gán thành 3 biến mới để tạo ra vùng nhớ mới, khi thay đổi giá trị không ảnh hưởng đến biến khác
        // vì có nhiều biến trỏ đến res.data2
        this.data2DB_L1 = JSON.parse(JSON.stringify(res.data2));
        this.data2DB_L2 = JSON.parse(JSON.stringify(res.data2));
        this.data2DB_L3 = JSON.parse(JSON.stringify(res.data2));
        this.deliveryNos = res.deliveryNos;

        /////////////tách ra batch
        this.listBatch = this.data2DB.map((item) => {
          return item.mO_Seq;
        });
        this.batch = this.listBatch[0];

        // data2 là 1 mảng length = 1 vì chỉ chứa 1 batch
        this.data2 = this.data2DB_L1.filter((item) => {
          return item.mO_Seq == this.batch;
        });

        this.data1 = this.data2DB_L2.filter((item) => {
          return item.mO_Seq == this.batch;
        })[0].dataDetail;
        this.data1Const = this.data2DB_L3.filter((item) => {
          return item.mO_Seq == this.batch;
        })[0].dataDetail;
        this.getReasonOfSupplier(this.modifyQrCodeMain.supplier_ID);
      });
  }
  getReasonOfSupplier(supplier: string) {
    this.reasonDataList.length = 0;
    this.modifyQrCodeService.getReasonOfSupplier(supplier).subscribe((res) => {
      res.forEach((x) => {
        let reasonModel: ModifyReasonQtyModel = {
          reason_Code: x.reason_Code.trim(),
          reason_Name: x.reason_Name.trim(),
          qtyOfSizes: this.data1.map((item) => {
            return {
              tool_Size: item.tool_Size,
              order_Size: item.order_Size,
              qty: 0,
            };
          }),
          qtyOfSizesLeft: this.data1.map((item) => {
            return {
              tool_Size: item.tool_Size,
              order_Size: item.order_Size,
              qty: 0,
            };
          }),
          qtyOfSizesRight: this.data1.map((item) => {
            return {
              tool_Size: item.tool_Size,
              order_Size: item.order_Size,
              qty: 0,
            };
          }),
          totalModifyQty: 0,
          totalLeft: 0,
          totalRight: 0,
        };
        this.reasonDataList.push(reasonModel);
      });
      console.log(this.reasonDataList);
    });
  }
  shareLeftRight() {
    this.leftRightInBatch.length = 0;
    this.data1Const.forEach((element1, i) => {
      debugger
      // Nếu size đó có thay đổi số lượng thì thực hiện tính toán
      if (this.modifyQtyList[i] > 0) {
        let reasons: ReasonDetail[] = [];
        let batchs: BatchDetail[] = [];
        this.reasonDataList.forEach((element2) => {
          let left = element2.qtyOfSizesLeft[i].qty;
          let right = element2.qtyOfSizesRight[i].qty;
          if (left !== 0 && right !== 0) {
            let itemReasonOfSize: ReasonDetail = {
              reason_code: element2.reason_Code,
              tool_size: element1.tool_Size,
              order_size: element1.order_Size,
              left: left,
              right: right,
            };
            reasons.push(itemReasonOfSize);
          }
        });

        // Gửi vào hàm getLeftRight để tính toán
        let qty = this.data1Const[i].totalInstockQty;
        // qty hiển thị bên ngoài phải khác 0 thì mới add vào để tính toán
        if (qty != 0) {
          let batchItemOfSize: BatchDetail = {
            batch: this.batch,
            left: qty,
            right: qty,
          };
          batchs.push(batchItemOfSize);
        }
        if(batchs.length > 0) {
          let shareLeftAndRightOfBatch = this.funtionUtility.getLeftRight(
            reasons,
            batchs
          );
          this.leftRightInBatch.push(...shareLeftAndRightOfBatch);
  
          // data2 là 1 mảng length = 1 vì chỉ chứa 1 batch
          let leftAndRight = shareLeftAndRightOfBatch
            .filter((x) => x.batch == this.batch)
            .map((x) => x.left + x.right)
            .reduce((a, c) => {
              return a + c;
            });
          this.data2[0].dataDetail[i].modifyQty = leftAndRight / 2;
        }
      } else {
        // Nếu size đó ko có thay đổi số lượng thì qty ở bên ngoài phải hiển thị như ban đầu,khi chưa giảm
        this.data2[0].dataDetail[i].totalInstockQty =
          this.data1Const[i].totalInstockQty;
        this.data2[0].dataDetail[i].modifyQty = 0;
      }
    });
  }
  changeInputReason(
    e: number,
    item1: ModifyReasonQtyModel,
    item2: QtyOfSize,
    i1: number,
    i2: number
  ) {
    item1.qtyOfSizesLeft[i2].qty = e;
    item1.qtyOfSizesRight[i2].qty = e;
  }
  changeInputLeftOrRight(i1: number, i2: number) {
    // Đang change Input ở Right
    let qtyRight = this.reasonDataList[i1].qtyOfSizesRight[i2].qty;
    let qtyLeft = this.reasonDataList[i1].qtyOfSizesLeft[i2].qty;
    this.reasonDataList[i1].qtyOfSizes[i2].qty = (qtyRight + qtyLeft) / 2;
  }
  ngAfterContentChecked() {
    if (this.data1.length > 0) {
      this.modifyQtyList.length = 0;
      let countData = this.data1Const.length;
      for (let i = 0; i < countData; i++) {
        if (
          this.data1[i].totalInstockQty > this.data1Const[i].totalInstockQty
        ) {
          this.data1[i].totalInstockQty = this.data1Const[i].totalInstockQty;
        }
      }
      this.stockQtyATotal = this.data1Const
        .map((o) => o.totalInstockQty)
        .reduce((a, c) => {
          return a + c;
        });
      this.actualQtyTotal = this.data1
        .map((o) => o.totalInstockQty)
        .reduce((a, c) => {
          return a + c;
        });

      this.data2.forEach((item) => {
        item.total = item.dataDetail
          .map((o) => o.totalInstockQty)
          .reduce((a, c) => {
            return a + c;
          });
      });

      // ------------------------------------------------------------------------------------------------------------------//
      // Hiển thị số đôi lỗi theo từng size của all các reason
      if (this.reasonDataList.length > 0) {
        for (let y = 0; y < countData; y++) {
          let modifyQtyOfSize = this.reasonDataList
            .map((x) => x.qtyOfSizes)
            .map((x) => x[y].qty)
            .reduce((a, c) => {
              return a + c;
            });
          this.modifyQtyList.push(modifyQtyOfSize);
        }
      }

      // ------------------------------------------------------------------------------------------------------------------//
      // Hiển thị tổng số đôi lỗi của all Size
      if (this.modifyQtyList.length > 0) {
        this.modifyQtyTotal = this.modifyQtyList.reduce((a, c) => {
          return a + c;
        });
      }

      // Set màu cho ô nào có thay đổi giá trị
      let countSize = this.data1Const.length;
      for (let t = 0; t < countSize; t++) {
        if (
          this.data2[0].dataDetail[t].totalInstockQty !==
          this.data1Const[t].totalInstockQty
        ) {
          this.data2[0].dataDetail[t].isChange = true;
        } else {
          this.data2[0].dataDetail[t].isChange = false;
        }
      }
      // ------------------------------------------------------------------------------------------------------------------//
      // Hiển thị tổng số chiếc Right và Left của mỗi reason
      if (this.reasonDataList.length > 0) {
        this.reasonDataList.forEach((item) => {
          item.totalRight = item.qtyOfSizesRight
            .map((x) => x.qty)
            .reduce((a, c) => {
              return a + c;
            });
          item.totalLeft = item.qtyOfSizesLeft
            .map((x) => x.qty)
            .reduce((a, c) => {
              return a + c;
            });
          item.totalModifyQty = (item.totalRight + item.totalLeft) / 2;
        });
      }

      // ------------------------------------------------------------------------------------------------------------------//
      this.data1.map(
        (x, i) =>
          (x.totalInstockQty =
            this.data1Const[i].totalInstockQty - this.modifyQtyList[i])
      );
      // ------------------------------------------------------------------------------------------------------------------//
      this.shareLeftRight();
      // ------------------------------------------------------------------------------------------------------------------//
      // Tính số lượng bị trừ của mỗi size theo batch(Mỗi ô)
      for (let n = 0; n < countSize; n++) {
        this.data2[0].dataDetail[n].totalInstockQty =
          this.data1Const[n].totalInstockQty -
          this.data2[0].dataDetail[n].modifyQty;
      }
    }
  }

  changeBatch() {
    this.data2 = this.data2DB_L1.filter((item) => {
      return item.mO_Seq == this.batch;
    });
    this.data1 = this.data2DB_L2.filter((item) => {
      return item.mO_Seq == this.batch;
    })[0].dataDetail;
    this.data1Const = this.data2DB_L3.filter((item) => {
      return item.mO_Seq == this.batch;
    })[0].dataDetail;
    this.modifyQtyList.map((x) => 0);

    // Các qty của reason khi chuyển Batch phải trả về giá trị ban đầu là 0
    this.reasonDataList.map((x) => {
      x.qtyOfSizes.map((y) => (y.qty = 0));
      x.qtyOfSizesLeft.map((y) => (y.qty = 0));
      x.qtyOfSizesRight.map((y) => (y.qty = 0));
      return x;
    });
  }

  changeInput(e, i) {
    // load lại giá trị của cột đó tương ứng order-size.
    this.data2DB.forEach((item1) => {
      this.data2.forEach((item2) => {
        if (item2.mO_Seq === item1.mO_Seq) {
          item2.dataDetail[i].totalInstockQty =
            item1.dataDetail[i].totalInstockQty;
        }
      });
    });
    if (e > this.data1Const[i].totalInstockQty) {
      this.data1[i].totalInstockQty = this.data1Const[i].totalInstockQty;
      this.data2[0].dataDetail[i].totalInstockQty =
        this.data1Const[i].totalInstockQty;
      (<HTMLInputElement>(
        document.getElementById(this.data1Const[i].order_Size)
      )).value = this.data1Const[i].totalInstockQty.toString();
    } else {
      let valueInput = e;
      let instockQtyValue = this.data2[0].dataDetail[i].totalInstockQty;
      if (instockQtyValue !== null) {
        if (valueInput <= instockQtyValue) {
          this.data2[0].dataDetail[i].totalInstockQty = valueInput;
        } else {
          this.data2[0].dataDetail[i].totalInstockQty = valueInput;
        }
      }
    }
  }

  changePageByBath() {
    if (this.modifyType == 0) {
      this.router.navigate(["/modify-store/no-by-batch-out"]);
    }
  }

  submitTable() {
    let checkError = this.modifyQtyList.some((x,i) => x > this.data1Const[i].totalInstockQty);
    if(checkError) {
      return this.alertify.error('The number of pairs out has exceeded the allowed limit!');
    }
    // Check tổng số lượng thay đổi khi nào > 0 thì mới cho phép save data
    if (this.modifyQtyTotal == 0) {
      return this.alertify.error("Please change value input!");
    }
    //this.spinner.show();
    let param = {
      model: this.modifyQrCodeMain,
      data: this.data2,
      reason_code: '',
      isMissing: this.checkMissing,
      reasonDetail: this.leftRightInBatch
    };
    console.log(param);
    // this.modifyQrCodeService.saveNoByBatch(param).subscribe(
    //   (res) => {
    //     this.modifyQrCodeService.changeModifyQrCodeAfterSave(res);
    //     this.modifyQrCodeService.changeOtherType("Other Out");
    //     this.router.navigate(["/modify-store/list-qrcode-change"]);
    //     this.spinner.hide();
    //   },
    //   (error) => {
    //     this.alertify.error(error);
    //     this.spinner.hide();
    //   }
    // );
  }
  changeOtherType() {
    this.router.navigate(["/modify-store/by-batch-in"]);
  }
}
