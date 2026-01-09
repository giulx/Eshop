import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';

import { routes } from './app.routes';

/**
 * Configurazione globale dell'applicazione Angular.
 *
 * Questo file sostituisce completamente AppModule (approccio "standalone"),
 * come raccomandato nelle versioni Angular 16+.
 *
 * Qui vengono registrati:
 * - il router applicativo
 * - il client HTTP
 * - eventuali provider globali (interceptor, guard, hydration, ecc.)
 */
export const appConfig: ApplicationConfig = {
  providers: [
    /**
     * Router applicativo con definizione delle rotte principali.
     * Supporta lazy-loading e guard standalone.
     */
    provideRouter(routes),

    /**
     * HttpClient globale per l'intera applicazione.
     * Da qui possono essere aggiunti interceptor (auth, error handling).
     */
    provideHttpClient(),
  ],
};
