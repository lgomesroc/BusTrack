import { NgModule } from '@angular/core';

import {
  RouterModule,
  Routes
} from '@angular/router';

import { MainScreenComponent }
  from './login/main-screen/main-screen.component';

import { EnterTheSystemComponent }
  from './login/enter-the-system/enter-the-system.component';

import { CreateAnAccountComponent }
  from './login/create-an-account/create-an-account.component';

import { UpdatePasswordComponent }
  from './login/update-password/update-password.component';

import { ConfirmationComponent }
  from './login/confirmation/confirmation.component';

import { ConcludedComponent }
  from './login/concluded/concluded.component';

import { DashboardComponent }
  from './main/dashboard/dashboard.component';

import { SidebarComponent }
  from './main/sidebar/sidebar.component';

import { TripsComponent }
  from './main/trips/trips.component';

import { BusesComponent }
  from './main/buses/buses.component';

import { AuthenticationGuard }
  from './guards/authentication.guard';

import { BrowserNavigationGuard }
  from './guards/browser-navigation.guard';

const routes: Routes = [

  // Área pública

  {
    path: 'main-screen',
    component: MainScreenComponent,
    canActivate: [BrowserNavigationGuard]
  },

  {
    path: 'enter-the-system',
    component: EnterTheSystemComponent,
    canActivate: [BrowserNavigationGuard]
  },

  {
    path: 'create-an-account',
    component: CreateAnAccountComponent,
    canActivate: [BrowserNavigationGuard]
  },

  {
    path: 'update-password',
    component: UpdatePasswordComponent,
    canActivate: [BrowserNavigationGuard]
  },

  {
    path: 'confirmation',
    component: ConfirmationComponent,
    canActivate: [BrowserNavigationGuard]
  },

  {
    path: 'conclusion',
    component: ConcludedComponent,
    canActivate: [BrowserNavigationGuard]
  },

  // Área autenticada

  {
    path: 'dashboard',
    component: DashboardComponent,
    canActivate: [
      AuthenticationGuard,
      BrowserNavigationGuard
    ]
  },

  {
    path: 'trips',
    component: TripsComponent,
    canActivate: [
      AuthenticationGuard,
      BrowserNavigationGuard
    ]
  },

  {
    path: 'buses',
    component: BusesComponent,
    canActivate: [
      AuthenticationGuard,
      BrowserNavigationGuard
    ]
  },

  // Módulos ainda não implementados

  {
    path: 'routes',
    component: DashboardComponent,
    canActivate: [
      AuthenticationGuard,
      BrowserNavigationGuard
    ]
  },

  {
    path: 'drivers',
    component: DashboardComponent,
    canActivate: [
      AuthenticationGuard,
      BrowserNavigationGuard
    ]
  },

  {
    path: 'passengers',
    component: DashboardComponent,
    canActivate: [
      AuthenticationGuard,
      BrowserNavigationGuard
    ]
  },

  {
    path: 'maintenance',
    component: DashboardComponent,
    canActivate: [
      AuthenticationGuard,
      BrowserNavigationGuard
    ]
  },

  // Sidebar

  {
    path: 'sidebar',
    component: SidebarComponent,
    canActivate: [
      AuthenticationGuard,
      BrowserNavigationGuard
    ]
  },

  // Entrada padrão

  {
    path: '',
    redirectTo: '/main-screen',
    pathMatch: 'full'
  },

  // Rota inexistente

  {
    path: '**',
    redirectTo: '/main-screen'
  }

];

@NgModule({
  imports: [
    RouterModule.forRoot(routes)
  ],
  exports: [
    RouterModule
  ]
})
export class AppRoutingModule {
}
