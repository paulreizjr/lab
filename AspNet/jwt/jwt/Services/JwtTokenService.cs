using jwt.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace jwt.Services
{
    /// <summary>
    /// JWT Token Service Interface
    /// 
    /// PURPOSE:
    /// - Defines contract for JWT token generation and validation
    /// - Enables dependency injection and testing (easy to mock)
    /// - Separates authentication logic from controllers
    /// - Promotes single responsibility principle
    /// 
    /// SCENARIOS TO USE:
    /// - When implementing JWT authentication in ASP.NET Core
    /// - For dependency injection in controllers and services
    /// - When you need to unit test authentication logic
    /// - To centralize token generation logic across your application
    /// 
    /// SCENARIOS NOT TO USE:
    /// - Simple applications where inline token generation is acceptable
    /// - When using third-party authentication providers (Auth0, Okta, etc.)
    /// - If using ASP.NET Core Identity with built-in token providers
    /// </summary>
    public interface IJwtTokenService
    {
        /// <summary>
        /// Generates a JWT token for a user
        /// </summary>
        /// <param name="userClaims">User information to embed in the token</param>
        /// <returns>JWT token string</returns>
        string GenerateToken(UserClaims userClaims);

        /// <summary>
        /// Validates a JWT token
        /// </summary>
        /// <param name="token">JWT token to validate</param>
        /// <returns>ClaimsPrincipal if valid, null otherwise</returns>
        ClaimsPrincipal? ValidateToken(string token);
    }

    /// <summary>
    /// JWT Token Service Implementation
    /// 
    /// PURPOSE:
    /// - Creates and validates JWT tokens
    /// - Handles token signing with secret key
    /// - Manages token expiration and claims
    /// - Encapsulates all JWT-related cryptographic operations
    /// 
    /// SCENARIOS TO USE:
    /// - After user successfully authenticates (username/password check)
    /// - When generating tokens for registration or password reset
    /// - For implementing token refresh mechanisms
    /// - In API endpoints that require user authentication
    /// 
    /// SCENARIOS NOT TO USE:
    /// - Don't use for OAuth flows (use OAuth libraries)
    /// - Don't use if you need opaque tokens that can be revoked immediately
    ///   (JWT tokens can't be revoked until they expire unless you maintain a blacklist)
    /// - Don't use for long-lived sessions without refresh token strategy
    /// - Avoid if your requirements need server-side session management
    /// 
    /// SECURITY CONSIDERATIONS:
    /// - Secret key must be kept secure (use environment variables or Key Vault)
    /// - Always use HTTPS to prevent token interception
    /// - Keep token expiration times short (15-60 minutes)
    /// - Implement refresh tokens for better user experience
    /// - Consider token blacklisting for logout functionality
    /// </summary>
    public class JwtTokenService : IJwtTokenService
    {
        private readonly JwtSettings _jwtSettings;

        public JwtTokenService(Microsoft.Extensions.Options.IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        /// <summary>
        /// Generates a JWT token with user claims
        /// 
        /// TOKEN STRUCTURE:
        /// - Header: Algorithm (HS256) and token type (JWT)
        /// - Payload: User claims (sub, name, roles, custom claims)
        /// - Signature: HMACSHA256(header + payload, secret key)
        /// 
        /// WORKFLOW:
        /// 1. Create security key from secret
        /// 2. Define signing credentials with HS256 algorithm
        /// 3. Build list of claims from user data
        /// 4. Create token descriptor with claims, expiration, issuer, audience
        /// 5. Generate and return token string
        /// </summary>
        public string GenerateToken(UserClaims userClaims)
        {
            // Create symmetric security key from the secret key
            // This key is used to sign the token
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            
            // Define signing credentials with HMAC-SHA256 algorithm
            // This ensures token integrity - any modification will invalidate the signature
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Build claims list - these are statements about the user
            var claims = new List<Claim>
            {
                // Standard JWT claims (defined in RFC 7519)
                new Claim(JwtRegisteredClaimNames.Sub, userClaims.UserId),        // Subject (user ID)
                new Claim(JwtRegisteredClaimNames.Name, userClaims.Username),     // User's name
                new Claim(JwtRegisteredClaimNames.Email, userClaims.Email),       // User's email
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // JWT ID (unique token identifier)
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()) // Issued at
            };

            // Add role claims for authorization
            // Each role is added as a separate claim of type "role"
            // Used with [Authorize(Roles = "Admin")] attributes
            foreach (var role in userClaims.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Add custom claims for application-specific data
            // These can be any additional user attributes you need
            foreach (var customClaim in userClaims.CustomClaims)
            {
                claims.Add(new Claim(customClaim.Key, customClaim.Value));
            }

            // Create token descriptor with all required information
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = credentials
            };

            // Create token handler and generate the actual token
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Return token as string
            // Client will include this in Authorization header: "Bearer {token}"
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Validates a JWT token and returns claims if valid
        /// 
        /// VALIDATION CHECKS:
        /// - Signature verification (token hasn't been tampered with)
        /// - Expiration check (token hasn't expired)
        /// - Issuer validation (token was created by your application)
        /// - Audience validation (token is intended for your application)
        /// 
        /// NOTE: In most cases, you don't need to call this manually
        /// ASP.NET Core's JWT middleware handles validation automatically
        /// This method is useful for custom validation scenarios
        /// </summary>
        public ClaimsPrincipal? ValidateToken(string token)
        {
            // Create symmetric security key for signature verification
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));

            // Define validation parameters
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = securityKey,
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                ValidateLifetime = true, // Check token expiration
                ClockSkew = TimeSpan.Zero // No tolerance for expired tokens
            };

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                return principal;
            }
            catch
            {
                // Token validation failed (expired, invalid signature, etc.)
                return null;
            }
        }
    }
}
