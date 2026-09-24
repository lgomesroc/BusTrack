# Histórico de Atualizações

Este documento registra as principais alterações, correções, implementações e evoluções realizadas no Bus Track ao longo do desenvolvimento do projeto.

## 2026-09-24

* Atualização da configuração do projeto para suportar diferentes ambientes de execução utilizando arquivos `environment.ts` e `environment.prod.ts` no frontend Angular.
* Configuração da substituição automática do ambiente de desenvolvimento pelo ambiente de produção durante o build do Angular.
* Centralização da URL da API no objeto `environment`, eliminando URLs fixas da API espalhadas pelo frontend.
* Atualização da configuração do backend para permitir que as configurações do MongoDB sejam fornecidas por variáveis de ambiente, preparando a aplicação para execução em ambientes de produção.
* Criação e configuração do cluster MongoDB Atlas para utilização como banco de dados da aplicação em produção.
* Validação da comunicação da aplicação com o endpoint de Health Check utilizando a configuração de ambiente.
* Investigação da conectividade entre o ambiente local e o MongoDB Atlas, identificando bloqueio de conexão TCP na porta 27017 pela rede corporativa utilizada no ambiente de desenvolvimento.
* Preparação da arquitetura de implantação utilizando Angular, Render, ASP.NET Core, Docker e MongoDB Atlas.
* Definição da utilização de Dockerfile próprio para o backend no Render, mantendo o `docker-compose` destinado ao ambiente local de desenvolvimento.
* Exclusão do `PassengerLimitValidationServiceDB`, que não possuía utilização no código atual após análise das referências existentes.
* Correções e ajustes adicionais nos serviços de motoristas, passageiros e atualização de senha.
* Ajustes nos testes de integração dos serviços de motoristas e passageiros para acompanhar as alterações realizadas na camada de serviços.
* Validação da compilação da solução após as alterações, mantendo **0 erros e 0 warnings** no `dotnet build`.

## 2026-09-23

* Continuação da preparação do Bus Track para execução em ambiente de produção.
* Configuração inicial do MongoDB Atlas como banco de dados externo para o ambiente de produção.
* Configuração do acesso do MongoDB Atlas através de autenticação por usuário e senha.
* Configuração inicial da lista de acesso de endereços IP do MongoDB Atlas para testes de conectividade.
* Implementação da configuração do backend para receber `ConnectionStrings__BusTrackDBConnection` e `ConnectionStrings__DatabaseName` através de variáveis de ambiente.
* Validação do funcionamento do backend utilizando o endpoint `GET /api/Health`.
* Teste de acesso real ao MongoDB Atlas através da aplicação, identificando posteriormente a restrição de conexão da rede corporativa à porta TCP 27017.

## 2026-09-22

* Continuação dos ajustes de configuração do frontend Angular para separar as configurações de desenvolvimento e produção.
* Criação da estrutura de ambientes do frontend em `src/environments`.
* Ajustes na configuração do Angular para utilizar `environment.prod.ts` durante o build de produção.
* Atualização dos serviços Angular para utilizar a URL da API definida no ambiente de execução.

## 2026-09-21

* Continuação da organização estrutural do projeto após a migração para .NET 10.
* Ajustes na configuração da aplicação para preparar a execução local e futura implantação em ambiente externo.
* Criação do endpoint de verificação de disponibilidade da API para utilização pelo monitoramento da aplicação.
* Revisão das regras de disponibilidade da aplicação para impedir a continuidade de uma sessão quando a comunicação com o sistema é perdida.

## 2026-09-20

* Implementação e validação do controle de autenticação das rotas protegidas do frontend Angular.
* Ajustes no gerenciamento da sessão utilizando `sessionStorage`.
* Atualização da navegação entre as áreas autenticadas e públicas da aplicação.
* Ajustes no módulo de Viagens e na integração com os dados relacionados a ônibus, motoristas, rotas e passageiros.
* Correções e melhorias na interface do módulo de Viagens.
* Ajustes nos serviços Angular responsáveis pela comunicação com a API.

## 2026-09-19

* Continuação da implementação do módulo de Ônibus.
* Integração completa das operações de listagem, criação, edição e exclusão de ônibus entre frontend, API e MongoDB.
* Ajustes de responsividade e interface do módulo de Ônibus.
* Correções no serviço Angular responsável pelas operações de ônibus.
* Ajustes no modelo, serviço e repositório de ônibus para manter a integração entre as camadas da aplicação.
* Continuação dos ajustes no módulo de Viagens e na apresentação dos detalhes das viagens.
* Atualização das informações apresentadas nas viagens para incluir os dados relacionados aos registros associados.
* Validação das alterações realizadas no frontend e backend.

## 2026-09-18

