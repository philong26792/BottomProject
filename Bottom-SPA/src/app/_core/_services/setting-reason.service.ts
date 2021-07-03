import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { PaginatedResult } from '../_models/pagination';
import { SettingReason } from '../_models/setting-reason';

const API = environment.apiUrl;

@Injectable({
  providedIn: 'root'
})
export class SettingReasonService {
  dataAddandEditMail = new BehaviorSubject<any>(null);

  constructor(private http: HttpClient) { }

  getAllReason(pageNumber?,reasonCode?,reasonName?,trans_toHP?) {
    const paginatedResult: PaginatedResult<SettingReason[]> = new PaginatedResult<SettingReason[]>();
    let params = new HttpParams();
    params = params.append('pageNumber', pageNumber);
    params = params.append('reasonCode', reasonCode == 'all' ? '' : reasonCode);
    params = params.append('reasonName', reasonName);
    params = params.append('trans_toHP', trans_toHP == 'all' ? '' : trans_toHP);
    return this.http.get<SettingReason[]>(`${API}SettingReason`, { observe: 'response', params: params })
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

  deleteReason = (item: SettingReason) => this.http.post(`${API}SettingReason/delete`, item);

  updateAndCreatReason(item: SettingReason, check: boolean) {
    if (check)
      return this.http.post(`${API}SettingReason/edit`, item);
    return this.http.post(`${API}SettingReason`, item)
  }
  getReasonCode()
  {
    return this.http.get<any>(`${API}SettingReason/GetReasonCode`);
  }
}
