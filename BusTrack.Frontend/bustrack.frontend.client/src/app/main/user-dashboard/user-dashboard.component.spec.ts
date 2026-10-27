import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { CommonModule } from '@angular/common';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { Router } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { of, throwError } from 'rxjs';
import { UserDashboardComponent } from './user-dashboard.component';
import { SessionService } from '../../services/session.service';
import { TripService } from '../../services/trip.service';
import { environment } from '../../../environments/environment';

describe('UserDashboardComponent', () => {
  let fixture: ComponentFixture<UserDashboardComponent>;
  let component: UserDashboardComponent;
  let http: HttpTestingController;
  let session: jasmine.SpyObj<SessionService>;
  let trips: jasmine.SpyObj<TripService>;
  const trip = { id:'t1', bus:null, driver:null, route:{id:'r1',name:'Centro',description:'',origin:'Origem',destination:'Destino',distance:10}, departureTime:'2099-10-20T12:00:00Z', startedAt:null, arrivalTime:null, duration:30, limitPassengers:20, status:'Agendada' as const, passengers:[] };
  beforeEach(async () => {
    session = jasmine.createSpyObj<SessionService>('SessionService',['clearSession']);
    trips = jasmine.createSpyObj<TripService>('TripService',['getMyTrips']);
    trips.getMyTrips.and.returnValue(of([trip]));
    await TestBed.configureTestingModule({declarations:[UserDashboardComponent],imports:[CommonModule,HttpClientTestingModule,RouterTestingModule],providers:[{provide:SessionService,useValue:session},{provide:TripService,useValue:trips}],schemas:[NO_ERRORS_SCHEMA]}).compileComponents();
    http = TestBed.inject(HttpTestingController);
    fixture = TestBed.createComponent(UserDashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });
  afterEach(() => { fixture.destroy(); http.verify(); });
  it('loads only the current user trips', () => {
    http.expectOne(`${environment.apiUrl}/api/UserPresence/presence`).flush(null);
    expect(trips.getMyTrips).toHaveBeenCalled();
    expect(component.trips).toEqual([trip]);
    expect(component.upcomingTrips.length).toBe(1);
    expect(component.loadingTrips).toBeFalse();
  });
  it('shows an error when the trips request fails', () => {
    trips.getMyTrips.and.returnValue(throwError(() => new Error('offline')));
    component.carregarMinhasViagens();
    http.expectOne(`${environment.apiUrl}/api/UserPresence/presence`).flush(null);
    expect(component.tripsError).toContain('Não foi possível carregar suas viagens');
    expect(component.loadingTrips).toBeFalse();
  });
  it('navigates to trips', () => {
    const navigate = spyOn(TestBed.inject(Router),'navigate').and.returnValue(Promise.resolve(true));
    component.navegarParaViagens();
    expect(navigate).toHaveBeenCalledWith(['/trips']);
  });
  it('clears the session after logout', () => {
    http.expectOne(`${environment.apiUrl}/api/UserPresence/presence`).flush(null);
    const navigate = spyOn(TestBed.inject(Router),'navigate').and.returnValue(Promise.resolve(true));
    component.sair();
    http.expectOne(`${environment.apiUrl}/api/UserPresence/presence`).flush(null);
    expect(session.clearSession).toHaveBeenCalled();
    expect(navigate).toHaveBeenCalledWith(['/enter-the-system'],{replaceUrl:true});
  });
});