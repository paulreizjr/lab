using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Collections.Concurrent;

namespace ValidationAttributeExamples
{
    // ============================================================================
    // ValidationAttribute Abstract Class - Comprehensive Examples
    // ============================================================================
    
    /*
     * PURPOSE:
     * ValidationAttribute is an abstract base class in System.ComponentModel.DataAnnotations
     * that provides a framework for creating custom validation logic for model properties.
     * It enables declarative validation through attributes, promoting clean separation of concerns.
     * 
     * KEY BENEFITS:
     * - Declarative validation (metadata-driven)
     * - Reusable validation logic
     * - Integration with ASP.NET MVC, Entity Framework, and other frameworks
     * - Consistent validation patterns across applications
     * - Support for client-side validation (in web applications)
     */
    
    // ============================================================================
    // SCENARIOS TO USE ValidationAttribute:
    // ============================================================================
    /*
     * 1. Model validation in web applications (ASP.NET MVC/Core)
     * 2. Entity validation in Entity Framework
     * 3. Data Transfer Object (DTO) validation
     * 4. Configuration object validation
     * 5. Simple, stateless validation rules
     * 6. Cross-cutting validation concerns
     * 7. When you need framework integration (automatic validation)
     */
    
    // ============================================================================
    // SCENARIOS NOT TO USE ValidationAttribute:
    // ============================================================================
    /*
     * 1. Complex business logic validation (use domain services instead)
     * 2. Validation requiring database queries or external services
     * 3. Stateful validation logic
     * 4. Validation with complex dependencies between multiple properties
     * 5. Performance-critical scenarios requiring highly optimized validation
     * 6. When validation logic changes frequently (prefer strategy pattern)
     * 7. Async validation scenarios (ValidationAttribute is synchronous)
     */

    // ============================================================================
    // CUSTOM VALIDATION ATTRIBUTE EXAMPLES
    // ============================================================================

