# Estrutura do Projeto

Este documento apresenta a estrutura de diretórios e arquivos do **Bus Track**, funcionando como um mapa de navegação do repositório.

Enquanto a documentação de [arquitetura](architecture.md) explica **como o sistema é organizado, como suas camadas se relacionam e quais responsabilidades pertencem a cada parte**, este documento tem um objetivo mais direto:

> **mostrar onde cada parte do projeto está localizada.**

A ideia é que este arquivo possa ser utilizado como uma referência rápida durante o desenvolvimento, manutenção e evolução do sistema.

---

## Visão geral

O repositório está organizado em diferentes projetos e áreas, cada uma responsável por uma parte específica da aplicação.

```text
BusTrack/
│
├── BusTrack.API/
├── BusTrack.DB/
├── BusTrack.Frontend/
├── BusTrack.Program/
├── BusTrack.Tests/
├── BusTrack.Updater/
│
├── docs/
├── docker/
│
├── appsettings.json
├── BusTrack.sln
├── BusTrack.csproj
├── Dockerfile
├── docker-compose.yml
├── README.md
└── LICENSE.md
```

As principais áreas são:

| Diretório           | Responsabilidade                                               |
| ------------------- | -------------------------------------------------------------- |
| `BusTrack.API`      | Controllers, DTOs, serviços, modelos e contratos da API        |
| `BusTrack.DB`       | Persistência, entidades, repositórios e acesso ao MongoDB      |
| `BusTrack.Frontend` | Aplicação web desenvolvida com Angular                         |
| `BusTrack.Program`  | Inicialização da aplicação e configuração da infraestrutura    |
| `BusTrack.Tests`    | Testes automatizados                                           |
| `BusTrack.Updater`  | Componentes responsáveis por atualizações específicas de dados |
| `docs`              | Documentação técnica do projeto                                |
| `docker`            | Arquivos relacionados à execução e configuração com Docker     |

---

# BusTrack.API

```text
BusTrack.API/
│
├── ControllersAPI/
├── DTOAPI/
├── InterfacesAPI/
│   └── IServicesAPI/
├── MappingsAPI/
├── ModelsAPI/
└── ServicesAPI/
```

A pasta `BusTrack.API` concentra os componentes relacionados à camada de aplicação da API.

## ControllersAPI

```text
BusTrack.API/
└── ControllersAPI/
```

Contém os controllers responsáveis por receber as requisições HTTP e encaminhá-las para os serviços correspondentes.

Exemplos de funcionalidades encontradas nessa área:

```text
BusControllerAPI
DriverControllerAPI
PassengerControllerAPI
RouteControllerAPI
TripControllerAPI
TripsPassengerControllerAPI
AuthenticationControllerAPI
AccountControllerAPI
CreateAccountControllerAPI
DashboardControllerAPI
HealthController
```

Quando a dúvida for:

> "Onde está o endpoint responsável por determinada funcionalidade?"

o primeiro lugar para procurar é:

```text
BusTrack.API/ControllersAPI/
```

---

## DTOAPI

```text
BusTrack.API/
└── DTOAPI/
```

Contém os **Data Transfer Objects (DTOs)** utilizados na comunicação da API.

Os DTOs representam os dados que entram e saem da camada de API sem necessariamente expor diretamente as classes utilizadas internamente pela persistência.

Exemplos:

```text
BusDTOAPI
DriverDTOAPI
PassengerDTOAPI
RouteDTOAPI
TripDTOAPI
...
```

---

## InterfacesAPI

```text
BusTrack.API/
└── InterfacesAPI/
    └── IServicesAPI/
```

Contém os contratos das operações disponibilizadas pelos serviços da API.

Quando existir uma implementação como:

```text
ServicesAPI/TripServiceAPI.cs
```

seu contrato correspondente deverá ser procurado em:

```text
InterfacesAPI/IServicesAPI/ITripServiceAPI.cs
```

Essa relação facilita a localização entre **contrato e implementação**.

---

## MappingsAPI

```text
BusTrack.API/
└── MappingsAPI/
```

Contém as configurações utilizadas pelo **AutoMapper** para realizar conversões entre os diferentes modelos utilizados pela aplicação.

Exemplos de conversões:

```text
DTO
 ↓
Model
 ↓
DB
```

e no caminho inverso:

```text
DB
 ↓
Model / DTO
 ↓
API
```

---

## ModelsAPI

```text
BusTrack.API/
└── ModelsAPI/
```

