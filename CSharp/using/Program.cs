using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

// Using directive for namespace imports
// PURPOSE: Simplifies code by avoiding fully qualified type names
using FileMode = System.IO.FileMode; // Using alias

namespace UsingExamples
{
    /// <summary>
    /// Comprehensive examples of the 'using' keyword in C#
    /// 
    /// PURPOSE:
    /// The 'using' keyword has three distinct purposes:
    /// 1. DIRECTIVE: Import namespaces to avoid fully qualified names
    /// 2. STATEMENT: Automatic disposal of IDisposable resources (deterministic cleanup)
    /// 3. DECLARATION: C# 8.0+ simplified syntax for disposal at scope end
    /// 
    /// SCENARIOS TO USE:
    /// - Managing unmanaged resources (file handles, database connections, network streams)
    /// - Ensuring deterministic cleanup of IDisposable objects
    /// - Releasing locks and synchronization primitives
    /// - Cleaning up expensive resources promptly
    /// - Database connections and transactions
    /// - File I/O operations
    /// - Network connections and HTTP clients
    /// - Graphics and UI resources
    /// 
    /// SCENARIOS NOT TO USE:
    /// - Objects that don't implement IDisposable
    /// - When you need to access the resource after the using scope
    /// - Pooled resources that should be returned, not disposed
    /// - When disposal order needs to be controlled explicitly
    /// - Long-lived resources that span multiple methods
    /// 
    /// MEMORY ALLOCATION:
    /// - using statement itself has zero overhead
    /// - Guarantees Dispose() is called, which may free unmanaged resources
    /// - Does NOT cause garbage collection - only releases unmanaged resources
    /// - Managed memory still collected by GC later
    /// - Reduces memory pressure by freeing resources promptly
    /// - Prevents resource leaks from exceptions
    /// 
    /// MULTITHREAD ASPECTS:
    /// - using ensures disposal even in multithreaded scenarios
    /// - Disposal happens on the thread executing the using block
    /// - Thread synchronization primitives (locks, semaphores) should use using
    /// - Be cautious with shared resources across threads
    /// - Dispose() should be thread-safe in IDisposable implementations
    /// - Multiple threads disposing same object = undefined behavior
    /// </summary>
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== Using Keyword Examples ===\n");

            // Example 1: Basic Using Statement
            BasicUsingStatementExamples();

            // Example 2: Using Declarations (C# 8.0+)
            UsingDeclarationExamples();

            // Example 3: Multiple Resources
            MultipleResourcesExamples();

            // Example 4: File I/O with Using
            FileIOExamples();

            // Example 5: Database Connections
            // DatabaseConnectionExamples(); // Commented out - requires SQL Server package

            // Example 6: Network Resources
            await NetworkResourceExamples();

            // Example 7: Custom IDisposable
            CustomDisposableExamples();

            // Example 8: Exception Handling with Using
            ExceptionHandlingExamples();

            // Example 9: Thread Synchronization
            ThreadSynchronizationExamples();

            // Example 10: Using vs Try-Finally
            UsingVsTryFinallyExamples();

            // Example 11: Nested Using Statements
            NestedUsingExamples();

            // Example 12: Common Pitfalls
            CommonPitfallsExamples();

