# Arquitetura do Bus Track

## Visão geral

O Bus Track é uma aplicação organizada em diferentes projetos, cada um responsável por uma parte específica da solução.

A separação permite dividir as responsabilidades do sistema entre processamento das regras da aplicação, acesso e persistência dos dados, configuração e inicialização, interface do usuário, testes automatizados e componentes auxiliares.

A solução é composta principalmente pelos projetos:

* `BusTrack.API`;
* `BusTrack.DB`;
* `BusTrack.Program`;
* `BusTrack.Frontend`;
* `BusTrack.Tests`;
* `BusTrack.Updater`.

O objetivo dessa organização é evitar que todas as responsabilidades da aplicação fiquem concentradas em um único projeto.

Cada projeto possui uma finalidade específica e se comunica com os demais quando necessário.

---

## Organização da solução

A solução é organizada pelo arquivo `BusTrack.sln`, que reúne os diferentes projetos que compõem a aplicação.

A estrutura principal é:

```text
BusTrack
├── BusTrack.API
├── BusTrack.DB
├── BusTrack.Program
├── BusTrack.Frontend
├── BusTrack.Tests
└── BusTrack.Updater
```

O projeto `BusTrack.Frontend` possui internamente o cliente Angular e o servidor responsável pela integração e execução do frontend durante o desenvolvimento.

Essa divisão permite que backend, frontend, persistência, testes e componentes auxiliares sejam mantidos separadamente.

---

# BusTrack.API

O projeto `BusTrack.API` concentra a camada responsável pela exposição dos recursos da aplicação através da API.

É nessa camada que ficam os componentes responsáveis por receber requisições, processar as operações através dos serviços e devolver as respostas ao cliente.

Entre seus principais elementos estão:

* Controllers;
* DTOs;
* Models;
* Interfaces de serviços;
* Services;
* Mapeamentos.

### Controllers

Os Controllers representam a entrada das requisições destinadas à API.

Eles recebem as requisições HTTP, encaminham as operações para os serviços responsáveis e retornam as respostas apropriadas.

A responsabilidade do Controller não é concentrar toda a regra de negócio.

Sua função principal é atuar como ponto de entrada e saída da API.

### DTOs

Os DTOs representam os objetos utilizados na comunicação da API.

Eles permitem separar a estrutura utilizada na comunicação externa da estrutura utilizada internamente para persistência dos dados.

Isso evita que as classes utilizadas diretamente pelo banco de dados precisem ser necessariamente expostas pela API.

### Services

Os Services concentram operações relacionadas às funcionalidades da aplicação.

Eles recebem as solicitações provenientes dos Controllers e utilizam os componentes necessários para executar as operações.

Essa separação evita que Controllers precisem conhecer diretamente os detalhes de acesso ao banco de dados.

### Interfaces

As interfaces definem contratos para os serviços utilizados pela aplicação.

Isso permite que os componentes dependam de abstrações em vez de depender diretamente de implementações específicas.

Essa organização também facilita a substituição de implementações e a criação de testes utilizando dependências simuladas.

### Mapeamentos

Os mapeamentos são utilizados para converter objetos entre diferentes representações utilizadas pela aplicação.

O projeto utiliza AutoMapper para auxiliar nesse processo.

Dessa forma, DTOs, modelos utilizados pela API e entidades utilizadas na persistência podem permanecer separados.

---

# BusTrack.DB

O projeto `BusTrack.DB` concentra as responsabilidades relacionadas à persistência e ao acesso aos dados.

Ele contém os componentes utilizados para representar e acessar as informações armazenadas no banco de dados.

Entre seus principais elementos estão:

* Classes de dados;
* Modelos;
* Interfaces;
* Repositórios;
* Componentes relacionados ao acesso ao banco;
* Serviços relacionados à persistência.

## Repositories

Os repositórios são responsáveis pelas operações de acesso aos dados.

Eles isolam da camada de API os detalhes necessários para consultar, inserir, atualizar ou remover informações do banco.

Dessa maneira, um Service não precisa conhecer diretamente todos os detalhes da implementação utilizada para executar uma consulta.

Por exemplo, para obter determinado registro, o Service utiliza o contrato fornecido pelo repositório, enquanto o repositório é responsável por executar a operação no banco de dados.

