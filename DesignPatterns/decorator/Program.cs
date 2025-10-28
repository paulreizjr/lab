using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Diagnostics;

/*
 * DECORATOR DESIGN PATTERN IN C#
 * 
 * PURPOSE:
 * The Decorator pattern allows behavior to be added to objects dynamically without altering
 * their structure. It provides a flexible alternative to subclassing for extending functionality
 * by wrapping objects in decorator classes that add new behaviors while maintaining the same interface.
 * 
 * CORE BENEFITS:
 * - Adds functionality to objects without modifying their structure
 * - Provides flexible alternative to inheritance for extending behavior
 * - Allows multiple decorators to be stacked for complex behavior combinations
 * - Follows Single Responsibility Principle by separating core functionality from decorations
 * - Enables runtime composition of behaviors
 * - Supports the Open/Closed Principle - open for extension, closed for modification
 * - Allows selective application of features to specific object instances
 * - Eliminates the need for complex inheritance hierarchies
 * 
 * SCENARIOS TO USE:
 * - Adding optional features to objects (encryption, compression, logging)
 * - UI components with various visual decorations (borders, shadows, effects)
 * - Stream processing with filters (buffering, encryption, compression)
 * - Middleware patterns in web applications (authentication, caching, logging)
 * - Adding cross-cutting concerns (logging, metrics, retry logic)
 * - Configuration and feature toggles for objects
 * - Protocol layers in networking (encryption, compression, routing)
 * - Text formatting with multiple styles (bold, italic, underline)
 * - Caching layers with different strategies
 * - Input validation and transformation pipelines
 * - Game objects with temporary power-ups or effects
 * - Financial calculations with various fees and discounts
 * 
 * SCENARIOS NOT TO USE:
 * - When the core object interface changes frequently
 * - Simple static behavior that doesn't require runtime flexibility
 * - Performance-critical code where wrapper overhead is unacceptable
 * - When inheritance would be simpler and sufficient
 * - Objects with tightly coupled components that can't be separated
 * - When decorator chains become too complex to understand or debug
 * - Memory-constrained environments where wrapper objects are too expensive
 * - When the pattern creates too many small classes reducing maintainability
 */

