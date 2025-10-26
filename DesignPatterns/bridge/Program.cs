using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.Json;

/*
 * BRIDGE DESIGN PATTERN IN C#
 * 
 * PURPOSE:
 * The Bridge pattern separates an abstraction from its implementation, allowing both to vary
 * independently. It uses composition over inheritance to connect different hierarchies and
 * provides a bridge between the abstraction and implementation layers.
 * 
 * CORE BENEFITS:
 * - Decouples abstraction from implementation for independent evolution
 * - Enables runtime switching of implementations
 * - Reduces the number of classes in complex inheritance hierarchies
 * - Follows the Open/Closed Principle - open for extension, closed for modification
 * - Improves code maintainability by separating concerns
 * - Allows sharing of implementations among multiple abstractions
 * - Supports platform-specific implementations without affecting client code
 * - Enables gradual migration between different implementation strategies
 * 
 * SCENARIOS TO USE:
 * - When you need to avoid permanent binding between abstraction and implementation
 * - Supporting multiple platforms (Windows, Linux, macOS) with different implementations
 * - Device drivers and hardware abstraction layers
 * - Different rendering engines (2D/3D graphics, different APIs)
 * - Multiple database backends with common interface
 * - Different messaging systems (email, SMS, push notifications)
 * - Cross-platform UI frameworks with platform-specific rendering
 * - Different authentication mechanisms (OAuth, LDAP, local)
 * - Multiple payment processing backends
 * - Different logging destinations (file, database, cloud)
 * - Various encryption algorithms with common interface
 * - Different compression algorithms
 * 
 * SCENARIOS NOT TO USE:
 * - When abstraction and implementation will never change independently
 * - Simple systems with only one implementation expected
 * - When the overhead of the bridge structure outweighs benefits
 * - Performance-critical code where the extra indirection is costly
 * - When inheritance would be simpler and sufficient
 * - Small projects with well-defined, stable requirements
 * - When the implementation details are tightly coupled to the abstraction
 * - If you're dealing with simple data transfer objects
 */

