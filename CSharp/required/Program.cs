/*
 * C# 'required' Keyword Examples
 * 
 * PURPOSE:
 * The 'required' keyword (introduced in C# 11) enforces that certain properties 
 * or fields must be initialized during object creation, either through:
 * - Object initializer syntax
 * - Constructor parameters
 * - Property assignment in constructors
 * 
 * This provides compile-time safety for ensuring critical properties are never left uninitialized.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace RequiredExamples
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== C# 'required' Keyword Examples ===\n");
            
            // Run all examples
            BasicRequiredExample();
            RequiredWithConstructorExample();
            RequiredWithInheritanceExample();
            RequiredWithRecordsExample();
            RequiredVsNullableExample();
            MemoryAllocationExample();
            WhenToUseAndNotUseExamples();
            
            Console.WriteLine("\n=== End of Examples ===");
        }

        #region Basic Required Example
        
        /*
         * BASIC REQUIRED USAGE
         * 
         * SCENARIO TO USE: When you have properties that are essential for object validity
         * and should never be null or uninitialized after construction.
         */
        static void BasicRequiredExample()
        {
            Console.WriteLine("1. Basic Required Example:");
            
            // ✅ Valid - all required properties are initialized
            var person = new Person
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@email.com"
                // Age is optional, so it can be omitted
            };
            
            Console.WriteLine($"Created person: {person.FirstName} {person.LastName}, Email: {person.Email}, Age: {person.Age}");
            
            /* 
             * ❌ The following would cause COMPILE-TIME ERROR:
             * var invalidPerson = new Person
             * {
             *     FirstName = "John"
             *     // Missing required LastName and Email - compiler error!
             * };
             */
            
            Console.WriteLine();
        }
        
        public class Person
        {
            // Required properties - MUST be initialized during object creation
            public required string FirstName { get; set; }
            public required string LastName { get; set; }
            public required string Email { get; set; }
            
            // Optional property - can be left with default value
            public int Age { get; set; } = 0;
            
            /*
             * MEMORY ALLOCATION NOTE:
             * - 'required' doesn't change memory allocation patterns
             * - String properties still allocate on the heap
             * - The object itself is allocated on the heap
             * - 'required' only affects compile-time checking, not runtime memory layout
             */
        }
        
        #endregion

        #region Required with Constructor Example
        
        /*
         * REQUIRED WITH CONSTRUCTORS
         * 
         * SCENARIO TO USE: When you want to combine constructor parameters with 
         * required properties for flexible initialization options.
         */
        static void RequiredWithConstructorExample()
        {
            Console.WriteLine("2. Required with Constructor Example:");
            
            // Option 1: Use constructor that satisfies all required properties
            var product1 = new Product("Laptop", 999.99m, "Electronics");
            Console.WriteLine($"Product 1: {product1.Name} - ${product1.Price}, Category: {product1.Category ?? "None"}");
            
            // Option 2: Use object initializer with parameterless constructor
            var product2 = new Product
            {
                Name = "Mouse",
                Price = 29.99m,
                Category = "Accessories"
            };
            Console.WriteLine($"Product 2: {product2.Name} - ${product2.Price}, Category: {product2.Category}");
            
            // Option 3: Pure object initializer
            var product3 = new Product
            {
                Name = "Keyboard",
                Price = 79.99m,
                Category = "Electronics"
            };
            Console.WriteLine($"Product 3: {product3.Name} - ${product3.Price}, Category: {product3.Category}");
            
            Console.WriteLine();
        }
        
        public class Product
        {
            public required string Name { get; set; }
            public required decimal Price { get; set; }
            public required string? Category { get; set; } // Can be null, but must be explicitly set
            
            // Parameterless constructor for object initializer syntax
            public Product() { }
            
            /*
             * IMPORTANT CONSTRUCTOR NOTES:
             * 
             * 1. [SetsRequiredMembers] attribute tells the compiler that this constructor
             *    initializes all required members, so object initializer is not needed
             * 
             * 2. Without [SetsRequiredMembers], you cannot use constructors with object
             *    initializers unless the constructor sets ALL required members
             * 
             * 3. The compiler enforces that all required members are initialized either:
             *    - By a constructor marked with [SetsRequiredMembers], OR
             *    - Through object initializer syntax
             */
            
            // Constructor that satisfies some required properties
            // NOTE: This constructor cannot be used with object initializers
            public Product(string name, decimal price)
            {
                Name = name;
                Price = price;
                // Category is still required - this constructor can only be used
                // if you also provide a way to set Category (like another constructor)
            }
            
            // Constructor that satisfies all required properties
            [SetsRequiredMembers]
            public Product(string name, decimal price, string? category)
            {
                Name = name;
                Price = price;
                Category = category;
            }
            
            /*
             * MEMORY ALLOCATION NOTE:
             * - Constructor parameters are stack-allocated during the call
             * - String properties (Name, Category) are heap-allocated
             * - Decimal (Price) is a value type, stored inline within the object
             */
        }
        
        #endregion

        #region Required with Inheritance
        
        /*
         * REQUIRED WITH INHERITANCE
         * 
         * SCENARIO TO USE: When derived classes need to ensure base class required 
         * properties are properly initialized.
         */
        static void RequiredWithInheritanceExample()
        {
            Console.WriteLine("3. Required with Inheritance Example:");
            
            var employee = new Employee
            {
                // Base class required properties
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@company.com",
                
                // Derived class required properties
                EmployeeId = "EMP001",
                Department = "Engineering"
            };
            
            Console.WriteLine($"Employee: {employee.FirstName} {employee.LastName}");
            Console.WriteLine($"ID: {employee.EmployeeId}, Dept: {employee.Department}");
            Console.WriteLine($"Email: {employee.Email}");
            
            Console.WriteLine();
        }
        
        public class Employee : Person
        {
            // Additional required properties in derived class
            public required string EmployeeId { get; set; }
            public required string Department { get; set; }
            
            // Optional property specific to Employee
            public DateTime? HireDate { get; set; }
            
            /*
             * INHERITANCE MEMORY NOTE:
             * - Derived class contains all base class fields/properties
             * - Memory layout includes base class data first, then derived class data
             * - 'required' enforcement applies to the final object creation, regardless of inheritance hierarchy
             */
        }
        
        #endregion

        #region Required with Records
        
        /*
         * REQUIRED WITH RECORDS
         * 
         * SCENARIO TO USE: When creating immutable data structures where certain 
         * properties are mandatory for object validity.
         */
        static void RequiredWithRecordsExample()
        {
            Console.WriteLine("4. Required with Records Example:");
            
            // Primary constructor satisfies all required properties
            var address1 = new Address("123 Main St", "Anytown", "12345");
            Console.WriteLine($"Address 1: {address1}");
            
            // Object initializer syntax (requires all required properties)
            var address2 = new Address
            {
                Street = "456 Oak Ave",
                City = "Somewhere",
                ZipCode = "67890"
            };
            Console.WriteLine($"Address 2: {address2}");
            
            Console.WriteLine();
        }
        
        public record Address
        {
            public required string Street { get; init; }
            public required string City { get; init; }
            public required string ZipCode { get; init; }
            public string? Country { get; init; } = "USA"; // Optional with default
            
            // Primary constructor for convenience
            [SetsRequiredMembers]
            public Address(string street, string city, string zipCode)
            {
                Street = street;
                City = city;
                ZipCode = zipCode;
            }
            
            // Parameterless constructor for object initializer
            public Address() { }
            
            /*
             * RECORDS AND MEMORY:
             * - Records are reference types (allocated on heap) by default
             * - 'init' properties provide immutability after construction
             * - 'required' + 'init' ensures immutable objects are properly initialized
             * - Records generate value-based equality and ToString() implementations
             */
        }
        
        #endregion

        #region Required vs Nullable
        
        /*
         * REQUIRED VS NULLABLE REFERENCE TYPES
         * 
         * PURPOSE: Demonstrates the interaction between 'required' and nullable reference types
         */
        static void RequiredVsNullableExample()
        {
            Console.WriteLine("5. Required vs Nullable Example:");
            
            var config = new Configuration
            {
                ServiceName = "MyService",        // Required non-nullable
                ConnectionString = null,          // Required but nullable - must be explicitly set to null
                TimeoutSeconds = 30               // Optional with default
            };
            
            Console.WriteLine($"Service: {config.ServiceName}");
            Console.WriteLine($"Connection: {config.ConnectionString ?? "Not set"}");
            Console.WriteLine($"Timeout: {config.TimeoutSeconds}s");
            
            Console.WriteLine();
        }
        
        public class Configuration
        {
            public required string ServiceName { get; set; }           // Required, non-nullable
            public required string? ConnectionString { get; set; }     // Required, but can be null
            public int TimeoutSeconds { get; set; } = 30;             // Optional with default
            
            /*
             * NULLABLE + REQUIRED SCENARIOS:
             * 
             * TO USE:
             * - When a property must be explicitly considered (even if set to null)
             * - Configuration scenarios where explicit null has meaning
             * - When you want to force developers to make conscious decisions about null values
             * 
             * NOT TO USE:
             * - When null has no meaningful difference from uninitialized
             * - When the property should never be null in practice
             */
        }
        
        #endregion

        #region Memory Allocation Deep Dive
        
        /*
         * MEMORY ALLOCATION ANALYSIS
         * 
         * PURPOSE: Detailed explanation of how 'required' affects memory allocation
         */
        static void MemoryAllocationExample()
        {
            Console.WriteLine("6. Memory Allocation Analysis:");
            
            var heavyObject = new HeavyObject
            {
                LargeData = new byte[1000],      // Required - heap allocation for array
                Metadata = "Important data",     // Required - heap allocation for string
                ProcessingOptions = new Dictionary<string, object> // Required - heap allocation
                {
                    ["Option1"] = "Value1",
                    ["Option2"] = 42
                }
            };
            
            Console.WriteLine($"Heavy object created with {heavyObject.LargeData.Length} bytes");
            Console.WriteLine($"Metadata: {heavyObject.Metadata}");
            Console.WriteLine($"Options count: {heavyObject.ProcessingOptions.Count}");
            
            /*
             * MEMORY ALLOCATION BREAKDOWN:
             * 
             * 1. HeavyObject instance: Heap allocation (~24-48 bytes object header + fields)
             * 2. LargeData byte array: Heap allocation (1000 bytes + array header)
             * 3. Metadata string: Heap allocation (string length + header)
             * 4. ProcessingOptions Dictionary: Heap allocation (dictionary overhead + entries)
             * 5. Dictionary string keys: Individual heap allocations
             * 6. Dictionary values: Heap allocations for reference types, inline for value types
             * 
             * REQUIRED IMPACT ON MEMORY:
             * - 'required' itself adds NO memory overhead
             * - It's purely a compile-time feature
             * - Memory allocation patterns are identical to non-required properties
             * - The benefit is preventing null reference exceptions that could cause memory leaks
             *   in cleanup scenarios
             */
            
            Console.WriteLine();
        }
        
        public class HeavyObject
        {
            public required byte[] LargeData { get; set; }
            public required string Metadata { get; set; }
            public required Dictionary<string, object> ProcessingOptions { get; set; }
            
            // Optional properties don't change memory layout
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
            public int ProcessingCount { get; set; } = 0;
        }
        
        #endregion

        #region When to Use and Not Use
        
        /*
         * COMPREHENSIVE GUIDE: WHEN TO USE vs WHEN NOT TO USE 'required'
         */
        static void WhenToUseAndNotUseExamples()
        {
            Console.WriteLine("7. When to Use vs Not Use Examples:");
            
            // GOOD USE CASES
            Console.WriteLine("✅ GOOD use cases demonstrated:");
            
            // 1. Domain entities with essential properties
            var user = new UserAccount
            {
                Username = "john_doe",
                Email = "john@example.com",
                PasswordHash = "hashed_password_123"
            };
            Console.WriteLine($"User: {user.Username} ({user.Email})");
            
            // 2. Configuration objects
            var dbConfig = new DatabaseConfig
            {
                ConnectionString = "Server=localhost;Database=Test;",
                CommandTimeout = 30
            };
            Console.WriteLine($"DB Config: {dbConfig.ConnectionString}");
            
            // 3. DTOs for API contracts
            var apiRequest = new CreateOrderRequest
            {
                CustomerId = "CUST001",
                ProductId = "PROD123",
                Quantity = 2
            };
            Console.WriteLine($"Order: {apiRequest.Quantity} of {apiRequest.ProductId}");
            
            Console.WriteLine("\n❌ AVOID using 'required' in these scenarios:");
            Console.WriteLine("- Properties with sensible defaults (use default values instead)");
            Console.WriteLine("- Internal implementation details (not part of public API)");
            Console.WriteLine("- Properties that are genuinely optional in business logic");
            Console.WriteLine("- High-frequency objects where initialization overhead matters");
            Console.WriteLine("- Properties that are set by frameworks (like Entity Framework)");
            
            Console.WriteLine();
        }
        
        #region Good Use Case Examples
        
        public class UserAccount
        {
            public required string Username { get; set; }      // Essential for identity
            public required string Email { get; set; }         // Essential for communication
            public required string PasswordHash { get; set; }  // Essential for security
            
            // Optional properties with defaults
            public bool IsActive { get; set; } = true;
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
            public string? DisplayName { get; set; }           // Genuinely optional
        }
        
        public class DatabaseConfig
        {
            public required string ConnectionString { get; set; }  // Essential for DB connection
            public int CommandTimeout { get; set; } = 30;         // Has sensible default
            public bool EnableRetry { get; set; } = true;         // Has sensible default
        }
        
        public class CreateOrderRequest  // API DTO
        {
            public required string CustomerId { get; set; }    // Required by business logic
            public required string ProductId { get; set; }     // Required by business logic
            public required int Quantity { get; set; }         // Required by business logic
            
            public string? Notes { get; set; }                 // Optional additional info
            public DateTime? RequestedDelivery { get; set; }   // Optional preference
        }
        
        #endregion
        
        #region Anti-Pattern Examples (What NOT to do)
        
        /*
         * ANTI-PATTERNS - Don't use 'required' for these:
         */
        
        // ❌ DON'T: Use required for properties with sensible defaults
        public class BadExample1
        {
            // This should have a default value instead of being required
            // public required string Status { get; set; }  // BAD
            
            public string Status { get; set; } = "Pending";   // GOOD
        }
        
        // ❌ DON'T: Use required for framework-managed properties
        public class BadExample2
        {
            // Don't make database-generated properties required
            // public required int Id { get; set; }  // BAD - Entity Framework will set this
            
            public int Id { get; set; }  // GOOD - let EF handle it
            public required string Name { get; set; }  // GOOD - business requirement
        }
        
        // ❌ DON'T: Use required for internal implementation details
        public class BadExample3
        {
            // Don't make internal caches or computed properties required
            // public required List<string> InternalCache { get; set; }  // BAD
            
            private readonly List<string> _internalCache = new();  // GOOD
            public required string ImportantData { get; set; }     // GOOD - external requirement
        }
        
        #endregion
        
        #endregion
    }
}
