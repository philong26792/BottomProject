  import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { PaginatedResult } from '../_models/pagination';
import { SettingMailSupllier } from '../_models/setting-mail-supplier';

@Injectable({
  providedIn: 'root'
})
export class SettingMailService {
  baseUrl = environment.apiUrl;
  dataAddandEditMail = new BehaviorSubject<any>(null);

  constructor(private http: HttpClient) { }

  getAllSupllierNo() {
    return this.http.get<any>(this.baseUrl + 'SettingMail');
  }

  
  getAllSubcon() {
    return this.http.get<any>(this.baseUrl + 'SettingMail/getallsubcon');
  }


  search(pageNumber?, factoryParms?, supplier?) {
    const paginatedResult: PaginatedResult<SettingMailSupllier[]> = new PaginatedResult<SettingMailSupllier[]>();
    let params = new HttpParams();
    params = params.append('pageNumber', pageNumber);
    params = params.append('factory', factoryParms);
    params = params.append('supplierNo', supplier == 'all' ? '' : supplier);

    return this.http.get<SettingMailSupllier[]>(this.baseUrl + 'SettingMail/search', { observe: 'response', params: params })
      .pipe(
        map(response => {
          paginatedResult.result = response.body;
          if (response.headers.get('Pagination') != null) {
            paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
          }
          return paginatedResult;
        }),
      );;
  }

  getDataAddAndEdit(data: any) {
    this.dataAddandEditMail.next(data);
  }

  updateSettingMailSupplier(item: SettingMailSupllier, check: boolean) {
    if (check)
      return this.http.post(this.baseUrl + 'SettingMail/edit', item);

    return this.http.post(this.baseUrl + 'SettingMail/create', item);
  }
  deleteSettingMail(item: SettingMailSupllier) {
    return this.http.post(this.baseUrl + 'SettingMail/delete', item)
  }
}
