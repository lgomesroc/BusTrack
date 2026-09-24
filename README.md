README

# Bus Track

![Venha conhecer o projeto Bus Track.](OIG4.jpeg)

O projeto Bus Track fornece uma aplicação para gerenciamento de informações relacionadas ao transporte de passageiros, incluindo ônibus, motoristas, rotas, passageiros e viagens.

O backend disponibiliza uma API RESTful desenvolvida em C# com ASP.NET Core e .NET 10. O banco de dados utilizado é o MongoDB, responsável pela persistência das informações. O frontend é desenvolvido com Angular e fornece a interface utilizada pelos usuários do sistema.

O projeto também possui diferentes tipos de testes automatizados, incluindo testes unitários, testes de integração, testes de performance e testes de usabilidade.

## Funcionalidades Principais

O objetivo principal do projeto é realizar o controle tanto de quantos passageiros embarcaram durante aquela viagem sendo que há uma média e se ficar abaixo, sofrerá punição.

- Ônibus: Informações sobre os ônibus, como número, modelo, marca e capacidade.
- Motoristas: Informações sobre os motoristas, como nome, idade e licença de motorista.
- Rotas: Informações sobre as rotas, como origem, destino e número de paradas.
- Passageiros: Informações sobre os passageiros, como nome, idade e endereço.

## Ferramentas utilizadas

O projeto Bus Track é composto por um backend desenvolvido em C# com ASP.NET Core, um frontend desenvolvido com Angular, persistência de dados utilizando MongoDB e diferentes tipos de testes automatizados.

### Backend — `BusTrack.csproj`

* **C#**
* **.NET 10**
* **ASP.NET Core**
* **MongoDB**
* **AutoMapper** — mapeamento entre modelos e DTOs.
* **BCrypt.Net-Next** — hashing de senhas.
* **Newtonsoft.Json** — serialização e desserialização JSON.
* **Swashbuckle.AspNetCore** — documentação da API com Swagger.
* **System.Linq.Async** — operações LINQ assíncronas.
* **Selenium.WebDriver** — automação utilizada nos testes de usabilidade.
* **BenchmarkDotNet** — testes de performance.
* **Moq** — criação de mocks nos testes.

### Banco de dados

* **MongoDB**
* **MongoDB.Driver**
* **MongoDB.Bson**

### Frontend — `bustrack.frontend.client.esproj`

* **Angular**
* **TypeScript**
* **HTML**
* **CSS**

O frontend é responsável pela interface utilizada pelos usuários do sistema e pelas telas de autenticação, cadastro, dashboard, atualização de senha e registro das informações relacionadas às viagens.

### Frontend Server — `BusTrack.Frontend.Server.csproj`

Servidor ASP.NET Core responsável pela integração do frontend Angular com a aplicação.

* **C#**
* **.NET 10**
* **ASP.NET Core**
* **Microsoft.AspNetCore.SpaProxy**
* **MongoDB.Driver**
* **AutoMapper**
* **Newtonsoft.Json**
* **Swashbuckle.AspNetCore**
* **BenchmarkDotNet**
* **Selenium.WebDriver**
* **Moq**

### Testes — `BusTrack.Tests.csproj`

Projeto separado para os testes automatizados da aplicação.

* **xUnit** — estrutura dos testes.
* **Moq** — criação de mocks.
* **Microsoft.NET.Test.Sdk** — infraestrutura de execução dos testes.
* **coverlet.collector** — coleta de cobertura de testes.
* **Microsoft.AspNetCore.TestHost** — criação de servidores de teste para os testes de integração.
* **BenchmarkDotNet** — testes de performance.
* **Selenium.WebDriver** — testes de usabilidade.

### Configuração geral

Todos os projetos .NET da solução foram atualizados de **.NET 7 para .NET 10**.

Também estão habilitados:

* `ImplicitUsings`
* Nullable Reference Types

A solução utiliza o arquivo `BusTrack.sln` para organizar os projetos do backend, frontend server e testes.

### Segurança

No parte de backend utilizei o JWT para token e Bcrypt para hashing de senhas como pode verificar logo acima.

