import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Select2OptionData } from 'ng-select2';
import { SettingMailSupllier } from '../../../_core/_models/setting-mail-supplier';
import { AlertifyService } from '../../../_core/_services/alertify.service';
import { SettingMailService } from '../../../_core/_services/setting-mail.service';
import { commonPerFactory } from '../../../_core/_utility/common-per-factory';

@Component({
  selector: 'app-add-mail',
  templateUrl: './add-mail.component.html',
  styleUrls: ['./add-mail.component.scss']
})
export class AddMailComponent implements OnInit {
  settingMailSupplier: SettingMailSupllier = new SettingMailSupllier();
  checkSave: boolean;
  factory_ID: string
  listDataFactory: Array<Select2OptionData> = [{ id: commonPerFactory.defaultRackFactory, text: commonPerFactory.defaultRackFactory }];
  listDataSupplier: Array<Select2OptionData>;
  listSubcon: Array<Select2OptionData>;

  dataSupplier: any;
  dataSubcon: any;


  constructor(
    private _service: SettingMailService,
    private router: Router,
    private alertify: AlertifyService) { }

  ngOnInit() {
    this._service.dataAddandEditMail.asObservable().subscribe(res => {
      this.checkSave = res.check;
      this.dataSubcon = res.dataSubcon;
      this.dataSupplier = res.dataSupplier;
      this.listDataSupplier = res.dataSupplier.map(item => { return { id: item.t3_Supplier, text: item.t3_Supplier } });
      this.listSubcon = res.dataSubcon.map(item => { return { id: item.subcon_ID, text: item.subcon_ID } })
      this.listSubcon.push({ id: ' ', text: ' ' });
      this.listDataSupplier.push({ id: 'ZZZZ', text: 'ZZZZ' });
      this.cancel();
      if (res.check)
        this.settingMailSupplier = res.item;
    })
  }

  cancel() {
    this.settingMailSupplier = new SettingMailSupllier();
    this.settingMailSupplier.supplier_No = this.dataSupplier[0].t3_Supplier;
    this.settingMailSupplier.subcon_ID = this.dataSubcon[0].subcon_ID;
    this.settingMailSupplier.supplier_Name = this.dataSupplier[0].t3_Supplier_Name
    this.settingMailSupplier.subcon_Name = this.dataSubcon[0].subcon_Name
  }

  changeSupplier = (event: any) => this.settingMailSupplier.supplier_Name = this.dataSupplier.find(x => x.t3_Supplier == event).t3_Supplier_Name;

  changeSubcon = (event: any) => this.settingMailSupplier.subcon_Name = this.dataSubcon.find(x => x.subcon_ID == event).subcon_Name

  save() {
    console.log(this.settingMailSupplier);
    this._service.updateSettingMailSupplier(this.settingMailSupplier, this.checkSave).subscribe(() => {
      this.cancel();
      if (this.checkSave) {
        this.alertify.success("Update setting mail success");
        this.router.navigate(['/rack/setting-mail/list-mail']);
      }
      else {
        this.alertify.success("Add setting mail success");
        this.router.navigate(['/rack/setting-mail/list-mail']);
      }

    }, error => {
      this.alertify.error("Setting mail (supplier NO) exits ")
      this.cancel();
    })
  }

}
