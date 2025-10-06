using System.Net;
using System.Net.Http;
using System.Text;
using RichardSzalay.MockHttp;
using SystemNetHttpExamples;
using Xunit;

namespace SystemNetHttp.Tests
{
    /// <summary>
    /// Unit tests for HttpService class demonstrating various xUnit testing patterns
    /// </summary>
    public class HttpServiceTests : IDisposable
    {
        private readonly MockHttpMessageHandler _mockHttpMessageHandler;
        private readonly HttpClient _httpClient;
        private readonly HttpService _httpService;

        public HttpServiceTests()
        {
            // Setup: Create mock HTTP handler and client for each test
            _mockHttpMessageHandler = new MockHttpMessageHandler();
            _httpClient = new HttpClient(_mockHttpMessageHandler);
            _httpService = new HttpService(_httpClient);
        }

        public void Dispose()
        {
            // Cleanup: Dispose resources after each test
            _httpClient?.Dispose();
            _mockHttpMessageHandler?.Dispose();
        }

        #region Success Scenarios

        [Fact]
        public async Task GetPostAsync_WithValidPostId_ReturnsExpectedJson()
        {
            // Arrange
            const int postId = 1;
            const string expectedResponse = """
                {
                  "userId": 1,
                  "id": 1,
                  "title": "sunt aut facere repellat provident occaecati excepturi optio reprehenderit",
                  "body": "quia et suscipit\nsuscipit recusandae consequuntur expedita et cum\nreprehenderit molestiae ut ut quas totam\nnostrum rerum est autem sunt rem eveniet architecto"
                }
                """;

            _mockHttpMessageHandler
                .When($"https://jsonplaceholder.typicode.com/posts/{postId}")
                .Respond("application/json", expectedResponse);

            // Act
            string result = await _httpService.GetPostAsync(postId);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Contains("\"id\": 1", result);
            Assert.Contains("sunt aut facere", result);
        }

        [Fact]
        public async Task GetPostAsync_WithValidUrl_ReturnsResponseContent()
        {
            // Arrange
            const string url = "https://api.example.com/data";
            const string expectedResponse = "Test response content";

            _mockHttpMessageHandler
                .When(url)
                .Respond("text/plain", expectedResponse);

            // Act
            string result = await _httpService.GetPostAsync(url);

            // Assert
            Assert.Equal(expectedResponse, result);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(100)]
        public async Task GetPostAsync_WithDifferentPostIds_ReturnsCorrespondingData(int postId)
        {
            // Arrange
            string expectedResponse = $"{{\"id\": {postId}, \"title\": \"Post {postId}\"}}";

            _mockHttpMessageHandler
                .When($"https://jsonplaceholder.typicode.com/posts/{postId}")
                .Respond("application/json", expectedResponse);

            // Act
            string result = await _httpService.GetPostAsync(postId);

            // Assert
            Assert.Contains($"\"id\": {postId}", result);
            Assert.Contains($"Post {postId}", result);
        }

        #endregion

        #region Error Scenarios

        [Fact]
        public async Task GetPostAsync_WhenHttpRequestFails_ThrowsHttpRequestException()
        {
            // Arrange
            const int postId = 1;

            _mockHttpMessageHandler
                .When($"https://jsonplaceholder.typicode.com/posts/{postId}")
                .Respond(HttpStatusCode.InternalServerError);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<HttpRequestException>(
                () => _httpService.GetPostAsync(postId)
            );

            Assert.NotNull(exception);
        }

        [Fact]
        public async Task GetPostAsync_WhenRequestTimesOut_ThrowsTaskCanceledException()
        {
            // Arrange
            const int postId = 1;
            
            // Create HttpClient with very short timeout
            using var timeoutClient = new HttpClient(_mockHttpMessageHandler)
            {
                Timeout = TimeSpan.FromMilliseconds(1)
            };
            var timeoutService = new HttpService(timeoutClient);

            _mockHttpMessageHandler
                .When($"https://jsonplaceholder.typicode.com/posts/{postId}")
                .Respond(async () =>
                {
                    await Task.Delay(100); // Delay longer than timeout
                    return new HttpResponseMessage(HttpStatusCode.OK);
                });

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(
                () => timeoutService.GetPostAsync(postId)
            );
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task GetPostAsync_WithInvalidUrl_ThrowsArgumentException(string invalidUrl)
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _httpService.GetPostAsync(invalidUrl)
            );

