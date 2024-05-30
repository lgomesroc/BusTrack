// Função para verificar números repetidos ou sequenciais (se necessário)
export function hasRepeatedOrSequentialNumbersRule(password: string): boolean {for (let i = 0; i < password.length - 2; i++) {
  const currentChar = password[i];
  const nextChar = password[i + 1];
  const nextNextChar = password[i + 2];

  // Valida sequências de teclas adjacentes
  if (currentChar === nextChar) {
    return true;
  }

  // Valida sequências de teclas com um intervalo
  if (currentChar === nextNextChar) {
    return true;
  }

  // Valida sequências numéricas adjacentes
  if (isNaN(parseInt(currentChar)) || isNaN(parseInt(nextChar))) {
    continue;
  }

  const currentDigit = parseInt(currentChar, 10);
  const nextDigit = parseInt(nextChar, 10);

  if (nextDigit === currentDigit + 1) {
    return true;
  }
}

return false;
}
