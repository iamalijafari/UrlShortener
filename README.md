# URL Shortener API

[![Build](https://github.com/iamalijafari/UrlShortener/actions/workflows/ci.yml/badge.svg)](https://github.com/iamalijafari/UrlShortener/actions/workflows/ci.yml)
![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-17-4169E1?logo=postgresql&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-Enabled-2496ED?logo=docker&logoColor=white)
![Swagger](https://img.shields.io/badge/OpenAPI-Swagger-85EA2D?logo=swagger)
![License](https://img.shields.io/github/license/iamalijafari/UrlShortener)

A production-ready URL Shortener API built with **ASP.NET Core 9**, following **Clean Architecture**, **Domain-Driven Design (DDD)**, and **CQRS** principles.

This project demonstrates modern backend development practices including layered architecture, MediatR, FluentValidation, Entity Framework Core, PostgreSQL, Docker, Health Checks, and comprehensive testing.

---

## Features

- Create shortened URLs
- Redirect users using short URLs
- Retrieve URL information by short code
- Disable shortened URLs
- URL expiration support
- Automatic database migrations
- Health Checks
- Swagger documentation
- Global exception handling
- FluentValidation
- Repository Pattern
- CQRS with MediatR
- Docker & Docker Compose support
- Unit and Integration Tests

---

## Tech Stack

| Technology            | Purpose              |
| --------------------- | -------------------- |
| ASP.NET Core 9        | Web API              |
| C#                    | Programming Language |
| Entity Framework Core | ORM                  |
| PostgreSQL            | Database             |
| MediatR               | CQRS                 |
| FluentValidation      | Request Validation   |
| Swagger / OpenAPI     | API Documentation    |
| Docker                | Containerization     |
| xUnit                 | Testing              |

---

# Architecture

The project follows **Clean Architecture** to keep business logic isolated from infrastructure and presentation concerns.

```
src
│
├── UrlShortener.Api
│
├── UrlShortener.Application
│
├── UrlShortener.Domain
│
└── UrlShortener.Infrastructure

tests
│
├── UrlShortener.Api.Tests
└── UrlShortener.Domain.Tests
```

Dependency flow

```
API
 ↓
Application
 ↓
Domain
 ↑
Infrastructure
```

The Domain layer has no dependency on external frameworks.

---

# Implemented Features

### Create Short URL

Creates a new shortened URL.

```
POST /api/shorturls
```

Example

```json
{
  "originalUrl": "https://github.com"
}
```

---

### Get Short URL

Returns information about an existing short URL.

```
GET /api/shorturls/{code}
```

---

### Redirect

Redirects to the original URL.

```
GET /{code}
```

---

### Disable Short URL

Disables an existing short URL.

```
PATCH /api/shorturls/{code}/disable
```

---

### URL Expiration

Expired URLs cannot be redirected.
Shortened URLs expire after 5 days by default.

---

### Health Check

```
GET /health
```

Checks the health of the API and PostgreSQL database.

---

# Validation

Validation is implemented using **FluentValidation** and MediatR pipeline behaviors.

Examples:

- Invalid URL
- Empty URL
- Invalid short code

---

# Exception Handling

Centralized exception handling middleware returns consistent API responses.

Handled exceptions include:

- ValidationException
- NotFoundException
- DomainException

---

# Swagger Documentation

Swagger is included with XML documentation.

Available when running locally:

```
/swagger
```

API endpoints include:

- XML documentation
- Request descriptions
- Response descriptions
- HTTP status codes

---

# Docker Support

Run the complete application using Docker.

```
docker compose up --build
```

This starts:

- ASP.NET Core API
- PostgreSQL

Automatic EF Core migrations are applied on startup.

---

# Running Locally

Clone the repository

```bash
git clone https://github.com/iamalijafari/urlshortener.git
```

Navigate to the project

```bash
cd urlshortener
```

Update the connection string inside

```
appsettings.json
```

Example

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5433;Database=UrlShortener;Username=postgres;Password=postgres"
}
```

Apply migrations

```bash
dotnet ef database update
```

Run the application

```bash
dotnet run --project src/UrlShortener.Api
```

---

# Running with Docker

```bash
docker compose up --build
```

Swagger

```
http://localhost:8080/swagger
```

Health Check

```
http://localhost:8080/health
```

---

# Testing

Run all tests

```bash
dotnet test
```

The solution contains:

- Domain Tests
- Application Tests
- Integration Tests
- Validation Tests
- Redirect Tests
- Expiration Tests
- Disable Tests

---

# Project Highlights

- Clean Architecture
- Domain-Driven Design (DDD)
- CQRS with MediatR
- Entity Framework Core
- PostgreSQL
- FluentValidation
- Repository Pattern
- Health Checks
- Docker Support
- Swagger Documentation
- Automatic Database Migration
- Global Exception Handling
- Unit & Integration Testing

---

# Future Improvements

- JWT Authentication
- User Management
- Click Analytics
- Custom Short URLs
- QR Code Generation
- Redis Cache
- API Versioning
- GitHub Actions (CI/CD)
- Rate Limiting
- OpenTelemetry

---

# Author

**Ali Jafari**

Backend Developer

- ASP.NET Core
- C#
- .NET
- PostgreSQL
- Clean Architecture
- Domain-Driven Design

---

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
