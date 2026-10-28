# Funcionalidades

## Visão geral

O BusTrack é uma aplicação para gerenciamento de transporte coletivo. A V1 reúne autenticação, gerenciamento operacional e recursos administrativos para ônibus, motoristas, rotas, passageiros e viagens.

A documentação deste arquivo representa o estado atual da aplicação. Funcionalidades antigas que eram apenas planejadas não devem ser tratadas como pendências da V1.

---

## Listas operacionais

As principais listas operacionais do frontend possuem filtros e paginação para evitar tabelas extensas e facilitar a consulta conforme a quantidade de registros cresce.

A V1 contempla:

- filtro e paginação de ônibus;
- filtro e paginação de motoristas;
- filtro e paginação de passageiros;
- filtro por status e paginação de viagens;
- filtro e paginação de manutenções;
- página com até 10 registros por vez;
- retorno à primeira página quando um novo filtro é aplicado;
- mensagens específicas quando o filtro não encontra registros.

A paginação atual é aplicada no frontend sobre os dados retornados pela API. Ela melhora a usabilidade das telas e evita a renderização de listas completas, mas não substitui paginação no backend para volumes muito grandes.

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
- aplicar as regras relacionadas à capacidade de passageiros.

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
- detalhes de viagem;
- relações entre passageiros e viagens;
- controle de permissões;
- navegação da aplicação;
- monitoramento de disponibilidade;
- filtros e paginação das listas operacionais.

Não há, no estado atual da documentação, uma funcionalidade principal de negócio conhecida como pendência obrigatória para concluir a V1.

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
