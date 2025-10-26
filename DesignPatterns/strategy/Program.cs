using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using System.Text;

/*
In the strategy pattern you are abstracting algorithms into separate strategy classes.
Instead of the context class implementing multiple algorithms with conditional logic,
the context delegates the responsibility of algorithm selection to the strategy classes.
*/

/*
 * STRATEGY DESIGN PATTERN IN C#
 * 
 * PURPOSE:
 * The Strategy pattern defines a family of algorithms, encapsulates each one, and makes them
 * interchangeable. It lets the algorithm vary independently from clients that use it.
 * The pattern enables selecting algorithms at runtime and promotes composition over inheritance.
 * 
 * CORE BENEFITS:
 * - Eliminates conditional statements for algorithm selection
 * - Makes algorithms interchangeable and easy to extend
 * - Promotes Open/Closed Principle - open for extension, closed for modification
 * - Encapsulates algorithm implementation details
 * - Enables runtime algorithm switching
 * - Simplifies unit testing by isolating algorithm logic
 * 
 * SCENARIOS TO USE:
 * - When you have multiple ways to perform a task (sorting, compression, encryption)
 * - When you want to switch algorithms at runtime based on context
 * - To eliminate large if-else or switch statements for algorithm selection
 * - When implementing payment processing with multiple payment methods
 * - For data validation with different validation rules
 * - In gaming for different AI behaviors or movement patterns
 * - For file processing with different formats (CSV, JSON, XML)
 * - When implementing different pricing strategies in e-commerce
 * - For database access with different providers
 * - When building pluggable architectures with swappable components
 * 
 * SCENARIOS NOT TO USE:
 * - When you only have one algorithm and it's unlikely to change
 * - If algorithm selection logic is simple and doesn't justify the extra complexity
 * - When performance is critical and strategy pattern adds unnecessary overhead
 * - If algorithms are tightly coupled with the context and can't be separated
 * - When the number of strategies is very small and unlikely to grow
 * - If clients need to understand algorithm details to choose appropriately
 * - When algorithms share significant amounts of common code
 * - If the strategy interface becomes too complex or unwieldy
 */

