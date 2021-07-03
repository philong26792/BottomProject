import { Component, OnInit, OnDestroy } from '@angular/core';
import { RackLocation } from '../../../_core/_models/rack-location';
import { AlertifyService } from '../../../_core/_services/alertify.service';
import { Router } from '@angular/router';
import { WmsCode } from '../../../_core/_models/wms-code';
import { RackService } from '../../../_core/_services/rack.service';
import { commonPerFactory } from '../../../_core/_utility/common-per-factory';

@Component({
  selector: 'app-rack-form',
  templateUrl: './rack-form.component.html',
  styleUrls: ['./rack-form.component.scss']
})
export class RackFormComponent implements OnInit {
  rack: any = {};
  flag = "0";
  factories: WmsCode[];
  whs: WmsCode[];
  buildings: WmsCode[];
  floors: WmsCode[];
  areas: WmsCode[];
  constructor(
    private rackServcie: RackService,
    private alertify: AlertifyService,
    private router: Router) { }
  ngOnInit() {
    this.loadDatas();
    this.rackServcie.currentRack.subscribe(rack => this.rack = rack);
    this.rackServcie.currentFlag.subscribe(flag => this.flag = flag);
    if (this.flag === "0") {
      this.rack.factory_ID = commonPerFactory.defaultRackFactory;
      this.rack.area_ID = "";
      this.rack.wH_ID = commonPerFactory.defaultRackWH;
      this.rack.floor_ID = "";
      this.rack.build_ID = "";
    }
  }

  create() {
    if (this.rack.factory_ID === "" || this.rack.area_ID === "" || this.rack.wH_ID === "" ||
      this.rack.floor_ID === "" || this.rack.build_ID === "" || this.rack.rack_Location === "") {
      this.alertify.error("Please enter full information!");
    } else {
      delete this.rack.updated_Time;
      delete this.rack.rack_Invalid_date;
      delete this.rack.cBM;
      delete this.rack.max_per;
      this.rackServcie.create(this.rack).subscribe(
        () => {
          this.alertify.success("Add succeed");
          this.router.navigate(["/rack/main"]);
        },
        error => {
          this.alertify.error(error);
        }
      )
    }
  }

  update() {
    this.rackServcie.update(this.rack).subscribe(
      () => {
        this.alertify.success("Updated succeed");
        this.router.navigate(["/rack/main"]);
      },
      error => {
        this.alertify.error(error)
      }
    )
  }

  loadDatas() {
    this.rackServcie.getFactories()
      .subscribe((res) => {
        console.log(res);
        this.factories = res;
      }, error => {
        this.alertify.error(error);
      });
    this.rackServcie.getWHs()
      .subscribe((res) => {
        this.whs = res;
      }, error => {
        this.alertify.error(error);
      });
    this.rackServcie.getBuildings()
      .subscribe((res) => {
        this.buildings = res;
      }, error => {
        this.alertify.error(error);
      });
    this.rackServcie.getFloors()
      .subscribe((res) => {
        this.floors = res;
      }, error => {
        this.alertify.error(error);
      });
    this.rackServcie.getAreas()
      .subscribe((res) => {
        this.areas = res;
      }, error => {
        this.alertify.error(error);
      });
  }
  backList() {
    this.router.navigate(["/rack/main"]);
  }

}
