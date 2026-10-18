import { Component, OnDestroy, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { catchError, interval, of, Subscription } from 'rxjs';
import { environment } from '../../../environments/environment';
import { SessionService } from '../../services/session.service';

interface ActiveUser {
  userId: string;
  fullName: string;
  email: string;
  role: string;
  lastSeenUtc: string;
}
interface AdminDashboardData {
  generatedAtUtc: string;
  users: { total: number; administrators: number; normalUsers: number };
  buses: { total: number };
  drivers: { total: number };
  passengers: { total: number };
  routes: { total: number };
  trips: { total: number; upcoming: number };
  activeUsers: ActiveUser[];
}

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit, OnDestroy {
  dashboard: AdminDashboardData | null = null;
  isLoading = false;
  errorMessage = '';
  private refreshSubscription?: Subscription;
  private presenceSubscription?: Subscription;

  constructor(private router: Router, private http: HttpClient, private sessionService: SessionService) {}

  get isAdmin(): boolean { return this.sessionService.isAdmin(); }

  ngOnInit(): void {
    this.registrarPresenca();
    this.refreshSubscription = interval(30000).subscribe(() => {
      this.registrarPresenca();
      if (this.isAdmin) this.carregarDashboard();
    });
    if (this.isAdmin) this.carregarDashboard();
  }

  ngOnDestroy(): void {
    this.refreshSubscription?.unsubscribe();
    this.presenceSubscription?.unsubscribe();
  }

  private registrarPresenca(): void {
    this.presenceSubscription?.unsubscribe();
    this.presenceSubscription = this.http.post(`${environment.apiUrl}/api/UserPresence/presence`, {})
      .pipe(catchError(() => of(null))).subscribe();
  }

  carregarDashboard(): void {
    if (!this.isAdmin) return;
    this.isLoading = true;
    this.http.get<AdminDashboardData>(`${environment.apiUrl}/api/Dashboard/admin`).subscribe({
      next: data => { this.dashboard = data; this.isLoading = false; this.errorMessage = ''; },
      error: () => { this.errorMessage = 'Não foi possível carregar os indicadores reais da API. Verifique se a API está atualizada.'; this.isLoading = false; }
    });
  }

  navegarParaViagens(): void { this.router.navigate(['/trips']); }
  navegarParaOnibus(): void { this.router.navigate(['/buses']); }
  navegarParaRotas(): void { this.router.navigate(['/routes']); }
  navegarParaMotoristas(): void { this.router.navigate(['/drivers']); }
  navegarParaPassageiros(): void { this.router.navigate(['/passengers']); }
  navegarParaManutencao(): void { this.router.navigate(['/maintenance']); }

  sair(): void {
    this.http.delete(`${environment.apiUrl}/api/UserPresence/presence`).pipe(catchError(() => of(null))).subscribe({
      complete: () => {
        this.sessionService.clearSession();
        this.router.navigate(['/enter-the-system'], { replaceUrl: true });
      }
    });
  }
}