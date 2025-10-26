using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using System.Text;
using System.Diagnostics;

/*
In the template method pattern you are abstracting the skeleton of an algorithm
into a base class. Instead of implementing the entire algorithm in each subclass,
you define the overall structure in the base class and allow subclasses to implement
specific steps of the algorithm.
*/

/*
 * TEMPLATE METHOD DESIGN PATTERN IN C#
 * 
 * PURPOSE:
 * The Template Method pattern defines the skeleton of an algorithm in a base class,
 * letting subclasses override specific steps of the algorithm without changing its structure.
 * It promotes code reuse by extracting common behavior into a base class while allowing
 * customization of specific steps through inheritance.
 * 
 * CORE BENEFITS:
 * - Eliminates code duplication by extracting common algorithm structure
 * - Provides a clear framework for implementing similar algorithms
 * - Ensures consistent algorithm execution order across implementations
 * - Follows the Hollywood Principle ("Don't call us, we'll call you")
 * - Enables easy extension of algorithms through inheritance
 * - Provides hooks for optional customization points
 * 
 * SCENARIOS TO USE:
 * - When you have multiple classes with similar algorithms that differ only in specific steps
 * - For implementing data processing pipelines with common structure but different operations
 * - In framework development where you want to provide extension points
 * - When building parsers that share common parsing logic but handle different formats
 * - For implementing various authentication mechanisms with common flow
 * - In testing frameworks where test execution follows a standard pattern
 * - For building report generators with common structure but different content
 * - When implementing game AI with shared decision-making process but different behaviors
 * - For building installation wizards with standard steps but platform-specific actions
 * - In web frameworks for request processing pipelines
 * 
 * SCENARIOS NOT TO USE:
 * - When algorithms are completely different and don't share common structure
 * - If the algorithm structure is likely to change frequently (violates stable abstraction)
 * - When you need multiple inheritance or composition is more appropriate
 * - If the template method becomes too complex with too many abstract methods
 * - When performance is critical and virtual method calls add unacceptable overhead
 * - If subclasses need to override the algorithm structure itself
 * - When the Liskov Substitution Principle would be violated
 * - If strategy pattern or composition would be more flexible for the use case
 */

