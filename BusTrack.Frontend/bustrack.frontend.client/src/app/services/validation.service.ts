import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ValidationService {
  cpfInvalido: boolean = false; 
  fullName: string = '';
  cpf: string = '';
  email: string = '';
  password: string = '';
  confirmPassword: string = '';
  errorMessage: string = '';
  loginAttempts: number = 5;


  constructor() {}

  validarCPF(cpf: string): boolean {
    cpf = cpf.replace(/\D/g, '');

    if (cpf.length !== 11) {
      this.cpfInvalido = true;
      return false;
    }

    if (/^(\d)\1{10}$/.test(cpf)) {
      this.cpfInvalido = true;
      return false;
    }

    let sum = 0;
    for (let i = 0; i < 9; i++) {
      sum += parseInt(cpf.charAt(i)) * (10 - i);
    }
    let remainder = sum % 11;
    let digit = remainder < 2 ? 0 : 11 - remainder;

    if (digit !== parseInt(cpf.charAt(9))) {
      this.cpfInvalido = true;
      return false;
    }

    sum = 0;
    for (let i = 0; i < 10; i++) {
      sum += parseInt(cpf.charAt(i)) * (11 - i);
    }
    remainder = sum % 11;
    digit = remainder < 2 ? 0 : 11 - remainder;

    if (digit !== parseInt(cpf.charAt(10))) {
      this.cpfInvalido = true;
      return false;
    }

    this.cpfInvalido = false;
    return true;
  }

  validarSenha(password: string): boolean {
    if (password.length < 8) {
      return false;
    }

    if ((password.match(/[A-Z]/g) || []).length < 2) {
      return false;
    }

    if ((password.match(/[a-z]/g) || []).length < 2) {
      return false;
    }

    if (!/(?!.*(\d)\1)(?!.*(\d)(\d)\2)\d{2}/.test(password)) {
      return false;
    }

    if (!/[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]+/.test(password)) {
      return false;
    }

    return true;
  }
}
