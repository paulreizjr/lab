using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

/* 
 * POINTERS AND UNSAFE CODE IN C#
 * 
 * PURPOSE:
 * - Direct memory access and manipulation
 * - Interoperability with unmanaged code (C/C++ libraries)
 * - Performance-critical operations where garbage collection overhead must be minimized
 * - Low-level data structure manipulation
 * - Working with memory-mapped files or hardware interfaces
 * 
 * SCENARIOS TO USE:
 * - Interop with native libraries that expect pointers
 * - Performance-critical algorithms (image processing, mathematical computations)
 * - Working with large data structures where copying would be expensive
 * - Implementing custom memory allocators
 * - Direct manipulation of byte arrays for protocols or file formats
 * 
 * SCENARIOS NOT TO USE:
 * - Regular business logic applications
 * - When type safety is paramount
 * - In web applications or most enterprise software
 * - When working with managed collections (List<T>, Dictionary<T,K>, etc.)
 * - Code that needs to be verifiable or run in partial trust environments
 * - When memory management complexity outweighs performance benefits
 * 
 * MEMORY ALLOCATION CONSIDERATIONS:
 * - Stack allocation: stackalloc for temporary, short-lived data
 * - Heap allocation: Marshal.AllocHGlobal/Marshal.FreeHGlobal for longer-lived data
 * - Pinning: fixed statement pins managed memory temporarily
 * - Always match allocations with deallocations to prevent memory leaks
 * 
 * MULTITHREADING ASPECTS:
 * - Pointers themselves are not thread-safe
 * - Multiple threads accessing the same memory location require synchronization
 * - Race conditions can cause memory corruption
 * - Use appropriate locking mechanisms (lock, Monitor, Interlocked operations)
 * - Consider memory barriers and volatile semantics
 */

