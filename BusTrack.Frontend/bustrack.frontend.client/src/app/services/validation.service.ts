// validation.service.ts

import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ValidationService {
  cpfInvalido: boolean = false; // Adicione esta propriedade
  fullName: string = '';
  cpf: string = '';
  email: string = '';
  password: string = '';
  confirmPassword: string = '';
  errorMessage: string = '';
  loginAttempts: number = 5;


  constructor() {}

  validarCPF(cpf: string): boolean {
    // Remove caracteres não numéricos
    cpf = cpf.replace(/\D/g, '');

    // Verifica se o CPF tem 11 dígitos
    if (cpf.length !== 11) {
      this.cpfInvalido = true;
      return false;
    }

    // Verifica se todos os dígitos são iguais
    if (/^(\d)\1{10}$/.test(cpf)) {
      this.cpfInvalido = true;
      return false;
    }

    // Calcula o primeiro dígito verificador
    let sum = 0;
    for (let i = 0; i < 9; i++) {
      sum += parseInt(cpf.charAt(i)) * (10 - i);
    }
    let remainder = sum % 11;
    let digit = remainder < 2 ? 0 : 11 - remainder;

    // Verifica se o primeiro dígito verificador é igual ao do CPF
    if (digit !== parseInt(cpf.charAt(9))) {
      this.cpfInvalido = true;
      return false;
    }

    // Calcula o segundo dígito verificador
    sum = 0;
    for (let i = 0; i < 10; i++) {
      sum += parseInt(cpf.charAt(i)) * (11 - i);
    }
    remainder = sum % 11;
    digit = remainder < 2 ? 0 : 11 - remainder;

    // Verifica se o segundo dígito verificador é igual ao do CPF
    if (digit !== parseInt(cpf.charAt(10))) {
      this.cpfInvalido = true;
      return false;
    }

    // CPF válido
    this.cpfInvalido = false;
    return true;
  }

  validarSenha(password: string): boolean {
    if (password.length < 8) {
      return false;
    }

    // Verifica se há pelo menos duas letras maiúsculas
    if ((password.match(/[A-Z]/g) || []).length < 2) {
      return false;
    }

    // Verifica se há pelo menos duas letras minúsculas
    if ((password.match(/[a-z]/g) || []).length < 2) {
      return false;
    }

    // Verifica se há pelo menos dois números que não sejam sequenciais nem repetidos
    if (!/(?!.*(\d)\1)(?!.*(\d)(\d)\2)\d{2}/.test(password)) {
      return false;
    }

    // Verifica se há pelo menos dois caracteres especiais
    if (!/[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]+/.test(password)) {
      return false;
    }

    // Senha válida
    return true;
  }
}
