import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { forkJoin } from 'rxjs';
import { SessionService } from '../../services/session.service';
import { Trip, TripPayload, TripStatus, Bus, Driver, Route, TripService } from '../../services/trip.service';
import { RouteService } from '../../services/route.service';
import { DriverService } from '../../services/driver.service';

@Component({
  selector: 'app-trips',
  templateUrl: './trips.component.html',
  styleUrls: ['./trips.component.css']
})
export class TripsComponent implements OnInit {
  trips: Trip[] = [];
  buses: Bus[] = [];
  drivers: Driver[] = [];
  routes: Route[] = [];
  editingTrip: Trip | null = null;
  isCreating = false;
  isLoading = false;
  errorMessage = '';
  readonly statuses: TripStatus[] = ['Agendada', 'Em andamento', 'Concluída', 'Cancelada'];
  tripForm = this.criarFormularioVazio();

  constructor(
    private router: Router,
    private tripService: TripService,
    private routeService: RouteService,
    private driverService: DriverService,
    public sessionService: SessionService
  ) {}

  ngOnInit(): void { this.carregarDados(); }

  carregarDados(): void {
    this.isLoading = true;
    this.errorMessage = '';
    forkJoin({ trips: this.tripService.getTrips(), buses: this.tripService.getBuses(), drivers: this.driverService.getDrivers(), routes: this.routeService.getRoutes() }).subscribe({
      next: result => {
        this.trips = result.trips;
        this.buses = result.buses;
        this.drivers = result.drivers;
        this.routes = result.routes;
        this.isLoading = false;
      },
      error: error => {
        console.error('Erro ao carregar dados das viagens:', error);
        this.errorMessage = 'Não foi possível carregar os dados das viagens. Verifique se a API está em execução.';
        this.isLoading = false;
      }
    });
  }

  carregarViagens(): void {
    this.tripService.getTrips().subscribe({
      next: trips => this.trips = trips,
      error: error => {
        console.error('Erro ao carregar viagens:', error);
        this.errorMessage = 'Não foi possível carregar as viagens.';
      }
    });
  }

  iniciarCriacao(): void {
    this.isCreating = true;
    this.editingTrip = null;
    this.errorMessage = '';
    this.tripForm = this.criarFormularioVazio();
  }

  iniciarEdicao(trip: Trip): void {
    if (trip.status === 'Concluída' || trip.status === 'Cancelada') {
      this.errorMessage = 'Viagens concluídas ou canceladas não podem ser alteradas.';
      return;
    }
    this.isCreating = false;
    this.editingTrip = trip;
    this.errorMessage = '';
    this.tripForm = {
      busId: trip.bus?.id ?? '',
      driverId: trip.driver?.id ?? '',
      routeId: trip.route?.id ?? '',
      departureTime: this.converterParaDatetimeLocal(trip.departureTime),
      plannedDuration: trip.plannedDuration,
      arrivalTime: this.converterParaDatetimeLocal(trip.arrivalTime ?? ''),
      duration: trip.duration,
      limitPassengers: trip.limitPassengers,
      status: trip.status || 'Agendada',
      passengers: (trip.passengers ?? []).map(passenger => passenger.id)
    };
  }

  cancelarEdicao(): void {
    this.isCreating = false;
    this.editingTrip = null;
    this.errorMessage = '';
    this.tripForm = this.criarFormularioVazio();
  }

  aoAlterarStatus(): void {
    if (this.tripForm.status === 'Cancelada') {
      this.tripForm.arrivalTime = '';
      this.tripForm.duration = 0;
      this.tripForm.limitPassengers = 0;
      this.tripForm.passengers = [];
    } else if (this.tripForm.status === 'Agendada' || this.tripForm.status === 'Em andamento') {
      this.tripForm.arrivalTime = '';
      this.tripForm.duration = 0;
      this.tripForm.limitPassengers = 0;
    }
  }

  salvarViagem(): void {
    this.errorMessage = '';
    if (!this.validarFormulario()) return;
    this.isLoading = true;

    const trip: TripPayload = {
      busId: this.tripForm.busId,
      driverId: this.tripForm.driverId,
      routeId: this.tripForm.routeId,
      departureTime: this.converterParaIso(this.tripForm.departureTime),
      startedAt: this.editingTrip?.startedAt ?? null,
      arrivalTime: this.tripForm.status === 'Concluída' ? this.converterParaIso(this.tripForm.arrivalTime) : null,
      duration: this.tripForm.status === 'Concluída' ? Number(this.tripForm.duration) : 0,
      plannedDuration: Number(this.tripForm.plannedDuration),
      limitPassengers: this.tripForm.status === 'Concluída' ? Number(this.tripForm.limitPassengers) : 0,
      status: this.tripForm.status,
      passengers: this.tripForm.status === 'Concluída' ? this.tripForm.passengers : []
    };

    const request = this.editingTrip
      ? this.tripService.updateTrip(this.editingTrip.id, trip)
      : this.tripService.createTrip(trip);

    request.subscribe({
      next: () => {
        this.isLoading = false;
        this.cancelarEdicao();
        this.carregarDados();
      },
      error: error => {
        console.error('Erro ao salvar viagem:', error);
        this.errorMessage = error?.error?.message || 'Não foi possível salvar a viagem. Verifique os dados informados.';
        this.isLoading = false;
      }
    });
  }

