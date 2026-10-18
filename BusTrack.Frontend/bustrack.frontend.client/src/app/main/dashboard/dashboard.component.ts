import { Component, OnDestroy, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { catchError, interval, of, Subscription } from 'rxjs';
import { environment } from '../../../environments/environment';
import { SessionService } from '../../services/session.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit, OnDestroy {
  private presenceTimer?: Subscription;
  private presenceRequest?: Subscription;

  constructor(
    private router: Router,
    private sessionService: SessionService,
    private http: HttpClient
  ) {}

  get isAdmin(): boolean {
    return this.sessionService.isAdmin();
  }

  ngOnInit(): void {
    this.registrarPresenca();
    this.presenceTimer = interval(30000).subscribe(() => this.registrarPresenca());
  }

  ngOnDestroy(): void {
    this.presenceTimer?.unsubscribe();
    this.presenceRequest?.unsubscribe();
  }

  private registrarPresenca(): void {
    this.presenceRequest?.unsubscribe();
    this.presenceRequest = this.http.post(
      `${environment.apiUrl}/api/UserPresence/presence`, {}
    ).pipe(catchError(() => of(null))).subscribe();
  }

  navegarParaViagens(): void { this.router.navigate(['/trips']); }
  navegarParaOnibus(): void { this.router.navigate(['/buses']); }
  navegarParaRotas(): void { this.router.navigate(['/routes']); }
  navegarParaMotoristas(): void { this.router.navigate(['/drivers']); }
  navegarParaPassageiros(): void { this.router.navigate(['/passengers']); }
  navegarParaManutencao(): void { this.router.navigate(['/maintenance']); }

  sair(): void {
    this.presenceTimer?.unsubscribe();
    this.presenceRequest?.unsubscribe();
    this.http.delete(`${environment.apiUrl}/api/UserPresence/presence`)
      .pipe(catchError(() => of(null)))
      .subscribe(() => {
        this.sessionService.clearSession();
        this.router.navigate(['/enter-the-system'], { replaceUrl: true });
      });
  }
}