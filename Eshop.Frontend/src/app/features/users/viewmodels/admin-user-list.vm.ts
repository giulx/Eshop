import { DestroyRef, Injectable, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

import { UserApiService } from '../services/user-api.service';
import { UserModel } from '../models/user.model';
import { PagedResult } from '../../../shared/models/paged-result.model';

@Injectable()
export class AdminUserListVm {
  private readonly userApi = inject(UserApiService);
  private readonly destroyRef = inject(DestroyRef);

  readonly users = signal<UserModel[]>([]);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  readonly searchTerm = signal('');
  readonly page = signal(1);
  readonly pageSize = signal(20);
  readonly total = signal(0);

  readonly hasSearch = computed(() => this.searchTerm().trim().length > 0);

  readonly maxPage = computed(() => {
    const size = Math.max(1, this.pageSize());
    const tot = Math.max(0, this.total());
    return Math.max(1, Math.ceil(tot / size));
  });

  readonly canPrev = computed(() => this.page() > 1);
  readonly canNext = computed(() => this.page() < this.maxPage());

  init(): void {
    this.loadUsers();
  }

  private loadUsers(): void {
    if (this.loading()) return;

    this.loading.set(true);
    this.error.set(null);

    const q = this.searchTerm().trim();
    const search = q.length ? q : null;

    this.userApi
      .getPagedAdmin(search, this.page(), this.pageSize())
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (res: PagedResult<UserModel>) => {
          this.users.set(res.items ?? []);
          this.page.set(res.page ?? this.page());
          this.pageSize.set(res.pageSize ?? this.pageSize());
          this.total.set(res.total ?? 0);
          this.loading.set(false);

          // se per qualsiasi motivo la pagina “corrente” supera maxPage, riallineo
          const mp = this.maxPage();
          if (this.page() > mp) {
            this.page.set(mp);
            // ricarico coerente con pagina corretta (una sola volta)
            this.loading.set(false);
            this.loadUsers();
          }
        },
        error: (err) => {
          this.loading.set(false);

          if (err?.status === 401 || err?.status === 403) {
            this.error.set('Non sei autorizzata a visualizzare gli utenti.');
            return;
          }

          this.error.set('Errore nel caricamento degli utenti.');
        },
      });
  }

  setSearchTerm(value: string): void {
    this.searchTerm.set(value);
  }

  onSearch(): void {
    this.page.set(1);
    this.loadUsers();
  }

  clearSearch(): void {
    this.searchTerm.set('');
    this.page.set(1);
    this.loadUsers();
  }

  goToPage(newPage: number): void {
    const target = Math.trunc(newPage);
    if (target < 1 || target > this.maxPage() || target === this.page()) return;

    this.page.set(target);
    this.loadUsers();
  }

  nextPage(): void {
    if (!this.canNext()) return;
    this.page.update((p) => p + 1);
    this.loadUsers();
  }

  prevPage(): void {
    if (!this.canPrev()) return;
    this.page.update((p) => p - 1);
    this.loadUsers();
  }

  getRoleLabel(user: UserModel): string {
    return user.isAdmin ? 'Admin' : 'Cliente';
  }
}
