import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { OutputM } from '../../../_core/_models/outputM';
import { OutputService } from '../../../_core/_services/output.service';
import { AlertifyService } from '../../../_core/_services/alertify.service';
import { Router } from '@angular/router';
import { PackingPrintAll } from '../../../_core/_viewmodels/packing-print-all';
import { InputService } from '../../../_core/_services/input.service';
import { NgxSpinnerService } from 'ngx-spinner';
import { FormControl } from '@angular/forms';
import { debounceTime } from 'rxjs/operators';

@Component({
  selector: 'app-output-main',
  templateUrl: './output-main.component.html',
  styleUrls: ['./output-main.component.scss'],
})
export class OutputMainComponent implements OnInit {
  outputs: OutputM[] = [];
  qrCodeId = '';
  output: any = [];
  flagSubmit: boolean = false;
  flagFinish: boolean = false;
  packingPrintAll: PackingPrintAll[] = [];
  checkScanOutput: number = 1; // nếu check = 1 là output theo material sheetsize
  // nếu check = 2 là output theo collection transferform
  // nếu check = 3 là output theo sorting form (qrcodeId)
  @ViewChild('inputQrCodeId') inputQrCodeId:ElementRef;
  scanQrcode: FormControl = new FormControl();

  constructor(
    private outputService: OutputService,
    private alertify: AlertifyService,
    private inputService: InputService,
    private router: Router,
    private spinnerService: NgxSpinnerService
  ) { }

  ngOnInit() {
    this.outputService.currentListOutputM.subscribe((res) => {
      this.outputs = res;
    }).unsubscribe();
    this.outputService.currentFlagFinish.subscribe((res) => {
      this.flagFinish = res;
    }).unsubscribe();
    this.outputService.currentFlagSubmit.subscribe((res) => {
      this.flagSubmit = res;
    }).unsubscribe();
    this.outputService.currentCheckScanOutput.subscribe((res) => {
      this.checkScanOutput = res;
    }).unsubscribe();
    this.inputService.clearDataChangeMenu();
    this.getOutputMain();
  }

  getOutputMain() {
    this.scanQrcode.valueChanges.pipe(debounceTime(500)).subscribe((valueInput) => {
      if (valueInput) {
        this.qrCodeId = valueInput.toString().toUpperCase();
        ///////////////////
        if (this.checkScanOutput === 1) {
          if (this.qrCodeId.length == 10) {
            this.spinnerService.show();
            this.outputService.getMainByQrCodeId(this.qrCodeId).subscribe(
              (res) => {
                if (res != null) {
                  if (res.outputs.length === 0) {
                    this.outputs = [];
                    this.alertify.error(res.message);
                  }
                  else {
                    // reset lại data phòng trường hợp người dùng không clear trước khi quét mã mới, trong khi đã quét mã cũ rồi
                    this.resetData();
    
                    this.outputs = res.outputs;
                    this.outputService.changeListOutputM(this.outputs);
    
                    // Group by materialsheetsize theo tool_Size rồi gán vào listmaterialsheetsize trong output service để dùng chung
                    const groups = new Set(
                      res.outputTotalNeedQty.map((item) => item.tool_Size)
                    ),
                      results = [];
                    groups.forEach((g) =>
                      results.push({
                        name: g,
                        value: res.outputTotalNeedQty
                          .filter((i) => i.tool_Size === g)
                          .reduce((qty, j) => {
                            return (qty += j.qty);
                          }, 0),
                      })
                    );
                    this.outputService.changeListMaterialSheetSize(results);
                  }
                }
                this.spinnerService.hide();
              },
              (error) => {
                this.spinnerService.hide();
                this.alertify.error(error);
              }
            );
            this.outputService.changeFlagSubmit(false);
            this.flagFinish = false;
            this.scanQrcode.reset();
          }
        } else if (this.checkScanOutput === 2) {
          if (this.qrCodeId.length == 17) {
            this.spinnerService.show();
            this.outputService.getMainByQrCodeIdByCollectionTransferForm(this.qrCodeId).subscribe(
              (res) => {
                if (res != null) {
                  if (res.outputs.length === 0) {
                    this.outputs = [];
                    this.alertify.error(res.message);
                  }
                  else {
                    // reset lại data phòng trường hợp người dùng không clear trước khi quét mã mới, trong khi đã quét mã cũ rồi
                    this.resetData();
    
                    this.outputs = res.outputs;
                    this.outputService.changeListOutputM(this.outputs);
    
                    // Group by materialsheetsize theo tool_Size rồi gán vào listmaterialsheetsize trong output service để dùng chung
                    const groups = new Set(
                      res.outputTotalNeedQty.map((item) => item.tool_Size)
                    ),
                      results = [];
                    groups.forEach((g) =>
                      results.push({
                        name: g,
                        value: res.outputTotalNeedQty
                          .filter((i) => i.tool_Size === g)
                          .reduce((qty, j) => {
                            return (qty += j.qty);
                          }, 0),
                      })
                    );
                    this.outputService.changeListMaterialSheetSize(results);
                  }
                }
                this.spinnerService.hide();
              },
              (error) => {
                this.spinnerService.hide();
                this.alertify.error(error);
              }
            );
            this.outputService.changeFlagSubmit(false);
            this.flagFinish = false;
            this.scanQrcode.reset();
          }
        } else {
          if (this.qrCodeId.length >= 15) {
            this.spinnerService.show();
            this.outputService.getMainByQrCodeIdBySortingForm(this.qrCodeId).subscribe(
              (res) => {
                if (res != null) {
                  if (res.outputs.length === 0) {
                    this.outputs = [];
                    this.alertify.error(res.message);
                  }
                  else {
                    // reset lại data phòng trường hợp người dùng không clear trước khi quét mã mới, trong khi đã quét mã cũ rồi
                    this.resetData();
    
                    this.outputs = res.outputs;
                    this.outputService.changeListOutputM(this.outputs);
    
                    // Group by materialsheetsize theo tool_Size rồi gán vào listmaterialsheetsize trong output service để dùng chung
                    const groups = new Set(
                      res.outputTotalNeedQty.map((item) => item.tool_Size)
                    ),
                      results = [];
                    groups.forEach((g) =>
                      results.push({
                        name: g,
                        value: res.outputTotalNeedQty
                          .filter((i) => i.tool_Size === g)
                          .reduce((qty, j) => {
                            return (qty += j.qty);
                          }, 0),
                      })
                    );
                    this.outputService.changeListMaterialSheetSize(results);
                  }
                }
                this.spinnerService.hide();
              },
              (error) => {
                this.spinnerService.hide();
                this.alertify.error(error);
              }
            );
            this.outputService.changeFlagSubmit(false);
            this.flagFinish = false;
            this.scanQrcode.reset();
          }
        }

      }
    });
  }

