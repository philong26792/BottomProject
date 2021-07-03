import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Output } from '../_models/output';
import { BehaviorSubject } from 'rxjs';
import { MaterialSheetSize } from '../_models/material-sheet-size';
import { OutputM, OutputParam } from '../_models/outputM';
import { TransferDetail } from '../_models/transfer-detail';
import { OutputDetail } from '../_models/output-detail';
import { OutputPrintQrCode } from '../_models/output-print-qrcode';

@Injectable({
  providedIn: 'root'
})
export class OutputService {
  baseUrl = environment.apiUrl;
  listMaterialSheetSizeSource = new BehaviorSubject<Array<any>>([]);
  currentListMaterialSheetSize = this.listMaterialSheetSizeSource.asObservable();
  outputMSource = new BehaviorSubject<Object>({});
  currentOutputM = this.outputMSource.asObservable();
  listOutputMSource = new BehaviorSubject<Array<OutputM>>([]);
  currentListOutputM = this.listOutputMSource.asObservable();
  flagFinishSource = new BehaviorSubject<boolean>(false);
  currentFlagFinish = this.flagFinishSource.asObservable();
  flagSubmitSource = new BehaviorSubject<boolean>(false);
  currentFlagSubmit = this.flagSubmitSource.asObservable();
  listOutputSaveSource = new BehaviorSubject<Array<any>>([]);
  currentListOutputSave = this.listOutputSaveSource.asObservable();
  paramPrintQrCodeAgainSource = new BehaviorSubject<any[]>([]);
  currentParamPrintQrCodeAgain = this.paramPrintQrCodeAgainSource.asObservable();
  printQrCodeSource = new BehaviorSubject<string>('0');
  currentPrintQrCode = this.printQrCodeSource.asObservable();
  checkScanOutputSource = new BehaviorSubject<number>(1);
  currentCheckScanOutput = this.checkScanOutputSource.asObservable();

  constructor(private http: HttpClient) { }

  changeListMaterialSheetSize(listMaterialSheetSize: Array<any>) {
    this.listMaterialSheetSizeSource.next(listMaterialSheetSize);
  }
  changeOutputM(outputM: object) {
    this.outputMSource.next(outputM);
  }
  changeListOutputM(listOutputM: OutputM[]) {
    this.listOutputMSource.next(listOutputM);
  }
  changeFlagFinish(flag: boolean) {
    this.flagFinishSource.next(flag);
  }
  changeFlagSubmit(flag: boolean) {
    this.flagSubmitSource.next(flag);
  }
  changeListOutputSave(listOutputSave: any[]) {
    this.listOutputSaveSource.next(listOutputSave);
  }
  changeParamPrintQrCodeAgain(paramPrintQrCodeAgain: any[]) {
    this.paramPrintQrCodeAgainSource.next(paramPrintQrCodeAgain);
  }
  changeCheckScanOutput(checkScanOutput: number) {
    this.checkScanOutputSource.next(checkScanOutput);
  }
  getMainByQrCodeId(qrCodeId: string) {
    return this.http.get<Output>(this.baseUrl + 'Output/GetByQrCodeId', { params: { qrCodeId: qrCodeId } });
  }
  getMainByQrCodeIdByCollectionTransferForm(qrCodeId: string) {
    return this.http.get<Output>(this.baseUrl + 'Output/GetByQrCodeIdByCollectionTransferForm', { params: { qrCodeId: qrCodeId } });
  }
  getMainByQrCodeIdBySortingForm(qrCodeId: string) {
    return this.http.get<Output>(this.baseUrl + 'Output/GetByQrCodeIdBySortingForm', { params: { qrCodeId: qrCodeId } });
  }
  saveOutput(outputM: OutputM, listTransactionDetail: TransferDetail[]) {
    const param = {output: outputM, transactionDetail: listTransactionDetail};
    return this.http.post(this.baseUrl + 'Output/Save', param);
  }
  getOutputDetail(transacNo: string) {
    return this.http.get<OutputDetail>(this.baseUrl + 'Output/detail/' + transacNo);
  }
  saveListOutput(listOutput: OutputParam[]) {
    return this.http.post(this.baseUrl + 'Output/savelistoutput', listOutput);
  }
  printQrCode(paramPrintQrCodeAgain: any) {
    return this.http.post<OutputPrintQrCode[]>(this.baseUrl + 'Output/printqrcodeagain', paramPrintQrCodeAgain);
  }
  changePrintQrCode(option: string) {
    this.printQrCodeSource.next(option);
  }
}
