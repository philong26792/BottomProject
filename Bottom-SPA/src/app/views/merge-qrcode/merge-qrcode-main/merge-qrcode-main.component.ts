import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Select2OptionData } from 'ng-select2';
import { NgxSpinnerService } from 'ngx-spinner';
import { fromEvent } from 'rxjs';
import { debounceTime, distinctUntilChanged, map, switchMap } from 'rxjs/operators';
import { AlertifyService } from '../../../_core/_services/alertify.service';
import { MergeQrcodeService } from '../../../_core/_services/merge-qrcode.service';
import { PackingListService } from '../../../_core/_services/packing-list.service';
import { FunctionUtility } from '../../../_core/_utility/function-utility';
import { MaterialInformation } from '../../../_core/_viewmodels/merge-qrcode/material-information';
import { MergeQrCodeModel } from '../../../_core/_viewmodels/merge-qrcode/merge-qrcode-model';

@Component({
  selector: 'app-merge-qrcode-main',
  templateUrl: './merge-qrcode-main.component.html',
  styleUrls: ['./merge-qrcode-main.component.css']
})
export class MergeQrcodeMainComponent implements OnInit {
  mO_No: string = '';
  materialNo: string = 'All';
  materialListDB: MaterialInformation[] = [];
  materialList: Array<Select2OptionData>;
  materialName: string = '';
  supplier_ID: string = 'All';
  supplierList: Array<Select2OptionData>;
  disable = false;
  checkAll = false;
  optionsSelectSupplier = {
    placeholder: "Select supplier...",
    allowClear: true,
    width: "100%",
  };
  optionsSelectMaterial = {
    placeholder: "Select material...",
    allowClear: true,
    width: "100%",
  };
  dataResult: MergeQrCodeModel[] = [];
  alerts: any = [
    {
      type: "success",
      msg: `You successfully read this important alert message.`,
    },
    {
      type: "info",
      msg: `This alert needs your attention, but it's not super important.`,
    },
    {
      type: "danger",
      msg: `Better check yourself, you're not looking too good.`,
    },
  ];
  constructor(private packingListService: PackingListService,
              private mergeQrCodeService: MergeQrcodeService,
              private functionUtility: FunctionUtility,
              private spinner: NgxSpinnerService,
              private router: Router,
              private alertify: AlertifyService) { }

  ngOnInit(): void {
    this.getListSupplier();
    this.getListMaterial();
  }
  getListSupplier() {
    this.packingListService.supplierList().subscribe((res) => {
      this.supplierList = res.map((obj) => {
        return {
          id: obj.supplier_No,
          text: obj.supplier_No + "-" + obj.supplier_Name,
        };
      });
      this.supplierList.unshift({
        id: "All",
        text: "All Supplier",
      });
    });
  }
  getListMaterial() {
    let monoChange = document.getElementById('mO_No');
    fromEvent(monoChange, 'keyup').pipe(
      debounceTime(500),
      distinctUntilChanged(),
      switchMap(data => this.mergeQrCodeService.getMaterialInformationByPo(this.mO_No))
    ).subscribe((result) => {
      this.materialListDB = result;
      this.materialList = result.map(obj => {
            return {
              id: obj.material_ID,
              text: obj.material_ID
            };
          });
          this.materialList.unshift({
            id: "All",
            text: "All Material",
          });
    }, error => {
      this.alertify.error(error);
    })
  }
  changedSupplier(e: any): void {
    this.supplier_ID = e;
  }
  changeMaterial(e: any): void {
    this.materialNo = e;
    if(this.materialNo === 'All') {
      this.materialName = '';
    } else {
      let materialInfo = this.materialListDB.find(x => x.material_ID === e);
      this.materialName = materialInfo.material_Name;
    }
  }
  search() {
      if(this.functionUtility.checkEmpty(this.mO_No)) {
        this.alertify.error('Please enter Plan No!!!');
      } else {
        this.spinner.show();
        let param = {
          mO_No: this.mO_No,
          supplier_ID: this.supplier_ID,
          material_ID: this.materialNo
        }
        this.mergeQrCodeService.searchOfMerge(param).subscribe(res => {
          this.dataResult = res;
          this.spinner.hide();
        }, error => {
          this.alertify.error(error);
          this.spinner.hide();
        })
      }     
  }

  mergeQRCode() {
    let dataMaterial = Array.from(new Set(this.dataResult.map(x => x.material_ID)));
    let dataRack = Array.from(new Set(this.dataResult.map(x => x.rack_Location)));
    if(dataMaterial.length > 1) {
      this.alertify.error('Must be the same Material!!!');
    } else {
      if(dataRack.length > 1) {
        this.alertify.error('Must be the same Rack!!!');
      } else {
        this.spinner.show();
        this.mergeQrCodeService.mergeQrCode(this.dataResult).subscribe(res => {
          this.mergeQrCodeService.changeListQrCodeAfterMerge(res);
          this.spinner.hide();
          this.alertify.success('Merge Data successfully!');
          this.router.navigate(['/merge-qrcode/after']);
        }, error => {
          this.spinner.hide();
          this.alertify.error(error);
        });
      }
    }    
  }
}