  detail(output: OutputM, index: number) {
    if (this.flagSubmit === true) {
      this.router.navigate(['output/detail', output.transacNo]);
    } else {
      this.router.navigate(['output/detail-preview', index]);
    }
  }

  process(output: OutputM) {
    this.outputService.changeOutputM(output);

    this.router.navigate(['output/process']);
  }

  print(qrCodeId: string, qrCodeVersion: number, moSeq: string) {
    this.outputService.changePrintQrCode('2');
    const paramPrintQrCodeAgain = [{
      qrCode_ID: qrCodeId,
      qrCode_Version: qrCodeVersion,
      mO_Seq: moSeq
    }];
    this.outputService.changeParamPrintQrCodeAgain(paramPrintQrCodeAgain);
    this.router.navigate(['/output/print-qrcode-again']);
  }

  submit() {
    this.spinnerService.show();
    // flag submit == false
    this.flagSubmit = true;
    this.outputService.changeFlagSubmit(true);
    // kiểm tra output nào mà không output ra(không process) thì loại bỏ
    this.outputs.forEach((e, i) => {
      if (e.transOutQty === 0) {
        this.outputs.splice(i, 1);
      }
    });

    //// gửi lên server lưu 1 list output
    let tmpListOutputSave: any = [];
    this.outputService.currentListOutputSave.subscribe(res => {
      tmpListOutputSave = res;
    }).unsubscribe();
    this.outputService
      .saveListOutput(tmpListOutputSave)
      .subscribe(
        () => {
          this.spinnerService.hide();
          this.alertify.success('Save succeed');
        },
        (error) => {
          this.alertify.error(error);
          this.spinnerService.hide();
        }
      );
    //// ---------

    // gán lại mấy giá trị dùng chung trên service thành giá trị mặc định ban đầu để ouput đơn khác không bị nhớ ouput cũ
    this.clear();
    this.flagFinish = false;
  }
  cancel() {
    this.clear();
    this.outputService.changeListOutputM([]);
    this.outputs = [];
    this.scanQrcode.reset();
  }
  upperCase() {
    this.qrCodeId = this.qrCodeId.toUpperCase();
  }
  resetData() {
    this.clear();
    this.outputs = [];
  }
  clear() {
    this.outputService.changeListMaterialSheetSize([]);
    this.outputService.changeFlagFinish(false);
    this.outputService.changeListOutputSave([]);
    this.outputService.changeOutputM({});
  }

  changeCheck(e, number) {
    this.resetData();
    this.inputQrCodeId.nativeElement.focus();
    this.scanQrcode.reset();
    this.checkScanOutput = number;
    this.outputService.changeCheckScanOutput(this.checkScanOutput);
  }
}
