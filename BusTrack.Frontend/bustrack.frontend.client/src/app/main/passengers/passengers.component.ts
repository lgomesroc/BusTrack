import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import {
  Passenger,
  PassengerPayload,
  PassengerService
} from '../../services/passenger.service';
import { SessionService } from '../../services/session.service';

@Component({
  selector: 'app-passengers',
  templateUrl: './passengers.component.html',
  styleUrls: ['./passengers.component.css']
})
export class PassengersComponent implements OnInit {
  passengers: Passenger[] = [];
  passengerForm: PassengerPayload = this.emptyPassenger();
  editingPassenger: Passenger | null = null;
  isCreating = false;
  isLoading = false;
  errorMessage = '';

  constructor(
    private router: Router,
    private passengerService: PassengerService,
    public sessionService: SessionService
  ) {}

  ngOnInit(): void {
    this.loadPassengers();
  }

  loadPassengers(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.passengerService.getPassengers().subscribe({
      next: passengers => {
        this.passengers = passengers ?? [];
        this.isLoading = false;
      },
      error: error => {
        console.error('Erro ao carregar passageiros:', error);
        this.errorMessage =
          'Não foi possível carregar os passageiros. Verifique se a API está disponível.';
        this.isLoading = false;
      }
    });
  }

  startCreate(): void {
    this.isCreating = true;
    this.editingPassenger = null;
    this.passengerForm = this.emptyPassenger();
    this.errorMessage = '';
  }

  startEdit(passenger: Passenger): void {
    this.isCreating = false;
    this.editingPassenger = passenger;
    this.passengerForm = {
      name: passenger.name,
      email: passenger.email,
      cpf: passenger.cpf,
      phone: passenger.phone
    };
    this.errorMessage = '';
  }

  cancelEdit(): void {
    this.isCreating = false;
    this.editingPassenger = null;
    this.passengerForm = this.emptyPassenger();
    this.errorMessage = '';
  }

  savePassenger(): void {
    this.errorMessage = '';

    const payload: PassengerPayload = {
      name: this.passengerForm.name.trim(),
      email: this.passengerForm.email.trim(),
      cpf: this.passengerForm.cpf.trim(),
      phone: this.passengerForm.phone.trim()
    };

    if (!payload.name || !payload.cpf) {
      this.errorMessage = 'Preencha o nome e o CPF do passageiro.';
      return;
    }

    this.isLoading = true;

    const request = this.editingPassenger
      ? this.passengerService.updatePassenger(
          this.editingPassenger.id,
          payload
        )
      : this.passengerService.createPassenger(payload);

    request.subscribe({
      next: savedPassenger => {
        if (this.editingPassenger) {
          this.passengers = this.passengers.map(passenger =>
            passenger.id === this.editingPassenger?.id
              ? savedPassenger
              : passenger
          );
        } else {
          this.passengers = [...this.passengers, savedPassenger];
        }

        this.isLoading = false;
        this.cancelEdit();
      },
      error: error => {
        console.error('Erro ao salvar passageiro:', error);
        this.errorMessage =
          'Não foi possível salvar o passageiro. Confira os dados e tente novamente.';
        this.isLoading = false;
      }
    });
  }

  deletePassenger(passenger: Passenger): void {
    const label = passenger.name?.trim() || passenger.cpf;

    if (!window.confirm(`Deseja realmente excluir o passageiro "${label}"?`)) {
      return;
    }

    this.errorMessage = '';
    this.isLoading = true;

    this.passengerService.deletePassenger(passenger.id).subscribe({
      next: () => {
        this.passengers = this.passengers.filter(
          item => item.id !== passenger.id
        );
        this.isLoading = false;
      },
      error: error => {
        console.error('Erro ao excluir passageiro:', error);
        this.errorMessage = 'Não foi possível excluir o passageiro.';
        this.isLoading = false;
      }
    });
  }

  backToDashboard(): void {
    this.router.navigate(['/dashboard']);
  }

  private emptyPassenger(): PassengerPayload {
    return {
      name: '',
      email: '',
      cpf: '',
      phone: ''
    };
  }
}
