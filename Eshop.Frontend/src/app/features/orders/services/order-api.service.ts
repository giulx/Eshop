import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { OrderPreviewModel } from '../models/order-preview.model';
import { ConfirmOrderResultModel } from '../models/confirm-order-result.model';
import { OrderReadModel } from '../models/order-read.model';

interface AdminOrderActionResponse {
  message: string;
  code: string;
}

@Injectable({ providedIn: 'root' })
export class OrderApiService {
  private readonly http = inject(HttpClient);

  private readonly baseUrl = '/api/order';

  // cliente – preview / conferma
  getPreview(): Observable<OrderPreviewModel> {
    return this.http.get<OrderPreviewModel>(`${this.baseUrl}/preview`);
  }

  confirm(): Observable<ConfirmOrderResultModel> {
    return this.http.post<ConfirmOrderResultModel>(`${this.baseUrl}/conferma`, {});
  }

  // cliente – i miei ordini
  getMyOrders(): Observable<OrderReadModel[]> {
    return this.http.get<OrderReadModel[]>(`${this.baseUrl}/miei`);
  }

  getMyOrderById(id: number): Observable<OrderReadModel> {
    return this.http.get<OrderReadModel>(`${this.baseUrl}/${id}`);
  }

  cancelMyOrder(id: number): Observable<AdminOrderActionResponse> {
    return this.http.delete<AdminOrderActionResponse>(`${this.baseUrl}/${id}`);
  }

  // admin – ordini
  getAllOrders(): Observable<OrderReadModel[]> {
    return this.http.get<OrderReadModel[]>(`${this.baseUrl}`);
  }

  adminMarkAsProcessing(id: number): Observable<AdminOrderActionResponse> {
    return this.http.put<AdminOrderActionResponse>(
      `${this.baseUrl}/admin/${id}/processing`,
      {}
    );
  }

  adminMarkAsShipped(id: number): Observable<AdminOrderActionResponse> {
    return this.http.put<AdminOrderActionResponse>(
      `${this.baseUrl}/admin/${id}/shipped`,
      {}
    );
  }

  adminCancel(id: number): Observable<AdminOrderActionResponse> {
    return this.http.delete<AdminOrderActionResponse>(
      `${this.baseUrl}/admin/${id}`
    );
  }

  adminDeleteHard(id: number): Observable<AdminOrderActionResponse> {
    return this.http.delete<AdminOrderActionResponse>(
      `${this.baseUrl}/admin/${id}/hard`
    );
  }
}
