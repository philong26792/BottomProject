import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { PaginatedResult } from '../_models/pagination';
import { ReceiveAfterSubmit } from '../_models/receive-after-submit';
import { MaterialParam } from '../_viewmodels/material-param';
import { Material2Model } from '../_viewmodels/material2-model';
import { ReceiveNoDetail } from '../_viewmodels/receive-no-detail';

@Injectable({
  providedIn: 'root'
})
export class ReceivingService {
  baseUrl = environment.apiUrl;
  receiveSource = new BehaviorSubject<ReceiveAfterSubmit[]>([]);
  currentReceive = this.receiveSource.asObservable();
  receiveItemSource = new BehaviorSubject<object>({});
  currentReceiveitem = this.receiveItemSource.asObservable();
  receiveNoDetailSource = new BehaviorSubject<ReceiveNoDetail[]>([]);
  currentReceiveNoDetail = this.receiveNoDetailSource.asObservable();
  paramSource = new BehaviorSubject<MaterialParam>(null);
  currentParam = this.paramSource.asObservable();
  constructor(private http: HttpClient) { }
  search(page?, itemsPerPage?, text?: MaterialParam): Observable<PaginatedResult<Material2Model[]>> {
    const paginatedResult: PaginatedResult<Material2Model[]> = new PaginatedResult<Material2Model[]>();
    let params = new HttpParams();
    if (page != null && itemsPerPage != null) {
      params = params.append('pageNumber', page);
      params = params.append('pageSize', itemsPerPage);
    }
    return this.http.post<any>(this.baseUrl + 'receivingMaterial/search/', text, { observe: 'response', params })
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
  receivingMaterial(data: Material2Model[]) {
    return this.http.post<any>(this.baseUrl + 'receivingMaterial/receiving/', data, {});
  }
  changeReceive(data: ReceiveAfterSubmit[]) {
    this.receiveSource.next(data);
  }
  changeReceiveItem(data: any) {
    this.receiveItemSource.next(data);
  }
  changeReceiveNoDetail(data: ReceiveNoDetail[]) {
    this.receiveNoDetailSource.next(data);
  }
  changeParam(data: MaterialParam) {
    this.paramSource.next(data);
  }

}
