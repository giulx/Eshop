import { Injectable, inject } from '@angular/core';
import { Router } from '@angular/router';

import { OrderApiService } from '../services/order-api.service';
import { OrderReadModel } from '../models/order-read.model';
import { AuthService } from '../../auth/services/auth.service';

@Injectable()
export class MyOrdersVm {
  private readonly orderApi = inject(OrderApiService);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  orders: OrderReadModel[] = [];

  loading = false;
  error: string | null = null;
  success: string | null = null;

  // id dell’ordine che sto annullando (per disabilitare solo quel bottone)
  private cancellingOrderId: number | null = null;

  init(): void {
    this.loadOrders();
  }

  private loadOrders(): void {
    this.loading = true;
    this.error = null;
    this.success = null;

    this.orderApi.getMyOrders().subscribe({
      next: (orders: OrderReadModel[]) => {
        this.loading = false;
        this.orders = orders;
      },
      error: (err: any) => {
        this.loading = false;

        if (err.status === 401) {
          this.auth.logout();
          this.router.navigate(['/login'], { queryParams: { reason: 'my_orders' } });
          return;
        }

        this.error = 'Errore nel caricamento dei tuoi ordini.';
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

  canCancel(order: OrderReadModel): boolean {
    return order.status === 0;
  }

  isCancelling(order: OrderReadModel): boolean {
    return this.cancellingOrderId === order.id;
  }

  cancel(order: OrderReadModel): void {
    if (!this.canCancel(order)) return;

    this.cancellingOrderId = order.id;
    this.error = null;
    this.success = null;

    this.orderApi.cancelMyOrder(order.id).subscribe({
      next: () => {
        this.cancellingOrderId = null;
        this.success = 'Ordine annullato correttamente.';
        this.loadOrders();
      },
      error: (err: any) => {
        this.cancellingOrderId = null;

        if (err.status === 401) {
          this.auth.logout();
          this.router.navigate(['/login'], { queryParams: { reason: 'cancel_order' } });
          return;
        }

        if (err.error?.code === 'cannot_cancel') {
          this.error = 'Questo ordine non è più annullabile.';
        } else {
          this.error = 'Errore durante l’annullamento dell’ordine.';
        }
      },
    });
  }
}