namespace DecoratorPattern
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== Decorator Design Pattern Examples ===\n");

            // Example 1: Coffee Shop Ordering System
            CoffeeShopExample();

            // Example 2: Text Processing Pipeline
            TextProcessingExample();

            // Example 3: Stream Processing Decorators
            StreamProcessingExample();

            // Example 4: Web Request Middleware
            await WebMiddlewareExample();

            // Example 5: UI Component Decorators
            UIComponentExample();

            // Example 6: Thread-Safe Decorator Pattern
            await ThreadSafeDecoratorExample();

            // Example 7: Data Transformation Pipeline
            DataTransformationExample();

            // Example 8: Gaming Power-up System
            GamingPowerUpExample();

            // Example 9: Notification System
            NotificationSystemExample();

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        #region Example 1: Coffee Shop Ordering System

        static void CoffeeShopExample()
        {
            Console.WriteLine("1. Coffee Shop Ordering System:");
            Console.WriteLine("================================");

            // Start with a simple coffee
            ICoffee coffee = new SimpleCoffee();
            Console.WriteLine($"Order: {coffee.GetDescription()}");
            Console.WriteLine($"Cost: ${coffee.GetCost():F2}\n");

            // Add milk
            coffee = new MilkDecorator(coffee);
            Console.WriteLine($"Order: {coffee.GetDescription()}");
            Console.WriteLine($"Cost: ${coffee.GetCost():F2}\n");

            // Add sugar
            coffee = new SugarDecorator(coffee);
            Console.WriteLine($"Order: {coffee.GetDescription()}");
            Console.WriteLine($"Cost: ${coffee.GetCost():F2}\n");

            // Add whipped cream
            coffee = new WhippedCreamDecorator(coffee);
            Console.WriteLine($"Order: {coffee.GetDescription()}");
            Console.WriteLine($"Cost: ${coffee.GetCost():F2}\n");

            // Create a more complex order
            ICoffee complexCoffee = new SimpleCoffee();
            complexCoffee = new MilkDecorator(complexCoffee);
            complexCoffee = new SugarDecorator(complexCoffee);
            complexCoffee = new VanillaDecorator(complexCoffee);
            complexCoffee = new WhippedCreamDecorator(complexCoffee);
            
            Console.WriteLine("Complex Order:");
            Console.WriteLine($"Description: {complexCoffee.GetDescription()}");
            Console.WriteLine($"Final Cost: ${complexCoffee.GetCost():F2}");
            Console.WriteLine($"Calories: {complexCoffee.GetCalories()}");

            Console.WriteLine();
        }

        #endregion

        #region Example 2: Text Processing Pipeline

        static void TextProcessingExample()
        {
            Console.WriteLine("2. Text Processing Pipeline:");
            Console.WriteLine("=============================");

            string originalText = "This is a sample text for processing.";
            Console.WriteLine($"Original: {originalText}");

            // Create text processor with various decorators
            ITextProcessor processor = new BasicTextProcessor(originalText);
            
            // Add uppercase transformation
            processor = new UppercaseDecorator(processor);
            Console.WriteLine($"Uppercase: {processor.Process()}");

            // Add trimming
            processor = new TrimDecorator(processor);
            Console.WriteLine($"Trimmed: '{processor.Process()}'");

            // Add prefix and suffix
            processor = new PrefixDecorator(processor, "[PROCESSED] ");
            processor = new SuffixDecorator(processor, " [END]");
            Console.WriteLine($"With markers: {processor.Process()}");

            // Create a different pipeline
            ITextProcessor pipeline = new BasicTextProcessor("  hello world  ");
            pipeline = new TrimDecorator(pipeline);
            pipeline = new CapitalizeDecorator(pipeline);
            pipeline = new ReverseDecorator(pipeline);
            
            Console.WriteLine($"\nPipeline result: {pipeline.Process()}");
            Console.WriteLine($"Processing steps: {pipeline.GetProcessingSteps()}");

            Console.WriteLine();
        }

        #endregion

        #region Example 3: Stream Processing Decorators

        static void StreamProcessingExample()
        {
            Console.WriteLine("3. Stream Processing Decorators:");
            Console.WriteLine("=================================");

            var data = "This is sensitive data that needs to be processed securely.";
            Console.WriteLine($"Original data: {data}");

            // Create a basic data stream
            IDataStream stream = new BasicDataStream(data);
            
            // Add compression
            stream = new CompressionDecorator(stream);
            Console.WriteLine($"Compressed size: {stream.GetSize()} bytes");

            // Add encryption
            stream = new EncryptionDecorator(stream);
            Console.WriteLine($"Encrypted data: {stream.Read().Substring(0, Math.Min(50, stream.Read().Length))}...");

            // Add logging
            stream = new LoggingDecorator(stream);
            var processedData = stream.Read();
            Console.WriteLine($"Logged and processed data retrieved");

            // Add caching
            stream = new CachingDecorator(stream);
            var cachedData1 = stream.Read();
            var cachedData2 = stream.Read(); // This should be served from cache
            
            Console.WriteLine($"Cache performance: First read vs Second read");
            Console.WriteLine($"Data integrity check: {cachedData1 == cachedData2}");

            Console.WriteLine();
        }

        #endregion

        #region Example 4: Web Request Middleware

        static async Task WebMiddlewareExample()
        {
            Console.WriteLine("4. Web Request Middleware:");
            Console.WriteLine("===========================");

            // Create request context
            var request = new HttpRequestContext
            {
                Method = "GET",
                Path = "/api/users/123",
                Headers = new Dictionary<string, string>
                {
                    ["Authorization"] = "Bearer token123",
                    ["Content-Type"] = "application/json"
                }
            };

            Console.WriteLine($"Processing request: {request.Method} {request.Path}");

            // Build middleware pipeline
            IRequestHandler handler = new BasicRequestHandler();
            
            // Add authentication middleware
            handler = new AuthenticationMiddleware(handler);
            
            // Add logging middleware
            handler = new LoggingMiddleware(handler);
            
            // Add rate limiting middleware
            handler = new RateLimitingMiddleware(handler);
            
            // Add caching middleware
            handler = new CachingMiddleware(handler);
            
            // Process the request
            var response = await handler.HandleAsync(request);
            
            Console.WriteLine($"Response Status: {response.StatusCode}");
            Console.WriteLine($"Response Body: {response.Body}");
            Console.WriteLine($"Processing Time: {response.ProcessingTimeMs}ms");

            Console.WriteLine();
        }

        #endregion

        #region Example 5: UI Component Decorators

        static void UIComponentExample()
        {
            Console.WriteLine("5. UI Component Decorators:");
            Console.WriteLine("============================");

            // Create a basic button
            IUIComponent button = new BasicButton("Click Me", 100, 40);
            Console.WriteLine("Basic button:");
            button.Render();
            Console.WriteLine($"Size: {button.GetWidth()}x{button.GetHeight()}\n");

            // Add border decoration
            button = new BorderDecorator(button, 2, "solid");
            Console.WriteLine("With border:");
            button.Render();
            Console.WriteLine($"Size: {button.GetWidth()}x{button.GetHeight()}\n");

            // Add shadow decoration
            button = new ShadowDecorator(button, 5, "gray");
            Console.WriteLine("With shadow:");
            button.Render();
            Console.WriteLine($"Size: {button.GetWidth()}x{button.GetHeight()}\n");

            // Add background decoration
            button = new BackgroundDecorator(button, "blue");
            Console.WriteLine("With background:");
            button.Render();
            Console.WriteLine($"Size: {button.GetWidth()}x{button.GetHeight()}\n");

            // Create a more complex component
            IUIComponent complexButton = new BasicButton("Submit", 120, 50);
            complexButton = new BorderDecorator(complexButton, 1, "dashed");
            complexButton = new ShadowDecorator(complexButton, 3, "black");
            complexButton = new BackgroundDecorator(complexButton, "green");
            complexButton = new AnimationDecorator(complexButton, "fadeIn");
            
            Console.WriteLine("Complex decorated button:");
            complexButton.Render();
            Console.WriteLine($"Final size: {complexButton.GetWidth()}x{complexButton.GetHeight()}");

            Console.WriteLine();
        }

        #endregion

        #region Example 6: Thread-Safe Decorator Pattern

        static async Task ThreadSafeDecoratorExample()
        {
            Console.WriteLine("6. Thread-Safe Decorator Pattern:");
            Console.WriteLine("==================================");

            // Create a thread-safe data processor
            IDataProcessor processor = new BasicDataProcessor();
            
            // Add thread-safe decorators
            processor = new ThreadSafeLoggingDecorator(processor);
            processor = new ThreadSafeMetricsDecorator(processor);
            processor = new ThreadSafeCachingDecorator(processor);

            // Process data concurrently
            var tasks = new List<Task>();
            var random = new Random();

            for (int i = 1; i <= 10; i++)
            {
                int taskId = i;
                tasks.Add(Task.Run(async () =>
                {
                    var data = $"Data from task {taskId}";
                    await Task.Delay(random.Next(100, 500)); // Simulate work
                    
                    var result = await processor.ProcessAsync(data);
                    Console.WriteLine($"Task {taskId}: {result}");
                }));
            }

            await Task.WhenAll(tasks);

            // Display metrics
            if (processor is ThreadSafeMetricsDecorator metricsDecorator)
            {
                Console.WriteLine($"\nProcessing metrics:");
                Console.WriteLine($"Total requests: {metricsDecorator.GetTotalRequests()}");
                Console.WriteLine($"Average processing time: {metricsDecorator.GetAverageProcessingTime():F2}ms");
            }

            Console.WriteLine();
        }

        #endregion

        #region Example 7: Data Transformation Pipeline

        static void DataTransformationExample()
        {
            Console.WriteLine("7. Data Transformation Pipeline:");
            Console.WriteLine("=================================");

            var rawData = new List<string> { "  Apple  ", "BANANA", "cherry", "  GRAPE  " };
            Console.WriteLine($"Raw data: [{string.Join(", ", rawData.Select(s => $"\"{s}\""))}]");

            // Create transformation pipeline
            IDataTransformer transformer = new BasicDataTransformer(rawData);
            
            // Add various transformations
            transformer = new TrimTransformer(transformer);
            transformer = new LowercaseTransformer(transformer);
            transformer = new CapitalizeFirstLetterTransformer(transformer);
            transformer = new SortTransformer(transformer);
            transformer = new FilterTransformer(transformer, item => item.Length > 4);

            var transformedData = transformer.Transform();
            Console.WriteLine($"Transformed data: [{string.Join(", ", transformedData.Select(s => $"\"{s}\""))}]");

            // Show transformation steps
            Console.WriteLine($"\nTransformation pipeline:");
            foreach (var step in transformer.GetTransformationSteps())
            {
                Console.WriteLine($"  - {step}");
            }

            Console.WriteLine();
        }

        #endregion

        #region Example 8: Gaming Power-up System

        static void GamingPowerUpExample()
        {
            Console.WriteLine("8. Gaming Power-up System:");
            Console.WriteLine("===========================");

            // Create a basic player character
            IGameCharacter player = new BasicPlayer("Hero", 100, 10, 5);
            Console.WriteLine("Basic player stats:");
            player.DisplayStats();
            Console.WriteLine($"Battle power: {player.CalculateBattlePower()}\n");

            // Apply strength power-up
            player = new StrengthPowerUp(player, 25);
            Console.WriteLine("After strength power-up:");
            player.DisplayStats();
            Console.WriteLine($"Battle power: {player.CalculateBattlePower()}\n");

            // Apply speed power-up
            player = new SpeedPowerUp(player, 15);
            Console.WriteLine("After speed power-up:");
            player.DisplayStats();
            Console.WriteLine($"Battle power: {player.CalculateBattlePower()}\n");

            // Apply health boost
            player = new HealthBoostPowerUp(player, 50);
            Console.WriteLine("After health boost:");
            player.DisplayStats();
            Console.WriteLine($"Battle power: {player.CalculateBattlePower()}\n");

            // Apply shield protection
            player = new ShieldPowerUp(player, 30);
            Console.WriteLine("After shield power-up:");
            player.DisplayStats();
            Console.WriteLine($"Battle power: {player.CalculateBattlePower()}\n");

            // Simulate battle
            Console.WriteLine("Battle simulation:");
            var damage = 25;
            Console.WriteLine($"Incoming damage: {damage}");
            var actualDamage = player.TakeDamage(damage);
            Console.WriteLine($"Actual damage taken: {actualDamage}");
            Console.WriteLine("Stats after damage:");
            player.DisplayStats();

            Console.WriteLine();
        }

        #endregion

        #region Example 9: Notification System

        static void NotificationSystemExample()
        {
            Console.WriteLine("9. Notification System:");
            Console.WriteLine("========================");

            var message = "Your order #12345 has been shipped!";
            var user = "john.doe@example.com";

            // Create basic notification
            INotification notification = new BasicNotification(message, user);
            
            // Add email encryption
            notification = new EncryptedNotificationDecorator(notification);
            
            // Add priority marking
            notification = new PriorityNotificationDecorator(notification, Priority.High);
            
            // Add timestamp
            notification = new TimestampNotificationDecorator(notification);
            
            // Add retry mechanism
            notification = new RetryNotificationDecorator(notification, maxRetries: 3);
            
            // Add formatting
            notification = new FormattedNotificationDecorator(notification, "HTML");

            Console.WriteLine("Sending decorated notification:");
            notification.Send();

            Console.WriteLine($"\nNotification metadata:");
            Console.WriteLine($"Priority: {notification.GetPriority()}");
            Console.WriteLine($"Timestamp: {notification.GetTimestamp()}");
            Console.WriteLine($"Encryption: {notification.IsEncrypted()}");
            Console.WriteLine($"Retry count: {notification.GetRetryCount()}");

            // Create a different notification configuration
            INotification simpleNotification = new BasicNotification("Welcome to our service!", user);
            simpleNotification = new TimestampNotificationDecorator(simpleNotification);
            simpleNotification = new FormattedNotificationDecorator(simpleNotification, "Plain");
            
            Console.WriteLine("\nSending simple notification:");
            simpleNotification.Send();

            Console.WriteLine();
        }

        #endregion
    }

    #region Core Decorator Pattern Classes

    // Component interface
    public interface IComponent
    {
        string Operation();
    }

    // Concrete component
    public class ConcreteComponent : IComponent
    {
        public string Operation()
        {
            return "ConcreteComponent";
        }
    }

    // Base decorator
    public abstract class Decorator : IComponent
    {
        protected IComponent component;

        public Decorator(IComponent component)
        {
            this.component = component;
        }

        public virtual string Operation()
        {
            return component.Operation();
        }
    }

    // Concrete decorators
    public class ConcreteDecoratorA : Decorator
    {
        public ConcreteDecoratorA(IComponent component) : base(component) { }

        public override string Operation()
        {
            return $"DecoratorA({base.Operation()})";
        }
    }

    public class ConcreteDecoratorB : Decorator
    {
        public ConcreteDecoratorB(IComponent component) : base(component) { }

        public override string Operation()
        {
            return $"DecoratorB({base.Operation()})";
        }
    }

    #endregion

    #region Example 1: Coffee Shop Classes

    public interface ICoffee
    {
        string GetDescription();
        decimal GetCost();
        int GetCalories();
    }

    public class SimpleCoffee : ICoffee
    {
        public string GetDescription() => "Simple Coffee";
        public decimal GetCost() => 2.00m;
        public int GetCalories() => 5;
    }

    public abstract class CoffeeDecorator : ICoffee
    {
        protected ICoffee coffee;

        public CoffeeDecorator(ICoffee coffee)
        {
            this.coffee = coffee;
        }

        public virtual string GetDescription() => coffee.GetDescription();
        public virtual decimal GetCost() => coffee.GetCost();
        public virtual int GetCalories() => coffee.GetCalories();
    }

    public class MilkDecorator : CoffeeDecorator
    {
        public MilkDecorator(ICoffee coffee) : base(coffee) { }

        public override string GetDescription() => $"{base.GetDescription()}, Milk";
        public override decimal GetCost() => base.GetCost() + 0.50m;
        public override int GetCalories() => base.GetCalories() + 20;
    }

    public class SugarDecorator : CoffeeDecorator
    {
        public SugarDecorator(ICoffee coffee) : base(coffee) { }

        public override string GetDescription() => $"{base.GetDescription()}, Sugar";
        public override decimal GetCost() => base.GetCost() + 0.25m;
        public override int GetCalories() => base.GetCalories() + 16;
    }

    public class WhippedCreamDecorator : CoffeeDecorator
    {
        public WhippedCreamDecorator(ICoffee coffee) : base(coffee) { }

        public override string GetDescription() => $"{base.GetDescription()}, Whipped Cream";
        public override decimal GetCost() => base.GetCost() + 0.75m;
        public override int GetCalories() => base.GetCalories() + 50;
    }

    public class VanillaDecorator : CoffeeDecorator
    {
        public VanillaDecorator(ICoffee coffee) : base(coffee) { }

        public override string GetDescription() => $"{base.GetDescription()}, Vanilla";
        public override decimal GetCost() => base.GetCost() + 0.60m;
        public override int GetCalories() => base.GetCalories() + 10;
    }

    #endregion

    #region Example 2: Text Processing Classes

    public interface ITextProcessor
    {
        string Process();
        int GetProcessingSteps();
    }

    public class BasicTextProcessor : ITextProcessor
    {
        private readonly string text;

        public BasicTextProcessor(string text)
        {
            this.text = text;
        }

        public string Process() => text;
        public int GetProcessingSteps() => 1;
    }

    public abstract class TextProcessorDecorator : ITextProcessor
    {
        protected ITextProcessor processor;

        public TextProcessorDecorator(ITextProcessor processor)
        {
            this.processor = processor;
        }

        public virtual string Process() => processor.Process();
        public virtual int GetProcessingSteps() => processor.GetProcessingSteps() + 1;
    }

    public class UppercaseDecorator : TextProcessorDecorator
    {
        public UppercaseDecorator(ITextProcessor processor) : base(processor) { }

        public override string Process() => base.Process().ToUpper();
    }

    public class TrimDecorator : TextProcessorDecorator
    {
        public TrimDecorator(ITextProcessor processor) : base(processor) { }

        public override string Process() => base.Process().Trim();
    }

    public class PrefixDecorator : TextProcessorDecorator
    {
        private readonly string prefix;

        public PrefixDecorator(ITextProcessor processor, string prefix) : base(processor)
        {
            this.prefix = prefix;
        }

        public override string Process() => prefix + base.Process();
    }

    public class SuffixDecorator : TextProcessorDecorator
    {
        private readonly string suffix;

        public SuffixDecorator(ITextProcessor processor, string suffix) : base(processor)
        {
            this.suffix = suffix;
        }

        public override string Process() => base.Process() + suffix;
    }

    public class CapitalizeDecorator : TextProcessorDecorator
    {
        public CapitalizeDecorator(ITextProcessor processor) : base(processor) { }

        public override string Process()
        {
            var text = base.Process();
            return string.IsNullOrEmpty(text) ? text : char.ToUpper(text[0]) + text.Substring(1).ToLower();
        }
    }

    public class ReverseDecorator : TextProcessorDecorator
    {
        public ReverseDecorator(ITextProcessor processor) : base(processor) { }

        public override string Process()
        {
            var text = base.Process();
            return new string(text.Reverse().ToArray());
        }
    }

    #endregion

    #region Example 3: Stream Processing Classes

    public interface IDataStream
    {
        string Read();
        void Write(string data);
        int GetSize();
    }

    public class BasicDataStream : IDataStream
    {
        private string data;

        public BasicDataStream(string data)
        {
            this.data = data;
        }

        public string Read() => data;
        public void Write(string data) => this.data = data;
        public int GetSize() => data.Length;
    }

    public abstract class DataStreamDecorator : IDataStream
    {
        protected IDataStream stream;

        public DataStreamDecorator(IDataStream stream)
        {
            this.stream = stream;
        }

        public virtual string Read() => stream.Read();
        public virtual void Write(string data) => stream.Write(data);
        public virtual int GetSize() => stream.GetSize();
    }

    public class CompressionDecorator : DataStreamDecorator
    {
        public CompressionDecorator(IDataStream stream) : base(stream) { }

        public override string Read()
        {
            var compressed = base.Read();
            // Simulate decompression
            Console.WriteLine("  Decompressing data...");
            return compressed;
        }

        public override void Write(string data)
        {
            // Simulate compression
            Console.WriteLine("  Compressing data...");
            base.Write(data);
        }

        public override int GetSize()
        {
            // Simulate compression ratio of 60%
            return (int)(base.GetSize() * 0.6);
        }
    }

    public class EncryptionDecorator : DataStreamDecorator
    {
        public EncryptionDecorator(IDataStream stream) : base(stream) { }

        public override string Read()
        {
            var encrypted = base.Read();
            // Simulate decryption
            Console.WriteLine("  Decrypting data...");
            return encrypted;
        }

        public override void Write(string data)
        {
            // Simulate encryption
            Console.WriteLine("  Encrypting data...");
            var encrypted = Convert.ToBase64String(Encoding.UTF8.GetBytes(data));
            base.Write(encrypted);
        }
    }

    public class LoggingDecorator : DataStreamDecorator
    {
        public LoggingDecorator(IDataStream stream) : base(stream) { }

        public override string Read()
        {
            Console.WriteLine("  [LOG] Reading data from stream");
            var data = base.Read();
            Console.WriteLine($"  [LOG] Read {data.Length} characters");
            return data;
        }

        public override void Write(string data)
        {
            Console.WriteLine($"  [LOG] Writing {data.Length} characters to stream");
            base.Write(data);
            Console.WriteLine("  [LOG] Write operation completed");
        }
    }

    public class CachingDecorator : DataStreamDecorator
    {
        private string? cachedData;
        private bool isCached;

        public CachingDecorator(IDataStream stream) : base(stream) { }

        public override string Read()
        {
            if (isCached && cachedData != null)
            {
                Console.WriteLine("  [CACHE] Serving data from cache");
                return cachedData;
            }

            Console.WriteLine("  [CACHE] Cache miss, reading from source");
            cachedData = base.Read();
            isCached = true;
            return cachedData;
        }

        public override void Write(string data)
        {
            Console.WriteLine("  [CACHE] Invalidating cache");
            isCached = false;
            cachedData = null;
            base.Write(data);
        }
    }

    #endregion

    #region Example 4: Web Middleware Classes

    public class HttpRequestContext
    {
        public string Method { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public Dictionary<string, string> Headers { get; set; } = new();
        public string Body { get; set; } = string.Empty;
    }

    public class HttpResponseContext
    {
        public int StatusCode { get; set; }
        public string Body { get; set; } = string.Empty;
        public Dictionary<string, string> Headers { get; set; } = new();
        public long ProcessingTimeMs { get; set; }
    }

    public interface IRequestHandler
    {
        Task<HttpResponseContext> HandleAsync(HttpRequestContext request);
    }

    public class BasicRequestHandler : IRequestHandler
    {
        public async Task<HttpResponseContext> HandleAsync(HttpRequestContext request)
        {
            await Task.Delay(50); // Simulate processing
            
            return new HttpResponseContext
            {
                StatusCode = 200,
                Body = $"{{\"message\": \"Success\", \"path\": \"{request.Path}\"}}",
                ProcessingTimeMs = 50
            };
        }
    }

    public abstract class MiddlewareDecorator : IRequestHandler
    {
        protected IRequestHandler handler;

        public MiddlewareDecorator(IRequestHandler handler)
        {
            this.handler = handler;
        }

        public virtual async Task<HttpResponseContext> HandleAsync(HttpRequestContext request)
        {
            return await handler.HandleAsync(request);
        }
    }

    public class AuthenticationMiddleware : MiddlewareDecorator
    {
        public AuthenticationMiddleware(IRequestHandler handler) : base(handler) { }

        public override async Task<HttpResponseContext> HandleAsync(HttpRequestContext request)
        {
            Console.WriteLine("  [AUTH] Checking authentication...");
            
            if (!request.Headers.ContainsKey("Authorization"))
            {
                return new HttpResponseContext
                {
                    StatusCode = 401,
                    Body = "{\"error\": \"Unauthorized\"}"
                };
            }

            Console.WriteLine("  [AUTH] Authentication successful");
            return await base.HandleAsync(request);
        }
    }

    public class LoggingMiddleware : MiddlewareDecorator
    {
        public LoggingMiddleware(IRequestHandler handler) : base(handler) { }

        public override async Task<HttpResponseContext> HandleAsync(HttpRequestContext request)
        {
            var stopwatch = Stopwatch.StartNew();
            Console.WriteLine($"  [LOG] {request.Method} {request.Path} - Started");
            
            var response = await base.HandleAsync(request);
            
            stopwatch.Stop();
            response.ProcessingTimeMs += stopwatch.ElapsedMilliseconds;
            Console.WriteLine($"  [LOG] {request.Method} {request.Path} - Completed in {stopwatch.ElapsedMilliseconds}ms");
            
            return response;
        }
    }

    public class RateLimitingMiddleware : MiddlewareDecorator
    {
        private static readonly Dictionary<string, DateTime> lastRequestTimes = new();
        private static readonly object lockObject = new object();

        public RateLimitingMiddleware(IRequestHandler handler) : base(handler) { }

        public override async Task<HttpResponseContext> HandleAsync(HttpRequestContext request)
        {
            var clientId = request.Headers.GetValueOrDefault("Authorization", "anonymous");
            
            lock (lockObject)
            {
                if (lastRequestTimes.TryGetValue(clientId, out var lastTime))
                {
                    if (DateTime.Now - lastTime < TimeSpan.FromSeconds(1))
                    {
                        Console.WriteLine("  [RATE] Rate limit exceeded");
                        return new HttpResponseContext
                        {
                            StatusCode = 429,
                            Body = "{\"error\": \"Rate limit exceeded\"}"
                        };
                    }
                }
                
                lastRequestTimes[clientId] = DateTime.Now;
            }

            Console.WriteLine("  [RATE] Rate limit check passed");
            return await base.HandleAsync(request);
        }
    }

    public class CachingMiddleware : MiddlewareDecorator
    {
        private static readonly Dictionary<string, (HttpResponseContext Response, DateTime CachedAt)> cache = new();
        private static readonly object cacheLock = new object();

        public CachingMiddleware(IRequestHandler handler) : base(handler) { }

        public override async Task<HttpResponseContext> HandleAsync(HttpRequestContext request)
        {
            if (request.Method == "GET")
            {
                var cacheKey = $"{request.Method}:{request.Path}";
                
                lock (cacheLock)
                {
                    if (cache.TryGetValue(cacheKey, out var cached))
                    {
                        if (DateTime.Now - cached.CachedAt < TimeSpan.FromMinutes(5))
                        {
                            Console.WriteLine("  [CACHE] Serving from cache");
                            return cached.Response;
                        }
                        else
                        {
                            cache.Remove(cacheKey);
                        }
                    }
                }

                Console.WriteLine("  [CACHE] Cache miss");
                var response = await base.HandleAsync(request);
                
                lock (cacheLock)
                {
                    cache[cacheKey] = (response, DateTime.Now);
                }
                
                return response;
            }

            return await base.HandleAsync(request);
        }
    }

    #endregion

    #region Example 5: UI Component Classes

    public interface IUIComponent
    {
        void Render();
        int GetWidth();
        int GetHeight();
    }

    public class BasicButton : IUIComponent
    {
        private readonly string text;
        private readonly int width;
        private readonly int height;

        public BasicButton(string text, int width, int height)
        {
            this.text = text;
            this.width = width;
            this.height = height;
        }

        public void Render()
        {
            Console.WriteLine($"  Button: '{text}'");
        }

        public int GetWidth() => width;
        public int GetHeight() => height;
    }

    public abstract class UIComponentDecorator : IUIComponent
    {
        protected IUIComponent component;

        public UIComponentDecorator(IUIComponent component)
        {
            this.component = component;
        }

        public virtual void Render() => component.Render();
        public virtual int GetWidth() => component.GetWidth();
        public virtual int GetHeight() => component.GetHeight();
    }

    public class BorderDecorator : UIComponentDecorator
    {
        private readonly int borderWidth;
        private readonly string borderStyle;

        public BorderDecorator(IUIComponent component, int borderWidth, string borderStyle) : base(component)
        {
            this.borderWidth = borderWidth;
            this.borderStyle = borderStyle;
        }

        public override void Render()
        {
            base.Render();
            Console.WriteLine($"    + Border: {borderWidth}px {borderStyle}");
        }

        public override int GetWidth() => base.GetWidth() + (borderWidth * 2);
        public override int GetHeight() => base.GetHeight() + (borderWidth * 2);
    }

    public class ShadowDecorator : UIComponentDecorator
    {
        private readonly int shadowSize;
        private readonly string shadowColor;

        public ShadowDecorator(IUIComponent component, int shadowSize, string shadowColor) : base(component)
        {
            this.shadowSize = shadowSize;
            this.shadowColor = shadowColor;
        }

        public override void Render()
        {
            base.Render();
            Console.WriteLine($"    + Shadow: {shadowSize}px {shadowColor}");
        }

        public override int GetWidth() => base.GetWidth() + shadowSize;
        public override int GetHeight() => base.GetHeight() + shadowSize;
    }

    public class BackgroundDecorator : UIComponentDecorator
    {
        private readonly string backgroundColor;

        public BackgroundDecorator(IUIComponent component, string backgroundColor) : base(component)
        {
            this.backgroundColor = backgroundColor;
        }

        public override void Render()
        {
            Console.WriteLine($"    + Background: {backgroundColor}");
            base.Render();
        }
    }

    public class AnimationDecorator : UIComponentDecorator
    {
        private readonly string animationType;

        public AnimationDecorator(IUIComponent component, string animationType) : base(component)
        {
            this.animationType = animationType;
        }

        public override void Render()
        {
            base.Render();
            Console.WriteLine($"    + Animation: {animationType}");
        }
    }

    #endregion

    #region Example 6: Thread-Safe Decorator Classes

    public interface IDataProcessor
    {
        Task<string> ProcessAsync(string data);
    }

    public class BasicDataProcessor : IDataProcessor
    {
        public async Task<string> ProcessAsync(string data)
        {
            await Task.Delay(100); // Simulate processing
            return $"Processed: {data}";
        }
    }

    public abstract class ThreadSafeDecoratorBase : IDataProcessor
    {
        protected IDataProcessor processor;

        public ThreadSafeDecoratorBase(IDataProcessor processor)
        {
            this.processor = processor;
        }

        public virtual async Task<string> ProcessAsync(string data)
        {
            return await processor.ProcessAsync(data);
        }
    }

    public class ThreadSafeLoggingDecorator : ThreadSafeDecoratorBase
    {
        private static readonly object logLock = new object();

        public ThreadSafeLoggingDecorator(IDataProcessor processor) : base(processor) { }

        public override async Task<string> ProcessAsync(string data)
        {
            lock (logLock)
            {
                Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] Processing: {data}");
            }

            var result = await base.ProcessAsync(data);

            lock (logLock)
            {
                Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] Completed: {result}");
            }

            return result;
        }
    }

    public class ThreadSafeMetricsDecorator : ThreadSafeDecoratorBase
    {
        private static long totalRequests = 0;
        private static long totalProcessingTimeMs = 0;
        private static readonly object metricsLock = new object();

        public ThreadSafeMetricsDecorator(IDataProcessor processor) : base(processor) { }

        public override async Task<string> ProcessAsync(string data)
        {
            var stopwatch = Stopwatch.StartNew();
            var result = await base.ProcessAsync(data);
            stopwatch.Stop();

            lock (metricsLock)
            {
                totalRequests++;
                totalProcessingTimeMs += stopwatch.ElapsedMilliseconds;
            }

            return result;
        }

        public long GetTotalRequests()
        {
            lock (metricsLock)
            {
                return totalRequests;
            }
        }

        public double GetAverageProcessingTime()
        {
            lock (metricsLock)
            {
                return totalRequests > 0 ? (double)totalProcessingTimeMs / totalRequests : 0;
            }
        }
    }

    public class ThreadSafeCachingDecorator : ThreadSafeDecoratorBase
    {
        private static readonly ConcurrentDictionary<string, string> cache = new();

        public ThreadSafeCachingDecorator(IDataProcessor processor) : base(processor) { }

        public override async Task<string> ProcessAsync(string data)
        {
            if (cache.TryGetValue(data, out var cachedResult))
            {
                return cachedResult;
            }

            var result = await base.ProcessAsync(data);
            cache.TryAdd(data, result);
            return result;
        }
    }

    #endregion

    #region Example 7: Data Transformation Classes

    public interface IDataTransformer
    {
        List<string> Transform();
        List<string> GetTransformationSteps();
    }

    public class BasicDataTransformer : IDataTransformer
    {
        private readonly List<string> data;

        public BasicDataTransformer(List<string> data)
        {
            this.data = new List<string>(data);
        }

        public List<string> Transform() => new List<string>(data);
        public List<string> GetTransformationSteps() => new List<string> { "Basic data loading" };
    }

    public abstract class DataTransformerDecorator : IDataTransformer
    {
        protected IDataTransformer transformer;

        public DataTransformerDecorator(IDataTransformer transformer)
        {
            this.transformer = transformer;
        }

        public virtual List<string> Transform() => transformer.Transform();
        public virtual List<string> GetTransformationSteps() => transformer.GetTransformationSteps();
    }

    public class TrimTransformer : DataTransformerDecorator
    {
        public TrimTransformer(IDataTransformer transformer) : base(transformer) { }

        public override List<string> Transform()
        {
            return base.Transform().Select(item => item.Trim()).ToList();
        }

        public override List<string> GetTransformationSteps()
        {
            var steps = base.GetTransformationSteps();
            steps.Add("Trim whitespace");
            return steps;
        }
    }

    public class LowercaseTransformer : DataTransformerDecorator
    {
        public LowercaseTransformer(IDataTransformer transformer) : base(transformer) { }

        public override List<string> Transform()
        {
            return base.Transform().Select(item => item.ToLower()).ToList();
        }

        public override List<string> GetTransformationSteps()
        {
            var steps = base.GetTransformationSteps();
            steps.Add("Convert to lowercase");
            return steps;
        }
    }

    public class CapitalizeFirstLetterTransformer : DataTransformerDecorator
    {
        public CapitalizeFirstLetterTransformer(IDataTransformer transformer) : base(transformer) { }

        public override List<string> Transform()
        {
            return base.Transform().Select(item => 
                string.IsNullOrEmpty(item) ? item : char.ToUpper(item[0]) + item.Substring(1)
            ).ToList();
        }

        public override List<string> GetTransformationSteps()
        {
            var steps = base.GetTransformationSteps();
            steps.Add("Capitalize first letter");
            return steps;
        }
    }

    public class SortTransformer : DataTransformerDecorator
    {
        public SortTransformer(IDataTransformer transformer) : base(transformer) { }

        public override List<string> Transform()
        {
            var data = base.Transform();
            data.Sort();
            return data;
        }

        public override List<string> GetTransformationSteps()
        {
            var steps = base.GetTransformationSteps();
            steps.Add("Sort alphabetically");
            return steps;
        }
    }

    public class FilterTransformer : DataTransformerDecorator
    {
        private readonly Func<string, bool> predicate;

        public FilterTransformer(IDataTransformer transformer, Func<string, bool> predicate) : base(transformer)
        {
            this.predicate = predicate;
        }

        public override List<string> Transform()
        {
            return base.Transform().Where(predicate).ToList();
        }

        public override List<string> GetTransformationSteps()
        {
            var steps = base.GetTransformationSteps();
            steps.Add("Filter by predicate");
            return steps;
        }
    }

    #endregion

    #region Example 8: Gaming Power-up Classes

    public interface IGameCharacter
    {
        string Name { get; }
        int Health { get; }
        int Strength { get; }
        int Speed { get; }
        
        void DisplayStats();
        int CalculateBattlePower();
        int TakeDamage(int damage);
    }

    public class BasicPlayer : IGameCharacter
    {
        public string Name { get; private set; }
        public int Health { get; private set; }
        public int Strength { get; private set; }
        public int Speed { get; private set; }

        public BasicPlayer(string name, int health, int strength, int speed)
        {
            Name = name;
            Health = health;
            Strength = strength;
            Speed = speed;
        }

        public void DisplayStats()
        {
            Console.WriteLine($"  {Name} - Health: {Health}, Strength: {Strength}, Speed: {Speed}");
        }

        public int CalculateBattlePower()
        {
            return Health + Strength * 2 + Speed;
        }

        public virtual int TakeDamage(int damage)
        {
            var actualDamage = Math.Max(1, damage);
            Health = Math.Max(0, Health - actualDamage);
            return actualDamage;
        }
    }

    public abstract class PowerUpDecorator : IGameCharacter
    {
        protected IGameCharacter character;

        public PowerUpDecorator(IGameCharacter character)
        {
            this.character = character;
        }

        public virtual string Name => character.Name;
        public virtual int Health => character.Health;
        public virtual int Strength => character.Strength;
        public virtual int Speed => character.Speed;

        public virtual void DisplayStats() => character.DisplayStats();
        public virtual int CalculateBattlePower() => character.CalculateBattlePower();
        public virtual int TakeDamage(int damage) => character.TakeDamage(damage);
    }

    public class StrengthPowerUp : PowerUpDecorator
    {
        private readonly int strengthBonus;

        public StrengthPowerUp(IGameCharacter character, int strengthBonus) : base(character)
        {
            this.strengthBonus = strengthBonus;
        }

        public override int Strength => base.Strength + strengthBonus;

        public override void DisplayStats()
        {
            Console.WriteLine($"  {Name} - Health: {Health}, Strength: {Strength} (+{strengthBonus}), Speed: {Speed}");
        }
    }

    public class SpeedPowerUp : PowerUpDecorator
    {
        private readonly int speedBonus;

        public SpeedPowerUp(IGameCharacter character, int speedBonus) : base(character)
        {
            this.speedBonus = speedBonus;
        }

        public override int Speed => base.Speed + speedBonus;

        public override void DisplayStats()
        {
            Console.WriteLine($"  {Name} - Health: {Health}, Strength: {Strength}, Speed: {Speed} (+{speedBonus})");
        }
    }

    public class HealthBoostPowerUp : PowerUpDecorator
    {
        private readonly int healthBonus;

        public HealthBoostPowerUp(IGameCharacter character, int healthBonus) : base(character)
        {
            this.healthBonus = healthBonus;
        }

        public override int Health => base.Health + healthBonus;

        public override void DisplayStats()
        {
            Console.WriteLine($"  {Name} - Health: {Health} (+{healthBonus}), Strength: {Strength}, Speed: {Speed}");
        }
    }

    public class ShieldPowerUp : PowerUpDecorator
    {
        private readonly int shieldStrength;

        public ShieldPowerUp(IGameCharacter character, int shieldStrength) : base(character)
        {
            this.shieldStrength = shieldStrength;
        }

        public override void DisplayStats()
        {
            Console.WriteLine($"  {Name} - Health: {Health}, Strength: {Strength}, Speed: {Speed}, Shield: {shieldStrength}");
        }

        public override int TakeDamage(int damage)
        {
            var reducedDamage = Math.Max(1, damage - shieldStrength / 2);
            Console.WriteLine($"  Shield absorbed {damage - reducedDamage} damage");
            return base.TakeDamage(reducedDamage);
        }
    }

    #endregion

    #region Example 9: Notification System Classes

    public enum Priority { Low, Normal, High, Critical }

    public interface INotification
    {
        void Send();
        Priority GetPriority();
        DateTime GetTimestamp();
        bool IsEncrypted();
        int GetRetryCount();
    }

    public class BasicNotification : INotification
    {
        protected string message;
        protected string recipient;

        public BasicNotification(string message, string recipient)
        {
            this.message = message;
            this.recipient = recipient;
        }

        public virtual void Send()
        {
            Console.WriteLine($"  Sending notification to {recipient}: {message}");
        }

        public virtual Priority GetPriority() => Priority.Normal;
        public virtual DateTime GetTimestamp() => DateTime.Now;
        public virtual bool IsEncrypted() => false;
        public virtual int GetRetryCount() => 0;
    }

    public abstract class NotificationDecorator : INotification
    {
        protected INotification notification;

        public NotificationDecorator(INotification notification)
        {
            this.notification = notification;
        }

        public virtual void Send() => notification.Send();
        public virtual Priority GetPriority() => notification.GetPriority();
        public virtual DateTime GetTimestamp() => notification.GetTimestamp();
        public virtual bool IsEncrypted() => notification.IsEncrypted();
        public virtual int GetRetryCount() => notification.GetRetryCount();
    }

    public class EncryptedNotificationDecorator : NotificationDecorator
    {
        public EncryptedNotificationDecorator(INotification notification) : base(notification) { }

        public override void Send()
        {
            Console.WriteLine("  Encrypting notification...");
            base.Send();
        }

        public override bool IsEncrypted() => true;
    }

    public class PriorityNotificationDecorator : NotificationDecorator
    {
        private readonly Priority priority;

        public PriorityNotificationDecorator(INotification notification, Priority priority) : base(notification)
        {
            this.priority = priority;
        }

        public override void Send()
        {
            Console.WriteLine($"  [PRIORITY: {priority}]");
            base.Send();
        }

        public override Priority GetPriority() => priority;
    }

    public class TimestampNotificationDecorator : NotificationDecorator
    {
        private readonly DateTime timestamp;

        public TimestampNotificationDecorator(INotification notification) : base(notification)
        {
            timestamp = DateTime.Now;
        }

        public override void Send()
        {
            Console.WriteLine($"  [TIMESTAMP: {timestamp:yyyy-MM-dd HH:mm:ss}]");
            base.Send();
        }

        public override DateTime GetTimestamp() => timestamp;
    }

    public class RetryNotificationDecorator : NotificationDecorator
    {
        private readonly int maxRetries;
        private int retryCount = 0;

        public RetryNotificationDecorator(INotification notification, int maxRetries) : base(notification)
        {
            this.maxRetries = maxRetries;
        }

        public override void Send()
        {
            for (int attempt = 0; attempt <= maxRetries; attempt++)
            {
                try
                {
                    if (attempt > 0)
                    {
                        Console.WriteLine($"  Retry attempt {attempt}/{maxRetries}");
                        retryCount = attempt;
                    }
                    
                    base.Send();
                    break; // Success, exit retry loop
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  Attempt {attempt + 1} failed: {ex.Message}");
                    if (attempt == maxRetries)
                    {
                        throw; // Final attempt failed
                    }
                }
            }
        }

        public override int GetRetryCount() => retryCount;
    }

    public class FormattedNotificationDecorator : NotificationDecorator
    {
        private readonly string format;

        public FormattedNotificationDecorator(INotification notification, string format) : base(notification)
        {
            this.format = format;
        }

        public override void Send()
        {
            Console.WriteLine($"  Formatting as {format}...");
            base.Send();
        }
    }

    #endregion
}

