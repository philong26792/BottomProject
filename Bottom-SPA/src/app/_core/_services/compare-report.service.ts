import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { StockCompare } from '../_models/stock-compare';

@Injectable({
  providedIn: 'root'
})
export class CompareReportService {
  baseUrl = environment.apiUrl;
  constructor(private http: HttpClient, private spinner: NgxSpinnerService) { }
  getStockCompare(receiveDate: string): Observable<StockCompare[]> {
    return this.http.get<StockCompare[]>(this.baseUrl + 'compareReport/getCompare?Receive_Date=' + receiveDate, {});
  }
  search(receive_Date: string, pageNumber?, pageSize?) {
    const paginatedResult: any = [];
    let params = new HttpParams();
    if (pageNumber != null && pageSize != null) {
      params = params.append('pageNumber', pageNumber);
      params = params.append('pageSize', pageSize);
    }
    params = params.append('receive_Date', receive_Date);
    return this.http.get<StockCompare[]>(this.baseUrl + 'compareReport/search',
    { observe: 'response', params })
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
  exportExcel(receive_Date: string) {
    this.spinner.show();
    return this.http.get(this.baseUrl + 'compareReport/exportExcel/', {params: {receive_Date:receive_Date}, responseType: 'blob'}).subscribe(
      (result: Blob) => {
        if (result.type !== 'application/xlsx') {
          alert(result.type);
        }
        const blob = new Blob([result]);
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        const currentTime = new Date();
        const filename = 'Excel_Compare_Report' + currentTime.getFullYear().toString() +
          (currentTime.getMonth() + 1) + currentTime.getDate() +
          currentTime.toLocaleTimeString().replace(/[ ]|[,]|[:]/g, '').trim() + '.xlsx';
        link.href = url;
        link.setAttribute('download', filename);
        document.body.appendChild(link);
        link.click();
        this.spinner.hide();
      }
    );
  }
}
