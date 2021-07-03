import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { QrCodeSplitDetail } from '../_models/merge-qrcode/qr-code-split-detail';
import { SplitDataByOffset } from '../_models/merge-qrcode/split-data-by-Offset';
import { SplitMain } from '../_models/merge-qrcode/split-main';
import { SplitProcess } from '../_models/merge-qrcode/split-process';
import { WMSB_Transaction_Detail } from '../_models/transaction-detail';
import { FunctionUtility } from '../_utility/function-utility';
import { MaterialInformation } from '../_viewmodels/merge-qrcode/material-information';
import { MergeQrCodeModel } from '../_viewmodels/merge-qrcode/merge-qrcode-model';
import { QrCodeAfterMerge } from '../_viewmodels/merge-qrcode/qrCode-after-merge';

@Injectable({
  providedIn: 'root'
})
export class MergeQrcodeService {
  baseUrl = environment.apiUrl;
  paramSearchSplitMainSource = new BehaviorSubject<any>({moNo: '', qrCodeID: ''});
  currentparamSearchSplitMain = this.paramSearchSplitMainSource.asObservable();
  paramSearchOtherSplitMainSource = new BehaviorSubject<any>({moNo: null, qrCodeID: ''});
  currentparamSearchOtherSplitMain = this.paramSearchOtherSplitMainSource.asObservable();
  listQrCodeAfterMergeSource = new BehaviorSubject<QrCodeAfterMerge[]>([]);
  currentListQrCodeAfterMerge = this.listQrCodeAfterMergeSource.asObservable();
  qrCodeAfterMergeSource = new BehaviorSubject<QrCodeAfterMerge>(null);
  currentQrCodeAfterMerge = this.qrCodeAfterMergeSource.asObservable();

  constructor(private http: HttpClient,
    private functionUtility: FunctionUtility) { }

  searchOfMerge(param: any): Observable<MergeQrCodeModel[]> {
    return this.http.post<MergeQrCodeModel[]>(this.baseUrl + 'mergeQrCode/searchOfMerge', param);
  }
  mergeQrCode(param: MergeQrCodeModel[]): Observable<QrCodeAfterMerge[]> {
    return this.http.post<QrCodeAfterMerge[]>(this.baseUrl + 'mergeQrCode/mergeQrCode', param);
  }
  getQrCodeDetail(param: QrCodeAfterMerge): Observable<WMSB_Transaction_Detail[]> {
    return this.http.post<WMSB_Transaction_Detail[]>(this.baseUrl + 'mergeQrCode/qrCodeDetail', param);
  }
  getMaterialInformation(): Observable<MaterialInformation[]> {
    return this.http.get<MaterialInformation[]>(this.baseUrl + 'mergeQrCode/materialInformation', {});
  }
  getMaterialInformationByPo(moNo: string): Observable<MaterialInformation[]> {
    let params = new HttpParams();
    params = params.append('moNo', moNo);
    return this.http.get<MaterialInformation[]>(this.baseUrl + 'mergeQrCode/getMaterialInformationByPO', {params});
  }
  changeListQrCodeAfterMerge(data: QrCodeAfterMerge[]) {
    this.listQrCodeAfterMergeSource.next(data);
  }
  changeQrCodeAfterMerge(data: QrCodeAfterMerge) {
    this.qrCodeAfterMergeSource.next(data);
  }

  changeParamSearchSplitMain(param: any) {
    this.paramSearchSplitMainSource.next(param);
  }
  changeParamSearchOtherSplitMain(param: any) {
    this.paramSearchOtherSplitMainSource.next(param);
  }

  searchSplitMain(moNo: string, qrCodeID: string, timeFrom: Date, timeEnd: Date, searchByPrebook: boolean) {
    debugger
    let params = new HttpParams();
    params = params.append('moNo', moNo);
    params = params.append('qrCodeID', qrCodeID);
    params = params.append('timeFrom', timeFrom == null ? '' : this.functionUtility.getDateFormat(timeFrom));
    params = params.append('timeEnd', timeEnd == null ? '' : this.functionUtility.getDateFormat(timeEnd));
    params = params.append('searchByPrebook', searchByPrebook.toString());
    return this.http.get<MergeQrCodeModel[]>(this.baseUrl + 'mergeQrCode/TransactionForSplit', { params });
  }

  searchOtherSplitMain(moNo: string, qrCodeID: string) {
    let params = new HttpParams();
    params = params.append('moNo', moNo);
    params = params.append('qrCodeID', qrCodeID);
    return this.http.get<MergeQrCodeModel[]>(this.baseUrl + 'mergeQrCode/TransactionForOtherSplit', { params });
  }

  searchSplitInfoDetail(transacNo: string) {
    let params = new HttpParams();
    params = params.append('transacNo', transacNo);
    return this.http.get<SplitMain>(this.baseUrl + 'mergeQrCode/SplitInfoDetail', { params });
  }

  qrCodeSplitDetail(transacNo: string) {
    return this.http.get<QrCodeSplitDetail>(this.baseUrl + 'mergeQrCode/QrCodeSplitDetail/' + transacNo);
  }

  searchSplitProcess(transacNo: string) {
    return this.http.get<SplitProcess>(this.baseUrl + 'mergeQrCode/SplitProcess/' + transacNo);
  }
  getDataSplitByOffsetNo(offsetNo: string, materialId: string, moNo: string, transacNo: string) {
    let params = new HttpParams();
    params = params.append('offsetNo', offsetNo);
    params = params.append('materialId', materialId);
    params = params.append('moNo', moNo);
    params = params.append('transacNoParent', transacNo);
    return this.http.get<SplitDataByOffset[]>(this.baseUrl + 'mergeQrCode/DataSplitByOffsetNo', { params });
  }

  getDataOtherSplitByMONo( materialId: string, moNo: string, moSeq: string, transacNo: string, dMoNo: string) {
    let params = new HttpParams();
    params = params.append('moSeq', moSeq);
    params = params.append('materialId', materialId);
    params = params.append('moNo', moNo);
    params = params.append('transacNoParent', transacNo);
    params = params.append('dMoNo', dMoNo);
    return this.http.get<SplitDataByOffset>(this.baseUrl + 'mergeQrCode/DataOtherSplitByMONo', { params });
  }

  saveDataSplit(dataSplit: SplitDataByOffset[]) {
    return this.http.post<boolean>(this.baseUrl + 'mergeQrCode/SaveSplitData/', dataSplit);
  }

  saveDataOtherSplit(dataSplit: SplitDataByOffset[]) {
    return this.http.post<boolean>(this.baseUrl + 'mergeQrCode/SaveOtherSplitData/', dataSplit);
  }
}
