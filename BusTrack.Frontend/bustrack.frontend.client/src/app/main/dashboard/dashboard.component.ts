import { Component, OnDestroy, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { catchError, interval, of, Subscription } from 'rxjs';
import { environment } from '../../../environments/environment';
import { SessionService } from '../../services/session.service';
import { Trip, TripService } from '../../services/trip.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit, OnDestroy {
  private presenceTimer?: Subscription;
  private presenceRequest?: Subscription;

  trips: Trip[] = [];
  loadingTrips = true;
  tripsError = '';

  constructor(
    private router: Router,
    private sessionService: SessionService,
    private http: HttpClient,
    private tripService: TripService
  ) {}

  get isAdmin(): boolean { return this.sessionService.isAdmin(); }
  get upcomingTrips(): Trip[] {
    const now = Date.now();
    return this.trips.filter(trip => new Date(trip.departureTime).getTime() >= now)
      .sort((a, b) => new Date(a.departureTime).getTime() - new Date(b.departureTime).getTime());
  }
  get pastTrips(): Trip[] {
    const now = Date.now();
    return this.trips.filter(trip => new Date(trip.departureTime).getTime() < now)
      .sort((a, b) => new Date(b.departureTime).getTime() - new Date(a.departureTime).getTime());
  }

  ngOnInit(): void {
    this.registrarPresenca();
    this.presenceTimer = interval(30000).subscribe(() => this.registrarPresenca());
    this.carregarMinhasViagens();
  }

  ngOnDestroy(): void {
    this.presenceTimer?.unsubscribe();
    this.presenceRequest?.unsubscribe();
  }

  private carregarMinhasViagens(): void {
    this.loadingTrips = true;
    this.tripsError = '';
    this.tripService.getMyTrips().pipe(
      catchError(() => {
        this.tripsError = 'Não foi possível carregar suas viagens. Tente novamente mais tarde.';
        return of([] as Trip[]);
      })
    ).subscribe(trips => {
      this.trips = trips;
      this.loadingTrips = false;
    });
  }

  private registrarPresenca(): void {
    this.presenceRequest?.unsubscribe();
    this.presenceRequest = this.http.post(`${environment.apiUrl}/api/UserPresence/presence`, {})
      .pipe(catchError(() => of(null))).subscribe();
  }

  navegarParaViagens(): void { this.router.navigate(['/trips']); }
  navegarParaRotas(): void { this.router.navigate(['/routes']); }
  navegarParaOnibus(): void { this.router.navigate(['/buses']); }
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