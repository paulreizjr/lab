using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

/*
 * PROXY DESIGN PATTERN EXAMPLE
 * 
 * PURPOSE:
 * The Proxy pattern provides a placeholder or surrogate for another object to control access to it.
 * It acts as an intermediary that can add functionality like lazy loading, access control, 
 * caching, logging, or resource management without changing the original object's interface.
 * 
 * MEMORY ALLOCATION BENEFITS:
 * - Lazy initialization: Delays expensive object creation until actually needed
 * - Caching: Stores results to avoid repeated expensive operations
 * - Resource pooling: Manages limited resources efficiently
 * - Virtual proxy: Creates expensive objects on-demand
 * - Smart references: Adds reference counting or cleanup behavior
 * 
 * SCENARIOS TO USE:
 * - Lazy loading of expensive resources (large images, database connections)
 * - Access control and security (authentication, authorization)
 * - Caching expensive operations or remote calls
 * - Logging and monitoring object usage
 * - Resource management (connection pooling, file handles)
 * - Virtual objects for memory-intensive operations
 * - Remote proxy for distributed systems
 * 
 * SCENARIOS NOT TO USE:
 * - Simple objects where proxy overhead exceeds benefits
 * - When direct access is required for performance-critical code
 * - Objects that don't need additional behavior or control
 * - When the proxy interface becomes too complex
 * - Real-time systems where proxy overhead is unacceptable
 * 
 * MULTITHREADING ASPECTS:
 * - Proxy must handle concurrent access to shared resources
 * - Lazy initialization requires thread-safe patterns (Lazy<T>, locks)
 * - Caching proxies need thread-safe collections (ConcurrentDictionary)
 * - Access control proxies must synchronize permission checks
 * - Resource pooling proxies need thread-safe resource management
 */

namespace ProxyPattern
{
    // Subject interface that both RealSubject and Proxy implement
    public interface IImageService
    {
        void DisplayImage(string filename);
        byte[] LoadImageData(string filename);
        ImageMetadata GetImageMetadata(string filename);
    }

    // Real Subject: The actual object that performs the work
    // This represents an expensive resource that we want to control access to
    public class ExpensiveImageService : IImageService
    {
        private readonly Dictionary<string, byte[]> _loadedImages = new Dictionary<string, byte[]>();

        public ExpensiveImageService()
        {
            // Simulate expensive initialization (database connection, resource allocation, etc.)
            Console.WriteLine("[REAL SERVICE] Initializing expensive image service...");
            Thread.Sleep(1000); // Simulate expensive setup
            Console.WriteLine("[REAL SERVICE] Image service initialized.");
        }

        public void DisplayImage(string filename)
        {
            Console.WriteLine($"[REAL SERVICE] Displaying image: {filename}");
            var data = LoadImageData(filename);
            Console.WriteLine($"[REAL SERVICE] Image displayed. Size: {data.Length} bytes");
        }

        public byte[] LoadImageData(string filename)
        {
            // Check if already loaded (simple caching in real service)
            if (_loadedImages.ContainsKey(filename))
            {
                Console.WriteLine($"[REAL SERVICE] Returning cached image data for: {filename}");
                return _loadedImages[filename];
            }

            // Simulate expensive file loading operation
            Console.WriteLine($"[REAL SERVICE] Loading image from disk: {filename}");
            Thread.Sleep(500); // Simulate file I/O delay
            
            // Generate mock image data
            byte[] imageData = new byte[1024 * 100]; // 100KB mock image
            new Random().NextBytes(imageData);
            
            _loadedImages[filename] = imageData;
            Console.WriteLine($"[REAL SERVICE] Image loaded and cached: {filename}");
            
            return imageData;
        }

        public ImageMetadata GetImageMetadata(string filename)
        {
            Console.WriteLine($"[REAL SERVICE] Getting metadata for: {filename}");
            Thread.Sleep(200); // Simulate metadata extraction
            
            return new ImageMetadata
            {
                Filename = filename,
                Width = 1920,
                Height = 1080,
                Format = "JPEG",
                SizeBytes = 1024 * 100
            };
        }
    }

    // Supporting class for image metadata
    public class ImageMetadata
    {
        public required string Filename { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public required string Format { get; set; }
        public long SizeBytes { get; set; }

        public override string ToString()
        {
            return $"{Filename} ({Width}x{Height}, {Format}, {SizeBytes} bytes)";
        }
    }

