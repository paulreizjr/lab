using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncExamples
{
    // ========================================
    // ASYNC/AWAIT PURPOSE AND DEFINITION
    // ========================================
    
    /*
     * PURPOSE OF ASYNC/AWAIT:
     * - Enables asynchronous programming to avoid blocking threads
     * - Improves application responsiveness and scalability
     * - Allows efficient use of system resources (threads, memory)
     * - Provides a simpler syntax for handling asynchronous operations
     * - Prevents UI freezing and improves server throughput
     */

    class Program
    {
        private static readonly HttpClient httpClient = new HttpClient();
        
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== C# ASYNC/AWAIT EXAMPLES ===\n");

            // ========================================
            // GOOD SCENARIOS TO USE ASYNC/AWAIT
            // ========================================
            
            /*
             * EXCELLENT SCENARIOS FOR ASYNC/AWAIT:
             * 1. I/O Operations: File reading/writing, database queries, web requests
             * 2. Network Operations: HTTP calls, API requests, downloading files
             * 3. UI Applications: Keeping UI responsive during long operations
             * 4. Server Applications: Handling many concurrent requests efficiently
             * 5. Timer-based Operations: Delays without blocking threads
             * 6. Resource-intensive Operations: When you can parallelize work
             */

            await BasicAsyncExample();
            await MultipleAsyncOperations();
            await AsyncWithErrorHandling();
            await ConfigureAwaitExample();
            await AsyncMemoryAndPerformance();
            
            Console.WriteLine("\n=== SCENARIOS NOT TO USE ASYNC/AWAIT ===");
            WhenNotToUseAsync();

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        static async Task BasicAsyncExample()
        {
            Console.WriteLine("--- Basic Async Example ---");
            
            /*
             * FUNDAMENTAL CONCEPTS:
             * - async keyword: Marks method as asynchronous
             * - await keyword: Asynchronously waits for Task completion
             * - Task: Represents an asynchronous operation
             * - Task<T>: Represents an async operation that returns a value
             */

            Console.WriteLine("Starting async operation...");
            var stopwatch = Stopwatch.StartNew();

            // This simulates I/O work without blocking the current thread
            string result = await SimulateAsyncWork("Database Query", 2000);

            /*
             * WHAT HAPPENS DURING AWAIT:
             * 
             * 1. YIELD CONTROL: When await is encountered, the method yields control back to caller
             * 2. STATE MACHINE: The async method is transformed into a state machine by compiler
             * 3. CONTINUATION: Code after await becomes a continuation (delegate) that runs when task completes
             * 4. THREAD FREEDOM: The original thread is freed to do other work while waiting
             * 5. CONTEXT RESUMPTION: When task completes, continuation resumes on appropriate context
             */

            /*
             * .NET THREAD POOL OVERVIEW:
             * 
             * WHAT IS IT?
             * - Managed pool of worker threads for executing tasks and background work
             * - Eliminates overhead of creating/destroying threads manually
             * - Handles asynchronous I/O operations and queued work items
             * 
             * HOW IT WORKS:
             * - Maintains reusable worker threads that handle multiple tasks
             * - Assigns queued tasks to available threads
             * - Creates new threads (up to limits) when all threads are busy
             * - Automatically manages thread lifecycle and resource allocation
             * 
             * DEFAULT SIZING:
             * - Minimum threads: Typically equals number of CPU cores
             * - Maximum threads: Often 1024+ (depends on system resources)
             * - Configurable via ThreadPool.SetMinThreads/SetMaxThreads
             */

            /*
             * CONTINUATION EXECUTION:
             * 
             * WHERE DOES THE CODE BELOW RUN?
             * - During await: Original thread is released, can do other work
             * - After completion: Continuation scheduled on thread pool thread
             * - Context matters: May resume on UI thread (with SynchronizationContext)
             *                   or on any thread pool thread (without context)
             * 
             * The code below this comment runs as a continuation on whatever thread
             * the thread pool assigns when the awaited task completes.
             */

            stopwatch.Stop();
            Console.WriteLine($"Result: {result}");
            Console.WriteLine($"Total time: {stopwatch.ElapsedMilliseconds}ms");
            Console.WriteLine("Main thread was not blocked during the wait!\n");
        }

        static async Task MultipleAsyncOperations()
        {
            Console.WriteLine("--- Multiple Async Operations ---");
            
            /*
             * CONCURRENT vs SEQUENTIAL EXECUTION:
             * - Sequential: await each operation one by one
             * - Concurrent: Start all operations, then await all results
             * - Use Task.WhenAll for concurrent execution
             */

            var stopwatch = Stopwatch.StartNew();

            // ❌ SEQUENTIAL (slower) - each operation waits for the previous
            Console.WriteLine("Sequential execution:");
            var seq1 = await SimulateAsyncWork("API Call 1", 1000);
            var seq2 = await SimulateAsyncWork("API Call 2", 1000);
            var seq3 = await SimulateAsyncWork("API Call 3", 1000);
            
            var sequentialTime = stopwatch.ElapsedMilliseconds;
            Console.WriteLine($"Sequential results: {seq1}, {seq2}, {seq3}");
            Console.WriteLine($"Sequential time: {sequentialTime}ms");

            // ✅ CONCURRENT (faster) - all operations run simultaneously
            stopwatch.Restart();
            Console.WriteLine("\nConcurrent execution:");
            
            // each task below goes to the thread pool and runs in parallel

            Task<string> task1 = SimulateAsyncWork("API Call 1", 1000);
            Task<string> task2 = SimulateAsyncWork("API Call 2", 1000);
            Task<string> task3 = SimulateAsyncWork("API Call 3", 1000);

            // Wait for all tasks to complete
            string[] results = await Task.WhenAll(task1, task2, task3);
            
            var concurrentTime = stopwatch.ElapsedMilliseconds;
            Console.WriteLine($"Concurrent results: {string.Join(", ", results)}");
            Console.WriteLine($"Concurrent time: {concurrentTime}ms");
            Console.WriteLine($"Time saved: {sequentialTime - concurrentTime}ms\n");
        }

        static async Task AsyncWithErrorHandling()
        {
            Console.WriteLine("--- Async Error Handling ---");
            
            /*
             * ERROR HANDLING IN ASYNC:
             * - Use try-catch blocks around await calls
             * - AggregateException for Task.WhenAll with multiple failures
             * - CancellationToken for cooperative cancellation
             * - TimeoutException for operations that take too long
             */

            // Example 1: Basic error handling
            try
            {
                await SimulateAsyncWorkWithError("Failing operation");
                Console.WriteLine("This won't be reached");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Caught exception: {ex.Message}");
            }

            // Example 2: Handling multiple concurrent operations with errors
            try
            {
                Task<string> goodTask = SimulateAsyncWork("Good operation", 500);
                Task<string> badTask = SimulateAsyncWorkWithError("Bad operation");
                
                await Task.WhenAll(goodTask, badTask);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"One or more operations failed: {ex.Message}");
            }

            // Example 3: Cancellation token usage
            using var cts = new CancellationTokenSource();
            cts.CancelAfter(1500); // Cancel after 1.5 seconds

            try
            {
                await SimulateAsyncWorkWithCancellation("Long operation", 3000, cts.Token);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Operation was cancelled due to timeout");
            }

            Console.WriteLine();
        }

        static async Task ConfigureAwaitExample()
        {
            Console.WriteLine("--- ConfigureAwait Example ---");
            
            /*
             * CONFIGUREAWAIT DETAILED EXPLANATION:
             * 
             * WHAT IS SYNCHRONIZATION CONTEXT?
             * - A mechanism that controls WHERE continuations (code after await) execute
             * - Different contexts: UI thread (WPF/WinForms), ASP.NET context, thread pool context
             * - Captured automatically by await to maintain thread affinity when needed
             * 
             * DEFAULT BEHAVIOR (ConfigureAwait(true) or no ConfigureAwait):
             * - await captures the current SynchronizationContext or TaskScheduler
             * - Continuation runs on the SAME context (e.g., UI thread returns to UI thread)
             * - Ensures UI updates happen on UI thread, maintains request context in web apps
             * 
             * WITH CONFIGUREAWAIT(FALSE):
             * - Tells await: "I don't care what thread the continuation runs on"
             * - Continuation may run on any available thread pool thread
             * - Faster because no context switching overhead
             * - BUT: Cannot safely update UI or access context-specific resources
             *   context-specific resources samples: UI elements, HttpContext, etc.
             * 
             * WHEN TO USE CONFIGUREAWAIT(FALSE):
             * ✅ Library code that doesn't need specific thread context
             * ✅ Background processing that doesn't touch UI
             * ✅ Performance-critical code where context switching is expensive
             * ✅ Code that might be called from UI thread (prevents deadlocks)
             * 
             * WHEN NOT TO USE CONFIGUREAWAIT(FALSE):
             * ❌ UI event handlers that need to update UI after await
             * ❌ ASP.NET controllers that need HttpContext after await
             * ❌ Code that accesses thread-specific resources after await
             * 
             * DEADLOCK PREVENTION:
             * - Classic deadlock: UI thread waits for async operation that tries to resume on
             *   busy UI thread
             * - ConfigureAwait(false) breaks this cycle by not requiring original thread
             * 
             * PERFORMANCE IMPACT:
             * - Context switching can add 10-100+ microseconds per await
             * - In high-throughput scenarios, this adds up significantly
             * - ConfigureAwait(false) eliminates this overhead
             */

            Console.WriteLine("Demonstrating ConfigureAwait usage:");

            // In library code, always use ConfigureAwait(false)
            // by library code I mean code that could be used by any application, like a NuGet package or shared utility

            await PerformLibraryOperation().ConfigureAwait(false);
            
            // In application code, you can omit ConfigureAwait or use true
            await PerformApplicationOperation();
            
            Console.WriteLine("ConfigureAwait examples completed\n");
        }

        static async Task AsyncMemoryAndPerformance()
        {
            Console.WriteLine("--- Async Memory and Performance ---");
            
            /*
             * MEMORY ALLOCATION IN ASYNC:
             * 
             * 1. STATE MACHINE: Compiler generates state machine for async methods
             * 2. HEAP ALLOCATION: Async state machine is allocated on heap
             * 3. BOXING: Local variables may be boxed in state machine
             * 4. TASK OVERHEAD: Each async operation creates Task object
             * 5. CONTEXT SWITCHING: Potential thread pool thread switching
             * 
             * PERFORMANCE CONSIDERATIONS:
             * - Async methods have overhead compared to synchronous versions
             * - Benefits outweigh costs for I/O-bound operations
             * - Avoid async for CPU-bound work unless using Task.Run
             * - Consider ValueTask for high-frequency, often-synchronous operations
             */

            Console.WriteLine("Memory allocation demonstration:");
            
            // Measure memory before async operations
            long memoryBefore = GC.GetTotalMemory(false);
            
            // Create multiple async operations to show allocation
            var tasks = new List<Task<string>>();
            for (int i = 0; i < 100; i++)
            {
                tasks.Add(SimulateAsyncWork($"Operation {i}", 10));
            }
            
            await Task.WhenAll(tasks);
            
            long memoryAfter = GC.GetTotalMemory(false);
            Console.WriteLine($"Memory allocated: {memoryAfter - memoryBefore} bytes");
            
            // Demonstrate ValueTask for potentially synchronous operations
            await ValueTaskExample();
            
            Console.WriteLine();
        }

        // ========================================
        // HELPER METHODS
        // ========================================

        static async Task<string> SimulateAsyncWork(string operation, int delayMs)
        {
            /*
             * Simulates I/O-bound work like:
             * - Database queries
             * - Web API calls  
             * - File operations
             * - Network requests
             */
            await Task.Delay(delayMs); // does this cause the task to go back to the thread pool? 
            // yes, it does. Task.Delay uses a timer internally and when the timer completes, it queues the continuation to the thread pool. 
            // This allows the current thread to be freed up while waiting for the delay to complete.
            return $"{operation} completed";
        }

        static async Task<string> SimulateAsyncWorkWithError(string operation)
        {
            await Task.Delay(100);
            throw new InvalidOperationException($"{operation} failed!");
        }

        static async Task<string> SimulateAsyncWorkWithCancellation(string operation, int delayMs, CancellationToken cancellationToken)
        {
            await Task.Delay(delayMs, cancellationToken);
            return $"{operation} completed";
        }

        static async Task PerformLibraryOperation()
        {
            // Library method - use ConfigureAwait(false)
            await Task.Delay(100).ConfigureAwait(false);
            Console.WriteLine("Library operation completed (ConfigureAwait false)");
        }

        static async Task PerformApplicationOperation()
        {
            // Application method - can capture context
            await Task.Delay(100);
            Console.WriteLine("Application operation completed");
        }

        static async ValueTask<int> ValueTaskExample()
        {
            /*
             * VALUETASK BENEFITS:
             * - Reduces allocations when result is often available synchronously
             * - Useful for caching scenarios or fast-path operations
             * - Should only be used when you have performance measurements proving benefit
             */
            
            // Simulate a cached result (synchronous path)
            if (DateTime.Now.Millisecond % 2 == 0)
            {
                Console.WriteLine("ValueTask: Synchronous path (no allocation)");
                return 42; // No Task allocation needed
            }
            
            // Async path when cache miss
            Console.WriteLine("ValueTask: Asynchronous path");
            await Task.Delay(50);
            return 42;
        }

        static void WhenNotToUseAsync()
        {
            /*
             * SCENARIOS NOT TO USE ASYNC/AWAIT:
             * 
             * 1. CPU-BOUND OPERATIONS: Pure computation without I/O
             *    ❌ Don't use for: Mathematical calculations, data processing, algorithms
             *    ✅ Use instead: Synchronous methods, or Task.Run for parallelization
             * 
             * 2. VERY SHORT OPERATIONS: Operations that complete in microseconds
             *    ❌ Don't use for: Memory access, simple property getters, basic calculations
             *    ✅ Use instead: Synchronous methods (async overhead not worth it)
             * 
             * 3. LIBRARY CONSTRUCTORS: Constructors cannot be async
             *    ❌ Don't use for: Object initialization that requires async work
             *    ✅ Use instead: Factory methods, lazy initialization, or async initialization pattern
             * 
             * 4. PROPERTY GETTERS: Properties should not perform I/O
             *    ❌ Don't use for: Property getters that need async operations
             *    ✅ Use instead: Async methods with clear names like GetDataAsync()
             * 
             * 5. FIRE-AND-FORGET: When you don't need to wait for completion
             *    ❌ Don't use await for: Logging, fire-and-forget operations
             *    ✅ Use instead: Task.Run without await, or dedicated background services
             * 
             * 6. SIMPLE SYNCHRONOUS OPERATIONS: When operation is inherently synchronous
             *    ❌ Don't use for: String manipulation, LINQ queries on in-memory data
             *    ✅ Use instead: Regular synchronous methods
             */

            Console.WriteLine("Examples of what NOT to use async/await for:");
            Console.WriteLine("❌ CPU-intensive calculations (no I/O benefit)");
            Console.WriteLine("❌ Property getters (should be fast and synchronous)");
            Console.WriteLine("❌ Constructors (cannot be async)");
            Console.WriteLine("❌ Very short operations (overhead > benefit)");
            Console.WriteLine("❌ Fire-and-forget operations (don't need await)");
            
            Console.WriteLine("\n✅ Better alternatives:");
            Console.WriteLine("  - Use Task.Run for CPU-bound work that needs parallelization");
            Console.WriteLine("  - Use synchronous methods for fast, non-I/O operations");
            Console.WriteLine("  - Use factory methods instead of async constructors");
            Console.WriteLine("  - Use background services for fire-and-forget work");
            Console.WriteLine("  - Consider caching for frequently accessed data");
            
            // Example of what NOT to do
            Console.WriteLine("\n--- Examples of Poor Async Usage ---");
            
            // ❌ BAD: Async method that doesn't do I/O
            Console.WriteLine("Don't do this - async method with no actual async work:");
            // BadAsyncExample(); // This would be poor design
            
            // ❌ BAD: Async property getter
            Console.WriteLine("Don't do this - async property getters");
            
            // ✅ GOOD: Appropriate usage
            Console.WriteLine("✅ Good: Use async for actual I/O operations");
        }

        // Example of BAD async usage (commented out because it's an anti-pattern)
        /*
        static async Task<int> BadAsyncExample()
        {
            // This is bad - no actual async work, just overhead
            return await Task.FromResult(42); // Don't do this!
        }
        */

        // ✅ GOOD: This should just be synchronous
        static int GoodSyncExample()
        {
            return 42; // Simple, fast, no I/O needed
        }
    }
}
