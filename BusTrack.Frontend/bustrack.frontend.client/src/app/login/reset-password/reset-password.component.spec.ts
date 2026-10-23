
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule } from '@angular/forms';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ActivatedRoute, convertToParamMap, Router } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';

import { ResetPasswordComponent } from './reset-password.component';
import { environment } from '../../../environments/environment';

describe('ResetPasswordComponent', () => {
  let component: ResetPasswordComponent;
  let fixture: ComponentFixture<ResetPasswordComponent>;
  let httpMock: HttpTestingController;
  let router: Router;

  const apiUrl =
    `${environment.apiUrl}/AuthenticationControllerAPI/reset-password`;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ResetPasswordComponent],
      imports: [
        FormsModule,
        HttpClientTestingModule,
        RouterTestingModule
      ],
      providers: [
        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: {
              queryParamMap: convertToParamMap({
                email: 'usuario@exemplo.com'
              })
            }
          }
        }
      ]
    }).compileComponents();

    httpMock = TestBed.inject(HttpTestingController);
    router = TestBed.inject(Router);
    fixture = TestBed.createComponent(ResetPasswordComponent);
    component = fixture.componentInstance;

    fixture.detectChanges();
  });

  afterEach(() => {
    httpMock.verify();
  });

  function fillValidForm(): void {
    component.email = 'usuario@exemplo.com';
    component.recoveryCode = 'codigo-teste';
    component.newPassword = 'NovaSenha123!';
    component.confirmPassword = 'NovaSenha123!';
  }

  it('deve criar o componente', () => {
    expect(component).toBeTruthy();
  });

  it('deve carregar o e-mail dos parâmetros da rota', () => {
    expect(component.email).toBe('usuario@exemplo.com');
  });

  it('deve rejeitar campos obrigatórios vazios sem chamar a API', () => {
    component.submit();

    expect(component.errorMessage).toBe('Preencha todos os campos.');
    expect(component.isSubmitting).toBeFalse();

    httpMock.expectNone(apiUrl);
  });

  it('deve rejeitar senha com menos de 8 caracteres', () => {
    fillValidForm();
    component.newPassword = 'Ab1!';
    component.confirmPassword = 'Ab1!';

    component.submit();

    expect(component.errorMessage)
      .toBe('A senha deve ter pelo menos 8 caracteres.');

    httpMock.expectNone(apiUrl);
  });

  it('deve rejeitar senhas diferentes', () => {
    fillValidForm();
    component.confirmPassword = 'OutraSenha123!';

    component.submit();

    expect(component.errorMessage).toBe('As senhas não coincidem.');

    httpMock.expectNone(apiUrl);
  });

  it('deve enviar os dados de recuperação para a API', () => {
    fillValidForm();

    component.submit();

    const request = httpMock.expectOne(apiUrl);

    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual({
      email: 'usuario@exemplo.com',
      recoveryCode: 'codigo-teste',
      newPassword: 'NovaSenha123!'
    });

    request.flush({
      success: true,
      message: 'Senha redefinida com sucesso.'
    });

    expect(component.successMessage)
      .toBe('Senha redefinida com sucesso.');
    expect(component.errorMessage).toBe('');
    expect(component.isSubmitting).toBeFalse();
    expect(component.recoveryCode).toBe('');
    expect(component.newPassword).toBe('');
    expect(component.confirmPassword).toBe('');
  });

  it('deve exibir erro genérico quando a API rejeitar a recuperação', () => {
    fillValidForm();

    component.submit();

    const request = httpMock.expectOne(apiUrl);

    request.flush(
      {
        success: false,
        message: 'Mensagem específica interna'
      },
      {
        status: 400,
        statusText: 'Bad Request'
      }
    );

    expect(component.errorMessage).toBe(
      'Não foi possível validar a recuperação. ' +
      'Verifique os dados e o código informado.'
    );
    expect(component.errorMessage).not.toContain('administrador');
    expect(component.isSubmitting).toBeFalse();
  });

  it('deve voltar para a tela de login', async () => {
    const navigateSpy = spyOn(router, 'navigate')
      .and.resolveTo(true);

    component.backToLogin();

    expect(navigateSpy).toHaveBeenCalledWith(['/enter-the-system']);
  });

  it('deve exibir a mensagem de erro na interface', () => {
    component.errorMessage =
      'Não foi possível validar a recuperação. ' +
      'Verifique os dados e o código informado.';

    fixture.detectChanges();

    const errorElement: HTMLElement | null =
      fixture.nativeElement.querySelector('[role="alert"]');

    expect(errorElement).not.toBeNull();
    expect(errorElement?.textContent)
      .toContain('Não foi possível validar a recuperação.');
  });
});
