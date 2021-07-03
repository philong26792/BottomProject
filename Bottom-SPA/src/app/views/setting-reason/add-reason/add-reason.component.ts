import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { SettingReason } from '../../../_core/_models/setting-reason';
import { AlertifyService } from '../../../_core/_services/alertify.service';
import { SettingReasonService } from '../../../_core/_services/setting-reason.service';

@Component({
  selector: 'app-add-reason',
  templateUrl: './add-reason.component.html',
  styleUrls: ['./add-reason.component.scss']
})
export class AddReasonComponent implements OnInit {
  reason: SettingReason = new SettingReason();
  check: boolean;
  trans_toHP: boolean = false;
  is_Shortage: boolean = false;
  constructor(private _service: SettingReasonService,
    private _router: Router,
    private _alert: AlertifyService) { }

  ngOnInit(): void {
    this._service.dataAddandEditMail.asObservable().subscribe(res => {
      if (res.item)
        this.reason = res.item;
      this.check = res.check;
      if (this.reason.trans_toHP == 'Y') {
        this.trans_toHP = true;
      }
      if (this.reason.is_Shortage == 'Y') {
        this.is_Shortage = true;
      }
    })
  }

  // cancel = () => this.reason = new SettingReason();

  cancel() {
    this.reason = new SettingReason()

  }

  save() {
    this._service.updateAndCreatReason(this.reason, this.check).subscribe(() => {
      this.cancel();
      if (this.check) {
        this._alert.success("Update setting reason success");
      }
      else {
        this._alert.success("Add setting reason success");
      }
      this._router.navigateByUrl('/rack/setting-reason/list-reason');
    }, error => {
      this._alert.error("Setting reason exits ")
      this.cancel();
    })
  }
  transToHPchange() {
    this.trans_toHP = !this.trans_toHP;
    if (this.trans_toHP == true) {
      this.reason.trans_toHP = "Y";
    }
    else {
      this.reason.trans_toHP = "N";
    }
  }
  isShortageChange() {
    this.is_Shortage = !this.is_Shortage;
    this.is_Shortage == true ?
      this.reason.is_Shortage = "Y"
      :
      this.reason.is_Shortage = "N";

  }
}
