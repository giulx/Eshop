import { Injectable, inject } from '@angular/core';
import { Router } from '@angular/router';

import { CartApiService } from '../services/cart-api.service';
import { CartModel } from '../models/cart.model';
import { CartItemModel } from '../models/cart-item.model';
import { ChangeCartQuantityRequest } from '../models/change-cart-quantity-request.model';
import { RemoveCartItemRequest } from '../models/remove-cart-item-request.model';
import { AuthService } from '../../auth/services/auth.service';

@Injectable()
export class CartVm {
  private readonly cartApi = inject(CartApiService);
  private readonly router = inject(Router);
  private readonly auth = inject(AuthService);

  cart: CartModel | null = null;

  loading = false;
  updating = false;

  error: string | null = null;
  success: string | null = null;

  init(): void {
    this.loadCart();
  }

  private createEmptyCart(): CartModel {
    return {
      id: 0,
      customerId: 0,
      items: [],
      total: { value: 0, currency: 'EUR' },
    };
  }

  private loadCart(): void {
    this.loading = true;
    this.error = null;
    this.success = null;

    this.cartApi.getMyCart().subscribe({
      next: (cart: CartModel | null) => {
        this.loading = false;

        if (!cart || !cart.items || cart.items.length === 0) {
          this.cart = this.createEmptyCart();
          return;
        }

        this.cart = cart;
      },
      error: (err) => {
        this.loading = false;

        if (err.status === 401) {
          this.auth.logout();
          this.router.navigate(['/login'], { queryParams: { reason: 'cart' } });
          return;
        }

        if (err.status === 404) {
          this.cart = this.createEmptyCart();
          return;
        }

        this.error = 'Errore nel caricamento del carrello.';
      },
    });
  }

  hasItems(): boolean {
    return !!this.cart && this.cart.items.length > 0;
  }

  getTotalFormatted(): string {
    if (!this.cart) return '0,00 EUR';

    const t = this.cart.total;
    const value = t.value ?? 0;

    const formatted = value.toLocaleString('it-IT', {
      minimumFractionDigits: 2,
      maximumFractionDigits: 2,
    });

    return `${formatted} ${t.currency}`;
  }

  increaseQuantity(item: CartItemModel): void {
    const newQty = item.quantity + 1;
    this.changeQuantity(item.productId, newQty);
  }

  decreaseQuantity(item: CartItemModel): void {
    const newQty = item.quantity - 1;
    if (newQty <= 0) {
      this.removeItem(item);
      return;
    }
    this.changeQuantity(item.productId, newQty);
  }

  private changeQuantity(productId: number, quantity: number): void {
    if (!this.cart) return;

    this.updating = true;
    this.error = null;
    this.success = null;

    const body: ChangeCartQuantityRequest = { productId, quantity };

    this.cartApi.changeQuantity(body).subscribe({
      next: (updatedCart: CartModel | null) => {
        this.updating = false;

        if (!updatedCart || !updatedCart.items || updatedCart.items.length === 0) {
          this.cart = this.createEmptyCart();
        } else {
          this.cart = updatedCart;
        }

        this.success = 'Quantità aggiornata.';
      },
      error: (err) => {
        this.updating = false;

        if (err.status === 401) {
          this.auth.logout();
          this.router.navigate(['/login'], { queryParams: { reason: 'cart' } });
          return;
        }

        this.error =
          err.status === 404
            ? 'Carrello o articolo non trovati.'
            : 'Errore nell’aggiornamento della quantità.';
      },
    });
  }

  removeItem(item: CartItemModel): void {
    if (!this.cart) return;

    this.updating = true;
    this.error = null;
    this.success = null;

    const body: RemoveCartItemRequest = { productId: item.productId };

    this.cartApi.removeItem(body).subscribe({
      next: (updatedCart: CartModel | null) => {
        this.updating = false;

        if (!updatedCart || !updatedCart.items || updatedCart.items.length === 0) {
          this.cart = this.createEmptyCart();
        } else {
          this.cart = updatedCart;
        }

        this.success = 'Articolo rimosso dal carrello.';
      },
      error: (err) => {
        this.updating = false;

        if (err.status === 401) {
          this.auth.logout();
          this.router.navigate(['/login'], { queryParams: { reason: 'cart' } });
          return;
        }

        this.error =
          err.status === 404
            ? 'Carrello o articolo non trovati.'
            : 'Errore nella rimozione dell’articolo dal carrello.';
      },
    });
  }

  proceedToCheckout(): void {
    if (!this.hasItems()) {
      this.error = 'Il carrello è vuoto.';
      return;
    }

    this.router.navigate(['/ordine/preview']);
  }
}