namespace BridgePattern
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== Bridge Design Pattern Examples ===\n");

            // Example 1: Graphics Rendering System
            GraphicsRenderingExample();

            // Example 2: Database Connection Bridge
            DatabaseConnectionExample();

            // Example 3: Messaging System Bridge
            MessagingSystemExample();

            // Example 4: Device Driver Bridge
            DeviceDriverExample();

            // Example 5: Thread-Safe Bridge Pattern
            await ThreadSafeBridgeExample();

            // Example 6: Authentication System Bridge
            AuthenticationSystemExample();

            // Example 7: File Storage Bridge
            await FileStorageBridgeExample();

            // Example 8: Compression Algorithm Bridge
            CompressionAlgorithmExample();

            // Example 9: Cross-Platform UI Bridge
            CrossPlatformUIExample();

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        #region Example 1: Graphics Rendering System

        static void GraphicsRenderingExample()
        {
            Console.WriteLine("1. Graphics Rendering System:");
            Console.WriteLine("==============================");

            // Different rendering implementations
            IRenderer directXRenderer = new DirectXRenderer();
            IRenderer openGLRenderer = new OpenGLRenderer();
            IRenderer vulkanRenderer = new VulkanRenderer();

            // Different shape abstractions using various renderers
            Shape circle = new Circle(directXRenderer, 5.0f, 10.0f, 15.0f);
            circle.Draw();
            circle.Resize(1.5f);

            Shape rectangle = new Rectangle(openGLRenderer, 0.0f, 0.0f, 20.0f, 10.0f);
            rectangle.Draw();
            rectangle.Move(5.0f, 5.0f);

            Shape triangle = new Triangle(vulkanRenderer, 2.0f, 3.0f, 5.0f);
            triangle.Draw();

            // Switch renderer at runtime
            Console.WriteLine("\nSwitching circle renderer to OpenGL:");
            circle = new Circle(openGLRenderer, circle.X, circle.Y, ((Circle)circle).Radius);
            circle.Draw();

            Console.WriteLine();
        }

        #endregion

        #region Example 2: Database Connection Bridge

        static void DatabaseConnectionExample()
        {
            Console.WriteLine("2. Database Connection Bridge:");
            Console.WriteLine("===============================");

            // Different database implementations
            var databases = new UserRepository[]
            {
                new UserRepository(new SqlServerDatabase()),
                new UserRepository(new MySqlDatabase()),
                new UserRepository(new MongoDatabase())
            };

            var user = new User { Id = 1, Name = "Alice", Email = "alice@example.com" };

            foreach (var db in databases)
            {
                Console.WriteLine($"\nUsing {db.GetType().Name} with {db.Database.GetType().Name}:");
                db.Save(user);
                var retrieved = db.FindById(1);
                Console.WriteLine($"Found: {retrieved?.Name}");
                db.Delete(1);
            }

            Console.WriteLine();
        }

        #endregion

        #region Example 3: Messaging System Bridge

        static void MessagingSystemExample()
        {
            Console.WriteLine("3. Messaging System Bridge:");
            Console.WriteLine("============================");

            // Different messaging implementations
            var messageServices = new NotificationSender[]
            {
                new NotificationSender(new EmailService()),
                new NotificationSender(new SmsService()),
                new NotificationSender(new PushNotificationService())
            };

            var message = new Message
            {
                Recipient = "user@example.com",
                Subject = "Test Notification",
                Content = "This is a test message"
            };

            foreach (var service in messageServices)
            {
                Console.WriteLine($"\n{service.MessageService.GetType().Name}:");
                service.SendMessage(message);
                service.SendUrgentMessage(message);
            }

            Console.WriteLine();
        }

        #endregion

        #region Example 4: Device Driver Bridge

        static void DeviceDriverExample()
        {
            Console.WriteLine("4. Device Driver Bridge:");
            Console.WriteLine("=========================");

            // Different device implementations
            var devices = new Device[]
            {
                new Printer(new InkjetDriver()),
                new Printer(new LaserDriver()),
                new Scanner(new FlatbedDriver()),
                new Scanner(new SheetFedDriver())
            };

            var document = "Important Document Content";

            foreach (var device in devices)
            {
                Console.WriteLine($"\n{device.GetType().Name} with {device.Driver.GetType().Name}:");
                
                if (device is Printer printer)
                {
                    printer.Print(document);
                    printer.PrintDoubleSided(document);
                }
                else if (device is Scanner scanner)
                {
                    scanner.Scan();
                    scanner.ScanToFile("scanned_document.pdf");
                }
            }

            Console.WriteLine();
        }

        #endregion

        #region Example 5: Thread-Safe Bridge Pattern

        static async Task ThreadSafeBridgeExample()
        {
            Console.WriteLine("5. Thread-Safe Bridge Pattern:");
            Console.WriteLine("===============================");

            // Thread-safe cache implementations
            var caches = new CacheManager[]
            {
                new CacheManager(new MemoryCache()),
                new CacheManager(new RedisCache()),
                new CacheManager(new FileCache())
            };

            var tasks = new List<Task>();

            foreach (var cache in caches)
            {
                tasks.Add(Task.Run(async () =>
                {
                    var cacheType = cache.CacheImplementation.GetType().Name;
                    Console.WriteLine($"\n[{cacheType}] Starting concurrent operations");

                    // Simulate concurrent cache operations
                    var concurrentTasks = new List<Task>();
                    
                    for (int i = 0; i < 5; i++)
                    {
                        int key = i;
                        concurrentTasks.Add(Task.Run(async () =>
                        {
                            await cache.SetAsync($"key{key}", $"value{key}", TimeSpan.FromMinutes(1));
                            await Task.Delay(50);
                            var value = await cache.GetAsync($"key{key}");
                            Console.WriteLine($"[{cacheType}] Retrieved: {value}");
                        }));
                    }

                    await Task.WhenAll(concurrentTasks);
                    Console.WriteLine($"[{cacheType}] Completed concurrent operations");
                }));
            }

            await Task.WhenAll(tasks);
            Console.WriteLine();
        }

        #endregion

        #region Example 6: Authentication System Bridge

        static void AuthenticationSystemExample()
        {
            Console.WriteLine("6. Authentication System Bridge:");
            Console.WriteLine("=================================");

            // Different authentication implementations
            var authSystems = new AuthenticationManager[]
            {
                new AuthenticationManager(new LocalAuthProvider()),
                new AuthenticationManager(new LdapAuthProvider()),
                new AuthenticationManager(new OAuthProvider())
            };

            var credentials = new Credentials
            {
                Username = "testuser",
                Password = "password123",
                Token = "oauth-token-xyz"
            };

            foreach (var auth in authSystems)
            {
                Console.WriteLine($"\n{auth.AuthProvider.GetType().Name}:");
                var loginResult = auth.Login(credentials);
                Console.WriteLine($"Login: {loginResult}");

                if (loginResult)
                {
                    var user = auth.GetCurrentUser();
                    Console.WriteLine($"Current user: {user.Username} ({user.Role})");
                    
                    var hasPermission = auth.HasPermission("READ_DATA");
                    Console.WriteLine($"Has READ_DATA permission: {hasPermission}");
                    
                    auth.Logout();
                    Console.WriteLine("Logged out");
                }
            }

            Console.WriteLine();
        }

        #endregion

        #region Example 7: File Storage Bridge

        static async Task FileStorageBridgeExample()
        {
            Console.WriteLine("7. File Storage Bridge:");
            Console.WriteLine("========================");

            // Different storage implementations
            var storageManagers = new FileManager[]
            {
                new FileManager(new LocalFileStorage()),
                new FileManager(new CloudFileStorage()),
                new FileManager(new NetworkFileStorage())
            };

            var fileContent = "This is sample file content for storage testing.";
            var fileName = "test-file.txt";

            foreach (var storage in storageManagers)
            {
                Console.WriteLine($"\n{storage.Storage.GetType().Name}:");
                
                try
                {
                    await storage.SaveFileAsync(fileName, fileContent);
                    var content = await storage.ReadFileAsync(fileName);
                    Console.WriteLine($"Read content: {content}");
                    
                    var exists = await storage.FileExistsAsync(fileName);
                    Console.WriteLine($"File exists: {exists}");
                    
                    await storage.DeleteFileAsync(fileName);
                    Console.WriteLine("File deleted");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            Console.WriteLine();
        }

        #endregion

        #region Example 8: Compression Algorithm Bridge

        static void CompressionAlgorithmExample()
        {
            Console.WriteLine("8. Compression Algorithm Bridge:");
            Console.WriteLine("=================================");

            var data = "This is sample data that will be compressed using different algorithms. " +
                      "The Bridge pattern allows us to switch compression algorithms at runtime.";

            // Different compression implementations
            var compressors = new DataCompressor[]
            {
                new DataCompressor(new GzipCompression()),
                new DataCompressor(new ZipCompression()),
                new DataCompressor(new LzmaCompression())
            };

            foreach (var compressor in compressors)
            {
                Console.WriteLine($"\n{compressor.CompressionAlgorithm.GetType().Name}:");
                
                var compressed = compressor.Compress(data);
                Console.WriteLine($"Original size: {data.Length} bytes");
                Console.WriteLine($"Compressed size: {compressed.Length} bytes");
                Console.WriteLine($"Compression ratio: {(1.0 - (double)compressed.Length / data.Length):P2}");
                
                var decompressed = compressor.Decompress(compressed);
                Console.WriteLine($"Decompression successful: {decompressed == data}");
            }

            Console.WriteLine();
        }

        #endregion

        #region Example 9: Cross-Platform UI Bridge

        static void CrossPlatformUIExample()
        {
            Console.WriteLine("9. Cross-Platform UI Bridge:");
            Console.WriteLine("=============================");

            // Different platform implementations
            var platforms = new IPlatformUI[]
            {
                new WindowsUI(),
                new MacOSUI(),
                new LinuxUI()
            };

            foreach (var platform in platforms)
            {
                Console.WriteLine($"\n{platform.GetType().Name}:");
                
                var window = new Window(platform, "Sample Application", 800, 600);
                window.Show();
                window.SetTitle("Updated Title");
                window.Resize(1024, 768);
                
                var button = new Button(platform, "Click Me", 100, 50);
                button.Render();
                button.SetEnabled(false);
                
                window.Hide();
            }

            Console.WriteLine();
        }

        #endregion
    }

    #region Core Bridge Pattern Classes

    // Abstraction
    public abstract class RemoteControl
    {
        protected IDevice device;

        protected RemoteControl(IDevice device)
        {
            this.device = device;
        }

        public abstract void Power();
        public abstract void VolumeUp();
        public abstract void VolumeDown();
    }

    // Refined Abstraction
    public class BasicRemote : RemoteControl
    {
        public BasicRemote(IDevice device) : base(device) { }

        public override void Power()
        {
            if (device.IsEnabled())
                device.Disable();
            else
                device.Enable();
        }

        public override void VolumeUp()
        {
            device.SetVolume(device.GetVolume() + 10);
        }

        public override void VolumeDown()
        {
            device.SetVolume(device.GetVolume() - 10);
        }
    }

    // Implementation Interface
    public interface IDevice
    {
        bool IsEnabled();
        void Enable();
        void Disable();
        int GetVolume();
        void SetVolume(int volume);
    }

    // Concrete Implementations
    public class Television : IDevice
    {
        private bool enabled = false;
        private int volume = 50;

        public bool IsEnabled() => enabled;
        public void Enable() { enabled = true; Console.WriteLine("TV turned on"); }
        public void Disable() { enabled = false; Console.WriteLine("TV turned off"); }
        public int GetVolume() => volume;
        public void SetVolume(int volume) { this.volume = Math.Max(0, Math.Min(100, volume)); Console.WriteLine($"TV volume: {this.volume}"); }
    }

    public class Radio : IDevice
    {
        private bool enabled = false;
        private int volume = 30;

        public bool IsEnabled() => enabled;
        public void Enable() { enabled = true; Console.WriteLine("Radio turned on"); }
        public void Disable() { enabled = false; Console.WriteLine("Radio turned off"); }
        public int GetVolume() => volume;
        public void SetVolume(int volume) { this.volume = Math.Max(0, Math.Min(100, volume)); Console.WriteLine($"Radio volume: {this.volume}"); }
    }

    #endregion

    #region Example 1: Graphics Rendering Classes

    // Implementation interface
    public interface IRenderer
    {
        void RenderCircle(float x, float y, float radius);
        void RenderRectangle(float x, float y, float width, float height);
        void RenderTriangle(float x, float y, float size);
        void SetColor(string color);
    }

    // Concrete implementations
    public class DirectXRenderer : IRenderer
    {
        public void RenderCircle(float x, float y, float radius)
        {
            Console.WriteLine($"  DirectX: Rendering circle at ({x}, {y}) with radius {radius}");
        }

        public void RenderRectangle(float x, float y, float width, float height)
        {
            Console.WriteLine($"  DirectX: Rendering rectangle at ({x}, {y}) size {width}x{height}");
        }

        public void RenderTriangle(float x, float y, float size)
        {
            Console.WriteLine($"  DirectX: Rendering triangle at ({x}, {y}) size {size}");
        }

        public void SetColor(string color)
        {
            Console.WriteLine($"  DirectX: Setting color to {color}");
        }
    }

    public class OpenGLRenderer : IRenderer
    {
        public void RenderCircle(float x, float y, float radius)
        {
            Console.WriteLine($"  OpenGL: Drawing circle at ({x}, {y}) with radius {radius}");
        }

        public void RenderRectangle(float x, float y, float width, float height)
        {
            Console.WriteLine($"  OpenGL: Drawing rectangle at ({x}, {y}) size {width}x{height}");
        }

        public void RenderTriangle(float x, float y, float size)
        {
            Console.WriteLine($"  OpenGL: Drawing triangle at ({x}, {y}) size {size}");
        }

        public void SetColor(string color)
        {
            Console.WriteLine($"  OpenGL: Setting color to {color}");
        }
    }

    public class VulkanRenderer : IRenderer
    {
        public void RenderCircle(float x, float y, float radius)
        {
            Console.WriteLine($"  Vulkan: Creating circle at ({x}, {y}) with radius {radius}");
        }

        public void RenderRectangle(float x, float y, float width, float height)
        {
            Console.WriteLine($"  Vulkan: Creating rectangle at ({x}, {y}) size {width}x{height}");
        }

        public void RenderTriangle(float x, float y, float size)
        {
            Console.WriteLine($"  Vulkan: Creating triangle at ({x}, {y}) size {size}");
        }

        public void SetColor(string color)
        {
            Console.WriteLine($"  Vulkan: Setting color to {color}");
        }
    }

    // Abstraction
    public abstract class Shape
    {
        protected IRenderer renderer;
        public float X { get; protected set; }
        public float Y { get; protected set; }

        protected Shape(IRenderer renderer, float x, float y)
        {
            this.renderer = renderer;
            X = x;
            Y = y;
        }

        public abstract void Draw();
        public abstract void Resize(float factor);
        
        public virtual void Move(float deltaX, float deltaY)
        {
            X += deltaX;
            Y += deltaY;
            Console.WriteLine($"  Moved to ({X}, {Y})");
        }
    }

    // Refined abstractions
    public class Circle : Shape
    {
        public float Radius { get; private set; }

        public Circle(IRenderer renderer, float x, float y, float radius) : base(renderer, x, y)
        {
            Radius = radius;
        }

        public override void Draw()
        {
            renderer.SetColor("Blue");
            renderer.RenderCircle(X, Y, Radius);
        }

        public override void Resize(float factor)
        {
            Radius *= factor;
            Console.WriteLine($"  Circle resized to radius {Radius}");
        }
    }

    public class Rectangle : Shape
    {
        public float Width { get; private set; }
        public float Height { get; private set; }

        public Rectangle(IRenderer renderer, float x, float y, float width, float height) : base(renderer, x, y)
        {
            Width = width;
            Height = height;
        }

        public override void Draw()
        {
            renderer.SetColor("Red");
            renderer.RenderRectangle(X, Y, Width, Height);
        }

        public override void Resize(float factor)
        {
            Width *= factor;
            Height *= factor;
            Console.WriteLine($"  Rectangle resized to {Width}x{Height}");
        }
    }

    public class Triangle : Shape
    {
        public float Size { get; private set; }

        public Triangle(IRenderer renderer, float x, float y, float size) : base(renderer, x, y)
        {
            Size = size;
        }

        public override void Draw()
        {
            renderer.SetColor("Green");
            renderer.RenderTriangle(X, Y, Size);
        }

        public override void Resize(float factor)
        {
            Size *= factor;
            Console.WriteLine($"  Triangle resized to size {Size}");
        }
    }

    #endregion

    #region Example 2: Database Connection Classes

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    // Implementation interface
    public interface IDatabase
    {
        void Connect();
        void Disconnect();
        void Insert(string table, object data);
        object? Select(string table, int id);
        void Delete(string table, int id);
    }

    // Concrete implementations
    public class SqlServerDatabase : IDatabase
    {
        public void Connect() => Console.WriteLine("  SQL Server: Connected");
        public void Disconnect() => Console.WriteLine("  SQL Server: Disconnected");
        
        public void Insert(string table, object data)
        {
            Console.WriteLine($"  SQL Server: INSERT INTO {table}");
        }

        public object? Select(string table, int id)
        {
            Console.WriteLine($"  SQL Server: SELECT FROM {table} WHERE Id = {id}");
            return new User { Id = id, Name = "SQL User", Email = "sql@example.com" };
        }

        public void Delete(string table, int id)
        {
            Console.WriteLine($"  SQL Server: DELETE FROM {table} WHERE Id = {id}");
        }
    }

    public class MySqlDatabase : IDatabase
    {
        public void Connect() => Console.WriteLine("  MySQL: Connected");
        public void Disconnect() => Console.WriteLine("  MySQL: Disconnected");
        
        public void Insert(string table, object data)
        {
            Console.WriteLine($"  MySQL: INSERT INTO {table}");
        }

        public object? Select(string table, int id)
        {
            Console.WriteLine($"  MySQL: SELECT FROM {table} WHERE Id = {id}");
            return new User { Id = id, Name = "MySQL User", Email = "mysql@example.com" };
        }

        public void Delete(string table, int id)
        {
            Console.WriteLine($"  MySQL: DELETE FROM {table} WHERE Id = {id}");
        }
    }

    public class MongoDatabase : IDatabase
    {
        public void Connect() => Console.WriteLine("  MongoDB: Connected");
        public void Disconnect() => Console.WriteLine("  MongoDB: Disconnected");
        
        public void Insert(string table, object data)
        {
            Console.WriteLine($"  MongoDB: db.{table}.insertOne()");
        }

        public object? Select(string table, int id)
        {
            Console.WriteLine($"  MongoDB: db.{table}.findOne({{_id: {id}}})");
            return new User { Id = id, Name = "Mongo User", Email = "mongo@example.com" };
        }

        public void Delete(string table, int id)
        {
            Console.WriteLine($"  MongoDB: db.{table}.deleteOne({{_id: {id}}})");
        }
    }

    // Abstraction
    public class UserRepository
    {
        public IDatabase Database { get; private set; }

        public UserRepository(IDatabase database)
        {
            Database = database;
            database.Connect();
        }

        public void Save(User user)
        {
            Database.Insert("Users", user);
        }

        public User? FindById(int id)
        {
            return Database.Select("Users", id) as User;
        }

        public void Delete(int id)
        {
            Database.Delete("Users", id);
        }
    }

    #endregion

    #region Example 3: Messaging System Classes

    public class Message
    {
        public string Recipient { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }

    // Implementation interface
    public interface IMessageService
    {
        void Send(Message message);
        void SendWithPriority(Message message, string priority);
    }

    // Concrete implementations
    public class EmailService : IMessageService
    {
        public void Send(Message message)
        {
            Console.WriteLine($"  Email: Sending to {message.Recipient}");
            Console.WriteLine($"  Subject: {message.Subject}");
        }

        public void SendWithPriority(Message message, string priority)
        {
            Console.WriteLine($"  Email: Sending {priority} priority email to {message.Recipient}");
        }
    }

    public class SmsService : IMessageService
    {
        public void Send(Message message)
        {
            Console.WriteLine($"  SMS: Sending to {message.Recipient}");
            Console.WriteLine($"  Message: {message.Content.Substring(0, Math.Min(160, message.Content.Length))}");
        }

        public void SendWithPriority(Message message, string priority)
        {
            Console.WriteLine($"  SMS: Sending {priority} priority SMS to {message.Recipient}");
        }
    }

    public class PushNotificationService : IMessageService
    {
        public void Send(Message message)
        {
            Console.WriteLine($"  Push: Sending notification to {message.Recipient}");
            Console.WriteLine($"  Title: {message.Subject}");
        }

        public void SendWithPriority(Message message, string priority)
        {
            Console.WriteLine($"  Push: Sending {priority} priority notification to {message.Recipient}");
        }
    }

    // Abstraction
    public class NotificationSender
    {
        public IMessageService MessageService { get; private set; }

        public NotificationSender(IMessageService messageService)
        {
            MessageService = messageService;
        }

        public void SendMessage(Message message)
        {
            MessageService.Send(message);
        }

        public void SendUrgentMessage(Message message)
        {
            MessageService.SendWithPriority(message, "HIGH");
        }
    }

    #endregion

    #region Example 4: Device Driver Classes

    // Implementation interface
    public interface IDeviceDriver
    {
        void Initialize();
        void SendCommand(string command);
        string ReceiveData();
    }

    // Concrete implementations
    public class InkjetDriver : IDeviceDriver
    {
        public void Initialize() => Console.WriteLine("  Inkjet driver initialized");
        public void SendCommand(string command) => Console.WriteLine($"  Inkjet: {command}");
        public string ReceiveData() => "Inkjet status: Ready";
    }

    public class LaserDriver : IDeviceDriver
    {
        public void Initialize() => Console.WriteLine("  Laser driver initialized");
        public void SendCommand(string command) => Console.WriteLine($"  Laser: {command}");
        public string ReceiveData() => "Laser status: Ready";
    }

    public class FlatbedDriver : IDeviceDriver
    {
        public void Initialize() => Console.WriteLine("  Flatbed scanner driver initialized");
        public void SendCommand(string command) => Console.WriteLine($"  Flatbed: {command}");
        public string ReceiveData() => "Flatbed scanner status: Ready";
    }

    public class SheetFedDriver : IDeviceDriver
    {
        public void Initialize() => Console.WriteLine("  Sheet-fed scanner driver initialized");
        public void SendCommand(string command) => Console.WriteLine($"  Sheet-fed: {command}");
        public string ReceiveData() => "Sheet-fed scanner status: Ready";
    }

    // Abstraction
    public abstract class Device
    {
        public IDeviceDriver Driver { get; protected set; }

        protected Device(IDeviceDriver driver)
        {
            Driver = driver;
            driver.Initialize();
        }

        public abstract void PerformOperation();
    }

    // Refined abstractions
    public class Printer : Device
    {
        public Printer(IDeviceDriver driver) : base(driver) { }

        public override void PerformOperation()
        {
            Driver.SendCommand("PRINT_TEST_PAGE");
        }

        public void Print(string document)
        {
            Driver.SendCommand($"PRINT: {document}");
        }

        public void PrintDoubleSided(string document)
        {
            Driver.SendCommand($"PRINT_DUPLEX: {document}");
        }
    }

    public class Scanner : Device
    {
        public Scanner(IDeviceDriver driver) : base(driver) { }

        public override void PerformOperation()
        {
            Driver.SendCommand("SCAN_TEST");
        }

        public void Scan()
        {
            Driver.SendCommand("SCAN");
            var status = Driver.ReceiveData();
            Console.WriteLine($"  Scan result: {status}");
        }

        public void ScanToFile(string filename)
        {
            Driver.SendCommand($"SCAN_TO_FILE: {filename}");
        }
    }

    #endregion

    #region Example 5: Thread-Safe Cache Classes

    // Implementation interface
    public interface ICacheImplementation
    {
        Task SetAsync(string key, string value, TimeSpan expiration);
        Task<string?> GetAsync(string key);
        Task RemoveAsync(string key);
        Task<bool> ExistsAsync(string key);
    }

    // Thread-safe implementations
    public class MemoryCache : ICacheImplementation
    {
        private readonly ConcurrentDictionary<string, (string Value, DateTime Expiry)> _cache = new();

        public async Task SetAsync(string key, string value, TimeSpan expiration)
        {
            await Task.Delay(10); // Simulate async operation
            var expiry = DateTime.Now.Add(expiration);
            _cache.AddOrUpdate(key, (value, expiry), (k, v) => (value, expiry));
            Console.WriteLine($"  MemoryCache: Set {key}");
        }

        public async Task<string?> GetAsync(string key)
        {
            await Task.Delay(5);
            if (_cache.TryGetValue(key, out var cached) && cached.Expiry > DateTime.Now)
            {
                return cached.Value;
            }
            return null;
        }

        public async Task RemoveAsync(string key)
        {
            await Task.Delay(5);
            _cache.TryRemove(key, out _);
        }

        public async Task<bool> ExistsAsync(string key)
        {
            await Task.Delay(5);
            return _cache.ContainsKey(key) && _cache[key].Expiry > DateTime.Now;
        }
    }

    public class RedisCache : ICacheImplementation
    {
        private readonly ConcurrentDictionary<string, (string Value, DateTime Expiry)> _mockRedis = new();

        public async Task SetAsync(string key, string value, TimeSpan expiration)
        {
            await Task.Delay(50); // Simulate network latency
            var expiry = DateTime.Now.Add(expiration);
            _mockRedis.AddOrUpdate(key, (value, expiry), (k, v) => (value, expiry));
            Console.WriteLine($"  RedisCache: Set {key}");
        }

        public async Task<string?> GetAsync(string key)
        {
            await Task.Delay(30);
            if (_mockRedis.TryGetValue(key, out var cached) && cached.Expiry > DateTime.Now)
            {
                return cached.Value;
            }
            return null;
        }

        public async Task RemoveAsync(string key)
        {
            await Task.Delay(30);
            _mockRedis.TryRemove(key, out _);
        }

        public async Task<bool> ExistsAsync(string key)
        {
            await Task.Delay(25);
            return _mockRedis.ContainsKey(key) && _mockRedis[key].Expiry > DateTime.Now;
        }
    }

    public class FileCache : ICacheImplementation
    {
        private readonly object _fileLock = new object();

        public async Task SetAsync(string key, string value, TimeSpan expiration)
        {
            await Task.Run(() =>
            {
                lock (_fileLock)
                {
                    // Simulate file writing
                    Thread.Sleep(100);
                    Console.WriteLine($"  FileCache: Set {key} to file");
                }
            });
        }

        public async Task<string?> GetAsync(string key)
        {
            return await Task.Run(() =>
            {
                lock (_fileLock)
                {
                    Thread.Sleep(80);
                    return $"cached_value_for_{key}";
                }
            });
        }

        public async Task RemoveAsync(string key)
        {
            await Task.Run(() =>
            {
                lock (_fileLock)
                {
                    Thread.Sleep(60);
                    Console.WriteLine($"  FileCache: Removed {key}");
                }
            });
        }

        public async Task<bool> ExistsAsync(string key)
        {
            return await Task.Run(() =>
            {
                lock (_fileLock)
                {
                    Thread.Sleep(40);
                    return true; // Simulate file exists
                }
            });
        }
    }

    // Abstraction
    public class CacheManager
    {
        public ICacheImplementation CacheImplementation { get; private set; }

        public CacheManager(ICacheImplementation cacheImplementation)
        {
            CacheImplementation = cacheImplementation;
        }

        public async Task SetAsync(string key, string value, TimeSpan expiration)
        {
            await CacheImplementation.SetAsync(key, value, expiration);
        }

        public async Task<string?> GetAsync(string key)
        {
            return await CacheImplementation.GetAsync(key);
        }

        public async Task RemoveAsync(string key)
        {
            await CacheImplementation.RemoveAsync(key);
        }

        public async Task<bool> ExistsAsync(string key)
        {
            return await CacheImplementation.ExistsAsync(key);
        }
    }

    #endregion

    #region Example 6: Authentication System Classes

    public class Credentials
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }

    public class AuthenticatedUser
    {
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public List<string> Permissions { get; set; } = new();
    }

    // Implementation interface
    public interface IAuthProvider
    {
        bool Authenticate(Credentials credentials);
        AuthenticatedUser GetUser(string username);
        bool HasPermission(string username, string permission);
        void Logout(string username);
    }

    // Concrete implementations
    public class LocalAuthProvider : IAuthProvider
    {
        private readonly Dictionary<string, string> _users = new()
        {
            ["testuser"] = "password123"
        };

        public bool Authenticate(Credentials credentials)
        {
            Console.WriteLine($"  Local: Authenticating {credentials.Username}");
            return _users.ContainsKey(credentials.Username) && 
                   _users[credentials.Username] == credentials.Password;
        }

        public AuthenticatedUser GetUser(string username)
        {
            return new AuthenticatedUser
            {
                Username = username,
                Role = "User",
                Permissions = new List<string> { "READ_DATA", "WRITE_DATA" }
            };
        }

        public bool HasPermission(string username, string permission)
        {
            var user = GetUser(username);
            return user.Permissions.Contains(permission);
        }

        public void Logout(string username)
        {
            Console.WriteLine($"  Local: Logged out {username}");
        }
    }

    public class LdapAuthProvider : IAuthProvider
    {
        public bool Authenticate(Credentials credentials)
        {
            Console.WriteLine($"  LDAP: Authenticating {credentials.Username}");
            // Simulate LDAP authentication
            return !string.IsNullOrEmpty(credentials.Username);
        }

        public AuthenticatedUser GetUser(string username)
        {
            return new AuthenticatedUser
            {
                Username = username,
                Role = "Admin",
                Permissions = new List<string> { "READ_DATA", "WRITE_DATA", "DELETE_DATA" }
            };
        }

        public bool HasPermission(string username, string permission)
        {
            var user = GetUser(username);
            return user.Permissions.Contains(permission);
        }

        public void Logout(string username)
        {
            Console.WriteLine($"  LDAP: Logged out {username}");
        }
    }

    public class OAuthProvider : IAuthProvider
    {
        public bool Authenticate(Credentials credentials)
        {
            Console.WriteLine($"  OAuth: Validating token for {credentials.Username}");
            return !string.IsNullOrEmpty(credentials.Token);
        }

        public AuthenticatedUser GetUser(string username)
        {
            return new AuthenticatedUser
            {
                Username = username,
                Role = "Guest",
                Permissions = new List<string> { "READ_DATA" }
            };
        }

        public bool HasPermission(string username, string permission)
        {
            var user = GetUser(username);
            return user.Permissions.Contains(permission);
        }

        public void Logout(string username)
        {
            Console.WriteLine($"  OAuth: Revoked token for {username}");
        }
    }

    // Abstraction
    public class AuthenticationManager
    {
        public IAuthProvider AuthProvider { get; private set; }
        private string? _currentUser;

        public AuthenticationManager(IAuthProvider authProvider)
        {
            AuthProvider = authProvider;
        }

        public bool Login(Credentials credentials)
        {
            if (AuthProvider.Authenticate(credentials))
            {
                _currentUser = credentials.Username;
                return true;
            }
            return false;
        }

        public AuthenticatedUser GetCurrentUser()
        {
            if (_currentUser == null)
                throw new InvalidOperationException("No user logged in");
            
            return AuthProvider.GetUser(_currentUser);
        }

        public bool HasPermission(string permission)
        {
            if (_currentUser == null)
                return false;
            
            return AuthProvider.HasPermission(_currentUser, permission);
        }

        public void Logout()
        {
            if (_currentUser != null)
            {
                AuthProvider.Logout(_currentUser);
                _currentUser = null;
            }
        }
    }

    #endregion

    #region Example 7: File Storage Classes

    // Implementation interface
    public interface IFileStorage
    {
        Task SaveAsync(string fileName, string content);
        Task<string> ReadAsync(string fileName);
        Task DeleteAsync(string fileName);
        Task<bool> ExistsAsync(string fileName);
    }

    // Concrete implementations
    public class LocalFileStorage : IFileStorage
    {
        public async Task SaveAsync(string fileName, string content)
        {
            await Task.Delay(50);
            Console.WriteLine($"  Local: Saved {fileName} to local disk");
        }

        public async Task<string> ReadAsync(string fileName)
        {
            await Task.Delay(30);
            Console.WriteLine($"  Local: Read {fileName} from local disk");
            return $"Local content of {fileName}";
        }

        public async Task DeleteAsync(string fileName)
        {
            await Task.Delay(20);
            Console.WriteLine($"  Local: Deleted {fileName} from local disk");
        }

        public async Task<bool> ExistsAsync(string fileName)
        {
            await Task.Delay(10);
            return true; // Simulate file exists
        }
    }

    public class CloudFileStorage : IFileStorage
    {
        public async Task SaveAsync(string fileName, string content)
        {
            await Task.Delay(200);
            Console.WriteLine($"  Cloud: Uploaded {fileName} to cloud storage");
        }

        public async Task<string> ReadAsync(string fileName)
        {
            await Task.Delay(150);
            Console.WriteLine($"  Cloud: Downloaded {fileName} from cloud storage");
            return $"Cloud content of {fileName}";
        }

        public async Task DeleteAsync(string fileName)
        {
            await Task.Delay(100);
            Console.WriteLine($"  Cloud: Deleted {fileName} from cloud storage");
        }

        public async Task<bool> ExistsAsync(string fileName)
        {
            await Task.Delay(80);
            return true;
        }
    }

    public class NetworkFileStorage : IFileStorage
    {
        public async Task SaveAsync(string fileName, string content)
        {
            await Task.Delay(120);
            Console.WriteLine($"  Network: Saved {fileName} to network share");
        }

        public async Task<string> ReadAsync(string fileName)
        {
            await Task.Delay(90);
            Console.WriteLine($"  Network: Read {fileName} from network share");
            return $"Network content of {fileName}";
        }

        public async Task DeleteAsync(string fileName)
        {
            await Task.Delay(60);
            Console.WriteLine($"  Network: Deleted {fileName} from network share");
        }

        public async Task<bool> ExistsAsync(string fileName)
        {
            await Task.Delay(40);
            return true;
        }
    }

    // Abstraction
    public class FileManager
    {
        public IFileStorage Storage { get; private set; }

        public FileManager(IFileStorage storage)
        {
            Storage = storage;
        }

        public async Task SaveFileAsync(string fileName, string content)
        {
            await Storage.SaveAsync(fileName, content);
        }

        public async Task<string> ReadFileAsync(string fileName)
        {
            return await Storage.ReadAsync(fileName);
        }

        public async Task DeleteFileAsync(string fileName)
        {
            await Storage.DeleteAsync(fileName);
        }

        public async Task<bool> FileExistsAsync(string fileName)
        {
            return await Storage.ExistsAsync(fileName);
        }
    }

    #endregion

    #region Example 8: Compression Algorithm Classes

    // Implementation interface
    public interface ICompressionAlgorithm
    {
        byte[] Compress(string data);
        string Decompress(byte[] compressedData);
        string GetAlgorithmName();
    }

    // Concrete implementations
    public class GzipCompression : ICompressionAlgorithm
    {
        public byte[] Compress(string data)
        {
            Console.WriteLine("  Gzip: Compressing data");
            // Simulate compression
            var bytes = Encoding.UTF8.GetBytes(data);
            var compressed = new byte[bytes.Length / 2]; // Simulate 50% compression
            return compressed;
        }

        public string Decompress(byte[] compressedData)
        {
            Console.WriteLine("  Gzip: Decompressing data");
            return "Decompressed data from Gzip";
        }

        public string GetAlgorithmName() => "GZIP";
    }

    public class ZipCompression : ICompressionAlgorithm
    {
        public byte[] Compress(string data)
        {
            Console.WriteLine("  Zip: Compressing data");
            var bytes = Encoding.UTF8.GetBytes(data);
            var compressed = new byte[bytes.Length * 3 / 5]; // Simulate 40% compression
            return compressed;
        }

        public string Decompress(byte[] compressedData)
        {
            Console.WriteLine("  Zip: Decompressing data");
            return "Decompressed data from Zip";
        }

        public string GetAlgorithmName() => "ZIP";
    }

    public class LzmaCompression : ICompressionAlgorithm
    {
        public byte[] Compress(string data)
        {
            Console.WriteLine("  LZMA: Compressing data");
            var bytes = Encoding.UTF8.GetBytes(data);
            var compressed = new byte[bytes.Length / 3]; // Simulate 67% compression
            return compressed;
        }

        public string Decompress(byte[] compressedData)
        {
            Console.WriteLine("  LZMA: Decompressing data");
            return "Decompressed data from LZMA";
        }

        public string GetAlgorithmName() => "LZMA";
    }

    // Abstraction
    public class DataCompressor
    {
        public ICompressionAlgorithm CompressionAlgorithm { get; private set; }

        public DataCompressor(ICompressionAlgorithm compressionAlgorithm)
        {
            CompressionAlgorithm = compressionAlgorithm;
        }

        public byte[] Compress(string data)
        {
            Console.WriteLine($"Using {CompressionAlgorithm.GetAlgorithmName()} algorithm");
            return CompressionAlgorithm.Compress(data);
        }

        public string Decompress(byte[] compressedData)
        {
            return CompressionAlgorithm.Decompress(compressedData);
        }

        public void SwitchAlgorithm(ICompressionAlgorithm newAlgorithm)
        {
            CompressionAlgorithm = newAlgorithm;
            Console.WriteLine($"Switched to {newAlgorithm.GetAlgorithmName()} algorithm");
        }
    }

    #endregion

    #region Example 9: Cross-Platform UI Classes

    // Implementation interface
    public interface IPlatformUI
    {
        void CreateWindow(string title, int width, int height);
        void ShowWindow();
        void HideWindow();
        void SetWindowTitle(string title);
        void ResizeWindow(int width, int height);
        void CreateButton(string text, int width, int height);
        void RenderButton();
        void SetButtonEnabled(bool enabled);
    }

    // Concrete implementations
    public class WindowsUI : IPlatformUI
    {
        public void CreateWindow(string title, int width, int height)
        {
            Console.WriteLine($"  Windows: Creating window '{title}' ({width}x{height})");
        }

        public void ShowWindow()
        {
            Console.WriteLine("  Windows: ShowWindow() called");
        }

        public void HideWindow()
        {
            Console.WriteLine("  Windows: HideWindow() called");
        }

        public void SetWindowTitle(string title)
        {
            Console.WriteLine($"  Windows: SetWindowText('{title}')");
        }

        public void ResizeWindow(int width, int height)
        {
            Console.WriteLine($"  Windows: SetWindowPos({width}x{height})");
        }

        public void CreateButton(string text, int width, int height)
        {
            Console.WriteLine($"  Windows: CreateWindow('BUTTON', '{text}', {width}x{height})");
        }

        public void RenderButton()
        {
            Console.WriteLine("  Windows: InvalidateRect() - button rendered");
        }

        public void SetButtonEnabled(bool enabled)
        {
            Console.WriteLine($"  Windows: EnableWindow({enabled})");
        }
    }

    public class MacOSUI : IPlatformUI
    {
        public void CreateWindow(string title, int width, int height)
        {
            Console.WriteLine($"  macOS: NSWindow initWithContentRect:'{title}' ({width}x{height})");
        }

        public void ShowWindow()
        {
            Console.WriteLine("  macOS: [window makeKeyAndOrderFront:]");
        }

        public void HideWindow()
        {
            Console.WriteLine("  macOS: [window orderOut:]");
        }

        public void SetWindowTitle(string title)
        {
            Console.WriteLine($"  macOS: [window setTitle:@'{title}']");
        }

        public void ResizeWindow(int width, int height)
        {
            Console.WriteLine($"  macOS: [window setFrame:NSMakeRect(..., {width}, {height})]");
        }

        public void CreateButton(string text, int width, int height)
        {
            Console.WriteLine($"  macOS: NSButton buttonWithTitle:@'{text}' frame:({width}x{height})");
        }

        public void RenderButton()
        {
            Console.WriteLine("  macOS: [button setNeedsDisplay:YES]");
        }

        public void SetButtonEnabled(bool enabled)
        {
            Console.WriteLine($"  macOS: [button setEnabled:{(enabled ? "YES" : "NO")}]");
        }
    }

    public class LinuxUI : IPlatformUI
    {
        public void CreateWindow(string title, int width, int height)
        {
            Console.WriteLine($"  Linux: gtk_window_new() '{title}' ({width}x{height})");
        }

        public void ShowWindow()
        {
            Console.WriteLine("  Linux: gtk_widget_show()");
        }

        public void HideWindow()
        {
            Console.WriteLine("  Linux: gtk_widget_hide()");
        }

        public void SetWindowTitle(string title)
        {
            Console.WriteLine($"  Linux: gtk_window_set_title('{title}')");
        }

        public void ResizeWindow(int width, int height)
        {
            Console.WriteLine($"  Linux: gtk_window_resize({width}, {height})");
        }

        public void CreateButton(string text, int width, int height)
        {
            Console.WriteLine($"  Linux: gtk_button_new_with_label('{text}') size:({width}x{height})");
        }

        public void RenderButton()
        {
            Console.WriteLine("  Linux: gtk_widget_queue_draw()");
        }

        public void SetButtonEnabled(bool enabled)
        {
            Console.WriteLine($"  Linux: gtk_widget_set_sensitive({(enabled ? "TRUE" : "FALSE")})");
        }
    }

    // Abstraction
    public abstract class UIElement
    {
        protected IPlatformUI platform;

        protected UIElement(IPlatformUI platform)
        {
            this.platform = platform;
        }
    }

    // Refined abstractions
    public class Window : UIElement
    {
        private string title;
        private int width, height;

        public Window(IPlatformUI platform, string title, int width, int height) : base(platform)
        {
            this.title = title;
            this.width = width;
            this.height = height;
            platform.CreateWindow(title, width, height);
        }

        public void Show()
        {
            platform.ShowWindow();
        }

        public void Hide()
        {
            platform.HideWindow();
        }

        public void SetTitle(string newTitle)
        {
            title = newTitle;
            platform.SetWindowTitle(newTitle);
        }

        public void Resize(int newWidth, int newHeight)
        {
            width = newWidth;
            height = newHeight;
            platform.ResizeWindow(newWidth, newHeight);
        }
    }

    public class Button : UIElement
    {
        private string text;
        private int width, height;
        private bool enabled = true;

        public Button(IPlatformUI platform, string text, int width, int height) : base(platform)
        {
            this.text = text;
            this.width = width;
            this.height = height;
            platform.CreateButton(text, width, height);
        }

        public void Render()
        {
            platform.RenderButton();
        }

        public void SetEnabled(bool enabled)
        {
            this.enabled = enabled;
            platform.SetButtonEnabled(enabled);
        }
    }

    #endregion
}

