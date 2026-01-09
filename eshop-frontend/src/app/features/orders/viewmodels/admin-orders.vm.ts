import { Injectable, inject } from '@angular/core';
import { Router } from '@angular/router';

import { OrderApiService } from '../services/order-api.service';
import { OrderReadModel } from '../models/order-read.model';
import { AuthService } from '../../auth/services/auth.service';

type SearchMode = 'orderId' | 'customerId';

@Injectable()
export class AdminOrdersVm {
  private readonly orderApi = inject(OrderApiService);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  orders: OrderReadModel[] = [];

  loading = false;
  error: string | null = null;
  success: string | null = null;

  /** id dell'ordine su cui sto eseguendo un'azione (per disabilitare bottoni) */
  workingOrderId: number | null = null;

  searchMode: SearchMode = 'orderId';
  searchText = '';
  statusFilter = 'all';

  init(): void {
    this.loadOrders();
  }

  private loadOrders(): void {
    this.loading = true;
    this.error = null;
    this.success = null;

    this.orderApi.getAllOrders().subscribe({
      next: (orders: OrderReadModel[]) => {
        this.loading = false;
        this.orders = orders;
      },
      error: (err: any) => {
        this.loading = false;

        if (err.status === 401 || err.status === 403) {
          this.auth.logout();
          this.router.navigate(['/login'], { queryParams: { reason: 'admin_orders' } });
          return;
        }

        this.error = 'Errore nel caricamento degli ordini.';
      },
    });
  }

  updateSearchMode(value: string): void {
    if (value === 'orderId' || value === 'customerId') {
      this.searchMode = value;
    }
  }

  updateSearchText(value: string): void {
    this.searchText = value;
  }

  updateStatusFilter(value: string): void {
    this.statusFilter = value;
  }

  clearFilters(): void {
    this.searchText = '';
    this.statusFilter = 'all';
    this.searchMode = 'orderId';
  }

  get filteredOrders(): OrderReadModel[] {
    let result = [...this.orders];

    const text = this.searchText.trim();
    if (text) {
      const needle = text.toLowerCase();

      result = result.filter(order => {
        if (this.searchMode === 'orderId') {
          return order.id.toString().toLowerCase().includes(needle);
        }
        return order.customerId.toString().toLowerCase().includes(needle);
      });
    }

    if (this.statusFilter !== 'all') {
      const statusNumber = Number(this.statusFilter);
      result = result.filter(o => o.status === statusNumber);
    }

    return result;
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

  private setWorking(id: number | null): void {
    this.workingOrderId = id;
    this.error = null;
    this.success = null;
  }

  /** Admin: consentito solo se ordine è Pagato (0) */
  canMarkAsProcessing(order: OrderReadModel): boolean {
    return order.status === 0;
  }

  /** Admin: consentito solo se ordine è In elaborazione (1) */
  canMarkAsShipped(order: OrderReadModel): boolean {
    return order.status === 1;
  }

  /** Admin: consentito se ordine è Pagato (0) o In elaborazione (1) */
  canCancel(order: OrderReadModel): boolean {
    return order.status === 0 || order.status === 1;
  }

  markAsProcessing(order: OrderReadModel): void {
    if (!this.canMarkAsProcessing(order)) return;

    this.setWorking(order.id);

    this.orderApi.adminMarkAsProcessing(order.id).subscribe({
      next: () => {
        this.setWorking(null);
        order.status = 1; // Processing
        this.success = `Ordine #${order.id} messo in elaborazione.`;
      },
      error: (err: any) => {
        this.setWorking(null);

        if (err.status === 401 || err.status === 403) {
          this.auth.logout();
          this.router.navigate(['/login'], { queryParams: { reason: 'admin_orders' } });
          return;
        }

        this.error = 'Impossibile cambiare lo stato in "in elaborazione".';
      },
    });
  }

  markAsShipped(order: OrderReadModel): void {
    if (!this.canMarkAsShipped(order)) return;

    this.setWorking(order.id);

    this.orderApi.adminMarkAsShipped(order.id).subscribe({
      next: () => {
        this.setWorking(null);
        order.status = 2; // Shipped
        this.success = `Ordine #${order.id} segnato come spedito.`;
      },
      error: (err: any) => {
        this.setWorking(null);

        if (err.status === 401 || err.status === 403) {
          this.auth.logout();
          this.router.navigate(['/login'], { queryParams: { reason: 'admin_orders' } });
          return;
        }

        this.error = 'Impossibile segnare l’ordine come spedito.';
      },
    });
  }

  cancelOrder(order: OrderReadModel): void {
    if (!this.canCancel(order)) return;

    this.setWorking(order.id);

    this.orderApi.adminCancel(order.id).subscribe({
      next: () => {
        this.setWorking(null);
        order.status = 3; // Cancelled
        this.success = `Ordine #${order.id} annullato.`;
      },
      error: (err: any) => {
        this.setWorking(null);

        if (err.status === 401 || err.status === 403) {
          this.auth.logout();
          this.router.navigate(['/login'], { queryParams: { reason: 'admin_orders' } });
          return;
        }

        this.error = 'Impossibile annullare l’ordine.';
      },
    });
  }

  deleteHard(order: OrderReadModel): void {
    if (!confirm(`Vuoi davvero ELIMINARE definitivamente l’ordine #${order.id}?`)) return;

    this.setWorking(order.id);

    this.orderApi.adminDeleteHard(order.id).subscribe({
      next: () => {
        this.setWorking(null);
        this.orders = this.orders.filter(o => o.id !== order.id);
        this.success = `Ordine #${order.id} eliminato definitivamente.`;
      },
      error: (err: any) => {
        this.setWorking(null);

        if (err.status === 401 || err.status === 403) {
          this.auth.logout();
          this.router.navigate(['/login'], { queryParams: { reason: 'admin_orders' } });
          return;
        }

        this.error = 'Impossibile eliminare definitivamente l’ordine.';
      },
    });
  }

  isWorkingOn(order: OrderReadModel): boolean {
    return this.workingOrderId === order.id;
  }
}
