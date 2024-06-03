README

# Bus Track

![Venha conhecer o projeto Bus Track.](OIG4.jpeg)

O projeto Bus Track fornece uma API RESTful para gerenciar dados de ônibus. A API permite que os fiscais obtenham, criem, atualizem e excluam dados de ônibus.
O DB é o banco de dados NoSQL e o escolhido foi o MongoDB. O Front é o que o fiscal que é o usuário irá verificar e interagir com o sistema e o escolhido foi o Angular. Tem os testes que serão o unitário, integração, perfomance e usabilidade. O unitário para verificar pequenas partes do código se está saindo como o planejado e o de usabilidade para garantir que a interface do usuário seja intuitiva e fácil de usar.

## Funcionalidades Principais

O objetivo principal do projeto é realizar o controle tanto de quantos passageiros embarcaram durante aquela viagem sendo que há uma média e se ficar abaixo, sofrerá punição.

- Ônibus: Informações sobre os ônibus, como número, modelo, marca e capacidade.
- Motoristas: Informações sobre os motoristas, como nome, idade e licença de motorista.
- Rotas: Informações sobre as rotas, como origem, destino e número de paradas.
- Passageiros: Informações sobre os passageiros, como nome, idade e endereço.

## Ferramentas utilizadas
O projeto BusTrack.API utiliza as seguintes ferramentas e tecnologias:

Projeto BusTrack Backend (BusTrack.csproj)
Autenticação e Autorização:
Implementação do Auth0 para gerenciamento de autenticação e autorização utilizando o pacote Auth0.ManagementApi.

Mapeamento de Objetos:
Uso do AutoMapper para mapeamento entre objetos de domínio e DTOs.

Hashing de Senhas:
Implementação de hashing de senhas com BCrypt.Net-Next.

Documentação de API:
Configuração do Swagger para documentação da API utilizando Swashbuckle.AspNetCore.

Banco de Dados:
Integração com MongoDB utilizando MongoDB.Bson e MongoDB.Driver.

JWT:
Manipulação de tokens JWT utilizando System.IdentityModel.Tokens.Jwt.

Testes:
Configuração e execução de testes unitários com xunit e Moq.
Uso de coverlet.collector para coleta de cobertura de testes.
Uso de BenchmarkDotNet para benchmarking.

Outros Pacotes:
Microsoft.AspNetCore.Hosting.Abstractions
Microsoft.AspNetCore.OpenApi
Microsoft.AspNetCore.SpaProxy
Microsoft.AspNetCore.TestHost
Microsoft.Extensions.DependencyInjection
Newtonsoft.Json
Selenium.WebDriver
System.Linq.Async

Frontend Server (BusTrack.Frontend.Server.csproj)
Configuração de Proxy para SPA:
Configuração do proxy para o servidor de desenvolvimento Angular utilizando Microsoft.AspNetCore.SpaProxy.

Configuração de Pacotes:
Configuração semelhante ao backend, incluindo pacotes como Auth0.ManagementApi, AutoMapper, BenchmarkDotNet, coverlet.collector, MongoDB.Bson, MongoDB.Driver, Moq, Newtonsoft.Json, Swashbuckle.AspNetCore, System.IdentityModel.Tokens.Jwt, xunit, entre outros.

Testes (BusTrack.Test.csproj)
Configuração de Testes Unitários e de Integração:
Projeto dedicado aos testes, com referências aos projetos de backend e frontend server.
Uso de xunit para estrutura de testes.
Uso de coverlet.collector para cobertura de testes.
Uso de Moq para criação de mocks.

Configuração Geral
Framework:
Alvo do .NET 7.0 para todos os projetos.

Usings e Nullable:
Usings implícitos habilitados e nullable reference types habilitados.

Integração com CI/CD:
Configuração do projeto para integração contínua e entrega contínua utilizando Microsoft.NET.Test.Sdk, Microsoft.TestPlatform.TestHost, e 
Microsoft.VisualStudio.TestPlatform.

