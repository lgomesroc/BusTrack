# Configuração geral

## Visão geral

A solução BusTrack é composta por projetos que trabalham em conjunto para formar a aplicação. As configurações técnicas mantêm backend, frontend, banco de dados e testes alinhados.

---

## .NET 10

Os projetos .NET da solução utilizam .NET 10.

A migração de .NET 7 para .NET 10 foi concluída. Os detalhes históricos dessa migração estão documentados em `docs/dotnet-10-migration/dotnet-10-migration.md`.

---

## ImplicitUsings

Os projetos utilizam `ImplicitUsings` para disponibilizar automaticamente namespaces comuns durante a compilação, reduzindo declarações repetitivas de `using`.

---

## Nullable Reference Types

A solução utiliza Nullable Reference Types para tornar explícitos os contratos relacionados a valores que podem ser `null` e permitir que o compilador identifique possíveis problemas de nulabilidade.

---

## Solução

O arquivo `BusTrack.sln` organiza os projetos que compõem a solução.

A configuração de cada projeto permanece específica às suas responsabilidades. O projeto de testes possui dependências próprias, enquanto backend e frontend possuem suas respectivas configurações de execução.

---

## Autenticação e sessão

A aplicação utiliza JWT Bearer para autenticação das áreas protegidas.

A configuração de autenticação inclui:

- issuer;
- audience;
- chave de assinatura;
- validade do token;
- roles `USER` e `ADMIN`;
- controle de sessão por navegador;
- controle de inatividade;
- presença do usuário.

Segredos e chaves devem ser fornecidos por configuração de ambiente e não devem ser armazenados no repositório.

---

## Banco de dados

O BusTrack utiliza MongoDB como banco de dados da aplicação.

A configuração do banco deve ser fornecida de acordo com o ambiente de execução. O ambiente de testes possui configuração própria para evitar dependência de dados da aplicação durante a execução da suíte.

---

## Docker

O projeto possui configuração Docker para execução dos componentes necessários ao ambiente de desenvolvimento e testes.

As configurações específicas de cada ambiente devem ser mantidas fora do código quando envolverem segredos ou credenciais.

---

## Frontend

O frontend utiliza Angular e TypeScript.

As configurações de acesso à API devem apontar para o backend correspondente ao ambiente em execução.

---

## Recursos fora da V1

Google Authentication, MFA/2FA, reCAPTCHA e eventual autenticação baseada em cookies são recursos planejados para a V2. Eles não são requisitos de configuração para a V1 atual.

---

## Resumo

A configuração atual da solução está baseada em .NET 10, ASP.NET Core, Angular, TypeScript, MongoDB, JWT Bearer, Docker e configuração por ambiente.
