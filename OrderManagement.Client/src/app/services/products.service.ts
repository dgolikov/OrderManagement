import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../environments/environment';
import { Observable } from 'rxjs';
import { IPage } from '../models/responses/page.model';
import { IProduct } from '../models/responses/product.model';

@Injectable({
  providedIn: 'root',
})
export class ProductService {
  private readonly _http: HttpClient = inject(HttpClient);
  private readonly _apiUrl: string = `${environment.apiUrl}/products`;

  public getPage(pageNumber: number = 1, pageSize: number = 20, searchTerm?: string): Observable<IPage<IProduct>> {
    let url = `${this._apiUrl}?pageNumber=${pageNumber}&pageSize=${pageSize}`;
    if (searchTerm) {
      url += `&searchTerm=${encodeURIComponent(searchTerm)}`;
    }
    return this._http.get<IPage<IProduct>>(url);
  }
}
