using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SystemThreadingTasksExamples
{
    /// <summary>
    /// Comprehensive examples of System.Threading.Tasks namespace
    /// 
    /// PURPOSE:
    /// - System.Threading.Tasks provides Task-based Asynchronous Pattern (TAP)
    /// - Enables asynchronous and parallel programming
    /// - Allows non-blocking operations and better resource utilization
    /// - Provides abstractions over threading complexity
    /// 
    /// SCENARIOS TO USE:
    /// - I/O-bound operations (file access, network calls, database operations)
    /// - CPU-intensive operations that can be parallelized
    /// - UI applications to prevent blocking the main thread
    /// - Server applications to handle multiple concurrent requests
    /// - Background processing and periodic tasks
    /// 
    /// SCENARIOS NOT TO USE:
    /// - Very short-running operations (overhead > benefit)
    /// - Operations that require sequential execution
    /// - When working with thread-unsafe resources without proper synchronization
    /// - Simple calculations that complete quickly
    /// - When the calling context must remain synchronous
    /// 
    /// MEMORY ALLOCATION:
    /// - Tasks allocate memory on the heap
    /// - Async state machines create additional allocations
    /// - Consider ValueTask for high-frequency, often-synchronous operations
    /// - Task pooling helps reduce allocations
    /// 
    /// MULTITHREAD ASPECTS:
    /// - Tasks may execute on different threads via ThreadPool
    /// - ConfigureAwait(false) prevents context switching overhead
    /// - Synchronization primitives needed for shared state
    /// - Race conditions and deadlocks are possible
    /// </summary>
    class Program
    {
        private static readonly HttpClient httpClient = new HttpClient();
        
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== System.Threading.Tasks Examples ===\n");
            
            // Example 1: Basic Task Creation and Execution
            await BasicTaskExamples();
            
            // Example 2: Task.Run for CPU-bound work
            await CpuBoundTaskExamples();
            
            // Example 3: Async/Await pattern
            await AsyncAwaitExamples();
            
            // Example 4: Task Parallel Library (TPL)
            await ParallelTaskExamples();
            
            // Example 5: Task Synchronization
            await TaskSynchronizationExamples();
            
            // Example 6: Exception Handling
            await ExceptionHandlingExamples();
            
            // Example 7: Cancellation
            await CancellationExamples();
            
            // Example 8: Memory and Performance Considerations
            await MemoryPerformanceExamples();
            
            Console.WriteLine("\n=== All Examples Completed ===");
        }

        #region Basic Task Examples
        
        /// <summary>
        /// Demonstrates basic Task creation, starting, and waiting
        /// 
        /// MEMORY ALLOCATION:
        /// - Each Task allocates ~96 bytes on heap
        /// - Task.Run queues work to ThreadPool, minimal additional allocation
        /// 
        /// MULTITHREAD ASPECTS:
        /// - Tasks execute on ThreadPool threads
        /// - Multiple tasks can run concurrently on different threads
        /// </summary>
        static async Task BasicTaskExamples()
        {
            Console.WriteLine("--- Basic Task Examples ---");
            
            // Creating a task that returns a value
            // GOOD FOR: Operations that produce a result asynchronously
            Task<int> taskWithResult = Task.Run(() =>
            {
                Console.WriteLine($"Task executing on thread: {Thread.CurrentThread.ManagedThreadId}");
                Thread.Sleep(1000); // Simulate work
                return 42;
            });
            
            // Creating a task without return value
            // GOOD FOR: Fire-and-forget operations, background work
            Task taskWithoutResult = Task.Run(() =>
            {
                Console.WriteLine($"Background task on thread: {Thread.CurrentThread.ManagedThreadId}");
                Thread.Sleep(500);
            });
            
            // Wait for completion and get result
            int result = await taskWithResult;
            Console.WriteLine($"Task result: {result}");
            
            await taskWithoutResult;
            Console.WriteLine("Background task completed\n");
        }
        
        #endregion

        #region CPU-Bound Task Examples
        
        /// <summary>
        /// Demonstrates Task.Run for CPU-intensive operations
        /// 
        /// SCENARIOS TO USE:
        /// - Mathematical calculations
        /// - Data processing
        /// - Image/video processing
        /// - Cryptographic operations
        /// 
        /// MEMORY ALLOCATION:
        /// - Task overhead for each Task.Run call
        /// - Consider Parallel.For for simple loops instead
        /// </summary>
        static async Task CpuBoundTaskExamples()
        {
            Console.WriteLine("--- CPU-Bound Task Examples ---");

            // CPU-intensive calculation
            // GOOD FOR: Moving heavy computation off the main thread
            var stopwatch = Stopwatch.StartNew();
            
            int fiboParam = 2; // Increased for more intensive calculation

            Task<long> fibonacciTask = Task.Run(() => CalculateFibonacci(fiboParam));

            // Do other work while calculation runs
            Console.WriteLine("Fibonacci calculation started, doing other work...");
            await Task.Delay(100); // Simulate other work
            
            long fibResult = await fibonacciTask;
            stopwatch.Stop();

            Console.WriteLine($"Fibonacci({fiboParam}) = {fibResult}");
            Console.WriteLine($"Calculation took: {stopwatch.ElapsedMilliseconds}ms");
            Console.WriteLine($"Main thread ID: {Thread.CurrentThread.ManagedThreadId}\n");
        }
        
        /// <summary>
        /// Recursive Fibonacci calculation (intentionally inefficient for demonstration)
        /// </summary>
        static long CalculateFibonacci(int n)
        {
            Console.WriteLine($"Fibonacci calculation on thread: {Thread.CurrentThread.ManagedThreadId}");
            if (n <= 1) return n;
            return CalculateFibonacci(n - 1) + CalculateFibonacci(n - 2);
        }
        
        #endregion

        #region Async/Await Examples
        
        /// <summary>
        /// Demonstrates async/await pattern for I/O-bound operations
        /// 
        /// SCENARIOS TO USE:
        /// - File I/O operations
        /// - Network requests
        /// - Database queries
        /// - Any I/O-bound operation
        /// 
        /// MEMORY ALLOCATION:
        /// - Async state machine allocation (~100-200 bytes)
        /// - Local variables captured in state machine
        /// - Consider ValueTask for frequently-synchronous operations
        /// 
        /// MULTITHREAD ASPECTS:
        /// - await may resume on different thread
        /// - ConfigureAwait(false) prevents synchronization context capture
        /// </summary>
        static async Task AsyncAwaitExamples()
        {
            Console.WriteLine("--- Async/Await Examples ---");
            
            // I/O-bound operation - reading a file
            // GOOD FOR: Non-blocking file access
            string content = await ReadFileAsync("example.txt");
            Console.WriteLine($"File read on thread: {Thread.CurrentThread.ManagedThreadId}");
            
            // Multiple concurrent I/O operations
            // GOOD FOR: Improving throughput with concurrent operations
            Task<string> webContent1 = FetchWebContentAsync("https://httpbin.org/delay/1");
            Task<string> webContent2 = FetchWebContentAsync("https://httpbin.org/delay/1");
            Task<string> webContent3 = FetchWebContentAsync("https://httpbin.org/delay/1");
            
            // Wait for all to complete concurrently
            string[] results = await Task.WhenAll(webContent1, webContent2, webContent3);
            Console.WriteLine($"Fetched {results.Length} web resources concurrently");
            Console.WriteLine($"After await on thread: {Thread.CurrentThread.ManagedThreadId}\n");
        }
        
        /// <summary>
        /// Simulates reading a file asynchronously
        /// </summary>
        static async Task<string> ReadFileAsync(string fileName)
        {
            // Simulate file I/O
            await Task.Delay(500); // Represents I/O wait time
            return $"Content of {fileName}";
        }
        
        /// <summary>
        /// Demonstrates network I/O with proper ConfigureAwait usage
        /// </summary>
        static async Task<string> FetchWebContentAsync(string url)
        {
            try
            {
                // ConfigureAwait(false) for library code - avoids capturing synchronization context. Explain this
                // GOOD FOR: Library code that doesn't need to resume on original context
                // Example: In a UI app, this prevents deadlocks and improves performance
                // If true, continue on the captured context (same thread or context as before the await)
                // In console apps, it has less impact but is still a good practice for libraries
                // MEMORY BENEFIT: Reduces allocations in some scenarios
                // MULTITHREAD BENEFIT: Allows continuation on any thread
                var response = await httpClient.GetStringAsync(url).ConfigureAwait(false);
                return $"Fetched {response.Length} characters from {url}";
            }
            catch (Exception ex)
            {
                return $"Error fetching {url}: {ex.Message}";
            }
        }
        
        #endregion

        #region Parallel Task Examples
        
        /// <summary>
        /// Demonstrates Task Parallel Library (TPL) for parallel execution
        /// 
        /// SCENARIOS TO USE:
        /// - Processing collections in parallel
        /// - CPU-bound operations that can be divided
        /// - Independent operations that can run concurrently
        /// 
        /// SCENARIOS NOT TO USE:
        /// - Small collections (overhead > benefit)
        /// - Operations with dependencies
        /// - When order of execution matters
        /// 
        /// MEMORY ALLOCATION:
        /// - Parallel.ForEach partitions data, some overhead
        /// - Task.WhenAll creates array of tasks
        /// - Consider memory usage with large datasets
        /// </summary>
        static async Task ParallelTaskExamples()
        {
            Console.WriteLine("--- Parallel Task Examples ---");
            
            // Parallel processing of a collection
            var numbers = new List<int>();
            for (int i = 1; i <= 10; i++)
                numbers.Add(i);
            
            // Process items in parallel
            // GOOD FOR: CPU-intensive operations on collections
            var parallelResults = new List<Task<int>>();
            
            foreach (var number in numbers)
            {
                // Create task for each item
                parallelResults.Add(Task.Run(() => ProcessNumber(number)));
            }
            
            // Wait for all to complete
            int[] results = await Task.WhenAll(parallelResults);
            Console.WriteLine($"Processed {results.Length} numbers in parallel");
            Console.WriteLine($"Sum of results: {results.Sum()}");
            
            // Alternative: Parallel.ForEach for simple scenarios
            // GOOD FOR: Simple operations without return values
            var threadIds = new List<int>();
            var lockObject = new object();
            
            Parallel.ForEach(numbers, number =>
            {
                var threadId = Thread.CurrentThread.ManagedThreadId;
                lock (lockObject) // Synchronization needed for shared state
                {
                    threadIds.Add(threadId);
                }
                Thread.Sleep(100); // Simulate work
            });
            
            Console.WriteLine($"Parallel.ForEach used {threadIds.Distinct().Count()} different threads");
            Console.WriteLine();
        }
        
        /// <summary>
        /// Simulates processing a number
        /// </summary>
        static int ProcessNumber(int number)
        {
            Console.WriteLine($"Processing {number} on thread {Thread.CurrentThread.ManagedThreadId}");
            Thread.Sleep(200); // Simulate CPU work
            return number * number;
        }
        
        #endregion

        #region Task Synchronization Examples
        
        /// <summary>
        /// Demonstrates synchronization patterns with tasks
        /// 
        /// MULTITHREAD ASPECTS:
        /// - Multiple tasks may access shared resources
        /// - Synchronization primitives prevent race conditions
        /// - SemaphoreSlim provides async-compatible throttling
        /// - TaskCompletionSource enables custom async patterns
        /// 
        /// MEMORY ALLOCATION:
        /// - Synchronization primitives have minimal overhead
        /// - SemaphoreSlim is lightweight compared to Semaphore
        /// </summary>
        static async Task TaskSynchronizationExamples()
        {
            Console.WriteLine("--- Task Synchronization Examples ---");
            
            // Throttling concurrent operations with SemaphoreSlim
            // GOOD FOR: Limiting concurrent database connections, API calls
            var semaphore = new SemaphoreSlim(2, 2); // Allow max 2 concurrent operations
            var tasks = new List<Task>();
            
            for (int i = 1; i <= 5; i++)
            {
                int taskId = i;
                tasks.Add(ThrottledOperation(taskId, semaphore));
            }

            await Task.WhenAll(tasks);
            
            // TaskCompletionSource for custom async patterns
            // GOOD FOR: Converting callback-based APIs to async
            var tcs = new TaskCompletionSource<string>();
            
            // Simulate async operation with callback
            Timer timer = new Timer(_ =>
            {
                tcs.SetResult("Timer completed!");
            }, null, 1000, Timeout.Infinite);

            string timerResult = await tcs.Task;
            // todo: this is so nice - explain
            // The TaskCompletionSource allows us to create a Task that can be completed manually.
            // This is useful for bridging the gap between callback-based code and async/await.
            // In this example, we simulate a timer that completes the Task after 1 second.

            Console.WriteLine(timerResult);
            timer.Dispose();
            
            Console.WriteLine();
        }
        
        /// <summary>
        /// Demonstrates throttled operation using SemaphoreSlim
        /// </summary>
        static async Task ThrottledOperation(int id, SemaphoreSlim semaphore)
        {
            await semaphore.WaitAsync(); // Wait for available slot
            try
            {
                Console.WriteLine($"Operation {id} started on thread {Thread.CurrentThread.ManagedThreadId}");
                await Task.Delay(1000); // Simulate work
                Console.WriteLine($"Operation {id} completed");
            }
            finally
            {
                semaphore.Release(); // Always release the semaphore
            }
        }
        
        #endregion

        #region Exception Handling Examples
        
        /// <summary>
        /// Demonstrates exception handling in async operations
        /// 
        /// SCENARIOS TO USE:
        /// - Graceful degradation in distributed systems
        /// - Retry logic for transient failures
        /// - Aggregate exception handling for parallel operations
        /// 
        /// MULTITHREAD ASPECTS:
        /// - Exceptions cross thread boundaries via Tasks
        /// - AggregateException for multiple failed tasks
        /// - Unhandled exceptions can terminate application
        /// </summary>
        static async Task ExceptionHandlingExamples()
        {
            Console.WriteLine("--- Exception Handling Examples ---");
            
            // Handling single task exception
            try
            {
                await FailingTask();
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Caught exception: {ex.Message}");
            }
            
            // Handling multiple task exceptions
            var failingTasks = new List<Task>
            {
                FailingTaskWithDelay(1, 100),
                FailingTaskWithDelay(2, 200),
                SuccessfulTask(3, 150)
            };
            
            try
            {
                await Task.WhenAll(failingTasks);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"First exception: {ex.Message}");
                
                // To see all exceptions, inspect the tasks individually
                foreach (var task in failingTasks)
                {
                    if (task.IsFaulted && task.Exception != null)
                    {
                        foreach (var innerEx in task.Exception.InnerExceptions)
                        {
                            Console.WriteLine($"Task exception: {innerEx.Message}");
                        }
                    }
                }
            }
            
            Console.WriteLine();
        }
        
        static async Task FailingTask()
        {
            await Task.Delay(100);
            throw new InvalidOperationException("This task always fails");
        }
        
        static async Task FailingTaskWithDelay(int id, int delay)
        {
            await Task.Delay(delay);
            throw new InvalidOperationException($"Task {id} failed");
        }
        
        static async Task SuccessfulTask(int id, int delay)
        {
            await Task.Delay(delay);
            Console.WriteLine($"Task {id} completed successfully");
        }
        
        #endregion

        #region Cancellation Examples
        
        /// <summary>
        /// Demonstrates cancellation patterns with CancellationToken
        /// 
        /// SCENARIOS TO USE:
        /// - User-initiated cancellation (UI scenarios)
        /// - Timeout-based cancellation
        /// - Graceful shutdown scenarios
        /// - Resource cleanup in long-running operations
        /// 
        /// MEMORY ALLOCATION:
        /// - CancellationTokenSource allocates ~48 bytes
        /// - Linked tokens create additional allocations
        /// - Proper disposal prevents memory leaks
        /// 
        /// MULTITHREAD ASPECTS:
        /// - Cancellation is thread-safe
        /// - OperationCanceledException propagates across await boundaries
        /// - Cooperative cancellation requires checking token periodically
        /// </summary>
        static async Task CancellationExamples()
        {
            Console.WriteLine("--- Cancellation Examples ---");
            
            // Timeout-based cancellation
            // GOOD FOR: Preventing operations from running indefinitely
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
            
            try
            {
                await LongRunningOperation(cts.Token);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Operation was cancelled due to timeout");
            }
            
            // Manual cancellation
            using var manualCts = new CancellationTokenSource();
            
            // Start a long-running task
            var longTask = VeryLongOperation(manualCts.Token);
            
            // Cancel after short delay
            _ = Task.Run(async () =>
            {
                await Task.Delay(500);
                manualCts.Cancel();
                Console.WriteLine("Cancellation requested");
            });
            
            try
            {
                await longTask;
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Long operation was cancelled");
            }
            
            Console.WriteLine();
        }
        
        /// <summary>
        /// Simulates a long-running operation that supports cancellation
        /// </summary>
        static async Task LongRunningOperation(CancellationToken cancellationToken)
        {
            for (int i = 0; i < 10; i++)
            {
                // Check for cancellation periodically
                cancellationToken.ThrowIfCancellationRequested();
                
                Console.WriteLine($"Working... step {i + 1}");
                await Task.Delay(500, cancellationToken); // Delay also supports cancellation
            }
            Console.WriteLine("Long operation completed");
        }

        static async Task VeryLongOperation(CancellationToken cancellationToken)
        {
            Console.WriteLine("Very long operation started");
            await Task.Delay(5000, cancellationToken); // 5 second delay
            Console.WriteLine("Very long operation completed");
        }
        
        #endregion

        #region Memory and Performance Examples
        
        /// <summary>
        /// Demonstrates memory and performance considerations
        /// 
        /// MEMORY ALLOCATION CONCERNS:
        /// - Task<T> allocates ~96 bytes per instance
        /// - Async state machines allocate ~100-200 bytes
        /// - ValueTask<T> reduces allocations for synchronous paths
        /// - Object pooling can help with high-frequency scenarios
        /// 
        /// PERFORMANCE CONSIDERATIONS:
        /// - Task.Run has overhead - don't use for trivial operations
        /// - ConfigureAwait(false) reduces context switching overhead (todo: explain this)
        /// - Parallel operations should exceed coordination overhead
        /// - Consider ThreadPool exhaustion under high load
        /// </summary>
        static async Task MemoryPerformanceExamples()
        {
            Console.WriteLine("--- Memory and Performance Examples ---");
            
            // Demonstrating ValueTask for potentially synchronous operations
            // GOOD FOR: High-frequency operations that often complete synchronously
            var cache = new Dictionary<int, string>();
            
            // First call will be async (cache miss)
            string result1 = await GetCachedValueAsync(1, cache);
            Console.WriteLine($"First call result: {result1}");
            
            // Second call will be synchronous (cache hit) - ValueTask reduces allocation
            string result2 = await GetCachedValueAsync(1, cache);
            Console.WriteLine($"Second call result: {result2}");
            
            // Demonstrating Task.Yield for cooperative multitasking
            // GOOD FOR: Yielding control in long-running synchronous operations
            await CooperativeOperation();
            
            // Memory pressure demonstration
            await MemoryPressureExample();
            
            Console.WriteLine();
        }
        
        /// <summary>
        /// Demonstrates ValueTask usage for cache scenarios
        /// Reduces allocations when result is immediately available
        /// </summary>
        static async ValueTask<string> GetCachedValueAsync(int key, Dictionary<int, string> cache)
        {
            if (cache.TryGetValue(key, out string? cachedValue))
            {
                // Synchronous path - ValueTask doesn't allocate
                Console.WriteLine("Cache hit - synchronous return");
                return cachedValue;
            }
            
            // Asynchronous path - simulates expensive operation
            Console.WriteLine("Cache miss - async operation");
            await Task.Delay(100);
            
            string value = $"Value for key {key}";
            cache[key] = value;
            return value;
        }
        
        /// <summary>
        /// Demonstrates cooperative multitasking with Task.Yield
        /// </summary>
        static async Task CooperativeOperation()
        {
            Console.WriteLine("Starting cooperative operation");
            
            for (int i = 0; i < 5; i++)
            {
                // Do some work
                Console.WriteLine($"Work iteration {i + 1}");
                
                // Yield control to allow other tasks to run
                // GOOD FOR: Preventing long-running sync operations from blocking
                await Task.Yield();
            }
            
            Console.WriteLine("Cooperative operation completed");
        }
        
        /// <summary>
        /// Demonstrates memory considerations with many tasks
        /// </summary>
        static async Task MemoryPressureExample()
        {
            Console.WriteLine("Creating many tasks to demonstrate memory usage...");
            
            // DON'T DO THIS: Creating too many tasks can exhaust memory
            // Better to use Parallel.ForEach or limit concurrency
            const int taskCount = 1000;
            var tasks = new List<Task>(taskCount);
            
            // Use SemaphoreSlim to limit concurrency
            using var semaphore = new SemaphoreSlim(Environment.ProcessorCount * 2);
            
            for (int i = 0; i < taskCount; i++)
            {
                tasks.Add(LimitedConcurrencyTask(i, semaphore));
            }
            
            await Task.WhenAll(tasks);
            Console.WriteLine($"Completed {taskCount} tasks with limited concurrency");
        }
        
        static async Task LimitedConcurrencyTask(int id, SemaphoreSlim semaphore)
        {
            await semaphore.WaitAsync();
            try
            {
                // Minimal work to demonstrate pattern
                await Task.Delay(1);
            }
            finally
            {
                semaphore.Release();
            }
        }
        
        #endregion
    }
}
