import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ModifyQrCodeAfterSaveModel } from '../_models/modify-qrcode-after-save-model';
import { ModifyQrCodeMain } from '../_models/modify-qrcode-main';
import { Setting_T2Delivery } from '../_models/setting-t2-delivery';
import { SettingReason } from '../_models/setting-reason';
import { TransferDetail } from '../_models/transfer-detail';
import { FunctionUtility } from '../_utility/function-utility';

@Injectable({
  providedIn: 'root'
})
export class ModifyQrcodeService {
  baseUrl = environment.apiUrl;
  modifyQrCodeMainSource = new BehaviorSubject<ModifyQrCodeMain>(null);
  currentModifyQrCodeMain = this.modifyQrCodeMainSource.asObservable();
  modifyQrCodeAfterSaveSource = new BehaviorSubject<ModifyQrCodeAfterSaveModel[]>(null);
  currentmodifyQrCodeAfterSave = this.modifyQrCodeAfterSaveSource.asObservable();
  qrCodeModelSource = new BehaviorSubject<ModifyQrCodeAfterSaveModel>(null);
  currentQrCodeModel = this.qrCodeModelSource.asObservable();
  listMissingPrintSource = new BehaviorSubject<string[]>([]);
  currentlistMissingPrint = this.listMissingPrintSource.asObservable();
  otherTypeSource = new BehaviorSubject<string>('');
  currentOtherType = this.otherTypeSource.asObservable();
  constructor(private http: HttpClient, 
              private functionUtility: FunctionUtility) { }
  search(moNo: string, supplierId: string, qrCodeId: string):  Observable<ModifyQrCodeMain[]> {
    return this.http.get<ModifyQrCodeMain[]>(this.baseUrl + 'modifyStore/search', 
    {params: {moNo : moNo, supplierId : supplierId, qrCodeId: qrCodeId}})
  }
  getDetailModifyQrCode(moNo: string, materialId: string) {
    return this.http.get<any>(this.baseUrl + 'modifyStore/modifyDetail', 
    {params: {moNo: moNo, materialId: materialId}});
  }
  getDetailQrCode(model: ModifyQrCodeAfterSaveModel): Observable<TransferDetail[]> {
    return this.http.post<TransferDetail[]>(this.baseUrl + 'modifyStore/getDetailQrCode', model, {});
  }
  getAllReason(): Observable<SettingReason[]> {
    return this.http.get<SettingReason[]>(this.baseUrl + 'settingReason/getAll', {});
  }
  saveNoByBatch(param: any): Observable<ModifyQrCodeAfterSaveModel[]> {
    return this.http.post<ModifyQrCodeAfterSaveModel[]>(this.baseUrl + 'modifyStore/saveNoByBatchOut', param, {});
  }
  saveNoByBatchIn(param: any): Observable<ModifyQrCodeAfterSaveModel[]> {
    return this.http.post<ModifyQrCodeAfterSaveModel[]>(this.baseUrl + 'modifyStore/saveNoByBatchIn', param, {});
  }
  changeQrCodeModifyMain(model: ModifyQrCodeMain) {
    this.modifyQrCodeMainSource.next(model);
  }
  changeModifyQrCodeAfterSave(data: ModifyQrCodeAfterSaveModel[]) {
    this.modifyQrCodeAfterSaveSource.next(data);
  }
  changeQrCodeModel(data: ModifyQrCodeAfterSaveModel) {
    this.qrCodeModelSource.next(data);
  }
  changeListMissingPrint(data: string[]) {
    this.listMissingPrintSource.next(data);
  }
  changeOtherType(data: string) {
    this.otherTypeSource.next(data);
  }
  getPlanNoByQrCodeId(qrCodeID: string) {
    return this.http.get<any>(this.baseUrl + 'modifyStore/PlanNoByQRCodeID/' + qrCodeID);
  }
  getReasonOfSupplier(supplierId: string) {
    return this.http.get<Setting_T2Delivery[]>(this.baseUrl + 'modifyStore/getReasonOfSupplierID',
                      {params: {supplierId:supplierId}});
  }
}
