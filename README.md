# Enterprise Order System API


A production-ready backend API built with ASP.NET Core using Clean Architecture, featuring secure authentication with JWT, refresh token rotation, and reuse detection.
---

## Features

- ✅ Clean Architecture (Domain, Application, Infrastructure, API)
- 🔐 JWT Authentication
- 🔄 Refresh Token with Rotation
- 💀 Refresh Token Reuse Detection (Session Hijack Protection)
- ⚠️ Global Exception Handling
- 📦 MediatR (CQRS Pattern)
- 🗄️ Entity Framework Core (SQL Server)

---

## Security Highlights

This project implements advanced authentication mechanisms used in real-world systems:

- Short-lived access tokens
- Long-lived refresh tokens
- Token rotation on every refresh
- **Reuse detection to prevent stolen token attacks**
- Automatic session invalidation on suspicious activity

---

##  Architecture

```text
Domain (Core business logic)
Application  Use cases (CQRS, MediatR)
Infrastructure (Database, JWT, external services)
API  Controllers, Middleware
```

## Tech Stack

- ASP.NET Core 8
- Entity Framework Core
- SQL Server
- MediatR
- BCrypt (Password Hashing)

## How to Run
```text
git clone https://github.com/your-username/your-repo.git

cd EnterpriseOrderSystem

dotnet restore
dotnet ef database update
dotnet run
```
## API Endpoints
## Auth
- POST /api/auth/register
- POST /api/auth/login
- POST /api/auth/refresh

## Products
- GET /api/products/public
- GET /api/products (Protected)






## Example Flow
1. Login -> receive access & refresh token
2. Access protected endpoint
3. Refresh token when expired
4. System detects reuse if token is compromised


## Notes
This project focuses on backend architecture and security practices aligned with enterprise-level systems.


## Author

Hendi Dwi Purwanto

ASP.NET Developer | Clean Architecture | Backend Systems
