# Tecnologias do Bus Track

## Visão geral

O Bus Track foi desenvolvido utilizando tecnologias diferentes para atender às necessidades de backend, frontend, persistência de dados, autenticação, testes, documentação e implantação.

A escolha das tecnologias não foi feita apenas para reunir diferentes ferramentas em um mesmo projeto. Cada tecnologia possui uma finalidade dentro da solução e foi escolhida considerando aspectos como produtividade, organização do código, facilidade de manutenção, capacidade de evolução e adequação ao tipo de aplicação desenvolvido.

Algumas tecnologias já estão efetivamente utilizadas na aplicação.

Outras fazem parte da evolução planejada do projeto e ainda não foram implementadas completamente. Quando isso ocorrer, essa documentação identifica explicitamente a situação para evitar que uma tecnologia planejada seja confundida com uma funcionalidade já existente.

---

# Backend

## C#

C# é a principal linguagem utilizada no backend do Bus Track.

A escolha do C# está relacionada principalmente ao ecossistema .NET e à possibilidade de construir uma API estruturada, tipada e adequada para aplicações de backend.

A tipagem estática permite detectar diversos problemas durante a compilação, antes que determinadas situações cheguem ao ambiente de execução.

A linguagem também fornece recursos importantes para o projeto, como:

* Programação orientada a objetos;
* Interfaces;
* Generics;
* LINQ;
* Programação assíncrona;
* Tratamento de exceções;
* Injeção de dependência através do ecossistema .NET;
* Recursos modernos da linguagem.

O C# também permite que o projeto utilize uma única linguagem principal no backend, mantendo uma estrutura consistente entre Controllers, Services, Repositories, modelos e demais componentes.

### Por que C# foi utilizado?

A escolha também está relacionada ao objetivo do Bus Track como projeto de portfólio.

C# e .NET são amplamente utilizados no desenvolvimento de APIs e sistemas corporativos, permitindo que o projeto demonstre conhecimentos relacionados a uma stack utilizada profissionalmente.

O Bus Track também foi utilizado como oportunidade para trabalhar conceitos importantes do desenvolvimento backend, como separação de responsabilidades, interfaces, injeção de dependência, acesso a dados, testes automatizados e APIs REST.

---

# .NET 10

O projeto utiliza .NET 10 como plataforma principal do backend.

A adoção do .NET 10 faz parte da evolução do projeto a partir de uma versão anterior do .NET.

A atualização permite trabalhar com uma versão moderna da plataforma e manter o projeto alinhado com uma geração atual do ecossistema .NET.

Entre os benefícios relevantes para o projeto estão:

* Runtime moderno;
* APIs atualizadas;
* Melhorias de desempenho;
* Melhorias de produtividade;
* Suporte a recursos atuais do ecossistema;
* Integração com ASP.NET Core;
* Suporte à programação assíncrona;
* Injeção de dependência integrada.

A migração para .NET 10 também faz parte da própria evolução técnica do Bus Track.

A documentação específica dessa migração está disponível em:

[Documentação da migração para .NET 10](../dotnet-10-migration.md)

---

# ASP.NET Core

ASP.NET Core é utilizado para construir a API do Bus Track.

A tecnologia fornece a infraestrutura necessária para receber requisições HTTP, executar Controllers, configurar middleware, registrar serviços e disponibilizar endpoints.

A escolha do ASP.NET Core está diretamente relacionada à escolha do C# e do .NET.

O framework fornece recursos necessários para construir uma API moderna sem que o projeto precise implementar manualmente toda a infraestrutura de comunicação HTTP.

Entre os recursos utilizados ou previstos estão:

* Controllers;
* Injeção de dependência;
* Middleware;
* Configuração por ambiente;
* APIs REST;
* Tratamento de exceções;
* CORS;
* Swagger;
* Integração com testes;
* Autenticação e autorização.

---

# ASP.NET Core Web API

A aplicação utiliza o modelo de Web API do ASP.NET Core para disponibilizar seus recursos.

