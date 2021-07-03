import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { BehaviorSubject } from 'rxjs';
import { HttpClient, HttpParams } from '@angular/common/http';
import { RackArea } from '../_models/rack-area';
import { KanBanRackDetail } from '../_viewmodels/kanban-rack-detail';
import { PoListT3 } from '../_models/po-list-t3';
import { KanbanByRackDetail } from '../_models/kanban-by-rack-detail';
import { PaginatedResult } from '../_models/pagination';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class KanbanByrackService {
  baseUrl = environment.apiUrl;
  codeNameSource = new BehaviorSubject<string>('');
  currentCodeName = this.codeNameSource.asObservable();
  rackDetailT2Source = new BehaviorSubject<KanBanRackDetail[]>([]);
  currentrackDetailT2 = this.rackDetailT2Source.asObservable();
  rackSource = new BehaviorSubject<string>('');
  currentRack = this.rackSource.asObservable();
  constructor(private http: HttpClient) { }
  getKanbanByRackArea() {
    return this.http.get<RackArea[]>(this.baseUrl + 'KanbanByRack/getkanbanbyrack');
  }
  getKanbanRackDetail(rack: string, pageNumber?, pageSize?) {
    // const paginatedResult: PaginatedResult<KanBanRackDetail[]> = new PaginatedResult<KanBanRackDetail[]>();
    const paginatedResult: any = [];

    let params = new HttpParams();

    if (pageNumber != null && pageSize != null) {
      params = params.append('pageNumber', pageNumber);
      params = params.append('pageSize', pageSize);
    }
    params = params.append('rackLocation', rack);
    return this.http.get<KanBanRackDetail[]>(this.baseUrl + 'KanbanByRack/getdetailRack',
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
  getKanbanByRackAreaDetail(build: string) {
    return this.http.get<KanbanByRackDetail[]>(this.baseUrl + 'KanbanByRack/getkanbanbyrackdetail/' + build, {});
  }
  changeRackDetailT2(data: KanBanRackDetail[]) {
    this.rackDetailT2Source.next(data);
  }
  changeRack(rack: string) {
    this.rackSource.next(rack);
  }
  exportExcelRackDetailT2(rackLocation: string) {
    return this.http.get(this.baseUrl + 'KanbanByRack/exportExcelRackDetail/' + rackLocation, { responseType: 'blob' })
      .subscribe((result: Blob) => {
        
        if (result.type !== 'application/xlsx') {
          alert(result.type);
        }
        const blob = new Blob([result]);
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        const currentTime = new Date();
        const filename = 'Excel_KanBanByCategoryDetail_' + currentTime.getFullYear().toString() +
          (currentTime.getMonth() + 1) + currentTime.getDate() +
          currentTime.toLocaleTimeString().replace(/[ ]|[,]|[:]/g, '').trim() + '.xlsx';
        link.href = url;
        link.setAttribute('download', filename);
        document.body.appendChild(link);
        link.click();
      });
  }
  getPoListT3(rackLoacion: string, pageNumber?, pageSize?) {
    // const paginatedResult: PaginatedResult<PoListT3[]> = new PaginatedResult<PoListT3[]>();
    const paginatedResult: any = [];

    let params = new HttpParams();

    if (pageNumber != null && pageSize != null) {
      params = params.append('pageNumber', pageNumber);
      params = params.append('pageSize', pageSize);
    }
    params = params.append('rackLoacion', rackLoacion);
    return this.http.get<PoListT3[]>(this.baseUrl + 'KanbanByRack/getlistpot3',
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
  exportExcelPoListT3(rackLocation: string) {
    return this.http.get(this.baseUrl + 'KanbanByRack/exportexcellistpot3',
      { params: { rackLocation: rackLocation }, responseType: 'blob' })
      .subscribe((result: Blob) => {
        if (result.type !== 'application/xlsx') {
          alert(result.type);
        }
        const blob = new Blob([result]);
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        const currentTime = new Date();
        const filename = 'Excel_KanBanByRackPolist_' + currentTime.getFullYear().toString() +
          (currentTime.getMonth() + 1) + currentTime.getDate() +
          currentTime.toLocaleTimeString().replace(/[ ]|[,]|[:]/g, '').trim() + '.xlsx';
        link.href = url;
        link.setAttribute('download', filename);
        document.body.appendChild(link);
        link.click();
      });
  }

  exportExcelKanbanByRackDetail(build_id: string) {
    return this.http.get(this.baseUrl + 'KanbanByRack/exportExcelKanbanByRackDetail/' + build_id, { responseType: 'blob' })
      .subscribe((result: Blob) => {
        
        if (result.type !== 'application/xlsx') {
          alert(result.type);
        }
        const blob = new Blob([result]);
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        const currentTime = new Date();
        const filename = 'Excel_KanBanByRackDetail_' + currentTime.getFullYear().toString() +
          (currentTime.getMonth() + 1) + currentTime.getDate() +
          currentTime.toLocaleTimeString().replace(/[ ]|[,]|[:]/g, '').trim() + '.xlsx';
        link.href = url;
        link.setAttribute('download', filename);
        document.body.appendChild(link);
        link.click();
      });
  }
}
