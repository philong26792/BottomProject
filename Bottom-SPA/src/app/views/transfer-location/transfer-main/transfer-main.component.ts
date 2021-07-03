import { Component, OnInit } from '@angular/core';
import { AlertifyService } from '../../../_core/_services/alertify.service';
import { TransferService } from '../../../_core/_services/transfer.service';
import { TransferM } from '../../../_core/_models/transferM';
import { FunctionUtility } from '../../../_core/_utility/function-utility';
import { InputService } from '../../../_core/_services/input.service';
import { NgxSpinnerService } from 'ngx-spinner';
import { FormControl } from '@angular/forms';
import { debounceTime } from 'rxjs/operators';

@Component({
  selector: 'app-transfer-main',
  templateUrl: './transfer-main.component.html',
  styleUrls: ['./transfer-main.component.scss'],
})
export class TransferMainComponent implements OnInit {
  transfers: TransferM[] = [];
  qrCodeId = '';
  toLocation = '';
  transferNo = '';
  flagSubmit = false;
  scanQrcode: FormControl = new FormControl();

  constructor(
    private transferService: TransferService,
    private alertify: AlertifyService,
    private inputService: InputService,
    private functionUtility: FunctionUtility,
    private spinnerService: NgxSpinnerService
  ) { }

  ngOnInit() {
    this.inputService.clearDataChangeMenu();
    this.getTransferMain();
  }
  enter() {
    document.getElementById('qrCodeId').focus();
  }
  getTransferMain() {
    this.scanQrcode.valueChanges.pipe(debounceTime(500)).subscribe((valueInput) => {
      if (valueInput && valueInput.length >= 15) {
        this.qrCodeId = this.scanQrcode.value;
        // nếu chưa nhập to location thì thông báo lên lỗi
        if (this.toLocation === '') {
          this.alertify.error('Please Scan To Location!');
          this.qrCodeId = '';
          return;
        }
        let flag = true;
        this.transfers.forEach((item) => {
          if (item.qrCodeId === this.qrCodeId.trim()) {
            flag = false;
          }
        });
        if (flag) {
          this.spinnerService.show();
          this.transferService.getMainByQrCodeId(this.qrCodeId).subscribe(
            async (res) => {
              // nếu có dữ liệu thì mới thêm ngược lại qrcodeid đó chưa được input nên thông báo lỗi
              if (res != null) {
                // nếu from location giống to location thì thông báo lỗi, và to location có cùng area với from location ko
                if (res.fromLocation === this.toLocation) {
                  this.alertify.error('To Location same From Location!');
                  this.spinnerService.hide();
                }
                else {
                  // lấy ra transferNo mới theo yêu cầu: TB(ngày thực hiện yyyymmdd) 3 mã số random number. (VD: TB20200310001)
                  res.transferNo = await this.functionUtility.getTransferNo();
                  res.toLocation = this.toLocation;
                  this.transfers.push(res);
                  this.spinnerService.hide();
                }
              }
              else {
                this.alertify.error('This QRCode has not been input yet!');
                this.spinnerService.hide();
              }
            },
            (error) => {
              this.alertify.error(error);
              this.spinnerService.hide();
            }
          );
        } else {
          this.alertify.error('This QRCode scanded!');
        }
        this.scanQrcode.reset();
      }
    });
  }

  remove(qrCodeId: string) {
    this.alertify.confirm('Delete', 'Are you sure Delete', () => {
      this.transfers.forEach((e, i) => {
        if (e.qrCodeId === qrCodeId) {
          this.transfers.splice(i, 1);
        }
      });
    });
  }
  uppercase() {
    this.toLocation = this.toLocation.toUpperCase();
  }
  submitMain() {
    this.spinnerService.show();
    this.flagSubmit = true;
    this.transferService.submitMain(this.transfers).subscribe(
      () => {
        this.scanQrcode.reset();
        this.toLocation = '';
        this.alertify.success('Submit succeed');
        this.spinnerService.hide();
      },
      (error) => {
        this.alertify.error(error);
        this.spinnerService.hide();
      }
    );
  }

  clear() {
    this.scanQrcode.reset();
    this.toLocation = '';
    this.transfers = [];
    this.flagSubmit = false;
    document.getElementById('toLocation').removeAttribute('disabled');
  }

  // khi rời chuột khỏi ô to location  thì check xem to location đó có đúng ko
  checkExistLocation() {
    // kiểm tra tolocation có toàn khoảng trắng hay không
    if (this.functionUtility.checkEmpty(this.toLocation)) {
      this.toLocation = '';
      document.getElementById('toLocation').focus();
      this.alertify.error('Rack Location does not exist please scan again!');
    } else {
        this.transferService.checkExistLocation(this.toLocation).subscribe(res => {
          if (res === false) {
            this.toLocation = '';
  
            document.getElementById('toLocation').focus();
            this.alertify.error('Rack Location does not exist please scan again!');
          }
          else {
            document.getElementById('toLocation').setAttribute('disabled', 'disabled');
          }
        });
    }
  }
}
