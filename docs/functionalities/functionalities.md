
# Funcionalidades

## Visão geral

O BusTrack é uma aplicação para gerenciamento de transporte coletivo. A V1 reúne autenticação, gerenciamento operacional e recursos administrativos para ônibus, motoristas, rotas, passageiros, viagens e manutenções.

Esta documentação descreve as funcionalidades da aplicação e as regras de negócio implementadas. Recursos planejados para versões futuras não devem ser apresentados como funcionalidades concluídas da V1.

---

## Ônibus

O sistema permite:

- cadastrar ônibus;
- consultar a lista de ônibus;
- consultar um ônibus específico;
- editar ônibus;
- excluir ônibus;
- associar ônibus às viagens;
- associar ônibus às manutenções;
- verificar a disponibilidade do ônibus nos intervalos de operação planejados.

A disponibilidade de um ônibus deve considerar as viagens e as manutenções ativas que utilizam o mesmo veículo.

---

## Motoristas

O sistema permite:

- cadastrar motoristas;
- consultar a lista de motoristas;
- consultar um motorista específico;
- editar motoristas;
- excluir motoristas;
- associar motoristas às viagens;
- utilizar o motorista como recurso para verificar conflitos de horário entre viagens.

Um motorista não deve ser alocado em duas viagens com intervalos de operação sobrepostos.

---

## Rotas

O sistema permite:

- cadastrar rotas;
- consultar a lista de rotas;
- consultar uma rota específica;
- editar rotas;
- excluir rotas;
- utilizar rotas nas viagens.

---

## Passageiros

O sistema permite:

- cadastrar passageiros;
- consultar a lista de passageiros;
- consultar um passageiro específico;
- editar passageiros;
- excluir passageiros;
- associar passageiros às viagens;
- consultar os passageiros relacionados a uma viagem.

A associação entre passageiro e viagem é mantida separadamente do cadastro do passageiro.

---

## Viagens

Uma viagem reúne as informações necessárias para representar uma operação de transporte.

O sistema permite:

- cadastrar viagens;
- consultar a lista de viagens;
- consultar uma viagem específica;
- consultar os detalhes de uma viagem;
- editar viagens conforme as regras de status;
- excluir viagens conforme as permissões aplicáveis;
- associar ônibus;
- associar motorista;
- associar rota;
- associar passageiros;
- informar a data e o horário previstos de saída;
- definir a duração planejada da viagem em minutos;
- registrar o início efetivo da viagem;
- registrar a data e o horário reais de chegada;
- registrar a duração real da viagem;
- registrar a quantidade de passageiros transportados;
- aplicar as regras relacionadas à capacidade de passageiros;
- validar conflitos de horário com outras viagens e manutenções.

### Duração planejada

A duração planejada representa o intervalo estimado de operação da viagem. Ela é utilizada para determinar o horário previsto de término e verificar a disponibilidade dos recursos envolvidos.

A duração planejada deve ser maior que zero e não pode ultrapassar 1.440 minutos.

O intervalo da viagem é calculado a partir do horário previsto de saída e da duração planejada.

### Status das viagens

Os status operacionais são:

- **Agendada:** a viagem está planejada e ainda não começou;
- **Em andamento:** a operação da viagem foi iniciada;
- **Concluída:** a viagem foi encerrada com os dados reais exigidos;
- **Cancelada:** a viagem foi cancelada.

As transições de status obedecem às regras de negócio da aplicação.

Uma viagem agendada pode iniciar no horário previsto de saída ou depois dele. Uma viagem não pode ser iniciada antes do horário previsto.

Para concluir uma viagem, é necessário informar a chegada real, uma quantidade positiva de passageiros e uma duração compatível com o tempo decorrido entre o início e a chegada.

Viagens concluídas ou canceladas não podem ter seus dados alterados pelas operações de atualização de viagem.

---

## Manutenções

O sistema permite gerenciar as manutenções associadas aos ônibus.

As informações de uma manutenção incluem:

- ônibus relacionado;
- tipo de serviço;
- descrição;
- data do serviço;
- horário previsto de início;
- duração planejada em minutos;
- custo;
- status;
- data de conclusão, quando aplicável;
- informações de exclusão lógica, quando aplicável.

### Duração planejada da manutenção

A duração planejada define o intervalo durante o qual o ônibus ficará reservado para a manutenção.

O intervalo é calculado com base na data do serviço, no horário previsto de início e na duração planejada.

A duração deve ser positiva para que o intervalo possa ser utilizado na verificação de conflitos.

### Status das manutenções

Os status operacionais considerados nas verificações de disponibilidade são:

- **Agendada:** a manutenção está planejada;
- **Em andamento:** a manutenção está em execução;
- **Concluída:** a manutenção foi encerrada;
- **Cancelada:** a manutenção foi cancelada.

