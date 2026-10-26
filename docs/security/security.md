# Segurança

## Estado atual

O backend é a autoridade para autenticação e autorização. As regras de segurança não dependem apenas do Angular: qualquer cliente pode chamar a API diretamente.

A aplicação utiliza JWT Bearer para autenticar requisições protegidas. O token é assinado com HMAC-SHA256 e a API valida assinatura, emissor, público e validade. A chave de assinatura é fornecida por configuração de ambiente e deve possuir pelo menos 32 bytes.

## Senhas

As senhas são armazenadas como hashes BCrypt, nunca em texto puro.

A V1 possui:

- alteração de senha;
- recuperação de senha;
- redefinição de senha;
- histórico de senhas;
- política de senhas.

As regras de senha são aplicadas no backend e não dependem apenas das validações da interface.

## JWT e sessão

- O login e o cadastro comum são endpoints anônimos.
- A verificação de saúde da aplicação é anônima para monitoramento.
- Os demais endpoints protegidos exigem autenticação válida, salvo exceção explícita.
- O frontend mantém o token em `sessionStorage` e o envia como `Authorization: Bearer <token>` para a API.
- O logout remove o token local.
- A sessão possui validade e controle de inatividade.
- O sistema identifica o navegador associado à sessão.
- Abas do mesmo navegador compartilham a sessão.
- Navegadores diferentes podem possuir sessões independentes.
- O mesmo navegador não mantém mais de uma sessão ativa.
- O sistema possui controle de presença do usuário.

Como o JWT de acesso é stateless, o logout local não revoga cópias do token que tenham sido obtidas anteriormente; elas permanecem válidas até sua expiração conforme as regras da aplicação.

## Autorização USER / ADMIN

A API utiliza RBAC (Role-Based Access Control).

- `USER`: executa as operações permitidas para usuários comuns.
- `ADMIN`: possui as permissões administrativas previstas pela aplicação.
- O cadastro público cria somente `USER`.
- O cliente não pode escolher ou promover a própria role.
- Endpoints administrativos exigem `ADMIN` no backend.
- A interface não é considerada mecanismo de autorização.

As mesmas regras devem ser aplicadas a rotas alternativas ou legadas que representem as mesmas operações.

## Dashboard administrativo

O Dashboard administrativo é protegido por role `ADMIN` e disponibiliza recursos administrativos, incluindo consulta, pesquisa, paginação de usuários e informações de presença/usuários online.

## Bootstrap do ADMIN

A criação do primeiro administrador é um procedimento controlado e não faz parte do cadastro público.

O endpoint de bootstrap utiliza um segredo fornecido por configuração de ambiente e não deve receber credenciais administrativas por meio de parâmetros controlados pelo frontend.

## Respostas HTTP esperadas

- `401 Unauthorized`: autenticação ausente, inválida ou expirada.
- `403 Forbidden`: usuário autenticado sem a permissão necessária.
- `404 Not Found`: recurso inexistente, quando aplicável.

## Proteção da interface

O Angular pode ocultar ações que o usuário não possui permissão para executar. Essa proteção é apenas de apresentação. A API deve negar a operação quando a role não for suficiente.

## Presença e múltiplos navegadores

A V1 possui controle de sessão por navegador e presença do usuário.

A regra atual permite que o mesmo usuário mantenha sessões independentes em navegadores diferentes, enquanto impede mais de uma sessão no mesmo navegador. Abas do mesmo navegador compartilham a sessão existente.

## Recursos fora da V1

Os seguintes mecanismos são planejados para a V2 e não são pendências da V1:

- autenticação com Google;
- MFA/2FA;
- reCAPTCHA;
- adoção de cookies como mecanismo de autenticação/sessão, caso essa arquitetura seja escolhida.

## Limitações arquiteturais

- JWT de acesso é stateless.
- CORS não é mecanismo de autorização.
- Restrições de interface, como bloquear cópia ou menu de contexto, não substituem segurança no backend.
- Segredos nunca devem ser armazenados no repositório, frontend, documentação ou logs.
