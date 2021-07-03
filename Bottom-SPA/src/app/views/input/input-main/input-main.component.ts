import { Component, OnInit } from '@angular/core';
import { AlertifyService } from '../../../_core/_services/alertify.service';
import { Router } from '@angular/router';
import { InputService } from '../../../_core/_services/input.service';
import { InputDetail } from '../../../_core/_models/input-detail';
import { PackingPrintAll } from '../../../_core/_viewmodels/packing-print-all';
import { TransferService } from '../../../_core/_services/transfer.service';
import { NgxSpinnerService } from 'ngx-spinner';
import { FunctionUtility } from '../../../_core/_utility/function-utility';
import { OutputService } from '../../../_core/_services/output.service';
import { FormControl } from '@angular/forms';
import { debounceTime } from 'rxjs/operators';

@Component({
  selector: 'app-input-main',
  templateUrl: './input-main.component.html',
  styleUrls: ['./input-main.component.scss']
})
export class InputMainComponent implements OnInit {
  result: any = [];
  resultDetail: InputDetail;
  listInputNo: any = [];
  qrCodeID = "";
  rackLocation = "";
  err = true;
  checkSubmit: boolean;
  checkV696: boolean;
  checkInputRack: boolean;
  qrCodeIdCurrent = '';
  packingPrintAll: PackingPrintAll[] = [];
  scanQrcode: FormControl = new FormControl();
  
  constructor(private inputService: InputService,
              private outputService: OutputService,
              private transferService: TransferService,
              private alertify: AlertifyService,
              private router: Router,
              private spinerService: NgxSpinnerService,
              private functionUtility: FunctionUtility) { }

  ngOnInit() {
    this.inputService.currentCheckSubmit.subscribe(res => this.checkSubmit = res);
    this.inputService.currentFlag.subscribe(flag => this.rackLocation = flag);
    this.inputService.currentListInputMain.subscribe(listInputMain => this.result = listInputMain);
    this.inputService.currentInputRackLocation.subscribe(res => this.checkInputRack = res);
    this.getInputMain();
  }

  // Sự kiện enter khi nhập xong input rackloction
  enter() {
    document.getElementById("scanQrCodeId").focus();
  }
  getMainByQrCodeID(qrCodeId: string) {
    this.inputService.getMainByQrCodeID(qrCodeId)
      .subscribe((res) => {
        if (res === null) {
          this.alertify.error('Does not exist QRCodeID!!!');
        } else if (res.is_Scanned === 'Y') {
          this.alertify.error('This qrCode has been scanned!!!');
        } else {
          if (res != null) {
            this.result.push(res);
            this.result.forEach(element => {
              if (element.qrCode_Id.trim() === qrCodeId) {
                element.rack_Location = this.rackLocation;
                element.inStock_Qty = element.accumated_Qty;
                element.trans_In_Qty = element.accumated_Qty;
                // thêm thuộc tính listAccumatedQty vào từng phần tử để khi mà submit xong rồi ấn detail nhớ cái cũ
                element.listAccumatedQty = element.detail_Size.map(e => {
                  return e.qty;
                });
              }
            });
          }
        }
      }, error => {
        this.alertify.error(error);
      });
  }
  printMiss(qrCode: string) {
    this.inputService.findMiss(qrCode).subscribe(res => {
      this.inputService.changeMissingPrint('1');
      let missingNo = res.missingNo;
      this.router.navigate(['/input/missing-print/', missingNo]);
    }, error => {
      this.alertify.error(error);
    })
  }
  upperCase() {
    this.rackLocation = this.rackLocation.toUpperCase();
  }
  getInputMain() {
    this.scanQrcode.valueChanges.pipe(debounceTime(500)).subscribe((valueInput) => {
      if (valueInput && valueInput.length >= 15) {
        this.checkSubmit = false;
        if (this.rackLocation === "") {
          this.alertify.error("Please Scan Rack Location!");
          document.getElementById("rackLoca").focus();
          this.scanQrcode.reset();
          this.qrCodeIdCurrent = '';
          (<HTMLInputElement>document.getElementById('scanQrCodeId')).value = '';
        } else {
          let flag = true;
          this.result.forEach(item => {
            if (item.qrCode_Id === valueInput)
              flag = false;
          });
          if (flag) {
            this.checkV696 = false;
            this.qrCodeIdCurrent = valueInput.toString().trim();
            this.inputService.checkQrCodeIdV696(this.qrCodeIdCurrent).subscribe(res => {
              // Nếu QrCodeID không thuộc V696
              if (!res.result) {
                this.inputService.checkRacLocation(this.rackLocation).subscribe(res1 => {
                  // Nếu Rack không thuộc A012 thì input bình thường
                  if (!res1.result) {
                    this.getMainByQrCodeID(this.qrCodeIdCurrent);
                  } else {
                    // Nếu Rack thuộc A012 thì ko đc input,và báo lỗi cho người dùng nhập lại
                    this.alertify.error('Please enter QRCode of RB factory');
                    document.getElementById("scanQrCodeId").focus();
                  }
                });
              }
              // Nếu QrCodeID Có thuộc V696
              else {
                this.checkV696 = true;
                this.inputService.checkRacLocation(this.rackLocation).subscribe(res => {
                  // Nếu RackLocation không thuộc A012 thì báo lỗi và yêu cầu nhập lại
                  if (!res.result) {
                    this.alertify.error('Please enter QRCode not of RB factory!');
                    document.getElementById('scanQrCodeId').focus();
                  } else {
                    // Nếu RackLocation thuộc A012 thì xử lý bình thường
                    this.getMainByQrCodeID(this.qrCodeIdCurrent);
                  }
                })
              }
            });
          } else
            this.alertify.error("This QRCode scanded!");
          this.scanQrcode.reset();
        }
      }
    });
  }
  printQrCode(qrCodeId: string) {
    this.outputService.changePrintQrCode('4');
    this.inputService.findQrCodePrint(qrCodeId).subscribe(res => {
      let qrCode = [];
      qrCode.push({
        qrCode_ID: qrCodeId,
        qrCode_Version: res.qrCode_Version,
        mO_Seq: res.mO_Seq
      });
      this.outputService.changeParamPrintQrCodeAgain(qrCode);
      this.router.navigate(['/output/print-qrcode-again']);
    });
  }
  getDetailByQRCode(inputDetail: any) {
    this.inputService.changeListInputMain(this.result);
    this.inputService.changeInputDetail(inputDetail);
    this.inputService.changeFlag(this.rackLocation);
    this.inputService.changeListAccumatedQty(inputDetail.listAccumatedQty);
    this.router.navigate(["/input/print"]);
  }

