// src/app/features/auth/services/auth.service.ts

import { Injectable, computed, inject, signal } from '@angular/core';
import { BehaviorSubject, Observable, map, tap } from 'rxjs';

import { AuthApiService } from './auth-api.service';
import { AuthUser } from '../models/auth-user.model';
import { LoginRequest } from '../models/login-request.model';
import { LoginResult } from '../models/login-result.model';

const STORAGE_TOKEN_KEY = 'eshop_token';
const STORAGE_USER_KEY = 'eshop_user';

/**
 * AuthService
 * - Espone sia API RxJS (Observable) sia API Signals (Angular 17+).
 * - Mantiene "snapshot helpers" per casi sincroni (guard/interceptor).
 * - La fonte di verità è la coppia tokenSubject/userSubject, sincronizzata con i signals.
 */
@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly authApi = inject(AuthApiService);

  // =========================
  // State interno (fonte di verità)
  // =========================
  private readonly tokenSubject = new BehaviorSubject<string | null>(null);
  private readonly userSubject = new BehaviorSubject<AuthUser | null>(null);

  // =========================
  // OBSERVABLE API (compatibilità)
  // =========================
  readonly token$: Observable<string | null> = this.tokenSubject.asObservable();
  readonly user$: Observable<AuthUser | null> = this.userSubject.asObservable();

  readonly isLoggedIn$: Observable<boolean> = this.user$.pipe(map((u) => !!u));
  readonly isAdmin$: Observable<boolean> = this.user$.pipe(map((u) => !!u?.isAdmin));

  // =========================
  // SIGNAL API (Angular 17+)
  // =========================
  readonly user = signal<AuthUser | null>(null);
  readonly isLoggedIn = computed(() => !!this.user());
  readonly isAdmin = computed(() => !!this.user()?.isAdmin);

  constructor() {
    this.restoreFromStorage();
  }

  // =========================
  // Snapshot helpers (guard / interceptor / azioni sincrone)
  // =========================
  getTokenSnapshot(): string | null {
    return this.tokenSubject.value;
  }

  getUserSnapshot(): AuthUser | null {
    return this.userSubject.value;
  }

  // =========================
  // Auth actions
  // =========================
  login(req: LoginRequest): Observable<LoginResult> {
    return this.authApi.login(req).pipe(
      tap((result) => {
        if (!result?.success || !result.token || result.userId == null) {
          this.logout();
          return;
        }

        if (this.isTokenExpired(result.token)) {
          this.logout();
          return;
        }

        const user: AuthUser = {
          userId: result.userId,
          name: result.name ?? '',
          surname: result.surname ?? '',
          isAdmin: !!result.isAdmin,
        };

        this.setSession(result.token, user);
      })
    );
  }

  logout(): void {
    this.setSession(null, null);
  }

  // =========================
  // Storage / session
  // =========================
  private restoreFromStorage(): void {
    const token = localStorage.getItem(STORAGE_TOKEN_KEY);
    const userJson = localStorage.getItem(STORAGE_USER_KEY);

    if (!token || !userJson) {
      this.setSession(null, null);
      return;
    }

    if (this.isTokenExpired(token)) {
      this.setSession(null, null);
      return;
    }

    try {
      const user = JSON.parse(userJson) as AuthUser;
      // sanity check minimo
      if (!user || typeof user.userId !== 'number') {
        this.setSession(null, null);
        return;
      }

      this.setSession(token, user);
    } catch {
      this.setSession(null, null);
    }
  }

  /**
   * Applica lo stato di sessione in modo atomico:
   * - BehaviorSubjects (Observable API)
   * - Signals (Signal API)
   * - LocalStorage (persistenza)
   */
  private setSession(token: string | null, user: AuthUser | null): void {
    this.tokenSubject.next(token);
    this.userSubject.next(user);
    this.user.set(user);

    if (token && user) {
      localStorage.setItem(STORAGE_TOKEN_KEY, token);
      localStorage.setItem(STORAGE_USER_KEY, JSON.stringify(user));
      return;
    }

    localStorage.removeItem(STORAGE_TOKEN_KEY);
    localStorage.removeItem(STORAGE_USER_KEY);
  }

  // =========================
  // JWT helpers
  // =========================
  private decodeTokenPayload(token: string): { exp?: number } | null {
    try {
      const parts = token.split('.');
      if (parts.length !== 3) return null;

      const payloadBase64 = parts[1].replace(/-/g, '+').replace(/_/g, '/');
      const json = atob(payloadBase64);
      return JSON.parse(json) as { exp?: number };
    } catch {
      return null;
    }
  }

  private isTokenExpired(token: string): boolean {
    const payload = this.decodeTokenPayload(token);

    // Se non c'è exp, per sicurezza consideriamo il token scaduto
    if (!payload?.exp) return true;

    const now = Math.floor(Date.now() / 1000);
    return payload.exp < now;
  }
}
