using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Xml;

/*
In this pattern you are not abstracting anything new but rather adapting something existing to fit a new context.
*/

/*
 * ADAPTER DESIGN PATTERN IN C#
 * 
 * PURPOSE:
 * The Adapter pattern allows incompatible interfaces to work together by acting as a bridge
 * between two incompatible interfaces. It converts the interface of a class into another
 * interface that clients expect, enabling classes to work together that couldn't otherwise
 * due to incompatible interfaces.
 * 
 * CORE BENEFITS:
 * - Enables integration of existing classes with incompatible interfaces
 * - Promotes code reuse by making existing functionality work with new systems
 * - Provides a clean separation between business logic and interface conversion
 * - Allows gradual migration from legacy systems to new architectures
 * - Enables use of third-party libraries without modifying their source code
 * - Supports both object adapter (composition) and class adapter (inheritance) approaches
 * 
 * SCENARIOS TO USE:
 * - Integrating third-party libraries with incompatible interfaces
 * - Working with legacy systems that can't be modified
 * - Converting data between different formats (JSON to XML, CSV to Objects)
 * - Integrating different database providers with a common interface
 * - Adapting different logging frameworks to a unified logging interface
 * - Converting between different authentication mechanisms
 * - Bridging different payment gateway APIs
 * - Adapting different cloud storage providers (AWS S3, Azure Blob, Google Cloud)
 * - Converting between different messaging systems (RabbitMQ, Azure Service Bus)
 * - Adapting different caching systems (Redis, Memcached, In-Memory)
 * 
 * SCENARIOS NOT TO USE:
 * - When you can modify the target class to implement the desired interface directly
 * - If the interfaces are very similar and only minor changes are needed
 * - When performance is critical and the adapter adds unnecessary overhead
 * - If you're designing new systems from scratch (use proper interfaces from the start)
 * - When the adaptation logic is complex and would be better as a separate service
 * - If the adapter would need to maintain significant state or complex logic
 * - When the cost of creating adapters exceeds the benefit of integration
 * - If there are simpler alternatives like extension methods or wrapper functions
 */