/*
 * MEMORY ALLOCATION CONSIDERATIONS:
 * =================================
 * 
 * 1. BRIDGE OVERHEAD:
 *    - Bridge pattern adds one level of indirection with minimal memory overhead
 *    - Each abstraction holds a reference to an implementation interface
 *    - Implementation objects are typically allocated once and reused
 *    - Multiple abstractions can share the same implementation instance
 * 
 * 2. OBJECT COMPOSITION:
 *    - Abstractions compose with implementations rather than inheriting
 *    - Allows for dynamic switching of implementations at runtime
 *    - Implementation objects can be pooled or cached for efficiency
 *    - Consider using dependency injection for implementation lifecycle management
 * 
 * 3. IMPLEMENTATION SHARING:
 *    - Multiple abstraction instances can share single implementation instances
 *    - Stateless implementations are ideal for sharing across abstractions
 *    - Stateful implementations may require per-abstraction instances
 *    - Consider using factory patterns for implementation creation
 * 
 * 4. MEMORY OPTIMIZATION:
 *    - Lazy initialization of expensive implementation objects
 *    - Implementation caching for frequently used configurations
 *    - Consider weak references for non-critical implementation caches
 *    - Dispose pattern for implementations that manage unmanaged resources
 * 
 * MULTITHREAD CONSIDERATIONS:
 * ===========================
 * 
 * 1. IMPLEMENTATION THREAD SAFETY:
 *    - Bridge abstractions delegate thread safety to implementations
 *    - Thread safety depends on the specific implementation used
 *    - Stateless implementations are inherently thread-safe
 *    - Stateful implementations require careful synchronization design
 * 
 * 2. SHARED IMPLEMENTATION ACCESS:
 *    - Multiple abstraction instances may share implementation objects
 *    - Concurrent access to shared implementations requires thread safety
 *    - Consider using ThreadLocal storage for per-thread implementations
 *    - Immutable implementations eliminate thread safety concerns
 * 
 * 3. DYNAMIC IMPLEMENTATION SWITCHING:
 *    - Runtime switching of implementations requires careful synchronization
 *    - Use atomic references or locks when changing implementations
 *    - Consider the impact of switching implementations during ongoing operations
 *    - Ensure proper cleanup of previous implementations after switching
 * 
 * 4. ASYNC OPERATIONS:
 *    - Bridge pattern works well with async/await patterns
 *    - Implementations can provide async methods for I/O operations
 *    - Consider cancellation token support in implementation interfaces
 *    - Async implementations should handle concurrent operations properly
 * 
 * 5. PERFORMANCE CONSIDERATIONS:
 *    - Bridge adds minimal performance overhead (one virtual method call)
 *    - Implementation method calls are typically not inlined
 *    - Benefits usually outweigh the small performance cost
 *    - Profile implementation switching costs if done frequently
 * 
 * 6. BEST PRACTICES:
 *    - Design implementation interfaces to be stateless when possible
 *    - Use dependency injection containers for implementation management
 *    - Implement proper error handling and logging in implementations
 *    - Consider using factory methods for creating appropriate implementations
 *    - Document thread safety guarantees of each implementation
 *    - Use configuration-based implementation selection when appropriate
 *    - Implement proper resource cleanup in implementation Dispose methods
 *    - Consider using adapter patterns when integrating legacy implementations
 */