Essa abordagem permite separar a interface do usuário do processamento realizado pelo backend.

O frontend não precisa conhecer os detalhes internos da aplicação.

Ele realiza requisições para endpoints da API, e o backend processa essas operações.

Essa separação também permite que o backend possa ser consumido por outros clientes no futuro, além do frontend Angular atual.

---

# REST e HTTP

A comunicação entre o frontend e o backend utiliza requisições HTTP através da API.

Os recursos da aplicação são expostos através de endpoints e métodos HTTP apropriados.

Essa abordagem permite representar operações como:

* Consulta de dados;
* Criação de registros;
* Atualização;
* Exclusão;
* Autenticação;
* Verificações de disponibilidade.

A utilização de uma API baseada em HTTP também facilita a separação entre frontend e backend.

---

# MongoDB

MongoDB é o banco de dados utilizado pelo Bus Track.

A escolha do MongoDB está relacionada ao modelo de documentos e à flexibilidade oferecida pelo banco para representar os dados da aplicação.

O projeto trabalha com entidades relacionadas a:

* Usuários;
* Ônibus;
* Motoristas;
* Rotas;
* Passageiros;
* Viagens;
* Relações entre viagens e passageiros.

O modelo orientado a documentos é adequado para uma aplicação que trabalha com diferentes tipos de entidades e estruturas de dados que podem evoluir durante o desenvolvimento.

Outro fator importante é a integração do MongoDB com o ecossistema .NET através do driver oficial.

---

# MongoDB.Driver

`MongoDB.Driver` é o driver utilizado pelo backend para comunicação com o MongoDB.

Ele permite que a aplicação:

* Estabeleça conexão com o MongoDB;
* Acesse bancos de dados;
* Acesse collections;
* Execute consultas;
* Insira documentos;
* Atualize documentos;
* Remova documentos;
* Trabalhe de forma assíncrona com as operações de banco.

O driver é utilizado principalmente pela camada de persistência da aplicação.

---

# MongoDB.Bson

`MongoDB.Bson` fornece os recursos relacionados ao formato BSON utilizado internamente pelo MongoDB.

O BSON é a representação binária de documentos utilizada pelo MongoDB.

A utilização dessa biblioteca permite que o projeto trabalhe corretamente com os tipos e estruturas utilizados pelo banco.

---

# AutoMapper

AutoMapper é utilizado para realizar conversões entre diferentes representações dos dados.

No Bus Track existem diferentes estruturas utilizadas em partes diferentes da aplicação.

Por exemplo:

* DTOs utilizados pela API;
* Models utilizados pela aplicação;
* Classes utilizadas pela persistência.

O AutoMapper reduz a necessidade de escrever manualmente diversas conversões entre essas estruturas.

Isso é especialmente útil em uma aplicação que possui diversos módulos e entidades.

### Por que não utilizar diretamente as classes do banco na API?

Porque isso aumentaria o acoplamento entre a API e a persistência.

Separar essas representações permite que a estrutura interna do banco seja modificada sem necessariamente alterar o contrato exposto pela API.

---

# BCrypt.Net-Next

`BCrypt.Net-Next` é utilizado para realizar o hashing das senhas.

As senhas não devem ser armazenadas em texto puro.

O sistema utiliza BCrypt para transformar a senha original em um hash que pode ser armazenado no banco.

Durante a autenticação, a senha informada pelo usuário é comparada com o hash armazenado utilizando o mecanismo de verificação do BCrypt.

O projeto também utiliza esse mecanismo no controle do histórico de senhas.

### Por que BCrypt?

BCrypt foi escolhido por ser um algoritmo desenvolvido especificamente para hashing de senhas e por possuir um mecanismo de custo computacional que dificulta ataques baseados em tentativa massiva de senhas.

---

# JWT

JWT significa JSON Web Token.

A utilização de JWT faz parte da evolução planejada para a autenticação do Bus Track.

**No estado atual da aplicação, a autenticação baseada em JWT ainda não está implementada efetivamente.**

