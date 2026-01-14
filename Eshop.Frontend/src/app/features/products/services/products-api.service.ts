import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { PagedResult } from '../../../shared/models/paged-result.model';
import {
  ProductModel,
  ProductCreateRequest,
  ProductUpdateRequest,
} from '../models/product.model';

@Injectable({ providedIn: 'root' })
export class ProductsApiService {
  private readonly http = inject(HttpClient);

  private readonly baseUrl = '/api/v1/Product';

  getAll(
    search: string | null,
    page = 1,
    pageSize = 20
  ): Observable<PagedResult<ProductModel>> {
    let params = new HttpParams().set('page', page).set('pageSize', pageSize);

    const trimmed = search?.trim();
    if (trimmed) {
      params = params.set('search', trimmed);
    }

    return this.http.get<PagedResult<ProductModel>>(this.baseUrl, { params });
  }

  getById(id: number): Observable<ProductModel> {
    return this.http.get<ProductModel>(`${this.baseUrl}/${id}`);
  }

  create(body: ProductCreateRequest): Observable<ProductModel> {
    return this.http.post<ProductModel>(this.baseUrl, body);
  }

  update(id: number, body: ProductUpdateRequest): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, body);
  }

  deleteById(id: number): Observable<{ message: string }> {
    return this.http.delete<{ message: string }>(`${this.baseUrl}/${id}`);
  }

  deleteAll(): Observable<{ message: string }> {
    return this.http.delete<{ message: string }>(this.baseUrl);
  }
}
