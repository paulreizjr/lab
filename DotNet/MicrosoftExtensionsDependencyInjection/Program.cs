/*
 * Microsoft.Extensions.DependencyInjection Namespace - Comprehensive Examples
 * 
 * PURPOSE:
 * The Microsoft.Extensions.DependencyInjection namespace provides a built-in IoC (Inversion of Control)
 * container for .NET applications. It enables dependency injection patterns, promotes loose coupling,
 * improves testability, and manages object lifetimes automatically.
 * 
 * Core classes include:
 * - IServiceCollection: Interface for registering services
 * - ServiceCollection: Default implementation for service registration
 * - IServiceProvider: Interface for service resolution
 * - ServiceProvider: Default implementation for service resolution
 * - ServiceDescriptor: Describes how a service should be registered
 * - ServiceLifetime: Enum defining service lifetimes (Singleton, Scoped, Transient)
 * 
 * SCENARIOS TO USE:
 * 1. Loose coupling between classes and their dependencies
 * 2. Unit testing with mock dependencies
 * 3. Plugin architectures and modular applications
 * 4. Configuration-driven service selection
 * 5. Cross-cutting concerns (logging, caching, security)
 * 6. Repository and service layer patterns
 * 7. Factory pattern implementations
 * 8. Decorator pattern implementations
 * 9. Strategy pattern implementations
 * 10. Complex object graph construction
 * 11. Environment-specific implementations
 * 12. Multi-tenant applications
 * 13. Microservices with shared abstractions
 * 14. Clean architecture implementations
 * 
 * SCENARIOS NOT TO USE:
 * 1. Simple utilities with no dependencies (static classes)
 * 2. Value objects and data transfer objects (DTOs)
 * 3. Performance-critical paths requiring direct instantiation
 * 4. Very simple applications with minimal dependencies
 * 5. When you need specific constructor overloads
 * 6. Circular dependency scenarios (indicates design issues)
 * 7. When object creation order is critical
 * 8. Memory-constrained embedded systems
 * 9. When you need precise control over object disposal
 * 10. Simple console utilities or scripts
 * 
 * MEMORY ALLOCATION:
 * - ServiceCollection: ~1-2KB base + 100-200 bytes per registration
 * - ServiceProvider: ~2-5KB base + dependencies
 * - Singleton services: Created once, persist for application lifetime
 * - Scoped services: Created per scope, disposed when scope ends
 * - Transient services: Created every time requested (highest memory impact)
 * - Service resolution: ~100-500 bytes overhead per resolution
 * - Object graphs: Cumulative memory of all resolved dependencies
 * - Disposal tracking: Additional ~50-100 bytes per disposable service
 * 
 * PERFORMANCE CONSIDERATIONS:
 * - Service resolution time: ~10-100 microseconds depending on complexity
 * - First-time resolution slower due to compilation
 * - Compiled expressions cached for subsequent resolutions
 * - Deep object graphs increase resolution time
 * - Singleton lifetime offers best performance
 * - Avoid service location pattern (anti-pattern)
 */

