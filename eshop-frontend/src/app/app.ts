// src/app/app.ts
import { CommonModule } from '@angular/common';
import { Component, computed, inject } from '@angular/core';
import { Router, RouterModule, RouterOutlet } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';

import { AuthService } from './features/auth/services/auth.service';
import type { AuthUser } from './features/auth/models/auth-user.model';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule, // direttive/pipes base (es. @if, @for, date/number pipes, ecc.)
    RouterOutlet, // <router-outlet />
    RouterModule, // routerLink / routerLinkActive
  ],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App {
  /** AuthService usato come sorgente (RxJS) → convertito in Signals per template moderno. */
  private readonly authService = inject(AuthService);

  /** Router per redirect esplicito post-logout. */
  private readonly router = inject(Router);

  /**
   * Utente autenticato (null se guest).
   * Nota: usiamo AuthUser (non UserModel) per coerenza con AuthService.
   */
  readonly user = toSignal<AuthUser | null>(this.authService.user$, {
    initialValue: null,
  });

  /** Flag admin derivato dall'utente corrente (Signal). */
  readonly isAdmin = computed(() => !!this.user()?.isAdmin);

  /**
   * Logout: pulizia stato auth + redirect alla login.
   * queryParams utile per mostrare un messaggio ("sei uscito correttamente") nella login page.
   */
  onLogout(): void {
    this.authService.logout();
    this.router.navigate(['/login'], {
      queryParams: { reason: 'logout' },
    });
  }
}
