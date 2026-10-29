import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { OperationalReport, OperationalReportService } from '../../services/operational-report.service';
import { SessionService } from '../../services/session.service';

@Component({
  selector: 'app-operational-reports',
  templateUrl: './operational-reports.component.html',
  styleUrls: ['./operational-reports.component.css']
})
export class OperationalReportsComponent implements OnInit {
  startDate = this.getMonthStart();
  endDate = this.getToday();
  report: OperationalReport | null = null;
  isLoading = false;
  errorMessage = '';
  hasSearched = false;

  constructor(
    private readonly router: Router,
    private readonly reportService: OperationalReportService,
    public readonly sessionService: SessionService
  ) {}

  ngOnInit(): void {
    if (!this.sessionService.isAdmin()) {
      void this.router.navigate(['/dashboard'], { replaceUrl: true });
      return;
    }

    this.loadReport();
  }

  loadReport(): void {
    this.errorMessage = '';
    this.report = null;

    if (!this.startDate || !this.endDate) {
      this.errorMessage = 'Informe a data inicial e a data final.';
      this.hasSearched = true;
      return;
    }

    if (this.startDate > this.endDate) {
      this.errorMessage = 'A data inicial não pode ser posterior à data final.';
      this.hasSearched = true;
      return;
    }

    this.isLoading = true;
    this.hasSearched = true;

    this.reportService.getReport(this.startDate, this.endDate).subscribe({
      next: report => {
        this.report = report;
        this.isLoading = false;
      },
      error: error => {
        this.errorMessage =
          error?.error?.message ??
          'Não foi possível carregar os relatórios. Verifique se a API está atualizada e tente novamente.';
        this.isLoading = false;
      }
    });
  }

  get hasTrips(): boolean {
    return (this.report?.totalTrips ?? 0) > 0;
  }

  get hasMaintenance(): boolean {
    return (this.report?.maintenanceCosts ?? []).length > 0 ||
      (this.report?.totalMaintenanceCost ?? 0) > 0;
  }

  backToDashboard(): void {
    void this.router.navigate(['/dashboard']);
  }

  private getToday(): string {
    return this.toDateInputValue(new Date());
  }

  private getMonthStart(): string {
    const today = new Date();
    return this.toDateInputValue(new Date(today.getFullYear(), today.getMonth(), 1));
  }

  private toDateInputValue(date: Date): string {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }
}
