# Funcionalidades

## Visão geral

O Bus Track é uma aplicação destinada ao gerenciamento de informações relacionadas a transporte coletivo.

A aplicação reúne funcionalidades para administrar ônibus, motoristas, rotas, passageiros e viagens, além de disponibilizar recursos relacionados às contas dos usuários e à autenticação no sistema.

A proposta é centralizar essas informações em uma única aplicação, permitindo que os dados utilizados nas viagens sejam cadastrados, consultados, alterados e relacionados entre si.

As funcionalidades estão organizadas em módulos para que cada responsabilidade do sistema tenha seu próprio espaço.

---

## Ônibus

O módulo de ônibus é responsável pelo gerenciamento dos veículos utilizados no sistema de transporte.

A aplicação permite trabalhar com os dados cadastrais dos ônibus e utilizar essas informações nas demais funcionalidades que dependem de um veículo.

Entre as informações relacionadas ao ônibus estão:

* Identificação do ônibus;
* Número;
* Placa;
* Modelo;
* Linha;
* Rotas relacionadas.

O cadastro de um ônibus permite que o veículo passe a fazer parte dos dados disponíveis para utilização no sistema.

Depois de cadastrado, o ônibus pode ser consultado e seus dados podem ser atualizados ou removidos conforme as operações disponibilizadas pelo sistema.

A existência de um cadastro separado para os ônibus permite que as informações do veículo sejam reutilizadas por outras funcionalidades, principalmente pelas viagens.

### Objetivo do módulo

O objetivo do módulo é manter centralizadas as informações dos veículos utilizados pela aplicação, evitando que os dados de um ônibus precisem ser cadastrados novamente cada vez que ele for utilizado em uma operação.

---

## Motoristas

O módulo de motoristas é responsável pelo gerenciamento dos profissionais que podem ser associados às operações de transporte.

A aplicação mantém os motoristas como entidades próprias, permitindo seu cadastramento e gerenciamento independentemente das viagens.

As operações relacionadas ao módulo incluem:

* Cadastro de motorista;
* Consulta de motoristas;
* Consulta de um motorista específico;
* Atualização de dados;
* Exclusão de motorista.

O motorista cadastrado pode posteriormente ser relacionado a uma viagem.

Essa separação permite que o mesmo motorista seja utilizado em diferentes operações sem que seus dados precisem ser duplicados.

### Objetivo do módulo

O objetivo é manter os dados dos motoristas organizados e disponíveis para utilização pelas funcionalidades que dependem dessa informação.

---

## Rotas

O módulo de rotas é responsável pelo gerenciamento das rotas utilizadas pelo sistema de transporte.

Uma rota representa uma informação que pode ser utilizada na organização das viagens e na associação dos ônibus às operações de transporte.

O sistema possui uma estrutura própria para as rotas, permitindo que elas sejam tratadas como informações independentes dentro da aplicação.

As operações relacionadas ao módulo incluem:

* Cadastro de rotas;
* Consulta de rotas;
* Consulta de uma rota específica;
* Atualização;
* Exclusão.

As rotas podem ser relacionadas aos ônibus e utilizadas no contexto das viagens.

### Objetivo do módulo

O objetivo é manter as rotas separadas das demais informações do sistema, permitindo reutilizá-las quando necessário e evitando a duplicação dos mesmos dados em diferentes registros.

---

## Passageiros

O módulo de passageiros é responsável pelo gerenciamento das pessoas que utilizam o serviço de transporte.

A aplicação mantém os passageiros como registros independentes, permitindo que seus dados sejam cadastrados e posteriormente utilizados em viagens.

As operações disponíveis incluem:

* Cadastro de passageiro;
* Consulta de passageiros;
* Consulta de um passageiro específico;
* Atualização;
* Exclusão.

O passageiro também pode ser associado a uma viagem.

Essa associação é tratada separadamente do cadastro principal do passageiro. Dessa maneira, os dados cadastrais do passageiro permanecem independentes das viagens das quais ele participa.

