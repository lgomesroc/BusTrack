# Funcionalidades

## Visão geral

O BusTrack é uma aplicação para gerenciamento de transporte coletivo. A solução contém módulos operacionais, autenticação, controle de sessão e recursos administrativos.

Este documento diferencia funcionalidades implementadas de fluxos que ainda não estão completos. A presença de uma tela ou endpoint, isoladamente, não significa que a funcionalidade esteja pronta para uso.

---

## Ônibus

O sistema permite:

- cadastrar ônibus;
- consultar ônibus;
- consultar um ônibus específico;
- editar ônibus;
- excluir ônibus;
- utilizar ônibus no contexto das viagens.

---

## Motoristas

O sistema permite:

- cadastrar motoristas;
- consultar motoristas;
- consultar um motorista específico;
- editar motoristas;
- excluir motoristas;
- associar motoristas às viagens.

---

## Rotas

O sistema permite:

- cadastrar rotas;
- consultar rotas;
- consultar uma rota específica;
- editar rotas;
- excluir rotas;
- utilizar rotas nas viagens.

---

## Passageiros

O sistema permite:

- cadastrar passageiros;
- consultar passageiros;
- consultar um passageiro específico;
- editar passageiros;
- excluir passageiros;
- associar passageiros às viagens.

A associação entre passageiro e viagem é mantida separadamente do cadastro do passageiro.

---

## Viagens

Uma viagem reúne as informações necessárias para representar uma operação de transporte.

O sistema permite:

- cadastrar viagens;
- consultar viagens;
- consultar uma viagem específica;
- editar viagens;
- excluir viagens;
- associar ônibus;
- associar motorista;
- associar rota;
- associar passageiros;
- consultar os detalhes da viagem;
- aplicar as regras relacionadas à capacidade de passageiros;
- informar a duração prevista da viagem;
- impedir conflitos de agenda do mesmo ônibus;
- impedir conflitos de agenda do mesmo motorista.

### Conflitos de recursos

A duração prevista define o intervalo operacional usado para verificar a disponibilidade dos recursos.

Para uma viagem com saída em `T` e duração prevista de `D` minutos, o período considerado é:

`[T, T + D)`

Uma nova viagem entra em conflito quando os intervalos se sobrepõem. Assim, uma viagem que termina exatamente no horário de saída da próxima não é considerada conflitante.

A regra é aplicada no backend e considera:

- o mesmo ônibus não pode estar alocado em duas viagens simultâneas;
- o mesmo motorista não pode estar alocado em duas viagens simultâneas;
- viagens `Concluída` e `Cancelada` não bloqueiam novos agendamentos;
- a própria viagem é ignorada durante uma atualização para não gerar conflito consigo mesma;
- a duração prevista deve ser maior que zero;
- a verificação é feita antes da persistência da viagem.

A interface informa a duração prevista e apresenta ao usuário uma mensagem de erro quando a API rejeita a alocação por conflito. A API permanece como autoridade para a regra de disponibilidade.

---

## Manutenções

O módulo de manutenções possui consulta, criação, atualização e exclusão de registros vinculados a ônibus. O backend valida os dados, datas, custo e transições de status. A atualização e exclusão são restritas a `ADMIN`; a consulta do histórico de auditoria também é administrativa. A exclusão exige justificativa e a elegibilidade depende do status da manutenção. Existem testes unitários do serviço e testes de integração com persistência real. A tela Angular está registrada na navegação/roteamento, mas a cobertura de ponta a ponta e a documentação de regras devem continuar alinhadas ao comportamento do serviço.

### Disponibilidade do ônibus durante manutenção

Ao cadastrar uma viagem, o backend verifica se o ônibus possui manutenção com status `Agendada` ou `Em andamento` na mesma data de serviço. Nessa versão, a manutenção contém `ServiceDate`, sem horário inicial/final ou duração planejada; por isso, a regra considera o dia de serviço inteiro, em vez de inventar um intervalo de horas.

Manutenções concluídas, canceladas ou excluídas logicamente não bloqueiam a viagem. A verificação ocorre no backend e é combinada com a validação de conflitos de horário entre viagens para o mesmo ônibus ou motorista.

---

## Contas e autenticação

O código contém os seguintes recursos:

- criação de conta e login;
- logout e controle de sessão;
- alteração de senha autenticada;
- política e histórico de senhas;
- autenticação por JWT;
- autorização por roles `USER` e `ADMIN`;
- bootstrap controlado do primeiro administrador.

O cadastro público cria contas com role `USER`; a role não deve ser definida pelo cliente.

### Estado da recuperação de senha

