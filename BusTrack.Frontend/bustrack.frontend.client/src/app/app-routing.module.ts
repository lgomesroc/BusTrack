import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { MainScreenComponent } from './login/main-screen/main-screen.component';
import { EnterTheSystemComponent } from './login/enter-the-system/enter-the-system.component';
import { CreateAnAccountComponent } from './login/create-an-account/create-an-account.component';
import { UpdatePasswordComponent } from './login/update-password/update-password.component';
import { ConfirmationComponent } from './login/confirmation/confirmation.component';
import { ConcludedComponent } from './login/concluded/concluded.component';
import { DashboardComponent } from './main/dashboard/dashboard.component';
import { SidebarComponent } from './main/sidebar/sidebar.component';
import { TripsComponent } from './main/trips/trips.component';

const routes: Routes = [

  // Área pública

  { path: 'main-screen', component: MainScreenComponent },
  { path: 'enter-the-system', component: EnterTheSystemComponent },
  { path: 'create-an-account', component: CreateAnAccountComponent },
  { path: 'update-password', component: UpdatePasswordComponent },
  { path: 'confirmation', component: ConfirmationComponent },
  { path: 'conclusion', component: ConcludedComponent },

  // Área autenticada

  { path: 'dashboard', component: DashboardComponent },

  // Módulo de viagens

  { path: 'trips', component: TripsComponent },

  // Módulos ainda não implementados

  {
    path: 'buses',
    component: DashboardComponent
  },

  {
    path: 'routes',
    component: DashboardComponent
  },

  {
    path: 'drivers',
    component: DashboardComponent
  },

  {
    path: 'passengers',
    component: DashboardComponent
  },

  {
    path: 'maintenance',
    component: DashboardComponent
  },

  // Componente da barra lateral

  { path: 'sidebar', component: SidebarComponent },

  // Entrada padrão

  { path: '', redirectTo: '/main-screen', pathMatch: 'full' },

  // Rota inexistente

  { path: '**', redirectTo: '/main-screen' }

];

@NgModule({

  imports: [
    RouterModule.forRoot(routes)
  ],

  exports: [
    RouterModule
  ]

})

export class AppRoutingModule { }