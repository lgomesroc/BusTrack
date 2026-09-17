import { Component } from '@angular/core';

interface Trip {
  id: string;
  busId: string;
  driverId: string;
  routeId: string;
  departureTime: string;
  passengerCount: number;
}

@Component({
  selector: 'app-trips',
  templateUrl: './trips.component.html',
  styleUrls: ['./trips.component.css']
})
export class TripsComponent {

  trips: Trip[] = [];

  editingTrip: Trip | null = null;

  isCreating = false;

  tripForm: Trip = {
    id: '',
    busId: '',
    driverId: '',
    routeId: '',
    departureTime: '',
    passengerCount: 0
  };

  iniciarCriacao(): void {
    this.isCreating = true;
    this.editingTrip = null;

    this.tripForm = {
      id: '',
      busId: '',
      driverId: '',
      routeId: '',
      departureTime: '',
      passengerCount: 0
    };
  }

  iniciarEdicao(trip: Trip): void {
    this.isCreating = false;
    this.editingTrip = trip;

    this.tripForm = {
      ...trip
    };
  }

  cancelarEdicao(): void {
    this.isCreating = false;
    this.editingTrip = null;

    this.tripForm = {
      id: '',
      busId: '',
      driverId: '',
      routeId: '',
      departureTime: '',
      passengerCount: 0
    };
  }

  salvarViagem(): void {
    if (this.editingTrip) {
      const index = this.trips.findIndex(
        trip => trip.id === this.editingTrip?.id
      );

      if (index !== -1) {
        this.trips[index] = {
          ...this.tripForm
        };
      }
    } else {
      const newTrip: Trip = {
        ...this.tripForm,
        id: crypto.randomUUID()
      };

      this.trips.push(newTrip);
    }

    this.cancelarEdicao();
  }

  excluirViagem(trip: Trip): void {
    this.trips = this.trips.filter(
      item => item.id !== trip.id
    );
  }

  voltarAoDashboard(): void {
    window.location.href = '/dashboard';
  }
}
