export function preventForwardNavigationRule(): void {
  /*
   * A navegação pelos botões Voltar e Avançar
   * do navegador agora é controlada exclusivamente
   * pelo BrowserNavigationGuard.
   *
   * Esta função foi mantida temporariamente para
   * evitar alterações desnecessárias nos componentes
   * que ainda possuem sua chamada.
   *
   * Não registra listeners, não altera o histórico
   * e não exibe mensagens.
   */
}
