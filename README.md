README

# Bus Track

![Venha conhecer o projeto Bus Track.](OIG4.jpeg)

O projeto Bus Track fornece uma API RESTful para gerenciar dados de ônibus. A API permite que os fiscais obtenham, criem, atualizem e excluam dados de ônibus.
O DB é o banco de dados NoSQL e o escolhido foi o MongoDB. O Front é o que o fiscal que é o usuário irá verificar e interagir com o sistema e o escolhido foi o Angular. Tem os testes que serão o unitário, integração, perfomance e usabilidade. O unirário para verificar pequenas partes do código se está saindo como o planejado e o de usabilidade para garantir que a interface do usuário seja intuitiva e fácil de usar.

## Funcionalidades Principais

O objetivo principal do projeto é realizar o controle tanto de quantos passageiros embarcaram durante aquela viagem sendo que há uma média e se ficar abaixo, sofrerá punicção.

- Ônibus: Informações sobre os ônibus, como número, modelo, marca e capacidade.
- Motoristas: Informações sobre os motoristas, como nome, idade e licença de motorista.
- Rotas: Informações sobre as rotas, como origem, destino e número de paradas.
- Passageiros: Informações sobre os passageiros, como nome, idade e endereço.

## Ferramentas utilizadas

O projeto BusTrack.API utiliza as seguintes ferramentas e tecnologias:

- Testes:
  XUnit: Framework de testes unitários para .NET.
  MSTest: Framework de testes unitários da Microsoft.

- Banco de dados: NoSQL e o escolhido é o MongoDB

- API: ASP.NET Core: Framework web para .NET.

- Swagger: Ferramenta de documentação de APIs RESTful.

- Injeção de dependência: Autofac: Framework de injeção de dependência para .NET.

Os testes unitários cobrem as funcionalidades da API, incluindo a validação de dados, o tratamento de erros e o desempenho.

O banco de dados utilizado é o MongoDB.

A API é implementada usando o ASP.NET Core. O Swagger é usado para documentar as rotas e os métodos da API.

A injeção de dependência é usada para desacoplar os componentes da API. O Autofac é o framework de injeção de dependência utilizado.

###Configuração do MongoDB

Durante a instalação do MongoDB, optamos por executar o serviço como usuário de serviço de rede. Essa decisão foi tomada para simplificar o processo de conexão ao banco de dados e facilitar o desenvolvimento e a demonstração do projeto.

Executar o serviço como usuário de serviço de rede permite que o MongoDB seja iniciado com permissões limitadas, o que é adequado para ambientes de desenvolvimento e uso pessoal. Isso elimina a necessidade de configurar um login e senha específicos para acessar o banco de dados, tornando o processo de desenvolvimento mais rápido e direto.

Se você estiver implantando este projeto em um ambiente de produção ou se a segurança for uma preocupação significativa, recomendamos revisar e ajustar as configurações de segurança do MongoDB de acordo com as necessidades específicas do seu ambiente.

Para mais informações sobre a instalação e configuração do MongoDB, consulte a documentação oficial do MongoDB.

### Segurança

Não foi pensado em segurança para esse projeto

## Histórico de Atualizações

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

## Estrutura das pastas do projeto

