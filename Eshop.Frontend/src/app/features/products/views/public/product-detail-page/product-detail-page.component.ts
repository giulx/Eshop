import { CommonModule } from '@angular/common';
import { Component, DestroyRef, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

import { ProductsApiService } from '../../../services/products-api.service';
import { ProductModel } from '../../../models/product.model';

import { CartApiService } from '../../../../cart/services/cart-api.service';
import { AddCartItemRequest } from '../../../../cart/models/add-cart-item-request.model';

import { AuthService } from '../../../../auth/services/auth.service';

@Component({
  selector: 'app-product-detail-page',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './product-detail-page.component.html',
  styleUrls: ['./product-detail-page.component.css'],
})
export class ProductDetailPageComponent implements OnInit {
  // Router / route
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  // API
  private readonly productsApi = inject(ProductsApiService);
  private readonly cartApi = inject(CartApiService);

  // Auth
  private readonly auth = inject(AuthService);

  // Destroy lifecycle (unsubscribe + cleanup)
  private readonly destroyRef = inject(DestroyRef);

  // =========================
  // STATE (Signals)
  // =========================
  readonly product = signal<ProductModel | null>(null);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  readonly adding = signal(false);
  readonly addError = signal<string | null>(null);
  readonly addSuccess = signal<string | null>(null);

  // Toast overlay
  readonly toastMessage = signal<string | null>(null);
  readonly toastType = signal<'success' | 'error'>('success');

  private toastTimeoutId: ReturnType<typeof setTimeout> | null = null;

  ngOnInit(): void {
    // Cleanup timer quando la view viene distrutta (evita memory leak)
    this.destroyRef.onDestroy(() => {
      if (this.toastTimeoutId) clearTimeout(this.toastTimeoutId);
    });

    // Leggo l'id dalla rotta (/:id)
    const idParam = this.route.snapshot.paramMap.get('id');
    const id = idParam ? Number(idParam) : NaN;

    if (!Number.isFinite(id) || id <= 0) {
      this.error.set('ID prodotto non valido.');
      return;
    }

    this.loadProduct(id);
  }

  /**
   * Carica il dettaglio prodotto dal backend.
   * Gestisce loading/error e aggiorna la view tramite signals.
   */
  private loadProduct(id: number): void {
    this.loading.set(true);
    this.error.set(null);
    this.product.set(null);

    this.productsApi
      .getById(id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (p) => {
          this.product.set(p);
          this.loading.set(false);
        },
        error: () => {
          this.error.set('Prodotto non trovato o errore nel caricamento.');
          this.loading.set(false);
        },
      });
  }

  /**
   * Aggiunge 1 unità del prodotto al carrello.
   * - Se non autenticata → redirect login con returnUrl
   * - Se stock 0 → blocco lato frontend (UX)
   * - Gestione errori 401 e generici
   */
  onAddToCart(): void {
    // reset messaggi
    this.addError.set(null);
    this.addSuccess.set(null);

    // ✅ Angular 17+ signal auth (NO getUserSnapshot)
    const currentUser = this.auth.user(); // AuthUser | null
    if (!currentUser) {
      this.showToast('Per aggiungere al carrello devi prima effettuare il login.', 'error');
      this.router.navigate(['/login'], {
        queryParams: { returnUrl: this.router.url, reason: 'cart' },
      });
      return;
    }

    const p = this.product();
    if (!p || p.availableQuantity <= 0) {
      this.addError.set('Prodotto non disponibile.');
      this.showToast('Prodotto non disponibile.', 'error');
      return;
    }

    const body: AddCartItemRequest = {
      productId: p.id,
      quantity: 1,
    };

    this.adding.set(true);

    this.cartApi
      .addItem(body)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.adding.set(false);
          this.addSuccess.set('Prodotto aggiunto al carrello.');
          this.showToast('Prodotto aggiunto al carrello.', 'success');
        },
        error: (err) => {
          this.adding.set(false);

          // Token scaduto / non valido
          if (err?.status === 401) {
            this.addError.set('Devi essere loggata per aggiungere al carrello.');
            this.showToast('Per aggiungere al carrello devi effettuare il login.', 'error');
            this.router.navigate(['/login'], {
              queryParams: { returnUrl: this.router.url, reason: 'cart' },
            });
            return;
          }

          this.addError.set('Errore durante l’aggiunta al carrello.');
          this.showToast('Errore durante l’aggiunta al carrello.', 'error');
        },
      });
  }

  /**
   * Toast “non bloccante” (overlay).
   * Timer resettato a ogni chiamata.
   */
  private showToast(message: string, type: 'success' | 'error' = 'success'): void {
    this.toastMessage.set(message);
    this.toastType.set(type);

    if (this.toastTimeoutId) clearTimeout(this.toastTimeoutId);

    this.toastTimeoutId = setTimeout(() => {
      this.toastMessage.set(null);
      this.toastTimeoutId = null;
    }, 2500);
  }
}
