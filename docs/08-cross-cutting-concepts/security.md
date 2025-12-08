# Security

## Authentication

### ASP.NET Core Identity

* **User Management**: ASP.NET Core Identity for user registration, login, and profile management
* **Password Storage**: Secure hashing with built-in password policies
* **Database**: User data stored in PostgreSQL via Identity tables

### Authentication Methods

**1. Cookie Authentication** (Primary for web UI)
* Session-based authentication for Angular frontend
* Secure, HTTP-only cookies
* Automatic renewal on activity

**2. API Tokens**
* Users can generate personal API tokens
* Long-lived tokens for programmatic API access
* Token management (create, revoke, list) via user profile

**3. Basic Authentication**
* Username/password in Authorization header
* Supported for simple integrations and scripts
* Less preferred, but available for compatibility

## Authorization

* Role-based access control via ASP.NET Core Identity roles
* Policy-based authorization at endpoint level
* Claims-based permissions for fine-grained access control
* Anonymous access explicitly configured where needed
