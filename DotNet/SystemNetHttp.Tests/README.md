# Unit Testing HTTP Methods with xUnit

This project demonstrates how to write comprehensive unit tests for HTTP methods using xUnit, including testing the `BasicXUnitTestExample` method and similar HTTP operations.

## Overview

Testing HTTP methods presents unique challenges:
- External dependencies (APIs, network)
- Async operations
- Error handling (timeouts, HTTP errors)
- Resource management (HttpClient disposal)

This project shows multiple approaches to solve these challenges.

## Project Structure

```
SystemNetHttp.Tests/
├── HttpServiceTests.cs          # Tests for refactored, testable service
├── ProgramIntegrationTests.cs   # Tests for original static methods
├── XUnitExampleTests.cs         # General xUnit patterns and examples
└── SystemNetHttp.Tests.csproj   # Test project file
```

## Key Testing Approaches

### 1. Dependency Injection Approach (Recommended)

**File:** `HttpServiceTests.cs`

```csharp
public class HttpServiceTests : IDisposable
{
    private readonly MockHttpMessageHandler _mockHttpMessageHandler;
    private readonly HttpClient _httpClient;
    private readonly HttpService _httpService;

    public HttpServiceTests()
    {
        _mockHttpMessageHandler = new MockHttpMessageHandler();
        _httpClient = new HttpClient(_mockHttpMessageHandler);
        _httpService = new HttpService(_httpClient);
    }

    [Fact]
    public async Task GetPostAsync_WithValidPostId_ReturnsExpectedJson()
    {
        // Arrange
        const int postId = 1;
        const string expectedResponse = "{ ... }";

        _mockHttpMessageHandler
            .When($"https://jsonplaceholder.typicode.com/posts/{postId}")
            .Respond("application/json", expectedResponse);

        // Act
        string result = await _httpService.GetPostAsync(postId);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("\"id\": 1", result);
    }
}
```

**Benefits:**
- ✅ Fast execution (no network calls)
- ✅ Predictable results
- ✅ Tests all scenarios (success, failure, timeout)
- ✅ No external dependencies

### 2. Wrapper Method Approach

**File:** `ProgramIntegrationTests.cs`

For testing existing static methods, create a testable wrapper:

```csharp
[Fact]
public async Task BasicXUnitTestExample_WithSuccessfulResponse_ReturnsJsonContent()
{
    // Arrange
    const string expectedResponse = "{ ... }";
    
    _mockHttpMessageHandler
        .When("https://jsonplaceholder.typicode.com/posts/1")
        .Respond("application/json", expectedResponse);

    // Act
    string result = await BasicXUnitTestExampleWrapper(_mockHttpClient);

    // Assert
    Assert.NotNull(result);
    Assert.NotEmpty(result);
}

private static async Task<string> BasicXUnitTestExampleWrapper(HttpClient httpClient)
{
    // Copy of original method logic but with injected HttpClient
    string responseContent = string.Empty;
    try
    {
        string url = "https://jsonplaceholder.typicode.com/posts/1";
        responseContent = await httpClient.GetStringAsync(url);
    }
    catch (HttpRequestException ex)
    {
        System.Diagnostics.Debug.WriteLine($"HTTP Request failed: {ex.Message}");
    }
    catch (TaskCanceledException ex)
    {
        System.Diagnostics.Debug.WriteLine($"Request timeout: {ex.Message}");
    }
    
    return responseContent;
}
```

### 3. Integration Tests (Use Sparingly)

```csharp
[Fact(Skip = "Real integration test - enable when needed")]
public async Task GetPost_WithRealApi_ReturnsValidJson()
{
    // This actually calls the real API
    const string url = "https://jsonplaceholder.typicode.com/posts/1";
    string result = await _httpClient.GetStringAsync(url);
    
    Assert.NotNull(result);
    Assert.Contains("\"id\":", result);
}
```

## Required NuGet Packages

```xml
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
<PackageReference Include="xunit" Version="2.6.1" />
<PackageReference Include="xunit.runner.visualstudio" Version="2.5.3" />
<PackageReference Include="Moq" Version="4.20.69" />
<PackageReference Include="RichardSzalay.MockHttp" Version="7.0.0" />
```

## Key xUnit Patterns

### 1. Basic Test Structure

```csharp
[Fact]
public async Task MethodName_Scenario_ExpectedResult()
{
    // Arrange - Set up test data and mocks
    
    // Act - Execute the method under test
    
    // Assert - Verify the results
}
```

### 2. Parameterized Tests

```csharp
[Theory]
[InlineData(1)]
[InlineData(5)]
[InlineData(100)]
public async Task GetPostAsync_WithDifferentPostIds_ReturnsCorrespondingData(int postId)
{
    // Test runs once for each InlineData value
}
```

### 3. Exception Testing

