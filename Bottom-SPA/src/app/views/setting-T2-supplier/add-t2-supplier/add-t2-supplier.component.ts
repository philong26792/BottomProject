import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Select2OptionData } from 'ng-select2';
import { SettingReason } from '../../../_core/_models/setting-reason';
import { ReasonInfo, SettingT2Delivery } from '../../../_core/_models/setting-t2-supplier';
import { AlertifyService } from '../../../_core/_services/alertify.service';
import { PackingListService } from '../../../_core/_services/packing-list.service';
import { SettingT2SupplierService } from '../../../_core/_services/setting-t2-supplier.service';
import { commonPerFactory } from '../../../_core/_utility/common-per-factory';

@Component({
  selector: 'app-add-t2-supplier',
  templateUrl: './add-t2-supplier.component.html',
  styleUrls: ['./add-t2-supplier.component.css']
})
export class AddT2SupplierComponent implements OnInit {
  t2: SettingT2Delivery = new SettingT2Delivery();
  check: boolean;
  listSupplierDB: any[] = [];
  listReason: Array<Select2OptionData> = [];
  listDataFactory: Array<Select2OptionData> = [{ id: commonPerFactory.defaultRackFactory, text: commonPerFactory.defaultRackFactory == 'C' ? 'SHC' : commonPerFactory.defaultRackFactory }]
  factory: string = commonPerFactory.defaultRackFactory;
  input_delivery: boolean = true;
  flag: boolean = true;
  optionsSelectSupplierID = {
    placeholder: "Select Supplier...",
    allowClear: true,
    width: "100%",
  };
  reason: ReasonInfo = new ReasonInfo();
  supplierIDList: Array<Select2OptionData>;
  constructor(
    private settingT2Service: SettingT2SupplierService,
    private packingListService: PackingListService,
    private router: Router,
    private alert: AlertifyService
  ) {
    this.t2.reasons = new Array<ReasonInfo>();
    this.t2.is_Valid = 'Y';
  }
  optionsSelectReasonCode = {
    placeholder: "Select Reason...",
    allowClear: true,
    width: "100%",
  }
  ngOnInit(): void {
    this.settingT2Service.dataAddorEdit.asObservable().subscribe(res => {
      this.t2.factory_ID = this.factory;
      if (res.item)
        this.t2 = res.item;
      this.check = res.check;
      if (this.t2.input_Delivery == 'N')
        this.input_delivery = false;
      this.t2.factory_ID = this.factory;
    });
    this.getSupplierID();
    this.getReason();

    
  }

  cancel() {
    let temp = this.t2.reasons;
    this.t2 = new SettingT2Delivery();
    this.t2.reasons = new Array<ReasonInfo>();
    this.t2.reasons = temp;
    this.t2.factory_ID = this.factory;
    this.t2.is_Valid = 'Y';
  }

  save() {
    this.input_delivery ? this.t2.input_Delivery = 'Y' : this.t2.input_Delivery = 'N';
    console.log(this.t2);

    this.settingT2Service.CreateorEdit(this.t2, this.check).subscribe(() => {
      if (this.check)
        this.alert.success("Update setting T2 supplier success");
      else
        this.alert.success("Add setting t2 supplier success");
      this.cancel();
      this.router.navigateByUrl('/rack/setting-t2-supplier/list-t2-supplier');
    }, err => {
      this.alert.error("Setting T2 Supplier exists");
      this.cancel();
    });
  }

  inputChange() {
    this.input_delivery = !this.input_delivery;
    if (this.input_delivery)
      this.t2.input_Delivery = "Y";
    else
      this.t2.input_Delivery = "N";
  }

  changeFactory(e: any): void {
    this.t2.factory_ID = e;
    this.factory = e;
    this.getSupplierID();
  }
  getReason() {
    this.settingT2Service.getAllReason().subscribe(res => {
      this.listReason = res.map(item => {
        return { id: item.reason_Code, text: item.reason_Code + '-' + item.reason_Ename + '-' + item.reason_Lname, additional: item.reason_Ename, }
      });
    })
  }
  getSupplierID() {
    this.packingListService.supplierList().subscribe(res => {
      this.listSupplierDB = JSON.parse(JSON.stringify(res));
      this.supplierIDList = res.map(item => {
        return {id: item.supplier_No, text: `${item.supplier_No}-${item.supplier_Name}`};
      });
      this.supplierIDList.unshift({ id: ' ', text: 'All Supplier' });
    });
  }
  changedSupplierID(e: any): void {
    this.t2.t2_Supplier_ID = e;
    this.getSuppliername(this.t2.t2_Supplier_ID);
  }
  getSuppliername(id: string) {
    this.t2.t2_Supplier_Name = this.listSupplierDB.find(x => x.supplier_No.trim() === id.trim()).supplier_Name;
  }
  changeReasonCode(e: any): void {
    this.reason = new ReasonInfo();
    this.reason.reason_Code = e;
    this.reason.reason_Name = this.listReason.find(x => x.id == this.reason.reason_Code).additional;
  }
  Add() {
    this.t2.reasons.push(this.reason);
  }

  delete(param: ReasonInfo) {
    this.t2.reasons.splice(this.t2.reasons.indexOf(param), 1);
  }

}
