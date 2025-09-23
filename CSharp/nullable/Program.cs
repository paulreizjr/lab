using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NullableExamples
{
    // ========================================
    // NULLABLE REFERENCE TYPES PURPOSE
    // ========================================
    
    /*
     * PURPOSE OF NULLABLE REFERENCE TYPES:
     * - Helps prevent NullReferenceException at compile time
     * - Provides static analysis to detect potential null dereferences
     * - Makes null-safety part of the type system
     * - Improves code quality and developer confidence
     * - Enables better API design with clear null contracts
     * - Opt-in feature (requires <Nullable>enable</Nullable> in .csproj)
     */

    // Example class demonstrating nullable annotations
    public class Person
    {
        // Non-nullable properties - compiler ensures they're never null
        public string FirstName { get; set; }
        public string LastName { get; set; }
        
        // Nullable property - can be null
        public string? MiddleName { get; set; }
        
        // Nullable reference type in constructor
        public Person(string firstName, string lastName, string? middleName = null)
        {
            FirstName = firstName;   // Must be assigned (non-nullable)
            LastName = lastName;     // Must be assigned (non-nullable)
            MiddleName = middleName; // Can be null
        }

        // Method returning nullable string
        public string? GetFullMiddleName()
        {
            return MiddleName?.ToUpper(); // Safe navigation
        }

        // Method with nullable parameter
        public void UpdateMiddleName(string? newMiddleName)
        {
            MiddleName = newMiddleName; // Allowed - both are nullable
        }
    }

    // Example of a service class with nullable patterns
    public class UserService
    {
        private readonly Dictionary<int, Person> _users = new();

        // Returns nullable - user might not exist
        public Person? FindUser(int id)
        {
            _users.TryGetValue(id, out Person? user);
            return user; // Could be null
        }

        // Non-nullable return - guaranteed to return a user or throw
        public Person GetUser(int id)
        {
            if (_users.TryGetValue(id, out Person? user))
            {
                return user; // Compiler knows this is not null
            }
            throw new ArgumentException($"User with id {id} not found");
        }

        // Method with nullable attribute annotations
        public bool TryGetUser(int id, [NotNullWhen(true)] out Person? user)
        {
            return _users.TryGetValue(id, out user);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== C# NULLABLE REFERENCE TYPES EXAMPLES ===\n");

            // ========================================
            // GOOD SCENARIOS TO USE NULLABLE TYPES
            // ========================================
            
            /*
             * EXCELLENT SCENARIOS FOR NULLABLE REFERENCE TYPES:
             * 1. New projects - enables null-safety from the start
             * 2. API boundaries - clearly communicate null contracts
             * 3. Data models - distinguish required vs optional fields
             * 4. Method return values - indicate when null is possible
             * 5. Collections and arrays - when elements can be null
             * 6. Configuration classes - optional settings
             * 7. Database entities - nullable columns
             */

            BasicNullableUsage();
            NullableWithCollections();
            NullablePatternMatching();
            NullForgivingOperator();
            NullableAttributes();
            MemoryAndPerformanceImpact();
            
            Console.WriteLine("\n=== SCENARIOS NOT TO USE NULLABLE ===");
            WhenNotToUseNullable();

            Console.WriteLine("\nPress any key to exit...");
        }

        static void BasicNullableUsage()
        {
            Console.WriteLine("--- Basic Nullable Reference Types Usage ---");
            
            /*
             * BASIC NULLABLE SYNTAX:
             * - Type? : Nullable reference type (can be null)
             * - Type  : Non-nullable reference type (should never be null)
             * - ?. : Null-conditional operator (safe navigation)
             * - ?? : Null-coalescing operator (default value)
             * - ??= : Null-coalescing assignment
             */

            // Non-nullable variables - compiler ensures assignment
            string nonNullable = "Hello World";
            // string nonNullable2; // ❌ Compiler warning: may be null

            // Nullable variables - explicitly can be null
            string? nullable = null; // ✅ OK
            string? anotherNullable = "Can also have values";
            Console.WriteLine($"Another example: {anotherNullable}");

            Console.WriteLine($"Non-nullable: {nonNullable}");
            Console.WriteLine($"Nullable: {nullable ?? "Was null, using default"}");

            // Safe navigation with null-conditional operator
            int? length = nullable?.Length; // Returns null if nullable is null
            Console.WriteLine($"Length: {length ?? 0}");

            // Null-coalescing assignment
            nullable ??= "Default value"; // Assign only if null
            Console.WriteLine($"After ??= : {nullable}");

            // Working with Person class
            Person person = new Person("John", "Doe", null);
            Console.WriteLine($"Full name: {person.FirstName} {person.MiddleName ?? "(no middle)"} {person.LastName}");

            Person personWithMiddle = new Person("Jane", "Smith", "Marie");
            Console.WriteLine($"Full name: {personWithMiddle.FirstName} {personWithMiddle.MiddleName} {personWithMiddle.LastName}");
            
            Console.WriteLine();
        }

        static void NullableWithCollections()
        {
            Console.WriteLine("--- Nullable with Collections ---");
            
            /*
             * COLLECTIONS AND NULLABLE:
             * - List<T?> : List of nullable elements
             * - List<T>? : Nullable list (entire list can be null)
             * - List<T?>? : Nullable list of nullable elements
             */

            // List of nullable strings
            List<string?> namesWithNulls = new List<string?>
            {
                "Alice", 
                null, 
                "Bob", 
                null, 
                "Charlie"
            };

            Console.WriteLine("Processing list with nullable elements:");
            foreach (string? name in namesWithNulls)
            {
                // Safe handling of nullable elements
                if (name != null)
                {
                    Console.WriteLine($"  Name: {name} (Length: {name.Length})");
                }
                else
                {
                    Console.WriteLine("  Name: <null>");
                }
            }

            // Nullable list itself
            List<string>? nullableList = null;
            Console.WriteLine($"Nullable list count: {nullableList?.Count ?? 0}");

            // Initialize the nullable list
            nullableList = new List<string> { "Item1", "Item2" };
            Console.WriteLine($"After initialization count: {nullableList.Count}");

            // Dictionary with nullable values
            Dictionary<string, Person?> personLookup = new Dictionary<string, Person?>
            {
                ["admin"] = new Person("Admin", "User"),
                ["guest"] = null // Guest user doesn't exist
            };

            Person? admin = personLookup["admin"];
            Person? guest = personLookup["guest"];
            
            Console.WriteLine($"Admin: {admin?.FirstName ?? "Not found"}");
            Console.WriteLine($"Guest: {guest?.FirstName ?? "Not found"}");
            
            Console.WriteLine();
        }

        static void NullablePatternMatching()
        {
            Console.WriteLine("--- Nullable with Pattern Matching ---");
            
            /*
             * PATTERN MATCHING WITH NULLABLE:
             * - is null : Check for null
             * - is not null : Check for non-null
             * - Switch expressions with null patterns
             */

            UserService userService = new UserService();
            
            // Add some test users
            AddTestUsers(userService);

            // Pattern matching with nullable return
            for (int i = 1; i <= 3; i++)
            {
                Person? user = userService.FindUser(i);
                
                string result = user switch
                {
                    null => "User not found",
                    { MiddleName: null } => $"{user.FirstName} {user.LastName} (no middle name)",
                    { MiddleName: not null } => $"{user.FirstName} {user.MiddleName} {user.LastName}",
                };
                
                Console.WriteLine($"User {i}: {result}");
            }

            // Using is patterns
            Person? testUser = userService.FindUser(1);
            if (testUser is not null)
            {
                Console.WriteLine($"Found user: {testUser.FirstName}");
            }

            // Null-conditional with pattern matching
            string middleInfo = testUser?.MiddleName switch
            {
                null => "No middle name",
                var middle => $"Middle name: {middle}"
            };
            Console.WriteLine(middleInfo);
            
            Console.WriteLine();
        }

        static void NullForgivingOperator()
        {
            Console.WriteLine("--- Null-Forgiving Operator (!) ---");
            
            /*
             * NULL-FORGIVING OPERATOR (!):
             * - Tells compiler "I know this could be null, but trust me it's not"
             * - Use sparingly and only when you're certain
             * - Suppresses nullable warnings
             * - Runtime behavior unchanged - can still throw NullReferenceException
             */

            UserService userService = new UserService();
            AddTestUsers(userService);

            // Without null-forgiving operator - compiler warning
            Person? foundUser = userService.FindUser(1);
            // string firstName = foundUser.FirstName; // ❌ Warning: possible null

            // With null-forgiving operator - suppresses warning
            string firstName = foundUser!.FirstName; // ✅ No warning (but risky!)
            Console.WriteLine($"First name: {firstName}");

            // Better approach - proper null checking
            if (foundUser != null)
            {
                string safeFirstName = foundUser.FirstName; // ✅ Safe, no warning
                Console.WriteLine($"Safe first name: {safeFirstName}");
            }

            // Null-forgiving with method calls
            string? possiblyNull = GetPossiblyNullString();
            if (possiblyNull != null)
            {
                // int length = possiblyNull.Length; // ❌ Would still warn without null check
                int length = possiblyNull!.Length;   // ✅ No warning, safe here because of null check
                Console.WriteLine($"Length using !: {length}");
            }
            else
            {
                Console.WriteLine("GetPossiblyNullString returned null");
            }

            // ⚠️ WARNING: This will throw NullReferenceException!
            try
            {
                string? definitelyNull = null;
                int badLength = definitelyNull!.Length; // Runtime exception!
            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine($"Caught expected exception: {ex.Message}");
            }
            
            Console.WriteLine();
        }

        static void NullableAttributes()
        {
            Console.WriteLine("--- Nullable Attributes ---");
            
            /*
             * NULLABLE ATTRIBUTES:
             * - [NotNull] : Parameter/return value will not be null
             * - [MaybeNull] : May return null despite non-nullable type
             * - [NotNullWhen(bool)] : Parameter is not null when method returns specified bool
             * - [MaybeNullWhen(bool)] : Parameter may be null when method returns specified bool
             * - [NotNullIfNotNull] : Return value is not null if parameter is not null
             */

            UserService userService = new UserService();
            AddTestUsers(userService);

            // Using TryGetUser with NotNullWhen attribute
            if (userService.TryGetUser(1, out Person? user))
            {
                // Compiler knows 'user' is not null here due to [NotNullWhen(true)]
                Console.WriteLine($"Found user: {user.FirstName} {user.LastName}");
            }

            // Demonstrate custom methods with attributes
            string? input = "Hello World";
            if (IsNotNullOrEmpty(input, out string nonNullInput))
            {
                // Compiler knows nonNullInput is not null
                Console.WriteLine($"Processing: {nonNullInput.ToUpper()}");
            }

            // Testing with null input
            string? nullInput = null;
            if (!IsNotNullOrEmpty(nullInput, out string _))
            {
                Console.WriteLine("Input was null or empty");
            }
            
            Console.WriteLine();
        }

        static void MemoryAndPerformanceImpact()
        {
            Console.WriteLine("--- Memory and Performance Impact ---");
            
            /*
             * MEMORY ALLOCATION WITH NULLABLE REFERENCE TYPES:
             * 
             * 1. COMPILE-TIME FEATURE: Nullable reference types are purely compile-time
             * 2. NO RUNTIME OVERHEAD: No additional memory or performance cost at runtime
             * 3. METADATA ONLY: Nullability information stored in metadata for tools
             * 4. SAME IL CODE: Generated IL code is identical with/without nullable
             * 5. STATIC ANALYSIS: All checking happens during compilation
             * 
             * COMPARISON WITH NULLABLE VALUE TYPES:
             * - Nullable<T> (T?) for value types HAS runtime overhead (extra bool + value)
             * - Nullable reference types have NO runtime overhead
             */

            Console.WriteLine("Memory comparison demonstration:");
            
            // Reference types - nullable annotations don't affect memory
            string nonNullableRef = "Hello";
            string? nullableRef = "World";
            
            Console.WriteLine($"Non-nullable string reference: {IntPtr.Size} bytes (pointer)"); // what is going on here? 
            // IntPtr.Size gives size of a pointer (4 bytes on 32-bit, 8 bytes on 64-bit)
            // Both references (nullable and non-nullable) are just pointers to heap objects
            Console.WriteLine($"Nullable string reference: {IntPtr.Size} bytes (pointer)");
            Console.WriteLine("👆 Same memory usage - nullable is compile-time only!");
            
            // Use the variables to avoid warnings
            _ = nonNullableRef.Length;
            _ = nullableRef?.Length;

            // Value types - these DO have memory overhead
            int nonNullableValue = 42;
            int? nullableValue = 42;
            
            // Safe way to show size without unsafe code
            Console.WriteLine($"Non-nullable int: {sizeof(int)} bytes");
            Console.WriteLine($"Nullable int: approximately {sizeof(int) + sizeof(bool)} bytes (int + bool)");
            Console.WriteLine("👆 Nullable value types have overhead (bool + value)");
            Console.WriteLine("Note: Actual Nullable<T> size may be optimized by runtime");
            
            // Use the variables to avoid warnings
            _ = nonNullableValue;
            _ = nullableValue;

            // Performance - no difference for reference types
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            for (int i = 0; i < 1000000; i++)
            {
                ProcessNonNullable("test");
            }
            var nonNullableTime = stopwatch.ElapsedMilliseconds;
            
            stopwatch.Restart();
            for (int i = 0; i < 1000000; i++)
            {
                ProcessNullable("test");
            }
            var nullableTime = stopwatch.ElapsedMilliseconds;

            Console.WriteLine($"Non-nullable processing time: {nonNullableTime}ms");
            Console.WriteLine($"Nullable processing time: {nullableTime}ms");
            Console.WriteLine("Performance is essentially identical!");
            
            Console.WriteLine();
        }

        // Helper methods for examples
        static void AddTestUsers(UserService userService)
        {
            // Using reflection to add users (simulating data)
            var usersField = typeof(UserService).GetField("_users",
                // the statement below means we want non-public instance fields
                // including private and protected fields
                // the | operator means "or" 
                // as the | we could also use & to mean "and"
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
                );
            var users = (Dictionary<int, Person>)usersField!.GetValue(userService)!;
            
            users[1] = new Person("Alice", "Johnson", "Marie");
            users[2] = new Person("Bob", "Smith", null);
        }

        static string? GetPossiblyNullString()
        {
            return DateTime.Now.Millisecond % 2 == 0 ? "Not null" : null;
        }

        // Custom method with nullable attributes
        static bool IsNotNullOrEmpty([NotNullWhen(true)] string? input, [NotNullWhen(true)] out string output)
        {
            if (!string.IsNullOrEmpty(input))
            {
                output = input;
                return true;
            }
            output = string.Empty; // Provide a non-null value when returning false
            return false;
        }

        static void ProcessNonNullable(string input)
        {
            _ = input.Length; // Simulate processing
        }

        static void ProcessNullable(string? input)
        {
            _ = input?.Length; // Simulate processing
        }

        static void WhenNotToUseNullable()
        {
            /*
             * SCENARIOS NOT TO USE NULLABLE REFERENCE TYPES:
             * 
             * 1. LEGACY CODEBASES: Large existing codebases with many null-related patterns
             *    ❌ Don't enable globally: Will generate thousands of warnings
             *    ✅ Use instead: Enable gradually, file-by-file or assembly-by-assembly
             * 
             * 2. EXTERNAL LIBRARIES: When working heavily with libraries that don't support nullable
             *    ❌ Don't force nullable: Creates friction and many warnings
             *    ✅ Use instead: Enable nullable only for your own code
             * 
             * 3. RAPID PROTOTYPING: When building quick prototypes or proof-of-concepts
             *    ❌ Don't add nullable overhead: Slows down experimentation
             *    ✅ Use instead: Keep disabled during prototyping, enable when stabilizing
             * 
             * 4. PERFORMANCE-CRITICAL CODE: Where compile-time analysis overhead matters
             *    ❌ Don't enable: Compilation may be slower (minimal impact usually)
             *    ✅ Use instead: Enable selectively or after optimization phase
             * 
             * 5. REFLECTION-HEAVY CODE: Code that uses lots of reflection and dynamic typing
             *    ❌ Don't rely on nullable: Static analysis limited with dynamic code
             *    ✅ Use instead: Combine with runtime null checks
             * 
             * 6. INTEROP CODE: P/Invoke and COM interop scenarios
             *    ❌ Don't assume nullable works: External code doesn't follow C# rules
             *    ✅ Use instead: Manual null checking at boundaries
             */

            Console.WriteLine("Scenarios where nullable reference types are challenging:");
            Console.WriteLine("❌ Large legacy codebases (too many warnings)");
            Console.WriteLine("❌ Heavy use of non-nullable-aware libraries");
            Console.WriteLine("❌ Rapid prototyping (adds development overhead)");
            Console.WriteLine("❌ Reflection-heavy or dynamic code");
            Console.WriteLine("❌ P/Invoke and interop scenarios");
            Console.WriteLine("❌ Performance-critical compilation scenarios");
            
            Console.WriteLine("\n✅ Better approaches for these scenarios:");
            Console.WriteLine("  - Enable nullable gradually (per-file or per-assembly)");
            Console.WriteLine("  - Use #nullable disable for problematic files");
            Console.WriteLine("  - Combine with runtime null checking at boundaries");
            Console.WriteLine("  - Enable after stabilizing prototypes");
            Console.WriteLine("  - Use nullable-aware wrapper APIs for external libraries");
            
            // Example of gradual adoption
            Console.WriteLine("\n--- Example: Gradual Nullable Adoption ---");
            Console.WriteLine("File-level control:");
            Console.WriteLine("#nullable enable    // Enable for this file");
            Console.WriteLine("#nullable disable   // Disable for this file");
            Console.WriteLine("#nullable restore   // Restore project setting");
            
            // Project-level control examples
            Console.WriteLine("\nProject-level control in .csproj:");
            Console.WriteLine("<Nullable>enable</Nullable>          <!-- All files -->"); 
            Console.WriteLine("<Nullable>warnings</Nullable>        <!-- Warnings only -->");
            Console.WriteLine("<Nullable>annotations</Nullable>     <!-- Annotations only -->");
            Console.WriteLine("<Nullable>disable</Nullable>         <!-- Disabled -->");
        }
    }
}