            Assert.Contains("URL cannot be null or empty", exception.Message);
            Assert.Equal("url", exception.ParamName);
        }

        [Fact]
        public async Task GetPostAsync_WhenNotFound_ThrowsHttpRequestException()
        {
            // Arrange
            const int postId = 999;

            _mockHttpMessageHandler
                .When($"https://jsonplaceholder.typicode.com/posts/{postId}")
                .Respond(HttpStatusCode.NotFound);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<HttpRequestException>(
                () => _httpService.GetPostAsync(postId)
            );

            Assert.NotNull(exception);
        }

        #endregion

        #region Constructor Tests

        [Fact]
        public void Constructor_WithNullHttpClient_ThrowsArgumentNullException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(
                () => new HttpService(null!)
            );

            Assert.Equal("httpClient", exception.ParamName);
        }

        [Fact]
        public void Constructor_WithValidHttpClient_CreatesInstance()
        {
            // Arrange
            using var httpClient = new HttpClient();

            // Act
            var service = new HttpService(httpClient);

            // Assert
            Assert.NotNull(service);
        }

        #endregion

        #region Integration-style Tests (still mocked but more realistic)

        [Fact]
        public async Task GetPostAsync_WithRealJsonResponse_CanBeDeserialized()
        {
            // Arrange
            const int postId = 1;
            const string jsonResponse = """
                {
                  "userId": 1,
                  "id": 1,
                  "title": "sunt aut facere repellat provident occaecati excepturi optio reprehenderit",
                  "body": "quia et suscipit\nsuscipit recusandae consequuntur expedita et cum\nreprehenderit molestiae ut ut quas totam\nnostrum rerum est autem sunt rem eveniet architecto"
                }
                """;

            _mockHttpMessageHandler
                .When($"https://jsonplaceholder.typicode.com/posts/{postId}")
                .Respond("application/json", jsonResponse);

            // Act
            string result = await _httpService.GetPostAsync(postId);

            // Assert - Verify the response can be used as JSON
            Assert.NotNull(result);
            Assert.StartsWith("{", result.Trim());
            Assert.EndsWith("}", result.Trim());
            
            // Could also deserialize to verify structure
            // var post = JsonSerializer.Deserialize<BlogPost>(result);
            // Assert.Equal(1, post.Id);
        }

        [Fact]
        public async Task GetPostAsync_WithLargeResponse_HandlesCorrectly()
        {
            // Arrange
            const int postId = 1;
            var largeContent = new StringBuilder();
            for (int i = 0; i < 1000; i++)
            {
                largeContent.AppendLine($"Line {i}: This is a large response content for testing purposes.");
            }
            string expectedResponse = largeContent.ToString();

            _mockHttpMessageHandler
                .When($"https://jsonplaceholder.typicode.com/posts/{postId}")
                .Respond("text/plain", expectedResponse);

            // Act
            string result = await _httpService.GetPostAsync(postId);

            // Assert
            Assert.Equal(expectedResponse, result);
            Assert.True(result.Length > 50000); // Verify it's actually large
        }

        #endregion

        #region HTTP Status Code Tests

        [Theory]
        [InlineData(HttpStatusCode.BadRequest)]
        [InlineData(HttpStatusCode.Unauthorized)]
        [InlineData(HttpStatusCode.Forbidden)]
        [InlineData(HttpStatusCode.NotFound)]
        [InlineData(HttpStatusCode.InternalServerError)]
        [InlineData(HttpStatusCode.BadGateway)]
        [InlineData(HttpStatusCode.ServiceUnavailable)]
        public async Task GetPostAsync_WithErrorStatusCodes_ThrowsHttpRequestException(HttpStatusCode statusCode)
        {
            // Arrange
            const int postId = 1;

            _mockHttpMessageHandler
                .When($"https://jsonplaceholder.typicode.com/posts/{postId}")
                .Respond(statusCode);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<HttpRequestException>(
                () => _httpService.GetPostAsync(postId)
            );

            Assert.NotNull(exception);
        }

        #endregion
    }
}