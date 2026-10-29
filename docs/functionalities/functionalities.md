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
- consultar os detalhes da viagem;
- editar viagens conforme as regras de status;
- excluir viagens conforme as permissões aplicáveis;
- associar ônibus;
- associar motorista;
- associar rota;
- associar passageiros;
- aplicar as regras relacionadas à capacidade de passageiros;
- informar a data e o horário previstos de saída;
- definir a duração planejada da viagem em minutos;
- registrar o início efetivo da viagem;
- registrar a data e o horário reais de chegada;
- registrar a duração real da viagem;
- registrar a quantidade de passageiros transportados;
- impedir conflitos de horário para o mesmo ônibus;
- impedir conflitos de horário para o mesmo motorista;
- impedir a alocação de ônibus indisponíveis por manutenção ativa;
- validar conflitos de horário com outras viagens e manutenções;
- controlar o ciclo de vida da viagem entre `Agendada`, `Em andamento`, `Concluída` e `Cancelada`.

### Duração planejada

A duração planejada representa o intervalo estimado de operação da viagem. Ela é utilizada para determinar o horário previsto de término e verificar a disponibilidade dos recursos envolvidos.

A duração planejada deve ser maior que zero e não pode ultrapassar 1.440 minutos.

O intervalo da viagem é calculado a partir do horário previsto de saída e da duração planejada. As validações de conflito consideram o intervalo operacional da viagem.

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

A duração planejada define o intervalo durante o qual o ônibus ficará reservado para a manutenção. O intervalo é calculado com base na data do serviço, no horário previsto de início e na duração planejada.

A duração deve ser positiva para que o intervalo possa ser utilizado na verificação de conflitos.

### Status das manutenções

Os status operacionais considerados nas verificações de disponibilidade são:

- **Agendada:** a manutenção está planejada;
- **Em andamento:** a manutenção está em execução;
- **Concluída:** a manutenção foi encerrada;
- **Cancelada:** a manutenção foi cancelada.

Manutenções concluídas, canceladas ou excluídas logicamente não devem bloquear novos intervalos como se fossem manutenções ativas.

As operações de criação, consulta, atualização e exclusão devem respeitar as permissões e as regras de negócio aplicáveis ao módulo. O sistema também permite registrar custos e consultar o histórico de auditoria das alterações de manutenção.

---

## Relatórios operacionais

O administrador pode consultar um relatório consolidado informando uma data inicial e uma data final.

O relatório apresenta:

- quantidade total de viagens no período;
- viagens concluídas;
- viagens canceladas;
- viagens agendadas;
- viagens em andamento;
- utilização dos ônibus, com quantidade de viagens e viagens concluídas por ônibus;
- quantidade de viagens por motorista e quantidade de viagens concluídas;
- custo total das manutenções no período;
- custo total e quantidade de manutenções por ônibus.

A consulta é disponibilizada pelo endpoint `GET /api/OperationalReports?startDate=YYYY-MM-DD&endDate=YYYY-MM-DD` e exige a role `ADMIN`.

O período é inclusivo: a data inicial e a data final fazem parte da consulta. As viagens são consideradas pela data/hora de saída e as manutenções pela data do serviço.

### Exportação dos relatórios

Os relatórios podem ser exportados em formato CSV para uso externo. A exportação contempla:

- resumo das viagens por status do período;
- utilização dos ônibus;
- viagens por motorista;
- custos de manutenção por ônibus.

Os arquivos são gerados no navegador com o período selecionado no nome do arquivo. O CSV utiliza BOM UTF-8 para preservar a acentuação em programas como o Excel e escapa vírgulas, aspas e quebras de linha.

Os botões de exportação ficam desabilitados quando a seção correspondente não possui dados. A exportação não cria dados adicionais no banco de dados.

Os relatórios são calculados a partir dos dados operacionais existentes. Não há armazenamento separado de dados de relatório.

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
- regras de conflito entre viagens;
- integração entre manutenção e disponibilidade de ônibus;
- ciclo de vida e transições de status das viagens;
- detalhes de viagem;
- relações entre passageiros e viagens;
- duração planejada e dados reais das viagens;
- controle de conflitos de recursos entre viagens;
- gerenciamento de manutenções;
- duração planejada das manutenções;
- validação de conflitos de horário entre viagens;
- validação de conflitos de horário entre viagens e manutenções;
- relatórios operacionais;
- exportação dos relatórios operacionais em CSV;
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
