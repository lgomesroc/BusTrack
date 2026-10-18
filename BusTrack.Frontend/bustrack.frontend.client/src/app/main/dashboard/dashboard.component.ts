import { Component, OnDestroy, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { catchError, interval, of, Subscription } from 'rxjs';
import { environment } from '../../../environments/environment';
import { SessionService } from '../../services/session.service';

interface AdminDashboardData {
  generatedAtUtc: string;
  users: { total: number; administrators: number; normalUsers: number };
  buses: { total: number };
  drivers: { total: number };
  passengers: { total: number };
  routes: { total: number };
  trips: { total: number; upcoming: number };
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

  constructor(
    private router: Router,
    private http: HttpClient,
    private sessionService: SessionService
  ) {}

  get isAdmin(): boolean { return this.sessionService.isAdmin(); }

  ngOnInit(): void {
    if (this.isAdmin) {
      this.carregarDashboard();
      this.refreshSubscription = interval(30000).subscribe(() => this.carregarDashboard());
    }
  }

  ngOnDestroy(): void { this.refreshSubscription?.unsubscribe(); }

  carregarDashboard(): void {
    if (!this.isAdmin) return;
    this.isLoading = true;
    this.http.get<AdminDashboardData>(`${environment.apiUrl}/api/DashboardAPI/admin`).subscribe({
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
    this.sessionService.clearSession();
    this.router.navigate(['/enter-the-system'], { replaceUrl: true });
  }
}