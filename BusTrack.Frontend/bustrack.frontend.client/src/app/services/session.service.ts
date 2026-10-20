import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class SessionService {
  private readonly tokenKey = 'busTrackAccessToken';
  private readonly browserIdKey = 'busTrackBrowserId';

  getBrowserId(): string {
    let browserId = localStorage.getItem(this.browserIdKey);

    if (!browserId) {
      browserId = crypto.randomUUID().replace(/-/g, '');
      localStorage.setItem(this.browserIdKey, browserId);
    }

    return browserId;
  }

  startSession(accessToken: string): void {
    if (!accessToken) {
      this.clearSession();
      return;
    }

    localStorage.setItem(this.tokenKey, accessToken);
  }

  clearSession(): void {
    localStorage.removeItem(this.tokenKey);
  }

  getAccessToken(): string | null {
    const token = localStorage.getItem(this.tokenKey);

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
      const payload = this.readPayload(token);
      const roleClaim =
        payload[
          'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'
        ] ?? payload['role'] ?? payload['roles'];

      return Array.isArray(roleClaim)
        ? roleClaim.includes('ADMIN')
        : roleClaim === 'ADMIN';
    } catch {
      return false;
    }
  }

  private isTokenValid(token: string): boolean {
    try {
      const payload = this.readPayload(token);

      return typeof payload['exp'] === 'number'
        && (payload['exp'] as number) * 1000 > Date.now();
    } catch {
      return false;
    }
  }

  private readPayload(token: string): Record<string, unknown> {
    const payloadPart = token.split('.')[1];

    if (!payloadPart) {
      throw new Error('Token JWT inválido.');
    }

    const base64 = payloadPart
      .replace(/-/g, '+')
      .replace(/_/g, '/');

    const decoded = atob(
      base64.padEnd(
        base64.length + ((4 - base64.length % 4) % 4),
        '='
      )
    );

    return JSON.parse(decoded) as Record<string, unknown>;
  }
}