A presença de configurações relacionadas ao JWT no projeto não significa que o backend já esteja emitindo e validando tokens JWT.

A implementação planejada deverá utilizar JWT para representar a autenticação do usuário através de tokens.

A intenção é evoluir o mecanismo atual de sessão para uma solução de autenticação baseada em tokens adequada para uma API.

Quando implementado, o mecanismo deverá envolver:

* Geração de tokens;
* Validação de tokens;
* Claims;
* Expiração;
* Issuer;
* Audience;
* Chave de assinatura;
* Autenticação das requisições;
* Autorização de recursos protegidos.

Portanto, JWT é uma **tecnologia planejada**, e não uma funcionalidade atualmente concluída.

---

# Swagger / OpenAPI

O Bus Track utiliza Swagger através do `Swashbuckle.AspNetCore`.

Swagger fornece uma interface para visualizar e testar os endpoints disponibilizados pela API.

Isso facilita:

* Desenvolvimento;
* Testes manuais;
* Inspeção dos endpoints;
* Consulta dos parâmetros;
* Consulta das respostas;
* Entendimento da API por outros desenvolvedores.

A documentação da API é especialmente útil durante o desenvolvimento porque permite testar os endpoints sem depender exclusivamente do frontend.

---

# Newtonsoft.Json

`Newtonsoft.Json` é uma biblioteca utilizada para trabalhar com serialização e desserialização JSON.

JSON é utilizado como formato de comunicação entre o frontend e a API.

A biblioteca permite converter objetos para JSON e JSON para objetos quando necessário.

O projeto utiliza o ecossistema moderno do ASP.NET Core, que possui recursos próprios para JSON, portanto a utilização do Newtonsoft.Json deve ser entendida como uma dependência específica da solução, e não como requisito obrigatório para toda comunicação JSON do sistema.

---

# System.Linq.Async

`System.Linq.Async` fornece recursos para realizar operações LINQ sobre sequências assíncronas.

O projeto trabalha com operações assíncronas principalmente no acesso aos dados.

A utilização dessa biblioteca permite trabalhar com operações LINQ em determinados cenários envolvendo sequências assíncronas.

Ela complementa os recursos LINQ existentes no .NET quando o processamento envolve fontes assíncronas.

---

# Angular

Angular é utilizado para construir o frontend do Bus Track.

A escolha do Angular está relacionada à necessidade de construir uma aplicação frontend estruturada, baseada em componentes e adequada para uma aplicação com diversas telas e funcionalidades.

O framework permite organizar a interface em:

* Componentes;
* Services;
* Rotas;
* Guards;
* Modelos;
* Formulários;
* Recursos compartilhados.

Essa estrutura é especialmente útil para o Bus Track porque a aplicação possui diferentes áreas, como autenticação, dashboard, ônibus, viagens e outras funcionalidades.

---

# TypeScript

TypeScript é a linguagem utilizada no desenvolvimento do cliente Angular.

A escolha do TypeScript permite adicionar tipagem estática ao desenvolvimento frontend.

Isso é importante em uma aplicação que possui diferentes modelos de dados, serviços e componentes.

A tipagem permite representar estruturas como:

* Dados de ônibus;
* Dados de passageiros;
* Dados de viagens;
* Respostas da API;
* Estados utilizados pelos componentes.

Além disso, TypeScript facilita a manutenção do código à medida que a aplicação cresce.

---

# HTML

HTML é utilizado para estruturar as páginas e componentes apresentados ao usuário.

No Angular, os templates dos componentes utilizam HTML para definir a estrutura visual da interface.

Ele é responsável pela estrutura dos elementos, enquanto o CSS é utilizado para definir sua apresentação.

---

# CSS

CSS é utilizado para controlar a apresentação visual da aplicação.

No Bus Track, CSS é utilizado para definir aspectos como:

* Layout;
* Espaçamento;
* Tipografia;
* Responsividade;
* Formatação dos componentes;
* Aparência das telas.

