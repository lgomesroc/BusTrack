export function limitCharactersRule(campo: HTMLInputElement, limite: number): void {
    campo.addEventListener('input', () => {
        if (campo.value.length > limite) {
            campo.value = campo.value.slice(0, limite);
            alert('O campo excede o limite de caracteres.');
        }
    });
}
