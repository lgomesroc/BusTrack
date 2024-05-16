import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { MainScreenComponent } from './login/main-screen/main-screen.component'; // Importe o componente de login
import { EnterTheSystemComponent } from './login/enter-the-system/enter-the-system.component';
import { CreateAnAccountComponent } from './login/create-an-account/create-an-account.component';
import { UpdatePasswordComponent } from './login/update-password/update-password.component';
import { ConfirmationComponent } from './login/confirmation/confirmation.component';
import { ConcludedComponent } from './login/concluded/concluded.component';


const routes: Routes = [
  { path: 'Tela principal', component: MainScreenComponent }, // Adicione a rota para a tela de login
  { path: 'Tela de login', component: EnterTheSystemComponent },
  { path: 'Tela de criar conta', component: CreateAnAccountComponent },
  { path: 'Tela de atualizar senha', component: UpdatePasswordComponent },
  { path: 'Tela de confirmação', component: ConfirmationComponent },
  { path: 'Tela de conclusão', component: ConcludedComponent },
  // Adicione outras rotas conforme necessário
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
