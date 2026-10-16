import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { SessionService } from '../../services/session.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent {

  constructor(
    private router: Router,
    private sessionService: SessionService
  ) {}

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
    this.sessionService.clearSession();

    this.router.navigate(
      ['/enter-the-system'],
      { replaceUrl: true }
    );
  }
}