Já no frontend criei regras para não salvar senha, copiar informações tanto com o teclado quanto selecionando com o botão esquerdo do mouse, o botão direito está desativado em todo o sistema e tem um temporizador de conexão de 20 minutos. Além disso, não pode voltar ou avançar na tela do sistema com os botões do navegador. Também há a política de senha que o usuário, a cada 45 dias, é obrigado a trocar a senha e o aviso será mostrado quando estiver desconectado.


## Histórico de Atualizações

### 2026-09-24

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

### 2026-09-23

* Continuação da preparação do BusTrack para execução em ambiente de produção.
* Configuração inicial do MongoDB Atlas como banco de dados externo para o ambiente de produção.
* Configuração do acesso do MongoDB Atlas através de autenticação por usuário e senha.
* Configuração inicial da lista de acesso de endereços IP do MongoDB Atlas para testes de conectividade.
* Implementação da configuração do backend para receber `ConnectionStrings__BusTrackDBConnection` e `ConnectionStrings__DatabaseName` através de variáveis de ambiente.
* Validação do funcionamento do backend utilizando o endpoint `GET /api/Health`.
* Teste de acesso real ao MongoDB Atlas através da aplicação, identificando posteriormente a restrição de conexão da rede corporativa à porta TCP 27017.

### 2026-09-22

* Continuação dos ajustes de configuração do frontend Angular para separar as configurações de desenvolvimento e produção.
* Criação da estrutura de ambientes do frontend em `src/environments`.
* Ajustes na configuração do Angular para utilizar `environment.prod.ts` durante o build de produção.
* Atualização dos serviços Angular para utilizar a URL da API definida no ambiente de execução.

### 2026-09-21

* Continuação da organização estrutural do projeto após a migração para .NET 10.
* Ajustes na configuração da aplicação para preparar a execução local e futura implantação em ambiente externo.
* Criação do endpoint de verificação de disponibilidade da API para utilização pelo monitoramento da aplicação.
* Revisão das regras de disponibilidade da aplicação para impedir a continuidade de uma sessão quando a comunicação com o sistema é perdida.

### 2026-09-20

* Implementação e validação do controle de autenticação das rotas protegidas do frontend Angular.
* Ajustes no gerenciamento da sessão utilizando `sessionStorage`.
* Atualização da navegação entre as áreas autenticadas e públicas da aplicação.
* Ajustes no módulo de Viagens e na integração com os dados relacionados a ônibus, motoristas, rotas e passageiros.
* Correções e melhorias na interface do módulo de Viagens.
* Ajustes nos serviços Angular responsáveis pela comunicação com a API.

### 2026-09-19

* Continuação da implementação do módulo de Ônibus.
* Integração completa das operações de listagem, criação, edição e exclusão de ônibus entre frontend, API e MongoDB.
* Ajustes de responsividade e interface do módulo de Ônibus.
* Correções no serviço Angular responsável pelas operações de ônibus.
* Ajustes no modelo, serviço e repositório de ônibus para manter a integração entre as camadas da aplicação.
* Continuação dos ajustes no módulo de Viagens e na apresentação dos detalhes das viagens.
* Atualização das informações apresentadas nas viagens para incluir os dados relacionados aos registros associados.
* Validação das alterações realizadas no frontend e backend.

### 2026-09-18

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

### 2026-09-17

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

### 2026-09-16

* Início da refatoração estrutural do BusTrack após a migração para .NET 10.
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

### 2026-09-11

* Atualização dos projetos .NET da versão 7 para a versão 10.
* Atualização das dependências principais utilizadas pelo backend e pelos testes.
* Atualização do driver do MongoDB para a versão 3.11.1.
* Atualização do AutoMapper para a versão 16.2.0.
* Remoção de dependências que não eram mais necessárias após a atualização para o .NET 10.
* Ajustes nos testes para compatibilidade com a versão atualizada do AutoMapper.

## 2024-06-03

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

## Estrutura do projeto