```csharp
[Fact]
public async Task GetPostAsync_WhenHttpRequestFails_ThrowsHttpRequestException()
{
    // Arrange - Set up mock to return error
    
    // Act & Assert
    var exception = await Assert.ThrowsAsync<HttpRequestException>(
        () => _httpService.GetPostAsync(postId)
    );
    
    Assert.NotNull(exception);
}
```

### 4. Resource Cleanup

```csharp
public class HttpServiceTests : IDisposable
{
    public void Dispose()
    {
        _httpClient?.Dispose();
        _mockHttpMessageHandler?.Dispose();
    }
}
```

## Testing Scenarios

### ✅ Success Cases
- Valid HTTP responses
- JSON deserialization
- Different status codes (200, 201, etc.)
- Large responses
- Empty responses

### ❌ Error Cases
- HTTP errors (404, 500, etc.)
- Network timeouts
- Invalid URLs
- Malformed JSON
- Connection failures

### 🔧 Edge Cases
- Null/empty parameters
- Very large responses
- Concurrent requests
- Memory usage

## Best Practices

### 1. Make Code Testable
```csharp
// ❌ Hard to test (static HttpClient)
private static readonly HttpClient httpClient = new HttpClient();

// ✅ Easy to test (dependency injection)
public class HttpService
{
    private readonly HttpClient _httpClient;
    public HttpService(HttpClient httpClient) => _httpClient = httpClient;
}
```

### 2. Use Mocking for External Dependencies
```csharp
// ✅ Mock HTTP calls
_mockHttpMessageHandler
    .When("https://api.example.com/data")
    .Respond("application/json", "{ \"result\": \"success\" }");
```

### 3. Test Error Scenarios
```csharp
// ✅ Test all possible outcomes
[Theory]
[InlineData(HttpStatusCode.BadRequest)]
[InlineData(HttpStatusCode.NotFound)]
[InlineData(HttpStatusCode.InternalServerError)]
public async Task GetData_WithErrorCodes_ThrowsException(HttpStatusCode statusCode)
```

### 4. Proper Resource Management
```csharp
// ✅ Implement IDisposable for cleanup
public void Dispose()
{
    _httpClient?.Dispose();
    _mockHttpMessageHandler?.Dispose();
}
```

### 5. Meaningful Test Names
```csharp
// ❌ Poor naming
[Fact] public void Test1() { }

// ✅ Descriptive naming
[Fact] public async Task GetPostAsync_WithValidId_ReturnsExpectedJson() { }
```

## Running Tests

```bash
# Run all tests
dotnet test

# Run with detailed output
dotnet test --verbosity normal

# Run specific test class
dotnet test --filter "HttpServiceTests"

# Run specific test method
dotnet test --filter "GetPostAsync_WithValidPostId_ReturnsExpectedJson"

# Generate code coverage
dotnet test --collect:"XPlat Code Coverage"
```

## Common Assertions for HTTP Tests

```csharp
// Response validation
Assert.NotNull(result);
Assert.NotEmpty(result);

// JSON structure validation
Assert.StartsWith("{", result.Trim());
Assert.EndsWith("}", result.Trim());
Assert.Contains("\"id\":", result);

// Content validation
Assert.True(result.Length > 50);

// Exception validation
var exception = await Assert.ThrowsAsync<HttpRequestException>(() => method());
Assert.Contains("expected message", exception.Message);

// Collection validation
Assert.All(items, item => Assert.NotNull(item));
```

## Refactoring for Testability

### Original Method (Hard to Test)
```csharp
private static async Task<string> BasicXUnitTestExample()
{
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
```

### Refactored for Testing
```csharp
public interface IHttpService
{
    Task<string> GetPostAsync(string url);
}

public class HttpService : IHttpService
{
    private readonly HttpClient _httpClient;
    
    public HttpService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }
    
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
            throw; // Re-throw to allow caller to handle
        }
        catch (TaskCanceledException)
        {
            throw; // Re-throw to allow caller to handle
        }
    }
}
```

## Memory Considerations

```csharp
[Fact]
public async Task HttpRequest_MemoryUsage_IsReasonable()
{
    // Measure memory before
    long memoryBefore = GC.GetTotalMemory(true);
    
    // Perform HTTP operation
    string result = await _httpService.GetPostAsync(1);
    
    // Force garbage collection
    GC.Collect();
    GC.WaitForPendingFinalizers();
    GC.Collect();
    
    // Measure memory after
    long memoryAfter = GC.GetTotalMemory(false);
    long memoryUsed = memoryAfter - memoryBefore;
    
    // Assert memory usage is reasonable
    Assert.True(memoryUsed < 1_000_000); // Less than 1MB
}
```

This comprehensive testing approach ensures your HTTP methods are reliable, maintainable, and thoroughly tested.