```plaintext
BusTrack
├── BusTrack.API
│   ├── ControllersAPI
│   │   ├── AccountControllerAPI.cs
│   │   ├── AuthenticationControllerAPI.cs
│   │   ├── BusControllerAPI.cs
│   │   ├── CreateAccountControllerAPI.cs
│   │   ├── DashboardControllerAPI.cs
│   │   ├── DriverControllerAPI.cs
│   │   ├── PassengerControllerAPI.cs
│   │   ├── RouteControllerAPI.cs
│   │   ├── TripControllerAPI.cs
│   │   ├── TripsPassengerControllerAPI.cs
│   │   └── UserControllerAPI.cs
│   ├── DTOAPI
│   │   ├── BusDTOAPI.cs
│   │   ├── DriverDTOAPI.cs
│   │   ├── PassengerDTOAPI.cs
│   │   ├── RouteDTOAPI.cs
│   │   ├── TripDTOAPI.cs
│   │   └── TripPassengerDTOAPI.cs
│   ├── InterfacesAPI
│   │   ├── ServicesAPI
│   │   │   ├── IAccountServiceAPI.cs
│   │   │   ├── IBusServiceAPI.cs
│   │   │   ├── IDriverServiceAPI.cs
│   │   │   ├── IEmailConfirmationServiceAPI.cs
│   │   │   ├── IPassengerServiceAPI.cs
│   │   │   ├── IRouteServiceAPI.cs
│   │   │   ├── ITripServiceAPI.cs
│   │   │   ├── ITripsPassengerServiceAPI.cs
│   │   │   ├── IUserAuthenticationServiceAPI.cs
│   │   │   └── IUserServiceAPI.cs
│   ├── MappingsAPI
│   │   └── MappingProfileAPI.cs
│   ├── ModelsAPI
│   │   ├── AccountModelAPI.cs
│   │   ├── BusModelAPI.cs
│   │   ├── DriverModelAPI.cs
│   │   ├── PassengerModelAPI.cs
│   │   ├── RouteModelAPI.cs
│   │   ├── TripModelAPI.cs
│   │   ├── TripsPassengerModelAPI.cs
│   │   └── UserModelAPI.cs
│   ├── ServicesAPI
│   │   └── AccountServiceAPI.cs
│   │   ├── BusServiceAPI.cs
│   │   ├── DriverServiceAPI.cs
│   │   ├── PassengerServiceAPI.cs
│   │   ├── RouteServiceAPI.cs
│   │   ├── TripServiceAPI.cs
│   │   ├── TripsPassengerServiceAPI.cs
│   │   ├── UserAuthenticationServiceAPI.cs
│   │   └── UserServiceAPI.cs
├── BusTrack.DB
│   ├── ClassesDB
│   │   ├── BusDB.cs
│   │   ├── DriverDB.cs
│   │   ├── EmailConfirmationDB
│   │   ├── InspectorDB
│   │   ├── LoginDB
│   │   ├── PassengerDB.cs
│   │   ├── PasswordRecordDB.cs
│   │   ├── RouteDB.cs
│   │   ├── TripDB.cs
│   │   ├── TripsPassengerDB.cs
│   │   └── UserDB.cs
│   │   └── UserRegistrationDB.cs
│   ├── ConnectionsDB
│   │   └── ConnectionDB.cs
│   ├── DataBaseDB
│   │   └── database.json
│   ├── InterfacesDB
│   │   ├── IModelsDB
│   │   │   ├── IBusModelDB.cs
│   │   │   ├── IDriverModelDB.cs
│   │   │   ├── IPassengerModelDB.cs
│   │   │   ├── IRouteModelDB.cs
│   │   │   ├── ITripModelDB.cs
│   │   │   └── ITripsPassengerModelDB.cs
│   │   └── IRepositoriesDB
│   │   │    ├── IBusRepositoryDB.cs
│   │   │    ├── IDriverRepositoryDB.cs
│   │   │    ├── IPassengerRepositoryDB.cs
│   │   │    ├── IRouteRepositoryDB.cs
│   │   │    ├── ITripRepositoryDB.cs
│   │   │    ├── ITripsPassengerRepositoryDB.cs
│   │   │    └── IUserRepositoryDB
│   ├── ModelsDB
│   │   ├── AccountModelDB.cs
│   │   ├── BusModelDB.cs
│   │   ├── DriverModelDB.cs
│   │   ├── PassengerModelDB.cs
│   │   ├── RouteModelDB.cs
│   │   ├── TripModelDB.cs
│   │   ├── TripsPassengerModelDB.cs
│   │   └── UserModelDB.cs
│   ├── RepositoriesDB
│   │   ├── BusRepositoryDB.cs
│   │   ├── DriverRepositoryDB.cs
│   │   ├── InspectorRepositoryDB.cs
│   │   ├── PassengerRepositoryDB.cs
│   │   ├── RouteRepositoryDB.cs
│   │   ├── TripRepositoryDB.cs
│   │   ├── TripsPassengerRepositoryDB.cs
│   │   └── UserRepositoryDB.cs
│   ├── ServicesDB
│   │   ├── BusSingleTripConstraintServiceDB.cs
│   │   ├── DepartureTimeValidationServiceDB.cs
│   │   ├── MinTripDurationContraintServiceDB.cs
│   │   ├── PassengerLimitValidationServiceDB.cs
│   │   ├── TripMappingServiceDB.cs
│   │   ├── TripServiceDB.cs
│   │   └── TripStatusUpdateServiceDB.cs
│   └── ...
├── BusTrack.Front.Angular
│   ├── bustrack.frontend.client
│   │   ├── src
│   │   │   ├── app
│   │   │   │   ├── login
│   │   │   │   │   ├── concluded/
│   │   │   │   │   │   ├── concluded.component.css
│   │   │   │   │   │   ├── concluded.component.html
│   │   │   │   │   │   ├── concluded.component.spec.ts
│   │   │   │   │   │   ├── concluded.component.ts
│   │   │   │   │   │   ├── OIG4.jpgeg
│   │   │   │   │   ├── confirmation/
│   │   │   │   │   │   ├── confirmation.component.css
│   │   │   │   │   │   ├── confirmation.component.html
│   │   │   │   │   │   ├── confirmation.component.spec.ts
│   │   │   │   │   │   ├── confirmation.component.ts
│   │   │   │   │   │   ├── OIG4.jpgeg
│   │   │   │   │   ├── create-an-account
│   │   │   │   │   │   ├── create-an-account.component.css
│   │   │   │   │   │   ├── create-an-account.component.html
│   │   │   │   │   │   ├── create-an-account.component.spec.ts
│   │   │   │   │   │   ├── create-an-account.component.ts
│   │   │   │   │   │   ├── OIG4.jpgeg
│   │   │   │   │   ├── enter-the-system
│   │   │   │   │   │   ├── enter-the-system.component.css
│   │   │   │   │   │   ├── enter-the-system.component.html
│   │   │   │   │   │   ├── enter-the-system.component.spec.ts
│   │   │   │   │   │   ├── enter-the-system.component.ts
│   │   │   │   │   │   ├── OIG4.jpgeg
│   │   │   │   │   ├── main-screen        # Pasta contendo
│   │   │   │   │   │   ├── main-screen.component.css
│   │   │   │   │   │   ├── main-screen.component.html
│   │   │   │   │   │   ├── main-screen.component.spec.ts
│   │   │   │   │   │   ├── main-screen.component.ts
│   │   │   │   │   │   ├── OIG4.jpeg
│   │   │   │   │   ├── rules
│   │   │   │   │   │   ├── blockSavePasswordRules
│   │   │   │   │   │   │   ├── blockSavePasswordRule.ts
│   │   │   │   │   │   ├── checkPasswordStrengthpasswordRules
│   │   │   │   │   │   │   ├── checkPasswordStrengthpasswordRule.ts
│   │   │   │   │   │   ├── disableInteractionsRules
│   │   │   │   │   │   │   ├── disableInteractionsRule.ts
│   │   │   │   │   │   ├── disableKeyboardShortcutsRules
│   │   │   │   │   │   │   ├── disableKeyboardShortcutsRule.ts
│   │   │   │   │   │   ├── hasRepeatedOrSequentialNumbersRules
│   │   │   │   │   │   │   ├── hasRepeatedOrSequentialNumbersRule.ts
│   │   │   │   │   │   ├── inactivityTimerRules
│   │   │   │   │   │   │   ├── inactivityTimerRule.ts
│   │   │   │   │   │   ├── preventBackNavigationRules
│   │   │   │   │   │   │   ├── preventBackNavigationRule.ts
│   │   │   │   │   │   ├── preventForwardNavigationRules
│   │   │   │   │   │   │   ├── preventForwardNavigationRule.ts
│   │   │   │   │   │   ├── sessionTimeoutRules
│   │   │   │   │   │   │   ├── sessionTimeoutRule.ts
│   │   │   │   │   │   ├── startInactivityTimerRules
│   │   │   │   │   │   │   ├── startInactivityTimerRule.ts
│   │   │   │   │   ├── update-password
│   │   │   │   │   │   ├── OIG4.jpeg
│   │   │   │   │   │   ├── update-password.component.css
│   │   │   │   │   │   ├── update-password.component.html
│   │   │   │   │   │   ├── update-password.component.spec
│   │   │   │   │   │   ├── update-password.component.ts
│   │   │   │   │   ├── login.component.css
│   │   │   │   │   ├── login.component.html
│   │   │   │   │   ├── login.component.spec.ts
│   │   │   │   │   ├── login.component.ts
│   │   │   │   │   ├── OIG4.jpeg
│   │   │   │   ├── main
│   │   │   │   │   ├── dashboard
│   │   │   │   │   │   ├── dashboard.component.css
│   │   │   │   │   │   ├── dashboard.component.html
│   │   │   │   │   │   ├── dashboard.component.spec.ts
│   │   │   │   │   │   ├── dashboard.component.ts
│   │   │   │   │   │   ├── OIG4.jpeg
│   │   │   │   │   ├── rules-main
│   │   │   │   │   │   ├── limitCharactersRules
│   │   │   │   │   │   │   ├── limitCharactersRule.ts
│   │   │   │   │   │   ├── validateEmailFormatRules
│   │   │   │   │   │   │   ├── validateEmailFormatRule.ts
│   │   │   │   │   │   ├── validateFieldsRequiredRules
│   │   │   │   │   │   │   ├── validateFieldsRequiredRule.ts
│   │   │   │   │   ├── sidebar
│   │   │   │   │   │   ├── sidebar.component.css
│   │   │   │   │   │   ├── sidebar.component.html
│   │   │   │   │   │   ├── sidebar.component.spec.ts
│   │   │   │   │   │   ├── sidebar.component.ts
│   ├── BusTrack.Frontend.Server
│   │    └── ...
│   └── ...
├── BusTrack.Program
│   ├── DataBaseServicesExtensionsProgram
│   │   └── DataBaseServicesExtensionsProgram.cs
│   ├── ExtensionsProgram
│   │   └── ExtensionsProgram.cs
│   │   └── ServiceExtensionProgram.cs
│   ├── MiddlewareProgram
│   │   └── ErrorHandlingMiddleware.cs
│   └── Program.cs
├── BusTrack.Tests
│   ├── IntegrationTests
│   │   ├── CustomWebApplicationFactory
│   │   │   └── CustomWebApplicationFactory.cs
│   │   ├── ControllersAPIIntegrationTests
│   │   │   ├── BusControllerAPIIntegrationTest.cs
│   │   │   ├── DriverControllerAPIIntegrationTest.cs
│   │   │   ├── PassengerControllerAPIIntegrationTest.cs
│   │   │   ├── RouteControllerAPIIntegrationTest.cs
│   │   │   ├── TripControllerAPIIntegrationTest.cs
│   │   │   └── TripsPassengerControllerAPIIntegrationTest.cs
│   ├── ServicesAPIIntegrationTests
│   │   ├── BusServiceAPIIntegrationTest.cs
│   │   ├── DriverServiceAPIIntegrationTest.cs
│   │   ├── PassengerServiceAPIIntegrationTest.cs
│   │   ├── RouteServiceAPIIntegrationTest.cs
│   │   ├── TripServiceAPIIntegrationTest.cs
│   │   └── TripsPassengerServiceAPIIntegrationTest.cs
│   ├── PerformanceTests
│   │   ├── PassengerServiceAPIPerformanceTests.cs
│   │   └── RouteServiceAPIPerformanceTests.cs
│   ├── UnitTests
│   │   ├── ControllersAPIUnitTests
│   │   │   ├── BusControllerAPIUnitTests.cs
│   │   │   ├── DriverControllerAPIUnitTests.cs
│   │   │   ├── PassengerControllerAPIUnitTests.cs
│   │   │   ├── RouteControllerAPIUnitTests.cs
│   │   │   ├── TripControllerAPIUnitTests.cs
│   │   │   └── TripsPassengerControllerAPIUnitTests.cs
│   └── ...  # Outros testes unitários (arquivos não mostrados).
│   ├── UsabilityTests
│   │   └── UsabilityTests.cs
├── BusTrack.Updater
│  ├── DriversUpdater
│  │  └── DriverNameUpdater.cs
│  ├── PassengerUpdater
│  │  └── PassengerNameUpdater.cs
│  └── ...      # Outras subpastas para diferentes tipos de atualização (se necessário)
└── BusTrack.sln
```

## Estrutura do Projeto

O projeto está dividido em duas partes interligadas:

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
Para usar a API, você pode usar uma ferramenta como o Postman ou o Insomnia para enviar solicitações HTTP à API.

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
