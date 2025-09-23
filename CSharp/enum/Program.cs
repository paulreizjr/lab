using System;

namespace EnumExamples
{
    // ========================================
    // ENUM PURPOSE AND DEFINITION
    // ========================================
    
    /*
     * PURPOSE OF ENUMS:
     * - Enums define a set of named constants that represent distinct values
     * - They provide type safety and make code more readable and maintainable
     * - They prevent invalid values and make intentions clear
     * - They are value types that inherit from System.Enum
     */

    // Basic enum - represents days of the week
    public enum DayOfWeek
    {
        Sunday,    // 0 by default
        Monday,    // 1
        Tuesday,   // 2
        Wednesday, // 3
        Thursday,  // 4
        Friday,    // 5
        Saturday   // 6
    }

    // Enum with explicit values
    public enum Priority : byte  // Using byte as underlying type for memory efficiency
    {
        Low = 1,
        Medium = 5,
        High = 10,
        Critical = 20
    }

    // Flags enum - allows bitwise operations for combinations
    [Flags]
    public enum FilePermissions : int
    {
        None = 0,
        Read = 1,      // 0001
        Write = 2,     // 0010
        Execute = 4,   // 0100
        Delete = 8,    // 1000
        ReadWrite = Read | Write,           // 0011
        FullAccess = Read | Write | Execute | Delete  // 1111
    }

    // Enum representing different states in a workflow
    public enum OrderStatus
    {
        Pending,
        Processing,
        Shipped,
        Delivered,
        Cancelled,
        Refunded
    }

    // Enum for HTTP status codes (partial example)
    public enum HttpStatusCode : int
    {
        OK = 200,
        NotFound = 404,
        InternalServerError = 500,
        BadRequest = 400
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== C# ENUM EXAMPLES ===\n");

            // ========================================
            // BASIC ENUM USAGE SCENARIOS
            // ========================================
            
            /*
             * GOOD SCENARIOS TO USE ENUMS:
             * 1. Fixed set of related constants (days, months, status codes)
             * 2. State machines and workflow states
             * 3. Configuration options with limited choices
             * 4. Type-safe alternatives to magic numbers
             * 5. Bitwise flags for permissions/options
             */

            BasicEnumUsage();
            EnumWithSwitchStatement();
            FlagsEnumExample();
            EnumConversions();
            EnumMemoryAllocation();

            Console.WriteLine("\n=== SCENARIOS NOT TO USE ENUMS ===");
            WhenNotToUseEnums();

            Console.WriteLine("\n=== END OF ENUM EXAMPLES ===");
        }

        static void BasicEnumUsage()
        {
            Console.WriteLine("--- Basic Enum Usage ---");
            
            // Declare and assign enum values
            DayOfWeek today = DayOfWeek.Monday;
            Priority taskPriority = Priority.High;

            Console.WriteLine($"Today is: {today}");
            Console.WriteLine($"Task priority: {taskPriority} (value: {(int)taskPriority})");
            
            // Enum comparison is type-safe
            if (today == DayOfWeek.Monday)
            {
                Console.WriteLine("It's Monday - start of work week!");
            }

            // Using enum in method parameters provides type safety
            ProcessOrder(OrderStatus.Processing);
            
            Console.WriteLine();
        }

        static void EnumWithSwitchStatement()
        {
            Console.WriteLine("--- Enum with Switch Statement ---");
            
            /*
             * EXCELLENT SCENARIO: State machines and decision logic
             * Enums work perfectly with switch statements for handling different states
             */
            
            OrderStatus[] statuses = { 
                OrderStatus.Pending, 
                OrderStatus.Shipped, 
                OrderStatus.Delivered, 
                OrderStatus.Cancelled 
            };

            foreach (var status in statuses)
            {
                string message = status switch
                {
                    OrderStatus.Pending => "Order received, awaiting processing",
                    OrderStatus.Processing => "Order is being prepared",
                    OrderStatus.Shipped => "Order is on its way",
                    OrderStatus.Delivered => "Order has been delivered",
                    OrderStatus.Cancelled => "Order has been cancelled",
                    OrderStatus.Refunded => "Order has been refunded",
                    _ => "Unknown status"
                };
                
                Console.WriteLine($"Status: {status} -> {message}");
            }
            
            Console.WriteLine();
        }

        static void FlagsEnumExample()
        {
            Console.WriteLine("--- Flags Enum Example ---");
            
            /*
             * FLAGS SCENARIO: When you need to combine multiple options
             * Perfect for permissions, configuration options, or any bitwise operations
             */
            
            // Combine multiple permissions using bitwise OR
            FilePermissions userPermissions = FilePermissions.Read | FilePermissions.Write;
            FilePermissions adminPermissions = FilePermissions.FullAccess;

            Console.WriteLine($"User permissions: {userPermissions}");
            Console.WriteLine($"Admin permissions: {adminPermissions}");

            // Check if specific permission exists using bitwise AND
            if ((userPermissions & FilePermissions.Read) == FilePermissions.Read)
            // how this works? Bitwise AND compares each bit; if the Read bit is set in userPermissions, the result equals FilePermissions.Read
            {
                Console.WriteLine("User has read permission");
            }

            if ((userPermissions & FilePermissions.Execute) == FilePermissions.Execute)
            {
                Console.WriteLine("User has execute permission");
            }
            else
            {
                Console.WriteLine("User does NOT have execute permission");
            }

            // Add permission
            userPermissions |= FilePermissions.Execute;
            Console.WriteLine($"After adding execute: {userPermissions}");

            // Remove permission
            userPermissions &= ~FilePermissions.Write;
            Console.WriteLine($"After removing write: {userPermissions}");
            
            Console.WriteLine();
        }

