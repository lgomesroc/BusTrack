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

A política desejada para recursos operacionais é:

- `USER`: consultar e criar viagens, motoristas, passageiros, ônibus, rotas e manutenções.
- `ADMIN`: as mesmas operações, além de editar e excluir esses recursos.
- Consultar a própria conta é permitido a ambos.
- USER não pode editar/excluir a própria conta nem administrar outras contas, salvo futura regra explícita aprovada.
- O cadastro público cria somente USER; o cliente não escolhe a role.
- Endpoints administrativos exigem role ADMIN no backend. Ocultar botões no Angular não é proteção suficiente.

As regras devem cobrir todos os endpoints, inclusive rotas antigas/duplicadas e associações de passageiros a viagens. Respostas esperadas: 401 para token ausente/inválido/expirado e 403 para usuário autenticado sem permissão.

## Bootstrap do primeiro ADMIN

A criação inicial do ADMIN não deve estar disponível no frontend. O procedimento será executado por operador autorizado via `curl` em Linux, usando um segredo de bootstrap configurado fora do código. A operação deve falhar se já existir ADMIN e se o segredo não corresponder. Se a configuração do segredo estiver ausente, o bootstrap deve ficar desabilitado.

**Não use o procedimento em produção até que o endpoint de bootstrap e seus testes estejam implementados e revisados.** Nunca inclua segredo de bootstrap no repositório ou no comando salvo no histórico de shell; prefira variável de ambiente e entrada segura.

## Testes obrigatórios

- Token ausente/inválido em endpoint protegido: 401.
- USER consultando/criando recurso permitido: sucesso.
- USER editando/excluindo recurso operacional: 403.
- ADMIN editando/excluindo recurso operacional: sucesso quando o recurso existe.
- Cadastro público com `role=ADMIN`: nunca cria ADMIN.
- Bootstrap com segredo errado ou ADMIN já existente: recusado.
- Bootstrap válido no estado inicial: cria apenas o primeiro ADMIN.
- Nenhum endpoint legado deve contornar essas regras.
