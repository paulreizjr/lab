using System.Net.Http;

namespace SystemNetHttpExamples
{
    /// <summary>
    /// Service interface for HTTP operations - enables dependency injection and mocking
    /// </summary>
    public interface IHttpService
    {
        Task<string> GetPostAsync(int postId);
        Task<string> GetPostAsync(string url);
    }

    /// <summary>
    /// HTTP service implementation that wraps HttpClient operations
    /// This makes the HTTP operations testable by allowing mocking
    /// </summary>
    public class HttpService : IHttpService
    {
        private readonly HttpClient _httpClient;

        public HttpService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        /// <summary>
        /// Gets a post by ID from JSONPlaceholder API
        /// </summary>
        /// <param name="postId">The ID of the post to retrieve</param>
        /// <returns>JSON string containing the post data</returns>
        /// <exception cref="HttpRequestException">Thrown when HTTP request fails</exception>
        /// <exception cref="TaskCanceledException">Thrown when request times out</exception>
        public async Task<string> GetPostAsync(int postId)
        {
            string url = $"https://jsonplaceholder.typicode.com/posts/{postId}";
            return await GetPostAsync(url);
        }

        /// <summary>
        /// Gets content from the specified URL
        /// </summary>
        /// <param name="url">The URL to retrieve content from</param>
        /// <returns>Response content as string</returns>
        /// <exception cref="HttpRequestException">Thrown when HTTP request fails</exception>
        /// <exception cref="TaskCanceledException">Thrown when request times out</exception>
        public async Task<string> GetPostAsync(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("URL cannot be null or empty", nameof(url));

            try
            {
                return await _httpClient.GetStringAsync(url);
            }
            catch (HttpRequestException)
            {
                // Re-throw to allow caller to handle
                throw;
            }
            catch (TaskCanceledException)
            {
                // Re-throw to allow caller to handle
                throw;
            }
        }
    }
}