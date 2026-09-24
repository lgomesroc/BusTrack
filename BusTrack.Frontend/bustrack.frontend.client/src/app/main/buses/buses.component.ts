import {
  Component,
  OnInit
} from '@angular/core';

import { Router } from '@angular/router';

import {
  Bus,
  BusService
} from '../../services/bus.service';

@Component({
  selector: 'app-buses',
  templateUrl: './buses.component.html',
  styleUrls: ['./buses.component.css']
})
export class BusesComponent implements OnInit {

  buses: Bus[] = [];

  editingBus: Bus | null = null;

  isCreating = false;

  isLoading = false;

  errorMessage = '';

  busForm: Bus =
    this.criarFormularioVazio();

  constructor(
    private router: Router,
    private busService: BusService
  ) {
  }

  ngOnInit(): void {

    this.carregarOnibus();
  }

  carregarOnibus(): void {

    this.isLoading = true;

    this.errorMessage = '';

    this.busService
      .getBuses()
      .subscribe({

        next: (buses) => {

          this.buses = buses;

          this.isLoading = false;
        },

        error: (error) => {

          console.error(
            'Erro ao carregar ônibus:',
            error
          );

          this.errorMessage =
            'Não foi possível carregar os ônibus. ' +
            'Verifique se a API está em execução.';

          this.isLoading = false;
        }
      });
  }

  iniciarCriacao(): void {

    this.isCreating = true;

    this.editingBus = null;

    this.errorMessage = '';

    this.busForm =
      this.criarFormularioVazio();
  }

  iniciarEdicao(bus: Bus): void {

    this.isCreating = false;

    this.editingBus = bus;

    this.errorMessage = '';

    this.busForm = {
      ...bus
    };
  }

  cancelarEdicao(): void {

    this.isCreating = false;

    this.editingBus = null;

    this.busForm =
      this.criarFormularioVazio();
  }

  salvarOnibus(): void {

    this.errorMessage = '';

    if (!this.validarFormulario()) {
      return;
    }

    this.isLoading = true;

    const bus: Omit<Bus, 'id'> = {

      number:
        this.busForm.number,

      licensePlate:
        this.busForm.licensePlate,

      model:
        this.busForm.model,

      capacity:
        this.busForm.capacity
    };

    if (this.editingBus) {

      this.busService
        .updateBus(
          this.editingBus.id,
          bus
        )
        .subscribe({

          next: (updatedBus) => {

            const index =
              this.buses.findIndex(
                item =>
                  item.id ===
                  updatedBus.id
              );

            if (index !== -1) {

              this.buses[index] =
                updatedBus;
            }

            this.isLoading = false;

            this.cancelarEdicao();
          },

          error: (error) => {

            console.error(
              'Erro ao atualizar ônibus:',
              error
            );

            this.errorMessage =
              'Não foi possível atualizar o ônibus.';

            this.isLoading = false;
          }
        });

      return;
    }

    this.busService
      .createBus(bus)
      .subscribe({

        next: (createdBus) => {

          this.buses.push(
            createdBus
          );

          this.isLoading = false;

          this.cancelarEdicao();
        },

        error: (error) => {

          console.error(
            'Erro ao criar ônibus:',
            error
          );

          this.errorMessage =
            'Não foi possível criar o ônibus. ' +
            'Verifique os dados informados ' +
            'e se a API está em execução.';

          this.isLoading = false;
        }
      });
  }

  excluirOnibus(bus: Bus): void {

    const confirmar =
      window.confirm(
        `Deseja realmente excluir o ônibus ${this.identificarOnibus(bus)}?`
      );

    if (!confirmar) {
      return;
    }

    this.errorMessage = '';

    this.isLoading = true;

    this.busService
      .deleteBus(bus.id)
      .subscribe({

        next: () => {

          this.buses =
            this.buses.filter(
              item =>
                item.id !== bus.id
            );

          this.isLoading = false;
        },

        error: (error) => {

          console.error(
            'Erro ao excluir ônibus:',
            error
          );

          this.errorMessage =
            'Não foi possível excluir o ônibus.';

          this.isLoading = false;
        }
      });
  }

  voltarAoDashboard(): void {

    this.router.navigate(
      ['/dashboard']
    );
  }

  private criarFormularioVazio(): Bus {

    return {

      id: '',

      number: '',

      licensePlate: '',

      model: '',

      capacity: 0
    };
  }

  private validarFormulario(): boolean {

    if (
      !this.busForm.number.trim() ||
      !this.busForm.licensePlate.trim() ||
      !this.busForm.model.trim()
    ) {

      this.errorMessage =
        'Preencha o número, a placa e o modelo do ônibus.';

      return false;
    }

    if (
      this.busForm.capacity <= 0
    ) {

      this.errorMessage =
        'A capacidade do ônibus deve ser maior que zero.';

      return false;
    }

    return true;
  }

  private identificarOnibus(
    bus: Bus
  ): string {

    if (
      bus.number.trim()
    ) {

      return `nº ${bus.number}`;
    }

    return bus.licensePlate;
  }
}
