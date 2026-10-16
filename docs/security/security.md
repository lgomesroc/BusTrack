# Segurança

## Estado atual

O backend é a autoridade para autenticação e autorização. Regras de segurança não podem depender apenas do Angular: qualquer cliente pode chamar a API diretamente, por exemplo usando `curl`.

A aplicação utiliza JWT Bearer para autenticar as requisições protegidas. O token é assinado com HMAC-SHA256 e a API valida assinatura, emissor, público e validade. A chave de assinatura é fornecida por configuração de ambiente e deve ter pelo menos 32 bytes.

## Senhas

As senhas são armazenadas como hashes BCrypt, nunca em texto puro. O fluxo de cadastro e alteração de senha deve validar os dados no backend; validações apenas na interface podem ser contornadas.

O projeto também contém mecanismos relacionados a histórico e validade de senhas. A cobertura efetiva desses mecanismos deve ser confirmada nos serviços que tratam cada operação; a documentação não deve ser interpretada como prova de que todos os fluxos de senha estão completos.

## JWT e sessão

- Login e cadastro comum são endpoints anônimos.
- A verificação de saúde da aplicação é anônima para monitoramento.
- Os demais endpoints exigem um token válido por política padrão, salvo exceção explícita.
- O frontend armazena o token em `sessionStorage` e o envia como `Authorization: Bearer <token>` apenas para a API configurada.
- O frontend limpa a sessão quando o token expira ou uma chamada protegida retorna `401`.
- O logout remove o token local. Como não há revogação/refresh-token store, um token copiado pode continuar válido até expirar; nunca registre nem compartilhe tokens.

Consulte [JWT authentication](jwt-authentication.md) para configuração local e produção.

## Regras de autorização de negócio

O sistema deve aplicar as permissões no backend com roles (RBAC — Role-Based Access Control):

| Operação | USER | ADMIN |
|---|---:|---:|
| Consultar viagens, motoristas, passageiros, ônibus, rotas e manutenção | Permitido | Permitido |
| Criar viagens, motoristas, passageiros, ônibus, rotas e manutenção | Permitido | Permitido |
| Editar ou excluir esses recursos | Proibido | Permitido |
| Consultar a própria conta | Permitido | Permitido |
| Editar/excluir a própria conta | Proibido, salvo fluxo explicitamente aprovado | Permitido conforme endpoint administrativo |
| Listar ou gerenciar contas de outras pessoas | Proibido | Permitido por endpoint administrativo protegido |
| Criar/promover ADMIN pelo cadastro público ou frontend | Proibido | Proibido |
| Bootstrap controlado do primeiro ADMIN via ferramenta de linha de comando | Não aplicável | Procedimento operacional restrito |

As mesmas regras devem abranger endpoints duplicados/legados e relações como passageiros associados a viagens. Uma rota alternativa não pode permitir uma operação que a rota principal proíbe.

O cadastro público deve sempre criar uma conta com role `USER`, ignorando qualquer role enviada pelo cliente. A role deve vir de uma fonte confiável do backend e ser incluída como claim assinada no JWT. Nunca aceite uma role enviada pelo frontend como prova de autorização.

## Bootstrap do ADMIN

A criação do primeiro administrador não deve ser disponibilizada como botão ou formulário do frontend. O procedimento deve ser executado manualmente por um operador autorizado via `curl` no ambiente Linux, usando um segredo de bootstrap fornecido por variável de ambiente, nunca codificado no repositório.

O endpoint de bootstrap, quando habilitado, deve:
- recusar a operação se já existir um ADMIN;
- exigir segredo de bootstrap forte e configurado fora do código;
- não aceitar role ou identidade privilegiada escolhida livremente no cadastro público;
- não retornar nem registrar segredos ou hashes;
- falhar fechado se a configuração necessária estiver ausente;
- ser coberto por testes de primeira execução, repetição e segredo inválido.

O procedimento deve ser documentado e testado antes de ser usado em produção. Não execute bootstrap em produção até que essa proteção esteja implementada e validada.

## Respostas HTTP esperadas

- `401 Unauthorized`: token ausente, inválido ou expirado para endpoint protegido.
- `403 Forbidden`: identidade autenticada sem a role necessária.
- `404 Not Found`: recurso inexistente (quando apropriado, sem revelar dados de outro usuário).

## Proteção da interface

O Angular deve ocultar ações de edição/exclusão e páginas administrativas de usuários sem a role apropriada, melhorando a experiência de uso. Isso é apenas apresentação; o backend deve negar a operação mesmo se alguém modificar a interface ou chamar a API diretamente.

## Limitações e itens para revisão

- JWT de acesso é stateless; logout no navegador não revoga cópias do token.
- CORS limita origens de navegador, mas não é mecanismo de autorização e não impede chamadas via `curl`.
- Bloquear botão direito, seleção, cópia ou navegação do browser não protege dados fornecidos pela API.
- Revisar endpoints legados/duplicados, especialmente os de viagens e usuários.
- Validar os DTOs no backend, sem confiar nas validações do Angular.
- Garantir que exceções não revelem detalhes internos, credenciais ou dados sensíveis.
- Manter testes para token ausente/inválido, USER permitido, USER bloqueado (403), ADMIN permitido e tentativa de promoção indevida.

## Configuração

A chave JWT deve ser exclusiva por ambiente. Nunca coloque segredos reais em `appsettings.json`, no frontend, na documentação, no histórico Git ou em logs. Consulte [JWT authentication](jwt-authentication.md) para as variáveis necessárias.
