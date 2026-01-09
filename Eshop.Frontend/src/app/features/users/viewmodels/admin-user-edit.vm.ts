import { Injectable, inject, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

import { UserApiService } from '../services/user-api.service';
import { UserModel } from '../models/user.model';
import { UserUpdateRequest } from '../models/user-update-request.model';

export interface AdminUserEditFormModel {
  name: string;
  surname: string;
  street: string;
  city: string;
  postalCode: string;
  number: string;
  nuovaPassword: string;
}

@Injectable()
export class AdminUserEditVm {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly userApi = inject(UserApiService);

  readonly loading = signal(false);
  readonly saving = signal(false);

  readonly error = signal<string | null>(null);
  readonly success = signal<string | null>(null);

  readonly userId = signal<number | null>(null);

  // read-only
  readonly email = signal('');
  readonly isAdmin = signal(false);

  // form model
  readonly formModel = signal<AdminUserEditFormModel>({
    name: '',
    surname: '',
    street: '',
    city: '',
    postalCode: '',
    number: '',
    nuovaPassword: '',
  });

  init(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    const id = idParam ? Number(idParam) : NaN;

    if (Number.isNaN(id) || id <= 0) {
      this.error.set('ID utente non valido.');
      return;
    }

    this.userId.set(id);
    this.loadUser(id);
  }

  private loadUser(id: number): void {
    this.loading.set(true);
    this.error.set(null);
    this.success.set(null);

    this.userApi.getAdminUserById(id).subscribe({
      next: (user: UserModel) => {
        this.email.set(user.email);
        this.isAdmin.set(!!user.isAdmin);

        this.formModel.set({
          name: user.name ?? '',
          surname: user.surname ?? '',
          street: user.street ?? '',
          city: user.city ?? '',
          postalCode: user.postalCode ?? '',
          number: user.number ?? '',
          nuovaPassword: '',
        });

        this.loading.set(false);
      },
      error: (err) => {
        this.loading.set(false);

        if (err?.status === 404) {
          this.error.set('Utente non trovato.');
          return;
        }

        if (err?.status === 401 || err?.status === 403) {
          this.error.set('Non sei autorizzata a modificare questo utente.');
          this.router.navigate(['/login']);
          return;
        }

        this.error.set('Errore nel caricamento dei dati utente.');
      },
    });
  }

  save(currentForm: AdminUserEditFormModel): void {
    const id = this.userId();
    if (id == null) {
      this.error.set('ID utente non valido.');
      return;
    }

    this.error.set(null);
    this.success.set(null);
    this.saving.set(true);

    // Trim + costruzione body “parziale”
    const body: UserUpdateRequest = {};

    const name = (currentForm.name ?? '').trim();
    const surname = (currentForm.surname ?? '').trim();

    const street = (currentForm.street ?? '').trim();
    const city = (currentForm.city ?? '').trim();
    const postalCode = (currentForm.postalCode ?? '').trim();
    const number = (currentForm.number ?? '').trim();

    const nuovaPassword = (currentForm.nuovaPassword ?? '').trim();

    // Nome/cognome: in update service li gestisci come "se vuoto -> lascia com'era",
    // ma a UI li richiediamo, quindi li mandiamo sempre.
    body.name = name;
    body.surname = surname;

    // Address: manda solo se valorizzato (opzionale)
    if (street) body.street = street;
    if (city) body.city = city;
    if (postalCode) body.postalCode = postalCode;
    if (number) body.number = number;

    if (nuovaPassword) body.nuovaPassword = nuovaPassword;

    this.userApi.updateAdminUser(id, body).subscribe({
      next: (res) => {
        this.saving.set(false);
        this.success.set(res?.message ?? 'Utente aggiornato correttamente.');

        // reset password field
        this.formModel.update((v) => ({ ...v, nuovaPassword: '' }));
      },
      error: (err) => {
        this.saving.set(false);

        if (err?.status === 404) {
          this.error.set('Utente non trovato.');
          return;
        }

        if (err?.status === 401 || err?.status === 403) {
          this.error.set('Non sei autorizzata a salvare modifiche su questo utente.');
          return;
        }

        if (err?.status === 400) {
          this.error.set('Dati non validi: controlla i campi evidenziati.');
          return;
        }

        this.error.set('Errore durante il salvataggio delle modifiche.');
      },
    });
  }

  backToList(): void {
    this.router.navigate(['/admin/utenti']);
  }
}
