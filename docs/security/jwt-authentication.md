# Autenticação JWT e autorização

## Autenticação

O BusTrack emite JWTs assinados após login válido. O algoritmo é HMAC-SHA256 e a validade padrão é de 30 minutos. Endpoints protegidos exigem `Authorization: Bearer <token>`.

A API valida assinatura, issuer, audience e expiração. A chave é obrigatória e deve ter pelo menos 32 bytes. O token de acesso é guardado no `sessionStorage`; o logout limpa o armazenamento local, mas não revoga tokens copiados antes da expiração.

## Configuração local com Docker Compose

1. Copie `.env.example` para `.env` na raiz do repositório.
2. Gere um segredo aleatório com pelo menos 32 bytes e defina `BUSTRACK_JWT_SECRET`.
3. Inicie ou reconstrua o ambiente com `docker compose up -d --build`.

No PowerShell, gere uma chave Base64 aleatória:

```powershell
[Convert]::ToBase64String([Security.Cryptography.RandomNumberGenerator]::GetBytes(48))
```

Não versione o arquivo `.env`.

## Render / produção

Configure no serviço backend:

- `JwtSettings__SecretKey`: segredo aleatório exclusivo com pelo menos 32 bytes.
- `JwtSettings__Issuer`: `BusTrack` (ou outro emissor estável, igual ao validado pela API).
- `JwtSettings__Audience`: `BusTrackFrontend` (ou a audiência configurada para o cliente).
- `JwtSettings__ExpirationInMinutes`: `30` (opcional; intervalo aceito de 1 a 1440).

A API deve falhar ao iniciar se a chave, o issuer ou a audience forem ausentes ou inválidos. Nunca reutilize uma chave de teste em produção.

O GitHub Actions gera uma chave temporária para os testes do Docker Compose.

## Autorização USER / ADMIN

A API emite um claim de role assinado no JWT. A política aplicada aos recursos operacionais é:

- `USER`: consultar e criar viagens, motoristas, passageiros, ônibus, rotas e manutenções.
- `ADMIN`: as mesmas operações, além de editar e excluir esses recursos.
- Consultar a própria conta é permitido a ambos.
- USER não pode editar/excluir a própria conta nem administrar outras contas, salvo futura regra explícita aprovada.
- O cadastro público cria somente USER; o cliente não escolhe a role.
- Endpoints administrativos exigem role ADMIN no backend. Ocultar botões no Angular não é proteção suficiente.

As regras devem cobrir todos os endpoints, inclusive rotas antigas/duplicadas e associações de passageiros a viagens. Respostas esperadas: 401 para token ausente/inválido/expirado e 403 para usuário autenticado sem permissão.

## Bootstrap do primeiro ADMIN

O endpoint `POST /AuthenticationControllerAPI/bootstrap-admin` usa a variável de ambiente `BootstrapAdmin__Secret`, cria o primeiro ADMIN se nenhum existir e recusa segredo incorreto. Sem a configuração, o endpoint fica indisponível. Ele não é usado pelo Angular; a operação é feita por operador autorizado via `curl` em Linux. Configure o valor fora do código, não o versione e remova a configuração após criar o administrador. Uma trava persistida no MongoDB impede dois bootstraps simultâneos; se o processo for interrompido durante a operação, revise a trava e o estado da conta antes de tentar novamente.

## Testes obrigatórios

- Token ausente/inválido em endpoint protegido: 401.
- USER consultando/criando recurso permitido: sucesso.
- USER editando/excluindo recurso operacional: 403.
- ADMIN editando/excluindo recurso operacional: sucesso quando o recurso existe.
- Cadastro público com `role=ADMIN`: nunca cria ADMIN.
- Bootstrap com segredo errado ou ADMIN já existente: recusado.
- Bootstrap válido no estado inicial: cria apenas o primeiro ADMIN.
- Nenhum endpoint legado deve contornar essas regras.