namespace TemplateMethodPattern
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== Template Method Design Pattern Examples ===\n");

            // Example 1: Data Processing Pipeline
            DataProcessingExample();

            // Example 2: Authentication Framework
            AuthenticationExample();

            // Example 3: Report Generation System
            ReportGenerationExample();

            // Example 4: Game AI Decision Making
            GameAIExample();

            // Example 5: Thread-Safe Template Processing
            await ThreadSafeTemplateExample();

            // Example 6: File Processing Templates
            FileProcessingExample();

            // Example 7: Async Template Methods
            await AsyncTemplateExample();

            // Example 8: Testing Framework Template
            TestingFrameworkExample();

            // Example 9: Installation Wizard Template
            InstallationWizardExample();

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        #region Example 1: Data Processing Pipeline

        static void DataProcessingExample()
        {
            Console.WriteLine("1. Data Processing Pipeline:");
            Console.WriteLine("=============================");

            var processors = new DataProcessor[]
            {
                new CsvDataProcessor(),
                new JsonDataProcessor(),
                new XmlDataProcessor()
            };

            var sampleData = "id,name,value\n1,John,100\n2,Jane,200";

            foreach (var processor in processors)
            {
                Console.WriteLine($"\nProcessing with {processor.GetType().Name}:");
                var result = processor.ProcessData(sampleData);
                Console.WriteLine($"Result: {result.RecordsProcessed} records, Status: {result.Status}");
            }

            Console.WriteLine();
        }

        #endregion

        #region Example 2: Authentication Framework

        static void AuthenticationExample()
        {
            Console.WriteLine("2. Authentication Framework:");
            Console.WriteLine("=============================");

            var authMethods = new AuthenticationMethod[]
            {
                new PasswordAuthentication(),
                new TwoFactorAuthentication(),
                new BiometricAuthentication(),
                new OAuthAuthentication()
            };

            var credentials = new Dictionary<string, object>
            {
                ["username"] = "john.doe",
                ["password"] = "securepass123",
                ["token"] = "2FA123456",
                ["fingerprint"] = "bio_data_hash"
            };

            foreach (var method in authMethods)
            {
                Console.WriteLine($"\nTesting {method.GetType().Name}:");
                var result = method.Authenticate(credentials);
                Console.WriteLine($"Result: {(result.IsSuccessful ? "Success" : "Failed")} - {result.Message}");
            }

            Console.WriteLine();
        }

        #endregion

        #region Example 3: Report Generation System

        static void ReportGenerationExample()
        {
            Console.WriteLine("3. Report Generation System:");
            Console.WriteLine("=============================");

            var generators = new ReportGenerator[]
            {
                new SalesReportGenerator(),
                new InventoryReportGenerator(),
                new PerformanceReportGenerator()
            };

            var reportData = new
            {
                StartDate = DateTime.Now.AddDays(-30),
                EndDate = DateTime.Now,
                Department = "Sales"
            };

            foreach (var generator in generators)
            {
                Console.WriteLine($"\nGenerating {generator.GetType().Name.Replace("Generator", "")}:");
                var report = generator.GenerateReport(reportData);
                Console.WriteLine($"Title: {report.Title}");
                Console.WriteLine($"Pages: {report.PageCount}, Generated: {report.GeneratedAt:yyyy-MM-dd HH:mm}");
            }

            Console.WriteLine();
        }

        #endregion

        #region Example 4: Game AI Decision Making

        static void GameAIExample()
        {
            Console.WriteLine("4. Game AI Decision Making:");
            Console.WriteLine("============================");

            var aiControllers = new AIController[]
            {
                new WarriorAI(),
                new MageAI(),
                new ArcherAI(),
                new HealerAI()
            };

            var gameState = new GameState
            {
                PlayerHealth = 75,
                EnemyHealth = 60,
                PlayerMana = 50,
                DistanceToEnemy = 15
            };

            foreach (var ai in aiControllers)
            {
                Console.WriteLine($"\n{ai.GetType().Name} decision process:");
                var action = ai.DecideAction(gameState);
                Console.WriteLine($"Action: {action.Type} - {action.Description}");
                Console.WriteLine($"Priority: {action.Priority}, Cost: {action.Cost}");
            }

            Console.WriteLine();
        }

        #endregion

        #region Example 5: Thread-Safe Template Processing

        static async Task ThreadSafeTemplateExample()
        {
            Console.WriteLine("5. Thread-Safe Template Processing:");
            Console.WriteLine("====================================");

            var processor = new ThreadSafeProcessor();
            var tasks = new List<Task>();

            // Create multiple tasks that process data concurrently
            for (int i = 1; i <= 5; i++)
            {
                int taskId = i;
                tasks.Add(Task.Run(async () =>
                {
                    ThreadSafeHandler handler = taskId % 2 == 0
                        ? new EvenNumberHandler()
                        : new OddNumberHandler();

                    for (int j = 1; j <= 3; j++)
                    {
                        var data = new ProcessingData { Id = taskId * 10 + j, Value = taskId * j };
                        var result = await processor.ProcessAsync(handler, data);
                        Console.WriteLine($"[Task {taskId}] {handler.GetType().Name}: {result}");
                        await Task.Delay(100);
                    }
                }));
            }

            await Task.WhenAll(tasks);
            Console.WriteLine();
        }

        #endregion

        #region Example 6: File Processing Templates

        static void FileProcessingExample()
        {
            Console.WriteLine("6. File Processing Templates:");
            Console.WriteLine("==============================");

            var processors = new FileProcessor[]
            {
                new ImageFileProcessor(),
                new DocumentFileProcessor(),
                new VideoFileProcessor()
            };

            var files = new[]
            {
                new FileInfo { Name = "photo.jpg", Size = 2048000, Type = "image" },
                new FileInfo { Name = "document.pdf", Size = 512000, Type = "document" },
                new FileInfo { Name = "video.mp4", Size = 104857600, Type = "video" }
            };

            foreach (var file in files)
            {
                var processor = processors.FirstOrDefault(p => p.CanProcess(file.Type));
                if (processor != null)
                {
                    Console.WriteLine($"\nProcessing {file.Name} with {processor.GetType().Name}:");
                    var result = processor.ProcessFile(file);
                    Console.WriteLine($"Status: {result.Status}, Output: {result.OutputPath}");
                    Console.WriteLine($"Processing time: {result.ProcessingTime}ms");
                }
            }

            Console.WriteLine();
        }

        #endregion

        #region Example 7: Async Template Methods

        static async Task AsyncTemplateExample()
        {
            Console.WriteLine("7. Async Template Methods:");
            Console.WriteLine("===========================");

            var downloaders = new AsyncDownloader[]
            {
                new HttpDownloader(),
                new FtpDownloader(),
                new TorrentDownloader()
            };

            var requests = new[]
            {
                new DownloadRequest { Url = "https://example.com/file1.zip", Destination = "downloads/" },
                new DownloadRequest { Url = "ftp://files.example.com/file2.tar", Destination = "downloads/" },
                new DownloadRequest { Url = "magnet:?xt=urn:btih:example", Destination = "downloads/" }
            };

            for (int i = 0; i < requests.Length; i++)
            {
                var downloader = downloaders[i];
                var request = requests[i];

                Console.WriteLine($"\n{downloader.GetType().Name} downloading:");
                try
                {
                    var result = await downloader.DownloadAsync(request);
                    Console.WriteLine($"Status: {result.Status}, Downloaded: {result.BytesDownloaded} bytes");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            Console.WriteLine();
        }

        #endregion

        #region Example 8: Testing Framework Template

        static void TestingFrameworkExample()
        {
            Console.WriteLine("8. Testing Framework Template:");
            Console.WriteLine("===============================");

            var testSuites = new TestSuite[]
            {
                new UnitTestSuite(),
                new IntegrationTestSuite(),
                new PerformanceTestSuite()
            };

            foreach (var suite in testSuites)
            {
                Console.WriteLine($"\nRunning {suite.GetType().Name}:");
                var result = suite.RunTests();
                Console.WriteLine($"Tests: {result.TotalTests}, Passed: {result.PassedTests}, Failed: {result.FailedTests}");
                Console.WriteLine($"Execution time: {result.ExecutionTime}ms");
            }

            Console.WriteLine();
        }

        #endregion

        #region Example 9: Installation Wizard Template

        static void InstallationWizardExample()
        {
            Console.WriteLine("9. Installation Wizard Template:");
            Console.WriteLine("=================================");

            var installers = new InstallationWizard[]
            {
                new WindowsInstaller(),
                new LinuxInstaller(),
                new MacInstaller()
            };

            var installConfig = new InstallationConfig
            {
                TargetDirectory = "/opt/myapp",
                CreateShortcuts = true,
                RegisterService = true
            };

            foreach (var installer in installers)
            {
                Console.WriteLine($"\n{installer.GetType().Name} installation:");
                var result = installer.Install(installConfig);
                Console.WriteLine($"Status: {(result.Success ? "Success" : "Failed")}");
                Console.WriteLine($"Message: {result.Message}");
                if (result.Success)
                {
                    Console.WriteLine($"Installed to: {result.InstallPath}");
                }
            }

            Console.WriteLine();
        }

        #endregion
    }

    #region Core Template Method Classes

    // Abstract base class defining the template method
    // MEMORY ALLOCATION: Base class overhead is minimal, virtual method table per derived class
    public abstract class TemplateMethodBase<TInput, TOutput>
    {
        // Template method - defines the algorithm skeleton
        // This method should be sealed or final to prevent overriding the algorithm structure
        public TOutput Execute(TInput input)
        {
            try
            {
                PreProcess(input);
                ValidateInput(input);
                var result = ProcessCore(input);
                PostProcess(result);
                return result;
            }
            catch (Exception ex)
            {
                HandleError(ex);
                throw;
            }
        }

        // Hook methods - can be overridden but have default implementations
        protected virtual void PreProcess(TInput input) { }
        protected virtual void PostProcess(TOutput output) { }
        protected virtual void HandleError(Exception exception) { }

        // Abstract methods - must be implemented by subclasses
        protected abstract void ValidateInput(TInput input);
        protected abstract TOutput ProcessCore(TInput input);
    }

    #endregion

    #region Example 1: Data Processing Classes

    public class DataProcessingResult
    {
        public int RecordsProcessed { get; set; }
        public string Status { get; set; } = string.Empty;
        public TimeSpan ProcessingTime { get; set; }
    }

    public abstract class DataProcessor
    {
        // Template method defining the data processing algorithm
        public DataProcessingResult ProcessData(string rawData)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                Console.WriteLine("  Starting data processing...");

                // Step 1: Validate input
                ValidateData(rawData);

                // Step 2: Parse data (varies by format)
                var parsedData = ParseData(rawData);

                // Step 3: Transform data (common preprocessing)
                var transformedData = TransformData(parsedData);

                // Step 4: Process data (format-specific logic)
                var processedCount = ProcessRecords(transformedData);

                // Step 5: Cleanup (common)
                Cleanup();

                stopwatch.Stop();

                return new DataProcessingResult
                {
                    RecordsProcessed = processedCount,
                    Status = "Success",
                    ProcessingTime = stopwatch.Elapsed
                };
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                LogError(ex);
                return new DataProcessingResult
                {
                    RecordsProcessed = 0,
                    Status = $"Error: {ex.Message}",
                    ProcessingTime = stopwatch.Elapsed
                };
            }
        }

        // Hook methods with default implementations
        protected virtual void ValidateData(string data)
        {
            if (string.IsNullOrWhiteSpace(data))
                throw new ArgumentException("Data cannot be null or empty");
        }

        protected virtual string[] TransformData(string[] data)
        {
            // Common transformation: trim whitespace
            return data.Select(line => line.Trim()).Where(line => !string.IsNullOrEmpty(line)).ToArray();
        }

        protected virtual void Cleanup()
        {
            Console.WriteLine("  Cleaning up resources...");
        }

        protected virtual void LogError(Exception ex)
        {
            Console.WriteLine($"  Error occurred: {ex.Message}");
        }

        // Abstract methods that must be implemented
        protected abstract string[] ParseData(string rawData);
        protected abstract int ProcessRecords(string[] data);
    }

    public class CsvDataProcessor : DataProcessor
    {
        protected override string[] ParseData(string rawData)
        {
            Console.WriteLine("  Parsing CSV data...");
            return rawData.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        }

        protected override int ProcessRecords(string[] data)
        {
            Console.WriteLine("  Processing CSV records...");
            // Skip header row
            return Math.Max(0, data.Length - 1);
        }
    }

    public class JsonDataProcessor : DataProcessor
    {
        protected override string[] ParseData(string rawData)
        {
            Console.WriteLine("  Parsing JSON data...");
            // Simulate JSON parsing
            return new[] { rawData };
        }

        protected override int ProcessRecords(string[] data)
        {
            Console.WriteLine("  Processing JSON objects...");
            // Simulate counting JSON objects
            return data.Length;
        }

        protected override void ValidateData(string data)
        {
            base.ValidateData(data);
            // Additional JSON-specific validation
            if (!data.TrimStart().StartsWith("{") && !data.TrimStart().StartsWith("["))
                throw new ArgumentException("Invalid JSON format");
        }
    }

    public class XmlDataProcessor : DataProcessor
    {
        protected override string[] ParseData(string rawData)
        {
            Console.WriteLine("  Parsing XML data...");
            // Simulate XML parsing
            return rawData.Split('<', StringSplitOptions.RemoveEmptyEntries);
        }

        protected override int ProcessRecords(string[] data)
        {
            Console.WriteLine("  Processing XML elements...");
            // Count XML elements
            return data.Count(element => !element.StartsWith("/") && !element.StartsWith("?"));
        }
    }

    #endregion

    #region Example 2: Authentication Framework Classes

    public class AuthenticationResult
    {
        public bool IsSuccessful { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public TimeSpan AuthenticationTime { get; set; }
    }

    public abstract class AuthenticationMethod
    {
        // Template method for authentication process
        public AuthenticationResult Authenticate(Dictionary<string, object> credentials)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                Console.WriteLine("  Initializing authentication...");

                // Step 1: Initialize authentication context
                InitializeAuthentication();

                // Step 2: Validate credentials format
                ValidateCredentials(credentials);

                // Step 3: Perform authentication (method-specific)
                var isValid = PerformAuthentication(credentials);

                // Step 4: Generate token if successful
                string token = string.Empty;
                if (isValid)
                {
                    token = GenerateAuthenticationToken(credentials);
                }

                // Step 5: Log authentication attempt
                LogAuthenticationAttempt(credentials, isValid);

                // Step 6: Cleanup
                CleanupAuthentication();

                stopwatch.Stop();

                return new AuthenticationResult
                {
                    IsSuccessful = isValid,
                    Message = isValid ? "Authentication successful" : "Authentication failed",
                    Token = token,
                    AuthenticationTime = stopwatch.Elapsed
                };
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                return new AuthenticationResult
                {
                    IsSuccessful = false,
                    Message = $"Authentication error: {ex.Message}",
                    AuthenticationTime = stopwatch.Elapsed
                };
            }
        }

        // Hook methods
        protected virtual void InitializeAuthentication() { }
        protected virtual void CleanupAuthentication() { }

        protected virtual void LogAuthenticationAttempt(Dictionary<string, object> credentials, bool success)
        {
            var username = credentials.TryGetValue("username", out var user) ? user.ToString() : "unknown";
            Console.WriteLine($"  Authentication attempt for {username}: {(success ? "SUCCESS" : "FAILED")}");
        }

        protected virtual string GenerateAuthenticationToken(Dictionary<string, object> credentials)
        {
            return $"TOKEN_{Guid.NewGuid().ToString()[..8]}";
        }

        // Abstract methods
        protected abstract void ValidateCredentials(Dictionary<string, object> credentials);
        protected abstract bool PerformAuthentication(Dictionary<string, object> credentials);
    }

    public class PasswordAuthentication : AuthenticationMethod
    {
        protected override void ValidateCredentials(Dictionary<string, object> credentials)
        {
            if (!credentials.ContainsKey("username") || !credentials.ContainsKey("password"))
                throw new ArgumentException("Username and password are required");
        }

        protected override bool PerformAuthentication(Dictionary<string, object> credentials)
        {
            Console.WriteLine("  Verifying username and password...");
            // Simulate password verification
            var password = credentials["password"].ToString();
            return password?.Length >= 8;
        }
    }

    public class TwoFactorAuthentication : AuthenticationMethod
    {
        protected override void ValidateCredentials(Dictionary<string, object> credentials)
        {
            if (!credentials.ContainsKey("username") || !credentials.ContainsKey("password") || !credentials.ContainsKey("token"))
                throw new ArgumentException("Username, password, and 2FA token are required");
        }

        protected override bool PerformAuthentication(Dictionary<string, object> credentials)
        {
            Console.WriteLine("  Verifying credentials and 2FA token...");
            // Simulate 2FA verification
            var token = credentials["token"].ToString();
            return token?.StartsWith("2FA") == true;
        }
    }

    public class BiometricAuthentication : AuthenticationMethod
    {
        protected override void ValidateCredentials(Dictionary<string, object> credentials)
        {
            if (!credentials.ContainsKey("fingerprint") && !credentials.ContainsKey("faceprint"))
                throw new ArgumentException("Biometric data is required");
        }

        protected override bool PerformAuthentication(Dictionary<string, object> credentials)
        {
            Console.WriteLine("  Processing biometric data...");
            // Simulate biometric verification
            return credentials.ContainsKey("fingerprint");
        }
    }

    public class OAuthAuthentication : AuthenticationMethod
    {
        protected override void ValidateCredentials(Dictionary<string, object> credentials)
        {
            if (!credentials.ContainsKey("username"))
                throw new ArgumentException("Username is required for OAuth");
        }

        protected override bool PerformAuthentication(Dictionary<string, object> credentials)
        {
            Console.WriteLine("  Redirecting to OAuth provider...");
            // Simulate OAuth flow
            return true; // Assume OAuth succeeds
        }
    }

    #endregion

    #region Example 3: Report Generation Classes

    public class Report
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public int PageCount { get; set; }
        public DateTime GeneratedAt { get; set; }
    }

    public abstract class ReportGenerator
    {
        // Template method for report generation
        public Report GenerateReport(object data)
        {
            Console.WriteLine("  Starting report generation...");

            // Step 1: Initialize report
            var report = InitializeReport();

            // Step 2: Gather data
            var reportData = GatherData(data);

            // Step 3: Generate content (varies by report type)
            report.Content = GenerateContent(reportData);

            // Step 4: Format report (common formatting)
            FormatReport(report);

            // Step 5: Add metadata
            AddMetadata(report);

            // Step 6: Finalize
            FinalizeReport(report);

            Console.WriteLine("  Report generation completed");
            return report;
        }

        // Hook methods
        protected virtual Report InitializeReport()
        {
            return new Report { GeneratedAt = DateTime.Now };
        }

        protected virtual void FormatReport(Report report)
        {
            // Common formatting
            report.PageCount = Math.Max(1, report.Content.Length / 1000);
        }

        protected virtual void AddMetadata(Report report)
        {
            // Add common metadata
            report.Content += $"\n\n--- Generated on {report.GeneratedAt:yyyy-MM-dd HH:mm} ---";
        }

        protected virtual void FinalizeReport(Report report) { }

        // Abstract methods
        protected abstract object GatherData(object inputData);
        protected abstract string GenerateContent(object data);
    }

    public class SalesReportGenerator : ReportGenerator
    {
        protected override object GatherData(object inputData)
        {
            Console.WriteLine("  Gathering sales data...");
            return new { TotalSales = 50000, ProductsSold = 150, TopProduct = "Widget A" };
        }

        protected override string GenerateContent(object data)
        {
            Console.WriteLine("  Generating sales report content...");
            dynamic salesData = data;
            return $"SALES REPORT\n" +
                   $"Total Sales: ${salesData.TotalSales}\n" +
                   $"Products Sold: {salesData.ProductsSold}\n" +
                   $"Top Product: {salesData.TopProduct}";
        }

        protected override Report InitializeReport()
        {
            var report = base.InitializeReport();
            report.Title = "Monthly Sales Report";
            return report;
        }
    }

    public class InventoryReportGenerator : ReportGenerator
    {
        protected override object GatherData(object inputData)
        {
            Console.WriteLine("  Gathering inventory data...");
            return new { TotalItems = 1250, LowStockItems = 15, OutOfStockItems = 3 };
        }

        protected override string GenerateContent(object data)
        {
            Console.WriteLine("  Generating inventory report content...");
            dynamic inventoryData = data;
            return $"INVENTORY REPORT\n" +
                   $"Total Items: {inventoryData.TotalItems}\n" +
                   $"Low Stock Items: {inventoryData.LowStockItems}\n" +
                   $"Out of Stock Items: {inventoryData.OutOfStockItems}";
        }

        protected override Report InitializeReport()
        {
            var report = base.InitializeReport();
            report.Title = "Inventory Status Report";
            return report;
        }
    }

    public class PerformanceReportGenerator : ReportGenerator
    {
        protected override object GatherData(object inputData)
        {
            Console.WriteLine("  Gathering performance data...");
            return new { AvgResponseTime = 250, ErrorRate = 0.02, Uptime = 99.9 };
        }

        protected override string GenerateContent(object data)
        {
            Console.WriteLine("  Generating performance report content...");
            dynamic perfData = data;
            return $"PERFORMANCE REPORT\n" +
                   $"Average Response Time: {perfData.AvgResponseTime}ms\n" +
                   $"Error Rate: {perfData.ErrorRate:P2}\n" +
                   $"Uptime: {perfData.Uptime}%";
        }

        protected override Report InitializeReport()
        {
            var report = base.InitializeReport();
            report.Title = "System Performance Report";
            return report;
        }
    }

    #endregion

    #region Example 4: Game AI Classes

    public class GameState
    {
        public int PlayerHealth { get; set; }
        public int EnemyHealth { get; set; }
        public int PlayerMana { get; set; }
        public int DistanceToEnemy { get; set; }
    }

    public class AIAction
    {
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Priority { get; set; }
        public int Cost { get; set; }
    }

    public abstract class AIController
    {
        // Template method for AI decision making
        public AIAction DecideAction(GameState gameState)
        {
            Console.WriteLine("  Analyzing game state...");

            // Step 1: Assess situation
            var situation = AssessSituation(gameState);

            // Step 2: Evaluate options (character-specific)
            var options = EvaluateOptions(gameState, situation);

            // Step 3: Apply character-specific strategy
            var preferredAction = SelectAction(options, gameState);

            // Step 4: Validate action (common validation)
            if (!ValidateAction(preferredAction, gameState))
            {
                preferredAction = GetDefaultAction(gameState);
            }

            // Step 5: Log decision
            LogDecision(preferredAction, situation);

            return preferredAction;
        }

        // Hook methods
        protected virtual string AssessSituation(GameState gameState)
        {
            if (gameState.PlayerHealth < 30) return "Critical";
            if (gameState.EnemyHealth < 30) return "Advantage";
            return "Normal";
        }

        protected virtual bool ValidateAction(AIAction action, GameState gameState)
        {
            return action.Cost <= gameState.PlayerMana;
        }

        protected virtual AIAction GetDefaultAction(GameState gameState)
        {
            return new AIAction { Type = "Wait", Description = "No valid action available", Priority = 1, Cost = 0 };
        }

        protected virtual void LogDecision(AIAction action, string situation)
        {
            Console.WriteLine($"  Situation: {situation}, Decision: {action.Type}");
        }

        // Abstract methods
        protected abstract List<AIAction> EvaluateOptions(GameState gameState, string situation);
        protected abstract AIAction SelectAction(List<AIAction> options, GameState gameState);
    }

    public class WarriorAI : AIController
    {
        protected override List<AIAction> EvaluateOptions(GameState gameState, string situation)
        {
            var options = new List<AIAction>();

            if (gameState.DistanceToEnemy <= 5)
            {
                options.Add(new AIAction { Type = "Sword Attack", Description = "Powerful melee attack", Priority = 8, Cost = 10 });
                options.Add(new AIAction { Type = "Shield Block", Description = "Defensive stance", Priority = 6, Cost = 5 });
            }
            else
            {
                options.Add(new AIAction { Type = "Charge", Description = "Close distance quickly", Priority = 7, Cost = 15 });
            }

            if (gameState.PlayerHealth < 50)
            {
                options.Add(new AIAction { Type = "Health Potion", Description = "Restore health", Priority = 9, Cost = 0 });
            }

            return options;
        }

        protected override AIAction SelectAction(List<AIAction> options, GameState gameState)
        {
            // Warriors prefer high-priority aggressive actions
            return options.OrderByDescending(a => a.Priority).First();
        }
    }

    public class MageAI : AIController
    {
        protected override List<AIAction> EvaluateOptions(GameState gameState, string situation)
        {
            var options = new List<AIAction>();

            if (gameState.PlayerMana >= 30)
            {
                options.Add(new AIAction { Type = "Fireball", Description = "Ranged magic attack", Priority = 8, Cost = 30 });
            }

            if (gameState.PlayerMana >= 20 && gameState.DistanceToEnemy <= 10)
            {
                options.Add(new AIAction { Type = "Ice Shield", Description = "Magical protection", Priority = 7, Cost = 20 });
            }

            if (gameState.PlayerMana >= 50)
            {
                options.Add(new AIAction { Type = "Lightning Bolt", Description = "High damage spell", Priority = 9, Cost = 50 });
            }

            options.Add(new AIAction { Type = "Staff Hit", Description = "Weak melee attack", Priority = 3, Cost = 5 });

            return options;
        }

        protected override AIAction SelectAction(List<AIAction> options, GameState gameState)
        {
            // Mages prefer efficient mana usage
            return options.Where(a => a.Cost <= gameState.PlayerMana)
                         .OrderByDescending(a => (double)a.Priority / Math.Max(1, a.Cost))
                         .First();
        }
    }

    public class ArcherAI : AIController
    {
        protected override List<AIAction> EvaluateOptions(GameState gameState, string situation)
        {
            var options = new List<AIAction>();

            if (gameState.DistanceToEnemy >= 10)
            {
                options.Add(new AIAction { Type = "Bow Shot", Description = "Ranged arrow attack", Priority = 8, Cost = 10 });
                options.Add(new AIAction { Type = "Power Shot", Description = "Charged arrow attack", Priority = 9, Cost = 25 });
            }
            else
            {
                options.Add(new AIAction { Type = "Retreat", Description = "Create distance", Priority = 7, Cost = 5 });
                options.Add(new AIAction { Type = "Knife Strike", Description = "Close combat attack", Priority = 5, Cost = 8 });
            }

            return options;
        }

        protected override AIAction SelectAction(List<AIAction> options, GameState gameState)
        {
            // Archers prefer maintaining distance
            if (gameState.DistanceToEnemy < 10)
            {
                return options.FirstOrDefault(a => a.Type == "Retreat") ?? options.First();
            }
            return options.OrderByDescending(a => a.Priority).First();
        }
    }

    public class HealerAI : AIController
    {
        protected override List<AIAction> EvaluateOptions(GameState gameState, string situation)
        {
            var options = new List<AIAction>();

            if (gameState.PlayerHealth < 70 && gameState.PlayerMana >= 25)
            {
                options.Add(new AIAction { Type = "Heal", Description = "Restore health points", Priority = 9, Cost = 25 });
            }

            if (gameState.PlayerMana >= 40)
            {
                options.Add(new AIAction { Type = "Barrier", Description = "Protective spell", Priority = 7, Cost = 40 });
            }

            options.Add(new AIAction { Type = "Staff Strike", Description = "Basic attack", Priority = 4, Cost = 5 });

            return options;
        }

        protected override AIAction SelectAction(List<AIAction> options, GameState gameState)
        {
            // Healers prioritize survival and support
            return options.OrderByDescending(a => a.Priority).First();
        }
    }

    #endregion

    #region Example 5: Thread-Safe Template Classes

    // MULTITHREAD ASPECTS: Thread-safe template method implementation
    public class ProcessingData
    {
        public int Id { get; set; }
        public int Value { get; set; }
    }

    public abstract class ThreadSafeHandler
    {
        private static readonly object _lockObject = new object();
        private static int _processedCount = 0;

        // Template method with thread safety
        public async Task<string> ProcessAsync(ProcessingData data)
        {
            try
            {
                // Step 1: Validate (thread-safe)
                await ValidateAsync(data);

                // Step 2: Pre-process (may need synchronization)
                await PreProcessAsync(data);

                // Step 3: Core processing (handler-specific)
                var result = await ProcessCoreAsync(data);

                // Step 4: Post-process (thread-safe increment)
                await PostProcessAsync(result);

                // Step 5: Update global counter (thread-safe)
                lock (_lockObject)
                {
                    _processedCount++;
                    result += $" (Total processed: {_processedCount})";
                }

                return result;
            }
            catch (Exception ex)
            {
                return $"Error processing {data.Id}: {ex.Message}";
            }
        }

        // Hook methods
        protected virtual async Task ValidateAsync(ProcessingData data)
        {
            await Task.Delay(10); // Simulate validation
            if (data.Id <= 0) throw new ArgumentException("Invalid ID");
        }

        protected virtual async Task PreProcessAsync(ProcessingData data)
        {
            await Task.Delay(50); // Simulate preprocessing
        }

        protected virtual async Task PostProcessAsync(string result)
        {
            await Task.Delay(20); // Simulate post-processing
        }

        // Abstract method
        protected abstract Task<string> ProcessCoreAsync(ProcessingData data);
    }

    public class EvenNumberHandler : ThreadSafeHandler
    {
        protected override async Task<string> ProcessCoreAsync(ProcessingData data)
        {
            await Task.Delay(100); // Simulate processing
            var result = data.Value * 2;
            return $"Even handler: {data.Id} -> {result}";
        }
    }

    public class OddNumberHandler : ThreadSafeHandler
    {
        protected override async Task<string> ProcessCoreAsync(ProcessingData data)
        {
            await Task.Delay(150); // Simulate processing
            var result = data.Value * 3;
            return $"Odd handler: {data.Id} -> {result}";
        }
    }

    public class ThreadSafeProcessor
    {
        public async Task<string> ProcessAsync(ThreadSafeHandler handler, ProcessingData data)
        {
            return await handler.ProcessAsync(data);
        }
    }

    #endregion

    #region Example 6: File Processing Classes

    public class FileInfo
    {
        public string Name { get; set; } = string.Empty;
        public long Size { get; set; }
        public string Type { get; set; } = string.Empty;
    }

    public class FileProcessingResult
    {
        public string Status { get; set; } = string.Empty;
        public string OutputPath { get; set; } = string.Empty;
        public long ProcessingTime { get; set; }
    }

    public abstract class FileProcessor
    {
        // Template method for file processing
        public FileProcessingResult ProcessFile(FileInfo fileInfo)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                Console.WriteLine($"  Processing {fileInfo.Name}...");

                // Step 1: Validate file
                ValidateFile(fileInfo);

                // Step 2: Create output directory
                var outputPath = CreateOutputPath(fileInfo);

                // Step 3: Process file (format-specific)
                var processedPath = ProcessFileCore(fileInfo, outputPath);

                // Step 4: Optimize output (if needed)
                OptimizeOutput(processedPath);

                // Step 5: Update metadata
                UpdateMetadata(processedPath, fileInfo);

                stopwatch.Stop();

                return new FileProcessingResult
                {
                    Status = "Success",
                    OutputPath = processedPath,
                    ProcessingTime = stopwatch.ElapsedMilliseconds
                };
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                return new FileProcessingResult
                {
                    Status = $"Error: {ex.Message}",
                    ProcessingTime = stopwatch.ElapsedMilliseconds
                };
            }
        }

        // Hook methods
        protected virtual void ValidateFile(FileInfo fileInfo)
        {
            if (fileInfo.Size > 100_000_000) // 100MB
                throw new ArgumentException("File too large");
        }

        protected virtual string CreateOutputPath(FileInfo fileInfo)
        {
            return $"output/{Path.GetFileNameWithoutExtension(fileInfo.Name)}_processed";
        }

        protected virtual void OptimizeOutput(string outputPath) { }

        protected virtual void UpdateMetadata(string outputPath, FileInfo originalFile)
        {
            Console.WriteLine($"  Metadata updated for {outputPath}");
        }

        // Abstract methods
        public abstract bool CanProcess(string fileType);
        protected abstract string ProcessFileCore(FileInfo fileInfo, string outputPath);
    }

    public class ImageFileProcessor : FileProcessor
    {
        public override bool CanProcess(string fileType)
        {
            return fileType.Equals("image", StringComparison.OrdinalIgnoreCase);
        }

        protected override string ProcessFileCore(FileInfo fileInfo, string outputPath)
        {
            Console.WriteLine("  Resizing and optimizing image...");
            // Simulate image processing
            Thread.Sleep(200);
            return $"{outputPath}.jpg";
        }

        protected override void OptimizeOutput(string outputPath)
        {
            Console.WriteLine("  Compressing image...");
        }
    }

    public class DocumentFileProcessor : FileProcessor
    {
        public override bool CanProcess(string fileType)
        {
            return fileType.Equals("document", StringComparison.OrdinalIgnoreCase);
        }

        protected override string ProcessFileCore(FileInfo fileInfo, string outputPath)
        {
            Console.WriteLine("  Converting document format...");
            // Simulate document processing
            Thread.Sleep(150);
            return $"{outputPath}.pdf";
        }

        protected override void OptimizeOutput(string outputPath)
        {
            Console.WriteLine("  Optimizing document size...");
        }
    }

    public class VideoFileProcessor : FileProcessor
    {
        public override bool CanProcess(string fileType)
        {
            return fileType.Equals("video", StringComparison.OrdinalIgnoreCase);
        }

        protected override string ProcessFileCore(FileInfo fileInfo, string outputPath)
        {
            Console.WriteLine("  Transcoding video...");
            // Simulate video processing
            Thread.Sleep(500);
            return $"{outputPath}.mp4";
        }

        protected override void OptimizeOutput(string outputPath)
        {
            Console.WriteLine("  Compressing video...");
        }

        protected override void ValidateFile(FileInfo fileInfo)
        {
            base.ValidateFile(fileInfo);
            if (fileInfo.Size > 1_000_000_000) // 1GB for video
                throw new ArgumentException("Video file too large");
        }
    }

    #endregion

    #region Example 7: Async Template Classes

    public class DownloadRequest
    {
        public string Url { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
    }

    public class DownloadResult
    {
        public string Status { get; set; } = string.Empty;
        public long BytesDownloaded { get; set; }
        public TimeSpan Duration { get; set; }
    }

    public abstract class AsyncDownloader
    {
        // Async template method
        public async Task<DownloadResult> DownloadAsync(DownloadRequest request)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                // Step 1: Validate request
                await ValidateRequestAsync(request);

                // Step 2: Initialize connection
                await InitializeConnectionAsync(request);

                // Step 3: Download (protocol-specific)
                var bytesDownloaded = await PerformDownloadAsync(request);

                // Step 4: Verify download
                await VerifyDownloadAsync(request, bytesDownloaded);

                // Step 5: Cleanup
                await CleanupAsync();

                stopwatch.Stop();

                return new DownloadResult
                {
                    Status = "Success",
                    BytesDownloaded = bytesDownloaded,
                    Duration = stopwatch.Elapsed
                };
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                await HandleErrorAsync(ex);
                return new DownloadResult
                {
                    Status = $"Error: {ex.Message}",
                    Duration = stopwatch.Elapsed
                };
            }
        }

        // Hook methods
        protected virtual async Task ValidateRequestAsync(DownloadRequest request)
        {
            await Task.Delay(10);
            if (string.IsNullOrEmpty(request.Url))
                throw new ArgumentException("URL is required");
        }

        protected virtual async Task VerifyDownloadAsync(DownloadRequest request, long bytesDownloaded)
        {
            await Task.Delay(50);
            Console.WriteLine($"  Verified {bytesDownloaded} bytes downloaded");
        }

        protected virtual async Task CleanupAsync()
        {
            await Task.Delay(20);
            Console.WriteLine("  Cleanup completed");
        }

        protected virtual async Task HandleErrorAsync(Exception ex)
        {
            await Task.Delay(10);
            Console.WriteLine($"  Error handled: {ex.Message}");
        }

        // Abstract methods
        protected abstract Task InitializeConnectionAsync(DownloadRequest request);
        protected abstract Task<long> PerformDownloadAsync(DownloadRequest request);
    }

    public class HttpDownloader : AsyncDownloader
    {
        protected override async Task InitializeConnectionAsync(DownloadRequest request)
        {
            await Task.Delay(100);
            Console.WriteLine("  HTTP connection established");
        }

        protected override async Task<long> PerformDownloadAsync(DownloadRequest request)
        {
            await Task.Delay(800);
            Console.WriteLine("  HTTP download completed");
            return 1024000; // 1MB
        }
    }

    public class FtpDownloader : AsyncDownloader
    {
        protected override async Task InitializeConnectionAsync(DownloadRequest request)
        {
            await Task.Delay(200);
            Console.WriteLine("  FTP connection established");
        }

        protected override async Task<long> PerformDownloadAsync(DownloadRequest request)
        {
            await Task.Delay(1200);
            Console.WriteLine("  FTP download completed");
            return 2048000; // 2MB
        }
    }

    public class TorrentDownloader : AsyncDownloader
    {
        protected override async Task InitializeConnectionAsync(DownloadRequest request)
        {
            await Task.Delay(300);
            Console.WriteLine("  Torrent peers connected");
        }

        protected override async Task<long> PerformDownloadAsync(DownloadRequest request)
        {
            await Task.Delay(1500);
            Console.WriteLine("  Torrent download completed");
            return 5120000; // 5MB
        }
    }

    #endregion

    #region Example 8: Testing Framework Classes

    public class TestResult
    {
        public int TotalTests { get; set; }
        public int PassedTests { get; set; }
        public int FailedTests { get; set; }
        public long ExecutionTime { get; set; }
    }

    public abstract class TestSuite
    {
        // Template method for test execution
        public TestResult RunTests()
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                Console.WriteLine("  Setting up test environment...");

                // Step 1: Setup test environment
                SetupTestEnvironment();

                // Step 2: Discover tests
                var tests = DiscoverTests();

                // Step 3: Run tests (suite-specific)
                var results = ExecuteTests(tests);

                // Step 4: Collect results
                var summary = CollectResults(results);

                // Step 5: Cleanup
                CleanupTestEnvironment();

                stopwatch.Stop();
                summary.ExecutionTime = stopwatch.ElapsedMilliseconds;

                return summary;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                Console.WriteLine($"  Test execution error: {ex.Message}");
                return new TestResult { ExecutionTime = stopwatch.ElapsedMilliseconds };
            }
        }

        // Hook methods
        protected virtual void SetupTestEnvironment()
        {
            Console.WriteLine("  Test environment setup completed");
        }

        protected virtual void CleanupTestEnvironment()
        {
            Console.WriteLine("  Test environment cleanup completed");
        }

        protected virtual TestResult CollectResults(List<bool> results)
        {
            return new TestResult
            {
                TotalTests = results.Count,
                PassedTests = results.Count(r => r),
                FailedTests = results.Count(r => !r)
            };
        }

        // Abstract methods
        protected abstract List<string> DiscoverTests();
        protected abstract List<bool> ExecuteTests(List<string> tests);
    }

    public class UnitTestSuite : TestSuite
    {
        protected override List<string> DiscoverTests()
        {
            Console.WriteLine("  Discovering unit tests...");
            return new List<string> { "TestAdd", "TestSubtract", "TestMultiply", "TestDivide" };
        }

        protected override List<bool> ExecuteTests(List<string> tests)
        {
            Console.WriteLine("  Executing unit tests...");
            var results = new List<bool>();

            foreach (var test in tests)
            {
                // Simulate test execution
                Thread.Sleep(50);
                var passed = !test.Contains("Divide"); // Simulate divide test failing
                results.Add(passed);
                Console.WriteLine($"    {test}: {(passed ? "PASS" : "FAIL")}");
            }

            return results;
        }
    }

    public class IntegrationTestSuite : TestSuite
    {
        protected override List<string> DiscoverTests()
        {
            Console.WriteLine("  Discovering integration tests...");
            return new List<string> { "TestDatabaseConnection", "TestApiIntegration", "TestFileSystem" };
        }

        protected override List<bool> ExecuteTests(List<string> tests)
        {
            Console.WriteLine("  Executing integration tests...");
            var results = new List<bool>();

            foreach (var test in tests)
            {
                Thread.Sleep(200); // Integration tests take longer
                var passed = true; // All integration tests pass
                results.Add(passed);
                Console.WriteLine($"    {test}: {(passed ? "PASS" : "FAIL")}");
            }

            return results;
        }

        protected override void SetupTestEnvironment()
        {
            base.SetupTestEnvironment();
            Console.WriteLine("  Starting test database...");
        }
    }

    public class PerformanceTestSuite : TestSuite
    {
        protected override List<string> DiscoverTests()
        {
            Console.WriteLine("  Discovering performance tests...");
            return new List<string> { "TestLoadTime", "TestThroughput", "TestMemoryUsage" };
        }

        protected override List<bool> ExecuteTests(List<string> tests)
        {
            Console.WriteLine("  Executing performance tests...");
            var results = new List<bool>();

            foreach (var test in tests)
            {
                Thread.Sleep(300); // Performance tests take longest
                var passed = !test.Contains("Memory"); // Simulate memory test failing
                results.Add(passed);
                Console.WriteLine($"    {test}: {(passed ? "PASS" : "FAIL")}");
            }

            return results;
        }
    }

    #endregion

    #region Example 9: Installation Wizard Classes

    public class InstallationConfig
    {
        public string TargetDirectory { get; set; } = string.Empty;
        public bool CreateShortcuts { get; set; }
        public bool RegisterService { get; set; }
    }

    public class InstallationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string InstallPath { get; set; } = string.Empty;
    }

    public abstract class InstallationWizard
    {
        // Template method for installation process
        public InstallationResult Install(InstallationConfig config)
        {
            try
            {
                Console.WriteLine("  Starting installation...");

                // Step 1: Check prerequisites
                CheckPrerequisites();

                // Step 2: Validate configuration
                ValidateConfiguration(config);

                // Step 3: Create directories
                CreateDirectories(config);

                // Step 4: Copy files (platform-specific)
                CopyFiles(config);

                // Step 5: Configure system (platform-specific)
                ConfigureSystem(config);

                // Step 6: Register application
                if (config.RegisterService)
                {
                    RegisterService(config);
                }

                // Step 7: Create shortcuts
                if (config.CreateShortcuts)
                {
                    CreateShortcuts(config);
                }

                // Step 8: Finalize installation
                FinalizeInstallation(config);

                return new InstallationResult
                {
                    Success = true,
                    Message = "Installation completed successfully",
                    InstallPath = config.TargetDirectory
                };
            }
            catch (Exception ex)
            {
                return new InstallationResult
                {
                    Success = false,
                    Message = $"Installation failed: {ex.Message}"
                };
            }
        }

        // Hook methods
        protected virtual void CheckPrerequisites()
        {
            Console.WriteLine("  Checking prerequisites...");
        }

        protected virtual void ValidateConfiguration(InstallationConfig config)
        {
            if (string.IsNullOrEmpty(config.TargetDirectory))
                throw new ArgumentException("Target directory is required");
        }

        protected virtual void CreateDirectories(InstallationConfig config)
        {
            Console.WriteLine($"  Creating directories in {config.TargetDirectory}...");
        }

        protected virtual void CreateShortcuts(InstallationConfig config)
        {
            Console.WriteLine("  Creating shortcuts...");
        }

        protected virtual void FinalizeInstallation(InstallationConfig config)
        {
            Console.WriteLine("  Installation finalized");
        }

        // Abstract methods
        protected abstract void CopyFiles(InstallationConfig config);
        protected abstract void ConfigureSystem(InstallationConfig config);
        protected abstract void RegisterService(InstallationConfig config);
    }

    public class WindowsInstaller : InstallationWizard
    {
        protected override void CopyFiles(InstallationConfig config)
        {
            Console.WriteLine("  Copying Windows executable files...");
            Thread.Sleep(300);
        }

        protected override void ConfigureSystem(InstallationConfig config)
        {
            Console.WriteLine("  Updating Windows registry...");
            Thread.Sleep(150);
        }

        protected override void RegisterService(InstallationConfig config)
        {
            Console.WriteLine("  Registering Windows service...");
            Thread.Sleep(100);
        }

        protected override void CheckPrerequisites()
        {
            base.CheckPrerequisites();
            Console.WriteLine("  Checking .NET Framework version...");
        }
    }

    public class LinuxInstaller : InstallationWizard
    {
        protected override void CopyFiles(InstallationConfig config)
        {
            Console.WriteLine("  Copying Linux binary files...");
            Thread.Sleep(250);
        }

        protected override void ConfigureSystem(InstallationConfig config)
        {
            Console.WriteLine("  Updating system configuration files...");
            Thread.Sleep(120);
        }

        protected override void RegisterService(InstallationConfig config)
        {
            Console.WriteLine("  Creating systemd service...");
            Thread.Sleep(80);
        }

        protected override void CheckPrerequisites()
        {
            base.CheckPrerequisites();
            Console.WriteLine("  Checking Linux dependencies...");
        }
    }

    public class MacInstaller : InstallationWizard
    {
        protected override void CopyFiles(InstallationConfig config)
        {
            Console.WriteLine("  Copying macOS application bundle...");
            Thread.Sleep(200);
        }

        protected override void ConfigureSystem(InstallationConfig config)
        {
            Console.WriteLine("  Updating macOS preferences...");
            Thread.Sleep(100);
        }

        protected override void RegisterService(InstallationConfig config)
        {
            Console.WriteLine("  Creating LaunchDaemon...");
            Thread.Sleep(90);
        }

        protected override void CheckPrerequisites()
        {
            base.CheckPrerequisites();
            Console.WriteLine("  Checking macOS version compatibility...");
        }
    }

    #endregion
}

