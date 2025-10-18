using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace IteratorExamples
{
    /*
     * C# ITERATORS - COMPREHENSIVE EXAMPLES AND GUIDE
     * 
     * PURPOSE:
     * - Iterators provide a way to generate sequences of values on-demand (lazy evaluation)
     * - They implement IEnumerable/IEnumerator without manually writing state machines
     * - Use 'yield return' to produce values one at a time
     * - Use 'yield break' to terminate iteration early
     * 
     * SCENARIOS TO USE:
     * - Processing large datasets without loading everything into memory
     * - Generating infinite sequences (Fibonacci, prime numbers, etc.)
     * - Reading files line by line
     * - Creating custom collections with complex iteration logic
     * - Chain operations with LINQ for deferred execution
     * - When you need lazy evaluation for performance
     * 
     * SCENARIOS NOT TO USE:
     * - When you need random access to elements (use arrays/lists instead)
     * - When iteration order might change during enumeration
     * - For simple, small collections where overhead isn't worth it
     * - When you need to enumerate the same sequence multiple times efficiently
     * - In performance-critical loops where allocation overhead matters
     * 
     * MEMORY ALLOCATION:
     * - Iterators create state machines (classes) behind the scenes
     * - Each enumeration creates a new enumerator object
     * - Memory usage is typically O(1) for the iterator itself
     * - Deferred execution means values aren't computed until needed
     * - Be careful with closures capturing variables (can cause memory leaks)
     * 
     * MULTITHREADING ASPECTS:
     * - Iterators are NOT thread-safe by default
     * - Each thread should get its own enumerator instance
     * - Shared state in iterators can cause race conditions
     * - Consider using concurrent collections for thread-safe scenarios
     * - Avoid modifying captured variables from multiple threads
     */

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== C# ITERATOR EXAMPLES ===\n");

            // Example 1: Basic Iterator
            BasicIteratorExample();

            // Example 2: Fibonacci Sequence (Infinite Iterator)
            FibonacciExample();

            // Example 3: File Processing Iterator
            FileProcessingExample();

            // Example 4: Memory Allocation Demonstration
            MemoryAllocationExample();

            // Example 5: Threading Considerations
            ThreadingExample();

            // Example 6: Iterator with Parameters
            ParameterizedIteratorExample();

            // Example 7: Early Termination with yield break
            EarlyTerminationExample();

            // Example 8: Iterator Performance Comparison
            PerformanceComparisonExample();

            Console.WriteLine("\n=== END OF EXAMPLES ===");
        }

        #region Basic Iterator Example
        /// <summary>
        /// Demonstrates basic iterator usage with yield return
        /// MEMORY: Creates state machine, O(1) memory per iteration
        /// THREADING: Not thread-safe, each thread needs own enumerator
        /// </summary>
        static void BasicIteratorExample()
        {
            Console.WriteLine("1. BASIC ITERATOR EXAMPLE:");
            Console.WriteLine("Purpose: Generate numbers 1-5 with custom logic");

            foreach (int number in GetNumbers())
            {
                Console.WriteLine($"   Generated: {number}");
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Basic iterator method using yield return
        /// Each call to this method creates a NEW enumerator instance
        /// </summary>
        static IEnumerable<int> GetNumbers()
        {
            Console.WriteLine("   Iterator started"); // Called when enumeration begins
            
            yield return 1;
            Console.WriteLine("   After yielding 1");
            
            yield return 2;
            Console.WriteLine("   After yielding 2");
            
            yield return 3;
            Console.WriteLine("   After yielding 3");
            
            // Iterator ends here - no more yield statements
            Console.WriteLine("   Iterator completed");
        }
        #endregion

        #region Fibonacci Iterator Example
        /// <summary>
        /// Demonstrates infinite sequence generation
        /// SCENARIO: Perfect for mathematical sequences where you don't know how many you need
        /// MEMORY: O(1) - only stores current state, not entire sequence
        /// </summary>
        static void FibonacciExample()
        {
            Console.WriteLine("2. FIBONACCI SEQUENCE (INFINITE ITERATOR):");
            Console.WriteLine("Purpose: Generate Fibonacci numbers on-demand");
            Console.WriteLine("Benefit: No memory waste for unused numbers");

            int count = 0;
            foreach (long fib in GenerateFibonacci())
            {
                Console.WriteLine($"   Fib[{count}] = {fib}");
                count++;
                
                // Stop after 10 numbers to avoid infinite loop
                if (count >= 10) break;
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Infinite Fibonacci sequence generator
        /// MEMORY EFFICIENT: Only keeps track of last two numbers
        /// </summary>
        static IEnumerable<long> GenerateFibonacci()
        {
            long a = 0, b = 1;
            
            while (true) // Infinite sequence
            {
                yield return a;
                (a, b) = (b, a + b); // Tuple deconstruction for swap
            }
        }
        #endregion

        #region File Processing Example
        /// <summary>
        /// Simulates reading large files line by line
        /// SCENARIO: Processing large files without loading entire content into memory
        /// MEMORY: O(1) per line instead of O(n) for entire file
        /// </summary>
        static void FileProcessingExample()
        {
            Console.WriteLine("3. FILE PROCESSING ITERATOR:");
            Console.WriteLine("Purpose: Process large files line by line without memory overhead");

            // Simulate processing a large file
            foreach (string line in ReadLinesFromLargeFile())
            {
                if (line.Contains("ERROR"))
                {
                    Console.WriteLine($"   Found error: {line}");
                }
            }
            Console.WriteLine("   File processing completed");
            Console.WriteLine();
        }

        /// <summary>
        /// Simulates reading lines from a large file
        /// In real scenario, would use StreamReader.ReadLine()
        /// </summary>
        static IEnumerable<string> ReadLinesFromLargeFile()
        {
            // Simulate file content (in real world, this would be StreamReader)
            string[] simulatedFileContent = {
                "INFO: Application started",
                "DEBUG: User logged in",
                "ERROR: Database connection failed",
                "INFO: Retry attempt 1",
                "INFO: Connection successful",
                "ERROR: Validation failed for user input"
            };

            foreach (string line in simulatedFileContent)
            {
                // Simulate processing time
                Thread.Sleep(10);
                yield return line;
            }
        }
        #endregion

        #region Memory Allocation Demonstration
        /// <summary>
        /// Demonstrates memory allocation patterns of iterators
        /// Shows difference between eager and lazy evaluation
        /// </summary>
        static void MemoryAllocationExample()
        {
            Console.WriteLine("4. MEMORY ALLOCATION DEMONSTRATION:");
            Console.WriteLine("Comparing iterator vs. list allocation");

            // Iterator approach - lazy evaluation
            Console.WriteLine("   Iterator approach (lazy):");
            var iteratorSequence = GenerateExpensiveSequence();
            Console.WriteLine("   Iterator created (no computation yet)");
            
            // Only compute first 3 values
            int iteratorCount = 0;
            foreach (int value in iteratorSequence)
            {
                Console.WriteLine($"   Computed: {value}");
                if (++iteratorCount >= 3) break;
            }

            Console.WriteLine();

            // List approach - eager evaluation
            Console.WriteLine("   List approach (eager):");
            var listSequence = GenerateExpensiveSequenceAsList();
            Console.WriteLine("   All values computed and stored in memory");
            
            Console.WriteLine($"   Taking first 3 from list of {listSequence.Count}");
            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine($"   Retrieved: {listSequence[i]}");
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Iterator version - computes values on demand
        /// MEMORY: O(1) - only current computation in memory
        /// </summary>
        static IEnumerable<int> GenerateExpensiveSequence()
        {
            for (int i = 1; i <= 1_000_000; i++)
            {
                // Simulate expensive computation
                int result = i * i + i;
                yield return result;
            }
        }

        /// <summary>
        /// List version - computes all values upfront
        /// MEMORY: O(n) - all values stored in memory
        /// </summary>
        static List<int> GenerateExpensiveSequenceAsList()
        {
            var list = new List<int>();
            for (int i = 1; i <= 1_000_000; i++)
            {
                int result = i * i + i;
                list.Add(result);
            }
            return list;
        }
        #endregion

        #region Threading Example
        /// <summary>
        /// Demonstrates threading considerations with iterators
        /// IMPORTANT: Iterators are NOT thread-safe by default
        /// </summary>
        static void ThreadingExample()
        {
            Console.WriteLine("5. THREADING CONSIDERATIONS:");
            Console.WriteLine("Demonstrating thread-safety issues and solutions");

            // WRONG WAY - Sharing enumerator between threads (NOT THREAD-SAFE)
            Console.WriteLine("   Wrong way - shared enumerator:");
            DemonstrateSharedEnumeratorProblem();

            Console.WriteLine();

            // RIGHT WAY - Each thread gets its own enumerator
            Console.WriteLine("   Right way - separate enumerators:");
            DemonstrateSeparateEnumerators();

            Console.WriteLine();
        }

        /// <summary>
        /// Shows problems with sharing enumerators between threads
        /// DON'T DO THIS - Race conditions will occur
        /// </summary>
        static void DemonstrateSharedEnumeratorProblem()
        {
            var sequence = GetThreadUnsafeSequence();
            var enumerator = sequence.GetEnumerator();

            var tasks = new Task[2];
            
            // Both tasks share the same enumerator (BAD!)
            tasks[0] = Task.Run(() =>
            {
                for (int i = 0; i < 3; i++)
                {
                    if (enumerator.MoveNext())
                        Console.WriteLine($"   Thread 1: {enumerator.Current}");
                }
            });

            tasks[1] = Task.Run(() =>
            {
                for (int i = 0; i < 3; i++)
                {
                    if (enumerator.MoveNext())
                        Console.WriteLine($"   Thread 2: {enumerator.Current}");
                }
            });

            Task.WaitAll(tasks);
            enumerator.Dispose();
        }

        /// <summary>
        /// Shows correct way - each thread gets separate enumerator
        /// </summary>
        static void DemonstrateSeparateEnumerators()
        {
            var sequence = GetThreadSafeSequence();

            var tasks = new Task[2];

            // Each task gets its own enumerator (GOOD!)
            tasks[0] = Task.Run(() =>
            {
                foreach (int value in sequence) // New enumerator for this thread
                {
                    Console.WriteLine($"   Thread 1: {value}");
                    if (value >= 3) break;
                }
            });

            tasks[1] = Task.Run(() =>
            {
                foreach (int value in sequence) // New enumerator for this thread
                {
                    Console.WriteLine($"   Thread 2: {value}");
                    if (value >= 3) break;
                }
            });

            Task.WaitAll(tasks);
        }

        static IEnumerable<int> GetThreadUnsafeSequence()
        {
            for (int i = 1; i <= 10; i++)
            {
                Thread.Sleep(10); // Simulate work
                yield return i;
            }
        }

        static IEnumerable<int> GetThreadSafeSequence()
        {
            for (int i = 1; i <= 10; i++)
            {
                Thread.Sleep(10); // Simulate work
                yield return i;
            }
        }
        #endregion

        #region Parameterized Iterator Example
        /// <summary>
        /// Demonstrates iterators with parameters
        /// SCENARIO: Creating reusable iterator factories
        /// </summary>
        static void ParameterizedIteratorExample()
        {
            Console.WriteLine("6. PARAMETERIZED ITERATOR:");
            Console.WriteLine("Purpose: Create reusable iterator factories with parameters");

            Console.WriteLine("   Even numbers from 2 to 10:");
            foreach (int even in GetNumbersInRange(2, 10, 2))
            {
                Console.WriteLine($"   {even}");
            }

            Console.WriteLine("   Odd numbers from 1 to 9:");
            foreach (int odd in GetNumbersInRange(1, 9, 2))
            {
                Console.WriteLine($"   {odd}");
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Parameterized iterator for generating number sequences
        /// Parameters are captured in the iterator's closure
        /// MEMORY: Parameters are captured and stored with the iterator state
        /// </summary>
        static IEnumerable<int> GetNumbersInRange(int start, int end, int step)
        {
            Console.WriteLine($"   Starting sequence: {start} to {end}, step {step}");
            
            for (int i = start; i <= end; i += step)
            {
                yield return i;
            }
            
            Console.WriteLine($"   Completed sequence: {start} to {end}");
        }
        #endregion

        #region Early Termination Example
        /// <summary>
        /// Demonstrates yield break for early termination
        /// SCENARIO: Conditional iteration based on runtime conditions
        /// </summary>
        static void EarlyTerminationExample()
        {
            Console.WriteLine("7. EARLY TERMINATION WITH YIELD BREAK:");
            Console.WriteLine("Purpose: Conditionally terminate iteration");

            Console.WriteLine("   Numbers until first negative:");
            foreach (int number in GetNumbersUntilNegative())
            {
                Console.WriteLine($"   {number}");
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Iterator that terminates when encountering negative number
        /// Uses yield break for conditional termination
        /// </summary>
        static IEnumerable<int> GetNumbersUntilNegative()
        {
            int[] numbers = { 1, 2, 3, -1, 4, 5 };

            foreach (int number in numbers)
            {
                if (number < 0)
                {
                    Console.WriteLine("   Encountered negative number, stopping iteration");
                    yield break; // Terminates the iterator
                }
                
                yield return number;
            }
            
            Console.WriteLine("   Reached end of array");
        }
        #endregion

        #region Performance Comparison
        /// <summary>
        /// Compares performance characteristics of different approaches
        /// </summary>
        static void PerformanceComparisonExample()
        {
            Console.WriteLine("8. PERFORMANCE COMPARISON:");
            Console.WriteLine("Comparing iterator vs traditional approaches");

            const int itemCount = 1000;

            // Iterator approach
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            int iteratorSum = 0;
            int iteratorProcessed = 0;
            
            foreach (int value in GenerateSquares(itemCount))
            {
                iteratorSum += value;
                iteratorProcessed++;
                if (iteratorProcessed >= 10) break; // Only process first 10
            }
            
            stopwatch.Stop();
            Console.WriteLine($"   Iterator: Processed {iteratorProcessed} items in {stopwatch.ElapsedTicks} ticks");
            Console.WriteLine($"   Sum of first {iteratorProcessed}: {iteratorSum}");

            // Array approach
            stopwatch.Restart();
            var array = GenerateSquaresArray(itemCount);
            int arraySum = 0;
            int arrayProcessed = Math.Min(10, array.Length);
            
            for (int i = 0; i < arrayProcessed; i++)
            {
                arraySum += array[i];
            }
            
            stopwatch.Stop();
            Console.WriteLine($"   Array: Generated {array.Length} items, processed {arrayProcessed} in {stopwatch.ElapsedTicks} ticks");
            Console.WriteLine($"   Sum of first {arrayProcessed}: {arraySum}");
            Console.WriteLine($"   Memory overhead: Array used {array.Length * sizeof(int)} bytes");
            Console.WriteLine();
        }

        /// <summary>
        /// Iterator version - lazy evaluation
        /// </summary>
        static IEnumerable<int> GenerateSquares(int count)
        {
            for (int i = 1; i <= count; i++)
            {
                yield return i * i;
            }
        }

        /// <summary>
        /// Array version - eager evaluation
        /// </summary>
        static int[] GenerateSquaresArray(int count)
        {
            var array = new int[count];
            for (int i = 0; i < count; i++)
            {
                array[i] = (i + 1) * (i + 1);
            }
            return array;
        }
        #endregion
    }

    /*
     * ADDITIONAL BEST PRACTICES AND CONSIDERATIONS:
     * 
     * 1. DEFERRED EXECUTION:
     *    - Iterator methods don't execute until enumeration begins
     *    - Each foreach creates a new enumerator instance
     *    - Side effects in iterators can be surprising
     * 
     * 2. EXCEPTION HANDLING:
     *    - Exceptions in iterators are deferred until enumeration
     *    - Use try/finally carefully with yield statements
     *    - Consider wrapping iterators for immediate validation
     * 
     * 3. DISPOSAL:
     *    - Enumerators implement IDisposable
     *    - foreach automatically disposes enumerators
     *    - Manual enumeration requires explicit disposal
     * 
     * 4. DEBUGGING:
     *    - Iterators can be harder to debug due to state machines
     *    - Use logging/tracing within iterators carefully
     *    - Consider extracting complex logic from iterator methods
     * 
     * 5. PERFORMANCE NOTES:
     *    - Iterator overhead is small but exists
     *    - For tight loops, consider alternatives
     *    - LINQ chains with iterators provide excellent composability
     *    - Memory allocation happens for each enumerator instance
     */
}
