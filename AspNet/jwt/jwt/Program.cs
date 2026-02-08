
using jwt.Models;
using jwt.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace jwt
{
    /// <summary>
    /// ASP.NET Core Web API with JWT Authentication
    /// 
    /// PURPOSE OF JWT (JSON Web Tokens):
    /// - Stateless authentication: No server-side session storage needed
    /// - Scalable: Works across multiple servers without shared session state
    /// - Self-contained: Token carries all necessary user information
    /// - Cross-platform: Works with any client (web, mobile, desktop)
    /// - API-friendly: Perfect for REST APIs and microservices
    /// 
    /// HOW JWT WORKS:
    /// 1. User logs in with credentials (username/password)
    /// 2. Server validates credentials and generates JWT token
    /// 3. Token contains user claims (ID, roles, permissions) and is signed
    /// 4. Client stores token and includes it in subsequent requests
    /// 5. Server validates token signature and extracts claims
    /// 6. No database lookup needed for each request (stateless)
    /// 
    /// JWT STRUCTURE:
    /// - Header: Algorithm and token type (e.g., {"alg":"HS256","typ":"JWT"})
    /// - Payload: Claims about the user (e.g., {"sub":"12345","name":"John","role":"Admin"})
    /// - Signature: HMACSHA256(base64UrlEncode(header) + "." + base64UrlEncode(payload), secret)
    /// - Format: xxxxx.yyyyy.zzzzz (header.payload.signature)
    /// 
    /// ============================================================================
    /// SCENARIOS TO USE JWT AUTHENTICATION:
    /// ============================================================================
    /// 
    /// ✅ REST APIs for mobile applications
    ///    - Mobile apps need stateless auth that works across app restarts
    ///    - JWT can be stored securely in mobile device secure storage
    /// 
    /// ✅ Single Page Applications (SPAs) - React, Angular, Vue
    ///    - SPAs make many API calls and need efficient auth
    ///    - JWT eliminates need for session cookies
    /// 
    /// ✅ Microservices architecture
    ///    - Services can validate tokens independently
    ///    - No need for shared session store across services
    ///    - Token can carry authorization data for service-to-service calls
    /// 
    /// ✅ Third-party API integrations
    ///    - Provide API access to external developers
    ///    - Token-based auth is standard for public APIs
    /// 
    /// ✅ Cross-domain/CORS scenarios
    ///    - Unlike cookies, JWTs work seamlessly across domains
    ///    - Perfect for APIs serving multiple frontend applications
    /// 
    /// ✅ Distributed systems requiring scalability
    ///    - No sticky sessions needed - any server can validate token
    ///    - Easy horizontal scaling without session state management
    /// 
    /// ✅ Multi-tenant SaaS applications
    ///    - Tenant ID can be embedded in token
    ///    - Each request knows its tenant context without DB lookup
    /// 
    /// ✅ Stateless authentication for IoT devices
    ///    - Devices can authenticate without maintaining sessions
    ///    - Tokens can be refreshed periodically
    /// 
    /// ============================================================================
    /// SCENARIOS NOT TO USE JWT (Consider alternatives):
    /// ============================================================================
    /// 
    /// ❌ Traditional server-rendered web applications
    ///    - Cookie-based session auth is simpler and more secure
    ///    - HttpOnly cookies prevent XSS attacks
    ///    - Server-side sessions easier to revoke immediately
    ///    - Use ASP.NET Core Identity with cookie authentication instead
    /// 
    /// ❌ When immediate token revocation is critical
    ///    - JWTs can't be invalidated until they expire
    ///    - If user changes password or gets banned, token remains valid
    ///    - Would need token blacklist (defeats stateless purpose)
    ///    - For these cases, use opaque tokens with server-side validation
    /// 
    /// ❌ High-security applications requiring audit trails
    ///    - Can't track every request authentication in real-time
    ///    - No server-side record of token usage
    ///    - Consider session-based auth with detailed logging
    /// 
    /// ❌ Applications with very frequent permission changes
    ///    - Permissions are baked into token at creation time
    ///    - If user's role changes, old token still has old permissions
    ///    - Need short expiration times (impacts user experience)
    /// 
    /// ❌ When all clients are same-origin
    ///    - If your API only serves your own web app (same domain)
    ///    - Cookie-based auth is simpler and more secure
    ///    - Browser handles cookie security automatically
    /// 
    /// ❌ Applications storing sensitive data in tokens
    ///    - JWT payload is only base64 encoded, NOT encrypted
    ///    - Anyone can decode and read token contents
    ///    - Never put passwords, credit cards, or PII in tokens
    /// 
    /// ❌ Real-time applications requiring live session control
    ///    - Can't force logout or session timeout in real-time
    ///    - Use SignalR with server-side session management instead
    /// 
    /// ============================================================================
    /// SECURITY BEST PRACTICES:
    /// ============================================================================
    /// 
    /// 🔒 Always use HTTPS in production (prevent token interception)
    /// 🔒 Keep secret key secure (use environment variables or Key Vault)
    /// 🔒 Use strong secret keys (at least 256 bits for HS256)
    /// 🔒 Set short expiration times (15-60 minutes for access tokens)
    /// 🔒 Implement refresh tokens for longer sessions
    /// 🔒 Validate token signature, expiration, issuer, and audience
    /// 🔒 Never store sensitive data in JWT payload
    /// 🔒 Implement rate limiting on authentication endpoints
    /// 🔒 Use CORS properly to restrict API access
    /// 🔒 Consider token rotation strategy for refresh tokens
    /// 🔒 Log authentication attempts for security monitoring
    /// 🔒 Implement account lockout after failed login attempts
    /// 
    /// ============================================================================
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ========================================================================
            // STEP 1: Configure JWT Settings from appsettings.json
            // ========================================================================
            // Binds JwtSettings section from appsettings.json to JwtSettings class
            // Enables dependency injection of IOptions<JwtSettings>
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

            // ========================================================================
            // STEP 2: Register JWT Token Service
            // ========================================================================
            // Register our custom JWT service for dependency injection
            // Controllers can now inject IJwtTokenService to generate tokens
            builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

            // ========================================================================
            // STEP 3: Configure JWT Authentication
            // ========================================================================
            // This is the core JWT authentication configuration
            // ASP.NET Core will automatically validate tokens on protected endpoints
            
            var jwtSettings = builder.Configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT Secret Key not found");
            var key = Encoding.ASCII.GetBytes(secretKey);

            builder.Services.AddAuthentication(options =>
            {
                // Set JWT as the default authentication scheme
                // This means [Authorize] attributes will use JWT by default
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                // Configure how to validate JWT tokens
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // Validate the token signature (prevent tampering)
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),

                    // Validate the token was issued by your application
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["Issuer"],

                    // Validate the token is intended for your application
                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"],

                    // Validate the token hasn't expired
                    ValidateLifetime = true,

                    // No clock skew - token expires exactly when specified
                    // Default is 5 minutes of tolerance for clock differences
                    ClockSkew = TimeSpan.Zero
                };

                // Optional: Configure events for custom behavior
                options.Events = new JwtBearerEvents
                {
                    // Called when authentication fails
                    OnAuthenticationFailed = context =>
                    {
                        // Log authentication failures for security monitoring
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                        logger.LogWarning("JWT Authentication failed: {Message}", context.Exception.Message);
                        return Task.CompletedTask;
                    },

                    // Called when token is validated successfully
                    OnTokenValidated = context =>
                    {
                        // Optional: Add additional validation logic
                        // Example: Check if user still exists in database
                        // Example: Check if user account is active
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                        var userId = context.Principal?.FindFirst("sub")?.Value;
                        logger.LogInformation("Token validated for user: {UserId}", userId);
                        return Task.CompletedTask;
                    },

                    // Called when authentication challenge is needed (401)
                    OnChallenge = context =>
                    {
                        // Customize the 401 response if needed
                        return Task.CompletedTask;
                    }
                };
            });

            // ========================================================================
            // STEP 4: Add Authorization Policies (Optional)
            // ========================================================================
            // Define custom authorization policies beyond simple role checks
            // Useful for complex authorization scenarios
            builder.Services.AddAuthorization(options =>
            {
                // Example: Require specific claim value
                options.AddPolicy("RequireAdminRole", policy =>
                    policy.RequireRole("Admin"));

                // Example: Require multiple roles (AND logic)
                options.AddPolicy("RequireManagerAndFinance", policy =>
                    policy.RequireRole("Manager", "Finance"));

                // Example: Custom claim requirement
                options.AddPolicy("RequireDepartment", policy =>
                    policy.RequireClaim("department"));

                // Example: Complex policy with custom requirement
                options.AddPolicy("RequireEmployeeId", policy =>
                    policy.RequireClaim("employeeId"));

                // You can create custom authorization handlers for complex logic
                // Example: Check if user owns a resource, check business rules, etc.
            });

            // ========================================================================
            // STEP 5: Configure Controllers and API Documentation
            // ========================================================================
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            // Configure Swagger to support JWT authentication
            // This adds an "Authorize" button in Swagger UI
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "JWT Authentication API",
                    Version = "v1",
                    Description = "ASP.NET Core Web API with JWT Authentication Examples"
                });

                // Add JWT authentication to Swagger
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below. Example: 'Bearer eyJhbGc...'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            var app = builder.Build();

            // ========================================================================
            // STEP 6: Configure HTTP Request Pipeline
            // ========================================================================
            
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // ========================================================================
            // CRITICAL: Middleware Order Matters!
            // ========================================================================
            // 1. UseAuthentication() - Validates JWT token and populates User.Claims
            // 2. UseAuthorization() - Checks if user has permission for endpoint
            // 
            // Always call UseAuthentication() before UseAuthorization()
            // Always call before MapControllers()
            app.UseAuthentication();  // ← Must come first
            app.UseAuthorization();   // ← Must come second

            app.MapControllers();

            app.Run();
        }
    }
}

