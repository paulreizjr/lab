/*
 * CancellationToken Examples - Comprehensive Guide
 * 
 * PURPOSE:
 * CancellationToken is a struct that represents a notification that operations should be canceled.
 * It provides a cooperative cancellation mechanism for long-running operations, allowing them to
 * check periodically if cancellation has been requested and respond gracefully.
 * 
 * SCENARIOS TO USE:
 * - Long-running operations (file I/O, network requests, CPU-intensive tasks)
 * - Operations that need to be canceled based on user input or timeout
 * - Background tasks that should stop when the application shuts down
 * - Async operations in web applications (HTTP requests with timeouts)
 * - Parallel operations where you want to cancel all if one fails
 * 
 * SCENARIOS NOT TO USE:
 * - Very short operations (overhead > benefit)
 * - Operations that cannot be safely interrupted
 * - Critical system operations that must complete
 * - Simple synchronous calculations
 * 
 * MEMORY ALLOCATION:
 * CancellationToken is a lightweight struct (value type) - no heap allocation for the token itself.
 * However, CancellationTokenSource (which creates tokens) is a class and allocates on the heap.
 * Registering callbacks with token.Register() allocates memory for callback storage.
 * Best practice: reuse CancellationTokenSource when possible, avoid excessive callback registrations.
 */

