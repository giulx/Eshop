import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { LoginRequest } from '../models/login-request.model';
import { LoginResult } from '../models/login-result.model';
import { RegisterRequest } from '../models/register-request.model';

@Injectable({ providedIn: 'root' })
export class AuthApiService {

  private readonly http = inject(HttpClient);

  private readonly authBaseUrl = '/api/Auth';
  private readonly usersBaseUrl = '/api/Users';

  login(body: LoginRequest): Observable<LoginResult> {
    return this.http.post<LoginResult>(`${this.authBaseUrl}/login`, body);
  }

  // stesso payload usato dal backend per creazione utente
  register(body: RegisterRequest): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.usersBaseUrl}/register`, body);
  }
}
