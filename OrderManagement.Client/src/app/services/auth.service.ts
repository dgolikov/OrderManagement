import { HttpClient, HttpHeaders, HttpRequest } from '@angular/common/http';
import { computed, effect, inject, Injectable, signal } from '@angular/core';
import { Router } from '@angular/router';
import { environment } from '../environments/environment';
import { firstValueFrom, map, Observable, Subscriber } from 'rxjs';
import { ITokenPair } from '../models/responses/token-pair.model';
import { ILoginRequest } from '../models/requests/login-request.model';
import { IAuthInfoModel } from '../models/auth-info.model';
import { IUser } from '../models/responses/user.model';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly _http: HttpClient = inject(HttpClient);
  private readonly _router: Router = inject(Router);
  private readonly _apiUrl: string = `${environment.apiUrl}/auth`;

  private _refreshInProgress: boolean = false;
  private _requests: CallerRequest[] = [];

  private readonly _accessToken = signal<string>(localStorage.getItem('accessToken') || '');
  private readonly _refreshToken = signal<string>(localStorage.getItem('refreshToken') || '');
  private readonly _currentUser = signal<IUser | null>(null);

  public readonly currentUser = this._currentUser.asReadonly();

  private readonly _payload = computed<IAuthInfoModel | null>(() => {
    if (!this._accessToken()) {
      return null;
    }

    const payload = atob(this._accessToken().split('.')[1]);
    return {
      userId: JSON.parse(payload).userId,
    };
  });

  public readonly isAuthorized = computed(() => !!this._payload());

  constructor() {
    console.log('test');
    effect(() => {
      localStorage.setItem('accessToken', this._accessToken());
      localStorage.setItem('refreshToken', this._refreshToken());
    });

    effect(() => {
      if (this.isAuthorized()) {
        this.loadCurrentUser();
      }
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

  public async loadCurrentUser(): Promise<void> {
    if (!this._accessToken()) return;

    const userId = this._payload()?.userId;
    console.log(this._payload());
    if (!userId) return;

    if (this._currentUser()) return;

    try {
      const user = await firstValueFrom(this._http.get<IUser>(`${environment.apiUrl}/users/me`));
      this._currentUser.set(user);
    } catch (error) {
      console.error('Failed to load user info', error);
    }
  }

  public logout(): Observable<void> {
    return this._http.post<void>(`${this._apiUrl}/logout`, {}).pipe(
      map(() => {
        this._accessToken.set('');
        this._refreshToken.set('');
        this._currentUser.set(null);
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

  public handleUnauthorizedError(subscriber: Subscriber<any>, request: HttpRequest<any>) {
    this._requests.push({
      subscriber: subscriber,
      failedRequest: request,
    });

    if (this._refreshInProgress) return;

    this._refreshInProgress = true;
    this.refreshTokens().subscribe({
      next: () => {
        this.repeatFailedRequests();
      },
      error: () => {
        this._router.navigate(['auth']).then(() => {});
      },
      complete: () => {
        this._refreshInProgress = false;
      },
    });
  }

  private repeatFailedRequests() {
    this._requests.forEach((callerRequest) => {
      const request = callerRequest.failedRequest.clone({
        headers: this.addAuthorizationHeader(callerRequest.failedRequest.headers),
      });
      this.repeatRequest(request, callerRequest.subscriber);
    });
    this._requests = [];
  }

  private repeatRequest(request: HttpRequest<any>, subscriber: Subscriber<any>) {
    this._http.request(request).subscribe({
      next: (response) => subscriber.next(response),
      error: (error) => {
        if (error.status === 401) this.logout();
        subscriber.error(error);
      },
      complete: () => subscriber.complete(),
    });
  }
}

type CallerRequest = {
  subscriber: Subscriber<any>;
  failedRequest: HttpRequest<any>;
};
