using jwt.Models;
using jwt.Services;
using Microsoft.AspNetCore.Mvc;

namespace jwt.Controllers
{
    /// <summary>
    /// Authentication Controller
    /// 
    /// PURPOSE:
    /// - Handles user authentication and JWT token generation
    /// - Provides login endpoint for username/password authentication
    /// - Returns JWT tokens for authenticated users
    /// - Demonstrates basic authentication flow
    /// 
    /// SCENARIOS TO USE:
    /// - When building REST APIs for mobile apps or SPAs
    /// - For stateless authentication across microservices
    /// - When you need programmatic API access (API keys, service-to-service)
    /// - For applications requiring fine-grained permission control
    /// - When building APIs consumed by third-party developers
    /// 
    /// SCENARIOS NOT TO USE:
    /// - Server-rendered web apps (use cookie-based authentication instead)
    /// - When you need immediate token revocation (JWT can't be revoked until expiry)
    /// - Simple internal apps where session-based auth is sufficient
    /// - When integrating with existing SSO systems (use OAuth/SAML)
    /// - High-security scenarios requiring audit trails of each authentication
    /// 
    /// SECURITY BEST PRACTICES:
    /// - Always use HTTPS in production (prevent token interception)
    /// - Implement rate limiting on login endpoint (prevent brute force)
    /// - Use strong password hashing (bcrypt, Argon2)
    /// - Implement account lockout after failed attempts
    /// - Log authentication attempts for security monitoring
    /// - Consider multi-factor authentication (MFA)
    /// - Implement refresh tokens for better security
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IJwtTokenService _tokenService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IJwtTokenService tokenService, ILogger<AuthController> logger)
        {
            _tokenService = tokenService;
            _logger = logger;
        }

        /// <summary>
        /// Login endpoint - authenticates user and returns JWT token
        /// 
        /// ENDPOINT: POST /api/auth/login
        /// 
        /// REQUEST BODY EXAMPLE:
        /// {
        ///     "username": "john.doe",
        ///     "password": "SecurePassword123!"
        /// }
        /// 
        /// SUCCESSFUL RESPONSE (200 OK):
        /// {
        ///     "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
        ///     "expiration": "2026-01-19T15:30:00Z",
        ///     "username": "john.doe",
        ///     "email": "john.doe@example.com",
        ///     "roles": ["User", "Manager"]
        /// }
        /// 
        /// ERROR RESPONSE (401 Unauthorized):
        /// {
        ///     "message": "Invalid username or password"
        /// }
        /// 
        /// WORKFLOW:
        /// 1. Validate request model (check required fields)
        /// 2. Verify credentials against database (currently mocked)
        /// 3. If valid, create user claims with user information
        /// 4. Generate JWT token with claims
        /// 5. Return token and user information to client
        /// 
        /// CLIENT-SIDE USAGE:
        /// After receiving the token, client should:
        /// 1. Store token securely (HttpOnly cookie or secure storage)
        /// 2. Include in subsequent requests: Authorization: Bearer {token}
        /// 3. Handle token expiration (refresh or re-authenticate)
        /// 4. Clear token on logout
        /// </summary>
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            _logger.LogInformation("Login attempt for user: {Username}", request.Username);

            // ===================================================================
            // IMPORTANT: MOCK AUTHENTICATION - REPLACE WITH REAL IMPLEMENTATION
            // ===================================================================
            // In production, you should:
            // 1. Query database to find user by username
            // 2. Verify password hash using BCrypt, Argon2, or similar
            // 3. Check if account is locked or disabled
            // 4. Implement rate limiting to prevent brute force attacks
            // 5. Log authentication attempts for security monitoring
            // 
            // Example with ASP.NET Core Identity:
            // var user = await _userManager.FindByNameAsync(request.Username);
            // var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
            // if (!result.Succeeded) return Unauthorized();
            // ===================================================================

            // Mock user validation (REPLACE THIS IN PRODUCTION!)
            if (!ValidateUserCredentials(request.Username, request.Password))
            {
                _logger.LogWarning("Failed login attempt for user: {Username}", request.Username);
                return Unauthorized(new { message = "Invalid username or password" });
            }

            // In production, fetch these from your database
            var userClaims = new UserClaims
            {
                UserId = "12345",                    // From database user ID
                Username = request.Username,
                Email = $"{request.Username}@example.com",
                Roles = new List<string> { "User", "Manager" }, // From database user roles
                CustomClaims = new Dictionary<string, string>
                {
                    { "department", "Engineering" },  // Application-specific claims
                    { "employeeId", "EMP-12345" }
                }
            };

            // Generate JWT token
            var token = _tokenService.GenerateToken(userClaims);
            var expiration = DateTime.UtcNow.AddMinutes(60); // Should match JwtSettings.ExpirationInMinutes

            _logger.LogInformation("Successful login for user: {Username}", request.Username);

            // Return token and user information
            return Ok(new LoginResponse
            {
                Token = token,
                Expiration = expiration,
                Username = userClaims.Username,
                Email = userClaims.Email,
                Roles = userClaims.Roles
            });
        }

        /// <summary>
        /// Register endpoint - creates new user account
        /// 
        /// PURPOSE:
        /// - Demonstrates user registration with automatic login
        /// - Shows how to generate token for new users
        /// 
        /// NOTE: This is a simplified example
        /// In production, add:
        /// - Email verification
        /// - Password strength validation
        /// - Username uniqueness check
        /// - CAPTCHA to prevent bot registrations
        /// - Terms of service acceptance
        /// </summary>
        [HttpPost("register")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Register([FromBody] LoginRequest request)
        {
            _logger.LogInformation("Registration attempt for user: {Username}", request.Username);

            // ===================================================================
            // IMPORTANT: MOCK REGISTRATION - REPLACE WITH REAL IMPLEMENTATION
            // ===================================================================
            // In production:
            // 1. Validate username doesn't already exist
            // 2. Enforce password complexity rules
            // 3. Hash password with BCrypt or Argon2
            // 4. Save user to database
            // 5. Send verification email
            // 6. Consider requiring email confirmation before login
            // ===================================================================

            // Check if user already exists (mock)
            if (request.Username == "existing.user")
            {
                return BadRequest(new { message = "Username already exists" });
            }

            // Create new user (in production, save to database)
            var userClaims = new UserClaims
            {
                UserId = Guid.NewGuid().ToString(),
                Username = request.Username,
                Email = $"{request.Username}@example.com",
                Roles = new List<string> { "User" } // New users get basic role
            };

            // Generate token for immediate login
            var token = _tokenService.GenerateToken(userClaims);
            var expiration = DateTime.UtcNow.AddMinutes(60);

            _logger.LogInformation("Successful registration for user: {Username}", request.Username);

            return Ok(new LoginResponse
            {
                Token = token,
                Expiration = expiration,
                Username = userClaims.Username,
                Email = userClaims.Email,
                Roles = userClaims.Roles
            });
        }

        /// <summary>
        /// Mock credential validation
        /// 
        /// REPLACE THIS IN PRODUCTION with:
        /// - Database query to find user
        /// - Password hash verification
        /// - Account status checks (locked, disabled, etc.)
        /// 
        /// For demonstration, accepts any username with password "password123"
        /// </summary>
        private bool ValidateUserCredentials(string username, string password)
        {
            // MOCK VALIDATION - DO NOT USE IN PRODUCTION
            // In production, use:
            // var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            // return user != null && BCrypt.Verify(password, user.PasswordHash);
            
            return !string.IsNullOrEmpty(username) && password == "password123";
        }

        /// <summary>
        /// Refresh token endpoint (example implementation)
        /// 
        /// PURPOSE:
        /// - Allows users to get new access token without re-authenticating
        /// - Improves user experience (no repeated logins)
        /// - Maintains security with short-lived access tokens
        /// 
        /// TYPICAL IMPLEMENTATION:
        /// 1. Client sends refresh token (long-lived, stored securely)
        /// 2. Server validates refresh token against database
        /// 3. If valid, generate new access token
        /// 4. Optionally rotate refresh token (one-time use)
        /// 
        /// SECURITY CONSIDERATIONS:
        /// - Store refresh tokens in database (can be revoked)
        /// - Use longer expiration (7-30 days)
        /// - Implement token rotation (invalidate after use)
        /// - Track token family to detect theft
        /// - Revoke all tokens on suspicious activity
        /// 
        /// NOTE: This is a placeholder - implement with proper refresh token storage
        /// </summary>
        [HttpPost("refresh")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult RefreshToken([FromBody] string refreshToken)
        {
            _logger.LogInformation("Token refresh attempt");

            // In production:
            // 1. Validate refresh token against database
            // 2. Check if token is expired or revoked
            // 3. Get user information from token
            // 4. Generate new access token
            // 5. Optionally generate new refresh token (rotation)
            // 6. Invalidate old refresh token

            return Unauthorized(new { message = "Refresh token implementation required" });
        }

        /// <summary>
        /// Logout endpoint (example)
        /// 
        /// PURPOSE:
        /// - Demonstrates token blacklisting approach
        /// - Provides clean logout experience
        /// 
        /// JWT LOGOUT CHALLENGES:
        /// - JWTs are stateless - server doesn't track them
        /// - Can't "delete" a JWT until it expires naturally
        /// 
        /// LOGOUT STRATEGIES:
        /// 1. Client-side: Simply delete token (token still valid until expiry)
        /// 2. Token blacklist: Store revoked tokens in Redis/database
        /// 3. Short expiration: Keep tokens short-lived (15-30 minutes)
        /// 4. Refresh tokens: Revoke refresh token on logout
        /// 
        /// RECOMMENDED APPROACH:
        /// - Use short-lived access tokens (15-30 min)
        /// - Implement refresh tokens stored in database
        /// - On logout, delete refresh token from database
        /// - Client deletes access token (will expire soon anyway)
        /// </summary>
        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Logout()
        {
            // In production:
            // 1. Get user ID from current claims (User.FindFirst(ClaimTypes.NameIdentifier))
            // 2. Revoke all refresh tokens for this user
            // 3. Optionally add access token to blacklist (if using blacklist approach)
            // 4. Log logout event

            _logger.LogInformation("User logged out");
            return Ok(new { message = "Logged out successfully" });
        }
    }
}
