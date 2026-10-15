import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Route } from '../../services/trip.service';
import { RouteService } from '../../services/route.service';

@Component({
  selector: 'app-routes',
  templateUrl: './routes.component.html',
  styleUrls: ['./routes.component.css']
})
export class RoutesComponent implements OnInit {
  routes: Route[] = [];
  routeForm: Route = this.emptyRoute();
  editingRoute: Route | null = null;
  isCreating = false;
  isLoading = false;
  errorMessage = '';

  constructor(
    private router: Router,
    private routeService: RouteService
  ) {}

  ngOnInit(): void {
    this.loadRoutes();
  }

  loadRoutes(): void {
    this.isLoading = true;
    this.errorMessage = '';
    this.routeService.getRoutes().subscribe({
      next: routes => {
        this.routes = routes ?? [];
        this.isLoading = false;
      },
      error: error => {
        console.error('Erro ao carregar rotas:', error);
        this.errorMessage = 'Não foi possível carregar as rotas. Verifique se a API está disponível.';
        this.isLoading = false;
      }
    });
  }

  startCreate(): void {
    this.isCreating = true;
    this.editingRoute = null;
    this.routeForm = this.emptyRoute();
    this.errorMessage = '';
  }

  startEdit(route: Route): void {
    this.isCreating = false;
    this.editingRoute = route;
    this.routeForm = { ...route };
    this.errorMessage = '';
  }

  cancelEdit(): void {
    this.isCreating = false;
    this.editingRoute = null;
    this.routeForm = this.emptyRoute();
  }

  saveRoute(): void {
    this.errorMessage = '';
    if (!this.validateForm()) return;

    const payload = {
      name: this.routeForm.name.trim(),
      description: (this.routeForm.description ?? '').trim(),
      origin: this.routeForm.origin.trim(),
      destination: this.routeForm.destination.trim(),
      distance: Number(this.routeForm.distance)
    };

    this.isLoading = true;
    const request = this.editingRoute
      ? this.routeService.updateRoute(this.editingRoute.id, payload)
      : this.routeService.createRoute(payload);

    request.subscribe({
      next: savedRoute => {
        if (this.editingRoute) {
          this.routes = this.routes.map(route =>
            route.id === this.editingRoute?.id ? savedRoute : route
          );
        } else {
          this.routes = [...this.routes, savedRoute];
        }
        this.isLoading = false;
        this.cancelEdit();
      },
      error: error => {
        console.error('Erro ao salvar rota:', error);
        this.errorMessage = 'Não foi possível salvar a rota. Confira os dados e tente novamente.';
        this.isLoading = false;
      }
    });
  }

  deleteRoute(route: Route): void {
    const label = route.name?.trim() || route.origin + ' → ' + route.destination;
    if (!window.confirm(`Deseja realmente excluir a rota "${label}"?`)) return;

    this.errorMessage = '';
    this.isLoading = true;
    this.routeService.deleteRoute(route.id).subscribe({
      next: () => {
        this.routes = this.routes.filter(item => item.id !== route.id);
        this.isLoading = false;
      },
      error: error => {
        console.error('Erro ao excluir rota:', error);
        this.errorMessage = 'Não foi possível excluir a rota.';
        this.isLoading = false;
      }
    });
  }

  backToDashboard(): void {
    this.router.navigate(['/dashboard']);
  }

  private emptyRoute(): Route {
    return {
      id: '',
      name: '',
      description: '',
      origin: '',
      destination: '',
      distance: 0
    };
  }

  private validateForm(): boolean {
    if (
      !this.routeForm.name.trim() ||
      !this.routeForm.origin.trim() ||
      !this.routeForm.destination.trim()
    ) {
      this.errorMessage = 'Preencha o nome, a origem e o destino da rota.';
      return false;
    }

    const rawDistance = this.routeForm.distance as number | string | null | undefined;
    if (rawDistance === null || rawDistance === undefined || String(rawDistance).trim() === '') {
      this.errorMessage = 'Informe a distância da rota.';
      return false;
    }

    const distance = Number(rawDistance);
    if (!Number.isInteger(distance) || distance < 0) {
      this.errorMessage = 'A distância deve ser um número inteiro igual ou maior que zero.';
      return false;
    }

    return true;
  }
}
