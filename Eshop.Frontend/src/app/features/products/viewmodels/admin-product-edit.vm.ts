import { Injectable, inject } from '@angular/core';
import { BehaviorSubject, finalize } from 'rxjs';

import { ProductsApiService } from '../services/products-api.service';
import {
  ProductModel,
  ProductCreateRequest,
  ProductUpdateRequest,
} from '../models/product.model';

@Injectable()
export class AdminProductEditViewModel {
  readonly product$ = new BehaviorSubject<ProductModel | null>(null);
  readonly loading$ = new BehaviorSubject<boolean>(false);
  readonly error$ = new BehaviorSubject<string | null>(null);
  readonly success$ = new BehaviorSubject<string | null>(null);

  readonly currencies: ReadonlyArray<{ code: string; label: string }> = [
    { code: 'EUR', label: 'Euro (€)' },
    { code: 'USD', label: 'Dollaro (USD)' },
    { code: 'GBP', label: 'Sterlina (GBP)' },
  ];

  private readonly api = inject(ProductsApiService);

  private currentId: number | null = null;

  initForCreate(): void {
    this.currentId = null;
    this.error$.next(null);
    this.success$.next(null);

    this.product$.next({
      id: 0,
      name: '',
      description: null,
      price: 0,
      currency: 'EUR',
      availableQuantity: 0,
    });
  }

  initForEdit(id: number): void {
    this.currentId = id;
    this.loadProduct(id);
  }

  private loadProduct(id: number): void {
    this.loading$.next(true);
    this.error$.next(null);
    this.success$.next(null);

    this.api
      .getById(id)
      .pipe(finalize(() => this.loading$.next(false)))
      .subscribe({
        next: (p) => {
          this.product$.next(this.normalizeProduct(p));
        },
        error: () => {
          this.error$.next('Prodotto non trovato o errore nel caricamento.');
        },
      });
  }

  save(formValue: {
    name: string;
    description?: string | null;
    price: number;
    currency: string;
    availableQuantity: number;
  }): void {
    this.loading$.next(true);
    this.error$.next(null);
    this.success$.next(null);

    const name = (formValue.name ?? '').trim();
    const description = (formValue.description ?? '').trim() || null;

    // NOTE: i valori della form possono arrivare come string (es. "0.01" o "0,01")
    const priceRaw = (formValue as any).price;
    const qtyRaw = (formValue as any).availableQuantity;

    const price = Number(String(priceRaw ?? '').trim().replace(',', '.'));
    const currency = (formValue.currency ?? 'EUR').trim().toUpperCase();
    const availableQuantity = Number(String(qtyRaw ?? '').trim());

    if (this.currentId == null) {
      const body: ProductCreateRequest = {
        name,
        description,
        price,
        currency,
        availableQuantity,
      };

      this.api
        .create(body)
        .pipe(finalize(() => this.loading$.next(false)))
        .subscribe({
          next: (created) => {
            this.success$.next(`Prodotto creato con ID ${created.id}.`);

            const normalized = this.normalizeProduct(created);
            this.product$.next(normalized);
            this.currentId = created.id;
          },
          error: () => {
            this.error$.next('Errore durante la creazione del prodotto.');
          },
        });

      return;
    }

    const body: ProductUpdateRequest = {
      name,
      description,
      price,
      currency,
      availableQuantity,
    };

    this.api
      .update(this.currentId, body)
      .pipe(finalize(() => this.loading$.next(false)))
      .subscribe({
        next: () => {
          this.success$.next('Prodotto aggiornato con successo.');
          this.loadProduct(this.currentId!);
        },
        error: () => {
          this.error$.next('Errore durante l’aggiornamento del prodotto.');
        },
      });
  }

  private normalizeProduct(p: ProductModel): ProductModel {
    const currency = p.currency?.trim().toUpperCase();
    return {
      ...p,
      currency: currency ? currency : 'EUR',
    };
  }
}
