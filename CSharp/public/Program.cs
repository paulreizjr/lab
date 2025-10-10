using System;
using System.Collections.Generic;

namespace PublicKeywordExample
{
    // PURPOSE OF PUBLIC KEYWORD:
    // The 'public' access modifier makes classes, methods, properties, and fields
    // accessible from any other code in the same assembly or another assembly that references it.
    // It provides the widest possible access level in C#'s access control system.

    /// <summary>
    /// Main program demonstrating public keyword usage
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Public Keyword in C# - Comprehensive Examples ===\n");

            PublicClassExample();
            PublicMethodsExample();
            PublicPropertiesExample();
            PublicFieldsExample();
            PublicEventsExample();
            MemoryAllocationExample();
            AntiPatternsExample();
            BestPracticesExample();

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        /// <summary>
        /// Demonstrates public classes and their accessibility
        /// </summary>
        static void PublicClassExample()
        {
            Console.WriteLine("1. PUBLIC CLASSES:");
            Console.WriteLine("   Purpose: Make classes accessible across assemblies and namespaces\n");

            // SCENARIO TO USE: API classes, utility classes, data models
            // Public classes can be instantiated from anywhere
            var vehicle = new Vehicle("Toyota", "Camry", 2023);
            Console.WriteLine($"   Created vehicle: {vehicle.Make} {vehicle.Model} ({vehicle.Year})");

            var calculator = new MathUtility();
            Console.WriteLine($"   Calculator ready: {calculator.GetType().Name}");

            // Public classes are essential for:
            // - Libraries and frameworks
            // - API endpoints and data contracts
            // - Shared utilities across projects
            // - Plugin architectures

            Console.WriteLine();
        }

        /// <summary>
        /// Demonstrates public methods and their use cases
        /// </summary>
        static void PublicMethodsExample()
        {
            Console.WriteLine("2. PUBLIC METHODS:");
            Console.WriteLine("   Purpose: Expose functionality that external code needs to access\n");

            var mathUtil = new MathUtility();
            
            // SCENARIO TO USE: Core functionality that needs to be accessible
            double result = mathUtil.Add(10.5, 20.3);
            Console.WriteLine($"   Addition result: {result}");

            // Public methods are essential for:
            // - API endpoints
            // - Core business logic
            // - Utility functions
            // - Event handlers
            // - Interface implementations

            var dataProcessor = new DataProcessor();
            dataProcessor.ProcessData("Sample data");
            
            Console.WriteLine();
        }

        /// <summary>
        /// Demonstrates public properties and encapsulation
        /// </summary>
        static void PublicPropertiesExample()
        {
            Console.WriteLine("3. PUBLIC PROPERTIES:");
            Console.WriteLine("   Purpose: Controlled access to object state with encapsulation\n");

            var account = new BankAccount("John Doe", 1000.00m);
            
            // SCENARIO TO USE: Exposing object state with validation and control
            Console.WriteLine($"   Account holder: {account.AccountHolder}");
            Console.WriteLine($"   Initial balance: ${account.Balance:F2}");
            
            // Public properties allow controlled access
            account.Deposit(500.00m);
            Console.WriteLine($"   After deposit: ${account.Balance:F2}");
            
            // Properties can have different access levels for get/set
            Console.WriteLine($"   Account number (read-only): {account.AccountNumber}");

            // Public properties are ideal for:
            // - Data transfer objects (DTOs)
            // - Configuration settings
            // - Computed values
            // - Validated state changes

            Console.WriteLine();
        }

        /// <summary>
        /// Demonstrates public fields (and why to avoid them)
        /// </summary>
        static void PublicFieldsExample()
        {
            Console.WriteLine("4. PUBLIC FIELDS:");
            Console.WriteLine("   Purpose: Direct access to data (use sparingly)\n");

            var config = new SystemConfiguration();
            
            // SCENARIO TO USE: Constants, readonly values, simple data containers
            Console.WriteLine($"   Max connections: {SystemConfiguration.MaxConnections}");
            Console.WriteLine($"   Default timeout: {SystemConfiguration.DefaultTimeoutSeconds}");
            
            // Public fields for simple data structures
            var point = new Point2D { X = 10, Y = 20 };
            Console.WriteLine($"   Point coordinates: ({point.X}, {point.Y})");

            // WARNING: Public fields bypass encapsulation
            // Better to use properties in most cases
            
            Console.WriteLine();
        }

