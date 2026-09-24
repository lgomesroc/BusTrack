# Segurança

## Visão geral

A segurança do Bus Track é composta por mecanismos distribuídos entre backend e frontend. O backend é responsável principalmente pela proteção das credenciais, autenticação e acesso aos dados, enquanto o frontend possui mecanismos adicionais para controlar a sessão e determinadas formas de interação com a aplicação.

A solução também possui regras relacionadas à utilização das senhas, controle de sessão, indisponibilidade da aplicação e navegação pelo navegador.

Alguns mecanismos fazem parte da implementação atual, enquanto outros estão previstos para a evolução da aplicação.

---

## Proteção de senhas

As senhas dos usuários não devem ser armazenadas em texto puro.

O Bus Track utiliza **BCrypt** para gerar o hash das senhas antes de armazená-las no banco de dados.

Durante uma autenticação, a senha informada pelo usuário é comparada com o hash armazenado. Dessa forma, o sistema não precisa armazenar a senha original.

O BCrypt também é utilizado no controle do histórico de senhas, permitindo verificar se uma nova senha já foi utilizada anteriormente.

---

## Histórico de senhas

O sistema mantém um histórico das senhas utilizadas pelo usuário.

Quando uma senha é alterada, a nova senha é armazenada como hash e adicionada ao histórico.

O sistema verifica se a nova senha já foi utilizada anteriormente. Caso tenha sido utilizada, a alteração é rejeitada e o usuário precisa informar outra senha.

O histórico possui um limite de cinco senhas armazenadas.

Esse mecanismo reduz a possibilidade de o usuário alternar repetidamente entre as mesmas senhas.

---

## Política de alteração de senha

O Bus Track possui uma política de validade de senha.

A senha possui um período de utilização de até **45 dias**. Após esse período, o usuário deve realizar a alteração da senha.

O sistema também possui uma verificação relacionada ao tempo de utilização da senha para identificar quando ela está próxima do limite estabelecido.

O objetivo é evitar que uma mesma senha permaneça sendo utilizada indefinidamente.

---

## Autenticação

A autenticação é responsável por verificar as credenciais fornecidas pelo usuário e permitir o acesso às áreas protegidas da aplicação.

O frontend utiliza uma sessão controlada pelo `SessionService`. Quando o usuário realiza a autenticação, a sessão é iniciada. Quando a sessão é encerrada ou invalidada, suas informações são removidas.

O acesso às rotas protegidas também é controlado pelo `AuthenticationGuard`.

Quando uma rota exige autenticação e não existe uma sessão válida, o usuário é direcionado para a tela de entrada do sistema.

---

## JWT

A utilização de **JSON Web Token (JWT)** faz parte da evolução planejada da segurança do Bus Track.

Atualmente, o projeto possui configurações relacionadas ao JWT no arquivo de configuração, mas a autenticação baseada em JWT ainda não está implementada como mecanismo efetivo de autenticação da API.

A implementação futura deverá utilizar tokens para representar a autenticação do usuário nas requisições protegidas da API.

A implementação deverá contemplar a geração do token, validação, expiração e proteção das rotas que exigem autenticação.

Portanto, JWT não deve ser considerado uma funcionalidade já implementada no estado atual do projeto.

---

## Controle da sessão

O frontend utiliza `sessionStorage` para manter o estado de autenticação durante a sessão do navegador.

A aplicação possui um serviço específico para controlar esse estado.

Quando uma nova sessão é iniciada, o estado de autenticação é registrado.

Quando a sessão precisa ser encerrada, o registro é removido.

A aplicação também limpa uma eventual sessão existente durante uma nova inicialização do frontend. Isso evita que uma sessão anterior seja automaticamente restaurada depois que a aplicação é encerrada e iniciada novamente.

---

## Controle de indisponibilidade

O frontend possui um mecanismo de monitoramento da disponibilidade da aplicação.

Esse mecanismo verifica a comunicação com a API e com o próprio frontend.

Quando a aplicação perde a comunicação durante uma sessão autenticada, o sistema invalida a sessão atual e bloqueia a interação com a página que estava aberta.

O usuário recebe uma mensagem informando que a comunicação com o sistema foi perdida e é direcionado novamente para a tela de autenticação.

Esse comportamento impede que uma sessão continue sendo utilizada normalmente depois que a aplicação deixa de estar disponível.

---

## Controle de inatividade

A aplicação possui um temporizador relacionado à sessão do usuário.

Após o período definido de **20 minutos de inatividade**, a sessão pode ser encerrada de acordo com as regras implementadas para controle de utilização.

Esse mecanismo tem como objetivo reduzir o período em que uma sessão permanece disponível quando o usuário deixa a aplicação aberta sem interação.

O controle de inatividade complementa os demais mecanismos de controle da sessão.

---

## Proteção de credenciais no frontend

O frontend possui regras para evitar que determinadas informações sensíveis sejam manipuladas por mecanismos comuns de interação da interface.