Contém modelos utilizados pela camada de API e pela representação dos dados durante o processamento das operações.

---

## ServicesAPI

```text
BusTrack.API/
└── ServicesAPI/
```

Contém os serviços responsáveis pela execução das operações da aplicação.

Exemplos:

```text
BusServiceAPI.cs
DriverServiceAPI.cs
PassengerServiceAPI.cs
RouteServiceAPI.cs
TripServiceAPI.cs
TripsPassengerServiceAPI.cs
UpdatePasswordServiceAPI.cs
...
```

Se você estiver procurando a implementação de uma regra ou operação relacionada a viagens, por exemplo:

```text
BusTrack.API/
└── ServicesAPI/
    └── TripServiceAPI.cs
```

---

# BusTrack.DB

```text
BusTrack.DB/
│
├── ClassesDB/
├── ConnectionsDB/
├── InterfacesDB/
├── ModelsDB/
├── RepositoriesDB/
└── ServicesDB/
```

A pasta `BusTrack.DB` concentra os componentes relacionados à persistência e ao acesso ao MongoDB.

---

## ClassesDB

```text
BusTrack.DB/
└── ClassesDB/
```

Contém as classes que representam as entidades persistidas no banco de dados.

Exemplos:

```text
BusDB
DriverDB
PassengerDB
RouteDB
TripDB
...
```

Quando a dúvida for:

> "Qual classe representa essa entidade no banco?"

procure primeiro em:

```text
BusTrack.DB/ClassesDB/
```

---

## InterfacesDB

```text
BusTrack.DB/
└── InterfacesDB/
```

Contém os contratos relacionados à camada de persistência.

Dentro dela estão os contratos utilizados pelos repositórios e pelos demais componentes que dependem do acesso aos dados.

---

## ModelsDB

```text
BusTrack.DB/
└── ModelsDB/
```

Contém modelos utilizados durante o processamento dos dados da camada de banco.

---

## RepositoriesDB

```text
BusTrack.DB/
└── RepositoriesDB/
```

Contém os repositórios responsáveis pela comunicação com as coleções do MongoDB.

Exemplos:

```text
BusRepositoryDB.cs
DriverRepositoryDB.cs
PassengerRepositoryDB.cs
RouteRepositoryDB.cs
TripRepositoryDB.cs
...
```

Se você estiver procurando:

> "Onde está o código que realmente consulta ou altera o MongoDB?"

procure em:

```text
BusTrack.DB/RepositoriesDB/
```

---

## ServicesDB

```text
BusTrack.DB/
└── ServicesDB/
```

Contém componentes relacionados a operações e regras específicas da camada de dados que permanecem nessa área do projeto.

---

# BusTrack.Frontend

```text
BusTrack.Frontend/
└── bustrack.frontend.client/
    └── src/
        ├── app/
        ├── environments/
        └── ...
```

O frontend do Bus Track é desenvolvido utilizando **Angular**.

A aplicação está organizada principalmente dentro de:

```text
BusTrack.Frontend/
└── bustrack.frontend.client/
```

---

## src/app

```text
src/
└── app/
```

É o principal ponto de organização da aplicação Angular.

Dentro dela ficam as áreas relacionadas às telas, componentes, serviços, guards e demais funcionalidades do frontend.

Entre as áreas existentes estão:

```text
login/
main/
guards/
services/
```

---

## login

```text
src/app/login/
```

Concentra as funcionalidades relacionadas ao fluxo de autenticação e acesso ao sistema.

Exemplos:

```text
main-screen/
enter-the-system/
create-an-account/
update-password/
confirmation/
concluded/
```

---

## main

```text
src/app/main/
```

Concentra as funcionalidades da área autenticada da aplicação.

Entre as áreas existentes estão:

```text
dashboard/
sidebar/
trips/
buses/
```

Outros módulos poderão ser incorporados nessa área conforme o sistema evoluir.

---

## guards

```text
src/app/guards/
```

Contém os guards responsáveis pelo controle de navegação e autenticação.

Exemplos:

```text
authentication.guard.ts
browser-navigation.guard.ts
```

Se a dúvida for:

> "Onde está o bloqueio de acesso de usuário não autenticado?"

procure em:

```text
src/app/guards/authentication.guard.ts
```

Se a dúvida for:

> "Onde está o bloqueio dos botões Voltar e Avançar do navegador?"

procure em:

```text
src/app/guards/browser-navigation.guard.ts
```

---

## services

```text
src/app/services/
```

Contém os serviços utilizados pelo frontend para comunicação com a API e gerenciamento de funcionalidades compartilhadas.

