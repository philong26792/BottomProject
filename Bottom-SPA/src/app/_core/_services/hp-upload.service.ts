import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class HpUploadService {
  baseUrl = environment.apiUrl;
  constructor(private http: HttpClient) { }
  getInfomartionHpUpload() {
    return this.http.get<any>(this.baseUrl + 'HPUploadTimeLog', {});
  }
}