## Separação da persistência

A separação entre API e persistência permite que as responsabilidades permaneçam organizadas.

A API trabalha com as operações da aplicação.

A camada de persistência trabalha com o armazenamento e recuperação dos dados.

Essa divisão também facilita os testes, pois os repositórios podem ser substituídos por implementações simuladas quando necessário.

---

# BusTrack.Program

O projeto `BusTrack.Program` contém o ponto de entrada da aplicação e as configurações responsáveis pela inicialização do backend.

Entre suas responsabilidades estão:

* Inicialização da aplicação;
* Registro das dependências;
* Configuração dos serviços;
* Configuração da persistência;
* Configuração do pipeline da aplicação;
* Extensões utilizadas durante a inicialização;
* Configurações relacionadas ao tratamento de erros.

O `Program.cs` funciona como o ponto onde diferentes partes da aplicação são reunidas para formar o backend executável.

É nesse momento que as implementações são associadas aos contratos utilizados pelas demais camadas.

Por exemplo, uma interface de repositório pode ser registrada no sistema de injeção de dependência para utilizar sua implementação correspondente.

---

# Injeção de dependência

A aplicação utiliza injeção de dependência para controlar como os componentes são criados e utilizados.

Em vez de cada classe criar diretamente suas próprias dependências, elas podem recebê-las através do mecanismo de injeção de dependência da aplicação.

Isso reduz o acoplamento entre os componentes.

Um Service, por exemplo, pode depender de uma interface de repositório sem precisar criar diretamente a implementação concreta desse repositório.

A configuração dessas dependências é realizada durante a inicialização da aplicação.

---

# BusTrack.Frontend

O projeto `BusTrack.Frontend` concentra os componentes necessários para executar e disponibilizar o frontend da aplicação.

Dentro dele existe o cliente Angular responsável pela interface utilizada pelo usuário e o servidor utilizado durante a integração e execução da aplicação frontend.

A separação permite distinguir a aplicação responsável pela interface do usuário da infraestrutura utilizada para executá-la.

---

# bustrack.frontend.client

O `bustrack.frontend.client` é o cliente Angular da aplicação.

É nele que estão concentrados os componentes responsáveis pela interface e pelo comportamento do frontend.

Entre seus principais elementos estão:

* Componentes;
* Telas;
* Serviços;
* Modelos;
* Guards;
* Regras de validação;
* Regras relacionadas à sessão;
* Recursos estáticos;
* Configurações de ambiente.

## Componentes

Os componentes representam as diferentes partes da interface.

Eles são utilizados para organizar as telas e seus comportamentos.

As principais áreas da aplicação incluem:

* Login;
* Criação de conta;
* Confirmação;
* Conclusão;
* Atualização de senha;
* Dashboard;
* Ônibus;
* Viagens;
* Navegação principal.

## Services

Os Services do frontend concentram funcionalidades compartilhadas e comunicação com a API.

Eles permitem que os componentes não precisem concentrar diretamente toda a lógica de comunicação com o backend.

Entre as responsabilidades existentes estão:

* Comunicação com a API;
* Controle de sessão;
* Verificação de disponibilidade;
* Operações relacionadas aos módulos da aplicação.

## Guards

Os Guards são utilizados para controlar o acesso às rotas.

O `AuthenticationGuard`, por exemplo, verifica se existe uma sessão autenticada antes de permitir o acesso às áreas protegidas.

O `BrowserNavigationGuard` possui outra responsabilidade: controlar tentativas de navegação realizadas pelos botões Voltar e Avançar do navegador.

A existência de Guards separados permite que cada regra de navegação tenha uma responsabilidade específica.

---

# BusTrack.Frontend.Server

O `BusTrack.Frontend.Server` é o servidor responsável pela integração e execução do frontend Angular no ambiente da aplicação.

Durante o desenvolvimento, ele participa da execução do cliente Angular e da integração necessária para que o frontend seja utilizado juntamente com o backend.

O projeto também utiliza o mecanismo de SPA Proxy durante o desenvolvimento.

Esse servidor não representa a camada principal de regras de negócio da aplicação.

As regras relacionadas ao processamento dos dados permanecem no backend.

---

# BusTrack.Tests

O projeto `BusTrack.Tests` concentra os testes automatizados da solução.