Exemplos:

```text
bus.service.ts
trip.service.ts
session.service.ts
application-availability.service.ts
...
```

Se uma tela precisa buscar ou enviar dados para a API, normalmente o serviço correspondente estará nessa pasta.

---

## environments

```text
src/environments/
```

Contém as configurações específicas de cada ambiente.

Exemplos:

```text
environment.ts
environment.prod.ts
```

A configuração de desenvolvimento utiliza a API local, enquanto a configuração de produção aponta para a API publicada.

---

# BusTrack.Program

```text
BusTrack.Program/
│
├── DataBaseServicesExtensionsProgram/
├── ExtensionsProgram/
└── Program.cs
```

O projeto `BusTrack.Program` concentra a inicialização da aplicação e configurações de infraestrutura.

O arquivo principal é:

```text
BusTrack.Program/Program.cs
```

É nele que são configurados componentes como:

* Controllers
* AutoMapper
* CORS
* MongoDB
* Injeção de dependência
* serviços da aplicação
* middleware
* Swagger
* execução da aplicação

As extensões de configuração ficam organizadas nas respectivas pastas.

---

# BusTrack.Tests

```text
BusTrack.Tests/
│
├── IntegrationTests/
├── UnitTests/
├── UsabilityTests/
└── ...
```

Contém os testes automatizados do projeto.

## IntegrationTests

```text
BusTrack.Tests/
└── IntegrationTests/
```

Contém testes que verificam a integração entre diferentes componentes da aplicação.

Entre as áreas estão:

```text
ControllersAPIIntegrationTests/
ServicesAPIIntegrationTests/
MappingsIntegrationTests/
CustomWebApplicationFactory/
```

---

## UnitTests

```text
BusTrack.Tests/
└── UnitTests/
```

Contém testes unitários de componentes isolados.

Exemplo:

```text
ControllersAPIUnitTests/
```

---

## UsabilityTests

```text
BusTrack.Tests/
└── UsabilityTests/
```

Contém testes relacionados à interação e ao comportamento da aplicação utilizando ferramentas de automação.

---

# BusTrack.Updater

```text
BusTrack.Updater/
```

Contém componentes responsáveis por operações específicas de atualização de dados.

Exemplos:

```text
DriverNameUpdater
PassengerNameUpdater
```

Quando uma funcionalidade estiver relacionada a uma transformação ou atualização específica encapsulada nessa área, procure primeiro neste projeto.

---

# docs

```text
docs/
│
├── architecture.md
├── project-structure.md
└── dotnet-10-migration.md
```

A pasta `docs` concentra a documentação técnica do projeto.

### architecture.md

Explica **como a aplicação é arquitetada**, incluindo:

* organização das camadas;
* responsabilidades;
* comunicação entre componentes;
* fluxo entre frontend, API e banco;
* decisões estruturais;
* critérios utilizados para organização do código.

[Ver documentação da arquitetura](architecture.md)

### project-structure.md

Este documento.

Seu objetivo é funcionar como um **mapa de navegação do código**, permitindo localizar rapidamente pastas e arquivos.

### dotnet-10-migration.md

Documenta o processo de migração do projeto para o **.NET 10**, incluindo alterações e decisões relacionadas à atualização da solução.

---

# Arquivos da raiz

Alguns arquivos importantes ficam diretamente na raiz do repositório.

```text
BusTrack/
├── appsettings.json
├── BusTrack.sln
├── BusTrack.csproj
├── Dockerfile
├── docker-compose.yml
├── README.md
└── LICENSE.md
```

## appsettings.json

Contém configurações padrão da aplicação.

As configurações que precisam variar entre ambientes podem ser sobrescritas por variáveis de ambiente.

## BusTrack.sln

Arquivo da solução .NET que organiza os projetos que fazem parte da solução.

## BusTrack.csproj

Arquivo de configuração principal do projeto raiz.

## Dockerfile

Define a construção da imagem utilizada para empacotar a aplicação no ambiente de execução.

## docker-compose.yml

Utilizado principalmente para orquestrar o ambiente local de desenvolvimento.

A utilização do Docker Compose local não significa que o mesmo arquivo precise ser utilizado pelo ambiente de produção.

## README.md

É a porta de entrada da documentação do projeto.

Este arquivo apresenta o sistema de forma geral e direciona para as documentações técnicas mais específicas.

---

# Como encontrar um arquivo

Este documento foi criado também para servir como um índice de navegação.

