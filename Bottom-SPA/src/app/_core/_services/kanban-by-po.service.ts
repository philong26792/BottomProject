import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpParams, HttpClient } from '@angular/common/http';
import { PaginatedResult } from '../_models/pagination';
import { KanbanByPo } from '../_models/kanban-by-po';
import { map } from 'rxjs/operators';
import { KanbanByPoParam } from '../_viewmodels/kanban-by-po-param';
import { FunctionUtility } from '../_utility/function-utility';
import { BehaviorSubject } from 'rxjs';
import { KanbanByPoDetail } from '../_models/kanban-by-po-detail';
import { KanbanByPoDetailByReceivingType } from '../_models/kanban-by-po-detail-by-receiving-type';

@Injectable({
  providedIn: 'root'
})
export class KanbanByPoService {
  baseUrl = environment.apiUrl;
  kanbanByPoSource = new BehaviorSubject<any>([]);
  currentKanbanByPo = this.kanbanByPoSource.asObservable();
  kanbanByPoDetailSource = new BehaviorSubject<any>([]);
  currentKanbanByPoDetail = this.kanbanByPoDetailSource.asObservable();
  paramSearchSource = new BehaviorSubject<any>({
    dateType: 1,
    dateStart: new Date(),
    dateEnd: new Date(),
    line: '',
    moNo: '',
    supplier: '',
    article: '',
    modelName: '',
    kind: 0,
    currenPage: 1
  });
  currentParamSearch = this.paramSearchSource.asObservable();

  constructor(private http: HttpClient, private functionUtility: FunctionUtility) { }

  changeKanbanByPo(kanbanByPo: any) {
    this.kanbanByPoSource.next(kanbanByPo);
  }
  changeKanbanByPoDetail(kanbanByPoDetail: KanbanByPoDetail) {
    this.kanbanByPoDetailSource.next(kanbanByPoDetail);
  }

  changeParamSearch(paramSearch: any) {
    this.paramSearchSource.next(paramSearch);
  }

