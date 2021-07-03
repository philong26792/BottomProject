import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { MaterialSearch } from '../_viewmodels/material-search';
import { Observable, BehaviorSubject } from 'rxjs';
import { PaginatedResult } from '../_models/pagination';
import { map } from 'rxjs/operators';
import { MaterialModel } from '../_viewmodels/material-model';
import { ReceiveNoMain } from '../_viewmodels/receive_no_main';
import { ReceiveNoDetail } from '../_viewmodels/receive-no-detail';
import { MaterialEditModel } from '../_viewmodels/material-edit-model';
import { OrderSizeByBatch } from '../_viewmodels/order-size-by-batch';

@Injectable({
  providedIn: 'root'
})
export class MaterialService {
  baseUrl = environment.apiUrl;
  materialSource = new BehaviorSubject<MaterialModel>(null);
  receiveNoMainSource = new BehaviorSubject<ReceiveNoMain>(null);
  receiveNoDetailSource = new BehaviorSubject<ReceiveNoDetail[]>([]);
  materialSearchSource = new BehaviorSubject<MaterialSearch>(null);
  currentMaterial = this.materialSource.asObservable();
  currentReceiveNoDetail = this.receiveNoDetailSource.asObservable();
  currentReceiveNoMainItem = this.receiveNoMainSource.asObservable();
  currentMaterialSearch = this.materialSearchSource.asObservable();
  constructor(private http: HttpClient) { }
  search(page?, itemsPerPage?, text?: MaterialSearch): Observable<PaginatedResult<MaterialModel[]>> {
    const paginatedResult: PaginatedResult<MaterialModel[]> = new PaginatedResult<MaterialModel[]>();

    let params = new HttpParams();

    if (page != null && itemsPerPage != null) {
      params = params.append('pageNumber', page);
      params = params.append('pageSize', itemsPerPage);
    }
    return this.http.post<any>(this.baseUrl + 'receiving/search/', text, { observe: 'response', params })
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

  changeMaterialModel(materialModel: MaterialModel) {
    this.materialSource.next(materialModel);
  }
  changeReceiveNoMainItem(receiveNoMain: ReceiveNoMain) {
    this.receiveNoMainSource.next(receiveNoMain);
  }
  changeReceiveNoDetail(receiveNoDetails: ReceiveNoDetail[]) {
    this.receiveNoDetailSource.next(receiveNoDetails);
  }
  changeMaterialSearch(param: MaterialSearch) {
    this.materialSearchSource.next(param);
  }
  searchByPurchase(model: MaterialModel) {
    return this.http.post<any>(this.baseUrl + 'receiving/searchTable/', model);
  }
  updateMaterial(model: OrderSizeByBatch[]): Observable<ReceiveNoMain[]> {
    return this.http.post<any>(this.baseUrl + 'receiving/updateMaterial/', model);
  }
  receiveNoDetails(receiveNo: any): Observable<ReceiveNoDetail[]> {
    return this.http.get<ReceiveNoDetail[]>(this.baseUrl + 'receiving/receiveNoDetails/' + receiveNo, {});
  }
  receiveNoMain(materialModel: MaterialModel): Observable<ReceiveNoMain[]> {
    return this.http.post<any>(this.baseUrl + 'receiving/receiveNoMain', materialModel);
  }

  // Đóng purchase
  closePurchase(materialModel: MaterialModel) {
    return this.http.post<any>(this.baseUrl + 'receiving/closePurchase/', materialModel);
  }
  receiveByPurchase(materialModel: MaterialModel): Observable<ReceiveNoMain[]> {
    return this.http.post<any>(this.baseUrl + 'receiving/receiveByPurchase/', materialModel);
  }

  // kiểm tra status của purchaseNo.
  statusPurchase(materialModel: MaterialModel){
    return this.http.post<any>(this.baseUrl + 'receiving/statusPurchase/', materialModel);
  }

  editMaterial(receiveNoMain: ReceiveNoMain): Observable<MaterialEditModel[]> {
    return this.http.post<any>(this.baseUrl + 'receiving/editMaterial/', receiveNoMain);
  }
  
  editDetail(materialEditModels: MaterialEditModel[]) {
    return this.http.post<any>(this.baseUrl + 'receiving/editDetail/', materialEditModels);
  }
  getDMONo(moNo: string) {
    let params = new HttpParams();
    params = params.append('moNo', moNo);
    return this.http.get<any>(this.baseUrl + 'receiving/getDMONo', {params});
  }
  checkInputDelivery(supplier_ID: string) {
    return this.http.get<boolean>(this.baseUrl + 'Receiving/checkInputDelivery', {params: {supplier_ID:supplier_ID}});
  }
}