A recuperação de senha **não deve ser considerada concluída**. O frontend possui uma tela `forgot-password` que envia uma nova senha diretamente para `AuthenticationControllerAPI/forgot-password`, endpoint protegido por `ADMIN`. Já `PasswordRecoveryControllerAPI` expõe `POST /api/password-recovery/request`, mas a própria resposta informa que a verificação segura ainda precisa ser implementada. Também existe um componente `reset-password`, que envia código de recuperação, mas ele não está registrado nas rotas nem no módulo Angular revisados. Portanto, tela, API e fluxo de recuperação ainda não estão alinhados e precisam ser corrigidos antes de divulgar essa funcionalidade como pronta.

---

## Sessões e presença do usuário

A V1 possui controle de sessão e presença do usuário.

O sistema permite:

- manter a sessão autenticada no navegador;
- compartilhar a sessão entre abas do mesmo navegador;
- permitir sessões independentes em navegadores diferentes;
- impedir mais de uma sessão no mesmo navegador;
- identificar o navegador da sessão;
- controlar a validade da sessão;
- controlar inatividade;
- detectar a presença do usuário;
- encerrar a sessão quando a comunicação necessária com a aplicação é perdida.

O token de acesso é mantido em `sessionStorage` no frontend.

---

## Dashboard

A aplicação tem um dashboard geral e componentes específicos para os papéis `USER` e `ADMIN`. A rota `/dashboard` usa o redirecionamento por papel quando o guard correspondente está configurado na versão em análise; confirme o destino efetivo com os testes de rota antes de assumir que ambos os papéis chegam à mesma tela.

### Usuário comum

O usuário acessa as funcionalidades permitidas pela role `USER`.

### Administrador

O administrador possui o Dashboard administrativo, incluindo informações administrativas e gerenciamento/consulta de usuários.

O Dashboard administrativo possui:

- visualização de usuários;
- pesquisa de usuários;
- paginação;
- informações de usuários online/presença;
- indicadores administrativos disponíveis pela API.

---

## Navegação

A aplicação possui navegação entre os módulos e controle específico dos botões Voltar e Avançar do navegador.

O `AuthenticationGuard` protege as rotas autenticadas. O `BrowserNavigationGuard` trata a navegação pelos controles do navegador conforme as regras da aplicação.

---

## Disponibilidade e sessão

A aplicação monitora a disponibilidade dos serviços necessários ao funcionamento do sistema.

Quando uma sessão autenticada perde a comunicação necessária com a aplicação, o frontend trata a situação e encerra o acesso conforme as regras da sessão.

---

## Controle de acesso

A autorização é aplicada no backend e não depende somente da interface Angular.

A interface pode ocultar ações que o usuário não possui permissão para executar, mas a API continua sendo a autoridade para autorização.

As operações permitidas para `USER` e `ADMIN` são definidas pelas regras atuais dos endpoints. A proteção também deve abranger rotas antigas ou alternativas que representem a mesma operação.

---

## Funcionalidades da V1

A V1 atualmente contempla:

- autenticação e contas;
- alteração de senha e política/histórico de senhas;
- política e histórico de senhas;
- JWT e autorização;
- controle de sessão;
- múltiplos navegadores e abas;
- User Presence;
- controle de inatividade;
- Dashboard de usuário;
- Dashboard administrativo;
- gerenciamento de usuários;
- gerenciamento de ônibus;
- gerenciamento de motoristas;
- gerenciamento de rotas;
- gerenciamento de passageiros;
- gerenciamento de viagens;
- gerenciamento de manutenções;
- detalhes de viagem;
- relações entre passageiros e viagens;
- controle de permissões;
- navegação da aplicação;
- monitoramento de disponibilidade;
- detecção de conflitos de agenda entre viagens para ônibus e motoristas.

A recuperação de senha permanece como pendência funcional concreta: o pedido público ainda informa que a verificação segura não está implementada e os endpoints/telas não formam um fluxo coerente. A V1 não deve ser declarada totalmente concluída enquanto isso e os testes de navegação por papel não forem resolvidos.

---

## Roadmap da V2

Os recursos abaixo não fazem parte da V1 e devem ser tratados como evolução futura:

- login/autenticação com Google;
- autenticação multifator (MFA/2FA);
- reCAPTCHA;
- adoção de cookies como mecanismo de autenticação/sessão, caso seja escolhida essa arquitetura;
- novas extensões de segurança e autenticação;
- novas extensões dos módulos operacionais conforme necessidade do produto.

Esses itens não devem ser apresentados como funcionalidades faltantes da V1.
