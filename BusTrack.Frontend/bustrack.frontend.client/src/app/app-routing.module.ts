
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
import { TripDetailsComponent } from './main/trip-details/trip-details.component';
import { BusesComponent } from './main/buses/buses.component';
import { RoutesComponent } from './main/routes/routes.component';
import { DriversComponent } from './main/drivers/drivers.component';
import { PassengersComponent } from './main/passengers/passengers.component';
import { MaintenanceComponent } from './main/maintenance/maintenance.component';

import { AuthenticationGuard } from './guards/authentication.guard';
import { BrowserNavigationGuard } from './guards/browser-navigation.guard';

const protectedGuards = [
  AuthenticationGuard,
  BrowserNavigationGuard
];

const routes: Routes = [
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
  {
    path: 'dashboard',
    component: DashboardComponent,
    canActivate: protectedGuards
  },
  {
    path: 'trips',
    component: TripsComponent,
    canActivate: protectedGuards
  },
  {
    path: 'trips/:id',
    component: TripDetailsComponent,
    canActivate: protectedGuards
  },
  {
    path: 'buses',
    component: BusesComponent,
    canActivate: protectedGuards
  },
  {
    path: 'routes',
    component: RoutesComponent,
    canActivate: protectedGuards
  },
  {
    path: 'drivers',
    component: DriversComponent,
    canActivate: protectedGuards
  },
  {
    path: 'passengers',
    component: PassengersComponent,
    canActivate: protectedGuards
  },
  {
    path: 'maintenance',
    component: MaintenanceComponent,
    canActivate: protectedGuards
  },
  {
    path: 'sidebar',
    component: SidebarComponent,
    canActivate: protectedGuards
  },
  {
    path: '',
    redirectTo: '/main-screen',
    pathMatch: 'full'
  },
  {
    path: '**',
    redirectTo: '/main-screen'
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {}
