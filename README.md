# CertiFlow Infrastructure

## Overview

CertiFlow runs on a single VPS using:

- ASP.NET Core (.NET 8)
- PostgreSQL
- Docker & Docker Compose
- Traefik reverse proxy

The VPS hosts both:

- Development environment
- Production environment

Environments are separated using:

- separate Docker Compose files
- separate containers
- separate databases
- separate domains
- separate ASP.NET Core environments

---

## Domains

| Environment | Domain |
|---|---|
| Production | https://certiflow.sarasjodin.se |
| Development | https://dev-certiflow.sarasjodin.se |

---

## Git Flow

| Branch | Purpose |
|---|---|
| `main` | Production |
| `develop` | Development |
| `feature/*` | Feature work |

Deployment flow:

```text
feature/* → develop → dev deploy
develop → main → production deploy
```

---

## Docker Compose Files

| File | Purpose |
|---|---|
| `docker-compose.local.yml` | Local development |
| `docker-compose.dev.yml` | Development server |
| `docker-compose.yml` | Production server |

---

## Environment Variables

Development:

```yaml
ASPNETCORE_ENVIRONMENT: "Development"
APP_ENVIRONMENTAL_LABEL: "Dev"
```

Production:

```yaml
ASPNETCORE_ENVIRONMENT: "Production"
APP_ENVIRONMENTAL_LABEL: "Production"
```

---

## Deployment

### Development

```bash
git checkout develop
git pull
docker compose -f docker-compose.dev.yml up -d --build
```

### Production

```bash
git checkout main
git pull
docker compose -f docker-compose.yml up -d --build
```

---

## Run locally

Start PostgreSQL:

```bash
docker compose -f docker-compose.local.yml up -d
```

Run application:

```bash
cd CertiFlowApp
dotnet run
```

Application runs on:
https://localhost:xxxx

---

## Requirements

- .NET 8 SDK
- Docker Desktop

## Security

---

Secrets and runtime data are NOT version controlled.

Examples:

```text
.env
.env.dev
postgres_data/
postgres_data_dev/
```