        static void EnumConversions()
        {
            Console.WriteLine("--- Enum Conversions ---");
            
            // Convert enum to int
            Priority p = Priority.High;
            int priorityValue = (int)p;
            Console.WriteLine($"Priority.High as int: {priorityValue}");

            // Convert int to enum (be careful - no validation!)
            Priority convertedPriority = (Priority)10;
            Console.WriteLine($"Int 10 as Priority: {convertedPriority}");

            // Safe conversion using Enum.IsDefined
            byte testValue = 15; // Using byte since Priority underlying type is byte
            if (Enum.IsDefined(typeof(Priority), testValue))
            {
                Priority safePriority = (Priority)testValue;
                Console.WriteLine($"Safe conversion: {safePriority}");
            }
            else
            {
                Console.WriteLine($"Value {testValue} is not a valid Priority");
            }

            // Parse string to enum
            if (Enum.TryParse<DayOfWeek>("Friday", out DayOfWeek parsedDay))
            {
                Console.WriteLine($"Parsed day: {parsedDay}");
            }

            // Get all enum values
            Console.WriteLine("All Priority values:");
            foreach (Priority priority in Enum.GetValues<Priority>())
            {
                Console.WriteLine($"  {priority} = {(int)priority}");
            }
            
            Console.WriteLine();
        }

        static void EnumMemoryAllocation()
        {
            Console.WriteLine("--- Enum Memory Allocation ---");
            
            /*
             * MEMORY ALLOCATION FACTS:
             * 
             * 1. Enums are VALUE TYPES - stored on stack (or inline in objects)
             * 2. Default underlying type is 'int' (4 bytes)
             * 3. You can specify smaller types for memory efficiency: byte (1), short (2), long (8)
             * 4. No heap allocation for enum values themselves
             * 5. Boxing occurs when converting to object (creates heap allocation)
             * 6. Enum arrays are stored contiguously in memory
             */

            // Demonstrate different underlying types and their sizes
            Console.WriteLine("Memory sizes of different enum underlying types:");
            Console.WriteLine($"DayOfWeek (int): {sizeof(int)} bytes");
            Console.WriteLine($"Priority (byte): {sizeof(byte)} bytes");
            Console.WriteLine($"FilePermissions (int): {sizeof(int)} bytes");

            // Show that enums are value types - no reference equality
            Priority p1 = Priority.High;
            Priority p2 = Priority.High;
            
            Console.WriteLine($"p1 == p2: {p1 == p2}"); // True - value comparison
            Console.WriteLine($"p1.Equals(p2): {p1.Equals(p2)}"); // True
            
            // Boxing demonstration (causes heap allocation)
            object boxedEnum = p1; // Boxing occurs here
            Console.WriteLine($"Boxed enum: {boxedEnum}");
            
            // Array of enums - efficient memory layout
            Priority[] priorities = { Priority.Low, Priority.Medium, Priority.High };
            Console.WriteLine($"Array of 3 Priority enums uses approximately: {priorities.Length * sizeof(byte)} bytes");
            
            Console.WriteLine();
        }

        static void ProcessOrder(OrderStatus status)
        {
            // Method demonstrating type-safe parameter usage
            Console.WriteLine($"Processing order with status: {status}");
        }

        static void WhenNotToUseEnums()
        {
            /*
             * SCENARIOS NOT TO USE ENUMS:
             * 
             * 1. DYNAMIC VALUES: When the set of values changes frequently or is determined at runtime
             *    ❌ Don't use for: User-defined categories, database-driven lookup values
             *    ✅ Use instead: Dictionary<int, string>, database tables, configuration files
             * 
             * 2. LARGE SETS OF VALUES: When you have hundreds or thousands of possible values
             *    ❌ Don't use for: All possible HTTP status codes, country codes, product IDs
             *    ✅ Use instead: Constants class, lookup tables, external configuration
             * 
             * 3. VALUES WITH COMPLEX DATA: When each value needs additional properties
             *    ❌ Don't use for: Storing name, description, and behavior for each option
             *    ✅ Use instead: Classes, structs, or records with properties
             * 
             * 4. FREQUENT ADDITIONS: When new values are added regularly through updates
             *    ❌ Don't use for: Plugin types, feature flags that change often
             *    ✅ Use instead: Registry pattern, configuration-driven approaches
             * 
             * 5. MATHEMATICAL OPERATIONS: When you need to perform complex math on values
             *    ❌ Don't use for: Mathematical constants that need calculations
             *    ✅ Use instead: const fields, static readonly fields, or calculated properties
             */

            Console.WriteLine("Examples of what NOT to use enums for:");
            Console.WriteLine("❌ User-defined categories (dynamic)");
            Console.WriteLine("❌ Large sets like all country codes (too many values)");
            Console.WriteLine("❌ Complex objects with multiple properties");
            Console.WriteLine("❌ Values that change frequently via configuration");
            Console.WriteLine("❌ Mathematical constants requiring calculations");
            
            Console.WriteLine("\n✅ Better alternatives:");
            Console.WriteLine("  - Dictionary<TKey, TValue> for dynamic mappings");
            Console.WriteLine("  - Classes/structs for complex data");
            Console.WriteLine("  - Configuration files for changeable values");
            Console.WriteLine("  - Const/static readonly for mathematical constants");
        }
    }
}