### Objetivo do módulo

O objetivo é manter os passageiros cadastrados de forma centralizada e permitir que eles sejam relacionados às viagens realizadas pelo sistema.

---

## Viagens

O módulo de viagens representa uma das principais funcionalidades do Bus Track.

Uma viagem reúne informações de diferentes partes do sistema para representar uma operação de transporte.

Uma viagem pode envolver informações relacionadas a:

* Ônibus;
* Motorista;
* Rota;
* Passageiros;
* Dados específicos da viagem.

A utilização de entidades separadas permite que a viagem não precise armazenar novamente todas as informações cadastrais desses elementos.

Por exemplo, o ônibus utilizado em uma viagem já possui seu próprio cadastro. O motorista também possui seu próprio cadastro. A rota possui seu próprio registro. A viagem utiliza essas informações para representar a operação realizada.

### Cadastro e gerenciamento

O módulo permite trabalhar com os registros das viagens e consultar informações relacionadas a cada uma delas.

As operações do módulo incluem o gerenciamento dos dados necessários para registrar e consultar uma viagem.

Além das informações básicas, o sistema possui estruturas específicas para consultar os detalhes de uma viagem.

### Passageiros da viagem

A relação entre viagens e passageiros possui tratamento próprio.

Um passageiro cadastrado no sistema pode ser associado a uma viagem sem que seu cadastro seja duplicado.

Isso permite representar quais passageiros participam de determinada viagem mantendo separados:

* o cadastro do passageiro;
* o cadastro da viagem;
* a relação entre passageiro e viagem.

Essa separação também permite consultar os dados da viagem juntamente com as informações dos passageiros relacionados a ela.

### Regras e validações

As operações relacionadas às viagens precisam respeitar as regras existentes na aplicação para garantir que os dados utilizados sejam consistentes.

As validações fazem parte da camada responsável pelas regras da aplicação e não devem ser confundidas simplesmente com validações visuais da tela.

A capacidade do ônibus, por exemplo, é uma informação relevante para as regras relacionadas aos passageiros de uma viagem. As regras desse tipo devem ser tratadas no contexto apropriado do domínio da viagem.

### Objetivo do módulo

O objetivo do módulo de viagens é reunir as informações necessárias para representar uma operação de transporte e relacionar corretamente os principais elementos envolvidos nessa operação.

---

## Usuários e contas

O Bus Track possui funcionalidades relacionadas ao gerenciamento das contas utilizadas para acessar o sistema.

Essas funcionalidades fazem parte da área de autenticação e permitem que o usuário tenha uma conta própria para utilizar as áreas protegidas da aplicação.

### Criação de conta

O sistema possui uma funcionalidade para criação de novas contas.

O usuário informa os dados necessários para criar sua conta e o sistema realiza o processamento dessas informações antes de permitir sua utilização.

A criação da conta é uma etapa independente do login.

### Login

Depois de possuir uma conta, o usuário pode utilizar a funcionalidade de login para acessar o sistema.

O processo de autenticação verifica as credenciais informadas e, quando válidas, permite o acesso às áreas protegidas da aplicação.

O acesso às áreas autenticadas é controlado pelo sistema de sessão e pelos mecanismos de proteção das rotas do frontend.

### Alteração de senha

O sistema possui uma funcionalidade específica para alteração da senha.

A alteração não consiste apenas em substituir uma string armazenada no banco de dados. A senha é processada utilizando hashing e o sistema mantém um histórico das senhas utilizadas.

O histórico permite verificar se uma nova senha já foi utilizada anteriormente.

### Histórico de senhas

O sistema mantém registros das senhas anteriores do usuário para aplicar a regra de reutilização de senha.

Quando uma nova senha é informada, o sistema verifica o histórico antes de aceitar a alteração.

Dessa maneira, uma senha que já tenha sido utilizada recentemente pode ser rejeitada.

