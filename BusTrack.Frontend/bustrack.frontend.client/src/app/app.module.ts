import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
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


@NgModule({
  declarations: [
    AppComponent,
    MainScreenComponent,
    CreateAnAccountComponent,
    EnterTheSystemComponent,
    ConfirmationComponent,
    ConcludedComponent,
    UpdatePasswordComponent,
    DashboardComponent,
    SidebarComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