A separação dos testes em um projeto próprio permite testar os componentes da aplicação sem misturar código de produção com código utilizado exclusivamente para validação.

Os testes estão organizados de acordo com o tipo de comportamento que precisa ser verificado.

## Testes unitários

Os testes unitários verificam partes específicas da aplicação de maneira isolada.

Quando uma classe possui dependências externas, essas dependências podem ser substituídas por objetos simulados.

O objetivo é verificar o comportamento do componente testado sem depender necessariamente de toda a aplicação.

## Testes de integração

Os testes de integração verificam a comunicação entre diferentes partes da aplicação.

Nesse tipo de teste, componentes que normalmente trabalhariam juntos são executados em conjunto para verificar se a integração funciona conforme esperado.

A infraestrutura utilizada inclui recursos como `WebApplicationFactory` e `Microsoft.AspNetCore.TestHost`.

## Testes de performance

Os testes de performance são utilizados para medir o comportamento de determinados componentes em relação ao desempenho.

Eles permitem analisar operações específicas sem tratar uma medição de desempenho como se fosse um teste funcional comum.

## Testes de usabilidade

Os testes de usabilidade automatizam interações com a interface da aplicação.

Eles permitem verificar comportamentos que dependem da interação com as telas do sistema.

---

# BusTrack.Updater

O projeto `BusTrack.Updater` contém componentes auxiliares utilizados para atualização de dados específicos da aplicação.

Atualmente existem componentes relacionados à atualização de informações de motoristas e passageiros.

A existência desse projeto mantém essas operações auxiliares separadas das responsabilidades principais da API e da persistência.

Isso evita concentrar todos os componentes relacionados a atualização de dados dentro de um único projeto.

---

# Comunicação entre as camadas

A comunicação entre as principais camadas do backend segue uma divisão de responsabilidades.

Uma requisição enviada para a API é recebida inicialmente por um Controller.

O Controller encaminha a operação para o Service correspondente.

O Service executa a operação necessária e, quando precisa acessar dados persistidos, utiliza o contrato fornecido pelo Repository.

O Repository executa a operação de persistência.

O resultado retorna pelas mesmas camadas até chegar ao Controller, que produz a resposta HTTP para o cliente.

De forma simplificada:

```text
Cliente
Controller
Service
Repository
Banco de dados
```

O caminho de retorno ocorre no sentido inverso.

Essa organização evita que uma única classe seja responsável simultaneamente por receber requisições, executar regras da aplicação e acessar diretamente o banco.

---

# Responsabilidade de cada camada

A divisão pode ser entendida da seguinte maneira:

### API

É responsável pela comunicação HTTP e pelas operações expostas ao cliente.

### Service

É responsável por coordenar as operações da aplicação e aplicar as regras correspondentes às funcionalidades.

### Repository

É responsável pelo acesso aos dados persistidos.

### Banco de dados

É responsável pelo armazenamento das informações.

### Frontend

É responsável pela interface utilizada pelo usuário, navegação, estado da sessão e comunicação com a API.

### Tests

São responsáveis por verificar o comportamento dos componentes e a integração entre diferentes partes da solução.

### Program

É responsável por reunir e configurar os componentes necessários para executar a aplicação.

### Updater

É responsável por componentes auxiliares relacionados à atualização de determinados dados.

---

# Comunicação entre frontend e backend

O frontend e o backend são aplicações separadas dentro da solução.

O frontend é responsável pela interação com o usuário.

Quando uma operação precisa ser processada pelo servidor, o frontend realiza uma requisição para a API.

A API recebe a requisição, processa a operação através de seus Services e acessa a persistência quando necessário.

Depois do processamento, a API retorna uma resposta para o frontend.

O frontend utiliza essa resposta para atualizar a interface apresentada ao usuário.

Essa separação permite que a interface e o processamento do sistema evoluam de maneira independente.

---

# Persistência de dados

A persistência é isolada em uma camada própria.

O acesso aos dados é realizado através dos componentes existentes no projeto `BusTrack.DB`.

A API não precisa implementar diretamente os detalhes das operações de persistência.

Essa separação também permite que os Services trabalhem com contratos definidos por interfaces.

A configuração necessária para estabelecer a conexão com o banco é realizada durante a inicialização da aplicação.

---