A separação entre HTML e CSS permite manter a estrutura e a apresentação da interface organizadas.

---

# ASP.NET Core SPA Proxy

`Microsoft.AspNetCore.SpaProxy` é utilizado na integração do frontend Angular com o ambiente de desenvolvimento ASP.NET Core.

O SPA Proxy facilita a execução conjunta do backend e do frontend durante o desenvolvimento.

Ele permite que a aplicação ASP.NET Core encaminhe a execução para o servidor de desenvolvimento do frontend quando necessário.

Essa tecnologia está relacionada principalmente ao ambiente de desenvolvimento e à integração entre as duas partes da aplicação.

---

# xUnit

xUnit é utilizado como framework principal para os testes automatizados do projeto.

Ele fornece a estrutura necessária para definir e executar testes.

A utilização de um framework de testes separado permite verificar automaticamente comportamentos da aplicação e reduzir a dependência de testes exclusivamente manuais.

---

# Moq

Moq é utilizado para criação de mocks nos testes.

Mocks permitem substituir dependências reais por objetos controlados durante a execução dos testes.

Isso é particularmente importante nos testes unitários.

Por exemplo, um Service que depende de um Repository pode ser testado utilizando um mock do Repository em vez de realizar uma operação real no banco de dados.

---

# Microsoft.NET.Test.Sdk

`Microsoft.NET.Test.Sdk` fornece a infraestrutura necessária para execução dos testes automatizados no ecossistema .NET.

Ele trabalha juntamente com o framework de testes para permitir que os testes sejam descobertos e executados pelas ferramentas disponíveis.

---

# coverlet.collector

Coverlet é utilizado para coleta de informações relacionadas à cobertura dos testes.

A cobertura permite analisar quais partes do código foram exercitadas pelos testes.

Ela não determina sozinha a qualidade dos testes, mas fornece uma métrica adicional para identificar partes do código que podem não estar sendo testadas.

---

# Microsoft.AspNetCore.TestHost

`Microsoft.AspNetCore.TestHost` é utilizado para testes que precisam executar componentes do ASP.NET Core em um ambiente controlado de teste.

Ele é especialmente útil nos testes de integração da API.

Isso permite testar comportamentos envolvendo a aplicação sem necessariamente depender de uma implantação externa do backend.

---

# WebApplicationFactory

`WebApplicationFactory` é utilizado em conjunto com a infraestrutura de testes do ASP.NET Core.

Ele permite criar uma instância da aplicação para utilização durante os testes de integração.

Isso possibilita testar endpoints e comportamentos da aplicação em um ambiente controlado.

---

# BenchmarkDotNet

BenchmarkDotNet é utilizado para testes e medições de performance.

Diferentemente dos testes funcionais, que verificam se determinado comportamento está correto, benchmarks procuram medir aspectos relacionados ao desempenho de determinadas operações.

O uso do BenchmarkDotNet permite comparar operações de forma mais controlada e obter métricas relacionadas à execução.

---

# Selenium WebDriver

Selenium WebDriver é utilizado para automação de testes de interface.

Ele permite simular interações com o navegador, como:

* Acesso a páginas;
* Preenchimento de campos;
* Cliques;
* Navegação;
* Verificação de elementos.

Isso permite testar comportamentos que envolvem a interface completa da aplicação.

---

# Docker

Docker faz parte da estratégia de execução e implantação do Bus Track.

A utilização de containers permite empacotar a aplicação juntamente com o ambiente necessário para sua execução.

No ambiente local, o projeto possui configuração através de Docker Compose.

Para a implantação do backend no Render, a estratégia planejada utiliza um Dockerfile específico para construir a imagem da API.

### Por que Docker?

Docker reduz a dependência do ambiente específico onde a aplicação está sendo executada.

Em vez de depender exclusivamente da configuração manual de uma máquina, a aplicação pode ser executada a partir de uma imagem contendo o ambiente necessário.

Isso também facilita a implantação em plataformas que suportam containers.