/*
 * MEMORY ALLOCATION CONSIDERATIONS:
 * =================================
 * 
 * 1. INHERITANCE OVERHEAD:
 *    - Each derived class has a virtual method table (VMT) in memory
 *    - Base class fields are inherited by all derived classes
 *    - Consider using composition over inheritance for memory-sensitive scenarios
 *    - Virtual method calls have slight overhead compared to direct calls
 * 
 * 2. TEMPLATE METHOD STACK:
 *    - Template method execution creates a call stack with multiple method invocations
 *    - Each abstract/virtual method call adds to stack depth
 *    - Consider stack overflow risks with deeply nested template methods
 *    - Use iterative approaches for very deep template hierarchies
 * 
 * 3. OBJECT LIFECYCLE:
 *    - Template method objects typically have short lifespans
 *    - Avoid holding references to large objects in template method classes
 *    - Consider object pooling for frequently instantiated template classes
 *    - Implement proper disposal for template classes that hold resources
 * 
 * 4. DATA SHARING:
 *    - Template methods often process shared data structures
 *    - Be mindful of memory usage when passing large data between steps
 *    - Consider streaming approaches for large dataset processing
 *    - Use lazy loading for expensive data initialization
 * 
 * MULTITHREAD CONSIDERATIONS:
 * ===========================
 * 
 * 1. TEMPLATE METHOD THREAD SAFETY:
 *    - Template method instances are typically NOT thread-safe by default
 *    - Each thread should use separate template method instances
 *    - Shared state in base classes requires synchronization
 *    - Consider using ThreadLocal storage for per-thread state
 * 
 * 2. ABSTRACT METHOD IMPLEMENTATIONS:
 *    - Derived class implementations must be thread-safe if shared
 *    - Use immutable data structures where possible
 *    - Synchronize access to mutable shared resources
 *    - Consider async/await patterns for I/O-bound operations
 * 
 * 3. HOOK METHOD SAFETY:
 *    - Hook methods may be called from multiple threads
 *    - Default implementations should be thread-safe
 *    - Document thread safety requirements for overrideable methods
 *    - Use concurrent collections for shared data structures
 * 
 * 4. RESOURCE MANAGEMENT:
 *    - Template methods often manage resources (files, connections, etc.)
 *    - Use proper disposal patterns (using statements, try-finally)
 *    - Consider async disposal for async template methods
 *    - Implement timeout mechanisms for long-running operations
 * 
 * 5. EXCEPTION HANDLING:
 *    - Template methods should handle exceptions at appropriate levels
 *    - Consider providing rollback mechanisms for failed operations
 *    - Use proper exception propagation to maintain call stack information
 *    - Implement logging and monitoring for production template methods
 * 
 * 6. PERFORMANCE CONSIDERATIONS:
 *    - Virtual method calls have slight performance overhead
 *    - Consider using generics to reduce boxing for value types
 *    - Profile template method execution under load
 *    - Consider using delegates instead of virtual methods for performance-critical scenarios
 *    - Cache expensive computations when possible
 * 
 * 7. BEST PRACTICES:
 *    - Keep template methods focused and cohesive
 *    - Minimize the number of abstract methods required
 *    - Provide meaningful default implementations for hook methods
 *    - Document the algorithm structure and extension points clearly
 *    - Use dependency injection for external dependencies
 *    - Consider using the strategy pattern for varying algorithms
 *    - Implement proper error handling and logging
 *    - Make template methods testable through dependency injection
 */
