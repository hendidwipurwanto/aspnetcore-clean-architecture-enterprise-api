# Enterprise Order System API

A production-ready backend API built with **ASP.NET Core** using **Clean Architecture**, demonstrating secure authentication with **JWT**, **Refresh Token Rotation**, and **Refresh Token Reuse Detection**.

This project showcases how to design a **secure, scalable, and maintainable backend system** using modern .NET architecture practices.

---

# Features

* Clean Architecture (Domain, Application, Infrastructure, API)
* JWT Authentication
* Refresh Token Rotation
* Refresh Token Reuse Detection (Session Hijack Protection)
* Global Exception Handling Middleware
* CQRS Pattern using MediatR
* Entity Framework Core (SQL Server)
* Unit Testing (Application Layer)
* Domain Logic Testing

---

# Tech Stack

* ASP.NET Core 8 Web API
* Entity Framework Core
* MediatR (CQRS)
* SQL Server
* xUnit
* Moq
* FluentAssertions
* MockQueryable.Moq

---

# Architecture

This project follows **Clean Architecture principles**, separating responsibilities across layers.

```
                 ┌──────────────────────────┐
                 │        API Layer          │
                 │ Controllers / Middleware  │
                 └─────────────▲────────────┘
                               │
                 ┌─────────────┴────────────┐
                 │    Application Layer      │
                 │ Commands / Queries        │
                 │ Handlers / DTOs           │
                 │ Interfaces (CQRS)         │
                 └─────────────▲────────────┘
                               │
             ┌─────────────────┴─────────────────┐
             │           Domain Layer            │
             │    Entities & Business Rules     │
             └─────────────────▲─────────────────┘
                               │
                 ┌─────────────┴────────────┐
                 │   Infrastructure Layer    │
                 │ Database / JWT / EF Core  │
                 └──────────────────────────┘
```

---

# Authentication Flow

```
User
 │
 │ Login Request
 ▼
API Controller
 │
 │ Validate Credentials
 ▼
Generate Access Token (JWT)
 │
Generate Refresh Token
 │
Store Refresh Token in Database
 │
Return Tokens to Client
```

---

# Refresh Token Rotation

Each refresh token usage generates a new refresh token.

```
Client sends refresh token
        │
        ▼
Validate refresh token
        │
Mark old token as USED
        │
Revoke old token
        │
Generate new refresh token
        │
Store new refresh token
        │
Return new tokens
```

---

# Refresh Token Reuse Detection

If an attacker tries to reuse a refresh token:

```
Attacker reuses refresh token
        │
        ▼
System detects reused token
        │
All refresh tokens revoked
        │
User session invalidated
        │
User must login again
```

This protects against **session hijacking attacks**.

---

# Global Exception Handling

The API uses middleware to standardize error responses.

Example:

```json
{
  "success": false,
  "message": "Refresh token reuse detected. Please login again.",
  "errors": null
}
```

---

# API Endpoints

## Authentication

| Method | Endpoint           |
| ------ | ------------------ |
| POST   | /api/auth/register |
| POST   | /api/auth/login    |
| POST   | /api/auth/refresh  |

---

## Protected Example Endpoint

| Method | Endpoint         |
| ------ | ---------------- |
| GET    | /api/products    |
| GET    | /api/products/me |

Requires header:

```
Authorization: Bearer {access_token}
```

---

# Example API Flow

### Register

```
POST /api/auth/register
```

```json
{
  "email": "admin@mail.com",
  "password": "123456"
}
```

---

### Login

```
POST /api/auth/login
```

Response

```json
{
  "token": "jwt-access-token",
  "refreshToken": "refresh-token",
  "email": "admin@mail.com",
  "role": "User"
}
```

---

### Refresh Token

```
POST /api/auth/refresh
```

```json
{
  "refreshToken": "your-refresh-token"
}
```

---

# Testing

Unit tests are implemented for both **Application Layer** and **Domain Layer**.

### Application Tests

* Refresh token reuse detection
* Valid refresh token flow
* Invalid refresh token
* Expired refresh token

### Domain Tests

* Refresh token revoke behavior
* Refresh token usage tracking
* Idempotent operations

Run tests:

```
dotnet test
```

---

# Running the Project

### 1 Clone Repository

```
git clone https://github.com/hendidwipurwanto/aspnetcore-clean-architecture-enterprise-api.git
```

---

### 2 Configure Database

Edit `appsettings.json`

```
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=EnterpriseOrderDb;Trusted_Connection=True"
}
```

---

### 3 Run Migration

```
dotnet ef database update
```

---

### 4 Run API

```
dotnet run
```

Swagger UI

```
https://localhost:7098/swagger
```

---

# Security Highlights

This project implements production-level authentication practices:

* Short-lived access tokens
* Long-lived refresh tokens
* Refresh token rotation
* Refresh token reuse detection
* Global exception handling

---

# Future Improvements

Possible enhancements:

* Role Based Authorization
* Redis Distributed Cache
* Multi-Tenant Architecture
* API Rate Limiting
* Background Jobs
* OpenTelemetry Logging

---

# Purpose

This project demonstrates how to build a **secure backend system with modern .NET architecture practices**, including:

* Clean Architecture
* CQRS Pattern
* Advanced Authentication Flow
* Production-grade API Security

---

# Author

Hendi Dwi Purwanto

Senior .NET Backend Developer
Specializing in ASP.NET Core, Clean Architecture, and Secure API Design.

GitHub
https://github.com/hendidwipurwanto

LinkedIn
https://linkedin.com/in/hendidwipurwanto