namespace StrategyPattern
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== Strategy Design Pattern Examples ===\n");

            // Example 1: Basic Sorting Strategies
            SortingStrategiesExample();

            // Example 2: Payment Processing Strategies
            PaymentProcessingExample();

            // Example 3: Compression Strategies
            CompressionStrategiesExample();

            // Example 4: Validation Strategies
            ValidationStrategiesExample();

            // Example 5: Thread-Safe Strategy Manager
            await ThreadSafeStrategyExample();

            // Example 6: Dynamic Strategy Loading
            DynamicStrategyExample();

            // Example 7: Async Strategies
            await AsyncStrategyExample();

            // Example 8: Gaming AI Strategies
            GamingAIExample();

            // Example 9: Data Export Strategies
            DataExportExample();

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        #region Example 1: Basic Sorting Strategies

        static void SortingStrategiesExample()
        {
            Console.WriteLine("1. Sorting Strategies:");
            Console.WriteLine("======================");

            var data = new int[] { 64, 34, 25, 12, 22, 11, 90 };
            var context = new SortingContext();

            // Test different sorting algorithms
            Console.WriteLine($"Original data: [{string.Join(", ", data)}]");

            // Bubble Sort
            var bubbleData = (int[])data.Clone();
            context.SetStrategy(new BubbleSortStrategy());
            context.Sort(bubbleData);
            Console.WriteLine($"Bubble Sort:   [{string.Join(", ", bubbleData)}]");

            // Quick Sort
            var quickData = (int[])data.Clone();
            context.SetStrategy(new QuickSortStrategy());
            context.Sort(quickData);
            Console.WriteLine($"Quick Sort:    [{string.Join(", ", quickData)}]");

            // Merge Sort
            var mergeData = (int[])data.Clone();
            context.SetStrategy(new MergeSortStrategy());
            context.Sort(mergeData);
            Console.WriteLine($"Merge Sort:    [{string.Join(", ", mergeData)}]");

            Console.WriteLine();
        }

        #endregion

        #region Example 2: Payment Processing Strategies

        static void PaymentProcessingExample()
        {
            Console.WriteLine("2. Payment Processing Strategies:");
            Console.WriteLine("==================================");

            var paymentProcessor = new PaymentProcessor();
            var order = new Order(1, 99.99m, "Widget");

            // Credit Card Payment
            paymentProcessor.SetPaymentMethod(new CreditCardPayment("1234-5678-9012-3456", "John Doe"));
            var result1 = paymentProcessor.ProcessPayment(order);
            Console.WriteLine($"Credit Card: {result1.Status} - {result1.Message}");

            // PayPal Payment
            paymentProcessor.SetPaymentMethod(new PayPalPayment("john.doe@email.com"));
            var result2 = paymentProcessor.ProcessPayment(order);
            Console.WriteLine($"PayPal: {result2.Status} - {result2.Message}");

            // Bank Transfer
            paymentProcessor.SetPaymentMethod(new BankTransferPayment("123456789", "BANK123"));
            var result3 = paymentProcessor.ProcessPayment(order);
            Console.WriteLine($"Bank Transfer: {result3.Status} - {result3.Message}");

            // Cryptocurrency
            paymentProcessor.SetPaymentMethod(new CryptocurrencyPayment("1A1zP1eP5QGefi2DMPTfTL5SLmv7DivfNa", "Bitcoin"));
            var result4 = paymentProcessor.ProcessPayment(order);
            Console.WriteLine($"Cryptocurrency: {result4.Status} - {result4.Message}");

            Console.WriteLine();
        }

        #endregion

        #region Example 3: Compression Strategies

        static void CompressionStrategiesExample()
        {
            Console.WriteLine("3. Compression Strategies:");
            Console.WriteLine("===========================");

            var compressor = new DataCompressor();
            var data = "This is a sample text that will be compressed using different algorithms. " +
                      "The Strategy pattern allows us to switch between compression algorithms easily.";

            Console.WriteLine($"Original size: {data.Length} characters");

            // ZIP Compression
            compressor.SetCompressionStrategy(new ZipCompressionStrategy());
            var zipResult = compressor.Compress(data);
            Console.WriteLine($"ZIP: {zipResult.CompressedSize} bytes ({zipResult.CompressionRatio:P1} reduction)");

            // GZIP Compression  
            compressor.SetCompressionStrategy(new GzipCompressionStrategy());
            var gzipResult = compressor.Compress(data);
            Console.WriteLine($"GZIP: {gzipResult.CompressedSize} bytes ({gzipResult.CompressionRatio:P1} reduction)");

            // LZ4 Compression
            compressor.SetCompressionStrategy(new Lz4CompressionStrategy());
            var lz4Result = compressor.Compress(data);
            Console.WriteLine($"LZ4: {lz4Result.CompressedSize} bytes ({lz4Result.CompressionRatio:P1} reduction)");

            Console.WriteLine();
        }

        #endregion

        #region Example 4: Validation Strategies

        static void ValidationStrategiesExample()
        {
            Console.WriteLine("4. Validation Strategies:");
            Console.WriteLine("==========================");

            var validator = new UserValidator();

            var users = new[]
            {
                new User("john@email.com", "password123", 25),
                new User("invalid-email", "pass", 16),
                new User("admin@company.com", "SecureP@ss123", 30)
            };

            foreach (var user in users)
            {
                Console.WriteLine($"\nValidating user: {user.Email}");

                // Email validation
                validator.SetValidationStrategy(new EmailValidationStrategy());
                var emailResult = validator.Validate(user);
                Console.WriteLine($"  Email: {(emailResult.IsValid ? "✓" : "✗")} {emailResult.Message}");

                // Password validation
                validator.SetValidationStrategy(new PasswordValidationStrategy());
                var passwordResult = validator.Validate(user);
                Console.WriteLine($"  Password: {(passwordResult.IsValid ? "✓" : "✗")} {passwordResult.Message}");

                // Age validation
                validator.SetValidationStrategy(new AgeValidationStrategy());
                var ageResult = validator.Validate(user);
                Console.WriteLine($"  Age: {(ageResult.IsValid ? "✓" : "✗")} {ageResult.Message}");
            }

            Console.WriteLine();
        }

        #endregion

        #region Example 5: Thread-Safe Strategy Manager

        static async Task ThreadSafeStrategyExample()
        {
            Console.WriteLine("5. Thread-Safe Strategy Manager:");
            Console.WriteLine("=================================");

            var calculator = new ThreadSafeCalculator();
            var tasks = new List<Task>();

            // Create multiple tasks that use different calculation strategies
            for (int i = 1; i <= 5; i++)
            {
                int taskId = i;
                tasks.Add(Task.Run(async () =>
                {
                    ICalculationStrategy strategy = (taskId % 3) switch
                    {
                        0 => new AdditionStrategy(),
                        1 => new MultiplicationStrategy(),
                        _ => new PowerStrategy()
                    };

                    calculator.SetStrategy(strategy);

                    for (int j = 1; j <= 3; j++)
                    {
                        var result = calculator.Calculate(taskId, j);
                        Console.WriteLine($"[Task {taskId}] {strategy.GetType().Name}: {taskId} op {j} = {result}");
                        await Task.Delay(100);
                    }
                }));
            }

            await Task.WhenAll(tasks);
            Console.WriteLine();
        }

        #endregion

        #region Example 6: Dynamic Strategy Loading

        static void DynamicStrategyExample()
        {
            Console.WriteLine("6. Dynamic Strategy Loading:");
            Console.WriteLine("=============================");

            var factory = new StrategyFactory();
            var processor = new DocumentProcessor();

            var documents = new[]
            {
                new Document("report.pdf", "PDF content here"),
                new Document("data.csv", "Name,Age,City\nJohn,25,NYC"),
                new Document("config.xml", "<config><setting>value</setting></config>")
            };

            foreach (var doc in documents)
            {
                Console.WriteLine($"\nProcessing: {doc.FileName}");

                var strategy = factory.CreateStrategy(doc.FileName);
                if (strategy != null)
                {
                    processor.SetProcessingStrategy(strategy);
                    var result = processor.ProcessDocument(doc);
                    Console.WriteLine($"  Result: {result}");
                }
                else
                {
                    Console.WriteLine("  No strategy available for this file type");
                }
            }

            Console.WriteLine();
        }

        #endregion

        #region Example 7: Async Strategies

        static async Task AsyncStrategyExample()
        {
            Console.WriteLine("7. Async Strategy Examples:");
            Console.WriteLine("============================");

            var downloader = new FileDownloader();
            var urls = new[]
            {
                "https://example.com/small-file.txt",
                "https://example.com/large-file.zip",
                "https://api.example.com/data.json"
            };

            foreach (var url in urls)
            {
                Console.WriteLine($"\nDownloading: {url}");

                // Choose strategy based on file type/size
                IAsyncDownloadStrategy strategy = url.Contains("large")
                    ? new ChunkedDownloadStrategy()
                    : url.Contains("api")
                    ? new ApiDownloadStrategy()
                    : new SimpleDownloadStrategy();

                downloader.SetStrategy(strategy);
                var result = await downloader.DownloadAsync(url);
                Console.WriteLine($"  Status: {result.Status} - {result.Message}");
            }

            Console.WriteLine();
        }

        #endregion

        #region Example 8: Gaming AI Strategies

        static void GamingAIExample()
        {
            Console.WriteLine("8. Gaming AI Strategies:");
            Console.WriteLine("=========================");

            var enemies = new[]
            {
                new Enemy("Goblin", EnemyType.Aggressive, 100),
                new Enemy("Guard", EnemyType.Defensive, 150),
                new Enemy("Scout", EnemyType.Evasive, 80)
            };

            var player = new Player { X = 50, Y = 50, Health = 100 };

            foreach (var enemy in enemies)
            {
                Console.WriteLine($"\n{enemy.Name} ({enemy.Type}) encounters player:");

                var aiController = new AIController();
                IAIStrategy strategy = enemy.Type switch
                {
                    EnemyType.Aggressive => new AggressiveAIStrategy(),
                    EnemyType.Defensive => new DefensiveAIStrategy(),
                    EnemyType.Evasive => new EvasiveAIStrategy(),
                    _ => new AggressiveAIStrategy()
                };

                aiController.SetStrategy(strategy);
                var action = aiController.DecideAction(enemy, player);
                Console.WriteLine($"  Action: {action.Type} - {action.Description}");
            }

            Console.WriteLine();
        }

        #endregion

        #region Example 9: Data Export Strategies

        static void DataExportExample()
        {
            Console.WriteLine("9. Data Export Strategies:");
            Console.WriteLine("===========================");

            var exporter = new DataExporter();
            var data = new[]
            {
                new { Name = "John Doe", Age = 30, Department = "IT" },
                new { Name = "Jane Smith", Age = 25, Department = "HR" },
                new { Name = "Bob Johnson", Age = 35, Department = "Finance" }
            };

            // Export to different formats
            var formats = new IExportStrategy[]
            {
                new CsvExportStrategy(),
                new JsonExportStrategy(),
                new XmlExportStrategy()
            };

            foreach (var strategy in formats)
            {
                exporter.SetExportStrategy(strategy);
                var result = exporter.Export(data, $"employees.{strategy.FileExtension}");
                Console.WriteLine($"{strategy.FormatName}: {result.BytesWritten} bytes written to {result.FileName}");
            }

            Console.WriteLine();
        }

        #endregion
    }

    #region Core Strategy Pattern Classes

    // Strategy interface
    public interface IStrategy<in TInput, out TOutput>
    {
        TOutput Execute(TInput input);
    }

    // Context class that uses strategies
    // MEMORY ALLOCATION: Holds reference to current strategy - minimal memory overhead
    public class StrategyContext<TInput, TOutput>
    {
        private IStrategy<TInput, TOutput>? _strategy;

        public void SetStrategy(IStrategy<TInput, TOutput> strategy)
        {
            _strategy = strategy;
        }

        public TOutput ExecuteStrategy(TInput input)
        {
            if (_strategy == null)
                throw new InvalidOperationException("Strategy not set");

            return _strategy.Execute(input);
        }
    }

    #endregion

    #region Example 1: Sorting Strategy Classes

    public interface ISortingStrategy
    {
        void Sort(int[] array);
        string AlgorithmName { get; }
    }

    public class SortingContext
    {
        private ISortingStrategy? _strategy;

        public void SetStrategy(ISortingStrategy strategy)
        {
            _strategy = strategy;
        }

        public void Sort(int[] array)
        {
            if (_strategy == null)
                throw new InvalidOperationException("Sorting strategy not set");

            Console.WriteLine($"  Using {_strategy.AlgorithmName}");
            _strategy.Sort(array);
        }
    }

    public class BubbleSortStrategy : ISortingStrategy
    {
        public string AlgorithmName => "Bubble Sort";

        public void Sort(int[] array)
        {
            int n = array.Length;
            for (int i = 0; i < n - 1; i++)
            {
                for (int j = 0; j < n - i - 1; j++)
                {
                    if (array[j] > array[j + 1])
                    {
                        (array[j], array[j + 1]) = (array[j + 1], array[j]);
                    }
                }
            }
        }
    }

    public class QuickSortStrategy : ISortingStrategy
    {
        public string AlgorithmName => "Quick Sort";

        public void Sort(int[] array)
        {
            QuickSort(array, 0, array.Length - 1);
        }

        private void QuickSort(int[] array, int low, int high)
        {
            if (low < high)
            {
                int pi = Partition(array, low, high);
                QuickSort(array, low, pi - 1);
                QuickSort(array, pi + 1, high);
            }
        }

        private int Partition(int[] array, int low, int high)
        {
            int pivot = array[high];
            int i = low - 1;

            for (int j = low; j < high; j++)
            {
                if (array[j] < pivot)
                {
                    i++;
                    (array[i], array[j]) = (array[j], array[i]);
                }
            }

            (array[i + 1], array[high]) = (array[high], array[i + 1]);
            return i + 1;
        }
    }

    public class MergeSortStrategy : ISortingStrategy
    {
        public string AlgorithmName => "Merge Sort";

        public void Sort(int[] array)
        {
            MergeSort(array, 0, array.Length - 1);
        }

        private void MergeSort(int[] array, int left, int right)
        {
            if (left < right)
            {
                int middle = (left + right) / 2;
                MergeSort(array, left, middle);
                MergeSort(array, middle + 1, right);
                Merge(array, left, middle, right);
            }
        }

        private void Merge(int[] array, int left, int middle, int right)
        {
            int n1 = middle - left + 1;
            int n2 = right - middle;

            int[] leftArray = new int[n1];
            int[] rightArray = new int[n2];

            Array.Copy(array, left, leftArray, 0, n1);
            Array.Copy(array, middle + 1, rightArray, 0, n2);

            int i = 0, j = 0, k = left;

            while (i < n1 && j < n2)
            {
                if (leftArray[i] <= rightArray[j])
                    array[k++] = leftArray[i++];
                else
                    array[k++] = rightArray[j++];
            }

            while (i < n1) array[k++] = leftArray[i++];
            while (j < n2) array[k++] = rightArray[j++];
        }
    }

    #endregion

    #region Example 2: Payment Processing Classes

    public class Order
    {
        public int Id { get; }
        public decimal Amount { get; }
        public string Description { get; }

        public Order(int id, decimal amount, string description)
        {
            Id = id;
            Amount = amount;
            Description = description;
        }
    }

    public class PaymentResult
    {
        public bool Status { get; set; }
        public string Message { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
    }

    public interface IPaymentStrategy
    {
        PaymentResult ProcessPayment(Order order);
        string PaymentMethodName { get; }
    }

    public class PaymentProcessor
    {
        private IPaymentStrategy? _paymentMethod;

        public void SetPaymentMethod(IPaymentStrategy paymentMethod)
        {
            _paymentMethod = paymentMethod;
        }

        public PaymentResult ProcessPayment(Order order)
        {
            if (_paymentMethod == null)
                throw new InvalidOperationException("Payment method not set");

            Console.WriteLine($"  Processing ${order.Amount} payment using {_paymentMethod.PaymentMethodName}");
            return _paymentMethod.ProcessPayment(order);
        }
    }

    public class CreditCardPayment : IPaymentStrategy
    {
        private readonly string _cardNumber;
        private readonly string _cardHolderName;

        public string PaymentMethodName => "Credit Card";

        public CreditCardPayment(string cardNumber, string cardHolderName)
        {
            _cardNumber = cardNumber;
            _cardHolderName = cardHolderName;
        }

        public PaymentResult ProcessPayment(Order order)
        {
            // Simulate credit card processing
            return new PaymentResult
            {
                Status = true,
                Message = $"Credit card payment successful",
                TransactionId = $"CC_{Guid.NewGuid().ToString()[..8]}"
            };
        }
    }

    public class PayPalPayment : IPaymentStrategy
    {
        private readonly string _email;

        public string PaymentMethodName => "PayPal";

        public PayPalPayment(string email)
        {
            _email = email;
        }

        public PaymentResult ProcessPayment(Order order)
        {
            return new PaymentResult
            {
                Status = true,
                Message = $"PayPal payment successful",
                TransactionId = $"PP_{Guid.NewGuid().ToString()[..8]}"
            };
        }
    }

    public class BankTransferPayment : IPaymentStrategy
    {
        private readonly string _accountNumber;
        private readonly string _routingNumber;

        public string PaymentMethodName => "Bank Transfer";

        public BankTransferPayment(string accountNumber, string routingNumber)
        {
            _accountNumber = accountNumber;
            _routingNumber = routingNumber;
        }

        public PaymentResult ProcessPayment(Order order)
        {
            return new PaymentResult
            {
                Status = true,
                Message = $"Bank transfer initiated",
                TransactionId = $"BT_{Guid.NewGuid().ToString()[..8]}"
            };
        }
    }

    public class CryptocurrencyPayment : IPaymentStrategy
    {
        private readonly string _walletAddress;
        private readonly string _currency;

        public string PaymentMethodName => $"{_currency} Cryptocurrency";

        public CryptocurrencyPayment(string walletAddress, string currency)
        {
            _walletAddress = walletAddress;
            _currency = currency;
        }

        public PaymentResult ProcessPayment(Order order)
        {
            return new PaymentResult
            {
                Status = true,
                Message = $"{_currency} payment successful",
                TransactionId = $"CRYPTO_{Guid.NewGuid().ToString()[..8]}"
            };
        }
    }

    #endregion

    #region Example 3: Compression Strategy Classes

    public class CompressionResult
    {
        public byte[] CompressedData { get; set; } = Array.Empty<byte>();
        public int OriginalSize { get; set; }
        public int CompressedSize { get; set; }
        public double CompressionRatio => (double)(OriginalSize - CompressedSize) / OriginalSize;
    }

    public interface ICompressionStrategy
    {
        CompressionResult Compress(string data);
        string AlgorithmName { get; }
    }

    public class DataCompressor
    {
        private ICompressionStrategy? _strategy;

        public void SetCompressionStrategy(ICompressionStrategy strategy)
        {
            _strategy = strategy;
        }

        public CompressionResult Compress(string data)
        {
            if (_strategy == null)
                throw new InvalidOperationException("Compression strategy not set");

            Console.WriteLine($"  Using {_strategy.AlgorithmName}");
            return _strategy.Compress(data);
        }
    }

    public class ZipCompressionStrategy : ICompressionStrategy
    {
        public string AlgorithmName => "ZIP Compression";

        public CompressionResult Compress(string data)
        {
            // Simulate ZIP compression
            var bytes = Encoding.UTF8.GetBytes(data);
            var compressedSize = (int)(bytes.Length * 0.6); // Simulate 40% compression

            return new CompressionResult
            {
                CompressedData = new byte[compressedSize],
                OriginalSize = bytes.Length,
                CompressedSize = compressedSize
            };
        }
    }

    public class GzipCompressionStrategy : ICompressionStrategy
    {
        public string AlgorithmName => "GZIP Compression";

        public CompressionResult Compress(string data)
        {
            var bytes = Encoding.UTF8.GetBytes(data);
            var compressedSize = (int)(bytes.Length * 0.55); // Simulate 45% compression

            return new CompressionResult
            {
                CompressedData = new byte[compressedSize],
                OriginalSize = bytes.Length,
                CompressedSize = compressedSize
            };
        }
    }

    public class Lz4CompressionStrategy : ICompressionStrategy
    {
        public string AlgorithmName => "LZ4 Compression";

        public CompressionResult Compress(string data)
        {
            var bytes = Encoding.UTF8.GetBytes(data);
            var compressedSize = (int)(bytes.Length * 0.7); // Simulate 30% compression (faster, less compression)

            return new CompressionResult
            {
                CompressedData = new byte[compressedSize],
                OriginalSize = bytes.Length,
                CompressedSize = compressedSize
            };
        }
    }

    #endregion

    #region Example 4: Validation Strategy Classes

    public class User
    {
        public string Email { get; }
        public string Password { get; }
        public int Age { get; }

        public User(string email, string password, int age)
        {
            Email = email;
            Password = password;
            Age = age;
        }
    }

    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public interface IValidationStrategy
    {
        ValidationResult Validate(User user);
        string ValidationName { get; }
    }

    public class UserValidator
    {
        private IValidationStrategy? _strategy;

        public void SetValidationStrategy(IValidationStrategy strategy)
        {
            _strategy = strategy;
        }

        public ValidationResult Validate(User user)
        {
            if (_strategy == null)
                throw new InvalidOperationException("Validation strategy not set");

            return _strategy.Validate(user);
        }
    }

    public class EmailValidationStrategy : IValidationStrategy
    {
        public string ValidationName => "Email Validation";

        public ValidationResult Validate(User user)
        {
            bool isValid = user.Email.Contains("@") && user.Email.Contains(".");
            return new ValidationResult
            {
                IsValid = isValid,
                Message = isValid ? "Valid email format" : "Invalid email format"
            };
        }
    }

    public class PasswordValidationStrategy : IValidationStrategy
    {
        public string ValidationName => "Password Validation";

        public ValidationResult Validate(User user)
        {
            bool isValid = user.Password.Length >= 8 &&
                          user.Password.Any(char.IsUpper) &&
                          user.Password.Any(char.IsDigit);

            return new ValidationResult
            {
                IsValid = isValid,
                Message = isValid ? "Strong password" : "Password must be 8+ characters with uppercase and numbers"
            };
        }
    }

    public class AgeValidationStrategy : IValidationStrategy
    {
        public string ValidationName => "Age Validation";

        public ValidationResult Validate(User user)
        {
            bool isValid = user.Age >= 18 && user.Age <= 120;
            return new ValidationResult
            {
                IsValid = isValid,
                Message = isValid ? "Valid age" : "Age must be between 18 and 120"
            };
        }
    }

    #endregion

    #region Example 5: Thread-Safe Strategy Classes

    // MULTITHREAD ASPECTS: Thread-safe strategy management
    public interface ICalculationStrategy
    {
        double Calculate(double a, double b);
        string OperationName { get; }
    }

    public class ThreadSafeCalculator
    {
        private readonly object _strategyLock = new object();
        private ICalculationStrategy? _strategy;

        public void SetStrategy(ICalculationStrategy strategy)
        {
            lock (_strategyLock)
            {
                _strategy = strategy;
            }
        }

        public double Calculate(double a, double b)
        {
            ICalculationStrategy? currentStrategy;
            lock (_strategyLock)
            {
                currentStrategy = _strategy;
            }

            if (currentStrategy == null)
                throw new InvalidOperationException("Strategy not set");

            return currentStrategy.Calculate(a, b);
        }
    }

    public class AdditionStrategy : ICalculationStrategy
    {
        public string OperationName => "Addition";
        public double Calculate(double a, double b) => a + b;
    }

    public class MultiplicationStrategy : ICalculationStrategy
    {
        public string OperationName => "Multiplication";
        public double Calculate(double a, double b) => a * b;
    }

    public class PowerStrategy : ICalculationStrategy
    {
        public string OperationName => "Power";
        public double Calculate(double a, double b) => Math.Pow(a, b);
    }

    #endregion

    #region Example 6: Dynamic Strategy Loading Classes

    public class Document
    {
        public string FileName { get; }
        public string Content { get; }

        public Document(string fileName, string content)
        {
            FileName = fileName;
            Content = content;
        }
    }

    public interface IDocumentProcessingStrategy
    {
        string ProcessDocument(Document document);
        string SupportedExtension { get; }
    }

    public class DocumentProcessor
    {
        private IDocumentProcessingStrategy? _strategy;

        public void SetProcessingStrategy(IDocumentProcessingStrategy strategy)
        {
            _strategy = strategy;
        }

        public string ProcessDocument(Document document)
        {
            if (_strategy == null)
                throw new InvalidOperationException("Processing strategy not set");

            return _strategy.ProcessDocument(document);
        }
    }

    public class PdfProcessingStrategy : IDocumentProcessingStrategy
    {
        public string SupportedExtension => ".pdf";

        public string ProcessDocument(Document document)
        {
            return $"Extracted text from PDF: {document.Content.Length} characters";
        }
    }

    public class CsvProcessingStrategy : IDocumentProcessingStrategy
    {
        public string SupportedExtension => ".csv";

        public string ProcessDocument(Document document)
        {
            var lines = document.Content.Split('\n');
            return $"Parsed CSV: {lines.Length} rows";
        }
    }

    public class XmlProcessingStrategy : IDocumentProcessingStrategy
    {
        public string SupportedExtension => ".xml";

        public string ProcessDocument(Document document)
        {
            var tagCount = document.Content.Split('<').Length - 1;
            return $"Parsed XML: {tagCount} tags found";
        }
    }

    public class StrategyFactory
    {
        private readonly Dictionary<string, Func<IDocumentProcessingStrategy>> _strategies;

        public StrategyFactory()
        {
            _strategies = new Dictionary<string, Func<IDocumentProcessingStrategy>>
            {
                { ".pdf", () => new PdfProcessingStrategy() },
                { ".csv", () => new CsvProcessingStrategy() },
                { ".xml", () => new XmlProcessingStrategy() }
            };
        }

        public IDocumentProcessingStrategy? CreateStrategy(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLower();
            return _strategies.TryGetValue(extension, out var factory) ? factory() : null;
        }
    }

    #endregion

    #region Example 7: Async Strategy Classes

    public class DownloadResult
    {
        public bool Status { get; set; }
        public string Message { get; set; } = string.Empty;
        public long BytesDownloaded { get; set; }
    }

    public interface IAsyncDownloadStrategy
    {
        Task<DownloadResult> DownloadAsync(string url, CancellationToken cancellationToken = default);
        string StrategyName { get; }
    }

    public class FileDownloader
    {
        private IAsyncDownloadStrategy? _strategy;

        public void SetStrategy(IAsyncDownloadStrategy strategy)
        {
            _strategy = strategy;
        }

        public async Task<DownloadResult> DownloadAsync(string url, CancellationToken cancellationToken = default)
        {
            if (_strategy == null)
                throw new InvalidOperationException("Download strategy not set");

            Console.WriteLine($"  Using {_strategy.StrategyName}");
            return await _strategy.DownloadAsync(url, cancellationToken);
        }
    }

    public class SimpleDownloadStrategy : IAsyncDownloadStrategy
    {
        public string StrategyName => "Simple Download";

        public async Task<DownloadResult> DownloadAsync(string url, CancellationToken cancellationToken = default)
        {
            await Task.Delay(500, cancellationToken); // Simulate download
            return new DownloadResult
            {
                Status = true,
                Message = "Download completed",
                BytesDownloaded = 1024
            };
        }
    }

    public class ChunkedDownloadStrategy : IAsyncDownloadStrategy
    {
        public string StrategyName => "Chunked Download";

        public async Task<DownloadResult> DownloadAsync(string url, CancellationToken cancellationToken = default)
        {
            await Task.Delay(1500, cancellationToken); // Simulate chunked download
            return new DownloadResult
            {
                Status = true,
                Message = "Chunked download completed",
                BytesDownloaded = 10240
            };
        }
    }

    public class ApiDownloadStrategy : IAsyncDownloadStrategy
    {
        public string StrategyName => "API Download";

        public async Task<DownloadResult> DownloadAsync(string url, CancellationToken cancellationToken = default)
        {
            await Task.Delay(300, cancellationToken); // Simulate API call
            return new DownloadResult
            {
                Status = true,
                Message = "API data retrieved",
                BytesDownloaded = 512
            };
        }
    }

    #endregion

    #region Example 8: Gaming AI Strategy Classes

    public enum EnemyType { Aggressive, Defensive, Evasive }

    public class Enemy
    {
        public string Name { get; }
        public EnemyType Type { get; }
        public int Health { get; set; }
        public int X { get; set; } = 25;
        public int Y { get; set; } = 25;

        public Enemy(string name, EnemyType type, int health)
        {
            Name = name;
            Type = type;
            Health = health;
        }
    }

    public class Player
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Health { get; set; }
    }

    public class AIAction
    {
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public interface IAIStrategy
    {
        AIAction DecideAction(Enemy enemy, Player player);
        string StrategyName { get; }
    }

    public class AIController
    {
        private IAIStrategy? _strategy;

        public void SetStrategy(IAIStrategy strategy)
        {
            _strategy = strategy;
        }

        public AIAction DecideAction(Enemy enemy, Player player)
        {
            if (_strategy == null)
                throw new InvalidOperationException("AI strategy not set");

            return _strategy.DecideAction(enemy, player);
        }
    }

    public class AggressiveAIStrategy : IAIStrategy
    {
        public string StrategyName => "Aggressive AI";

        public AIAction DecideAction(Enemy enemy, Player player)
        {
            var distance = Math.Sqrt(Math.Pow(player.X - enemy.X, 2) + Math.Pow(player.Y - enemy.Y, 2));

            return distance < 10
                ? new AIAction { Type = "Attack", Description = "Charges directly at player" }
                : new AIAction { Type = "Chase", Description = "Moves aggressively towards player" };
        }
    }

    public class DefensiveAIStrategy : IAIStrategy
    {
        public string StrategyName => "Defensive AI";

        public AIAction DecideAction(Enemy enemy, Player player)
        {
            return enemy.Health < 50
                ? new AIAction { Type = "Retreat", Description = "Falls back to defensive position" }
                : new AIAction { Type = "Guard", Description = "Maintains defensive stance" };
        }
    }

    public class EvasiveAIStrategy : IAIStrategy
    {
        public string StrategyName => "Evasive AI";

        public AIAction DecideAction(Enemy enemy, Player player)
        {
            var distance = Math.Sqrt(Math.Pow(player.X - enemy.X, 2) + Math.Pow(player.Y - enemy.Y, 2));

            return distance < 20
                ? new AIAction { Type = "Evade", Description = "Moves to avoid direct confrontation" }
                : new AIAction { Type = "Patrol", Description = "Continues patrol pattern" };
        }
    }

    #endregion

    #region Example 9: Data Export Strategy Classes

    public class ExportResult
    {
        public string FileName { get; set; } = string.Empty;
        public int BytesWritten { get; set; }
        public bool Success { get; set; }
    }

    public interface IExportStrategy
    {
        ExportResult Export<T>(IEnumerable<T> data, string fileName);
        string FormatName { get; }
        string FileExtension { get; }
    }

    public class DataExporter
    {
        private IExportStrategy? _strategy;

        public void SetExportStrategy(IExportStrategy strategy)
        {
            _strategy = strategy;
        }

        public ExportResult Export<T>(IEnumerable<T> data, string fileName)
        {
            if (_strategy == null)
                throw new InvalidOperationException("Export strategy not set");

            Console.WriteLine($"  Exporting to {_strategy.FormatName}");
            return _strategy.Export(data, fileName);
        }
    }

    public class CsvExportStrategy : IExportStrategy
    {
        public string FormatName => "CSV";
        public string FileExtension => "csv";

        public ExportResult Export<T>(IEnumerable<T> data, string fileName)
        {
            var csv = new StringBuilder();
            var properties = typeof(T).GetProperties();

            // Header
            csv.AppendLine(string.Join(",", properties.Select(p => p.Name)));

            // Data rows
            foreach (var item in data)
            {
                var values = properties.Select(p => p.GetValue(item)?.ToString() ?? "");
                csv.AppendLine(string.Join(",", values));
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            return new ExportResult
            {
                FileName = fileName,
                BytesWritten = bytes.Length,
                Success = true
            };
        }
    }

    public class JsonExportStrategy : IExportStrategy
    {
        public string FormatName => "JSON";
        public string FileExtension => "json";

        public ExportResult Export<T>(IEnumerable<T> data, string fileName)
        {
            // Simulate JSON serialization
            var json = $"[{string.Join(",", data.Select(item => $"{{\"{typeof(T).Name}\": \"{item}\"}}"))}]";
            var bytes = Encoding.UTF8.GetBytes(json);

            return new ExportResult
            {
                FileName = fileName,
                BytesWritten = bytes.Length,
                Success = true
            };
        }
    }

    public class XmlExportStrategy : IExportStrategy
    {
        public string FormatName => "XML";
        public string FileExtension => "xml";

        public ExportResult Export<T>(IEnumerable<T> data, string fileName)
        {
            var xml = new StringBuilder();
            xml.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            xml.AppendLine($"<{typeof(T).Name}s>");

            foreach (var item in data)
            {
                xml.AppendLine($"  <{typeof(T).Name}>{item}</{typeof(T).Name}>");
            }

            xml.AppendLine($"</{typeof(T).Name}s>");

            var bytes = Encoding.UTF8.GetBytes(xml.ToString());
            return new ExportResult
            {
                FileName = fileName,
                BytesWritten = bytes.Length,
                Success = true
            };
        }
    }

    #endregion
}