            Console.WriteLine("\n=== All Examples Completed ===");
        }

        #region Basic Using Statement Examples

        /// <summary>
        /// Demonstrates basic using statement for automatic resource disposal
        /// 
        /// PURPOSE:
        /// - Ensures Dispose() is called automatically when scope exits
        /// - Works even if exceptions occur
        /// - Equivalent to try-finally with Dispose() in finally block
        /// 
        /// MEMORY ALLOCATION:
        /// - StreamWriter allocates buffer for file writing
        /// - using ensures buffer is flushed and file handle released
        /// - Without using, file handle may remain open until GC finalizer runs
        /// </summary>
        static void BasicUsingStatementExamples()
        {
            Console.WriteLine("--- Basic Using Statement Examples ---");

            string filePath = "basic_example.txt";

            // Using statement with StreamWriter
            // GOOD FOR: Automatic disposal of file handles
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("First line");
                writer.WriteLine("Second line");
                writer.WriteLine("Third line");
                Console.WriteLine("  Writing to file...");
            } // writer.Dispose() automatically called here

            Console.WriteLine("  File handle closed automatically");

            // Reading the file back
            using (StreamReader reader = new StreamReader(filePath))
            {
                string content = reader.ReadToEnd();
                Console.WriteLine($"  File content:\n{content}");
            } // reader.Dispose() automatically called here

            // Cleanup
            File.Delete(filePath);

            Console.WriteLine();
        }

        #endregion

        #region Using Declarations (C# 8.0+)

        /// <summary>
        /// Demonstrates using declarations - simplified syntax introduced in C# 8.0
        /// 
        /// PURPOSE:
        /// - Cleaner syntax without braces
        /// - Disposal happens at end of enclosing scope
        /// - Reduces nesting and improves readability
        /// 
        /// SCENARIOS TO USE:
        /// - When resource is needed throughout entire method
        /// - When multiple resources have same lifetime
        /// - To reduce code indentation
        /// </summary>
        static void UsingDeclarationExamples()
        {
            Console.WriteLine("--- Using Declarations (C# 8.0+) ---");

            string filePath = "declaration_example.txt";

            // Using declaration - no braces needed
            // Disposal happens at end of scope (using statement block or method)
            {
                using StreamWriter writer = new StreamWriter(filePath);
                
                writer.WriteLine("Using declaration example");
                writer.WriteLine("Disposal happens at end of scope");
                
                Console.WriteLine("  Writer is still available here");
                Console.WriteLine("  It will be disposed when scope ends");
            } // writer disposed here (end of scope)

            // Now we can safely open the file for reading
            // Multiple using declarations
            {
                using StreamReader reader = new StreamReader(filePath);
                using var memoryStream = new MemoryStream();
                
                // All resources available here
                string content = reader.ReadToEnd();
                Console.WriteLine($"  Content: {content}");

                // All resources disposed here in reverse order
                // Order: memoryStream, reader
            } // Both disposed here

            // Now the file is closed and can be deleted
            File.Delete(filePath);

            Console.WriteLine();
        }

        #endregion

        #region Multiple Resources Examples

        /// <summary>
        /// Demonstrates managing multiple IDisposable resources
        /// 
        /// SCENARIOS TO USE:
        /// - Reading from one file while writing to another
        /// - Database operations with multiple connections
        /// - Network operations with multiple streams
        /// 
        /// MEMORY ALLOCATION:
        /// - Each resource allocates independently
        /// - All disposed in reverse order (LIFO - Last In, First Out)
        /// - Ensures proper cleanup even if one disposal throws exception
        /// </summary>
        static void MultipleResourcesExamples()
        {
            Console.WriteLine("--- Multiple Resources Examples ---");

            string sourceFile = "source.txt";
            string destFile = "destination.txt";

            // Create source file
            File.WriteAllText(sourceFile, "Content to copy");

            // Pattern 1: Nested using statements (traditional)
            Console.WriteLine("Pattern 1: Nested using statements");
            using (StreamReader reader = new StreamReader(sourceFile))
            {
                using (StreamWriter writer = new StreamWriter(destFile))
                {
                    string line;
                    while ((line = reader.ReadLine()!) != null)
                    {
                        writer.WriteLine(line.ToUpper());
                    }
                    Console.WriteLine("  File copied and transformed");
                }
            }

            // Pattern 2: Multiple using statements (C# style)
            Console.WriteLine("\nPattern 2: Multiple using in single statement");
            string destFile2 = "destination2.txt";
            using (StreamReader reader = new StreamReader(sourceFile))
            using (StreamWriter writer = new StreamWriter(destFile2))
            {
                // Both resources available here
                string content = reader.ReadToEnd();
                writer.Write(content.ToLower());
                Console.WriteLine("  File copied with second pattern");
            } // Disposed in reverse order: writer, then reader

            // Pattern 3: Using declarations (C# 8.0+)
            Console.WriteLine("\nPattern 3: Using declarations");
            string destFile3 = "destination3.txt";
            {
                using StreamReader reader3 = new StreamReader(sourceFile);
                using StreamWriter writer3 = new StreamWriter(destFile3);
                
                writer3.Write(reader3.ReadToEnd().Replace("Content", "Modified"));
                Console.WriteLine("  File copied with using declarations");
            } // Both disposed here

            // Cleanup - now all files are closed
            File.Delete(sourceFile);
            File.Delete(destFile);
            File.Delete(destFile2);
            File.Delete(destFile3);

            Console.WriteLine();
        }

        #endregion

        #region File I/O Examples

        /// <summary>
        /// Demonstrates using statement with various file I/O operations
        /// 
        /// PURPOSE:
        /// - File handles are unmanaged resources
        /// - Must be closed to allow other processes to access files
        /// - using ensures files are closed even if exceptions occur
        /// 
        /// MEMORY ALLOCATION:
        /// - FileStream allocates native buffer (typically 4KB)
        /// - Buffer flushed and released on disposal
        /// - File handle returned to OS
        /// 
        /// SCENARIOS NOT TO USE:
        /// - When you need the file to remain open across method boundaries
        /// - When using memory-mapped files that need extended lifetime
        /// </summary>
        static void FileIOExamples()
        {
            Console.WriteLine("--- File I/O Examples ---");

            string filePath = "fileio_example.bin";

            // Writing binary data
            Console.WriteLine("Writing binary data:");
            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            using (BinaryWriter writer = new BinaryWriter(fs))
            {
                writer.Write(42);
                writer.Write(3.14159);
                writer.Write("Hello, Using!");
                Console.WriteLine("  Binary data written");
            }

            // Reading binary data
            Console.WriteLine("\nReading binary data:");
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (BinaryReader reader = new BinaryReader(fs))
            {
                int intValue = reader.ReadInt32();
                double doubleValue = reader.ReadDouble();
                string stringValue = reader.ReadString();

                Console.WriteLine($"  Read: {intValue}, {doubleValue}, {stringValue}");
            }

            // Using FileStream directly
            Console.WriteLine("\nDirect FileStream usage:");
            using (FileStream fs = File.OpenWrite("direct_example.txt"))
            {
                byte[] data = System.Text.Encoding.UTF8.GetBytes("Direct write");
                fs.Write(data, 0, data.Length);
                Console.WriteLine("  Data written directly to FileStream");
            }

            // Cleanup
            File.Delete(filePath);
            File.Delete("direct_example.txt");

            Console.WriteLine();
        }

        #endregion

        #region Database Connection Examples

        /// <summary>
        /// Demonstrates using statement with database connections
        /// 
        /// NOTE: This example is commented out because it requires System.Data.SqlClient package
        /// To use: Install-Package System.Data.SqlClient
        /// 
        /// PURPOSE:
        /// - Database connections are expensive resources
        /// - Connection pooling requires proper disposal
        /// - Ensures connections are returned to pool
        /// 
        /// MEMORY ALLOCATION:
        /// - SqlConnection allocates native connection
        /// - Disposal returns connection to pool (doesn't necessarily close it)
        /// - Improves connection reuse and application scalability
        /// 
        /// SCENARIOS TO USE:
        /// - Every database operation
        /// - Critical for connection pooling
        /// - Prevents connection pool exhaustion
        /// 
        /// MULTITHREAD ASPECTS:
        /// - Each thread should have its own connection
        /// - using ensures connection is properly returned to pool
        /// - Pool handles thread-safe connection distribution
        /// </summary>
        static void DatabaseConnectionExamples()
        {
            Console.WriteLine("--- Database Connection Examples ---");
            Console.WriteLine("This example requires System.Data.SqlClient package");
            Console.WriteLine("Conceptual patterns shown:\n");

            // Conceptual examples (without actual SQL types)
            Console.WriteLine("Pattern 1: Connection with Command");
            Console.WriteLine("  using (SqlConnection connection = new SqlConnection(connectionString))");
            Console.WriteLine("  {");
            Console.WriteLine("      connection.Open();");
            Console.WriteLine("      using (SqlCommand command = connection.CreateCommand())");
            Console.WriteLine("      {");
            Console.WriteLine("          // Execute command");
            Console.WriteLine("      } // Command disposed");
            Console.WriteLine("  } // Connection returned to pool");

            Console.WriteLine("\nPattern 2: Transaction with using");
            Console.WriteLine("  using (SqlConnection connection = new SqlConnection(connectionString))");
            Console.WriteLine("  using (SqlTransaction transaction = connection.BeginTransaction())");
            Console.WriteLine("  {");
            Console.WriteLine("      try");
            Console.WriteLine("      {");
            Console.WriteLine("          // Execute commands");
            Console.WriteLine("          transaction.Commit();");
            Console.WriteLine("      }");
            Console.WriteLine("      catch");
            Console.WriteLine("      {");
            Console.WriteLine("          transaction.Rollback();");
            Console.WriteLine("      }");
            Console.WriteLine("  } // Transaction and connection disposed");

            Console.WriteLine();
        }

        #endregion

        #region Network Resources Examples

        /// <summary>
        /// Demonstrates using statement with network resources
        /// 
        /// PURPOSE:
        /// - Network resources are expensive and limited
        /// - Sockets and connections must be properly closed
        /// - Prevents socket exhaustion
        /// 
        /// MEMORY ALLOCATION:
        /// - HttpClient uses socket pool
        /// - Proper disposal prevents socket leaks
        /// - Note: HttpClient is designed to be reused (singleton pattern)
        /// 
        /// SCENARIOS NOT TO USE:
        /// - DON'T dispose HttpClient in tight loops (socket exhaustion)
        /// - Use single long-lived instance instead
        /// </summary>
        static async Task NetworkResourceExamples()
        {
            Console.WriteLine("--- Network Resources Examples ---");

            // Pattern 1: HttpClient (note: should be singleton in real apps)
            Console.WriteLine("Pattern 1: HttpClient");
            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(30);
                
                try
                {
                    HttpResponseMessage response = await client.GetAsync("https://httpbin.org/get");
                    string content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"  Response received: {content.Length} characters");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  Error: {ex.Message}");
                }
            }

            // Pattern 2: HttpResponseMessage
            Console.WriteLine("\nPattern 2: HttpResponseMessage with using");
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = await client.GetAsync("https://httpbin.org/headers"))
                {
                    Console.WriteLine($"  Status: {response.StatusCode}");
                    
                    using (Stream stream = await response.Content.ReadAsStreamAsync())
                    {
                        Console.WriteLine($"  Stream opened for reading");
                        // Process stream
                    }
                }
            }

            // Pattern 3: NetworkStream (conceptual - requires actual network connection)
            Console.WriteLine("\nPattern 3: NetworkStream (conceptual)");
            Console.WriteLine("  Would use: using (NetworkStream stream = client.GetStream())");

            Console.WriteLine();
        }

        #endregion

        #region Custom IDisposable Examples

        /// <summary>
        /// Demonstrates creating custom IDisposable classes
        /// 
        /// PURPOSE:
        /// - Custom classes managing unmanaged resources need IDisposable
        /// - Follow Dispose pattern for proper resource cleanup
        /// - Implement finalizer for unmanaged resources
        /// 
        /// MEMORY ALLOCATION:
        /// - Dispose() releases unmanaged resources immediately
        /// - Finalizer (~ClassName) is backup if Dispose() not called
        /// - Suppress finalizer in Dispose() to improve GC performance
        /// 
        /// MULTITHREAD ASPECTS:
        /// - Dispose() should be thread-safe
        /// - Use Interlocked or locks to prevent multiple disposal
        /// - Set disposed flag atomically
        /// </summary>
        static void CustomDisposableExamples()
        {
            Console.WriteLine("--- Custom IDisposable Examples ---");

            // Pattern 1: Basic IDisposable
            Console.WriteLine("Pattern 1: Basic disposal");
            using (var resource = new ManagedResource("Resource1"))
            {
                resource.DoWork();
            } // Dispose called here

            // Pattern 2: Dispose pattern with finalizer
            Console.WriteLine("\nPattern 2: Full dispose pattern");
            using (var resource = new UnmanagedResourceWrapper("Handle123"))
            {
                resource.UseResource();
            }

            // Pattern 3: Without using (manual disposal)
            Console.WriteLine("\nPattern 3: Manual disposal (not recommended)");
            var resource3 = new ManagedResource("Resource3");
            try
            {
                resource3.DoWork();
            }
            finally
            {
                resource3.Dispose(); // Must remember to call Dispose()
            }

            Console.WriteLine();
        }

        class ManagedResource : IDisposable
        {
            private string _name;
            private bool _disposed = false;

            public ManagedResource(string name)
            {
                _name = name;
                Console.WriteLine($"  {_name} created");
            }

            public void DoWork()
            {
                if (_disposed)
                    throw new ObjectDisposedException(_name);

                Console.WriteLine($"  {_name} doing work");
            }

            public void Dispose()
            {
                if (_disposed)
                    return;

                Console.WriteLine($"  {_name} disposed");
                _disposed = true;
            }
        }

        class UnmanagedResourceWrapper : IDisposable
        {
            private IntPtr _handle; // Simulates unmanaged resource
            private bool _disposed = false;

            public UnmanagedResourceWrapper(string handle)
            {
                _handle = new IntPtr(handle.GetHashCode()); // Simulated handle
                Console.WriteLine($"  Unmanaged resource allocated: {handle}");
            }

            public void UseResource()
            {
                if (_disposed)
                    throw new ObjectDisposedException("UnmanagedResourceWrapper");

                Console.WriteLine($"  Using unmanaged resource: {_handle}");
            }

            // Finalizer - called by GC if Dispose not called
            ~UnmanagedResourceWrapper()
            {
                Dispose(false);
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this); // Prevent finalizer from running
            }

            protected virtual void Dispose(bool disposing)
            {
                if (_disposed)
                    return;

                if (disposing)
                {
                    // Dispose managed resources
                    Console.WriteLine("  Disposing managed resources");
                }

                // Free unmanaged resources
                if (_handle != IntPtr.Zero)
                {
                    Console.WriteLine($"  Freeing unmanaged resource: {_handle}");
                    _handle = IntPtr.Zero;
                }

                _disposed = true;
            }
        }

        #endregion

        #region Exception Handling Examples

        /// <summary>
        /// Demonstrates how using statement handles exceptions
        /// 
        /// PURPOSE:
        /// - using ensures Dispose() is called even when exceptions occur
        /// - Equivalent to try-finally block
        /// - Guarantees resource cleanup in error scenarios
        /// 
        /// SCENARIOS TO USE:
        /// - Critical to prevent resource leaks during exceptions
        /// - Any operation that might throw exceptions
        /// - Ensures consistent cleanup
        /// </summary>
        static void ExceptionHandlingExamples()
        {
            Console.WriteLine("--- Exception Handling Examples ---");

            string filePath = "exception_example.txt";

            // Example 1: Exception during resource use
            Console.WriteLine("Example 1: Exception with automatic cleanup");
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.WriteLine("First line");
                    throw new InvalidOperationException("Simulated error!");
                    // writer.WriteLine("This won't execute");
                }
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"  Caught exception: {ex.Message}");
                Console.WriteLine("  File handle was still closed properly");
            }

            // Example 2: Exception in Dispose
            Console.WriteLine("\nExample 2: Exception during disposal");
            try
            {
                using (var resource = new ProblematicResource())
                {
                    resource.DoWork();
                } // Dispose throws exception
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"  Caught disposal exception: {ex.Message}");
            }

            // Example 3: Multiple exceptions
            Console.WriteLine("\nExample 3: Exception in both using and Dispose");
            try
            {
                using (var resource = new ProblematicResource())
                {
                    resource.DoWork();
                    throw new ArgumentException("Work exception");
                } // Dispose also throws, but work exception is propagated
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  Caught: {ex.GetType().Name} - {ex.Message}");
            }

            // Cleanup
            if (File.Exists(filePath))
                File.Delete(filePath);

            Console.WriteLine();
        }

        class ProblematicResource : IDisposable
        {
            public void DoWork()
            {
                Console.WriteLine("  Doing work...");
            }

            public void Dispose()
            {
                Console.WriteLine("  Dispose called (will throw exception)");
                throw new InvalidOperationException("Disposal failed!");
            }
        }

        #endregion

        #region Thread Synchronization Examples

        /// <summary>
        /// Demonstrates using statement with thread synchronization primitives
        /// 
        /// PURPOSE:
        /// - Locks and synchronization objects must be released
        /// - using ensures release even if exceptions occur
        /// - Critical for preventing deadlocks
        /// 
        /// SCENARIOS TO USE:
        /// - Always use with SemaphoreSlim, ReaderWriterLockSlim
        /// - Ensures locks are released
        /// - Prevents deadlocks from unhandled exceptions
        /// 
        /// MULTITHREAD ASPECTS:
        /// - Critical for thread safety
        /// - Locks must be released on same thread
        /// - using prevents common deadlock scenarios
        /// </summary>
        static void ThreadSynchronizationExamples()
        {
            Console.WriteLine("--- Thread Synchronization Examples ---");

            var semaphore = new SemaphoreSlim(2, 2);
            var lockObject = new ReaderWriterLockSlim();
            var sharedResource = 0;

            // Example 1: SemaphoreSlim
            Console.WriteLine("Example 1: SemaphoreSlim");
            var tasks = new List<Task>();

            // Note: SemaphoreSlim.WaitAsync() returns Task, not IDisposable
            // So we cannot use it directly with 'using' statement
            // Must use try-finally pattern instead

            // Correct SemaphoreSlim pattern
            Console.WriteLine("SemaphoreSlim pattern:");
            Task.Run(async () =>
            {
                await semaphore.WaitAsync();
                try
                {
                    Console.WriteLine("  Semaphore acquired");
                    await Task.Delay(100);
                }
                finally
                {
                    semaphore.Release();
                    Console.WriteLine("  Semaphore released");
                }
            }).Wait();

            for (int i = 0; i < 5; i++)
            {
                int taskId = i;
                tasks.Add(Task.Run(async () =>
                {
                    Console.WriteLine($"  Task {taskId} waiting for semaphore");
                    await semaphore.WaitAsync();
                    try
                    {
                        Console.WriteLine($"  Task {taskId} acquired semaphore");
                        await Task.Delay(100);
                    }
                    finally
                    {
                        semaphore.Release();
                        Console.WriteLine($"  Task {taskId} released semaphore");
                    }
                }));
            }

            Task.WaitAll(tasks.ToArray());

            // Example 2: ReaderWriterLockSlim
            Console.WriteLine("\nExample 2: ReaderWriterLockSlim");
            
            // Write lock
            using (var writeLock = new WriteLockScope(lockObject))
            {
                sharedResource++;
                Console.WriteLine($"  Write: {sharedResource}");
            }

            // Read lock
            using (var readLock = new ReadLockScope(lockObject))
            {
                Console.WriteLine($"  Read: {sharedResource}");
            }

            // Cleanup
            semaphore.Dispose();
            lockObject.Dispose();

            Console.WriteLine();
        }

        // Helper class to make locks work with using statement
        struct WriteLockScope : IDisposable
        {
            private readonly ReaderWriterLockSlim _lock;

            public WriteLockScope(ReaderWriterLockSlim lockObject)
            {
                _lock = lockObject;
                _lock.EnterWriteLock();
            }

            public void Dispose()
            {
                _lock.ExitWriteLock();
            }
        }

        struct ReadLockScope : IDisposable
        {
            private readonly ReaderWriterLockSlim _lock;

            public ReadLockScope(ReaderWriterLockSlim lockObject)
            {
                _lock = lockObject;
                _lock.EnterReadLock();
            }

            public void Dispose()
            {
                _lock.ExitReadLock();
            }
        }

        #endregion

        #region Using vs Try-Finally Examples

        /// <summary>
        /// Demonstrates equivalence between using and try-finally
        /// 
        /// PURPOSE:
        /// - using is syntactic sugar for try-finally with Dispose()
        /// - Compiler transforms using into try-finally
        /// - using is more concise and less error-prone
        /// 
        /// SCENARIOS TO USE:
        /// - Prefer using statement for IDisposable objects
        /// - Only use try-finally if you need custom cleanup logic
        /// </summary>
        static void UsingVsTryFinallyExamples()
        {
            Console.WriteLine("--- Using vs Try-Finally Examples ---");

            string file1 = "using_example.txt";
            string file2 = "finally_example.txt";

            // Pattern 1: Using statement (recommended)
            Console.WriteLine("Pattern 1: Using statement");
            using (StreamWriter writer = new StreamWriter(file1))
            {
                writer.WriteLine("Using statement");
            } // Dispose called automatically

            // Pattern 2: Equivalent try-finally
            Console.WriteLine("Pattern 2: Equivalent try-finally");
            StreamWriter? writer2 = null;
            try
            {
                writer2 = new StreamWriter(file2);
                writer2.WriteLine("Try-finally pattern");
            }
            finally
            {
                writer2?.Dispose(); // Must explicitly call Dispose()
            }

            // Demonstrate they're equivalent
            Console.WriteLine("  Both patterns achieve the same result");
            Console.WriteLine("  Using statement is cleaner and less error-prone");

            // What the compiler actually generates for using:
            Console.WriteLine("\nCompiler-generated code (conceptual):");
            Console.WriteLine("  StreamWriter writer = new StreamWriter(path);");
            Console.WriteLine("  try { /* using block */ }");
            Console.WriteLine("  finally { if (writer != null) writer.Dispose(); }");

            // Cleanup
            File.Delete(file1);
            File.Delete(file2);

            Console.WriteLine();
        }

        #endregion

        #region Nested Using Examples

        /// <summary>
        /// Demonstrates nested using statements and disposal order
        /// 
        /// PURPOSE:
        /// - Resources disposed in reverse order (LIFO)
        /// - Inner resources disposed before outer resources
        /// - Ensures correct cleanup order
        /// 
        /// MEMORY ALLOCATION:
        /// - Each level allocates its resources
        /// - Disposal frees resources from innermost to outermost
        /// </summary>
        static void NestedUsingExamples()
        {
            Console.WriteLine("--- Nested Using Examples ---");

            Console.WriteLine("Disposal order demonstration:");
            using (var outer = new OrderedDisposable("Outer"))
            {
                Console.WriteLine("  Inside outer using");
                
                using (var middle = new OrderedDisposable("Middle"))
                {
                    Console.WriteLine("  Inside middle using");
                    
                    using (var inner = new OrderedDisposable("Inner"))
                    {
                        Console.WriteLine("  Inside inner using");
                        Console.WriteLine("  Doing work with all resources");
                    }
                    Console.WriteLine("  After inner disposed");
                }
                Console.WriteLine("  After middle disposed");
            }
            Console.WriteLine("  After outer disposed");

            Console.WriteLine("\nNote: Resources disposed in reverse order (LIFO)");
            Console.WriteLine();
        }

        class OrderedDisposable : IDisposable
        {
            private readonly string _name;

            public OrderedDisposable(string name)
            {
                _name = name;
                Console.WriteLine($"    [{_name}] Created");
            }

            public void Dispose()
            {
                Console.WriteLine($"    [{_name}] Disposed");
            }
        }

        #endregion

        #region Common Pitfalls Examples

        /// <summary>
        /// Demonstrates common mistakes and pitfalls with using statement
        /// 
        /// SCENARIOS NOT TO USE:
        /// - Don't use with pooled objects that shouldn't be disposed
        /// - Don't dispose objects that will be used later
        /// - Don't create HttpClient in using (socket exhaustion)
        /// - Don't dispose objects you don't own
        /// </summary>
        static void CommonPitfallsExamples()
        {
            Console.WriteLine("--- Common Pitfalls Examples ---");

            // Pitfall 1: Using with null
            Console.WriteLine("Pitfall 1: Using with null");
            StreamWriter? writer = null;
            using (writer) // This is safe - Dispose() won't be called on null
            {
                Console.WriteLine("  Using with null is safe (no-op)");
            }

            // Pitfall 2: Disposing HttpClient in loop (DON'T DO THIS)
            Console.WriteLine("\nPitfall 2: HttpClient disposal");
            Console.WriteLine("  DON'T: Create and dispose HttpClient in loop");
            Console.WriteLine("  Causes socket exhaustion!");
            Console.WriteLine("  DO: Use singleton HttpClient instead");
            
            // Bad pattern (for demonstration only)
            // for (int i = 0; i < 1000; i++)
            // {
            //     using (HttpClient client = new HttpClient())
            //     {
            //         // This exhausts sockets!
            //     }
            // }

            // Good pattern
            // static readonly HttpClient _client = new HttpClient();

            // Pitfall 3: Using disposed object
            Console.WriteLine("\nPitfall 3: Using disposed object");
            var resource = new ManagedResource("TestResource");
            resource.Dispose();
            
            try
            {
                resource.DoWork(); // Throws ObjectDisposedException
            }
            catch (ObjectDisposedException ex)
            {
                Console.WriteLine($"  Caught: {ex.GetType().Name}");
            }

            // Pitfall 4: Disposing object you don't own
            Console.WriteLine("\nPitfall 4: Don't dispose objects you don't own");
            var collection = new List<IDisposable>
            {
                new ManagedResource("Shared1"),
                new ManagedResource("Shared2")
            };

            // BAD: Don't do this if other code uses the collection
            // using (collection[0])
            // {
            //     // Now collection[0] is disposed, but collection still references it!
            // }

            Console.WriteLine("  Rule: Only dispose objects you create");

            // Pitfall 5: Using with captured variables
            Console.WriteLine("\nPitfall 5: Careful with captured variables in closures");
            var actions = new List<Action>();
            
            for (int i = 0; i < 3; i++)
            {
                var tempResource = new ManagedResource($"Resource{i}");
                // BAD: Resource might be disposed before action runs
                actions.Add(() => tempResource.DoWork());
                tempResource.Dispose();
            }

            Console.WriteLine("  Captured disposed resources will throw exceptions");

            // Cleanup
            foreach (var res in collection)
            {
                res.Dispose();
            }

            Console.WriteLine();
        }

        #endregion
    }
}