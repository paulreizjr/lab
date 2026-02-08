using System.ComponentModel.DataAnnotations;

namespace jwt.Models
{
    /// <summary>
    /// Login Request Model
    /// 
    /// PURPOSE:
    /// - Data Transfer Object (DTO) for user login requests
    /// - Accepts username and password from client
    /// - Provides validation for required fields
    /// 
    /// SCENARIOS TO USE:
    /// - POST endpoints that handle user authentication
    /// - When you need to validate login credentials
    /// - As the request body for /api/auth/login endpoints
    /// - When implementing username/password authentication flow
    /// 
    /// SCENARIOS NOT TO USE:
    /// - Don't use for OAuth/OpenID Connect flows (use different models)
    /// - Don't use for passwordless authentication (magic links, WebAuthn)
    /// - Don't use for token refresh (use separate RefreshTokenRequest model)
    /// - Don't store this object after authentication (contains plaintext password)
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// User's username or email
        /// Marked as required - validation will fail if not provided
        /// </summary>
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// User's password in plaintext
        /// SECURITY NOTE: Never log this value, never store unhashed
        /// Always transmitted over HTTPS only
        /// </summary>
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;
    }
}
