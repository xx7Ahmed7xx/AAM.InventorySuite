import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { tap } from 'rxjs/operators';

export interface LoginDto {
  username: string;
  password: string;
}

export interface AuthResponseDto {
  userId: number;
  username: string;
  email: string;
  role: string;
  token: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'https://localhost:7210/api';
  private currentUserSubject = new BehaviorSubject<AuthResponseDto | null>(this.getStoredUser());
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor(private http: HttpClient) {}

  login(credentials: LoginDto): Observable<AuthResponseDto> {
    return this.http.post<any>(`${this.apiUrl}/auth/login`, credentials)
      .pipe(
        tap(response => {
          // Convert role from number to string if needed
          const authResponse: AuthResponseDto = {
            userId: response.userId,
            username: response.username,
            email: response.email,
            role: this.convertRoleToString(response.role),
            token: response.token
          };
          this.storeUser(authResponse);
          this.currentUserSubject.next(authResponse);
        })
      );
  }

  private convertRoleToString(role: string | number): string {
    if (typeof role === 'number') {
      switch (role) {
        case 1:
          return 'Cashier';
        case 2:
          return 'Moderator';
        case 3:
          return 'SuperAdmin';
        default:
          return 'Cashier';
      }
    }
    return role;
  }

  logout(): void {
    localStorage.removeItem('currentUser');
    localStorage.removeItem('token');
    this.currentUserSubject.next(null);
  }

  getCurrentUser(): AuthResponseDto | null {
    return this.currentUserSubject.value;
  }

  getToken(): string | null {
    const user = this.getStoredUser();
    return user?.token || null;
  }

  isAuthenticated(): boolean {
    return this.getCurrentUser() !== null;
  }

  hasRole(role: string): boolean {
    const user = this.getCurrentUser();
    return user?.role === role;
  }

  isSuperAdmin(): boolean {
    return this.hasRole('SuperAdmin');
  }

  isModerator(): boolean {
    return this.hasRole('Moderator') || this.isSuperAdmin();
  }

  private getStoredUser(): AuthResponseDto | null {
    const stored = localStorage.getItem('currentUser');
    if (!stored) return null;
    
    const user = JSON.parse(stored);
    // Ensure role is a string (convert from number if needed)
    if (user && typeof user.role === 'number') {
      user.role = this.convertRoleToString(user.role);
    }
    return user;
  }

  private storeUser(user: AuthResponseDto): void {
    localStorage.setItem('currentUser', JSON.stringify(user));
    localStorage.setItem('token', user.token);
  }
}

