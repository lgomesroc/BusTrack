# Funcionalidades

## Visão geral

O BusTrack é uma aplicação para gerenciamento de transporte coletivo. A V1 reúne autenticação, gerenciamento operacional e recursos administrativos para ônibus, motoristas, rotas, passageiros e viagens.

A documentação deste arquivo representa o estado atual da aplicação. Funcionalidades antigas que eram apenas planejadas não devem ser tratadas como pendências da V1.

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
- controlar o ciclo de vida entre `Agendada`, `Em andamento`, `Concluída` e `Cancelada`;
- impedir sobreposição de viagens para o mesmo ônibus ou motorista;
- impedir a alocação de ônibus indisponível por manutenção ativa;
- registrar auditoria das operações importantes.

### Ciclo de vida

As transições permitidas são:

```text
Agendada
  ├──> Em andamento ───> Concluída
  │          │
  └──────────┴─────────> Cancelada
```

Viagens concluídas ou canceladas não podem ser alteradas.

### Conflitos de recursos

O backend verifica a sobreposição do intervalo operacional das viagens antes de permitir a alocação do mesmo ônibus ou motorista em outra viagem.

### Disponibilidade por manutenção

Uma viagem não pode alocar um ônibus que possua manutenção `Agendada` ou `Em andamento` na data correspondente. Manutenções concluídas ou canceladas não mantêm o bloqueio.

---

## Auditoria de operações

A V1 mantém histórico das operações administrativas importantes de viagens e manutenções.

Para viagens, são registrados:

- criação;
- alteração de dados;
- mudança de status;
- cancelamento;
- exclusão;
- usuário responsável;
- data e hora UTC da operação;
- snapshot do registro após a operação;
- campos que foram alterados, com valor anterior e novo valor quando aplicável.

Para manutenções, a V1 também registra criação, alteração e exclusão, incluindo o responsável, data/hora, justificativa e alterações realizadas.

O histórico de auditoria de viagens é acessível somente a administradores pelo endpoint específico de auditoria. O histórico de manutenção segue a mesma regra administrativa.

As informações de auditoria são persistidas separadamente dos documentos operacionais, preservando o histórico mesmo quando o registro principal sofre alterações posteriores.

---

## Manutenções

O sistema permite:

- cadastrar manutenções;
- consultar manutenções;
- editar manutenções conforme o estado;
- cancelar/excluir conforme as regras da aplicação;
- associar manutenção a um ônibus;
- controlar os estados `Agendada`, `Em andamento`, `Concluída` e `Cancelada`;
- controlar datas e custos;
- consultar o histórico de auditoria da manutenção.

---

## Relatórios operacionais

A V1 possui relatórios operacionais baseados nos dados existentes.

A consulta permite analisar:

- viagens por período;
- resumo de viagens por status;
- utilização dos ônibus;
- viagens por motorista;
- quantidade de manutenções por ônibus;
- custos de manutenção por ônibus;
- custo total de manutenção do período.

A interface Angular possui filtros por data, tabelas para os agrupamentos e mensagens para períodos sem dados.

Os relatórios podem ser exportados inicialmente em CSV, respeitando o período selecionado e os dados disponíveis na consulta.

---

## Contas e autenticação

A V1 possui os seguintes recursos:

- criação de conta;
- login;
- logout;
- alteração de senha;
- recuperação de senha;
- redefinição de senha;
- histórico de senhas;
- política de senhas;
- autenticação por JWT;
- autorização por roles `USER` e `ADMIN`;
- bootstrap controlado do primeiro administrador.

O cadastro público cria contas com role `USER`. A role não é definida pelo cliente.

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

O Dashboard é o ponto central da área autenticada.

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

## Paginação e filtros operacionais

As principais listas operacionais possuem filtro e paginação no frontend:

- ônibus: número, placa e modelo;
- motoristas: nome e CPF;
- passageiros: nome, CPF, e-mail e telefone;
- viagens: status;
- manutenções: serviço, status e ônibus.

A interface apresenta dez registros por página e reinicia a paginação quando o filtro é alterado. A paginação atual é client-side sobre os dados retornados pela API.

---

## Funcionalidades da V1

A V1 atualmente contempla:

- autenticação e contas;
- recuperação e alteração de senha;
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
- ciclo de vida das viagens;
- detecção de conflitos de ônibus e motoristas;
- integração entre manutenção e disponibilidade dos ônibus;
- gerenciamento de manutenções;
- auditoria de operações importantes;
- relatórios operacionais;
- interface dos relatórios;
- exportação dos relatórios em CSV;
- filtros e paginação das listas operacionais;
- detalhes de viagem;
- relações entre passageiros e viagens;
- controle de permissões;
- navegação da aplicação;
- monitoramento de disponibilidade.

O foco da V1 é oferecer um fluxo operacional completo e rastreável, evitando ampliar o escopo com módulos que não sejam necessários para o objetivo principal do sistema.

---

## Roadmap da V2

Os recursos abaixo não fazem parte da V1 e devem ser tratados como evolução futura:

- login/autenticação com Google;
- autenticação multifator (MFA/2FA);
- reCAPTCHA;
- adoção de cookies como mecanismo de autenticação/sessão, caso seja escolhida essa arquitetura;
- paginação server-side para volumes muito grandes;
- novas extensões de segurança e autenticação;
- novas extensões dos módulos operacionais conforme necessidade do produto.

Esses itens não devem ser apresentados como funcionalidades faltantes da V1.
