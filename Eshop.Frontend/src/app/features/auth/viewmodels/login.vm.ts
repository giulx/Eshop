import { Injectable, inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { LoginResult } from '../models/login-result.model';

@Injectable()
export class LoginVm {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  formModel = {
    email: '',
    password: '',
  };

  loading = false;
  error: string | null = null;
  errorCode: string | null = null;

  init(): void {
    const user = this.authService.getUserSnapshot();
    if (user) {
      this.router.navigate([user.isAdmin ? '/admin/prodotti' : '/prodotti']);
      return;
    }

    this.loading = false;
    this.error = null;
    this.errorCode = null;
  }

  onSubmit(): void {
    if (!this.formModel.email || !this.formModel.password) {
      this.errorCode = null;
      this.error = 'Inserisci email e password.';
      return;
    }

    this.loading = true;
    this.error = null;
    this.errorCode = null;

    this.authService
      .login({
        email: this.formModel.email,
        password: this.formModel.password,
      })
      .subscribe({
        next: (result: LoginResult) => {
          this.loading = false;

          if (!result.success) {
            this.errorCode = result.errorCode ?? null;
            this.error = this.mapErrorCodeToMessage(result.errorCode, result.message);
            return;
          }

          const user = this.authService.getUserSnapshot();
          this.router.navigate([user?.isAdmin ? '/admin/prodotti' : '/prodotti']);
        },

        error: (err) => {
          console.error('❌ Errore durante il login:', err);
          this.loading = false;

          const apiError = err?.error;

          if (err.status === 400 && apiError?.errors) {
            const emailError: string | undefined = apiError.errors.Email?.[0];
            const passwordError: string | undefined = apiError.errors.Password?.[0];

            const message =
              emailError ||
              passwordError ||
              apiError.title ||
              'Alcuni dati inseriti non sono validi.';

            this.errorCode = emailError ? 'invalid_email' : null;
            this.error = message;
            return;
          }

          if (err.status === 401 && apiError) {
            const loginResult = apiError as LoginResult;
            this.errorCode = loginResult.errorCode ?? null;
            this.error = this.mapErrorCodeToMessage(loginResult.errorCode, loginResult.message);
            return;
          }

          this.errorCode = null;
          this.error = 'Impossibile comunicare con il server. Riprova più tardi.';
        },
      });
  }

  private mapErrorCodeToMessage(
    errorCode?: string | null,
    fallbackMessage?: string | null
  ): string {
    switch (errorCode) {
      case 'user_not_found':
        return 'Nessun account trovato con questa email.';

      case 'wrong_password':
        return 'Password errata. Riprova.';

      case 'email_not_confirmed':
        return 'Email non confermata. Controlla la tua casella di posta.';

      case 'account_locked':
        return 'Account temporaneamente bloccato per troppi tentativi falliti.';

      case 'invalid_email':
        return "L'email inserita non è valida.";

      case 'login_failed':
        return 'Credenziali non valide.';

      default:
        if (fallbackMessage && fallbackMessage.trim().length > 0) {
          return fallbackMessage;
        }
        return 'Si è verificato un problema durante il login.';
    }
  }
}
