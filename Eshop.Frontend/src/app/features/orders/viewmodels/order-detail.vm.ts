import { Injectable, inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

import { OrderApiService } from '../services/order-api.service';
import { OrderReadModel } from '../models/order-read.model';
import { AuthService } from '../../auth/services/auth.service';

@Injectable()
export class OrderDetailVm {
  private readonly orderApi = inject(OrderApiService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly auth = inject(AuthService);

  order: OrderReadModel | null = null;

  loading = false;
  error: string | null = null;

  init(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    const id = idParam ? Number(idParam) : NaN;

    if (!id || isNaN(id)) {
      this.error = 'Ordine non valido.';
      return;
    }

    this.loadOrder(id);
  }

  private loadOrder(id: number): void {
    this.loading = true;
    this.error = null;
    this.order = null;

    this.orderApi.getMyOrderById(id).subscribe({
      next: (order: OrderReadModel) => {
        this.loading = false;
        this.order = order;
      },
      error: (err: any) => {
        this.loading = false;

        if (err.status === 401) {
          this.auth.logout();
          this.router.navigate(['/login'], { queryParams: { reason: 'order_detail' } });
          return;
        }

        this.error =
          err.status === 404
            ? 'Ordine non trovato.'
            : 'Errore nel caricamento del dettaglio ordine.';
      },
    });
  }

  getStatusLabel(status: number): string {
    switch (status) {
      case 0:
        return 'Pagato';
      case 1:
        return 'In elaborazione';
      case 2:
        return 'Spedito';
      case 3:
        return 'Annullato';
      default:
        return `Stato ${status}`;
    }
  }

  backToOrders(): void {
    this.router.navigate(['/ordini']);
  }
}
