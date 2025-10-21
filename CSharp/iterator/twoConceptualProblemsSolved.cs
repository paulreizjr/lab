using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iterator
{
    internal class twoConceptualProblemsSolved
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Problem 1: Large File Processing ===");
            DemonstrateFileProcessing();
            
            Console.WriteLine("\n=== Problem 2: Fibonacci Sequence Generation ===");
            DemonstrateFibonacci();
            
            Console.ReadLine();
        }

        #region Problem 1: Processing Large Files Line by Line

        /// <summary>
        /// PROBLEM 1: Large File Processing
        /// 
        /// You need to process a very large log file (potentially gigabytes) that contains
        /// millions of lines. Each line represents a user action with timestamp and details.
        /// You only want to process lines that match certain criteria (e.g., error logs from today).
        /// 
        /// CHALLENGES:
        /// - Loading the entire file into memory would cause OutOfMemoryException
        /// - You need to filter lines based on conditions
        /// - Processing should be lazy - only read what you need
        /// - Should be memory efficient and performant
        /// 
        /// This is a perfect use case for iterators because:
        /// - Deferred execution: Lines are only read when enumerated
        /// - Memory efficient: Only one line in memory at a time
        /// - Composable: Can chain multiple filtering operations
        /// - Lazy evaluation: Stops processing when you break out of loop
        /// </summary>
        static void DemonstrateFileProcessing()
        {
            // Simulate a large log file with sample data
            var sampleLogLines = new string[]
            {
                "2024-10-17 09:15:23 INFO User login: john@example.com",
                "2024-10-17 09:16:45 ERROR Database connection failed",
                "2024-10-17 09:17:12 INFO User logout: jane@example.com", 
                "2024-10-17 09:18:33 ERROR Null reference exception in OrderService",
                "2024-10-16 08:45:12 ERROR Old error from yesterday",
                "2024-10-17 09:19:44 INFO New order created: Order#12345"
            };

            Console.WriteLine("Good Solution using Iterator:");
            // Using the iterator - memory efficient, lazy evaluation
            var todaysErrors = GetTodaysErrorLogs(sampleLogLines);
            
            foreach (var errorLog in todaysErrors)
            {
                Console.WriteLine($"Found error: {errorLog}");
                // In real scenario, you might break early after finding what you need
                // The iterator ensures no unnecessary processing happens
            }

            Console.WriteLine("\nBad Solution (loading everything):");
            // Demonstrating the bad approach
            var allErrorsInMemory = GetAllErrorsInMemoryBadApproach(sampleLogLines);
            foreach (var error in allErrorsInMemory.Where(e => e.Contains("2024-10-17")))
            {
                Console.WriteLine($"Found error: {error}");
            }
        }

        /// <summary>
        /// GOOD SOLUTION: Iterator method for processing large files
        /// 
        /// This method uses yield return to create an iterator that:
        /// - Reads one line at a time (memory efficient)
        /// - Only processes lines when they're actually enumerated (lazy)
        /// - Allows early termination without processing remaining lines
        /// - Can be easily composed with other LINQ operations
        /// </summary>
        /// <param name="logLines">Simulated file lines (in real scenario, this would be File.ReadLines())</param>
        /// <returns>Filtered error logs from today</returns>
        static IEnumerable<string> GetTodaysErrorLogs(IEnumerable<string> logLines)
        {
            string today = DateTime.Now.ToString("yyyy-MM-dd");
            
            foreach (string line in logLines)
            {
                // Simulate some processing cost
                Console.WriteLine($"Processing line: {line.Substring(0, Math.Min(30, line.Length))}...");
                
                // Only yield lines that match our criteria
                if (line.Contains(today) && line.Contains("ERROR"))
                {
                    yield return line;
                }
                
                // The beauty of iterators: if the caller breaks out of the loop,
                // we stop processing here automatically
            }
        }

        /// <summary>
        /// BAD SOLUTION: Loading everything into memory
        /// 
        /// This approach has several problems:
        /// - Loads ALL lines into memory at once (OutOfMemoryException risk)
        /// - Processes ALL lines even if we only need a few
        /// - Cannot break early - all processing happens upfront
        /// - Creates unnecessary memory pressure
        /// - Not composable or reusable
        /// </summary>
        /// <param name="logLines">All log lines</param>
        /// <returns>All error lines loaded in memory</returns>
        static List<string> GetAllErrorsInMemoryBadApproach(IEnumerable<string> logLines)
        {
            var allErrors = new List<string>();
            
            // BAD: Process and store everything in memory
            foreach (string line in logLines)
            {
                Console.WriteLine($"Loading into memory: {line.Substring(0, Math.Min(30, line.Length))}...");
                
                if (line.Contains("ERROR"))
                {
                    allErrors.Add(line); // Consuming memory for ALL errors
                }
            }
            
            // BAD: Caller has to filter again, and we've already done all the work
            return allErrors;
        }

        #endregion

        #region Problem 2: Infinite Fibonacci Sequence Generation

        /// <summary>
        /// PROBLEM 2: Infinite Mathematical Sequence Generation (Fibonacci)
        /// 
        /// You need to generate Fibonacci numbers on-demand for various purposes:
        /// - Sometimes you need just the first 10 numbers
        /// - Sometimes you need numbers until they exceed a certain value
        /// - Sometimes you need to find the first Fibonacci number divisible by a specific number
        /// 
        /// CHALLENGES:
        /// - The sequence is potentially infinite
        /// - You don't know in advance how many numbers you'll need
        /// - Pre-calculating all numbers would be impossible/wasteful
        /// - Different consumers need different amounts of the sequence
        /// 
        /// This is perfect for iterators because:
        /// - Infinite sequences can be represented
        /// - Numbers are generated only when requested (lazy)
        /// - Memory usage stays constant regardless of sequence length
        /// - Can be consumed by different algorithms with different stopping criteria
        /// </summary>
        static void DemonstrateFibonacci()
        {
            Console.WriteLine("Good Solution using Iterator:");
            
            // Example 1: Get first 10 Fibonacci numbers
            Console.WriteLine("First 10 Fibonacci numbers:");
            foreach (var fib in GenerateFibonacci().Take(10))
            {
                Console.WriteLine(fib);
            }
            
            // Example 2: Get Fibonacci numbers less than 1000
            Console.WriteLine("\nFibonacci numbers less than 1000:");
            foreach (var fib in GenerateFibonacci().TakeWhile(x => x < 1000))
            {
                Console.WriteLine(fib);
            }
            
            // Example 3: Find first Fibonacci number divisible by 17
            var firstDivisibleBy17 = GenerateFibonacci().First(x => x % 17 == 0);
            Console.WriteLine($"\nFirst Fibonacci number divisible by 17: {firstDivisibleBy17}");

            Console.WriteLine("\nBad Solution (pre-calculating array):");
            // Demonstrating the bad approach
            var fibArray = GenerateFibonacciArrayBadApproach(20); // Had to guess how many we need!
            Console.WriteLine($"Pre-calculated array length: {fibArray.Length}");
            Console.WriteLine($"First 5 from array: {string.Join(", ", fibArray.Take(5))}");
        }

        /// <summary>
        /// GOOD SOLUTION: Iterator for infinite Fibonacci sequence
        /// 
        /// This iterator method:
        /// - Generates Fibonacci numbers on-demand (lazy evaluation)
        /// - Uses constant memory (only stores last two numbers)
        /// - Can generate infinite sequence without running out of memory
        /// - Allows consumers to decide when to stop
        /// - Composable with LINQ operations (Take, TakeWhile, First, etc.)
        /// </summary>
        /// <returns>Infinite sequence of Fibonacci numbers</returns>
        static IEnumerable<long> GenerateFibonacci()
        {
            long previous = 0;
            long current = 1;
            
            // Start with the first two Fibonacci numbers
            yield return previous; // 0
            yield return current;  // 1
            
            // Generate infinite sequence
            while (true)
            {
                long next = previous + current;
                yield return next;
                
                // Update for next iteration - constant memory usage
                previous = current;
                current = next;
                
                // The iterator only calculates the next number when requested
                // If the consumer breaks or stops enumeration, we stop here
            }
        }

        /// <summary>
        /// BAD SOLUTION: Pre-calculating Fibonacci array
        /// 
        /// Problems with this approach:
        /// - Must decide upfront how many numbers to generate
        /// - Wastes memory storing numbers that might not be used
        /// - Cannot handle infinite or very large sequences
        /// - Inflexible - consumers can't determine their own stopping criteria
        /// - All computation happens upfront, even for unused numbers
        /// - Risk of integer overflow for large sequences
        /// </summary>
        /// <param name="count">Number of Fibonacci numbers to pre-calculate</param>
        /// <returns>Array containing pre-calculated Fibonacci numbers</returns>
        static long[] GenerateFibonacciArrayBadApproach(int count)
        {
            // BAD: Must specify exact count upfront
            var fibArray = new long[count];
            
            if (count > 0) fibArray[0] = 0;
            if (count > 1) fibArray[1] = 1;
            
            // BAD: Calculate ALL numbers whether they'll be used or not
            for (int i = 2; i < count; i++)
            {
                fibArray[i] = fibArray[i - 1] + fibArray[i - 2];
                Console.WriteLine($"Pre-calculating: F({i}) = {fibArray[i]}");
            }
            
            // BAD: What if the consumer needs more numbers? 
            // What if they need fewer? Memory wasted either way.
            return fibArray;
        }

        #endregion

        #region Additional Examples and Comparisons

        /// <summary>
        /// SUMMARY OF WHY ITERATORS ARE SUPERIOR FOR THESE PROBLEMS:
        /// 
        /// 1. MEMORY EFFICIENCY:
        ///    - Iterators use O(1) memory regardless of sequence length
        ///    - Bad solutions use O(n) memory, risking OutOfMemoryException
        /// 
        /// 2. LAZY EVALUATION:
        ///    - Work is done only when needed
        ///    - Can break early without wasting computation
        ///    - Bad solutions do all work upfront
        /// 
        /// 3. COMPOSABILITY:
        ///    - Iterators work seamlessly with LINQ
        ///    - Can chain operations (Where, Take, TakeWhile, etc.)
        ///    - Bad solutions require additional processing steps
        /// 
        /// 4. FLEXIBILITY:
        ///    - Consumers decide when to stop
        ///    - Same iterator can serve different needs
        ///    - Bad solutions lock in decisions at creation time
        /// 
        /// 5. INFINITE SEQUENCES:
        ///    - Iterators can represent infinite sequences
        ///    - Bad solutions cannot handle infinite data
        /// </summary>
        static void PrintIteratorAdvantages()
        {
            Console.WriteLine("Iterator advantages demonstrated above:");
            Console.WriteLine("- Memory efficiency");
            Console.WriteLine("- Lazy evaluation"); 
            Console.WriteLine("- Composability with LINQ");
            Console.WriteLine("- Flexibility for different use cases");
            Console.WriteLine("- Support for infinite sequences");
        }

        #endregion
    }
}