Manutenções concluídas, canceladas ou excluídas logicamente não devem bloquear novos intervalos como se fossem manutenções ativas.

As operações de criação, consulta, atualização e exclusão devem respeitar as permissões e as regras de negócio aplicáveis ao módulo.

---

## Conflitos entre viagens e manutenções

A disponibilidade do ônibus deve considerar os intervalos de operação das viagens e das manutenções.

### Viagem que conflita com manutenção

Uma viagem não pode ser cadastrada ou reagendada para um intervalo que se sobreponha a uma manutenção ativa do mesmo ônibus.

A verificação considera:

- identificação do ônibus;
- data e horário de saída da viagem;
- duração planejada da viagem;
- data e horário de início da manutenção;
- duração planejada da manutenção;
- status da manutenção;
- exclusão lógica da manutenção.

### Manutenção que conflita com viagem

Uma manutenção também não deve ser agendada ou alterada para um intervalo que se sobreponha a uma viagem ativa do mesmo ônibus.

A verificação deve considerar o intervalo planejado da viagem e o intervalo planejado da manutenção.

### Limites dos intervalos

Dois intervalos são conflitantes quando existe sobreposição entre eles. Um intervalo que termina exatamente quando o outro começa não representa sobreposição por si só.

As validações devem considerar os horários efetivos e as regras de fuso horário da aplicação, evitando comparar apenas as datas e ignorar os horários.

### Conflitos entre viagens

Além das manutenções, a verificação de disponibilidade deve impedir a sobreposição de viagens que utilizem:

- o mesmo ônibus;
- o mesmo motorista.

A validação deve desconsiderar a própria viagem quando uma operação estiver atualizando seus dados e não deve tratar viagens concluídas ou canceladas como operações ativas para essa finalidade.

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
- impedir mais de uma sessão ativa no mesmo navegador, conforme as regras implementadas;
- identificar o navegador associado à sessão;
- controlar a validade da sessão;
- controlar a inatividade;
- detectar a presença do usuário;
- encerrar a sessão quando a comunicação necessária com a aplicação é perdida.

O token de acesso é mantido em `sessionStorage` no frontend.

---

## Dashboard

O Dashboard é o ponto central da área autenticada.

### Usuário comum

O usuário acessa as funcionalidades permitidas pela role `USER`.

### Administrador

O administrador possui acesso ao Dashboard administrativo, incluindo informações administrativas e recursos de gerenciamento e consulta de usuários.

O Dashboard administrativo contempla:

- visualização de usuários;
- pesquisa de usuários;
- paginação;
- informações de usuários online e presença;
- indicadores administrativos disponibilizados pela API.

---

## Navegação

A aplicação possui navegação entre os módulos e tratamento específico dos botões Voltar e Avançar do navegador.

O `AuthenticationGuard` protege as rotas autenticadas. O `BrowserNavigationGuard` trata a navegação pelos controles do navegador conforme as regras da aplicação.

---

## Disponibilidade dos serviços

A aplicação monitora a disponibilidade dos serviços necessários ao funcionamento do sistema.

Quando uma sessão autenticada perde a comunicação necessária com a aplicação, o frontend trata a situação conforme as regras de sessão e disponibilidade.

---

## Controle de acesso

A autorização é aplicada no backend e não depende somente da interface Angular.

A interface pode ocultar ações que o usuário não possui permissão para executar, mas a API continua sendo a autoridade para autorização.

As operações permitidas para `USER` e `ADMIN` são definidas pelas regras atuais dos endpoints. A proteção também deve abranger rotas antigas ou alternativas que representem a mesma operação.

---

## Funcionalidades contempladas na V1

A V1 contempla os módulos e recursos implementados de:

- autenticação e contas;
- recuperação e alteração de senha;
- política e histórico de senhas;
- JWT e autorização;
- controle de sessão;
- múltiplos navegadores e abas;
- presença do usuário;
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
- duração planejada e dados reais das viagens;
- gerenciamento de manutenções;
- duração planejada das manutenções;
- validação de conflitos de horário entre viagens;
- validação de conflitos de horário entre viagens e manutenções;
- controle de permissões;
- navegação da aplicação;
- monitoramento de disponibilidade.

A documentação funcional não substitui a execução de testes. A conclusão técnica de cada funcionalidade deve ser confirmada pelos testes unitários, de integração e pela validação dos fluxos relevantes da aplicação.

---

## Roadmap da V2

Os recursos abaixo são possibilidades de evolução futura e não devem ser considerados funcionalidades concluídas da V1 sem implementação e validação específicas:

- login e autenticação com Google;
- autenticação multifator (MFA/2FA);
- reCAPTCHA;
- adoção de cookies como mecanismo de autenticação e sessão, caso essa arquitetura seja escolhida;
- novas extensões de segurança e autenticação;
- novas extensões dos módulos operacionais conforme a necessidade do produto.
