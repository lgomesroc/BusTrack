import { Component, OnDestroy, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { catchError, interval, of, Subscription } from 'rxjs';
import { environment } from '../../../environments/environment';
import { SessionService } from '../../services/session.service';
import { Trip, TripService } from '../../services/trip.service';

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
  trips: Trip[] = [];
  loadingTrips = true;
  tripsError = '';

  private refreshSubscription?: Subscription;
  private presenceSubscription?: Subscription;
  private tripsSubscription?: Subscription;

  constructor(
    private router: Router,
    private sessionService: SessionService,
    private http: HttpClient,
    private tripService: TripService
  ) {}

  get isAdmin(): boolean {
    return this.sessionService.isAdmin();
  }

  get upcomingTrips(): Trip[] {
    const now = Date.now();
    return this.trips
      .filter(trip => new Date(trip.departureTime).getTime() >= now)
      .sort((a, b) => new Date(a.departureTime).getTime() - new Date(b.departureTime).getTime());
  }

  get pastTrips(): Trip[] {
    const now = Date.now();
    return this.trips
      .filter(trip => new Date(trip.departureTime).getTime() < now)
      .sort((a, b) => new Date(b.departureTime).getTime() - new Date(a.departureTime).getTime());
  }

  ngOnInit(): void {
    this.registrarPresenca();
    this.refreshSubscription = interval(30000).subscribe(() => {
      this.registrarPresenca();
      if (this.isAdmin) this.carregarDashboard();
    });

    if (this.isAdmin) {
      this.carregarDashboard();
    } else {
      this.carregarMinhasViagens();
    }
  }

  ngOnDestroy(): void {
    this.refreshSubscription?.unsubscribe();
    this.presenceSubscription?.unsubscribe();
    this.tripsSubscription?.unsubscribe();
  }

  private registrarPresenca(): void {
    this.presenceSubscription?.unsubscribe();
    this.presenceSubscription = this.http
      .post(`${environment.apiUrl}/api/UserPresence/presence`, {})
      .pipe(catchError(() => of(null)))
      .subscribe();
  }

  carregarDashboard(): void {
    if (!this.isAdmin) return;
    this.isLoading = true;
    this.http.get<AdminDashboardData>(`${environment.apiUrl}/api/Dashboard/admin`).subscribe({
      next: data => {
        this.dashboard = data;
        this.isLoading = false;
        this.errorMessage = '';
      },
      error: () => {
        this.errorMessage = 'Não foi possível carregar os indicadores reais da API. Verifique se a API está atualizada.';
        this.isLoading = false;
      }
    });
  }

  private carregarMinhasViagens(): void {
    this.loadingTrips = true;
    this.tripsError = '';
    this.tripsSubscription = this.tripService.getMyTrips().pipe(
      catchError(() => {
        this.tripsError = 'Não foi possível carregar suas viagens. Tente novamente mais tarde.';
        return of([] as Trip[]);
      })
    ).subscribe(trips => {
      this.trips = trips;
      this.loadingTrips = false;
    });
  }

  navegarParaViagens(): void { this.router.navigate(['/trips']); }
  navegarParaRotas(): void { this.router.navigate(['/routes']); }
  navegarParaOnibus(): void { this.router.navigate(['/buses']); }
  navegarParaMotoristas(): void { this.router.navigate(['/drivers']); }
  navegarParaPassageiros(): void { this.router.navigate(['/passengers']); }
  navegarParaManutencao(): void { this.router.navigate(['/maintenance']); }

  sair(): void {
    this.refreshSubscription?.unsubscribe();
    this.presenceSubscription?.unsubscribe();
    this.tripsSubscription?.unsubscribe();
    this.http.delete(`${environment.apiUrl}/api/UserPresence/presence`)
      .pipe(catchError(() => of(null)))
      .subscribe(() => {
        this.sessionService.clearSession();
        this.router.navigate(['/enter-the-system'], { replaceUrl: true });
      });
  }
}