# Arquitetura do Bus Track

## Visão geral

O Bus Track é uma aplicação composta por backend, frontend, persistência de dados e testes automatizados.

A solução utiliza C# com ASP.NET Core e .NET 10 no backend, Angular no frontend e MongoDB como banco de dados NoSQL.

A arquitetura está organizada em projetos e responsabilidades separadas, permitindo que cada parte do sistema tenha uma função específica.

## Componentes da solução

### BusTrack.API

Responsável pela API REST da aplicação.

Contém:

* Controllers;
* DTOs;
* Models;
* Interfaces de serviços;
* Services;
* Mapeamentos com AutoMapper.

Entre os recursos disponibilizados estão:

* gerenciamento de ônibus;
* gerenciamento de motoristas;
* gerenciamento de passageiros;
* gerenciamento de rotas;
* gerenciamento de viagens;
* relacionamento entre viagens e passageiros;
* gerenciamento de usuários;
* autenticação;
* criação de contas;
* atualização de senhas;
* dashboard.

### BusTrack.DB

Responsável pela persistência e pelas regras relacionadas aos dados.

Contém:

* classes de dados;
* modelos;
* interfaces;
* repositórios;
* conexão com o MongoDB;
* serviços relacionados às regras de persistência e validação.

O projeto utiliza o MongoDB como banco de dados NoSQL.

### BusTrack.Program

Contém o ponto de entrada e configurações auxiliares da aplicação.

Entre suas responsabilidades estão:

* configuração da aplicação;
* registro de dependências;
* configuração do banco de dados;
* extensões;
* middleware de tratamento de erros.

### BusTrack.Frontend

Contém a estrutura responsável pelo frontend da aplicação.

O projeto possui o cliente Angular e o servidor ASP.NET Core utilizado para integração e execução do frontend durante o desenvolvimento.

### bustrack.frontend.client

É a aplicação Angular.

Contém:

* componentes;
* telas;
* serviços;
* modelos;
* regras de validação;
* regras relacionadas à sessão;
* recursos estáticos.

As principais áreas da interface incluem:

* login;
* criação de conta;
* confirmação;
* conclusão;
* atualização de senha;
* dashboard;
* navegação principal.

### BusTrack.Frontend.Server

É o servidor ASP.NET Core responsável pela integração do frontend Angular com a aplicação.

Também utiliza o SPA Proxy durante o desenvolvimento.

### BusTrack.Tests

Projeto separado para os testes automatizados.

Os testes estão organizados em:

* testes unitários;
* testes de integração;
* testes de performance;
* testes de usabilidade.

As principais tecnologias utilizadas nos testes são:

* xUnit;
* Moq;
* Microsoft.NET.Test.Sdk;
* coverlet;
* Microsoft.AspNetCore.TestHost;
* BenchmarkDotNet;
* Selenium WebDriver.

### BusTrack.Updater

Contém componentes auxiliares utilizados para atualização de dados específicos da aplicação.

Atualmente possui componentes relacionados à atualização de dados de motoristas e passageiros.

## Fluxo principal do backend

De forma geral, uma requisição para a API segue o fluxo:

```text
Cliente
   ↓
Controller
   ↓
Service
   ↓
Repository
   ↓
MongoDB
```

Quando necessário, o resultado percorre o caminho inverso:

```text
MongoDB
   ↓
Repository
   ↓
Service
   ↓
Controller
   ↓
Cliente
```

Os DTOs são utilizados na comunicação da API, enquanto os modelos e classes da camada `BusTrack.DB` representam os dados utilizados na persistência.

O AutoMapper é utilizado para realizar o mapeamento entre diferentes representações dos dados.

## Frontend

O frontend utiliza Angular e TypeScript.

A aplicação possui componentes separados por responsabilidade e serviços utilizados para comunicação e regras compartilhadas.

Entre as responsabilidades existentes estão:

* autenticação;
* cadastro;
* atualização de senha;
* dashboard;
* validação de campos;
* controle de sessão;
* regras de interação da interface;
* comunicação com a API.

## Segurança

O backend utiliza:

* JWT para autenticação baseada em tokens;
* BCrypt para hashing de senhas.

O frontend possui regras adicionais relacionadas à sessão e à interface, incluindo:

* temporizador de inatividade;
* temporizador de sessão;
* prevenção de navegação pelo histórico do navegador;
* bloqueio de determinadas interações da interface;
* regras relacionadas ao armazenamento de senhas;
* regras de política de senha;
* validação de força de senha.

A política de senha também possui mecanismo para exigir atualização periódica da senha.

## Banco de dados

O Bus Track utiliza MongoDB como banco de dados NoSQL.

A comunicação com o banco é realizada através do MongoDB Driver.

A camada `BusTrack.DB` concentra a maior parte da responsabilidade relacionada ao acesso e tratamento dos dados.

## Testes

A solução possui diferentes tipos de testes.

### Testes unitários

Verificam partes específicas da aplicação de maneira isolada.

Mocks são utilizados quando necessário para substituir dependências externas.

### Testes de integração

Verificam a comunicação entre diferentes componentes da aplicação e da API.

A infraestrutura de testes utiliza `Microsoft.AspNetCore.TestHost` e WebApplicationFactory.

### Testes de performance

Utilizam BenchmarkDotNet para avaliar o desempenho de determinados serviços.

### Testes de usabilidade

Utilizam Selenium WebDriver para automatizar interações com a interface.

## Organização da solução

A solução é organizada através do arquivo:

```text
BusTrack.sln
```

Os principais projetos são:

```text
BusTrack
├── BusTrack.API
├── BusTrack.DB
├── BusTrack.Program
├── BusTrack.Frontend
├── BusTrack.Tests
└── BusTrack.Updater
```

O projeto frontend possui internamente o cliente Angular e o servidor ASP.NET Core.

## Tecnologias principais

### Backend

* C#
* .NET 10
* ASP.NET Core
* MongoDB
* AutoMapper
* JWT
* BCrypt
* Swagger

### Frontend

* Angular
* TypeScript
* HTML
* CSS

### Testes

* xUnit
* Moq
* BenchmarkDotNet
* Selenium WebDriver
* Microsoft.AspNetCore.TestHost
* coverlet
