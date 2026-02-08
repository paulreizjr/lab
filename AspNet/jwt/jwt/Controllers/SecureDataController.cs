using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace jwt.Controllers
{
    /// <summary>
    /// Protected API Controller - Demonstrates JWT Authorization
    /// 
    /// PURPOSE:
    /// - Shows how to protect API endpoints with JWT authentication
    /// - Demonstrates role-based authorization
    /// - Shows how to access user claims from authenticated requests
    /// - Provides examples of different authorization scenarios
    /// 
    /// AUTHORIZATION STRATEGIES DEMONSTRATED:
    /// 1. [Authorize] - Requires valid JWT token (any authenticated user)
    /// 2. [Authorize(Roles = "Admin")] - Requires specific role(s)
    /// 3. [AllowAnonymous] - Explicitly allows unauthenticated access
    /// 4. Policy-based authorization - Custom authorization logic
    /// 
    /// SCENARIOS TO USE JWT AUTHORIZATION:
    /// - REST APIs for mobile applications
    /// - Single Page Applications (React, Angular, Vue)
    /// - Microservices communication (service-to-service)
    /// - Third-party API integrations
    /// - Cross-platform applications requiring consistent auth
    /// - APIs with multiple client types (web, mobile, desktop)
    /// - Stateless distributed systems
    /// - APIs requiring scalability (no server-side session state)
    /// 
    /// SCENARIOS NOT TO USE:
    /// - Traditional server-rendered MVC applications (use cookie auth)
    /// - When you need immediate token revocation (logout, password change)
    /// - Applications requiring real-time session control
    /// - When all clients are same-origin (cookie auth is simpler)
    /// - High-security scenarios requiring audit of every request
    /// - When compliance requires server-side session tracking
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SecureDataController : ControllerBase
    {
        private readonly ILogger<SecureDataController> _logger;

        public SecureDataController(ILogger<SecureDataController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Public endpoint - No authentication required
        /// 
        /// PURPOSE:
        /// - Demonstrates endpoint accessible to everyone
        /// - Useful for public data, health checks, documentation
        /// 
        /// USAGE:
        /// GET /api/securedata/public
        /// No Authorization header needed
        /// 
        /// WHEN TO USE:
        /// - Public API documentation or metadata
        /// - Health check endpoints
        /// - Public product catalogs
        /// - Marketing content APIs
        /// - CORS preflight endpoints
        /// </summary>
        [HttpGet("public")]
        [AllowAnonymous] // Explicitly mark as public (not required if no [Authorize] on controller)
        public IActionResult GetPublicData()
        {
            return Ok(new
            {
                message = "This is public data - no authentication required",
                timestamp = DateTime.UtcNow,
                accessLevel = "Public"
            });
        }

        /// <summary>
        /// Protected endpoint - Requires valid JWT token
        /// 
        /// PURPOSE:
        /// - Demonstrates basic JWT authentication
        /// - Shows how to access user claims
        /// - Returns personalized data for authenticated user
        /// 
        /// USAGE:
        /// GET /api/securedata/protected
        /// Headers: Authorization: Bearer {your-jwt-token}
        /// 
        /// AUTHENTICATION FLOW:
        /// 1. Client includes JWT token in Authorization header
        /// 2. ASP.NET Core middleware validates token signature
        /// 3. Middleware checks token expiration
        /// 4. Middleware validates issuer and audience
        /// 5. If valid, populates User.Claims with token claims
        /// 6. Controller action executes
        /// 7. Returns 401 Unauthorized if token invalid/missing
        /// 
        /// WHEN TO USE:
        /// - User-specific data endpoints
        /// - Profile management APIs
        /// - Personal dashboards
        /// - User settings
        /// - Any endpoint requiring user identification
        /// </summary>
        [HttpGet("protected")]
        [Authorize] // Requires valid JWT token
        public IActionResult GetProtectedData()
        {
            // Extract user information from JWT claims
            // These claims were embedded in the token when it was generated
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            _logger.LogInformation("Protected endpoint accessed by user: {Username}", username);

            return Ok(new
            {
                message = "This data is only accessible with valid JWT token",
                userId = userId,
                username = username,
                email = email,
                accessLevel = "Authenticated User",
                timestamp = DateTime.UtcNow
            });
        }

        /// <summary>
        /// Admin-only endpoint - Requires JWT token with Admin role
        /// 
        /// PURPOSE:
        /// - Demonstrates role-based authorization
        /// - Restricts access to users with specific roles
        /// - Shows how to implement hierarchical access control
        /// 
        /// USAGE:
        /// GET /api/securedata/admin
        /// Headers: Authorization: Bearer {admin-jwt-token}
        /// 
        /// AUTHORIZATION FLOW:
        /// 1. Validates JWT token (same as [Authorize])
        /// 2. Checks if token contains role claim with value "Admin"
        /// 3. Returns 403 Forbidden if user doesn't have required role
        /// 4. Returns 401 Unauthorized if token is invalid/missing
        /// 
        /// ROLE-BASED AUTHORIZATION SCENARIOS:
        /// - Administrative functions (user management, system config)
        /// - Content moderation tools
        /// - Financial operations (refunds, adjustments)
        /// - System monitoring and analytics
        /// - Data export and reporting
        /// 
        /// ALTERNATIVE SYNTAX:
        /// [Authorize(Roles = "Admin,SuperAdmin")] // OR logic - either role works
        /// [Authorize(Roles = "Admin")]            // AND logic (combine multiple attributes)
        /// [Authorize(Roles = "Manager")]          // User needs both Admin AND Manager
        /// </summary>
        [HttpGet("admin")]
        [Authorize(Roles = "Admin")] // Requires Admin role claim in JWT
        public IActionResult GetAdminData()
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

            _logger.LogInformation("Admin endpoint accessed by user: {Username}", username);

            return Ok(new
            {
                message = "This data is only accessible to administrators",
                username = username,
                roles = roles,
                accessLevel = "Administrator",
                timestamp = DateTime.UtcNow,
                sensitiveData = "User management, system configuration, etc."
            });
        }

        /// <summary>
        /// Manager or Admin endpoint - Multiple roles allowed
        /// 
        /// PURPOSE:
        /// - Shows how to allow multiple roles (OR logic)
        /// - Useful for hierarchical permissions
        /// 
        /// USAGE:
        /// Accepts users with either "Manager" OR "Admin" role
        /// </summary>
        [HttpGet("management")]
        [Authorize(Roles = "Manager,Admin")] // Comma-separated = OR logic
        public IActionResult GetManagementData()
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

            return Ok(new
            {
                message = "Accessible to managers and administrators",
                username = username,
                roles = roles,
                accessLevel = "Management",
                features = new[] { "Reports", "Team Management", "Approvals" }
            });
        }

        /// <summary>
        /// Custom claims example - Access application-specific claims
        /// 
        /// PURPOSE:
        /// - Demonstrates accessing custom claims from JWT
        /// - Shows how to use claims for business logic
        /// - Useful for multi-tenant applications
        /// 
        /// CUSTOM CLAIMS SCENARIOS:
        /// - Tenant ID (multi-tenant SaaS applications)
        /// - Department or organization
        /// - Subscription level or plan
        /// - Feature flags or entitlements
        /// - Geographic region or timezone
        /// - Language preference
        /// - Custom permissions beyond roles
        /// </summary>
        [HttpGet("custom-claims")]
        [Authorize]
        public IActionResult GetCustomClaimsData()
        {
            // Access custom claims that were added during token generation
            var department = User.FindFirst("department")?.Value;
            var employeeId = User.FindFirst("employeeId")?.Value;

            // Get all claims for debugging
            var allClaims = User.Claims.Select(c => new
            {
                type = c.Type,
                value = c.Value
            }).ToList();

            return Ok(new
            {
                message = "Custom claims from JWT token",
                department = department,
                employeeId = employeeId,
                allClaims = allClaims
            });
        }

        /// <summary>
        /// User profile endpoint - Return current user information
        /// 
        /// PURPOSE:
        /// - Common pattern for "Get Current User" endpoints
        /// - Returns user information from JWT claims
        /// - No database query needed (stateless)
        /// 
        /// USAGE PATTERN:
        /// Client calls this after login to get user profile
        /// Useful for populating UI with user information
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        public IActionResult GetCurrentUser()
        {
            return Ok(new
            {
                userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                username = User.FindFirst(ClaimTypes.Name)?.Value,
                email = User.FindFirst(ClaimTypes.Email)?.Value,
                roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList(),
                isAuthenticated = User.Identity?.IsAuthenticated ?? false,
                authenticationType = User.Identity?.AuthenticationType
            });
        }

        /// <summary>
        /// Example: Update user data endpoint
        /// 
        /// PURPOSE:
        /// - Shows POST request with authorization
        /// - Demonstrates accessing user ID for data operations
        /// 
        /// COMMON PATTERN:
        /// - User can only update their own data
        /// - Use user ID from token claims to ensure data isolation
        /// - Prevents users from accessing other users' data
        /// </summary>
        [HttpPost("update-profile")]
        [Authorize]
        public IActionResult UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            // Get user ID from JWT claims
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // In production:
            // 1. Validate user owns the resource they're trying to update
            // 2. Update database with user ID from claims (not from request body!)
            // 3. Never trust user-supplied IDs - always use claims

            _logger.LogInformation("User {UserId} updating profile", userId);

            return Ok(new
            {
                message = "Profile updated successfully",
                userId = userId,
                updatedFields = request
            });
        }

        /// <summary>
        /// Example: Resource authorization - Check ownership
        /// 
        /// PURPOSE:
        /// - Demonstrates checking resource ownership
        /// - Shows pattern for user-specific data access
        /// 
        /// SECURITY PATTERN:
        /// - Always verify user owns the resource
        /// - Use user ID from JWT claims, not request parameters
        /// - Return 403 Forbidden if user doesn't own resource
        /// - Return 404 Not Found to hide existence of resources
        /// </summary>
        [HttpGet("document/{documentId}")]
        [Authorize]
        public IActionResult GetDocument(int documentId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // In production:
            // 1. Query database for document
            // 2. Check if document.OwnerId == userId
            // 3. Return 403 if user doesn't own document
            // 4. Return 404 if document doesn't exist

            // Mock ownership check
            bool userOwnsDocument = true; // Replace with actual database check

            if (!userOwnsDocument)
            {
                _logger.LogWarning("User {UserId} attempted to access document {DocumentId} without permission", userId, documentId);
                return Forbid(); // 403 Forbidden
            }

            return Ok(new
            {
                documentId = documentId,
                ownerId = userId,
                message = "Document data (only accessible to owner)"
            });
        }
    }

    /// <summary>
    /// Example request model for profile updates
    /// </summary>
    public class UpdateProfileRequest
    {
        public string? DisplayName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Bio { get; set; }
    }
}
