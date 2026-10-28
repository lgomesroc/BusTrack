import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Passenger, PassengerPayload, PassengerService } from '../../services/passenger.service';
import { SessionService } from '../../services/session.service';
import { OperationalListState } from '../../services/operational-list-state';

@Component({ selector: 'app-passengers', templateUrl: './passengers.component.html', styleUrls: ['./passengers.component.css'] })
export class PassengersComponent extends OperationalListState<Passenger> implements OnInit {
  passengers: Passenger[] = []; passengerForm: PassengerPayload = this.emptyPassenger(); editingPassenger: Passenger | null = null; isCreating = false; isLoading = false; errorMessage = '';
  constructor(private router: Router, private passengerService: PassengerService, public sessionService: SessionService) { super(); }
  ngOnInit(): void { this.loadPassengers(); }
  loadPassengers(): void { this.isLoading = true; this.errorMessage = ''; this.passengerService.getPassengers().subscribe({ next: passengers => { this.passengers = passengers ?? []; this.currentPage = Math.min(this.currentPage, this.getTotalPages(this.passengers, ['name','cpf','email','phone'])); this.isLoading = false; }, error: error => { console.error('Erro ao carregar passageiros:', error); this.errorMessage = 'Não foi possível carregar os passageiros. Verifique se a API está disponível.'; this.isLoading = false; } }); }
  startCreate(): void { this.isCreating = true; this.editingPassenger = null; this.passengerForm = this.emptyPassenger(); this.errorMessage = ''; }
  startEdit(passenger: Passenger): void { this.isCreating = false; this.editingPassenger = passenger; this.passengerForm = { name: passenger.name, email: passenger.email, cpf: this.formatCpf(passenger.cpf), phone: passenger.phone }; this.errorMessage = ''; }
  cancelEdit(): void { this.isCreating = false; this.editingPassenger = null; this.passengerForm = this.emptyPassenger(); this.errorMessage = ''; }
  savePassenger(): void { this.errorMessage = ''; const payload: PassengerPayload = { name: this.passengerForm.name.trim(), email: this.passengerForm.email.trim(), cpf: this.formatCpf(this.passengerForm.cpf).replace(/\D/g, ''), phone: this.passengerForm.phone.trim() }; if (!payload.name || !payload.cpf) { this.errorMessage = 'Preencha o nome e o CPF do passageiro.'; return; } this.isLoading = true; const request = this.editingPassenger ? this.passengerService.updatePassenger(this.editingPassenger.id, payload) : this.passengerService.createPassenger(payload); request.subscribe({ next: saved => { this.passengers = this.editingPassenger ? this.passengers.map(p => p.id === this.editingPassenger?.id ? saved : p) : [...this.passengers, saved]; this.isLoading = false; this.cancelEdit(); }, error: error => { console.error('Erro ao salvar passageiro:', error); this.errorMessage = 'Não foi possível salvar o passageiro. Confira os dados e tente novamente.'; this.isLoading = false; } }); }
  deletePassenger(passenger: Passenger): void { const label = passenger.name?.trim() || passenger.cpf; if (!window.confirm(`Deseja realmente excluir o passageiro "${label}"?`)) return; this.errorMessage = ''; this.isLoading = true; this.passengerService.deletePassenger(passenger.id).subscribe({ next: () => { this.passengers = this.passengers.filter(item => item.id !== passenger.id); this.currentPage = Math.min(this.currentPage, this.getTotalPages(this.passengers, ['name','cpf','email','phone'])); this.isLoading = false; }, error: error => { console.error('Erro ao excluir passageiro:', error); this.errorMessage = 'Não foi possível excluir o passageiro.'; this.isLoading = false; } }); }
  backToDashboard(): void { this.router.navigate(['/dashboard']); }
  onCpfChange(value: string): void { this.passengerForm.cpf = this.formatCpf(value); }
  formatCpf(value: string | null | undefined): string { const digits = (value ?? '').replace(/\D/g, '').slice(0, 11); return digits.replace(/(\d{3})(\d)/, '$1.$2').replace(/(\d{3})(\d)/, '$1.$2').replace(/(\d{3})(\d{1,2})$/, '$1-$2'); }
  private emptyPassenger(): PassengerPayload { return { name: '', email: '', cpf: '', phone: '' }; }
}