        /// <summary>
        /// Demonstrates public events for communication
        /// </summary>
        static void PublicEventsExample()
        {
            Console.WriteLine("5. PUBLIC EVENTS:");
            Console.WriteLine("   Purpose: Enable external code to subscribe to notifications\n");

            var fileProcessor = new FileProcessor();
            
            // SCENARIO TO USE: Observer pattern, notifications, loose coupling
            fileProcessor.FileProcessed += (sender, fileName) =>
            {
                Console.WriteLine($"   Event received: File '{fileName}' was processed");
            };

            fileProcessor.ProcessFile("document.txt");

            // Public events are essential for:
            // - UI frameworks (button clicks, data changes)
            // - Background services (progress updates)
            // - Domain events in business logic
            // - Plugin architectures

            Console.WriteLine();
        }

        /// <summary>
        /// MEMORY ALLOCATION considerations with public members
        /// </summary>
        static void MemoryAllocationExample()
        {
            Console.WriteLine("6. MEMORY ALLOCATION CONSIDERATIONS:");
            Console.WriteLine("   Public keyword itself has no memory overhead\n");

            // MEMORY ALLOCATION FACTS:
            // 1. 'public' is a compile-time access modifier
            // 2. No runtime memory overhead for the access modifier itself
            // 3. Public members follow standard .NET memory allocation rules
            // 4. Properties may have slight overhead due to method calls vs direct field access
            // 5. Public classes are subject to same GC rules as any other class

            Console.WriteLine("   ✓ 'public' keyword has no memory overhead");
            Console.WriteLine("   ✓ Public properties: small method call overhead vs fields");
            Console.WriteLine("   ✓ Public classes: same memory footprint as internal/private classes");
            Console.WriteLine("   ✓ Access modifiers are compile-time only");

            // Memory allocation examples
            var publicClass = new Vehicle("Honda", "Civic", 2023);  // Same allocation as private class
            
            // Property access (slight overhead due to method call)
            string make = publicClass.Make;  // Method call to get_Make()
            
            // Field access (direct memory access)
            var point = new Point2D { X = 5, Y = 10 };  // Direct field access
            int x = point.X;  // Direct memory read

            Console.WriteLine($"   Memory allocation demo: {make}, Point: ({point.X}, {point.Y})");
            Console.WriteLine();
        }

        /// <summary>
        /// SCENARIOS NOT TO USE public (Anti-patterns)
        /// </summary>
        static void AntiPatternsExample()
        {
            Console.WriteLine("7. SCENARIOS NOT TO USE PUBLIC (Anti-patterns):");
            Console.WriteLine("   When public access violates encapsulation or security\n");

            // ANTI-PATTERN 1: Exposing internal implementation details
            Console.WriteLine("   ❌ Anti-pattern 1: Exposing internal implementation");
            Console.WriteLine("      public List<string> _internalCache; // Breaks encapsulation");
            Console.WriteLine("      public string _connectionString; // Security risk");

            // ANTI-PATTERN 2: Public fields for mutable state
            Console.WriteLine("\n   ❌ Anti-pattern 2: Public mutable fields");
            Console.WriteLine("      public decimal balance; // No validation possible");
            Console.WriteLine("      public bool isActive; // No change tracking");

            // ANTI-PATTERN 3: Over-exposing utility methods
            Console.WriteLine("\n   ❌ Anti-pattern 3: Over-exposing helper methods");
            Console.WriteLine("      public void InternalHelperMethod(); // Should be private");
            Console.WriteLine("      public void TemporaryDebugMethod(); // Should be removed");

            // ANTI-PATTERN 4: Public members that should be protected/internal
            Console.WriteLine("\n   ❌ Anti-pattern 4: Wrong access level");
            Console.WriteLine("      public void OnlyUsedByDerivedClasses(); // Should be protected");
            Console.WriteLine("      public void OnlyUsedInThisAssembly(); // Should be internal");

            Console.WriteLine();
        }

