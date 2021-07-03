import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { PaginatedResult } from '../_models/pagination';
import { TransferFormGenerate } from '../_models/transfer-form-generate';
import { TransferFormPrint } from '../_models/transfer-form-print';
import { FunctionUtility } from '../_utility/function-utility';

@Injectable({
  providedIn: 'root'
})
export class TransferFormService {
  baseUrl = environment.apiUrl;
  transferFormPrintDetailSource = new BehaviorSubject<TransferFormGenerate[]>([]);
  currentTransferFormPrintDetail = this.transferFormPrintDetailSource.asObservable();
  checkSearchSource = new BehaviorSubject<boolean>(false);
  currentCheckSearch = this.checkSearchSource.asObservable();
  paramSearchTransferFormPrintSource = new BehaviorSubject<any>({
    fromTime: new Date(),
    toTime: new Date(),
    planNo: '',
    release: 'N',
    currentPage: 1,
    t3Supplier: 'all'
  });
  currentParamSearchTransferFormPrint = this.paramSearchTransferFormPrintSource.asObservable();

  constructor(private http: HttpClient, private functionUtility: FunctionUtility) { }

  changeTransferFormPrintDetail(transferFormPrintDetail: TransferFormGenerate[]) {
    this.transferFormPrintDetailSource.next(transferFormPrintDetail);
  }

  changeparamSearchTransferFormPrint(paramSearchTransferFormPrint: any) {
    this.paramSearchTransferFormPrintSource.next(paramSearchTransferFormPrint);
  }

  getTransferFormGenerate(fromTime: Date, toTime: Date, moNo: string, isSubcont: string,
                          t2Supplier: string, t3Supplier: string, pageNumber?, pageSize?) {
    const paginatedResult: PaginatedResult<TransferFormGenerate[]> = new PaginatedResult<TransferFormGenerate[]>();

    let params = new HttpParams();

    if (pageNumber != null && pageSize != null) {
      params = params.append('pageNumber', pageNumber);
      params = params.append('pageSize', pageSize);
    }

    const t1 = fromTime == null ? '' : this.functionUtility.getDateFormat(fromTime);
    const t2 = toTime == null ? '' : this.functionUtility.getDateFormat(toTime);
    t3Supplier = t3Supplier === 'all' ? '' : t3Supplier;
    t2Supplier = t2Supplier === 'all' ? '' : t2Supplier;

    return this.http.get<TransferFormGenerate[]>(this.baseUrl + 'TransferForm/GetTransferFormGenerate?fromTime='
      + t1 + '&toTime=' + t2 + '&moNo=' + moNo + '&isSubcont=' + isSubcont+ '&t2Supplier=' + t2Supplier
      + '&t3Supplier=' + t3Supplier, { observe: 'response', params })
      .pipe(
        map(response => {
          paginatedResult.result = response.body;
          if (response.headers.get('Pagination') != null) {
            paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
          }
          return paginatedResult;
        }),
      );
  }
  generateTransferForm(listGenerateTransform: TransferFormGenerate[]) {
    return this.http.post(this.baseUrl + 'TransferForm/GenerateTransferForm', listGenerateTransform);
  }

  getTransferFormPrint(fromTime: Date, toTime: Date, moNo: string, isRelease: string, t2Supplier: string,
                        t3Supplier: string, pageNumber?, pageSize?) {
    const paginatedResult: PaginatedResult<TransferFormGenerate[]> = new PaginatedResult<TransferFormGenerate[]>();

    let params = new HttpParams();

    if (pageNumber != null && pageSize != null) {
      params = params.append('pageNumber', pageNumber);
      params = params.append('pageSize', pageSize);
    }

    const t1 = fromTime == null ? '' : this.functionUtility.getDateFormat(fromTime);
    const t2 = toTime == null ? '' : this.functionUtility.getDateFormat(toTime);
    t3Supplier = t3Supplier === 'all' ? '' : t3Supplier;
    t2Supplier = t2Supplier === 'all' ? '' : t2Supplier;

    return this.http.get<TransferFormGenerate[]>(this.baseUrl + 'TransferForm/GetTransferFormPrint?fromTime='
      + t1 + '&toTime=' + t2 + '&moNo=' + moNo + '&isRelease=' + isRelease + '&t2Supplier=' + t2Supplier 
      + '&t3Supplier=' + t3Supplier, { observe: 'response', params })
      .pipe(
        map(response => {
          paginatedResult.result = response.body;
          if (response.headers.get('Pagination') != null) {
            paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
          }
          return paginatedResult;
        }),
      );
  }

  releaseTransferForm(listReleaseTransform: TransferFormGenerate[]) {
    return this.http.post(this.baseUrl + 'TransferForm/ReleaseTransferForm', listReleaseTransform);
  }

  sendEmail(listDataSendEmail: TransferFormGenerate[]) {
    return this.http.post(this.baseUrl + 'TransferForm/SendEmail', listDataSendEmail);
  }

  getInfoTransferFormPrintDetail(listPrintTransform: TransferFormGenerate[]) {
    return this.http.post<TransferFormPrint[]>(this.baseUrl + 'TransferForm/GetInfoTransferFormPrintDetail', listPrintTransform);
  }
}
