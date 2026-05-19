# Changelog

All notable changes to this project will be documented in this file.

This project adheres to Semantic Versioning.

---

## [Unreleased]

### Planned
- Roles and authorization
- Initial domain model and business-related database migration

---

## [0.2.4] - 2026-05-19

### Added
- ASP.NET Core Identity account lockout configuration
- Lockout verification using PostgreSQL and Identity UserManager
- Protected admin page using ASP.NET Core authorization
- Scaffolded Identity Login and Lockout pages
- Authentication and Identity documentation in README.md

### Changed
- Enabled lockout on failed login attempts
- Enabled ASP.NET Core authentication and authorization middleware
- Restricted default admin user seeding to Development environment only

### Security
- Password hashing using ASP.NET Core Identity
- Account lockout after repeated failed login attempts
- Authentication required for administrative pages
- Established ASP.NET Core Identity security mechanisms used instead of custom authentication logic

---

## [0.2.3] - 2026-05-19

### Added
- Login flow using ASP.NET Core Identity UI
- Direct logout endpoint using POST
- Temporary authentication status in the main layout in Swedish for MVP testing

---

## [0.2.2] - 2026-05-18

### Added
- ASP.NET Core Identity setup
- Identity integration with Entity Framework Core and PostgreSQL
- ApplicationUser entity
- Initial Identity database migration
- Identity tables for authentication and authorization

### Security
- Secure password hashing through ASP.NET Core Identity
- Foundation for authentication and authorization

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
- Docker-based development environment
- PostgreSQL integration with EF Core
- Separate Development and Production environments
- Traefik reverse proxy with HTTPS
- Persistent ASP.NET Core Data Protection keys
  to avoid issues with cookies, antiforgery tokens,
  and authentication after container rebuilds
- `.env.example` template and `README.md`

### Security
- User Secrets for local development
- Environment variables for deployment environments

### Infrastructure
- Separate Docker Compose files for Development and Production
- Separate PostgreSQL databases for Development and Production