export function hasRepeatedOrSequentialNumbersRule(
  password: string
): boolean {
  for (let i = 0; i < password.length - 1; i++) {
    const currentChar = password[i];
    const nextChar = password[i + 1];

    // Detecta caracteres adjacentes repetidos somente
    // quando existe uma sequência de pelo menos 3 caracteres.
    if (
      i + 2 < password.length &&
      currentChar === nextChar
    ) {
      return true;
    }

    // Detecta um caractere repetido com outro entre eles.
    if (
      i + 2 < password.length &&
      currentChar === password[i + 2]
    ) {
      return true;
    }

    // Detecta números consecutivos, inclusive no final da senha.
    if (
      /^\d$/.test(currentChar) &&
      /^\d$/.test(nextChar) &&
      Number(nextChar) === Number(currentChar) + 1
    ) {
      return true;
    }
  }

  return false;
}
