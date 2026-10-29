
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { SessionService } from '../../services/session.service';
import { Bus, BusService } from '../../services/bus.service';
import {
  Maintenance,
  MaintenancePayload,
  MaintenanceService,
  MaintenanceStatus
} from '../../services/maintenance.service';

@Component({
  selector: 'app-maintenance',
  templateUrl: './maintenance.component.html',
  styleUrls: ['./maintenance.component.css']
})
export class MaintenanceComponent implements OnInit {
  maintenances: Maintenance[] = [];
  buses: Bus[] = [];
  form: MaintenancePayload = this.emptyForm();
  editing: Maintenance | null = null;
  isCreating = false;
  isLoading = false;
  errorMessage = '';

  statuses: MaintenanceStatus[] = ['Agendada'];

  constructor(
    private router: Router,
    private maintenanceService: MaintenanceService,
    private busService: BusService,
    public sessionService: SessionService
  ) {}

  ngOnInit(): void {
    this.loadBuses();
    this.loadMaintenances();
  }

  loadBuses(): void {
    this.busService.getBuses().subscribe({
      next: buses => {
        this.buses = buses ?? [];
      },
      error: () => {
        this.errorMessage = 'Não foi possível carregar os ônibus.';
      }
    });
  }

  loadMaintenances(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.maintenanceService.getAll().subscribe({
      next: items => {
        this.maintenances = items ?? [];
        this.isLoading = false;
      },
      error: error => {
        console.error('Erro ao carregar manutenções:', error);
        this.errorMessage = 'Não foi possível carregar as manutenções.';
        this.isLoading = false;
      }
    });
  }

  startCreate(): void {
    if (this.isLoading) {
      return;
    }

    this.isCreating = true;
    this.editing = null;
    this.form = this.emptyForm();
    this.statuses = ['Agendada'];
    this.errorMessage = '';
  }

  startEdit(item: Maintenance): void {
    this.isCreating = false;
    this.editing = item;
    this.statuses = this.getAllowedStatuses(item.status);

    this.form = {
      busId: item.busId,
      serviceType: item.serviceType,
      description: item.description,
      serviceDate: this.getDateInputValue(item.serviceDate),
      serviceStartTime: this.getTimeInputValue(item.serviceStartTime),
      plannedDurationMinutes: item.plannedDurationMinutes,
      completedDate: item.completedDate ?? null,
      cost: item.cost,
      status: item.status
    };

    this.errorMessage = '';
  }

  canEdit(item: Maintenance): boolean {
    return (
      item.status === 'Agendada' ||
      item.status === 'Em andamento'
    );
  }

  get isReadOnly(): boolean {
    return this.editing !== null && !this.canEdit(this.editing);
  }

  private getAllowedStatuses(
    currentStatus: MaintenanceStatus
  ): MaintenanceStatus[] {
    switch (currentStatus) {
      case 'Agendada':
        return ['Agendada', 'Em andamento', 'Cancelada'];

      case 'Em andamento':
        return ['Em andamento', 'Concluída', 'Cancelada'];

      default:
        return [currentStatus];
    }
  }

  private getDateInputValue(value: string | null | undefined): string {
    if (!value) {
      return '';
    }

    // Mantém o dia do calendário brasileiro sem converter o valor
    // para UTC antes de preencher o input type="date".
    const datePart = value.match(/^(\d{4}-\d{2}-\d{2})/);

    if (datePart) {
      return datePart[1];
    }

    const date = new Date(value);

    if (Number.isNaN(date.getTime())) {
      return '';
    }

    const parts = new Intl.DateTimeFormat('en-CA', {
      timeZone: 'America/Sao_Paulo',
      year: 'numeric',
      month: '2-digit',
      day: '2-digit'
    }).formatToParts(date);

    const year = parts.find(part => part.type === 'year')?.value;
    const month = parts.find(part => part.type === 'month')?.value;
    const day = parts.find(part => part.type === 'day')?.value;

    return year && month && day
      ? `${year}-${month}-${day}`
      : '';
  }

  cancelEdit(): void {
    this.isCreating = false;
    this.editing = null;
    this.form = this.emptyForm();
    this.statuses = ['Agendada'];
    this.errorMessage = '';
  }