###Configuração do MongoDB

Durante a instalação do MongoDB, optei por executar o serviço como usuário de serviço de rede. Essa decisão foi tomada para simplificar o processo de conexão ao banco de dados e facilitar o desenvolvimento e a demonstração do projeto.

Executar o serviço como usuário de serviço de rede permite que o MongoDB seja iniciado com permissões limitadas, o que é adequado para ambientes de desenvolvimento e uso pessoal. Isso elimina a necessidade de configurar um login e senha específicos para acessar o banco de dados, tornando o processo de desenvolvimento mais rápido e direto.

### Segurança

No parte de backend utilizei o JWT para token e Bcrypt para hashing de senhas como pode verificar logo acima.

Já no frontend criei regras para não salvar senha, copiar informações tanto com o teclado quanto selecionando com o botão esquerdo do mouse, o botão direito está desativado em todo o sistema e tem um temporizador de conexão de 20 minutos. Além disso, não pode voltar ou avançar na tela do sistema com os botões do navegador. Também há a política de senha que o usuário, a cada 45 dias, é obrigado a trocar a senha e o aviso será mostrado quando estiver desconectado.



## Histórico de Atualizações

#2024-06-03

- Lançamento do sistema Bus Track. Foi salvo no GitHub a versão completa.


### 2024-04-26

- Retirada de todos os erros do projeto.


### 2024-04-21

- Criado a estrutura do frontend com Angular.

### 2024-04-20

- Criado a estrutura de testes de integração.

### 2024-04-17

- Criado a estrutura de testes unitários.

### 2024-04-16

- Criado a estrutura da API.

### 2024-04-15

- Dividindo o Program em pastas e arquivos menores.

### 2024-04-11

- Adicionado a conexão do projeto com o banco de dados Bus Track

### 2024-04-10

- Adicionados serviços relacionados ao banco de dados na pasta ServicesDB.

### 2024-04-07

- Lançamento inicial do projeto no GitHub.



## Estrutura das pastas do projeto com suas respectivas funções:

