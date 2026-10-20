
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';

import { MainScreenComponent } from './login/main-screen/main-screen.component';
import { CreateAnAccountComponent } from './login/create-an-account/create-an-account.component';
import { EnterTheSystemComponent } from './login/enter-the-system/enter-the-system.component';
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

import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { JwtAuthInterceptor } from './services/jwt-auth.interceptor';

@NgModule({
  declarations: [
    AppComponent,
    MainScreenComponent,
    CreateAnAccountComponent,
    EnterTheSystemComponent,
    UpdatePasswordComponent,
    ConfirmationComponent,
    ConcludedComponent,
    DashboardComponent,
    SidebarComponent,
    TripsComponent,
    TripDetailsComponent,
    BusesComponent,
    RoutesComponent,
    DriversComponent,
    PassengersComponent,
    MaintenanceComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule
  ],
  providers: [
    provideAnimationsAsync(),
    {
      provide: HTTP_INTERCEPTORS,
      useClass: JwtAuthInterceptor,
      multi: true
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule {}
