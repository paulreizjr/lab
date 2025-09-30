/*
 * Factory Method Design Pattern Examples
 * 
 * PURPOSE:
 * The Factory Method pattern provides an interface for creating objects in a superclass,
 * but allows subclasses to alter the type of objects that will be created. It promotes
 * loose coupling by eliminating the need to bind application-specific classes into the code.
 * 
 * KEY BENEFITS:
 * - Decouples object creation from object usage
 * - Follows the Open/Closed Principle (open for extension, closed for modification)
 * - Provides a consistent interface for creating related objects
 * - Enables runtime decision making about which concrete class to instantiate
 * - Supports the Single Responsibility Principle by separating creation logic
 */

using System;
using System.Collections.Generic;
using System.IO;

namespace FactoryMethodExamples
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Factory Method Design Pattern Examples ===\n");

            // Run all factory method examples
            BasicFactoryMethodExample();
            LoggerFactoryExample();
            DocumentFactoryExample();
            PaymentProcessorFactoryExample();
            DatabaseConnectionFactoryExample();
            MemoryAllocationExample();
            WhenToUseAndNotUseFactoryMethod();

            Console.WriteLine("\n=== End of Examples ===");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        #region Basic Factory Method Example

        /*
         * BASIC FACTORY METHOD PATTERN
         * 
         * SCENARIOS TO USE:
         * - When you need to create objects but don't know the exact types beforehand
         * - When object creation logic is complex and should be centralized
         * - When you want to provide a library of objects with a common interface
         * 
         * SCENARIOS NOT TO USE:
         * - When you only have one type of object to create (unnecessary complexity)
         * - When object creation is simple and doesn't vary
         * - When performance is critical and object creation overhead matters
         */
        static void BasicFactoryMethodExample()
        {
            Console.WriteLine("1. Basic Factory Method Example:");

            /*
             * MEMORY ALLOCATION NOTES:
             * - Factory method creates objects on demand (heap allocation)
             * - Each created object allocates memory for its instance variables
             * - Factory itself is typically a lightweight object or static method
             * - Polymorphism through interfaces/base classes has minimal overhead
             */

            // Using factory method to create different types of vehicles
            IVehicleFactory carFactory = new CarFactory();
            IVehicleFactory bikeFactory = new BikeFactory();
            IVehicleFactory truckFactory = new TruckFactory();

            // Create vehicles without knowing concrete types
            IVehicle car = carFactory.CreateVehicle();
            IVehicle bike = bikeFactory.CreateVehicle();
            IVehicle truck = truckFactory.CreateVehicle();

            Console.WriteLine("Vehicles created through factory methods:");
            car.Start();
            car.Drive();
            car.Stop();

            bike.Start();
            bike.Drive();
            bike.Stop();

            truck.Start();
            truck.Drive();
            truck.Stop();

            Console.WriteLine("Basic factory method example completed.\n");
        }

        // Product interface - defines what all concrete products must implement
        public interface IVehicle
        {
            void Start();
            void Drive();
            void Stop();
        }

        // Concrete products - specific implementations
        public class Car : IVehicle
        {
            public void Start() => Console.WriteLine("Car: Starting engine...");
            public void Drive() => Console.WriteLine("Car: Driving on road");
            public void Stop() => Console.WriteLine("Car: Stopping and parking");
        }

        public class Bike : IVehicle
        {
            public void Start() => Console.WriteLine("Bike: Kickstart activated");
            public void Drive() => Console.WriteLine("Bike: Riding on two wheels");
            public void Stop() => Console.WriteLine("Bike: Braking to stop");
        }

        public class Truck : IVehicle
        {
            public void Start() => Console.WriteLine("Truck: Diesel engine starting");
            public void Drive() => Console.WriteLine("Truck: Hauling heavy cargo");
            public void Stop() => Console.WriteLine("Truck: Air brakes engaged");
        }

        // Creator interface - declares the factory method
        public interface IVehicleFactory
        {
            IVehicle CreateVehicle(); // This is the factory method
        }

        // Concrete creators - implement the factory method
        public class CarFactory : IVehicleFactory
        {
            public IVehicle CreateVehicle()
            {
                Console.WriteLine("CarFactory: Creating a new car");
                return new Car(); // Factory decides which concrete class to instantiate
            }
        }

        public class BikeFactory : IVehicleFactory
        {
            public IVehicle CreateVehicle()
            {
                Console.WriteLine("BikeFactory: Creating a new bike");
                return new Bike();
            }
        }

        public class TruckFactory : IVehicleFactory
        {
            public IVehicle CreateVehicle()
            {
                Console.WriteLine("TruckFactory: Creating a new truck");
                return new Truck();
            }
        }

        #endregion

        #region Logger Factory Example

        /*
         * REAL-WORLD EXAMPLE: LOGGER FACTORY
         * 
         * SCENARIOS TO USE:
         * - Different logging destinations (file, console, database, cloud)
         * - Runtime configuration of logging behavior
         * - Plugin-based architectures where new loggers can be added
         * 
         * This example shows how Factory Method enables flexible logging strategies
         */
        static void LoggerFactoryExample()
        {
            Console.WriteLine("2. Logger Factory Example:");

            /*
             * MEMORY ALLOCATION NOTES:
             * - Each logger type may have different memory requirements
             * - File loggers may buffer data in memory before writing
             * - Database loggers might maintain connection pools
             * - Console loggers have minimal memory footprint
             */

            // Create different types of loggers through factory
            LoggerFactory factory = new LoggerFactory();

            ILogger consoleLogger = factory.CreateLogger(LoggerType.Console);
            ILogger fileLogger = factory.CreateLogger(LoggerType.File);
            ILogger databaseLogger = factory.CreateLogger(LoggerType.Database);

            // Use loggers polymorphically - same interface, different behavior
            List<ILogger> loggers = new List<ILogger> { consoleLogger, fileLogger, databaseLogger };

            foreach (var logger in loggers)
            {
                logger.Initialize();
                logger.Log("Application started", LogLevel.Info);
                logger.Log("Processing user request", LogLevel.Debug);
                logger.Log("Database connection failed", LogLevel.Error);
                logger.Shutdown();
                Console.WriteLine();
            }

            Console.WriteLine("Logger factory example completed.\n");
        }

        public enum LoggerType { Console, File, Database }
        public enum LogLevel { Debug, Info, Warning, Error }

        public interface ILogger
        {
            void Initialize();
            void Log(string message, LogLevel level);
            void Shutdown();
        }

        public class ConsoleLogger : ILogger
        {
            public void Initialize() => Console.WriteLine("ConsoleLogger: Initialized");
            
            public void Log(string message, LogLevel level)
            {
                Console.WriteLine($"ConsoleLogger [{level}]: {message}");
            }
            
            public void Shutdown() => Console.WriteLine("ConsoleLogger: Shutdown");
        }

        public class FileLogger : ILogger
        {
            private string logFilePath = "application.log";

            public void Initialize() => Console.WriteLine($"FileLogger: Initialized (writing to {logFilePath})");
            
            public void Log(string message, LogLevel level)
            {
                // In real implementation, this would write to file
                Console.WriteLine($"FileLogger [{level}]: {message} -> {logFilePath}");
            }
            
            public void Shutdown() => Console.WriteLine("FileLogger: Flushed buffers and closed file");
        }

        public class DatabaseLogger : ILogger
        {
            private string connectionString = "Server=localhost;Database=Logs;";

            public void Initialize() => Console.WriteLine($"DatabaseLogger: Connected to database");
            
            public void Log(string message, LogLevel level)
            {
                // In real implementation, this would insert into database
                Console.WriteLine($"DatabaseLogger [{level}]: {message} -> Database");
            }
            
            public void Shutdown() => Console.WriteLine("DatabaseLogger: Connection closed");
        }

        public class LoggerFactory
        {
            public ILogger CreateLogger(LoggerType type)
            {
                /*
                 * FACTORY METHOD IMPLEMENTATION:
                 * - Centralizes object creation logic
                 * - Can be easily extended with new logger types
                 * - Client code doesn't need to know about concrete classes
                 */
                return type switch
                {
                    LoggerType.Console => new ConsoleLogger(),
                    LoggerType.File => new FileLogger(),
                    LoggerType.Database => new DatabaseLogger(),
                    _ => throw new ArgumentException($"Unknown logger type: {type}")
                };
            }
        }

        #endregion

        #region Document Factory Example

        /*
         * DOCUMENT PROCESSING FACTORY
         * 
         * SCENARIOS TO USE:
         * - Multiple file formats with different processing requirements
         * - Plugin architectures for document readers/writers
         * - When new document types need to be added without changing existing code
         */
        static void DocumentFactoryExample()
        {
            Console.WriteLine("3. Document Factory Example:");

            /*
             * MEMORY ALLOCATION NOTES:
             * - Different document types have varying memory footprints
             * - PDF processors might load entire documents into memory
             * - CSV processors can stream data with minimal memory usage
             * - XML processors may build DOM trees (memory intensive)
             */

            string[] documentPaths = { "report.pdf", "data.csv", "config.xml" };

            foreach (string path in documentPaths)
            {
                try
                {
                    // Factory determines document type from file extension
                    IDocument document = DocumentFactory.CreateDocument(path);
                    
                    document.Open();
                    document.Process();
                    document.Close();
                    Console.WriteLine();
                }
                catch (NotSupportedException ex)
                {
                    Console.WriteLine($"Error: {ex.Message}\n");
                }
            }

            Console.WriteLine("Document factory example completed.\n");
        }

        public interface IDocument
        {
            void Open();
            void Process();
            void Close();
        }

        public class PdfDocument : IDocument
        {
            private string filePath;

            public PdfDocument(string path)
            {
                filePath = path;
            }

            public void Open() => Console.WriteLine($"PdfDocument: Opening PDF file {filePath}");
            public void Process() => Console.WriteLine("PdfDocument: Extracting text and images from PDF");
            public void Close() => Console.WriteLine("PdfDocument: Closing PDF file and releasing memory");
        }

        public class CsvDocument : IDocument
        {
            private string filePath;

            public CsvDocument(string path)
            {
                filePath = path;
            }

            public void Open() => Console.WriteLine($"CsvDocument: Opening CSV file {filePath}");
            public void Process() => Console.WriteLine("CsvDocument: Parsing comma-separated values");
            public void Close() => Console.WriteLine("CsvDocument: Closing CSV file stream");
        }

        public class XmlDocument : IDocument
        {
            private string filePath;

            public XmlDocument(string path)
            {
                filePath = path;
            }

            public void Open() => Console.WriteLine($"XmlDocument: Opening XML file {filePath}");
            public void Process() => Console.WriteLine("XmlDocument: Parsing XML structure and validating schema");
            public void Close() => Console.WriteLine("XmlDocument: Disposing XML parser and releasing resources");
        }

        public static class DocumentFactory
        {
            public static IDocument CreateDocument(string filePath)
            {
                /*
                 * FACTORY METHOD WITH LOGIC:
                 * - Uses file extension to determine document type
                 * - Could be extended to examine file headers or MIME types
                 * - Easily extensible for new document types
                 */
                string extension = Path.GetExtension(filePath).ToLower();

                return extension switch
                {
                    ".pdf" => new PdfDocument(filePath),
                    ".csv" => new CsvDocument(filePath),
                    ".xml" => new XmlDocument(filePath),
                    _ => throw new NotSupportedException($"Document type '{extension}' is not supported")
                };
            }
        }

        #endregion

        #region Payment Processor Factory Example

        /*
         * PAYMENT PROCESSING FACTORY
         * 
         * SCENARIOS TO USE:
         * - Multiple payment gateways (PayPal, Stripe, Square)
         * - Different payment methods (credit card, digital wallet, bank transfer)
         * - When payment processing logic varies significantly between providers
         */
        static void PaymentProcessorFactoryExample()
        {
            Console.WriteLine("4. Payment Processor Factory Example:");

            /*
             * MEMORY ALLOCATION NOTES:
             * - Each payment processor may maintain different connection state
             * - Credit card processors might cache encryption keys
             * - Digital wallet processors may store session tokens
             * - Bank transfer processors might maintain transaction queues
             */

            PaymentRequest[] requests = 
            {
                new PaymentRequest { Amount = 99.99m, Method = PaymentMethod.CreditCard },
                new PaymentRequest { Amount = 49.50m, Method = PaymentMethod.DigitalWallet },
                new PaymentRequest { Amount = 199.00m, Method = PaymentMethod.BankTransfer }
            };

            foreach (var request in requests)
            {
                IPaymentProcessor processor = PaymentProcessorFactory.CreateProcessor(request.Method);
                
                processor.Initialize();
                bool success = processor.ProcessPayment(request.Amount);
                processor.Cleanup();

                Console.WriteLine($"Payment result: {(success ? "Success" : "Failed")}\n");
            }

            Console.WriteLine("Payment processor factory example completed.\n");
        }

        public enum PaymentMethod { CreditCard, DigitalWallet, BankTransfer }

        public class PaymentRequest
        {
            public decimal Amount { get; set; }
            public PaymentMethod Method { get; set; }
        }

        public interface IPaymentProcessor
        {
            void Initialize();
            bool ProcessPayment(decimal amount);
            void Cleanup();
        }

        public class CreditCardProcessor : IPaymentProcessor
        {
            public void Initialize() => Console.WriteLine("CreditCardProcessor: Connecting to payment gateway");
            
            public bool ProcessPayment(decimal amount)
            {
                Console.WriteLine($"CreditCardProcessor: Processing ${amount:F2} credit card payment");
                Console.WriteLine("CreditCardProcessor: Validating card, checking funds, authorizing transaction");
                return true; // Simulate successful payment
            }
            
            public void Cleanup() => Console.WriteLine("CreditCardProcessor: Closing secure connection");
        }

        public class DigitalWalletProcessor : IPaymentProcessor
        {
            public void Initialize() => Console.WriteLine("DigitalWalletProcessor: Establishing secure session");
            
            public bool ProcessPayment(decimal amount)
            {
                Console.WriteLine($"DigitalWalletProcessor: Processing ${amount:F2} digital wallet payment");
                Console.WriteLine("DigitalWalletProcessor: Authenticating user, transferring funds from wallet");
                return true;
            }
            
            public void Cleanup() => Console.WriteLine("DigitalWalletProcessor: Terminating session and clearing tokens");
        }

        public class BankTransferProcessor : IPaymentProcessor
        {
            public void Initialize() => Console.WriteLine("BankTransferProcessor: Connecting to banking network");
            
            public bool ProcessPayment(decimal amount)
            {
                Console.WriteLine($"BankTransferProcessor: Processing ${amount:F2} bank transfer");
                Console.WriteLine("BankTransferProcessor: Initiating ACH transfer, queuing for batch processing");
                return true;
            }
            
            public void Cleanup() => Console.WriteLine("BankTransferProcessor: Disconnecting from banking network");
        }

        public static class PaymentProcessorFactory
        {
            public static IPaymentProcessor CreateProcessor(PaymentMethod method)
            {
                return method switch
                {
                    PaymentMethod.CreditCard => new CreditCardProcessor(),
                    PaymentMethod.DigitalWallet => new DigitalWalletProcessor(),
                    PaymentMethod.BankTransfer => new BankTransferProcessor(),
                    _ => throw new ArgumentException($"Unsupported payment method: {method}")
                };
            }
        }

        #endregion

        #region Database Connection Factory Example

        /*
         * DATABASE CONNECTION FACTORY
         * 
         * SCENARIOS TO USE:
         * - Multiple database providers (SQL Server, MySQL, PostgreSQL, Oracle)
         * - Environment-specific connections (development, staging, production)
         * - When connection logic varies significantly between database types
         */
        static void DatabaseConnectionFactoryExample()
        {
            Console.WriteLine("5. Database Connection Factory Example:");

            /*
             * MEMORY ALLOCATION NOTES:
             * - Database connections are expensive resources (network sockets, memory buffers)
             * - Connection pools manage and reuse connections to minimize allocation overhead
             * - Different databases may have different memory requirements for drivers
             * - Connection objects typically hold references to native resources
             */

            DatabaseType[] dbTypes = { DatabaseType.SqlServer, DatabaseType.MySQL, DatabaseType.PostgreSQL };

            foreach (var dbType in dbTypes)
            {
                IDatabaseConnection connection = DatabaseConnectionFactory.CreateConnection(dbType);
                
                connection.Connect();
                connection.ExecuteQuery("SELECT COUNT(*) FROM Users");
                connection.ExecuteCommand("INSERT INTO Logs (Message, Timestamp) VALUES ('Test', NOW())");
                connection.Disconnect();
                Console.WriteLine();
            }

            Console.WriteLine("Database connection factory example completed.\n");
        }

        public enum DatabaseType { SqlServer, MySQL, PostgreSQL }

        public interface IDatabaseConnection
        {
            void Connect();
            void ExecuteQuery(string sql);
            void ExecuteCommand(string sql);
            void Disconnect();
        }

        public class SqlServerConnection : IDatabaseConnection
        {
            private string connectionString = "Server=localhost;Database=MyApp;Integrated Security=true;";

            public void Connect() => Console.WriteLine($"SqlServerConnection: Connected to SQL Server");
            
            public void ExecuteQuery(string sql) => 
                Console.WriteLine($"SqlServerConnection: Executing query with T-SQL: {sql}");
            
            public void ExecuteCommand(string sql) => 
                Console.WriteLine($"SqlServerConnection: Executing command with T-SQL: {sql}");
            
            public void Disconnect() => Console.WriteLine("SqlServerConnection: Connection closed and returned to pool");
        }

        public class MySqlConnection : IDatabaseConnection
        {
            private string connectionString = "Server=localhost;Database=myapp;Uid=user;Pwd=password;";

            public void Connect() => Console.WriteLine($"MySqlConnection: Connected to MySQL");
            
            public void ExecuteQuery(string sql) => 
                Console.WriteLine($"MySqlConnection: Executing query with MySQL dialect: {sql}");
            
            public void ExecuteCommand(string sql) => 
                Console.WriteLine($"MySqlConnection: Executing command with MySQL dialect: {sql}");
            
            public void Disconnect() => Console.WriteLine("MySqlConnection: Connection closed");
        }

        public class PostgreSqlConnection : IDatabaseConnection
        {
            private string connectionString = "Host=localhost;Database=myapp;Username=user;Password=password;";

            public void Connect() => Console.WriteLine($"PostgreSqlConnection: Connected to PostgreSQL");
            
            public void ExecuteQuery(string sql) => 
                Console.WriteLine($"PostgreSqlConnection: Executing query with PostgreSQL syntax: {sql}");
            
            public void ExecuteCommand(string sql) => 
                Console.WriteLine($"PostgreSqlConnection: Executing command with PostgreSQL syntax: {sql}");
            
            public void Disconnect() => Console.WriteLine("PostgreSqlConnection: Connection closed");
        }

        public static class DatabaseConnectionFactory
        {
            public static IDatabaseConnection CreateConnection(DatabaseType dbType)
            {
                /*
                 * FACTORY WITH CONFIGURATION:
                 * - Could read from configuration files or environment variables
                 * - Might implement connection pooling logic
                 * - Can handle database-specific initialization requirements
                 */
                return dbType switch
                {
                    DatabaseType.SqlServer => new SqlServerConnection(),
                    DatabaseType.MySQL => new MySqlConnection(),
                    DatabaseType.PostgreSQL => new PostgreSqlConnection(),
                    _ => throw new ArgumentException($"Unsupported database type: {dbType}")
                };
            }
        }

        #endregion

        #region Memory Allocation Analysis

        /*
         * MEMORY ALLOCATION DEEP DIVE
         * 
         * PURPOSE: Detailed analysis of Factory Method pattern memory implications
         */
        static void MemoryAllocationExample()
        {
            Console.WriteLine("6. Memory Allocation Analysis:");

            /*
             * DETAILED MEMORY BREAKDOWN:
             * 
             * 1. FACTORY OBJECTS:
             *    - Static factories: No instance allocation (class metadata only)
             *    - Instance factories: Small objects (~24-48 bytes + fields)
             *    - Factory methods: No additional allocation (just method calls)
             * 
             * 2. PRODUCT OBJECTS:
             *    - Each created product allocates on heap
             *    - Size depends on instance fields and inherited data
             *    - Interface/base class polymorphism adds virtual table overhead (~8 bytes)
             * 
             * 3. MEMORY PATTERNS:
             *    - Factory Method doesn't change allocation patterns vs direct instantiation
             *    - Additional indirection may cause slight CPU overhead
             *    - Polymorphism enables better memory management (common cleanup code)
             * 
             * 4. GARBAGE COLLECTION:
             *    - Products created by factories are collected like any other objects
             *    - Factory instances (if any) are typically long-lived
             *    - Interface references don't prevent garbage collection
             */

            long memoryBefore = GC.GetTotalMemory(true);
            Console.WriteLine($"Memory before factory operations: {memoryBefore:N0} bytes");

            // Create multiple objects using factory methods
            const int numObjects = 10; // Reduced from 1000 to make output manageable
            var objects = new List<object>();

            // Create different types through factories to show allocation patterns
            VehicleFactoryRegistry registry = new VehicleFactoryRegistry();
            
            for (int i = 0; i < numObjects; i++)
            {
                // Cycle through different factory types
                var factoryType = (VehicleType)(i % 3);
                var vehicle = registry.CreateVehicle(factoryType);
                objects.Add(vehicle); // Keep reference to prevent GC
                
                // Only show some creation messages to avoid spam
                if (i < 3 || i >= numObjects - 3)
                {
                    Console.WriteLine($"Created {factoryType} via factory (object {i + 1})");
                }
                else if (i == 3)
                {
                    Console.WriteLine("... (creating more objects silently) ...");
                }
            }

            long memoryAfter = GC.GetTotalMemory(false);
            Console.WriteLine($"Memory after creating {numObjects} objects: {memoryAfter:N0} bytes");
            Console.WriteLine($"Memory increase: {memoryAfter - memoryBefore:N0} bytes");
            Console.WriteLine($"Average per object: {(memoryAfter - memoryBefore) / numObjects:N0} bytes");

            // Test memory usage of factory vs direct instantiation
            Console.WriteLine("\nComparing factory vs direct instantiation:");
            
            long directMemoryBefore = GC.GetTotalMemory(true);
            var directObjects = new List<Car>();
            for (int i = 0; i < numObjects; i++)
            {
                directObjects.Add(new Car()); // Direct instantiation
            }
            long directMemoryAfter = GC.GetTotalMemory(false);

            Console.WriteLine($"Direct instantiation memory: {directMemoryAfter - directMemoryBefore:N0} bytes");
            Console.WriteLine($"Factory method overhead: {(memoryAfter - memoryBefore) - (directMemoryAfter - directMemoryBefore):N0} bytes");

            // Clean up to show garbage collection
            objects.Clear();
            directObjects.Clear();
            objects = null;
            directObjects = null;
            
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            long memoryAfterCleanup = GC.GetTotalMemory(true);
            Console.WriteLine($"Memory after cleanup: {memoryAfterCleanup:N0} bytes");
            Console.WriteLine("Memory allocation analysis completed.\n");
        }

        public enum VehicleType { Car, Bike, Truck }

        public class VehicleFactoryRegistry
        {
            private readonly Dictionary<VehicleType, IVehicleFactory> _factories;

            public VehicleFactoryRegistry()
            {
                // Initialize factory registry - this shows how factories themselves use memory
                _factories = new Dictionary<VehicleType, IVehicleFactory>
                {
                    { VehicleType.Car, new CarFactory() },
                    { VehicleType.Bike, new BikeFactory() },
                    { VehicleType.Truck, new TruckFactory() }
                };
            }

            public IVehicle CreateVehicle(VehicleType type)
            {
                if (_factories.TryGetValue(type, out IVehicleFactory factory))
                {
                    return factory.CreateVehicle();
                }
                throw new ArgumentException($"Unknown vehicle type: {type}");
            }
        }

        #endregion

        #region When to Use and Not Use Factory Method

        /*
         * COMPREHENSIVE GUIDE: WHEN TO USE vs WHEN NOT TO USE FACTORY METHOD
         */
        static void WhenToUseAndNotUseFactoryMethod()
        {
            Console.WriteLine("7. When to Use vs Not Use Factory Method:");

            Console.WriteLine("✅ GOOD scenarios for Factory Method pattern:");
            Console.WriteLine("   • Multiple related classes that implement a common interface");
            Console.WriteLine("   • Object creation logic is complex or varies based on conditions");
            Console.WriteLine("   • Need to support plugin architectures or extensible frameworks");
            Console.WriteLine("   • Runtime determination of which concrete class to instantiate");
            Console.WriteLine("   • Want to centralize and standardize object creation");
            Console.WriteLine("   • Different object initialization requirements");
            Console.WriteLine("   • Testing scenarios where you need to inject mock objects");

            Console.WriteLine("\n❌ AVOID Factory Method when:");
            Console.WriteLine("   • Only one concrete class exists (unnecessary abstraction)");
            Console.WriteLine("   • Object creation is simple and unlikely to change");
            Console.WriteLine("   • Performance is critical and every method call matters");
            Console.WriteLine("   • The codebase is small and complexity isn't worth the benefit");
            Console.WriteLine("   • Direct instantiation is clearer and more straightforward");
            Console.WriteLine("   • You're creating value types or simple data structures");

            Console.WriteLine("\n🎯 ALTERNATIVES to consider:");
            Console.WriteLine("   • Abstract Factory: When you need families of related objects");
            Console.WriteLine("   • Builder Pattern: When objects have many optional parameters");
            Console.WriteLine("   • Dependency Injection: When you need better testability and inversion of control");
            Console.WriteLine("   • Simple Factory: When you don't need the full flexibility of Factory Method");
            Console.WriteLine("   • Object Pool: When object creation is expensive and objects are reusable");
            Console.WriteLine("   • Prototype Pattern: When you need to clone existing objects");

            Console.WriteLine("\n⚠️  COMMON FACTORY METHOD MISTAKES:");
            Console.WriteLine("   • Over-engineering simple object creation");
            Console.WriteLine("   • Creating factories for every class (violates YAGNI principle)");
            Console.WriteLine("   • Not making factory methods easily extensible");
            Console.WriteLine("   • Forgetting to handle unknown types gracefully");
            Console.WriteLine("   • Creating memory leaks by not properly disposing factory-created objects");
            Console.WriteLine("   • Making factory methods too complex (should delegate to constructors)");

            Console.WriteLine("\n💡 FACTORY METHOD BEST PRACTICES:");
            Console.WriteLine("   • Keep factory methods simple - delegate complex initialization to constructors");
            Console.WriteLine("   • Use enums or string constants for type parameters (avoid magic strings)");
            Console.WriteLine("   • Consider using generic factory methods where appropriate");
            Console.WriteLine("   • Document which exceptions factory methods can throw");
            Console.WriteLine("   • Make factories easily testable and mockable");
            Console.WriteLine("   • Consider thread safety if factories will be used concurrently");
            Console.WriteLine("   • Use dependency injection containers for complex factory scenarios");

            Console.WriteLine("\n🔧 REAL-WORLD FACTORY METHOD EXAMPLES:");
            Console.WriteLine("   • .NET's XmlReader.Create() method");
            Console.WriteLine("   • Database connection factories (SqlConnection, OracleConnection)");
            Console.WriteLine("   • Web API controller factories");
            Console.WriteLine("   • Logging frameworks (NLog, Serilog target factories)");
            Console.WriteLine("   • Serialization libraries (JSON, XML formatter factories)");
            Console.WriteLine("   • Game development (enemy spawning, item creation)");
            Console.WriteLine("   • UI frameworks (control factories, theme providers)");

            Console.WriteLine("\nFactory Method guidance completed.");
        }

        #endregion
    }
}