  excluirViagem(trip: Trip): void {
    if (trip.status === 'Concluída' || trip.status === 'Cancelada') {
      this.errorMessage = 'Viagens concluídas ou canceladas não podem ser excluídas por esta tela.';
      return;
    }
    if (!window.confirm('Deseja realmente excluir esta viagem?')) return;
    this.isLoading = true;
    this.tripService.deleteTrip(trip.id).subscribe({
      next: () => {
        this.trips = this.trips.filter(item => item.id !== trip.id);
        this.isLoading = false;
      },
      error: error => {
        console.error('Erro ao excluir viagem:', error);
        this.errorMessage = 'Não foi possível excluir a viagem.';
        this.isLoading = false;
      }
    });
  }

  voltarAoDashboard(): void { this.router.navigate(['/dashboard']); }

  identificarOnibus(trip: Trip): string {
    if (!trip.bus) return 'Não informado';
    return [trip.bus.number ? `Nº ${trip.bus.number}` : '', trip.bus.licensePlate, trip.bus.model].filter(Boolean).join(' - ') || 'Não informado';
  }

  identificarMotorista(trip: Trip): string { return trip.driver?.name || 'Não informado'; }

  identificarRota(trip: Trip): string {
    if (!trip.route) return 'Não informada';
    return `${trip.route.name} - ${trip.route.origin} → ${trip.route.destination}`;
  }

  podeAlterarStatus(trip: Trip): boolean { return trip.status === 'Agendada' || trip.status === 'Em andamento'; }

  private criarFormularioVazio() {
    return { busId: '', driverId: '', routeId: '', departureTime: '', plannedDuration: 60, arrivalTime: '', duration: 0, limitPassengers: 0, status: 'Agendada' as TripStatus, passengers: [] as string[] };
  }

  private validarFormulario(): boolean {
    if (!this.tripForm.busId || !this.tripForm.driverId || !this.tripForm.routeId || !this.tripForm.departureTime) {
      this.errorMessage = 'Informe o ônibus, o motorista, a rota e a data e hora de saída.';
      return false;
    }

    if (!Number.isInteger(Number(this.tripForm.plannedDuration)) || Number(this.tripForm.plannedDuration) <= 0) {
      this.errorMessage = 'Informe a duração prevista da viagem em minutos.';
      return false;
    }

    if (this.isCreating && new Date(this.tripForm.departureTime).getTime() <= Date.now()) {
      this.errorMessage = 'A saída deve ser agendada para um horário futuro.';
      return false;
    }

    if (!this.isCreating && this.editingTrip) {
      const current = this.editingTrip.status || 'Agendada';
      const next = this.tripForm.status;
      const allowed = current === 'Agendada'
        ? ['Em andamento', 'Concluída', 'Cancelada']
        : current === 'Em andamento'
          ? ['Concluída', 'Cancelada']
          : [];
      if (!allowed.includes(next)) {
        this.errorMessage = `Não é permitida a transição de ${current} para ${next}.`;
        return false;
      }
      if (next === 'Em andamento' && Date.now() < new Date(this.editingTrip.departureTime).getTime()) {
        this.errorMessage = 'A viagem só pode entrar em andamento no horário previsto de saída ou depois dele.';
        return false;
      }
    }

    if (this.tripForm.status === 'Concluída') {
      if (!this.tripForm.arrivalTime || !Number.isInteger(Number(this.tripForm.duration)) || Number(this.tripForm.duration) <= 0 || !Number.isInteger(Number(this.tripForm.limitPassengers)) || Number(this.tripForm.limitPassengers) <= 0) {
        this.errorMessage = 'Para concluir, informe a chegada real, a duração em minutos e a quantidade de passageiros transportados.';
        return false;
      }
      const startedAt = this.editingTrip?.startedAt
        ? new Date(this.editingTrip.startedAt)
        : new Date(this.tripForm.departureTime);
      const arrivalAt = new Date(this.tripForm.arrivalTime);
      if (arrivalAt <= startedAt || arrivalAt > new Date()) {
        this.errorMessage = 'A chegada real deve ser posterior ao início da viagem e não pode estar no futuro.';
        return false;
      }
      const minutes = Math.floor((arrivalAt.getTime() - startedAt.getTime()) / 60000);
      if (Number(this.tripForm.duration) !== minutes) {
        this.errorMessage = `A duração deve corresponder ao tempo real transcorrido: ${minutes} minuto(s).`;
        return false;
      }
    }

    return true;
  }

  private converterParaIso(value: string): string {
    return value ? new Date(value).toISOString() : '';
  }

  private converterParaDatetimeLocal(value: string): string {
    if (!value) return '';
    const date = new Date(value);
    const localDate = new Date(date.getTime() - date.getTimezoneOffset() * 60000);
    return localDate.toISOString().slice(0, 16);
  }
}