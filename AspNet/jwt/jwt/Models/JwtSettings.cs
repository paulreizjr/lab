namespace jwt.Models
{
    /// <summary>
    /// JWT Configuration Settings Model
    /// 
    /// PURPOSE:
    /// - Strongly-typed configuration class for JWT authentication settings
    /// - Maps to appsettings.json JwtSettings section
    /// - Provides type-safe access to JWT configuration values
    /// 
    /// SCENARIOS TO USE:
    /// - When you need to inject JWT settings into services
    /// - For dependency injection pattern with IOptions<JwtSettings>
    /// - When centralizing JWT configuration in one place
    /// - For easier testing and configuration management
    /// 
    /// SCENARIOS NOT TO USE:
    /// - Don't hardcode values directly in this class (use appsettings.json instead)
    /// - Don't use for storing actual tokens (this is for configuration only)
    /// - Don't expose this class directly in API responses (security risk)
    /// </summary>
    public class JwtSettings
    {
        /// <summary>
        /// Secret key used for signing JWT tokens
        /// IMPORTANT: Should be at least 256 bits (32 characters) for HS256 algorithm
        /// Store in environment variables or Azure Key Vault in production
        /// </summary>
        public string SecretKey { get; set; } = string.Empty;

        /// <summary>
        /// Token issuer - identifies who created the token
        /// Usually your application name or domain
        /// </summary>
        public string Issuer { get; set; } = string.Empty;

        /// <summary>
        /// Token audience - identifies who the token is intended for
        /// Usually your API consumers or application users
        /// </summary>
        public string Audience { get; set; } = string.Empty;

        /// <summary>
        /// Token expiration time in minutes
        /// Typical values: 15-60 minutes for access tokens, 7-30 days for refresh tokens
        /// Shorter is more secure but requires more frequent re-authentication
        /// </summary>
        public int ExpirationInMinutes { get; set; }
    }
}
