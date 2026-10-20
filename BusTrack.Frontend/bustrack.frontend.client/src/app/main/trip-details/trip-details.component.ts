
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';

import { Trip, TripService } from '../../services/trip.service';

@Component({
  selector: 'app-trip-details',
  templateUrl: './trip-details.component.html',
  styleUrls: ['./trip-details.component.css']
})
export class TripDetailsComponent implements OnInit {
  trip: Trip | null = null;
  isLoading = false;
  errorMessage = '';
  notFound = false;

  constructor(
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly tripService: TripService
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');

    if (!id || !id.trim()) {
      this.notFound = true;
      this.errorMessage = 'O identificador da viagem não foi informado.';
      return;
    }

    this.carregarDetalhes(id);
  }

  carregarDetalhes(id: string): void {
    this.isLoading = true;
    this.errorMessage = '';
    this.notFound = false;
    this.trip = null;

    this.tripService.getTripById(id).subscribe({
      next: (trip) => {
        if (!trip || !trip.id) {
          this.notFound = true;
          this.errorMessage = 'A viagem solicitada não foi encontrada.';
          this.isLoading = false;
          return;
        }

        this.trip = {
          ...trip,
          passengers: trip.passengers ?? []
        };

        this.isLoading = false;
      },
      error: (error: HttpErrorResponse) => {
        this.isLoading = false;

        if (error.status === 404) {
          this.notFound = true;
          this.errorMessage = 'A viagem solicitada não foi encontrada.';
          return;
        }

        if (error.status === 0) {
          this.errorMessage =
            'Não foi possível conectar à API. Verifique se o serviço está em execução.';
          return;
        }

        if (error.status === 401 || error.status === 403) {
          this.errorMessage =
            'Você não tem autorização para consultar esta viagem.';
          return;
        }

        this.errorMessage =
          'Não foi possível carregar os detalhes da viagem. Tente novamente.';
      }
    });
  }

  voltarParaViagens(): void {
    this.router.navigate(['/trips']);
  }

  identificarOnibus(): string {
    if (!this.trip?.bus) {
      return 'Não informado';
    }

    const numero = this.trip.bus.number
      ? `Nº ${this.trip.bus.number}`
      : '';

    const placa = this.trip.bus.licensePlate ?? '';
    const modelo = this.trip.bus.model ?? '';

    return [numero, placa, modelo]
      .filter(value => value.trim().length > 0)
      .join(' — ') || 'Não informado';
  }

  identificarMotorista(): string {
    return this.trip?.driver?.name || 'Não informado';
  }

  identificarRota(): string {
    const route = this.trip?.route;

    if (!route) {
      return 'Não informada';
    }

    return `${route.name} — ${route.origin} → ${route.destination}`;
  }

  quantidadePassageiros(): number {
    return this.trip?.passengers?.length ?? 0;
  }
}