  search(pageNumber?, pageSize?, dateType?, dateStart?, dateEnd?, line?, moNo?, supplier?, article?, kind?, modelName?) {
    const kanbanByPoParam = new KanbanByPoParam();
    kanbanByPoParam.article = article;
    kanbanByPoParam.dateType = parseInt(dateType);
    kanbanByPoParam.moNo = moNo;
    kanbanByPoParam.supplier = supplier;
    kanbanByPoParam.kind = parseInt(kind);
    kanbanByPoParam.line = line;
    kanbanByPoParam.dateStart = dateStart;
    kanbanByPoParam.dateEnd = dateEnd;
    kanbanByPoParam.modelName = modelName;

    const paginatedResult: PaginatedResult<KanbanByPo[]> = new PaginatedResult<KanbanByPo[]>();

    let params = new HttpParams();

    if (pageNumber != null && pageSize != null) {
      params = params.append('pageNumber', pageNumber);
      params = params.append('pageSize', pageSize);
    }

    return this.http.post<KanbanByPo[]>(this.baseUrl + 'KanbanByPo', kanbanByPoParam, { observe: 'response', params })
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

  getDetailKanbanByPo(moNo: string, moSeq: string) {
    return this.http.get<KanbanByPoDetail[]>(this.baseUrl + 'KanbanByPo', {params: {moNo: moNo, moSeq: moSeq}});
  }

  getDetailKanbanByPoByReceivingType(moNo: string, moSeq: string, materialId: string) {
    return this.http.get<KanbanByPoDetailByReceivingType[]>(this.baseUrl + 'KanbanByPo/GetKanbanByPoDetailReceivingType', {params: {moNo: moNo, moSeq: moSeq, materialId: materialId}});
  }

  getLine() {
    return this.http.get<any[]>(this.baseUrl + 'KanbanByPo/getline');
  }

  getSupplier() {
    return this.http.get<any[]>(this.baseUrl + 'KanbanByPo/getsupplier');
  }

  exportExcelMain(dateType, dateStart, dateEnd, line, moNo, supplier, article, modelName) {
    const kanbanByPoParam = new KanbanByPoParam();
    kanbanByPoParam.article = article;
    kanbanByPoParam.dateType = parseInt(dateType);
    kanbanByPoParam.moNo = moNo;
    kanbanByPoParam.supplier = supplier;
    kanbanByPoParam.line = line;
    kanbanByPoParam.dateStart = dateStart;
    kanbanByPoParam.dateEnd = dateEnd;
    kanbanByPoParam.modelName = modelName;
    return this.http.post(this.baseUrl + 'KanbanByPo/Excel-main-summary', kanbanByPoParam, { responseType: 'blob' })
      .subscribe((result: Blob) => {
        if (result.type !== 'application/xlsx') {
          alert(result.type);
        }
        const blob = new Blob([result]);
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        const currentTime = new Date();
        const filename = 'Excel_KanBanByPoMain_' + currentTime.getFullYear().toString() +
          (currentTime.getMonth() + 1) + currentTime.getDate() +
          currentTime.toLocaleTimeString().replace(/[ ]|[,]|[:]/g, '').trim() + '.xlsx';
        link.href = url;
        link.setAttribute('download', filename);
        document.body.appendChild(link);
        link.click();
      });
  }

  exportExcelMainDetail(dateType, dateStart, dateEnd, line, moNo, supplier, article, modelName) {
    const kanbanByPoParam = new KanbanByPoParam();
    kanbanByPoParam.article = article;
    kanbanByPoParam.dateType = parseInt(dateType);
    kanbanByPoParam.moNo = moNo;
    kanbanByPoParam.supplier = supplier;
    kanbanByPoParam.line = line;
    kanbanByPoParam.dateStart = dateStart;
    kanbanByPoParam.dateEnd = dateEnd;
    kanbanByPoParam.modelName = modelName;
    return this.http.post(this.baseUrl + 'KanbanByPo/Excel-main-detail', kanbanByPoParam, { responseType: 'blob' })
      .subscribe((result: Blob) => {
        if (result.type !== 'application/xlsx') {
          alert(result.type);
        }
        const blob = new Blob([result]);
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        const currentTime = new Date();
        const filename = 'Excel_KanBanByPoMainDetail_' + currentTime.getFullYear().toString() +
          (currentTime.getMonth() + 1) + currentTime.getDate() +
          currentTime.toLocaleTimeString().replace(/[ ]|[,]|[:]/g, '').trim() + '.xlsx';
        link.href = url;
        link.setAttribute('download', filename);
        document.body.appendChild(link);
        link.click();
      });
  }

  exportExcelDetail(moNo: string, moSeq: string) {
    return this.http.get(this.baseUrl + 'KanbanByPo/Excel-Detail', { params: {moNo: moNo, moSeq: moSeq}, responseType: 'blob' })
      .subscribe((result: Blob) => {
        if (result.type !== 'application/xlsx') {
          alert(result.type);
        }
        const blob = new Blob([result]);
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        const currentTime = new Date();
        const filename = 'Excel_KanBanByPoDetail_' + currentTime.getFullYear().toString() +
          (currentTime.getMonth() + 1) + currentTime.getDate() +
          currentTime.toLocaleTimeString().replace(/[ ]|[,]|[:]/g, '').trim() + '.xlsx';
        link.href = url;
        link.setAttribute('download', filename);
        document.body.appendChild(link);
        link.click();
      });
  }

  exportExcelDetailWithSize(dateType, dateStart, dateEnd, line, moNo, article, modelName, moSeq) {
    const kanbanByPoParam = new KanbanByPoParam();
    kanbanByPoParam.article = article;
    kanbanByPoParam.dateType = parseInt(dateType);
    kanbanByPoParam.moNo = moNo;
    kanbanByPoParam.line = line;
    kanbanByPoParam.dateStart = dateStart;
    kanbanByPoParam.dateEnd = dateEnd;
    kanbanByPoParam.modelName = modelName;
    kanbanByPoParam.moSeq = moSeq;
    return this.http.post(this.baseUrl + 'KanbanByPo/Excel-main-detail', kanbanByPoParam, { responseType: 'blob' })
      .subscribe((result: Blob) => {
        if (result.type !== 'application/xlsx') {
          alert(result.type);
        }
        const blob = new Blob([result]);
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        const currentTime = new Date();
        const filename = 'Excel_KanBanByPoMainDetail_' + currentTime.getFullYear().toString() +
          (currentTime.getMonth() + 1) + currentTime.getDate() +
          currentTime.toLocaleTimeString().replace(/[ ]|[,]|[:]/g, '').trim() + '.xlsx';
        link.href = url;
        link.setAttribute('download', filename);
        document.body.appendChild(link);
        link.click();
      });
  }
}