  save(): void {
    this.errorMessage = '';

    if (this.isReadOnly) {
      this.errorMessage =
        'Manutenções concluídas ou canceladas são somente para consulta.';
      return;
    }

    if (
      !this.form.busId?.trim() ||
      !this.form.serviceType?.trim() ||
      !this.form.description?.trim() ||
      !this.form.serviceDate ||
      !this.form.serviceStartTime ||
      !this.form.plannedDurationMinutes ||
      this.form.plannedDurationMinutes < 1 ||
      this.form.plannedDurationMinutes > 1440
    ) {
      this.errorMessage =
        'Preencha o ônibus, o tipo de serviço, a descrição, a data, o horário e uma duração entre 1 e 1440 minutos.';
      return;
    }

    const serviceType = this.form.serviceType.trim();
    const description = this.form.description.trim();

    if (serviceType.length > 80) {
      this.errorMessage =
        'O tipo de serviço deve ter no máximo 80 caracteres.';
      return;
    }

    if (description.length > 1000) {
      this.errorMessage =
        'A descrição deve ter no máximo 1000 caracteres.';
      return;
    }

    const costText = String(this.form.cost)
      .trim()
      .replace(',', '.');

    const cost = Number(costText);

    if (
      costText === '' ||
      !Number.isFinite(cost) ||
      cost < 0 ||
      cost > 999999999 ||
      !/^\d+(\.\d{1,2})?$/.test(costText)
    ) {
      this.errorMessage =
        'Informe um custo válido, entre zero e 999999999, com no máximo duas casas decimais.';
      return;
    }

    const serviceDateInput = this.form.serviceDate;
    const serviceDate = new Date(
      `${serviceDateInput}T00:00:00`
    );

    if (Number.isNaN(serviceDate.getTime())) {
      this.errorMessage = 'Informe uma data de serviço válida.';
      return;
    }

    const today = this.getBrazilTodayString();
    const selectedStatus = this.form.status;

    if (
      selectedStatus === 'Agendada' &&
      serviceDateInput < today
    ) {
      this.errorMessage =
        'Uma manutenção agendada deve ter data de hoje ou futura.';
      return;
    }

    if (
      (selectedStatus === 'Em andamento' ||
        selectedStatus === 'Concluída') &&
      serviceDateInput > today
    ) {
      this.errorMessage =
        'Uma manutenção em andamento ou concluída não pode ter data futura.';
      return;
    }

    if (this.editing && !this.canEdit(this.editing)) {
      this.errorMessage =
        'Manutenções concluídas ou canceladas são somente para consulta.';
      return;
    }

    const payload: MaintenancePayload = {
      ...this.form,
      busId: this.form.busId.trim(),
      serviceType,
      description,
      cost,

      // IMPORTANTE: não substituir por new Date(serviceDateInput).
      // Esta conversão preserva a correção do dia selecionado.
      serviceDate: serviceDate.toISOString(),
      serviceStartTime: `${serviceDateInput}T${this.form.serviceStartTime}:00`,
      plannedDurationMinutes: Number(this.form.plannedDurationMinutes)
    };

    this.isLoading = true;

    const request = this.editing
      ? this.maintenanceService.update(this.editing.id, payload)
      : this.maintenanceService.create(payload);

    request.subscribe({
      next: saved => {
        if (this.editing) {
          this.maintenances = this.maintenances.map(item =>
            item.id === saved.id ? saved : item
          );
        } else {
          this.maintenances = [saved, ...this.maintenances];
        }

        this.isLoading = false;
        this.cancelEdit();
      },
      error: error => {
        console.error('Erro ao salvar manutenção:', error);
        this.errorMessage =
          error?.error?.message ??
          'Não foi possível salvar a manutenção. Confira os dados e se o ônibus existe.';
        this.isLoading = false;
      }
    });
  }

  delete(item: Maintenance): void {
    if (this.isLoading) {
      return;
    }

    this.errorMessage = '';

    const reasonInput = window.prompt(
      `Informe a justificativa para excluir a manutenção "${item.serviceType}" do ônibus ${item.busNumber}:`
    );

    // Cancelar o prompt não executa a exclusão.
    if (reasonInput === null) {
      return;
    }

    const reason = reasonInput.trim();

    if (!reason) {
      this.errorMessage =
        'Informe uma justificativa para excluir a manutenção.';
      return;
    }

    if (reason.length > 1000) {
      this.errorMessage =
        'A justificativa deve ter no máximo 1000 caracteres.';
      return;
    }

    if (
      !window.confirm(
        `Confirma a exclusão lógica desta manutenção?\n\nJustificativa: ${reason}`
      )
    ) {
      return;
    }

    this.isLoading = true;

    this.maintenanceService.delete(item.id, reason).subscribe({
      next: () => {
        this.maintenances = this.maintenances.filter(
          current => current.id !== item.id
        );
        this.isLoading = false;
      },
      error: error => {
        console.error('Erro ao excluir manutenção:', error);
        this.errorMessage =
          error?.error?.message ??
          'Não foi possível excluir a manutenção.';
        this.isLoading = false;
      }
    });
  }

  backToDashboard(): void {
    this.router.navigate(['/dashboard']);
  }

  private getBrazilTodayString(): string {
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

  private getTimeInputValue(value: string | null | undefined): string {
    if (!value) return '';
    const timePart = value.split('T')[1];
    const match = timePart?.match(/^(\d{2}:\d{2})/);
    return match?.[1] ?? value.slice(0, 5);
  }

  private emptyForm(): MaintenancePayload {
    return {
      busId: '',
      serviceType: '',
      description: '',
      serviceDate: '',
      serviceStartTime: '08:00',
      plannedDurationMinutes: 60,
      completedDate: null,
      cost: 0,
      status: 'Agendada'
    };
  }
}
