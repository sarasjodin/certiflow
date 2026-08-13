# Changelog

All notable changes to this project will be documented in this file.

This project adheres to Semantic Versioning.

---

## [Unreleased]

### Future development

- Administrator management of user roles
- Extended verification and approval workflows
- Client-specific functionality
- Further Quality Dashboard improvements

### Planned

- Fixes identified during functional testing

---

## [0.6.0] - 2026-08-13

### Added

- Role-based authorization for application roles
- Pending account page for users without an assigned role
- Authorization policies for internal and approval workflows

### Changed

- Restricted CRUD and administration pages based on roles
- Authenticated internal users are redirected to the Quality Dashboard
- Improved public and authenticated layouts
- Redesigned public dashboard with statistic cards
- Improved delete handling for related data

---

## [0.5.0] - 2026-08-11

### Added

- Public dashboard as the home page
- Public dashboard DTO and service
- Public statistics for approved jobs, registered measurements, and available tools
- Login option for unauthenticated users

### Changed

- Updated navigation based on authentication status
- Management navigation is only shown to authenticated users
- Replaced the previous home page with the public dashboard


## [0.4.0] - 2026-08-10

### Added
- CRUD for Customers, Jobs, Tools, and Measurements
- New form input models and read models
- Services using IDbContextFactory
- Tool calibration logic
- Measurement workflow with Job and Tool relationships
- Automatic MeasuredAtUtc and current user assignment
- Shared date and time formatting
- New database changes and EF Core migrations
- Automatic application of pending migrations in Development
- Updated navigation and new UI pages

## [0.3.0] - 2026-08-03

### Added
- Added initial MVP domain model for Customers, Jobs, Measurements, Tools, Deviations and AuditLogs
- Added shared AuditableEntity base class for audit fields
- Added business-related enums for jobs, measurements, deviations, audit actions and tool calibration
- Added separate EF Core configuration classes for all domain entities using IEntityTypeConfiguration
- Added initial MVP database migration (AddMvpDomainModel)
- Added project .editorconfig for consistent code formatting and automatic removal of unused using directives

### Changed
- Updated AppDbContext to register domain entities
- Updated MVP database documentation to match the implemented domain model

### TEsted
- Verified successful migration against the local PostgreSQL database

## [0.2.7] - 2026-05-20

### Added
- Added Identity role seeding for Operator, Verifier, Approver, SystemAdmin and Client
- Added centralized ApplicationRoles constants for role management
- Added development test users with predefined roles
- Added support for users with multiple roles
- Added environment-based seed email domains for local and dev environments

### Fixed
- Fixed Identity role registration by enabling AddRoles<IdentityRole>()
- Fixed startup order so role seeding runs before user seeding
- Fixed development authorization seed setup for ASP.NET Core Identity

---

## [0.2.6] - 2026-05-20

### Fixed

- Sync env_file config between `docker-compose.yml` and `docker-compose.dev.yml`

---

## [0.2.5] - 2026-05-19

### Added
- Basic ASP.NET Core Identity security settings
- Protected admin page requiring authentication
- Brief documentation of the selected Identity solution

### Fixed
- Fixed database configuration for Docker-based Development and Production environments.
- Replaced ASP.NET Core connection string configuration with explicit PostgreSQL environment variables.

### Changed
- Updated database configuration to use:
  - POSTGRES_HOST
  - POSTGRES_PORT
  - POSTGRES_DB
  - POSTGRES_USER
  - POSTGRES_PASSWORD
- Standardized environment handling across local, Development, and Production environments.
- Updated `.env.local.example` with explicit localhost and PostgreSQL port configuration.

### Security
- Password hashing handled by ASP.NET Core Identity
- Account lockout configured for failed login attempts
- Administrative pages protected with ASP.NET Core authorization
- Established ASP.NET Core Identity mechanisms used instead of custom authentication logic
- Continued using User Secrets for local development secrets
- Continued using `.env` files for VPS Development and Production environments

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


## Legend

- **Added**: new features or components
- **Changed**: updates to existing behavior
- **Deprecated**: soon-to-be removed features
- **Removed**: deprecated features now gone
- **Fixed**: bug fixes
- **Security**: security-related fixes or enhancements
- **Notes**: related comments, limitations, or clarifications