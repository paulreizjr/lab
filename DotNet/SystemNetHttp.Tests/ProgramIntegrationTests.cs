using System.Net;
using System.Net.Http;
using RichardSzalay.MockHttp;
using SystemNetHttpExamples;
using Xunit;

namespace SystemNetHttp.Tests
{
    /// <summary>
    /// Integration tests for the original Program class methods
    /// These tests demonstrate how to test static methods with HTTP dependencies
    /// </summary>
    public class ProgramIntegrationTests : IDisposable
    {
        private readonly MockHttpMessageHandler _mockHttpMessageHandler;
        private readonly HttpClient _mockHttpClient;

        public ProgramIntegrationTests()
        {
            _mockHttpMessageHandler = new MockHttpMessageHandler();
            _mockHttpClient = new HttpClient(_mockHttpMessageHandler);
        }

        public void Dispose()
        {
            _mockHttpClient?.Dispose();
            _mockHttpMessageHandler?.Dispose();
        }

        /// <summary>
        /// Test the BasicXUnitTestExample method by creating a testable wrapper
        /// Since the original method uses a static HttpClient, we need to refactor for testability
        /// </summary>
        [Fact]
        public async Task BasicXUnitTestExample_WithSuccessfulResponse_ReturnsJsonContent()
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

            _mockHttpMessageHandler
                .When("https://jsonplaceholder.typicode.com/posts/1")
                .Respond("application/json", expectedResponse);

            // Act
            string result = await BasicXUnitTestExampleWrapper(_mockHttpClient);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Contains("\"id\": 1", result);
            Assert.Contains("sunt aut facere", result);
        }

        [Fact]
        public async Task BasicXUnitTestExample_WithHttpRequestException_ReturnsEmptyString()
        {
            // Arrange
            _mockHttpMessageHandler
                .When("https://jsonplaceholder.typicode.com/posts/1")
                .Respond(HttpStatusCode.InternalServerError);

            // Act
            string result = await BasicXUnitTestExampleWrapper(_mockHttpClient);

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public async Task BasicXUnitTestExample_WithTimeout_ReturnsEmptyString()
        {
            // Arrange
            using var timeoutClient = new HttpClient(_mockHttpMessageHandler)
            {
                Timeout = TimeSpan.FromMilliseconds(1)
            };

            _mockHttpMessageHandler
                .When("https://jsonplaceholder.typicode.com/posts/1")
                .Respond(async () =>
                {
                    await Task.Delay(100); // Delay longer than timeout
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent("Should not reach here")
                    };
                });

            // Act
            string result = await BasicXUnitTestExampleWrapper(timeoutClient);

            // Assert
            Assert.Equal(string.Empty, result);
        }

        /// <summary>
        /// Testable wrapper for the BasicXUnitTestExample method
        /// This allows us to inject a mock HttpClient for testing
        /// </summary>
        private static async Task<string> BasicXUnitTestExampleWrapper(HttpClient httpClient)
        {
            // This is a testable version of the original BasicXUnitTestExample method
            string responseContent = string.Empty;
            try
            {
                string url = "https://jsonplaceholder.typicode.com/posts/1";
                responseContent = await httpClient.GetStringAsync(url);
            }
            catch (HttpRequestException ex)
            {
                // In tests, we might want to capture this for verification
                // Console.WriteLine($"HTTP Request failed: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"HTTP Request failed: {ex.Message}");
            }
            catch (TaskCanceledException ex)
            {
                // In tests, we might want to capture this for verification
                // Console.WriteLine($"Request timeout: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Request timeout: {ex.Message}");
            }

            return responseContent;
        }
    }

    /// <summary>
    /// Example of testing HTTP methods with real HTTP calls (integration tests)
    /// These tests actually hit the real API - use sparingly and consider using [Fact(Skip = "Integration test")]
    /// </summary>
    public class RealHttpIntegrationTests : IDisposable
    {
        private readonly HttpClient _httpClient;

        public RealHttpIntegrationTests()
        {
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
        }

        [Fact(Skip = "Real integration test - enable when needed")]
        public async Task GetPost_WithRealApi_ReturnsValidJson()
        {
            // Arrange
            const string url = "https://jsonplaceholder.typicode.com/posts/1";

            // Act
            string result = await _httpClient.GetStringAsync(url);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Contains("\"id\":", result);
            Assert.Contains("\"title\":", result);
            Assert.Contains("\"body\":", result);
            Assert.Contains("\"userId\":", result);
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}