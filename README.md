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

## Database environments

The project uses separate PostgreSQL databases for:

- Local development
- VPS Development environment
- VPS Production environment

### Local database

Started via:
```bash
docker compose -f docker-compose.local.yml up -d
```

Access PostgreSQL:
```bash
docker exec -it certiflow-local-db \
psql -U certiflow_local_user -d certiflow_local_db
```

### Development database (VPS)

Started via:
```bash
docker compose -f docker-compose.dev.yml up -d
```

Access PostgreSQL:
```bash
docker exec -it certiflow-db-dev \
psql -U <dev_user> -d <dev_database>
```

### Production database (VPS)

Started via:
```bash
docker compose -f docker-compose.yml up -d
```
Access PostgreSQL:
```bash
docker exec -it certiflow-db \
psql -U <prod_user> -d <prod_database>
```

---

## Authentication and Identity

CertiFlow uses ASP.NET Core Identity with Entity Framework Core and PostgreSQL.

Passwords are stored using ASP.NET Core Identity password hashing.
The application does not implement custom password hashing or custom authentication logic.

Account lockout is enabled for repeated failed login attempts:

- Maximum failed attempts: 5
- Lockout duration: 5 minutes
- Lockout enabled for new users

Administrative pages require authentication using ASP.NET Core authorization.

Scaffolded Identity UI is only used for the pages that require customization.
The project still uses ASP.NET Core Identity for password hashing, cookies, lockout, and authentication.

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