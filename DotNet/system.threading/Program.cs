/*
 * System.Threading Namespace Examples
 * 
 * PURPOSE:
 * The System.Threading namespace provides types and functionality for:
 * - Creating and managing threads
 * - Synchronizing access to shared resources
 * - Coordinating thread execution and communication
 * - Managing thread-safe operations and collections
 * - Implementing producer-consumer patterns
 * - Handling thread cancellation and timeouts
 * 
 * This enables writing concurrent and parallel applications that can utilize
 * multiple CPU cores effectively while maintaining data integrity and avoiding race conditions.
 */

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SystemThreadingExamples
{
    class Program
    {
        // Shared resources for demonstration
        private static int _sharedCounter = 0;
        private static readonly object _lockObject = new object();
        private static readonly Mutex _mutex = new Mutex();
        private static readonly Semaphore _semaphore = new Semaphore(2, 2); // Allow 2 concurrent access
        private static readonly AutoResetEvent _autoEvent = new AutoResetEvent(false);
        private static readonly ManualResetEvent _manualEvent = new ManualResetEvent(false);

        static void Main(string[] args)
        {
            Console.WriteLine("=== System.Threading Namespace Examples ===\n");

            // Run all threading examples
            BasicThreadExample();
            ThreadSynchronizationExample();
            MutexExample();
            SemaphoreExample();
            AutoResetEventExample();
            ManualResetEventExample();
            ThreadLocalExample();
            ThreadPoolExample();
            CancellationTokenExample();
            ProducerConsumerExample();
            MemoryAllocationExample();
            WhenToUseAndNotUseThreading();

            Console.WriteLine("\n=== End of Examples ===");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        #region Basic Thread Example

        /*
         * BASIC THREAD USAGE
         * 
         * SCENARIOS TO USE:
         * - Long-running background operations that don't need frequent communication
         * - CPU-intensive work that can be parallelized
         * - Independent tasks that can run concurrently
         * 
         * SCENARIOS NOT TO USE:
         * - Short-lived operations (thread creation overhead not worth it)
         * - I/O operations (use async/await instead)
         * - Operations that need frequent synchronization with main thread
         */
        static void BasicThreadExample()
        {
            Console.WriteLine("1. Basic Thread Example:");

            /*
             * MEMORY ALLOCATION NOTES:
             * - Each thread allocates ~1MB stack space by default
             * - Thread objects themselves are small (~few KB)
             * - Context switching has CPU overhead
             * - Too many threads can cause memory pressure and performance degradation
             */

            // Create and start a background thread
            Thread backgroundThread = new Thread(() =>
            {
                for (int i = 0; i < 5; i++)
                {
                    Console.WriteLine($"Background thread: {i} (Thread ID: {Thread.CurrentThread.ManagedThreadId})");
                    Thread.Sleep(1000); // Simulate work
                }
            })
            {
                Name = "BackgroundWorker",
                IsBackground = true // Dies when main thread exits
            };

            Console.WriteLine($"Main thread ID: {Thread.CurrentThread.ManagedThreadId}");
            backgroundThread.Start();

            // Do work on main thread while background thread runs
            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine($"Main thread: {i}");
                Thread.Sleep(800);
            }

            backgroundThread.Join(); // Wait for background thread to complete
            Console.WriteLine("Basic thread example completed.\n");
        }

        #endregion

        #region Thread Synchronization with Lock

        /*
         * THREAD SYNCHRONIZATION WITH LOCK
         * 
         * SCENARIOS TO USE:
         * - Protecting shared mutable state from race conditions
         * - Ensuring atomic operations on shared data
         * - Coordinating access to non-thread-safe resources
         * 
         * SCENARIOS NOT TO USE:
         * - Protecting immutable data (no synchronization needed)
         * - Single-threaded scenarios
         * - When using thread-safe collections (ConcurrentQueue, etc.)
         */
        static void ThreadSynchronizationExample()
        {
            Console.WriteLine("2. Thread Synchronization (Lock) Example:");

            const int numThreads = 10;
            const int incrementsPerThread = 10000;
            Thread[] threads = new Thread[numThreads];

            /*
             * MEMORY ALLOCATION NOTES:
             * - lock statement uses Monitor internally (no additional allocation)
             * - Object used for locking should be private and readonly
             * - Each waiting thread consumes stack space while blocked
             * - Contention can cause threads to yield CPU, affecting performance
             */

            // Demonstrate race condition WITHOUT synchronization
            Console.WriteLine("Attempting to demonstrate race condition (may take a few tries):");
            
            for (int attempt = 1; attempt <= 3; attempt++)
            {
                Console.WriteLine($"Attempt {attempt}:");
                _sharedCounter = 0;
                var unsafeThreads = new Thread[numThreads];

                for (int i = 0; i < numThreads; i++)
                {
                    unsafeThreads[i] = new Thread(() =>
                    {
                        for (int j = 0; j < incrementsPerThread; j++)
                        {
                            // RACE CONDITION: Multiple threads modifying shared variable
                            // Make the race condition more visible by reading, modifying, and writing separately
                            int temp = _sharedCounter;
                            
                            // Force a context switch to make race condition more likely
                            if (j % 100 == 0) 
                            {
                                Thread.Sleep(0); // Yield to other threads
                                // return control to task scheduler
                            }

                            temp = temp + 1;
                            _sharedCounter = temp; // This creates a larger window for race conditions
                        }
                    });
                }

                var sw = Stopwatch.StartNew();
                foreach (var thread in unsafeThreads)
                    thread.Start();
                foreach (var thread in unsafeThreads)
                    thread.Join(); // Wait for all threads to finish
                sw.Stop();

                Console.WriteLine($"  Expected: {numThreads * incrementsPerThread}, Actual: {_sharedCounter}");
                Console.WriteLine($"  Execution time: {sw.ElapsedMilliseconds}ms");
                
                if (_sharedCounter != numThreads * incrementsPerThread)
                {
                    Console.WriteLine($"  🔴 RACE CONDITION DETECTED! Lost {numThreads * incrementsPerThread - _sharedCounter} increments!");
                    Console.WriteLine("     This happens because multiple threads read-modify-write the same memory location simultaneously.");
                    break; // Found the race condition, no need to continue
                }
                else
                {
                    Console.WriteLine("  ⚠️  Race condition not visible this attempt (but code is still unsafe!)");
                }
            }

            Console.WriteLine("\nNow demonstrating SAFE version with proper synchronization:");

            // Demonstrate proper synchronization WITH lock
            _sharedCounter = 0;
            var stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < numThreads; i++)
            {
                threads[i] = new Thread(() =>
                {
                    for (int j = 0; j < incrementsPerThread; j++)
                    {
                        lock (_lockObject)
                        {
                            // Same operation as unsafe version, but protected by lock
                            int temp = _sharedCounter;

                            // Force a context switch to make race condition more likely
                            if (j % 100 == 0)
                            {
                                Thread.Sleep(0); // Yield to other threads
                                // return control to task scheduler
                            }

                            temp = temp + 1;
                            _sharedCounter = temp; // Now this is thread-safe
                        }
                    }
                });
            }

            foreach (var thread in threads)
                thread.Start();
            foreach (var thread in threads)
                thread.Join();
            stopwatch.Stop();

            Console.WriteLine($"WITH synchronization - Expected: {numThreads * incrementsPerThread}, Actual: {_sharedCounter}");
            Console.WriteLine($"Safe execution time: {stopwatch.ElapsedMilliseconds}ms");
            
            if (_sharedCounter == numThreads * incrementsPerThread)
            {
                Console.WriteLine("✅ SUCCESS! Synchronization prevented data loss - all increments counted correctly.");
                Console.WriteLine("   The lock ensures only one thread can modify the shared variable at a time.");
            }
            
            Console.WriteLine("Thread synchronization example completed.\n");
        }

        #endregion

        #region Mutex Example

        /*
         * MUTEX (MUTUAL EXCLUSION)
         * 
         * SCENARIOS TO USE:
         * - Cross-process synchronization (named mutexes)
         * - Ensuring only one instance of application (a process) runs
         * - Synchronizing access to system-wide resources
         *   system-wide resources like files, devices, etc.
         * 
         * SCENARIOS NOT TO USE:
         * - Within same process (threads) (use lock instead - it's faster)
         * - High-frequency synchronization (significant overhead)
         * - When Monitor/lock is sufficient
         */
        static void MutexExample()
        {
            Console.WriteLine("3. Mutex Example:");

            /*
             * MEMORY ALLOCATION NOTES:
             * - Mutex is a Windows kernel object (heavier than lock)
             * - Named mutexes persist beyond application lifetime
             * - Each mutex uses system resources (handles)
             * - Abandoned mutexes can cause AbandonedMutexException
             */

            const int numThreads = 3;
            Thread[] threads = new Thread[numThreads];

            for (int i = 0; i < numThreads; i++)
            {
                int threadId = i;
                threads[i] = new Thread(() =>
                {
                    Console.WriteLine($"Thread {threadId} waiting for mutex...");
                    
                    try
                    {
                        _mutex.WaitOne(); // Acquire mutex
                        Console.WriteLine($"Thread {threadId} acquired mutex");
                        
                        // Simulate exclusive work
                        Thread.Sleep(1000);
                        
                        Console.WriteLine($"Thread {threadId} releasing mutex");
                    }
                    catch (AbandonedMutexException)
                    {
                        Console.WriteLine($"Thread {threadId} acquired an abandoned mutex");
                    }
                    finally
                    {
                        _mutex.ReleaseMutex(); // Always release in finally
                    }
                });
            }

            foreach (var thread in threads)
                thread.Start();
            foreach (var thread in threads)
                thread.Join();

            Console.WriteLine("Mutex example completed.\n");
        }

        #endregion

        #region Semaphore Example

        /*
         * SEMAPHORE (COUNTING SEMAPHORE)
         * 
         * SCENARIOS TO USE:
         * - Limiting concurrent access to resources (connection pools, etc.)
         *   another example is a database connection pool, where you want to limit the number of concurrent connections.
         *   another example is a thread pool, where you want to limit the number of concurrent threads.
         * - Throttling resource usage (max N operations at once)
         * - Producer-consumer scenarios with bounded queues
         * 
         * SCENARIOS NOT TO USE:
         * - Binary synchronization (use AutoResetEvent instead)
         * - When unlimited concurrency is acceptable
         * - Simple mutual exclusion (use lock instead)
         */
        static void SemaphoreExample()
        {
            Console.WriteLine("4. Semaphore Example (Max 2 concurrent access):");

            /*
             * MEMORY ALLOCATION NOTES:
             * - Semaphore maintains internal counter (lightweight)
             * - Kernel synchronization object (more overhead than lock)
             * - Supports cross-process synchronization when named
             * - Blocked threads consume stack space while waiting
             */

            const int numThreads = 5;
            Thread[] threads = new Thread[numThreads];

            for (int i = 0; i < numThreads; i++)
            {
                int threadId = i;
                threads[i] = new Thread(() =>
                {
                    Console.WriteLine($"Thread {threadId} waiting for semaphore...");
                    
                    _semaphore.WaitOne(); // Acquire semaphore slot
                    try
                    {
                        Console.WriteLine($"Thread {threadId} entered critical section");
                        Thread.Sleep(2000); // Simulate work
                        Console.WriteLine($"Thread {threadId} leaving critical section");
                    }
                    finally
                    {
                        _semaphore.Release(); // Release semaphore slot
                    }
                });
            }

            foreach (var thread in threads)
                thread.Start();
            foreach (var thread in threads)
                thread.Join();

            Console.WriteLine("Semaphore example completed.\n");
        }

        #endregion

        #region AutoResetEvent Example

        /*
         * AUTORESETEVENT (BINARY SEMAPHORE)
         * 
         * SCENARIOS TO USE:
         * - Thread coordination and signaling
         * - Implementing simple producer-consumer patterns
         * - One-to-one thread communication
         * 
         * SCENARIOS NOT TO USE:
         * - One-to-many signaling (use ManualResetEvent)
         * - High-frequency signaling (consider other primitives)
         * - When simple lock is sufficient
         */
        static void AutoResetEventExample()
        {
            Console.WriteLine("5. AutoResetEvent Example:");

            /*
             * MEMORY ALLOCATION NOTES:
             * - Lightweight Windows kernel object
             * - Automatically resets after each signal
             * - Only one waiting thread is released per signal
             * - Minimal memory overhead but involves kernel transitions
             */

            // Consumer thread waiting for signal
            Thread consumerThread = new Thread(() =>
            {
                for (int i = 0; i < 3; i++)
                {
                    Console.WriteLine("Consumer: Waiting for signal...");
                    _autoEvent.WaitOne(); // Wait for signal
                    Console.WriteLine($"Consumer: Received signal {i + 1}");
                }
            });

            consumerThread.Start();

            // Producer thread sending signals
            for (int i = 0; i < 3; i++)
            {
                Thread.Sleep(1500); // this cause the thread to sleep for 1.5 seconds
                Console.WriteLine($"Producer: Sending signal {i + 1}");
                _autoEvent.Set(); // Send signal (automatically resets)
            }

            consumerThread.Join(); // Wait for consumer to finish
            Console.WriteLine("AutoResetEvent example completed.\n");
        }

        #endregion

        #region ManualResetEvent Example

        /*
         * MANUALRESETEVENT
         * 
         * SCENARIOS TO USE:
         * - Broadcasting signals to multiple threads
         * - Implementing barriers and checkpoints
         * - One-to-many thread coordination
         * 
         * SCENARIOS NOT TO USE:
         * - Simple one-to-one signaling (use AutoResetEvent)
         * - When automatic reset behavior is desired
         * - Frequent start/stop patterns (consider other approaches)
         */
        static void ManualResetEventExample()
        {
            Console.WriteLine("6. ManualResetEvent Example:");

            /*
             * MEMORY ALLOCATION NOTES:
             * - Remains signaled until manually reset
             * - All waiting threads are released simultaneously
             * - Useful for broadcasting state changes
             * - Kernel object with associated system overhead
             */

            const int numWaiters = 3;
            Thread[] waiters = new Thread[numWaiters];

            // Create multiple threads waiting for the same signal
            for (int i = 0; i < numWaiters; i++)
            {
                int threadId = i;
                waiters[i] = new Thread(() =>
                {
                    Console.WriteLine($"Waiter {threadId}: Waiting for manual reset event...");
                    _manualEvent.WaitOne();
                    Console.WriteLine($"Waiter {threadId}: Event signaled! Proceeding...");
                    Thread.Sleep(1000); // Simulate work after signal
                    Console.WriteLine($"Waiter {threadId}: Work completed");
                });
            }

            // Start all waiting threads
            foreach (var waiter in waiters)
                waiter.Start();

            Thread.Sleep(2000); // Let threads start waiting

            Console.WriteLine("Signaling all waiting threads...");
            _manualEvent.Set(); // Signal all waiting threads

            foreach (var waiter in waiters)
                waiter.Join();

            _manualEvent.Reset(); // Manually reset for future use
            Console.WriteLine("ManualResetEvent example completed.\n");
        }

        #endregion

        #region ThreadLocal Example

        /*
         * THREADLOCAL<T> (THREAD-LOCAL STORAGE)
         * 
         * SCENARIOS TO USE:
         * - Storing per-thread state without synchronization
         * - Thread-specific caching or context
         * - Avoiding shared state and race conditions
         * 
         * SCENARIOS NOT TO USE:
         * - When threads need to share data
         * - Short-lived threads (initialization overhead)
         * - When global state is required
         */
        static void ThreadLocalExample()
        {
            Console.WriteLine("7. ThreadLocal Example:");

            /*
             * MEMORY ALLOCATION NOTES:
             * - Each thread gets its own copy of the value
             * - No synchronization overhead (thread-safe by design)
             * - Memory allocated per thread using the ThreadLocal
             * - Values are garbage collected when threads exit
             */

            ThreadLocal<int> threadLocalValue = new ThreadLocal<int>(() =>
            {
                // Initialize with thread ID
                int initialValue = Thread.CurrentThread.ManagedThreadId;
                Console.WriteLine($"Initializing ThreadLocal for thread {initialValue}");
                return initialValue;
            });

            const int numThreads = 4;
            Thread[] threads = new Thread[numThreads];

            for (int i = 0; i < numThreads; i++)
            {
                threads[i] = new Thread(() =>
                {
                    // Each thread has its own value
                    Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId}: Initial value = {threadLocalValue.Value}");
                    
                    // Modify the thread-local value
                    threadLocalValue.Value += 100;
                    Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId}: Modified value = {threadLocalValue.Value}");
                    
                    Thread.Sleep(1000);
                    Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId}: Final value = {threadLocalValue.Value}");
                });
            }

            foreach (var thread in threads)
                thread.Start();
            foreach (var thread in threads)
                thread.Join();

            threadLocalValue.Dispose(); // Clean up resources
            Console.WriteLine("ThreadLocal example completed.\n");
        }

        #endregion

        #region ThreadPool Example

        /*
         * THREADPOOL
         * 
         * SCENARIOS TO USE:
         * - Short-lived operations that benefit from thread reuse
         * - Background processing without thread management overhead
         * - Parallel processing of multiple small tasks
         * 
         * SCENARIOS NOT TO USE:
         * - Long-running operations (blocks pool threads)
         * - Operations that require specific thread configuration
         * - CPU-intensive work that should use all cores (use Task.Run instead)
         */
        static void ThreadPoolExample()
        {
            Console.WriteLine("8. ThreadPool Example:");

            /*
             * MEMORY ALLOCATION NOTES:
             * - Reuses threads to avoid creation/destruction overhead
             * - Default minimum threads = CPU cores, maximum much higher
             * - Work items are queued and executed when threads available
             * - Much more memory efficient than creating dedicated threads
             */

            Console.WriteLine($"ThreadPool - Min Workers: {ThreadPool.ThreadCount}");
            Console.WriteLine($"Available Workers: {ThreadPool.PendingWorkItemCount}");

            const int numWorkItems = 10;
            var countdown = new CountdownEvent(numWorkItems);

            var stopwatch = Stopwatch.StartNew();

            // Queue work items to thread pool
            for (int i = 0; i < numWorkItems; i++)
            {
                int workId = i;
                ThreadPool.QueueUserWorkItem(state =>
                {
                    Console.WriteLine($"Work item {workId} executing on thread {Thread.CurrentThread.ManagedThreadId}");
                    Thread.Sleep(500); // Simulate work
                    Console.WriteLine($"Work item {workId} completed");
                    countdown.Signal(); // Decrement counter
                });
            }

            countdown.Wait(); // Wait for all work items to complete
            stopwatch.Stop();

            Console.WriteLine($"All thread pool work completed in {stopwatch.ElapsedMilliseconds}ms");
            Console.WriteLine("ThreadPool example completed.\n");
        }

        #endregion

        #region CancellationToken Example

        /*
         * CANCELLATIONTOKEN AND CANCELLATIONTOKENSOURCE
         * 
         * SCENARIOS TO USE:
         * - Cooperative cancellation of long-running operations
         * - Implementing timeouts for operations
         * - Graceful shutdown of background services
         * 
         * SCENARIOS NOT TO USE:
         * - Forceful thread termination (use Thread.Abort - deprecated)
         * - Operations that cannot be cancelled gracefully
         * - Simple boolean flags are sufficient
         */
        static void CancellationTokenExample()
        {
            Console.WriteLine("9. CancellationToken Example:");

            /*
             * MEMORY ALLOCATION NOTES:
             * - CancellationToken is a lightweight struct (passed by value)
             * - CancellationTokenSource manages cancellation state
             * - Registered callbacks are stored and invoked on cancellation
             * - Cancellation tokens can be chained and combined
             */

            using var cts = new CancellationTokenSource();
            var token = cts.Token;

            // Start a cancellable operation
            var workerThread = new Thread(() =>
            {
                try
                {
                    for (int i = 0; i < 10; i++)
                    {
                        // Check for cancellation before doing work
                        token.ThrowIfCancellationRequested();
                        
                        Console.WriteLine($"Worker: Processing item {i + 1}");
                        Thread.Sleep(1000);
                    }
                    Console.WriteLine("Worker: All work completed successfully");
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Worker: Operation was cancelled");
                }
            });

            workerThread.Start();

            // Cancel after 3 seconds
            Thread.Sleep(3000);
            Console.WriteLine("Main: Requesting cancellation...");
            cts.Cancel();

            workerThread.Join();
            Console.WriteLine("CancellationToken example completed.\n");
        }

        #endregion

        #region Producer-Consumer Example

        /*
         * PRODUCER-CONSUMER PATTERN
         * 
         * SCENARIOS TO USE:
         * - Decoupling data production from consumption
         * - Buffering between fast producers and slow consumers
         * - Processing pipelines with multiple stages
         * 
         * SCENARIOS NOT TO USE:
         * - Simple one-to-one data transfer
         * - When synchronous processing is acceptable
         * - Memory usage is a critical constraint (queues use memory)
         */
        static void ProducerConsumerExample()
        {
            Console.WriteLine("10. Producer-Consumer Example:");

            /*
             * MEMORY ALLOCATION NOTES:
             * - ConcurrentQueue is thread-safe and lock-free
             * - Items are stored in queue segments (efficient memory usage)
             * - No blocking - producers/consumers use polling or signaling
             * - Memory grows with queue size - consider bounded queues for large datasets
             */

            var queue = new ConcurrentQueue<string>();
            var cts = new CancellationTokenSource();
            var token = cts.Token;

            // Producer thread
            var producer = new Thread(() =>
            {
                int item = 0;
                while (!token.IsCancellationRequested && item < 10)
                {
                    var data = $"Item-{++item}";
                    queue.Enqueue(data);
                    Console.WriteLine($"Producer: Added {data} (Queue size: ~{queue.Count})");
                    Thread.Sleep(500);
                }
                Console.WriteLine("Producer: Finished producing");
            });

            // Consumer thread
            var consumer = new Thread(() =>
            {
                while (!token.IsCancellationRequested)
                {
                    if (queue.TryDequeue(out string item))
                    {
                        Console.WriteLine($"Consumer: Processing {item}");
                        Thread.Sleep(800); // Consumer is slower than producer
                        Console.WriteLine($"Consumer: Completed {item}");
                    }
                    else
                    {
                        Thread.Sleep(100); // Wait a bit before checking again
                    }
                }
                
                // Process remaining items
                while (queue.TryDequeue(out string item))
                {
                    Console.WriteLine($"Consumer: Final processing {item}");
                }
                Console.WriteLine("Consumer: Finished consuming");
            });

            producer.Start();
            consumer.Start();

            // Let them run for a while
            Thread.Sleep(8000);
            cts.Cancel();

            producer.Join();
            consumer.Join();

            Console.WriteLine($"Remaining items in queue: {queue.Count}");
            Console.WriteLine("Producer-Consumer example completed.\n");
        }

        #endregion

        #region Memory Allocation Analysis

        /*
         * MEMORY ALLOCATION DEEP DIVE
         * 
         * PURPOSE: Detailed analysis of threading memory costs and considerations
         */
        static void MemoryAllocationExample()
        {
            Console.WriteLine("11. Memory Allocation Analysis:");

            /*
             * DETAILED MEMORY BREAKDOWN:
             * 
             * 1. THREAD STACK:
             *    - Default: 1MB per thread on 32-bit, 4MB on 64-bit
             *    - Committed memory grows as needed, but virtual address space is reserved
             *    - Can be configured via Thread constructor or ProcessThread
             * 
             * 2. SYNCHRONIZATION PRIMITIVES:
             *    - lock (Monitor): ~40 bytes overhead per object
             *    - Mutex: Windows kernel object (~1KB + system resources)
             *    - Semaphore: Kernel object with counter (~few KB)
             *    - Events: Kernel objects (~1-2KB each)
             * 
             * 3. THREAD OBJECTS:
             *    - Thread instance: ~few KB for .NET object + native thread
             *    - Context switching: CPU cache misses, TLB flushes. TLB means Translation Lookaside Buffer
             * 
             * 4. CONCURRENT COLLECTIONS:
             *    - ConcurrentQueue: Segments of 32 items each
             *    - ConcurrentDictionary: More memory than regular Dictionary due to locking
             * 
             * PERFORMANCE IMPLICATIONS:
             * - More threads ≠ better performance (context switching overhead)
             * - Optimal thread count often equals CPU core count for CPU-bound work
             * - I/O-bound work can benefit from more threads (up to hundreds)
             */

            // Measure memory before creating threads
            long memoryBefore = GC.GetTotalMemory(true);
            Console.WriteLine($"Memory before threading operations: {memoryBefore:N0} bytes");

            // Create multiple threads to show memory impact
            const int numThreads = 10;
            Thread[] threads = new Thread[numThreads];
            var barriers = new Barrier[numThreads];
            // Barrier is used to synchronize main thread and worker threads
            // Each barrier will wait for 2 participants: main thread and worker thread

            for (int i = 0; i < numThreads; i++)
            {
                barriers[i] = new Barrier(2); // Each barrier for main thread + worker thread
                int threadId = i;
                threads[i] = new Thread(() =>
                {
                    // Allocate some local data to show stack usage
                    byte[] localData = new byte[1024]; // 1KB local allocation
                    localData[0] = (byte)threadId; // Use the data
                    
                    Console.WriteLine($"Thread {threadId}: Allocated {localData.Length} bytes locally");
                    
                    barriers[threadId].SignalAndWait(); // Synchronize with main thread
                    // explain this line above better
                    // This line signals that this thread has reached the barrier and then waits for the other participant (main thread) to reach the barrier as well.
                    // This ensures that all threads have allocated their local data before proceeding.
                    // This helps in measuring memory usage after all threads have started and allocated their data.
                });
            }

            // Start all threads
            foreach (var thread in threads)
                thread.Start();

            // Wait for all threads to allocate their data
            foreach (var barrier in barriers)
                barrier.SignalAndWait();

            long memoryAfter = GC.GetTotalMemory(false);
            Console.WriteLine($"Memory after creating {numThreads} threads: {memoryAfter:N0} bytes");
            Console.WriteLine($"Memory increase: {memoryAfter - memoryBefore:N0} bytes");
            Console.WriteLine($"Average per thread: {(memoryAfter - memoryBefore) / numThreads:N0} bytes");

            // Clean up
            foreach (var thread in threads)
                thread.Join();
            foreach (var barrier in barriers)
                barrier.Dispose();

            // Force garbage collection to clean up
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            long memoryAfterCleanup = GC.GetTotalMemory(true);
            Console.WriteLine($"Memory after cleanup: {memoryAfterCleanup:N0} bytes");
            Console.WriteLine("Memory allocation analysis completed.\n");
        }

        #endregion

        #region When to Use and Not Use Threading

        /*
         * COMPREHENSIVE GUIDE: WHEN TO USE vs WHEN NOT TO USE THREADING
         */
        static void WhenToUseAndNotUseThreading()
        {
            Console.WriteLine("12. When to Use vs Not Use Threading:");

            Console.WriteLine("✅ GOOD scenarios for System.Threading:");
            Console.WriteLine("   • CPU-intensive work that can be parallelized");
            Console.WriteLine("   • Producer-consumer patterns with different processing speeds");
            Console.WriteLine("   • Background services and long-running operations");
            Console.WriteLine("   • Resource pools with limited concurrent access");
            Console.WriteLine("   • Event-driven architectures with multiple handlers");
            Console.WriteLine("   • When you need fine-grained control over thread behavior");
            Console.WriteLine("   • Cross-process synchronization scenarios");

            Console.WriteLine("\n❌ AVOID System.Threading when:");
            Console.WriteLine("   • I/O-bound operations (use async/await instead)");
            Console.WriteLine("   • Simple sequential operations");
            Console.WriteLine("   • Operations where Task Parallel Library (TPL) is sufficient");
            Console.WriteLine("   • Web applications (use async controllers instead)");
            Console.WriteLine("   • When thread safety isn't properly understood");
            Console.WriteLine("   • Short-lived operations (overhead > benefit)");
            Console.WriteLine("   • Mobile applications with limited resources");

            Console.WriteLine("\n🎯 ALTERNATIVES to consider:");
            Console.WriteLine("   • Task Parallel Library (TPL) for structured parallelism");
            Console.WriteLine("   • async/await for asynchronous programming");
            Console.WriteLine("   • Parallel LINQ (PLINQ) for data parallelism");
            Console.WriteLine("   • System.Threading.Channels for producer-consumer");
            Console.WriteLine("   • Concurrent collections instead of locks");
            Console.WriteLine("   • ThreadPool instead of manual thread creation");

            Console.WriteLine("\n⚠️  COMMON THREADING MISTAKES:");
            Console.WriteLine("   • Creating too many threads (context switching overhead)");
            Console.WriteLine("   • Not handling thread exceptions properly");
            Console.WriteLine("   • Sharing mutable state without synchronization");
            Console.WriteLine("   • Using Thread.Abort (deprecated and dangerous)");
            Console.WriteLine("   • Blocking UI threads with Thread.Sleep");
            Console.WriteLine("   • Not disposing synchronization primitives");
            Console.WriteLine("   • Accessing UI controls from background threads");

            Console.WriteLine("\n💡 BEST PRACTICES:");
            Console.WriteLine("   • Use thread-safe collections when possible");
            Console.WriteLine("   • Always dispose IDisposable synchronization objects");
            Console.WriteLine("   • Handle OperationCanceledException in cancellable operations");
            Console.WriteLine("   • Use ThreadLocal<T> to avoid shared mutable state");
            Console.WriteLine("   • Consider using higher-level abstractions (TPL, async/await)");
            Console.WriteLine("   • Profile and measure performance - don't assume threading helps");
            Console.WriteLine("   • Design for graceful shutdown and cancellation");

            Console.WriteLine("\nThreading guidance completed.");
        }

        #endregion
    }
}
