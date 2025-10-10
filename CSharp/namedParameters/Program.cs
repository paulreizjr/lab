using System;

namespace NamedParametersExample
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Named Parameters in C# - Comprehensive Examples ===\n");

            // PURPOSE OF NAMED PARAMETERS:
            // Named parameters allow you to specify arguments by parameter name rather than position.
            // This improves code readability, flexibility, and maintainability, especially when:
            // - Methods have many parameters
            // - Some parameters have default values
            // - You want to skip optional parameters in the middle of the parameter list

            BasicNamedParameterExample();
            DefaultParametersExample();
            OptionalParametersExample();
            ComplexMethodExample();
            MemoryAllocationExample();
            AntiPatternsExample();

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        /// <summary>
        /// Basic example showing named parameters vs positional parameters
        /// </summary>
        static void BasicNamedParameterExample()
        {
            Console.WriteLine("1. BASIC NAMED PARAMETERS EXAMPLE:");
            Console.WriteLine("   Purpose: Improve code readability and argument clarity\n");

            // Traditional positional parameters - less readable
            CreateUser("John", "Doe", 30, "john.doe@email.com");

            // Named parameters - much more readable and self-documenting
            CreateUser(
                firstName: "Jane",
                lastName: "Smith", 
                age: 25,
                email: "jane.smith@email.com"
            );

            // Mixed approach - positional first, then named (C# allows this)
            CreateUser("Bob", "Johnson", age: 35, email: "bob.johnson@email.com");

            Console.WriteLine();
        }

        /// <summary>
        /// Creates a user with the given parameters
        /// </summary>
        static void CreateUser(string firstName, string lastName, int age, string email)
        {
            Console.WriteLine($"   Created user: {firstName} {lastName}, Age: {age}, Email: {email}");
        }

        /// <summary>
        /// Example showing named parameters with default values
        /// SCENARIO TO USE: When you have multiple optional parameters with defaults
        /// </summary>
        static void DefaultParametersExample()
        {
            Console.WriteLine("2. DEFAULT PARAMETERS WITH NAMED ARGUMENTS:");
            Console.WriteLine("   Scenario to use: Multiple optional parameters with sensible defaults\n");

            // Without named parameters, you'd need to specify all parameters or use overloads
            ConfigureConnection("localhost", 5432, "mydb", "user", "pass", true, 30);

            // With named parameters, you can specify only what you need to override
            ConfigureConnection(
                server: "production-server",
                database: "prod_db",
                username: "admin",
                password: "secret123"
                // port, enableSsl, and timeout use their default values
            );

            // You can also override parameters in any order
            ConfigureConnection(
                database: "test_db",
                server: "test-server", 
                timeout: 60,  // Override timeout but keep other defaults
                username: "testuser",
                password: "testpass"
            );

            Console.WriteLine();
        }

        /// <summary>
        /// Database connection configuration with default parameters
        /// </summary>
        static void ConfigureConnection(
            string server, 
            int port = 5432, 
            string database = "defaultdb", 
            string username = "guest", 
            string password = "guest", 
            bool enableSsl = false, 
            int timeout = 30)
        {
            Console.WriteLine($"   Connection: {server}:{port}/{database} " +
                            $"(User: {username}, SSL: {enableSsl}, Timeout: {timeout}s)");
        }

        /// <summary>
        /// Example showing optional parameters in the middle of parameter list
        /// SCENARIO TO USE: When you need to skip optional parameters in the middle
        /// </summary>
        static void OptionalParametersExample()
        {
            Console.WriteLine("3. OPTIONAL PARAMETERS - SKIPPING MIDDLE PARAMETERS:");
            Console.WriteLine("   Scenario to use: When you need to skip optional parameters in the middle\n");

            // Without named parameters, you'd have to specify all parameters up to the one you want
            // This becomes cumbersome and error-prone
            
            // With named parameters, you can easily skip optional parameters in the middle
            ProcessOrder(
                orderId: "ORD-001",
                customerName: "Alice Johnson",
                priority: "High",        // Skip shipping address (optional)
                expedited: true          // But specify expedited shipping
            );

            // Another example skipping different optional parameters
            ProcessOrder(
                orderId: "ORD-002",
                customerName: "Bob Wilson",
                shippingAddress: "123 Main St, City, State 12345"
                // Skip priority and expedited (use defaults)
            );

            Console.WriteLine();
        }

        /// <summary>
        /// Process an order with multiple optional parameters
        /// </summary>
        static void ProcessOrder(
            string orderId, 
            string customerName, 
            string shippingAddress = null, 
            string priority = "Normal", 
            bool expedited = false)
        {
            Console.WriteLine($"   Processing Order {orderId} for {customerName}");
            Console.WriteLine($"   Address: {shippingAddress ?? "Use default address"}");
            Console.WriteLine($"   Priority: {priority}, Expedited: {expedited}");
        }

        /// <summary>
        /// Complex method example showing when named parameters shine
        /// SCENARIO TO USE: Methods with many parameters, especially of the same type
        /// </summary>
        static void ComplexMethodExample()
        {
            Console.WriteLine("4. COMPLEX METHOD - MANY PARAMETERS:");
            Console.WriteLine("   Scenario to use: Methods with many parameters, especially of same type\n");

            // This would be very error-prone with positional parameters
            // (imagine mixing up left/right/top/bottom margins!)
            
            CreateDocument(
                title: "Annual Report 2024",
                author: "Finance Department",
                pageSize: "A4",
                orientation: "Portrait",
                leftMargin: 1.0,
                rightMargin: 1.0,
                topMargin: 1.5,
                bottomMargin: 1.5,
                fontSize: 12,
                fontFamily: "Arial",
                includeHeader: true,
                includeFooter: true,
                pageNumbers: true
            );

            Console.WriteLine();
        }

        /// <summary>
        /// Creates a document with extensive formatting options
        /// </summary>
        static void CreateDocument(
            string title,
            string author,
            string pageSize = "Letter",
            string orientation = "Portrait",
            double leftMargin = 1.0,
            double rightMargin = 1.0,
            double topMargin = 1.0,
            double bottomMargin = 1.0,
            int fontSize = 11,
            string fontFamily = "Times New Roman",
            bool includeHeader = false,
            bool includeFooter = false,
            bool pageNumbers = false)
        {
            Console.WriteLine($"   Document: '{title}' by {author}");
            Console.WriteLine($"   Format: {pageSize} {orientation}, Font: {fontFamily} {fontSize}pt");
            Console.WriteLine($"   Margins: L{leftMargin}\", R{rightMargin}\", T{topMargin}\", B{bottomMargin}\"");
            Console.WriteLine($"   Options: Header={includeHeader}, Footer={includeFooter}, PageNums={pageNumbers}");
        }

        /// <summary>
        /// MEMORY ALLOCATION CONSIDERATIONS with Named Parameters
        /// </summary>
        static void MemoryAllocationExample()
        {
            Console.WriteLine("5. MEMORY ALLOCATION CONSIDERATIONS:");
            Console.WriteLine("   Named parameters have minimal memory overhead\n");

            // MEMORY ALLOCATION FACTS:
            // 1. Named parameters are a COMPILE-TIME feature
            // 2. At runtime, they behave identically to positional parameters
            // 3. No additional memory allocation occurs
            // 4. No performance penalty at runtime
            // 5. The parameter names are used only during compilation for validation

            Console.WriteLine("   ✓ Named parameters are compile-time only - no runtime overhead");
            Console.WriteLine("   ✓ Same memory allocation as positional parameters");
            Console.WriteLine("   ✓ No additional objects created");
            Console.WriteLine("   ✓ No performance penalty");

            // Example: Both calls below generate identical IL code
            MemoryEfficientMethod(value1: 42, value2: "test", value3: true);
            MemoryEfficientMethod(42, "test", true); // Identical at runtime

            Console.WriteLine();
        }

        /// <summary>
        /// Method to demonstrate memory efficiency
        /// </summary>
        static void MemoryEfficientMethod(int value1, string value2, bool value3)
        {
            Console.WriteLine($"   Called with: {value1}, {value2}, {value3}");
        }

        /// <summary>
        /// SCENARIOS NOT TO USE named parameters (Anti-patterns)
        /// </summary>
        static void AntiPatternsExample()
        {
            Console.WriteLine("6. SCENARIOS NOT TO USE NAMED PARAMETERS (Anti-patterns):");
            Console.WriteLine("   When named parameters can hurt readability or maintainability\n");

            // ANTI-PATTERN 1: Simple methods with few, obvious parameters
            Console.WriteLine("   ❌ Anti-pattern 1: Simple methods with obvious parameters");
            // This is unnecessary and verbose:
            // Math.Max(x: 5, y: 10);  // Overkill
            Console.WriteLine("      Math.Max(x: 5, y: 10) // Too verbose for simple operations");
            
            // Better:
            Console.WriteLine("      Math.Max(5, 10) // Clean and clear");

            // ANTI-PATTERN 2: When parameter order is logical and unlikely to change
            Console.WriteLine("\n   ❌ Anti-pattern 2: Logical parameter order");
            // This is unnecessarily verbose:
            // Console.WriteLine(format: "Hello {0}", arg0: "World");
            Console.WriteLine("      Console.WriteLine(format: \"Hello {0}\", arg0: \"World\") // Overkill");
            
            // Better:
            Console.WriteLine("      Console.WriteLine(\"Hello {0}\", \"World\") // Natural order");

            // ANTI-PATTERN 3: Single parameter methods
            Console.WriteLine("\n   ❌ Anti-pattern 3: Single parameter methods");
            Console.WriteLine("      DoSomething(input: value) // Redundant for single parameters");

            // ANTI-PATTERN 4: Mixing named and positional inconsistently
            Console.WriteLine("\n   ❌ Anti-pattern 4: Inconsistent mixing");
            Console.WriteLine("      SomeMethod(\"pos1\", named2: \"val2\", \"pos3\") // Confusing pattern");

            // WHEN TO USE - SUMMARY:
            Console.WriteLine("\n   ✅ WHEN TO USE NAMED PARAMETERS:");
            Console.WriteLine("      • Methods with 4+ parameters");
            Console.WriteLine("      • Multiple parameters of the same type");
            Console.WriteLine("      • Optional parameters (especially in the middle)");
            Console.WriteLine("      • Configuration/setup methods");
            Console.WriteLine("      • When parameter purpose isn't obvious from context");
            Console.WriteLine("      • To skip optional parameters");
            Console.WriteLine("      • Boolean parameters (flag purposes)");

            Console.WriteLine("\n   ❌ WHEN NOT TO USE NAMED PARAMETERS:");
            Console.WriteLine("      • Simple methods with 1-3 obvious parameters");
            Console.WriteLine("      • Mathematical operations (Add, Subtract, etc.)");
            Console.WriteLine("      • Common framework methods (Console.WriteLine, etc.)");
            Console.WriteLine("      • When parameter order is well-established and logical");
            Console.WriteLine("      • Performance-critical inner loops (minimal impact, but avoid if extreme performance needed)");

            Console.WriteLine();
        }
    }
}
