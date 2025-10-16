using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DelegateExamples
{
    /// <summary>
    /// Comprehensive examples of delegates in C#
    /// 
    /// PURPOSE:
    /// - Delegates are type-safe function pointers that reference methods
    /// - Enable callback mechanisms and event-driven programming
    /// - Allow methods to be passed as parameters to other methods
    /// - Provide the foundation for events and LINQ expressions
    /// - Support multicast operations (invoking multiple methods)
    /// 
    /// SCENARIOS TO USE:
    /// - Event handling (button clicks, data changes, notifications)
    /// - Callback methods (async operations, completion handlers)
    /// - LINQ and functional programming patterns
    /// - Strategy pattern implementation (swappable algorithms)
    /// - Decoupling components in loosely-coupled architectures
    /// - Custom sorting and filtering logic
    /// 
    /// SCENARIOS NOT TO USE:
    /// - When direct method calls are sufficient and clearer
    /// - Performance-critical tight loops (delegate invocation has overhead)
    /// - Simple sequential code without callbacks
    /// - When interfaces would provide better abstraction
    /// - Overuse can lead to hard-to-debug code
    /// 
    /// MEMORY ALLOCATION:
    /// - Each delegate instance allocates ~40-80 bytes on the heap
    /// - Multicast delegates create linked list of invocation targets
    /// - Lambda captures create closure objects (additional heap allocation)
    /// - Static method delegates are lighter than instance method delegates
    /// - Delegate caching can reduce allocations in hot paths
    /// 
    /// MULTITHREAD ASPECTS:
    /// - Delegates themselves are immutable and thread-safe
    /// - Multicast delegate invocation is NOT thread-safe during add/remove
    /// - Event keyword provides thread-safe add/remove operations
    /// - Target method thread-safety depends on implementation
    /// - Be cautious with captured variables in multi-threaded scenarios
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Delegate Examples ===\n");

            // Example 1: Basic Delegate Declaration and Usage
            BasicDelegateExamples();

            // Example 2: Multicast Delegates
            MulticastDelegateExamples();

            // Example 3: Built-in Delegates (Action, Func, Predicate)
            BuiltInDelegateExamples();

            // Example 4: Delegates as Callbacks
            CallbackDelegateExamples();

            // Example 5: Delegates with LINQ
            LinqDelegateExamples();

            // Example 6: Event Pattern with Delegates
            EventDelegateExamples();

            // Example 7: Anonymous Methods and Lambdas
            AnonymousMethodsAndLambdas();

            // Example 8: Delegate Performance and Memory
            PerformanceAndMemoryExamples();

            // Example 9: Thread Safety with Delegates
            ThreadSafetyExamples();

            Console.WriteLine("\n=== All Examples Completed ===");
        }

        #region Basic Delegate Examples

        // Delegate declaration - defines a type that can reference methods
        // MEMORY: Delegate type itself is metadata, instances allocate ~40-80 bytes
        delegate int MathOperation(int x, int y);
        delegate void LogMessage(string message);
        delegate bool ValidationRule(string input);

        /// <summary>
        /// Demonstrates basic delegate declaration, instantiation, and invocation
        /// 
        /// SCENARIOS TO USE:
        /// - When you need to pass behavior as a parameter
        /// - Implementing callback patterns
        /// - Creating flexible, configurable operations
        /// 
        /// MEMORY ALLOCATION:
        /// - Each delegate instance allocates on heap
        /// - Static method delegates are slightly lighter
        /// - Caching delegates can improve performance
        /// </summary>
        static void BasicDelegateExamples()
        {
            Console.WriteLine("--- Basic Delegate Examples ---");

            // Create delegate instance pointing to a method
            // MEMORY: Allocates delegate object on heap (~40-80 bytes)
            MathOperation addOperation = Add;
            MathOperation multiplyOperation = Multiply;

            // Invoke delegates
            int result1 = addOperation(5, 3);
            int result2 = multiplyOperation(5, 3);

            Console.WriteLine($"Add: 5 + 3 = {result1}");
            Console.WriteLine($"Multiply: 5 * 3 = {result2}");

            // Passing delegates as parameters
            // GOOD FOR: Strategy pattern, flexible behavior injection
            int addResult = PerformOperation(10, 20, Add);
            int subtractResult = PerformOperation(10, 20, Subtract);

            Console.WriteLine($"PerformOperation with Add: {addResult}");
            Console.WriteLine($"PerformOperation with Subtract: {subtractResult}");

            // Using delegate for validation
            ValidationRule emailValidator = IsValidEmail;
            ValidationRule lengthValidator = IsValidLength;

            Console.WriteLine($"Is 'test@example.com' valid email? {emailValidator("test@example.com")}");
            Console.WriteLine($"Is 'hello' valid length? {lengthValidator("hello")}");

            Console.WriteLine();
        }

        static int Add(int x, int y) => x + y;
        static int Subtract(int x, int y) => x - y;
        static int Multiply(int x, int y) => x * y;

        static int PerformOperation(int a, int b, MathOperation operation)
        {
            // Delegate allows us to execute different operations without knowing which one
            return operation(a, b);
        }

        static bool IsValidEmail(string input) => input.Contains("@");
        static bool IsValidLength(string input) => input.Length >= 5;

        #endregion

        #region Multicast Delegate Examples

        /// <summary>
        /// Demonstrates multicast delegates - delegates that reference multiple methods
        /// 
        /// SCENARIOS TO USE:
        /// - Event broadcasting to multiple subscribers
        /// - Notification systems
        /// - Observer pattern implementation
        /// 
        /// SCENARIOS NOT TO USE:
        /// - When you need return values (only last method's return is used)
        /// - Performance-critical paths (invokes all methods sequentially)
        /// - When order of execution is critical but undefined
        /// 
        /// MEMORY ALLOCATION:
        /// - Creates linked list of invocation targets
        /// - Each addition creates new delegate instance
        /// - Can grow significantly with many subscribers
        /// 
        /// MULTITHREAD ASPECTS:
        /// - Adding/removing delegates is NOT thread-safe
        /// - Use events or explicit locking for thread safety
        /// - Each method in chain executes sequentially on calling thread
        /// </summary>
        static void MulticastDelegateExamples()
        {
            Console.WriteLine("--- Multicast Delegate Examples ---");

            LogMessage? logger = null;

            // Adding multiple methods to delegate
            // MEMORY: Each += creates a new delegate instance
            logger += LogToConsole;
            logger += LogToDebug;
            logger += LogWithTimestamp;

            // Invoke all methods in the delegate chain
            Console.WriteLine("Invoking multicast delegate:");
            logger?.Invoke("This is a multicast message");

            Console.WriteLine("\nRemoving LogToDebug:");
            logger -= LogToDebug; // Remove one method

            logger?.Invoke("After removing one handler");

            // Demonstrating that return values are problematic with multicast
            MathOperation multiOp = Add;
            multiOp += Multiply;
            multiOp += Subtract;

            // WARNING: Only the last delegate's return value is returned!
            int result = multiOp(10, 5);
            Console.WriteLine($"\nMulticast with return value (only last result): {result}");

            Console.WriteLine();
        }

        static void LogToConsole(string message)
        {
            Console.WriteLine($"[Console] {message}");
        }

        static void LogToDebug(string message)
        {
            Debug.WriteLine($"[Debug] {message}");
        }

        static void LogWithTimestamp(string message)
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {message}");
        }

        #endregion

        #region Built-in Delegate Examples

        /// <summary>
        /// Demonstrates built-in generic delegates: Action, Func, Predicate
        /// 
        /// PURPOSE:
        /// - Action<T>: delegates that return void
        /// - Func<T, TResult>: delegates that return a value
        /// - Predicate<T>: delegates that return bool (specialized Func)
        /// 
        /// SCENARIOS TO USE:
        /// - Preferred over custom delegate declarations
        /// - LINQ operations
        /// - Async callbacks
        /// - Event handlers (when simple)
        /// 
        /// MEMORY ALLOCATION:
        /// - Same as custom delegates (~40-80 bytes per instance)
        /// - Generic types avoid boxing for value types
        /// - Lambda expressions may create closure objects
        /// </summary>
        static void BuiltInDelegateExamples()
        {
            Console.WriteLine("--- Built-in Delegate Examples ---");

            // Action<T> - method with parameters, no return value
            // GOOD FOR: Side effects, operations that don't return values
            Action<string> print = message => Console.WriteLine(message);
            Action<int, int> printSum = (a, b) => Console.WriteLine($"Sum: {a + b}");

            print("Using Action delegate");
            printSum(10, 20);

            // Func<T, TResult> - method with parameters and return value
            // GOOD FOR: Transformations, calculations, data retrieval
            Func<int, int, int> add = (x, y) => x + y;
            Func<string, int> getLength = str => str.Length;
            Func<int> getRandom = () => new Random().Next(100);

            Console.WriteLine($"Func add: {add(5, 7)}");
            Console.WriteLine($"Func getLength: {getLength("Hello")}");
            Console.WriteLine($"Func getRandom: {getRandom()}");

            // Predicate<T> - method that takes parameter and returns bool
            // GOOD FOR: Filtering, validation, conditional logic
            Predicate<int> isEven = num => num % 2 == 0;
            Predicate<string> isNotEmpty = str => !string.IsNullOrEmpty(str);

            Console.WriteLine($"Is 4 even? {isEven(4)}");
            Console.WriteLine($"Is empty string valid? {isNotEmpty("")}");

            // Using with collections
            var numbers = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var evenNumbers = numbers.FindAll(isEven); // Predicate used for filtering

            Console.WriteLine($"Even numbers: {string.Join(", ", evenNumbers)}");

            Console.WriteLine();
        }

        #endregion

        #region Callback Delegate Examples

        /// <summary>
        /// Demonstrates delegates used as callbacks for asynchronous operations
        /// 
        /// SCENARIOS TO USE:
        /// - Async operation completion handlers
        /// - Progress reporting
        /// - Custom event notifications
        /// - Decoupled communication between components
        /// 
        /// MEMORY ALLOCATION:
        /// - Callback delegate allocates on heap
        /// - Captured variables in closures create additional allocations
        /// - Consider weak references for long-lived callbacks
        /// </summary>
        static void CallbackDelegateExamples()
        {
            Console.WriteLine("--- Callback Delegate Examples ---");

            // Simulate async operation with callback
            Console.WriteLine("Starting async operation...");
            PerformAsyncOperation(result =>
            {
                Console.WriteLine($"Async operation completed with result: {result}");
            });

            // Progress reporting with callback
            Console.WriteLine("\nProcessing items with progress callback:");
            ProcessItemsWithProgress(10, progress =>
            {
                Console.WriteLine($"Progress: {progress}%");
            });

            // Error handling with callbacks
            Console.WriteLine("\nOperation with success and error callbacks:");
            RiskyOperation(
                onSuccess: result => Console.WriteLine($"Success: {result}"),
                onError: error => Console.WriteLine($"Error: {error}")
            );

            Console.WriteLine();
        }

        static void PerformAsyncOperation(Action<string> onComplete)
        {
            // Simulate work
            Thread.Sleep(500);
            onComplete("Operation successful");
        }

        static void ProcessItemsWithProgress(int totalItems, Action<int> onProgress)
        {
            for (int i = 0; i < totalItems; i++)
            {
                Thread.Sleep(50); // Simulate work
                int progressPercentage = ((i + 1) * 100) / totalItems;
                onProgress(progressPercentage);
            }
        }

        static void RiskyOperation(Action<string> onSuccess, Action<string> onError)
        {
            try
            {
                // Simulate risky operation
                if (DateTime.Now.Second % 2 == 0)
                {
                    onSuccess("Data retrieved successfully");
                }
                else
                {
                    throw new InvalidOperationException("Random failure occurred");
                }
            }
            catch (Exception ex)
            {
                onError(ex.Message);
            }
        }

        #endregion

        #region LINQ Delegate Examples

        /// <summary>
        /// Demonstrates delegates in LINQ operations
        /// 
        /// PURPOSE:
        /// - LINQ methods accept delegates (Func, Predicate) for operations
        /// - Enables functional programming style in C#
        /// - Provides declarative data manipulation
        /// 
        /// SCENARIOS TO USE:
        /// - Data filtering, transformation, aggregation
        /// - Query composition
        /// - Functional pipeline operations
        /// 
        /// MEMORY ALLOCATION:
        /// - Lambda expressions create delegate instances
        /// - LINQ deferred execution may cache delegates
        /// - Complex closures can create significant allocations
        /// </summary>
        static void LinqDelegateExamples()
        {
            Console.WriteLine("--- LINQ Delegate Examples ---");

            var products = new List<Product>
            {
                new Product { Name = "Laptop", Price = 999.99m, Category = "Electronics" },
                new Product { Name = "Mouse", Price = 29.99m, Category = "Electronics" },
                new Product { Name = "Desk", Price = 299.99m, Category = "Furniture" },
                new Product { Name = "Chair", Price = 199.99m, Category = "Furniture" },
                new Product { Name = "Monitor", Price = 349.99m, Category = "Electronics" }
            };

            // Where uses Func<T, bool> delegate
            var expensiveProducts = products.Where(p => p.Price > 200);
            Console.WriteLine("Expensive products (> $200):");
            foreach (var p in expensiveProducts)
            {
                Console.WriteLine($"  {p.Name}: ${p.Price}");
            }

            // Select uses Func<T, TResult> delegate for transformation
            var productNames = products.Select(p => p.Name.ToUpper());
            Console.WriteLine($"\nProduct names: {string.Join(", ", productNames)}");

            // OrderBy uses Func<T, TKey> delegate for sorting
            var sortedByPrice = products.OrderBy(p => p.Price);
            Console.WriteLine("\nSorted by price:");
            foreach (var p in sortedByPrice)
            {
                Console.WriteLine($"  {p.Name}: ${p.Price}");
            }

            // Aggregate uses Func<TAccumulate, TSource, TAccumulate> delegate
            // what is this 0m? It's a decimal literal for zero
            // what (total, product) means? It's a lambda that takes the current total and a product, returning the new total
            // are we passing 3 parameters to Aggregate? Yes, the first is the seed (0m), the second is the lambda
            // the lambda takes two parameters: the accumulated total and the current product, and returns the new total
            var totalPrice = products.Aggregate(0m, (total, product) => total + product.Price);
            Console.WriteLine($"\nTotal price: ${totalPrice}");

            // Custom delegate for complex filtering
            Func<Product, bool> isAffordableElectronics = p =>
                p.Category == "Electronics" && p.Price < 500;

            var affordableElectronics = products.Where(isAffordableElectronics);
            Console.WriteLine("\nAffordable electronics:");
            foreach (var p in affordableElectronics)
            {
                Console.WriteLine($"  {p.Name}: ${p.Price}");
            }

            Console.WriteLine();
        }

        class Product
        {
            public string Name { get; set; } = string.Empty;
            public decimal Price { get; set; }
            public string Category { get; set; } = string.Empty;
        }

        #endregion

        #region Event Delegate Examples

        /// <summary>
        /// Demonstrates event pattern using delegates
        /// 
        /// PURPOSE:
        /// - Events provide encapsulated multicast delegate pattern
        /// - Enable publisher-subscriber communication
        /// - Standard pattern for notifications in .NET
        /// 
        /// SCENARIOS TO USE:
        /// - UI event handling
        /// - Domain event notifications
        /// - Observer pattern implementation
        /// - Loosely coupled component communication
        /// 
        /// MULTITHREAD ASPECTS:
        /// - event keyword provides thread-safe add/remove
        /// - Event invocation still needs thread safety consideration
        /// - Subscribers may run on different threads
        /// </summary>
        static void EventDelegateExamples()
        {
            Console.WriteLine("--- Event Delegate Examples ---");

            var publisher = new EventPublisher();

            // Subscribe to events using delegates
            publisher.DataChanged += OnDataChanged;
            publisher.DataChanged += OnDataChangedVerbose;

            // Subscribe using lambda
            publisher.ProgressChanged += (sender, progress) =>
            {
                Console.WriteLine($"  [Lambda] Progress: {progress}%");
            };

            // Raise events
            publisher.SimulateDataChange("Initial data");
            publisher.SimulateProgress();

            // Unsubscribe
            publisher.DataChanged -= OnDataChangedVerbose;
            Console.WriteLine("\nAfter unsubscribing verbose handler:");
            publisher.SimulateDataChange("Updated data");

            Console.WriteLine();
        }

        static void OnDataChanged(object sender, string data)
        {
            Console.WriteLine($"  [Handler1] Data changed: {data}");
        }

        static void OnDataChangedVerbose(object sender, string data)
        {
            Console.WriteLine($"  [Handler2] Received notification about data change: {data}");
        }

        class EventPublisher
        {
            // Event declarations using delegates
            // MULTITHREAD: event keyword provides thread-safe add/remove
            public event Action<object, string>? DataChanged;
            public event Action<object, int>? ProgressChanged;

            public void SimulateDataChange(string newData)
            {
                Console.WriteLine($"Publishing data change: {newData}");
                // Null-conditional operator for safe invocation
                DataChanged?.Invoke(this, newData);
            }

            public void SimulateProgress()
            {
                Console.WriteLine("Publishing progress updates:");
                for (int i = 0; i <= 100; i += 25)
                {
                    ProgressChanged?.Invoke(this, i);
                    Thread.Sleep(100);
                }
            }
        }

        #endregion

        #region Anonymous Methods and Lambdas

        /// <summary>
        /// Demonstrates anonymous methods and lambda expressions with delegates
        /// 
        /// PURPOSE:
        /// - Inline delegate implementation without separate method
        /// - Concise syntax for simple operations
        /// - Can capture outer variables (closures)
        /// 
        /// MEMORY ALLOCATION:
        /// - Lambda without captures: minimal allocation
        /// - Lambda with captures: creates closure object (~24+ bytes)
        /// - Each unique lambda creates new delegate instance
        /// - Captured variables stored in closure object
        /// 
        /// MULTITHREAD ASPECTS:
        /// - Captured variables can cause race conditions
        /// - Be careful with modified captured variables
        /// - Closures can extend variable lifetime unexpectedly
        /// </summary>
        static void AnonymousMethodsAndLambdas()
        {
            Console.WriteLine("--- Anonymous Methods and Lambdas ---");

            // Lambda expression - concise syntax
            Func<int, int> square = x => x * x;
            Console.WriteLine($"Square of 5: {square(5)}");

            // Lambda with multiple parameters
            Func<int, int, int> max = (a, b) => a > b ? a : b;
            Console.WriteLine($"Max of 10 and 15: {max(10, 15)}");

            // Lambda with statement body
            Action<string> printBoxed = message =>
            {
                Console.WriteLine("┌" + new string('─', message.Length + 2) + "┐");
                Console.WriteLine($"│ {message} │");
                Console.WriteLine("└" + new string('─', message.Length + 2) + "┘");
            };
            printBoxed("Boxed Message");

            // Closure - capturing outer variables
            // MEMORY: Creates closure object that holds captured variable
            int multiplier = 5;
            Func<int, int> multiplyByFive = x => x * multiplier;
            Console.WriteLine($"3 * {multiplier} = {multiplyByFive(3)}");

            // WARNING: Captured variable modification
            multiplier = 10; // Changes affect the lambda
            Console.WriteLine($"3 * {multiplier} = {multiplyByFive(3)} (multiplier changed!)");

            // Common closure pitfall in loops
            Console.WriteLine("\nClosure pitfall demonstration:");
            var actions = new List<Action>();

            // WRONG: All actions will print 5
            for (int i = 0; i < 5; i++)
            {
                actions.Add(() => Console.WriteLine($"Wrong: {i}")); // Captures loop variable
            }

            // Correct: Capture loop variable value
            for (int i = 0; i < 5; i++)
            {
                int localCopy = i; // Create local copy
                actions.Add(() => Console.WriteLine($"Correct: {localCopy}"));
            }

            Console.WriteLine("Invoking wrong closures:");
            actions.Take(5).ToList().ForEach(a => a());

            Console.WriteLine("Invoking correct closures:");
            actions.Skip(5).ToList().ForEach(a => a());

            Console.WriteLine();
        }

        #endregion

        #region Performance and Memory Examples

        /// <summary>
        /// Demonstrates performance and memory considerations with delegates
        /// 
        /// MEMORY ALLOCATION:
        /// - Delegate instance: ~40-80 bytes
        /// - Closure object: ~24+ bytes + captured variables
        /// - Multicast delegates: linked list overhead
        /// 
        /// PERFORMANCE CONSIDERATIONS:
        /// - Delegate invocation: ~2-3x slower than direct call
        /// - Virtual method call: similar performance to delegate
        /// - Multicast delegate: linear overhead with subscriber count
        /// - Static method delegates: slightly faster than instance methods
        /// </summary>
        static void PerformanceAndMemoryExamples()
        {
            Console.WriteLine("--- Performance and Memory Examples ---");

            const int iterations = 1_000_000;

            // Direct method call benchmark
            var sw = Stopwatch.StartNew();
            long sum = 0;
            for (int i = 0; i < iterations; i++)
            {
                sum += Add(i, 1);
            }
            sw.Stop();
            Console.WriteLine($"Direct method calls: {sw.ElapsedMilliseconds}ms (sum: {sum})");

            // Delegate invocation benchmark
            MathOperation addDelegate = Add;
            sw.Restart();
            sum = 0;
            for (int i = 0; i < iterations; i++)
            {
                sum += addDelegate(i, 1);
            }
            sw.Stop();
            Console.WriteLine($"Delegate calls: {sw.ElapsedMilliseconds}ms (sum: {sum})");

            // Lambda with no captures
            Func<int, int, int> lambdaNoCapture = (x, y) => x + y;
            sw.Restart();
            sum = 0;
            for (int i = 0; i < iterations; i++)
            {
                sum += lambdaNoCapture(i, 1);
            }
            sw.Stop();
            Console.WriteLine($"Lambda (no capture): {sw.ElapsedMilliseconds}ms (sum: {sum})");

            // Lambda with captures - creates closure
            int addValue = 1;
            Func<int, int> lambdaWithCapture = x => x + addValue;
            sw.Restart();
            sum = 0;
            for (int i = 0; i < iterations; i++)
            {
                sum += lambdaWithCapture(i);
            }
            sw.Stop();
            Console.WriteLine($"Lambda (with capture): {sw.ElapsedMilliseconds}ms (sum: {sum})");

            // Delegate caching - reuse instead of recreating
            Console.WriteLine("\nDelegate caching comparison:");
            
            // Bad: Creating new delegate each time
            sw.Restart();
            for (int i = 0; i < 10_000_000; i++)
            {
                ProcessWithDelegate(i, x => x * 2); // Creates new delegate each iteration
            }
            sw.Stop();
            var timeWithoutCaching = sw.ElapsedMilliseconds;
            Console.WriteLine($"Without caching: {timeWithoutCaching}ms");

            // Good: Reusing cached delegate
            Func<int, int> cachedDelegate = x => x * 2;
            sw.Restart();
            for (int i = 0; i < 10_000_000; i++)
            {
                ProcessWithDelegate(i, cachedDelegate); // Reuses same delegate
            }
            sw.Stop();
            var timeWithCaching = sw.ElapsedMilliseconds;
            Console.WriteLine($"With caching: {timeWithCaching}ms");
            Console.WriteLine($"Improvement: {((double)(timeWithoutCaching - timeWithCaching) / timeWithoutCaching * 100):F1}%");

            Console.WriteLine();
        }

        static void ProcessWithDelegate(int value, Func<int, int> operation)
        {
            _ = operation(value);
        }

        #endregion

        #region Thread Safety Examples

        /// <summary>
        /// Demonstrates thread safety concerns with delegates
        /// 
        /// MULTITHREAD ASPECTS:
        /// - Delegate invocation list can be modified during enumeration
        /// - Events provide thread-safe add/remove operations
        /// - Target methods need their own thread safety
        /// - Captured variables in closures can cause race conditions
        /// 
        /// SCENARIOS TO USE:
        /// - Use events instead of public delegates for thread safety
        /// - Copy delegate reference before invocation in multithreaded code
        /// - Protect shared state accessed by delegate targets
        /// </summary>
        static void ThreadSafetyExamples()
        {
            Console.WriteLine("--- Thread Safety Examples ---");

            // Demonstrating race condition with captured variable
            Console.WriteLine("Race condition with captured variable:");
            int counter = 0;
            var tasks = new List<Task>();

            Action incrementUnsafe = () =>
            {
                for (int i = 0; i < 1000; i++)
                {
                    counter++; // NOT thread-safe
                }
            };

            for (int i = 0; i < 10; i++)
            {
                tasks.Add(Task.Run(incrementUnsafe));
            }
            Task.WaitAll(tasks.ToArray());
            Console.WriteLine($"Unsafe counter (expected 10000): {counter}");

            // Thread-safe version using Interlocked
            Console.WriteLine("\nThread-safe version:");
            counter = 0;
            tasks.Clear();

            Action incrementSafe = () =>
            {
                for (int i = 0; i < 1000; i++)
                {
                    Interlocked.Increment(ref counter); // Thread-safe
                }
            };

            for (int i = 0; i < 10; i++)
            {
                tasks.Add(Task.Run(incrementSafe));
            }
            Task.WaitAll(tasks.ToArray());
            Console.WriteLine($"Safe counter: {counter}");

            // Thread-safe event invocation pattern
            Console.WriteLine("\nThread-safe event invocation:");
            var threadSafePublisher = new ThreadSafeEventPublisher();

            // Multiple threads subscribing
            var subscribeTasks = new List<Task>();
            for (int i = 0; i < 5; i++)
            {
                int threadNum = i;
                subscribeTasks.Add(Task.Run(() =>
                {
                    threadSafePublisher.DataReceived += (data) =>
                        Console.WriteLine($"  Thread {threadNum} received: {data}");
                }));
            }
            Task.WaitAll(subscribeTasks.ToArray());
            Thread.Sleep(100); // Ensure subscriptions complete

            // Raise event from multiple threads
            var publishTasks = new List<Task>();
            for (int i = 0; i < 3; i++)
            {
                int msgNum = i;
                publishTasks.Add(Task.Run(() =>
                    threadSafePublisher.RaiseEvent($"Message {msgNum}")));
            }
            Task.WaitAll(publishTasks.ToArray());

            Console.WriteLine();
        }

        class ThreadSafeEventPublisher
        {
            // Using event keyword ensures thread-safe add/remove
            public event Action<string>? DataReceived;

            public void RaiseEvent(string data)
            {
                // Create local copy to avoid race conditions during invocation
                var handler = DataReceived;
                handler?.Invoke(data);
            }
        }

        #endregion
    }
}