namespace AdapterPattern
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== Adapter Design Pattern Examples ===\n");

            // Example 1: Third-Party Library Integration
            ThirdPartyLibraryExample();

            // Example 2: Data Format Conversion
            DataFormatConversionExample();

            // Example 3: Database Provider Adaptation
            DatabaseProviderExample();

            // Example 4: Legacy System Integration
            LegacySystemExample();

            // Example 5: Thread-Safe Adapter Pattern
            await ThreadSafeAdapterExample();

            // Example 6: Cloud Storage Adapters
            await CloudStorageExample();

            // Example 7: Payment Gateway Adapters
            PaymentGatewayExample();

            // Example 8: Logging Framework Adapters
            LoggingFrameworkExample();

            // Example 9: Caching System Adapters
            CachingSystemExample();

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        #region Example 1: Third-Party Library Integration

        static void ThirdPartyLibraryExample()
        {
            Console.WriteLine("1. Third-Party Library Integration:");
            Console.WriteLine("====================================");

            // We have a modern media player interface but need to use legacy audio/video libraries
            IMediaPlayer mediaPlayer = new AudioPlayerAdapter(new Mp3Player());
            mediaPlayer.Play("song.mp3");

            mediaPlayer = new VideoPlayerAdapter(new Mp4Player());
            mediaPlayer.Play("movie.mp4");

            // Advanced media player that handles multiple formats
            var advancedPlayer = new AdvancedMediaPlayer();
            advancedPlayer.Play("song.mp3");
            advancedPlayer.Play("movie.mp4");
            advancedPlayer.Play("audio.vlc");

            Console.WriteLine();
        }

        #endregion

        #region Example 2: Data Format Conversion

        static void DataFormatConversionExample()
        {
            Console.WriteLine("2. Data Format Conversion:");
            Console.WriteLine("===========================");

            var person = new Person { Name = "John Doe", Age = 30, Email = "john@example.com" };

            // Convert to different formats using adapters
            IDataFormatter formatter = new JsonFormatterAdapter(new JsonDataSerializer());
            var jsonData = formatter.Format(person);
            Console.WriteLine($"JSON: {jsonData}");

            formatter = new XmlFormatterAdapter(new XmlDataSerializer());
            var xmlData = formatter.Format(person);
            Console.WriteLine($"XML: {xmlData}");

            formatter = new CsvFormatterAdapter(new CsvDataSerializer());
            var csvData = formatter.Format(person);
            Console.WriteLine($"CSV: {csvData}");

            // Parse data back
            IDataParser parser = new JsonParserAdapter(new JsonDataDeserializer());
            var parsedPerson = parser.Parse<Person>(jsonData);
            Console.WriteLine($"Parsed: {parsedPerson.Name}, {parsedPerson.Age}, {parsedPerson.Email}");

            Console.WriteLine();
        }

        #endregion

        #region Example 3: Database Provider Adaptation

        static void DatabaseProviderExample()
        {
            Console.WriteLine("3. Database Provider Adaptation:");
            Console.WriteLine("=================================");

            // Different database providers adapted to common interface
            var databases = new IDataRepository[]
            {
                new SqlServerAdapter(new SqlServerProvider()),
                new MySqlAdapter(new MySqlProvider()),
                new MongoDbAdapter(new MongoDbProvider())
            };

            var user = new User { Id = 1, Name = "Alice", Email = "alice@example.com" };

            foreach (var db in databases)
            {
                Console.WriteLine($"\nUsing {db.GetType().Name}:");
                db.Save(user);
                var retrieved = db.GetById<User>(1);
                Console.WriteLine($"Retrieved: {retrieved?.Name}");
            }

            Console.WriteLine();
        }

        #endregion

        #region Example 4: Legacy System Integration

        static void LegacySystemExample()
        {
            Console.WriteLine("4. Legacy System Integration:");
            Console.WriteLine("==============================");

            // Modern order processing system
            var orderProcessor = new ModernOrderProcessor();

            // Legacy payment system (can't be modified)
            var legacyPayment = new LegacyPaymentSystem();
            var paymentAdapter = new LegacyPaymentAdapter(legacyPayment);

            // Legacy inventory system
            var legacyInventory = new LegacyInventorySystem();
            var inventoryAdapter = new LegacyInventoryAdapter(legacyInventory);

            // Process order using adapters
            var order = new Order
            {
                Id = "ORD-001",
                ProductId = "PROD-123",
                Quantity = 2,
                Amount = 99.99m
            };

            Console.WriteLine($"Processing order {order.Id}:");
            var success = orderProcessor.ProcessOrder(order, paymentAdapter, inventoryAdapter);
            Console.WriteLine($"Order processing: {(success ? "Success" : "Failed")}");

            Console.WriteLine();
        }

        #endregion

        #region Example 5: Thread-Safe Adapter Pattern

        static async Task ThreadSafeAdapterExample()
        {
            Console.WriteLine("5. Thread-Safe Adapter Pattern:");
            Console.WriteLine("================================");

            var legacyCounter = new LegacyCounter();
            var threadSafeAdapter = new ThreadSafeCounterAdapter(legacyCounter);

            var tasks = new List<Task>();

            // Create multiple tasks that increment the counter concurrently
            for (int i = 1; i <= 5; i++)
            {
                int taskId = i;
                tasks.Add(Task.Run(async () =>
                {
                    for (int j = 0; j < 10; j++)
                    {
                        threadSafeAdapter.Increment();
                        Console.WriteLine($"[Task {taskId}] Counter: {threadSafeAdapter.GetValue()}");
                        await Task.Delay(50);
                    }
                }));
            }

            await Task.WhenAll(tasks);
            Console.WriteLine($"Final counter value: {threadSafeAdapter.GetValue()}");

            Console.WriteLine();
        }

        #endregion

        #region Example 6: Cloud Storage Adapters

        static async Task CloudStorageExample()
        {
            Console.WriteLine("6. Cloud Storage Adapters:");
            Console.WriteLine("===========================");

            var storageProviders = new ICloudStorage[]
            {
                new AwsS3Adapter(new AwsS3Service()),
                new AzureBlobAdapter(new AzureBlobService()),
                new GoogleCloudAdapter(new GoogleCloudService())
            };

            var fileData = Encoding.UTF8.GetBytes("This is sample file content for cloud storage testing.");

            foreach (var storage in storageProviders)
            {
                Console.WriteLine($"\n{storage.GetType().Name}:");

                try
                {
                    var uploadResult = await storage.UploadAsync("test-file.txt", fileData);
                    Console.WriteLine($"Upload: {uploadResult}");

                    var downloadedData = await storage.DownloadAsync("test-file.txt");
                    Console.WriteLine($"Downloaded: {Encoding.UTF8.GetString(downloadedData)}");

                    var deleteResult = await storage.DeleteAsync("test-file.txt");
                    Console.WriteLine($"Delete: {deleteResult}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            Console.WriteLine();
        }

        #endregion

        #region Example 7: Payment Gateway Adapters

        static void PaymentGatewayExample()
        {
            Console.WriteLine("7. Payment Gateway Adapters:");
            Console.WriteLine("=============================");

            var gateways = new IPaymentGateway[]
            {
                new StripeAdapter(new StripeService()),
                new PayPalAdapter(new PayPalService()),
                new SquareAdapter(new SquareService())
            };

            var payment = new PaymentRequest
            {
                Amount = 100.00m,
                Currency = "USD",
                PaymentMethod = "credit_card",
                CustomerEmail = "customer@example.com"
            };

            foreach (var gateway in gateways)
            {
                Console.WriteLine($"\n{gateway.GetType().Name}:");
                var result = gateway.ProcessPayment(payment);
                Console.WriteLine($"Result: {result.Status} - {result.Message}");
                if (result.IsSuccess)
                {
                    Console.WriteLine($"Transaction ID: {result.TransactionId}");
                }
            }

            Console.WriteLine();
        }

        #endregion

        #region Example 8: Logging Framework Adapters

        static void LoggingFrameworkExample()
        {
            Console.WriteLine("8. Logging Framework Adapters:");
            Console.WriteLine("===============================");

            var loggers = new ILogger[]
            {
                new NLogAdapter(new NLogLogger()),
                new SerilogAdapter(new SerilogLogger()),
                new Log4NetAdapter(new Log4NetLogger())
            };

            foreach (var logger in loggers)
            {
                Console.WriteLine($"\n{logger.GetType().Name}:");
                logger.LogInfo("This is an info message");
                logger.LogWarning("This is a warning message");
                logger.LogError("This is an error message", new Exception("Sample exception"));
            }

            Console.WriteLine();
        }

        #endregion

        #region Example 9: Caching System Adapters

        static void CachingSystemExample()
        {
            Console.WriteLine("9. Caching System Adapters:");
            Console.WriteLine("============================");

            var caches = new ICacheProvider[]
            {
                new RedisCacheAdapter(new RedisCache()),
                new MemcachedAdapter(new MemcachedCache()),
                new InMemoryCacheAdapter(new InMemoryCache())
            };

            foreach (var cache in caches)
            {
                Console.WriteLine($"\n{cache.GetType().Name}:");

                // Set cache value
                cache.Set("user:123", "John Doe", TimeSpan.FromMinutes(5));
                Console.WriteLine("Set cache value: user:123 = John Doe");

                // Get cache value
                var value = cache.Get<string>("user:123");
                Console.WriteLine($"Retrieved value: {value}");

                // Check if exists
                var exists = cache.Exists("user:123");
                Console.WriteLine($"Key exists: {exists}");

                // Remove from cache
                cache.Remove("user:123");
                Console.WriteLine("Removed from cache");

                // Try to get removed value
                var removedValue = cache.Get<string>("user:123");
                Console.WriteLine($"After removal: {removedValue ?? "null"}");
            }

            Console.WriteLine();
        }

        #endregion
    }

    #region Core Adapter Pattern Classes

    // Target interface that client expects
    public interface ITarget
    {
        string Request();
    }

    // Adaptee - existing class with incompatible interface
    public class Adaptee
    {
        public string SpecificRequest()
        {
            return "Special request from Adaptee";
        }
    }

    // Object Adapter - uses composition
    public class ObjectAdapter : ITarget
    {
        private readonly Adaptee _adaptee;

        public ObjectAdapter(Adaptee adaptee)
        {
            _adaptee = adaptee;
        }

        public string Request()
        {
            return _adaptee.SpecificRequest();
        }
    }

    // Class Adapter - uses inheritance (when possible)
    public class ClassAdapter : Adaptee, ITarget
    {
        public string Request()
        {
            return SpecificRequest();
        }
    }

    #endregion

    #region Example 1: Media Player Classes

    // Target interface
    public interface IMediaPlayer
    {
        void Play(string filename);
    }

    // Legacy audio player (Adaptee)
    public class Mp3Player
    {
        public void PlayMp3(string filename)
        {
            Console.WriteLine($"  Playing MP3 file: {filename}");
        }
    }

    // Legacy video player (Adaptee)
    public class Mp4Player
    {
        public void PlayMp4(string filename)
        {
            Console.WriteLine($"  Playing MP4 file: {filename}");
        }
    }

    // Another legacy player
    public class VlcPlayer
    {
        public void PlayVlc(string filename)
        {
            Console.WriteLine($"  Playing VLC file: {filename}");
        }
    }

    // Adapters
    public class AudioPlayerAdapter : IMediaPlayer
    {
        private readonly Mp3Player _mp3Player;

        public AudioPlayerAdapter(Mp3Player mp3Player)
        {
            _mp3Player = mp3Player;
        }

        public void Play(string filename)
        {
            if (filename.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase))
            {
                _mp3Player.PlayMp3(filename);
            }
            else
            {
                Console.WriteLine($"  Unsupported audio format: {filename}");
            }
        }
    }

    public class VideoPlayerAdapter : IMediaPlayer
    {
        private readonly Mp4Player _mp4Player;

        public VideoPlayerAdapter(Mp4Player mp4Player)
        {
            _mp4Player = mp4Player;
        }

        public void Play(string filename)
        {
            if (filename.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase))
            {
                _mp4Player.PlayMp4(filename);
            }
            else
            {
                Console.WriteLine($"  Unsupported video format: {filename}");
            }
        }
    }

    // Advanced player that uses multiple adapters
    public class AdvancedMediaPlayer : IMediaPlayer
    {
        private readonly Dictionary<string, IMediaPlayer> _adapters;

        public AdvancedMediaPlayer()
        {
            _adapters = new Dictionary<string, IMediaPlayer>
            {
                [".mp3"] = new AudioPlayerAdapter(new Mp3Player()),
                [".mp4"] = new VideoPlayerAdapter(new Mp4Player()),
                [".vlc"] = new VlcPlayerAdapter(new VlcPlayer())
            };
        }

        public void Play(string filename)
        {
            var extension = Path.GetExtension(filename).ToLower();
            if (_adapters.TryGetValue(extension, out var adapter))
            {
                adapter.Play(filename);
            }
            else
            {
                Console.WriteLine($"  Unsupported format: {filename}");
            }
        }
    }

    public class VlcPlayerAdapter : IMediaPlayer
    {
        private readonly VlcPlayer _vlcPlayer;

        public VlcPlayerAdapter(VlcPlayer vlcPlayer)
        {
            _vlcPlayer = vlcPlayer;
        }

        public void Play(string filename)
        {
            if (filename.EndsWith(".vlc", StringComparison.OrdinalIgnoreCase))
            {
                _vlcPlayer.PlayVlc(filename);
            }
            else
            {
                Console.WriteLine($"  VLC player cannot play: {filename}");
            }
        }
    }

    #endregion

    #region Example 2: Data Format Conversion Classes

    public class Person
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Email { get; set; } = string.Empty;
    }

    // Target interfaces
    public interface IDataFormatter
    {
        string Format(object data);
    }

    public interface IDataParser
    {
        T Parse<T>(string data);
    }

    // Legacy serializers (Adaptees)
    public class JsonDataSerializer
    {
        public string SerializeToJson(object obj)
        {
            return System.Text.Json.JsonSerializer.Serialize(obj);
        }
    }

    public class JsonDataDeserializer
    {
        public T DeserializeFromJson<T>(string json)
        {
            return System.Text.Json.JsonSerializer.Deserialize<T>(json) ?? throw new InvalidOperationException("Deserialization failed");
        }
    }

    public class XmlDataSerializer
    {
        public string SerializeToXml(object obj)
        {
            // Simplified XML serialization
            if (obj is Person person)
            {
                return $"<Person><Name>{person.Name}</Name><Age>{person.Age}</Age><Email>{person.Email}</Email></Person>";
            }
            return "<Unknown/>";
        }
    }

    public class CsvDataSerializer
    {
        public string SerializeToCsv(object obj)
        {
            if (obj is Person person)
            {
                return $"{person.Name},{person.Age},{person.Email}";
            }
            return string.Empty;
        }
    }

    // Adapters
    public class JsonFormatterAdapter : IDataFormatter
    {
        private readonly JsonDataSerializer _serializer;

        public JsonFormatterAdapter(JsonDataSerializer serializer)
        {
            _serializer = serializer;
        }

        public string Format(object data)
        {
            return _serializer.SerializeToJson(data);
        }
    }

    public class JsonParserAdapter : IDataParser
    {
        private readonly JsonDataDeserializer _deserializer;

        public JsonParserAdapter(JsonDataDeserializer deserializer)
        {
            _deserializer = deserializer;
        }

        public T Parse<T>(string data)
        {
            return _deserializer.DeserializeFromJson<T>(data);
        }
    }

    public class XmlFormatterAdapter : IDataFormatter
    {
        private readonly XmlDataSerializer _serializer;

        public XmlFormatterAdapter(XmlDataSerializer serializer)
        {
            _serializer = serializer;
        }

        public string Format(object data)
        {
            return _serializer.SerializeToXml(data);
        }
    }

    public class CsvFormatterAdapter : IDataFormatter
    {
        private readonly CsvDataSerializer _serializer;

        public CsvFormatterAdapter(CsvDataSerializer serializer)
        {
            _serializer = serializer;
        }

        public string Format(object data)
        {
            return _serializer.SerializeToCsv(data);
        }
    }

    #endregion

    #region Example 3: Database Provider Classes

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    // Target interface
    public interface IDataRepository
    {
        void Save<T>(T entity);
        T? GetById<T>(int id);
        void Delete<T>(int id);
    }

    // Legacy database providers (Adaptees)
    public class SqlServerProvider
    {
        public void Insert(string table, object data)
        {
            Console.WriteLine($"  SQL Server: INSERT INTO {table}");
        }

        public object Select(string table, int id)
        {
            Console.WriteLine($"  SQL Server: SELECT FROM {table} WHERE Id = {id}");
            return new User { Id = id, Name = "User from SQL Server", Email = "sql@example.com" };
        }
    }

    public class MySqlProvider
    {
        public void AddRecord(object record)
        {
            Console.WriteLine("  MySQL: Adding record");
        }

        public object FindRecord(int id)
        {
            Console.WriteLine($"  MySQL: Finding record with ID {id}");
            return new User { Id = id, Name = "User from MySQL", Email = "mysql@example.com" };
        }
    }

    public class MongoDbProvider
    {
        public void InsertDocument(object document)
        {
            Console.WriteLine("  MongoDB: Inserting document");
        }

        public object GetDocument(int id)
        {
            Console.WriteLine($"  MongoDB: Getting document with ID {id}");
            return new User { Id = id, Name = "User from MongoDB", Email = "mongo@example.com" };
        }
    }

    // Adapters
    public class SqlServerAdapter : IDataRepository
    {
        private readonly SqlServerProvider _provider;

        public SqlServerAdapter(SqlServerProvider provider)
        {
            _provider = provider;
        }

        public void Save<T>(T entity)
        {
            _provider.Insert(typeof(T).Name, entity!);
        }

        public T? GetById<T>(int id)
        {
            var result = _provider.Select(typeof(T).Name, id);
            return (T?)result;
        }

        public void Delete<T>(int id)
        {
            Console.WriteLine($"  SQL Server: DELETE FROM {typeof(T).Name} WHERE Id = {id}");
        }
    }

    public class MySqlAdapter : IDataRepository
    {
        private readonly MySqlProvider _provider;

        public MySqlAdapter(MySqlProvider provider)
        {
            _provider = provider;
        }

        public void Save<T>(T entity)
        {
            _provider.AddRecord(entity!);
        }

        public T? GetById<T>(int id)
        {
            var result = _provider.FindRecord(id);
            return (T?)result;
        }

        public void Delete<T>(int id)
        {
            Console.WriteLine($"  MySQL: Deleting record with ID {id}");
        }
    }

    public class MongoDbAdapter : IDataRepository
    {
        private readonly MongoDbProvider _provider;

        public MongoDbAdapter(MongoDbProvider provider)
        {
            _provider = provider;
        }

        public void Save<T>(T entity)
        {
            _provider.InsertDocument(entity!);
        }

        public T? GetById<T>(int id)
        {
            var result = _provider.GetDocument(id);
            return (T?)result;
        }

        public void Delete<T>(int id)
        {
            Console.WriteLine($"  MongoDB: Deleting document with ID {id}");
        }
    }

    #endregion

    #region Example 4: Legacy System Integration Classes

    public class Order
    {
        public string Id { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Amount { get; set; }
    }

    // Modern interfaces
    public interface IPaymentProcessor
    {
        bool ProcessPayment(decimal amount, string orderId);
    }

    public interface IInventoryManager
    {
        bool CheckAvailability(string productId, int quantity);
        void ReserveItems(string productId, int quantity);
    }

    // Legacy systems (cannot be modified)
    public class LegacyPaymentSystem
    {
        public int MakePayment(double dollarAmount, string transactionId)
        {
            Console.WriteLine($"  Legacy Payment: Processing ${dollarAmount} for transaction {transactionId}");
            return dollarAmount > 0 ? 1 : 0; // 1 = success, 0 = failure
        }
    }

    public class LegacyInventorySystem
    {
        public bool IsItemAvailable(string item, int count)
        {
            Console.WriteLine($"  Legacy Inventory: Checking {count} units of {item}");
            return true; // Assume items are available
        }

        public void HoldItems(string item, int count)
        {
            Console.WriteLine($"  Legacy Inventory: Holding {count} units of {item}");
        }
    }

    // Adapters
    public class LegacyPaymentAdapter : IPaymentProcessor
    {
        private readonly LegacyPaymentSystem _legacySystem;

        public LegacyPaymentAdapter(LegacyPaymentSystem legacySystem)
        {
            _legacySystem = legacySystem;
        }

        public bool ProcessPayment(decimal amount, string orderId)
        {
            var result = _legacySystem.MakePayment((double)amount, orderId);
            return result == 1;
        }
    }

    public class LegacyInventoryAdapter : IInventoryManager
    {
        private readonly LegacyInventorySystem _legacySystem;

        public LegacyInventoryAdapter(LegacyInventorySystem legacySystem)
        {
            _legacySystem = legacySystem;
        }

        public bool CheckAvailability(string productId, int quantity)
        {
            return _legacySystem.IsItemAvailable(productId, quantity);
        }

        public void ReserveItems(string productId, int quantity)
        {
            _legacySystem.HoldItems(productId, quantity);
        }
    }

    // Modern order processor
    public class ModernOrderProcessor
    {
        public bool ProcessOrder(Order order, IPaymentProcessor paymentProcessor, IInventoryManager inventoryManager)
        {
            try
            {
                // Check inventory
                if (!inventoryManager.CheckAvailability(order.ProductId, order.Quantity))
                {
                    Console.WriteLine("  Insufficient inventory");
                    return false;
                }

                // Reserve items
                inventoryManager.ReserveItems(order.ProductId, order.Quantity);

                // Process payment
                if (!paymentProcessor.ProcessPayment(order.Amount, order.Id))
                {
                    Console.WriteLine("  Payment failed");
                    return false;
                }

                Console.WriteLine("  Order processed successfully");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  Order processing failed: {ex.Message}");
                return false;
            }
        }
    }

    #endregion

    #region Example 5: Thread-Safe Adapter Classes

    // MULTITHREAD ASPECTS: Non-thread-safe legacy counter
    public class LegacyCounter
    {
        private int _count = 0;

        public void IncrementCounter()
        {
            // Simulate non-atomic operation
            var temp = _count;
            Thread.Sleep(1); // Simulate processing time
            _count = temp + 1;
        }

        public int GetCounter()
        {
            return _count;
        }
    }

    // Thread-safe adapter
    public class ThreadSafeCounterAdapter
    {
        private readonly LegacyCounter _legacyCounter;
        private readonly object _lock = new object();

        public ThreadSafeCounterAdapter(LegacyCounter legacyCounter)
        {
            _legacyCounter = legacyCounter;
        }

        public void Increment()
        {
            lock (_lock)
            {
                _legacyCounter.IncrementCounter();
            }
        }

        public int GetValue()
        {
            lock (_lock)
            {
                return _legacyCounter.GetCounter();
            }
        }
    }

    #endregion

    #region Example 6: Cloud Storage Classes

    public interface ICloudStorage
    {
        Task<bool> UploadAsync(string fileName, byte[] data);
        Task<byte[]> DownloadAsync(string fileName);
        Task<bool> DeleteAsync(string fileName);
    }

    // Different cloud services (Adaptees)
    public class AwsS3Service
    {
        public async Task<string> PutObjectAsync(string key, byte[] content)
        {
            await Task.Delay(200); // Simulate network call
            Console.WriteLine($"  AWS S3: Uploaded {key} ({content.Length} bytes)");
            return "aws-upload-id";
        }

        public async Task<byte[]> GetObjectAsync(string key)
        {
            await Task.Delay(150);
            Console.WriteLine($"  AWS S3: Downloaded {key}");
            return Encoding.UTF8.GetBytes("Downloaded from AWS S3");
        }

        public async Task<bool> DeleteObjectAsync(string key)
        {
            await Task.Delay(100);
            Console.WriteLine($"  AWS S3: Deleted {key}");
            return true;
        }
    }

    public class AzureBlobService
    {
        public async Task<bool> UploadBlobAsync(string blobName, byte[] data)
        {
            await Task.Delay(180);
            Console.WriteLine($"  Azure Blob: Uploaded {blobName} ({data.Length} bytes)");
            return true;
        }

        public async Task<byte[]> DownloadBlobAsync(string blobName)
        {
            await Task.Delay(120);
            Console.WriteLine($"  Azure Blob: Downloaded {blobName}");
            return Encoding.UTF8.GetBytes("Downloaded from Azure Blob");
        }

        public async Task<bool> RemoveBlobAsync(string blobName)
        {
            await Task.Delay(90);
            Console.WriteLine($"  Azure Blob: Removed {blobName}");
            return true;
        }
    }

    public class GoogleCloudService
    {
        public async Task<string> StoreFileAsync(string fileName, byte[] fileData)
        {
            await Task.Delay(220);
            Console.WriteLine($"  Google Cloud: Stored {fileName} ({fileData.Length} bytes)");
            return "gcp-file-id";
        }

        public async Task<byte[]> RetrieveFileAsync(string fileName)
        {
            await Task.Delay(140);
            Console.WriteLine($"  Google Cloud: Retrieved {fileName}");
            return Encoding.UTF8.GetBytes("Downloaded from Google Cloud");
        }

        public async Task<bool> EraseFileAsync(string fileName)
        {
            await Task.Delay(110);
            Console.WriteLine($"  Google Cloud: Erased {fileName}");
            return true;
        }
    }

    // Adapters
    public class AwsS3Adapter : ICloudStorage
    {
        private readonly AwsS3Service _service;

        public AwsS3Adapter(AwsS3Service service)
        {
            _service = service;
        }

        public async Task<bool> UploadAsync(string fileName, byte[] data)
        {
            var result = await _service.PutObjectAsync(fileName, data);
            return !string.IsNullOrEmpty(result);
        }

        public async Task<byte[]> DownloadAsync(string fileName)
        {
            return await _service.GetObjectAsync(fileName);
        }

        public async Task<bool> DeleteAsync(string fileName)
        {
            return await _service.DeleteObjectAsync(fileName);
        }
    }

    public class AzureBlobAdapter : ICloudStorage
    {
        private readonly AzureBlobService _service;

        public AzureBlobAdapter(AzureBlobService service)
        {
            _service = service;
        }

        public async Task<bool> UploadAsync(string fileName, byte[] data)
        {
            return await _service.UploadBlobAsync(fileName, data);
        }

        public async Task<byte[]> DownloadAsync(string fileName)
        {
            return await _service.DownloadBlobAsync(fileName);
        }

        public async Task<bool> DeleteAsync(string fileName)
        {
            return await _service.RemoveBlobAsync(fileName);
        }
    }

    public class GoogleCloudAdapter : ICloudStorage
    {
        private readonly GoogleCloudService _service;

        public GoogleCloudAdapter(GoogleCloudService service)
        {
            _service = service;
        }

        public async Task<bool> UploadAsync(string fileName, byte[] data)
        {
            var result = await _service.StoreFileAsync(fileName, data);
            return !string.IsNullOrEmpty(result);
        }

        public async Task<byte[]> DownloadAsync(string fileName)
        {
            return await _service.RetrieveFileAsync(fileName);
        }

        public async Task<bool> DeleteAsync(string fileName)
        {
            return await _service.EraseFileAsync(fileName);
        }
    }

    #endregion

    #region Example 7: Payment Gateway Classes

    public class PaymentRequest
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
    }

    public class PaymentResult
    {
        public bool IsSuccess { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
    }

    public interface IPaymentGateway
    {
        PaymentResult ProcessPayment(PaymentRequest request);
    }

    // Different payment services
    public class StripeService
    {
        public StripeChargeResult CreateCharge(int amountInCents, string currency, string source, string customerEmail)
        {
            Console.WriteLine($"  Stripe: Charging {amountInCents} cents in {currency}");
            return new StripeChargeResult
            {
                Id = $"ch_{Guid.NewGuid().ToString()[..10]}",
                Status = "succeeded",
                Paid = true
            };
        }
    }

    public class StripeChargeResult
    {
        public string Id { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public bool Paid { get; set; }
    }

    public class PayPalService
    {
        public PayPalPaymentResult ExecutePayment(decimal amount, string currency, string payerEmail)
        {
            Console.WriteLine($"  PayPal: Processing payment of {amount} {currency}");
            return new PayPalPaymentResult
            {
                TransactionId = $"PAY-{Guid.NewGuid().ToString()[..10]}",
                State = "approved",
                Success = true
            };
        }
    }

    public class PayPalPaymentResult
    {
        public string TransactionId { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public bool Success { get; set; }
    }

    public class SquareService
    {
        public SquareTransactionResult ProcessTransaction(long amountMoney, string currency, string customerId)
        {
            Console.WriteLine($"  Square: Processing {amountMoney} {currency}");
            return new SquareTransactionResult
            {
                TransactionId = $"SQ-{Guid.NewGuid().ToString()[..10]}",
                Status = "COMPLETED",
                IsSuccessful = true
            };
        }
    }

    public class SquareTransactionResult
    {
        public string TransactionId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public bool IsSuccessful { get; set; }
    }

    // Adapters
    public class StripeAdapter : IPaymentGateway
    {
        private readonly StripeService _service;

        public StripeAdapter(StripeService service)
        {
            _service = service;
        }

        public PaymentResult ProcessPayment(PaymentRequest request)
        {
            var amountInCents = (int)(request.Amount * 100);
            var result = _service.CreateCharge(amountInCents, request.Currency, request.PaymentMethod, request.CustomerEmail);

            return new PaymentResult
            {
                IsSuccess = result.Paid,
                Status = result.Status,
                Message = result.Paid ? "Payment successful" : "Payment failed",
                TransactionId = result.Id
            };
        }
    }

    public class PayPalAdapter : IPaymentGateway
    {
        private readonly PayPalService _service;

        public PayPalAdapter(PayPalService service)
        {
            _service = service;
        }

        public PaymentResult ProcessPayment(PaymentRequest request)
        {
            var result = _service.ExecutePayment(request.Amount, request.Currency, request.CustomerEmail);

            return new PaymentResult
            {
                IsSuccess = result.Success,
                Status = result.State,
                Message = result.Success ? "Payment approved" : "Payment failed",
                TransactionId = result.TransactionId
            };
        }
    }

    public class SquareAdapter : IPaymentGateway
    {
        private readonly SquareService _service;

        public SquareAdapter(SquareService service)
        {
            _service = service;
        }

        public PaymentResult ProcessPayment(PaymentRequest request)
        {
            var amountMoney = (long)(request.Amount * 100); // Square uses money in cents
            var result = _service.ProcessTransaction(amountMoney, request.Currency, request.CustomerEmail);

            return new PaymentResult
            {
                IsSuccess = result.IsSuccessful,
                Status = result.Status,
                Message = result.IsSuccessful ? "Transaction completed" : "Transaction failed",
                TransactionId = result.TransactionId
            };
        }
    }

    #endregion

    #region Example 8: Logging Framework Classes

    public interface ILogger
    {
        void LogInfo(string message);
        void LogWarning(string message);
        void LogError(string message, Exception? exception = null);
    }

    // Different logging frameworks
    public class NLogLogger
    {
        public void Info(string message)
        {
            Console.WriteLine($"  [NLog INFO] {message}");
        }

        public void Warn(string message)
        {
            Console.WriteLine($"  [NLog WARN] {message}");
        }

        public void Error(Exception? exception, string message)
        {
            Console.WriteLine($"  [NLog ERROR] {message} | Exception: {exception?.Message}");
        }
    }

    public class SerilogLogger
    {
        public void Information(string messageTemplate, params object[] propertyValues)
        {
            Console.WriteLine($"  [Serilog INFO] {string.Format(messageTemplate, propertyValues)}");
        }

        public void Warning(string messageTemplate, params object[] propertyValues)
        {
            Console.WriteLine($"  [Serilog WARN] {string.Format(messageTemplate, propertyValues)}");
        }

        public void Error(Exception? exception, string messageTemplate, params object[] propertyValues)
        {
            Console.WriteLine($"  [Serilog ERROR] {string.Format(messageTemplate, propertyValues)} | Exception: {exception?.Message}");
        }
    }

    public class Log4NetLogger
    {
        public void Info(object message)
        {
            Console.WriteLine($"  [Log4Net INFO] {message}");
        }

        public void Warn(object message)
        {
            Console.WriteLine($"  [Log4Net WARN] {message}");
        }

        public void Error(object message, Exception? exception = null)
        {
            Console.WriteLine($"  [Log4Net ERROR] {message} | Exception: {exception?.Message}");
        }
    }

    // Adapters
    public class NLogAdapter : ILogger
    {
        private readonly NLogLogger _logger;

        public NLogAdapter(NLogLogger logger)
        {
            _logger = logger;
        }

        public void LogInfo(string message)
        {
            _logger.Info(message);
        }

        public void LogWarning(string message)
        {
            _logger.Warn(message);
        }

        public void LogError(string message, Exception? exception = null)
        {
            _logger.Error(exception, message);
        }
    }

    public class SerilogAdapter : ILogger
    {
        private readonly SerilogLogger _logger;

        public SerilogAdapter(SerilogLogger logger)
        {
            _logger = logger;
        }

        public void LogInfo(string message)
        {
            _logger.Information(message);
        }

        public void LogWarning(string message)
        {
            _logger.Warning(message);
        }

        public void LogError(string message, Exception? exception = null)
        {
            _logger.Error(exception, message);
        }
    }

    public class Log4NetAdapter : ILogger
    {
        private readonly Log4NetLogger _logger;

        public Log4NetAdapter(Log4NetLogger logger)
        {
            _logger = logger;
        }

        public void LogInfo(string message)
        {
            _logger.Info(message);
        }

        public void LogWarning(string message)
        {
            _logger.Warn(message);
        }

        public void LogError(string message, Exception? exception = null)
        {
            _logger.Error(message, exception);
        }
    }

    #endregion

    #region Example 9: Caching System Classes

    public interface ICacheProvider
    {
        void Set<T>(string key, T value, TimeSpan expiration);
        T? Get<T>(string key);
        bool Exists(string key);
        void Remove(string key);
    }

    // Different caching systems
    public class RedisCache
    {
        private readonly Dictionary<string, (object Value, DateTime Expiry)> _cache = new();

        public void StringSet(string key, string value, TimeSpan? expiry = null)
        {
            var expiryTime = expiry.HasValue ? DateTime.Now.Add(expiry.Value) : DateTime.MaxValue;
            _cache[key] = (value, expiryTime);
            Console.WriteLine($"  Redis: Set {key} = {value}");
        }

        public string? StringGet(string key)
        {
            if (_cache.TryGetValue(key, out var cached) && cached.Expiry > DateTime.Now)
            {
                Console.WriteLine($"  Redis: Get {key} = {cached.Value}");
                return cached.Value.ToString();
            }
            Console.WriteLine($"  Redis: Key {key} not found or expired");
            return null;
        }

        public bool KeyExists(string key)
        {
            var exists = _cache.ContainsKey(key) && _cache[key].Expiry > DateTime.Now;
            Console.WriteLine($"  Redis: Key {key} exists: {exists}");
            return exists;
        }

        public bool KeyDelete(string key)
        {
            var removed = _cache.Remove(key);
            Console.WriteLine($"  Redis: Deleted {key}: {removed}");
            return removed;
        }
    }

    public class MemcachedCache
    {
        private readonly Dictionary<string, (object Value, DateTime Expiry)> _cache = new();

        public bool Store(string key, object value, int seconds)
        {
            var expiry = DateTime.Now.AddSeconds(seconds);
            _cache[key] = (value, expiry);
            Console.WriteLine($"  Memcached: Store {key} = {value}");
            return true;
        }

        public object? Retrieve(string key)
        {
            if (_cache.TryGetValue(key, out var cached) && cached.Expiry > DateTime.Now)
            {
                Console.WriteLine($"  Memcached: Retrieve {key} = {cached.Value}");
                return cached.Value;
            }
            Console.WriteLine($"  Memcached: Key {key} not found or expired");
            return null;
        }

        public bool Delete(string key)
        {
            var removed = _cache.Remove(key);
            Console.WriteLine($"  Memcached: Delete {key}: {removed}");
            return removed;
        }
    }

    public class InMemoryCache
    {
        private readonly ConcurrentDictionary<string, (object Value, DateTime Expiry)> _cache = new();

        public void Add(string key, object value, TimeSpan expiration)
        {
            var expiry = DateTime.Now.Add(expiration);
            _cache.AddOrUpdate(key, (value, expiry), (k, v) => (value, expiry));
            Console.WriteLine($"  InMemory: Add {key} = {value}");
        }

        public object? Get(string key)
        {
            if (_cache.TryGetValue(key, out var cached) && cached.Expiry > DateTime.Now)
            {
                Console.WriteLine($"  InMemory: Get {key} = {cached.Value}");
                return cached.Value;
            }
            Console.WriteLine($"  InMemory: Key {key} not found or expired");
            return null;
        }

        public void Remove(string key)
        {
            var removed = _cache.TryRemove(key, out _);
            Console.WriteLine($"  InMemory: Remove {key}: {removed}");
        }

        public bool ContainsKey(string key)
        {
            var exists = _cache.ContainsKey(key) && _cache[key].Expiry > DateTime.Now;
            Console.WriteLine($"  InMemory: Contains {key}: {exists}");
            return exists;
        }
    }

    // Adapters
    public class RedisCacheAdapter : ICacheProvider
    {
        private readonly RedisCache _cache;

        public RedisCacheAdapter(RedisCache cache)
        {
            _cache = cache;
        }

        public void Set<T>(string key, T value, TimeSpan expiration)
        {
            var serialized = System.Text.Json.JsonSerializer.Serialize(value);
            _cache.StringSet(key, serialized, expiration);
        }

        public T? Get<T>(string key)
        {
            var value = _cache.StringGet(key);
            if (value == null) return default(T);

            try
            {
                return System.Text.Json.JsonSerializer.Deserialize<T>(value);
            }
            catch
            {
                return default(T);
            }
        }

        public bool Exists(string key)
        {
            return _cache.KeyExists(key);
        }

        public void Remove(string key)
        {
            _cache.KeyDelete(key);
        }
    }

    public class MemcachedAdapter : ICacheProvider
    {
        private readonly MemcachedCache _cache;

        public MemcachedAdapter(MemcachedCache cache)
        {
            _cache = cache;
        }

        public void Set<T>(string key, T value, TimeSpan expiration)
        {
            _cache.Store(key, value!, (int)expiration.TotalSeconds);
        }

        public T? Get<T>(string key)
        {
            var value = _cache.Retrieve(key);
            return value is T ? (T)value : default(T);
        }

        public bool Exists(string key)
        {
            return _cache.Retrieve(key) != null;
        }

        public void Remove(string key)
        {
            _cache.Delete(key);
        }
    }

    public class InMemoryCacheAdapter : ICacheProvider
    {
        private readonly InMemoryCache _cache;

        public InMemoryCacheAdapter(InMemoryCache cache)
        {
            _cache = cache;
        }

        public void Set<T>(string key, T value, TimeSpan expiration)
        {
            _cache.Add(key, value!, expiration);
        }

        public T? Get<T>(string key)
        {
            var value = _cache.Get(key);
            return value is T ? (T)value : default(T);
        }

        public bool Exists(string key)
        {
            return _cache.ContainsKey(key);
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }
    }

    #endregion
}

/*
 * MEMORY ALLOCATION CONSIDERATIONS:
 * =================================
 * 
 * 1. ADAPTER OVERHEAD:
 *    - Each adapter instance creates additional object overhead
 *    - Adapters hold references to adaptee objects, preventing garbage collection
 *    - Consider using singleton adapters for stateless adaptees to reduce memory usage
 *    - Multiple adapters for the same adaptee can lead to memory multiplication
 * 
 * 2. OBJECT COMPOSITION:
 *    - Object adapter pattern uses composition, creating wrapper objects
 *    - Each adapter holds a reference to the adapted object
 *    - Consider object pooling for frequently created adapter instances
 *    - Be mindful of deep adapter chains that can create memory overhead
 * 
 * 3. DATA CONVERSION:
 *    - Adapters often perform data conversion between formats
 *    - Temporary objects created during conversion can impact GC pressure
 *    - Consider streaming approaches for large data conversions
 *    - Cache converted data when appropriate to avoid repeated conversions
 * 
 * 4. ADAPTER LIFETIME:
 *    - Consider adapter lifetime management in DI containers
 *    - Singleton adapters for stateless operations reduce memory footprint
 *    - Transient adapters for stateful operations may be necessary
 *    - Dispose adapters properly if they hold unmanaged resources
 * 
 * MULTITHREAD CONSIDERATIONS:
 * ===========================
 * 
 * 1. ADAPTER THREAD SAFETY:
 *    - Adapters are typically not thread-safe unless specifically designed
 *    - Thread safety depends on the underlying adaptee's thread safety
 *    - Consider using concurrent collections in adapters that maintain state
 *    - Implement proper synchronization for adapters that modify shared state
 * 
 * 2. ADAPTEE PROTECTION:
 *    - Adapters can provide thread-safe access to non-thread-safe adaptees
 *    - Use locks, semaphores, or other synchronization primitives in adapters
 *    - Consider using ThreadLocal storage for per-thread adaptee instances
 *    - Be careful with reentrancy issues when adapters call each other
 * 
 * 3. CONCURRENT OPERATIONS:
 *    - Multiple threads can use different adapter instances safely
 *    - Shared adapter instances require careful thread safety design
 *    - Consider using immutable adapters when possible
 *    - Async adapters should handle concurrent operations properly
 * 
 * 4. RESOURCE MANAGEMENT:
 *    - Adapters may manage resources (connections, file handles, etc.)
 *    - Implement proper disposal patterns for resource cleanup
 *    - Use using statements or try-finally blocks for resource management
 *    - Consider connection pooling in adapters that manage expensive resources
 * 
 * 5. PERFORMANCE CONSIDERATIONS:
 *    - Adapter pattern adds a level of indirection with slight performance cost
 *    - Virtual method calls in interfaces have minimal overhead
 *    - Data conversion in adapters can be expensive for large objects
 *    - Consider caching strategies for expensive adapter operations
 *    - Profile adapter performance under load to identify bottlenecks
 * 
 * 6. BEST PRACTICES:
 *    - Design adapters to be stateless when possible for better thread safety
 *    - Use dependency injection to manage adapter lifetimes
 *    - Implement proper error handling and logging in adapters
 *    - Consider using factory patterns for creating appropriate adapters
 *    - Document thread safety guarantees of adapter implementations
 *    - Use async/await patterns for I/O-bound adapter operations
 *    - Implement circuit breaker patterns for unreliable external services
 *    - Consider using adapter chains for complex transformations
 */