* Implementação do `BrowserNavigationGuard` para controlar a navegação pelos botões Voltar e Avançar do navegador.
* Neutralização das regras antigas de bloqueio de navegação para centralizar o controle no Angular Router.
* Implementação do controle de sessão da aplicação utilizando `sessionStorage`.
* Implementação do monitoramento da disponibilidade do frontend e da API.
* Criação do endpoint de Health Check da API em `GET /api/Health`.
* Implementação do módulo de Ônibus no frontend Angular.
* Implementação da integração do módulo de Ônibus com a API para listagem, criação, edição e exclusão de registros.
* Atualização do modelo `BusDB` e dos respectivos mapeamentos para integração entre API e banco de dados.
* Criação do `TripDetailsDTOAPI` para retornar informações completas das viagens.
* Atualização do serviço e do controller de Viagens para retornar dados relacionados ao ônibus, motorista, rota e passageiros, além dos horários, duração e limite de passageiros.
* Atualização da infraestrutura dos testes do serviço de Viagens para acompanhar as novas dependências e responsabilidades introduzidas na implementação.
* Atualização da tela de login para utilizar o controle de sessão da aplicação.
* Ajustes na inicialização do frontend e na estrutura de roteamento Angular.
* Validação da compilação do projeto com **0 erros e 0 warnings**.
* Os testes foram executados e apresentaram falhas em testes existentes após as alterações estruturais, permanecendo pendentes de correção.

## 2026-09-17

* Continuação da modernização do projeto para .NET 10.
* Atualização da estrutura dos serviços, repositórios, DTOs, modelos e interfaces da API.
* Ajustes de nulabilidade e contratos assíncronos para adequação ao .NET 10.
* Atualização das operações de viagens e passageiros associados a viagens utilizando persistência assíncrona com MongoDB.
* Atualização das configurações de mapeamento utilizando AutoMapper.
* Atualização das rotas e da estrutura principal do frontend Angular.
* Implementação da tela de Viagens com listagem, formulário e integração com a API.
* Atualização da navegação do Dashboard para os módulos do sistema.
* Ajustes na infraestrutura dos testes de integração, incluindo a factory utilizada pelos testes.
* Correção de avisos de compilação relacionados a nulabilidade e APIs obsoletas.
* Criação da documentação de arquitetura do projeto em `docs/architecture.md`.
* Criação da documentação da migração para .NET 10 em `docs/dotnet-10-migration.md`.
* Validação da compilação do projeto com **0 erros e 0 warnings**.
* Execução da suíte de testes revelou problemas de compatibilidade na infraestrutura e em alguns testes existentes, que permanecem em correção antes da integração da branch com a `main`.

## 2026-09-16

* Início da refatoração estrutural do Bus Track após a migração para .NET 10.
* Atualização dos contratos e implementações dos serviços de ônibus, motoristas, passageiros, viagens, passageiros de viagens e usuários.
* Atualização dos contratos e implementações dos repositórios utilizados pelos módulos refatorados.
* Atualização dos modelos e DTOs utilizados pela API.
* Adequação das operações de persistência ao uso assíncrono do MongoDB.
* Correções de mapeamento entre DTOs, modelos da API e entidades do banco de dados.
* Ajustes na configuração de injeção de dependências da aplicação.
* Correções de nulabilidade para eliminar warnings durante a compilação.
* Atualização da infraestrutura de testes para acompanhar as mudanças realizadas na aplicação.
* Atualização da estrutura de autenticação e criação de conta, incluindo tratamento de senha com BCrypt.
* Ajustes iniciais na estrutura do Dashboard e na navegação entre os módulos do sistema.
* Validação da compilação após as alterações estruturais.

## 2026-09-11

* Atualização dos projetos .NET da versão 7 para a versão 10.
* Atualização das dependências principais utilizadas pelo backend e pelos testes.
* Atualização do driver do MongoDB para a versão 3.11.1.
* Atualização do AutoMapper para a versão 16.2.0.
* Remoção de dependências que não eram mais necessárias após a atualização para o .NET 10.
* Ajustes nos testes para compatibilidade com a versão atualizada do AutoMapper.

## 2024-06-03

* Lançamento do sistema Bus Track.
* Publicação no GitHub da versão completa do projeto.

## 2024-04-26

* Correção dos erros existentes no projeto.

## 2024-04-21

* Criação da estrutura do frontend utilizando Angular.

## 2024-04-20

* Criação da estrutura de testes de integração.

## 2024-04-17

* Criação da estrutura de testes unitários.

## 2024-04-16

* Criação da estrutura da API.

## 2024-04-15

* Divisão do `Program` em pastas e arquivos menores para melhorar a organização do código.

## 2024-04-11

* Adição da conexão do projeto com o banco de dados Bus Track.

## 2024-04-10

* Adição dos serviços relacionados ao banco de dados na pasta `ServicesDB`.

## 2024-04-07

* Lançamento inicial do projeto no GitHub.
