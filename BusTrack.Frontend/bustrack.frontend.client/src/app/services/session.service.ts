import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class SessionService {
  private readonly tokenKey = 'busTrackAccessToken';

  startSession(accessToken: string): void {
    if (!accessToken) {
      this.clearSession();
      return;
    }

    sessionStorage.setItem(this.tokenKey, accessToken);
  }

  clearSession(): void {
    sessionStorage.removeItem(this.tokenKey);
  }

  getAccessToken(): string | null {
    const token = sessionStorage.getItem(this.tokenKey);

    if (!token || !this.isTokenValid(token)) {
      this.clearSession();
      return null;
    }

    return token;
  }

  isAuthenticated(): boolean {
    return this.getAccessToken() !== null;
  }

  isAdmin(): boolean {
    const token = this.getAccessToken();
    if (!token) {
      return false;
    }

    try {
      const payloadPart = token.split('.')[1];
      const base64 = payloadPart.replace(/-/g, '+').replace(/_/g, '/');
      const decoded = atob(base64.padEnd(base64.length + ((4 - base64.length % 4) % 4), '='));
      const payload = JSON.parse(decoded) as Record<string, unknown>;
      const roleClaim = payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']
        ?? payload['role']
        ?? payload['roles'];
      return Array.isArray(roleClaim)
        ? roleClaim.includes('ADMIN')
        : roleClaim === 'ADMIN';
    } catch {
      return false;
    }
  }

  private isTokenValid(token: string): boolean {
    try {
      const payloadPart = token.split('.')[1];
      if (!payloadPart) {
        return false;
      }

      const base64 = payloadPart
        .replace(/-/g, '+')
        .replace(/_/g, '/');
      const decoded = atob(base64.padEnd(base64.length + ((4 - base64.length % 4) % 4), '='));
      const payload = JSON.parse(decoded) as { exp?: number };

      return typeof payload.exp === 'number' &&
        payload.exp * 1000 > Date.now();
    } catch {
      return false;
    }
  }
}