    /// <summary>
    /// Custom validation attribute for validating age ranges
    /// MEMORY ALLOCATION: Minimal - only stores min/max values as fields
    /// THREAD SAFETY: Safe - immutable after construction, no shared state
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class AgeRangeAttribute : ValidationAttribute
    {
        private readonly int _minAge;
        private readonly int _maxAge;
        
        // MEMORY ALLOCATION: Constructor parameters are stored as readonly fields
        // This minimizes memory footprint and ensures immutability
        public AgeRangeAttribute(int minAge, int maxAge)
        {
            _minAge = minAge;
            _maxAge = maxAge;
            
            // Pre-format error message to avoid string allocations during validation
            ErrorMessage = $"Age must be between {_minAge} and {_maxAge} years.";
        }

        /// <summary>
        /// Core validation logic - called by validation frameworks
        /// THREAD SAFETY: Safe - no shared mutable state, parameters are method-local
        /// MEMORY ALLOCATION: Minimal - avoid unnecessary object creation in hot path
        /// </summary>
        public override bool IsValid(object? value)
        {
            // Handle null values (typically handled by [Required] if needed)
            if (value == null) return true;
            
            // MEMORY ALLOCATION: Direct casting avoids boxing/unboxing when possible
            if (value is int age)
            {
                return age >= _minAge && age <= _maxAge;
            }
            
            // Handle string conversion (common in web scenarios)
            if (value is string stringValue && int.TryParse(stringValue, out int parsedAge))
            {
                return parsedAge >= _minAge && parsedAge <= _maxAge;
            }
            
            return false; // Invalid type
        }

        /// <summary>
        /// Provides detailed validation context for better error reporting
        /// MEMORY ALLOCATION: Called only on validation failure, so allocation is acceptable
        /// </summary>
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (IsValid(value))
                return ValidationResult.Success;

            // MEMORY ALLOCATION: Only allocate detailed error message on failure
            string memberName = validationContext.MemberName ?? "Unknown";
            string detailedError = $"The field '{memberName}' {ErrorMessage}";
            
            return new ValidationResult(detailedError, new[] { memberName });
        }
    }

    /// <summary>
    /// Advanced validation attribute with caching for performance
    /// MEMORY ALLOCATION: Uses concurrent dictionary for caching validation results
    /// THREAD SAFETY: Uses ConcurrentDictionary for thread-safe caching
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class CachedEmailValidationAttribute : ValidationAttribute
    {
        // THREAD SAFETY: ConcurrentDictionary provides thread-safe operations
        // MEMORY ALLOCATION: Cache grows over time - consider implementing eviction policy for long-running apps
        private static readonly ConcurrentDictionary<string, bool> ValidationCache = new();
        private static readonly object CacheLock = new();
        private const int MaxCacheSize = 1000; // Prevent unbounded growth

        public override bool IsValid(object? value)
        {
            if (value == null) return true;
            
            string email = value.ToString()?.ToLowerInvariant() ?? string.Empty;
            
            // THREAD SAFETY: ConcurrentDictionary.GetOrAdd is atomic
            // MEMORY ALLOCATION: Reuses cached results to avoid repeated regex operations
            return ValidationCache.GetOrAdd(email, emailKey =>
            {
                // MEMORY ALLOCATION: Manage cache size to prevent memory leaks
                ManageCacheSize();
                
                // Simple email validation (in production, use more robust validation)
                return !string.IsNullOrWhiteSpace(emailKey) && 
                       emailKey.Contains('@') && 
                       emailKey.Contains('.');
            });
        }

        /// <summary>
        /// Prevents unbounded cache growth in long-running applications
        /// THREAD SAFETY: Uses lock for cache management operations
        /// </summary>
        private static void ManageCacheSize()
        {
            if (ValidationCache.Count <= MaxCacheSize) return;
            
            // THREAD SAFETY: Lock prevents concurrent cache clearing
            lock (CacheLock)
            {
                if (ValidationCache.Count > MaxCacheSize)
                {
                    ValidationCache.Clear(); // Simple eviction strategy
                }
            }
        }
    }

    // ============================================================================
    // EXAMPLE MODELS USING VALIDATION ATTRIBUTES
    // ============================================================================

    /// <summary>
    /// Example model demonstrating various validation scenarios
    /// MEMORY ALLOCATION: Validation attributes are metadata - no runtime allocation
    /// until validation is performed
    /// </summary>
    public class Person
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 50 characters")]
        public string Name { get; set; } = string.Empty;

        [AgeRange(18, 120)] // Custom validation attribute
        public int Age { get; set; }

        [CachedEmailValidation] // Custom validation with caching
        public string Email { get; set; } = string.Empty;

        [Range(0, double.MaxValue, ErrorMessage = "Salary must be positive")]
        public decimal Salary { get; set; }

        [RegularExpression(@"^\d{3}-\d{2}-\d{4}$", ErrorMessage = "SSN must be in format XXX-XX-XXXX")]
        public string? SSN { get; set; }
    }

    /// <summary>
    /// Example demonstrating validation in different threading scenarios
    /// </summary>
    public class ValidationExamples
    {
        /// <summary>
        /// Demonstrates basic validation usage
        /// MEMORY ALLOCATION: ValidationContext and ValidationResult objects allocated per validation
        /// THREAD SAFETY: Safe - each validation creates its own context
        /// </summary>
        public static List<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model);
            
            // MEMORY ALLOCATION: Validator.TryValidateObject allocates validation results
            Validator.TryValidateObject(model, validationContext, validationResults, validateAllProperties: true);
            
            return validationResults;
        }

        /// <summary>
        /// Demonstrates property-level validation
        /// MEMORY ALLOCATION: More efficient than full object validation when validating single properties
        /// </summary>
        public static bool ValidateProperty(object model, string propertyName, object? value)
        {
            var validationContext = new ValidationContext(model) { MemberName = propertyName };
            var validationResults = new List<ValidationResult>();
            
            return Validator.TryValidateProperty(value, validationContext, validationResults);
        }

        /// <summary>
        /// Thread-safe validation demonstration
        /// THREAD SAFETY: ValidationAttribute instances are shared but validation logic should be stateless
        /// MEMORY ALLOCATION: Each thread creates its own ValidationContext and results
        /// </summary>
        public static async Task<Dictionary<int, List<ValidationResult>>> ValidateMultipleModelsAsync(
            IEnumerable<Person> people)
        {
            var results = new ConcurrentDictionary<int, List<ValidationResult>>();
            
            // THREAD SAFETY: Parallel processing is safe because validation attributes are stateless
            await Task.Run(() =>
            {
                Parallel.ForEach(people.Select((person, index) => new { person, index }), item =>
                {
                    // MEMORY ALLOCATION: Each parallel operation allocates its own validation context
                    var validationResults = ValidateModel(item.person);
                    results.TryAdd(item.index, validationResults);
                });
            });
            
            return new Dictionary<int, List<ValidationResult>>(results);
        }
    }

    // ============================================================================
    // PERFORMANCE CONSIDERATIONS AND EXAMPLES
    // ============================================================================

    /// <summary>
    /// Performance-optimized validation attribute
    /// MEMORY ALLOCATION: Minimizes allocations in the hot path
    /// THREAD SAFETY: Stateless design ensures thread safety
    /// </summary>
    public class OptimizedStringLengthAttribute : ValidationAttribute
    {
        private readonly int _maxLength;
        private readonly int _minLength;

        public OptimizedStringLengthAttribute(int maxLength, int minLength = 0)
        {
            _maxLength = maxLength;
            _minLength = minLength;
        }

        public override bool IsValid(object? value)
        {
            // MEMORY ALLOCATION: Avoid string allocations in validation path
            if (value == null) return true;
            
            // Direct length check without string conversion when possible
            switch (value)
            {
                case string strValue:
                    int length = strValue.Length;
                    return length >= _minLength && length <= _maxLength;
                
                case char[] charArray:
                    int arrayLength = charArray.Length;
                    return arrayLength >= _minLength && arrayLength <= _maxLength;
                
                default:
                    // Fallback to string conversion only when necessary
                    string stringValue = value.ToString() ?? string.Empty;
                    return stringValue.Length >= _minLength && stringValue.Length <= _maxLength;
            }
        }
    }

    // ============================================================================
    // MAIN PROGRAM - DEMONSTRATION
    // ============================================================================

    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("=== ValidationAttribute Examples ===\n");

            // Example 1: Basic validation
            Console.WriteLine("1. Basic Validation Example:");
            await DemonstrateBasicValidation();

            Console.WriteLine("\n" + new string('=', 50) + "\n");

            // Example 2: Thread safety demonstration
            Console.WriteLine("2. Multi-threaded Validation Example:");
            await DemonstrateThreadSafeValidation();

            Console.WriteLine("\n" + new string('=', 50) + "\n");

            // Example 3: Performance considerations
            Console.WriteLine("3. Performance Considerations:");
            DemonstratePerformanceConsiderations();

            Console.WriteLine("\n" + new string('=', 50) + "\n");

            // Example 4: Memory allocation patterns
            Console.WriteLine("4. Memory Allocation Analysis:");
            DemonstrateMemoryAllocationPatterns();
        }

        private static async Task DemonstrateBasicValidation()
        {
            var people = new[]
            {
                new Person { Name = "John Doe", Age = 30, Email = "john@example.com", Salary = 50000 },
                new Person { Name = "", Age = 17, Email = "invalid-email", Salary = -1000 }, // Invalid
                new Person { Name = "Jane Smith", Age = 25, Email = "jane@example.com", Salary = 60000 }
            };

            foreach (var (person, index) in people.Select((p, i) => (p, i)))
            {
                var results = ValidationExamples.ValidateModel(person);
                Console.WriteLine($"Person {index + 1}: {(results.Count == 0 ? "Valid" : "Invalid")}");
                
                if (results.Count > 0)
                {
                    foreach (var result in results)
                    {
                        Console.WriteLine($"  - {result.ErrorMessage}");
                    }
                }
            }
        }

        private static async Task DemonstrateThreadSafeValidation()
        {
            // Create test data
            var people = Enumerable.Range(1, 100)
                .Select(i => new Person
                {
                    Name = $"Person {i}",
                    Age = 20 + (i % 50),
                    Email = $"person{i}@example.com",
                    Salary = 30000 + (i * 1000)
                })
                .ToList();

            // Add some invalid records
            people.AddRange(new[]
            {
                new Person { Name = "", Age = 16, Email = "invalid", Salary = -100 },
                new Person { Name = "Test", Age = 150, Email = "test@", Salary = 50000 }
            });

            Console.WriteLine($"Validating {people.Count} records in parallel...");
            
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var results = await ValidationExamples.ValidateMultipleModelsAsync(people);
            stopwatch.Stop();

            int validCount = results.Count(r => r.Value.Count == 0);
            int invalidCount = results.Count - validCount;

            Console.WriteLine($"Completed in {stopwatch.ElapsedMilliseconds}ms");
            Console.WriteLine($"Valid records: {validCount}");
            Console.WriteLine($"Invalid records: {invalidCount}");
        }

        private static void DemonstratePerformanceConsiderations()
        {
            Console.WriteLine("Performance Tips:");
            Console.WriteLine("• Validation attributes should be stateless for thread safety");
            Console.WriteLine("• Cache expensive validation results when appropriate");
            Console.WriteLine("• Avoid allocations in the hot validation path");
            Console.WriteLine("• Use property-level validation for incremental validation");
            Console.WriteLine("• Consider validation result pooling for high-throughput scenarios");
            
            // Demonstrate property-level validation performance
            var person = new Person { Name = "Test User", Age = 25, Email = "test@example.com" };
            
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < 10000; i++)
            {
                ValidationExamples.ValidateProperty(person, nameof(Person.Email), person.Email);
            }
            stopwatch.Stop();
            
            Console.WriteLine($"10,000 property validations completed in {stopwatch.ElapsedMilliseconds}ms");
        }

        private static void DemonstrateMemoryAllocationPatterns()
        {
            Console.WriteLine("Memory Allocation Patterns:");
            Console.WriteLine("• ValidationAttribute instances are created once as metadata");
            Console.WriteLine("• ValidationContext is allocated per validation operation");
            Console.WriteLine("• ValidationResult objects are allocated for each error");
            Console.WriteLine("• Consider object pooling for high-frequency validation");
            Console.WriteLine("• Cache validation results for expensive operations");
            Console.WriteLine("• Use string interning for common error messages");
            
            // Demonstrate the difference in allocations
            var person = new Person();
            
            // Method 1: Full object validation (more allocations)
            Console.WriteLine("\nFull object validation allocates:");
            Console.WriteLine("- 1 ValidationContext");
            Console.WriteLine("- Multiple ValidationResult objects (one per error)");
            Console.WriteLine("- Reflection metadata access");
            
            // Method 2: Property validation (fewer allocations)  
            Console.WriteLine("\nProperty-level validation allocates:");
            Console.WriteLine("- 1 ValidationContext per property");
            Console.WriteLine("- ValidationResult objects only for errors");
            Console.WriteLine("- Direct property access (less reflection)");
        }
    }
}
