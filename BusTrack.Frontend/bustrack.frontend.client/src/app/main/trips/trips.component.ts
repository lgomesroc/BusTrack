import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Trip, TripService } from '../../services/trip.service';

@Component({
  selector: 'app-trips',
  templateUrl: './trips.component.html',
  styleUrls: ['./trips.component.css']
})
export class TripsComponent implements OnInit {

  trips: Trip[] = [];

  editingTrip: Trip | null = null;

  isCreating = false;

  isLoading = false;

  errorMessage = '';

  tripForm: Trip = this.criarFormularioVazio();

  constructor(
    private router: Router,
    private tripService: TripService
  ) {}

  ngOnInit(): void {
    this.carregarViagens();
  }

  carregarViagens(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.tripService.getTrips().subscribe({
      next: (trips) => {
        this.trips = trips;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Erro ao carregar viagens:', error);

        this.errorMessage =
          'Não foi possível carregar as viagens. Verifique se a API está em execução.';

        this.isLoading = false;
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
    this.isCreating = false;
    this.editingTrip = trip;
    this.errorMessage = '';

    this.tripForm = {
      ...trip,
      passengers: [...(trip.passengers ?? [])]
    };
  }

  cancelarEdicao(): void {
    this.isCreating = false;
    this.editingTrip = null;

    this.tripForm = this.criarFormularioVazio();
  }

  salvarViagem(): void {
    this.errorMessage = '';

    if (!this.validarFormulario()) {
      return;
    }

    this.isLoading = true;

    const trip = {
      busId: this.tripForm.busId,
      driverId: this.tripForm.driverId,
      routeId: this.tripForm.routeId,
      departureTime: this.tripForm.departureTime,
      arrivalTime: this.tripForm.arrivalTime,
      duration: this.tripForm.duration,
      limitPassengers: this.tripForm.limitPassengers,
      passengers: this.tripForm.passengers
    };

    if (this.editingTrip) {
      this.tripService
        .updateTrip(this.editingTrip.id, trip)
        .subscribe({
          next: (updatedTrip) => {
            const index = this.trips.findIndex(
              item => item.id === updatedTrip.id
            );

            if (index !== -1) {
              this.trips[index] = updatedTrip;
            }

            this.isLoading = false;
            this.cancelarEdicao();
          },
          error: (error) => {
            console.error('Erro ao atualizar viagem:', error);

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
        next: (createdTrip) => {
          this.trips.push(createdTrip);

          this.isLoading = false;
          this.cancelarEdicao();
        },
        error: (error) => {
          console.error('Erro ao criar viagem:', error);

          this.errorMessage =
            'Não foi possível criar a viagem. Verifique os dados informados e se a API está em execução.';

          this.isLoading = false;
        }
      });
  }

  excluirViagem(trip: Trip): void {
    this.errorMessage = '';
    this.isLoading = true;

    this.tripService
      .deleteTrip(trip.id)
      .subscribe({
        next: () => {
          this.trips = this.trips.filter(
            item => item.id !== trip.id
          );

          this.isLoading = false;
        },
        error: (error) => {
          console.error('Erro ao excluir viagem:', error);

          this.errorMessage =
            'Não foi possível excluir a viagem.';

          this.isLoading = false;
        }
      });
  }

  voltarAoDashboard(): void {
    this.router.navigate(['/dashboard']);
  }

  private criarFormularioVazio(): Trip {
    return {
      id: '',
      busId: '',
      driverId: '',
      routeId: '',
      departureTime: '',
      arrivalTime: '',
      duration: 0,
      limitPassengers: 0,
      passengers: []
    };
  }

  private validarFormulario(): boolean {
    if (
      !this.tripForm.busId ||
      !this.tripForm.driverId ||
      !this.tripForm.routeId ||
      !this.tripForm.departureTime
    ) {
      this.errorMessage =
        'Preencha os dados obrigatórios da viagem.';

      return false;
    }

    return true;
  }
}