/*
 * MEMORY ALLOCATION CONSIDERATIONS:
 * =================================
 * 
 * 1. STRATEGY OBJECT CREATION:
 *    - Strategy objects are typically lightweight and contain minimal state
 *    - Consider using singleton pattern for stateless strategies to reduce object creation
 *    - Use object pooling for strategies that are frequently created and destroyed
 *    - Be mindful of strategy objects that hold large amounts of data or resources
 * 
 * 2. CONTEXT OBJECT OVERHEAD:
 *    - Context objects hold references to current strategy - minimal memory overhead
 *    - Multiple contexts can share the same strategy instance safely (if stateless)
 *    - Consider using dependency injection to manage strategy lifetimes
 *    - Avoid storing strategy objects as static fields unless they're truly stateless
 * 
 * 3. ALGORITHM DATA STRUCTURES:
 *    - Some strategies may require temporary data structures (arrays, lists, etc.)
 *    - Consider reusing data structures across strategy invocations when possible
 *    - Be aware of algorithms that have high memory complexity (O(n²), etc.)
 *    - Use streaming approaches for strategies that process large datasets
 * 
 * 4. STRATEGY CACHING:
 *    - Cache expensive-to-create strategies when appropriate
 *    - Consider using weak references for cached strategies to allow garbage collection
 *    - Balance between memory usage and strategy creation performance
 *    - Implement proper cleanup for cached strategies that hold resources
 * 
 * MULTITHREAD CONSIDERATIONS:
 * ===========================
 * 
 * 1. STRATEGY STATELESSNESS:
 *    - Strategies should be stateless to be thread-safe by default
 *    - If strategies must maintain state, use thread-local storage or synchronization
 *    - Avoid sharing mutable state between strategy instances
 *    - Consider immutable data structures for strategy implementations
 * 
 * 2. CONTEXT THREAD SAFETY:
 *    - Context objects are not thread-safe by default when changing strategies
 *    - Use locks or atomic operations when multiple threads change strategies
 *    - Consider using separate context instances per thread for high-concurrency scenarios
 *    - Thread-safe strategy switching requires proper synchronization
 * 
 * 3. STRATEGY SWITCHING:
 *    - Strategy changes during execution can cause race conditions
 *    - Use locks to synchronize strategy setting and execution
 *    - Consider using copy-on-write semantics for strategy references
 *    - Avoid changing strategies while they're being executed by other threads
 * 
 * 4. ASYNC STRATEGIES:
 *    - Async strategies should properly handle cancellation tokens
 *    - Use ConfigureAwait(false) to avoid context switching overhead
 *    - Be careful with exception handling in async strategy implementations
 *    - Consider using concurrent collections for strategies that need shared state
 * 
 * 5. PERFORMANCE CONSIDERATIONS:
 *    - Strategy pattern adds minimal overhead (single method call indirection)
 *    - Virtual method calls have slight performance cost compared to direct calls
 *    - Consider using generic strategies to avoid boxing for value types
 *    - Profile strategy execution under load to identify performance bottlenecks
 * 
 * 6. BEST PRACTICES:
 *    - Design strategies to be independent and loosely coupled
 *    - Use dependency injection for complex strategy dependencies
 *    - Implement proper error handling and logging in strategies
 *    - Consider using the factory pattern for strategy creation
 *    - Document thread safety guarantees for each strategy
 *    - Use strategy interfaces that are cohesive and focused
 *    - Consider using the null object pattern for default strategies
 */
