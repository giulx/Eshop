import { Injectable, inject } from '@angular/core';
import { BehaviorSubject, finalize } from 'rxjs';

import { ProductsApiService } from '../services/products-api.service';
import { ProductModel } from '../models/product.model';

@Injectable()
export class AdminProductListViewModel {
  private readonly api = inject(ProductsApiService);

  readonly products$ = new BehaviorSubject<ProductModel[]>([]);
  readonly loading$ = new BehaviorSubject<boolean>(false);
  readonly error$ = new BehaviorSubject<string | null>(null);

  init(): void {
    this.loadProducts();
  }

  loadProducts(): void {
    this.loading$.next(true);
    this.error$.next(null);

    this.api
      .getAll(null, 1, 1000)
      .pipe(finalize(() => this.loading$.next(false)))
      .subscribe({
        next: (res) => {
          this.products$.next(res.items);
        },
        error: () => {
          this.error$.next('Errore nel caricamento dei prodotti (admin).');
        },
      });
  }

  // Conferma eliminazione gestita dalla UI (modal custom), niente confirm() nativo qui
  deleteProduct(id: number): void {
    this.loading$.next(true);
    this.error$.next(null);

    this.api
      .deleteById(id)
      .pipe(finalize(() => this.loading$.next(false)))
      .subscribe({
        next: () => {
          this.loadProducts();
        },
        error: () => {
          this.error$.next('Errore durante l’eliminazione del prodotto.');
        },
      });
  }
}
