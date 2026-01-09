import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

import { AuthService } from '../services/auth.service';

/**
 * customerGuard
 * ------------------------------------------------------------
 * Consente l’accesso SOLO agli utenti autenticati NON admin (clienti).
 * - Se non loggato → redirect a /login con returnUrl
 * - Se admin → redirect all’area admin
 */
export const customerGuard: CanActivateFn = (_route, state) => {
  const auth = inject(AuthService);
  const router = inject(Router);

  const user = auth.user(); // ✅ signal<AuthUser | null>

  // Non autenticato → login
  if (!user) {
    return router.createUrlTree(['/login'], {
      queryParams: {
        reason: 'customer_guard',
        returnUrl: state.url,
      },
    });
  }

  // Admin → non deve entrare nell’area customer
  if (user.isAdmin) {
    return router.createUrlTree(['/admin/ordini']);
  }

  return true;
};
