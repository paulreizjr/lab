using System.Net.Http;
using RichardSzalay.MockHttp;
using SystemNetHttpExamples;
using Xunit;

namespace SystemNetHttp.Tests
{
    /// <summary>
    /// Direct tests for the original BasicXUnitTestExample method
    /// This shows how to test static methods that use static HttpClient
    /// </summary>
    public class OriginalMethodTests : IDisposable
    {
        private readonly MockHttpMessageHandler _mockHandler;
        private readonly HttpClient _mockHttpClient;

        public OriginalMethodTests()
        {
            _mockHandler = new MockHttpMessageHandler();
            _mockHttpClient = new HttpClient(_mockHandler);
        }

        public void Dispose()
        {
            _mockHttpClient?.Dispose();
            _mockHandler?.Dispose();
        }

        /// <summary>
        /// Test that demonstrates the challenge of testing the original method
        /// Since it uses a static HttpClient, we need to refactor it slightly
        /// </summary>
        [Fact]
        public async Task OriginalBasicXUnitTestExample_Refactored_WithSuccessfulResponse_ReturnsContent()
        {
            // Arrange
            const string expectedResponse = """
                {
                  "userId": 1,
                  "id": 1,
                  "title": "sunt aut facere repellat provident occaecati excepturi optio reprehenderit",
                  "body": "quia et suscipit\nsuscipit recusandae consequuntur expedita et cum\nreprehenderit molestiae ut ut quas totam\nnostrum rerum est autem sunt rem eveniet architecto"
                }
                """;

            _mockHandler
                .When("https://jsonplaceholder.typicode.com/posts/1")
                .Respond("application/json", expectedResponse);

            // Act - Using the refactored version that accepts HttpClient
            string result = await BasicXUnitTestExampleRefactored(_mockHttpClient);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedResponse, result);
            Assert.Contains("\"id\": 1", result);
        }

        [Fact]
        public async Task OriginalBasicXUnitTestExample_WithHttpError_ReturnsEmptyString()
        {
            // Arrange
            _mockHandler
                .When("https://jsonplaceholder.typicode.com/posts/1")
                .Respond(System.Net.HttpStatusCode.InternalServerError);

            // Act
            string result = await BasicXUnitTestExampleRefactored(_mockHttpClient);

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public async Task OriginalBasicXUnitTestExample_WithTimeout_ReturnsEmptyString()
        {
            // Arrange
            using var timeoutClient = new HttpClient(_mockHandler)
            {
                Timeout = TimeSpan.FromMilliseconds(1)
            };

            _mockHandler
                .When("https://jsonplaceholder.typicode.com/posts/1")
                .Respond(async () =>
                {
                    await Task.Delay(100); // Delay longer than timeout
                    return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
                });

            // Act
            string result = await BasicXUnitTestExampleRefactored(timeoutClient);

            // Assert
            Assert.Equal(string.Empty, result);
        }

        /// <summary>
        /// Refactored version of the original BasicXUnitTestExample method
        /// Key changes:
        /// 1. Accepts HttpClient as parameter (dependency injection)
        /// 2. Removed Console.WriteLine for cleaner testing
        /// 3. Made public static for easier testing
        /// </summary>
        public static async Task<string> BasicXUnitTestExampleRefactored(HttpClient httpClient)
        {
            string responseContent = string.Empty;
            try
            {
                string url = "https://jsonplaceholder.typicode.com/posts/1";
                responseContent = await httpClient.GetStringAsync(url);
            }
            catch (HttpRequestException)
            {
                // In the original: Console.WriteLine($"HTTP Request failed: {ex.Message}");
                // For testing: we just return empty string or could log to test output
            }
            catch (TaskCanceledException)
            {
                // In the original: Console.WriteLine($"Request timeout: {ex.Message}");
                // For testing: we just return empty string or could log to test output
            }

            return responseContent;
        }
    }
}

/*
 * TESTING YOUR ORIGINAL METHOD - APPROACHES:
 * 
 * 1. WRAPPER APPROACH (Recommended for legacy code):
 *    - Create a testable wrapper that accepts HttpClient as parameter
 *    - Copy the original logic but make HttpClient injectable
 *    - Test the wrapper method thoroughly
 * 
 * 2. REFACTOR APPROACH (Best for new code):
 *    - Extract HTTP operations into a service class
 *    - Use dependency injection for HttpClient
 *    - Make the service testable from the start
 * 
 * 3. PARTIAL REFACTOR:
 *    - Modify original method to accept optional HttpClient parameter
 *    - Keep backward compatibility with default static HttpClient
 *    - Example:
 *      private static async Task<string> BasicXUnitTestExample(HttpClient httpClient = null)
 *      {
 *          httpClient ??= Program.httpClient; // Use static as fallback
 *          // ... rest of method
 *      }
 * 
 * 4. INTEGRATION TESTING:
 *    - Test against real APIs (use sparingly)
 *    - Good for end-to-end validation
 *    - Slow and unreliable for unit testing
 * 
 * EXAMPLE OF MODIFYING YOUR ORIGINAL METHOD:
 * 
 * // Original method (hard to test)
 * private static async Task<string> BasicXUnitTestExample()
 * {
 *     // ... uses static httpClient
 * }
 * 
 * // Modified method (testable)
 * private static async Task<string> BasicXUnitTestExample(HttpClient httpClient = null)
 * {
 *     httpClient ??= Program.httpClient; // Use static as fallback
 *     
 *     string responseContent = string.Empty;
 *     try
 *     {
 *         string url = "https://jsonplaceholder.typicode.com/posts/1";
 *         responseContent = await httpClient.GetStringAsync(url);
 *     }
 *     catch (HttpRequestException ex)
 *     {
 *         Console.WriteLine($"HTTP Request failed: {ex.Message}");
 *     }
 *     catch (TaskCanceledException ex)
 *     {
 *         Console.WriteLine($"Request timeout: {ex.Message}");
 *     }
 *     
 *     return responseContent;
 * }
 * 
 * // Now you can test it:
 * [Fact]
 * public async Task BasicXUnitTestExample_WithMockHttpClient_Works()
 * {
 *     // Arrange
 *     var mockHandler = new MockHttpMessageHandler();
 *     var mockClient = new HttpClient(mockHandler);
 *     mockHandler.When("*").Respond("application/json", "test response");
 *     
 *     // Act
 *     string result = await BasicXUnitTestExample(mockClient);
 *     
 *     // Assert
 *     Assert.Equal("test response", result);
 * }
 */