O histórico também possui uma quantidade limitada de registros mantidos pela aplicação.

---

## Sessão do usuário

Depois que o usuário realiza o login com sucesso, o frontend mantém o estado da sessão para determinar se o usuário está autenticado.

A sessão é armazenada utilizando `sessionStorage`.

Isso permite que o frontend saiba se o usuário possui uma sessão autenticada durante a utilização da aplicação.

O sistema também possui um mecanismo de encerramento da sessão quando a aplicação é reinicializada, evitando que uma sessão anterior seja simplesmente restaurada automaticamente em uma nova inicialização do frontend.

### Proteção das áreas autenticadas

As áreas que exigem autenticação são protegidas pelo `AuthenticationGuard`.

Quando o usuário tenta acessar diretamente uma área protegida sem possuir uma sessão válida, o sistema impede o acesso e direciona o usuário para a tela de login.

Isso evita que a proteção dependa somente dos links apresentados na interface.

---

## Navegação da aplicação

A aplicação possui controle sobre a navegação entre suas páginas.

Além da proteção das áreas autenticadas, o sistema controla especificamente a utilização dos botões **Voltar** e **Avançar** do navegador.

A navegação realizada pelos controles do navegador é identificada pelo Angular Router e tratada pelo `BrowserNavigationGuard`.

Quando uma navegação desse tipo é detectada, o sistema impede a mudança de rota e informa ao usuário que a navegação pelos controles do navegador está desativada.

Essa regra é diferente da autenticação.

A autenticação determina se o usuário pode acessar determinada área.

O controle de navegação determina como o usuário pode se movimentar entre as rotas da aplicação.

---

## Disponibilidade da aplicação

O Bus Track possui um mecanismo de monitoramento da disponibilidade da aplicação.

O frontend verifica periodicamente se os serviços necessários continuam respondendo.

A verificação considera tanto a disponibilidade da API quanto a disponibilidade do frontend.

Quando a comunicação com o sistema é perdida enquanto existe uma sessão autenticada, a aplicação encerra a sessão atual e bloqueia a interface carregada.

O usuário recebe uma mensagem informando que a comunicação com o sistema foi perdida e é direcionado novamente para a tela de login.

### Objetivo

Esse comportamento evita que o usuário continue utilizando uma tela que não consegue mais se comunicar corretamente com os serviços responsáveis pelo processamento dos dados.

---

## Controle de inatividade

A aplicação possui mecanismos relacionados ao encerramento da sessão por inatividade.

O objetivo é impedir que uma sessão autenticada permaneça disponível indefinidamente sem interação do usuário.

Esse tipo de controle complementa a autenticação inicial, pois o fato de o usuário ter realizado login anteriormente não significa que sua sessão deva permanecer válida indefinidamente.

---

## Restrições de interação

O frontend possui algumas restrições de interação implementadas como parte do comportamento da aplicação.

Entre elas estão mecanismos relacionados a:

* Seleção e cópia de conteúdo em determinadas situações;
* Menu de contexto do botão direito;
* Atalhos de cópia;
* Salvamento automático de senhas pelo navegador;
* Navegação pelos botões Voltar e Avançar do navegador.

Essas restrições são implementadas no frontend e têm como objetivo controlar o comportamento da interface.

Elas não devem ser consideradas mecanismos de segurança do backend.

Por exemplo, impedir uma ação com JavaScript não impede que uma pessoa tecnicamente habilitada acesse diretamente uma API. A segurança real dos dados e das operações precisa ser aplicada também nas camadas responsáveis pelo processamento da aplicação.

---

## Dashboard

O Dashboard funciona como ponto central da área autenticada da aplicação.

Depois que o usuário entra no sistema, o Dashboard apresenta o ambiente principal a partir do qual os diferentes módulos podem ser acessados.

A função do Dashboard é servir como ponto de entrada para as funcionalidades disponíveis, e não substituir as telas específicas de cada módulo.

