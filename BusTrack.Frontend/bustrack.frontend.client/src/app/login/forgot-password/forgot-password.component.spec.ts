
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

import { ForgotPasswordComponent } from './forgot-password.component';

describe('ForgotPasswordComponent', () => {
  let component: ForgotPasswordComponent;
  let fixture: ComponentFixture<ForgotPasswordComponent>;
  let routerSpy: jasmine.SpyObj<Router>;

  beforeEach(async () => {
    routerSpy = jasmine.createSpyObj<Router>('Router', ['navigate']);

    await TestBed.configureTestingModule({
      declarations: [ForgotPasswordComponent],
      imports: [FormsModule],
      providers: [
        { provide: Router, useValue: routerSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ForgotPasswordComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('deve criar o componente', () => {
    expect(component).toBeTruthy();
  });

  it('deve iniciar os campos vazios', () => {
    expect(component.email).toBe('');
    expect(component.newPassword).toBe('');
    expect(component.confirmPassword).toBe('');
  });

  it('deve permitir preencher o e-mail', () => {
    component.email = 'usuario@empresa.com';

    expect(component.email).toBe('usuario@empresa.com');
  });

  it('deve permitir preencher a nova senha', () => {
    component.newPassword = 'Abcd12!!';

    expect(component.newPassword).toBe('Abcd12!!');
  });

  it('deve permitir preencher a confirmação da senha', () => {
    component.confirmPassword = 'Abcd12!!';

    expect(component.confirmPassword).toBe('Abcd12!!');
  });

  it('deve exibir aviso de que a recuperação ainda não está disponível', () => {
    spyOn(window, 'alert');

    component.submit();

    expect(window.alert).toHaveBeenCalledOnceWith(
      'A recuperação de senha ainda não está disponível. ' +
      'Ela será habilitada após a configuração da verificação segura de e-mail.'
    );
  });

  it('não deve navegar para outra página ao enviar a solicitação', () => {
    spyOn(window, 'alert');

    component.submit();

    expect(routerSpy.navigate).not.toHaveBeenCalled();
  });

  it('deve navegar para a tela de login ao cancelar', () => {
    routerSpy.navigate.and.returnValue(Promise.resolve(true));

    component.cancel();

    expect(routerSpy.navigate).toHaveBeenCalledOnceWith([
      '/enter-the-system'
    ]);
  });

  it('deve manter a recuperação desabilitada mesmo quando os campos são preenchidos', () => {
    spyOn(window, 'alert');

    component.email = 'usuario@empresa.com';
    component.newPassword = 'Abcd12!!';
    component.confirmPassword = 'Abcd12!!';

    component.submit();

    expect(window.alert).toHaveBeenCalled();
    expect(routerSpy.navigate).not.toHaveBeenCalled();
  });
});