    // Virtual Proxy: Delays expensive object creation until needed
    // MEMORY EFFICIENCY: Only creates the real service when actually required
    public class LazyImageServiceProxy : IImageService
    {
        // Lazy<T> provides thread-safe lazy initialization
        // THREAD SAFETY: Lazy<T> handles concurrent access during initialization
        private readonly Lazy<ExpensiveImageService> _lazyService;
        private readonly string _proxyId;

        public LazyImageServiceProxy(string proxyId = "Default")
        {
            _proxyId = proxyId;
            Console.WriteLine($"[LAZY PROXY {_proxyId}] Created (real service not yet initialized)");
            
            // MEMORY ALLOCATION: Real service creation is deferred
            _lazyService = new Lazy<ExpensiveImageService>(
                () => {
                    Console.WriteLine($"[LAZY PROXY {_proxyId}] Initializing real service for first time...");
                    return new ExpensiveImageService();
                },
                LazyThreadSafetyMode.ExecutionAndPublication // Thread-safe initialization
            );
        }

        public void DisplayImage(string filename)
        {
            Console.WriteLine($"[LAZY PROXY {_proxyId}] Display request for: {filename}");
            // Real service is created only when first accessed
            _lazyService.Value.DisplayImage(filename);
        }

        public byte[] LoadImageData(string filename)
        {
            Console.WriteLine($"[LAZY PROXY {_proxyId}] Load data request for: {filename}");
            return _lazyService.Value.LoadImageData(filename);
        }

        public ImageMetadata GetImageMetadata(string filename)
        {
            Console.WriteLine($"[LAZY PROXY {_proxyId}] Metadata request for: {filename}");
            return _lazyService.Value.GetImageMetadata(filename);
        }

        // Property to check if real service has been created (useful for testing)
        public bool IsServiceInitialized => _lazyService.IsValueCreated;
    }

    // Caching Proxy: Adds caching layer to reduce expensive operations
    // MEMORY MANAGEMENT: Uses ConcurrentDictionary for thread-safe caching
    public class CachingImageServiceProxy : IImageService
    {
        private readonly IImageService _realService;
        
        // Thread-safe caches for different types of data
        // THREAD SAFETY: ConcurrentDictionary provides thread-safe operations
        private readonly ConcurrentDictionary<string, byte[]> _imageDataCache = new ConcurrentDictionary<string, byte[]>();
        private readonly ConcurrentDictionary<string, ImageMetadata> _metadataCache = new ConcurrentDictionary<string, ImageMetadata>();
        
        // Cache statistics for monitoring
        private int _cacheHits = 0;
        private int _cacheMisses = 0;

        public CachingImageServiceProxy(IImageService realService)
        {
            _realService = realService ?? throw new ArgumentNullException(nameof(realService));
            Console.WriteLine("[CACHING PROXY] Created with caching enabled");
        }

        public void DisplayImage(string filename)
        {
            Console.WriteLine($"[CACHING PROXY] Display request for: {filename}");
            
            // For display, we still delegate to real service
            // but we could cache display state if needed
            _realService.DisplayImage(filename);
        }

        public byte[] LoadImageData(string filename)
        {
            Console.WriteLine($"[CACHING PROXY] Load data request for: {filename}");
            
            // MEMORY EFFICIENCY: Check cache first to avoid expensive loading
            return _imageDataCache.GetOrAdd(filename, key =>
            {
                Console.WriteLine($"[CACHING PROXY] Cache miss for: {key}");
                Interlocked.Increment(ref _cacheMisses);
                
                // Load from real service only if not in cache
                return _realService.LoadImageData(key);
            });
        }

        public ImageMetadata GetImageMetadata(string filename)
        {
            Console.WriteLine($"[CACHING PROXY] Metadata request for: {filename}");
            
            // Check cache first
            if (_metadataCache.TryGetValue(filename, out var cachedMetadata))
            {
                Console.WriteLine($"[CACHING PROXY] Cache hit for metadata: {filename}");
                Interlocked.Increment(ref _cacheHits);
                return cachedMetadata;
            }

            Console.WriteLine($"[CACHING PROXY] Cache miss for metadata: {filename}");
            Interlocked.Increment(ref _cacheMisses);
            
            // Load from real service and cache the result
            var metadata = _realService.GetImageMetadata(filename);
            _metadataCache.TryAdd(filename, metadata);
            
            return metadata;
        }

