import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { BehaviorSubject } from 'rxjs';
import { environment } from '../../../environments/environment';
import { TransferM } from '../_models/transferM';
import { PaginatedResult } from '../_models/pagination';
import { map } from 'rxjs/operators';
import { FunctionUtility } from '../_utility/function-utility';
import { HistotyDetail } from '../_models/history-detail';
import { NgxSpinnerService } from 'ngx-spinner';

@Injectable({
  providedIn: 'root'
})
export class TransferService {
  baseUrl = environment.apiUrl;
  paramSearchSource = new BehaviorSubject<any>({fromDate: this.functionUtility.getToDay(), toDate: this.functionUtility.getToDay(), status: '', moNo: '', materialId: ''});
  currentParamSearch = this.paramSearchSource.asObservable();

  constructor(private http: HttpClient,
    private spinner: NgxSpinnerService,
    private functionUtility: FunctionUtility) { }

  getMainByQrCodeId(qrCodeId: string) {
    return this.http.get<TransferM>(this.baseUrl + 'TransferLocation/' + qrCodeId, {});
  }

  submitMain(lists: TransferM[]) {
    return this.http.post(this.baseUrl + 'TransferLocation/submit', lists);
  }


  changeParamSearch(paramSearch: any) {
    this.paramSearchSource.next(paramSearch);
  }

  search(pageNumber?, pageSize?, transferHistoryParam?) {
    const paginatedResult: PaginatedResult<TransferM[]> = new PaginatedResult<TransferM[]>();

    let params = new HttpParams();

    if (pageNumber != null && pageSize != null) {
      params = params.append('pageNumber', pageNumber);
      params = params.append('pageSize', pageSize);
    }

    return this.http.post<TransferM[]>(this.baseUrl + 'TransferLocation/search', transferHistoryParam, { observe: 'response', params })
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

  getTransferDetail(transferNo: string) {
    return this.http.get<HistotyDetail>(this.baseUrl + 'TransferLocation/GetDetailTransaction', { params: { transferNo: transferNo } });
  }
  
  getTransferDetailForOutput(transferNo: string) {
    return this.http.get<HistotyDetail>(this.baseUrl + 'TransferLocation/GetDetailTransactionForOutput', { params: { transferNo: transferNo } });
  }

  checkExistLocation(rackLocation: string) {
    return this.http.get(this.baseUrl + 'TransferLocation/CheckExistRackLocation', { params: { rackLocation: rackLocation } });
  }

  checkRackLocationHaveTheSameArea(fromLocation: string, toLocation) {
    return this.http.get(this.baseUrl + 'TransferLocation/CheckRackLocationHaveTheSameArea',
      { params: { fromLocation: fromLocation, toLocation: toLocation } });
  }

  checkTransacNoDuplicate(transacNo: string) {
    return this.http.get<boolean>(this.baseUrl + 'TransferLocation/CheckTransacNoDuplicate',
      { params: { transacNo: transacNo } }).subscribe(res => {return res} );
  }

  exportExcel(param: any) {
    this.spinner.show();
    let url = '';
    if(param.transac_Type.toString() === "I") {
      url = 'historyReport/excelInputReport';
    } else {
      url = 'historyReport/excelOutputReport';
    }
    return this.http.post(this.baseUrl + url, param, {responseType: 'blob' })
      .subscribe((result: Blob) => {
        if (result.type !== 'application/xlsx') {
          alert(result.type);
        }
        const blob = new Blob([result]);
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        const currentTime = new Date();
        let title;
        if(param.transac_Type.toString() === "I") {
          title = "Input";
        } else {
          title = "Output";
        }
        const filename = 'Excel_' + title + 'Report' + currentTime.getFullYear().toString() +
          (currentTime.getMonth() + 1) + currentTime.getDate() +
          currentTime.toLocaleTimeString().replace(/[ ]|[,]|[:]/g, '').trim() + '.xlsx';
        link.href = url;
        link.setAttribute('download', filename);
        document.body.appendChild(link);
        link.click();
        this.spinner.hide();
      });
  }
}
