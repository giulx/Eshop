import { Injectable, inject } from '@angular/core';
import { Router } from '@angular/router';

import { OrderApiService } from '../services/order-api.service';
import { OrderPreviewModel } from '../models/order-preview.model';
import { ConfirmOrderResultModel } from '../models/confirm-order-result.model';
import { AuthService } from '../../auth/services/auth.service';

@Injectable()
export class OrderPreviewVm {
  private readonly orderApi = inject(OrderApiService);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  preview: OrderPreviewModel | null = null;

  loading = false;
  confirming = false;

  error: string | null = null;

  confirmError: string | null = null;
  confirmSuccess: string | null = null;

  paymentError: string | null = null;

  payment = {
    cardHolder: '',
    cardNumber: '',
    expiryMonth: '',
    expiryYear: '',
    cvc: '',
  };

  init(): void {
    this.loadPreview();
  }

  loadPreview(): void {
    this.loading = true;
    this.error = null;
    this.confirmError = null;
    this.confirmSuccess = null;
    this.paymentError = null;
    this.preview = null;

    this.orderApi.getPreview().subscribe({
      next: (preview: OrderPreviewModel) => {
        this.loading = false;
        this.preview = preview;

        const hasDiscarded = !!preview.discardedRows?.length;
        this.confirmSuccess = hasDiscarded
          ? 'Alcuni articoli non sono ordinabili. Controlla il riepilogo prima di procedere.'
          : 'Preview ordine pronta. Inserisci i dati di pagamento per confermare.';
      },
      error: (err: any) => {
        this.loading = false;

        if (err.status === 401) {
          this.auth.logout();
          this.router.navigate(['/login'], { queryParams: { reason: 'order_preview' } });
          return;
        }

        if (err.status === 404 || err.error?.code === 'cart_not_found') {
          this.error = 'Carrello non trovato o vuoto. Torna al carrello.';
        } else {
          this.error = 'Errore durante la creazione della preview ordine.';
        }
      },
    });
  }

  private validatePayment(): boolean {
    const { cardHolder, cardNumber, expiryMonth, expiryYear, cvc } = this.payment;

    if (!cardHolder || !cardNumber || !expiryMonth || !expiryYear || !cvc) {
      this.paymentError = 'Compila tutti i dati di pagamento.';
      return false;
    }

    const digits = cardNumber.replace(/\s+/g, '');
    if (digits.length < 12) {
      this.paymentError = 'Numero di carta non valido (simulazione).';
      return false;
    }

    if (cvc.length < 3 || cvc.length > 4) {
      this.paymentError = 'CVC non valido (simulazione).';
      return false;
    }

    this.paymentError = null;
    return true;
  }

  confirm(): void {
    if (!this.preview?.validRows?.length) {
      this.confirmError = 'Non ci sono righe ordinabili. Torna al carrello.';
      return;
    }

    if (!this.validatePayment()) return;

    this.confirming = true;
    this.confirmError = null;

    this.orderApi.confirm().subscribe({
      next: (res: ConfirmOrderResultModel) => {
        this.confirming = false;

        if (!res.success) {
          const code = res.errorCode || 'unknown_error';

          switch (code) {
            case 'stock_changed':
              this.confirmError =
                'La disponibilità è cambiata. Torna al carrello e rifai la preview.';
              break;
            case 'payment_failed':
              this.confirmError = 'Pagamento non riuscito (simulazione backend).';
              break;
            case 'no_items_orderable':
              this.confirmError = 'Nessuna riga ordinabile nel carrello.';
              break;
            case 'cart_not_found':
              this.confirmError = 'Carrello non trovato.';
              break;
            default:
              this.confirmError = 'Impossibile confermare l’ordine.';
              break;
          }
          return;
        }

        const orderId = res.order?.id;

        this.confirmSuccess = orderId
          ? `Ordine confermato! Numero ordine: #${orderId}.`
          : 'Ordine confermato!';

        this.router.navigate(orderId ? ['/ordine', orderId] : ['/ordini']);
      },
      error: (err: any) => {
        this.confirming = false;

        if (err.status === 401) {
          this.auth.logout();
          this.router.navigate(['/login'], { queryParams: { reason: 'order_confirm' } });
          return;
        }

        this.confirmError = 'Errore durante la conferma dell’ordine.';
      },
    });
  }

  backToCart(): void {
    this.router.navigate(['/carrello']);
  }
}