BusTrack                                       # Nome do projeto
 BusTrack.API                                  # Pasta em relação à API
  ControllersAPI                               # Pasta para controladores da API
   AccountControllerAPI.cs                     # Controller para operações relacionadas a contas de usuário
   AuthenticationControllerAPI.cs              # Controller para autenticação de usuários.
   BusControllerAPI.cs                         # Controller para gerenciar operações relacionadas a ônibus
   CreateAccountControllerAPI.cs               # Controller para criar novas contas de usuário.
   DashboardControllerAPI.cs                   # Controller para endpoints relacionados ao dashboard e salvamento de dados
   DriverControllerAPI.cs                      # Controller para gerenciar operações relacionadas a motoristas
   PassengerControllerAPI.cs                   # Controller para gerenciar operações relacionadas a passageiros
   RouteControllerAPI.cs                       # Controller para gerenciar operações relacionadas a rotas
   TripControllerAPI.cs                        # Controller para gerenciar operações relacionadas a viagens
   TripsPassengerControllerAPI.cs              # Controller para gerenciar operações relacionadas entre viagens e passageiros
   UserControllerAPI.cs                        # Controller para gerenciar operações relacionadas a usuários
  DTOAPI                                       # Pasta para DTOs da API
   BusDTOAPI.cs                                # DTO para ônibus
   DriverDTOAPI.cs                             # DTO para motoristas
   PassengerDTOAPI.cs                          # DTO para passageiros
   RouteDTOAPI.cs                              # DTO para rotas
   TripDTOAPI.cs                               # DTO para viagens
   TripPassengerDTOAPI.cs                      # DTO para associação entre viagens e passageiros
   UpdatePasswordDTOAPI.cs                     # DTO para atualização de senha.
  InterfacesAPI                                # Pasta para interfaces da API
   ServicesAPI                                 # Pasta para interfaces de serviço da API
    IAccountServiceAPI.cs                      # Interface para o serviço relacionado a contas de usuário.
    IBusServiceAPI.cs                          # Interface para serviço relacionado a ônibus
    IDriverServiceAPI.cs                       # Interface para serviço relacionado a motoristas
    IEmailConfirmationServiceAPI.cs            # Interface para o serviço de confirmação de e-mail na API
    IPassengerServiceAPI.cs                    # Interface para serviço relacionado a passageiros
    IRouteServiceAPI.cs                        # Interface para serviço relacionado a rotas
    ITripServiceAPI.cs                         # Interface para serviço relacionado a viagens
    ITripsPassengerServiceAPI.cs               # Interface para serviço relacionado entre viagens e passageiros
    IUpdatePasswordServiceAPI.cs               # Interface para serviço de atualização de senha. 
    IUserAuthenticationServiceAPI.cs           # Interface para o serviço de autenticação de usuários na API
    IUserServiceAPI.cs                         # Interface para o serviço de usuário na API
  MappingsAPI                                  # Pasta para configuração de mapeamentos
   MappingProfileAPI.cs                        # Arquivo para configuração de mapeamentos
  ModelsAPI                                    # Pasta para modelos da API
   AccountModelAPI.cs                          # Modelo para contas de usuário
   BusModelAPI.cs                              # Modelo para ônibus
   DriverModelAPI.cs                           # Modelo para motoristas
   PassengerModelAPI.cs                        # Modelo para passageiros
   RouteModelAPI.cs                            # Modelo para rotas
   TripModelAPI.cs                             # Modelo para viagens
   TripsPassengerModelAPI.cs                   # Modelo para associação entre viagens e passageiros
   UserModelAPI.cs                             # Modelo para usuários.
  ServicesAPI                                  # Pasta para serviços da API
   AccountServiceAPI.cs                        # Serviço para operações relacionadas a contas de usuário
   BusServiceAPI.cs                            # Serviço para operações relacionadas a ônibus
   DriverServiceAPI.cs                         # Serviço para operações relacionadas a motoristas
   PassengerServiceAPI.cs                      # Serviço para operações relacionadas a passageiros
   RouteServiceAPI.cs                          # Serviço para operações relacionadas a rotas
   TripServiceAPI.cs                           # Serviço para operações relacionadas a viagens
   TripsPassengerServiceAPI.cs                 # Serviço para operações relacionadas a associação entre viagens e passageiros
   UpdatePasswordServiceAPI.cs                 # Serviço para atualização de senha.
   UserAuthenticationServiceAPI.cs             # Serviço para autenticação de usuários.
   UserServiceAPI.cs                           # serviço para operações relacionados aos usuários
 BusTrack.DB                                   # Pasta em relação ao Banco de Dados
  ClassesDB                                    # Pasta para as classes das tabelas do banco de dados
   BusDB.cs                                    # Define a classe BusDB, representando a tabela de ônibus
   DriverDB.cs                                 # Define a classe DriverDB, representando a tabela de motoristas
   EmailConfirmationDB                         # Classe para representar a confirmação de e-mail.
   InspectorDB                                 # Classe para representar inspetores.
   LoginDB                                     # Classe para representar informações de login.
   PassengerDB.cs                              # Define a classe PassengerDB, representando a tabela de passageiros
   PasswordHistoryDB.cs                        # Classe representando o histórico de senhas.
   PasswordRecordDB.cs                         # Define a classe PasswordRecordDB, que representa a tabela de registros de senhas.
   RouteDB.cs                                  # Define a classe RouteDB, representando a tabela de rotas
   TripDB.cs                                   # Define a classe TripDB, representando a tabela de viagens
   TripsPassengerDB.cs                         # Define a classe TripsPassengerDB, representando a tabela de associação entre viagens e passageiros
   UserDB.cs                                   # Define a classe UserDB, para representar a tabela de usuários no banco de dados
   UserPasswordHistoryDB.cs                    # Classe representando o histórico de senhas de usuários.
   UserRegistrationDB.cs                       # Define a classe UserRegistrationDB, que representa a tabela de registros de usuários.
  ConnectionsDB                                # Pasta para configuração do banco de dados
   ConnectionDB.cs                             # Configura a conexão do banco de dados
  DataBaseDB                                   # Pasta para configuração do banco de dados
   database.json                               # Arquivo de configuração do banco de dados com o projeto.
  InterfacesDB                                 # Pasta para interfaces do banco de dados
   IModelsDB                                   # Pasta das interfaces dos modelos do banco de dados
    IBusModelDB.cs                             # Interface IBusModelDB
    IDriverModelDB.cs                          # Interface IDriverModelDB
    IPassengerModelDB.cs                       # Interface IPassengerModelDB
    IRouteModelDB.cs                           # Interface IRouteModelDB
    ITripModelDB.cs                            # Interface ITripModelDB
    ITripsPassengerModelDB.cs                  # Interface ITripsPassengerModelDB
   IRepositoriesDB                             # Pasta das interfaces dos repositórios do banco de dados
    IBusRepositoryDB.cs                        # Interface IBusRepositoryDB
    IDriverRepositoryDB.cs                     # Interface IDriverRepositoryDB
    IPassengerRepositoryDB.cs                  # Interface IPassengerRepositoryDB
    IRouteRepositoryDB.cs                      # Interface IRouteRepositoryDB
    ITripRepositoryDB.cs                       # Interface ITripRepositoryDB
    ITripsPassengerRepositoryDB.cs             # Interface ITripsPassengerRepositoryDB
    IUserRepositoryDB.cs                       # Interface para repositório de usuários.
  ModelsDB                                     # Pasta para modelos do banco de dados
   AccountModelDB.cs                           # Implementação do modelo de contas.
   BusModelDB.cs                               # Implementação da interface IBusModelDB
   DriverModelDB.cs                            # Implementação da interface IDriverModelDB
   PassengerModelDB.cs                         # Implementação da interface IPassengerModelDB
   RouteModelDB.cs                             # Implementação da interface IRouteModelDB
   TripModelDB.cs                              # Implementação da interface ITripModelDB
   TripsPassengerModelDB.cs                    # Implementação da interface ITripsPassengerModelDB
   UserModelDB.cs                              # Implementação do modelo de usuários.
  RepositoriesDB                               # Pasta para repositórios do banco de dados
   BusRepositoryDB.cs                          # Repositório para a classe BusDB
   DriverRepositoryDB.cs                       # Repositório para a classe DriverDB
   InspectorRepositoryDB.cs                    # Repositório para a classe InspectorDB.
   PassengerRepositoryDB.cs                    # Repositório para a classe PassengerDB
   RouteRepositoryDB.cs                        # Repositório para a classe RouteDB
   TripRepositoryDB.cs                         # Repositório para a classe TripDB
   TripsPassengerRepositoryDB.cs               # Repositório para a classe TripsPassengerDB
   UserRepositoryDB.cs                         # Repositório para a classe UserDB.
  ServicesDB                                   # Pasta para serviços do banco de dados
   BusSingleTripConstraintServiceDB.cs         # Serviço para restrição de viagem única
   DepartureTimeValidationServiceDB.cs         # Serviço para validação de hora de partida
   MinTripDurationContraintServiceDB.cs        # Serviço para restrição de duração mínima de viagem
   PassengerLimitValidationServiceDB.cs        # Serviço para validação de limite de passageiros
   PasswordHistoryValidationServiceDB.cs       # Serviço para validação de histórico de senhas.
   RouteConflictServiceDB.cs                   # Serviço para detecção de conflitos de rota.
   RouteSchedulerServiceDB.cs                  # Serviço de agendamento de rotas.
   TripMappingServiceDB.cs                     # Serviço para mapeamento de viagem
   TripServiceDB.cs                            # Serviço relacionado às viagens
   TripStatusUpdateServiceDB.cs                # Serviço para atualização de status de viagem
   UserEmailValidationServiceDB.cs             # Serviço de validação de e-mail de usuário.
   UserRoleValidationServiceDB.cs              # Serviço de validação de função de usuário.
 BusTrack.Frontend                             # Pasta raiz do projeto frontend
  bustrack.frontend.client                     # Pasta do cliente frontend
   src                                         # Pasta de código-fonte
    app                                        # Pasta principal do aplicativo
     login                                     # Pasta contendo componentes relacionados ao login
      concluded                                # Pasta contendo componentes relacionados à conclusão
       concluded.component.css                 # Estilos CSS para a tela de conclusão
       concluded.component.html                # Template HTML para a tela de conclusão
       concluded.component.spec.ts             # Teste para o componente de conclusão
       concluded.component.ts                  # Lógica TypeScript para a tela de conclusão
      confirmation/                            # Pasta contendo componentes relacionados à confirmação
       confirmation.component.css              # Estilos CSS para a tela de confirmação
       confirmation.component.html             # Template HTML para a tela de confirmação
       confirmation.component.spec.ts          # Teste para o componente de confirmação
       confirmation.component.ts               # Lógica TypeScript para a tela de confirmação
      create-an-account                        # Pasta contendo componentes relacionados à criação de conta
       create-an-account.component.css         # Estilos CSS para a tela de criação de conta
       create-an-account.component.html        # Template HTML para a tela de criação de conta
       create-an-account.component.spec.ts     # Teste para o componente de criação de conta
       create-an-account.component.ts          # Lógica TypeScript para a tela de criação de conta
      enter-the-system                         # Pasta contendo componentes relacionados à entrada no sistema
       enter-the-system.component.css          # Estilos CSS para a tela de entrada no sistema
       enter-the-system.component.html         # Template HTML para a tela de entrada no sistema
       enter-the-system.component.spec.ts      # Teste para o componente de entrada no sistema
       enter-the-system.component.ts           # Lógica TypeScript para a tela de entrada no sistema
      main-screen                              # Pasta contendo componentes relacionados à tela principal
       main-screen.component.css               # Estilos CSS para a tela principal
       main-screen.component.html              # Template HTML para a tela principal
       main-screen.component.spec.ts           # Teste para o componente de tela principal
       main-screen.component.ts                # Lógica TypeScript para a tela principal
      rules                                    # Regras
       blockCopyRules                          # Regras de bloqueio de cópia de informações sensíveis ou confidenciais
        blockCopyRule.ts                       # Contém a lógica TypeScript relacionada à regra de bloqueio de cópia de informações sensíveis ou confidenciais
       blockSavePasswordRules                  # Regras de Bloqueio de Salvamento de Senha
        blockSavePasswordRule.ts               # Contém a lógica TypeScript relacionada à regra de bloqueio de salvamento de senha.
       checkPasswordStrengthpasswordRules      # Regras de verificação da força das senhas dos usuários
        checkPasswordStrengthpasswordRule.ts   # Contém a lógica TypeScript relacionada à regra de verificação da força das senhas dos usuários
       disableInteractionsRules                # Regras de Desativação de Interações
        disableInteractionsRule.ts             # Contém a lógica TypeScript relacionada à regra de desativação de interações.
       disableKeyboardShortcutsRules           # Regras de Desativação de Atalhos de Teclado
        disableKeyboardShortcutsRule.ts        # Contém a lógica TypeScript relacionada à regra de desativação de atalhos de teclado.
       hasRepeatedOrSequentialNumbersRules     # Regras de detecção de números repetidos ou sequenciais em senhas
        hasRepeatedOrSequentialNumbersRule.ts  # Contém a lógica TypeScript relacionada à regra de detecção de números repetidos ou sequenciais em senhas
       inactivityTimerRules                    # Regras do Temporizador de Inatividade
        inactivityTimerRule.ts                 # Contém a lógica TypeScript relacionada às regras do temporizador de inatividade.
       preventBackNavigationRules              # Regras de Prevenção de Navegação para Trás
        preventBackNavigationRule.ts           # Contém a lógica TypeScript relacionada às regras de prevenção de navegação para trás.
       preventForwardNavigationRules           # Regras de Prevenção de Navegação para Frente
        preventForwardNavigationRule.ts        # Contém a lógica TypeScript relacionada às regras de prevenção de navegação para frente.
       sessionTimeoutRules                     # Regras de Expiração de Sessão):
        sessionTimeoutRule.ts                  # Contém a lógica TypeScript relacionada às regras de expiração de sessão.
      update-password                          # Pasta contendo componentes relacionados à atualização de senha.
       update-password.component.css           # Estilos CSS para o componente de atualização de senha.
       update-password.component.html          # Template HTML para o componente de atualização de senha.
       update-password.component.spec.ts       # Teste para o componente de atualização de senha.
       update-password.component.ts            # Lógica TypeScript para o componente de atualização de senha.
     main                                      # Pasta contendo componentes principais da aplicação, responsáveis por exibir a interface do usuário principal.
      dashboard                                # Contém os arquivos relacionados ao componente do painel principal da aplicação.
       dashboard.component.css                 # Arquivo de estilos CSS específicos para o componente do painel principal.
       dashboard.component.html                # Template HTML que define a estrutura do componente do painel principal.
       dashboard.component.spec.ts             # Arquivo de teste unitário para o componente do painel principal.
       dashboard.component.ts                  # Lógica TypeScript para o componente do painel principal.
      rules-main                               # Pasta que contém as regras específicas relacionadas ao painel principal da aplicação
       limitCharactersRules                    # Pasta que contém as regras relacionadas à limitação de caracteres em campos específicos do painel principal.
        limitCharactersRule.ts                 # Arquivo que define a lógica para a regra de limitação de caracteres.
       validateEmailFormatRules                # Pasta que Contém as regras relacionadas à validação do formato de email em campos específicos do painel principal
        validateEmailFormatRule.ts             # Arquivo que define a lógica para a regra de validação do formato de email em campos específicos do painel principal
       validateFieldsRequiredRules             # Pasta que Contém as regras relacionadas à validação de campos obrigatórios em campos específicos do painel principal
        validateFieldsRequiredRule.ts          # Arquivo que define a lógica para a regra de validação de campos obrigatórios em campos específicos do painel principal
      sidebar                                  # Pasta que contém os arquivos relacionados ao componente da barra lateral da aplicação.
       sidebar.component.css                   # Arquivo de estilos CSS específicos para o componente da barra lateral.
       sidebar.component.html                  # Template HTML que define a estrutura do componente da barra lateral.
       sidebar.component.spec.ts               # Arquivo de teste unitário para o componente da barra lateral.
       sidebar.component.ts                    # Lógica TypeScript para o componente da barra lateral.
     models                                    # Pasta que contém os modelos de dados da aplicação.
      user.model.ts                            # Define a estrutura de dados para o modelo de usuário da aplicação
     services                                  # Pasta que contém os serviços utilizados pela aplicação
      data.service.ts                          # Implementa um serviço para lidar com operações de manipulação de dados na aplicação
      user.service.ts                          # Implementa um serviço para lidar com operações relacionadas a usuários na aplicação
      validation.service.ts                    # Implementa um serviço para lidar com validações na aplicação
     app-routing.module.ts                     # Define as rotas da aplicação
     app.component.css                         # Arquivo de estilos CSS global para o componente principal da aplicação
     app.component.html                        # Template HTML principal da aplicação
     app.component.ts                          # Lógica TypeScript para o componente principal da aplicação
     app.module.ts                             # Módulo principal da aplicação, onde os componentes e serviços são declarados e importados.
    assets                                     # Pasta que contém recursos estáticos utilizados na aplicação, como imagens, fontes etc
     imagem                                    # Pasta que Contém as imagens utilizadas na aplicação
      OIG4.jpeg                                # Imagem que aparece nas telas do projeto
  BusTrack.Frontend.Server                     # Pasta do servidor frontend
 BusTrack.Program                              # Pasta para o projeto principal do programa
  DataBaseServicesExtensionsProgram            # Pasta para extensões de serviços de banco de dados
   DataBaseServicesExtensionsProgram.cs        # Contém métodos de extensão para adicionar serviços de banco de dados ao contêiner de injeção de dependência
  ExtensionsProgram                            # Pasta para extensões do programa
   ExtensionsProgram.cs                        # Arquivo que contém métodos de extensão para adicionar extensões personalizados ao contêiner de injeção de dependência
   ServiceExtensionProgram.cs                  # Arquivo que contém métodos de extensão para adicionar serviços personalizados ao contêiner de injeção de dependência
  MiddlewareProgram                            # Pasta para middleware do programa
   ErrorHandlingMiddleware.cs                  # Arquivo que é um middleware personalizado para lidar com erros
  Program.cs                                   # Arquivo principal do programa, que configura e executa o aplicativo
 BusTrack.Tests                                # Pasta em relação aos testes
  IntegrationTests                             # Contém testes de integração do sistema.
   CustomWebApplicationFactory                 # Contém classes auxiliares para testes de integração.
     CustomWebApplicationFactory.cs            # Define a lógica para criar instância de teste do seu aplicativo web.
   ControllersAPIIntegrationTests              # Testes de integração de controladores de API.
    BusControllerAPIIntegrationTest.cs         # Testa o controlador de ônibus da API.
    DriverControllerAPIIntegrationTest.cs      # Testa o controlador de motoristas da API.
    PassengerControllerAPIIntegrationTest.cs   # Testa o controlador de passageiros da API.
    RouteControllerAPIIntegrationTest.cs       # Testa o controlador de rotas da API.
    TripControllerAPIIntegrationTest.cs        # Testa o controlador de viagens da API.
    TripsPassengerControllerAPIIntegrationTest.cs  # Testa o controlador de viagens de passageiros da API. 
  ServicesAPIIntegrationTests                  # Testes de integração de serviços da API.
   BusServiceAPIIntegrationTest.cs             # Testa o serviço de ônibus da API.
   DriverServiceAPIIntegrationTest.cs          # Testa o serviço de motoristas da API.
   PassengerServiceAPIIntegrationTest.cs       # Testa o serviço de passageiros da API.
   RouteServiceAPIIntegrationTest.cs           # Testa o serviço de rotas da API.
   TripServiceAPIIntegrationTest.cs            # Testa o serviço de viagens da API.
   TripsPassengerServiceAPIIntegrationTest.cs  # Testa o serviço de viagens de passageiros da API.
  PerformanceTests                             # Contém testes de performance do sistema.
   PassengerServiceAPIPerformanceTests.cs      # Contém testes de performance para o serviço de API relacionado aos passageiros
   RouteServiceAPIPerformanceTests.cs          # Contém testes de performance para o serviço de API relacionado às rotas
  UnitTests                                    # Contém testes unitários do sistema.
   ControllersAPIUnitTests                     # Testes unitários de controladores de API.
    BusControllerAPIUnitTests.cs               # Testa a lógica do controlador de ônibus da API.
    DriverControllerAPIUnitTests.cs            # Testa a lógica do controlador de motoristas da API.
    PassengerControllerAPIUnitTests.cs         # Testa a lógica do controlador de passageiros da API.
    RouteControllerAPIUnitTests.cs             # Testa a lógica do controlador de rotas da API.
    TripControllerAPIUnitTests.cs              # Testa a lógica do controlador de viagens da API.
    TripsPassengerControllerAPIUnitTests.cs    # Testa a lógica do controlador de viagens de passageiros da API.
  UsabilityTests                               # Contém testes de usabilidade do sistema
   UsabilityTests.cs                           # Arquivo para testar diferentes aspectos da usabilidade do sistema
 BusTrack.Updater                              # Pasta para atualização de dados
  DriversUpdater                               # Pasta para atualização de dados de motoristas
   DriverNameUpdater.cs                        # Arquivo para atualizar nomes de motoristas
  PassengerUpdater                             # Pasta para atualização de dados de passageiros
   PassengerNameUpdater.cs                     # Arquivo para atualizar nomes de passageiros
 BusTrack.sln                                  # Arquivo de solução do Visual Studio



