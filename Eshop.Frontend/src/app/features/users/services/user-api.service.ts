import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { UserModel } from '../models/user.model';
import { UserCreateRequest } from '../models/user-create-request.model';
import { UserUpdateRequest } from '../models/user-update-request.model';
import { PagedResult } from '../../../shared/models/paged-result.model';

@Injectable({ providedIn: 'root' })
export class UserApiService {
  private readonly http = inject(HttpClient);

  // UsersController: [Route("api/[controller]")] => /api/users
  private readonly baseUrl = '/api/users';

  // =========================
  // PUBLIC / CUSTOMER
  // =========================

  register(body: UserCreateRequest): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.baseUrl}/register`, body);
  }

  getMe(): Observable<UserModel> {
    return this.http.get<UserModel>(`${this.baseUrl}/me`);
  }

  updateMe(body: UserUpdateRequest): Observable<{ message: string }> {
    return this.http.put<{ message: string }>(`${this.baseUrl}/me`, body);
  }

  deleteMe(): Observable<{ message: string }> {
    return this.http.delete<{ message: string }>(`${this.baseUrl}/me`);
  }

  // =========================
  // ADMIN
  // =========================
  // GET    /api/users?search=&page=&pageSize=
  // GET    /api/users/{id}
  // PUT    /api/users/{id}
  // DELETE /api/users/{id}

  getPagedAdmin(
    search: string | null,
    page: number = 1,
    pageSize: number = 20
  ): Observable<PagedResult<UserModel>> {
    let params = new HttpParams()
      .set('page', String(page))
      .set('pageSize', String(pageSize));

    const q = search?.trim();
    if (q) params = params.set('search', q);

    return this.http.get<PagedResult<UserModel>>(this.baseUrl, { params });
  }

  getAdminUserById(id: number): Observable<UserModel> {
    return this.http.get<UserModel>(`${this.baseUrl}/${id}`);
  }

  updateAdminUser(id: number, body: UserUpdateRequest): Observable<{ message: string }> {
    return this.http.put<{ message: string }>(`${this.baseUrl}/${id}`, body);
  }

  deleteAdmin(id: number): Observable<{ message: string }> {
    return this.http.delete<{ message: string }>(`${this.baseUrl}/${id}`);
  }
}
