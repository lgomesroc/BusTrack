# Bus Track

![Venha conhecer o projeto Bus Track.](OIG4.jpeg)

O Bus Track é uma aplicação para gerenciamento de transporte de passageiros, desenvolvida para centralizar informações e operações relacionadas à utilização dos veículos, motoristas, rotas, passageiros e viagens.

A aplicação permite organizar diferentes etapas do gerenciamento do transporte em um único sistema, desde o cadastro e manutenção dos registros até a criação e acompanhamento das viagens e dos dados relacionados a elas.

O projeto foi desenvolvido como uma aplicação completa, envolvendo backend, frontend, banco de dados, autenticação, controle de sessão, testes automatizados e preparação para execução em diferentes ambientes.

A solução também possui uma estrutura organizada por responsabilidades, com projetos separados para API, persistência, frontend e testes, além de documentação técnica específica para cada parte do sistema.

O Bus Track continua em evolução, com melhorias de arquitetura, segurança, infraestrutura, documentação e preparação para implantação em ambiente de produção.

## Funcionalidades

O Bus Track é uma aplicação para gerenciamento de transporte, permitindo controlar informações relacionadas a ônibus, motoristas, rotas, passageiros e viagens.

Entre as principais funcionalidades estão:

* **Ônibus:** cadastro, consulta, atualização e exclusão de ônibus, incluindo informações como número, placa, modelo e capacidade.
* **Motoristas:** gerenciamento dos motoristas cadastrados no sistema.
* **Rotas:** organização das rotas utilizadas pelo sistema de transporte.
* **Passageiros:** cadastro e gerenciamento dos passageiros.
* **Viagens:** criação, consulta e gerenciamento das viagens, relacionando ônibus, motoristas, rotas e passageiros.
* **Usuários e contas:** criação de contas, autenticação e gerenciamento de senha.

A documentação detalhada apresenta o funcionamento de cada módulo, suas regras de negócio, relacionamentos entre as funcionalidades e os principais comportamentos esperados da aplicação.

[Ver documentação completa das funcionalidades](docs/functionalities/functionalities.md)

## Tecnologias

O Bus Track utiliza uma stack composta por **C# e .NET 10 com ASP.NET Core** no backend, **Angular e TypeScript** no frontend e **MongoDB** para persistência dos dados.

A solução também utiliza ferramentas para mapeamento de dados, segurança, documentação da API e testes automatizados, além de tecnologias de infraestrutura planejadas para o ambiente de produção, como **Docker, MongoDB Atlas e Render**.

A documentação detalha cada tecnologia, explica sua finalidade no projeto, o motivo de sua escolha e diferencia os recursos que já estão implementados daqueles que fazem parte da evolução planejada.

[Ver documentação completa das tecnologias](docs/technologies/technologies.md)

### Configuração geral

A solução Bus Track utiliza projetos baseados em .NET 10 e mantém configurações comuns para padronizar o desenvolvimento entre seus diferentes projetos.

Entre as configurações utilizadas estão `ImplicitUsings`, Nullable Reference Types e a organização dos projetos através do arquivo `BusTrack.sln`.

A documentação detalhada explica a finalidade dessas configurações e como elas contribuem para a organização e segurança do código.

[Ver documentação completa da configuração](docs/configuration/configuration.md)

### Segurança

O Bus Track possui mecanismos de segurança distribuídos entre backend e frontend, incluindo **hash de senhas com BCrypt**, histórico e política de alteração de senhas, controle de sessão, proteção de rotas, monitoramento de disponibilidade e controle de inatividade.

O frontend também possui restrições de interação, como bloqueio de seleção e cópia de informações, desativação do botão direito e bloqueio da navegação pelos botões Voltar e Avançar do navegador.

A documentação detalhada explica cada mecanismo, suas responsabilidades e limitações. A implementação de **JWT** faz parte da evolução planejada da autenticação e ainda não é considerada uma funcionalidade implementada.

[Ver documentação completa de segurança](docs/security/security.md)

## Histórico de Atualizações

O Bus Track passou por diversas etapas de evolução desde sua criação, incluindo a implementação do frontend e da API, criação da estrutura de testes, migração dos projetos para .NET 10, refatoração das camadas da aplicação, implementação dos módulos de Ônibus e Viagens e preparação para implantação em ambiente de produção.

As atualizações mais recentes estão concentradas na organização da configuração de ambientes, integração planejada com MongoDB Atlas, preparação do deployment, melhorias de autenticação e sessão, além da evolução da documentação técnica do projeto.

O histórico completo apresenta as principais alterações realizadas desde o início do desenvolvimento do Bus Track.

[Ver histórico completo de atualizações](docs/history.md)

## Arquitetura

O Bus Track possui uma arquitetura organizada em projetos com responsabilidades separadas. A API concentra as operações do sistema, a camada de persistência concentra o acesso aos dados, o frontend é responsável pela interface e interação com o usuário, e os testes permanecem separados do código de produção.

A solução também utiliza interfaces, injeção de dependência, Services e Repositories para manter as responsabilidades organizadas e facilitar a evolução e os testes da aplicação.

A explicação completa da organização dos projetos, responsabilidades das camadas, comunicação entre frontend e backend e decisões arquiteturais está disponível na documentação.

[Ver documentação completa da arquitetura](docs/architecture.md)

## Estrutura do Projeto

O Bus Track é organizado em três áreas principais: **backend, frontend e testes**, além de arquivos e estruturas de suporte ao projeto.

O backend concentra a API, as regras de negócio, os serviços, os repositórios, os modelos, os DTOs e a persistência dos dados. O frontend concentra a aplicação Angular, seus componentes, telas, serviços, guards, regras de navegação e demais recursos da interface. A área de testes mantém os testes automatizados e a infraestrutura necessária para sua execução.

A solução também possui arquivos de configuração, documentação, arquivos relacionados ao banco de dados, infraestrutura, execução da aplicação, gerenciamento de dependências e organização da solução.

A documentação completa apresenta a estrutura do projeto de forma detalhada, incluindo **todos os projetos, pastas e principais arquivos**, explicando a finalidade e a responsabilidade de cada parte.

[Ver documentação completa da estrutura do projeto](docs/project-structure/project-structure.md)


### Nomenclatura

Os componentes, classes, projetos, propriedades e demais elementos do código seguem nomenclatura em inglês, mantendo um padrão consistente em toda a solução.

## Telas

O Bus Track possui telas destinadas aos diferentes fluxos da aplicação, incluindo entrada no sistema, autenticação, criação de conta, atualização de senha, confirmação, conclusão e dashboard.

A documentação de telas apresenta as imagens e a finalidade de cada uma das principais interfaces da aplicação.

[Ver documentação completa das telas](docs/screens/screens.md)

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
