import { Injectable, inject, signal } from '@angular/core';
import { finalize } from 'rxjs';

import { ProductsApiService } from '../services/products-api.service';
import { ProductModel } from '../models/product.model';

@Injectable()
export class ProductListViewModel {

  private readonly api = inject(ProductsApiService);

  // ======================
  // STATE
  // ======================
  readonly products = signal<ProductModel[]>([]);
  readonly loading  = signal(false);
  readonly error    = signal<string | null>(null);

  readonly page     = signal(1);
  readonly pageSize = signal(20);
  readonly total    = signal(0);

  private currentSearch: string | null = null;

  // ======================
  // INIT
  // ======================
  init(): void {
    this.loadPage(1);
  }

  // ======================
  // SEARCH
  // ======================
  setSearch(search: string | null): void {
    const normalized = search?.trim() || null;

    this.currentSearch = normalized;
    this.loadPage(1);
  }

  // ======================
  // LOAD PAGE
  // ======================
  loadPage(page: number): void {
    this.loading.set(true);
    this.error.set(null);

    const pageSize = this.pageSize();

    this.api
      .getAll(this.currentSearch, page, pageSize)
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: res => {
          this.products.set(res.items);
          this.page.set(res.page);
          this.pageSize.set(res.pageSize);
          this.total.set(res.total);
        },
        error: () => {
          this.error.set('Errore nel caricamento dei prodotti.');
        }
      });
  }

  // ======================
  // PAGINATION HELPERS
  // ======================
  nextPage(): void {
    const page = this.page();
    const pageSize = this.pageSize();
    const total = this.total();

    if (page * pageSize < total) {
      this.loadPage(page + 1);
    }
  }

  prevPage(): void {
    const page = this.page();

    if (page > 1) {
      this.loadPage(page - 1);
    }
  }

  // ======================
  // TEMPLATE HELPERS
  // ======================
  hasNextPage(): boolean {
    return this.page() * this.pageSize() < this.total();
  }

  hasPrevPage(): boolean {
    return this.page() > 1;
  }
}
