namespace jwt.Models
{
    /// <summary>
    /// Login Response Model
    /// 
    /// PURPOSE:
    /// - Data Transfer Object (DTO) for successful authentication responses
    /// - Returns JWT token and user information to the client
    /// - Provides a consistent response structure for login endpoints
    /// 
    /// SCENARIOS TO USE:
    /// - Returning tokens after successful authentication
    /// - When you need to send user data along with the token
    /// - In login/register endpoints that generate JWT tokens
    /// - For mobile apps or SPAs that need to store tokens client-side
    /// 
    /// SCENARIOS NOT TO USE:
    /// - Don't use for server-rendered applications using cookie-based auth
    /// - Don't include sensitive information (passwords, SSNs, etc.)
    /// - Don't use if implementing OAuth (use OAuth-specific response models)
    /// - Don't send in insecure connections (always require HTTPS)
    /// </summary>
    public class LoginResponse
    {
        /// <summary>
        /// JWT access token - the main authentication credential
        /// Client should include this in Authorization header: "Bearer {token}"
        /// Store securely on client-side (not in localStorage if XSS is a concern)
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// Token expiration date/time in UTC
        /// Client can use this to know when to refresh the token
        /// </summary>
        public DateTime Expiration { get; set; }

        /// <summary>
        /// Authenticated user's username
        /// Useful for displaying user info in the UI
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Optional: User's email address
        /// Include if needed by your application
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Optional: User's roles for frontend authorization logic
        /// Helps client-side apps show/hide UI elements based on permissions
        /// Note: Never rely on client-side role checks for security
        /// </summary>
        public List<string> Roles { get; set; } = new();
    }
}