Por exemplo, se você estiver procurando:

### Serviço de viagens

```text
BusTrack.API/
└── ServicesAPI/
    └── TripServiceAPI.cs
```

### Controller de viagens

```text
BusTrack.API/
└── ControllersAPI/
    └── TripControllerAPI.cs
```

### Repositório de viagens

```text
BusTrack.DB/
└── RepositoriesDB/
    └── TripRepositoryDB.cs
```

### Serviço de viagens no frontend

```text
BusTrack.Frontend/
└── bustrack.frontend.client/
    └── src/
        └── app/
            └── services/
                └── trip.service.ts
```

### Guard de autenticação

```text
BusTrack.Frontend/
└── bustrack.frontend.client/
    └── src/
        └── app/
            └── guards/
                └── authentication.guard.ts
```

### Configuração do ambiente de produção

```text
BusTrack.Frontend/
└── bustrack.frontend.client/
    └── src/
        └── environments/
            └── environment.prod.ts
```

---

# Regra de navegação

Quando estiver procurando algo no Bus Track, utilize esta lógica:

```text
Preciso entender COMO o sistema funciona?
                │
                ▼
        docs/architecture.md


Preciso descobrir ONDE está algo?
                │
                ▼
      docs/project-structure.md


Preciso saber O QUE é o projeto?
                │
                ▼
             README.md
```

Abaixo todos os arquivos e pastas atualizados do projeto num todo