---

# Docker Compose

Docker Compose é utilizado para organizar a execução de múltiplos componentes no ambiente local.

Ele permite definir os serviços necessários para executar a aplicação e suas dependências.

É importante diferenciar Docker Compose do Dockerfile.

O Dockerfile define como uma imagem é construída.

O Docker Compose define como diferentes serviços podem ser executados em conjunto.

### Utilização no Render

O Docker Compose não é o mecanismo planejado para executar continuamente a aplicação no Render.

No ambiente de produção, a API deverá possuir uma imagem própria construída a partir de seu Dockerfile.

O Render executa essa imagem como um Web Service.

Portanto, o projeto não precisa reconstruir o `docker-compose.yml` toda vez que o serviço acordar após um período de inatividade.

---

# MongoDB Atlas

MongoDB Atlas é o serviço de banco de dados em nuvem utilizado como destino de produção do Bus Track.

O MongoDB continua sendo o banco de dados utilizado pela aplicação, enquanto o Atlas fornece a infraestrutura gerenciada para executar esse banco na nuvem.

A utilização do Atlas permite separar o banco de produção do MongoDB utilizado localmente durante o desenvolvimento.

A estratégia planejada é:

* MongoDB local para desenvolvimento;
* MongoDB Atlas para produção.

Essa separação evita que a aplicação publicada dependa de um banco executando na máquina de desenvolvimento.

---

# Render

Render é a plataforma planejada para hospedagem da aplicação.

A arquitetura de implantação utiliza serviços separados para frontend e backend.

O backend ASP.NET Core será executado como Web Service utilizando container.

O frontend Angular será disponibilizado como Static Site.

O MongoDB ficará hospedado separadamente no MongoDB Atlas.

Essa estrutura permite manter cada parte da aplicação em seu ambiente apropriado.

---

# Deploy do backend

O backend será empacotado através de Docker.

O Dockerfile será responsável por construir a imagem da aplicação .NET.

Essa imagem será utilizada pelo Render para executar o Web Service.

As configurações específicas de produção, como a connection string do MongoDB e outras informações sensíveis, deverão ser fornecidas através de variáveis de ambiente.

As informações sensíveis não devem ser gravadas diretamente no código-fonte.

---

# Deploy do frontend

O frontend Angular será compilado utilizando a configuração de produção.

Durante essa compilação, o Angular utilizará o arquivo de ambiente correspondente à produção.

A URL da API será configurada para apontar para o backend publicado no Render.

O frontend será disponibilizado como Static Site.

---

# Variáveis de ambiente

As variáveis de ambiente são utilizadas para separar configurações do ambiente de desenvolvimento das configurações utilizadas em produção.

Entre as configurações que podem ser fornecidas dessa maneira estão:

* Connection string do MongoDB;
* Nome do banco;
* URL da API;
* Configurações relacionadas à autenticação;
* Chaves e segredos;
* Configurações específicas do ambiente.

Essa abordagem evita colocar credenciais e configurações específicas de produção diretamente no código.

---

# Git

Git é utilizado para controle de versão do projeto.

O versionamento permite acompanhar as alterações realizadas no código e manter um histórico das modificações.

O fluxo utilizado no projeto também considera branches para desenvolvimento de funcionalidades e alterações específicas.

A branch principal protegida representa a versão estável do projeto, enquanto alterações são desenvolvidas em branches próprias antes de serem integradas.

---

# GitHub

GitHub é utilizado como plataforma de hospedagem do código-fonte e como parte do fluxo de versionamento do projeto.

Além de armazenar o código, o repositório permite:

* Histórico de alterações;
* Branches;
* Pull Requests;
* Organização do projeto;
* Documentação;
* Visibilidade do portfólio.

O GitHub também funciona como parte importante da apresentação do Bus Track como projeto de portfólio.

---

# Estrutura tecnológica da solução

As tecnologias utilizadas no projeto possuem responsabilidades diferentes.

O C# e o .NET formam a base do backend.

