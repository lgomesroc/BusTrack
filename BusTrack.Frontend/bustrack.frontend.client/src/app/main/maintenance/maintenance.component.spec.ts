import { CommonModule, registerLocaleData } from '@angular/common';
import localePtBr from '@angular/common/locales/pt';
import { FormsModule } from '@angular/forms';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';

import { MaintenanceComponent } from './maintenance.component';
import { Bus, BusService } from '../../services/bus.service';
import {
  Maintenance,
  MaintenancePayload,
  MaintenanceService
} from '../../services/maintenance.service';
import { SessionService } from '../../services/session.service';

registerLocaleData(localePtBr);

describe('MaintenanceComponent', () => {
  let component: MaintenanceComponent;
  let fixture: ComponentFixture<MaintenanceComponent>;
  let router: jasmine.SpyObj<Router>;
  let busService: jasmine.SpyObj<BusService>;
  let maintenanceService: jasmine.SpyObj<MaintenanceService>;
  let sessionService: jasmine.SpyObj<SessionService>;

  const buses: Bus[] = [
    {
      id: 'bus-1',
      number: '101',
      licensePlate: 'ABC1D23',
      model: 'Ônibus de teste',
      capacity: 40
    }
  ];

  const maintenance: Maintenance = {
    id: 'maintenance-1',
    busId: 'bus-1',
    busNumber: '101',
    serviceType: 'Troca de óleo',
    description: 'Manutenção preventiva',
    serviceDate: '2026-10-27T03:00:00.000Z',
    completedDate: null,
    cost: 250.5,
    status: 'Agendada'
  };

  function brazilToday(): string {
    const parts = new Intl.DateTimeFormat('en-CA', {
      timeZone: 'America/Sao_Paulo',
      year: 'numeric',
      month: '2-digit',
      day: '2-digit'
    }).formatToParts(new Date());

    const year = parts.find(part => part.type === 'year')?.value;
    const month = parts.find(part => part.type === 'month')?.value;
    const day = parts.find(part => part.type === 'day')?.value;

    return `${year}-${month}-${day}`;
  }

  function dateOffset(offsetDays: number): string {
    const [year, month, day] = brazilToday().split('-').map(Number);

    return new Date(Date.UTC(year, month - 1, day + offsetDays))
      .toISOString()
      .slice(0, 10);
  }

  function validPayload(
    status: MaintenancePayload['status'] = 'Agendada',
    serviceDate = brazilToday()
  ): MaintenancePayload {
    return {
      busId: 'bus-1',
      serviceType: 'Troca de óleo',
      description: 'Manutenção preventiva',
      serviceDate,
      completedDate: null,
      cost: 250.5,
      status
    };
  }

  function initializeComponent(): void {
    fixture = TestBed.createComponent(MaintenanceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  }

  beforeEach(async () => {
    router = jasmine.createSpyObj<Router>('Router', ['navigate']);

    busService = jasmine.createSpyObj<BusService>(
      'BusService',
      ['getBuses']
    );

    maintenanceService = jasmine.createSpyObj<MaintenanceService>(
      'MaintenanceService',
      ['getAll', 'create', 'update', 'delete']
    );

    sessionService = jasmine.createSpyObj<SessionService>(
      'SessionService',
      ['isAdmin']
    );

    busService.getBuses.and.returnValue(of(buses));
    maintenanceService.getAll.and.returnValue(of([maintenance]));

    maintenanceService.create.and.returnValue(
      of({
        ...maintenance,
        id: 'maintenance-new'
      })
    );

    maintenanceService.update.and.returnValue(of(maintenance));
    maintenanceService.delete.and.returnValue(of(void 0));

    sessionService.isAdmin.and.returnValue(true);
    router.navigate.and.returnValue(Promise.resolve(true));

    await TestBed.configureTestingModule({
      imports: [CommonModule, FormsModule],
      declarations: [MaintenanceComponent],
      providers: [
        { provide: Router, useValue: router },
        { provide: BusService, useValue: busService },
        { provide: MaintenanceService, useValue: maintenanceService },
        { provide: SessionService, useValue: sessionService }
      ]
    }).compileComponents();
  });

  afterEach(() => {
    fixture?.destroy();
    TestBed.resetTestingModule();
  });

  it('creates the component and loads buses and maintenance records', () => {
    initializeComponent();

    expect(component).toBeTruthy();
    expect(busService.getBuses).toHaveBeenCalledTimes(1);
    expect(maintenanceService.getAll).toHaveBeenCalledTimes(1);
    expect(component.buses).toEqual(buses);
    expect(component.maintenances).toEqual([maintenance]);
    expect(component.isLoading).toBeFalse();
  });

  it('shows the maintenance history in the template', () => {
    initializeComponent();

    const element: HTMLElement = fixture.nativeElement;

    expect(element.textContent).toContain('Serviços / Manutenção');
    expect(element.textContent).toContain('Histórico de serviços');
    expect(element.textContent).toContain('Troca de óleo');
    expect(element.textContent).toContain('Manutenção preventiva');
    expect(element.textContent).toContain('Agendada');
  });

  it('shows the empty state when no maintenance records exist', () => {
    maintenanceService.getAll.and.returnValue(of([]));
    initializeComponent();

    const element: HTMLElement = fixture.nativeElement;

    expect(element.textContent).toContain('Nenhuma manutenção registrada');
    expect(element.querySelector('.empty-state')).not.toBeNull();
  });

  it('shows an error when maintenance records cannot be loaded', () => {
    maintenanceService.getAll.and.returnValue(
      throwError(() => new Error('API indisponível'))
    );

    spyOn(console, 'error');
    initializeComponent();

    expect(component.errorMessage).toBe(
      'Não foi possível carregar as manutenções.'
    );
    expect(component.isLoading).toBeFalse();
    expect(fixture.nativeElement.querySelector('[role="alert"]'))
      .not.toBeNull();
  });

  it('shows an error when buses cannot be loaded', () => {
    initializeComponent();

    busService.getBuses.and.returnValue(
      throwError(() => new Error('API indisponível'))
    );

    component.loadBuses();

    expect(component.errorMessage).toBe(
      'Não foi possível carregar os ônibus.'
    );
  });

  it('prevents starting a new maintenance while loading', () => {
    initializeComponent();
    component.isLoading = true;

    component.startCreate();

    expect(component.isCreating).toBeFalse();
  });

  it('opens a clean form for a new scheduled maintenance', () => {
    initializeComponent();
    component.startEdit(maintenance);

    component.startCreate();

    expect(component.isCreating).toBeTrue();
    expect(component.editing).toBeNull();
    expect(component.form.busId).toBe('');
    expect(component.form.status).toBe('Agendada');
    expect(component.statuses).toEqual(['Agendada']);
    expect(component.errorMessage).toBe('');
  });

  it('displays the create form and disables creation when there are no buses', () => {
    busService.getBuses.and.returnValue(of([]));
    initializeComponent();

    const element: HTMLElement = fixture.nativeElement;
    const button = element.querySelector(
      'button.primary-button'
    ) as HTMLButtonElement;

    expect(element.textContent).toContain(
      'Cadastre um ônibus antes de registrar uma manutenção.'
    );
    expect(button.disabled).toBeTrue();
  });

  it('does not submit when required fields are missing', () => {
    initializeComponent();
    component.startCreate();
    component.form = validPayload();
    component.form.description = '   ';

    component.save();

    expect(component.errorMessage).toBe(
      'Preencha o ônibus, o tipo de serviço, a descrição e a data.'
    );
    expect(maintenanceService.create).not.toHaveBeenCalled();
  });

  it('rejects a service type longer than 80 characters', () => {
    initializeComponent();
    component.startCreate();
    component.form = validPayload();
    component.form.serviceType = 'a'.repeat(81);

    component.save();

    expect(component.errorMessage).toContain('no máximo 80 caracteres');
    expect(maintenanceService.create).not.toHaveBeenCalled();
  });

  it('rejects a description longer than 1000 characters', () => {
    initializeComponent();
    component.startCreate();
    component.form = validPayload();
    component.form.description = 'a'.repeat(1001);

    component.save();

    expect(component.errorMessage).toContain('no máximo 1000 caracteres');
    expect(maintenanceService.create).not.toHaveBeenCalled();
  });

  it('rejects negative costs', () => {
    initializeComponent();
    component.startCreate();
    component.form = validPayload();
    component.form.cost = -1;

    component.save();

    expect(component.errorMessage).toContain('custo válido');
    expect(maintenanceService.create).not.toHaveBeenCalled();
  });

  it('rejects costs with more than two decimal places', () => {
    initializeComponent();
    component.startCreate();
    component.form = validPayload();
    component.form.cost = 10.123;

    component.save();

    expect(component.errorMessage).toContain('custo válido');
    expect(maintenanceService.create).not.toHaveBeenCalled();
  });

  it('accepts a zero cost', () => {
    initializeComponent();
    component.startCreate();
    component.form = validPayload();
    component.form.cost = 0;

    maintenanceService.create.and.returnValue(
      of({
        ...maintenance,
        cost: 0,
        id: 'maintenance-zero'
      })
    );

    component.save();

    expect(maintenanceService.create).toHaveBeenCalled();
    expect(component.errorMessage).toBe('');
  });

  it('rejects an invalid service date', () => {
    initializeComponent();
    component.startCreate();
    component.form = validPayload();
    component.form.serviceDate = 'not-a-date';

    component.save();

    expect(component.errorMessage).toBe('Informe uma data de serviço válida.');
    expect(maintenanceService.create).not.toHaveBeenCalled();
  });

  it('rejects a scheduled maintenance dated in the past', () => {
    initializeComponent();
    component.startCreate();
    component.form = validPayload('Agendada', dateOffset(-1));

    component.save();

    expect(component.errorMessage).toContain(
      'Uma manutenção agendada deve ter data de hoje ou futura.'
    );
    expect(maintenanceService.create).not.toHaveBeenCalled();
  });

  it('rejects an in-progress maintenance dated in the future', () => {
    initializeComponent();
    component.startCreate();
    component.form = validPayload('Em andamento', dateOffset(1));

    component.save();

    expect(component.errorMessage).toContain('não pode ter data futura');
    expect(maintenanceService.create).not.toHaveBeenCalled();
  });

  it('rejects a completed maintenance dated in the future', () => {
    initializeComponent();
    component.startCreate();
    component.form = validPayload('Concluída', dateOffset(1));

    component.save();

    expect(component.errorMessage).toContain('não pode ter data futura');
    expect(maintenanceService.create).not.toHaveBeenCalled();
  });

  it('creates a maintenance and adds the returned record to the history', () => {
    const saved: Maintenance = {
      ...maintenance,
      id: 'maintenance-new',
      serviceDate: `${brazilToday()}T03:00:00.000Z`
    };

    maintenanceService.create.and.returnValue(of(saved));

    initializeComponent();
    component.startCreate();
    component.form = validPayload();

    component.save();

    expect(maintenanceService.create).toHaveBeenCalledWith(
      jasmine.objectContaining({
        busId: 'bus-1',
        serviceType: 'Troca de óleo',
        description: 'Manutenção preventiva',
        cost: 250.5,
        status: 'Agendada'
      })
    );

    expect(component.maintenances[0]).toEqual(saved);
    expect(component.isCreating).toBeFalse();
    expect(component.editing).toBeNull();
    expect(component.isLoading).toBeFalse();
  });

  it('trims the service type, description and bus id before creating', () => {
    initializeComponent();
    component.startCreate();

    component.form = {
      ...validPayload(),
      busId: ' bus-1 ',
      serviceType: ' Troca de óleo ',
      description: ' Manutenção preventiva '
    };

    component.save();

    expect(maintenanceService.create).toHaveBeenCalledWith(
      jasmine.objectContaining({
        busId: 'bus-1',
        serviceType: 'Troca de óleo',
        description: 'Manutenção preventiva'
      })
    );
  });

  it('shows the API error when creating a maintenance fails', () => {
    maintenanceService.create.and.returnValue(
      throwError(() => ({
        error: { message: 'Ônibus não encontrado.' }
      }))
    );

    spyOn(console, 'error');

    initializeComponent();
    component.startCreate();
    component.form = validPayload();

    component.save();

    expect(component.errorMessage).toBe('Ônibus não encontrado.');
    expect(component.isLoading).toBeFalse();
  });

  it('offers only valid next statuses when editing a scheduled maintenance', () => {
    initializeComponent();

    component.startEdit({ ...maintenance, status: 'Agendada' });

    expect(component.statuses).toEqual([
      'Agendada',
      'Em andamento',
      'Cancelada'
    ]);
  });

  it('offers only valid next statuses when editing an in-progress maintenance', () => {
    initializeComponent();

    component.startEdit({ ...maintenance, status: 'Em andamento' });

    expect(component.statuses).toEqual([
      'Em andamento',
      'Concluída',
      'Cancelada'
    ]);
  });

  it('updates an editable maintenance and replaces it in the history', () => {
    const updated: Maintenance = {
      ...maintenance,
      serviceType: 'Troca de filtros'
    };

    maintenanceService.update.and.returnValue(of(updated));

    initializeComponent();
    component.startEdit(maintenance);

    component.form = {
      ...component.form,
      serviceType: 'Troca de filtros'
    };

    component.save();

    expect(maintenanceService.update).toHaveBeenCalledWith(
      maintenance.id,
      jasmine.objectContaining({
        serviceType: 'Troca de filtros'
      })
    );

    expect(component.maintenances[0]).toEqual(updated);
    expect(component.isLoading).toBeFalse();
  });

  it('keeps completed maintenance read-only and prevents updates', () => {
    initializeComponent();
    component.startEdit({ ...maintenance, status: 'Concluída' });

    expect(component.isReadOnly).toBeTrue();

    component.save();

    expect(component.errorMessage).toContain('somente para consulta');
    expect(maintenanceService.update).not.toHaveBeenCalled();
  });

  it('keeps cancelled maintenance read-only and prevents updates', () => {
    initializeComponent();
    component.startEdit({ ...maintenance, status: 'Cancelada' });

    expect(component.isReadOnly).toBeTrue();

    component.save();

    expect(component.errorMessage).toContain('somente para consulta');
    expect(maintenanceService.update).not.toHaveBeenCalled();
  });

  it('renders read-only fields and hides the save button for completed maintenance', () => {
    initializeComponent();
    component.startEdit({ ...maintenance, status: 'Concluída' });
    fixture.detectChanges();

    const element: HTMLElement = fixture.nativeElement;

    const saveButton = Array.from(
      element.querySelectorAll('button')
    ).find(button => button.textContent?.includes('Salvar alterações'));

    expect(element.textContent).toContain('Visualizar manutenção');
    expect(element.textContent).toContain('imutáveis');
    expect(saveButton).toBeUndefined();

    const serviceType = element.querySelector(
      '#maintenance-type'
    ) as HTMLInputElement;

    expect(serviceType.disabled).toBeTrue();
  });

  it('does not delete when the user cancels the reason prompt', () => {
    initializeComponent();
    spyOn(window, 'prompt').and.returnValue(null);

    component.delete(maintenance);

    expect(maintenanceService.delete).not.toHaveBeenCalled();
  });

  it('requires a non-empty deletion reason', () => {
    initializeComponent();
    spyOn(window, 'prompt').and.returnValue('   ');

    component.delete(maintenance);

    expect(component.errorMessage).toContain('Informe uma justificativa');
    expect(maintenanceService.delete).not.toHaveBeenCalled();
  });

  it('rejects a deletion reason longer than 1000 characters', () => {
    initializeComponent();
    spyOn(window, 'prompt').and.returnValue('a'.repeat(1001));

    component.delete(maintenance);

    expect(component.errorMessage).toContain('no máximo 1000 caracteres');
    expect(maintenanceService.delete).not.toHaveBeenCalled();
  });

  it('does not delete when the user declines confirmation', () => {
    initializeComponent();

    spyOn(window, 'prompt').and.returnValue('Registro duplicado');
    spyOn(window, 'confirm').and.returnValue(false);

    component.delete(maintenance);

    expect(maintenanceService.delete).not.toHaveBeenCalled();
  });

  it('deletes a maintenance after a reason and confirmation are provided', () => {
    initializeComponent();

    spyOn(window, 'prompt').and.returnValue('Registro duplicado');
    spyOn(window, 'confirm').and.returnValue(true);

    component.delete(maintenance);

    expect(maintenanceService.delete).toHaveBeenCalledWith(
      maintenance.id,
      'Registro duplicado'
    );

    expect(component.maintenances).not.toContain(maintenance);
    expect(component.isLoading).toBeFalse();
  });

  it('shows an error when deleting a maintenance fails', () => {
    maintenanceService.delete.and.returnValue(
      throwError(() => ({
        error: { message: 'Exclusão não permitida.' }
      }))
    );

    spyOn(console, 'error');
    spyOn(window, 'prompt').and.returnValue('Registro duplicado');
    spyOn(window, 'confirm').and.returnValue(true);

    initializeComponent();
    component.delete(maintenance);

    expect(component.errorMessage).toBe('Exclusão não permitida.');
    expect(component.isLoading).toBeFalse();
  });

  it('navigates back to the dashboard', () => {
    initializeComponent();

    component.backToDashboard();

    expect(router.navigate).toHaveBeenCalledWith(['/dashboard']);
  });

  it('hides administrative actions for a regular user', () => {
    sessionService.isAdmin.and.returnValue(false);
    initializeComponent();
    fixture.detectChanges();

    const element: HTMLElement = fixture.nativeElement;

    const editButton = element.querySelector(
      '.edit-button'
    ) as HTMLButtonElement;

    const deleteButton = element.querySelector(
      '.delete-button'
    ) as HTMLButtonElement;

    expect(editButton).not.toBeNull();
    expect(deleteButton).not.toBeNull();
    expect(editButton.hidden).toBeTrue();
    expect(deleteButton.hidden).toBeTrue();
  });
});
