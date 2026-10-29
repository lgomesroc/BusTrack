import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { CommonModule } from '@angular/common';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { Router } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { AdminDashboardComponent } from './admin-dashboard.component';
import { SessionService } from '../../services/session.service';
import { environment } from '../../../environments/environment';

describe('AdminDashboardComponent', () => {
  let fixture: ComponentFixture<AdminDashboardComponent>;
  let component: AdminDashboardComponent;
  let http: HttpTestingController;
  let session: jasmine.SpyObj<SessionService>;
  const dashboard = { generatedAtUtc:'2026-10-20T12:00:00Z', users:{total:8,administrators:1,normalUsers:7}, buses:{total:12}, drivers:{total:10}, passengers:{total:25}, routes:{total:6}, trips:{total:18,upcoming:4}, activeUsers:[] };
  const users = { generatedAtUtc:'2026-10-20T12:00:00Z', page:1, pageSize:10, total:1, totalPages:1, users:[{id:'u1',fullName:'Pessoa',email:'pessoa@example.com',role:'USER',createdAtUtc:null,lastSeenUtc:null,isOnline:false,status:'Offline'}] };
  beforeEach(async () => {
    session = jasmine.createSpyObj<SessionService>('SessionService',['clearSession']);
    await TestBed.configureTestingModule({declarations:[AdminDashboardComponent],imports:[CommonModule,HttpClientTestingModule,RouterTestingModule],providers:[{provide:SessionService,useValue:session}],schemas:[NO_ERRORS_SCHEMA]}).compileComponents();
    http = TestBed.inject(HttpTestingController);
    fixture = TestBed.createComponent(AdminDashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });
  afterEach(() => { fixture.destroy(); http.verify(); });
  function flushInitial(): void {
    http.expectOne(`${environment.apiUrl}/api/UserPresence/presence`).flush(null);
    http.expectOne(`${environment.apiUrl}/api/Dashboard/admin`).flush(dashboard);
    http.expectOne(r => r.url === `${environment.apiUrl}/api/Dashboard/users`).flush(users);
  }
  it('creates and loads administrative indicators and users', () => {
    flushInitial();
    expect(component.dashboard).toEqual(dashboard);
    expect(component.registeredUsers).toEqual(users.users);
    expect(component.usersTotal).toBe(1);
    expect(component.isLoading).toBeFalse();
  });
  it('sends the applied user search to the API', () => {
    flushInitial();
    component.userSearch = ' pessoa ';
    component.pesquisarUsuarios();
    const req = http.expectOne(r => r.url === `${environment.apiUrl}/api/Dashboard/users` && r.params.get('search') === 'pessoa');
    req.flush(users);
    expect(component.appliedSearch).toBe('pessoa');
  });
  it('changes page only when a next page exists', () => {
    flushInitial();
    component.usersTotalPages = 2;
    component.proximaPagina();
    http.expectOne(r => r.url === `${environment.apiUrl}/api/Dashboard/users` && r.params.get('page') === '2').flush({...users,page:2});
    expect(component.usersPage).toBe(2);
  });
  it('shows a message when the dashboard API fails', () => {
    http.expectOne(`${environment.apiUrl}/api/UserPresence/presence`).flush(null);
    http.expectOne(`${environment.apiUrl}/api/Dashboard/admin`).flush({}, {status:500,statusText:'Server Error'});
    http.expectOne(r => r.url === `${environment.apiUrl}/api/Dashboard/users`).flush(users);
    expect(component.errorMessage).toContain('Não foi possível carregar os indicadores');
    expect(component.isLoading).toBeFalse();
  });
  it('clears the session after logout', () => {
    flushInitial();
    const navigate = spyOn(TestBed.inject(Router),'navigate').and.returnValue(Promise.resolve(true));
    component.sair();
    http.expectOne(`${environment.apiUrl}/api/UserPresence/presence`).flush(null);
    expect(session.clearSession).toHaveBeenCalled();
    expect(navigate).toHaveBeenCalledWith(['/enter-the-system'],{replaceUrl:true});
  });
});