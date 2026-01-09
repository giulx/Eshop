import { DestroyRef, Injectable, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

import { UserApiService } from '../services/user-api.service';
import { UserModel } from '../models/user.model';
import { UserUpdateRequest } from '../models/user-update-request.model';
import { AuthService } from '../../auth/services/auth.service';

/**
 * Modello del form profilo lato frontend.
 * - Readonly: si aggiorna solo tramite signal.update().
 * - nuovaPassword è "volatile": non arriva mai dal backend.
 */
export type ProfileFormModel = Readonly<{
  name: string;
  surname: string;
  street: string;
  city: string;
  postalCode: string;
  number: string;
  nuovaPassword: string;
}>;

type ProfileField = keyof ProfileFormModel;

@Injectable()
export class ProfileVm {
  private readonly userApi = inject(UserApiService);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);

  readonly loading = signal(false);
  readonly saving = signal(false);
  readonly deleting = signal(false);

  readonly error = signal<string | null>(null);
  readonly success = signal<string | null>(null);

  // read-only
  readonly email = signal('');

  /**
   * Snapshot originale (senza password) per capire se ci sono modifiche.
   */
  private readonly original = signal<Omit<ProfileFormModel, 'nuovaPassword'>>({
    name: '',
    surname: '',
    street: '',
    city: '',
    postalCode: '',
    number: '',
  });

  readonly formModel = signal<ProfileFormModel>({
    name: '',
    surname: '',
    street: '',
    city: '',
    postalCode: '',
    number: '',
    nuovaPassword: '',
  });

  readonly hasChanges = computed(() => {
    const fm = this.formModel();
    const orig = this.original();

    const baseChanged =
      fm.name !== orig.name ||
      fm.surname !== orig.surname ||
      fm.street !== orig.street ||
      fm.city !== orig.city ||
      fm.postalCode !== orig.postalCode ||
      fm.number !== orig.number;

    const pwdChanged = fm.nuovaPassword.trim().length > 0;

    return baseChanged || pwdChanged;
  });

  /**
   * Validazione UI minimale, coerente con backend:
   * - name/surname non vuoti
   * - nuovaPassword: vuota oppure >= 8
   * - postalCode: se presente deve essere solo cifre (minimo controllo)
   */
  readonly canSubmit = computed(() => {
    if (this.loading() || this.saving() || this.deleting()) return false;
    if (!this.hasChanges()) return false;

    const fm = this.formModel();

    const nameOk = fm.name.trim().length > 0;
    const surnameOk = fm.surname.trim().length > 0;

    const pwd = fm.nuovaPassword.trim();
    const pwdOk = pwd.length === 0 || pwd.length >= 8;

    const cap = fm.postalCode.trim();
    const capOk = cap.length === 0 || /^[0-9]+$/.test(cap);

    return nameOk && surnameOk && pwdOk && capOk;
  });

  readonly canDelete = computed(() => !this.loading() && !this.saving() && !this.deleting());

  // =========================
  // Lifecycle
  // =========================
  init(): void {
    this.loadProfile();
  }

  setField<K extends ProfileField>(key: K, value: ProfileFormModel[K]): void {
    this.formModel.update((v) => (v[key] === value ? v : { ...v, [key]: value }));
  }

  // =========================
  // Data loading
  // =========================
  private loadProfile(): void {
    this.loading.set(true);
    this.error.set(null);
    this.success.set(null);

    this.userApi
      .getMe()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (user: UserModel) => {
          const normalized = this.normalizeUserToForm(user);

          this.email.set(user.email ?? '');

          this.original.set({
            name: normalized.name,
            surname: normalized.surname,
            street: normalized.street,
            city: normalized.city,
            postalCode: normalized.postalCode,
            number: normalized.number,
          });

          this.formModel.set({ ...normalized, nuovaPassword: '' });

          this.loading.set(false);
        },
        error: (err) => {
          this.loading.set(false);

          if (err?.status === 401) {
            this.router.navigate(['/login'], { queryParams: { reason: 'profile' } });
            return;
          }

          this.error.set('Errore nel caricamento del profilo utente.');
        },
      });
  }

  // =========================
  // Save
  // =========================
  onSubmit(): void {
    if (!this.canSubmit()) return;

    this.error.set(null);
    this.success.set(null);
    this.saving.set(true);

    const fm = this.formModel();
    const body = this.buildUpdateRequest(fm);

    this.userApi
      .updateMe(body)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (res) => {
          this.saving.set(false);
          this.success.set(res?.message ?? 'Profilo aggiornato correttamente.');

          // pulizia password
          this.formModel.update((v) => ({ ...v, nuovaPassword: '' }));

          // aggiorno snapshot originale (senza password)
          const now = this.formModel();
          this.original.set({
            name: now.name,
            surname: now.surname,
            street: now.street,
            city: now.city,
            postalCode: now.postalCode,
            number: now.number,
          });
        },
        error: (err) => {
          this.saving.set(false);

          if (err?.status === 401) {
            this.auth.logout();
            this.router.navigate(['/login'], { queryParams: { reason: 'profile_save' } });
            return;
          }

          if (err?.status === 400) {
            this.error.set('Dati non validi: controlla i campi.');
            return;
          }

          this.error.set('Errore durante l’aggiornamento del profilo.');
        },
      });
  }

  // =========================
  // Delete account
  // =========================
  onDeleteAccount(): void {
    if (!this.canDelete()) return;

    const ok = confirm('Sei sicura di voler eliminare il tuo account? L’operazione è definitiva.');
    if (!ok) return;

    this.deleting.set(true);
    this.error.set(null);
    this.success.set(null);

    this.userApi
      .deleteMe()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.deleting.set(false);
          this.auth.logout();
          this.router.navigate(['/prodotti']);
        },
        error: (err) => {
          this.deleting.set(false);

          if (err?.status === 401) {
            this.auth.logout();
            this.router.navigate(['/login'], { queryParams: { reason: 'profile_delete' } });
            return;
          }

          this.error.set('Errore durante l’eliminazione dell’account.');
        },
      });
  }

  // =========================
  // Helpers
  // =========================
  private normalizeUserToForm(user: UserModel): Omit<ProfileFormModel, 'nuovaPassword'> {
    return {
      name: user.name ?? '',
      surname: user.surname ?? '',
      street: user.street ?? '',
      city: user.city ?? '',
      postalCode: user.postalCode ?? '',
      number: user.number ?? '',
    };
  }

  /**
   * Costruisce la request di update.
   * Qui mandiamo SEMPRE name e surname (sono parte del profilo),
   * gli altri campi solo se valorizzati; password solo se compilata.
   */
  private buildUpdateRequest(fm: ProfileFormModel): UserUpdateRequest {
    const body: UserUpdateRequest = {};

    // sempre (coerente UX)
    body.name = fm.name.trim();
    body.surname = fm.surname.trim();

    const street = fm.street.trim();
    const city = fm.city.trim();
    const postalCode = fm.postalCode.trim();
    const number = fm.number.trim();
    const nuovaPassword = fm.nuovaPassword.trim();

    if (street) body.street = street;
    if (city) body.city = city;
    if (postalCode) body.postalCode = postalCode;
    if (number) body.number = number;
    if (nuovaPassword) body.nuovaPassword = nuovaPassword;

    return body;
  }
}
