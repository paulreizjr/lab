namespace jwt.Models
{
    /// <summary>
    /// User Claims Model
    /// 
    /// PURPOSE:
    /// - Represents user information to be embedded in JWT token claims
    /// - Claims are statements about the user (id, name, roles, etc.)
    /// - Used during token generation to create the JWT payload
    /// 
    /// SCENARIOS TO USE:
    /// - When generating JWT tokens with user-specific information
    /// - To store user identity and authorization data in the token
    /// - For stateless authentication (no need to query database on each request)
    /// - When you need to pass user context across microservices
    /// 
    /// SCENARIOS NOT TO USE:
    /// - Don't store sensitive data (passwords, credit cards, SSNs)
    /// - Don't store large amounts of data (tokens should be small)
    /// - Don't store data that changes frequently (token can't be updated)
    /// - Don't rely solely on claims for authorization in high-security scenarios
    ///   (always verify critical operations against the database)
    /// </summary>
    public class UserClaims
    {
        /// <summary>
        /// Unique user identifier (typically from database)
        /// Maps to "sub" (subject) claim in JWT standard
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// User's username or display name
        /// Maps to "name" claim in JWT standard
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// User's email address
        /// Maps to "email" claim in JWT standard
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// User's roles for role-based authorization
        /// Multiple roles can be added as separate claims
        /// Used with [Authorize(Roles = "Admin")] attributes
        /// </summary>
        public List<string> Roles { get; set; } = new();

        /// <summary>
        /// Optional: Additional custom claims
        /// Use for application-specific data like:
        /// - Tenant ID (multi-tenant applications)
        /// - Department or organization
        /// - Feature flags or permissions
        /// - Subscription level
        /// </summary>
        public Dictionary<string, string> CustomClaims { get; set; } = new();
    }
}
