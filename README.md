# AuthService

A comprehensive, production-ready authentication service built with .NET 9.0, providing secure user registration, login, JWT-based authentication, device management, and both REST API and GraphQL endpoints.

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Architecture](#architecture)
- [Technology Stack](#technology-stack)
- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Configuration](#configuration)
- [Usage](#usage)
- [Database Setup](#database-setup)
- [API Documentation](#api-documentation)
- [Authentication](#authentication)
- [Project Structure](#project-structure)
- [Testing](#testing)
- [Docker Support](#docker-support)
- [Development](#development)
- [Security Features](#security-features)
- [License](#license)

## Overview

AuthService is a clean architecture-based authentication microservice that provides:

- **User Registration & Login**: Secure user account creation and authentication
- **JWT Token Management**: Token-based authentication with device binding
- **Device Tracking**: Multi-device support with device information tracking
- **REST API**: Traditional REST endpoints for authentication operations
- **GraphQL API**: Modern GraphQL endpoint for flexible querying
- **Password Security**: PBKDF2-SHA256 password hashing with 100,000 iterations
- **Request Validation**: FluentValidation for input validation
- **Database Transactions**: ACID-compliant transaction management
- **Request Logging**: Comprehensive request/response logging middleware

## Features

### Core Features

- ✅ User registration with email and username validation
- ✅ User login with username or email
- ✅ JWT token generation with configurable expiration
- ✅ Device-based token management
- ✅ User profile retrieval
- ✅ User profile update
- ✅ Account deletion
- ✅ Password hashing using PBKDF2-SHA256
- ✅ Device information tracking
- ✅ Token validation with device verification
- ✅ Automatic device expiration management

### API Features

- ✅ RESTful API endpoints
- ✅ GraphQL API with queries and mutations
- ✅ Request/response validation
- ✅ CORS support
- ✅ Comprehensive error handling
- ✅ Request logging middleware

### Security Features

- ✅ Secure password hashing (PBKDF2-SHA256, 100,000 iterations)
- ✅ JWT token authentication
- ✅ Device-based token binding
- ✅ Token expiration management
- ✅ Input validation
- ✅ SQL injection protection (EF Core)
- ✅ Timing attack prevention

## Architecture

The project follows **Clean Architecture** principles with clear separation of concerns across multiple layers:

### Layer Structure

```
AuthService/
├── AuthService.API/              # Presentation Layer
│   ├── Configurations/           # Service configurations
│   ├── Controllers/              # REST API controllers
│   ├── DTOs/                     # Data Transfer Objects
│   ├── Extensions/               # Extension methods
│   ├── GraphQL/                  # GraphQL schema, queries, mutations
│   ├── Middleware/               # Custom middleware (JWT, logging, validation)
│   ├── Properties/               # Application properties
│   └── Validators/               # FluentValidation validators
│
├── AuthService.Application/      # Application Layer
│   ├── Configurations/           # Application configurations
│   ├── DependencyInjection/      # DI registration
│   ├── Exceptions/               # Custom exceptions
│   ├── Helpers/                  # Utility classes (PasswordHelper)
│   ├── Interfaces/               # Service interfaces
│   └── Services/                 # Business logic services
│
├── AuthService.Infrastructure/   # Infrastructure Layer
│   ├── Configurations/           # Infrastructure configurations
│   ├── Data/                     # DbContext
│   ├── DependencyInjection/      # DI registration
│   ├── Entities/                 # Domain entities
│   ├── Migrations/               # EF Core migrations
│   └── Repositories/             # Data access repositories
│
└── AuthService.Tests/            # Test Projects
    ├── Integration/              # Integration tests
    ├── Mocks/                    # Mock implementations
    ├── TestUtils/                # Test utilities
    └── Unit/                     # Unit tests
```

### Design Patterns

The project implements several design patterns to ensure clean, maintainable, and scalable code:

#### Structural Patterns

- **Repository Pattern**: Abstracted data access layer (`IUserRepository`, `IUserDeviceRepository`) that encapsulates database operations and provides a clean interface for data access
- **Unit of Work Pattern**: Transaction management (`IUnitOfWork`, `UnitOfWork`) that ensures data consistency by managing database transactions
- **DTO Pattern (Data Transfer Object)**: Data transfer objects (`LoginDto`, `RegisterDto`, `UpdateUserDto`) for transferring data between layers without exposing domain entities
- **Facade Pattern**: `ServiceConfiguration` acts as a facade that simplifies the registration of all application and infrastructure services
- **Adapter Pattern**: GraphQL types adapt domain entities for GraphQL API consumption

#### Behavioral Patterns

- **Middleware Pattern**: Cross-cutting concerns (logging, authentication, validation) handled through middleware pipeline (`RequestLoggingMiddleware`, `JwtAuthenticationMiddleware`, `ValidationMiddleware`)
- **Chain of Responsibility Pattern**: Request processing through middleware pipeline where each middleware can handle or pass the request to the next handler
- **Strategy Pattern**: Validation strategies implemented through FluentValidation validators (`LoginDtoValidator`, `RegisterDtoValidator`, `UpdateUserDtoValidator`)
- **Template Method Pattern**: FluentValidation's `AbstractValidator<T>` provides template methods for validation

#### Creational Patterns

- **Factory Pattern**: `CustomWebApplicationFactory` creates test web application instances with customized configuration for integration testing
- **Builder Pattern**: ASP.NET Core pipeline building (`builder.Services`, `app.Use...`) and FluentValidation rule building
- **Dependency Injection (DI) Pattern**: Constructor injection and service registration through `IServiceCollection` for loose coupling between layers

#### Configuration Patterns

- **Extension Method Pattern**: Extension methods for clean service registration (`AddJwtAuthentication()`, `AddCorsPolicy()`, `UseRequestLogging()`, etc.)
- **Options Pattern**: Configuration management using `IOptions<JwtConfiguration>` for strongly-typed configuration access
- **Fluent Interface Pattern**: Configuration methods that return the same type to allow method chaining

#### Architectural Patterns

- **Service Layer Pattern**: Business logic encapsulated in services (`UserService`, `TokenService`) that orchestrate domain operations
- **Clean Architecture**: Separation of concerns across multiple layers (API, Application, Infrastructure, Tests)
- **Dependency Inversion Principle**: High-level modules depend on abstractions (interfaces) rather than concrete implementations
- **Interface Segregation Principle**: Small, focused interfaces (`IUserService`, `ITokenService`, `IUserRepository`) that clients depend only on methods they use
- **Single Responsibility Principle**: Each class has a single, well-defined responsibility

## Technology Stack

### Core Framework
- **.NET 9.0**: Latest .NET framework
- **ASP.NET Core**: Web API framework
- **C# 12**: Modern C# language features

### Database
- **PostgreSQL**: Relational database
- **Entity Framework Core 9.0**: ORM framework
- **Npgsql.EntityFrameworkCore.PostgreSQL**: PostgreSQL provider

### Authentication & Security
- **JWT Bearer Authentication**: Token-based authentication
- **Microsoft.IdentityModel.Tokens**: JWT token handling
- **System.IdentityModel.Tokens.Jwt**: JWT creation and validation
- **PBKDF2-SHA256**: Password hashing algorithm

### API
- **REST API**: Traditional REST endpoints
- **GraphQL**: HotChocolate GraphQL server
  - HotChocolate.AspNetCore (15.1.11)
  - HotChocolate.Data (15.1.11)
  - HotChocolate.Data.EntityFramework (15.1.11)
  - HotChocolate.Types (15.1.11)
  - HotChocolate.AspNetCore.Authorization (15.1.11)

### Validation
- **FluentValidation** (12.1.0): Input validation
- **FluentValidation.AspNetCore** (11.3.1): ASP.NET Core integration

### Testing
- **xUnit**: Testing framework
- **Moq**: Mocking framework
- **FluentAssertions**: Assertion library
- **Microsoft.AspNetCore.Mvc.Testing**: Integration testing
- **Microsoft.EntityFrameworkCore.InMemory**: In-memory database for testing
- **Testcontainers**: Containerized testing

### DevOps
- **Docker**: Containerization support
- **Linux**: Default container target OS

## Prerequisites

Before you begin, ensure you have the following installed:

### Required

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) or higher
- [PostgreSQL](https://www.postgresql.org/download/) (version 12 or higher)
- [Entity Framework Core Tools](https://learn.microsoft.com/en-us/ef/core/cli/dotnet) for running migrations
- [Git](https://git-scm.com/downloads)

### Optional

- [Docker](https://www.docker.com/get-started) (for containerized deployment)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/) (for development)
- [Postman](https://www.postman.com/) or [Insomnia](https://insomnia.rest/) (for API testing)

### Install Entity Framework Core Tools

If you haven't installed EF Core Tools globally, run:

```bash
dotnet tool install --global dotnet-ef
```

To update EF Core Tools:

```bash
dotnet tool update --global dotnet-ef
```

Verify installation:

```bash
dotnet ef --version
```

## Installation

### 1. Clone the Repository

```bash
git clone <repository-url>
cd AuthService
```

### 2. Restore Dependencies

```bash
dotnet restore
```

This will restore all NuGet packages for all projects in the solution.

### 3. Build the Solution

```bash
dotnet build
```

Or build in Release mode:

```bash
dotnet build --configuration Release
```

### 4. Set Up PostgreSQL Database

#### Create Database

**Option 1: Using psql Command Line**

```bash
# Connect to PostgreSQL
psql -U postgres

# Create database
CREATE DATABASE AuthServiceDB;

# Exit psql
\q
```

**Option 2: Using createdb Command**

```bash
createdb -U postgres AuthServiceDB
```

**Option 3: Using pgAdmin**

1. Open pgAdmin
2. Connect to your PostgreSQL server
3. Right-click on "Databases" → "Create" → "Database"
4. Enter database name: `AuthServiceDB`
5. Click "Save"

#### Configure Database Connection

Update the connection string in `appsettings.json` or `appsettings.Development.json`:

**For `appsettings.Development.json`:**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=AuthServiceDB;Username=postgres;Password=your_password"
  }
}
```

**For `appsettings.json` (Production):**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=AuthServiceDB;Username=postgres;Password=your_password"
  }
}
```

**Important**: 
- Replace `your_password` with your actual PostgreSQL password
- Replace `localhost` with your database server address if remote
- Replace `5432` with your PostgreSQL port if different
- For production, use environment variables or secure configuration stores

### 5. Configure JWT Settings

Update JWT configuration in `appsettings.Development.json`:

```json
{
  "Jwt": {
    "SecretKey": "your-strong-secret-key-minimum-32-characters-long",
    "Issuer": "AuthService",
    "Audience": "AuthServiceAPI",
    "ExpiryMonths": 3
  }
}
```

**Security Note**: For production, use a strong, randomly generated secret key (minimum 32 characters). Consider using User Secrets for development:

```bash
cd AuthService.API
dotnet user-secrets set "Jwt:SecretKey" "your-strong-secret-key"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=AuthServiceDB;Username=postgres;Password=your_password"
```

### 6. Set Startup Project

**Important**: The startup project must be set to `AuthService.API`.

#### Using Visual Studio

1. Right-click on `AuthService.API` project in Solution Explorer
2. Select "Set as Startup Project"

#### Using Command Line

The startup project is automatically determined when using `dotnet run` from the API directory, or you can specify it:

```bash
dotnet run --project AuthService.API
```

#### Using Visual Studio Code

1. Open the workspace
2. Set `AuthService.API` as the startup project in `.vscode/launch.json` if configuring debug settings

### 7. Install Entity Framework Core Tools

**Important**: Entity Framework Core Tools are required for running migrations.

#### Option 1: Using Command Line (CMD/PowerShell/Bash)

```bash
# Install EF Core Tools globally
dotnet tool install --global dotnet-ef

# Verify installation
dotnet ef --version
```

**Update EF Core Tools** (if already installed):

```bash
dotnet tool update --global dotnet-ef
```

#### Option 2: Using NuGet Package Manager Console (Visual Studio)

1. Open Visual Studio
2. Go to **Tools** → **NuGet Package Manager** → **Package Manager Console**
3. Run the following command:

```powershell
dotnet tool install --global dotnet-ef
```

**Note**: Even in Package Manager Console, you need to use `dotnet tool` commands. The Package Manager Console is primarily for NuGet package management, not .NET tools.

### 8. Run Database Migrations

**Important**: The startup project must be set to `AuthService.API` before running migrations.

#### Option 1: Using Command Line (CMD/PowerShell/Bash)

**From Solution Root Directory:**

```bash
# Update database (applies all pending migrations)
dotnet ef database update --project AuthService.Infrastructure --startup-project AuthService.API
```

**From Infrastructure Directory:**

```bash
cd AuthService.Infrastructure
dotnet ef database update --startup-project ../AuthService.API
```

**Create a New Migration:**

```bash
# From solution root
dotnet ef migrations add MigrationName --project AuthService.Infrastructure --startup-project AuthService.API

# From Infrastructure directory
cd AuthService.Infrastructure
dotnet ef migrations add MigrationName --startup-project ../AuthService.API
```

**Remove Last Migration (if not applied to database):**

```bash
dotnet ef migrations remove --project AuthService.Infrastructure --startup-project AuthService.API
```

#### Option 2: Using NuGet Package Manager Console (Visual Studio)

1. Open Visual Studio
2. Go to **Tools** → **NuGet Package Manager** → **Package Manager Console**
3. Make sure **Default project** is set to `AuthService.Infrastructure` (or select it from dropdown)
4. Run the following commands:

**Update Database (apply migrations):**

```powershell
Update-Database -Project AuthService.Infrastructure -StartupProject AuthService.API
```

**Create a New Migration:**

```powershell
Add-Migration MigrationName -Project AuthService.Infrastructure -StartupProject AuthService.API
```

**Remove Last Migration:**

```powershell
Remove-Migration -Project AuthService.Infrastructure -StartupProject AuthService.API
```

**List All Migrations:**

```powershell
Get-Migration -Project AuthService.Infrastructure -StartupProject AuthService.API
```

**Note**: In Package Manager Console, you can use PowerShell cmdlets. Make sure the `Microsoft.EntityFrameworkCore.Tools` package is installed in your project.

#### Migration Commands Reference

| Action | Command Line | Package Manager Console |
|--------|-------------|------------------------|
| Update Database | `dotnet ef database update --project AuthService.Infrastructure --startup-project AuthService.API` | `Update-Database -Project AuthService.Infrastructure -StartupProject AuthService.API` |
| Add Migration | `dotnet ef migrations add MigrationName --project AuthService.Infrastructure --startup-project AuthService.API` | `Add-Migration MigrationName -Project AuthService.Infrastructure -StartupProject AuthService.API` |
| Remove Migration | `dotnet ef migrations remove --project AuthService.Infrastructure --startup-project AuthService.API` | `Remove-Migration -Project AuthService.Infrastructure -StartupProject AuthService.API` |
| List Migrations | `dotnet ef migrations list --project AuthService.Infrastructure --startup-project AuthService.API` | `Get-Migration -Project AuthService.Infrastructure -StartupProject AuthService.API` |

This will create the database schema (Users and UserDevices tables) based on your Entity Framework models.

### 9. Verify Database Migration

Check that the tables were created:

```sql
\c AuthServiceDB
\dt
```

You should see:
- `Users` table
- `UserDevices` table
- `__EFMigrationsHistory` table

### 10. Run the Application

#### Option 1: Using dotnet CLI

```bash
cd AuthService.API
dotnet run
```

#### Option 2: From Solution Root

```bash
dotnet run --project AuthService.API
```

#### Option 3: Using Visual Studio

1. Set `AuthService.API` as startup project
2. Press `F5` or click "Start" button

### 11. Verify Application is Running

The API will be available at:
- **REST API**: `http://localhost:5005` or `https://localhost:7007`
- **GraphQL**: `http://localhost:5005/graphql` or `https://localhost:7007/graphql`
- **GraphQL Playground**: `http://localhost:5005/graphql` (for interactive GraphQL queries)

Test the health of the API:

```bash
# Test REST API
curl http://localhost:5005/api/user/profile

# Test GraphQL (should return schema)
curl http://localhost:5005/graphql
```

### Troubleshooting

#### Database Connection Issues

- Verify PostgreSQL is running: `pg_isready` or `sudo systemctl status postgresql`
- Check connection string format and credentials
- Ensure database exists: `psql -U postgres -l`
- Check PostgreSQL is listening on the correct port: `netstat -an | grep 5432`

#### Migration Issues

- Ensure Entity Framework Tools are installed: `dotnet ef --version`
- Verify startup project is set to `AuthService.API`
- Check database connection string is correct
- Ensure database exists before running migrations

#### Port Already in Use

If port 5005 or 7007 is already in use, you can change it in `launchSettings.json`:

```json
{
  "applicationUrl": "http://localhost:5006"
}
```

Or set environment variable:

```bash
export ASPNETCORE_URLS="http://localhost:5006"
```

## Configuration

### Application Settings

The application uses `appsettings.json` for configuration. Key settings include:

#### JWT Configuration

```json
{
  "Jwt": {
    "SecretKey": "your-secret-key-here-minimum-32-characters",
    "Issuer": "AuthService",
    "Audience": "AuthServiceAPI",
    "ExpiryMonths": 3
  }
}
```


**Important**: In production, use a strong, randomly generated secret key (minimum 32 characters). Store sensitive configuration in:
- User Secrets (development)
- Environment Variables (production)
- Azure Key Vault / AWS Secrets Manager (cloud)

#### Database Configuration

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=AuthServiceDB;Username=postgres;Password=your_password"
  }
}
```

#### Logging Configuration

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```


### Environment-Specific Configuration

- **Development**: `appsettings.Development.json`
- **Production**: Use environment variables or secure configuration stores

### User Secrets (Development)

For local development, use User Secrets to store sensitive configuration:

```bash
dotnet user-secrets set "Jwt:SecretKey" "your-secret-key"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "your-connection-string"
```

## Usage

This section explains how to use the AuthService API after installation and configuration.

### Quick Start

1. **Ensure the application is running** (see [Installation](#installation) section)
2. **Register a new user** to get an authentication token
3. **Use the token** to access protected endpoints
4. **Manage your account** through the available endpoints

### Using the REST API

#### 1. Register a New User

**Endpoint**: `POST /api/user/register`

**Request**:
```bash
curl -X POST http://localhost:5005/api/user/register \
  -H "Content-Type: application/json" \
  -d '{
    "username": "johndoe",
    "email": "john.doe@example.com",
    "password": "SecurePass123",
    "deviceInfo": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36"
  }'
```

**Response**:
```json
{
  "success": true,
  "message": "User successfully registered.",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

**Save the token** - you'll need it for authenticated requests.

#### 2. Login

**Endpoint**: `POST /api/user/login`

**Request**:
```bash
curl -X POST http://localhost:5005/api/user/login \
  -H "Content-Type: application/json" \
  -d '{
    "usernameOrEmail": "johndoe",
    "password": "SecurePass123",
    "deviceInfo": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36"
  }'
```

**Response**:
```json
{
  "success": true,
  "message": "User successfully logged in.",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

#### 3. Get User Profile (Authenticated)

**Endpoint**: `GET /api/user/profile`

**Request**:
```bash
curl -X GET http://localhost:5005/api/user/profile \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

**Response**:
```json
{
  "success": true,
  "message": "Profile retrieved successfully.",
  "user": {
    "id": "123e4567-e89b-12d3-a456-426614174000",
    "username": "johndoe",
    "email": "john.doe@example.com",
    "createdAt": "2024-01-01T00:00:00Z"
  }
}
```

#### 4. Update User Profile (Authenticated)

**Endpoint**: `PUT /api/user/update`

**Request**:
```bash
curl -X PUT http://localhost:5005/api/user/update \
  -H "Authorization: Bearer YOUR_TOKEN_HERE" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "newusername",
    "email": "newemail@example.com",
    "password": "NewSecurePass123"
  }'
```

**Note**: All fields are optional. Only include the fields you want to update.

#### 5. Delete Account (Authenticated)

**Endpoint**: `DELETE /api/user/delete`

**Request**:
```bash
curl -X DELETE http://localhost:5005/api/user/delete \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

### Using the GraphQL API

#### Access GraphQL Playground

Navigate to `http://localhost:5005/graphql` in your browser to access the GraphQL Playground for interactive queries.

#### Register User via GraphQL

**Mutation**:
```graphql
mutation {
  registerUser(
    input: {
      username: "johndoe"
      email: "john.doe@example.com"
      password: "SecurePass123"
      deviceInfo: "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36"
    }
  ) {
    token
  }
}
```

**Response**:
```json
{
  "data": {
    "registerUser": {
      "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
    }
  }
}
```

#### Query Users via GraphQL

**Query** (Get all users):
```graphql
query {
  getUsers {
    id
    username
    email
    createdAt
  }
}
```

**Query** (Get user by ID):
```graphql
query {
  getUserById(id: "123e4567-e89b-12d3-a456-426614174000") {
    id
    username
    email
    createdAt
  }
}
```

**Note**: For authenticated GraphQL queries, include the token in the request headers:
```json
{
  "Authorization": "Bearer YOUR_TOKEN_HERE"
}
```

### Using with Postman

1. **Import Collection**: Create a new Postman collection
2. **Set Base URL**: `http://localhost:5005`
3. **Register/Login**: Get a token from register or login endpoint
4. **Set Environment Variable**: Save the token as an environment variable (e.g., `auth_token`)
5. **Use Token**: Add `Authorization: Bearer {{auth_token}}` header to authenticated requests

### Using with cURL

#### Save Token to Variable (Bash)

```bash
# Register and save token
TOKEN=$(curl -X POST http://localhost:5005/api/user/register \
  -H "Content-Type: application/json" \
  -d '{
    "username": "johndoe",
    "email": "john.doe@example.com",
    "password": "SecurePass123",
    "deviceInfo": "Desktop"
  }' | jq -r '.token')

# Use token for authenticated requests
curl -X GET http://localhost:5005/api/user/profile \
  -H "Authorization: Bearer $TOKEN"
```

#### Save Token to Variable (PowerShell)

```powershell
# Register and save token
$response = Invoke-RestMethod -Uri "http://localhost:5005/api/user/register" `
  -Method Post `
  -ContentType "application/json" `
  -Body '{
    "username": "johndoe",
    "email": "john.doe@example.com",
    "password": "SecurePass123",
    "deviceInfo": "Desktop"
  }'
$token = $response.token

# Use token for authenticated requests
Invoke-RestMethod -Uri "http://localhost:5005/api/user/profile" `
  -Method Get `
  -Headers @{ "Authorization" = "Bearer $token" }
```

### Using with JavaScript/TypeScript

#### Fetch API Example

```javascript
// Register user
const registerResponse = await fetch('http://localhost:5005/api/user/register', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
  },
  body: JSON.stringify({
    username: 'johndoe',
    email: 'john.doe@example.com',
    password: 'SecurePass123',
    deviceInfo: navigator.userAgent
  })
});

const { token } = await registerResponse.json();

// Get user profile
const profileResponse = await fetch('http://localhost:5005/api/user/profile', {
  method: 'GET',
  headers: {
    'Authorization': `Bearer ${token}`
  }
});

const profile = await profileResponse.json();
console.log(profile);
```

#### Axios Example

```javascript
import axios from 'axios';

const api = axios.create({
  baseURL: 'http://localhost:5005/api',
});

// Register user
const { data: { token } } = await api.post('/user/register', {
  username: 'johndoe',
  email: 'john.doe@example.com',
  password: 'SecurePass123',
  deviceInfo: navigator.userAgent
});

// Set default authorization header
api.defaults.headers.common['Authorization'] = `Bearer ${token}`;

// Get user profile
const { data: profile } = await api.get('/user/profile');
console.log(profile);
```

### Using with C# Client

```csharp
using System.Net.Http.Json;

var httpClient = new HttpClient();
httpClient.BaseAddress = new Uri("http://localhost:5005/api");

// Register user
var registerResponse = await httpClient.PostAsJsonAsync("/user/register", new
{
    username = "johndoe",
    email = "john.doe@example.com",
    password = "SecurePass123",
    deviceInfo = "Desktop"
});

var registerResult = await registerResponse.Content.ReadFromJsonAsync<RegisterResponse>();
var token = registerResult.Token;

// Set authorization header
httpClient.DefaultRequestHeaders.Authorization = 
    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

// Get user profile
var profileResponse = await httpClient.GetAsync("/user/profile");
var profile = await profileResponse.Content.ReadFromJsonAsync<ProfileResponse>();
```

### Important Notes

1. **Token Storage**: Store tokens securely. Never expose tokens in client-side code that can be accessed by others.
2. **Token Expiration**: Tokens expire after the duration specified in `Jwt:ExpiryMonths` (default: 3 months). Implement token refresh logic if needed.
3. **Device Information**: Provide meaningful device information for device tracking and security.
4. **Password Requirements**: 
   - Minimum 8 characters
   - At least one uppercase letter
   - At least one number
5. **Error Handling**: Always handle errors and check response status codes.
6. **HTTPS in Production**: Always use HTTPS in production environments.

## Database Setup

### Database Schema

The application uses two main tables:

#### Users Table

| Column | Type | Description |
|--------|------|-------------|
| Id | Guid (UUID) | Primary key |
| Username | string | Unique username |
| Email | string | Unique email address |
| PasswordHash | string | Hashed password (PBKDF2-SHA256) |
| CreatedAt | DateTime | Account creation timestamp |

#### UserDevices Table

| Column | Type | Description |
|--------|------|-------------|
| Id | Guid (UUID) | Primary key |
| UserId | Guid (UUID) | Foreign key to Users |
| DeviceId | string | Unique device identifier |
| DeviceInfo | string | Device information (e.g., browser, OS) |
| CreatedAt | DateTime | Device registration timestamp |
| LastUsedAt | DateTime | Last token usage timestamp |
| ExpiresAt | DateTime | Token expiration timestamp |

### Creating Migrations

**Using Command Line (CMD/PowerShell/Bash):**

```bash
# From solution root
dotnet ef migrations add MigrationName --project AuthService.Infrastructure --startup-project AuthService.API

# From Infrastructure directory
cd AuthService.Infrastructure
dotnet ef migrations add MigrationName --startup-project ../AuthService.API
```

**Using NuGet Package Manager Console (Visual Studio):**

1. Open **Package Manager Console**
2. Set **Default project** to `AuthService.Infrastructure`
3. Run:

```powershell
Add-Migration MigrationName -Project AuthService.Infrastructure -StartupProject AuthService.API
```

**Example:**
```powershell
Add-Migration AddUserDeviceTable -Project AuthService.Infrastructure -StartupProject AuthService.API
```

### Applying Migrations

**Using Command Line:**

```bash
# From solution root
dotnet ef database update --project AuthService.Infrastructure --startup-project AuthService.API

# From Infrastructure directory
cd AuthService.Infrastructure
dotnet ef database update --startup-project ../AuthService.API
```

**Using NuGet Package Manager Console:**

```powershell
Update-Database -Project AuthService.Infrastructure -StartupProject AuthService.API
```

This command applies all pending migrations to the database.

### Rolling Back Migrations

**Using Command Line:**

```bash
# Rollback to a specific migration
dotnet ef database update PreviousMigrationName --project AuthService.Infrastructure --startup-project AuthService.API

# Rollback all migrations (drop database)
dotnet ef database drop --project AuthService.Infrastructure --startup-project AuthService.API
```

**Using NuGet Package Manager Console:**

```powershell
# Rollback to a specific migration
Update-Database PreviousMigrationName -Project AuthService.Infrastructure -StartupProject AuthService.API

# Rollback all migrations (drop database)
Update-Database 0 -Project AuthService.Infrastructure -StartupProject AuthService.API
```

**Warning**: Rolling back migrations will lose data. Always backup your database before rolling back.

### Listing Migrations

**Using Command Line:**

```bash
dotnet ef migrations list --project AuthService.Infrastructure --startup-project AuthService.API
```

**Using NuGet Package Manager Console:**

```powershell
Get-Migration -Project AuthService.Infrastructure -StartupProject AuthService.API
```

### Removing the Last Migration

**Using Command Line:**

```bash
dotnet ef migrations remove --project AuthService.Infrastructure --startup-project AuthService.API
```

**Using NuGet Package Manager Console:**

```powershell
Remove-Migration -Project AuthService.Infrastructure -StartupProject AuthService.API
```

**Note**: You can only remove the last migration if it hasn't been applied to the database yet. If the migration has been applied, you must rollback first.

## API Documentation

### REST API Endpoints

#### Base URL

```
http://localhost:5005/api
```

#### 1. Register User

**Endpoint**: `POST /api/user/register`

**Authentication**: Not required

**Request Body**:
```json
{
  "username": "johndoe",
  "email": "john.doe@example.com",
  "password": "SecurePass123",
  "deviceInfo": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36"
}
```

**Response** (201 Created):
```json
{
  "success": true,
  "message": "User successfully registered.",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

**Validation Rules**:
- Username: 3-20 characters, required
- Email: Valid email format, required
- Password: Minimum 8 characters, at least one uppercase letter, at least one number
- DeviceInfo: Required

#### 2. Login User

**Endpoint**: `POST /api/user/login`

**Authentication**: Not required

**Request Body**:
```json
{
  "usernameOrEmail": "johndoe",
  "password": "SecurePass123",
  "deviceInfo": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36"
}
```

**Response** (200 OK):
```json
{
  "success": true,
  "message": "User successfully logged in.",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

**Error Response** (401 Unauthorized):
```json
{
  "success": false,
  "message": "Invalid credentials."
}
```

#### 3. Get User Profile

**Endpoint**: `GET /api/user/profile`

**Authentication**: Required (JWT Bearer token)

**Headers**:
```
Authorization: Bearer <token>
```

**Response** (200 OK):
```json
{
  "success": true,
  "message": "Profile retrieved successfully.",
  "user": {
    "id": "123e4567-e89b-12d3-a456-426614174000",
    "username": "johndoe",
    "email": "john.doe@example.com",
    "createdAt": "2024-01-01T00:00:00Z"
  }
}
```

#### 4. Update User Profile

**Endpoint**: `PUT /api/user/update`

**Authentication**: Required (JWT Bearer token)

**Request Body** (all fields optional):
```json
{
  "username": "newusername",
  "email": "newemail@example.com",
  "password": "NewSecurePass123"
}
```

**Response** (200 OK):
```json
{
  "success": true,
  "message": "Profile updated successfully."
}
```

**Validation Rules**:
- Username: 3-20 characters (if provided)
- Email: Valid email format (if provided)
- Password: Minimum 8 characters, at least one uppercase letter, at least one number (if provided)

#### 5. Delete User Account

**Endpoint**: `DELETE /api/user/delete`

**Authentication**: Required (JWT Bearer token)

**Response** (200 OK):
```json
{
  "success": true,
  "message": "Account deleted successfully."
}
```

### GraphQL API

#### Endpoint

```
http://localhost:5005/graphql
```

#### GraphQL Playground

Access the GraphQL playground at: `http://localhost:5005/graphql` or `https://localhost:7007/graphql`

#### Queries

##### Get All Users

```graphql
query {
  getUsers {
    id
    username
    email
    createdAt
  }
}
```

##### Get User by ID

```graphql
query {
  getUserById(id: "123e4567-e89b-12d3-a456-426614174000") {
    id
    username
    email
    createdAt
  }
}
```

#### Mutations

##### Register User

```graphql
mutation {
  registerUser(
    input: {
      username: "johndoe"
      email: "john.doe@example.com"
      password: "SecurePass123"
      deviceInfo: "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36"
    }
  ) {
    token
  }
}
```

#### GraphQL Authorization

GraphQL queries support field-level authorization:

- **Email Field**: The `email` field in the `User` type requires authentication. Unauthenticated requests will receive "You must authenticate to view!" instead of the actual email.
- **Password Field**: The `passwordHash` field is always hidden from GraphQL responses for security.
- **Other Fields**: `id`, `username`, and `createdAt` are publicly accessible.

To access protected fields, include the JWT token in the request headers:

```graphql
# Headers
{
  "Authorization": "Bearer <your-token>"
}
```

### Error Responses

All endpoints return standardized error responses:

```json
{
  "success": false,
  "message": "Error message description"
}
```

Common HTTP status codes:
- `200 OK`: Success
- `201 Created`: Resource created successfully
- `400 Bad Request`: Validation error or invalid input
- `401 Unauthorized`: Authentication required or invalid token
- `404 Not Found`: Resource not found
- `500 Internal Server Error`: Server error

## Authentication

### JWT Token Structure

The JWT token contains the following claims:

- `sub`: User ID (subject)
- `deviceId`: Device identifier
- `jti`: JWT ID (unique token identifier)
- `exp`: Expiration timestamp
- `iss`: Issuer (from configuration)
- `aud`: Audience (from configuration)

### Token Generation

Tokens are generated when:
1. User registers a new account
2. User logs in successfully

Tokens are valid for the duration specified in `Jwt:ExpiryMonths` (default: 3 months).

### Token Validation

Tokens are validated on each authenticated request:

1. **JWT Validation**: Token signature, expiration, issuer, and audience are validated
2. **Device Validation**: The device associated with the token is verified in the database
3. **Expiration Check**: Device expiration is checked against the current timestamp

### Using Tokens

Include the JWT token in the `Authorization` header:

```
Authorization: Bearer <your-token>
```

### Device Management

- Each login/registration creates or updates a device record
- Devices are automatically expired based on the token expiration
- Device `LastUsedAt` is updated on each authenticated request
- Multiple devices per user are supported

## Project Structure

Complete folder structure of the AuthService solution:

```
AuthService/
├─ .github/                                  # GitHub configuration
│  └─ workflows/                             # GitHub Actions workflows
│
├─ AuthService.API/                          # Presentation Layer (Web API)
│  ├─ Configurations/                        # Service configurations
│  │  ├─ CorsConfiguration.cs                # CORS policy configuration
│  │  ├─ DatabaseConfiguration.cs            # Database connection setup
│  │  ├─ GraphQLValidationConfiguration.cs   # GraphQL validation setup
│  │  ├─ ServiceConfiguration.cs             # Service registration
│  │  └─ ValidationConfiguration.cs          # FluentValidation setup
│  │
│  ├─ Controllers/                           # REST API Controllers
│  │  └─ UserController.cs                   # User management endpoints
│  │
│  ├─ DTOs/                                  # Data Transfer Objects
│  │  ├─ LoginDto.cs                         # Login request DTO
│  │  ├─ RegisterDto.cs                      # Registration request DTO
│  │  └─ UpdateUserDto.cs                    # Update user request DTO
│  │
│  ├─ Extensions/                            # Extension methods
│  │  └─ RequestLoggingMiddlewareExtensions.cs # Request logging extensions
│  │
│  ├─ GraphQL/                               # GraphQL API
│  │  ├─ InputTypes/                         # GraphQL input types
│  │  │  └─ RegisterUserInput.cs             # User registration input
│  │  ├─ Mutations/                          # GraphQL mutations
│  │  │  └─ UserMutation.cs                  # User-related mutations
│  │  ├─ Queries/                            # GraphQL queries
│  │  │  └─ UserQuery.cs                     # User-related queries
│  │  ├─ Types/                              # GraphQL types
│  │  │  ├─ AuthPayload.cs                   # Authentication response type
│  │  │  └─ UserType.cs                      # User type definition
│  │  └─ GraphQLConfiguration.cs             # GraphQL server setup
│  │
│  ├─ Middleware/                            # Custom middleware
│  │  ├─ JwtAuthenticationMiddleware.cs      # JWT authentication setup
│  │  ├─ RequestLoggingMiddleware.cs         # Request/response logging
│  │  └─ ValidationMiddleware.cs             # GraphQL validation middleware
│  │
│  ├─ Properties/                            # Application properties
│  │  └─ launchSettings.json                 # Launch configuration
│  │
│  ├─ Validators/                            # FluentValidation validators
│  │  ├─ LoginDtoValidator.cs                # Login validation rules
│  │  ├─ RegisterDtoValidator.cs             # Registration validation rules
│  │  └─ UpdateUserDtoValidator.cs           # Update user validation rules
│  │
│  ├─ appsettings.Development.json           # Development settings
│  ├─ appsettings.json                       # Application settings
│  ├─ AuthService.API.csproj                 # Project file
│  ├─ AuthService.API.csproj.user            # User-specific project settings
│  ├─ Dockerfile                             # Docker configuration
│  └─ Program.cs                             # Application entry point
│
├─ AuthService.Application/                  # Application Layer (Business Logic)
│  ├─ Configurations/                        # Configuration models
│  │  └─ JwtConfiguration.cs                 # JWT configuration model
│  │
│  ├─ DependencyInjection/                   # DI registration
│  │  └─ DependencyInjection.cs              # Service registration
│  │
│  ├─ Exceptions/                            # Custom exceptions
│  │  └─ DuplicateAccountException.cs        # Duplicate account exception
│  │
│  ├─ Helpers/                               # Utility classes
│  │  └─ PasswordHelper.cs                   # Password hashing and verification
│  │
│  ├─ Interfaces/                            # Service interfaces
│  │  ├─ ITokenService.cs                    # Token service interface
│  │  └─ IUserService.cs                     # User service interface
│  │
│  ├─ Services/                              # Business logic services
│  │  ├─ TokenService.cs                     # JWT token management
│  │  └─ UserService.cs                      # User business logic
│  │
│  └─ AuthService.Application.csproj         # Project file
│
├─ AuthService.Infrastructure/               # Infrastructure Layer (Data Access)
│  ├─ Configurations/                        # Infrastructure configurations (empty)
│  │
│  ├─ Data/                                  # Database context
│  │  └─ AppDbContext.cs                     # Entity Framework DbContext
│  │
│  ├─ DependencyInjection/                   # DI registration
│  │  └─ DependencyInjection.cs              # Infrastructure services registration
│  │
│  ├─ Entities/                              # Domain entities
│  │  ├─ User.cs                             # User entity
│  │  └─ UserDevice.cs                       # User device entity
│  │
│  ├─ Migrations/                            # EF Core migrations
│  │  ├─ 20251101194806_InitialCreate.cs     # Initial database schema
│  │  ├─ 20251101194806_InitialCreate.Designer.cs
│  │  ├─ 20251103211521_AddUserDeviceTable.cs # UserDevice table migration
│  │  ├─ 20251103211521_AddUserDeviceTable.Designer.cs
│  │  └─ AppDbContextModelSnapshot.cs        # EF Core model snapshot
│  │
│  ├─ Repositories/                          # Data access repositories
│  │  ├─ IUnitOfWork.cs                      # Unit of Work interface
│  │  ├─ IUserDeviceRepository.cs            # User device repository interface
│  │  ├─ IUserRepository.cs                  # User repository interface
│  │  ├─ UnitOfWork.cs                       # Unit of Work implementation
│  │  ├─ UserDeviceRepository.cs             # User device repository implementation
│  │  └─ UserRepository.cs                   # User repository implementation
│  │
│  └─ AuthService.Infrastructure.csproj      # Project file
│
├─ AuthService.Tests/                        # Test Projects
│  ├─ Integration/                           # Integration tests
│  │  ├─ UserFlowCollection.cs               # Test collection configuration
│  │  ├─ UserFlowFixture.cs                  # Test fixture setup
│  │  ├─ UserFlowTests.cs                    # User flow integration tests
│  │  └─ UserRegistrationTests.cs            # User registration tests
│  │
│  ├─ Mocks/                                 # Mock implementations
│  │  ├─ TokenServiceMock.cs                 # Token service mock
│  │  ├─ UnitOfWorkMock.cs                   # Unit of Work mock
│  │  └─ UserRepositoryMock.cs               # User repository mock
│  │
│  ├─ TestUtils/                             # Test utilities
│  │  └─ CustomWebApplicationFactory.cs      # Web application factory for testing
│  │
│  ├─ Unit/                                  # Unit tests
│  │  ├─ TokenServiceTests.cs                # Token service unit tests
│  │  └─ UserServiceTests.cs                 # User service unit tests
│  │
│  └─ AuthService.Tests.csproj               # Test project file
│
├─ .dockerignore                             # Docker ignore file
├─ .gitattributes                            # Git attributes configuration
├─ .gitignore                                # Git ignore file
├─ AuthService.sln                           # Solution file
├─ Migrations.txt                            # Migration documentation
└─ README.md                                 # Project documentation
```

### Layer Responsibilities

#### AuthService.API (Presentation Layer)
- **Controllers**: Handle HTTP requests and responses
- **GraphQL**: GraphQL schema, queries, mutations, and types
- **Middleware**: Cross-cutting concerns (authentication, logging, validation)
- **DTOs**: Data transfer objects for API requests/responses
- **Validators**: Input validation using FluentValidation
- **Configurations**: Service and middleware configurations
- **Extensions**: Extension methods for middleware registration

#### AuthService.Application (Application Layer)
- **Services**: Business logic implementation
- **Interfaces**: Service contracts
- **Helpers**: Utility classes (password hashing, etc.)
- **Configurations**: Application configuration models
- **Exceptions**: Custom business exceptions
- **DependencyInjection**: Service registration

#### AuthService.Infrastructure (Infrastructure Layer)
- **Data**: Entity Framework DbContext
- **Entities**: Domain entities (User, UserDevice)
- **Repositories**: Data access layer (Repository pattern)
- **Migrations**: Database schema migrations
- **DependencyInjection**: Infrastructure services registration
- **Configurations**: Infrastructure configuration (currently empty)

#### AuthService.Tests (Test Layer)
- **Unit**: Unit tests for services and helpers
- **Integration**: Integration tests for API endpoints
- **Mocks**: Mock implementations for testing
- **TestUtils**: Test utilities and factories

### Root Files
- **AuthService.sln**: Visual Studio solution file
- **README.md**: Project documentation
- **Migrations.txt**: Migration documentation
- **.gitignore**: Git ignore patterns
- **.gitattributes**: Git attributes configuration
- **.dockerignore**: Docker ignore patterns

## Testing

### Running Tests

```bash
dotnet test
```

### Running Specific Test Projects

```bash
# Unit tests
dotnet test AuthService.Tests/Unit

# Integration tests
dotnet test AuthService.Tests/Integration
```

### Test Coverage

The test suite includes:
- Unit tests for services (UserService, TokenService)
- Unit tests for helpers (PasswordHelper)
- Integration tests for API endpoints
- Integration tests for user flows

### Test Tools

- **xUnit**: Testing framework
- **Moq**: Mocking framework
- **FluentAssertions**: Assertion library
- **Microsoft.AspNetCore.Mvc.Testing**: Integration testing
- **Testcontainers**: Containerized database testing

## Docker Support

### Dockerfile

The project includes a Dockerfile for containerized deployment:

```dockerfile
# Multi-stage build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
# ... build steps ...

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
# ... runtime steps ...
```

### Building Docker Image

```bash
docker build -t authservice:latest -f AuthService.API/Dockerfile .
```

### Running Docker Container

```bash
docker run -p 8080:8080 \
  -e ConnectionStrings__DefaultConnection="Host=host.docker.internal;Port=5432;Database=AuthServiceDB;Username=postgres;Password=your_password" \
  -e Jwt__SecretKey="your-secret-key" \
  authservice:latest
```

### Docker Compose (Example)

Create a `docker-compose.yml` file:

```yaml
version: '3.8'

services:
  postgres:
    image: postgres:15
    environment:
      POSTGRES_DB: AuthServiceDB
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: your_password
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data

  authservice:
    build:
      context: .
      dockerfile: AuthService.API/Dockerfile
    ports:
      - "8080:8080"
    environment:
      ConnectionStrings__DefaultConnection: "Host=postgres;Port=5432;Database=AuthServiceDB;Username=postgres;Password=your_password"
      Jwt__SecretKey: "your-secret-key"
    depends_on:
      - postgres

volumes:
  postgres_data:
```

Run with:

```bash
docker-compose up
```

## Development

### Code Style

The project follows C# coding conventions and best practices:
- Use `Nullable` reference types
- Use `async/await` for asynchronous operations
- Use dependency injection
- Follow SOLID principles
- Use meaningful variable and method names

### Adding New Features

1. **Add Entity** (Infrastructure layer):
   - Create entity in `AuthService.Infrastructure/Entities`
   - Add DbSet to `AppDbContext`
   - Create migration

2. **Add Repository** (Infrastructure layer):
   - Create interface in `AuthService.Infrastructure/Repositories`
   - Implement repository
   - Register in `DependencyInjection.cs`

3. **Add Service** (Application layer):
   - Create interface in `AuthService.Application/Interfaces`
   - Implement service in `AuthService.Application/Services`
   - Register in `DependencyInjection.cs`

4. **Add API Endpoint** (API layer):
   - Add controller method or GraphQL query/mutation
   - Create DTOs and validators
   - Update configuration if needed

### Database Migrations

Create a migration:

```bash
cd AuthService.Infrastructure
dotnet ef migrations add MigrationName --startup-project ../AuthService.API
```

Apply migrations:

```bash
dotnet ef database update --startup-project ../AuthService.API
```

### Logging

The application includes comprehensive logging:
- Request/response logging via `RequestLoggingMiddleware`
- Authentication events logging
- Error logging
- Configuration logging (masked secrets)

Log levels can be configured in `appsettings.json`.

## Security Features

### Password Security

- **Algorithm**: PBKDF2 with SHA-256
- **Iterations**: 100,000 (configurable)
- **Salt**: 128-bit random salt per password
- **Key Size**: 256-bit derived key
- **Timing Attack Prevention**: Constant-time comparison using `CryptographicOperations.FixedTimeEquals`

### JWT Security

- **Algorithm**: HS256 (HMAC-SHA256)
- **Token Expiration**: Configurable (default: 3 months)
- **Device Binding**: Tokens are bound to specific devices
- **Token Validation**: Comprehensive validation on each request
- **Secret Key**: Strong secret key required (minimum 32 characters)

### Input Validation

- **FluentValidation**: Comprehensive input validation
- **Email Validation**: RFC-compliant email validation
- **Password Strength**: Minimum 8 characters, uppercase, number requirements
- **SQL Injection Protection**: Entity Framework Core parameterized queries

### Authentication Security

- **JWT Bearer Authentication**: Industry-standard token-based authentication
- **Device Validation**: Tokens are validated against device records
- **Expiration Management**: Automatic token and device expiration
- **CORS Configuration**: Configurable CORS policies

### Best Practices

- Never commit secrets to version control
- Use strong, randomly generated secret keys
- Store sensitive configuration in secure stores
- Use HTTPS in production
- Regularly rotate JWT secret keys
- Monitor and log authentication events
- Implement rate limiting (recommended)
- Use environment-specific configurations

## License

This project is licensed under the MIT License.

## Contributing

Contributions are welcome! Please follow these guidelines:

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new features
5. Ensure all tests pass
6. Submit a pull request

## Support

For issues, questions, or contributions, please open an issue on the GitHub repository.

## Acknowledgments

- Built with [.NET 9.0](https://dotnet.microsoft.com/)
- Uses [HotChocolate](https://chillicream.com/docs/hotchocolate) for GraphQL
- Uses [FluentValidation](https://fluentvalidation.net/) for validation
- Uses [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/) for data access

---

**Note**: This is a production-ready authentication service. Ensure proper security practices are followed in production environments, including secure secret management, HTTPS, rate limiting, and monitoring.
