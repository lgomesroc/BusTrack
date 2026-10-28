
import { Component, OnDestroy, OnInit } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
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
  lastSeenUtc: string | null;
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

interface RegisteredUser {
  id: string;
  fullName: string;
  email: string;
  role: string;
  createdAtUtc: string | null;
  lastSeenUtc: string | null;
  isOnline: boolean;
  status: string;
}

interface RegisteredUsersResponse {
  generatedAtUtc: string;
  page: number;
  pageSize: number;
  total: number;
  totalPages: number;
  users: RegisteredUser[];
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

  registeredUsers: RegisteredUser[] = [];
  usersLoading = false;
  usersError = '';
  userSearch = '';
  appliedSearch = '';
  usersPage = 1;
  usersPageSize = 10;
  usersTotal = 0;
  usersTotalPages = 0;

  trips: Trip[] = [];
  loadingTrips = true;
  tripsError = '';

  private refreshSubscription?: Subscription;
  private presenceSubscription?: Subscription;
  private tripsSubscription?: Subscription;
  private usersSubscription?: Subscription;

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
      .sort(
        (a, b) =>
          new Date(a.departureTime).getTime() -
          new Date(b.departureTime).getTime()
      );
  }

  get pastTrips(): Trip[] {
    const now = Date.now();

    return this.trips
      .filter(trip => new Date(trip.departureTime).getTime() < now)
      .sort(
        (a, b) =>
          new Date(b.departureTime).getTime() -
          new Date(a.departureTime).getTime()
      );
  }

  get usersStart(): number {
    return this.usersTotal === 0
      ? 0
      : (this.usersPage - 1) * this.usersPageSize + 1;
  }

  get usersEnd(): number {
    return Math.min(
      this.usersPage * this.usersPageSize,
      this.usersTotal
    );
  }

  ngOnInit(): void {
    this.registrarPresenca();

    this.refreshSubscription = interval(30000).subscribe(() => {
      this.registrarPresenca();

      if (this.isAdmin) {
        this.carregarDashboard();
        this.carregarUsuarios();
      }
    });

    if (this.isAdmin) {
      this.carregarDashboard();
      this.carregarUsuarios();
    } else {
      this.carregarMinhasViagens();
    }
  }

  ngOnDestroy(): void {
    this.refreshSubscription?.unsubscribe();
    this.presenceSubscription?.unsubscribe();
    this.tripsSubscription?.unsubscribe();
    this.usersSubscription?.unsubscribe();
  }

  private registrarPresenca(): void {
    this.presenceSubscription?.unsubscribe();

    this.presenceSubscription = this.http
      .post(`${environment.apiUrl}/api/UserPresence/presence`, {})
      .pipe(catchError(() => of(null)))
      .subscribe();
  }

  carregarDashboard(): void {
    if (!this.isAdmin) {
      return;
    }

    this.isLoading = true;

    this.http
      .get<AdminDashboardData>(
        `${environment.apiUrl}/api/Dashboard/admin`
      )
      .subscribe({
        next: data => {
          this.dashboard = data;
          this.isLoading = false;
          this.errorMessage = '';
        },
        error: () => {
          this.errorMessage =
            'Não foi possível carregar os indicadores. Verifique se a API está atualizada.';
          this.isLoading = false;
        }
      });
  }

  carregarUsuarios(): void {
    if (!this.isAdmin) {
      return;
    }

    this.usersSubscription?.unsubscribe();
    this.usersLoading = true;
    this.usersError = '';

    let params = new HttpParams()
      .set('page', this.usersPage)
      .set('pageSize', this.usersPageSize);

    if (this.appliedSearch.trim()) {
      params = params.set('search', this.appliedSearch.trim());
    }

    this.usersSubscription = this.http
      .get<RegisteredUsersResponse>(
        `${environment.apiUrl}/api/Dashboard/users`,
        { params }
      )
      .pipe(
        catchError(() => {
          this.usersError =
            'Não foi possível carregar a lista de usuários.';
          this.usersLoading = false;
          return of(null);
        })
      )
      .subscribe(response => {
        if (!response) {
          return;
        }

        this.registeredUsers = response.users;
        this.usersPage = response.page;
        this.usersPageSize = response.pageSize;
        this.usersTotal = response.total;
        this.usersTotalPages = response.totalPages;
        this.usersLoading = false;
      });
  }

  pesquisarUsuarios(): void {
    this.appliedSearch = this.userSearch.trim();
    this.usersPage = 1;
    this.carregarUsuarios();
  }

  limparPesquisa(): void {
    this.userSearch = '';
    this.appliedSearch = '';
    this.usersPage = 1;
    this.carregarUsuarios();
  }

  paginaAnterior(): void {
    if (this.usersPage <= 1 || this.usersLoading) {
      return;
    }

    this.usersPage--;
    this.carregarUsuarios();
  }

  proximaPagina(): void {
    if (
      this.usersPage >= this.usersTotalPages ||
      this.usersLoading
    ) {
      return;
    }

    this.usersPage++;
    this.carregarUsuarios();
  }

  private carregarMinhasViagens(): void {
    this.loadingTrips = true;
    this.tripsError = '';

    this.tripsSubscription = this.tripService
      .getMyTrips()
      .pipe(
        catchError(() => {
          this.tripsError =
            'Não foi possível carregar suas viagens. Tente novamente mais tarde.';
          return of([] as Trip[]);
        })
      )
      .subscribe(trips => {
        this.trips = trips;
        this.loadingTrips = false;
      });
  }

  navegarParaViagens(): void {
    this.router.navigate(['/trips']);
  }

  navegarParaRotas(): void {
    this.router.navigate(['/routes']);
  }

  navegarParaOnibus(): void {
    this.router.navigate(['/buses']);
  }

  navegarParaMotoristas(): void {
    this.router.navigate(['/drivers']);
  }

  navegarParaPassageiros(): void {
    this.router.navigate(['/passengers']);
  }

  navegarParaManutencao(): void {
    this.router.navigate(['/maintenance']);
  }

  navegarParaRelatorios(): void {
    this.router.navigate(['/operational-reports']);
  }

  sair(): void {
    this.refreshSubscription?.unsubscribe();
    this.presenceSubscription?.unsubscribe();
    this.tripsSubscription?.unsubscribe();
    this.usersSubscription?.unsubscribe();

    this.http
      .delete(`${environment.apiUrl}/api/UserPresence/presence`)
      .pipe(catchError(() => of(null)))
      .subscribe(() => {
        this.sessionService.clearSession();
        this.router.navigate(
          ['/enter-the-system'],
          { replaceUrl: true }
        );
      });
  }
}
