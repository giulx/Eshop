import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { CartModel } from '../models/cart.model';
import { AddCartItemRequest } from '../models/add-cart-item-request.model';
import { ChangeCartQuantityRequest } from '../models/change-cart-quantity-request.model';
import { RemoveCartItemRequest } from '../models/remove-cart-item-request.model';

@Injectable({ providedIn: 'root' })
export class CartApiService {
  private readonly http = inject(HttpClient);

  private readonly baseUrl = 'http://localhost:5138/api/cart';

  // può restituire null se il carrello non esiste o viene svuotato
  getMyCart(): Observable<CartModel | null> {
    return this.http.get<CartModel | null>(`${this.baseUrl}/mio`);
  }

  addItem(request: AddCartItemRequest): Observable<CartModel | null> {
    return this.http.post<CartModel | null>(`${this.baseUrl}/mio/item`, request);
  }

  changeQuantity(request: ChangeCartQuantityRequest): Observable<CartModel | null> {
    return this.http.put<CartModel | null>(`${this.baseUrl}/mio/item`, request);
  }

  removeItem(request: RemoveCartItemRequest): Observable<CartModel | null> {
    return this.http.request<CartModel | null>('DELETE', `${this.baseUrl}/mio/item`, {
      body: request,
    });
  }
}
