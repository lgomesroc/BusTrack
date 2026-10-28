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
- impedir conflitos de horário para o mesmo ônibus;
- impedir conflitos de horário para o mesmo motorista;
- impedir a alocação de ônibus indisponíveis por manutenção ativa;
- controlar o ciclo de vida da viagem entre `Agendada`, `Em andamento`, `Concluída` e `Cancelada`.

As validações de conflito consideram o intervalo operacional da viagem. A disponibilidade por manutenção considera a data da manutenção enquanto o modelo não possuir um intervalo de horário específico.

---

## Manutenções

O sistema permite registrar e consultar manutenções associadas aos ônibus, incluindo seus estados e custos.

As manutenções ativas são consideradas na validação de disponibilidade do ônibus para novas viagens.

---

## Relatórios operacionais

Administradores podem consultar relatórios por período, utilizando data inicial e final.

A tela de relatórios apresenta:

- resumo de viagens por status;
- utilização de cada ônibus;
- viagens por motorista;
- custos de manutenção por ônibus;
- custo total de manutenção do período;
- mensagens para períodos sem dados;
- estados de carregamento e erro.

Os relatórios são calculados sobre os dados operacionais existentes, sem criar uma estrutura de persistência duplicada para os resultados.

### Exportação dos relatórios

Os relatórios podem ser exportados em formato CSV para uso externo.

A exportação contempla:

- resumo das viagens por status do período;
- utilização dos ônibus;
- viagens por motorista;
- custos de manutenção por ônibus.

Os arquivos são gerados no navegador com o período selecionado no nome do arquivo. A exportação não cria dados adicionais no banco de dados.

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
- regras de conflito entre viagens;
- integração entre manutenção e disponibilidade de ônibus;
- ciclo de vida das viagens;
- detalhes de viagem;
- relações entre passageiros e viagens;
- relatórios operacionais;
- exportação dos relatórios operacionais em CSV;
- controle de permissões;
- navegação da aplicação;
- monitoramento de disponibilidade.

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
