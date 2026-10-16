import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { SessionService } from '../../services/session.service';
import { Bus, BusService } from '../../services/bus.service';
import { Maintenance, MaintenancePayload, MaintenanceService, MaintenanceStatus } from '../../services/maintenance.service';

@Component({ selector:'app-maintenance', templateUrl:'./maintenance.component.html', styleUrls:['./maintenance.component.css'] })
export class MaintenanceComponent implements OnInit {
  maintenances:Maintenance[]=[];
  buses:Bus[]=[];
  form:MaintenancePayload=this.emptyForm();
  editing:Maintenance|null=null;
  isCreating=false;
  isLoading=false;
  errorMessage='';
  readonly statuses:MaintenanceStatus[]=['Agendada','Em andamento','Concluída','Cancelada'];

  constructor(private router:Router,private maintenanceService:MaintenanceService,private busService:BusService,public sessionService:SessionService) {}
  ngOnInit():void { this.loadBuses(); this.loadMaintenances(); }
  loadBuses():void {
    this.busService.getBuses().subscribe({
      next:buses=>this.buses=buses??[],
      error:()=>this.errorMessage='Não foi possível carregar os ônibus.'
    });
  }
  loadMaintenances():void {
    this.isLoading=true; this.errorMessage='';
    this.maintenanceService.getAll().subscribe({
      next:items=>{this.maintenances=items??[];this.isLoading=false;},
      error:error=>{console.error('Erro ao carregar manutenções:',error);this.errorMessage='Não foi possível carregar as manutenções.';this.isLoading=false;}
    });
  }
  startCreate():void {this.isCreating=true;this.editing=null;this.form=this.emptyForm();this.errorMessage='';}
  startEdit(item:Maintenance):void {
    this.isCreating=false;this.editing=item;
    this.form={busId:item.busId,serviceType:item.serviceType,description:item.description,serviceDate:item.serviceDate?.slice(0,10)??'',completedDate:item.completedDate??null,cost:item.cost,status:item.status};
    this.errorMessage='';
  }
  cancelEdit():void {this.isCreating=false;this.editing=null;this.form=this.emptyForm();this.errorMessage='';}
  save():void {
    this.errorMessage='';
    if(!this.form.busId||!this.form.serviceType.trim()||!this.form.description.trim()||!this.form.serviceDate){
      this.errorMessage='Preencha o ônibus, o tipo de serviço, a descrição e a data.';return;
    }
    if(Number(this.form.cost)<0||!Number.isFinite(Number(this.form.cost))){this.errorMessage='Informe um custo válido, igual ou maior que zero.';return;}
    const payload:MaintenancePayload={...this.form,serviceType:this.form.serviceType.trim(),description:this.form.description.trim(),cost:Number(this.form.cost),serviceDate:new Date(this.form.serviceDate).toISOString()};
    this.isLoading=true;
    const request=this.editing?this.maintenanceService.update(this.editing.id,payload):this.maintenanceService.create(payload);
    request.subscribe({
      next:saved=>{if(this.editing){this.maintenances=this.maintenances.map(item=>item.id===saved.id?saved:item);}else{this.maintenances=[saved,...this.maintenances];}this.isLoading=false;this.cancelEdit();},
      error:error=>{console.error('Erro ao salvar manutenção:',error);this.errorMessage='Não foi possível salvar. Confira os dados e se o ônibus existe.';this.isLoading=false;}
    });
  }
  delete(item:Maintenance):void {
    if(!window.confirm(`Deseja excluir a manutenção "${item.serviceType}" do ônibus ${item.busNumber}?`))return;
    this.isLoading=true;this.errorMessage='';
    this.maintenanceService.delete(item.id).subscribe({
      next:()=>{this.maintenances=this.maintenances.filter(current=>current.id!==item.id);this.isLoading=false;},
      error:error=>{console.error('Erro ao excluir manutenção:',error);this.errorMessage='Não foi possível excluir a manutenção.';this.isLoading=false;}
    });
  }
  backToDashboard():void {this.router.navigate(['/dashboard']);}
  private emptyForm():MaintenancePayload {return {busId:'',serviceType:'',description:'',serviceDate:'',completedDate:null,cost:0,status:'Agendada'};}
}