import { CommonModule } from '@angular/common';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { Router } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { of } from 'rxjs';

import { DashboardComponent } from './dashboard.component';
import { SessionService } from '../../services/session.service';
import { TripService } from '../../services/trip.service';
import { environment } from '../../../environments/environment';

describe('DashboardComponent', () => {
  let component: DashboardComponent;
  let fixture: ComponentFixture<DashboardComponent>;
  let httpMock: HttpTestingController;
  let router: Router;

  let adminMode: boolean;
  let sessionServiceMock: jasmine.SpyObj<SessionService>;
  let tripServiceMock: jasmine.SpyObj<TripService>;

  const dashboardResponse = {
    generatedAtUtc: '2026-10-20T12:00:00Z',
    users: {
      total: 8,
      administrators: 1,
      normalUsers: 7
    },
    buses: { total: 12 },
    drivers: { total: 10 },
    passengers: { total: 25 },
    routes: { total: 6 },
    trips: {
      total: 18,
      upcoming: 4
    },
    activeUsers: [
      {
        userId: 'user-1',
        fullName: 'Usuário Online',
        email: 'online@example.com',
        role: 'USER',
        lastSeenUtc: '2026-10-20T11:59:30Z'
      }
    ]
  };

  const usersResponse = {
    generatedAtUtc: '2026-10-20T12:00:00Z',
    page: 1,
    pageSize: 10,
    total: 12,
    totalPages: 2,
    users: [
      {
        id: 'user-1',
        fullName: 'Usuário Online',
        email: 'online@example.com',
        role: 'USER',
        createdAtUtc: '2026-10-01T10:00:00Z',
        lastSeenUtc: '2026-10-20T11:59:30Z',
        isOnline: true,
        status: 'Online'
      }
    ]
  };

  function initializeComponent(): void {
    fixture = TestBed.createComponent(DashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  }

  function flushAdminInitialRequests(): void {
    const presenceRequest = httpMock.expectOne(
      `${environment.apiUrl}/api/UserPresence/presence`
    );

    expect(presenceRequest.request.method).toBe('POST');
    presenceRequest.flush(null);

    const dashboardRequest = httpMock.expectOne(
      `${environment.apiUrl}/api/Dashboard/admin`
    );

    expect(dashboardRequest.request.method).toBe('GET');
    dashboardRequest.flush(dashboardResponse);

    const usersRequest = httpMock.expectOne(
      request =>
        request.url === `${environment.apiUrl}/api/Dashboard/users` &&
        request.params.get('page') === '1' &&
        request.params.get('pageSize') === '10'
    );

    expect(usersRequest.request.method).toBe('GET');
    usersRequest.flush(usersResponse);
  }

  beforeEach(async () => {
    adminMode = true;

    sessionServiceMock = jasmine.createSpyObj<SessionService>(
      'SessionService',
      ['isAdmin', 'clearSession', 'isAuthenticated']
    );

    sessionServiceMock.isAdmin.and.callFake(() => adminMode);
    sessionServiceMock.isAuthenticated.and.returnValue(true);

    tripServiceMock = jasmine.createSpyObj<TripService>(
      'TripService',
      ['getMyTrips']
    );

    tripServiceMock.getMyTrips.and.returnValue(of([]));

    await TestBed.configureTestingModule({
      declarations: [DashboardComponent],
      imports: [
        CommonModule,
        HttpClientTestingModule,
        RouterTestingModule
      ],
      providers: [
        { provide: SessionService, useValue: sessionServiceMock },
        { provide: TripService, useValue: tripServiceMock }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();

    httpMock = TestBed.inject(HttpTestingController);
    router = TestBed.inject(Router);
  });

  afterEach(() => {
    fixture?.destroy();
    httpMock.verify();
  });

  it('should create the component', () => {
    initializeComponent();

    expect(component).toBeTruthy();

    flushAdminInitialRequests();
  });

  it('should load the administrative dashboard and registered users', () => {
    initializeComponent();

    flushAdminInitialRequests();

    expect(component.dashboard).toEqual(dashboardResponse);
    expect(component.isLoading).toBeFalse();
    expect(component.errorMessage).toBe('');

    expect(component.registeredUsers).toEqual(usersResponse.users);
    expect(component.usersTotal).toBe(12);
    expect(component.usersPage).toBe(1);
    expect(component.usersTotalPages).toBe(2);
    expect(component.usersLoading).toBeFalse();
    expect(component.usersError).toBe('');
  });

  it('should display the real counts returned by the API', () => {
    initializeComponent();

    flushAdminInitialRequests();

    expect(component.dashboard?.users.total).toBe(8);
    expect(component.dashboard?.users.administrators).toBe(1);
    expect(component.dashboard?.users.normalUsers).toBe(7);
    expect(component.dashboard?.buses.total).toBe(12);
    expect(component.dashboard?.drivers.total).toBe(10);
    expect(component.dashboard?.passengers.total).toBe(25);
    expect(component.dashboard?.routes.total).toBe(6);
    expect(component.dashboard?.trips.total).toBe(18);
    expect(component.dashboard?.trips.upcoming).toBe(4);
    expect(component.dashboard?.activeUsers.length).toBe(1);
  });

  it('should search registered users using the applied search term', () => {
    initializeComponent();

    flushAdminInitialRequests();

    component.userSearch = 'operador';
    component.pesquisarUsuarios();

    const request = httpMock.expectOne(
      `${environment.apiUrl}/api/Dashboard/users?page=1&pageSize=10&search=operador`
    );

    expect(request.request.method).toBe('GET');
    expect(request.request.params.get('search')).toBe('operador');

    request.flush({
      ...usersResponse,
      total: 1,
      totalPages: 1,
      users: [
        {
          ...usersResponse.users[0],
          fullName: 'Operador Teste'
        }
      ]
    });

    expect(component.usersPage).toBe(1);
    expect(component.usersTotal).toBe(1);
    expect(component.registeredUsers[0].fullName).toBe('Operador Teste');
    expect(component.usersLoading).toBeFalse();
  });

  it('should clear the search and reload the first page', () => {
    initializeComponent();

    flushAdminInitialRequests();

    component.userSearch = 'termo';
    component.appliedSearch = 'termo';
    component.usersPage = 2;

    component.limparPesquisa();

    const request = httpMock.expectOne(
      `${environment.apiUrl}/api/Dashboard/users?page=1&pageSize=10`
    );

    expect(request.request.params.has('search')).toBeFalse();

    request.flush({
      ...usersResponse,
      page: 1
    });

    expect(component.userSearch).toBe('');
    expect(component.appliedSearch).toBe('');
    expect(component.usersPage).toBe(1);
  });

  it('should load the next page when another page exists', () => {
    initializeComponent();

    flushAdminInitialRequests();

    component.proximaPagina();

    const request = httpMock.expectOne(
      `${environment.apiUrl}/api/Dashboard/users?page=2&pageSize=10`
    );

    request.flush({
      ...usersResponse,
      page: 2,
      users: []
    });

    expect(component.usersPage).toBe(2);
    expect(component.usersStart).toBe(11);
    expect(component.usersEnd).toBe(12);
  });

  it('should not request a page beyond the last page', () => {
    initializeComponent();

    flushAdminInitialRequests();

    component.usersPage = 2;
    component.usersTotalPages = 2;

    component.proximaPagina();

    httpMock.expectNone(
      request =>
        request.url === `${environment.apiUrl}/api/Dashboard/users` &&
        request.params.get('page') === '3'
    );

    expect(component.usersPage).toBe(2);
  });

  it('should show an error when the dashboard request fails', () => {
    initializeComponent();

    const presenceRequest = httpMock.expectOne(
      `${environment.apiUrl}/api/UserPresence/presence`
    );
    presenceRequest.flush(null);

    const dashboardRequest = httpMock.expectOne(
      `${environment.apiUrl}/api/Dashboard/admin`
    );

    dashboardRequest.flush(
      { message: 'Internal server error' },
      { status: 500, statusText: 'Server Error' }
    );

    const usersRequest = httpMock.expectOne(
      `${environment.apiUrl}/api/Dashboard/users?page=1&pageSize=10`
    );
    usersRequest.flush(usersResponse);

    expect(component.dashboard).toBeNull();
    expect(component.isLoading).toBeFalse();
    expect(component.errorMessage).toContain(
      'Não foi possível carregar os indicadores'
    );
  });

  it('should show an error when registered users cannot be loaded', () => {
    initializeComponent();

    const presenceRequest = httpMock.expectOne(
      `${environment.apiUrl}/api/UserPresence/presence`
    );
    presenceRequest.flush(null);

    const dashboardRequest = httpMock.expectOne(
      `${environment.apiUrl}/api/Dashboard/admin`
    );
    dashboardRequest.flush(dashboardResponse);

    const usersRequest = httpMock.expectOne(
      `${environment.apiUrl}/api/Dashboard/users?page=1&pageSize=10`
    );

    usersRequest.flush(
      { message: 'Internal server error' },
      { status: 500, statusText: 'Server Error' }
    );

    expect(component.usersError).toBe(
      'Não foi possível carregar a lista de usuários.'
    );
    expect(component.usersLoading).toBeFalse();
  });

  it('should load personal trips without requesting administrative data for a regular user', () => {
    adminMode = false;
    initializeComponent();

    const presenceRequest = httpMock.expectOne(
      `${environment.apiUrl}/api/UserPresence/presence`
    );

    expect(presenceRequest.request.method).toBe('POST');
    presenceRequest.flush(null);

    expect(tripServiceMock.getMyTrips).toHaveBeenCalledTimes(1);

    httpMock.expectNone(`${environment.apiUrl}/api/Dashboard/admin`);
    httpMock.expectNone(
      request => request.url === `${environment.apiUrl}/api/Dashboard/users`
    );

    expect(component.loadingTrips).toBeFalse();
    expect(component.trips).toEqual([]);
  });

  it('should clear the session and redirect after logout', () => {
    adminMode = false;
    initializeComponent();

    const presenceRequest = httpMock.expectOne(
      `${environment.apiUrl}/api/UserPresence/presence`
    );
    presenceRequest.flush(null);

    const navigateSpy = spyOn(router, 'navigate')
      .and.returnValue(Promise.resolve(true));

    component.sair();

    const deleteRequest = httpMock.expectOne(
      `${environment.apiUrl}/api/UserPresence/presence`
    );

    expect(deleteRequest.request.method).toBe('DELETE');
    deleteRequest.flush(null);

    expect(sessionServiceMock.clearSession).toHaveBeenCalledTimes(1);
    expect(navigateSpy).toHaveBeenCalledWith(
      ['/enter-the-system'],
      { replaceUrl: true }
    );
  });
});
