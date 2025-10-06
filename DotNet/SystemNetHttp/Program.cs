/*
 * System.Net.Http Namespace - Comprehensive Examples
 * 
 * PURPOSE:
 * The System.Net.Http namespace provides classes for HTTP client operations, allowing applications
 * to send HTTP requests and receive HTTP responses. It's the primary way to consume REST APIs,
 * web services, and perform HTTP communication in modern .NET applications.
 * 
 * Core classes include:
 * - HttpClient: Main class for sending HTTP requests
 * - HttpRequestMessage/HttpResponseMessage: Represent HTTP requests and responses
 * - HttpContent: Base class for HTTP message content
 * - HttpClientHandler: Configures behavior of HTTP requests
 * - DelegatingHandler: For custom HTTP middleware
 * 
 * SCENARIOS TO USE:
 * 1. Consuming REST APIs and web services
 * 2. Downloading files or web content
 * 3. Uploading data to web servers
 * 4. Authentication with bearer tokens/API keys
 * 5. Custom HTTP headers and content types
 * 6. Proxy and certificate handling
 * 7. Request/response middleware (logging, retry, caching)
 * 8. Microservices communication
 * 9. Web scraping and data extraction
 * 10. Health checks and monitoring endpoints
 * 
 * SCENARIOS NOT TO USE:
 * 1. File I/O operations (use File.ReadAllText, etc.)
 * 2. Database operations (use Entity Framework, ADO.NET)
 * 3. In-process communication (use dependency injection)
 * 4. Real-time communication (consider SignalR, WebSockets)
 * 5. When you need TCP/UDP sockets directly
 * 6. For SMTP email (use MailKit or SmtpClient)
 * 7. FTP operations (use specialized FTP libraries)
 * 8. When bandwidth is extremely limited (consider binary protocols)
 * 
 * MEMORY ALLOCATION:
 * - HttpClient: Should be reused (static/singleton pattern) - expensive to create/dispose
 * - HttpRequestMessage: ~1-2KB per request (header + content overhead)
 * - HttpResponseMessage: Variable size based on response content
 * - String content: Creates copies in memory - use streams for large data
 * - JSON serialization: Additional memory for deserialized objects
 * - Connection pooling: HttpClient reuses connections to reduce overhead
 * - Dispose pattern: Important for HttpResponseMessage to release resources
 */

