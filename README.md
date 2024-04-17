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

BusTrack                    # Nome do projeto
│
├── BusTrack.API           # Pasta em relação à API
│   │
│   ├── ControllersAPI     # Pasta para controladores da API
│   │   ├── BusControllerAPI.cs           # Controller para gerenciar operações relacionadas a ônibus
│   │   ├── DriverControllerAPI.cs        # Controller para gerenciar operações relacionadas a motoristas
│   │   ├── PassengerControllerAPI.cs     # Controller para gerenciar operações relacionadas a passageiros
│   │   ├── RouteControllerAPI.cs         # Controller para gerenciar operações relacionadas a rotas
│   │   ├── TripControllerAPI.cs          # Controller para gerenciar operações relacionadas a viagens
│   │   └── TripsPassengerControllerAPI.cs# Controller para gerenciar operações relacionadas a viagens
│   │
│   ├── DTOAPI              # Pasta para DTOs da API
│   │   ├── BusDTOAPI.cs                  # DTO para ônibus
│   │   ├── DriverDTOAPI.cs               # DTO para motoristas
│   │   ├── PassengerDTOAPI.cs            # DTO para passageiros
│   │   ├── RouteDTOAPI.cs                # DTO para rotas
│   │   ├── TripDTOAPI.cs                 # DTO para viagens
│   │   └── TripsPassengerDTOAPI.cs       # DTO para associação entre viagens e passageiros
│   │
│   ├── InterfacesAPI      # Pasta para interfaces da API
│   │   ├── IServicesAPI      # Pasta para interfaces de serviço da API
│   │   │   ├── IBusServiceAPI.cs               # Interface para serviço relacionado a ônibus
│   │   │   ├── IDriverServiceAPI.cs            # Interface para serviço relacionado a motoristas
│   │   │   ├── IPassengerServiceAPI.cs         # Interface para serviço relacionado a passageiros
│   │   │   ├── IRouteServiceAPI.cs             # Interface para serviço relacionado a rotas
│   │   │   ├── ITripServiceAPI.cs              # Interface para serviço relacionado a viagens
│   │   │   └── ITripsPassengerServiceAPI.cs    # Interface para serviço relacionado a viagens
│   │
│   ├── ModelsAPI          # Pasta para modelos da API
│   │   ├── BusModelAPI.cs                  # Modelo para ônibus
│   │   ├── DriverModelAPI.cs               # Modelo para motoristas
│   │   ├── PassengerModelAPI.cs            # Modelo para passageiros
│   │   ├── RouteModelAPI.cs                # Modelo para rotas
│   │   ├── TripModelAPI.cs                 # Modelo para viagens
│   │   └── TripsPassengerModelAPI.cs       # Modelo para associação entre viagens e passageiros
│   │
│   ├── MappingsAPI        # Pasta para configuração de mapeamentos
│   │   └── MappingProfileAPI.cs            # Arquivo para configuração de mapeamentos
│   │
│   ├── ServicesAPI        # Pasta para serviços da API
│   │   ├── BusServiceAPI.cs                # Serviço para operações relacionadas a ônibus
│   │   ├── DriverServiceAPI.cs             # Serviço para operações relacionadas a motoristas
│   │   ├── PassengerServiceAPI.cs          # Serviço para operações relacionadas a passageiros
│   │   ├── RouteServiceAPI.cs              # Serviço para operações relacionadas a rotas
│   │   ├── TripServiceAPI.cs               # Serviço para operações relacionadas a viagens
│   │   └── TripsPassengerServiceAPI.cs     # Serviço para operações relacionadas a associação entre viagens e passageiros
│   │
│   └── ...
│
├── BusTrack.DB            # Pasta em relação ao Banco de Dados
│   │
│   ├── ClassesDB          # Pasta para as classes das tabelas do banco de dados
│   │   ├── BusDB.cs       # Define a classe BusDB, representando a tabela de ônibus
│   │   ├── DriverDB.cs    # Define a classe DriverDB, representando a tabela de motoristas
│   │   ├── PassengerDB.cs # Define a classe PassengerDB, representando a tabela de passageiros
│   │   ├── RouteDB.cs     # Define a classe RouteDB, representando a tabela de rotas
│   │   ├── TripDB.cs      # Define a classe TripDB, representando a tabela de viagens
│   │   └── TripsPassengerDB.cs  # Define a classe TripsPassengerDB, representando a tabela de associação entre viagens e passageiros
│   │
│   ├── ConnectionsDB       # Pasta para configuração do banco de dados
│   │   └── ConnectionDB.cs # Configura a conexão do banco de dados
│   │
│   ├── DataBaseDB         # Pasta para configuração do banco de dados
│   │   └── database.json  # Arquivo de configuração do banco de dados com o projeto.
│   │
│   ├── InterfacesDB       # Pasta para interfaces do banco de dados
│   │   │
│   │   ├── IModelsDB      # Pasta das interfaces dos modelos do banco de dados
│   │   │   ├── IBusModelDB.cs       # Interface IBusModelDB
│   │   │   ├── IDriverModelDB.cs    # Interface IDriverModelDB
│   │   │   ├── IPassengerModelDB.cs # Interface IPassengerModelDB
│   │   │   ├── IRouteModelDB.cs     # Interface IRouteModelDB
│   │   │   ├── ITripModelDB.cs      # Interface ITripModelDB
│   │   │   └── ITripsPassengerModelDB.cs  # Interface ITripsPassengerModelDB
│   │   │
│   │   └── IRepositoriesDB  # Pasta das interfaces dos repositórios do banco de dados
│   │       ├── IBusRepositoryDB.cs       # Interface IBusRepositoryDB
│   │       ├── IDriverRepositoryDB.cs    # Interface IDriverRepositoryDB
│   │       ├── IPassengerRepositoryDB.cs # Interface IPassengerRepositoryDB
│   │       ├── IRouteRepositoryDB.cs     # Interface IRouteRepositoryDB
│   │       ├── ITripRepositoryDB.cs      # Interface ITripRepositoryDB
│   │       └── ITripsPassengerRepositoryDB.cs  # Interface ITripsPassengerRepositoryDB
│   │
│   ├── ModelsDB           # Pasta para modelos do banco de dados
│   │   ├── BusModelDB.cs             # Implementação da interface IBusModelDB
│   │   ├── DriverModelDB.cs          # Implementação da interface IDriverModelDB
│   │   ├── PassengerModelDB.cs       # Implementação da interface IPassengerModelDB
│   │   ├── RouteModelDB.cs           # Implementação da interface IRouteModelDB
│   │   ├── TripModelDB.cs            # Implementação da interface ITripModelDB
│   │   └── TripsPassengerModelDB.cs  # Implementação da interface ITripsPassengerModelDB
│   │
│   ├── RepositoriesDB     # Pasta para repositórios do banco de dados
│   │   ├── BusRepositoryDB.cs            # Repositório para a classe BusDB
│   │   ├── DriverRepositoryDB.cs         # Repositório para a classe DriverDB
│   │   ├── PassengerRepositoryDB.cs      # Repositório para a classe PassengerDB
│   │   ├── RouteRepositoryDB.cs          # Repositório para a classe RouteDB
│   │   ├── TripRepositoryDB.cs           # Repositório para a classe TripDB
│   │   └── TripsPassengerRepositoryDB.cs # Repositório para a classe TripsPassengerDB
│   │
│   ├── ServicesDB         # Pasta para serviços do banco de dados
│   │   ├── BusSingleTripConstraintServiceDB.cs   # Serviço para restrição de viagem única
│   │   ├── DepartureTimeValidationServiceDB.cs  # Serviço para validação de hora de partida
│   │   ├── MinTripDurationContraintServiceDB.cs # Serviço para restrição de duração mínima de viagem
│   │   ├── PassengerLimitValidationServiceDB.cs # Serviço para validação de limite de passageiros
│   │   ├── TripMappingServiceDB.cs             # Serviço para mapeamento de viagem
│   │   ├── TripServiceDB.cs                    # Serviço relacionado às viagens
│   │   └── TripStatusUpdateServiceDB.cs        # Serviço para atualização de status de viagem
│   │
│   └── ...
│
├── BusTrack.Frontend      # Pasta para o projeto do frontend (Angular, por exemplo)
│   │
│   └── src                # Pasta principal do código-fonte do frontend
│       └── ...            # Arquivos do código-fonte do frontend
│
├── BusTrack.Program       # Pasta para o projeto principal do programa
│   │
│   ├── DataBaseServicesExtensionsProgram  # Pasta para extensões de serviços de banco de dados
│   │   └── DataBaseServicesExtensionsProgram.cs  # Contém métodos de extensão para adicionar serviços de banco de dados ao contêiner de injeção de dependência
│   │
│   ├── ExtensionsProgram  # Pasta para extensões do programa
│   │   └── ExtensionsProgram.cs  # Arquivo que contém métodos de extensão para adicionar serviços personalizados ao contêiner de injeção de dependência
│   │   └── ServiceExtensionProgram.cs  # Arquivo que contém métodos de extensão para adicionar serviços personalizados ao contêiner de injeção de dependência
│   │
│   ├── MiddlewareProgram  # Pasta para middleware do programa
│   │   └── ErrorHandlingMiddleware.cs  # Arquivo que é um middleware personalizado para lidar com erros
│   │
│   └── Program.cs  # Arquivo principal do programa, que configura e executa o aplicativo
│   │
├── BusTrack.Tests         # Pasta para o projeto de testes
│   ├── IntegrationTests   # Pasta para testes de integração
│   │   └── ...            # Arquivos dos testes de integração
│   │
│   ├── PerformanceTests   # Pasta para testes de performance
│   │   └── ...            # Arquivos dos testes de performance
│   │
│   ├── UnitTests          # Pasta para testes unitários
│   │   └── ...            # Arquivos dos testes unitários
│   │
│   └── UsabilityTests     # Pasta para testes de usabilidade
│       └── ...            # Arquivos dos testes de usabilidade
│
├── BusTrack.Updater    # Pasta para atualização de dados
│  │
│  ├── DriversUpdater       # Pasta para atualização de dados de motoristas
│  │  └── DriverNameUpdater.cs # Arquivo para atualizar nomes de motoristas
│  │
│  ├── PassengerUpdater     # Pasta para atualização de dados de passageiros
│  │  └── PassengerNameUpdater.cs # Arquivo para atualizar nomes de passageiros
│  │
│  └── ...      # Outras subpastas para diferentes tipos de atualização (se necessário)
│
└── BusTrack.sln           # Arquivo de solução do Visual Studio


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