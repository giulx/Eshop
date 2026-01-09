import { Injectable, inject, signal } from '@angular/core';
import { finalize } from 'rxjs';

import { ProductsApiService } from '../services/products-api.service';
import { ProductModel } from '../models/product.model';

@Injectable()
export class ProductDetailViewModel {
  private readonly api = inject(ProductsApiService);

  readonly product = signal<ProductModel | null>(null);
  readonly loading = signal(false);
  readonly error   = signal<string | null>(null);

  loadProduct(id: number): void {
    this.loading.set(true);
    this.error.set(null);

    this.api
      .getById(id)
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (p) => this.product.set(p),
        error: () => {
          this.error.set('Prodotto non trovato o errore nel caricamento.');
          this.product.set(null);
        }
      });
  }
}
