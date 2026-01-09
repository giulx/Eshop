import { Component, DestroyRef, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { takeUntilDestroyed, toSignal } from '@angular/core/rxjs-interop';

import { UserApiService } from '../../../users/services/user-api.service';
import type { UserCreateRequest } from '../../../users/models/user-create-request.model';

import { AuthService } from '../../../auth/services/auth.service';

type CtrlName = 'name' | 'surname' | 'email' | 'password' | 'street' | 'city' | 'postalCode' | 'number';

@Component({
  selector: 'app-register-page',
  standalone: true,
  templateUrl: './register-page.component.html',
  styleUrls: ['./register-page.component.css'],
  imports: [ReactiveFormsModule, RouterLink],
})
export class RegisterPageComponent {
  private readonly fb = inject(FormBuilder);
  private readonly userApi = inject(UserApiService);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);

  readonly loading = signal(false);
  readonly error = signal<string | null>(null);
  readonly success = signal<string | null>(null);

  readonly passwordVisible = signal(false);
  togglePassword(): void {
    this.passwordVisible.update((v) => !v);
  }

  readonly form = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(50), Validators.pattern(/^[\p{L}\p{M}'\-\s]+$/u)]],
    surname: ['', [Validators.required, Validators.maxLength(50), Validators.pattern(/^[\p{L}\p{M}'\-\s]+$/u)]],
    email: ['', [Validators.required, Validators.email, Validators.maxLength(254)]],
    password: ['', [Validators.required, Validators.minLength(8), Validators.maxLength(100)]],
    street: ['', [Validators.required, Validators.maxLength(200), Validators.pattern(/^[\p{L}\p{M}0-9\s'\.,\-\/]+$/u)]],
    city: ['', [Validators.required, Validators.maxLength(200), Validators.pattern(/^[\p{L}\p{M}'\-\s\.]+$/u)]],
    postalCode: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(10), Validators.pattern(/^[0-9]+$/)]],
    number: ['', [Validators.required, Validators.maxLength(20), Validators.pattern(/^[\p{L}\p{M}0-9\s\/\-]+$/u)]],
  });

  // ✅ Signal dello status (si aggiorna in tempo reale)
  private readonly formStatus = toSignal(this.form.statusChanges, { initialValue: this.form.status });
  readonly canSubmit = computed(() => !this.loading() && this.formStatus() === 'VALID');

  /** Mostra errori "live" dopo che l'utente ha iniziato a interagire */
  shouldShowError(controlName: CtrlName): boolean {
    const c = this.form.controls[controlName];
    return c.invalid && (c.dirty || c.touched);
  }

  /** Helper per aria-invalid */
  ariaInvalid(controlName: CtrlName): 'true' | 'false' {
    return this.shouldShowError(controlName) ? 'true' : 'false';
  }

  /** Messaggio umano per il primo errore del campo */
  getErrorMessage(controlName: CtrlName): string {
    const c = this.form.controls[controlName];
    const e = c.errors;
    if (!e) return '';

    if (e['required']) return 'Campo obbligatorio.';
    if (e['email']) return 'Inserisci una email valida.';
    if (e['minlength']) return `Minimo ${e['minlength'].requiredLength} caratteri.`;
    if (e['maxlength']) return `Massimo ${e['maxlength'].requiredLength} caratteri.`;
    if (e['pattern']) {
      if (controlName === 'postalCode') return 'Il CAP deve contenere solo numeri.';
      return 'Formato non valido.';
    }
    return 'Valore non valido.';
  }

  submit(): void {
    if (this.loading()) return;

    this.error.set(null);
    this.success.set(null);

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.error.set('Controlla i campi evidenziati.');
      return;
    }

    const raw = this.form.getRawValue();

    const dto: UserCreateRequest = {
      name: raw.name.trim(),
      surname: raw.surname.trim(),
      email: raw.email.trim(),
      password: raw.password,
      street: raw.street.trim() || null,
      city: raw.city.trim() || null,
      postalCode: raw.postalCode.trim() || null,
      number: raw.number.trim() || null,
    };

    const loginEmail = dto.email;
    const loginPassword = raw.password;

    this.loading.set(true);

    this.userApi
      .register(dto)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          // ✅ Auto-login
          this.auth
            .login({ email: loginEmail, password: loginPassword })
            .pipe(takeUntilDestroyed(this.destroyRef))
            .subscribe({
              next: (res) => {
                this.loading.set(false);

                if (!res?.success) {
                  this.success.set('Registrazione completata. Ora puoi accedere.');
                  this.router.navigate(['/login'], { queryParams: { reason: 'registered' } });
                  return;
                }

                this.router.navigate(['/prodotti'], { queryParams: { reason: 'welcome' } });
              },
              error: () => {
                this.loading.set(false);
                this.success.set('Registrazione completata. Ora puoi accedere.');
                this.router.navigate(['/login'], { queryParams: { reason: 'registered' } });
              },
            });
        },
        error: (err) => {
          this.loading.set(false);

          if (err?.status === 409) {
            this.error.set('Email già registrata. Prova ad accedere.');
            return;
          }
          if (err?.status === 400) {
            this.error.set('Dati non validi. Ricontrolla i campi.');
            return;
          }
          this.error.set('Errore durante la registrazione. Riprova tra poco.');
        },
      });
  }
}