A solução está organizada em projetos separados para a API, banco de dados, frontend, servidor do frontend, testes e componentes auxiliares.

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
TripDetailsDTOAPI.cs                           
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
│   │   └── TripsPassengerDB.cs                       
│   │   └── UserDB.cs                               
│   │   └── UserPasswordHistoryDB.cs                
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
│   ├── database/
│   ├── deployment/
│   ├── api/
│   └── project-history/
│
├── docker/
│   ├── api/
│   │   └── Dockerfile
│   └── frontend/
│       └── Dockerfile        
├── appsettings.json
├── appsettings.Development.json
├── BusTrack.csproj
├── BusTrack.sln
├── firebase.firebaserc
├── fiberase.json
├── LICENSE.md
├── OIG4.jpeg
├── package-lock.json
├── package.json
└── README.md
```

### Principais responsabilidades

**`BusTrack.API`**
Contém a camada responsável pela API REST, incluindo controllers, DTOs, modelos, interfaces, serviços e mapeamentos.

**`BusTrack.DB`**
Contém a integração com o MongoDB, incluindo classes de dados, modelos, interfaces, repositórios, conexão com o banco e serviços relacionados às regras de persistência.

**`BusTrack.Program`**
Contém o ponto de entrada e configurações auxiliares da aplicação, incluindo registro de serviços, configuração do banco de dados, extensões e middleware.

**`BusTrack.Frontend`**
Contém o frontend Angular e o servidor ASP.NET Core responsável pela integração com a aplicação.

**`bustrack.frontend.client`**
Contém a aplicação Angular, seus componentes, modelos, serviços, regras de interface e recursos estáticos.

**`BusTrack.Frontend.Server`**
Contém o projeto ASP.NET Core utilizado como servidor do frontend e para a configuração do SPA Proxy durante o desenvolvimento.

**`BusTrack.Tests`**
Projeto dedicado aos testes automatizados, organizado em testes unitários, integração, performance e usabilidade.

**`BusTrack.Updater`**
Contém componentes auxiliares utilizados para atualização de dados específicos da aplicação.


## Estrutura do Projeto

O Bus Track é organizado em três áreas principais: backend, frontend e testes.

### Backend

O backend é desenvolvido em **C# com ASP.NET Core e .NET 10** e disponibiliza uma API RESTful para gerenciamento dos dados do sistema.

Entre os principais recursos estão:

* gerenciamento de ônibus;
* gerenciamento de motoristas;
* gerenciamento de rotas;
* gerenciamento de passageiros;
* gerenciamento de viagens;
* relacionamento entre viagens e passageiros;
* gerenciamento de usuários;
* autenticação;
* criação e atualização de contas;
* atualização de senhas;
* dashboard.

A documentação dos endpoints pode ser acessada através do **Swagger** durante a execução da API.

### Banco de dados

O sistema utiliza **MongoDB** como banco de dados NoSQL.

A camada `BusTrack.DB` concentra:

* conexão com o MongoDB;
* modelos de persistência;
* repositórios;
* interfaces;
* serviços de validação e regras relacionadas aos dados.

### Frontend

O frontend é desenvolvido com **Angular** e fornece a interface utilizada pelos usuários do sistema.

Entre as principais áreas estão:

* login;
* criação de conta;
* confirmação;
* conclusão;
* atualização de senha;
* dashboard;
* navegação principal;
* validações de campos;
* regras relacionadas à sessão e segurança da interface.

### Testes

O projeto possui diferentes tipos de testes:

* **Testes unitários** — verificam partes específicas da aplicação de forma isolada.
* **Testes de integração** — verificam a integração entre componentes da aplicação e a API.
* **Testes de performance** — utilizam BenchmarkDotNet para avaliar o desempenho de determinados serviços.
* **Testes de usabilidade** — utilizam Selenium WebDriver para verificar comportamentos da interface.

### Nomenclatura

Os componentes, classes, projetos, propriedades e demais elementos do código seguem nomenclatura em inglês, mantendo um padrão consistente em toda a solução.


## Telas
![Tela principal.](1717028557844.jpeg)
![Tela de login.](1717028557779.jpeg)
![Tela de criar conta.](1717028557786.jpeg)
![Tela de atualizar a senha.](1717028557715.jpeg)
![Tela de confirmação.](1717028558230.jpeg)
![Tela de conclusão.](1717028557758.jpeg)
![Tela de dashboard.](1717028557775.jpeg)


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