A criação de uma viagem, por exemplo, pertence ao módulo de viagens e não deve ser iniciada diretamente pelo Dashboard.

Essa separação mantém o Dashboard como um ponto de navegação e os módulos como responsáveis por suas próprias operações.

---

## Relação entre as funcionalidades

As funcionalidades do Bus Track foram separadas para que cada tipo de informação tenha seu próprio contexto.

Um ônibus possui seus próprios dados cadastrais.

Um motorista possui seu próprio cadastro.

Uma rota possui seus próprios dados.

Um passageiro possui seu próprio cadastro.

Uma viagem utiliza essas informações para representar uma operação de transporte.

Os passageiros de uma viagem são tratados por meio de uma relação específica entre passageiros e viagens.

Essa organização evita que uma mesma informação precise ser repetida em vários registros.

Por exemplo, os dados cadastrais de um motorista não precisam ser copiados para cada viagem realizada por ele. A viagem mantém a referência necessária para identificar o motorista utilizado naquela operação.

O mesmo princípio é aplicado aos demais elementos envolvidos na viagem.

---

## Separação entre cadastro e utilização

Uma característica importante das funcionalidades do sistema é a separação entre o cadastro de uma informação e sua utilização em outra operação.

O cadastro de um passageiro é uma responsabilidade do módulo de passageiros.

A utilização desse passageiro em uma viagem é uma responsabilidade relacionada ao módulo de viagens e à relação entre passageiros e viagens.

O mesmo conceito se aplica aos demais elementos.

Essa separação permite que as informações sejam reutilizadas e reduz a duplicação de dados dentro da aplicação.

---

## Funcionalidades atualmente implementadas

As funcionalidades que fazem parte da aplicação atualmente incluem:

* Gerenciamento de ônibus;
* Gerenciamento de motoristas;
* Gerenciamento de rotas;
* Gerenciamento de passageiros;
* Gerenciamento de viagens;
* Relação entre passageiros e viagens;
* Criação de contas;
* Login;
* Alteração de senha;
* Histórico de senhas;
* Controle de sessão;
* Proteção de rotas autenticadas;
* Controle da navegação pelos botões Voltar e Avançar do navegador;
* Monitoramento da disponibilidade da aplicação;
* Encerramento da sessão quando a comunicação com o sistema é perdida;
* Controle de inatividade;
* Restrições de determinadas interações no frontend;
* Dashboard como ponto central da área autenticada.

---

## Funcionalidades que fazem parte da evolução do projeto

O Bus Track continua sendo desenvolvido e algumas funcionalidades podem fazer parte de etapas posteriores do projeto.

Entre os recursos planejados para evolução estão melhorias e extensões dos módulos existentes e novos mecanismos de segurança e autenticação.

O JWT, por exemplo, possui configuração prevista no projeto, mas **não deve ser apresentado como uma funcionalidade atualmente implementada** enquanto sua implementação efetiva não estiver concluída.

Da mesma forma, funcionalidades futuras devem ser documentadas como planejadas somente quando realmente fizerem parte do planejamento do projeto.

A documentação deve manter essa distinção para que seja possível identificar claramente o que já está disponível na aplicação e o que ainda está em desenvolvimento.

---

## Resumo

As funcionalidades do Bus Track foram organizadas em módulos independentes para representar os principais elementos envolvidos no gerenciamento do transporte.

Ônibus, motoristas, rotas e passageiros possuem seus próprios cadastros. As viagens utilizam essas informações para representar as operações de transporte, enquanto a relação entre passageiros e viagens permite registrar quais passageiros participam de cada viagem.

Além dos módulos relacionados ao transporte, a aplicação possui funcionalidades de criação de contas, autenticação, gerenciamento de senhas, controle de sessão, proteção de rotas, controle de navegação e monitoramento da disponibilidade do sistema.

Essa divisão permite que cada funcionalidade tenha uma responsabilidade específica e que as informações possam ser reutilizadas entre diferentes partes da aplicação sem duplicação desnecessária de dados.
