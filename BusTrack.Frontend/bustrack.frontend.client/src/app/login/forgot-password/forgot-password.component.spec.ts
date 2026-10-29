
import { fakeAsync, TestBed, tick } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import {
  HttpTestingController,
  provideHttpClientTesting
} from '@angular/common/http/testing';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

import { ForgotPasswordComponent } from './forgot-password.component';
import { environment } from '../../../environments/environment';

describe('ForgotPasswordComponent', () => {
  let component: ForgotPasswordComponent;
  let fixture: import('@angular/core/testing').ComponentFixture<ForgotPasswordComponent>;
  let httpTestingController: HttpTestingController;
  let routerSpy: jasmine.SpyObj<Router>;

  const recoveryUrl =
    `${environment.apiUrl}/AuthenticationControllerAPI/forgot-password`;

  beforeEach(async () => {
    routerSpy = jasmine.createSpyObj<Router>('Router', ['navigate']);
    routerSpy.navigate.and.returnValue(Promise.resolve(true));

    await TestBed.configureTestingModule({
      declarations: [ForgotPasswordComponent],
      imports: [FormsModule],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        { provide: Router, useValue: routerSpy }
      ]
    }).compileComponents();

    httpTestingController = TestBed.inject(HttpTestingController);

    fixture = TestBed.createComponent(ForgotPasswordComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  afterEach(() => {
    httpTestingController.verify();
  });

  function preencherFormulario(): void {
    component.email = 'usuario@empresa.com';
    component.newPassword = 'Abcd1234!';
    component.confirmPassword = 'Abcd1234!';
  }

  it('deve criar o componente', () => {
    expect(component).toBeTruthy();
  });

  it('deve iniciar os campos vazios e sem mensagens', () => {
    expect(component.email).toBe('');
    expect(component.newPassword).toBe('');
    expect(component.confirmPassword).toBe('');
    expect(component.errorMessage).toBe('');
    expect(component.successMessage).toBe('');
    expect(component.isSubmitting).toBeFalse();
  });

  it('deve permitir preencher os campos', () => {
    preencherFormulario();

    expect(component.email).toBe('usuario@empresa.com');
    expect(component.newPassword).toBe('Abcd1234!');
    expect(component.confirmPassword).toBe('Abcd1234!');
  });

  it('deve rejeitar campos vazios ou e-mail inválido', () => {
    component.submit();

    expect(component.errorMessage).toBe(
      'Preencha os três campos corretamente.'
    );
    expect(component.isSubmitting).toBeFalse();

    component.email = 'email-invalido';
    component.newPassword = 'Abcd1234!';
    component.confirmPassword = 'Abcd1234!';
    component.submit();

    expect(component.errorMessage).toBe(
      'Preencha os três campos corretamente.'
    );
    httpTestingController.expectNone(recoveryUrl);
  });

  it('deve rejeitar senhas diferentes', () => {
    component.email = 'usuario@empresa.com';
    component.newPassword = 'Abcd1234!';
    component.confirmPassword = 'OutraSenha123!';
    component.submit();

    expect(component.errorMessage).toBe('As senhas não coincidem.');
    expect(component.isSubmitting).toBeFalse();
    httpTestingController.expectNone(recoveryUrl);
  });

  it('deve rejeitar senhas com menos de oito caracteres', () => {
    component.email = 'usuario@empresa.com';
    component.newPassword = 'Abc123!';
    component.confirmPassword = 'Abc123!';
    component.submit();

    expect(component.errorMessage).toBe(
      'A senha deve ter pelo menos 8 caracteres.'
    );
    expect(component.isSubmitting).toBeFalse();
    httpTestingController.expectNone(recoveryUrl);
  });

  it('deve enviar os dados válidos para a API', () => {
    preencherFormulario();

    component.submit();

    const request = httpTestingController.expectOne(recoveryUrl);

    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual({
      email: 'usuario@empresa.com',
      newPassword: 'Abcd1234!',
      confirmPassword: 'Abcd1234!'
    });
    expect(component.isSubmitting).toBeTrue();

    request.flush({
      success: true,
      message: 'Solicitação processada.'
    });

    expect(component.successMessage).toBe('Solicitação processada.');
    expect(component.newPassword).toBe('');
    expect(component.confirmPassword).toBe('');
    expect(component.isSubmitting).toBeFalse();
  });

  it('deve voltar ao login após a recuperação bem-sucedida', fakeAsync(() => {
    preencherFormulario();

    component.submit();

    const request = httpTestingController.expectOne(recoveryUrl);
    request.flush({
      success: true,
      message: 'Senha alterada com sucesso.'
    });

    tick(900);

    expect(routerSpy.navigate).toHaveBeenCalledOnceWith([
      '/enter-the-system'
    ]);
  }));

  it('deve exibir mensagem genérica quando a API falhar', () => {
    preencherFormulario();

    component.submit();

    const request = httpTestingController.expectOne(recoveryUrl);
    request.flush(
      { message: 'Erro interno detalhado' },
      { status: 500, statusText: 'Internal Server Error' }
    );

    expect(component.errorMessage).toBe(
      'Não foi possível concluir a recuperação. Verifique os dados informados.'
    );
    expect(component.isSubmitting).toBeFalse();
    expect(component.successMessage).toBe('');
  });

  it('deve exibir mensagem genérica quando a API rejeitar a solicitação', () => {
    preencherFormulario();

    component.submit();

    const request = httpTestingController.expectOne(recoveryUrl);
    request.flush({
      success: false,
      message: 'Detalhe interno'
    });

    expect(component.errorMessage).toBe(
      'Não foi possível concluir a recuperação. Verifique os dados informados.'
    );
    expect(component.isSubmitting).toBeFalse();
  });

  it('deve navegar para o login ao clicar em voltar', () => {
    component.backToLogin();

    expect(routerSpy.navigate).toHaveBeenCalledOnceWith([
      '/enter-the-system'
    ]);
  });
});