using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SystemNetHttpExamples
{
    public class Program
    {
        // HttpClient should be static/singleton to reuse connections
        // Creating new HttpClient instances can exhaust socket connections
        private static readonly HttpClient httpClient = new HttpClient();

        public static async Task Main(string[] args)
        {
            Console.WriteLine("=== System.Net.Http Namespace Examples ===\n");

            try
            {
                // Example 1: Basic HTTP GET request
                await BasicGetRequestExample();
                Console.WriteLine();

                // Example 2: HTTP POST with JSON content
                await PostJsonExample();
                Console.WriteLine();

                // Example 3: Custom headers and authentication
                await CustomHeadersExample();
                Console.WriteLine();

                // Example 4: File download with progress
                await FileDownloadExample();
                Console.WriteLine();

                // Example 5: HttpClient configuration and handlers
                await HttpClientConfigurationExample();
                Console.WriteLine();

                // Example 6: Error handling and retries
                await ErrorHandlingExample();
                Console.WriteLine();

                // Example 7: Memory allocation analysis
                await MemoryAllocationExample();
                Console.WriteLine();

                // Example 8: Advanced scenarios (streaming, multipart)
                await AdvancedScenariosExample();
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unhandled exception: {ex.Message}");
            }
            finally
            {
                // Dispose HttpClient when application shuts down
                httpClient.Dispose();
            }
        }

        /*
         * EXAMPLE 1: Basic HTTP GET Request
         * 
         * PURPOSE: Demonstrates the simplest way to make HTTP GET requests
         * 
         * MEMORY ALLOCATION:
         * - HttpClient: Reused static instance (0 additional allocation)
         * - Response content: ~2-50KB depending on API response
         * - JSON deserialization: Additional object allocations
         */
        private static async Task BasicGetRequestExample()
        {
            Console.WriteLine("1. Basic HTTP GET Request Example");
            Console.WriteLine("================================");

            try
            {
                // Make a simple GET request to a public API
                // JSONPlaceholder is a free testing service that returns fake JSON data
                string url = "https://jsonplaceholder.typicode.com/posts/1";
                
                // GetStringAsync is convenient but loads entire response into memory
                // For large responses, use GetAsync with streaming
                string responseContent = await httpClient.GetStringAsync(url);
                
                // Parse JSON response (creates additional objects in memory)
                var post = JsonSerializer.Deserialize<BlogPost>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                
                Console.WriteLine($"Retrieved post: ID={post?.Id}, Title='{post?.Title?.Substring(0, Math.Min(30, post?.Title?.Length ?? 0))}...'");
                Console.WriteLine($"Response size: {responseContent.Length} characters");
                
                // Alternative: Direct deserialization from stream (more memory efficient)
                using var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                
                var postFromStream = await JsonSerializer.DeserializeAsync<BlogPost>(
                    await response.Content.ReadAsStreamAsync(),
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                Console.WriteLine($"Stream deserialization: ID={postFromStream?.Id}");
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP Request failed: {ex.Message}");
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine($"Request timeout: {ex.Message}");
            }
        }

        private static async Task<string> BasicXUnitTestExample()
        {
            Console.WriteLine("1. Basic XUnit Test Example");
            Console.WriteLine("================================");
            string responseContent = string.Empty;
            try
            {
                string url = "https://jsonplaceholder.typicode.com/posts/1";

                responseContent = await httpClient.GetStringAsync(url);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP Request failed: {ex.Message}");
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine($"Request timeout: {ex.Message}");
            }
            
            return responseContent;
        }

        /*
         * EXAMPLE 2: HTTP POST with JSON Content
         * 
         * PURPOSE: Shows how to send data to APIs using POST requests
         * 
         * SCENARIOS TO USE:
         * - Creating new resources via REST APIs
         * - Submitting forms or user data
         * - Authentication requests
         * 
         * MEMORY ALLOCATION:
         * - JSON serialization: Creates string representation (~1.5x object size)
         * - StringContent: Additional byte array allocation
         * - Response: Variable based on server response
         */
        private static async Task PostJsonExample()
        {
            Console.WriteLine("2. HTTP POST with JSON Content Example");
            Console.WriteLine("=====================================");

            try
            {
                // Create object to send
                var newPost = new BlogPost
                {
                    Title = "My New Post",
                    Body = "This is the content of my new post.",
                    UserId = 1
                };

                // Serialize to JSON string
                string jsonContent = JsonSerializer.Serialize(newPost);
                Console.WriteLine($"Sending JSON: {jsonContent}");

                // Create HTTP content with proper content type
                var content = new StringContent(
                    jsonContent,
                    Encoding.UTF8,
                    "application/json"
                );

                // Send POST request
                var response = await httpClient.PostAsync(
                    "https://jsonplaceholder.typicode.com/posts",
                    content
                );

                // Ensure success status code
                response.EnsureSuccessStatusCode();

                // Read response
                string responseContent = await response.Content.ReadAsStringAsync();
                var createdPost = JsonSerializer.Deserialize<BlogPost>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                Console.WriteLine($"Created post with ID: {createdPost?.Id}");
                Console.WriteLine($"Response status: {response.StatusCode}");

                // Important: Dispose response to free resources
                response.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"POST request failed: {ex.Message}");
            }
        }

        /*
         * EXAMPLE 3: Custom Headers and Authentication
         * 
         * PURPOSE: Demonstrates how to add custom headers and handle authentication
         * 
         * SCENARIOS TO USE:
         * - API key authentication
         * - Bearer token authentication (JWT)
         * - Custom headers for tracking, versioning
         * - User-Agent strings for identification
         * 
         * MEMORY ALLOCATION:
         * - Headers: Small overhead (~100-500 bytes per request)
         * - Authorization tokens: Usually <1KB
         */
        private static async Task CustomHeadersExample()
        {
            Console.WriteLine("3. Custom Headers and Authentication Example");
            Console.WriteLine("===========================================");

            try
            {
                // Create a new HttpClient for this example (normally avoid this)
                using var client = new HttpClient();

                // Set default headers that apply to all requests
                client.DefaultRequestHeaders.Add("User-Agent", "MyApp/1.0");
                client.DefaultRequestHeaders.Add("X-API-Version", "2.0");
                
                // Add Authorization header (example with fake bearer token)
                client.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Bearer", "fake-jwt-token-here");

                // Create request with additional headers
                var request = new HttpRequestMessage(
                    HttpMethod.Get,
                    "https://jsonplaceholder.typicode.com/posts"
                );

                // Add request-specific headers
                request.Headers.Add("X-Request-ID", Guid.NewGuid().ToString());
                request.Headers.Add("X-Custom-Header", "custom-value");

                // Send request
                var response = await client.SendAsync(request);
                
                Console.WriteLine($"Request headers sent: {request.Headers.Count()}");
                Console.WriteLine($"Response status: {response.StatusCode}");
                Console.WriteLine($"Response headers received: {response.Headers.Count()}");

                // Display some response headers
                if (response.Content.Headers.ContentType != null)
                {
                    Console.WriteLine($"Content-Type: {response.Content.Headers.ContentType}");
                }

                response.Dispose();
                request.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Custom headers request failed: {ex.Message}");
            }
        }

        /*
         * EXAMPLE 4: File Download with Progress
         * 
         * PURPOSE: Shows how to download files efficiently with progress tracking
         * 
         * SCENARIOS TO USE:
         * - Downloading large files, images, documents
         * - Software updates
         * - Data exports from APIs
         * 
         * MEMORY ALLOCATION:
         * - Stream-based: Constant memory usage regardless of file size
         * - Buffer: Small fixed buffer (8KB-64KB typical)
         * - Progress callbacks: Minimal overhead
         */
        private static async Task FileDownloadExample()
        {
            Console.WriteLine("4. File Download with Progress Example");
            Console.WriteLine("=====================================");

            try
            {
                // Download a small image for demonstration
                string url = "https://via.placeholder.com/150/0000FF/808080?Text=Sample";
                string fileName = "downloaded_image.png";

                // Get response without reading content immediately
                using var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                // Get total file size if available
                long? totalBytes = response.Content.Headers.ContentLength;
                Console.WriteLine($"Total size: {totalBytes ?? -1} bytes");

                // Create file stream
                using var fileStream = File.Create(fileName);
                using var httpStream = await response.Content.ReadAsStreamAsync();

                // Download with progress
                var buffer = new byte[8192]; // 8KB buffer - good balance of memory vs performance
                long downloadedBytes = 0;
                int bytesRead;

                while ((bytesRead = await httpStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    await fileStream.WriteAsync(buffer, 0, bytesRead);
                    downloadedBytes += bytesRead;

                    // Show progress
                    if (totalBytes.HasValue)
                    {
                        double percentage = (double)downloadedBytes / totalBytes.Value * 100;
                        Console.WriteLine($"Progress: {percentage:F1}% ({downloadedBytes}/{totalBytes})");
                    }
                    else
                    {
                        Console.WriteLine($"Downloaded: {downloadedBytes} bytes");
                    }
                }

                Console.WriteLine($"File downloaded successfully: {fileName}");
                Console.WriteLine($"Final size: {new FileInfo(fileName).Length} bytes");

                // Clean up
                File.Delete(fileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"File download failed: {ex.Message}");
            }
        }

        /*
         * EXAMPLE 5: HttpClient Configuration and Handlers
         * 
         * PURPOSE: Advanced HttpClient configuration with custom handlers
         * 
         * SCENARIOS TO USE:
         * - Request/response logging
         * - Automatic retries
         * - Custom authentication
         * - Request modification
         * - Caching
         * - Proxy configuration
         * 
         * MEMORY ALLOCATION:
         * - Handler chain: Small overhead per handler (~100-500 bytes)
         * - Logging: Additional memory for log messages
         * - Retry logic: May duplicate requests (higher memory usage)
         */
        private static async Task HttpClientConfigurationExample()
        {
            Console.WriteLine("5. HttpClient Configuration and Handlers Example");
            Console.WriteLine("===============================================");

            try
            {
                // Create custom logging handler
                var loggingHandler = new LoggingHandler(new HttpClientHandler());

                // Configure HttpClient with custom handler
                using var client = new HttpClient(loggingHandler)
                {
                    Timeout = TimeSpan.FromSeconds(30), // 30 second timeout
                    BaseAddress = new Uri("https://jsonplaceholder.typicode.com/")
                };

                // Set default headers
                client.DefaultRequestHeaders.Add("Accept", "application/json");

                // Make request using relative URL (thanks to BaseAddress)
                var response = await client.GetAsync("posts/1");
                var content = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Response received: {content.Length} characters");
                Console.WriteLine($"Response status: {response.StatusCode}");

                response.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Configured client request failed: {ex.Message}");
            }
        }

        /*
         * EXAMPLE 6: Error Handling and Retries
         * 
         * PURPOSE: Robust error handling with retry logic
         * 
         * SCENARIOS TO USE:
         * - Unreliable network connections
         * - Rate-limited APIs
         * - Transient failures
         * - Circuit breaker patterns
         * 
         * MEMORY ALLOCATION:
         * - Exception objects: ~1-2KB per exception
         * - Retry attempts: Multiplies memory usage by retry count
         * - Backoff delays: Minimal memory impact
         */
        private static async Task ErrorHandlingExample()
        {
            Console.WriteLine("6. Error Handling and Retries Example");
            Console.WriteLine("====================================");

            const int maxRetries = 3;
            const string url = "https://httpstat.us/500"; // Always returns 500 error

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    Console.WriteLine($"Attempt {attempt}/{maxRetries}");

                    using var response = await httpClient.GetAsync(url);
                    
                    // This will throw HttpRequestException for non-success status codes
                    response.EnsureSuccessStatusCode();
                    
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Success: {content}");
                    break; // Success, exit retry loop

                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"HTTP error on attempt {attempt}: {ex.Message}");
                    
                    if (attempt == maxRetries)
                    {
                        Console.WriteLine("Max retries exceeded, giving up");
                        break;
                    }

                    // Exponential backoff: wait longer between retries
                    var delay = TimeSpan.FromSeconds(Math.Pow(2, attempt - 1));
                    Console.WriteLine($"Waiting {delay.TotalSeconds} seconds before retry...");
                    await Task.Delay(delay);
                }
                catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
                {
                    Console.WriteLine($"Timeout on attempt {attempt}");
                    
                    if (attempt == maxRetries)
                    {
                        Console.WriteLine("Max retries exceeded due to timeouts");
                        break;
                    }
                }
            }
        }

        /*
         * EXAMPLE 7: Memory Allocation Analysis
         * 
         * PURPOSE: Analyze memory usage patterns with HttpClient
         * 
         * MEMORY ALLOCATION DETAILS:
         * - HttpClient instance: ~2-4KB (reuse to amortize cost)
         * - Connection pool: ~10-50KB per endpoint
         * - Request/Response objects: ~1-3KB per request
         * - Content strings: Variable (response size dependent)
         * - JSON deserialization: ~2-5x object size in memory
         */
        private static async Task MemoryAllocationExample()
        {
            Console.WriteLine("7. Memory Allocation Analysis Example");
            Console.WriteLine("===================================");

            // Measure memory before requests
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            
            long memoryBefore = GC.GetTotalMemory(false);
            Console.WriteLine($"Memory before HTTP requests: {memoryBefore:N0} bytes");

            // Make multiple requests to analyze allocation patterns
            const int requestCount = 5;
            var tasks = new List<Task>();

            for (int i = 0; i < requestCount; i++)
            {
                tasks.Add(MakeMemoryEfficientRequest(i + 1));
            }

            await Task.WhenAll(tasks);

            // Force garbage collection to see retained memory
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            long memoryAfter = GC.GetTotalMemory(false);
            Console.WriteLine($"Memory after HTTP requests: {memoryAfter:N0} bytes");
            Console.WriteLine($"Memory difference: {memoryAfter - memoryBefore:N0} bytes");
            Console.WriteLine($"Average per request: {(memoryAfter - memoryBefore) / requestCount:N0} bytes");

            // Demonstrate memory-efficient vs memory-intensive approaches
            await CompareMemoryApproaches();
        }

        private static async Task MakeMemoryEfficientRequest(int requestId)
        {
            try
            {
                // Use using statements to ensure proper disposal
                using var response = await httpClient.GetAsync($"https://jsonplaceholder.typicode.com/posts/{requestId}");
                using var stream = await response.Content.ReadAsStreamAsync();
                
                // Stream-based deserialization (more memory efficient)
                var post = await JsonSerializer.DeserializeAsync<BlogPost>(stream, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                
                Console.WriteLine($"Request {requestId}: Retrieved post ID {post?.Id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Request {requestId} failed: {ex.Message}");
            }
        }

        private static async Task CompareMemoryApproaches()
        {
            Console.WriteLine("\nMemory Approach Comparison:");
            Console.WriteLine("===========================");

            // Memory-intensive approach: Load everything into strings
            Console.WriteLine("1. Memory-intensive approach (string-based):");
            long memBefore1 = GC.GetTotalMemory(true);
            
            var stringResponse = await httpClient.GetStringAsync("https://jsonplaceholder.typicode.com/posts");
            var posts1 = JsonSerializer.Deserialize<BlogPost[]>(stringResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            
            long memAfter1 = GC.GetTotalMemory(false);
            Console.WriteLine($"   Memory used: {memAfter1 - memBefore1:N0} bytes for {posts1?.Length} posts");

            // Memory-efficient approach: Stream-based
            Console.WriteLine("2. Memory-efficient approach (stream-based):");
            long memBefore2 = GC.GetTotalMemory(true);
            
            using var response = await httpClient.GetAsync("https://jsonplaceholder.typicode.com/posts");
            using var stream = await response.Content.ReadAsStreamAsync();
            var posts2 = await JsonSerializer.DeserializeAsync<BlogPost[]>(stream, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            
            long memAfter2 = GC.GetTotalMemory(false);
            Console.WriteLine($"   Memory used: {memAfter2 - memBefore2:N0} bytes for {posts2?.Length} posts");

            long savedMemory = (memAfter1 - memBefore1) - (memAfter2 - memBefore2);
            Console.WriteLine($"   Memory saved: {savedMemory:N0} bytes ({(double)savedMemory / (memAfter1 - memBefore1) * 100:F1}%)");
        }

        /*
         * EXAMPLE 8: Advanced Scenarios
         * 
         * PURPOSE: Advanced HttpClient usage patterns
         * 
         * SCENARIOS:
         * - Multipart form data (file uploads)
         * - Streaming large responses
         * - Custom content types
         * - Request cancellation
         * 
         * MEMORY ALLOCATION:
         * - Multipart content: ~1.5x total content size
         * - Streaming: Constant memory regardless of content size
         * - Cancellation: Minimal overhead (~100 bytes)
         */
        private static async Task AdvancedScenariosExample()
        {
            Console.WriteLine("8. Advanced Scenarios Example");
            Console.WriteLine("=============================");

            // Scenario 1: Multipart form data
            await MultipartFormDataExample();
            
            // Scenario 2: Request cancellation
            await RequestCancellationExample();
            
            // Scenario 3: Custom content types
            await CustomContentTypesExample();
        }

        private static async Task MultipartFormDataExample()
        {
            Console.WriteLine("Multipart Form Data Example:");
            
            try
            {
                // Create multipart form data content
                using var formContent = new MultipartFormDataContent();
                
                // Add text fields
                formContent.Add(new StringContent("John Doe"), "name");
                formContent.Add(new StringContent("john@example.com"), "email");
                
                // Add file content (simulated)
                var fileContent = new ByteArrayContent(Encoding.UTF8.GetBytes("Fake file content"));
                fileContent.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
                formContent.Add(fileContent, "file", "example.txt");
                
                // This would normally go to a file upload endpoint
                // Using JSONPlaceholder for demonstration (it will ignore the multipart data)
                var response = await httpClient.PostAsync(
                    "https://jsonplaceholder.typicode.com/posts",
                    formContent
                );
                
                Console.WriteLine($"Multipart upload response: {response.StatusCode}");
                response.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Multipart form error: {ex.Message}");
            }
        }

        private static async Task RequestCancellationExample()
        {
            Console.WriteLine("\nRequest Cancellation Example:");
            
            try
            {
                // Create cancellation token with timeout
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
                
                // Make request that might take too long
                var response = await httpClient.GetAsync(
                    "https://httpstat.us/200?sleep=5000", // Sleeps for 5 seconds
                    cts.Token
                );
                
                Console.WriteLine($"Request completed: {response.StatusCode}");
                response.Dispose();
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Request was cancelled due to timeout");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Request error: {ex.Message}");
            }
        }

        private static async Task CustomContentTypesExample()
        {
            Console.WriteLine("\nCustom Content Types Example:");
            
            try
            {
                // Send XML content
                var xmlContent = new StringContent(
                    "<person><name>John</name><age>30</age></person>",
                    Encoding.UTF8,
                    "application/xml"
                );
                
                var response = await httpClient.PostAsync(
                    "https://jsonplaceholder.typicode.com/posts",
                    xmlContent
                );
                
                Console.WriteLine($"XML content response: {response.StatusCode}");
                
                // Send binary content
                var binaryData = new byte[] { 0x89, 0x50, 0x4E, 0x47 }; // PNG header
                var binaryContent = new ByteArrayContent(binaryData);
                binaryContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");
                
                var binaryResponse = await httpClient.PostAsync(
                    "https://jsonplaceholder.typicode.com/posts",
                    binaryContent
                );
                
                Console.WriteLine($"Binary content response: {binaryResponse.StatusCode}");
                
                response.Dispose();
                binaryResponse.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Custom content error: {ex.Message}");
            }
        }
    }

    /*
     * Custom HTTP Handler for Logging
     * 
     * PURPOSE: Demonstrates how to create custom handlers for cross-cutting concerns
     * 
     * MEMORY ALLOCATION:
     * - Handler overhead: ~200-500 bytes per request
     * - Log messages: Variable based on logging detail
     */
    public class LoggingHandler : DelegatingHandler
    {
        public LoggingHandler(HttpMessageHandler innerHandler) : base(innerHandler)
        {
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, 
            CancellationToken cancellationToken)
        {
            // Log request
            Console.WriteLine($"[LOG] Sending {request.Method} request to {request.RequestUri}");
            
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            // Call next handler in chain
            var response = await base.SendAsync(request, cancellationToken);
            
            stopwatch.Stop();
            
            // Log response
            Console.WriteLine($"[LOG] Received {response.StatusCode} response in {stopwatch.ElapsedMilliseconds}ms");
            
            return response;
        }
    }

    /*
     * Data Transfer Objects for Examples
     */
    public class BlogPost
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Body { get; set; }
        public int UserId { get; set; }
    }
}