using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace MicrosoftExtensionsDependencyInjectionExamples
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("=== Microsoft.Extensions.DependencyInjection Namespace Examples ===\n");

            try
            {
                // Example 1: Basic service registration and resolution
                BasicServiceRegistrationExample();
                Console.WriteLine();

                // Example 2: Service lifetimes (Singleton, Scoped, Transient)
                ServiceLifetimesExample();
                Console.WriteLine();

                // Example 3: Interface-based dependency injection
                InterfaceBasedDependencyInjectionExample();
                Console.WriteLine();

                // Example 4: Constructor injection with multiple dependencies
                ConstructorInjectionExample();
                Console.WriteLine();

                // Example 5: Factory pattern with dependency injection
                FactoryPatternExample();
                Console.WriteLine();

                // Example 6: Decorator pattern implementation
                DecoratorPatternExample();
                Console.WriteLine();

                // Example 7: Configuration-driven service selection
                ConfigurationDrivenServicesExample();
                Console.WriteLine();

                // Example 8: Generic services and open generics
                GenericServicesExample();
                Console.WriteLine();

                // Example 9: Service scoping and disposal
                ServiceScopingExample();
                Console.WriteLine();

                // Example 10: Memory allocation analysis
                MemoryAllocationAnalysisExample();
                Console.WriteLine();

                // Example 11: Advanced scenarios (multiple implementations, keyed services)
                AdvancedScenariosExample();
                Console.WriteLine();

                // Example 12: Performance comparison and best practices
                PerformanceComparisonExample();
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unhandled exception: {ex.Message}");
            }
        }

        /*
         * EXAMPLE 1: Basic Service Registration and Resolution
         * 
         * PURPOSE: Demonstrates the fundamental concepts of DI container usage
         * 
         * MEMORY ALLOCATION:
         * - ServiceCollection: ~1KB base
         * - Service registrations: ~100-200 bytes each
         * - ServiceProvider: ~2-3KB base
         */
        private static void BasicServiceRegistrationExample()
        {
            Console.WriteLine("1. Basic Service Registration and Resolution Example");
            Console.WriteLine("==================================================");

            // Create service collection (registry)
            var services = new ServiceCollection();

            // Register services with different approaches
            services.AddTransient<Calculator>(); // Register concrete class
            services.AddTransient<IMessageService, EmailService>(); // Register interface with implementation
            services.AddSingleton<ConfigurationSettings>(); // Singleton registration

            // Build the service provider (container)
            using var serviceProvider = services.BuildServiceProvider();

            // Resolve services
            var calculator = serviceProvider.GetRequiredService<Calculator>();
            var messageService = serviceProvider.GetRequiredService<IMessageService>();
            var config = serviceProvider.GetRequiredService<ConfigurationSettings>();

            // Use the services
            var result = calculator.Add(10, 20);
            Console.WriteLine($"Calculator result: {result}");

            messageService.SendMessage("Hello from DI container!");

            config.SetValue("AppName", "DI Example");
            Console.WriteLine($"Configuration: {config.GetValue("AppName")}");
        }

        /*
         * EXAMPLE 2: Service Lifetimes
         * 
         * PURPOSE: Demonstrates the three service lifetimes and their behavior
         * 
         * MEMORY ALLOCATION:
         * - Singleton: One instance for entire application (lowest memory usage)
         * - Scoped: One instance per scope (moderate memory usage)
         * - Transient: New instance every time (highest memory usage)
         */
        private static void ServiceLifetimesExample()
        {
            Console.WriteLine("2. Service Lifetimes Example");
            Console.WriteLine("============================");

            var services = new ServiceCollection();

            // Register same service with different lifetimes
            services.AddSingleton<ICounterService, CounterService>(provider => 
                new CounterService("Singleton"));
            services.AddScoped<IScopedCounterService, ScopedCounterService>();
            services.AddTransient<ITransientService, TransientService>();

            using var serviceProvider = services.BuildServiceProvider();

            Console.WriteLine("=== Singleton Lifetime ===");
            // Singleton: Same instance every time
            var singleton1 = serviceProvider.GetRequiredService<ICounterService>();
            var singleton2 = serviceProvider.GetRequiredService<ICounterService>();
            
            singleton1.Increment();
            singleton2.Increment();
            
            Console.WriteLine($"Singleton1 count: {singleton1.GetCount()}"); // Will be 2
            Console.WriteLine($"Singleton2 count: {singleton2.GetCount()}"); // Will be 2 (same instance)
            Console.WriteLine($"Same instance: {ReferenceEquals(singleton1, singleton2)}");

            Console.WriteLine("\n=== Scoped Lifetime ===");
            // Scoped: Same instance within a scope, different across scopes
            using (var scope1 = serviceProvider.CreateScope())
            {
                var scoped1a = scope1.ServiceProvider.GetRequiredService<IScopedCounterService>();
                var scoped1b = scope1.ServiceProvider.GetRequiredService<IScopedCounterService>();
                
                scoped1a.Increment();
                scoped1b.Increment();
                
                Console.WriteLine($"Scope1 - Service A count: {scoped1a.GetCount()}"); // Will be 2
                Console.WriteLine($"Scope1 - Service B count: {scoped1b.GetCount()}"); // Will be 2 (same instance)
                Console.WriteLine($"Scope1 - Same instance: {ReferenceEquals(scoped1a, scoped1b)}");
            }

            using (var scope2 = serviceProvider.CreateScope())
            {
                var scoped2 = scope2.ServiceProvider.GetRequiredService<IScopedCounterService>();
                scoped2.Increment();
                
                Console.WriteLine($"Scope2 - Service count: {scoped2.GetCount()}"); // Will be 1 (new instance)
            }

            Console.WriteLine("\n=== Transient Lifetime ===");
            // Transient: New instance every time
            var transient1 = serviceProvider.GetRequiredService<ITransientService>();
            var transient2 = serviceProvider.GetRequiredService<ITransientService>();
            
            Console.WriteLine($"Transient1 ID: {transient1.GetInstanceId()}");
            Console.WriteLine($"Transient2 ID: {transient2.GetInstanceId()}");
            Console.WriteLine($"Same instance: {ReferenceEquals(transient1, transient2)}"); // Will be false
        }

        /*
         * EXAMPLE 3: Interface-Based Dependency Injection
         * 
         * PURPOSE: Shows how interfaces enable loose coupling and testability
         * 
         * SCENARIOS TO USE:
         * - When you need multiple implementations
         * - For unit testing with mocks
         * - Plugin architectures
         * - Environment-specific implementations
         */
        private static void InterfaceBasedDependencyInjectionExample()
        {
            Console.WriteLine("3. Interface-Based Dependency Injection Example");
            Console.WriteLine("===============================================");

            var services = new ServiceCollection();

            // Register different implementations for different scenarios
            services.AddSingleton<ILogger, ConsoleLogger>();
            services.AddTransient<IDataRepository, InMemoryDataRepository>();
            services.AddTransient<INotificationService, EmailNotificationService>();
            
            // Register a service that depends on other services
            services.AddTransient<OrderService>();

            using var serviceProvider = services.BuildServiceProvider();

            // The OrderService will automatically get its dependencies injected
            var orderService = serviceProvider.GetRequiredService<OrderService>();
            
            orderService.ProcessOrder(new Order
            {
                Id = 1,
                CustomerName = "John Doe",
                Amount = 99.99m,
                Items = new[] { "Laptop", "Mouse" }
            });

            Console.WriteLine("\n=== Switching Implementations ===");
            
            // Show how easy it is to switch implementations
            var servicesWithDifferentImplementation = new ServiceCollection();
            servicesWithDifferentImplementation.AddSingleton<ILogger, FileLogger>();
            servicesWithDifferentImplementation.AddTransient<IDataRepository, DatabaseDataRepository>();
            servicesWithDifferentImplementation.AddTransient<INotificationService, SmsNotificationService>();
            servicesWithDifferentImplementation.AddTransient<OrderService>();

            using var alternativeProvider = servicesWithDifferentImplementation.BuildServiceProvider();
            var alternativeOrderService = alternativeProvider.GetRequiredService<OrderService>();
            
            alternativeOrderService.ProcessOrder(new Order
            {
                Id = 2,
                CustomerName = "Jane Smith",
                Amount = 149.99m,
                Items = new[] { "Keyboard", "Monitor" }
            });
        }

        /*
         * EXAMPLE 4: Constructor Injection with Multiple Dependencies
         * 
         * PURPOSE: Demonstrates complex dependency graphs and constructor injection
         * 
         * MEMORY ALLOCATION:
         * - Deep dependency trees increase memory usage
         * - Container caches resolved constructors for performance
         * - Each dependency adds to total object graph memory
         */
        private static void ConstructorInjectionExample()
        {
            Console.WriteLine("4. Constructor Injection with Multiple Dependencies Example");
            Console.WriteLine("==========================================================");

            var services = new ServiceCollection();

            // Build a complex dependency graph
            services.AddSingleton<IConfiguration, AppConfiguration>();
            services.AddSingleton<ILogger, ConsoleLogger>();
            services.AddTransient<IEmailService, SmtpEmailService>();
            services.AddTransient<IAuditService, AuditService>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IAuthenticationService, AuthenticationService>();
            
            // Service with multiple dependencies
            services.AddTransient<UserManagementService>();

            using var serviceProvider = services.BuildServiceProvider();

            // The container will resolve the entire dependency graph automatically
            var userService = serviceProvider.GetRequiredService<UserManagementService>();

            // Demonstrate the service working with all its dependencies
            var user = userService.CreateUser("john.doe@example.com", "John Doe");
            userService.AuthenticateUser("john.doe@example.com", "password123");

            Console.WriteLine($"Created user: {user.Email} (ID: {user.Id})");
        }

        /*
         * EXAMPLE 5: Factory Pattern with Dependency Injection
         * 
         * PURPOSE: Shows how to implement factory patterns using DI
         * 
         * SCENARIOS TO USE:
         * - When object creation logic is complex
         * - When you need different instances based on parameters
         * - For object pooling or caching scenarios
         */
        private static void FactoryPatternExample()
        {
            Console.WriteLine("5. Factory Pattern with Dependency Injection Example");
            Console.WriteLine("===================================================");

            var services = new ServiceCollection();

            // Register dependencies for our factories
            services.AddSingleton<ILogger, ConsoleLogger>();
            services.AddTransient<IConfiguration, AppConfiguration>();

            // Register different payment processors
            services.AddTransient<CreditCardProcessor>();
            services.AddTransient<PayPalProcessor>();
            services.AddTransient<BankTransferProcessor>();

            // Register the factory
            services.AddTransient<IPaymentProcessorFactory, PaymentProcessorFactory>();

            // Register service that uses the factory
            services.AddTransient<PaymentService>();

            using var serviceProvider = services.BuildServiceProvider();

            var paymentService = serviceProvider.GetRequiredService<PaymentService>();

            // Process different types of payments
            paymentService.ProcessPayment("credit_card", 100.00m, "4111111111111111");
            paymentService.ProcessPayment("paypal", 50.00m, "user@example.com");
            paymentService.ProcessPayment("bank_transfer", 200.00m, "ACC123456789");

            Console.WriteLine("\n=== Factory with Func<T> ===");
            
            // Alternative factory approach using Func<T>
            var servicesWithFunc = new ServiceCollection();
            servicesWithFunc.AddSingleton<ILogger, ConsoleLogger>();
            servicesWithFunc.AddTransient<ReportGenerator>();
            
            // Register factory function
            servicesWithFunc.AddTransient<Func<ReportGenerator>>(provider => 
                () => provider.GetRequiredService<ReportGenerator>());

            using var funcProvider = servicesWithFunc.BuildServiceProvider();
            var reportFactory = funcProvider.GetRequiredService<Func<ReportGenerator>>();

            // Create multiple report generators
            var report1 = reportFactory();
            var report2 = reportFactory();
            
            report1.GenerateReport("Sales Report", new[] { "Item1", "Item2" });
            report2.GenerateReport("Inventory Report", new[] { "Stock1", "Stock2" });
        }

        /*
         * EXAMPLE 6: Decorator Pattern Implementation
         * 
         * PURPOSE: Shows how to implement decorator pattern with DI
         * 
         * SCENARIOS TO USE:
         * - Cross-cutting concerns (logging, caching, retry logic)
         * - Adding functionality without modifying existing code
         * - Chain of responsibility patterns
         */
        private static void DecoratorPatternExample()
        {
            Console.WriteLine("6. Decorator Pattern Implementation Example");
            Console.WriteLine("===========================================");

            var services = new ServiceCollection();

            // Register the base service
            services.AddTransient<BasicDataService>();
            
            // Register decorators that enhance the base service
            services.AddTransient<IDataService>(provider =>
            {
                var baseService = provider.GetRequiredService<BasicDataService>();
                var logger = provider.GetRequiredService<ILogger>();
                
                // Chain decorators: Caching -> Logging -> Base Service
                var loggingDecorator = new LoggingDataServiceDecorator(baseService, logger);
                var cachingDecorator = new CachingDataServiceDecorator(loggingDecorator);
                
                return cachingDecorator;
            });

            services.AddSingleton<ILogger, ConsoleLogger>();

            using var serviceProvider = services.BuildServiceProvider();

            var dataService = serviceProvider.GetRequiredService<IDataService>();

            // First call - will go through all decorators to base service
            Console.WriteLine("=== First Call (Cache Miss) ===");
            var data1 = dataService.GetData("user123");
            Console.WriteLine($"Retrieved: {data1}");

            // Second call - should hit cache
            Console.WriteLine("\n=== Second Call (Cache Hit) ===");
            var data2 = dataService.GetData("user123");
            Console.WriteLine($"Retrieved: {data2}");

            // Different key - cache miss again
            Console.WriteLine("\n=== Different Key (Cache Miss) ===");
            var data3 = dataService.GetData("user456");
            Console.WriteLine($"Retrieved: {data3}");
        }

        /*
         * EXAMPLE 7: Configuration-Driven Service Selection
         * 
         * PURPOSE: Shows how to select service implementations based on configuration
         * 
         * SCENARIOS TO USE:
         * - Environment-specific implementations (dev/prod)
         * - Feature flags and A/B testing
         * - Multi-tenant applications
         */
        private static void ConfigurationDrivenServicesExample()
        {
            Console.WriteLine("7. Configuration-Driven Service Selection Example");
            Console.WriteLine("=================================================");

            // Simulate different environments
            var environments = new[] { "Development", "Staging", "Production" };

            foreach (var environment in environments)
            {
                Console.WriteLine($"\n=== {environment} Environment ===");
                
                var services = new ServiceCollection();
                services.AddSingleton<ILogger, ConsoleLogger>();

                // Configuration-driven service registration
                if (environment == "Development")
                {
                    services.AddTransient<IStorageService, LocalFileStorageService>();
                    services.AddTransient<IEmailService, FakeEmailService>();
                }
                else if (environment == "Staging")
                {
                    services.AddTransient<IStorageService, AzureBlobStorageService>();
                    services.AddTransient<IEmailService, FakeEmailService>();
                }
                else // Production
                {
                    services.AddTransient<IStorageService, AzureBlobStorageService>();
                    services.AddTransient<IEmailService, ProductionSmtpEmailService>();
                }

                services.AddTransient<DocumentProcessor>();

                using var serviceProvider = services.BuildServiceProvider();
                var processor = serviceProvider.GetRequiredService<DocumentProcessor>();

                processor.ProcessDocument("important-document.pdf", "This is test content");
            }
        }

        /*
         * EXAMPLE 8: Generic Services and Open Generics
         * 
         * PURPOSE: Demonstrates registration and resolution of generic services
         * 
         * MEMORY ALLOCATION:
         * - Generic services create type-specific instances
         * - Open generics enable flexible, type-safe registration
         * - Each closed generic type has separate registration
         */
        private static void GenericServicesExample()
        {
            Console.WriteLine("8. Generic Services and Open Generics Example");
            Console.WriteLine("=============================================");

            var services = new ServiceCollection();

            // Register specific generic services
            services.AddTransient<IRepository<User>, ConcreteUserRepository>();
            services.AddTransient<IRepository<Product>, ProductRepository>();

            // Register open generic (works for any T)
            services.AddTransient(typeof(IValidator<>), typeof(DefaultValidator<>));
            services.AddTransient(typeof(ICache<>), typeof(MemoryCache<>));

            // Register generic service that uses other generic services
            services.AddTransient(typeof(GenericService<>));

            using var serviceProvider = services.BuildServiceProvider();

            // Resolve specific generic services
            var userRepository = serviceProvider.GetRequiredService<IRepository<User>>();
            var productRepository = serviceProvider.GetRequiredService<IRepository<Product>>();

            // Use the repositories
            var user = new User { Id = 1, Email = "test@example.com", Name = "Test User" };
            var product = new Product { Id = 1, Name = "Test Product", Price = 29.99m };

            userRepository.Save(user);
            productRepository.Save(product);

            var retrievedUser = userRepository.GetById(1);
            var retrievedProduct = productRepository.GetById(1);

            Console.WriteLine($"Retrieved user: {retrievedUser?.Name}");
            Console.WriteLine($"Retrieved product: {retrievedProduct?.Name}");

            // Resolve open generic services
            var userValidator = serviceProvider.GetRequiredService<IValidator<User>>();
            var productValidator = serviceProvider.GetRequiredService<IValidator<Product>>();

            Console.WriteLine($"User validation: {userValidator.Validate(user)}");
            Console.WriteLine($"Product validation: {productValidator.Validate(product)}");

            // Resolve generic service
            var userService = serviceProvider.GetRequiredService<GenericService<User>>();
            userService.ProcessEntity(user);
        }

        /*
         * EXAMPLE 9: Service Scoping and Disposal
         * 
         * PURPOSE: Demonstrates proper scoping and automatic disposal of services
         * 
         * MEMORY ALLOCATION:
         * - Scoped services are disposed when scope ends
         * - Disposable transient services are tracked and disposed
         * - Proper disposal prevents memory leaks
         */
        private static void ServiceScopingExample()
        {
            Console.WriteLine("9. Service Scoping and Disposal Example");
            Console.WriteLine("=======================================");

            var services = new ServiceCollection();

            // Register disposable services with different lifetimes
            services.AddSingleton<SingletonDisposableService>();
            services.AddScoped<ScopedDisposableService>();
            services.AddTransient<TransientDisposableService>();

            using var serviceProvider = services.BuildServiceProvider();

            // Get singleton service (will live for entire application)
            var singleton = serviceProvider.GetRequiredService<SingletonDisposableService>();
            Console.WriteLine($"Created singleton service: {singleton.Id}");

            // Create first scope
            Console.WriteLine("\n=== Creating First Scope ===");
            using (var scope1 = serviceProvider.CreateScope())
            {
                var scoped1 = scope1.ServiceProvider.GetRequiredService<ScopedDisposableService>();
                var transient1 = scope1.ServiceProvider.GetRequiredService<TransientDisposableService>();
                var transient2 = scope1.ServiceProvider.GetRequiredService<TransientDisposableService>();

                Console.WriteLine($"Scope1 - Scoped service: {scoped1.Id}");
                Console.WriteLine($"Scope1 - Transient1 service: {transient1.Id}");
                Console.WriteLine($"Scope1 - Transient2 service: {transient2.Id}");
            } // Scoped and transient services will be disposed here
            Console.WriteLine("First scope disposed - scoped and transient services disposed");

            // Create second scope
            Console.WriteLine("\n=== Creating Second Scope ===");
            using (var scope2 = serviceProvider.CreateScope())
            {
                var scoped2 = scope2.ServiceProvider.GetRequiredService<ScopedDisposableService>();
                Console.WriteLine($"Scope2 - Scoped service: {scoped2.Id} (new instance)");
            } // This scope's services will be disposed here
            Console.WriteLine("Second scope disposed");

            Console.WriteLine($"\nSingleton service still alive: {singleton.Id}");
        } // ServiceProvider disposed here - singleton will be disposed

        /*
         * EXAMPLE 10: Memory Allocation Analysis
         * 
         * PURPOSE: Analyze memory usage patterns with dependency injection
         * 
         * MEMORY ALLOCATION DETAILS:
         * - Container overhead: ~2-5MB depending on registrations
         * - Service resolution caching reduces subsequent allocations
         * - Singleton services: One-time allocation
         * - Scoped services: Allocated per scope
         * - Transient services: Allocated on every resolution
         */
        private static void MemoryAllocationAnalysisExample()
        {
            Console.WriteLine("10. Memory Allocation Analysis Example");
            Console.WriteLine("=====================================");

            // Measure memory before container creation
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            
            long memoryBefore = GC.GetTotalMemory(false);
            Console.WriteLine($"Memory before container creation: {memoryBefore:N0} bytes");

            // Create container with various services
            var services = new ServiceCollection();
            
            // Register various service types
            services.AddSingleton<ILogger, ConsoleLogger>();
            services.AddScoped<IDataRepository, InMemoryDataRepository>();
            services.AddTransient<INotificationService, EmailNotificationService>();
            services.AddTransient<ITransientService, TransientService>();
            services.AddTransient<Calculator>();
            services.AddTransient<OrderService>();
            
            // Add some open generics
            services.AddTransient(typeof(IRepository<>), typeof(GenericRepository<>));
            services.AddTransient(typeof(IValidator<>), typeof(DefaultValidator<>));

            using var serviceProvider = services.BuildServiceProvider();
            
            long memoryAfterContainer = GC.GetTotalMemory(false);
            Console.WriteLine($"Memory after container creation: {memoryAfterContainer:N0} bytes");
            Console.WriteLine($"Container overhead: {memoryAfterContainer - memoryBefore:N0} bytes");

            // Test singleton allocation (should be allocated once)
            Console.WriteLine("\n=== Singleton Service Allocation ===");
            long memoryBeforeSingleton = GC.GetTotalMemory(false);
            
            var logger1 = serviceProvider.GetRequiredService<ILogger>();
            var logger2 = serviceProvider.GetRequiredService<ILogger>();
            var logger3 = serviceProvider.GetRequiredService<ILogger>();
            
            long memoryAfterSingleton = GC.GetTotalMemory(false);
            Console.WriteLine($"Singleton allocation (3 resolutions): {memoryAfterSingleton - memoryBeforeSingleton:N0} bytes");
            Console.WriteLine($"Same instance: {ReferenceEquals(logger1, logger2)}");

            // Test transient allocation (new instance each time)
            Console.WriteLine("\n=== Transient Service Allocation ===");
            long memoryBeforeTransient = GC.GetTotalMemory(false);
            
            var transient1 = serviceProvider.GetRequiredService<ITransientService>();
            var transient2 = serviceProvider.GetRequiredService<ITransientService>();
            var transient3 = serviceProvider.GetRequiredService<ITransientService>();
            
            long memoryAfterTransient = GC.GetTotalMemory(false);
            Console.WriteLine($"Transient allocation (3 resolutions): {memoryAfterTransient - memoryBeforeTransient:N0} bytes");
            Console.WriteLine($"Different instances: {!ReferenceEquals(transient1, transient2)}");

            // Test scoped allocation
            Console.WriteLine("\n=== Scoped Service Allocation ===");
            long memoryBeforeScoped = GC.GetTotalMemory(false);
            
            using (var scope = serviceProvider.CreateScope())
            {
                var scoped1 = scope.ServiceProvider.GetRequiredService<IDataRepository>();
                var scoped2 = scope.ServiceProvider.GetRequiredService<IDataRepository>();
                var scoped3 = scope.ServiceProvider.GetRequiredService<IDataRepository>();
                
                long memoryAfterScoped = GC.GetTotalMemory(false);
                Console.WriteLine($"Scoped allocation (3 resolutions): {memoryAfterScoped - memoryBeforeScoped:N0} bytes");
                Console.WriteLine($"Same instance in scope: {ReferenceEquals(scoped1, scoped2)}");
            }
            
            // Force GC and measure cleanup
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            
            long memoryAfterGC = GC.GetTotalMemory(false);
            Console.WriteLine($"Memory after scope disposal and GC: {memoryAfterGC:N0} bytes");

            // Measure complex object graph allocation
            Console.WriteLine("\n=== Complex Object Graph Allocation ===");
            long memoryBeforeComplex = GC.GetTotalMemory(false);
            
            // Create multiple complex services
            for (int i = 0; i < 10; i++)
            {
                var orderService = serviceProvider.GetRequiredService<OrderService>();
                // Use the service to ensure it's fully constructed
                _ = orderService.ToString();
            }
            
            long memoryAfterComplex = GC.GetTotalMemory(false);
            Console.WriteLine($"Complex services (10 instances): {memoryAfterComplex - memoryBeforeComplex:N0} bytes");
            Console.WriteLine($"Average per complex service: {(memoryAfterComplex - memoryBeforeComplex) / 10:N0} bytes");
        }

        /*
         * EXAMPLE 11: Advanced Scenarios
         * 
         * PURPOSE: Demonstrates advanced DI scenarios and patterns
         */
        private static void AdvancedScenariosExample()
        {
            Console.WriteLine("11. Advanced Scenarios Example");
            Console.WriteLine("==============================");

            var services = new ServiceCollection();

            // Multiple implementations of same interface
            services.AddTransient<INotificationService, EmailNotificationService>();
            services.AddTransient<INotificationService, SmsNotificationService>();
            services.AddTransient<INotificationService, PushNotificationService>();

            // Service that consumes all implementations
            services.AddTransient<MultiNotificationService>();

            // Conditional registration
            services.AddTransient<IPaymentProcessor>(provider =>
            {
                // Choose implementation based on some condition
                var config = provider.GetService<IConfiguration>();
                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
                
                return environment == "Production" 
                    ? new RealPaymentProcessor()
                    : new MockPaymentProcessor();
            });

            // Self-registration
            services.AddTransient<SelfRegisteringService>();

            using var serviceProvider = services.BuildServiceProvider();

            // Resolve service that gets all implementations
            var multiNotificationService = serviceProvider.GetRequiredService<MultiNotificationService>();
            multiNotificationService.NotifyAll("Hello from advanced DI!");

            // Resolve conditionally registered service
            var paymentProcessor = serviceProvider.GetRequiredService<IPaymentProcessor>();
            paymentProcessor.ProcessPayment(100.00m);

            // Resolve self-registering service
            var selfRegistering = serviceProvider.GetRequiredService<SelfRegisteringService>();
            selfRegistering.DoWork();

            // Resolve all implementations manually
            Console.WriteLine("\n=== Manual Resolution of All Implementations ===");
            var allNotificationServices = serviceProvider.GetServices<INotificationService>();
            foreach (var service in allNotificationServices)
            {
                service.SendNotification($"Message from {service.GetType().Name}");
            }
        }

        /*
         * EXAMPLE 12: Performance Comparison and Best Practices
         * 
         * PURPOSE: Compare DI performance vs manual instantiation and show best practices
         */
        private static void PerformanceComparisonExample()
        {
            Console.WriteLine("12. Performance Comparison and Best Practices Example");
            Console.WriteLine("====================================================");

            const int iterations = 10000;

            // Setup DI container
            var services = new ServiceCollection();
            services.AddTransient<Calculator>();
            services.AddSingleton<ILogger, ConsoleLogger>();
            using var serviceProvider = services.BuildServiceProvider();

            // Warm up the container (first resolution is slower)
            _ = serviceProvider.GetRequiredService<Calculator>();

            // Test DI resolution performance
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
            {
                var calculator = serviceProvider.GetRequiredService<Calculator>();
                _ = calculator.Add(1, 2);
            }
            stopwatch.Stop();
            var diTime = stopwatch.ElapsedMilliseconds;

            // Test manual instantiation performance
            var logger = new ConsoleLogger();
            stopwatch.Restart();
            for (int i = 0; i < iterations; i++)
            {
                var calculator = new Calculator();
                _ = calculator.Add(1, 2);
            }
            stopwatch.Stop();
            var manualTime = stopwatch.ElapsedMilliseconds;

            Console.WriteLine($"DI Resolution ({iterations:N0} iterations): {diTime}ms");
            Console.WriteLine($"Manual Creation ({iterations:N0} iterations): {manualTime}ms");
            Console.WriteLine($"DI Overhead: {((double)diTime / manualTime - 1) * 100:F1}%");

            Console.WriteLine("\n=== Best Practices Demonstration ===");

            // ✅ DO: Register interfaces, not concrete classes when possible
            var goodServices = new ServiceCollection();
            goodServices.AddTransient<IDataRepository, InMemoryDataRepository>();

            // ❌ DON'T: Avoid service locator pattern
            // var badExample = serviceProvider.GetRequiredService<SomeService>();
            // badExample.ProcessData(serviceProvider); // Passing container around

            // ✅ DO: Use appropriate lifetimes
            goodServices.AddSingleton<IConfiguration, AppConfiguration>(); // Stateless, expensive to create
            goodServices.AddScoped<IDataRepository, DatabaseDataRepository>(); // Per-request state
            goodServices.AddTransient<IValidator<User>, DefaultValidator<User>>(); // Lightweight, stateless

            // ✅ DO: Validate dependencies at startup
            try
            {
                using var provider = goodServices.BuildServiceProvider();
                provider.GetRequiredService<IDataRepository>(); // This will throw if dependencies can't be resolved
                Console.WriteLine("✅ Dependency validation passed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Dependency validation failed: {ex.Message}");
            }

            Console.WriteLine("\n=== Memory Management Best Practices ===");
            Console.WriteLine("- Use singleton lifetime for expensive, stateless services");
            Console.WriteLine("- Use scoped lifetime for per-request state");
            Console.WriteLine("- Use transient lifetime for lightweight, stateless services");
            Console.WriteLine("- Always dispose ServiceProvider when done");
            Console.WriteLine("- Be careful with capturing references to scoped services");
            Console.WriteLine("- Consider factory patterns for complex object creation");
        }
    }

    #region Service Interfaces and Implementations

    // Basic services for examples
    public class Calculator
    {
        public int Add(int a, int b) => a + b;
        public int Subtract(int a, int b) => a - b;
        public int Multiply(int a, int b) => a * b;
        public double Divide(int a, int b) => b != 0 ? (double)a / b : 0;
    }

    public class ConfigurationSettings
    {
        private readonly Dictionary<string, string> _settings = new();

        public void SetValue(string key, string value) => _settings[key] = value;
        public string GetValue(string key) => _settings.TryGetValue(key, out var value) ? value : string.Empty;
    }

    // Message services
    public interface IMessageService
    {
        void SendMessage(string message);
    }

    public class EmailService : IMessageService
    {
        public void SendMessage(string message)
        {
            Console.WriteLine($"[EMAIL] {message}");
        }
    }

    public class SmsService : IMessageService
    {
        public void SendMessage(string message)
        {
            Console.WriteLine($"[SMS] {message}");
        }
    }

    // Counter services for lifetime demonstration
    public interface ICounterService
    {
        void Increment();
        int GetCount();
    }

    public class CounterService : ICounterService
    {
        private int _count = 0;
        private readonly string _name;

        public CounterService(string name = "Default")
        {
            _name = name;
            Console.WriteLine($"CounterService created: {_name}");
        }

        public void Increment() => _count++;
        public int GetCount() => _count;
    }

    public interface IScopedCounterService
    {
        void Increment();
        int GetCount();
    }

    public class ScopedCounterService : IScopedCounterService
    {
        private int _count = 0;
        public void Increment() => _count++;
        public int GetCount() => _count;
    }

    public interface ITransientService
    {
        string GetInstanceId();
    }

    public class TransientService : ITransientService
    {
        private readonly string _instanceId = Guid.NewGuid().ToString()[..8];
        public string GetInstanceId() => _instanceId;
    }

    // Logging interfaces
    public interface ILogger
    {
        void Log(string message);
        void LogError(string message);
    }

    public class ConsoleLogger : ILogger
    {
        public void Log(string message)
        {
            Console.WriteLine($"[LOG] {message}");
        }

        public void LogError(string message)
        {
            Console.WriteLine($"[ERROR] {message}");
        }
    }

    public class FileLogger : ILogger
    {
        public void Log(string message)
        {
            Console.WriteLine($"[FILE LOG] {message}");
        }

        public void LogError(string message)
        {
            Console.WriteLine($"[FILE ERROR] {message}");
        }
    }

    // Data access interfaces
    public interface IDataRepository
    {
        void Save<T>(T entity) where T : class;
        T? GetById<T>(int id) where T : class;
        void Delete<T>(int id) where T : class;
    }

    public class InMemoryDataRepository : IDataRepository
    {
        private readonly Dictionary<Type, Dictionary<int, object>> _storage = new();

        public void Save<T>(T entity) where T : class
        {
            var type = typeof(T);
            if (!_storage.ContainsKey(type))
                _storage[type] = new Dictionary<int, object>();

            // Simulate getting ID from entity
            var id = GetEntityId(entity);
            _storage[type][id] = entity;
            
            Console.WriteLine($"[IN-MEMORY] Saved {type.Name} with ID {id}");
        }

        public T? GetById<T>(int id) where T : class
        {
            var type = typeof(T);
            if (_storage.TryGetValue(type, out var typeStorage) && 
                typeStorage.TryGetValue(id, out var entity))
            {
                Console.WriteLine($"[IN-MEMORY] Retrieved {type.Name} with ID {id}");
                return (T)entity;
            }
            return null;
        }

        public void Delete<T>(int id) where T : class
        {
            var type = typeof(T);
            if (_storage.TryGetValue(type, out var typeStorage))
            {
                typeStorage.Remove(id);
                Console.WriteLine($"[IN-MEMORY] Deleted {type.Name} with ID {id}");
            }
        }

        private int GetEntityId<T>(T entity) where T : class
        {
            // Simple reflection to get Id property
            var idProperty = typeof(T).GetProperty("Id");
            return idProperty?.GetValue(entity) as int? ?? 0;
        }
    }

    public class DatabaseDataRepository : IDataRepository
    {
        public void Save<T>(T entity) where T : class
        {
            Console.WriteLine($"[DATABASE] Saved {typeof(T).Name} to database");
        }

        public T? GetById<T>(int id) where T : class
        {
            Console.WriteLine($"[DATABASE] Retrieved {typeof(T).Name} with ID {id} from database");
            return default(T);
        }

        public void Delete<T>(int id) where T : class
        {
            Console.WriteLine($"[DATABASE] Deleted {typeof(T).Name} with ID {id} from database");
        }
    }

    // Notification services
    public interface INotificationService
    {
        void SendNotification(string message);
    }

    public class EmailNotificationService : INotificationService
    {
        public void SendNotification(string message)
        {
            Console.WriteLine($"[EMAIL NOTIFICATION] {message}");
        }
    }

    public class SmsNotificationService : INotificationService
    {
        public void SendNotification(string message)
        {
            Console.WriteLine($"[SMS NOTIFICATION] {message}");
        }
    }

    public class PushNotificationService : INotificationService
    {
        public void SendNotification(string message)
        {
            Console.WriteLine($"[PUSH NOTIFICATION] {message}");
        }
    }

    // Domain models
    public class Order
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string[] Items { get; set; } = Array.Empty<string>();
    }

    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }

    // Complex service with multiple dependencies
    public class OrderService
    {
        private readonly ILogger _logger;
        private readonly IDataRepository _repository;
        private readonly INotificationService _notificationService;

        public OrderService(
            ILogger logger,
            IDataRepository repository,
            INotificationService notificationService)
        {
            _logger = logger;
            _repository = repository;
            _notificationService = notificationService;
        }

        public void ProcessOrder(Order order)
        {
            _logger.Log($"Processing order {order.Id} for {order.CustomerName}");
            
            // Save order
            _repository.Save(order);
            
            // Send notification
            _notificationService.SendNotification($"Order {order.Id} processed successfully");
            
            _logger.Log($"Order {order.Id} completed");
        }
    }

    // Configuration and authentication services
    public interface IConfiguration
    {
        string GetConnectionString(string name);
        T GetValue<T>(string key);
    }

    public class AppConfiguration : IConfiguration
    {
        private readonly Dictionary<string, object> _config = new()
        {
            { "ConnectionStrings:Default", "Server=localhost;Database=App;Trusted_Connection=true;" },
            { "SmtpServer", "smtp.example.com" },
            { "SmtpPort", 587 }
        };

        public string GetConnectionString(string name)
        {
            return _config.TryGetValue($"ConnectionStrings:{name}", out var value) 
                ? value.ToString() ?? string.Empty 
                : string.Empty;
        }

        public T GetValue<T>(string key)
        {
            if (_config.TryGetValue(key, out var value) && value is T typedValue)
                return typedValue;
            return default(T)!;
        }
    }

    public interface IEmailService
    {
        void SendEmail(string to, string subject, string body);
    }

    public class SmtpEmailService : IEmailService
    {
        public void SendEmail(string to, string subject, string body)
        {
            Console.WriteLine($"[EMAIL] To: {to}, Subject: {subject}");
        }
    }

    public class FakeEmailService : IEmailService
    {
        public void SendEmail(string to, string subject, string body)
        {
            Console.WriteLine($"[FAKE EMAIL] To: {to}, Subject: {subject}");
        }
    }

    public class ProductionSmtpEmailService : IEmailService
    {
        public void SendEmail(string to, string subject, string body)
        {
            Console.WriteLine($"[SMTP EMAIL] To: {to}, Subject: {subject}");
        }
    }

    public interface IAuditService
    {
        void LogAction(string action, string user);
    }

    public class AuditService : IAuditService
    {
        private readonly ILogger _logger;

        public AuditService(ILogger logger)
        {
            _logger = logger;
        }

        public void LogAction(string action, string user)
        {
            _logger.Log($"[AUDIT] User {user} performed action: {action}");
        }
    }

    public interface IUserRepository
    {
        User CreateUser(string email, string name);
        User? GetUserByEmail(string email);
    }

    public class UserRepository : IUserRepository
    {
        private readonly ILogger _logger;
        private readonly List<User> _users = new();
        private int _nextId = 1;

        public UserRepository(ILogger logger)
        {
            _logger = logger;
        }

        public User CreateUser(string email, string name)
        {
            var user = new User { Id = _nextId++, Email = email, Name = name };
            _users.Add(user);
            _logger.Log($"Created user: {email}");
            return user;
        }

        public User? GetUserByEmail(string email)
        {
            return _users.FirstOrDefault(u => u.Email == email);
        }
    }

    public interface IAuthenticationService
    {
        bool AuthenticateUser(string email, string password);
    }

    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuditService _auditService;
        private readonly ILogger _logger;

        public AuthenticationService(
            IUserRepository userRepository,
            IAuditService auditService,
            ILogger logger)
        {
            _userRepository = userRepository;
            _auditService = auditService;
            _logger = logger;
        }

        public bool AuthenticateUser(string email, string password)
        {
            var user = _userRepository.GetUserByEmail(email);
            if (user != null)
            {
                // Simulate password validation
                _auditService.LogAction("Login", email);
                _logger.Log($"User {email} authenticated successfully");
                return true;
            }

            _logger.LogError($"Authentication failed for {email}");
            return false;
        }
    }

    public class UserManagementService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthenticationService _authService;
        private readonly IEmailService _emailService;
        private readonly ILogger _logger;

        public UserManagementService(
            IUserRepository userRepository,
            IAuthenticationService authService,
            IEmailService emailService,
            ILogger logger)
        {
            _userRepository = userRepository;
            _authService = authService;
            _emailService = emailService;
            _logger = logger;
        }

        public User CreateUser(string email, string name)
        {
            var user = _userRepository.CreateUser(email, name);
            _emailService.SendEmail(email, "Welcome!", $"Welcome {name}!");
            return user;
        }

        public bool AuthenticateUser(string email, string password)
        {
            return _authService.AuthenticateUser(email, password);
        }
    }

    // Factory pattern interfaces and implementations
    public interface IPaymentProcessor
    {
        void ProcessPayment(decimal amount);
    }

    public class CreditCardProcessor : IPaymentProcessor
    {
        private readonly ILogger _logger;

        public CreditCardProcessor(ILogger logger)
        {
            _logger = logger;
        }

        public void ProcessPayment(decimal amount)
        {
            _logger.Log($"Processing credit card payment: ${amount}");
        }
    }

    public class PayPalProcessor : IPaymentProcessor
    {
        private readonly ILogger _logger;

        public PayPalProcessor(ILogger logger)
        {
            _logger = logger;
        }

        public void ProcessPayment(decimal amount)
        {
            _logger.Log($"Processing PayPal payment: ${amount}");
        }
    }

    public class BankTransferProcessor : IPaymentProcessor
    {
        private readonly ILogger _logger;

        public BankTransferProcessor(ILogger logger)
        {
            _logger = logger;
        }

        public void ProcessPayment(decimal amount)
        {
            _logger.Log($"Processing bank transfer: ${amount}");
        }
    }

    public class RealPaymentProcessor : IPaymentProcessor
    {
        public void ProcessPayment(decimal amount)
        {
            Console.WriteLine($"[REAL PAYMENT] Processing ${amount}");
        }
    }

    public class MockPaymentProcessor : IPaymentProcessor
    {
        public void ProcessPayment(decimal amount)
        {
            Console.WriteLine($"[MOCK PAYMENT] Simulating payment of ${amount}");
        }
    }

    public interface IPaymentProcessorFactory
    {
        IPaymentProcessor CreateProcessor(string type);
    }

    public class PaymentProcessorFactory : IPaymentProcessorFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public PaymentProcessorFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IPaymentProcessor CreateProcessor(string type)
        {
            return type.ToLower() switch
            {
                "credit_card" => _serviceProvider.GetRequiredService<CreditCardProcessor>(),
                "paypal" => _serviceProvider.GetRequiredService<PayPalProcessor>(),
                "bank_transfer" => _serviceProvider.GetRequiredService<BankTransferProcessor>(),
                _ => throw new ArgumentException($"Unknown payment type: {type}")
            };
        }
    }

    public class PaymentService
    {
        private readonly IPaymentProcessorFactory _factory;
        private readonly ILogger _logger;

        public PaymentService(IPaymentProcessorFactory factory, ILogger logger)
        {
            _factory = factory;
            _logger = logger;
        }

        public void ProcessPayment(string paymentType, decimal amount, string details)
        {
            _logger.Log($"Processing {paymentType} payment for ${amount}");
            
            var processor = _factory.CreateProcessor(paymentType);
            processor.ProcessPayment(amount);
            
            _logger.Log($"Payment completed using {details}");
        }
    }

    public class ReportGenerator
    {
        private readonly ILogger _logger;
        private readonly string _instanceId = Guid.NewGuid().ToString()[..8];

        public ReportGenerator(ILogger logger)
        {
            _logger = logger;
        }

        public void GenerateReport(string reportName, string[] data)
        {
            _logger.Log($"Generating {reportName} (Instance: {_instanceId})");
            _logger.Log($"Report contains {data.Length} items");
        }
    }

    // Decorator pattern implementation
    public interface IDataService
    {
        string GetData(string key);
    }

    public class BasicDataService : IDataService
    {
        public string GetData(string key)
        {
            // Simulate expensive data retrieval
            Thread.Sleep(100);
            return $"Data for {key} retrieved at {DateTime.Now:HH:mm:ss.fff}";
        }
    }

    public class LoggingDataServiceDecorator : IDataService
    {
        private readonly IDataService _inner;
        private readonly ILogger _logger;

        public LoggingDataServiceDecorator(IDataService inner, ILogger logger)
        {
            _inner = inner;
            _logger = logger;
        }

        public string GetData(string key)
        {
            _logger.Log($"Getting data for key: {key}");
            var result = _inner.GetData(key);
            _logger.Log($"Retrieved data for key: {key}");
            return result;
        }
    }

    public class CachingDataServiceDecorator : IDataService
    {
        private readonly IDataService _inner;
        private readonly Dictionary<string, string> _cache = new();

        public CachingDataServiceDecorator(IDataService inner)
        {
            _inner = inner;
        }

        public string GetData(string key)
        {
            if (_cache.TryGetValue(key, out var cachedData))
            {
                Console.WriteLine($"[CACHE HIT] {key}");
                return cachedData;
            }

            Console.WriteLine($"[CACHE MISS] {key}");
            var data = _inner.GetData(key);
            _cache[key] = data;
            return data;
        }
    }

    // Storage services for configuration-driven example
    public interface IStorageService
    {
        void SaveFile(string fileName, string content);
    }

    public class LocalFileStorageService : IStorageService
    {
        public void SaveFile(string fileName, string content)
        {
            Console.WriteLine($"[LOCAL FILE] Saved {fileName} to local disk");
        }
    }

    public class AzureBlobStorageService : IStorageService
    {
        public void SaveFile(string fileName, string content)
        {
            Console.WriteLine($"[AZURE BLOB] Saved {fileName} to Azure Blob Storage");
        }
    }

    public class DocumentProcessor
    {
        private readonly IStorageService _storageService;
        private readonly IEmailService _emailService;
        private readonly ILogger _logger;

        public DocumentProcessor(
            IStorageService storageService,
            IEmailService emailService,
            ILogger logger)
        {
            _storageService = storageService;
            _emailService = emailService;
            _logger = logger;
        }

        public void ProcessDocument(string fileName, string content)
        {
            _logger.Log($"Processing document: {fileName}");
            _storageService.SaveFile(fileName, content);
            _emailService.SendEmail("admin@example.com", "Document Processed", $"Document {fileName} has been processed");
        }
    }

    // Generic services
    public interface IRepository<T> where T : class
    {
        void Save(T entity);
        T? GetById(int id);
    }

    public class ConcreteUserRepository : IRepository<User>
    {
        private readonly List<User> _users = new();

        public void Save(User entity)
        {
            var existing = _users.FirstOrDefault(u => u.Id == entity.Id);
            if (existing != null)
                _users.Remove(existing);
            _users.Add(entity);
            Console.WriteLine($"[USER REPO] Saved user: {entity.Name}");
        }

        public User? GetById(int id)
        {
            return _users.FirstOrDefault(u => u.Id == id);
        }
    }

    public class ProductRepository : IRepository<Product>
    {
        private readonly List<Product> _products = new();

        public void Save(Product entity)
        {
            var existing = _products.FirstOrDefault(p => p.Id == entity.Id);
            if (existing != null)
                _products.Remove(existing);
            _products.Add(entity);
            Console.WriteLine($"[PRODUCT REPO] Saved product: {entity.Name}");
        }

        public Product? GetById(int id)
        {
            return _products.FirstOrDefault(p => p.Id == id);
        }
    }

    public class GenericRepository<T> : IRepository<T> where T : class
    {
        private readonly List<T> _entities = new();

        public void Save(T entity)
        {
            _entities.Add(entity);
            Console.WriteLine($"[GENERIC REPO] Saved {typeof(T).Name}");
        }

        public T? GetById(int id)
        {
            // This is a simplified implementation
            return _entities.FirstOrDefault();
        }
    }

    public interface IValidator<T>
    {
        bool Validate(T entity);
    }

    public class DefaultValidator<T> : IValidator<T>
    {
        public bool Validate(T entity)
        {
            Console.WriteLine($"[VALIDATOR] Validating {typeof(T).Name}");
            return entity != null; // Simple validation
        }
    }

    public interface ICache<T>
    {
        void Set(string key, T value);
        T? Get(string key);
    }

    public class MemoryCache<T> : ICache<T>
    {
        private readonly Dictionary<string, T> _cache = new();

        public void Set(string key, T value)
        {
            _cache[key] = value;
            Console.WriteLine($"[CACHE] Set {typeof(T).Name} with key: {key}");
        }

        public T? Get(string key)
        {
            return _cache.TryGetValue(key, out var value) ? value : default(T);
        }
    }

    public class GenericService<T> where T : class
    {
        private readonly IRepository<T> _repository;
        private readonly IValidator<T> _validator;
        private readonly ICache<T> _cache;

        public GenericService(
            IRepository<T> repository,
            IValidator<T> validator,
            ICache<T> cache)
        {
            _repository = repository;
            _validator = validator;
            _cache = cache;
        }

        public void ProcessEntity(T entity)
        {
            if (_validator.Validate(entity))
            {
                _repository.Save(entity);
                _cache.Set($"{typeof(T).Name}_{DateTime.Now.Ticks}", entity);
                Console.WriteLine($"[GENERIC SERVICE] Processed {typeof(T).Name}");
            }
        }
    }

    // Disposable services for scoping example
    public class SingletonDisposableService : IDisposable
    {
        public string Id { get; } = Guid.NewGuid().ToString()[..8];

        public SingletonDisposableService()
        {
            Console.WriteLine($"Created SingletonDisposableService: {Id}");
        }

        public void Dispose()
        {
            Console.WriteLine($"Disposed SingletonDisposableService: {Id}");
        }
    }

    public class ScopedDisposableService : IDisposable
    {
        public string Id { get; } = Guid.NewGuid().ToString()[..8];

        public ScopedDisposableService()
        {
            Console.WriteLine($"Created ScopedDisposableService: {Id}");
        }

        public void Dispose()
        {
            Console.WriteLine($"Disposed ScopedDisposableService: {Id}");
        }
    }

    public class TransientDisposableService : IDisposable
    {
        public string Id { get; } = Guid.NewGuid().ToString()[..8];

        public TransientDisposableService()
        {
            Console.WriteLine($"Created TransientDisposableService: {Id}");
        }

        public void Dispose()
        {
            Console.WriteLine($"Disposed TransientDisposableService: {Id}");
        }
    }

    // Advanced scenario services
    public class MultiNotificationService
    {
        private readonly IEnumerable<INotificationService> _notificationServices;

        public MultiNotificationService(IEnumerable<INotificationService> notificationServices)
        {
            _notificationServices = notificationServices;
        }

        public void NotifyAll(string message)
        {
            foreach (var service in _notificationServices)
            {
                service.SendNotification(message);
            }
        }
    }

    public class SelfRegisteringService
    {
        public void DoWork()
        {
            Console.WriteLine("[SELF-REGISTERING] Doing important work");
        }
    }

    #endregion
}