        /// <summary>
        /// Best practices for using public keyword
        /// </summary>
        static void BestPracticesExample()
        {
            Console.WriteLine("8. BEST PRACTICES FOR PUBLIC KEYWORD:");
            Console.WriteLine("   Guidelines for effective use of public access\n");

            Console.WriteLine("   ✅ WHEN TO USE PUBLIC:");
            Console.WriteLine("      • API contracts and interfaces");
            Console.WriteLine("      • Library classes and methods");
            Console.WriteLine("      • Data transfer objects (DTOs)");
            Console.WriteLine("      • Event declarations");
            Console.WriteLine("      • Constants and static readonly fields");
            Console.WriteLine("      • Extension methods");
            Console.WriteLine("      • Framework/infrastructure components");

            Console.WriteLine("\n   ❌ WHEN NOT TO USE PUBLIC:");
            Console.WriteLine("      • Internal implementation details");
            Console.WriteLine("      • Helper/utility methods used only internally");
            Console.WriteLine("      • Temporary or debug code");
            Console.WriteLine("      • Database connection strings or sensitive data");
            Console.WriteLine("      • Mutable fields (use properties instead)");
            Console.WriteLine("      • Methods that could break object invariants");

            Console.WriteLine("\n   🎯 DESIGN PRINCIPLES:");
            Console.WriteLine("      • Start with private, make public only when needed");
            Console.WriteLine("      • Favor properties over public fields");
            Console.WriteLine("      • Use interfaces for public contracts");
            Console.WriteLine("      • Document public APIs thoroughly");
            Console.WriteLine("      • Consider breaking changes when modifying public members");
            Console.WriteLine("      • Use internal for assembly-level access");

            Console.WriteLine();
        }
    }

    // ============================================================================
    // EXAMPLE CLASSES DEMONSTRATING PUBLIC KEYWORD USAGE
    // ============================================================================

    /// <summary>
    /// PUBLIC CLASS: Accessible from any assembly
    /// SCENARIO TO USE: API models, shared utilities, library classes
    /// </summary>
    public class Vehicle
    {
        // PUBLIC CONSTRUCTOR: Allows external instantiation
        public Vehicle(string make, string model, int year)
        {
            Make = make;
            Model = model;
            Year = year;
        }

        // PUBLIC PROPERTIES: Controlled access with validation
        public string Make { get; private set; }
        public string Model { get; private set; }
        public int Year { get; private set; }

        // PUBLIC METHOD: Core functionality exposed to external code
        public string GetDisplayName()
        {
            return $"{Year} {Make} {Model}";
        }

        // PUBLIC METHOD: Business logic that external code needs
        public bool IsVintage()
        {
            return DateTime.Now.Year - Year > 25;
        }
    }

    /// <summary>
    /// PUBLIC UTILITY CLASS: Mathematical operations
    /// SCENARIO TO USE: Shared functionality across multiple assemblies
    /// </summary>
    public class MathUtility
    {
        // PUBLIC METHODS: Core mathematical operations
        public double Add(double a, double b)
        {
            return a + b;
        }

        public double Subtract(double a, double b)
        {
            return a - b;
        }

        public double Multiply(double a, double b)
        {
            return a * b;
        }

        public double Divide(double a, double b)
        {
            if (b == 0)
                throw new DivideByZeroException("Cannot divide by zero");
            return a / b;
        }

        // PUBLIC STATIC METHOD: Utility function
        public static double CalculateCircleArea(double radius)
        {
            return Math.PI * radius * radius;
        }
    }

    /// <summary>
    /// PUBLIC CLASS: Demonstrates encapsulation with public properties
    /// SCENARIO TO USE: Domain objects with controlled state changes
    /// </summary>
    public class BankAccount
    {
        private decimal _balance;
        private static int _nextAccountNumber = 1000;

        public BankAccount(string accountHolder, decimal initialBalance)
        {
            AccountHolder = accountHolder;
            AccountNumber = _nextAccountNumber++;
            _balance = initialBalance >= 0 ? initialBalance : 0;
        }

        // PUBLIC PROPERTY: Read-only access to account holder
        public string AccountHolder { get; private set; }

        // PUBLIC PROPERTY: Read-only access to account number
        public int AccountNumber { get; private set; }

        // PUBLIC PROPERTY: Controlled access to balance
        public decimal Balance 
        { 
            get { return _balance; }
            private set { _balance = value; } // Private setter for internal control
        }

        // PUBLIC METHODS: Core banking operations
        public void Deposit(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Deposit amount must be positive");
            
            _balance += amount;
            Console.WriteLine($"   Deposited ${amount:F2}");
        }

        public bool Withdraw(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Withdrawal amount must be positive");
            
            if (amount > _balance)
            {
                Console.WriteLine($"   Insufficient funds for withdrawal of ${amount:F2}");
                return false;
            }

            _balance -= amount;
            Console.WriteLine($"   Withdrew ${amount:F2}");
            return true;
        }
    }

    /// <summary>
    /// PUBLIC CLASS: Configuration container
    /// SCENARIO TO USE: Application settings, constants
    /// </summary>
    public class SystemConfiguration
    {
        // PUBLIC STATIC READONLY FIELDS: Configuration constants
        public static readonly int MaxConnections = 100;
        public static readonly int DefaultTimeoutSeconds = 30;
        public static readonly string ApplicationName = "MyApplication";

        // PUBLIC CONST: Compile-time constants
        public const string Version = "1.0.0";
        public const int MaxRetryAttempts = 3;
    }

    /// <summary>
    /// PUBLIC STRUCT: Simple data container
    /// SCENARIO TO USE: Coordinate systems, simple value types
    /// NOTE: Public fields acceptable in structs for simple data
    /// </summary>
    public struct Point2D
    {
        // PUBLIC FIELDS: Acceptable for simple structs
        public int X;
        public int Y;

        public Point2D(int x, int y)
        {
            X = x;
            Y = y;
        }

        // PUBLIC METHOD: Utility operations
        public double DistanceFromOrigin()
        {
            return Math.Sqrt(X * X + Y * Y);
        }
    }

    /// <summary>
    /// PUBLIC CLASS: Data processing with events
    /// SCENARIO TO USE: Services that need to notify external code
    /// </summary>
    public class DataProcessor
    {
        // PUBLIC EVENT: Allows external subscription
        public event Action<string> DataProcessed;

        // PUBLIC METHOD: Core functionality
        public void ProcessData(string data)
        {
            Console.WriteLine($"   Processing data: {data}");
            
            // Simulate processing
            System.Threading.Thread.Sleep(100);
            
            // Notify subscribers
            DataProcessed?.Invoke(data);
        }
    }

    /// <summary>
    /// PUBLIC CLASS: File processing with events
    /// SCENARIO TO USE: Background services, progress notifications
    /// </summary>
    public class FileProcessor
    {
        // PUBLIC EVENT: File processing notifications
        public event Action<object, string> FileProcessed;

        // PUBLIC METHOD: File processing functionality
        public void ProcessFile(string fileName)
        {
            Console.WriteLine($"   Processing file: {fileName}");
            
            // Simulate file processing
            System.Threading.Thread.Sleep(50);
            
            // Raise event to notify subscribers
            OnFileProcessed(fileName);
        }

        // PROTECTED VIRTUAL METHOD: Allows derived classes to override event raising
        protected virtual void OnFileProcessed(string fileName)
        {
            FileProcessed?.Invoke(this, fileName);
        }
    }

    /// <summary>
    /// PUBLIC INTERFACE: Contract definition
    /// SCENARIO TO USE: API contracts, dependency injection, testability
    /// </summary>
    public interface IRepository<T>
    {
        // PUBLIC INTERFACE MEMBERS: All interface members are implicitly public
        void Add(T item);
        T GetById(int id);
        IEnumerable<T> GetAll();
        void Update(T item);
        void Delete(int id);
    }

    /// <summary>
    /// PUBLIC CLASS: Implements public interface
    /// SCENARIO TO USE: Concrete implementations of public contracts
    /// </summary>
    public class InMemoryRepository<T> : IRepository<T> where T : class
    {
        private readonly List<T> _items = new List<T>();

        // PUBLIC METHODS: Interface implementation must be public
        public void Add(T item)
        {
            _items.Add(item);
        }

        public T GetById(int id)
        {
            // Simplified implementation
            return _items.Count > id ? _items[id] : null;
        }

        public IEnumerable<T> GetAll()
        {
            return _items.AsReadOnly();
        }

        public void Update(T item)
        {
            // Simplified implementation
            Console.WriteLine("Item updated");
        }

        public void Delete(int id)
        {
            if (id >= 0 && id < _items.Count)
                _items.RemoveAt(id);
        }
    }
}