Entre os comportamentos implementados estão regras relacionadas à entrada de senhas e à forma como determinadas informações podem ser selecionadas ou copiadas.

Esses mecanismos funcionam como uma camada adicional de proteção da interface.

Eles não substituem mecanismos de segurança do backend, autenticação, autorização ou proteção adequada das informações armazenadas.

---

## Bloqueio de cópia e seleção

A aplicação possui regras para restringir a seleção e a cópia de determinadas informações exibidas na interface.

O objetivo é dificultar a utilização das operações convencionais de copiar e selecionar conteúdo através do teclado ou do mouse.

Esse mecanismo deve ser entendido como uma restrição de interação da interface e não como uma proteção absoluta contra extração de informações.

Um usuário com acesso técnico ao navegador ou às ferramentas de desenvolvimento ainda pode utilizar outros meios para obter informações disponibilizadas pelo frontend.

---

## Desativação do botão direito

O botão direito do mouse é desativado na aplicação.

Essa regra impede a abertura do menu de contexto convencional do navegador dentro da interface do sistema.

Assim como as demais regras de interação do frontend, essa medida possui caráter complementar e não deve ser considerada um mecanismo de segurança absoluto.

---

## Bloqueio da navegação pelo navegador

O Bus Track possui um controle específico para impedir a utilização dos botões **Voltar** e **Avançar** do navegador dentro da aplicação.

Esse comportamento é controlado pelo `BrowserNavigationGuard`, integrado ao sistema de rotas do Angular.

Quando uma tentativa de navegação através do histórico do navegador é identificada, a navegação é bloqueada e o usuário recebe uma mensagem informando que esse tipo de navegação está desativado.

A regra é aplicada tanto às áreas públicas quanto às áreas autenticadas definidas no sistema de rotas.

---

## Proteção das rotas

As áreas autenticadas da aplicação utilizam o `AuthenticationGuard`.

Esse mecanismo verifica se existe uma sessão válida antes de permitir o acesso a determinadas rotas.

Caso o usuário não esteja autenticado, a navegação para a área protegida é bloqueada e o usuário é direcionado para a tela de entrada do sistema.

O controle de rota é uma proteção no frontend. A proteção definitiva dos recursos da aplicação deverá ser realizada também no backend.

---

## Responsabilidade do frontend e do backend

As regras de segurança não ficam concentradas em uma única camada.

O frontend controla aspectos relacionados à sessão, navegação e interação com a interface.

O backend é responsável pela autenticação, processamento das credenciais, acesso aos dados e aplicação das regras que não podem depender exclusivamente do navegador.

Isso é importante porque qualquer mecanismo executado exclusivamente no frontend pode ser contornado por um usuário com conhecimento técnico ou acesso às ferramentas do navegador.

Por esse motivo, regras de segurança relacionadas a autorização e acesso aos dados devem permanecer no backend.

---

## Mecanismos atualmente implementados

Entre os mecanismos de segurança e controle atualmente presentes no projeto estão:

* Hash de senhas utilizando BCrypt.
* Verificação de histórico de senhas.
* Limitação do histórico de senhas.
* Política relacionada à validade da senha.
* Controle de sessão através do `SessionService`.
* Proteção de rotas através do `AuthenticationGuard`.
* Monitoramento da disponibilidade da aplicação.
* Invalidação da sessão quando a aplicação perde comunicação.
* Controle de inatividade.
* Restrições de seleção e cópia de informações.
* Desativação do botão direito.
* Bloqueio da navegação pelos botões Voltar e Avançar do navegador.

---

## Mecanismos planejados

A principal evolução relacionada à autenticação é a implementação efetiva de **JWT**.

A implementação deverá integrar o mecanismo de tokens ao processo de autenticação e à proteção das requisições da API.

Também deverão ser avaliados mecanismos complementares de autorização e proteção de endpoints conforme a evolução dos módulos da aplicação.

---

## Limitações das proteções do frontend

As restrições implementadas no frontend possuem uma finalidade complementar.

Desativar o botão direito, impedir seleção, limitar cópia ou bloquear determinadas formas de navegação não impede tecnicamente que um usuário com conhecimento suficiente obtenha informações disponibilizadas pelo navegador.

Essas regras existem para controlar a experiência de utilização da aplicação e dificultar operações comuns, mas não substituem autenticação, autorização, proteção de endpoints, armazenamento seguro de credenciais ou outras medidas aplicadas no backend.

---

## Resumo

A segurança do Bus Track combina proteção de credenciais, controle de sessão, proteção de rotas, política de senhas, monitoramento da disponibilidade e restrições de interação no frontend.

O BCrypt já é utilizado para proteção das senhas. O controle de sessão e as regras de navegação e interação também fazem parte da implementação atual.

JWT está previsto como evolução da autenticação e ainda não deve ser considerado uma funcionalidade implementada.

A separação dessas responsabilidades permite que mecanismos de segurança sejam aplicados de acordo com a camada responsável por cada aspecto da aplicação.
