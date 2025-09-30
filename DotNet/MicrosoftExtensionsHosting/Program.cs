/*
 * Microsoft.Extensions.Hosting Namespace - Comprehensive Examples
 * 
 * PURPOSE:
 * The Microsoft.Extensions.Hosting namespace provides a generic hosting framework for .NET applications.
 * It enables building long-running services, console applications, and background workers with built-in
 * dependency injection, configuration, logging, and application lifecycle management.
 * 
 * Core classes include:
 * - IHost: Main interface representing an application host
 * - HostBuilder: Builder pattern for configuring and creating hosts
 * - IHostedService: Interface for background services
 * - BackgroundService: Base class for long-running background tasks
 * - IHostApplicationLifetime: Controls application startup/shutdown
 * - IServiceCollection: Dependency injection container
 * - IConfiguration: Configuration management
 * 
 * SCENARIOS TO USE:
 * 1. Windows Services and Linux daemons
 * 2. Background task processing (message queues, scheduled jobs)
 * 3. Console applications with dependency injection
 * 4. Microservices and worker services
 * 5. ETL (Extract, Transform, Load) processes
 * 6. File monitoring and processing services
 * 7. Data synchronization services
 * 8. Health monitoring and alerting services
 * 9. API gateways and proxy services
 * 10. IoT device communication services
 * 11. Long-running data processing pipelines
 * 12. Timer-based automation tasks
 * 
 * SCENARIOS NOT TO USE:
 * 1. Simple one-time console utilities (overhead not justified)
 * 2. ASP.NET Core web applications (use WebApplication.CreateBuilder instead)
 * 3. Azure Functions (use Azure Functions runtime)
 * 4. WPF/WinForms desktop applications (different hosting model)
 * 5. Unity/Xamarin mobile apps (different dependency injection)
 * 6. Class libraries (not executable applications)
 * 7. Very lightweight scripts with minimal dependencies
 * 8. Applications requiring immediate startup (has initialization overhead)
 * 9. When you need full control over application lifecycle
 * 10. Memory-constrained embedded systems
 * 
 * MEMORY ALLOCATION:
 * - Host initialization: ~2-5MB baseline (DI container, configuration, logging)
 * - IServiceCollection: ~500KB-2MB depending on registered services
 * - Configuration providers: ~100-500KB per provider
 * - Logging framework: ~200KB-1MB depending on providers
 * - Each IHostedService: ~50-200KB base overhead
 * - Dependency injection proxies: ~1-5KB per service registration
 * - Background services: Variable based on work being performed
 * - Application lifetime events: ~10-50KB for event handlers
 * 
 * PERFORMANCE CONSIDERATIONS:
 * - Host startup time: 100-500ms depending on service count
 * - Memory footprint grows with service registrations
 * - Garbage collection pressure from background service allocations
 * - Thread pool usage for background services
 */

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace MicrosoftExtensionsHostingExamples
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("=== Microsoft.Extensions.Hosting Namespace Examples ===\n");

            try
            {
                // Example 1: Basic hosted service
                await BasicHostedServiceExample();
                Console.WriteLine();

                // Example 2: Background service with dependency injection
                await BackgroundServiceWithDIExample();
                Console.WriteLine();

                // Example 3: Multiple hosted services with configuration
                await MultipleServicesExample();
                Console.WriteLine();

                // Example 4: Application lifetime events
                await ApplicationLifetimeExample();
                Console.WriteLine();

                // Example 5: Hosted service with cancellation
                await ServiceWithCancellationExample();
                Console.WriteLine();

                // Example 6: Memory allocation analysis
                await MemoryAllocationExample();
                Console.WriteLine();

                // Example 7: Advanced configuration and logging
                await AdvancedConfigurationExample();
                Console.WriteLine();

                // Example 8: Service communication and coordination
                await ServiceCommunicationExample();
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unhandled exception: {ex.Message}");
            }
        }

        /*
         * EXAMPLE 1: Basic Hosted Service
         * 
         * PURPOSE: Demonstrates the simplest hosted service implementation
         * 
         * MEMORY ALLOCATION:
         * - Host: ~2-3MB baseline
         * - Service registration: ~1KB
         * - Service instance: ~500B-2KB depending on implementation
         */
        private static async Task BasicHostedServiceExample()
        {
            Console.WriteLine("1. Basic Hosted Service Example");
            Console.WriteLine("===============================");

            // Create and configure the host
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    // Register our hosted service
                    services.AddHostedService<SimpleTimerService>();
                })
                .ConfigureLogging(logging =>
                {
                    // Configure logging (optional)
                    logging.SetMinimumLevel(LogLevel.Information);
                    logging.AddConsole();
                })
                .Build();

            // Start the host and run for a limited time
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            
            try
            {
                await host.RunAsync(cts.Token);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Basic hosted service example completed");
            }
        }

        /*
         * EXAMPLE 2: Background Service with Dependency Injection
         * 
         * PURPOSE: Shows how to use dependency injection in background services
         * 
         * SCENARIOS TO USE:
         * - Services that need access to repositories, APIs, databases
         * - Complex business logic requiring multiple dependencies
         * - Services that need configuration or logging
         * 
         * MEMORY ALLOCATION:
         * - DI container: ~500KB-2MB depending on service count
         * - Service instances: Variable based on dependencies
         * - Scoped services: Created/disposed per operation cycle
         */
        private static async Task BackgroundServiceWithDIExample()
        {
            Console.WriteLine("2. Background Service with Dependency Injection Example");
            Console.WriteLine("======================================================");

            var host = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    // Register dependencies
                    services.AddSingleton<IDataRepository, InMemoryDataRepository>();
                    services.AddSingleton<INotificationService, ConsoleNotificationService>();
                    
                    // Register the background service
                    services.AddHostedService<DataProcessingService>();
                })
                .ConfigureLogging(logging =>
                {
                    logging.AddConsole();
                    logging.SetMinimumLevel(LogLevel.Information);
                })
                .Build();

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(8));
            
            try
            {
                await host.RunAsync(cts.Token);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Background service with DI example completed");
            }
        }

        /*
         * EXAMPLE 3: Multiple Hosted Services with Configuration
         * 
         * PURPOSE: Demonstrates running multiple services with configuration
         * 
         * MEMORY ALLOCATION:
         * - Configuration providers: ~100-500KB per provider
         * - Multiple service instances: Cumulative memory usage
         * - Service coordination: Additional overhead for communication
         */
        private static async Task MultipleServicesExample()
        {
            Console.WriteLine("3. Multiple Hosted Services with Configuration Example");
            Console.WriteLine("=====================================================");

            var host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, config) =>
                {
                    // Add configuration sources
                    config.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        {"TimerService:IntervalSeconds", "2"},
                        {"ProcessingService:BatchSize", "10"},
                        {"MonitoringService:CheckIntervalMs", "1000"}
                    });
                })
                .ConfigureServices((context, services) =>
                {
                    // Bind configuration sections
                    services.Configure<TimerServiceOptions>(
                        context.Configuration.GetSection("TimerService"));
                    services.Configure<ProcessingServiceOptions>(
                        context.Configuration.GetSection("ProcessingService"));

                    // Register multiple services
                    services.AddSingleton<ISharedState, SharedState>();
                    services.AddHostedService<ConfigurableTimerService>();
                    services.AddHostedService<ConfigurableProcessingService>();
                    services.AddHostedService<MonitoringService>();
                })
                .Build();

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            
            try
            {
                await host.RunAsync(cts.Token);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Multiple services example completed");
            }
        }

        /*
         * EXAMPLE 4: Application Lifetime Events
         * 
         * PURPOSE: Shows how to handle application startup, stopping, and stopped events
         * 
         * SCENARIOS TO USE:
         * - Resource initialization and cleanup
         * - Graceful shutdown procedures
         * - Health check registration
         * - Cache warming on startup
         * 
         * MEMORY ALLOCATION:
         * - Event handlers: ~10-50KB
         * - Lifetime management: ~100-200KB
         */
        private static async Task ApplicationLifetimeExample()
        {
            Console.WriteLine("4. Application Lifetime Events Example");
            Console.WriteLine("=====================================");

            var host = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddHostedService<LifetimeAwareService>();
                })
                .Build();

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(6));
            
            try
            {
                await host.RunAsync(cts.Token);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Application lifetime example completed");
            }
        }

        /*
         * EXAMPLE 5: Hosted Service with Cancellation
         * 
         * PURPOSE: Demonstrates proper cancellation handling in background services
         * 
         * SCENARIOS TO USE:
         * - Long-running operations that need graceful cancellation
         * - Services that make external API calls
         * - File processing that might take significant time
         * 
         * MEMORY ALLOCATION:
         * - CancellationToken: Minimal overhead (~100 bytes)
         * - Cancellation callbacks: ~50-200 bytes per callback
         */
        private static async Task ServiceWithCancellationExample()
        {
            Console.WriteLine("5. Hosted Service with Cancellation Example");
            Console.WriteLine("===========================================");

            var host = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddHostedService<CancellationAwareService>();
                })
                .Build();

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(7));
            
            try
            {
                await host.RunAsync(cts.Token);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Cancellation-aware service example completed");
            }
        }

        /*
         * EXAMPLE 6: Memory Allocation Analysis
         * 
         * PURPOSE: Analyze memory usage patterns in hosted services
         * 
         * MEMORY ALLOCATION DETAILS:
         * - Host baseline: ~2-5MB
         * - Service registrations: ~1-5KB per service
         * - DI container: ~500KB-2MB
         * - Background work: Variable based on service logic
         */
        private static async Task MemoryAllocationExample()
        {
            Console.WriteLine("6. Memory Allocation Analysis Example");
            Console.WriteLine("====================================");

            // Measure memory before host creation
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            
            long memoryBefore = GC.GetTotalMemory(false);
            Console.WriteLine($"Memory before host creation: {memoryBefore:N0} bytes");

            var host = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    // Register multiple services to show cumulative effect
                    services.AddSingleton<IDataRepository, InMemoryDataRepository>();
                    services.AddSingleton<INotificationService, ConsoleNotificationService>();
                    services.AddHostedService<MemoryAnalysisService>();
                })
                .Build();

            long memoryAfterHostCreation = GC.GetTotalMemory(false);
            Console.WriteLine($"Memory after host creation: {memoryAfterHostCreation:N0} bytes");
            Console.WriteLine($"Host creation overhead: {memoryAfterHostCreation - memoryBefore:N0} bytes");

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            
            long memoryAfterExecution = memoryAfterHostCreation;
            try
            {
                await host.RunAsync(cts.Token);
            }
            catch (OperationCanceledException)
            {
                // Measure memory after service execution
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                
                memoryAfterExecution = GC.GetTotalMemory(false);
                Console.WriteLine($"Memory after execution: {memoryAfterExecution:N0} bytes");
                Console.WriteLine($"Total memory usage: {memoryAfterExecution - memoryBefore:N0} bytes");
            }

            // Dispose host and measure cleanup
            host.Dispose();
            
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            
            long memoryAfterDisposal = GC.GetTotalMemory(false);
            Console.WriteLine($"Memory after host disposal: {memoryAfterDisposal:N0} bytes");
            Console.WriteLine($"Memory cleaned up: {memoryAfterExecution - memoryAfterDisposal:N0} bytes");
        }

        /*
         * EXAMPLE 7: Advanced Configuration and Logging
         * 
         * PURPOSE: Advanced host configuration with multiple providers
         * 
         * SCENARIOS TO USE:
         * - Complex configuration hierarchies
         * - Multiple logging providers
         * - Environment-specific settings
         * 
         * MEMORY ALLOCATION:
         * - Configuration: ~200KB-1MB depending on sources
         * - Logging providers: ~200KB-1MB per provider
         */
        private static async Task AdvancedConfigurationExample()
        {
            Console.WriteLine("7. Advanced Configuration and Logging Example");
            Console.WriteLine("============================================");

            var host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, config) =>
                {
                    // Multiple configuration sources
                    config.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        {"Database:ConnectionString", "Server=localhost;Database=TestDB"},
                        {"Api:BaseUrl", "https://api.example.com"},
                        {"Features:EnableAdvancedLogging", "true"}
                    });
                    
                    // Environment variables
                    config.AddEnvironmentVariables("MYAPP_");
                })
                .ConfigureLogging((context, logging) =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.AddDebug();
                    
                    // Conditional logging based on configuration
                    var enableAdvanced = context.Configuration.GetValue<bool>("Features:EnableAdvancedLogging");
                    if (enableAdvanced)
                    {
                        logging.SetMinimumLevel(LogLevel.Debug);
                        Console.WriteLine("Advanced logging enabled");
                    }
                    else
                    {
                        logging.SetMinimumLevel(LogLevel.Information);
                    }
                })
                .ConfigureServices((context, services) =>
                {
                    // Bind configuration to strongly-typed options
                    services.Configure<DatabaseOptions>(
                        context.Configuration.GetSection("Database"));
                    services.Configure<ApiOptions>(
                        context.Configuration.GetSection("Api"));
                    
                    services.AddHostedService<AdvancedConfigurationService>();
                })
                .Build();

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            
            try
            {
                await host.RunAsync(cts.Token);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Advanced configuration example completed");
            }
        }

        /*
         * EXAMPLE 8: Service Communication and Coordination
         * 
         * PURPOSE: Shows how services can communicate and coordinate work
         * 
         * SCENARIOS TO USE:
         * - Producer/consumer patterns
         * - Service orchestration
         * - Shared state management
         * - Event-driven architectures
         * 
         * MEMORY ALLOCATION:
         * - Shared state: Variable based on data size
         * - Event handlers: ~50-200 bytes per handler
         * - Message queues: Variable based on queue size
         */
        private static async Task ServiceCommunicationExample()
        {
            Console.WriteLine("8. Service Communication and Coordination Example");
            Console.WriteLine("================================================");

            var host = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    // Shared communication infrastructure
                    services.AddSingleton<IMessageBus, InMemoryMessageBus>();
                    services.AddSingleton<ISharedCounter, SharedCounter>();
                    
                    // Producer and consumer services
                    services.AddHostedService<ProducerService>();
                    services.AddHostedService<ConsumerService>();
                    services.AddHostedService<CoordinatorService>();
                })
                .Build();

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            
            try
            {
                await host.RunAsync(cts.Token);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Service communication example completed");
            }
        }
    }

    #region Example Services

    /*
     * Simple Timer Service - Basic IHostedService implementation
     * 
     * PURPOSE: Demonstrates the most basic hosted service pattern
     * MEMORY: ~500B-1KB base overhead
     */
    public class SimpleTimerService : IHostedService, IDisposable
    {
        private Timer? _timer;
        private int _executionCount = 0;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("SimpleTimerService starting...");
            
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
            
            return Task.CompletedTask;
        }

        private void DoWork(object? state)
        {
            var count = Interlocked.Increment(ref _executionCount);
            Console.WriteLine($"SimpleTimerService executing: {count}");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("SimpleTimerService stopping...");
            
            _timer?.Change(Timeout.Infinite, 0);
            
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }

    /*
     * Data Processing Background Service
     * 
     * PURPOSE: Shows dependency injection in background services
     * MEMORY: Variable based on injected dependencies
     */
    public class DataProcessingService : BackgroundService
    {
        private readonly IDataRepository _repository;
        private readonly INotificationService _notification;
        private readonly ILogger<DataProcessingService> _logger;

        public DataProcessingService(
            IDataRepository repository,
            INotificationService notification,
            ILogger<DataProcessingService> logger)
        {
            _repository = repository;
            _notification = notification;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("DataProcessingService started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Simulate data processing
                    var data = await _repository.GetPendingDataAsync();
                    
                    if (data.Any())
                    {
                        _logger.LogInformation($"Processing {data.Count} items");
                        
                        foreach (var item in data)
                        {
                            // Simulate processing work
                            await Task.Delay(100, stoppingToken);
                            await _repository.MarkAsProcessedAsync(item.Id);
                        }
                        
                        await _notification.NotifyAsync($"Processed {data.Count} items");
                    }
                    
                    await Task.Delay(2000, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in data processing");
                    await Task.Delay(5000, stoppingToken); // Back off on error
                }
            }
            
            _logger.LogInformation("DataProcessingService stopped");
        }
    }

    /*
     * Configurable Timer Service
     * 
     * PURPOSE: Shows configuration integration with hosted services
     * MEMORY: Configuration options add ~100-500 bytes
     */
    public class ConfigurableTimerService : BackgroundService
    {
        private readonly TimerServiceOptions _options;
        private readonly ILogger<ConfigurableTimerService> _logger;

        public ConfigurableTimerService(
            Microsoft.Extensions.Options.IOptions<TimerServiceOptions> options,
            ILogger<ConfigurableTimerService> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"ConfigurableTimerService started with {_options.IntervalSeconds}s interval");

            var interval = TimeSpan.FromSeconds(_options.IntervalSeconds);
            
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation($"ConfigurableTimerService tick at {DateTime.Now:HH:mm:ss}");
                
                try
                {
                    await Task.Delay(interval, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
            
            _logger.LogInformation("ConfigurableTimerService stopped");
        }
    }

    /*
     * Configurable Processing Service
     * 
     * PURPOSE: Another example of configuration-driven service
     */
    public class ConfigurableProcessingService : BackgroundService
    {
        private readonly ProcessingServiceOptions _options;
        private readonly ISharedState _sharedState;
        private readonly ILogger<ConfigurableProcessingService> _logger;

        public ConfigurableProcessingService(
            Microsoft.Extensions.Options.IOptions<ProcessingServiceOptions> options,
            ISharedState sharedState,
            ILogger<ConfigurableProcessingService> logger)
        {
            _options = options.Value;
            _sharedState = sharedState;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"ConfigurableProcessingService started with batch size {_options.BatchSize}");

            while (!stoppingToken.IsCancellationRequested)
            {
                var processedCount = _sharedState.IncrementProcessedCount(_options.BatchSize);
                _logger.LogInformation($"Processed batch of {_options.BatchSize}, total: {processedCount}");
                
                try
                {
                    await Task.Delay(3000, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
            
            _logger.LogInformation("ConfigurableProcessingService stopped");
        }
    }

    /*
     * Monitoring Service
     * 
     * PURPOSE: Shows service monitoring and health checking patterns
     */
    public class MonitoringService : BackgroundService
    {
        private readonly ISharedState _sharedState;
        private readonly ILogger<MonitoringService> _logger;

        public MonitoringService(ISharedState sharedState, ILogger<MonitoringService> logger)
        {
            _sharedState = sharedState;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("MonitoringService started");

            while (!stoppingToken.IsCancellationRequested)
            {
                var status = _sharedState.GetStatus();
                _logger.LogInformation($"System status: {status.ProcessedCount} items processed, " +
                                     $"Last update: {status.LastUpdate:HH:mm:ss}");
                
                try
                {
                    await Task.Delay(1000, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
            
            _logger.LogInformation("MonitoringService stopped");
        }
    }

    /*
     * Lifetime Aware Service
     * 
     * PURPOSE: Demonstrates application lifetime event handling
     */
    public class LifetimeAwareService : BackgroundService
    {
        private readonly IHostApplicationLifetime _lifetime;
        private readonly ILogger<LifetimeAwareService> _logger;

        public LifetimeAwareService(
            IHostApplicationLifetime lifetime,
            ILogger<LifetimeAwareService> logger)
        {
            _lifetime = lifetime;
            _logger = logger;
            
            // Register lifetime event handlers
            _lifetime.ApplicationStarted.Register(OnStarted);
            _lifetime.ApplicationStopping.Register(OnStopping);
            _lifetime.ApplicationStopped.Register(OnStopped);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("LifetimeAwareService executing...");

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation($"Service working at {DateTime.Now:HH:mm:ss}");
                
                try
                {
                    await Task.Delay(2000, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }

        private void OnStarted()
        {
            _logger.LogInformation("Application has started - performing startup tasks");
            // Perform startup initialization
        }

        private void OnStopping()
        {
            _logger.LogInformation("Application is stopping - performing cleanup");
            // Perform cleanup before shutdown
        }

        private void OnStopped()
        {
            _logger.LogInformation("Application has stopped");
            // Final cleanup
        }
    }

    /*
     * Cancellation Aware Service
     * 
     * PURPOSE: Shows proper cancellation handling in long-running operations
     */
    public class CancellationAwareService : BackgroundService
    {
        private readonly ILogger<CancellationAwareService> _logger;

        public CancellationAwareService(ILogger<CancellationAwareService> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("CancellationAwareService started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Simulate long-running work with cancellation support
                    await LongRunningOperationAsync(stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Operation was cancelled");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in long-running operation");
                    
                    // Wait before retrying, but respect cancellation
                    try
                    {
                        await Task.Delay(1000, stoppingToken);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                }
            }
            
            _logger.LogInformation("CancellationAwareService stopped gracefully");
        }

        private async Task LongRunningOperationAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting long-running operation");
            
            // Simulate work that checks for cancellation periodically
            for (int i = 0; i < 10; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                _logger.LogDebug($"Long operation step {i + 1}/10");
                await Task.Delay(500, cancellationToken);
            }
            
            _logger.LogInformation("Long-running operation completed");
        }
    }

    /*
     * Memory Analysis Service
     * 
     * PURPOSE: Service for analyzing memory allocation patterns
     */
    public class MemoryAnalysisService : BackgroundService
    {
        private readonly IDataRepository _repository;
        private readonly ILogger<MemoryAnalysisService> _logger;

        public MemoryAnalysisService(
            IDataRepository repository,
            ILogger<MemoryAnalysisService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("MemoryAnalysisService started");

            var iteration = 0;
            while (!stoppingToken.IsCancellationRequested && iteration < 3)
            {
                iteration++;
                
                long memoryBefore = GC.GetTotalMemory(false);
                
                // Simulate memory-intensive work
                var data = await _repository.GetLargeDataSetAsync();
                var processed = ProcessData(data);
                
                long memoryAfter = GC.GetTotalMemory(false);
                
                _logger.LogInformation($"Iteration {iteration}: Processed {processed} items, " +
                                     $"Memory usage: {memoryAfter - memoryBefore:N0} bytes");
                
                // Force garbage collection to see what gets cleaned up
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                
                long memoryAfterGC = GC.GetTotalMemory(false);
                _logger.LogInformation($"After GC: {memoryAfterGC:N0} bytes " +
                                     $"(cleaned up {memoryAfter - memoryAfterGC:N0} bytes)");
                
                try
                {
                    await Task.Delay(2000, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
            
            _logger.LogInformation("MemoryAnalysisService completed");
        }

        private int ProcessData(List<DataItem> data)
        {
            // Simulate data processing that creates temporary objects
            var processed = 0;
            foreach (var item in data)
            {
                var json = JsonSerializer.Serialize(item);
                var parsed = JsonSerializer.Deserialize<DataItem>(json);
                if (parsed != null)
                {
                    processed++;
                }
            }
            return processed;
        }
    }

    /*
     * Advanced Configuration Service
     * 
     * PURPOSE: Shows advanced configuration usage patterns
     */
    public class AdvancedConfigurationService : BackgroundService
    {
        private readonly DatabaseOptions _dbOptions;
        private readonly ApiOptions _apiOptions;
        private readonly ILogger<AdvancedConfigurationService> _logger;

        public AdvancedConfigurationService(
            Microsoft.Extensions.Options.IOptions<DatabaseOptions> dbOptions,
            Microsoft.Extensions.Options.IOptions<ApiOptions> apiOptions,
            ILogger<AdvancedConfigurationService> logger)
        {
            _dbOptions = dbOptions.Value;
            _apiOptions = apiOptions.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("AdvancedConfigurationService started");
            _logger.LogInformation($"Database: {_dbOptions.ConnectionString}");
            _logger.LogInformation($"API Base URL: {_apiOptions.BaseUrl}");

            var iteration = 0;
            while (!stoppingToken.IsCancellationRequested && iteration < 3)
            {
                iteration++;
                _logger.LogInformation($"Configuration-driven operation {iteration}");
                
                try
                {
                    await Task.Delay(1500, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
            
            _logger.LogInformation("AdvancedConfigurationService stopped");
        }
    }

    /*
     * Producer Service
     * 
     * PURPOSE: Demonstrates service communication - producer side
     */
    public class ProducerService : BackgroundService
    {
        private readonly IMessageBus _messageBus;
        private readonly ILogger<ProducerService> _logger;

        public ProducerService(IMessageBus messageBus, ILogger<ProducerService> logger)
        {
            _messageBus = messageBus;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ProducerService started");

            var messageId = 1;
            while (!stoppingToken.IsCancellationRequested)
            {
                var message = new Message
                {
                    Id = messageId++,
                    Content = $"Message from producer at {DateTime.Now:HH:mm:ss}",
                    Timestamp = DateTime.UtcNow
                };
                
                await _messageBus.PublishAsync(message);
                _logger.LogInformation($"Published message {message.Id}");
                
                try
                {
                    await Task.Delay(2000, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
            
            _logger.LogInformation("ProducerService stopped");
        }
    }

    /*
     * Consumer Service
     * 
     * PURPOSE: Demonstrates service communication - consumer side
     */
    public class ConsumerService : BackgroundService
    {
        private readonly IMessageBus _messageBus;
        private readonly ISharedCounter _counter;
        private readonly ILogger<ConsumerService> _logger;

        public ConsumerService(
            IMessageBus messageBus,
            ISharedCounter counter,
            ILogger<ConsumerService> logger)
        {
            _messageBus = messageBus;
            _counter = counter;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ConsumerService started");

            while (!stoppingToken.IsCancellationRequested)
            {
                var message = await _messageBus.ConsumeAsync(stoppingToken);
                
                if (message != null)
                {
                    _logger.LogInformation($"Consumed message {message.Id}: {message.Content}");
                    _counter.Increment();
                }
                
                try
                {
                    await Task.Delay(100, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
            
            _logger.LogInformation("ConsumerService stopped");
        }
    }

    /*
     * Coordinator Service
     * 
     * PURPOSE: Demonstrates service coordination and monitoring
     */
    public class CoordinatorService : BackgroundService
    {
        private readonly ISharedCounter _counter;
        private readonly ILogger<CoordinatorService> _logger;

        public CoordinatorService(ISharedCounter counter, ILogger<CoordinatorService> logger)
        {
            _counter = counter;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("CoordinatorService started");

            while (!stoppingToken.IsCancellationRequested)
            {
                var count = _counter.GetCount();
                _logger.LogInformation($"Coordinator: {count} messages processed so far");
                
                try
                {
                    await Task.Delay(3000, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
            
            _logger.LogInformation("CoordinatorService stopped");
        }
    }

    #endregion

    #region Configuration Classes

    public class TimerServiceOptions
    {
        public int IntervalSeconds { get; set; } = 5;
    }

    public class ProcessingServiceOptions
    {
        public int BatchSize { get; set; } = 1;
    }

    public class DatabaseOptions
    {
        public string ConnectionString { get; set; } = string.Empty;
    }

    public class ApiOptions
    {
        public string BaseUrl { get; set; } = string.Empty;
    }

    #endregion

    #region Data Models and Interfaces

    public class DataItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsProcessed { get; set; }
    }

    public class Message
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }

    public class SystemStatus
    {
        public int ProcessedCount { get; set; }
        public DateTime LastUpdate { get; set; }
    }

    public interface IDataRepository
    {
        Task<List<DataItem>> GetPendingDataAsync();
        Task<List<DataItem>> GetLargeDataSetAsync();
        Task MarkAsProcessedAsync(int id);
    }

    public interface INotificationService
    {
        Task NotifyAsync(string message);
    }

    public interface ISharedState
    {
        int IncrementProcessedCount(int increment = 1);
        SystemStatus GetStatus();
    }

    public interface IMessageBus
    {
        Task PublishAsync(Message message);
        Task<Message?> ConsumeAsync(CancellationToken cancellationToken);
    }

    public interface ISharedCounter
    {
        void Increment();
        int GetCount();
    }

    #endregion

    #region Service Implementations

    public class InMemoryDataRepository : IDataRepository
    {
        private readonly List<DataItem> _data;
        private readonly Random _random = new();

        public InMemoryDataRepository()
        {
            _data = Enumerable.Range(1, 20)
                .Select(i => new DataItem
                {
                    Id = i,
                    Name = $"Item {i}",
                    CreatedAt = DateTime.UtcNow.AddMinutes(-_random.Next(60)),
                    IsProcessed = false
                })
                .ToList();
        }

        public Task<List<DataItem>> GetPendingDataAsync()
        {
            var pending = _data.Where(x => !x.IsProcessed).Take(3).ToList();
            return Task.FromResult(pending);
        }

        public Task<List<DataItem>> GetLargeDataSetAsync()
        {
            // Simulate larger dataset for memory analysis
            var largeData = Enumerable.Range(1, 100)
                .Select(i => new DataItem
                {
                    Id = i,
                    Name = $"Large Dataset Item {i} with some additional data to increase memory usage",
                    CreatedAt = DateTime.UtcNow,
                    IsProcessed = false
                })
                .ToList();
            
            return Task.FromResult(largeData);
        }

        public Task MarkAsProcessedAsync(int id)
        {
            var item = _data.FirstOrDefault(x => x.Id == id);
            if (item != null)
            {
                item.IsProcessed = true;
            }
            return Task.CompletedTask;
        }
    }

    public class ConsoleNotificationService : INotificationService
    {
        public Task NotifyAsync(string message)
        {
            Console.WriteLine($"[NOTIFICATION] {message}");
            return Task.CompletedTask;
        }
    }

    public class SharedState : ISharedState
    {
        private int _processedCount = 0;
        private readonly object _lock = new();

        public int IncrementProcessedCount(int increment = 1)
        {
            lock (_lock)
            {
                _processedCount += increment;
                return _processedCount;
            }
        }

        public SystemStatus GetStatus()
        {
            lock (_lock)
            {
                return new SystemStatus
                {
                    ProcessedCount = _processedCount,
                    LastUpdate = DateTime.UtcNow
                };
            }
        }
    }

    public class InMemoryMessageBus : IMessageBus
    {
        private readonly Queue<Message> _messages = new();
        private readonly object _lock = new();

        public Task PublishAsync(Message message)
        {
            lock (_lock)
            {
                _messages.Enqueue(message);
            }
            return Task.CompletedTask;
        }

        public Task<Message?> ConsumeAsync(CancellationToken cancellationToken)
        {
            lock (_lock)
            {
                return Task.FromResult(_messages.Count > 0 ? _messages.Dequeue() : null);
            }
        }
    }

    public class SharedCounter : ISharedCounter
    {
        private int _count = 0;

        public void Increment()
        {
            Interlocked.Increment(ref _count);
        }

        public int GetCount()
        {
            return _count;
        }
    }

    #endregion
}
