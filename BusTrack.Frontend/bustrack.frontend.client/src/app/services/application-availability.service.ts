import { Injectable } from '@angular/core';

import { HttpClient } from '@angular/common/http';

import { Router } from '@angular/router';

import {
  Subscription,
  timer,
  forkJoin,
  of
} from 'rxjs';

import {
  catchError,
  switchMap
} from 'rxjs/operators';

import {
  SessionService
} from './session.service';

@Injectable({
  providedIn: 'root'
})
export class ApplicationAvailabilityService {

  private readonly healthUrl =
    'http://localhost:5066/api/Health';

  private readonly checkIntervalMs =
    1000;

  private monitoringSubscription:
    Subscription | null = null;

  private unavailable = false;

  private blocked = false;

  constructor(
    private http: HttpClient,

    private router: Router,

    private sessionService: SessionService
  ) {
  }

  startMonitoring(): void {

    if (this.monitoringSubscription) {
      return;
    }

    this.monitoringSubscription =
      timer(
        0,
        this.checkIntervalMs
      )
        .pipe(
          switchMap(() =>
            this.checkApplicationAvailability()
          )
        )
        .subscribe({
          next: (available) => {

            if (!available) {
              this.handleUnavailable();

              return;
            }

            this.handleAvailable();
          }
        });
  }

  stopMonitoring(): void {

    this.monitoringSubscription?.unsubscribe();

    this.monitoringSubscription = null;
  }

  private checkApplicationAvailability() {

    return forkJoin({

      api: this.checkApiAvailability(),

      frontend:
        this.checkFrontendAvailability()

    }).pipe(

      switchMap((result) => {

        return of(
          result.api &&
          result.frontend
        );

      }),

      catchError(() => {

        return of(false);

      })
    );
  }

  private checkApiAvailability() {

    return this.http
      .get(
        this.healthUrl,
        {
          responseType: 'text'
        }
      )
      .pipe(

        switchMap(() => {

          return of(true);

        }),

        catchError(() => {

          return of(false);

        })
      );
  }

  private checkFrontendAvailability() {

    return this.http
      .get(
        `${window.location.origin}/index.html?health=${Date.now()}`,
        {
          responseType: 'text',
          headers: {
            'Cache-Control': 'no-cache',
            'Pragma': 'no-cache'
          }
        }
      )
      .pipe(

        switchMap(() => {

          return of(true);

        }),

        catchError(() => {

          return of(false);

        })
      );
  }

  private handleAvailable(): void {

    if (!this.unavailable) {
      return;
    }

    /*
     * Depois que a aplicação perdeu a comunicação,
     * a sessão anterior permanece inválida.
     *
     * Mesmo que API e frontend voltem a responder,
     * não restauramos automaticamente o usuário.
     */
  }

  private handleUnavailable(): void {

    if (this.blocked) {
      return;
    }

    if (!this.sessionService.isAuthenticated()) {
      return;
    }

    this.blocked = true;

    this.unavailable = true;

    /*
     * Remove imediatamente a sessão.
     */
    this.sessionService.clearSession();

    /*
     * Bloqueia imediatamente qualquer interação
     * com a página que ainda estiver carregada.
     */
    this.blockPage();

    alert(
      'Esta página não está disponível.\n\n' +
      'A comunicação com o sistema foi perdida. ' +
      'Você será direcionado para a tela de login.'
    );

    this.router.navigate(
      ['/enter-the-system'],
      {
        replaceUrl: true
      }
    );
  }

  private blockPage(): void {

    document.body.style.pointerEvents =
      'none';

    document.body.style.userSelect =
      'none';

    document.body.style.cursor =
      'not-allowed';

    const overlay =
      document.createElement('div');

    overlay.id =
      'bus-track-availability-overlay';

    overlay.style.position =
      'fixed';

    overlay.style.top =
      '0';

    overlay.style.left =
      '0';

    overlay.style.width =
      '100%';

    overlay.style.height =
      '100%';

    overlay.style.zIndex =
      '2147483647';

    overlay.style.backgroundColor =
      'rgba(0, 0, 0, 0.85)';

    overlay.style.display =
      'flex';

    overlay.style.alignItems =
      'center';

    overlay.style.justifyContent =
      'center';

    overlay.style.flexDirection =
      'column';

    overlay.style.color =
      '#ffffff';

    overlay.style.fontFamily =
      'Arial, sans-serif';

    overlay.style.textAlign =
      'center';

    overlay.innerHTML = `
      <div
        style="
          max-width: 600px;
          padding: 40px;
        "
      >
        <h1
          style="
            margin-bottom: 20px;
          "
        >
          Página indisponível
        </h1>

        <p
          style="
            font-size: 18px;
            line-height: 1.6;
          "
        >
          A comunicação com o sistema foi perdida.
          Faça login novamente quando o sistema
          estiver disponível.
        </p>
      </div>
    `;

    document.body.appendChild(
      overlay
    );
  }
}