  remove(qrCode_Id: string) {
    this.result.forEach((e, i) => {
      if (e.qrCode_Id === qrCode_Id) {
        this.result.splice(i, 1);
      }
    });
  }

  async submitInput() {
    this.spinerService.show();
    var listInputNo = [];
    for (const element of this.result) {

      element.input_No = await this.functionUtility.getInputNo(element.plan_No, listInputNo);
      listInputNo.push(element.input_No);
    }
    this.result.forEach((e, i) => {
      if (e.input_No == null)
        this.err = false;
      else
        this.listInputNo.push(e.input_No);
    });
    console.log(listInputNo);
    if (this.err) {
      // Submit Input
      let inputModel = {
        transactionList: this.result,
        inputNoList: this.listInputNo
      }
      this.inputService.submitInputMain(inputModel).subscribe(
        () => {
          this.rackLocation = '';
          // this.result = [];
          this.alertify.success("Submit succeed");
          this.err = false;
          this.checkSubmit = true;
          this.inputService.changeCheckSubmit(this.checkSubmit);
          this.spinerService.hide();
        },
        error => {
          this.alertify.error(error);
          this.spinerService.hide();
        }
      )
    } else {
      this.alertify.error("error");
      this.spinerService.hide();
    }
  }

  // Check Rack Location khi rời chuột khỏi ô input
  checkExistLocation() {
    // kiểm tra tolocation có toàn khoảng trắng hay null hay undefined
    if (this.functionUtility.checkEmpty(this.rackLocation)) {
      this.rackLocation = '';
      document.getElementById('rackLoca').focus();
      this.alertify.error('Rack Location does not exist please scan again!');
    } else {
      this.transferService.checkExistLocation(this.rackLocation.trim()).subscribe(res => {
        if (res === false) {
          this.rackLocation = '';
          document.getElementById('rackLoca').focus();
          this.alertify.error('Rack Location does not exist please scan again!');
        } else {
          this.checkInputRack = true;
          this.inputService.changeInputRackLocation(true);
        }
      });
    }
  }
  clearData() {
    document.getElementById('rackLoca').focus();
    (<HTMLInputElement>document.getElementById('scanQrCodeId')).value = '';
    this.rackLocation = '';
    this.qrCodeID = '';
    this.qrCodeIdCurrent = '';
    this.checkInputRack = false;
    this.checkSubmit = false;
    this.result.length = 0;
    this.inputService.changeInputRackLocation(false);
    this.inputService.clearDataChangeMenu();
  }
}
