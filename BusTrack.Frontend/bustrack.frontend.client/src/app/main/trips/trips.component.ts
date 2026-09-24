import {
  Component,
  OnInit
} from '@angular/core';

import { Router } from '@angular/router';

import {
  forkJoin
} from 'rxjs';

import {
  Trip,
  TripPayload,
  Bus,
  Driver,
  Route,
  TripService
} from '../../services/trip.service';

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

  tripForm = this.criarFormularioVazio();

  constructor(
    private router: Router,

    private tripService: TripService
  ) {
  }

  ngOnInit(): void {

    this.carregarDados();
  }

  carregarDados(): void {

    this.isLoading = true;

    this.errorMessage = '';

    forkJoin({
      trips:
        this.tripService.getTrips(),

      buses:
        this.tripService.getBuses(),

      drivers:
        this.tripService.getDrivers(),

      routes:
        this.tripService.getRoutes()
    }).subscribe({

      next: (result) => {

        this.trips =
          result.trips;

        this.buses =
          result.buses;

        this.drivers =
          result.drivers;

        this.routes =
          result.routes;

        this.isLoading = false;
      },

      error: (error) => {

        console.error(
          'Erro ao carregar dados das viagens:',
          error
        );

        this.errorMessage =
          'Não foi possível carregar os dados das viagens. ' +
          'Verifique se a API está em execução.';

        this.isLoading = false;
      }
    });
  }

  carregarViagens(): void {

    this.tripService
      .getTrips()
      .subscribe({

        next: (trips) => {

          this.trips =
            trips;
        },

        error: (error) => {

          console.error(
            'Erro ao carregar viagens:',
            error
          );

          this.errorMessage =
            'Não foi possível carregar as viagens. ' +
            'Verifique se a API está em execução.';
        }
      });
  }

  iniciarCriacao(): void {

    this.isCreating = true;

    this.editingTrip = null;

    this.errorMessage = '';

    this.tripForm =
      this.criarFormularioVazio();
  }

  iniciarEdicao(
    trip: Trip
  ): void {

    this.isCreating = false;

    this.editingTrip = trip;

    this.errorMessage = '';

    this.tripForm = {

      busId:
        trip.bus?.id ?? '',

      driverId:
        trip.driver?.id ?? '',

      routeId:
        trip.route?.id ?? '',

      departureTime:
        this.converterParaDatetimeLocal(
          trip.departureTime
        ),

      arrivalTime:
        this.converterParaDatetimeLocal(
          trip.arrivalTime
        ),

      duration:
        trip.duration,

      limitPassengers:
        trip.limitPassengers,

      passengers:
        trip.passengers
          .map(
            passenger =>
              passenger.id
          )
    };
  }

  cancelarEdicao(): void {

    this.isCreating = false;

    this.editingTrip = null;

    this.tripForm =
      this.criarFormularioVazio();
  }

  salvarViagem(): void {

    this.errorMessage = '';

    if (!this.validarFormulario()) {
      return;
    }

    this.isLoading = true;

    const trip: TripPayload = {

      busId:
        this.tripForm.busId,

      driverId:
        this.tripForm.driverId,

      routeId:
        this.tripForm.routeId,

      departureTime:
        this.converterParaIso(
          this.tripForm.departureTime
        ),

      arrivalTime:
        this.converterParaIso(
          this.tripForm.arrivalTime
        ),

      duration:
        this.tripForm.duration,

      limitPassengers:
        this.tripForm.limitPassengers,

      passengers:
        this.tripForm.passengers
    };

    if (this.editingTrip) {

      this.tripService
        .updateTrip(
          this.editingTrip.id,
          trip
        )
        .subscribe({

          next: () => {

            this.isLoading = false;

            this.cancelarEdicao();

            this.carregarViagens();
          },

          error: (error) => {

            console.error(
              'Erro ao atualizar viagem:',
              error
            );

            this.errorMessage =
              'Não foi possível atualizar a viagem.';

            this.isLoading = false;
          }
        });

      return;
    }

    this.tripService
      .createTrip(trip)
      .subscribe({

        next: () => {

          this.isLoading = false;

          this.cancelarEdicao();

          this.carregarViagens();
        },

        error: (error) => {

          console.error(
            'Erro ao criar viagem:',
            error
          );

          this.errorMessage =
            'Não foi possível criar a viagem. ' +
            'Verifique os dados informados e se a API está em execução.';

          this.isLoading = false;
        }
      });
  }

  excluirViagem(
    trip: Trip
  ): void {

    this.errorMessage = '';

    const confirmar =
      window.confirm(
        'Deseja realmente excluir esta viagem?'
      );

    if (!confirmar) {
      return;
    }

    this.isLoading = true;

    this.tripService
      .deleteTrip(
        trip.id
      )
      .subscribe({

        next: () => {

          this.trips =
            this.trips.filter(
              item =>
                item.id !== trip.id
            );

          this.isLoading = false;
        },

        error: (error) => {

          console.error(
            'Erro ao excluir viagem:',
            error
          );

          this.errorMessage =
            'Não foi possível excluir a viagem.';

          this.isLoading = false;
        }
      });
  }

  voltarAoDashboard(): void {

    this.router.navigate(
      ['/dashboard']
    );
  }

  identificarOnibus(
    trip: Trip
  ): string {

    if (!trip.bus) {
      return 'Não informado';
    }

    if (trip.bus.number) {

      return (
        `Nº ${trip.bus.number} - ` +
        `${trip.bus.licensePlate}`
      );
    }

    return trip.bus.licensePlate;
  }

  identificarMotorista(
    trip: Trip
  ): string {

    if (!trip.driver) {
      return 'Não informado';
    }

    return trip.driver.name;
  }

  identificarRota(
    trip: Trip
  ): string {

    if (!trip.route) {
      return 'Não informada';
    }

    return (
      `${trip.route.name} - ` +
      `${trip.route.origin} → ` +
      `${trip.route.destination}`
    );
  }

  private criarFormularioVazio() {

    return {

      busId: '',

      driverId: '',

      routeId: '',

      departureTime: '',

      arrivalTime: '',

      duration: 0,

      limitPassengers: 0,

      passengers: [] as string[]
    };
  }

  private validarFormulario(): boolean {

    if (
      !this.tripForm.busId ||
      !this.tripForm.driverId ||
      !this.tripForm.routeId ||
      !this.tripForm.departureTime ||
      !this.tripForm.arrivalTime
    ) {

      this.errorMessage =
        'Preencha o ônibus, o motorista, a rota, ' +
        'a data e hora de saída e a data e hora de chegada.';

      return false;
    }

    if (
      this.tripForm.duration < 0
    ) {

      this.errorMessage =
        'A duração não pode ser negativa.';

      return false;
    }

    if (
      this.tripForm.limitPassengers < 0
    ) {

      this.errorMessage =
        'O limite de passageiros não pode ser negativo.';

      return false;
    }

    return true;
  }

  private converterParaIso(
    value: string
  ): string {

    if (!value) {
      return '';
    }

    return new Date(
      value
    ).toISOString();
  }

  private converterParaDatetimeLocal(
    value: string
  ): string {

    if (!value) {
      return '';
    }

    const date =
      new Date(value);

    const offset =
      date.getTimezoneOffset();

    const localDate =
      new Date(
        date.getTime() -
        offset * 60 * 1000
      );

    return localDate
      .toISOString()
      .slice(0, 16);
  }
}
