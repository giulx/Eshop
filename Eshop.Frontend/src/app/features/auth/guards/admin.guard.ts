import { Injectable } from '@angular/core';
import {
  CanActivateFn,
  Router
} from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { map } from 'rxjs';

export const adminGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  const router = inject(Router);

  return auth.isAdmin$.pipe(
    map(isAdmin => {
      if (isAdmin) {
        return true;
      }
      // se non è admin → o login, o homepage
      return router.createUrlTree(['/login']);
    })
  );
};