using System;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== CancellationToken Examples ===\n");
        
        // Example 1: Basic usage with timeout
        await Example1_BasicTimeoutCancellation();
        
        // Example 2: User-initiated cancellation
        await Example2_UserInitiatedCancellation();
        
        // Example 3: Cancellation with cleanup
        await Example3_CancellationWithCleanup();
        
        // Example 4: Multiple operations with shared cancellation
        await Example4_SharedCancellation();
        
        // Example 5: Linked cancellation tokens
        await Example5_LinkedCancellationTokens();
        
        // Example 6: File I/O with cancellation
        await Example6_FileOperationWithCancellation();
        
        Console.WriteLine("\n=== All examples completed ===");
    }

    /// <summary>
    /// Example 1: Basic timeout-based cancellation
    /// Demonstrates the most common use case - canceling operations that take too long
    /// </summary>
    static async Task Example1_BasicTimeoutCancellation()
    {
        Console.WriteLine("--- Example 1: Basic Timeout Cancellation ---");
        
        // Create a cancellation token source with 3-second timeout
        // Memory: CancellationTokenSource allocates on heap (~200 bytes)
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
        
        try
        {
            // Pass the token to the long-running operation
            await LongRunningTask("Task 1", cts.Token);
            Console.WriteLine("Task completed successfully");
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Task was canceled due to timeout");
        }
        
        Console.WriteLine();
    }

    /// <summary>
    /// Example 2: User-initiated cancellation using Console input
    /// Shows how to allow users to cancel operations manually
    /// </summary>
    static async Task Example2_UserInitiatedCancellation()
    {
        Console.WriteLine("--- Example 2: User-Initiated Cancellation ---");
        Console.WriteLine("Press 'c' to cancel the operation...");
        
        // Create cancellation token source without timeout
        using var cts = new CancellationTokenSource();
        
        // Start a task to monitor for user input
        var inputTask = Task.Run(() =>
        {
            while (!cts.Token.IsCancellationRequested)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true);
                    if (key.KeyChar == 'c' || key.KeyChar == 'C')
                    {
                        Console.WriteLine("\nUser requested cancellation...");
                        cts.Cancel(); // Request cancellation
                        break;
                    }
                }
                Thread.Sleep(100); // Small delay to reduce CPU usage
            }
        });
        
        try
        {
            await LongRunningTask("User-Cancellable Task", cts.Token);
            Console.WriteLine("Task completed successfully");
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Task was canceled by user");
        }
        
        Console.WriteLine();
    }

    /// <summary>
    /// Example 3: Cancellation with proper cleanup using callbacks
    /// Demonstrates how to perform cleanup when cancellation occurs
    /// </summary>
    static async Task Example3_CancellationWithCleanup()
    {
        Console.WriteLine("--- Example 3: Cancellation with Cleanup ---");
        
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
        
        // Register cleanup callback
        // Memory: Each registration allocates memory for callback storage (~50-100 bytes)
        var registration = cts.Token.Register(() =>
        {
            Console.WriteLine("Cleanup: Performing cleanup operations...");
            // Simulate cleanup work
            Thread.Sleep(500);
            Console.WriteLine("Cleanup: Resources released");
        });
        
        try
        {
            await TaskWithResources(cts.Token);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Task canceled - cleanup callback was executed");
        }
        finally
        {
            // Always dispose registration to free memory
            registration.Dispose();
        }
        
        Console.WriteLine();
    }

    /// <summary>
    /// Example 4: Multiple operations sharing the same cancellation token
    /// Shows how to coordinate cancellation across multiple operations
    /// </summary>
    static async Task Example4_SharedCancellation()
    {
        Console.WriteLine("--- Example 4: Shared Cancellation ---");
        
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(4));
        
        // Start multiple tasks with the same cancellation token
        var tasks = new[]
        {
            LongRunningTask("Shared Task 1", cts.Token),
            LongRunningTask("Shared Task 2", cts.Token),
            LongRunningTask("Shared Task 3", cts.Token)
        };
        
        try
        {
            // Wait for all tasks to complete
            await Task.WhenAll(tasks);
            Console.WriteLine("All tasks completed successfully");
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("All tasks were canceled");
        }
        
        Console.WriteLine();
    }

    /// <summary>
    /// Example 5: Linked cancellation tokens
    /// Demonstrates how to create hierarchical cancellation where child operations
    /// can be canceled independently or with their parent
    /// </summary>
    static async Task Example5_LinkedCancellationTokens()
    {
        Console.WriteLine("--- Example 5: Linked Cancellation Tokens ---");
        
        // Parent cancellation token with 6-second timeout
        using var parentCts = new CancellationTokenSource(TimeSpan.FromSeconds(6));
        
        // Child cancellation token with 3-second timeout, linked to parent
        // Memory: CreateLinkedTokenSource creates additional heap allocation
        using var childCts = CancellationTokenSource.CreateLinkedTokenSource(
            parentCts.Token, 
            new CancellationTokenSource(TimeSpan.FromSeconds(3)).Token);
        
        try
        {
            // This task will be canceled by whichever timeout occurs first (child's 3s)
            await LongRunningTask("Child Task (3s timeout)", childCts.Token);
            Console.WriteLine("Child task completed");
        }
        catch (OperationCanceledException)
        {
            if (parentCts.Token.IsCancellationRequested)
                Console.WriteLine("Canceled by parent token");
            else
                Console.WriteLine("Canceled by child token");
        }
        
        Console.WriteLine();
    }

    /// <summary>
    /// Example 6: File I/O operations with cancellation
    /// Real-world example showing cancellation in file operations
    /// </summary>
    static async Task Example6_FileOperationWithCancellation()
    {
        Console.WriteLine("--- Example 6: File I/O with Cancellation ---");
        
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
        string tempFilePath = Path.GetTempFileName();
        
        try
        {
            // Simulate writing large amounts of data to file
            await WriteToFileAsync(tempFilePath, cts.Token);
            Console.WriteLine($"File written successfully: {tempFilePath}");
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("File write operation was canceled");
            // Clean up partial file
            if (File.Exists(tempFilePath))
            {
                File.Delete(tempFilePath);
                Console.WriteLine("Partial file cleaned up");
            }
        }
        
        Console.WriteLine();
    }

    /// <summary>
    /// Simulates a long-running task that respects cancellation requests
    /// </summary>
    static async Task LongRunningTask(string taskName, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Starting {taskName}...");
        
        // Simulate work in chunks, checking for cancellation between chunks
        for (int i = 0; i < 10; i++)
        {
            // Check if cancellation was requested
            // This is the cooperative part - the operation voluntarily checks and responds
            cancellationToken.ThrowIfCancellationRequested();
            
            // Simulate work
            await Task.Delay(500, cancellationToken);
            
            Console.WriteLine($"{taskName}: Step {i + 1}/10 completed");
        }
        
        Console.WriteLine($"{taskName} completed successfully");
    }

    /// <summary>
    /// Simulates a task that uses resources requiring cleanup
    /// </summary>
    static async Task TaskWithResources(CancellationToken cancellationToken)
    {
        Console.WriteLine("Acquiring resources...");
        
        // Simulate resource acquisition
        await Task.Delay(500, cancellationToken);
        
        Console.WriteLine("Resources acquired, starting work...");
        
        // Simulate long work
        for (int i = 0; i < 5; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await Task.Delay(800, cancellationToken);
            Console.WriteLine($"Work progress: {i + 1}/5");
        }
    }

    /// <summary>
    /// Simulates writing data to a file with cancellation support
    /// </summary>
    static async Task WriteToFileAsync(string filePath, CancellationToken cancellationToken)
    {
        const int ChunkCount = 10;
        const int ChunkSize = 1024 * 100; // 100KB chunks
        
        using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true);
        
        var buffer = new byte[ChunkSize];
        new Random().NextBytes(buffer); // Fill with random data
        
        for (int i = 0; i < ChunkCount; i++)
        {
            // Check for cancellation before each write operation
            cancellationToken.ThrowIfCancellationRequested();
            
            // Write chunk to file
            await fileStream.WriteAsync(buffer, 0, buffer.Length, cancellationToken);
            
            Console.WriteLine($"Written chunk {i + 1}/{ChunkCount} ({(i + 1) * ChunkSize / 1024}KB)");
            
            // Simulate additional processing time
            await Task.Delay(300, cancellationToken);
        }
        
        await fileStream.FlushAsync(cancellationToken);
    }
}

/*
 * KEY TAKEAWAYS:
 * 
 * 1. MEMORY EFFICIENCY:
 *    - CancellationToken itself is a lightweight struct (16 bytes)
 *    - CancellationTokenSource is heavier (~200 bytes + overhead)
 *    - Each callback registration adds memory overhead
 *    - Always dispose CancellationTokenSource and registrations
 * 
 * 2. PERFORMANCE CONSIDERATIONS:
 *    - Check cancellation at reasonable intervals (not too frequently)
 *    - Use ThrowIfCancellationRequested() for simplicity
 *    - Pass tokens to async methods that support them (Task.Delay, Stream operations, etc.)
 * 
 * 3. BEST PRACTICES:
 *    - Always handle OperationCanceledException
 *    - Perform cleanup in finally blocks or using statements
 *    - Don't ignore cancellation requests in critical sections
 *    - Use timeouts to prevent indefinite waits
 *    - Consider linked tokens for hierarchical operations
 * 
 * 4. COMMON MISTAKES:
 *    - Not checking cancellation frequently enough
 *    - Ignoring OperationCanceledException
 *    - Forgetting to dispose CancellationTokenSource
 *    - Using cancellation for non-cooperative operations
 */
