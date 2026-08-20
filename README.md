# Life Timeline API

Life Timeline API is a .NET 10 ASP.NET Core backend for tracking personal milestones and important life events. It supports user registration and authentication, milestone creation and filtering, Redis-backed caching, and SQL Server persistence.

## Features

- User signup, login, and JWT refresh flow
- Secure milestone CRUD with user scoping
- Search, filtering, and pagination for milestone queries
- Monthly/yearly milestone statistics
- SQL Server persistence with EF Core
- Redis-based cache support for milestone queries and stats
- Swagger/OpenAPI documentation for local development
- Background cleanup for expired refresh tokens

## Tech Stack

- ASP.NET Core 10
- Entity Framework Core
- SQL Server
- Redis
- JWT authentication
- Swagger / Swashbuckle
- BCrypt password hashing

## Project Structure

```text
life-timeline-api/
├── Api/
│   └── Controllers/
│       ├── AuthController.cs
│       └── MilestonesController.cs
├── Application/
│   ├── Common/
│   ├── Dtos/
│   │   ├── Auth/
│   │   └── Milestone/
│   └── Services/
│       ├── Auth/
│       ├── Background/
│       └── Milestones/
├── Domain/
│   └── Entities/
├── Infrastructure/
│   └── Data/
├── Migrations/
├── appsettings.json
├── appsettings.Development.json
├── Program.cs
├── life-timeline-api.csproj
└── README.md
```

## Prerequisites

Before running the project, make sure you have:

- .NET 10 SDK
- SQL Server running locally or in Docker
- Redis running locally or in Docker
- A way to call the API (Swagger, Postman, curl, or frontend app)

## Local Setup

### 1. Install dependencies

```bash
dotnet restore
```

### 2. Start SQL Server

The default development configuration expects SQL Server on `localhost:1433` with the credentials in `appsettings.Development.json`.

Using Docker:

```bash
docker run --name life-timeline-sql \
  -e "ACCEPT_EULA=Y" \
  -e "MSSQL_SA_PASSWORD=Password@0" \
  -p 1433:1433 \
  -d mcr.microsoft.com/mssql/server:2022-latest
```

### 3. Start Redis

```bash
docker run --name life-timeline-redis \
  -p 6379:6379 \
  -d redis:7-alpine
```

### 4. Update application settings

Update the connection strings and JWT key in `appsettings.Development.json` or `appsettings.json` before running the app. The default values are placeholders and should be replaced with real secrets for local development.

Example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=LifeTimelineDb;User Id=sa;Password=Password@0;TrustServerCertificate=True",
    "Redis": "localhost:6379"
  },
  "Jwt": {
    "Key": "YOUR_SUPER_SECRET_KEY_HERE"
  }
}
```

> Use a long, random value for the JWT signing key in production.

### 5. Apply database migrations

```bash
dotnet ef database update
```

### 6. Run the API

```bash
dotnet run
```

For development with auto-reload:

```bash
dotnet watch run
```

The API will start on the ASP.NET default local URL, typically:

```text
https://localhost:5001
http://localhost:5000
```

In development, Swagger is enabled and is usually available at:

```text
https://localhost:5001/swagger
```

## Authentication

The API uses JWT bearer authentication. Protected endpoints require an `Authorization` header with the bearer token returned by the auth endpoints.

### Signup

```http
POST /api/auth/signup
Content-Type: application/json
```

Request body:

```json
{
  "email": "user@example.com",
  "password": "Password123!"
}
```

### Login

```http
POST /api/auth/login
Content-Type: application/json
```

Request body:

```json
{
  "email": "user@example.com",
  "password": "Password123!"
}
```

Response:

```json
{
  "message": "Login successful.",
  "accessToken": "<jwt-token>",
  "refreshToken": "<refresh-token>"
}
```

### Refresh token

```http
POST /api/auth/refresh
Content-Type: application/json
```

Request body:

```json
{
  "refreshToken": "<refresh-token>"
}
```

Response:

```json
{
  "accessToken": "<new-jwt-token>",
  "refreshToken": "<new-refresh-token>"
}
```

## Milestone Endpoints

All milestone routes require authentication.

### Create milestone

```http
POST /api/milestones
Authorization: Bearer <token>
Content-Type: application/json
```

Example body:

```json
{
  "title": "First Job",
  "description": "Started my first professional role.",
  "emoji": "💼",
  "mood": "happy",
  "date": "2026-08-20T00:00:00Z",
  "tags": ["career", "milestone"]
}
```

### Get milestones

```http
GET /api/milestones?page=1&pageSize=20&year=2026&mood=happy&tag=career&search=job
Authorization: Bearer <token>
```

Query parameters:

- `page` - page number, default `1`
- `pageSize` - items per page, default `20`
- `year` - optional year filter
- `mood` - optional mood filter
- `tag` - optional tag filter
- `search` - optional text filter

### Get milestone by id

```http
GET /api/milestones/{id}
Authorization: Bearer <token>
```

### Update milestone

```http
PUT /api/milestones/{id}
Authorization: Bearer <token>
Content-Type: application/json
```

### Delete milestone

```http
DELETE /api/milestones/{id}
Authorization: Bearer <token>
```

### Get milestone stats

```http
GET /api/milestones/stats
Authorization: Bearer <token>
```

Example response:

```json
{
  "total": 42,
  "thisMonth": 3,
  "thisYear": 18
}
```

## Data Model Highlights

The application uses a small domain model centered on milestones and users:

- `User`
  - Id
  - Email
  - PasswordHash
  - RefreshTokens
  - Milestones
- `Milestone`
  - Id
  - Title
  - Description
  - Emoji
  - Mood
  - Date
  - UserId
  - MilestoneTags
- `Tag`
  - Id
  - Name
- `RefreshToken`
  - Id
  - UserId
  - TokenHash
  - CreatedAt
  - ExpiresAt
  - RevokedAt

## Caching Behavior

The service layer uses Redis to cache milestone query results and stats for reuse. Cache keys are tied to the current user and a version marker, which is invalidated when milestone data changes. This helps reduce repeated database reads for common list and stats queries.

## Background Tasks

The application includes a hosted background service that periodically removes expired or revoked refresh tokens from the database. This keeps the auth state clean and helps prevent stale token accumulation.

## Migration Workflow

If you change the schema, generate migrations with:

```bash
dotnet ef migrations add <MigrationName>
```

Then apply them:

```bash
dotnet ef database update
```

## Troubleshooting

### SQL Server connection errors

- Confirm SQL Server is running
- Verify the `DefaultConnection` value in `appsettings.json`
- Check that the database exists or migrations have been applied

### Redis connection errors

- Confirm Redis is running on `localhost:6379`
- Check the `Redis` connection string in configuration

### JWT errors

- Confirm the `Jwt:Key` value is set and long enough
- Ensure the token is passed in the `Authorization` header as `Bearer <token>`

## License

This project currently does not include a formal license file. If you plan to share it publicly, add a license that matches your usage and distribution requirements.

## Notes

This project is structured as a personal-life milestone API and is suitable as a backend for a frontend application or mobile client. The current implementation keeps authentication and authorization simple but effective for a self-hosted service.