/*
 * MEMORY ALLOCATION CONSIDERATIONS:
 * =================================
 * 
 * 1. DECORATOR CHAIN OVERHEAD:
 *    - Each decorator adds object allocation overhead
 *    - Long decorator chains can create significant memory pressure
 *    - Consider object pooling for frequently created/destroyed decorators
 *    - Use struct decorators for simple stateless decorations to reduce heap allocation
 * 
 * 2. REFERENCE CHAIN MANAGEMENT:
 *    - Decorator chains hold references to wrapped objects, preventing garbage collection
 *    - Avoid circular references between decorators and their components
 *    - Consider weak references for non-essential decorator relationships
 *    - Implement proper disposal patterns for decorators managing resources
 * 
 * 3. SHARED OBJECT OPTIMIZATION:
 *    - Multiple decorators can wrap the same core object instance
 *    - Use flyweight pattern for stateless decorators to reduce memory usage
 *    - Cache expensive decorator configurations for reuse
 *    - Consider copy-on-write semantics for immutable decorator chains
 * 
 * 4. MEMORY EFFICIENCY STRATEGIES:
 *    - Lazy initialization of expensive decorator state
 *    - Use value types for simple decorator properties
 *    - Implement pooling for short-lived decorator instances
 *    - Consider using spans/memory for data processing decorators
 * 
 * MULTITHREAD CONSIDERATIONS:
 * ===========================
 * 
 * 1. DECORATOR THREAD SAFETY:
 *    - Decorators are typically not thread-safe unless explicitly designed
 *    - Thread safety depends on both decorator implementation and wrapped object
 *    - Stateless decorators are inherently thread-safe for read operations
 *    - Stateful decorators require careful synchronization design
 * 
 * 2. SHARED DECORATOR INSTANCES:
 *    - Multiple threads may share the same decorator chain
 *    - Use concurrent collections for thread-safe decorator state
 *    - Implement proper locking strategies for decorator modifications
 *    - Consider using ThreadLocal storage for per-thread decorator state
 * 
 * 3. CONCURRENT DECORATION OPERATIONS:
 *    - Adding/removing decorators at runtime requires synchronization
 *    - Use atomic operations or locks when modifying decorator chains
 *    - Consider immutable decorator chains for thread-safe scenarios
 *    - Implement copy-on-write for concurrent decorator modifications
 * 
 * 4. ASYNC DECORATOR PATTERNS:
 *    - Decorators work well with async/await patterns
 *    - Implement proper exception handling and cancellation token support
 *    - Consider using SemaphoreSlim for rate limiting decorators
 *    - Use ConfigureAwait(false) to avoid deadlocks in library code
 * 
 * 5. PERFORMANCE CONSIDERATIONS:
 *    - Each decorator adds a method call overhead (virtual dispatch)
 *    - Deep decorator chains can impact performance in tight loops
 *    - Consider flattening decorator chains for performance-critical scenarios
 *    - Profile decorator overhead vs. functionality benefits
 * 
 * 6. BEST PRACTICES:
 *    - Design decorators to be composable and order-independent when possible
 *    - Implement proper error handling that preserves decorator chain integrity
 *    - Use dependency injection for managing complex decorator configurations
 *    - Document thread safety guarantees of decorator implementations
 *    - Consider using factory patterns for creating appropriate decorator chains
 *    - Implement fluent interfaces for easier decorator composition
 *    - Use generic decorators to avoid boxing/unboxing overhead
 *    - Provide both synchronous and asynchronous decorator variants when needed
 */
