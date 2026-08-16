# CertiFlow

<img width="700" height="auto" alt="image" src="https://github.com/user-attachments/assets/1f3cda43-8d3b-47a4-bda9-47ab54f80b1d" />


**Project description**
CertiFlow är ett webbaserat kvalitetssystem för industrin, utvecklat i .NET 8 med Blazor Interactive Server, EF Core och PostgreSQL. Systemet hanterar kunder, jobb, verktyg och mätningar med fokus på spårbarhet, autentisering och strukturerad kvalitetsdata.

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
Local application secrets are stored using .NET User Secrets.
Environment files and database volumes are not version controlled:

```text
.env
.env.dev
.env.local
postgres_data/
postgres_data_dev/
---

## Database environments

The project uses separate PostgreSQL databases for:

- Local development
- VPS Development environment
- VPS Production environment

### Database

Started via:
```bash
docker compose -f <docker-compose-file> up -d
```

Access PostgreSQL:
```bash
docker exec -it <db-container-name> \
psql -U <user-name> -d <database-name>
```
---

## Database creation/migrations

CertiFlow uses Entity Framework Core migrations to create and update the PostgreSQL schema.
Local and Prod migrations are applied manually:

```
dotnet ef database update

```

while dev migrations has been automated from Program.cs:

```
Update VPS development environment
1. Verify that you are on the develop branch.
```
git branch --show-current
```
If not already on develop:
```
git checkout develop
```
2. Pull the latest changes:
```
git pull
```
3. Enter the SSH key password if prompted.
4. Rebuild and start the development containers:
```
docker compose -f docker-compose.dev.yml up -d --build
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

## Run locally

Start Docker Desktop
Run application:

```bash
cd CertiFlowApp
dotnet run

NB! Local app uses .NET not a Docker container.
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

---

## Seed-data
### Develop + Prod
docker compose --env-file <env-file> -f <compose-file> exec -T <db-service-name> \
  psql -U <db-user> -d <database-name> \
  < ~/apps/certiflow-seed/SeedDevelopmentBase.sql

### Prod
docker compose --env-file <env-file> -f <compose-file> exec -T <db-service-name> \
  psql -U <db-user> -d <database-name> \
  < ~/apps/certiflow-seed/SeedProductionWorkflow.sql

---
