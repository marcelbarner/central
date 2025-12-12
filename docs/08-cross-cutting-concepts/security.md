# Security

## Authentication

### ASP.NET Core Identity

The application uses **ASP.NET Core Identity** for authentication with a clean architecture approach:

**Layer Responsibilities**:
* **Domain Layer**: 
  - Defines `User` entity model (with `long` as ID type)
  - Depends only on `UserManager<User>` from Identity
  - Contains domain logic related to users
  
* **Infrastructure Layer**:
  - Implements `UserStore<User>` for data persistence
  - Configures EF Core mappings for Identity tables
  - PostgreSQL as the database backend
  
* **Server Layer**:
  - Uses `SignInManager<User>` for authentication operations
  - Exposes authentication endpoints (login, logout, get current user)
  - Manages migrations and database seeding via background jobs
  - Configures cookie authentication middleware

**Identity Configuration**:
* User ID type: `long` (instead of default `string`)
* Password hashing with built-in security policies
* Identity tables stored in PostgreSQL database

### Authentication Methods

**Cookie Authentication** (Implemented)
* Session-based authentication for web application
* Secure, HTTP-only cookies with SameSite protection
* Sliding expiration for active sessions
* No separate token management required

**API Tokens** (Future)
* Users can generate personal API tokens
* Long-lived tokens for programmatic API access
* Token management (create, revoke, list) via user profile

**Basic Authentication** (Future)
* Username/password in Authorization header
* Supported for simple integrations and scripts
* Less preferred, but available for compatibility

### Supported Operations

1. **Login**: Authenticate user with username/password
2. **Logout**: Clear authentication session
3. **Get Current User**: Retrieve authenticated user information

## Authorization

* Role-based access control via ASP.NET Core Identity roles
* Policy-based authorization at endpoint level
* Claims-based permissions for fine-grained access control
* Anonymous access explicitly configured where needed
