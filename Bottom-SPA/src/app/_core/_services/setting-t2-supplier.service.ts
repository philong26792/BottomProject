import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { PaginatedResult, Pagination } from '../_models/pagination';
import { SettingReason } from '../_models/setting-reason';
import { SettingT2Delivery, SettingT2Param } from '../_models/setting-t2-supplier';

@Injectable({
  providedIn: 'root'
})
export class SettingT2SupplierService {
  dataAddorEdit = new BehaviorSubject<any>(null);
  baseUrl = environment.apiUrl;
  constructor(private http: HttpClient) { }

  getAll(pagination: Pagination, T2params: SettingT2Param) {
    const paginatedResult: PaginatedResult<SettingT2Delivery[]> = new PaginatedResult<SettingT2Delivery[]>();
    let params = new HttpParams()
      .set('pageNumber', pagination.currentPage.toString())
      .set('pageSize', pagination.itemsPerPage.toString())
      .set('factory_id', T2params.factory_id)
      .set('reason_code', T2params.reason_code)
      .set('supplier_id', T2params.supplier_id)
      .set('input_delivery', T2params.input_delivery);
    return this.http.get<SettingT2Delivery[]>(`${this.baseUrl}SettingT2Supplier/GetAll`, { observe: 'response', params })
      .pipe(
        map(response => {
          paginatedResult.result = response.body;
          if (response.headers.get('Pagination')! + null) {
            paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
          }
          return paginatedResult;
        })
      );
  }

  CreateorEdit(item: SettingT2Delivery, flag: boolean) {
    if (flag)
      return this.http.put(`${this.baseUrl}SettingT2Supplier/edit`, item);
    return this.http.post(`${this.baseUrl}SettingT2Supplier/Create`, item);
  }

  delete(item: SettingT2Delivery) {
    return this.http.post(`${this.baseUrl}SettingT2Supplier/delete`, item);
  }

  getAllReason() {
    return this.http.get<any>(`${this.baseUrl}SettingReason/getAll`);
  }


}
