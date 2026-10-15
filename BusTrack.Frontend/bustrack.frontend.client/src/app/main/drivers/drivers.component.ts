import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Driver, DriverService } from '../../services/driver.service';

@Component({
  selector: 'app-drivers',
  templateUrl: './drivers.component.html',
  styleUrls: ['./drivers.component.css']
})
export class DriversComponent implements OnInit {
  drivers: Driver[] = [];
  driverForm: Driver = this.emptyDriver();
  editingDriver: Driver | null = null;
  isCreating = false;
  isLoading = false;
  errorMessage = '';

  constructor(
    private router: Router,
    private driverService: DriverService
  ) {}

  ngOnInit(): void {
    this.loadDrivers();
  }

  loadDrivers(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.driverService.getDrivers().subscribe({
      next: drivers => {
        this.drivers = drivers ?? [];
        this.isLoading = false;
      },
      error: error => {
        console.error('Erro ao carregar motoristas:', error);
        this.errorMessage = 'Não foi possível carregar os motoristas. Verifique se a API está disponível.';
        this.isLoading = false;
      }
    });
  }

  startCreate(): void {
    this.isCreating = true;
    this.editingDriver = null;
    this.driverForm = this.emptyDriver();
    this.errorMessage = '';
  }

  startEdit(driver: Driver): void {
    this.isCreating = false;
    this.editingDriver = driver;
    this.driverForm = { ...driver };
    this.errorMessage = '';
  }

  cancelEdit(): void {
    this.isCreating = false;
    this.editingDriver = null;
    this.driverForm = this.emptyDriver();
    this.errorMessage = '';
  }

  saveDriver(): void {
    this.errorMessage = '';

    const name = this.driverForm.name.trim();
    const licenseNumber = this.driverForm.licenseNumber.trim();

    if (!name || !licenseNumber) {
      this.errorMessage = 'Preencha o nome e o CPF/número de identificação do motorista.';
      return;
    }

    const payload = { name, licenseNumber };
    this.isLoading = true;

    const request = this.editingDriver
      ? this.driverService.updateDriver(this.editingDriver.id, payload)
      : this.driverService.createDriver(payload);

    request.subscribe({
      next: savedDriver => {
        if (this.editingDriver) {
          this.drivers = this.drivers.map(driver =>
            driver.id === this.editingDriver?.id ? savedDriver : driver
          );
        } else {
          this.drivers = [...this.drivers, savedDriver];
        }

        this.isLoading = false;
        this.cancelEdit();
      },
      error: error => {
        console.error('Erro ao salvar motorista:', error);
        this.errorMessage = 'Não foi possível salvar o motorista. Confira os dados e tente novamente.';
        this.isLoading = false;
      }
    });
  }

  deleteDriver(driver: Driver): void {
    const label = driver.name?.trim() || driver.licenseNumber;
    if (!window.confirm(`Deseja realmente excluir o motorista "${label}"?`)) {
      return;
    }

    this.errorMessage = '';
    this.isLoading = true;

    this.driverService.deleteDriver(driver.id).subscribe({
      next: () => {
        this.drivers = this.drivers.filter(item => item.id !== driver.id);
        this.isLoading = false;
      },
      error: error => {
        console.error('Erro ao excluir motorista:', error);
        this.errorMessage = 'Não foi possível excluir o motorista.';
        this.isLoading = false;
      }
    });
  }

  backToDashboard(): void {
    this.router.navigate(['/dashboard']);
  }

  private emptyDriver(): Driver {
    return {
      id: '',
      name: '',
      licenseNumber: ''
    };
  }
}