O ASP.NET Core fornece a infraestrutura para a API.

O MongoDB fornece a persistência dos dados.

O Angular e o TypeScript formam a base do frontend.

As ferramentas de testes permitem verificar diferentes aspectos da aplicação.

Docker fornece uma forma padronizada de empacotar e executar o backend.

MongoDB Atlas fornece a infraestrutura de banco de dados em produção.

Render fornece a infraestrutura planejada para publicação do backend e frontend.

Git e GitHub controlam e armazenam a evolução do código.

---

# Tecnologias implementadas e tecnologias planejadas

Nem todas as tecnologias mencionadas neste documento representam funcionalidades já concluídas.

### Atualmente utilizadas

Entre as tecnologias efetivamente utilizadas no estado atual do projeto estão:

* C#;
* .NET 10;
* ASP.NET Core;
* Angular;
* TypeScript;
* HTML;
* CSS;
* MongoDB;
* MongoDB.Driver;
* MongoDB.Bson;
* AutoMapper;
* BCrypt.Net-Next;
* Swagger;
* xUnit;
* Moq;
* Microsoft.NET.Test.Sdk;
* coverlet;
* Microsoft.AspNetCore.TestHost;
* BenchmarkDotNet;
* Selenium WebDriver.

### Planejadas ou em evolução

Algumas tecnologias fazem parte da evolução do projeto ou da estratégia de produção:

* JWT para autenticação baseada em tokens;
* Docker para empacotamento do backend em produção;
* Docker Compose para organização do ambiente local;
* MongoDB Atlas para banco de produção;
* Render para hospedagem;
* Configuração completa de produção através de variáveis de ambiente.

Essa distinção existe para que a documentação do projeto seja tecnicamente correta.

Uma tecnologia pode fazer parte da arquitetura planejada sem que sua implementação já esteja concluída.

---

# Critérios utilizados nas escolhas

As escolhas tecnológicas do Bus Track procuram equilibrar diferentes necessidades.

## Produtividade

As ferramentas escolhidas permitem desenvolver funcionalidades sem precisar implementar infraestrutura básica manualmente.

## Organização

A combinação entre projetos separados, interfaces, Services, Repositories e componentes permite manter responsabilidades distribuídas.

## Tipagem

C# e TypeScript fornecem tipagem estática tanto no backend quanto no frontend.

Isso ajuda a detectar inconsistências durante o desenvolvimento.

## Testabilidade

O uso de interfaces, injeção de dependência, mocks e diferentes tipos de testes permite verificar partes específicas da aplicação.

## Evolução

As tecnologias escolhidas permitem que a aplicação seja expandida posteriormente com novos módulos e mecanismos.

## Portabilidade

Docker e a separação entre aplicação e infraestrutura permitem executar o backend em diferentes ambientes sem depender exclusivamente da configuração manual de uma máquina específica.

## Adequação ao projeto

As tecnologias foram escolhidas considerando o tipo de aplicação desenvolvida: uma aplicação web com API, frontend separado, persistência de dados, autenticação, testes automatizados e implantação em nuvem.

---

# Resumo das escolhas

A escolha do C# e do .NET fornece a base para o desenvolvimento do backend.

O ASP.NET Core fornece a infraestrutura necessária para construir a API.

O MongoDB fornece a persistência orientada a documentos.

Angular e TypeScript fornecem uma estrutura organizada para o frontend.

AutoMapper reduz o código necessário para conversão entre diferentes representações dos dados.

BCrypt protege as senhas através de hashing.

Swagger facilita a documentação e exploração da API.

xUnit, Moq, TestHost, Coverlet, BenchmarkDotNet e Selenium permitem testar diferentes aspectos da aplicação.

Docker e Docker Compose fazem parte da estratégia de execução e implantação.

MongoDB Atlas representa a infraestrutura planejada para o banco em produção.

Render representa a plataforma planejada para hospedagem da aplicação.

Git e GitHub sustentam o controle de versão e a evolução do projeto.
