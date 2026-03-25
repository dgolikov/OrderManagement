import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../environments/environment';
import { Observable } from 'rxjs';
import { IPage } from '../models/responses/page.model';
import { IOrder } from '../models/responses/order.model';

@Injectable({
  providedIn: 'root',
})
export class OrdersService {
  private readonly _http: HttpClient = inject(HttpClient);
  private readonly _apiUrl: string = `${environment.apiUrl}/orders`;

  public getPage(pageNumber: number = 1, pageSize: number = 20): Observable<IPage<IOrder>> {
    const url = `${this._apiUrl}?pageNumber=${pageNumber}&pageSize=${pageSize}`;
    return this._http.get<IPage<IOrder>>(url);
  }
}
