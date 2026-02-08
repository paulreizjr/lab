# JWT Authentication in ASP.NET Core - Complete Guide

## Overview

This project demonstrates **JWT (JSON Web Token) authentication** implementation in ASP.NET Core 8.0 with comprehensive examples, comments, and best practices.

---

## 📋 Table of Contents

1. [What is JWT?](#what-is-jwt)
2. [When to Use JWT](#when-to-use-jwt)
3. [When NOT to Use JWT](#when-not-to-use-jwt)
4. [Project Structure](#project-structure)
5. [Getting Started](#getting-started)
6. [Testing the API](#testing-the-api)
7. [Security Best Practices](#security-best-practices)

---

## 🔐 What is JWT?

**JWT (JSON Web Token)** is an open standard (RFC 7519) for securely transmitting information between parties as a JSON object.

### JWT Structure

A JWT consists of three parts separated by dots (`.`):

```
xxxxx.yyyyy.zzzzz
```

1. **Header**: Algorithm and token type
   ```json
   {
     "alg": "HS256",
     "typ": "JWT"
   }
   ```

2. **Payload**: Claims about the user
   ```json
   {
     "sub": "12345",
     "name": "John Doe",
     "role": "Admin",
     "exp": 1516239022
   }
   ```

3. **Signature**: Ensures integrity
   ```
   HMACSHA256(
     base64UrlEncode(header) + "." + base64UrlEncode(payload),
     secret
   )
   ```

### How JWT Authentication Works

```
┌─────────┐                 ┌─────────┐                 ┌─────────┐
│ Client  │                 │   API   │                 │Database │
└────┬────┘                 └────┬────┘                 └────┬────┘
     │                           │                           │
     │ 1. POST /api/auth/login   │                           │
     │   (username, password)    │                           │
     ├──────────────────────────>│                           │
     │                           │ 2. Validate credentials   │
     │                           ├──────────────────────────>│
     │                           │<──────────────────────────┤
     │                           │ 3. Generate JWT token     │
     │ 4. Return JWT token       │                           │
     │<──────────────────────────┤                           │
     │                           │                           │
     │ 5. GET /api/securedata    │                           │
     │   Header: Bearer {token}  │                           │
     ├──────────────────────────>│                           │
     │                           │ 6. Validate token         │
     │                           │    (signature, expiry)    │
     │ 7. Return protected data  │                           │
     │<──────────────────────────┤                           │
     │                           │                           │
```

**Key Benefits:**
- ✅ **Stateless**: No server-side session storage
- ✅ **Scalable**: Works across multiple servers
- ✅ **Self-contained**: Token carries user information
- ✅ **Cross-platform**: Works with any client

---

## ✅ When to Use JWT

### Perfect Scenarios

#### 1. **REST APIs for Mobile Applications**
```
Why: Mobile apps need stateless auth that persists across app restarts
Example: iOS/Android app calling your API
```

#### 2. **Single Page Applications (SPAs)**
```
Why: SPAs make frequent API calls and need efficient authentication
Examples: React, Angular, Vue applications
```

#### 3. **Microservices Architecture**
```
Why: Each service can validate tokens independently
No shared session store needed across services
```

#### 4. **Third-Party API Integrations**
```
Why: Standard approach for public APIs
Example: Providing API access to external developers
```

#### 5. **Cross-Domain Scenarios**
```
Why: JWTs work seamlessly across different domains
Unlike cookies which have same-origin restrictions
```

#### 6. **Distributed Systems**
```
Why: Any server can validate the token
No sticky sessions or shared state required
Perfect for horizontal scaling
```

#### 7. **Multi-Tenant SaaS Applications**
```
Why: Embed tenant ID in token
Each request knows its context without DB lookup
```

---

## ❌ When NOT to Use JWT

### Use Alternatives Instead

#### 1. **Traditional Server-Rendered Web Apps**
```
❌ Don't use: JWT
✅ Use instead: Cookie-based authentication (ASP.NET Core Identity)
Why: HttpOnly cookies are more secure against XSS
     Simpler to implement and maintain
```

#### 2. **Need Immediate Token Revocation**
```
❌ Don't use: JWT
✅ Use instead: Opaque tokens with database validation
Why: JWTs can't be invalidated until expiration
     User banned? Token still valid until it expires
     Password changed? Old tokens still work
```

#### 3. **High-Security Applications**
```
❌ Don't use: JWT alone
✅ Use instead: Session-based auth with audit logging
Why: Need to track every authentication event
     Compliance requirements for detailed logs
```

#### 4. **Frequent Permission Changes**
```
❌ Don't use: JWT
✅ Use instead: Database-backed permissions
Why: Permissions are baked into token at creation
     User role changes don't affect existing tokens
```

#### 5. **Same-Origin Applications**
```
❌ Don't use: JWT
✅ Use instead: Cookie-based authentication
Why: If API only serves your own web app (same domain)
     Simpler and browser handles security automatically
```

#### 6. **Sensitive Data in Tokens**
```
❌ Never do: Store passwords, credit cards, SSN in JWT
✅ Remember: JWT payload is only base64 encoded, NOT encrypted
Why: Anyone can decode and read token contents
```

---

## 📁 Project Structure

```
jwt/
├── Controllers/
│   ├── AuthController.cs          # Login, register, token generation
│   └── SecureDataController.cs    # Protected endpoints examples
├── Models/
│   ├── JwtSettings.cs             # JWT configuration model
│   ├── LoginRequest.cs            # Login DTO
│   ├── LoginResponse.cs           # Token response DTO
│   └── UserClaims.cs              # User claims model
├── Services/
│   └── JwtTokenService.cs         # Token generation and validation
├── Program.cs                      # JWT configuration and setup
└── appsettings.json               # JWT settings (secret, issuer, etc.)
```

---

## 🚀 Getting Started

### 1. Install Dependencies

The project requires the JWT Bearer authentication package:

```bash
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
```

### 2. Configure JWT Settings

Edit `appsettings.json`:

```json
{
  "JwtSettings": {
    "SecretKey": "YourSuperSecretKeyThatShouldBeAtLeast32CharactersLong!",
    "Issuer": "YourAppName",
    "Audience": "YourAppUsers",
    "ExpirationInMinutes": 60
  }
}
```

**⚠️ IMPORTANT**: 
- Change `SecretKey` before deploying to production
- Use environment variables or Azure Key Vault for production
- Secret key must be at least 256 bits (32 characters) for HS256

### 3. Run the Application

```bash
dotnet restore
dotnet run
```

The API will be available at: `https://localhost:5001` (or configured port)

---

## 🧪 Testing the API

### Using Swagger UI

1. Navigate to `https://localhost:5001/swagger`
2. Test the authentication flow:

#### Step 1: Login

```http
POST /api/auth/login
Content-Type: application/json

{
  "username": "john.doe",
  "password": "password123"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiration": "2026-01-19T15:30:00Z",
  "username": "john.doe",
  "email": "john.doe@example.com",
  "roles": ["User", "Manager"]
}
```

#### Step 2: Use Token

Click the **Authorize** button in Swagger UI and enter:
```
Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Using cURL

#### 1. Login
```bash
curl -X POST https://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"john.doe","password":"password123"}'
```

#### 2. Access Protected Endpoint
```bash
curl -X GET https://localhost:5001/api/securedata/protected \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

#### 3. Access Admin Endpoint (requires Admin role)
```bash
curl -X GET https://localhost:5001/api/securedata/admin \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

### Using Postman

1. **Login Request**:
   - Method: POST
   - URL: `https://localhost:5001/api/auth/login`
   - Body (JSON):
     ```json
     {
       "username": "john.doe",
       "password": "password123"
     }
     ```

2. **Copy the token** from response

3. **Protected Request**:
   - Method: GET
   - URL: `https://localhost:5001/api/securedata/protected`
   - Headers:
     - Key: `Authorization`
     - Value: `Bearer YOUR_TOKEN_HERE`

---

## 🔒 Security Best Practices

### Configuration

✅ **Use HTTPS in Production**
```csharp
// In Program.cs
app.UseHttpsRedirection();
```

✅ **Store Secret Key Securely**
```csharp
// Development: appsettings.Development.json
// Production: Environment variables or Azure Key Vault
var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") 
    ?? builder.Configuration["JwtSettings:SecretKey"];
```

✅ **Set Appropriate Expiration**
```json
{
  "ExpirationInMinutes": 15  // Short for access tokens
}
```

### Token Management

✅ **Implement Refresh Tokens**
```
Access Token: 15-60 minutes (short-lived)
Refresh Token: 7-30 days (long-lived, stored in database)
```

✅ **Validate All Token Properties**
```csharp
ValidateIssuerSigningKey = true,
ValidateIssuer = true,
ValidateAudience = true,
ValidateLifetime = true,
ClockSkew = TimeSpan.Zero  // No tolerance for expired tokens
```

### Application Security

✅ **Never Store Sensitive Data in JWT**
```
❌ Don't: Passwords, credit cards, SSN
✅ Do: User ID, username, roles, non-sensitive claims
```

✅ **Implement Rate Limiting**
```csharp
// Prevent brute force attacks on login endpoint
// Use AspNetCoreRateLimit or similar
```

✅ **Add Account Lockout**
```csharp
// Lock account after X failed login attempts
// Implement with ASP.NET Core Identity
```

✅ **Log Security Events**
```csharp
_logger.LogWarning("Failed login attempt for user: {Username}", username);
_logger.LogInformation("Successful login for user: {Username}", username);
```

### Client-Side Security

✅ **Store Tokens Securely**
```
Web: Secure HttpOnly cookies (best) or sessionStorage
Mobile: Keychain (iOS) or Keystore (Android)
❌ Avoid: localStorage (vulnerable to XSS)
```

✅ **Handle Token Expiration**
```javascript
// Check token expiration before each request
// Refresh token automatically or redirect to login
if (tokenExpired()) {
  await refreshToken();
}
```

---

## 📚 Additional Resources

### Official Documentation
- [Microsoft Docs - JWT Bearer Authentication](https://learn.microsoft.com/en-us/aspnet/core/security/authentication)
- [JWT.io - Introduction to JWT](https://jwt.io/introduction)
- [RFC 7519 - JWT Standard](https://datatracker.ietf.org/doc/html/rfc7519)

### Related Topics
- ASP.NET Core Identity for user management
- Refresh token implementation
- OAuth 2.0 and OpenID Connect
- API rate limiting
- CORS configuration

---

## 🎯 Key Takeaways

1. **JWT is perfect for APIs** serving mobile apps, SPAs, and microservices
2. **Not for traditional web apps** - use cookie-based auth instead
3. **Tokens can't be revoked** - keep expiration times short
4. **Always use HTTPS** - tokens can be intercepted
5. **Never store sensitive data** - payload is not encrypted
6. **Implement refresh tokens** - for better user experience
7. **Validate everything** - signature, expiration, issuer, audience

---

## 📝 Notes

- This example uses **mock authentication** for demonstration
- Replace `ValidateUserCredentials()` with real database lookup
- Use **ASP.NET Core Identity** for production user management
- Implement **password hashing** with BCrypt or Argon2
- Add **multi-factor authentication** for enhanced security
- Consider **token blacklisting** for logout functionality

---

**Remember**: Security is a journey, not a destination. Keep learning and stay updated! 🔐
