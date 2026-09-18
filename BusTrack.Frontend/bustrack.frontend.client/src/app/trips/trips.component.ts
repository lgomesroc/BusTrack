import {
  Component,
  OnInit
} from '@angular/core';

import { Router } from '@angular/router';

import {
  Bus,
  Driver,
  Route,
  Trip,
  TripService
} from '../services/trip.service';

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

  tripForm = {
    busId: '',
    driverId: '',
    routeId: '',
    departureTime: '',
    arrivalTime: '',
    duration: 0,
    limitPassengers: 0
  };

  constructor(
    private tripService: TripService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.carregarDados();
  }

  carregarDados(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.tripService.getBuses().subscribe({
      next: buses => {
        this.buses = buses;
      },
      error: () => {
        this.errorMessage =
          'Não foi possível carregar os ônibus.';
      }
    });

    this.tripService.getDrivers().subscribe({
      next: drivers => {
        this.drivers = drivers;
      },
      error: () => {
        this.errorMessage =
          'Não foi possível carregar os motoristas.';
      }
    });

    this.tripService.getRoutes().subscribe({
      next: routes => {
        this.routes = routes;
      },
      error: () => {
        this.errorMessage =
          'Não foi possível carregar as rotas.';
      }
    });

    this.tripService.getTrips().subscribe({
      next: trips => {
        this.trips = trips;
        this.isLoading = false;
      },
      error: () => {
        this.errorMessage =
          'Não foi possível carregar as viagens.';
        this.isLoading = false;
      }
    });
  }

  iniciarCriacao(): void {
    this.isCreating = true;
    this.editingTrip = null;
    this.errorMessage = '';

    this.tripForm = {
      busId: '',
      driverId: '',
      routeId: '',
      departureTime: '',
      arrivalTime: '',
      duration: 0,
      limitPassengers: 0
    };
  }

  iniciarEdicao(trip: Trip): void {
    this.isCreating = false;
    this.editingTrip = trip;
    this.errorMessage = '';

    this.tripForm = {
      busId: trip.busId,
      driverId: trip.driverId,
      routeId: trip.routeId,
      departureTime: this.formatDateForInput(
        trip.departureTime
      ),
      arrivalTime: this.formatDateForInput(
        trip.arrivalTime
      ),
      duration: trip.duration,
      limitPassengers: trip.limitPassengers
    };
  }

  cancelarEdicao(): void {
    this.isCreating = false;
    this.editingTrip = null;
    this.errorMessage = '';

    this.tripForm = {
      busId: '',
      driverId: '',
      routeId: '',
      departureTime: '',
      arrivalTime: '',
      duration: 0,
      limitPassengers: 0
    };
  }

  salvarViagem(): void {
    this.errorMessage = '';

    if (
      !this.tripForm.busId ||
      !this.tripForm.driverId ||
      !this.tripForm.routeId ||
      !this.tripForm.departureTime ||
      !this.tripForm.arrivalTime
    ) {
      this.errorMessage =
        'Preencha todos os dados obrigatórios da viagem.';

      return;
    }

    const tripData: Omit<Trip, 'id'> = {
      busId: this.tripForm.busId,
      driverId: this.tripForm.driverId,
      routeId: this.tripForm.routeId,
      departureTime: new Date(
        this.tripForm.departureTime
      ).toISOString(),
      arrivalTime: new Date(
        this.tripForm.arrivalTime
      ).toISOString(),
      duration: this.tripForm.duration,
      limitPassengers: this.tripForm.limitPassengers,
      passengers: []
    };

    this.isLoading = true;

    if (this.editingTrip) {
      this.tripService
        .updateTrip(
          this.editingTrip.id,
          tripData
        )
        .subscribe({
          next: updatedTrip => {
            const index =
              this.trips.findIndex(
                trip =>
                  trip.id === updatedTrip.id
              );

            if (index !== -1) {
              this.trips[index] = updatedTrip;
            }

            this.isLoading = false;
            this.cancelarEdicao();
          },
          error: () => {
            this.isLoading = false;

            this.errorMessage =
              'Não foi possível atualizar a viagem.';
          }
        });

      return;
    }

    this.tripService
      .createTrip(tripData)
      .subscribe({
        next: createdTrip => {
          this.trips.push(createdTrip);

          this.isLoading = false;

          this.cancelarEdicao();
        },
        error: () => {
          this.isLoading = false;

          this.errorMessage =
            'Não foi possível criar a viagem. ' +
            'Verifique os dados informados e se a API está em execução.';
        }
      });
  }

  excluirViagem(trip: Trip): void {
    this.errorMessage = '';

    this.tripService
      .deleteTrip(trip.id)
      .subscribe({
        next: () => {
          this.trips =
            this.trips.filter(
              item => item.id !== trip.id
            );
        },
        error: () => {
          this.errorMessage =
            'Não foi possível excluir a viagem.';
        }
      });
  }

  voltarAoDashboard(): void {
    this.router.navigate(['/dashboard']);
  }

  obterOnibus(trip: Trip): string {
    const bus =
      this.buses.find(
        item => item.id === trip.busId
      );

    if (!bus) {
      return trip.busId;
    }

    return bus.number ||
      bus.licensePlate ||
      bus.model ||
      trip.busId;
  }

  obterMotorista(trip: Trip): string {
    const driver =
      this.drivers.find(
        item => item.id === trip.driverId
      );

    return driver?.name || trip.driverId;
  }

  obterRota(trip: Trip): string {
    const route =
      this.routes.find(
        item => item.id === trip.routeId
      );

    if (!route) {
      return trip.routeId;
    }

    return route.name ||
      `${route.origin} → ${route.destination}`;
  }

  private formatDateForInput(
    value: string
  ): string {
    const date = new Date(value);

    if (Number.isNaN(date.getTime())) {
      return '';
    }

    const year =
      date.getFullYear();

    const month =
      String(
        date.getMonth() + 1
      ).padStart(2, '0');

    const day =
      String(
        date.getDate()
      ).padStart(2, '0');

    const hours =
      String(
        date.getHours()
      ).padStart(2, '0');

    const minutes =
      String(
        date.getMinutes()
      ).padStart(2, '0');

    return `${year}-${month}-${day}T${hours}:${minutes}`;
  }
}