```text
BusTrack                                   
├── BusTrack.API                                  
│   ├── ControllersAPI                             
│   │   ├── AccountControllerAPI.cs                     
│   │   ├── AuthenticationControllerAPI.cs            
│   │   ├── BusControllerAPI.cs                       
│   │   ├── CreateAccountControllerAPI.cs             
│   │   ├── DashboardControllerAPI.cs                
│   │   ├── DriverControllerAPI.cs
│   │   ├── HealthController.cs                   
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
│   │   ├── TripDetailsDTOAPI.cs                           
│   │   ├── TripPassengerDTOAPI.cs                  
│   │   └── UpdatePasswordDTOAPI.cs                     
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
│   │   │   ├── IUpdatePasswordServiceAPI.cs                
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
│   └── ServicesAPI                                 
│   │   ├── AccountServiceAPI.cs                     
│   │   ├── BusServiceAPI.cs                       
│   │   ├── DriverServiceAPI.cs                      
│   │   ├── PassengerServiceAPI.cs                  
│   │   ├── RouteServiceAPI.cs                 
│   │   ├── TripServiceAPI.cs                        
│   │   ├── TripsPassengerServiceAPI.cs         
│   │   ├── UpdatePasswordServiceAPI.cs               
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
│   │   ├── PasswordHistoryDB.cs                       
│   │   ├── PasswordRecordDB.cs                   
│   │   ├── RouteDB.cs                             
│   │   ├── TripDB.cs                                 
│   │   ├── TripsPassengerDB.cs                       
│   │   ├── UserDB.cs                               
│   │   ├── UserPasswordHistoryDB.cs                
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
│   │   │   ├── IBusRepositoryDB.cs                      
│   │   │   ├── IDriverRepositoryDB.cs                   
│   │   │   ├── IPassengerRepositoryDB.cs                
│   │   │   ├── IRouteRepositoryDB.cs                    
│   │   │   ├── ITripRepositoryDB.cs                    
│   │   │   ├── ITripsPassengerRepositoryDB.cs         
│   │   │   └── IUserRepositoryDB.cs                      
│   ├── ModelsDB                                     
│   │   ├── AccountModelDB.cs                           
│   │   ├── BusModelDB.cs                          
│   │   ├── DriverModelDB.cs                        
│   │   ├── PassengerModelDB.cs                     
│   │   ├── RouteModelDB.cs                            
│   │   ├── TripModelDB.cs                             
│   │   └── TripsPassengerModelDB.cs                  
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
│   │   ├── PasswordHistoryValidationServiceDB.cs     
│   │   ├── RouteConflictServiceDB.cs                  
│   │   ├── RouteSchedulerServiceDB.cs                
│   │   ├── TripMappingServiceDB.cs                   
│   │   ├── TripServiceDB.cs                            
│   │   ├── TripStatusUpdateServiceDB.cs               
│   │   ├── UserEmailValidationServiceDB.cs          
│   │   └── UserRoleValidationServiceDB.cs             
├── BusTrack.Frontend                            
│   ├── bustrack.frontend.client
│   │   ├── src                                
│   │   │   └── app
│   │   │   │   ├── guards
│   │   │   │   │   ├── authentication.guard.ts
│   │   │   │   │   └── browser-navigation.guard.ts                           
│   │   │   │   ├── login                      
│   │   │   │   │   │   ├── concluded              
│   │   │   │   │   │   │   ├── concluded.component.css   
│   │   │   │   │   │   │   ├── concluded.component.html   
│   │   │   │   │   │   │   ├── concluded.component.spec.ts   
│   │   │   │   │   │   │   └── concluded.component.ts   
│   │   │   │   │   ├── confirmation            
│   │   │   │   │   │   ├── confirmation.component.css   
│   │   │   │   │   │   ├── confirmation.component.html  
│   │   │   │   │   │   ├── confirmation.component.spec.ts  
│   │   │   │   │   │   └── confirmation.component.ts   
│   │   │   │   │   ├── create-an-account       
│   │   │   │   │   │   ├── create-an-account.component.css   
│   │   │   │   │   │   ├── create-an-account.component.html  
│   │   │   │   │   │   ├── create-an-account.component.spec.ts  
│   │   │   │   │   │   └── create-an-account.component.ts   
│   │   │   │   │   ├── enter-the-system        
│   │   │   │   │   │   ├── enter-the-system.component.css  
│   │   │   │   │   │   ├── enter-the-system.component.html  
│   │   │   │   │   │   ├── enter-the-system.component.spec.ts    
│   │   │   │   │   │   └── enter-the-system.component.ts   
│   │   │   │   │   ├── main-screen             
│   │   │   │   │   │   ├── main-screen.component.css   
│   │   │   │   │   │   ├── main-screen.component.html   
│   │   │   │   │   │   ├── main-screen.component.spec.ts   
│   │   │   │   │   │   └── main-screen.component.ts   
│   │   │   │   │   ├── rules               
│   │   │   │   │   │   ├── blockCopyRules       
│   │   │   │   │   │   │   └── blockCopyRule.ts   
│   │   │   │   │   │   ├── blockSavePasswordRules   
│   │   │   │   │   │   │   └── blockSavePasswordRule.ts   
│   │   │   │   │   │   ├── checkPasswordStrengthpasswordRules  
│   │   │   │   │   │   │   └── checkPasswordStrengthpasswordRule.ts   
│   │   │   │   │   │   ├── disableInteractionsRules    
│   │   │   │   │   │   │   └── disableInteractionsRule.ts  
│   │   │   │   │   │   ├── disableKeyboardShortcutsRules   
│   │   │   │   │   │   │   └── disableKeyboardShortcutsRule.ts  
│   │   │   │   │   │   ├── hasRepeatedOrSequentialNumbersRules    
│   │   │   │   │   │   │   └── hasRepeatedOrSequentialNumbersRule.ts  
│   │   │   │   │   │   ├── inactivityTimerRules                  
│   │   │   │   │   │   │   └── inactivityTimerRule.ts               
│   │   │   │   │   │   ├── preventBackNavigationRules           
│   │   │   │   │   │   │   └── preventBackNavigationRule.ts       
│   │   │   │   │   │   ├── preventForwardNavigationRules         
│   │   │   │   │   │   │   └── preventForwardNavigationRule.ts      
│   │   │   │   │   │   ├── sessionTimeoutRules                    
│   │   │   │   │   │   │   └── sessionTimeoutRule.ts               
│   │   │   │   │   ├── update-password                      
│   │   │   │   │   │   ├── update-password.component.css         
│   │   │   │   │   │   ├── update-password.component.html        
│   │   │   │   │   │   ├── update-password.component.spec.ts      
│   │   │   │   │   │   └── update-password.component.ts           
│   │   │   │   ├── main
│   │   │   │   │   ├──buses
│   │   │   │   │   │   ├── buses.component.css
│   │   │   │   │   │   ├──buses.component.html
│   │   │   │   │   │   └──buses.component.ts                                
│   │   │   │   │   ├── dashboard                             
│   │   │   │   │   │   ├── dashboard.component.css             
│   │   │   │   │   │   ├── dashboard.component.html             
│   │   │   │   │   │   ├── dashboard.component.spec.ts         
│   │   │   │   │   │   └── dashboard.component.ts            
│   │   │   │   │   ├── rules-main                             
│   │   │   │   │   │   ├── limitCharactersRules                   
│   │   │   │   │   │   │   └── limitCharactersRule.ts               
│   │   │   │   │   │   ├── validateEmailFormatRules             
│   │   │   │   │   │   │   └── validateEmailFormatRule.ts             
│   │   │   │   │   │   ├── validateFieldsRequiredRules            
│   │   │   │   │   │   │   └── validateFieldsRequiredRule.ts       
│   │   │   │   │   ├── sidebar                               
│   │   │   │   │   │   ├── sidebar.component.css                
│   │   │   │   │   │   ├── sidebar.component.html                
│   │   │   │   │   │   ├── sidebar.component.spec.ts           
│   │   │   │   │   │   └── sidebar.component.ts 
│   │   │   │   │   └── trips
│   │   │   │   │   │   ├── tripscomponent.css
│   │   │   │   │   │   ├── tripscomponent.html
│   │   │   │   │   │   └── tripscomponent.ts
│   │   │   │   ├── models                                  
│   │   │   │   │   └── user.model.ts                        
│   │   │   │   ├── services
│   │   │   │   │   │   ├── application-availability.service.ts
│   │   │   │   │   │   └── bus.service.ts                              
│   │   │   │   │   ├── data.service.ts
│   │   │   │   │   ├── session.service.ts
│   │   │   │   │   ├── trip.service.ts                    
│   │   │   │   │   ├── user.service.ts                        
│   │   │   │   │   └── validation.service.ts                
│   │   │   │   ├── app-routing.module.ts                  
│   │   │   │   ├── app.component.css                       
│   │   │   │   ├── app.component.html                       
│   │   │   │   ├── app.component.ts                        
│   │   │   │   └── app.module.ts                         
│   │   │   ├── assets                                   
│   │   │   │   └── imagem                                   
│   │   │   │   │   └── OIG4.jpeg
│   │   │   └── environments/
│   │   │   │   ├── environment.ts
│   │   │   │   └── environment.prod.ts
│   │   ├── angular.json
│   │   ├── bustrack.frontend.client.esproj
│   │   ├── OIG4.jpeg
│   │   ├── package-lock.json
│   │   ├── package.json
│   │   ├── tsconfig.app.json
│   │   └── tsconfig.json                      
│   └── BusTrack.Frontend.Server
│   │   ├── Properties/
│   │   ├── BusTrack.Frontend.Server.csproj
│   │   └── Program.cs            
├── BusTrack.Program                             
│   ├── DataBaseServicesExtensionsProgram           
│   │   └── DataBaseServicesExtensionsProgram.cs        
│   ├── ExtensionsProgram                            
│   │   ├── ExtensionsProgram.cs                        
│   │   └── ServiceExtensionProgram.cs                  
│   ├── MiddlewareProgram                          
│   │   └── ErrorHandlingMiddleware.cs                
│   └── Program.cs                                 
├── BusTrack.Tests                              
│   ├── IntegrationTests                          
│   │   └── CustomWebApplicationFactory                
│   │       └── CustomWebApplicationFactory.cs          
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
│   └── UsabilityTests                               
│   │   └── UsabilityTests.cs           
├── BusTrack.Updater                           
│   ├── DriversUpdater                               
│   │   └── DriverNameUpdater.cs                       
│   └── PassengerUpdater                             
│   │   └── PassengerNameUpdater.cs
├── docs/
│   ├── architecture/
│   │   └── architecture.md
│   ├──configuration
│   │   └──configuration.md
│   ├── dotnet-10-migration/
│   │   └── dotnet-10-migration.md
│   ├── functionalities
│   │   └── functionalities.md
│   ├── history
│   │   └── history.md
│   ├── project-structure/
│   │   └── project-structure.md
│   ├── screens
│   │   ├── images
│   │   │   ├── 1717028557715.jpeg
│   │   │   ├── 1717028557758.jpeg
│   │   │   ├── 1717028557775.jpeg
│   │   │   ├── 1717028557779.jpeg
│   │   │   ├── 1717028557786.jpeg
│   │   │   ├── 1717028557844.jpeg
│   │   │   └── 1717028558230.jpeg
│   │   └──screens.md
│   ├──security
│   │   └──security.md
│   └── technologies
│   │   └── technologies.md
├── docker/
│   ├── api/
│   │   └── Dockerfile
│   └── frontend/
│   │   └── Dockerfile        
├── appsettings.json
├── appsettings.Development.json
├── BusTrack.csproj
├── BusTrack.sln
├── Dockerfile
├── firebase.firebaserc
├── fiberase.json
├── LICENSE.md
├── OIG4.jpeg
├── package-lock.json
├── package.json
└── README.md
```