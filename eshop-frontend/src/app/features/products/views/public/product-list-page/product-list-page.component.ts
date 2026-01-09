import { CommonModule } from '@angular/common';
import { Component, DestroyRef, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

import { ProductsApiService } from '../../../services/products-api.service';
import { ProductModel } from '../../../models/product.model';

@Component({
  selector: 'app-product-list-page',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './product-list-page.component.html',
  styleUrls: ['./product-list-page.component.css'],
})
export class ProductListPageComponent {
  private readonly productsApi = inject(ProductsApiService);
  private readonly destroyRef = inject(DestroyRef);

  readonly products = signal<ProductModel[]>([]);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  readonly page = signal(1);
  readonly pageSize = signal(20);
  readonly total = signal(0);

  readonly search = signal('');

  constructor() {
    this.loadProducts();
  }

  private loadProducts(): void {
    this.loading.set(true);
    this.error.set(null);

    this.productsApi
      .getAll(this.search(), this.page(), this.pageSize())
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (res) => {
          this.products.set(res.items);
          this.page.set(res.page);
          this.pageSize.set(res.pageSize);
          this.total.set(res.total);
          this.loading.set(false);
        },
        error: (err) => {
          console.error('Errore HTTP getAll:', err);
          this.error.set('Errore nel caricamento dei prodotti.');
          this.loading.set(false);
        },
      });
  }

  onSearch(): void {
    this.page.set(1);
    this.loadProducts();
  }

  changePage(newPage: number): void {
    if (newPage === this.page()) return;
    this.page.set(newPage);
    this.loadProducts();
  }
}
