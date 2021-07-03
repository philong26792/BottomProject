import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Code } from '../_models/code';
import { KanbanByCategoryDetail } from '../_models/kanban-by-category-detail';
import { KanbanByCategoryDetailByPo } from '../_models/kanban-by-category-detail-by-po';
import { KanbanByCategoryDetailByToolCode } from '../_models/kanban-by-category-detai-by-tool-code';
import { BehaviorSubject } from 'rxjs';
import { RackArea } from '../_models/rack-area';
import { PaginatedResult } from '../_models/pagination';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class KanbanService {
  baseUrl = environment.apiUrl;
  codeNameSource = new BehaviorSubject<string>('');
  currentCodeName = this.codeNameSource.asObservable();

  constructor(private http: HttpClient) { }

  changeCodeName(codeName: string) {
    this.codeNameSource.next(codeName);
  }

  getKanbanByCategory() {
    return this.http.get<Code[]>(this.baseUrl + 'Kanban/getkanbancategory');
  }

  getKanbanByCategoryDetail(codeId: string, pageNumber?, pageSize?) {
    const paginatedResult: PaginatedResult<KanbanByCategoryDetail[]> = new PaginatedResult<KanbanByCategoryDetail[]>();

    let params = new HttpParams();

    if (pageNumber != null && pageSize != null) {
      params = params.append('pageNumber', pageNumber);
      params = params.append('pageSize', pageSize);
    }
    params = params.append('codeId', codeId);
    return this.http.get<KanbanByCategoryDetail[]>(this.baseUrl + 'Kanban/getkanbancategorydetail',
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

  exportExcelKanBanByCategoryDetail(codeId: string) {
    return this.http.get(this.baseUrl + 'Kanban/exportexcelgetkanbancategorydetail/', { params: { codeId: codeId }, responseType: 'blob' })
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

  getKanbanByCategoryDetailByToolCode(codeId: string, toolCode: string, pageNumber?, pageSize?) {
    const paginatedResult: PaginatedResult<KanbanByCategoryDetailByToolCode[]> = new PaginatedResult<KanbanByCategoryDetailByToolCode[]>();

    let params = new HttpParams();

    if (pageNumber != null && pageSize != null) {
      params = params.append('pageNumber', pageNumber);
      params = params.append('pageSize', pageSize);
    }
    params = params.append('codeId', codeId);
    params = params.append('toolCode', toolCode);
    return this.http.get<KanbanByCategoryDetailByToolCode[]>(this.baseUrl + 'Kanban/getkanbancategorydetailbytoolcode',
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

  exportExcelKanBanByCategoryDetailByToolcode(codeId: string, toolCode: string) {
    return this.http.get(this.baseUrl + 'Kanban/exportexcelgetkanbancategorydetailbytoolcode/',
      { params: { codeId: codeId, toolCode: toolCode }, responseType: 'blob' })
      .subscribe((result: Blob) => {
        if (result.type !== 'application/xlsx') {
          alert(result.type);
        }
        const blob = new Blob([result]);
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        const currentTime = new Date();
        const filename = 'Excel_KanbanByCategoryDetailByToolCode_' + currentTime.getFullYear().toString() +
          (currentTime.getMonth() + 1) + currentTime.getDate() +
          currentTime.toLocaleTimeString().replace(/[ ]|[,]|[:]/g, '').trim() + '.xlsx';
        link.href = url;
        link.setAttribute('download', filename);
        document.body.appendChild(link);
        link.click();
      });
  }

  getKanbanByCategoryDetailByPo(codeId: string, toolCode: string, po: string) {
    return this.http.get<KanbanByCategoryDetailByPo[]>(this.baseUrl + 'Kanban/getkanbancategorydetailbypo',
      { params: { codeId: codeId, toolCode: toolCode, po: po } });
  }

  exportExcelKanBanByCategoryDetailByPo(codeId: string, toolCode: string, po: string) {
    return this.http.get(this.baseUrl + 'Kanban/exportexcelgetkanbancategorydetailbypo/',
      { params: { codeId: codeId, toolCode: toolCode, po: po }, responseType: 'blob' })
      .subscribe((result: Blob) => {
        if (result.type !== 'application/xlsx') {
          alert(result.type);
        }
        const blob = new Blob([result]);
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        const currentTime = new Date();
        const filename = 'Excel_KanBanByCategoryDetailByPo_' + currentTime.getFullYear().toString() +
          (currentTime.getMonth() + 1) + currentTime.getDate() +
          currentTime.toLocaleTimeString().replace(/[ ]|[,]|[:]/g, '').trim() + '.xlsx';
        link.href = url;
        link.setAttribute('download', filename);
        document.body.appendChild(link);
        link.click();
      });
  }


}
