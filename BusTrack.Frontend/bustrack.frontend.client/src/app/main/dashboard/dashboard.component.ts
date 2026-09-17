import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent {

  constructor(private router: Router) {}

  navegarParaViagens(): void {
    this.router.navigate(['/trips']);
  }

  navegarParaOnibus(): void {
    this.router.navigate(['/buses']);
  }

  navegarParaRotas(): void {
    this.router.navigate(['/routes']);
  }

  navegarParaMotoristas(): void {
    this.router.navigate(['/drivers']);
  }

  navegarParaPassageiros(): void {
    this.router.navigate(['/passengers']);
  }

  navegarParaManutencao(): void {
    this.router.navigate(['/maintenance']);
  }

  sair(): void {
    this.router.navigate(['/enter-the-system']);
  }
}
