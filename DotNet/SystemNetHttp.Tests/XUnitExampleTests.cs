using System.Text.Json;
using Xunit;

namespace SystemNetHttp.Tests
{
    /// <summary>
    /// Example test data classes for testing JSON deserialization
    /// </summary>
    public class BlogPost
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Body { get; set; }
        public int UserId { get; set; }
    }

    /// <summary>
    /// Tests demonstrating various xUnit features and patterns
    /// </summary>
    public class XUnitExampleTests
    {
        #region Basic Test Examples

        [Fact]
        public void Simple_Addition_Test()
        {
            // Arrange
            int a = 2;
            int b = 3;
            int expected = 5;

            // Act
            int result = a + b;

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(1, 1, 2)]
        [InlineData(2, 3, 5)]
        [InlineData(-1, 1, 0)]
        [InlineData(0, 0, 0)]
        public void Addition_WithMultipleInputs_ReturnsExpectedResult(int a, int b, int expected)
        {
            // Act
            int result = a + b;

            // Assert
            Assert.Equal(expected, result);
        }

        #endregion

        #region String and JSON Testing Examples

        [Fact]
        public void JsonDeserialization_WithValidJson_ReturnsCorrectObject()
        {
            // Arrange
            const string json = """
                {
                  "id": 1,
                  "title": "Test Post",
                  "body": "This is a test post body",
                  "userId": 123
                }
                """;

            // Act
            var post = JsonSerializer.Deserialize<BlogPost>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            // Assert
            Assert.NotNull(post);
            Assert.Equal(1, post.Id);
            Assert.Equal("Test Post", post.Title);
            Assert.Equal("This is a test post body", post.Body);
            Assert.Equal(123, post.UserId);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void EmptyOrNullString_Tests(string input)
        {
            // Assert
            Assert.True(string.IsNullOrWhiteSpace(input));
        }

        [Fact]
        public void String_Contains_Test()
        {
            // Arrange
            const string responseContent = """
                {
                  "userId": 1,
                  "id": 1,
                  "title": "sunt aut facere repellat",
                  "body": "quia et suscipit"
                }
                """;

            // Act & Assert
            Assert.Contains("\"id\": 1", responseContent);
            Assert.Contains("sunt aut facere", responseContent);
            Assert.DoesNotContain("nonexistent", responseContent);
        }

        #endregion

        #region Exception Testing Examples

        [Fact]
        public void DivideByZero_ThrowsException()
        {
            // Arrange
            int numerator = 10;
            int denominator = 0;

            // Act & Assert
            Assert.Throws<DivideByZeroException>(() => 
            {
                if (denominator == 0)
                    throw new DivideByZeroException("Cannot divide by zero");
                return numerator / denominator;
            });
        }

        [Fact]
        public async Task AsyncMethod_ThrowsException_CanBeTested()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await Task.Delay(1);
                throw new ArgumentException("Test exception");
            });

            Assert.Equal("Test exception", exception.Message);
        }

        #endregion

        #region Collection Testing Examples

        [Fact]
        public void Collection_Tests()
        {
            // Arrange
            var numbers = new List<int> { 1, 2, 3, 4, 5 };

            // Assert
            Assert.NotEmpty(numbers);
            Assert.Equal(5, numbers.Count);
            Assert.Contains(3, numbers);
            Assert.DoesNotContain(6, numbers);
            Assert.All(numbers, n => Assert.True(n > 0));
        }

        #endregion

        #region Custom Assertion Examples

        [Fact]
        public void HttpResponse_Validation_Example()
        {
            // Arrange - Simulate what your HTTP method might return
            const string responseContent = """
                {
                  "userId": 1,
                  "id": 1,
                  "title": "sunt aut facere repellat provident occaecati excepturi optio reprehenderit",
                  "body": "quia et suscipit\nsuscipit recusandae consequuntur expedita et cum\nreprehenderit molestiae ut ut quas totam\nnostrum rerum est autem sunt rem eveniet architecto"
                }
                """;

            // Act - This simulates what your BasicXUnitTestExample method returns
            
            // Assert - Multiple validations for HTTP response
            Assert.NotNull(responseContent);
            Assert.NotEmpty(responseContent);
            
            // Validate JSON structure
            Assert.StartsWith("{", responseContent.Trim());
            Assert.EndsWith("}", responseContent.Trim());
            
            // Validate specific content
            Assert.Contains("\"id\":", responseContent);
            Assert.Contains("\"userId\":", responseContent);
            Assert.Contains("\"title\":", responseContent);
            Assert.Contains("\"body\":", responseContent);
            
            // Validate that content is reasonable length (not just empty JSON)
            Assert.True(responseContent.Length > 50);
            
            // Test deserialization works
            var post = JsonSerializer.Deserialize<BlogPost>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            
            Assert.NotNull(post);
            Assert.True(post.Id > 0);
            Assert.NotNull(post.Title);
            Assert.NotEmpty(post.Title);
        }

        #endregion
    }
}