        // Method to get cache statistics
        public void PrintCacheStats()
        {
            Console.WriteLine($"[CACHING PROXY] Cache Statistics:");
            Console.WriteLine($"  - Cache Hits: {_cacheHits}");
            Console.WriteLine($"  - Cache Misses: {_cacheMisses}");
            Console.WriteLine($"  - Hit Ratio: {(_cacheHits + _cacheMisses > 0 ? (double)_cacheHits / (_cacheHits + _cacheMisses) * 100 : 0):F1}%");
            Console.WriteLine($"  - Images Cached: {_imageDataCache.Count}");
            Console.WriteLine($"  - Metadata Cached: {_metadataCache.Count}");
        }

        // Method to clear cache (useful for memory management)
        public void ClearCache()
        {
            _imageDataCache.Clear();
            _metadataCache.Clear();
            _cacheHits = 0;
            _cacheMisses = 0;
            Console.WriteLine("[CACHING PROXY] Cache cleared");
        }
    }

    // Security Proxy: Adds access control and authentication
    // ACCESS CONTROL: Manages permissions and user authentication
    public class SecurityImageServiceProxy : IImageService
    {
        private readonly IImageService _realService;
        private readonly ConcurrentDictionary<string, UserPermissions> _userPermissions;
        private readonly string _currentUser;

        // Simple permission flags
        [Flags]
        public enum UserPermissions
        {
            None = 0,
            Read = 1,
            Display = 2,
            Metadata = 4,
            All = Read | Display | Metadata
        }

        public SecurityImageServiceProxy(IImageService realService, string currentUser)
        {
            _realService = realService ?? throw new ArgumentNullException(nameof(realService));
            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
            
            // Initialize user permissions
            _userPermissions = new ConcurrentDictionary<string, UserPermissions>();
            
            // Set up some default permissions
            _userPermissions.TryAdd("admin", UserPermissions.All);
            _userPermissions.TryAdd("user", UserPermissions.Display | UserPermissions.Metadata);
            _userPermissions.TryAdd("guest", UserPermissions.Metadata);
            
            Console.WriteLine($"[SECURITY PROXY] Created for user: {currentUser}");
        }

        private bool HasPermission(UserPermissions required)
        {
            if (!_userPermissions.TryGetValue(_currentUser, out var userPerms))
            {
                Console.WriteLine($"[SECURITY PROXY] Access denied: Unknown user {_currentUser}");
                return false;
            }

            bool hasPermission = (userPerms & required) == required;
            if (!hasPermission)
            {
                Console.WriteLine($"[SECURITY PROXY] Access denied: User {_currentUser} lacks {required} permission");
            }
            else
            {
                Console.WriteLine($"[SECURITY PROXY] Access granted: User {_currentUser} has {required} permission");
            }

            return hasPermission;
        }

        public void DisplayImage(string filename)
        {
            Console.WriteLine($"[SECURITY PROXY] Display request from {_currentUser} for: {filename}");
            
            if (!HasPermission(UserPermissions.Display))
            {
                throw new UnauthorizedAccessException($"User {_currentUser} does not have display permissions");
            }

            _realService.DisplayImage(filename);
        }

        public byte[] LoadImageData(string filename)
        {
            Console.WriteLine($"[SECURITY PROXY] Load data request from {_currentUser} for: {filename}");
            
            if (!HasPermission(UserPermissions.Read))
            {
                throw new UnauthorizedAccessException($"User {_currentUser} does not have read permissions");
            }

            return _realService.LoadImageData(filename);
        }

        public ImageMetadata GetImageMetadata(string filename)
        {
            Console.WriteLine($"[SECURITY PROXY] Metadata request from {_currentUser} for: {filename}");
            
            if (!HasPermission(UserPermissions.Metadata))
            {
                throw new UnauthorizedAccessException($"User {_currentUser} does not have metadata permissions");
            }

            return _realService.GetImageMetadata(filename);
        }

        // Method to update user permissions
        public void UpdateUserPermissions(string user, UserPermissions permissions)
        {
            _userPermissions.AddOrUpdate(user, permissions, (key, oldValue) => permissions);
            Console.WriteLine($"[SECURITY PROXY] Updated permissions for {user}: {permissions}");
        }
    }

    // Logging Proxy: Adds logging and monitoring capabilities
    public class LoggingImageServiceProxy : IImageService
    {
        private readonly IImageService _realService;
        private readonly object _logLock = new object();
        private readonly List<string> _operationLog = new List<string>();

