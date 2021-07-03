import { Component, OnInit, TemplateRef } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { NgxSpinnerService } from 'ngx-spinner';
import { SplitDataByOffset } from '../../../_core/_models/merge-qrcode/split-data-by-Offset';
import { SplitProcess } from '../../../_core/_models/merge-qrcode/split-process';
import { AlertifyService } from '../../../_core/_services/alertify.service';
import { MergeQrcodeService } from '../../../_core/_services/merge-qrcode.service';
import { TransferService } from '../../../_core/_services/transfer.service';
import { FunctionUtility } from '../../../_core/_utility/function-utility';

@Component({
  selector: 'app-split-process',
  templateUrl: './split-process.component.html',
  styleUrls: ['./split-process.component.scss']
})
export class SplitProcessComponent implements OnInit {
  transacNo: string = '';
  offsetNo: string = 'all';
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
  splitDataByOffsetDetail: SplitDataByOffset = {
    mO_No: '',
    dMO_No: '',
    mO_Seq: '',
    material_ID: '',
    rack_Location: '',
    listSizeAndQty: [],
    checked: false,
    sumInstockQty: 0,
    plan_Start_STF: new Date(),
    crd: new Date(),
    sumMOQty: 0,
    sumAlreadyOffsetQty: 0,
    sumOffsetQty: 0,
  };
  moNoDetail: string = '';
  modalRef: BsModalRef | null;

  constructor(private route: ActivatedRoute,
    private router: Router,
    private spinner: NgxSpinnerService,
    private alertify: AlertifyService,
    private transferService: TransferService,
    private functionUtility: FunctionUtility,
    private mergeQrcodeService: MergeQrcodeService,
    private modalService: BsModalService) {
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
      this.getDataSplitByOffset();

      this.splitProccess.sumInstockQty = this.splitProccess.listSizeAndQty.reduce((sumInstockQty, item) => {
        return sumInstockQty += item.instock_Qty;
      }, 0);
      this.splitProccess.sumAccQty = this.splitProccess.listSizeAndQty.reduce((sumAccQty, item) => {
        return sumAccQty += item.act_Out_Qty;
      }, 0);
    }, error => {
      this.spinner.hide();
    });
  }

  getDataSplitByOffset() {
    this.spinner.show();
    this.mergeQrcodeService.getDataSplitByOffsetNo(this.offsetNo, this.splitProccess.transacMainMergeQrCode.material_ID, this.splitProccess.transacMainMergeQrCode.mO_No, this.transacNo)
      .subscribe(res => {
        this.listSplitDataByOffset = res;
        this.spinner.hide();

        this.listSplitDataByOffset.forEach(element => {
          element.sumInstockQty = element.listSizeAndQty.reduce((sumInstockQty, item) => {
            return sumInstockQty += item.instock_Qty;
          }, 0);
        });
      }, error => {
        this.spinner.hide();
      })
  }

  deleteItemDateSplitByOffset(index: number) {
    this.alertify.confirm('Delete', 'Do you want remove it', () => {
      this.listSplitDataByOffset.splice(index, 1);
    });
  }

  submit() {
    const listSplitDataByOffsetTooSubmit = this.listSplitDataByOffset.filter(x => x.checked == true);
    if (listSplitDataByOffsetTooSubmit.length == 0) {
      this.alertify.error('Please choose mono to split');
      return;
    }
    let racks = listSplitDataByOffsetTooSubmit.filter(x => this.functionUtility.checkEmpty(x.rack_Location));
    if (racks.length > 0) {
      this.alertify.error('Please enter all Rack Location');
      return;
    }
    let sumInstockQtyEquaZero = listSplitDataByOffsetTooSubmit.filter(x => x.sumInstockQty == 0);
    if (sumInstockQtyEquaZero.length > 0) {
      this.alertify.error('Please choose mono split lager 0');
      return;
    }
    this.spinner.show();
    this.mergeQrcodeService.saveDataSplit(listSplitDataByOffsetTooSubmit).subscribe(res => {
      if (res) {
        this.alertify.success('Save success!');
      } else {
        this.alertify.error('Error save server!');
      }
      this.spinner.hide();
      this.router.navigate(['/merge-qrcode/split/detail', this.transacNo]);
    }, error => {
      this.spinner.hide();
    });
  }
  checkRackExist(item: SplitDataByOffset) {
    if (item.rack_Location != null && item.rack_Location != '') {
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
  back() {
    this.router.navigate(['/merge-qrcode/split/detail', this.transacNo]);
  }

  cancel() {
    this.getData();
  }

  changeCheck(item: SplitDataByOffset) {
    item.checked = true;

    this.splitProccess.listSizeAndQty.forEach(elementParent => {
      item.listSizeAndQty.forEach(elementChild => {
        if (elementParent.tool_Size.trim() == elementChild.tool_Size.trim()
          && elementParent.model_Size.trim() == elementChild.model_Size.trim()) {
          if ((elementParent.instock_Qty - elementParent.act_Out_Qty) >= elementChild.instock_Qty) {
            elementParent.act_Out_Qty = elementParent.act_Out_Qty + elementChild.instock_Qty;
          } else {
            const tmp = elementParent.instock_Qty - elementParent.act_Out_Qty;
            elementParent.act_Out_Qty = elementParent.act_Out_Qty + tmp;
            elementChild.instock_Qty = tmp;
          }
        }
      });
    });

    this.splitProccess.sumInstockQty = this.splitProccess.listSizeAndQty.reduce((sumInstockQty, item) => {
      return sumInstockQty += item.instock_Qty;
    }, 0);
    this.splitProccess.sumAccQty = this.splitProccess.listSizeAndQty.reduce((sumAccQty, item) => {
      return sumAccQty += item.act_Out_Qty;
    }, 0);
    this.listSplitDataByOffset.forEach(element => {
      element.sumInstockQty = element.listSizeAndQty.reduce((sumInstockQty, item) => {
        return sumInstockQty += item.instock_Qty;
      }, 0);
    });
  }

  openModalShowDetail(template: TemplateRef<any>, item: SplitDataByOffset) {
    this.splitDataByOffsetDetail = item;
    this.moNoDetail = item.mO_No + item.mO_Seq;
    this.modalRef = this.modalService.show(template, { class: 'modal-lg' });
  }
}