## Estrutura do Projeto

O projeto está dividido em três partes interligadas:

- **BusTrack.API:** Inicialização da aplicação e ponto de entrada da API. (Ainda não implementado)

A API fornece as seguintes rotas:

/buses: Obtém uma lista de todos os ônibus.
/buses/{id}: Obtém um ônibus específico com base no ID.
/buses: Cria um novo ônibus.
/buses/{id}: Atualiza um ônibus existente.
/buses/{id}: Exclui um ônibus existente.
/drivers: Obtém uma lista de todos os motoristas.

/drivers/{id}: Obtém um motorista específico com base no ID.
/drivers: Cria um novo motorista.
/drivers/{id}: Atualiza um motorista existente.
/drivers/{id}: Exclui um motorista existente.
/routes: Obtém uma lista de todas as rotas.

/routes/{id}: Obtém uma rota específica com base no ID.
/routes: Cria uma nova rota.
/routes/{id}: Atualiza uma rota existente.
/routes/{id}: Exclui uma rota existente.
/passengers: Obtém uma lista de todos os passageiros.

/passengers/{id}: Obtém um passageiro específico com base no ID.
/passengers: Cria um novo passageiro.
/passengers/{id}: Atualiza um passageiro existente.
/passengers/{id}: Exclui um passageiro existente.