        public LoggingImageServiceProxy(IImageService realService)
        {
            _realService = realService ?? throw new ArgumentNullException(nameof(realService));
            Console.WriteLine("[LOGGING PROXY] Created with logging enabled");
        }

        private void LogOperation(string operation, string filename, TimeSpan duration)
        {
            string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} - {operation}({filename}) - Duration: {duration.TotalMilliseconds:F0}ms";
            
            // THREAD SAFETY: Synchronize access to log collection
            lock (_logLock)
            {
                _operationLog.Add(logEntry);
                Console.WriteLine($"[LOGGING PROXY] {logEntry}");
            }
        }

        public void DisplayImage(string filename)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                _realService.DisplayImage(filename);
            }
            finally
            {
                stopwatch.Stop();
                LogOperation("DisplayImage", filename, stopwatch.Elapsed);
            }
        }

        public byte[] LoadImageData(string filename)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                return _realService.LoadImageData(filename);
            }
            finally
            {
                stopwatch.Stop();
                LogOperation("LoadImageData", filename, stopwatch.Elapsed);
            }
        }

        public ImageMetadata GetImageMetadata(string filename)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                return _realService.GetImageMetadata(filename);
            }
            finally
            {
                stopwatch.Stop();
                LogOperation("GetImageMetadata", filename, stopwatch.Elapsed);
            }
        }

        public void PrintOperationLog()
        {
            Console.WriteLine("\n[LOGGING PROXY] Operation Log:");
            lock (_logLock)
            {
                foreach (var entry in _operationLog)
                {
                    Console.WriteLine($"  {entry}");
                }
            }
        }
    }

    // Composite Proxy: Combines multiple proxy behaviors
    // COMPOSITION: Shows how to stack multiple proxy behaviors
    public class CompositeProxyExample
    {
        public static IImageService CreateFullFeaturedProxy(string currentUser)
        {
            // Build proxy chain: Logging -> Security -> Caching -> Lazy
            // Each proxy wraps the next, creating a chain of behaviors
            
            Console.WriteLine("[COMPOSITE] Building proxy chain...");
            
            // 1. Start with lazy proxy (delays real service creation)
            var lazyProxy = new LazyImageServiceProxy("Composite");
            
            // 2. Add caching on top of lazy proxy
            var cachingProxy = new CachingImageServiceProxy(lazyProxy);
            
            // 3. Add security on top of caching proxy
            var securityProxy = new SecurityImageServiceProxy(cachingProxy, currentUser);
            
            // 4. Add logging on top of security proxy
            var loggingProxy = new LoggingImageServiceProxy(securityProxy);
            
            Console.WriteLine("[COMPOSITE] Proxy chain created: Logging -> Security -> Caching -> Lazy -> Real Service");
            
            return loggingProxy;
        }
    }

    // Multithreading demonstration
    public class MultithreadingDemo
    {
        public static async Task RunConcurrentTest()
        {
            Console.WriteLine("\n[MULTITHREAD TEST] Testing thread safety of proxies...");
            
            // Create a caching proxy to test concurrent access
            var realService = new ExpensiveImageService();
            var cachingProxy = new CachingImageServiceProxy(realService);
            
            // Create multiple tasks that access the same images concurrently
            var tasks = new Task[10];
            var imageFiles = new[] { "image1.jpg", "image2.jpg", "image3.jpg" };
            
            for (int i = 0; i < tasks.Length; i++)
            {
                int taskId = i;
                tasks[i] = Task.Run(async () =>
                {
                    for (int j = 0; j < 3; j++)
                    {
                        var filename = imageFiles[j % imageFiles.Length];
                        
                        // Test concurrent metadata access
                        var metadata = cachingProxy.GetImageMetadata(filename);
                        Console.WriteLine($"[THREAD {taskId}] Got metadata: {metadata.Filename}");
                        
                        // Test concurrent data loading
                        var data = cachingProxy.LoadImageData(filename);
                        Console.WriteLine($"[THREAD {taskId}] Loaded {data.Length} bytes for {filename}");
                        
                        await Task.Delay(50); // Small delay to increase concurrency
                    }
                });
            }
            
            await Task.WhenAll(tasks);
            
            // Show cache statistics after concurrent access
            if (cachingProxy is CachingImageServiceProxy cp)
            {
                cp.PrintCacheStats();
            }
            
            Console.WriteLine("[MULTITHREAD TEST] All tasks completed successfully");
        }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== PROXY PATTERN DEMONSTRATION ===\n");

            // Example 1: Lazy Proxy (Virtual Proxy)
            Console.WriteLine("1. LAZY PROXY (VIRTUAL PROXY) - Defers expensive object creation:");
            Console.WriteLine(new string('-', 60));
            
            var lazyProxy = new LazyImageServiceProxy("Example1");
            Console.WriteLine($"Service initialized: {lazyProxy.IsServiceInitialized}");
            
            Console.WriteLine("\nFirst method call (triggers real service creation):");
            lazyProxy.GetImageMetadata("vacation.jpg");
            Console.WriteLine($"Service initialized: {lazyProxy.IsServiceInitialized}");
            
            Console.WriteLine("\nSubsequent calls use existing service:");
            lazyProxy.DisplayImage("vacation.jpg");

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();

            // Example 2: Caching Proxy
            Console.WriteLine("\n\n2. CACHING PROXY - Reduces expensive operations:");
            Console.WriteLine(new string('-', 60));
            
            var realService = new ExpensiveImageService();
            var cachingProxy = new CachingImageServiceProxy(realService);
            
            // First access - cache miss
            Console.WriteLine("\nFirst access (cache miss):");
            cachingProxy.GetImageMetadata("photo1.jpg");
            cachingProxy.LoadImageData("photo1.jpg");
            
            // Second access - cache hit
            Console.WriteLine("\nSecond access (cache hit):");
            cachingProxy.GetImageMetadata("photo1.jpg");
            cachingProxy.LoadImageData("photo1.jpg");
            
            cachingProxy.PrintCacheStats();

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();

            // Example 3: Security Proxy
            Console.WriteLine("\n\n3. SECURITY PROXY - Access control and authentication:");
            Console.WriteLine(new string('-', 60));
            
            Console.WriteLine("\nTesting with 'admin' user (full permissions):");
            var adminProxy = new SecurityImageServiceProxy(realService, "admin");
            try
            {
                adminProxy.GetImageMetadata("secure.jpg");
                adminProxy.LoadImageData("secure.jpg");
                adminProxy.DisplayImage("secure.jpg");
                Console.WriteLine("All operations successful for admin");
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Access denied: {ex.Message}");
            }
            
            Console.WriteLine("\nTesting with 'guest' user (limited permissions):");
            var guestProxy = new SecurityImageServiceProxy(realService, "guest");
            try
            {
                guestProxy.GetImageMetadata("secure.jpg"); // Should work
                Console.WriteLine("Metadata access successful for guest");
                
                guestProxy.LoadImageData("secure.jpg"); // Should fail
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Expected access denial: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();

            // Example 4: Composite Proxy
            Console.WriteLine("\n\n4. COMPOSITE PROXY - Multiple behaviors combined:");
            Console.WriteLine(new string('-', 60));
            
            var compositeProxy = CompositeProxyExample.CreateFullFeaturedProxy("admin");
            
            Console.WriteLine("\nUsing composite proxy with all features:");
            compositeProxy.GetImageMetadata("composite.jpg");
            compositeProxy.DisplayImage("composite.jpg");
            
            // Access logging if it's a logging proxy
            if (compositeProxy is LoggingImageServiceProxy lp)
            {
                lp.PrintOperationLog();
            }

            Console.WriteLine("\nPress any key to continue to multithreading demo...");
            Console.ReadKey();

            // Example 5: Multithreading demonstration
            Console.WriteLine("\n\n5. MULTITHREADING DEMONSTRATION - Thread safety:");
            Console.WriteLine(new string('-', 60));
            await MultithreadingDemo.RunConcurrentTest();

            // Example 6: Memory efficiency comparison
            Console.WriteLine("\n\n6. MEMORY EFFICIENCY ANALYSIS:");
            Console.WriteLine(new string('-', 60));
            Console.WriteLine("Without proxy: Expensive objects created immediately, no caching");
            Console.WriteLine("With lazy proxy: Object creation deferred until needed");
            Console.WriteLine("With caching proxy: Results cached to avoid repeated expensive operations");
            Console.WriteLine("With composite proxy: Multiple optimizations combined");
            Console.WriteLine("\nMemory benefits:");
            Console.WriteLine("- Lazy loading: Saves memory when objects aren't used");
            Console.WriteLine("- Caching: Trades memory for performance, avoids duplicate work");
            Console.WriteLine("- Resource pooling: Manages limited resources efficiently");

            Console.WriteLine("\n\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
