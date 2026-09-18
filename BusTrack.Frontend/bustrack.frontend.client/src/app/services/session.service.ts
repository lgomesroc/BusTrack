import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class SessionService {

  private readonly sessionKey =
    'busTrackAuthenticated';

  startSession(): void {
    sessionStorage.setItem(
      this.sessionKey,
      'true'
    );
  }

  clearSession(): void {
    sessionStorage.removeItem(
      this.sessionKey
    );
  }

  isAuthenticated(): boolean {
    return (
      sessionStorage.getItem(
        this.sessionKey
      ) === 'true'
    );
  }
}
