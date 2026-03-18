import { HttpClient, HttpHeaders } from '@angular/common/http';
import { computed, effect, inject, Injectable, signal } from '@angular/core';
import { Router } from '@angular/router';
import { environment } from '../environments/environment';
import { map, Observable } from 'rxjs';
import { ITokenPair } from '../models/responses/token-pair.model';
import { ILoginRequest } from '../models/requests/login-request.model';
import { IAuthInfoModel } from '../models/auth-info.model';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly _http: HttpClient = inject(HttpClient);
  private readonly _router: Router = inject(Router);
  private readonly _apiUrl: string = `${environment.apiUrl}/auth`;

  private readonly _accessToken = signal<string>(localStorage.getItem('accessToken') || '');
  private readonly _refreshToken = signal<string>(localStorage.getItem('refreshToken') || '');

  private readonly _payload = computed<IAuthInfoModel | null>(() => {
    if (!this._accessToken()) {
      return null;
    }

    const payload = atob(this._accessToken().split('.')[1]);
    return {
      userId: JSON.parse(payload).sub,
    };
  });

  public get isAuthorized(): boolean {
    return !!this._payload();
  }

  constructor() {
    effect(() => {
      localStorage.setItem('accessToken', this._accessToken());
      localStorage.setItem('refreshToken', this._refreshToken());
    });
  }

  public login(request: ILoginRequest): Observable<void> {
    return this._http.post<ITokenPair>(`${this._apiUrl}/login`, JSON.stringify(request)).pipe(
      map((response) => {
        this._accessToken.set(response.accessToken);
        this._refreshToken.set(response.accessToken);
        this._router.navigate(['/']).then(() => {});
      }),
    );
  }

  public logout(): Observable<void> {
    return this._http.post<void>(`${this._apiUrl}/logout`, {}).pipe(
      map(() => {
        this._accessToken.set('');
        this._refreshToken.set('');
        this._router.navigate(['/sign-in']).then(() => {});
      }),
    );
  }

  public refreshTokens(): Observable<void> {
    return this._http
      .post<ITokenPair>(`${this._apiUrl}/refresh`, JSON.stringify({ token: this._refreshToken() }))
      .pipe(
        map((response) => {
          this._accessToken.set(response.accessToken);
          this._refreshToken.set(response.accessToken);
        }),
      );
  }

  public addAuthorizationHeader(header: HttpHeaders): HttpHeaders {
    return header.set('Authorization', `Bearer ${this._accessToken()}`);
  }
}
