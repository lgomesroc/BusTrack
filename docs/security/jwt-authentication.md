# Autenticação JWT e autorização

## Autenticação

O BusTrack emite JWTs assinados após login válido. O algoritmo utilizado é HMAC-SHA256 e a validade padrão é de 30 minutos.

Endpoints protegidos exigem:

```http
Authorization: Bearer <token>
```

A API valida assinatura, issuer, audience e expiração. A chave é obrigatória e deve possuir pelo menos 32 bytes.

O token de acesso é mantido no `sessionStorage` pelo frontend. O logout remove o token local.

## Configuração local com Docker Compose

1. Copie `.env.example` para `.env` na raiz do repositório.
2. Gere um segredo aleatório com pelo menos 32 bytes e configure `BUSTRACK_JWT_SECRET`.
3. Inicie ou reconstrua o ambiente com `docker compose up -d --build`.

No PowerShell, uma chave Base64 aleatória pode ser gerada com:

```powershell
[Convert]::ToBase64String([Security.Cryptography.RandomNumberGenerator]::GetBytes(48))
```

O arquivo `.env` não deve ser versionado.

## Render / produção

No serviço backend, configure:

- `JwtSettings__SecretKey`: segredo aleatório exclusivo do ambiente, com pelo menos 32 bytes.
- `JwtSettings__Issuer`: emissor utilizado pela API.
- `JwtSettings__Audience`: audiência utilizada pelo cliente.
- `JwtSettings__ExpirationInMinutes`: validade do token, conforme a configuração do ambiente.

Nunca reutilize uma chave de teste em produção.

## Autorização USER / ADMIN

A API inclui a role do usuário como claim assinada no JWT.

- `USER`: possui as permissões destinadas ao usuário comum.
- `ADMIN`: possui as permissões administrativas previstas pela aplicação.
- O cadastro público sempre cria `USER`.
- O cliente não escolhe a role.
- Endpoints administrativos exigem `ADMIN` no backend.
- A interface Angular não é considerada mecanismo de autorização.

As regras são aplicadas também às rotas alternativas ou legadas que representem as mesmas operações.

## Sessões

A aplicação associa a sessão a um navegador por meio do identificador de navegador.

As regras atuais são:

- abas do mesmo navegador compartilham a sessão;
- navegadores diferentes podem manter sessões independentes;
- o mesmo navegador não mantém mais de uma sessão ativa;
- a sessão possui validade;
- existe controle de inatividade;
- existe controle de presença do usuário.

## Bootstrap do primeiro ADMIN

O primeiro administrador é criado por procedimento controlado e não pelo cadastro público.

O endpoint de bootstrap utiliza um segredo fornecido por configuração de ambiente. O segredo não deve ser versionado nem exposto ao frontend.

## Respostas esperadas

- `401 Unauthorized`: token ausente, inválido ou expirado.
- `403 Forbidden`: usuário autenticado sem a role necessária.
- `404 Not Found`: recurso inexistente, quando aplicável.

## Recursos V2

Os recursos abaixo não fazem parte da autenticação da V1 e estão planejados para evolução futura:

- login com Google;
- MFA/2FA;
- reCAPTCHA;
- autenticação/sessão baseada em cookies, caso seja adotada essa arquitetura.
