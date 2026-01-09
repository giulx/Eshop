import { Injectable, inject } from '@angular/core';
import { Router } from '@angular/router';

import { AuthApiService } from '../services/auth-api.service';
import { RegisterRequest } from '../models/register-request.model';

@Injectable()
export class RegisterVm {
  private readonly authApi = inject(AuthApiService);
  private readonly router = inject(Router);

  form: RegisterRequest = {
    name: '',
    surname: '',
    email: '',
    password: '',
    street: '',
    city: '',
    postalCode: '',
    number: '',
  };

  loading = false;
  error: string | null = null;
  success: string | null = null;

  private isEmpty(value: string | null | undefined): boolean {
    return !value || value.trim().length === 0;
  }

  submit(): void {
    this.error = null;
    this.success = null;

    if (
      this.isEmpty(this.form.name) ||
      this.isEmpty(this.form.surname) ||
      this.isEmpty(this.form.email) ||
      this.isEmpty(this.form.password) ||
      this.isEmpty(this.form.street) ||
      this.isEmpty(this.form.city) ||
      this.isEmpty(this.form.postalCode) ||
      this.isEmpty(this.form.number)
    ) {
      this.error = 'Compila tutti i campi obbligatori.';
      return;
    }

    this.loading = true;

    this.authApi.register(this.form).subscribe({
      next: () => {
        this.loading = false;
        this.success = 'Registrazione completata! Ora puoi accedere.';
        this.router.navigate(['/login'], { queryParams: { registered: '1' } });
      },
      error: (err: any) => {
        this.loading = false;

        if (err.status === 409) {
          this.error = 'Questa email è già registrata.';
          return;
        }

        this.error = 'Errore durante la registrazione. Riprova più tardi.';
      },
    });
  }
}
