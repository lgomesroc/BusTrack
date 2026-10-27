import { Component, OnDestroy, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { catchError, of, Subscription } from 'rxjs';
import { environment } from '../../../environments/environment';
import { SessionService } from '../../services/session.service';
import { Trip, TripService } from '../../services/trip.service';

@Component({
  selector: 'app-user-dashboard',
  templateUrl: './user-dashboard.component.html',
  styleUrls: ['./user-dashboard.component.css']
})
export class UserDashboardComponent implements OnInit, OnDestroy {
  trips: Trip[] = [];
  loadingTrips = true;
  tripsError = '';
  private presenceSubscription?: Subscription;
  private tripsSubscription?: Subscription;

  constructor(private router: Router, private session: SessionService, private http: HttpClient, private tripService: TripService) {}

  get upcomingTrips(): Trip[] {
    const now = Date.now();
    return this.trips.filter(t => new Date(t.departureTime).getTime() >= now)
      .sort((a,b) => new Date(a.departureTime).getTime() - new Date(b.departureTime).getTime());
  }
  get pastTrips(): Trip[] {
    const now = Date.now();
    return this.trips.filter(t => new Date(t.departureTime).getTime() < now)
      .sort((a,b) => new Date(b.departureTime).getTime() - new Date(a.departureTime).getTime());
  }

  ngOnInit(): void {
    this.presenceSubscription = this.http.post(`${environment.apiUrl}/api/UserPresence/presence`, {})
      .pipe(catchError(() => of(null))).subscribe();
    this.carregarMinhasViagens();
  }
  ngOnDestroy(): void { this.presenceSubscription?.unsubscribe(); this.tripsSubscription?.unsubscribe(); }

  carregarMinhasViagens(): void {
    this.loadingTrips = true;
    this.tripsError = '';
    this.tripsSubscription = this.tripService.getMyTrips().pipe(catchError(() => {
      this.tripsError = 'Não foi possível carregar suas viagens. Tente novamente mais tarde.';
      return of([] as Trip[]);
    })).subscribe(trips => { this.trips = trips; this.loadingTrips = false; });
  }
  navegarParaViagens(): void { this.router.navigate(['/trips']); }
  sair(): void {
    this.presenceSubscription?.unsubscribe();
    this.tripsSubscription?.unsubscribe();
    this.http.delete(`${environment.apiUrl}/api/UserPresence/presence`).pipe(catchError(() => of(null)))
      .subscribe(() => { this.session.clearSession(); this.router.navigate(['/enter-the-system'],{replaceUrl:true}); });
  }
}