namespace PointerExamples
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== C# Pointers and Unsafe Code Examples ===\n");

            // Enable unsafe code compilation with: <AllowUnsafeBlocks>true</AllowUnsafeBlocks> in .csproj

            BasicPointerOperations();
            StackAllocationExample();
            HeapAllocationExample();
            StructPointerExample();
            ArrayPointerExample();
            StringPointerExample();
            InteropExample();
            MultithreadingExample();
            PerformanceExample();

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        /// <summary>
        /// Demonstrates basic pointer operations and syntax
        /// </summary>
        static unsafe void BasicPointerOperations()
        {
            Console.WriteLine("1. Basic Pointer Operations:");
            
            int value = 42;
            int* ptr = &value;  // Get address of value
            
            Console.WriteLine($"   Value: {value}");
            Console.WriteLine($"   Address: 0x{(long)ptr:X}");
            Console.WriteLine($"   Value via pointer: {*ptr}");
            
            // Modify value through pointer
            *ptr = 100;
            Console.WriteLine($"   Modified value: {value}");
            
            // Pointer arithmetic
            int[] array = { 1, 2, 3, 4, 5 };
            fixed (int* arrayPtr = array)
            {
                Console.WriteLine("   Array access via pointer arithmetic:");
                for (int i = 0; i < array.Length; i++)
                {
                    Console.WriteLine($"     array[{i}] = {*(arrayPtr + i)}");
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Stack allocation using stackalloc - fast but limited to current method scope
        /// MEMORY ALLOCATION: Stack-allocated memory is automatically freed when scope ends
        /// </summary>
        static unsafe void StackAllocationExample()
        {
            Console.WriteLine("2. Stack Allocation Example:");
            
            // Allocate array on stack - fast, no GC pressure
            // WARNING: Limited by stack size (typically ~1MB)
            const int size = 1000;
            int* stackArray = stackalloc int[size];
            
            // Initialize and use stack-allocated memory
            for (int i = 0; i < size; i++)
            {
                stackArray[i] = i * i;
            }
            
            Console.WriteLine($"   Stack-allocated array[100] = {stackArray[100]}");
            Console.WriteLine("   Stack memory automatically freed at method end");
            
            // Span<T> provides a safer alternative for stack allocation
            Span<int> spanArray = stackalloc int[size];
            for (int i = 0; i < size; i++)
            {
                spanArray[i] = i * 2;
            }
            Console.WriteLine($"   Span approach: spanArray[100] = {spanArray[100]}");
            Console.WriteLine();
        }

        /// <summary>
        /// Heap allocation for unmanaged memory
        /// MEMORY ALLOCATION: Manually managed - must call Marshal.FreeHGlobal
        /// </summary>
        static unsafe void HeapAllocationExample()
        {
            Console.WriteLine("3. Heap Allocation Example:");
            
            const int size = 1000;
            IntPtr unmanagedPtr = Marshal.AllocHGlobal(size * sizeof(int));
            
            try
            {
                int* heapArray = (int*)unmanagedPtr.ToPointer();
                
                // Initialize unmanaged memory
                for (int i = 0; i < size; i++)
                {
                    heapArray[i] = i * 3;
                }
                
                Console.WriteLine($"   Heap-allocated array[100] = {heapArray[100]}");
                Console.WriteLine("   Heap memory must be manually freed");
            }
            finally
            {
                // CRITICAL: Always free unmanaged memory
                Marshal.FreeHGlobal(unmanagedPtr);
                Console.WriteLine("   Heap memory freed");
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Working with struct pointers for high-performance data manipulation
        /// </summary>
        static unsafe void StructPointerExample()
        {
            Console.WriteLine("4. Struct Pointer Example:");
            
            // Define a simple point structure
            Point point = new Point { X = 10, Y = 20 };
            Point* pointPtr = &point;
            
            Console.WriteLine($"   Original point: ({point.X}, {point.Y})");
            
            // Modify through pointer
            pointPtr->X = 50;
            pointPtr->Y = 60;
            
            Console.WriteLine($"   Modified point: ({point.X}, {point.Y})");
            
            // Array of structs with pointer access
            Point[] points = new Point[3];
            fixed (Point* pointsPtr = points)
            {
                for (int i = 0; i < points.Length; i++)
                {
                    (pointsPtr + i)->X = i * 10;
                    (pointsPtr + i)->Y = i * 20;
                }
                
                Console.WriteLine("   Points array via pointers:");
                for (int i = 0; i < points.Length; i++)
                {
                    Point* currentPoint = pointsPtr + i;
                    Console.WriteLine($"     Point[{i}]: ({currentPoint->X}, {currentPoint->Y})");
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Demonstrates fixed statement for pinning managed arrays
        /// MEMORY ALLOCATION: Temporarily pins managed memory to prevent GC movement
        /// </summary>
        static unsafe void ArrayPointerExample()
        {
            Console.WriteLine("5. Array Pointer Example:");
            
            byte[] managedArray = new byte[100];
            
            // Pin managed array in memory
            fixed (byte* arrayPtr = managedArray)
            {
                // Fast memory operations using pointers
                for (int i = 0; i < managedArray.Length; i++)
                {
                    *(arrayPtr + i) = (byte)(i % 256);
                }
                
                Console.WriteLine("   Filled managed array via pointer");
                Console.WriteLine($"   First 10 bytes: {string.Join(", ", managedArray[0..10])}");
            }
            // Array is automatically unpinned here
            Console.WriteLine();
        }

        /// <summary>
        /// String manipulation using pointers (advanced scenario)
        /// </summary>
        static unsafe void StringPointerExample()
        {
            Console.WriteLine("6. String Pointer Example:");
            
            string text = "Hello, World!";
            
            fixed (char* charPtr = text)
            {
                Console.WriteLine("   String characters via pointer:");
                for (int i = 0; i < text.Length; i++)
                {
                    Console.Write($"'{*(charPtr + i)}' ");
                }
                Console.WriteLine();
                
                // Note: Strings are immutable, so we can't modify them
                // This is just for reading the underlying character data
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Interoperability example with native code
        /// </summary>
        static unsafe void InteropExample()
        {
            Console.WriteLine("7. Interop Example:");
            
            // Simulate native function call that expects a buffer
            byte[] buffer = new byte[256];
            
            fixed (byte* bufferPtr = buffer)
            {
                // In real scenarios, this would be a P/Invoke call to native code
                FillBufferSimulated(bufferPtr, buffer.Length);
            }
            
            Console.WriteLine($"   Buffer filled by 'native' function");
            Console.WriteLine($"   First 10 bytes: {string.Join(", ", buffer[0..10])}");
            Console.WriteLine();
        }

        /// <summary>
        /// Simulates a native function that fills a buffer
        /// </summary>
        static unsafe void FillBufferSimulated(byte* buffer, int size)
        {
            for (int i = 0; i < size; i++)
            {
                buffer[i] = (byte)(i % 256);
            }
        }

        /// <summary>
        /// Demonstrates multithreading considerations with pointers
        /// MULTITHREADING: Shows synchronization requirements for shared pointer access
        /// </summary>
        static unsafe void MultithreadingExample()
        {
            Console.WriteLine("8. Multithreading Example:");
            
            const int arraySize = 1000000;
            int[] sharedArray = new int[arraySize];
            object lockObject = new object();
            
            // WARNING: Without proper synchronization, this would cause race conditions
            Parallel.For(0, Environment.ProcessorCount, threadIndex =>
            {
                int start = threadIndex * (arraySize / Environment.ProcessorCount);
                int end = (threadIndex + 1) * (arraySize / Environment.ProcessorCount);
                
                // Each thread works on its own section - no synchronization needed
                fixed (int* arrayPtr = sharedArray)
                {
                    for (int i = start; i < end && i < arraySize; i++)
                    {
                        *(arrayPtr + i) = i * threadIndex;
                    }
                }
            });
            
            Console.WriteLine("   Multiple threads filled array sections safely");
            Console.WriteLine($"   Sample values: {sharedArray[100]}, {sharedArray[500]}, {sharedArray[900]}");
            
            // Example of synchronized access (when threads need to access same memory)
            int sharedCounter = 0;
            int* counterPtr = &sharedCounter;
            
            Parallel.For(0, 1000, i =>
            {
                lock (lockObject)
                {
                    (*counterPtr)++;  // Synchronized increment
                }
            });
            
            Console.WriteLine($"   Synchronized counter: {sharedCounter}");
            Console.WriteLine();
        }

        /// <summary>
        /// Performance comparison between safe and unsafe operations
        /// </summary>
        static unsafe void PerformanceExample()
        {
            Console.WriteLine("9. Performance Example:");
            
            const int iterations = 10000000;
            int[] array = new int[iterations];
            
            // Safe approach
            var sw = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
            {
                array[i] = i * 2;
            }
            sw.Stop();
            long safeTime = sw.ElapsedMilliseconds;
            
            // Reset array
            Array.Clear(array, 0, array.Length);
            
            // Unsafe approach
            sw.Restart();
            fixed (int* arrayPtr = array)
            {
                for (int i = 0; i < iterations; i++)
                {
                    *(arrayPtr + i) = i * 2;
                }
            }
            sw.Stop();
            long unsafeTime = sw.ElapsedMilliseconds;
            
            Console.WriteLine($"   Safe approach:   {safeTime}ms");
            Console.WriteLine($"   Unsafe approach: {unsafeTime}ms");
            Console.WriteLine($"   Performance gain: {((double)safeTime / unsafeTime):F2}x");
            Console.WriteLine();
            
            Console.WriteLine("   Note: Performance gains are typically modest and come with");
            Console.WriteLine("   increased complexity and reduced safety. Use judiciously.");
        }
    }

    /// <summary>
    /// Simple point structure for pointer examples
    /// </summary>
    public struct Point
    {
        public int X;
        public int Y;
    }
}
