# Changelog

All notable changes to this project will be documented in this file.

This project adheres to Semantic Versioning.

---

## [Unreleased]

### Planned
- En första MVP measurement workflow
- Authentication och authorization
- En första databas-migration

---

## [0.2.1] - 2026-05-18

### Added
- Global error handling
- User-friendly error page
- Logging of unexpected exceptions

### Changed
- Improved application stability during runtime errors

---

## [0.2.0] - 2026-05-18

### Added
- Basic application logging
- Global error handling
- Secure logging
- Environment-based log levels
- User-friendly error page
- Console logging with timestamps

### Changed
- Improved application stability and diagnostics

---

## [0.1.0] - 2026-04-28 - 2026-05-17

### Added
- Docker baserad dev miljö
- PostgreSQL integration med EF Core
- Separerade Development och production miljöer
- Traefik reverse proxy med HTTPS
- Persistent ASP.NET Core Data Protection keys
- `.env.example` mall och `README.md`

### Security
- User Secrets för lokal development
- Environment variables för deployment-miljön

### Infrastructure
- Separata Docker Compose filer för dev och prod
- Separata PostgreSQL databaser för dev och prod