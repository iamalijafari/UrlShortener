# URL Shortener API

A production-ready URL Shortener API built with **ASP.NET Core**, **Clean Architecture**, **Domain-Driven Design (DDD)**, **CQRS**, and **PostgreSQL**.

The project demonstrates modern backend development practices including validation, domain modeling, testing, and layered architecture.

---

## Features

- Create shortened URLs
- Redirect to the original URL
- Retrieve URL information by short code
- Disable shortened URLs
- URL expiration support
- Domain-driven design
- CQRS with MediatR
- FluentValidation
- Global exception handling
- Repository Pattern
- Entity Framework Core
- PostgreSQL
- Comprehensive unit and integration tests

---

## Tech Stack

| Technology            | Description        |
| --------------------- | ------------------ |
| ASP.NET Core          | Web API            |
| .NET                  | Backend Framework  |
| Entity Framework Core | ORM                |
| PostgreSQL            | Database           |
| MediatR               | CQRS               |
| FluentValidation      | Request Validation |
| xUnit                 | Testing            |
| Clean Architecture    | Project Structure  |
| Domain-Driven Design  | Domain Modeling    |

---

# Architecture

The solution follows **Clean Architecture** principles.

```
Presentation
│
├── UrlShortener.Api
│
Application
│
├── Commands
├── Queries
├── Behaviors
├── Validators
│
Domain
│
├── Entities
├── Value Objects
├── Repositories
│
Infrastructure
│
├── EF Core
├── PostgreSQL
├── Repository Implementations
│
Tests
```

Dependency Flow

```
API
 ↓
Application
 ↓
Domain
 ↑
Infrastructure
```

The Domain layer has no dependency on any external framework.

---

# Solution Structure

```
src/

    UrlShortener.Api

    UrlShortener.Application

    UrlShortener.Domain

    UrlShortener.Infrastructure

tests/

    UrlShortener.Api.Tests

    UrlShortener.Domain.Tests
```

---

# Implemented Use Cases

### Create Short URL

Creates a new shortened URL.

```
POST /api/shorturls
```

Example Request

```json
{
  "url": "https://www.google.com"
}
```

Example Response

```json
{
  "shortCode": "Ab12Cd",
  "shortUrl": "http://localhost:5000/Ab12Cd"
}
```

---

### Get URL By Code

```
GET /api/shorturls/{code}
```

Returns information about a shortened URL.

---

### Redirect

```
GET /{code}
```

Redirects the client to the original URL.

---

### Disable URL

Disables an existing shortened URL.

---

### URL Expiration

Expired URLs cannot be redirected.

---

# Domain Model

### Entity

- ShortUrl

### Value Objects

- OriginalUrl
- ShortCode

---

# Validation

Input validation is handled using **FluentValidation**.

Examples include:

- Invalid URL format
- Empty URL
- Invalid short code

Validation is executed automatically through MediatR pipeline behaviors.

---

# Exception Handling

The API includes centralized exception handling middleware.

Handled exceptions include:

- ValidationException
- NotFoundException
- DomainException

---

# Testing

The solution contains both unit and integration tests.

### Domain Tests

- Entity behavior
- Value Objects

### Application Tests

- Create URL
- Redirect
- Disable URL
- Expiration
- Validation
- Not Found

### Contract Tests

API contract verification.

Run all tests

```
dotnet test
```

---

# Getting Started

## Clone

```
git clone https://github.com/yourusername/url-shortener.git
```

```
cd url-shortener
```

---

## Configure Database

Update the PostgreSQL connection string inside

```
appsettings.json
```

Example

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=UrlShortener;Username=postgres;Password=postgres"
}
```

---

## Apply Migrations

```
dotnet ef database update
```

---

## Run

```
dotnet run --project src/UrlShortener.Api
```

---

# Project Highlights

✔ Clean Architecture

✔ Domain-Driven Design

✔ CQRS Pattern

✔ MediatR Pipeline Behaviors

✔ Repository Pattern

✔ Value Objects

✔ EF Core

✔ PostgreSQL

✔ FluentValidation

✔ Integration Testing

✔ Unit Testing

---

# Future Improvements

- JWT Authentication
- User Management
- Custom Short Codes
- Click Analytics
- Redis Cache
- API Versioning
- Docker Support
- GitHub Actions CI
- Rate Limiting
- OpenTelemetry
- QR Code Generation

---

# License

This project is licensed under the MIT License.

---

## Author

**Ali Jafari**

Backend Developer

- ASP.NET Core
- C#
- PostgreSQL
- Clean Architecture
- Domain-Driven Design
