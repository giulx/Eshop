// src/main.ts
import { bootstrapApplication } from '@angular/platform-browser';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';

import { App } from './app/app';
import { routes } from './app/app.routes';
import { authTokenInterceptor } from './app/features/auth/services/auth-token.interceptor';

bootstrapApplication(App, {
  providers: [
    /**
     * Router dell’applicazione:
     * - aree pubbliche
     * - area cliente
     * - area admin (protetta da guard)
     */
    provideRouter(routes),

    /**
     * HttpClient con interceptor funzionale.
     * L’interceptor allega il JWT (se presente) e gestisce i 401.
     */
    provideHttpClient(withInterceptors([authTokenInterceptor])),
  ],
}).catch((err) => console.error(err));