- **BusTrack.Test:** Camada de testes.
Aqui estão alguns exemplos de testes que podem ser realizados:

Teste de validação de dados:
Testa se os dados enviados à API são válidos.
Por exemplo, um teste pode verificar se o número do ônibus é um número inteiro positivo.

Teste de tratamento de erros:
Testa se a API trata os erros corretamente.
Por exemplo, um teste pode verificar se a API retorna um código de erro quando é enviado um número de ônibus inválido.

Teste de desempenho:
Testa o desempenho da API.
Por exemplo, um teste pode verificar quanto tempo leva para a API retornar uma lista de todos os ônibus.


- **BusTrack.Frontend:** Camada frontend.
Nesta parte foi instalado o Angular onde criei as telas e a funcionalidade principal do sistema que é registrar e salvar as informações da viagem. Há regras que foram citadas na seção segurança logo acima nesse README.


### Nomenclatura

Todos os componentes, incluindo nomes de classes, projetos, tabelas e propriedades, foram nomeados em inglês, seguindo as melhores práticas de programação.


## Feedback

Sinta-se à vontade para explorar e dar feedback através de elogios, sugestões e críticas.


## Como Contribuir

Se você deseja contribuir para o desenvolvimento deste projeto, siga as etapas abaixo:

### Relatando Problemas

Se encontrar algum problema ou tiver sugestões de melhorias, por favor, abra uma "Issue". Antes de criar uma nova "Issue", verifique se o problema já não foi relatado por outra pessoa.

### Contribuindo com Código

Se você deseja contribuir com código, siga estas etapas:

1. Fork do repositório.
2. Crie uma nova branch para suas alterações: `git checkout -b nome-da-sua-branch`.
3. Faça as alterações desejadas e faça commit: `git commit -m "Descrição das alterações"`.
4. Faça push para a sua branch: `git push origin nome-da-sua-branch`.
5. Abra um Pull Request (PR) com uma descrição clara das alterações propostas.

Agradeço antecipadamente por suas contribuições!

## Estrelas

Peço encarecidamente, se puder, dar estrela para que o projeto fique em destaque e mostre as outras pessoas.