# Segurança como responsabilidade distribuída

A segurança da aplicação não está concentrada em um único componente.

O backend possui responsabilidades relacionadas à proteção dos dados e das operações.

O frontend possui mecanismos complementares relacionados à sessão, proteção das rotas e comportamento da interface.

É importante distinguir essas duas responsabilidades.

Uma regra implementada apenas no frontend não deve ser considerada uma proteção suficiente para os dados da aplicação.

Por isso, regras que precisam realmente impedir uma operação devem ser aplicadas também no backend.

A autenticação baseada em JWT está prevista como uma evolução da aplicação, mas não deve ser considerada parte da arquitetura atualmente implementada enquanto sua implementação efetiva não estiver concluída.

---

# Testabilidade

A separação dos componentes também contribui para a testabilidade da aplicação.

Interfaces permitem que dependências sejam substituídas durante os testes.

A separação entre Controllers, Services e Repositories permite testar responsabilidades diferentes de maneira independente.

Os testes de integração complementam essa abordagem verificando o funcionamento conjunto dos componentes.

Dessa maneira, a solução não depende exclusivamente de testes manuais realizados através da interface.

---

# Organização por responsabilidade

A arquitetura do Bus Track procura manter cada projeto responsável por um conjunto específico de tarefas.

A API não deve assumir as responsabilidades do frontend.

O frontend não deve assumir as responsabilidades de persistência.

Os repositórios não devem assumir a responsabilidade pela interface.

Os testes permanecem separados do código de produção.

Os componentes auxiliares de atualização permanecem separados das principais responsabilidades da API.

Essa organização facilita a navegação pelo código e permite identificar onde determinada responsabilidade deve ser implementada.

---

# Evolução da arquitetura

A arquitetura foi estruturada para permitir que novas funcionalidades sejam adicionadas sem que todos os projetos precisem ser alterados simultaneamente.

Quando uma nova funcionalidade precisa ser adicionada, suas responsabilidades podem ser distribuídas entre os projetos correspondentes.

Uma funcionalidade que exige uma nova operação da API pode envolver um Controller, um Service e, quando necessário, um Repository.

No frontend, a mesma funcionalidade pode possuir componentes, serviços e rotas próprios.

Os testes correspondentes permanecem no projeto de testes.

Essa organização permite que a evolução do sistema aconteça de forma incremental.

---

# Decisões arquiteturais

Algumas decisões importantes da organização do Bus Track são:

* Separação entre API, persistência, frontend e testes;
* Utilização de interfaces para definir contratos entre componentes;
* Uso de injeção de dependência;
* Separação entre DTOs e estruturas utilizadas na persistência;
* Utilização de Services para coordenar operações da aplicação;
* Utilização de Repositories para acesso aos dados;
* Separação do cliente Angular e do servidor responsável por sua execução;
* Projeto separado para testes automatizados;
* Projeto separado para componentes auxiliares de atualização.

Essas decisões procuram reduzir responsabilidades excessivamente concentradas em um único componente e tornar a solução mais organizada e testável.

---

# Resumo da arquitetura

O Bus Track é organizado como uma solução composta por projetos com responsabilidades distintas.

O `BusTrack.API` concentra a comunicação com os clientes e as operações disponibilizadas pela aplicação.

O `BusTrack.DB` concentra o acesso e a persistência dos dados.

O `BusTrack.Program` reúne a configuração e inicialização do backend.

O `BusTrack.Frontend` contém a infraestrutura relacionada ao frontend, incluindo o cliente Angular e o servidor utilizado durante sua execução.

O `BusTrack.Tests` mantém os testes automatizados separados do código de produção.

O `BusTrack.Updater` mantém componentes auxiliares relacionados à atualização de dados específicos.

Essa divisão permite que a aplicação seja desenvolvida e evoluída mantendo as responsabilidades organizadas entre diferentes partes da solução.

## Documentação relacionada

Para consultar detalhes específicos sobre cada assunto:

* [Funcionalidades](functionalities/functionalities.md)
* [Tecnologias](technologies/technologies.md)
* [Estrutura do projeto](project-structure/project-structure.md)
* [Segurança](security/security.md)
* [Banco de dados](database/database.md)
* [Testes](testing/testing.md)
* [Deploy](deployment/deployment.md)
