# JWT authentication

BusTrack issues signed JWT access tokens after a successful login. Tokens use the HMAC-SHA256 algorithm and expire after 30 minutes by default. Protected API endpoints require `Authorization: Bearer <token>`.

## Local Docker Compose

1. Copy `.env.example` to `.env` in the repository root.
2. Generate a random secret of at least 32 bytes and place it in `BUSTRACK_JWT_SECRET`.
3. Start or rebuild the environment with `docker compose up -d --build`.

PowerShell can generate a random Base64 secret with:

```powershell
[Convert]::ToBase64String([Security.Cryptography.RandomNumberGenerator]::GetBytes(48))
```

The local `.env` is ignored by Git. Never commit its contents.

## Render / production

Configure these environment variables on the backend service:

- `JwtSettings__SecretKey`: a unique, randomly generated secret of at least 32 bytes.
- `JwtSettings__Issuer`: `BusTrack` (or another stable issuer used consistently by the API).
- `JwtSettings__Audience`: `BusTrackFrontend` (or the configured client audience).
- `JwtSettings__ExpirationInMinutes`: `30` (optional; accepted range is 1–1440).

The API intentionally fails to start if the signing key, issuer, or audience is missing or invalid. Do not use a sample or local-development key in production.

For GitHub Actions, add a repository Actions secret named `BUSTRACK_JWT_SECRET`. The backend integration workflow passes it to Docker Compose. Existing `BUSTRACK_TEST_EMAIL` and `BUSTRACK_TEST_PASSWORD` secrets are still required.

## Authorization behavior

- Login and account registration are anonymous.
- The health probe is anonymous for deployment monitoring.
- All other API endpoints require a valid token by default.
- Account and authentication-user detail/update/delete routes are restricted to the identity in the token; password hashes are not returned by these read endpoints.
- The Angular app stores the access token in `sessionStorage`, sends it as a Bearer token, and clears the local session after token expiry or a protected request returning 401.

Logout clears the browser's token. Because this implementation uses stateless access tokens and no refresh/revocation store, a copied token can remain usable until its expiration; keep the lifetime short and never log or share tokens.
